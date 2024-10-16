module ARCtrl.FileSystemHelper

open FsSpreadsheet

#if !FABLE_COMPILER
open FsSpreadsheet.Net
#endif

let directoryExists path =
    System.IO.Directory.Exists path

let createDirectory path =
    System.IO.Directory.CreateDirectory path |> ignore

let ensureDirectory path = 
    if not <| directoryExists path then
        createDirectory path

let fileExists path =
    System.IO.File.Exists path

let getSubDirectories path =
    System.IO.Directory.GetDirectories path

let getSubFiles path =
    System.IO.Directory.GetFiles path

let getAllFilePaths path =
    let rec allFiles dirs =
        if Seq.isEmpty dirs then Seq.empty else
            seq { yield! dirs |> Seq.collect getSubFiles
                  yield! dirs |> Seq.collect getSubDirectories |> allFiles }
    allFiles [path]

let readFileText path : string =
    System.IO.File.ReadAllText path

let readFileBinary path : byte [] =
    System.IO.File.ReadAllBytes path

let readFileXlsx path : FsWorkbook =
    FsWorkbook.fromXlsxFile path

let writeFileText path text =
    System.IO.File.WriteAllText(path, text)

let writeFileBinary path (bytes : byte []) =
    System.IO.File.WriteAllBytes(path,bytes)

let writeFileXlsx path (wb : FsWorkbook) =
    FsWorkbook.toXlsxFile path wb