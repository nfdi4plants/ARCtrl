namespace ARCtrl.CWL

open DynamicObj

type FileInstance () =
    inherit DynamicObj ()

    override this.GetHashCode (): int =
        this.DeepCopyProperties().GetHashCode()

    override this.Equals (o: obj): bool =
        match o with
        | :? FileInstance as o ->
            this.StructurallyEquals o
        | _ -> false

type DirectoryInstance () =
    inherit DynamicObj ()

    override this.Equals (o: obj): bool =
        match o with
        | :? DirectoryInstance as o ->
            this.StructurallyEquals o
        | _ -> false

    override this.GetHashCode (): int = 
        this.DeepCopyProperties().GetHashCode()

type DirentInstance = {
    // can be string or expression, but expression is string as well
    Entry: string
    Entryname: string option
    Writable: bool option
}

/// Represents a field in an InputRecordSchema
and InputRecordField = {
    Name: string
    Type: CWLType
    Doc: string option
    Label: string option
}

/// Represents a record schema for workflow input parameters
and InputRecordSchema = {
    Fields: ResizeArray<InputRecordField> option
    Label: string option
    Doc: string option
    Name: string option
}

/// Represents an enum schema for workflow input parameters
and InputEnumSchema = {
    Symbols: ResizeArray<string>
    Label: string option
    Doc: string option
    Name: string option
}

/// Represents an array schema for workflow input parameters
and InputArraySchema = {
    Items: CWLType
    Label: string option
    Doc: string option
    Name: string option
}

/// Primitive types with the concept of a file and directory as a builtin type.
and CWLType =
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
    | Array of InputArraySchema
    | Record of InputRecordSchema
    | Enum of InputEnumSchema
    | Union of ResizeArray<CWLType>

    static member file() = File(FileInstance())

    static member directory() = Directory(DirectoryInstance())

type SchemaDefRequirementType (types, definitions) as this =
    inherit DynamicObj ()
    do
        DynObj.setProperty (nameof types) definitions this

type SoftwarePackage = {
    Package: string
    Version: ResizeArray<string> option
    Specs: ResizeArray<string> option
}
