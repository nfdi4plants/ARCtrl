namespace ARCtrl.CWL

open DynamicObj
open YAMLicious.YAMLiciousTypes

type SchemaSaladString =
    | Literal of string
    | Include of string
    | Import of string
    with
    member this.Value =
        match this with
        | Literal value
        | Include value
        | Import value -> value

    member this.AsDirectiveString =
        match this with
        | Literal value -> value
        | Include value -> sprintf "$include: %s" value
        | Import value -> sprintf "$import: %s" value

module SchemaSaladString =

    let literal value = Literal value

    let includePath value = Include value

    let importPath value = Import value

    let value (saladString: SchemaSaladString) = saladString.Value

    let toDirectiveString (saladString: SchemaSaladString) = saladString.AsDirectiveString

type FileInstance (
    ?location: string,
    ?path: string,
    ?basename: string,
    ?dirname: string,
    ?nameroot: string,
    ?nameext: string,
    ?checksum: string,
    ?size: int64,
    ?secondaryFiles: YAMLElement,
    ?format: string,
    ?contents: string
) =
    inherit DynamicObj ()

    let mutable _location = location
    let mutable _path = path
    let mutable _basename = basename
    let mutable _dirname = dirname
    let mutable _nameroot = nameroot
    let mutable _nameext = nameext
    let mutable _checksum = checksum
    let mutable _size = size
    let mutable _secondaryFiles = secondaryFiles
    let mutable _format = format
    let mutable _contents = contents

    member this.Location
        with get() = _location
        and set(value) = _location <- value

    member this.Path
        with get() = _path
        and set(value) = _path <- value

    member this.Basename
        with get() = _basename
        and set(value) = _basename <- value

    member this.Dirname
        with get() = _dirname
        and set(value) = _dirname <- value

    member this.Nameroot
        with get() = _nameroot
        and set(value) = _nameroot <- value

    member this.Nameext
        with get() = _nameext
        and set(value) = _nameext <- value

    member this.Checksum
        with get() = _checksum
        and set(value) = _checksum <- value

    member this.Size
        with get() = _size
        and set(value) = _size <- value

    member this.SecondaryFiles
        with get() = _secondaryFiles
        and set(value) = _secondaryFiles <- value

    member this.Format
        with get() = _format
        and set(value) = _format <- value

    member this.Contents
        with get() = _contents
        and set(value) = _contents <- value

    static member KnownFieldNames =
        ResizeArray [|
            "class"
            "type"
            "location"
            "path"
            "basename"
            "dirname"
            "nameroot"
            "nameext"
            "checksum"
            "size"
            "secondaryFiles"
            "format"
            "contents"
        |]

    override this.GetHashCode (): int =
        hash (
            this.Location,
            this.Path,
            this.Basename,
            this.Dirname,
            this.Nameroot,
            this.Nameext,
            this.Checksum,
            this.Size,
            this.SecondaryFiles,
            this.Format,
            this.Contents,
            HashHelpers.hashDynamicProperties this
        )

    override this.Equals (o: obj): bool =
        match o with
        | :? FileInstance as o ->
            this.Location = o.Location &&
            this.Path = o.Path &&
            this.Basename = o.Basename &&
            this.Dirname = o.Dirname &&
            this.Nameroot = o.Nameroot &&
            this.Nameext = o.Nameext &&
            this.Checksum = o.Checksum &&
            this.Size = o.Size &&
            this.SecondaryFiles = o.SecondaryFiles &&
            this.Format = o.Format &&
            this.Contents = o.Contents &&
            HashHelpers.dynamicPropertiesEqual this o
        | _ -> false

