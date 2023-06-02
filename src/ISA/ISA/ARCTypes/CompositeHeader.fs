namespace ISA

open Fable.Core

// TODO: Do we need FreeText of string on this one too? Do we want to allow other IOTypes?
type IOType =
    | Source
    | Sample
    | Data
    | RawData
    | ProcessedData
    | Material

    with

    member this.asInput = $"Input [{this}]"
    member this.asOutput = $"Output [{this}]"

    /// Used to match exact string to IOType
    ///
    /// Exmp. 1: "Source" --> Source
    static member ofString (str: string) =  
        match str with
        | "Source"          -> Source
        | "Sample"          -> Sample
        | "Data"            -> Data
        | "RawData"         -> RawData
        | "ProcessedData"   -> ProcessedData
        | "Material"        -> Material
        | anyelse           -> failwith $"Unable to parse '{anyelse}' to IOType."

    /// Used to match Input/Output annotation table header to IOType
    ///
    /// Exmp. 1: "Input [Source]" --> Some Source
    static member tryOfHeaderString (str: string) =
        match Regex.tryParseIOTypeHeader str with
        | Some s -> IOType.ofString s |> Some
        | None -> None

    /// Used to match Input/Output annotation table header to IOType
    ///
    /// Exmp. 1: "Input [Source]" --> Some Source
    static member ofHeaderString (str: string) = 
        match Regex.tryParseIOTypeHeader str |> Option.map (fun x -> IOType.ofString x) with
        | Some iotype -> iotype
        | None -> failwith $"Unable to parse '{str}' to existing IOType."

    // There might be a better implementation for this?
    static member headerIsIOType (str: string) = 
        match IOType.tryOfHeaderString str with
        | Some _ -> true
        | None -> false
        

/// <summary>
/// Model of the different types of Building Blocks in an ARC Annotation Table.
/// </summary>
[<AttachMembers>]
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
    | FreeText of string

    //| Input of IOType
    //| Output of IOType

    with 

    static member deprecatedIOHeaderWarning (s : string) = ""

    // This feels strange as the new input and output are no longer part of CompositeHeaders
    static member getDeprecationWarning headers = 
        headers
        |> List.choose (fun i ->
            match i with 
            | FreeText s when s = "Sample" -> Some <| CompositeHeader.deprecatedIOHeaderWarning s
            | _ -> None   
        )

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
    /// A FeaturedColumn can be abstracted by Parameter/Factor/Characteristics and describes one common usecase of either.
    /// Such a block will contain TSR and TAN and can be used for directed Term search.
    /// </summary>
    member this.IsFeaturedColumn =
        match this with | ProtocolType -> true | anythingElse -> false

    /// <summary>
    /// Is true if the Building Block type is parsed to a single column. 
    ///
    /// This can be any input, output column, as well as for example: `ProtocolREF` and `Performer` with FreeText body cells.
    /// </summary>
    member this.IsSingleColumn =
        match this with 
        //| Input _ | Output _ 
        | ProtocolREF | ProtocolDescription | ProtocolUri | ProtocolVersion | Performer | Date -> true 
        | anythingElse -> false

    /// <summary>
    /// This function sets the associated term accession for featured columns. 
    /// 
    /// It contains the hardcoded term accessions.
    /// </summary>
    member this.GetFeaturedColumnAccession =
        if this.IsFeaturedColumn then
            match this with
            | ProtocolType -> "DPBO:1000161"
            | anyelse -> failwith $"Tried matching {anyelse} in getFeaturedColumnAccession, but is not a featured column."
        else
            failwith $"'{this}' is not listed as featured column type! No referenced accession available."

    override this.ToString() =
        match this with
        | Parameter oa          -> $"Parameter [{oa.NameText}]"
        | Factor oa             -> $"Factor [{oa.NameText}]"
        | Characteristic oa     -> $"Characteristics [{oa.NameText}]"
        | Component oa          -> $"Component [{oa.NameText}]"
        | ProtocolType          -> "Protocol Type" 
        | ProtocolREF           -> "Protocol REF"
        | ProtocolDescription   -> "Protocol Description"
        | ProtocolUri           -> "Protocol Uri"
        | ProtocolVersion       -> "Protocol Version"
        | Performer             -> "Performer"
        | Date                  -> "Date"
        | FreeText str          -> str

    // Regex duplication must be solved first
    /// <summary>
    /// Tries to create a BuildingBlockType from a given string. Returns `None` if none match.
    /// </summary>
    //static member tryOfString str =
    //    match str with
    //    | "Parameter" | "Parameter Value"   -> Some Parameter
    //    | "Factor" | "Factor Value"         -> Some Factor
    //    // "Characteristics" deprecated in v0.6.0
    //    | "Characteristics" | "Characteristic" | "Characteristics Value" -> Some Characteristic
    //    | "Component"           -> Some Component
    //    | "Sample Name"         -> Some Sample         
    //    | "Data File Name"      -> Some Data
    //    | "Raw Data File"       -> Some RawDataFile
    //    | "Derived Data File"   -> Some DerivedDataFile
    //    | "Source Name"         -> Some Source
    //    | "Protocol Type"       -> Some ProtocolType
    //    | "Protocol REF"        -> Some ProtocolREF
    //    | anythingElse          -> Some <| Freetext anythingElse

    /// <summary>
    /// Creates a BuildingBlockType from a given string.
    /// </summary>
    //static member ofString str =
    //    BuildingBlockType.tryOfString str
    //    |> function Some bbt -> bbt | None -> failwith $"Error: Unable to parse '{str}' to BuildingBlockType!"
