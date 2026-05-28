namespace ARCtrl.CWL

open DynamicObj
open Fable.Core
open YAMLicious.YAMLiciousTypes

[<AttachMembers>]
type OutputBinding (?glob: string, ?loadContents: bool, ?loadListing: LoadListingEnum, ?outputEval: string) =
    inherit DynamicObj ()

    let mutable _glob = glob
    let mutable _loadContents = loadContents
    let mutable _loadListing = loadListing
    let mutable _outputEval = outputEval

    member this.Glob
        with get() = _glob
        and set(value) = _glob <- value

    member this.LoadContents
        with get() = _loadContents
        and set(value) = _loadContents <- value

    member this.LoadListing
        with get() = _loadListing
        and set(value) = _loadListing <- value

    member this.OutputEval
        with get() = _outputEval
        and set(value) = _outputEval <- value

    override this.Equals(o: obj) =
        match o with
        | :? OutputBinding as other ->
            this.Glob = other.Glob &&
            this.LoadContents = other.LoadContents &&
            this.LoadListing = other.LoadListing &&
            this.OutputEval = other.OutputEval &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.Glob, this.LoadContents, this.LoadListing, this.OutputEval, DynamicObjHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        Set [| "loadContents"; "loadListing"; "glob"; "outputEval" |]

    static member create(?glob: string, ?loadContents: bool, ?loadListing: LoadListingEnum, ?outputEval: string) =
        OutputBinding(?glob = glob, ?loadContents = loadContents, ?loadListing = loadListing, ?outputEval = outputEval)

[<AttachMembers>]
type OutputSource =
    | Single of string
    | Multiple of ResizeArray<string>
    with
    member this.AsValues() =
        match this with
        | Single value -> ResizeArray [| value |]
        | Multiple values -> values

[<AttachMembers>]
type CWLOutput (
    name: string,
    ?type_: CWLType,
    ?outputBinding: OutputBinding,
    ?outputSource: OutputSource,
    ?label: string,
    ?secondaryFiles: YAMLElement,
    ?streamable: bool,
    ?doc: string,
    ?format: string
) =
    inherit DynamicObj ()

    let normalizeOutputSource = function
        | Some (Multiple values) when values.Count = 0 -> None
        | value -> value

    let mutable _name = name
    let mutable _type = type_
    let mutable _outputBinding = outputBinding
    let mutable _outputSource = normalizeOutputSource outputSource
    let mutable _label = label
    let mutable _secondaryFiles = secondaryFiles
    let mutable _streamable = streamable
    let mutable _doc = doc
    let mutable _format = format

    member this.Name
        with get() = _name
        and set(value) = _name <- value

    member this.Type_
        with get() = _type
        and set(value) = _type <- value

    member this.OutputBinding
        with get() = _outputBinding
        and set(value) = _outputBinding <- value

    member this.OutputSource
        with get() = _outputSource
        and set(value) =
            _outputSource <- normalizeOutputSource value

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

    member this.GetOutputSources() =
        match this.OutputSource with
        | Some outputSource -> outputSource.AsValues()
        | _ ->
            ResizeArray()

    override this.Equals(o: obj) =
        match o with
        | :? CWLOutput as other ->
            this.Name = other.Name &&
            this.Type_ = other.Type_ &&
            this.OutputBinding = other.OutputBinding &&
            this.OutputSource = other.OutputSource &&
            this.Label = other.Label &&
            this.SecondaryFiles = other.SecondaryFiles &&
            this.Streamable = other.Streamable &&
            this.Doc = other.Doc &&
            this.Format = other.Format &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (
            this.Name,
            this.Type_,
            this.OutputBinding,
            this.OutputSource,
            this.Label,
            this.SecondaryFiles,
            this.Streamable,
            this.Doc,
            this.Format,
            DynamicObjHelpers.hashDynamicProperties this
        )

    static member KnownFieldNames =
        Set [| "id"; "type"; "label"; "secondaryFiles"; "streamable"; "doc"; "format"; "outputBinding"; "outputSource" |]
