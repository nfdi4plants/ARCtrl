#r "nuget: Fable.Core, 4.0.0"
#r "nuget: FsSpreadsheet, 2.0.2"
#r "nuget: FsSpreadsheet.ExcelIO, 2.0.2"

//#I @"../src\ISA\ISA/bin\Debug\netstandard2.0"
#I @"../src\ARC/bin\Debug\netstandard2.0"
#r "ISA.dll"
#r "ARC.dll"
#r "Contract.dll"
#r "FileSystem.dll"
#r "ISA.Spreadsheet.dll"

open ISA
open ARC
open ARC.Path
open FileSystem
open Contract

open FsSpreadsheet
open FsSpreadsheet.ExcelIO

let [<Literal>] rootPath = @"C:\Users\Kevin\Desktop\TestARC"

open System

// https://github.com/nfdi4plants/arcIO.NET/blob/main/src/arcIO.NET/Assay.fs
let readFilePaths (arcPath: string) = 
    System.IO.Directory.EnumerateFiles(arcPath,"*",System.IO.SearchOption.AllDirectories)
    |> Array.ofSeq 
    |> Array.map (fun p -> System.IO.Path.GetRelativePath(rootPath, p))

let filePaths = readFilePaths (rootPath)

module ARC_IO =

    open FsSpreadsheet
    open FsSpreadsheet.ExcelIO
    let fullfillREADContract (arcRoot: string) (c: Contract) =
        let p = System.IO.Path.Combine [|arcRoot; c.Path|]
        match c with
        | {Operation = READ} ->
            let dto = DTO.Spreadsheet <| FsWorkbook.fromXlsxFile(p)
            {c with DTO = Some dto}
        | _ -> failwith "Tried reading from non-READ contract."

    let fullfillREADContracts (arcRoot: string) (cArr: Contract []) =
        cArr |> Array.map (fullfillREADContract arcRoot)

let getReadContracts (filePaths) =
    let xlsxReadContractFromPath (path: string) = Contract.createRead(path, DTOType.Spreadsheet)
    let fs = FileSystem.fromFilePaths filePaths
    let xlsxFileNames = [|AssayFileName; StudyFileName; InvestigationFileName|]
    let xlsxFiles = fs.Tree.Filter(fun p -> xlsxFileNames |> Array.contains p)
    match xlsxFiles with
    | Some xlsxPaths -> xlsxPaths.ToFilePaths() |> Array.map xlsxReadContractFromPath
    | None -> [||]

let initARCFromContracts (cArr: Contract []) =
    let isFile (fileName: string) (path: string) = (FileSystem.Path.getFileName path) = fileName
    let filteredContracts = cArr |> Array.choose (fun c ->
        match c with
        | {Operation = READ; DTOType = Some DTOType.Spreadsheet; DTO = Some (DTO.Spreadsheet fsworkbook); Path = p} ->
            Some (p, fsworkbook)
        | _ -> None
    )
    let investigation = 
        filteredContracts 
        |> Array.find (fun c -> fst c |> isFile InvestigationFileName)
        |> snd 
        |> ISA.Spreadsheet.ArcInvestigation.fromFsWorkbook
    let studies =
        filteredContracts 
        |> Array.filter (fun c -> fst c |> isFile StudyFileName)
        |> Array.map (fun c ->
            snd c
            |> ISA.Spreadsheet.ArcStudy.fromFsWorkbook
        )
    let assays =
        filteredContracts 
        |> Array.filter (fun c -> fst c |> isFile AssayFileName)
        |> Array.map (fun c ->
            snd c
            |> ISA.Spreadsheet.ArcAssay.fromFsWorkbook
        )
    let copy = investigation.Copy()
    let registeredStudies = copy.StudyIdentifiers
    registeredStudies |> Seq.iter (fun studyRegisteredIdent ->
        let studyOpt = studies |> Array.tryFind (fun s -> s.Identifier = studyRegisteredIdent)
        match studyOpt with
        | Some study -> // This study element is parsed from FsWorkbook and has no regsitered assays, yet
            printfn "Found study: %s" studyRegisteredIdent
            let registeredAssays = copy.GetStudy(studyRegisteredIdent).AssayIdentifiers
            registeredAssays |> Seq.iter (fun assayRegisteredIdent ->
                let assayOpt = assays |> Array.tryFind (fun a -> a.Identifier = assayRegisteredIdent)
                match assayOpt with
                | Some assay -> 
                    printfn "Found assay: %s - %s" studyRegisteredIdent assayRegisteredIdent 
                    study.AddAssay(assay)
                | None -> printfn "Unable to find registered assay '%s' in fullfilled READ contracts!" assayRegisteredIdent
            )
            investigation.SetStudy(studyRegisteredIdent, study)
        | None -> printfn "Unable to find registered study '%s' in fullfilled READ contracts!" studyRegisteredIdent
    )
    investigation

let i = 
    getReadContracts filePaths
    |> ARC_IO.fullfillREADContracts rootPath
    |> initARCFromContracts
