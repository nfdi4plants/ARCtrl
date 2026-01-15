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

type InputEnumSchema = {
    Symbols: ResizeArray<string>
    Label: string option
    Doc: string option
    Name: string option
}

/// Represents a field in an InputRecordSchema
[<CustomEquality; NoComparison>]
type InputRecordField = {
    Name: string
    Type: CWLType
    Doc: string option
    Label: string option
}
    with
        override this.Equals(o: obj): bool =
            match o with
            | :? InputRecordField as other ->
                this.Name = other.Name &&
                this.Type.Equals(other.Type) &&
                this.Doc = other.Doc &&
                this.Label = other.Label
            | _ -> false

        override this.GetHashCode(): int =
            hash (this.Name, this.Type, this.Doc, this.Label)

/// Represents a record schema for workflow input parameters
and [<CustomEquality; NoComparison>] InputRecordSchema = {
    Fields: ResizeArray<InputRecordField> option
    Label: string option
    Doc: string option
    Name: string option
}
    with
        override this.Equals(o: obj): bool =
            match o with
            | :? InputRecordSchema as other ->
                this.Label = other.Label &&
                this.Doc = other.Doc &&
                this.Name = other.Name &&
                match this.Fields, other.Fields with
                | None, None -> true
                | Some f1, Some f2 -> 
                    f1.Count = f2.Count && 
                    Seq.forall2 (fun (a: InputRecordField) (b: InputRecordField) -> a.Equals(b)) f1 f2
                | _ -> false
            | _ -> false

        override this.GetHashCode(): int =
            hash (this.Fields, this.Label, this.Doc, this.Name)

/// Represents an array schema for workflow input parameters
and [<CustomEquality; NoComparison>] InputArraySchema = {
    Items: CWLType
    Label: string option
    Doc: string option
    Name: string option
}
    with
        override this.Equals(o: obj): bool =
            match o with
            | :? InputArraySchema as other ->
                this.Items.Equals(other.Items) &&
                this.Label = other.Label &&
                this.Doc = other.Doc &&
                this.Name = other.Name
            | _ -> false

        override this.GetHashCode(): int =
            hash (this.Items, this.Label, this.Doc, this.Name)

/// Primitive types with the concept of a file and directory as a builtin type.
and [<CustomEquality; NoComparison>] CWLType =
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

    override this.Equals(o: obj): bool =
        match o with
        | :? CWLType as other ->
            match this, other with
            | File f1, File f2 -> f1.Equals(f2)
            | Directory d1, Directory d2 -> d1.Equals(d2)
            | Dirent di1, Dirent di2 -> di1 = di2
            | String, String -> true
            | Int, Int -> true
            | Long, Long -> true
            | Float, Float -> true
            | Double, Double -> true
            | Boolean, Boolean -> true
            | Stdout, Stdout -> true
            | Null, Null -> true
            | Array a1, Array a2 -> a1 = a2
            | Record r1, Record r2 -> r1 = r2
            | Enum e1, Enum e2 -> e1 = e2
            | Union u1, Union u2 -> 
                u1.Count = u2.Count && 
                Seq.forall2 (fun (t1: CWLType) (t2: CWLType) -> t1.Equals(t2)) u1 u2
            | _ -> false
        | _ -> false
    
    override this.GetHashCode(): int =
        match this with
        | File f -> hash (0, f.GetHashCode())
        | Directory d -> hash (1, d.GetHashCode())
        | Dirent di -> hash (2, di)
        | String -> hash 3
        | Int -> hash 4
        | Long -> hash 5
        | Float -> hash 6
        | Double -> hash 7
        | Boolean -> hash 8
        | Stdout -> hash 9
        | Null -> hash 10
        | Array a -> hash (11, a)
        | Record r -> hash (12, r)
        | Enum e -> hash (13, e)
        | Union u -> hash (14, u |> Seq.map (fun t -> t.GetHashCode()) |> Seq.toList)

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
