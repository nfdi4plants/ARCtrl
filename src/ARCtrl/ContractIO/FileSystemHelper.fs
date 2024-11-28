module ARCtrl.FileSystemHelper

open FsSpreadsheet
open CrossAsync

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



let readFileTextAsync (path : string) : CrossAsync<string> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "readFileText" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        return System.IO.File.ReadAllText path
    }
    #endif

let readFileBinaryAsync (path : string) : CrossAsync<byte []> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "readFileBinary" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        return System.IO.File.ReadAllBytes path
    }
    #endif

let readFileXlsxAsync (path : string) : CrossAsync<FsWorkbook> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    FsWorkbook.fromXlsxFile path
    #else
    crossAsync {
        return FsWorkbook.fromXlsxFile path
    }
    #endif


let moveFileAsync oldPath newPath =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "moveFile" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.File.Move(oldPath, newPath)
    }
    #endif

let moveDirectoryAsync oldPath newPath =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "moveDirectory" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.Directory.Move(oldPath, newPath)
    }
    #endif

let deleteFileAsync path =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "deleteFile" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.File.Delete path
    }
    #endif

let deleteDirectoryAsync path =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "deleteDirectory" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.Directory.Delete(path, true)
    }
    #endif


let writeFileTextAsync path text =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "writeFileText" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.File.WriteAllText(path, text)
    }
    #endif

let writeFileBinaryAsync path (bytes : byte []) =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "writeFileBinary" "./FileSystem.js"
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.File.WriteAllBytes(path, bytes)
    }
    #endif

let writeFileXlsxAsync path (wb : FsWorkbook) =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        FsWorkbook.toXlsxFile path wb
    #else
    crossAsync {
        FsWorkbook.toXlsxFile path wb
    }
    #endif


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

let getSubDirectoriesAsync (path : string) : CrossAsync<string []> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        let f : string -> CrossAsync<string []> = import "getSubDirectories" "./FileSystem.js"
        crossAsync {
            let! paths = f path
            return paths |> Array.map standardizeSlashes
        }
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        let paths = System.IO.Directory.GetDirectories path
        return paths |> Array.map (standardizeSlashes)
    }
    #endif

let getSubFilesAsync (path : string) : CrossAsync<string []> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        let f : string -> CrossAsync<string []> = import "getSubFiles" "./FileSystem.js"
        crossAsync {
            let! paths = f path
            return paths |> Array.map standardizeSlashes
        }
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        let paths = System.IO.Directory.GetFiles path
        return paths |> Array.map (standardizeSlashes)
        //return System.IO.Directory.GetFiles path
    }
    #endif


let getAllFilePathsAsync (directoryPath : string) =
    let directoryPath = standardizeSlashes directoryPath
    crossAsync {
        let rec allFiles (dirs : string seq) : CrossAsync<string seq> =
            crossAsync {
            if Seq.isEmpty dirs then
                return Seq.empty
            else
                
                let! subFiles = dirs |> Seq.map getSubFilesAsync |> CrossAsync.all
                let subFiles = subFiles |> Seq.concat
                let! subDirs = dirs |> Seq.map getSubDirectoriesAsync |> CrossAsync.all 
                let! subDirContents = subDirs |> Seq.map allFiles |> CrossAsync.all
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


let renameFileOrDirectoryAsync oldPath newPath =
    crossAsync {
        let! fileExists = fileExistsAsync oldPath
        let! directoryExists = directoryExistsAsync oldPath
        if fileExists then
            return! moveFileAsync oldPath newPath
        elif directoryExists then
            return! moveDirectoryAsync oldPath newPath
        else ()
    }


let deleteFileOrDirectoryAsync path =
    crossAsync {
        let! fileExists = fileExistsAsync path
        let! directoryExists = directoryExistsAsync path
        if fileExists then
            return! deleteFileAsync path
        elif directoryExists then
            return! deleteDirectoryAsync path
        else ()
    }