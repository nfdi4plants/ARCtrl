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

let toFilePaths (removeRoot: bool option) =
    fun (root: FileSystemTree) ->
        let removeRoot = defaultArg removeRoot true
        let res = ResizeArray<string>()
        let rec loop (output: string list) (parent: FileSystemTree)  =
            match parent with
            | File n -> 
                (n::output)
                |> List.rev 
                |> Array.ofList 
                |> Path.combineMany 
                |> res.Add
                //output full path
            | Folder (n, children) ->
                let nextOutput = n::output
                children
                |> Array.iter (fun filest -> 
                    loop nextOutput filest 
                )
        if removeRoot then
            match root with
            | Folder (n, children) -> 
                children |> Array.iter (loop [])
            | File n -> res.Add(n)
        else
            loop [] root
        res
        |> Array.ofSeq

let revertedPaths = toFilePaths None tree |> Array.map (fun s -> s.Replace('/','\\'))

Array.sort filePaths = Array.sort revertedPaths

Array.except filePaths revertedPaths