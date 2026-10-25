namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

/// The glob of an output binding, as a single pattern or as a list of patterns. A single pattern differs from a list
/// of one pattern, and the empty list stays an empty list, because CWL keeps the spellings apart. The lists compare by
/// their elements, because a ResizeArray compares references. A pattern is a string or an expression, and a directive
/// names a file with the text of the pattern.
[<RequireQualifiedAccess; CustomEquality; NoComparison; AttachMembers>]
type OutputGlob =
    | Single of SchemaSaladString
    | Multiple of ResizeArray<SchemaSaladString>
    with
    member this.AsValues() =
        match this with
        | Single value -> ResizeArray [| value |]
        | Multiple values -> values

    override this.Equals(o: obj) =
        match o with
        | :? OutputGlob as other ->
            match this, other with
            | Single left, Single right -> left = right
            | Multiple left, Multiple right -> HashHelpers.seqEqual left right
            | _ -> false
        | _ -> false

    override this.GetHashCode() =
        match this with
        | Single value -> HashHelpers.mergeHashes 1 (hash value)
        | Multiple values -> HashHelpers.mergeHashes 2 (HashHelpers.hashSeq values)

[<AttachMembers>]
type OutputBinding (?glob: OutputGlob, ?loadContents: SchemaSaladBool, ?loadListing: LoadListingEnum, ?outputEval: SchemaSaladString) =
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
        CWLKnownFieldNames.outputBinding

    static member create(?glob: OutputGlob, ?loadContents: SchemaSaladBool, ?loadListing: LoadListingEnum, ?outputEval: SchemaSaladString) =
        OutputBinding(?glob = glob, ?loadContents = loadContents, ?loadListing = loadListing, ?outputEval = outputEval)

/// The source of an output, as a single name or as a list of names. A name is a string, so a directive can stand in its
/// place. A single name differs from a list of one name, because CWL keeps the two spellings apart. The lists compare by
/// their elements, because a ResizeArray compares references.
[<CustomEquality; NoComparison; AttachMembers>]
type OutputSource =
    | Single of SchemaSaladString
    | Multiple of ResizeArray<SchemaSaladString>
    with
    member this.AsValues() =
        match this with
        | Single value -> ResizeArray [| value |]
        | Multiple values -> values

    override this.Equals(o: obj) =
        match o with
        | :? OutputSource as other ->
            match this, other with
            | Single left, Single right -> left = right
            | Multiple left, Multiple right -> HashHelpers.seqEqual left right
            | _ -> false
        | _ -> false

    override this.GetHashCode() =
        match this with
        | Single value -> HashHelpers.mergeHashes 1 (hash value)
        | Multiple values -> HashHelpers.mergeHashes 2 (HashHelpers.hashSeq values)

[<AttachMembers>]
type CWLOutput (
    name: string,
    ?type_: CWLType,
    ?outputBinding: OutputBinding,
    ?outputSource: OutputSource,
    ?label: SchemaSaladString,
    ?secondaryFiles: RawYaml,
    ?streamable: SchemaSaladBool,
    ?doc: DocText,
    ?format: SchemaSaladString
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
    let mutable _sourceNodes = SourceNodes.Empty
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

    /// The nodes that the doc and the label were read from. The encoder writes the layout of the author from them.
    /// They stay out of the equality and the hash of the object.
    member internal this.SourceNodes
        with get() = _sourceNodes
        and set(value) = _sourceNodes <- value

    member this.Format
        with get() = _format
        and set(value) = _format <- value

    /// Returns a new list of the sources that the document writes out as ids. The directives are left out, because they
    /// name files that the decoder does not read.
    member this.GetOutputSources() : ResizeArray<string> =
        match this.OutputSource with
        | Some outputSource -> outputSource.AsValues() |> Seq.choose SchemaSaladString.tryLiteral |> ResizeArray
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
        HashHelpers.hashFields [
            box this.Name
            box this.Type_
            box this.OutputBinding
            box this.OutputSource
            box this.Label
            box this.SecondaryFiles
            box this.Streamable
            box this.Doc
            box this.Format
            box (DynamicObjHelpers.hashDynamicProperties this)
        ]

    static member KnownFieldNames =
        CWLKnownFieldNames.cwlOutput
