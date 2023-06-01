namespace FileSystem

open Fable.Core

[<AttachMembers>]
type FileSystem = 
    {
        Tree : FileSystemTree
        History : Commit array
    }
    
    [<NamedParams>]
    static member create(tree : FileSystemTree, history : Commit array) = 
        {
            Tree = tree
            History = history
        }


    static member addFolder (path : string) (fileSystem : FileSystem) : FileSystem =
        FileSystemTree.addFolder |> ignore
        raise (System.NotImplementedException())

    static member addFile (path : string) (fileSystem : FileSystem) : FileSystem =
        FileSystemTree.addFile |> ignore
        raise (System.NotImplementedException())
