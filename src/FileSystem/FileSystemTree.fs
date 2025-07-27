namespace ARCtrl.FileSystem

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

    static member createRootFolder(children:FileSystemTree array) = 
        Folder(FileSystemTree.ROOT_NAME,children)
        
    /// Non-recursive lookup of child with the given name
    member this.TryGetChildByName(name : string) = 
        match this with
        | Folder (_,children) ->
            children
            |> Array.tryFind (fun c -> c.Name = name)
        | File _ -> None

    static member tryGetChildByName (name : string) =
        fun (fst : FileSystemTree) -> fst.TryGetChildByName name

    /// Non-recursive lookup of child with the given name
    member this.ContainsChildWithName(name : string) =
        match this with
        | Folder (_,children) ->
            children
            |> Array.exists (fun c -> c.Name = name)
        | File _ -> false

    /// Non-recursive lookup of child with the given name
    static member containsChildWithName (name : string) =
        fun (fst : FileSystemTree) -> fst.ContainsChildWithName name


    member this.TryGetPath(path:string) : FileSystemTree option =
        let rec loop (parent: FileSystemTree) (pathParts: string array) =
            match parent with
            | Folder ("root", children) ->
                children |> Array.tryPick (fun child -> loop child pathParts)
            | Folder (name, children) ->
                let nextPart = Array.head pathParts
                if name = nextPart then
                    if pathParts.Length = 1 then
                        Some parent
                    else
                        let remainingParts = Array.tail pathParts
                        children |> Array.tryPick (fun child -> loop child remainingParts)
                else
                    None // Current folder name does not match the next part of the path
            | File name -> 
                if pathParts.Length = 1 && pathParts.[0] = name then
                    Some parent
                else 
                    None // Cannot go deeper into a file
        let parts = ARCtrl.ArcPathHelper.split path
        loop this parts

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
        let splitPaths = paths |> Array.map ARCtrl.ArcPathHelper.split |> Array.distinct
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
                |> ARCtrl.ArcPathHelper.combineMany 
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

    member this.FilterFiles (predicate: string -> bool) =
        let rec loop (parent: FileSystemTree) =
            match parent with
            | File n -> 
                if predicate n then Some (File n) else None
            | Folder (n, children) ->
                Folder (n, children |> Array.choose loop)
                |> Some

        loop this

    static member filterFiles (predicate: string -> bool) =
        fun (tree: FileSystemTree) -> tree.FilterFiles predicate

    member this.FilterFolders (predicate: string -> bool) =
        let rec loop (parent: FileSystemTree) =
            match parent with
            | File n -> Some (File n)
            | Folder (n, children) ->
                if predicate n then 
                    Folder (n, children |> Array.choose loop)
                    |> Some
                else
                    None
        loop this

    static member filterFolders (predicate: string -> bool) =
        fun (tree: FileSystemTree) -> tree.FilterFolders predicate

    member this.Filter (predicate: string -> bool) = 
        let rec loop (parent: FileSystemTree) = 
            match parent with 
            | File n ->  
                if predicate n then Some (FileSystemTree.File n) else None 
            | Folder (n, children) -> 
                if predicate n then 
                    let filteredChildren = children |> Array.choose loop 
                    if Array.isEmpty filteredChildren then  
                        None
                    else
                        Some (FileSystemTree.Folder (n,filteredChildren))
                else
                    None
        loop this 

    static member filter (predicate: string -> bool) =
        fun (tree: FileSystemTree) -> tree.Filter predicate

    member this.Union(otherFST : FileSystemTree) =
        this.ToFilePaths() 
        |> Array.append (otherFST.ToFilePaths())
        |> Array.distinct
        |> FileSystemTree.fromFilePaths

    member this.Copy() =
        match this with
        | Folder(name,children) -> 
            Folder (name, children |> Array.map (fun c -> c.Copy()))
        | File(name) -> 
            File name




    static member createGitKeepFile() = 
        FileSystemTree.createFile ARCtrl.ArcPathHelper.GitKeepFileName

    static member createReadmeFile() = 
        FileSystemTree.createFile ARCtrl.ArcPathHelper.READMEFileName

    static member createEmptyFolder (name : string) = 
        FileSystemTree.createFolder(name, [|FileSystemTree.createGitKeepFile()|])

    static member createAssayFolder(assayName : string, ?hasDataMap) = 
        let hasDataMap = defaultArg hasDataMap false
        let dataset = FileSystemTree.createEmptyFolder ARCtrl.ArcPathHelper.AssayDatasetFolderName
        let protocols = FileSystemTree.createEmptyFolder ARCtrl.ArcPathHelper.AssayProtocolsFolderName
        let readme = FileSystemTree.createReadmeFile()
        let assayFile = FileSystemTree.createFile ARCtrl.ArcPathHelper.AssayFileName
        if hasDataMap then
            let dataMapFile = FileSystemTree.createFile ARCtrl.ArcPathHelper.DataMapFileName
            FileSystemTree.createFolder(assayName, [|dataset; protocols; assayFile; readme; dataMapFile|])
        else
            FileSystemTree.createFolder(assayName, [|dataset; protocols; assayFile; readme|])

    static member createStudyFolder(studyName : string, ?hasDataMap) = 
        let hasDataMap = defaultArg hasDataMap false
        let resources = FileSystemTree.createEmptyFolder ARCtrl.ArcPathHelper.StudiesResourcesFolderName
        let protocols = FileSystemTree.createEmptyFolder ARCtrl.ArcPathHelper.StudiesProtocolsFolderName
        let readme = FileSystemTree.createReadmeFile()
        let studyFile = FileSystemTree.createFile ARCtrl.ArcPathHelper.StudyFileName
        if hasDataMap then
            let dataMapFile = FileSystemTree.createFile ARCtrl.ArcPathHelper.DataMapFileName
            FileSystemTree.createFolder(studyName, [|resources; protocols; studyFile; readme; dataMapFile|])
        else
            FileSystemTree.createFolder(studyName, [|resources; protocols; studyFile; readme|])

    static member createWorkflowFolder(workflowName : string, ?hasDataMap) = 
        let hasDataMap = defaultArg hasDataMap false
        let readme = FileSystemTree.createReadmeFile()
        let workflowFile = FileSystemTree.createFile ARCtrl.ArcPathHelper.WorkflowFileName
        if hasDataMap then
            let dataMapFile = FileSystemTree.createFile ARCtrl.ArcPathHelper.DataMapFileName
            FileSystemTree.createFolder(workflowName, [|workflowFile; readme; dataMapFile|])
        else
            FileSystemTree.createFolder(workflowName, [|workflowFile; readme|])

    static member createRunFolder(runName : string, ?hasDataMap) = 
        let hasDataMap = defaultArg hasDataMap false
        let readme = FileSystemTree.createReadmeFile()
        let runFile = FileSystemTree.createFile ARCtrl.ArcPathHelper.RunFileName
        if hasDataMap then
            let dataMapFile = FileSystemTree.createFile ARCtrl.ArcPathHelper.DataMapFileName
            FileSystemTree.createFolder(runName, [|runFile; readme; dataMapFile|])
        else
            FileSystemTree.createFolder(runName, [|runFile; readme|])

    static member createInvestigationFile() = 
        FileSystemTree.createFile ARCtrl.ArcPathHelper.InvestigationFileName

    static member createAssaysFolder(assays : FileSystemTree array) =
        FileSystemTree.createFolder(
            ARCtrl.ArcPathHelper.AssaysFolderName, 
            Array.append [|FileSystemTree.createGitKeepFile()|] assays
        )

    static member createStudiesFolder(studies : FileSystemTree array) =
        FileSystemTree.createFolder(
            ARCtrl.ArcPathHelper.StudiesFolderName,
            Array.append [|FileSystemTree.createGitKeepFile()|] studies
        )

    static member createWorkflowsFolder(workflows : FileSystemTree array) =
        FileSystemTree.createFolder(
            ARCtrl.ArcPathHelper.WorkflowsFolderName, 
            Array.append [|FileSystemTree.createGitKeepFile()|] workflows
        )

    static member createRunsFolder(runs : FileSystemTree array) = 
        FileSystemTree.createFolder(
            ARCtrl.ArcPathHelper.RunsFolderName, 
            Array.append [|FileSystemTree.createGitKeepFile()|] runs
        )