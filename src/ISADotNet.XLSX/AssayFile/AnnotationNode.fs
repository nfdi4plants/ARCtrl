namespace ISADotNet.XLSX.AssayFile

open ISADotNet
open ISADotNet.XLSX
open AnnotationColumn

module AnnotationNode = 
    
    /// Splits the headers of an annotation table into 
    let splitIntoNodes (headers : seq<string>) =
        headers
        |> Seq.groupWhen (fun header -> 
            match (AnnotationColumn.SwateHeader.fromStringHeader header).Kind with
            | "Unit"                    -> false
            | "Term Source REF"         -> false
            | "Term Accession Number"   -> false
            | _ -> true
        )

    let tryGetUnitGetterFunction (headers:string seq) =

        Seq.tryPick tryParseUnitHeader headers
        |> Option.map (fun h -> 
            let unitNameGetter matrix i = 
                Dictionary.tryGetValue (h.HeaderString,i) matrix       
            let termAccessionGetter =
                match Seq.tryPick (tryParseTermAccessionNumberHeader h) headers with
                | Some h ->
                    fun matrix i -> 
                        match Dictionary.tryGetValue (h.HeaderString,i) matrix with
                        | Some "user-specific" -> None
                        | Some v -> Some v
                        | _ -> None 
                | None -> fun _ _ -> None
            let termSourceGetter =
                match Seq.tryPick (tryParseTermSourceReferenceHeader h) headers with
                | Some h ->
                    fun matrix i -> 
                        match Dictionary.tryGetValue (h.HeaderString,i) matrix with
                        | Some "user-specific" -> None
                        | Some v -> Some v
                        | _ -> None 
                | None -> fun _ _ -> None
            fun (matrix : System.Collections.Generic.Dictionary<(string * int),string>) i ->
                OntologyAnnotation.create 
                    None
                    (unitNameGetter matrix i |> Option.map AnnotationValue.fromString)
                    (termAccessionGetter matrix i |> Option.map URI.fromString)
                    (termSourceGetter matrix i)
                    None
        )
    
    let tryGetParameterGetterFunction (headers:string seq) =
        Seq.tryPick tryParseParameterHeader headers
        |> Option.map (fun h -> 
            let unitGetter = tryGetUnitGetterFunction headers
                  
            let category1, termAccessionGetter =
                match Seq.tryPick (tryParseTermAccessionNumberHeader h) headers with
                | Some h ->
                    h.Term,
                    fun matrix i -> 
                        match Dictionary.tryGetValue (h.HeaderString,i) matrix with
                        | Some "user-specific" -> None
                        | Some v when v.Contains (Option.defaultValue "" h.Term.Value.TermAccessionNumber) -> None
                        | Some v -> Some v
                        | _ -> None 
                | None -> None, fun _ _ -> None
            let category2, termSourceGetter =
                match Seq.tryPick (tryParseTermSourceReferenceHeader h) headers with
                | Some h ->
                    h.Term,
                    fun matrix i -> 
                        match Dictionary.tryGetValue (h.HeaderString,i) matrix with
                        | Some "user-specific" -> None
                        | Some v when v = (Option.defaultValue "" h.Term.Value.TermSourceREF) -> None
                        | Some v -> Some v
                        | _ -> None 
                | None -> None, fun _ _ -> None
    
            let valueGetter = 
                fun matrix i ->
                    let v = 
                        match Dictionary.tryGetValue (h.HeaderString,i) matrix with
                        | Some "user-specific" -> None
                        | Some v -> Some v
                        | _ -> None 
                    Value.fromOptions 
                        v
                        (termAccessionGetter matrix i)
                        (termSourceGetter matrix i)
                    
            let category = mergeOntology category1 category2 |> Option.map (Some >> ProtocolParameter.create None)
            
            category,
            fun (matrix : System.Collections.Generic.Dictionary<(string * int),string>) i ->
                ProcessParameterValue.create 
                    category
                    (valueGetter matrix i)
                    (unitGetter |> Option.map (fun f -> f matrix i))
        )
    
    let tryGetFactorGetterFunction (headers:string seq) =
        Seq.tryPick tryParseFactorHeader headers
        |> Option.map (fun h -> 
            let unitGetter = tryGetUnitGetterFunction headers
                  
            let category1, termAccessionGetter =
                match Seq.tryPick (tryParseTermAccessionNumberHeader h) headers with
                | Some h ->
                    h.Term,
                    fun matrix i -> 
                        match Dictionary.tryGetValue (h.HeaderString,i) matrix with
                        | Some "user-specific" -> None
                        | Some v when v.Contains (Option.defaultValue "" h.Term.Value.TermAccessionNumber) -> None
                        | Some v -> Some v
                        | _ -> None 
                | None -> None, fun _ _ -> None
            let category2, termSourceGetter =
                match Seq.tryPick (tryParseTermSourceReferenceHeader h) headers with
                | Some h ->
                    h.Term,
                    fun matrix i -> 
                        match Dictionary.tryGetValue (h.HeaderString,i) matrix with
                        | Some "user-specific" -> None
                        | Some v when v = (Option.defaultValue "" h.Term.Value.TermSourceREF) -> None
                        | Some v -> Some v
                        | _ -> None 
                | None -> None, fun _ _ -> None
    
            let valueGetter = 
                fun matrix i ->
                    let v = 
                        match Dictionary.tryGetValue (h.HeaderString,i) matrix with
                        | Some "user-specific" -> None
                        | Some v -> Some v
                        | _ -> None 
                    Value.fromOptions 
                        v
                        (termAccessionGetter matrix i)
                        (termSourceGetter matrix i)
                    
            let factor = 
                mergeOntology category1 category2 
                |> Option.map (fun oa ->  Factor.create None (oa.Name |> Option.map AnnotationValue.toString) (Some oa) None)
            
            factor,
            fun (matrix : System.Collections.Generic.Dictionary<(string * int),string>) i ->
                FactorValue.create 
                    None
                    factor
                    (valueGetter matrix i)
                    (unitGetter |> Option.map (fun f -> f matrix i))
        )

    let tryGetCharacteristicGetterFunction (headers:string seq) =
        Seq.tryPick tryParseCharacteristicsHeader headers
        |> Option.map (fun h -> 
            let unitGetter = tryGetUnitGetterFunction headers
                  
            let category1, termAccessionGetter =
                match Seq.tryPick (tryParseTermAccessionNumberHeader h) headers with
                | Some h ->
                    h.Term,
                    fun matrix i -> 
                        match Dictionary.tryGetValue (h.HeaderString,i) matrix with
                        | Some "user-specific" -> None
                        | Some v when v.Contains (Option.defaultValue "" h.Term.Value.TermAccessionNumber) -> None
                        | Some v -> Some v
                        | _ -> None 
                | None -> None, fun _ _ -> None
            let category2, termSourceGetter =
                match Seq.tryPick (tryParseTermSourceReferenceHeader h) headers with
                | Some h ->
                    h.Term,
                    fun matrix i -> 
                        match Dictionary.tryGetValue (h.HeaderString,i) matrix with
                        | Some "user-specific" -> None
                        | Some v when v = (Option.defaultValue "" h.Term.Value.TermSourceREF) -> None
                        | Some v -> Some v
                        | _ -> None 
                | None -> None, fun _ _ -> None
    
            let valueGetter = 
                fun matrix i ->
                    let v = 
                        match Dictionary.tryGetValue (h.HeaderString,i) matrix with
                        | Some "user-specific" -> None
                        | Some v -> Some v
                        | _ -> None 
                    Value.fromOptions 
                        v
                        (termAccessionGetter matrix i)
                        (termSourceGetter matrix i)
                    
            let characteristic = mergeOntology category1 category2 |> Option.map (Some >> MaterialAttribute.create None)            
            
            characteristic,
            fun (matrix : System.Collections.Generic.Dictionary<(string * int),string>) i ->
                MaterialAttributeValue.create 
                    None
                    characteristic
                    (valueGetter matrix i)
                    (unitGetter |> Option.map (fun f -> f matrix i))
        )

    let tryGetSampleNameGetter (headers:string seq) =
        Seq.tryPick tryParseSampleName headers
        |> Option.map (fun h -> 
            fun (matrix : System.Collections.Generic.Dictionary<(string * int),string>) i ->
                Dictionary.tryGetValue (h.HeaderString,i) matrix
        )

    let tryGetSourceNameGetter (headers:string seq) =
        Seq.tryPick tryParseSourceName headers
        |> Option.map (fun h -> 
            fun (matrix : System.Collections.Generic.Dictionary<(string * int),string>) i ->
                Dictionary.tryGetValue (h.HeaderString,i) matrix
        )