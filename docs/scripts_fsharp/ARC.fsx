#r "nuget: FsSpreadsheet.ExcelIO, 4.1.0"
#r "nuget: ARCtrl, 1.0.0-alpha9"

// # Create

open ARCtrl

/// Init a new empty ARC
let arc = ARC()

arc.FileSystem

// # Write

open ARCtrl.Contract
open FsSpreadsheet
open FsSpreadsheet.ExcelIO

let arcRootPath = @"C:\Users\Kevin\Desktop\NewTestARC"

/// From ARCtrl.NET
/// https://github.com/nfdi4plants/ARCtrl.NET/blob/f3eda8e96a3a7791288c1b5975050742c1d803d9/src/ARCtrl.NET/Contract.fs#L24
let fulfillWriteContract basePath (c : Contract) =
    let ensureDirectory (filePath : string) =
        let file = new System.IO.FileInfo(filePath);
        file.Directory.Create()
    match c.DTO with
    | Some (DTO.Spreadsheet wb) ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        ensureDirectory path
        FsWorkbook.toFile path (wb :?> FsWorkbook)
    | Some (DTO.Text t) ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        ensureDirectory path
        System.IO.File.WriteAllText(path,t)
    | None -> 
        let path = System.IO.Path.Combine(basePath, c.Path)
        ensureDirectory path
        System.IO.File.Create(path).Close()
    | _ -> 
        printfn "Contract %s is not an ISA contract" c.Path

/// From ARCtrl.NET
/// https://github.com/nfdi4plants/ARCtrl.NET/blob/f3eda8e96a3a7791288c1b5975050742c1d803d9/src/ARCtrl.NET/Arc.fs#L11
let write (arcPath: string) (arc:ARC) =
    arc.GetWriteContracts()
    |> Array.iter (fulfillWriteContract arcPath)

write arcRootPath arc