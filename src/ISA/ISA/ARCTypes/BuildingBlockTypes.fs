namespace ISA

open Fable.Core

/// <summary>
/// Model of the different types of Building Blocks in an ARC Annotation Table.
/// </summary>
[<AttachMembers>]
type BuildingBlockType =
    // Term columns
    | Parameter
    | Factor
    | Characteristic
    | Component
    // Source columns
    | Source
    // Output columns
    | Sample
    | Data // DEPRECATED at v0.6.0 [<ObsoleteAttribute>] 
    | RawDataFile
    | DerivedDataFile
    // Featured Columns
    | ProtocolType
    // Single Columns
    | ProtocolREF
    // everything else
    | Freetext of string

    /// <summary>
    /// Returns an array of all existing building block types except `Freetext of string` in no particular order.
    /// </summary>
    static member All = [|
            Parameter; Factor; Characteristic; Component
            //input
            Source;
            //output
            Sample; RawDataFile; DerivedDataFile;
            Data // deprecated
            // Featured
            ProtocolType
            // Single
            ProtocolREF
    |]

    /// <summary>
    /// Is true if this Building Block type is an InputColumn.
    /// </summary>
    member this.IsInputColumn =
        match this with | Source -> true | anythingElse -> false

    /// <summary>
    /// Is true if this Building Block type is an OutputColumn.
    /// </summary>
    member this.IsOutputColumn =
        match this with | Data | Sample | RawDataFile | DerivedDataFile -> true | anythingElse -> false

    /// <summary>
    /// Is true if this Building Block type is a TermColumn.
    ///
    /// The name "TermColumn" refers to all columns with the syntax "Parameter/Factor/etc [TERM-NAME]" and featured columns
    /// such as Protocol Type as these are also represented as a triplet of MainColumn-TSR-TAN.
    /// </summary>
    member this.IsTermColumn =
        match this with | Parameter | Factor | Characteristic | Component | ProtocolType -> true | anythingElse -> false

    /// <summary>
    /// Is true if the Building Block type is a FeaturedColumn. 
    ///
    /// A FeaturedColumn can be abstracted by Parameter/Factor/Characteristics and describes one common usecase of either.
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
        if this.IsFeaturedColumn then
            match this with
            | ProtocolType -> "DPBO:1000161"
            | anyelse -> failwith $"Tried matching {anyelse} in getFeaturedColumnAccession, but is not a featured column."
        else
            failwith $"'{this}' is not listed as featured column type! No referenced accession available."

    /// <summary>
    /// Is true if the Building Block type is deprecated and should not be used anymore.
    /// </summary>
    member this.IsDeprecated =
        match this with | Data -> true | anythingElse -> false

    override this.ToString() =
        match this with
        | Parameter         -> "Parameter"
        | Factor            -> "Factor"
        | Characteristic    -> "Characteristics"
        | Component         -> "Component"
        | Sample            -> "Sample Name"
        | Data              -> "Data File Name"
        | RawDataFile       -> "Raw Data File"
        | DerivedDataFile   -> "Derived Data File"
        | ProtocolType      -> "Protocol Type" 
        | Source            -> "Source Name"
        | ProtocolREF       -> "Protocol REF"
        | Freetext str      -> str

    /// <summary>
    /// Tries to create a BuildingBlockType from a given string. Returns `None` if none match.
    /// </summary>
    static member tryOfString str =
        match str with
        | "Parameter" | "Parameter Value"   -> Some Parameter
        | "Factor" | "Factor Value"         -> Some Factor
        // "Characteristics" deprecated in v0.6.0
        | "Characteristics" | "Characteristic" | "Characteristics Value" -> Some Characteristic
        | "Component"           -> Some Component
        | "Sample Name"         -> Some Sample         
        | "Data File Name"      -> Some Data
        | "Raw Data File"       -> Some RawDataFile
        | "Derived Data File"   -> Some DerivedDataFile
        | "Source Name"         -> Some Source
        | "Protocol Type"       -> Some ProtocolType
        | "Protocol REF"        -> Some ProtocolREF
        | anythingElse          -> Some <| Freetext anythingElse

    /// <summary>
    /// Creates a BuildingBlockType from a given string.
    /// </summary>
    static member ofString str =
        BuildingBlockType.tryOfString str
        |> function Some bbt -> bbt | None -> failwith $"Error: Unable to parse '{str}' to BuildingBlockType!"


type IOType =
    | Source
    | Sample
    | Data
    | RawData
    | ProcessedData
    | Material