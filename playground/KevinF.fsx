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

ARC_IO.initExistingARC(rootPath)