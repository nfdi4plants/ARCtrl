#r "nuget: Fable.Core, 4.0.0"

//#I @"../src\ISA\ISA/bin\Debug\netstandard2.0"
#I @"../src\ARC/bin\Debug\netstandard2.0"
#r "ISA.dll"
#r "ARC.dll"
#r "FileSystem.dll"

open ISA
open ARC
open FileSystem

let [<Literal>] rootPath = @"C:\Users\Kevin\Desktop\TestARC"

open System

// https://github.com/nfdi4plants/arcIO.NET/blob/main/src/arcIO.NET/Assay.fs
let readFilePaths (arcPath: string) = 
    System.IO.Directory.EnumerateFiles(arcPath,"*",System.IO.SearchOption.AllDirectories)
    |> Array.ofSeq 
    |> Array.map (fun p -> System.IO.Path.GetRelativePath(rootPath, p))

let filePaths = readFilePaths (rootPath) 

let tree = FileSystemTree.fromFilePaths(filePaths)

let newPath = @"studies\TestAssay1\resources\MyAwesomeRessource.pdf"

let newPath_newFolder = @"MyNewFolder/README.md"

let newPath_minimal = @"Test.md"

let rec createFSTFromSplitPath (splitPath: string []) =
    let head, tail = Array.head splitPath, Array.tail splitPath
    match head, tail with
    | any, [||] -> File any
    | any, anyChildren -> Folder (any, [|createFSTFromSplitPath anyChildren|])

let addFile (filePath: string) (tree: FileSystemTree) = 
    let existingPaths = tree.toFilePaths()
    let filePaths = [|
        filePath
        yield! existingPaths
    |]
    FileSystemTree.fromFilePaths (filePaths)

addFile newPath_newFolder tree