namespace ISADotNet.XLSX

open ISADotNet
open ISADotNet.API

module internal Option =
 
    /// If the value matches the default, a None is returned, else a Some is returned
    let fromValueWithDefault d v =
        if d = v then None
        else Some v

    /// Applies the function f on the value of the option if it exists, else applies it on the default value. If the result value matches the default, a None is returned
    let mapDefault (d : 'T) (f: 'T -> 'T) (o : 'T option) =
        match o with
        | Some v -> f v
        | None   -> f d
        |> fromValueWithDefault d


module OntologyAnnotation =

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
            Array.map3 OntologyAnnotation.fromString terms accessions sources
            |> Array.toList

    /// Returns the aggregated ISATab OntologyAnnotation Name, Accession number and ontology source from a list of ISAJson OntologyAnnotation objects
    let toAggregatedStrings (separator:char) (oas : OntologyAnnotation list) =
        if oas = [] then "","",""
        else
            oas
            |> List.map OntologyAnnotation.toString
            |> List.reduce (fun (terms, accessions, sources) (term, accession, source) -> 
                sprintf "%s%c%s" terms      separator term,
                sprintf "%s%c%s" accessions separator accession,
                sprintf "%s%c%s" sources    separator source
            ) 

module Component = 
        
    /// Returns a list of ISAJson Component objects from ISATab aggregated strings
    let fromAggregatedStrings (separator:char) (names:string) (terms:string) (accessions:string) (source:string) =
        let l = OntologyAnnotation.getLengthOfAggregatedStrings separator [|names;terms;accessions;source|]
        if l = 0 then []
        else 
            let names : string [] = if names = "" then Array.create l "" else names.Split(separator)
            let terms : string [] = if terms = "" then Array.create l "" else terms.Split(separator)
            let accessions : string [] = if accessions = "" then Array.create l "" else accessions.Split(separator)
            let sources : string [] = if source = "" then Array.create l "" else source.Split(separator)
            Array.map4 Component.fromString names terms accessions sources
            |> Array.toList

    /// Returns the aggregated ISATAb Component Name, Ontology Annotation value, Accession number and ontology source from a list of ISAJson Component objects
    let toAggregatedStrings (separator:char) (cs : Component list) =
        if cs = [] then "","","",""
        else
            cs
            |> List.map Component.toString
            |> List.reduce (fun (names,terms, accessions, sources) (name,term, accession, source) -> 
                sprintf "%s%c%s" names      separator name,
                sprintf "%s%c%s" terms      separator term,
                sprintf "%s%c%s" accessions separator accession,
                sprintf "%s%c%s" sources    separator source
            ) 

module ProtocolParameter =

    /// Returns a list of ISAJson ProtocolParameter objects from ISATab aggregated strings
    let fromAggregatedStrings (separator:char) (terms:string) (accessions:string) (source:string) =
        OntologyAnnotation.fromAggregatedStrings separator terms accessions source
        |> List.map (Some >> (ProtocolParameter.create None))

    /// Returns the aggregated ISATAb Ontology Annotation value, Accession number and ontology source from a list of ISAJson ProtocolParameter objects
    let toAggregatedStrings (separator:char) (oas : ProtocolParameter list) =
        if oas = [] then "","",""
        else
            oas
            |> List.map ProtocolParameter.toString
            |> List.reduce (fun (terms, accessions, sources) (term, accession, source) -> 
                sprintf "%s%c%s" terms      separator term,
                sprintf "%s%c%s" accessions separator accession,
                sprintf "%s%c%s" sources    separator source
            ) 

    