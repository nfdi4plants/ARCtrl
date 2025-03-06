namespace ARCtrl

open Fable.Core
open Fable.Core.JsInterop
open ARCtrl.Helper

[<AttachMembers>]
[<RequireQualifiedAccess>]
type IOType =
    | Source
    | Sample
    | Data
    | Material
    | FreeText of string

    // This is used for example in Swate to programmatically create Options for adding building blocks.
    // Having this member here, guarantees the any new IOTypes are implemented in tools.
    /// This member is used to iterate over all existing `IOType`s (excluding FreeText case).
    static member All = [|
        Source
        Sample
        Data
        Material
    |]

    static member Cases = 
        Microsoft.FSharp.Reflection.FSharpType.GetUnionCases(typeof<IOType>) 
        |> Array.map (fun x -> x.Tag, x.Name)

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

    /// Given two IOTypes, tries to return the one with a higher specificity. If both are equally specific, fail.
    ///
    /// E.g. RawDataFile is more specific than Source, but less specific than DerivedDataFile.
    ///
    /// E.g. Sample is equally specific to RawDataFile.
    member this.Merge(other) = 
        match this, other with
        | FreeText s1, FreeText s2 when s1 = s2 -> FreeText (s1)
        | FreeText s1, FreeText s2 -> failwith $"FreeText IO column names {s1} and {s2} do differ"
        | FreeText s, _ -> failwith $"FreeText IO column and {other} can not be merged"
        | Data, Source -> Data
        | Data, _ -> failwith $"Data IO column and {other} can not be merged"
        | Sample, Source -> Sample
        | Sample, Sample -> Sample
        | Sample, _ -> failwith $"Sample IO column and {other} can not be merged"
        | Source, Source -> Source
        | Source, _ -> other
        | Material, Source -> Material
        | Material, Material -> Material
        | Material, _ -> failwith $"Material IO column and {other} can not be merged"
        

    override this.ToString() =
        match this with
        | Source     -> "Source Name" 
        | Sample     -> "Sample Name" 
        | Data       -> "Data"
        | Material   -> "Material" 
        | FreeText s -> s

    /// Used to match only(!) IOType string to IOType (without Input/Output). This matching is case sensitive.
    ///
    /// Exmp. 1: "Source" --> Source
    ///
    /// Exmp. 2: "Raw Data File" | "RawDataFile" -> RawDataFile
    static member ofString (str: string) =  
        match str with
        | "Source" | "Source Name"                  -> Source
        | "Sample" | "Sample Name"                  -> Sample
        | "RawDataFile" | "Raw Data File"           
        | "DerivedDataFile" | "Derived Data File"  
        | "ImageFile" | "Image File"                
        | "Data"                                    -> Data
        | "Material"                                -> Material
        | _                                         -> FreeText str // use str to not store `str.ToLower()`

    /// Used to match Input/Output annotation table header to IOType
    ///
    /// Exmp. 1: "Input [Source]" --> Some Source
    static member tryOfHeaderString (str: string) =
        match Regex.tryParseIOTypeHeader str with
        | Some s -> IOType.ofString s |> Some
        | None -> None

#if FABLE_COMPILER

    static member source() = IOType.Source

    static member sample() = IOType.Sample

    static member data() = IOType.Data

    static member material() = IOType.Material

    static member freeText(s:string) = IOType.FreeText s

#else
#endif

