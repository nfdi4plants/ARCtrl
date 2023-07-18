#r "nuget: Fable.Core, 4.0.0"
#r "nuget: FsSpreadsheet, 2.0.2"
#r "nuget: FsSpreadsheet.ExcelIO, 2.0.2"

// #I @"../src\ARC/bin\Debug\netstandard2.0"
// #r "ARC.dll"
// #r "ISA.dll"
// #r "Contract.dll"
// #r "FileSystem.dll"
// #r "ISA.Spreadsheet.dll"

#i @"nuget: C:/Users/Kevin/source/repos/ISADotNet/pkg/"
#r "nuget: ARC"

open ARC
open Contract
let [<Literal>] rootPath = @"C:\Users\Kevin\Desktop\TestARC"

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
        |> ARC.createReadContracts
        |> fullfillREADContracts arcRootPath
        |> ARC.createFromReadContracts

ARC_IO.initExistingARC(rootPath)