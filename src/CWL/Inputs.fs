namespace ARCtrl.CWL

open DynamicObj
open Fable.Core
open YAMLicious.YAMLiciousTypes

[<AttachMembers>]
type InputBinding (
    ?prefix: string,
    ?position: int,
    ?itemSeparator: string,
    ?separate: bool,
    ?loadContents: bool,
    ?valueFrom: string,
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

    member this.Position
        with get() = _position
        and set(value) = _position <- value

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
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (
            this.Prefix,
            this.Position,
            this.ItemSeparator,
            this.Separate,
            this.LoadContents,
            this.ValueFrom,
            this.ShellQuote,
            HashHelpers.hashDynamicProperties this
        )

    static member KnownFieldNames =
        ResizeArray [| "loadContents"; "position"; "prefix"; "separate"; "itemSeparator"; "valueFrom"; "shellQuote" |]

    static member create
        (
            ?prefix: string,
            ?position: int,
            ?itemSeparator: string,
            ?separate: bool,
            ?loadContents: bool,
            ?valueFrom: string,
            ?shellQuote: bool
        ) =
        InputBinding(?prefix = prefix, ?position = position, ?itemSeparator = itemSeparator, ?separate = separate, ?loadContents = loadContents, ?valueFrom = valueFrom, ?shellQuote = shellQuote)


[<AttachMembers>]
type CWLInput (
    name: string,
    ?type_: CWLType,
    ?inputBinding: InputBinding,
    ?optional: bool,
    ?label: string,
    ?secondaryFiles: YAMLElement,
    ?streamable: bool,
    ?doc: string,
    ?format: string,
    ?loadContents: bool,
    ?loadListing: LoadListingEnum,
    ?defaultValue: YAMLElement
) =
    inherit DynamicObj ()

    let mutable _name = name
    let mutable _type = type_
    let mutable _inputBinding = inputBinding
    let mutable _optional = optional
    let mutable _label = label
    let mutable _secondaryFiles = secondaryFiles
    let mutable _streamable = streamable
    let mutable _doc = doc
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

    member this.Optional
        with get() = _optional
        and set(value) = _optional <- value

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
            this.Optional = other.Optional &&
            this.Label = other.Label &&
            this.SecondaryFiles = other.SecondaryFiles &&
            this.Streamable = other.Streamable &&
            this.Doc = other.Doc &&
            this.Format = other.Format &&
            this.LoadContents = other.LoadContents &&
            this.LoadListing = other.LoadListing &&
            this.DefaultValue = other.DefaultValue &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (
            this.Name,
            this.Type_,
            this.InputBinding,
            this.Optional,
            this.Label,
            this.SecondaryFiles,
            this.Streamable,
            this.Doc,
            this.Format,
            this.LoadContents,
            this.LoadListing,
            this.DefaultValue,
            HashHelpers.hashDynamicProperties this
        )

    static member KnownFieldNames =
        ResizeArray [| "id"; "type"; "label"; "secondaryFiles"; "streamable"; "doc"; "format"; "loadContents"; "loadListing"; "default"; "inputBinding" |]
