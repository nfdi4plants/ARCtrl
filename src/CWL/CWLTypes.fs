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

    /// The path of the $mixin directive in a mapping that holds one, and the empty text for any other node.
    member this.MixinPath : string =
        match this.Node.Kind with
        | YamlNodeKind.Mapping collection ->
            collection.Items
            |> List.tryPick (fun entry ->
                match entry.Key.Kind, entry.Value.Kind with
                | YamlNodeKind.Scalar key, YamlNodeKind.Scalar path when key.Value = "$mixin" -> Some path.Value
                | _ -> None)
            |> Option.defaultValue ""
        | _ -> ""

/// Reads a number that sits in an obj as the 64-bit integer that it holds. A JavaScript number is a float whether it
/// came from an int or a float, so a type test for int matches every number there, and converting 4.5 to an int64 raises
/// an exception. The test below keeps the numbers that are no integer out on every target.
module internal IntegerOf =

    let tryNumber (value: float) : int64 option =
        if System.Double.IsNaN value || System.Double.IsInfinity value || value <> System.Math.Floor value then
            None
        elif value >= -9.2233720368547758e18 && value < 9.2233720368547758e18 then
            Some (int64 value)
        else
            None

type SchemaSaladString =
    | Literal of string
    | Include of string
    | Import of string
    /// A $mixin on its own.
    | Mixin of string
    /// A $mixin beside other fields, kept as it was written. When the file holds an object, the fields complete it, and when
    /// the file holds a string, cwltool takes the string and drops the fields. The decoder reads no file, so it cannot tell
    /// which of the two happens.
    | Mixed of RawYaml
    with
    /// The text of a literal and the path of a directive.
    member this.Value =
        match this with
        | Literal value
        | Include value
        | Import value
        | Mixin value -> value
        | Mixed raw -> raw.MixinPath

    /// The text as a document spells it, with the directive in front of the path. The description of an expression tool in
    /// an RO-Crate and the properties of a node of the workflow graph show a string through it, so it is a text for display.
    /// The decoder and the encoder keep the directive as a case, so a document never holds this text.
    member this.AsDirectiveString =
        match this with
        | Literal value -> value
        | Include value -> sprintf "$include: %s" value
        | Import value -> sprintf "$import: %s" value
        | Mixin value -> sprintf "$mixin: %s" value
        | Mixed raw -> sprintf "$mixin: %s" raw.MixinPath

module SchemaSaladString =

    let literal value = Literal value

    let includePath value = Include value

    let importPath value = Import value

    let mixinPath value = Mixin value

    let value (saladString: SchemaSaladString) = saladString.Value

    /// The text of a literal string, and none for a directive, which names a file whose text the model does not hold.
    let tryLiteral (saladString: SchemaSaladString) : string option =
        match saladString with
        | Literal text -> Some text
        | Include _
        | Import _
        | Mixin _
        | Mixed _ -> None

