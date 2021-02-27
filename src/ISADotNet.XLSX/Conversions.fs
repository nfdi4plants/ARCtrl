namespace ISADotNet.XLSX

open ISADotNet
open ISADotNet.API

module URI =

    /// Create a ISAJson URI from a ISATab string entry
    let fromString (s : string) : URI=
        s

    /// Create a ISATab string URI from a ISAJson object
    let toString (s : URI) : string=
        s

module AnnotationValue =

    /// Create a ISAJson Annotation value from a ISATab string entry
    let fromString (s : string) = 
        try s |> int |> AnnotationValue.Int
        with | _ -> 
            try s |> float |> AnnotationValue.Float
            with
            | _ -> AnnotationValue.Text s

    /// Get a ISATab string Annotation Name from a ISAJson object
    let toString (v : AnnotationValue) = 
        match v with
        | Text s -> s
        | Int i -> string i
        | Float f -> string f

module OntologyAnnotation =

    /// Create a ISAJson Ontology Annotation value from ISATab string entries
    let fromString (term:string) (accession:string) (source:string) =
        OntologyAnnotation.create 
            None 
            (Option.fromValueWithDefault "" term |> Option.map AnnotationValue.fromString)
            (Option.fromValueWithDefault "" accession |> Option.map URI.fromString)
            (Option.fromValueWithDefault "" source)
            None

    /// Get a ISATab string entries from an ISAJson Ontology Annotation object
    let toString (oa : OntologyAnnotation) =
        oa.Name |> Option.map AnnotationValue.toString |> Option.defaultValue "",
        oa.TermAccessionNumber |> Option.map URI.toString |> Option.defaultValue "",
        oa.TermSourceREF |> Option.defaultValue ""

    /// Returns the length of a subpropertylist from the aggregated strings
    ///
    /// In ISATab format, some subproperties which are stored as lists in ISAJson are stored as semicolon delimited tables 
    /// 
    /// These strings should either contain the same number of semicolon delimited elements or be empty.
    let getLengthOfAggregatedStrings (separator:char) (strings: string []) =
        strings
        |> Array.fold (fun l s ->
            if s = "" then l
            elif l = 0 then s.Split(separator).Length
            else 
                let sl = s.Split(separator).Length
                if l = sl then l
                else failwithf "The length of the aggregated string %s does not match the length of the others" s
        ) 0

    /// Returns a list of ISAJson OntologyAnnotation objects from ISATab aggregated strings
    let fromAggregatedStrings (separator:char) (terms:string) (accessions:string) (source:string) : OntologyAnnotation list=
        let l = getLengthOfAggregatedStrings separator [|terms;accessions;source|]
        if l = 0 then []
        else 
            let terms : string [] = if terms = "" then Array.create l "" else terms.Split(separator)
            let accessions : string [] = if accessions = "" then Array.create l "" else accessions.Split(separator)
            let sources : string [] = if source = "" then Array.create l "" else source.Split(separator)
            Array.map3 fromString terms accessions sources
            |> Array.toList

    /// Returns the aggregated ISATab OntologyAnnotation Name, Accession number and ontology source from a list of ISAJson OntologyAnnotation objects
    let toAggregatedStrings (separator:char) (oas : OntologyAnnotation list) =
        if oas = [] then "","",""
        else
            oas
            |> List.map toString
            |> List.reduce (fun (terms, accessions, sources) (term, accession, source) -> 
                sprintf "%s%c%s" terms      separator term,
                sprintf "%s%c%s" accessions separator accession,
                sprintf "%s%c%s" sources    separator source
            ) 

module Component = 
    
    /// Create a ISAJson Component from ISATab string entries
    let fromString (name: string) (term:string) (accession:string) (source:string) =
        OntologyAnnotation.fromString term accession source
        |> Option.fromValueWithDefault OntologyAnnotation.empty
        |> Component.create (Option.fromValueWithDefault "" name)

    /// Get ISATab string entries from an ISAJson Component object
    let toString (c : Component) =
        let (n,t,a) = c.ComponentType |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")
        c.ComponentName |> Option.defaultValue "",n,t,a
        
    /// Returns a list of ISAJson Component objects from ISATab aggregated strings
    let fromAggregatedStrings (separator:char) (names:string) (terms:string) (accessions:string) (source:string) =
        let l = OntologyAnnotation.getLengthOfAggregatedStrings separator [|names;terms;accessions;source|]
        if l = 0 then []
        else 
            let names : string [] = if names = "" then Array.create l "" else names.Split(separator)
            let terms : string [] = if terms = "" then Array.create l "" else terms.Split(separator)
            let accessions : string [] = if accessions = "" then Array.create l "" else accessions.Split(separator)
            let sources : string [] = if source = "" then Array.create l "" else source.Split(separator)
            Array.map4 fromString names terms accessions sources
            |> Array.toList

    /// Returns the aggregated ISATAb Component Name, Ontology Annotation value, Accession number and ontology source from a list of ISAJson Component objects
    let toAggregatedStrings (separator:char) (cs : Component list) =
        if cs = [] then "","","",""
        else
            cs
            |> List.map toString
            |> List.reduce (fun (names,terms, accessions, sources) (name,term, accession, source) -> 
                sprintf "%s%c%s" names      separator name,
                sprintf "%s%c%s" terms      separator term,
                sprintf "%s%c%s" accessions separator accession,
                sprintf "%s%c%s" sources    separator source
            ) 

module ProtocolParameter =

    /// Create a ISAJson Protocol Parameter from ISATab string entries
    let fromString (term:string) (accession:string) (source:string) =
        OntologyAnnotation.fromString term accession source
        |> Option.fromValueWithDefault OntologyAnnotation.empty
        |> ProtocolParameter.create None

    /// Get ISATab string entries from an ISAJson ProtocolParameter object
    let toString (pp : ProtocolParameter) =
        pp.ParameterName |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")        

    /// Returns a list of ISAJson ProtocolParameter objects from ISATab aggregated strings
    let fromAggregatedStrings (separator:char) (terms:string) (accessions:string) (source:string) =
        OntologyAnnotation.fromAggregatedStrings separator terms accessions source
        |> List.map (Some >> (ProtocolParameter.create None))

    /// Returns the aggregated ISATAb Ontology Annotation value, Accession number and ontology source from a list of ISAJson ProtocolParameter objects
    let toAggregatedStrings (separator:char) (oas : ProtocolParameter list) =
        if oas = [] then "","",""
        else
            oas
            |> List.map toString
            |> List.reduce (fun (terms, accessions, sources) (term, accession, source) -> 
                sprintf "%s%c%s" terms      separator term,
                sprintf "%s%c%s" accessions separator accession,
                sprintf "%s%c%s" sources    separator source
            ) 


module Value =

    let fromOptions (value : string Option) (termAccesssion: string Option) (termSource: string Option) =
        match value, termSource, termAccesssion with
        | Some value, None, None ->
            match box value with
            | :? int as i -> Value.Int i
            | :? float as f -> Value.Float f
            | _ -> Value.Name value
            |> Some
        | None, None, None -> 
            None
        | _ -> 
            OntologyAnnotation.fromString (Option.defaultValue "" value) (Option.defaultValue "" termAccesssion) (Option.defaultValue "" termSource)
            |> Value.Ontology
            |> Some
