namespace ISADotNet.XLSX

open ISADotNet

module URI =

    let fromString (s : string) : URI=
        s

    let toString (s : URI) : string=
        s

module AnnotationValue =

    let fromString (s : string) = 
        try s |> int |> AnnotationValue.Int
        with | _ -> 
            try s |> float |> AnnotationValue.Float
            with
            | _ -> AnnotationValue.Text s

    let toString (v : AnnotationValue) = 
        match v with
        | Text s -> s
        | Int i -> string i
        | Float f -> string f

module OntologyAnnotation =

    let fromString (term:string) (accession:string) (source:string) =
        OntologyAnnotation.create null (AnnotationValue.fromString term) (URI.fromString accession) source []

    let toString (oa : OntologyAnnotation) =
        oa.Name |> AnnotationValue.toString,
        oa.TermAccessionNumber |> URI.toString,
        oa.TermSourceREF

    let fromAggregatedStrings (separator:char) (terms:string) (accessions:string) (source:string) =
        (terms.Split separator, accessions.Split separator, source.Split separator)
        |||> Array.map3 fromString 
        |> List.ofArray

    let toAggregatedStrings (separator:char) (oas : OntologyAnnotation list) =
        oas
        |> List.map toString
        |> List.reduce (fun (terms, accessions, sources) (term, accession, source) -> 
            sprintf "%s%c%s" terms      separator term,
            sprintf "%s%c%s" accessions separator accession,
            sprintf "%s%c%s" sources    separator source
        ) 

module Component = 
    
    let fromString (name: string) (term:string) (accession:string) (source:string) =
        OntologyAnnotation.fromString term accession source
        |> Component.create name

    let toString (c : Component) =
        let (n,t,a) = c.ComponentType |> OntologyAnnotation.toString
        c.ComponentName,n,t,a
        

    let fromAggregatedStrings (separator:char) (names:string) (terms:string) (accessions:string) (source:string) =
        
        (names.Split separator,terms.Split separator, accessions.Split separator, source.Split separator)
        |> fun (n,t,a,s) -> Array.map4 fromString n t a s
        |> List.ofArray

    let toAggregatedStrings (separator:char) (cs : Component list) =
        cs
        |> List.map toString
        |> List.reduce (fun (names,terms, accessions, sources) (name,term, accession, source) -> 
            sprintf "%s%c%s" names      separator name,
            sprintf "%s%c%s" terms      separator term,
            sprintf "%s%c%s" accessions separator accession,
            sprintf "%s%c%s" sources    separator source
        ) 

module ProtocolParameter =

    let fromString (term:string) (accession:string) (source:string) =
        OntologyAnnotation.create null (AnnotationValue.fromString term) (URI.fromString accession) source []
        |> ProtocolParameter.create null

    let toString (pp : ProtocolParameter) =
        let oa = pp.ParameterName
        oa.Name |> AnnotationValue.toString,
        oa.TermAccessionNumber |> URI.toString,
        oa.TermSourceREF

    let fromAggregatedStrings (separator:char) (terms:string) (accessions:string) (source:string) =
        (terms.Split separator, accessions.Split separator, source.Split separator)
        |||> Array.map3 fromString 
        |> List.ofArray

    let toAggregatedStrings (separator:char) (oas : ProtocolParameter list) =
        oas
        |> List.map toString
        |> List.reduce (fun (terms, accessions, sources) (term, accession, source) -> 
            sprintf "%s%c%s" terms      separator term,
            sprintf "%s%c%s" accessions separator accession,
            sprintf "%s%c%s" sources    separator source
        ) 