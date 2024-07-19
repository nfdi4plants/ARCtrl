namespace ARCtrl.CWL

open DynamicObj

module CWLTypes =

    type FileInstance () =
        inherit DynamicObj ()

    type DirectoryInstance () =
        inherit DynamicObj ()

    type DirentInstance = {
        // can be string or expression, but expression is string as well
        Entry: string
        Entryname: string option
        Writable: bool option
    }

    type CWLType =
        | File of FileInstance
        | Directory of DirectoryInstance
        | Dirent of DirentInstance
        | String
        | Int
        | Long
        | Float
        | Double
        | Boolean
        | Stdout
        | Null
        | Array of CWLType

    type Class = 
    | Workflow
    | CommandLineTool
    | ExpressionTool
