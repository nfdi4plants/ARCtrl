#r "nuget: Fable.Core, 4.0.0"
#r "nuget: FsSpreadsheet, 2.0.2"
#r "nuget: FsSpreadsheet.ExcelIO, 2.0.2"
#I @"../src\ARCtrl/bin\Debug\netstandard2.0"
#r "ISA.dll"
#r "Contract.dll"
#r "FileSystem.dll"
#r "ISA.Spreadsheet.dll"
#r "CWL.dll"
#r "ARCtrl.dll"

//#i @"nuget: C:/Users/Kevin/source/repos/ISADotNet/pkg/"
//#r "nuget: ARC"

let [<Literal>] rootPath = @"C:\Users\Kevin\Desktop\TestARC"
open Contract
open ARCtrl
open FileSystem

module ARC_IO =
    open FsSpreadsheet
    open FsSpreadsheet.ExcelIO
    let readFilePaths (arcPath: string) = 
        System.IO.Directory.EnumerateFiles(arcPath,"*",System.IO.SearchOption.AllDirectories)
        |> Array.ofSeq 
        |> Array.map (fun p -> System.IO.Path.GetRelativePath(rootPath, p))

    let fullfillREADContract (arcRoot: string) (c: Contract) =
        let p = System.IO.Path.Combine [|arcRoot; c.Path|]
        match c with
        | {Operation = READ} ->
            let dto = DTO.Spreadsheet <| FsWorkbook.fromXlsxFile(p)
            {c with DTO = Some dto}
        | _ -> failwith "Tried reading from non-READ contract."

    let fullfillREADContracts (arcRoot: string) (cArr: Contract []) =
        cArr |> Array.map (fullfillREADContract arcRoot)

    let initExistingARC (arcRootPath: string) =
        readFilePaths arcRootPath
        |> ARC.getReadContracts
        |> fullfillREADContracts arcRootPath
        //|> ARC.createFromReadContracts

let expectedStudyPathFromStudyIdentifier (studyIdentifier) =
    FileSystem.Path.combineMany[|Path.ARCtrl.StudiesFolderName; studyIdentifier; Path.ARCtrl.StudyFileName|]


let tryFindStudy (contracts: Contract []) (studyRegisteredIdent: string) =
    contracts |> Array.tryPick (fun c ->
        let expectedPath = expectedStudyPathFromStudyIdentifier studyRegisteredIdent
        match c with
        | {Operation = READ; DTOType = Some DTOType.ISA_Study; DTO = Some (DTO.Spreadsheet fsworkbook); Path = p} when p = expectedPath ->
            Some (p, fsworkbook)
        | _ -> None
    )

let tryFindAssay (contracts: Contract []) (assayRegisteredIdent: string) =
    contracts |> Array.tryPick (fun c ->
        let expectedPath = expectedPathFromStudyIdentifier assayRegisteredIdent
        match c with
        | {Operation = READ; DTOType = Some DTOType.ISA_Assay; DTO = Some (DTO.Spreadsheet fsworkbook); Path = p} when p = expectedPath ->
            Some (p, fsworkbook)
        | _ -> None
    )

/// <summary>
/// This function creates the ARC-model from fullfilled READ contracts. The necessary READ contracts can be created with `ARC.getReadContracts`.
/// </summary>
/// <param name="cArr">The fullfilled READ contracts.</param>
/// <param name="enableLogging">If this flag is set true, the function will print any missing/found assays/studies to the console. *Default* = false</param>
let ISAFromContracts (contracts: Contract []) =
    /// filter to only keep *fullfilled* READ contracts of type spreadsheet.
    let investigation = 
        let i = 
            contracts |> Array.choose (fun c ->
                match c with
                | {Operation = READ; DTOType = Some DTOType.ISA_Investigation; DTO = Some (DTO.Spreadsheet fsworkbook); Path = p} ->
                    Some (p, fsworkbook)
                | _ -> None
            )
        Array.exactlyOne i 
        |> snd
        |> ISA.Spreadsheet.ArcInvestigation.fromFsWorkbook
    // /// get studies from xlsx
    // let studies =
    //     filteredContracts 
    //     |> Array.filter (fun c -> fst c |> FileSystem.Path.isFile Path.StudyFileName)
    //     |> Array.map (snd >> ISA.Spreadsheet.ArcStudy.fromFsWorkbook)
    // /// get assays from xlsx
    // let assays =
    //     filteredContracts 
    //     |> Array.filter (fun c -> fst c |> FileSystem.Path.isFile Path.AssayFileName)
    //     |> Array.map (snd >> ISA.Spreadsheet.ArcAssay.fromFsWorkbook)
    // /// Create a investigation copy to check for registered studies/assays
    // let copy = investigation.Copy()
    // /// Get all registered studies
    // let registeredStudies = copy.StudyIdentifiers
    investigation.StudyIdentifiers |> Seq.iter (fun studyRegisteredIdent ->
        /// Try find registered study in parsed READ contracts
        let studyOpt = tryFindStudy contracts studyRegisteredIdent 
        match studyOpt with
        | Some study -> // This study element is parsed from FsWorkbook and has no regsitered assays, yet
            printfn "Found study: %s" studyRegisteredIdent
            let registeredAssays = copy.GetStudy(studyRegisteredIdent).AssayIdentifiers
            registeredAssays |> Seq.iter (fun assayRegisteredIdent ->
                /// Try find registered assay in parsed READ contracts
                let assayOpt = assays |> Array.tryFind (fun a -> a.Identifier = assayRegisteredIdent)
                match assayOpt with
                | Some assay -> 
                    if enableLogging then printfn "Found assay: %s - %s" studyRegisteredIdent assayRegisteredIdent 
                    study.AddAssay(assay)
                | None -> 
                    if enableLogging then printfn "Unable to find registered assay '%s' in fullfilled READ contracts!" assayRegisteredIdent
            )
            investigation.SetStudy(studyRegisteredIdent, study)
        | None -> 
             printfn "Unable to find registered study '%s' in fullfilled READ contracts!" studyRegisteredIdent
    )
    // investigation

ARC_IO.initExistingARC(rootPath)