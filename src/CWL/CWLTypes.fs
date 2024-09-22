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

    type InputRecordSchema () =
        inherit DynamicObj ()

    type InputEnumSchema () =
        inherit DynamicObj ()

    type InputArraySchema () =
        inherit DynamicObj ()

    type SchemaDefRequirementType (types, definitions) as this =
        inherit DynamicObj ()
        do
            DynObj.setValue this (nameof types) definitions

    type SoftwarePackage = {
        Package: string
        Version: string [] option
        Specs: string [] option
    }
