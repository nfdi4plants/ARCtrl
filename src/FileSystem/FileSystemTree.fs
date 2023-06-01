namespace FileSystem

open Fable.Core

[<AttachMembers>]
type FileSystemTree =
    | File of name:string
    | Folder of name:string * children:FileSystemTree array

    static member createFile name = File name

    static member createFolder(name:string, children:FileSystemTree array) = Folder(name,children)

    static member addFolder (path : string) (fileSystem : FileSystemTree) : FileSystemTree =
        let x = Path.split '/' path // todo: make OS agnostic!
        raise (System.NotImplementedException())

    static member addFile (path : string) (fileSystem : FileSystemTree) : FileSystemTree =
        let x = Path.split '/' path // todo: make OS agnostic!
        raise (System.NotImplementedException())

    // to-do: add function to create filesystem tree from a list of paths, is ideal to use from other tools.
    static member fromFilePaths (paths: string array) : FileSystemTree =
        raise (System.NotImplementedException())