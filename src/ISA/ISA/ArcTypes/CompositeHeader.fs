namespace ISA

open Fable.Core

[<AttachMembers>]
[<RequireQualifiedAccess>]
type IOType =
    | Source
    | Sample
    | RawDataFile
    | DerivedDataFile
    | ImageFile
    | Material
    // Do we need FreeText of string on this one too? Do we want to allow other IOTypes?
    // For now: yes
    | FreeText of string

    // This is used for example in Swate to programmatically create Options for adding building blocks.
    // Having this member here, guarantees the any new IOTypes are implemented in tools.
    /// This member is used to iterate over all existing `IOType`s (excluding FreeText case).
    static member All = [|
        Source
        Sample
        RawDataFile
        DerivedDataFile
        ImageFile
        Material
    |]

    member this.asInput = 
        let stringCreate x = $"Input [{x.ToString()}]"
        match this with
        | FreeText s -> stringCreate s
        | anyelse -> stringCreate anyelse
    member this.asOutput = 
        let stringCreate x = $"Output [{x.ToString()}]"
        match this with
        | FreeText s -> stringCreate s
        | anyelse -> stringCreate anyelse

    override this.ToString() =
        match this with
        | Source            -> "Source Name" 
        | Sample            -> "Sample Name" 
        | RawDataFile       -> "Raw Data File"
        | DerivedDataFile   -> "Derived Data File"
        | ImageFile         -> "Image File"
        | Material          -> "Material" 
        | FreeText s        -> s

    /// Used to match only(!) IOType string to IOType (without Input/Output). This matching is case sensitive.
    ///
    /// Exmp. 1: "Source" --> Source
    ///
    /// Exmp. 2: "Raw Data File" | "RawDataFile" -> RawDataFile
    static member ofString (str: string) =  
        match str with
        | "Source" | "Source Name"                  -> Source
        | "Sample" | "Sample Name"                  -> Sample
        | "RawDataFile" | "Raw Data File"           -> RawDataFile
        | "DerivedDataFile" | "Derived Data File"   -> DerivedDataFile
        | "ImageFile" | "Image File"                -> ImageFile
        | "Material"                                -> Material
        | _                                         -> FreeText str // use str to not store `str.ToLower()`

    /// Used to match Input/Output annotation table header to IOType
    ///
    /// Exmp. 1: "Input [Source]" --> Some Source
    static member tryOfHeaderString (str: string) =
        match Regex.tryParseIOTypeHeader str with
        | Some s -> IOType.ofString s |> Some
        | None -> None

