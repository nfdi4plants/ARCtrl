# ARCtrl

## Getting started

The main objective of the ARCtrl is to manage communication between in-memory ARC representation and on-disc representation. This is done using `Contract`s.

To create the `ARC` model from an existing ARC on your disc you can do the following:

```fsharp
// reference ARC from anywhere, in this case from local built nuget packages
#i @"nuget: C:/Users/Kevin/source/repos/ISADotNet/pkg/"
#r "nuget: ARC"
// Example is done in .NET, so we use FsSpreadsheet.ExcelIO to read in the xlsx files.
#r "nuget: FsSpreadsheet, 2.0.2"
#r "nuget: FsSpreadsheet.ExcelIO, 2.0.2"
open ARC
open Contract

/// Define the path to ARC root you want to read
let [<Literal>] rootPath = @"C:\Users\Kevin\Desktop\TestARC"

/// Functions found here might be moved to a ARCtrl.NETIO Implementation
module ARC_IO =
    open FsSpreadsheet
    open FsSpreadsheet.ExcelIO

    /// Read all files in ARC and return their relative paths to `rootPath`
    let readFilePaths (arcRootPath: string) = 
        System.IO.Directory.EnumerateFiles(arcPath,"*",System.IO.SearchOption.AllDirectories)
        |> Array.ofSeq 
        |> Array.map (fun p -> System.IO.Path.GetRelativePath(arcRootPath, p))

    /// Fullfill READ contract for xlsx files.
    /// Paths are always relative to ARC root
    let fullfillREADContract (arcRootPath: string) (c: Contract) =
        let p = System.IO.Path.Combine [|arcRootPath; c.Path|]
        match c with
        | {Operation = READ} ->
            let dto = DTO.Spreadsheet <| FsWorkbook.fromXlsxFile(p)
            {c with DTO = Some dto}
        | _ -> failwith "Tried reading from non-READ contract."

    let fullfillREADContracts (arcRootPath: string) (cArr: Contract []) =
        cArr |> Array.map (fullfillREADContract arcRootPath)

    /// Create and fullfill READ contracts for ARCtrl in .NET, then init ARC from existing ARC on disc.
    let readARC (arcRootPath: string) =
        readFilePaths arcRootPath               // Get all file paths relative to ARC root
        |> ARC.createReadContracts              // Get READ contracts from ARCtrl
        |> fullfillREADContracts arcRootPath    // Fullfill READ contracts in .NET
        |> ARC.createFromReadContracts          // Create ARC model from fullfilled READ contracts

// ArcInvestigation with all studies and assays
ARC_IO.readARC(rootPath)

```