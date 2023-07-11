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

    /// <summary>
    /// Reverts FileSystemTree back to an array of filepaths relative to ARC root.
    /// </summary>
    /// <param name="removeRoot">Will remove root `Folder` if set true. *Default*: `true`.</param>
    member this.toFilePaths (?removeRoot: bool) =
        let removeRoot = defaultArg removeRoot true
        let res = ResizeArray<string>()
        let rec loop (output: string list) (parent: FileSystemTree)  =
            match parent with
            | File n -> 
                (n::output)
                |> List.rev 
                |> Array.ofList 
                |> Path.combineMany 
                |> res.Add //output full path
            | Folder (n, children) ->
                let nextOutput = n::output
                children
                |> Array.iter (fun filest -> 
                    loop nextOutput filest 
                )
        // When creating FileSystemTree with `fromFilePaths` we create an `root` `Folder`. 
        // This root folder serves as relative parent for all filepaths given.
        // The removeRoot flag will remove this extra folder.
        if removeRoot then
            match this with
            | Folder (_, children) -> 
                children |> Array.iter (loop [])
            | File n -> res.Add(n)
        else
            loop [] this
        res
        |> Array.ofSeq

    static member toFilePaths (?removeRoot: bool) =
        fun (root: FileSystemTree) -> root.toFilePaths(?removeRoot=removeRoot)