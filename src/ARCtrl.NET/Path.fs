module ARCtrl.NET.Path

open System.IO

let ensureDirectory (filePath : string) =
    let file = new System.IO.FileInfo(filePath);
    file.Directory.Create()

let normalizePathSeparators (str:string) = str.Replace("\\","/")            

let getAllFilePaths (basePath: string) =
    let options = EnumerationOptions()
    options.RecurseSubdirectories <- true
    Directory.EnumerateFiles(basePath, "*", options)
    |> Seq.map (fun fp ->
        Path.GetRelativePath(basePath, fp)
        |> normalizePathSeparators
    )
    |> Array.ofSeq
