namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

/// The position of an input binding: a number, or an expression that evaluates to one. An expression is a string with
/// $( or ${ in it.
[<RequireQualifiedAccess>]
type BindingPosition =
    | Number of int
    | Expression of string

[<AttachMembers>]
type InputBinding (
    ?prefix: SchemaSaladString,
    ?position: BindingPosition,
    ?itemSeparator: SchemaSaladString,
    ?separate: bool,
    ?loadContents: bool,
    ?valueFrom: SchemaSaladString,
    ?shellQuote: bool
) =
    inherit DynamicObj ()

    let mutable _prefix = prefix
    let mutable _position = position
    let mutable _itemSeparator = itemSeparator
    let mutable _separate = separate
    let mutable _loadContents = loadContents
    let mutable _valueFrom = valueFrom
    let mutable _shellQuote = shellQuote

    member this.Prefix
        with get() = _prefix
        and set(value) = _prefix <- value

    #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
    member this.Position
        with [<EmitProperty("_position")>] get() = _position
        and set(value) = _position <- value
    #else
    member this.Position
        with get() = _position
        and set(value) = _position <- value
    #endif

    member this.ItemSeparator
        with get() = _itemSeparator
        and set(value) = _itemSeparator <- value

    member this.Separate
        with get() = _separate
        and set(value) = _separate <- value

    member this.LoadContents
        with get() = _loadContents
        and set(value) = _loadContents <- value

    member this.ValueFrom
        with get() = _valueFrom
        and set(value) = _valueFrom <- value

    member this.ShellQuote
        with get() = _shellQuote
        and set(value) = _shellQuote <- value

    override this.Equals(o: obj) =
        match o with
        | :? InputBinding as other ->
            this.Prefix = other.Prefix &&
            this.Position = other.Position &&
            this.ItemSeparator = other.ItemSeparator &&
            this.Separate = other.Separate &&
            this.LoadContents = other.LoadContents &&
            this.ValueFrom = other.ValueFrom &&
            this.ShellQuote = other.ShellQuote &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        HashHelpers.hashFields [
            box this.Prefix
            box this.Position
            box this.ItemSeparator
            box this.Separate
            box this.LoadContents
            box this.ValueFrom
            box this.ShellQuote
            box (DynamicObjHelpers.hashDynamicProperties this)
        ]

    static member KnownFieldNames =
        CWLKnownFieldNames.inputBinding

    static member create
        (
            ?prefix: SchemaSaladString,
            ?position: BindingPosition,
            ?itemSeparator: SchemaSaladString,
            ?separate: bool,
            ?loadContents: bool,
            ?valueFrom: SchemaSaladString,
            ?shellQuote: bool
        ) =
        InputBinding(?prefix = prefix, ?position = position, ?itemSeparator = itemSeparator, ?separate = separate, ?loadContents = loadContents, ?valueFrom = valueFrom, ?shellQuote = shellQuote)

/// The format of an input, as a single name or as a list of names. A name is an IRI or an expression, and a directive
/// names a file with the text of the name. A single name differs from a list of one name, and the empty list stays an
/// empty list, because CWL keeps the spellings apart. The lists compare by their elements, because a ResizeArray
/// compares references. The format of an output is a single name only.
[<RequireQualifiedAccess; CustomEquality; NoComparison; AttachMembers>]
type InputFormat =
    | Single of SchemaSaladString
    | Multiple of ResizeArray<SchemaSaladString>
    with
    member this.AsValues() =
        match this with
        | Single value -> ResizeArray [| value |]
        | Multiple values -> values

    override this.Equals(o: obj) =
        match o with
        | :? InputFormat as other ->
            match this, other with
            | Single left, Single right -> left = right
            | Multiple left, Multiple right -> HashHelpers.seqEqual left right
            | _ -> false
        | _ -> false

    override this.GetHashCode() =
        match this with
        | Single value -> HashHelpers.mergeHashes 1 (hash value)
        | Multiple values -> HashHelpers.mergeHashes 2 (HashHelpers.hashSeq values)

/// An input of a process. The type tells whether the input is optional: it is when the type is a union that contains
/// null. The parameter optional adds null to a type that has none, and it changes nothing for an input without a type.
[<AttachMembers>]
type CWLInput (
    name: string,
    ?type_: CWLType,
    ?inputBinding: InputBinding,
    ?optional: bool,
    ?label: SchemaSaladString,
    ?secondaryFiles: RawYaml,
    ?streamable: bool,
    ?doc: DocText,
    ?format: InputFormat,
    ?loadContents: bool,
    ?loadListing: LoadListingEnum,
    ?defaultValue: RawYaml
) =
    inherit DynamicObj ()

    let mutable _name = name
    let mutable _type =
        match type_, optional with
        | Some type_, Some true -> Some (CWLType.makeOptional type_)
        | _ -> type_
    let mutable _inputBinding = inputBinding
    let mutable _label = label
    let mutable _secondaryFiles = secondaryFiles
    let mutable _streamable = streamable
    let mutable _doc = doc
    let mutable _sourceNodes = SourceNodes.Empty
    let mutable _format = format
    let mutable _loadContents = loadContents
    let mutable _loadListing = loadListing
    let mutable _defaultValue = defaultValue

    member this.Name
        with get() = _name
        and set(value) = _name <- value

    member this.Type_
        with get() = _type
        and set(value) = _type <- value

    member this.InputBinding
        with get() = _inputBinding
        and set(value) = _inputBinding <- value

    /// True when the type is a union that contains null. The type holds this, and an input stores no flag of its own.
    member this.Optional =
        match _type with
        | Some type_ -> CWLType.isOptional type_
        | None -> false

    member this.Label
        with get() = _label
        and set(value) = _label <- value

    member this.SecondaryFiles
        with get() = _secondaryFiles
        and set(value) = _secondaryFiles <- value

    member this.Streamable
        with get() = _streamable
        and set(value) = _streamable <- value

    member this.Doc
        with get() = _doc
        and set(value) = _doc <- value

    /// The nodes that the doc and the label were read from. The encoder writes the layout of the author from them.
    /// They stay out of the equality and the hash of the object.
    member internal this.SourceNodes
        with get() = _sourceNodes
        and set(value) = _sourceNodes <- value

    /// The format of the input: one name or a list of names.
    member this.Format
        with get() = _format
        and set(value) = _format <- value

    member this.LoadContents
        with get() = _loadContents
        and set(value) = _loadContents <- value

    member this.LoadListing
        with get() = _loadListing
        and set(value) = _loadListing <- value

    member this.DefaultValue
        with get() = _defaultValue
        and set(value) = _defaultValue <- value

    override this.Equals(o: obj) =
        match o with
        | :? CWLInput as other ->
            this.Name = other.Name &&
            this.Type_ = other.Type_ &&
            this.InputBinding = other.InputBinding &&
            this.Label = other.Label &&
            this.SecondaryFiles = other.SecondaryFiles &&
            this.Streamable = other.Streamable &&
            this.Doc = other.Doc &&
            this.Format = other.Format &&
            this.LoadContents = other.LoadContents &&
            this.LoadListing = other.LoadListing &&
            this.DefaultValue = other.DefaultValue &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        HashHelpers.hashFields [
            box this.Name
            box this.Type_
            box this.InputBinding
            box this.Label
            box this.SecondaryFiles
            box this.Streamable
            box this.Doc
            box this.Format
            box this.LoadContents
            box this.LoadListing
            box this.DefaultValue
            box (DynamicObjHelpers.hashDynamicProperties this)
        ]

    static member KnownFieldNames =
        CWLKnownFieldNames.cwlInput