type DirectoryInstance (
    ?location: string,
    ?path: string,
    ?basename: string,
    ?listing: YAMLElement
) =
    inherit DynamicObj ()

    let mutable _location = location
    let mutable _path = path
    let mutable _basename = basename
    let mutable _listing = listing

    member this.Location
        with get() = _location
        and set(value) = _location <- value

    member this.Path
        with get() = _path
        and set(value) = _path <- value

    member this.Basename
        with get() = _basename
        and set(value) = _basename <- value

    member this.Listing
        with get() = _listing
        and set(value) = _listing <- value

    static member KnownFieldNames =
        ResizeArray [| "class"; "type"; "location"; "path"; "basename"; "listing" |]

    override this.Equals (o: obj): bool =
        match o with
        | :? DirectoryInstance as o ->
            this.Location = o.Location &&
            this.Path = o.Path &&
            this.Basename = o.Basename &&
            this.Listing = o.Listing &&
            HashHelpers.dynamicPropertiesEqual this o
        | _ -> false

    override this.GetHashCode (): int = 
        hash (this.Location, this.Path, this.Basename, this.Listing, HashHelpers.hashDynamicProperties this)

type DirentInstance (entry: SchemaSaladString, ?entryname: SchemaSaladString, ?writable: bool) =
    inherit DynamicObj ()

    let mutable _entry = entry
    let mutable _entryname = entryname
    let mutable _writable = writable

    member this.Entry
        with get() = _entry
        and set(value) = _entry <- value

    member this.Entryname
        with get() = _entryname
        and set(value) = _entryname <- value

    member this.Writable
        with get() = _writable
        and set(value) = _writable <- value

    override this.Equals(o: obj) =
        match o with
        | :? DirentInstance as other ->
            this.Entry = other.Entry &&
            this.Entryname = other.Entryname &&
            this.Writable = other.Writable &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.Entry, this.Entryname, this.Writable, HashHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        ResizeArray [| "entry"; "entryname"; "writable" |]

