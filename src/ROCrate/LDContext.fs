namespace ARCtrl.ROCrate

open System.Collections.Generic

module Dictionary = 

    let ofSeq (s : seq<'Key*'T>) = 
        let dict = Dictionary()
        s
        |> Seq.iter dict.Add
        dict

    let tryFind (key : 'Key) (dict : Dictionary<'Key,'T>) =
        let b,v = dict.TryGetValue key
        if b then Some v 
        else None


// Add second dictionary which maps from definition to term?
// Make LDContext to be nested hierarchical tree? Like this you can iterate through the tree and stop at the first match, kind of like a shadowing mechanism
type LDContext(?mappings : Dictionary<string,string>, ?baseContexts : ResizeArray<LDContext>) =

    let mutable baseContexts = Option.defaultValue (ResizeArray []) baseContexts

    let mappings : Dictionary<string,string> =
        match mappings with
        | Some m -> m
        | None -> Dictionary()

    let reverseMappings : Dictionary<string,string> =
        let dict = Dictionary()
        for kvp in mappings do
            dict.Add(kvp.Value,kvp.Key)
        dict

    let tryFindTerm (term : string) =
        match Dictionary.tryFind term mappings with
        | Some v -> Some v
        | None ->
            baseContexts
            |> Seq.tryPick (fun ctx -> ctx.TryResolveTerm term)

    let tryFindIri (iri : string) =
        match Dictionary.tryFind iri reverseMappings with
        | Some v -> Some v
        | None -> 
            baseContexts
            |> Seq.tryPick (fun ctx -> ctx.TryGetTerm iri)

    let tryCompactIRI (iri : string) =
        failwith "TryCompactIRI is Not implemented yet"

    member this.BaseContexts
        with get() = baseContexts
        and internal set(value) = baseContexts <- value

    member this.AddMapping(term,definition) =
        if mappings.ContainsKey(term) then
            mappings.[term] <- definition
            reverseMappings.[definition] <- term
        else
            mappings.Add(term,definition)
            reverseMappings.Add(definition,term)
        
    member this.TryResolveTerm(term : string) =
        // Handle compact IRI
        if term.Contains(":") then
            term.Split(':')
            |> Seq.map tryFindTerm
            |> Seq.reduce (fun acc x ->
                match acc,x with
                | Some v1, Some v2 ->
                    Some (v1 + "/" + v2)
                | _ -> None)
        else
            tryFindTerm term

    member this.TryGetTerm(iri : string) =
        tryFindIri iri

    static member fromMappingSeq(mappings : seq<string*string>) =
        LDContext(Dictionary.ofSeq mappings)

    // Append second context to the first one inplace
    static member combine_InPlace (first : LDContext) (second : LDContext) : LDContext =
        first.BaseContexts.Add second
        first

    // Create new context with the the two given contexts as baseContexts
    static member combine (first : LDContext) (second : LDContext) : LDContext =
        LDContext(baseContexts = ResizeArray([first;second]))

    static member tryCombineOptional (first : LDContext option) (second : LDContext option) : LDContext option =
        match first,second with
        | Some f, Some s -> Some (LDContext.combine f s)
        | Some f, None -> Some f
        | None, Some s -> Some s
        | _ -> None