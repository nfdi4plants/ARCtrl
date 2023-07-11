namespace FileSystem

open Fable.Core

[<AttachMembers>]
type FileSystemTree =
    | File of name:string
    | Folder of name:string * children:FileSystemTree array

    static member createFile name = File name

    static member createFolder(name:string, children:FileSystemTree array) = Folder(name,children)

    static member addFolder (path : string) (fileSystem : FileSystemTree) : FileSystemTree =
        let x = Path.split path // todo: make OS agnostic!
        raise (System.NotImplementedException())

    static member addFile (path : string) (fileSystem : FileSystemTree) : FileSystemTree =
        let x = Path.split path // todo: make OS agnostic!
        raise (System.NotImplementedException())

    /// <summary>
    /// Defines the name of the root folder of the FileSystemTree when created by `fromFilePaths`.
    /// </summary>
    static member ROOT_NAME = "root"

    /// <summary>
    /// Split `paths` by `\\` or `/` and sort them into FileSystemTree. All given paths must be relative to ARC root and are sorted into a folder called `root`.
    /// </summary>
    /// <param name="paths">A array of file paths relative to ARC root.</param>
    static member fromFilePaths (paths: string array) : FileSystemTree =
        // Split path by seperators into path sequence.
        let splitPaths = paths |> Array.map Path.split
        let root = FileSystemTree.createFolder(FileSystemTree.ROOT_NAME,[||])
        let rec loop (paths:string [] []) (parent: FileSystemTree) =
            // Files are always the last and only in path sequence.
            let files = paths |> Array.filter (fun p -> p.Length = 1) |> Array.map (Array.head >> FileSystemTree.createFile)
            // If we have more than one element in path sequence the first one must still be a folder.
            let folders = 
                paths 
                |> Array.filter (fun p -> p.Length > 1) 
                |> Array.groupBy(fun x -> Array.head x)
                |> Array.map (fun (folderName, children) -> 
                    let parent = FileSystemTree.createFolder(folderName, [||])
                    let children = children |> Array.map Array.tail
                    loop children parent 
                )
            match parent with
            | Folder (name, _) -> FileSystemTree.createFolder(name, [|yield! files; yield! folders|])
            | x -> x
        loop splitPaths root 