/// Represents an enumeration type with a defined set of valid symbol values.
/// Per the CWL specification, symbol order is semantically significant and preserved during serialization.
type InputEnumSchema (symbols: ResizeArray<string>, ?label: string, ?doc: string, ?name: string) =
    inherit DynamicObj ()

    let mutable _symbols = symbols
    let mutable _label = label
    let mutable _doc = doc
    let mutable _name = name

    member this.Symbols
        with get() = _symbols
        and set(value) = _symbols <- value

    member this.Label
        with get() = _label
        and set(value) = _label <- value

    member this.Doc
        with get() = _doc
        and set(value) = _doc <- value

    member this.Name
        with get() = _name
        and set(value) = _name <- value

    /// Equality comparison that treats symbol order as significant.
    /// Two enums are equal only if their symbols appear in the same order.
    /// This follows the CWL specification where enum symbol order matters.
    override this.Equals(o: obj): bool =
        match o with
        | :? InputEnumSchema as other ->
            this.Label = other.Label &&
            this.Doc = other.Doc &&
            this.Name = other.Name &&
            this.Symbols.Count = other.Symbols.Count &&
            Seq.forall2 (=) this.Symbols other.Symbols &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode(): int =
        hash (this.Symbols |> Seq.toList, this.Label, this.Doc, this.Name, HashHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        ResizeArray [| "type"; "symbols"; "label"; "doc"; "name" |]

/// Represents a field in an InputRecordSchema
type InputRecordField (name: string, type_: CWLType, ?doc: string, ?label: string) =
    inherit DynamicObj ()

    let mutable _name = name
    let mutable _type = type_
    let mutable _doc = doc
    let mutable _label = label

    member this.Name
        with get() = _name
        and set(value) = _name <- value

    member this.Type
        with get() = _type
        and set(value) = _type <- value

    member this.Doc
        with get() = _doc
        and set(value) = _doc <- value

    member this.Label
        with get() = _label
        and set(value) = _label <- value

    override this.Equals(o: obj): bool =
        match o with
        | :? InputRecordField as other ->
            this.Name = other.Name &&
            this.Type.Equals(other.Type) &&
            this.Doc = other.Doc &&
            this.Label = other.Label &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode(): int =
        hash (this.Name, this.Type, this.Doc, this.Label, HashHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        ResizeArray [| "name"; "type"; "doc"; "label" |]

/// Represents a record schema for workflow input parameters
and InputRecordSchema (?fields: ResizeArray<InputRecordField>, ?label: string, ?doc: string, ?name: string) =
    inherit DynamicObj ()

    let mutable _fields = fields
    let mutable _label = label
    let mutable _doc = doc
    let mutable _name = name

    member this.Fields
        with get() = _fields
        and set(value) = _fields <- value

    member this.Label
        with get() = _label
        and set(value) = _label <- value

    member this.Doc
        with get() = _doc
        and set(value) = _doc <- value

    member this.Name
        with get() = _name
        and set(value) = _name <- value

    override this.Equals(o: obj): bool =
        match o with
        | :? InputRecordSchema as other ->
            this.Label = other.Label &&
            this.Doc = other.Doc &&
            this.Name = other.Name &&
            (
                match this.Fields, other.Fields with
                | None, None -> true
                | Some f1, Some f2 ->
                    f1.Count = f2.Count &&
                    Seq.forall2 (fun (a: InputRecordField) (b: InputRecordField) -> a.Equals(b)) f1 f2
                | _ -> false
            ) &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode(): int =
        let fieldsHash =
            this.Fields
            |> Option.map (Seq.map (fun field -> field.GetHashCode()) >> Seq.toList)
        hash (fieldsHash, this.Label, this.Doc, this.Name, HashHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        ResizeArray [| "type"; "fields"; "label"; "doc"; "name" |]

/// Represents an array schema for workflow input parameters
and InputArraySchema (items: CWLType, ?label: string, ?doc: string, ?name: string) =
    inherit DynamicObj ()

    let mutable _items = items
    let mutable _label = label
    let mutable _doc = doc
    let mutable _name = name

    member this.Items
        with get() = _items
        and set(value) = _items <- value

    member this.Label
        with get() = _label
        and set(value) = _label <- value

    member this.Doc
        with get() = _doc
        and set(value) = _doc <- value

    member this.Name
        with get() = _name
        and set(value) = _name <- value

    override this.Equals(o: obj): bool =
        match o with
        | :? InputArraySchema as other ->
            this.Items.Equals(other.Items) &&
            this.Label = other.Label &&
            this.Doc = other.Doc &&
            this.Name = other.Name &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode(): int =
        hash (this.Items, this.Label, this.Doc, this.Name, HashHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        ResizeArray [| "type"; "items"; "label"; "doc"; "name" |]

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
            | Array a1, Array a2 -> a1.Equals(a2)
            | Record r1, Record r2 -> r1.Equals(r2)
            | Enum e1, Enum e2 -> e1.Equals(e2)
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

type SchemaDefRequirementType (name: string, type_: CWLType) =
    inherit DynamicObj ()

    let mutable _name = name
    let mutable _type = type_

    member this.Name
        with get() = _name
        and set(value) = _name <- value

    member this.Type_
        with get() = _type
        and set(value) = _type <- value

    override this.Equals(o: obj) =
        match o with
        | :? SchemaDefRequirementType as other ->
            this.Name = other.Name &&
            this.Type_.Equals(other.Type_) &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.Name, this.Type_, HashHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        ResizeArray [| "name"; "type" |]

type SoftwarePackage (package: string, ?version: ResizeArray<string>, ?specs: ResizeArray<string>) =
    inherit DynamicObj ()

    let mutable _package = package
    let mutable _version = version
    let mutable _specs = specs

    member this.Package
        with get() = _package
        and set(value) = _package <- value

    member this.Version
        with get() = _version
        and set(value) = _version <- value

    member this.Specs
        with get() = _specs
        and set(value) = _specs <- value

    override this.Equals(o: obj) =
        match o with
        | :? SoftwarePackage as other ->
            this.Package = other.Package &&
            this.Version = other.Version &&
            this.Specs = other.Specs &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.Package, this.Version, this.Specs, HashHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        ResizeArray [| "package"; "version"; "specs" |]