/// <summary>
/// Model of the different types of Building Blocks in an ARC Annotation Table.
/// </summary>
[<AttachMembers>]
[<RequireQualifiedAccess>]
type CompositeHeader = 
    // term
    | Component         of OntologyAnnotation
    | Characteristic    of OntologyAnnotation
    | Factor            of OntologyAnnotation
    | Parameter         of OntologyAnnotation
    // featured
    | ProtocolType
    // single
    | ProtocolDescription
    | ProtocolUri
    | ProtocolVersion
    | ProtocolREF
    | Performer
    | Date
    // single - io type
    | Input of IOType
    | Output of IOType
    // single - fallback
    | FreeText of string

    with 

    override this.ToString() =
        match this with
        | Parameter oa          -> $"Parameter [{oa.NameText}]"
        | Factor oa             -> $"Factor [{oa.NameText}]"
        | Characteristic oa     -> $"Characteristic [{oa.NameText}]"
        | Component oa          -> $"Component [{oa.NameText}]"
        | ProtocolType          -> "Protocol Type" 
        | ProtocolREF           -> "Protocol REF"
        | ProtocolDescription   -> "Protocol Description"
        | ProtocolUri           -> "Protocol Uri"
        | ProtocolVersion       -> "Protocol Version"
        | Performer             -> "Performer"
        | Date                  -> "Date"
        | Input io              -> io.asInput
        | Output io             -> io.asOutput
        | FreeText str          -> str

    /// <summary>
    /// Tries to create a `CompositeHeader` from a given string.
    /// </summary>
    static member OfHeaderString (str: string) =
        match str.Trim() with
        // Input/Output have similiar naming as Term, but are more specific. 
        // So they have to be called first.
        | Regex.ActivePatterns.Regex Regex.Pattern.InputPattern r ->
            let iotype = r.Groups.["iotype"].Value
            Input <| IOType.ofString (iotype)
        | Regex.ActivePatterns.Regex Regex.Pattern.OutputPattern r ->
            let iotype = r.Groups.["iotype"].Value
            Output <| IOType.ofString (iotype)
        // Is term column
        | Regex.ActivePatterns.TermColumn r ->
            match r.TermColumnType with
            | "Parameter" 
            | "Parameter Value"             -> Parameter (OntologyAnnotation.fromString r.TermName)
            | "Factor" 
            | "Factor Value"                -> Factor (OntologyAnnotation.fromString r.TermName)
            | "Characteristic" 
            | "Characteristics"
            | "Characteristics Value"       -> Characteristic (OntologyAnnotation.fromString r.TermName)
            | "Component"                   -> Component (OntologyAnnotation.fromString r.TermName)
            // TODO: Is this what we intend?
            | _                             -> FreeText str
        | "Date"                    -> Date
        | "Performer"               -> Performer
        | "Protocol Description"    -> ProtocolDescription
        | "Protocol Uri"            -> ProtocolUri
        | "Protocol Version"        -> ProtocolVersion
        | "Protocol Type"           -> ProtocolType
        | "Protocol REF"            -> ProtocolREF
        | anyelse                   -> FreeText anyelse


    /// Returns true if column is deprecated
    member this.IsDeprecated = 
        match this with 
        | FreeText s when s.ToLower() = "sample name" -> true
        | FreeText s when s.ToLower() = "source name" -> true
        | FreeText s when s.ToLower() = "data file name" -> true
        | FreeText s when s.ToLower() = "derived data file" -> true
        | _ -> false   

    /// <summary>
    /// Is true if this Building Block type is a TermColumn.
    ///
    /// The name "TermColumn" refers to all columns with the syntax "Parameter/Factor/etc [TERM-NAME]" and featured columns
    /// such as Protocol Type as these are also represented as a triplet of MainColumn-TSR-TAN.
    /// </summary>
    member this.IsTermColumn =
        match this with 
        | Parameter _ | Factor _| Characteristic _| Component _
        | ProtocolType -> true 
        | anythingElse -> false

    /// <summary>
    /// Is true if the Building Block type is a FeaturedColumn. 
    ///
    /// A FeaturedColumn can be abstracted by Parameter/Factor/Characteristic and describes one common usecase of either.
    /// Such a block will contain TSR and TAN and can be used for directed Term search.
    /// </summary>
    member this.IsFeaturedColumn =
        match this with | ProtocolType -> true | anythingElse -> false

    /// <summary>
    /// This function sets the associated term accession for featured columns. 
    /// 
    /// It contains the hardcoded term accessions.
    /// </summary>
    member this.GetFeaturedColumnAccession =
        match this with
        | ProtocolType -> "DPBO:1000161"
        | anyelse -> failwith $"Tried matching {anyelse} in getFeaturedColumnAccession, but is not a featured column."

    /// <summary>
    /// Is true if the Building Block type is parsed to a single column. 
    ///
    /// This can be any input, output column, as well as for example: `ProtocolREF` and `Performer` with FreeText body cells.
    /// </summary>
    member this.IsSingleColumn =
        match this with 
        | FreeText _
        | Input _ | Output _ 
        | ProtocolREF | ProtocolDescription | ProtocolUri | ProtocolVersion | Performer | Date -> true 
        | anythingElse -> false

    ///
    member this.IsIOType =
        match this with 
        | Input io | Output io -> true 
        | anythingElse -> false

    // lower case "i" because of clashing naming: 
    // Issue: https://github.com/dotnet/fsharp/issues/10359
    // Proposed design: https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1079-union-properties-visible.md
    member this.isInput =
        match this with 
        | Input io -> true 
        | anythingElse -> false

    member this.isOutput =
        match this with 
        | Input io -> true 
        | anythingElse -> false