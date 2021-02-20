namespace ISADotNet.XLSX.AssayFile

open FSharpSpreadsheetML
open ISADotNet
open ISADotNet.XLSX
open System.Text.RegularExpressions

module Dictionary =

    let tryGetValue k (dict:System.Collections.Generic.Dictionary<'K,'V>) = 
        let b,v = dict.TryGetValue(k)
        if b then Some v
        else None

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

module Column =

    type HeaderInfo =
        {
            HeaderString : string
            Kind : string
            Term : OntologyAnnotation option
            Number : int Option
        }

        static member create headerString kind term number =
            {
                HeaderString = headerString
                Kind = kind
                Term = term 
                Number = number       
            }

        static member fromStringHeader header =
        
            let nameRegex = Regex.Match(header,@"(?<= \[).*(?=\])")
            if nameRegex.Success then
                let kind = Regex.Match(header,@".*(?= \[)")
                let ontologySourceRegex = Regex.Match(header,@"(?<=#t)\S+:\d*")
                let numberRegex = Regex.Match(header,@"(?<= \(#)\d+")
                let number = if numberRegex.Success then Some (int numberRegex.Value) else None
                let termSource,termAccession = 
                    if ontologySourceRegex.Success then 
                        ontologySourceRegex.Value.Split ':'
                        |> fun o -> o.[0], o.[1]
                    else "", ""
                HeaderInfo.create header kind.Value (Some (OntologyAnnotation.fromString nameRegex.Value termSource termAccession)) number
            else
                HeaderInfo.create header header None None
        

    let mergeOntology (termSourceHeaderOntology : OntologyAnnotation Option) (termAccessionHeaderOntology : OntologyAnnotation Option) =
        match termSourceHeaderOntology, termAccessionHeaderOntology with
        | Some oa1, Some oa2 -> API.Update.UpdateAll.updateRecordType oa1 oa2 |> Some
        | Some oa, None -> Some oa
        | None, Some oa -> Some oa
        | None, None -> None

    
    let tryParseTermSourceReferenceHeader (termHeader:HeaderInfo) (header:string) =
        match HeaderInfo.fromStringHeader header with
        | h when h.Kind = "Term Source REF" && h.Number = termHeader.Number -> 
            match h.Term,termHeader.Term with
            | None, None -> Some h
            | Some t1, Some t2 when t1.Name = t2.Name -> Some h
            | _ -> None
        | _ -> None
    
    let tryParseTermAccessionNumberHeader (termHeader:HeaderInfo) (header:string) =
        match HeaderInfo.fromStringHeader header with
        | h when h.Kind = "Term Accession Number"  && h.Number = termHeader.Number -> 
            match h.Term,termHeader.Term with
            | None, None -> Some h
            | Some t1, Some t2 when t1.Name = t2.Name -> Some h
            | _ -> None
        | _ -> None
    
    let tryParseParameterHeader (header:string) =
        match HeaderInfo.fromStringHeader header with
        | h when h.Kind = "Parameter" ->
            Some h
        | _ -> None
    
    let tryParseUnitHeader (header:string) =
        match HeaderInfo.fromStringHeader header with
        | h when h.Kind = "Unit" ->
            Some h
        | _ -> None
    
    
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
    
            fun (matrix : System.Collections.Generic.Dictionary<(string * int),string>) i ->
                ProcessParameterValue.create 
                    category
                    (valueGetter matrix i)
                    (unitGetter |> Option.map (fun f -> f matrix i))
        )
    