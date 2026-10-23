namespace ARCtrl.CWL

open DynamicObj
open Fable.Core
open YAMLicious

/// A YAML value that a CWL object keeps as it was written, such as the default value of an input or the
/// secondaryFiles of a port. The node keeps its comments, blank lines, scalar styles and collection styles, and the
/// encoder writes them back. A comment behind the last entry of a raw value moves in front of that entry when the
/// encoder writes the value in front of other entries. YamlNode has no structural equality. DynamicObj compares and
/// hashes the properties of an object through the objects that it holds, so the value carries its own equality. Two
/// values are equal when their nodes have the same shape, tags, keys and scalar texts, and when each scalar is plain
/// in both or quoted in both. Comments, blank lines, anchors, collection styles and the kind of quote do not count.
[<CustomEquality; NoComparison>]
type RawYaml =
    { Node: YamlNode }

    override this.Equals(o: obj) =
        match o with
        | :? RawYaml as other -> HashHelpers.yamlNodeEqual this.Node other.Node
        | _ -> false

    override this.GetHashCode() =
        HashHelpers.hashYamlNode this.Node

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

    /// The text as a document spells it, with the directive in front of the path. A display of the string can use it.
    /// The decoder and the encoder keep the directive as a case, so no field of a document holds this text.
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

[<AttachMembers>]
type FileInstance (
    ?location: string,
    ?path: string,
    ?basename: string,
    ?dirname: string,
    ?nameroot: string,
    ?nameext: string,
    ?checksum: string,
    ?size: int64,
    ?secondaryFiles: RawYaml,
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
        CWLKnownFieldNames.fileInstance

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
            DynamicObjHelpers.hashDynamicProperties this
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
            DynamicObjHelpers.dynamicPropertiesEqual this o
        | _ -> false

[<AttachMembers>]
type DirectoryInstance (
    ?location: string,
    ?path: string,
    ?basename: string,
    ?listing: RawYaml
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
        CWLKnownFieldNames.directoryInstance

    override this.Equals (o: obj): bool =
        match o with
        | :? DirectoryInstance as o ->
            this.Location = o.Location &&
            this.Path = o.Path &&
            this.Basename = o.Basename &&
            this.Listing = o.Listing &&
            DynamicObjHelpers.dynamicPropertiesEqual this o
        | _ -> false

    override this.GetHashCode (): int = 
        hash (this.Location, this.Path, this.Basename, this.Listing, DynamicObjHelpers.hashDynamicProperties this)

[<AttachMembers>]
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
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.Entry, this.Entryname, this.Writable, DynamicObjHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        CWLKnownFieldNames.direntInstance

/// The nodes that the doc and the label of a CWL object were read from. A node holds the layout of the author, such as
/// the lines of a folded doc. The encoder writes the current text of the property into the node. The node gives the
/// lines of the document back while the text is unchanged, and an edited text keeps the style of the node and the lines
/// that still fit.
///
/// The layout is no part of the value of an object, so every SourceNodes is equal to every other one and hashes to 0.
/// DynamicObj compares and hashes all properties of an object on JavaScript and Python, whatever their visibility, and
/// the objects without an Equals of their own rely on that. YamlNode has no structural equality either.
[<CustomEquality; NoComparison>]
type internal SourceNodes =
    { Doc: YamlNode option
      Label: YamlNode option }

    override this.Equals(o: obj) =
        match o with
        | :? SourceNodes -> true
        | _ -> false

    override this.GetHashCode() = 0

    static member Empty : SourceNodes = { Doc = None; Label = None }

/// Represents an enumeration type with a defined set of valid symbol values.
/// Per the CWL specification, symbol order is semantically significant and preserved during serialization.
[<AttachMembers>]
type InputEnumSchema (symbols: ResizeArray<string>, ?label: string, ?doc: string, ?name: string) =
    inherit DynamicObj ()

    let mutable _symbols = symbols
    let mutable _label = label
    let mutable _doc = doc
    let mutable _name = name
    let mutable _sourceNodes = SourceNodes.Empty

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

    /// The nodes that the doc and the label were read from. The encoder writes the layout of the author from them.
    /// They stay out of the equality and the hash of the object.
    member internal this.SourceNodes
        with get() = _sourceNodes
        and set(value) = _sourceNodes <- value

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
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode(): int =
        hash (this.Symbols |> Seq.toList, this.Label, this.Doc, this.Name, DynamicObjHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        CWLKnownFieldNames.inputEnumSchema

/// Represents a field in an InputRecordSchema
[<AttachMembers>]
type InputRecordField (name: string, type_: CWLType, ?doc: string, ?label: string) =
    inherit DynamicObj ()

    let mutable _name = name
    let mutable _type = type_
    let mutable _doc = doc
    let mutable _label = label
    let mutable _sourceNodes = SourceNodes.Empty

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

    /// The nodes that the doc and the label were read from. The encoder writes the layout of the author from them.
    /// They stay out of the equality and the hash of the object.
    member internal this.SourceNodes
        with get() = _sourceNodes
        and set(value) = _sourceNodes <- value

    override this.Equals(o: obj): bool =
        match o with
        | :? InputRecordField as other ->
            this.Name = other.Name &&
            this.Type.Equals(other.Type) &&
            this.Doc = other.Doc &&
            this.Label = other.Label &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode(): int =
        hash (this.Name, this.Type, this.Doc, this.Label, DynamicObjHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        CWLKnownFieldNames.inputRecordField

/// Represents a record schema for workflow input parameters
and [<AttachMembers>] InputRecordSchema (?fields: ResizeArray<InputRecordField>, ?label: string, ?doc: string, ?name: string) =
    inherit DynamicObj ()

    let mutable _fields = fields
    let mutable _label = label
    let mutable _doc = doc
    let mutable _name = name
    let mutable _sourceNodes = SourceNodes.Empty

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

    /// The nodes that the doc and the label were read from. The encoder writes the layout of the author from them.
    /// They stay out of the equality and the hash of the object.
    member internal this.SourceNodes
        with get() = _sourceNodes
        and set(value) = _sourceNodes <- value

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
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode(): int =
        let fieldsHash =
            this.Fields
            |> Option.map (Seq.map (fun field -> field.GetHashCode()) >> Seq.toList)
        hash (fieldsHash, this.Label, this.Doc, this.Name, DynamicObjHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        CWLKnownFieldNames.inputRecordSchema

/// Represents an array schema for workflow input parameters
and [<AttachMembers>] InputArraySchema (items: CWLType, ?label: string, ?doc: string, ?name: string) =
    inherit DynamicObj ()

    let mutable _items = items
    let mutable _label = label
    let mutable _doc = doc
    let mutable _name = name
    let mutable _sourceNodes = SourceNodes.Empty

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

    /// The nodes that the doc and the label were read from. The encoder writes the layout of the author from them.
    /// They stay out of the equality and the hash of the object.
    member internal this.SourceNodes
        with get() = _sourceNodes
        and set(value) = _sourceNodes <- value

    override this.Equals(o: obj): bool =
        match o with
        | :? InputArraySchema as other ->
            this.Items.Equals(other.Items) &&
            this.Label = other.Label &&
            this.Doc = other.Doc &&
            this.Name = other.Name &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode(): int =
        hash (this.Items, this.Label, this.Doc, this.Name, DynamicObjHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        CWLKnownFieldNames.inputArraySchema

/// Primitive types with the concept of a file and directory as a builtin type.
and [<CustomEquality; NoComparison>] CWLType =
    /// A file. The type has no fields, because CWL spells it as the name File only. The format and the secondaryFiles
    /// of a file belong to the port that has this type, and the location and the other fields belong to a File value.
    | File
    /// A directory to present to a command line tool. The type has no fields, like File.
    | Directory
    | String
    | Int
    | Long
    | Float
    | Double
    | Boolean
    /// Validates for any non-null value.
    | Any
    /// The type of an output of a command line tool that holds the standard output of the tool. CWL admits it as the
    /// whole type of an output without an outputBinding.
    | Stdout
    /// The type of an output of a command line tool that holds the standard error of the tool. CWL admits it as the
    /// whole type of an output without an outputBinding.
    | Stderr
    /// The type of an input of a command line tool that reads from the standard input of the tool. CWL admits it as the
    /// whole type of an input without an inputBinding, and the tool must not set stdin itself.
    | Stdin
    /// A type that a SchemaDefRequirement defines, spelled by its name as written (Name, #Name or file#Name).
    | Reference of string
    | Null
    | Array of InputArraySchema
    | Record of InputRecordSchema
    | Enum of InputEnumSchema
    | Union of ResizeArray<CWLType>

    override this.Equals(o: obj): bool =
        match o with
        | :? CWLType as other ->
            match this, other with
            | File, File -> true
            | Directory, Directory -> true
            | String, String -> true
            | Int, Int -> true
            | Long, Long -> true
            | Float, Float -> true
            | Double, Double -> true
            | Boolean, Boolean -> true
            | Any, Any -> true
            | Stdout, Stdout -> true
            | Stderr, Stderr -> true
            | Stdin, Stdin -> true
            | Reference left, Reference right -> left = right
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
        | File -> hash 0
        | Directory -> hash 1
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
        | Any -> hash 15
        | Stderr -> hash 16
        | Stdin -> hash 17
        | Reference name -> hash (18, name)

/// The schema that a SchemaDefRequirement defines. CWL admits record and enum schemas there and no other type, so an
/// alias for a builtin type and an array schema have no case. A union of the two schema classes holds that rule in the
/// type itself, and no place has to check it. The schema keeps its own name, which is optional, and its own extension
/// fields.
[<RequireQualifiedAccess; CustomEquality; NoComparison; AttachMembers>]
type SchemaDefinition =
    | Record of InputRecordSchema
    | Enum of InputEnumSchema
    with
    override this.Equals(o: obj) =
        match o with
        | :? SchemaDefinition as other ->
            match this, other with
            | Record left, Record right -> left.Equals(right)
            | Enum left, Enum right -> left.Equals(right)
            | _ -> false
        | _ -> false

    override this.GetHashCode() =
        match this with
        | Record schema -> hash (0, schema)
        | Enum schema -> hash (1, schema)


[<AttachMembers>]
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
            HashHelpers.optionSeqEqual this.Version other.Version &&
            HashHelpers.optionSeqEqual this.Specs other.Specs &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.Package, HashHelpers.hashOptionSeq this.Version, HashHelpers.hashOptionSeq this.Specs, DynamicObjHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        CWLKnownFieldNames.softwarePackage

module FileInstance =

    let pathOrLocation (value: FileInstance) =
        value.Path
        |> Option.orElse value.Location
        |> Option.defaultValue ""

module DirectoryInstance =

    let pathOrLocation (value: DirectoryInstance) =
        value.Path
        |> Option.orElse value.Location
        |> Option.defaultValue ""

module InputRecordSchema =

    let tryGetFieldType (name: string) (schema: InputRecordSchema) =
        schema.Fields
        |> Option.bind (fun fields ->
            fields
            |> Seq.tryFind (fun field -> field.Name = name)
            |> Option.map (fun field -> field.Type)
        )

module CWLType =

    /// Compare two CWLTypes for equality, ignoring the label, the doc and the name of array and record schemas.
    ///
    /// This is deliberately laxer than CWLType.Equals, which compares them too.
    let rec typesEqual (left : CWLType) (right : CWLType) =
        match left, right with
        | CWLType.Array left, CWLType.Array right ->
            typesEqual left.Items right.Items
        | CWLType.Union left, CWLType.Union right ->
            left.Count = right.Count &&
            Seq.forall2 typesEqual left right
        | CWLType.Record left, CWLType.Record right ->
            match left.Fields, right.Fields with
            | None, None -> true
            | Some leftFields, Some rightFields ->
                leftFields.Count = rightFields.Count &&
                Seq.forall2 (fun (leftField: InputRecordField) (rightField: InputRecordField) ->
                    leftField.Name = rightField.Name &&
                    typesEqual leftField.Type rightField.Type
                ) leftFields rightFields
            | _ -> false
        | _ -> left = right

    /// True when the type is a union that contains null, so that a value of the type may be missing. The shorthand
    /// File? is such a union.
    let isOptional (type_ : CWLType) =
        match type_ with
        | CWLType.Union types -> types |> Seq.exists (function CWLType.Null -> true | _ -> false)
        | _ -> false

    /// The type with null added: the union of null and the type, with null in front. A type that admits null already
    /// stays as it is, and the members of a union stay the members of one union.
    let makeOptional (type_ : CWLType) =
        match type_ with
        | CWLType.Null -> type_
        | CWLType.Union types when isOptional type_ -> type_
        | CWLType.Union types -> CWLType.Union (ResizeArray (Seq.append [ CWLType.Null ] types))
        | _ -> CWLType.Union (ResizeArray [ CWLType.Null; type_ ])

    /// Helper function to check if a CWLType is or contains an Array type
    let isArrayType (type_ : CWLType) =
        match type_ with
        | CWLType.Array _ -> true
        | CWLType.Union types -> types |> Seq.exists (function CWLType.Array _ -> true | _ -> false)
        | _ -> false

    let tryGetNonNullUnionType (type_ : CWLType) =
        match type_ with
        | CWLType.Union types ->
            types
            |> Seq.tryFind (function
                | CWLType.Null -> false
                | _ -> true)
        | _ -> Some type_