/// An entry of a list of objects or numbers, such as the inputs, the outputs, the steps, the fields of a record or the
/// success codes. An entry is the value itself, or the import of a file that holds the value or a list of values, as in
/// inputs: [{$import: ports.yml}]. A value that a file completes through $mixin is a third kind, which keeps the mapping as
/// written. The decoder reads no file, so the import and the mixin stay entries of their own, in their place.
[<RequireQualifiedAccess; NoComparison; AttachMembers>]
type ListEntry<'T> =
    | Value of 'T
    | Imported of string
    /// A mapping with a $mixin directive: the alone directive, or the directive beside the fields that complete the
    /// object. The mapping is kept as it was written.
    | Mixed of RawYaml

    /// The object of a value entry, and none for an import or a mixin.
    static member tryValue (entry: ListEntry<'T>) : 'T option =
        match entry with
        | Value value -> Some value
        | Imported _
        | Mixed _ -> None

    /// The objects of the value entries, in order. The imports and the mixins stand for objects that the decoder does not
    /// read, so they are left out. The result is a new list.
    static member values (entries: seq<ListEntry<'T>>) : ResizeArray<'T> =
        entries |> Seq.choose ListEntry.tryValue |> ResizeArray

    /// The value entries for these objects, in order.
    static member ofValues (values: seq<'T>) : ResizeArray<ListEntry<'T>> =
        values |> Seq.map ListEntry.Value |> ResizeArray

    /// The entry with another object, and the same import or mixin.
    static member map (mapping: 'T -> 'U) (entry: ListEntry<'T>) : ListEntry<'U> =
        match entry with
        | Value value -> ListEntry.Value (mapping value)
        | Imported path -> ListEntry.Imported path
        | Mixed raw -> ListEntry.Mixed raw

/// A field that CWL types as a boolean: the value, or a directive that stands for it. An $import takes the data of a
/// file, and a $mixin does the same and joins it with the fields beside the directive, so both can stand where a boolean
/// stands. An $include yields the text of a file, which is a string, so it cannot. The decoder reads no file, so a
/// directive stays as written and a mixin keeps its mapping. A directive is a case of its own because a value that is
/// missing, or false, would pass for it.
[<RequireQualifiedAccess; NoComparison; AttachMembers>]
type SchemaSaladBool =
    | Value of bool
    | Imported of string
    /// A mapping with a $mixin directive, alone or beside other fields, kept as it was written.
    | Mixed of RawYaml

    /// The boolean of a value, and none for a directive, which names a file whose data the model does not hold.
    static member tryValue (value: SchemaSaladBool) : bool option =
        match value with
        | Value flag -> Some flag
        | Imported _
        | Mixed _ -> None

/// The size of a File object in bytes: the number, or a directive that stands for it. The directives that can stand
/// there are those of SchemaSaladBool, for the same reason.
[<RequireQualifiedAccess; NoComparison; AttachMembers>]
type FileSize =
    | Bytes of int64
    | Imported of string
    /// A mapping with a $mixin directive, alone or beside other fields, kept as it was written.
    | Mixed of RawYaml

    /// The number of bytes of a size, and none for a directive.
    static member tryBytes (size: FileSize) : int64 option =
        match size with
        | Bytes count -> Some count
        | Imported _
        | Mixed _ -> None

/// The doc of a CWL object. CWL admits one string or a list of strings, and Schema Salad joins the items of a list
/// without a separator. A string, a directive and a list of plain strings are each one value, so the list [a, b] and the
/// string ab are the same doc, and the layout of the author stays in the source node of the object. A directive in a
/// list names a file with a part of the text, and the model does not hold that text, so the parts stay apart. The
/// parts exist for that case only. A doc is the one string position that takes a list, so a list of strings has a type of
/// its own and no other field can hold one.
[<RequireQualifiedAccess; CustomEquality; NoComparison; AttachMembers>]
type DocText =
    /// A string or a directive, or a list of plain strings joined into one text.
    | Text of SchemaSaladString
    /// A list with a directive item, with every item in order.
    | Parts of ResizeArray<SchemaSaladString>

    override this.Equals(o: obj) =
        match o with
        | :? DocText as other ->
            match this, other with
            | Text left, Text right -> left = right
            | Parts left, Parts right -> HashHelpers.seqEqual left right
            | _ -> false
        | _ -> false

    override this.GetHashCode() =
        match this with
        | Text value -> HashHelpers.mergeHashes 1 (hash value)
        | Parts values -> HashHelpers.mergeHashes 2 (HashHelpers.hashSeq values)

    /// The doc that is the given text.
    static member literal (text: string) : DocText =
        Text (SchemaSaladString.Literal text)

    /// The text of the doc, and none for a doc with a directive.
    member this.TryText : string option =
        match this with
        | Text value -> SchemaSaladString.tryLiteral value
        | Parts _ -> None

[<AttachMembers>]
type FileInstance (
    ?location: string,
    ?path: string,
    ?basename: string,
    ?dirname: string,
    ?nameroot: string,
    ?nameext: string,
    ?checksum: string,
    ?size: FileSize,
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
        HashHelpers.hashFields [
            box this.Location
            box this.Path
            box this.Basename
            box this.Dirname
            box this.Nameroot
            box this.Nameext
            box this.Checksum
            box this.Size
            box this.SecondaryFiles
            box this.Format
            box this.Contents
            box (DynamicObjHelpers.hashDynamicProperties this)
        ]

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
type DirentInstance (entry: SchemaSaladString, ?entryname: SchemaSaladString, ?writable: SchemaSaladBool) =
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
      Label: YamlNode option
      /// The names of the list fields that the document writes as one directive, as in requirements: {$import: path}.
      /// The model holds such a field as a list with one imported entry, and the encoder writes the directive again
      /// while the list holds that entry only.
      WholeFields: Set<string> }

    override this.Equals(o: obj) =
        match o with
        | :? SourceNodes -> true
        | _ -> false

    override this.GetHashCode() = 0

    static member Empty : SourceNodes = { Doc = None; Label = None; WholeFields = Set.empty }

/// Represents an enumeration type with a defined set of valid symbol values.
/// Per the CWL specification, symbol order is semantically significant and preserved during serialization.
/// A symbol is a string, so a directive can stand in its place. An $include yields the text of a file as the symbol,
/// and an $import yields the symbol or the list of symbols of a file. The decoder reads no file, so a directive stays
/// an entry of the list, in its place.
[<AttachMembers>]
type InputEnumSchema (symbols: ResizeArray<SchemaSaladString>, ?label: SchemaSaladString, ?doc: DocText, ?name: string) =
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

    /// Returns a new list of the symbols that the document writes out as strings. The directives are left out, so a change
    /// of the list changes nothing in the enum. The entries of the enum are in Symbols.
    static member getSymbolValues (schema: InputEnumSchema) : ResizeArray<string> =
        schema.Symbols |> Seq.choose SchemaSaladString.tryLiteral |> ResizeArray

/// Represents a field in an InputRecordSchema
[<AttachMembers>]
type InputRecordField (name: string, type_: CWLType, ?doc: DocText, ?label: SchemaSaladString) =
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

/// Represents a record schema for workflow input parameters. An entry of the fields is a field, or the import of a file
/// that holds a field or a list of fields. The decoder reads no file, so the import stays an entry of its own, in its
/// place.
and [<AttachMembers>] InputRecordSchema (?fields: ResizeArray<ListEntry<InputRecordField>>, ?label: SchemaSaladString, ?doc: DocText, ?name: string) =
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
                | Some f1, Some f2 -> HashHelpers.seqEqual f1 f2
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

    /// Returns a new list of the fields of the record that are objects, or an empty list for a record without fields. The
    /// imports and the mixins are left out, so a change of the list changes nothing in the record. The entries of the
    /// record are in Fields.
    static member getFieldValuesOrEmpty (schema: InputRecordSchema) : ResizeArray<InputRecordField> =
        schema.Fields |> Option.map ListEntry.values |> Option.defaultValue (ResizeArray())

/// Represents an array schema for workflow input parameters
and [<AttachMembers>] InputArraySchema (items: CWLType, ?label: SchemaSaladString, ?doc: DocText, ?name: string) =
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
    /// A type that a file holds, as type: {$import: path}. The decoder reads no file, so the type stays unresolved. It can
    /// stand wherever a type can: as the type of a port, as a member of a type list and as the items of an array.
    | ImportedType of string
    /// A type that the text of a file names, as type: {$include: path}. The text is the name of a type, and the decoder
    /// reads no file, so the type stays unresolved like an import.
    | IncludedType of string
    /// A type that a file completes through $mixin, as type: {$mixin: path}. The mapping is kept as it was written, and the
    /// type stays unresolved like an import.
    | MixedType of RawYaml
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
            | ImportedType left, ImportedType right -> left = right
            | IncludedType left, IncludedType right -> left = right
            | MixedType left, MixedType right -> left.Equals(right)
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
        | ImportedType path -> hash (19, path)
        | MixedType raw -> hash (20, raw)
        | IncludedType path -> hash (21, path)

/// The schema that a SchemaDefRequirement defines. CWL admits record, enum and array schemas there and no other type,
/// so an alias for a builtin type has no case. A union of the three schema classes holds that rule in the type itself,
/// and no place has to check it. The schema keeps its own name, which is optional, and its own extension fields. A port
/// cannot name an array definition, because cwltool reports its name as an unknown type, so the case only holds the
/// definition of a document that never uses it.
[<RequireQualifiedAccess; CustomEquality; NoComparison; AttachMembers>]
type SchemaDefinition =
    | Record of InputRecordSchema
    | Enum of InputEnumSchema
    | Array of InputArraySchema
    with
    override this.Equals(o: obj) =
        match o with
        | :? SchemaDefinition as other ->
            match this, other with
            | Record left, Record right -> left.Equals(right)
            | Enum left, Enum right -> left.Equals(right)
            | Array left, Array right -> left.Equals(right)
            | _ -> false
        | _ -> false

    override this.GetHashCode() =
        match this with
        | Record schema -> hash (0, schema)
        | Enum schema -> hash (1, schema)
        | Array schema -> hash (2, schema)


[<AttachMembers>]
type SoftwarePackage (package: SchemaSaladString, ?version: ResizeArray<SchemaSaladString>, ?specs: ResizeArray<SchemaSaladString>) =
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
            |> Seq.choose ListEntry.tryValue
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
                Seq.forall2 (fun (leftEntry: ListEntry<InputRecordField>) (rightEntry: ListEntry<InputRecordField>) ->
                    match leftEntry, rightEntry with
                    | ListEntry.Value leftField, ListEntry.Value rightField ->
                        leftField.Name = rightField.Name &&
                        typesEqual leftField.Type rightField.Type
                    | ListEntry.Imported leftPath, ListEntry.Imported rightPath -> leftPath = rightPath
                    | ListEntry.Mixed leftRaw, ListEntry.Mixed rightRaw -> leftRaw.Equals rightRaw
                    | _ -> false
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
