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
type LDContext(?mappings : Dictionary<string,string>, ?baseContext : LDContext) =

    let mutable baseContext = baseContext

    let mappings : Dictionary<string,string> =
        match mappings with
        | Some m -> m
        | None -> Dictionary()

    let tryFindTerm (term : string) =
        match Dictionary.tryFind term mappings with
        | Some v -> Some v
        | None -> 
            match baseContext with
            | Some ctx -> ctx.TryResolveTerm term
            | None -> None

    member this.BaseContext
        with get() = baseContext
        and internal set(value) = baseContext <- value

    member this.AddMapping(term,definition) =
        if mappings.ContainsKey(term) then
            mappings.[term] <- definition
        else
            mappings.Add(term,definition)
        
    member this.TryResolveTerm(term : string) =
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

    static member fromMappingSeq(mappings : seq<string*string>) =
        LDContext(Dictionary.ofSeq mappings)

    // Find a way to do this without mutable state
    static member combine (first : LDContext) (second : LDContext) : LDContext =
        let rec combine (current : LDContext) =
            match current.BaseContext with
            | Some baseContext -> combine baseContext
            | None -> current.BaseContext <- Some second
        combine first
        first

 
    static member tryCombineOptional (first : LDContext option) (second : LDContext option) : LDContext option =
        match first,second with
        | Some f, Some s -> Some (LDContext.combine f s)
        | Some f, None -> Some f
        | None, Some s -> Some s
        | _ -> None