module ARCtrl.FileSystemHelper

open FsSpreadsheet
open CrossAsync
open Fable.Core
open Fable

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
open Fable.Core.JsInterop
open FsSpreadsheet.Js
#endif
#if FABLE_COMPILER_PYTHON
open FsSpreadsheet.Py
open Fable.Core.PyInterop

importAll "shutil"
importAll "os"
import "Path" "pathlib"

#endif

#if !FABLE_COMPILER
open FsSpreadsheet.Net
#endif

let directoryExistsAsync (path : string) : CrossAsync<bool> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "directoryExists" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (path : string) : bool = emitPyExpr (path) "Path(path).is_dir()"
        crossAsync {
            return f path
        }      
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        return System.IO.Directory.Exists path
    }
    #endif

let createDirectoryAsync (path : string) : CrossAsync<unit> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "createDirectory" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (path : string) : unit = emitPyExpr (path) "Path(path).mkdir(parents=True, exist_ok=True)"
        crossAsync {
            f path
        }
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.Directory.CreateDirectory path |> ignore
    }
    #endif

let ensureDirectoryAsync (path : string) : CrossAsync<unit> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "ensureDirectory" "${outDir}/FileSystem.js"
    #else
    crossAsync {
        let! exists = directoryExistsAsync path
        if not exists then
            return! createDirectoryAsync path
    }
    #endif

let ensureDirectoryOfFileAsync (filePath : string) : CrossAsync<unit> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "ensureDirectoryOfFile" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (path : string) : string = emitPyExpr (path) "Path(file_path).parent"
        crossAsync {
            return! ensureDirectoryAsync(f filePath) 
        }
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        let file = new System.IO.FileInfo(filePath);
        file.Directory.Create()
    }
    #endif

let fileExistsAsync (path : string) : CrossAsync<bool> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "fileExists" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (path : string) : bool = emitPyExpr (path) "Path(path).is_file()"
        crossAsync {
            return f path
        }
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        return System.IO.File.Exists path
    }
    #endif



let readFileTextAsync (path : string) : CrossAsync<string> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "readFileText" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (path : string) : string = emitPyStatement (path) "with open(path, 'r', encoding='utf-8') as f: return f.read()"
        crossAsync {
            return f path
        }
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        return System.IO.File.ReadAllText path
    }
    #endif

let readFileBinaryAsync (path : string) : CrossAsync<byte []> =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "readFileBinary" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (path : string) : byte [] = emitPyStatement (path) "with open(path, 'rb') as f: return f.read()"
        crossAsync {
            return f path
        }
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
        import "moveFile" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (oldPath : string) (newPath : string) : unit =
            // import "moveFile" "${outDir}/src/ARCtrl/ContractIO/file_system.py"
            emitPyStatement (oldPath,newPath) "shutil.move($0, $1)"
        crossAsync {
            f oldPath newPath
        }
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.File.Move(oldPath, newPath)
    }
    #endif

let moveDirectoryAsync oldPath newPath =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "moveDirectory" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        moveFileAsync oldPath newPath
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.Directory.Move(oldPath, newPath)
    }
    #endif

let deleteFileAsync path =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "deleteFile" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (path : string) : unit = emitPyStatement (path) """try:
            os.remove(path)
        except FileNotFoundError:
            pass"""
        crossAsync {
            f path
        }
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.File.Delete path
    }
    #endif

let deleteDirectoryAsync path =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "deleteDirectory" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (path : string) : unit = emitPyStatement (path) "shutil.rmtree(path, ignore_errors=True)"
        crossAsync {
            f path
        }
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.Directory.Delete(path, true)
    }
    #endif


let writeFileTextAsync path text =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "writeFileText" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (path :string) (text : string) : unit =
            emitPyStatement (path,text) "with open($0, 'w') as f: f.write($1)"
        crossAsync {
            f path text
        }
    #endif
    #if !FABLE_COMPILER
    crossAsync {
        System.IO.File.WriteAllText(path, text)
    }
    #endif

let writeFileBinaryAsync path (bytes : byte []) =
    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        import "writeFileBinary" "${outDir}/FileSystem.js"
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (path :string) (bytes : byte []) : unit =
            // let f : (string * byte []) -> unit = import "writeFileBinary" "${outDir}/src/ARCtrl/ContractIO/file_system.py"
            emitPyStatement (path,bytes) "with open($0, 'wb') as f: f.write($1)"
        crossAsync {
            f path bytes
        }
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
        let f : string -> CrossAsync<string []> = import "getSubDirectories" "${outDir}/FileSystem.js"
        crossAsync {
            let! paths = f path
            return paths |> Array.map standardizeSlashes
        }
    #endif
    #if FABLE_COMPILER_PYTHON

        let f (path : string) : string [] = emitPyExpr (path) "[str(entry) for entry in Path(path).iterdir() if entry.is_dir()]"
        crossAsync {
            let paths = f path
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
        let f : string -> CrossAsync<string []> = import "getSubFiles" "${outDir}/FileSystem.js"
        crossAsync {
            let! paths = f path
            return paths |> Array.map standardizeSlashes
        }
    #endif
    #if FABLE_COMPILER_PYTHON
        let f (path : string) : string [] = emitPyExpr (path) "[str(entry) for entry in Path(path).iterdir() if entry.is_file()]"
        crossAsync {
            let paths = f path
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