/// <summary>
/// Model of the different types of Building Blocks in an ARC Annotation Table.
/// </summary>
[<AttachMembers>]
[<RequireQualifiedAccess>]
[<StructuralComparison; StructuralEquality>]
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
    | Comment of string

    with 

    static member Cases = 
        Microsoft.FSharp.Reflection.FSharpType.GetUnionCases(typeof<CompositeHeader>) 
        |> Array.map (fun x -> x.Tag, x.Name)

    /// <summary>
    /// This function is used to programmatically create `CompositeHeaders` in JavaScript. Returns integer code representative of input type.
    ///
    /// 0: Expects no input
    ///
    /// 1: Expects OntologyAnnotation as input
    ///
    /// 2: Expects IOType as input
    ///
    /// 3: Expects string as input
    /// </summary>
    /// <param name="inp">Can be accessed from `CompositeHeader.Cases`</param>
    static member jsGetColumnMetaType(inp:int) =
        match inp with
        // no input
        | 4 | 5 | 6 | 7 | 8 | 9 | 10 -> 0
        // OntologyAnnotation as input
        | 0 | 1 | 2 | 3 -> 1
        // iotype as input
        | 11 | 12 -> 2
        // string as input
        | 13 | 14 -> 3
        | anyElse -> failwithf "Cannot assign input `Tag` (%i) to `CompositeHeader`" anyElse

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
        | Comment key           -> $"Comment [{key}]"
        | FreeText str          -> str

    /// If the column is a term column, returns the term as `OntologyAnnotation`. Otherwise returns an `OntologyAnnotation` with only the name.
    member this.ToTerm() =
        match this with
        | Parameter oa          -> oa
        | Factor oa             -> oa
        | Characteristic oa     -> oa
        | Component oa          -> oa
        | ProtocolType          -> OntologyAnnotation.create(this.ToString(), tan=this.GetFeaturedColumnAccession) 
        | ProtocolREF           -> OntologyAnnotation.create (this.ToString())  // use owl ontology in the future
        | ProtocolDescription   -> OntologyAnnotation.create (this.ToString())  // use owl ontology in the future
        | ProtocolUri           -> OntologyAnnotation.create (this.ToString())  // use owl ontology in the future
        | ProtocolVersion       -> OntologyAnnotation.create (this.ToString())  // use owl ontology in the future
        | Performer             -> OntologyAnnotation.create (this.ToString())  // use owl ontology in the future
        | Date                  -> OntologyAnnotation.create (this.ToString())  // use owl ontology in the future
        | Input _               -> OntologyAnnotation.create (this.ToString())  // use owl ontology in the future
        | Output _              -> OntologyAnnotation.create (this.ToString())  // use owl ontology in the future
        | Comment _            -> OntologyAnnotation.create (this.ToString())  // use owl ontology in the future    
        | FreeText _            -> OntologyAnnotation.create (this.ToString())  // use owl ontology in the future
        // owl ontology: https://github.com/nfdi4plants/ARC_ontology/blob/main/ARC_v2.0.owl

    /// <summary>
    /// Tries to create a `CompositeHeader` from a given string.
    /// </summary>
    static member OfHeaderString (str: string) =
        match str.Trim() with
        // Input/Output have similiar naming as Term, but are more specific. 
        // So they have to be called first.
        | Regex.ActivePatterns.Regex Regex.Pattern.InputPattern r ->
            let iotype = r.Groups.[Regex.Pattern.MatchGroups.iotype].Value
            Input <| IOType.ofString (iotype)
        | Regex.ActivePatterns.Regex Regex.Pattern.OutputPattern r ->
            let iotype = r.Groups.[Regex.Pattern.MatchGroups.iotype].Value
            Output <| IOType.ofString (iotype)
        | Regex.ActivePatterns.Regex Regex.Pattern.CommentPattern r ->
            Comment r.Groups.[Regex.Pattern.MatchGroups.commentKey].Value
        // Is term column
        | Regex.ActivePatterns.TermColumn r ->
            match r.TermColumnType with
            | "Parameter" 
            | "Parameter Value"             -> Parameter (OntologyAnnotation.create r.TermName)
            | "Factor" 
            | "Factor Value"                -> Factor (OntologyAnnotation.create r.TermName)
            | "Characteristic" 
            | "Characteristics"
            | "Characteristics Value"       -> Characteristic (OntologyAnnotation.create r.TermName)
            | "Component"                   -> Component (OntologyAnnotation.create r.TermName)
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
    /// Is true if this Building Block type is a CvParamColumn.
    ///
    /// The name "CvParamColumn" refers to all columns with the syntax "Parameter/Factor/etc [TERM-NAME]".
    ///
    /// Does return false for featured columns such as Protocol Type.
    /// </summary>
    member this.IsCvParamColumn =
        match this with 
        | Parameter _ | Factor _| Characteristic _| Component _ -> true
        | anythingElse -> false

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

    member this.IsDataColumn =
        match this with 
        | Input IOType.Data | Output IOType.Data -> true 
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
    /// This function gets the associated term accession for featured columns. 
    /// 
    /// It contains the hardcoded term accessions.
    /// </summary>
    member this.GetFeaturedColumnAccession =
        match this with
        | ProtocolType -> "DPBO:1000161"
        | anyelse -> failwith $"Tried matching {anyelse} in getFeaturedColumnAccession, but is not a featured column."

    /// <summary>
    /// This function gets the associated term accession for term columns. 
    /// </summary>
    member this.GetColumnAccessionShort =
        match this with
        | ProtocolType -> this.GetFeaturedColumnAccession
        | Parameter oa -> oa.TermAccessionShort
        | Factor oa -> oa.TermAccessionShort
        | Characteristic oa -> oa.TermAccessionShort
        | Component oa -> oa.TermAccessionShort
        | anyelse -> failwith $"Tried matching {anyelse}, but is not a column with an accession."

    /// <summary>
    /// Is true if the Building Block type is parsed to a single column. 
    ///
    /// This can be any input, output column, as well as for example: `ProtocolREF` and `Performer` with FreeText body cells.
    /// </summary>
    member this.IsSingleColumn =
        match this with 
        | FreeText _
        | Input _ | Output _ 
        | Comment _ 
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
        | Output io -> true 
        | anythingElse -> false

    member this.isParameter =
        match this with 
        | Parameter _ -> true 
        | anythingElse -> false

    member this.isFactor =
        match this with 
        | Factor _ -> true 
        | anythingElse -> false

    member this.isCharacteristic =
        match this with 
        | Characteristic _ -> true 
        | anythingElse -> false

    member this.isComponent =
        match this with
        | Component _ -> true
        | anythingElse -> false

    member this.isProtocolType =
        match this with
        | ProtocolType -> true
        | anythingElse -> false

    member this.isProtocolREF =
        match this with
        | ProtocolREF -> true
        | anythingElse -> false

    member this.isProtocolDescription =
        match this with
        | ProtocolDescription -> true
        | anythingElse -> false

    member this.isProtocolUri =
        match this with
        | ProtocolUri -> true
        | anythingElse -> false

    member this.isProtocolVersion =
        match this with
        | ProtocolVersion -> true
        | anythingElse -> false

    member this.isProtocolColumn =
        match this with
        | ProtocolREF | ProtocolDescription | ProtocolUri | ProtocolVersion | ProtocolType -> true
        | anythingElse -> false

    member this.isPerformer =
        match this with
        | Performer -> true
        | anythingElse -> false

    member this.isDate =
        match this with
        | Date -> true
        | anythingElse -> false

    member this.isComment =
        match this with
        | Comment _ -> true
        | anythingElse -> false

    member this.isFreeText =
        match this with
        | FreeText _ -> true
        | anythingElse -> false

    member this.TryInput() =
        match this with
        | Input io -> Some io
        | _ -> None

    member this.TryOutput() =
        match this with
        | Output io -> Some io
        | _ -> None

    member this.TryIOType() =
        match this with
        | Output io | Input io -> Some io
        | _ -> None

    member this.IsUnique =
        match this with
        | ProtocolType | ProtocolREF | ProtocolDescription | ProtocolUri | ProtocolVersion | Performer | Date | Input _ | Output _ -> true
        | _ -> false

    member this.Copy() =
        match this with
        | Parameter oa -> Parameter (oa.Copy())
        | Factor oa -> Factor (oa.Copy())
        | Characteristic oa -> Characteristic (oa.Copy())
        | Component oa -> Component (oa.Copy())
        | _ -> this

    /// <summary>
    /// Returns the term of the column if it is a term column. Otherwise returns None.
    ///
    /// Term columns are Parameter, Factor, Characteristic and Component.
    /// </summary>
    member this.TryGetTerm() =
        match this with
        | Parameter oa -> Some oa
        | Factor oa -> Some oa
        | Characteristic oa -> Some oa
        | Component oa -> Some oa
        | _ -> None

#if FABLE_COMPILER
    
    [<CompiledName("component")>]
    static member _component(oa:OntologyAnnotation) = Component oa

    static member characteristic(oa:OntologyAnnotation) = Characteristic oa

    static member factor(oa:OntologyAnnotation) = Factor oa

    static member parameter(oa:OntologyAnnotation) = Parameter oa

    static member protocolType() = ProtocolType

    static member protocolDescription() = ProtocolDescription

    static member protocolUri() = ProtocolUri

    static member protocolVersion() = ProtocolVersion

    static member protocolREF() = ProtocolREF

    static member performer() = Performer

    static member date() = Date

    static member input(io:IOType) = Input io

    static member output(io:IOType) = Output io

    static member freeText(s:string) = FreeText s

    static member comment(s:string) = Comment s

#endif
