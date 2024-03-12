#r "nuget: FsSpreadsheet.Net"
#r "nuget: ARCtrl"

open ARCtrl
open ARCtrl.Contract
open FsSpreadsheet
open FsSpreadsheet.Net

// # Write

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
        FsWorkbook.toXlsxFile path (wb :?> FsWorkbook)
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

// # Read

open System.IO

// from ARCtrl.NET
// https://github.com/nfdi4plants/ARCtrl.NET/blob/ba3d2fabe007d9ca2c8e07b62d02ddc5264306d0/src/ARCtrl.NET/Contract.fs#L7
let fulfillReadContract basePath (c : Contract) =
    match c.DTOType with
    | Some DTOType.ISA_Assay 
    | Some DTOType.ISA_Investigation 
    | Some DTOType.ISA_Study ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        let wb = FsWorkbook.fromXlsxFile path |> box |> DTO.Spreadsheet
        {c with DTO = Some wb}
    | Some DTOType.PlainText ->
        let path: string = System.IO.Path.Combine(basePath, c.Path)
        let text = System.IO.File.ReadAllText(path) |> DTO.Text
        {c with DTO = Some text}
    | _ -> 
        printfn "Contract %s is not an ISA contract" c.Path 
        c