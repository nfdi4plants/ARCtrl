#r "nuget: Fable.Core, 4.0.0"
#r "nuget: FsSpreadsheet.ExcelIO"

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
    let xlsxFiles = fs.Tree.Filter(fun p ->xlsxFileNames |> Array.contains p)
    match xlsxFiles with
    | Some xlsxPaths -> xlsxPaths.ToFilePaths() |> Array.map xlsxReadContractFromPath
    | None -> [||]

// let initARCFromContracts (cArr: Contract []) =
//     let mutable state = ArcInvestigation.createEmpty()
//     cArr
//     |> Array.iter (fun frc ->
//         match frc with
//         | {Operation = READ; DTOType = Some DTOType.Spreadsheet; DTO = Some fsworkbook; Path = p} ->
//             match p with
//             | AssayFileName -> 
//                 ISA.Spreadsheet.ArcAssay.fromFsWorkbook fsworkbook
//                 |> state.
//         | {Operation = READ; DTOType = Some DTOType.Spreadsheet; DTO = None} ->
//             printfn "Contract not fullfilled will be skipped?"
//         | _ -> failwithf "The given contract does not contain the expected information: %A" frc
//     )

getReadContracts filePaths
|> ARC_IO.fullfillREADContracts rootPath
|> initARCFromContracts