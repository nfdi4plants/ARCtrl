module ARCtrl.FileSystemHelper

open FsSpreadsheet
open CrossAsync
open FsSpreadsheet.Net

let directoryExistsAsync path =
    crossAsync {
        return System.IO.Directory.Exists path
    }

let createDirectoryAsync path =
    crossAsync {
        System.IO.Directory.CreateDirectory path |> ignore
    }

let ensureDirectoryAsync path =
    crossAsync {
        let! exists = directoryExistsAsync path
        if not <| exists then
            return! createDirectoryAsync path
    }

let ensureDirectoryOfFileAsync (filePath : string) =
    crossAsync {
        let file = new System.IO.FileInfo(filePath);
        file.Directory.Create()
    }

let fileExistsAsync path =
    crossAsync {
        return System.IO.File.Exists path
    }

let getSubDirectoriesAsync path =
    crossAsync {
        return System.IO.Directory.GetDirectories path
    }

let getSubFilesAsync path =
    crossAsync {
        return System.IO.Directory.GetFiles path
    }

/// Return the absolute path relative to the directoryPath
let makeRelative directoryPath (path : string) =
    if directoryPath = "." || directoryPath = "/" || directoryPath = "" then
        path
    else
        if path.StartsWith(directoryPath) then 
            path.Substring(directoryPath.Length)
        else path
    
    

let standardizeSlashes (path : string) = 
    path.Replace("\\","/")              

let getAllFilePathsAsync (directoryPath : string) =
    crossAsync {
        let rec allFiles (dirs : string seq) : CrossAsync<string seq> =
            crossAsync {
            if Seq.isEmpty dirs then
                return Seq.empty
            else
                
                let! subFiles = dirs |> Seq.map getSubFilesAsync |> CrossAsync.sequential
                let subFiles = subFiles |> Seq.concat
                let! subDirs = dirs |> Seq.map getSubDirectoriesAsync |> CrossAsync.sequential 
                let! subDirContents = subDirs |> Seq.map allFiles |> CrossAsync.sequential
                let subDirContents = subDirContents |> Seq.concat
                return subFiles |> Seq.append subDirContents
            }
        let! allFiles = allFiles [directoryPath]
        let allFilesRelative = 
            allFiles
            |> Seq.toArray
            |> Array.map (makeRelative directoryPath >> standardizeSlashes)
        return allFilesRelative
    }

let readFileTextAsync path : Async<string> =
    crossAsync {
        return System.IO.File.ReadAllText path
    }

let readFileBinaryAsync path : Async<byte []> =
    crossAsync {
        return System.IO.File.ReadAllBytes path
    }

let readFileXlsxAsync path : Async<FsWorkbook> =
    crossAsync {
        return FsWorkbook.fromXlsxFile path
    }

let renameFileOrDirectoryAsync oldPath newPath =
    crossAsync {
        let! fileExists = fileExistsAsync oldPath
        let! directoryExists = directoryExistsAsync oldPath
        if fileExists then
            System.IO.File.Move(oldPath, newPath)
        elif directoryExists then
            System.IO.Directory.Move(oldPath, newPath)
        else ()
    }

let writeFileTextAsync path text =
    crossAsync {
        System.IO.File.WriteAllText(path, text)
    } 

let writeFileBinaryAsync path (bytes : byte []) =
    crossAsync {
        System.IO.File.WriteAllBytes(path, bytes)
    }

let writeFileXlsxAsync path (wb : FsWorkbook) =
    crossAsync {
        FsWorkbook.toXlsxFile path wb
    }

let deleteFileOrDirectoryAsync path =
    crossAsync {
        let! fileExists = fileExistsAsync path
        let! directoryExists = directoryExistsAsync path
        if fileExists then
            System.IO.File.Delete path
        elif directoryExists then
            System.IO.Directory.Delete(path, true)
        else ()
    }