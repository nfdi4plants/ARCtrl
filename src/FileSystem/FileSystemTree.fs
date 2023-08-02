namespace FileSystem

open Fable.Core

open ARCtrl

[<AttachMembers>]
type FileSystemTree =
    | File of name:string
    | Folder of name:string * children:FileSystemTree array

    member this.Name = match this with | File n | Folder (n,_) -> n
    member this.isFolder = match this with | Folder _ -> true | _ -> false
    member this.isFile = match this with | File _ -> true | _ -> false
    /// <summary>
    /// Defines the name of the root folder of the FileSystemTree when created by `fromFilePaths`.
    /// </summary>
    static member ROOT_NAME = "root"

    static member createFile name = File name

    static member createFolder(name:string, ?children:FileSystemTree array) = 
        let children = defaultArg children [||]
        Folder(name,children)

    member this.AddFile (path: string) : FileSystemTree =
        let existingPaths = this.ToFilePaths()
        let filePaths = [|
            path
            yield! existingPaths
        |]
        FileSystemTree.fromFilePaths (filePaths)

    static member addFile (path: string) =
        fun (tree: FileSystemTree) -> tree.AddFile(path)

    /// <summary>
    /// Split `paths` by `\\` or `/` and sort them into FileSystemTree. All given paths must be relative to ARC root and are sorted into a folder called `root`.
    /// </summary>
    /// <param name="paths">A array of file paths relative to ARC root.</param>
    static member fromFilePaths (paths: string array) : FileSystemTree =
        // Split path by seperators into path sequence.
        let splitPaths = paths |> Array.map Path.split |> Array.distinct
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
    member this.ToFilePaths (?removeRoot: bool) =
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
        fun (root: FileSystemTree) -> root.ToFilePaths(?removeRoot=removeRoot)

    member this.Filter (predicate: string -> bool) =
        let rec loop (parent: FileSystemTree) =
            match parent with
            | File n -> 
                if predicate n then Some (File n) else None
            | Folder (n, children) ->
                let filteredChildren = children |> Array.choose loop
                if Array.isEmpty filteredChildren then 
                    None 
                else 
                    Folder (n, filteredChildren)
                    |> Some 
        loop this

    static member filter (predicate: string -> bool) =
        fun (tree: FileSystemTree) -> tree.Filter predicate



    static member initAssayFolder(assayName : string) = 
        let dataset = FileSystemTree.createFolder("dataset", [|FileSystemTree.createFile ".gitkeep"|])
        let protocols = FileSystemTree.createFolder("protocols", [|FileSystemTree.createFile ".gitkeep"|])
        let readme = FileSystemTree.createFile "README.md"
        let assayFile = FileSystemTree.createFile "isa.assay.xlsx"
        FileSystemTree.createFolder(assayName, [|dataset; protocols; assayFile; readme|])

    static member initStudyFolder(studyName : string) = 
        let resources = FileSystemTree.createFolder("resources", [|FileSystemTree.createFile ".gitkeep"|])
        let protocols = FileSystemTree.createFolder("protocols", [|FileSystemTree.createFile ".gitkeep"|])
        let readme = FileSystemTree.createFile "README.md"
        let studyFile = FileSystemTree.createFile "isa.study.xlsx"
        FileSystemTree.createFolder(studyName, [|resources; protocols; studyFile; readme|])

    static member initInvestigationFile() = 
        FileSystemTree.createFile "isa.investigation.xlsx"


    static member createAssaysFolder(assays : FileSystemTree array) =
        FileSystemTree.createFolder("assays", Array.append [|FileSystemTree.createFile ".gitkeep"|] assays)

    static member createStudiesFolder(studies : FileSystemTree array) =
        FileSystemTree.createFolder("studies", Array.append [|FileSystemTree.createFile ".gitkeep"|] studies)

    static member createWorkflowsFolder(workflows : FileSystemTree array) =
        FileSystemTree.createFolder("assays", Array.append [|FileSystemTree.createFile ".gitkeep"|] workflows)

    static member createRunsFolder(runs : FileSystemTree array) = 
        FileSystemTree.createFolder("runs", Array.append [|FileSystemTree.createFile ".gitkeep"|] runs)

