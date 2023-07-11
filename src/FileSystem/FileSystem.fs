namespace FileSystem

open Fable.Core

[<AttachMembers>]
type FileSystem = 
    {
        Tree : FileSystemTree
        History : Commit array
    }
    
    [<NamedParams>]
    static member create(tree : FileSystemTree, ?history : Commit array) = 
        let history = defaultArg history [||]
        {
            Tree = tree
            History = history
        }

    member this.AddFile (path : string) : FileSystem =
        {
            this with
                Tree = this.Tree.AddFile(path)
        }

    static member fromFilePaths (paths : string []) : FileSystem =
        let tree = FileSystemTree.fromFilePaths paths
        FileSystem.create(tree)
