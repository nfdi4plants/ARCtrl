module ARCtrl.FileSystemHelper

open FsSpreadsheet

open FsSpreadsheet.Net

let directoryExists path =
    System.IO.Directory.Exists path

let createDirectory path =
    System.IO.Directory.CreateDirectory path |> ignore

let ensureDirectory path = 
    if not <| directoryExists path then
        createDirectory path

let ensureDirectoryOfFile (filePath : string) =
    let file = new System.IO.FileInfo(filePath);
    file.Directory.Create()

let fileExists path =
    System.IO.File.Exists path

let getSubDirectories path =
    System.IO.Directory.GetDirectories path

let getSubFiles path =
    System.IO.Directory.GetFiles path

/// Return the absolute path relative to the directoryPath
let makeRelative directoryPath (path : string) = 
    if directoryPath = "." || directoryPath = "/" || directoryPath = "" then path
    else
        if path.StartsWith(directoryPath) then 
            path.Substring(directoryPath.Length)
        else path

let standardizeSlashes (path : string) = 
    path.Replace("\\","/")              

let getAllFilePaths (directoryPath : string) =
    let rec allFiles dirs =
        if Seq.isEmpty dirs then Seq.empty else
            seq { yield! dirs |> Seq.collect getSubFiles
                  yield! dirs |> Seq.collect getSubDirectories |> allFiles }
    
    allFiles [directoryPath] |> Seq.toArray
    |> Array.map (makeRelative directoryPath >> standardizeSlashes)

let readFileText path : string =
    System.IO.File.ReadAllText path

let readFileBinary path : byte [] =
    System.IO.File.ReadAllBytes path

let readFileXlsx path : FsWorkbook =
    FsWorkbook.fromXlsxFile path

let renameFileOrDirectory oldPath newPath =
    if fileExists oldPath then
        System.IO.File.Move(oldPath, newPath)
    elif directoryExists oldPath then
        System.IO.Directory.Move(oldPath, newPath)
    else ()

let writeFileText path text =
    System.IO.File.WriteAllText(path, text)

let writeFileBinary path (bytes : byte []) =
    System.IO.File.WriteAllBytes(path,bytes)

let writeFileXlsx path (wb : FsWorkbook) =
    FsWorkbook.toXlsxFile path wb

let deleteFileOrDirectory path =
    if fileExists path then
        System.IO.File.Delete path
    elif directoryExists path then
        System.IO.Directory.Delete(path, true)
    else ()