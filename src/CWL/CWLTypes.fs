namespace ARCtrl.CWL

open DynamicObj

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

/// Primitive types with the concept of a file and directory as a builtin type.
type CWLType =
    /// Represents a file (or group of files when secondaryFiles is provided)
    | File of FileInstance
    /// Represents a directory to present to a command line tool.
    /// Directories are represented as objects with class of Directory. Directory objects have a number of properties that provide metadata about the directory.
    | Directory of DirectoryInstance
    /// Define a file or subdirectory that must be placed in the designated output directory prior to executing the command line tool.
    /// May be the result of executing an expression, such as building a configuration file from a template.
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

type InputRecordSchema () =
    inherit DynamicObj ()

type InputEnumSchema () =
    inherit DynamicObj ()

type InputArraySchema () =
    inherit DynamicObj ()

type SchemaDefRequirementType (types, definitions) as this =
    inherit DynamicObj ()
    do
        DynObj.setProperty (nameof types) definitions this

type SoftwarePackage = {
    Package: string
    Version: ResizeArray<string> option
    Specs: ResizeArray<string> option
}
