module ARCtrl.FileSystemHelper

open FsSpreadsheet

open CrossAsync
open Fable.Core
open Fable

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
open Fable.Core.JsInterop
open FsSpreadsheet.Js
#endif
#if !FABLE_COMPILER
open FsSpreadsheet.Net
#endif

let directoryExistsAsync (path : string) : CrossAsync<bool> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "directoryExists" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        return System.IO.Directory.Exists path
    }
    #endif

let createDirectoryAsync (path : string) : CrossAsync<unit> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "createDirectory" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.Directory.CreateDirectory path |> ignore
    }
    #endif

let ensureDirectoryAsync (path : string) : CrossAsync<unit> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "ensureDirectory" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        let! exists = directoryExistsAsync path
        if not <| exists then
            return! createDirectoryAsync path
    }
    #endif

let ensureDirectoryOfFileAsync (filePath : string) : CrossAsync<unit> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "ensureDirectoryOfFile" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        let file = new System.IO.FileInfo(filePath);
        file.Directory.Create()
    }
    #endif

let fileExistsAsync (path : string) : CrossAsync<bool> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "fileExists" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        return System.IO.File.Exists path
    }
    #endif

let getSubDirectoriesAsync (path : string) : CrossAsync<string []> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "getSubDirectories" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        return System.IO.Directory.GetDirectories path
    }
    #endif

let getSubFilesAsync (path : string) : CrossAsync<string []> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "getSubFiles" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        return System.IO.Directory.GetFiles path
    }
    #endif

/// Return the absolute path relative to the directoryPath
let makeRelative directoryPath (path : string) : string =
    if directoryPath = "." || directoryPath = "/" || directoryPath = "" then
        path
    else
        if path.StartsWith(directoryPath) then 
            path.Substring(directoryPath.Length)
        else path
    
    

let standardizeSlashes (path : string) : string =
    path.Replace("\\","/")              

let getAllFilePathsAsync (directoryPath : string) : CrossAsync<string []> =
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

let readFileTextAsync (path : string) : CrossAsync<string> =
    crossAsync {
        return System.IO.File.ReadAllText path
    }

let readFileBinaryAsync (path : string) : CrossAsync<byte []> =
    crossAsync {
        return System.IO.File.ReadAllBytes path
    }

let readFileXlsxAsync (path : string) : CrossAsync<FsWorkbook> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    FsWorkbook.fromXlsxFile path
    #else
    crossAsync {
        return FsWorkbook.fromXlsxFile path
    }
    #endif

let renameFileOrDirectoryAsync oldPath newPath : CrossAsync<unit> =
    crossAsync {
        let! fileExists = fileExistsAsync oldPath
        let! directoryExists = directoryExistsAsync oldPath
        if fileExists then
            System.IO.File.Move(oldPath, newPath)
        elif directoryExists then
            System.IO.Directory.Move(oldPath, newPath)
        else ()
    }

let writeFileTextAsync (path : string) text : CrossAsync<unit> =
    crossAsync {
        System.IO.File.WriteAllText(path, text)
    } 

let writeFileBinaryAsync (path : string) (bytes : byte []) : CrossAsync<unit> =
    crossAsync {
        System.IO.File.WriteAllBytes(path, bytes)
    }

let writeFileXlsxAsync (path : string) (wb : FsWorkbook) : CrossAsync<unit> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    FsWorkbook.toXlsxFile path wb
    #else
    crossAsync {
        return FsWorkbook.toXlsxFile path wb
    }
    #endif
    

let deleteFileOrDirectoryAsync (path : string) : CrossAsync<unit> =
    crossAsync {
        let! fileExists = fileExistsAsync path
        let! directoryExists = directoryExistsAsync path
        if fileExists then
            System.IO.File.Delete path
        elif directoryExists then
            System.IO.Directory.Delete(path, true)
        else ()
    }