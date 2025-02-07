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
type LDContext(?mappings : Dictionary<string,string>) =

    let mappings : Dictionary<string,string> =
        match mappings with
        | Some m -> m
        | None -> Dictionary()

    member this.AddMapping(term,definition) =
        if mappings.ContainsKey(term) then
            mappings.[term] <- definition
        else
            mappings.Add(term,definition)
        
    member this.TryResolveTerm(term : string) =
        Dictionary.tryFind term mappings

    static member fromMappingSeq(mappings : seq<string*string>) =
        LDContext(Dictionary.ofSeq mappings)

    static member combine (first : LDContext) (second : LDContext) : LDContext =
        failwith "Not implemented yet"

    static member tryCombineOptional (first : LDContext option) (second : LDContext option) : LDContext option =
        failwith "Not implemented yet"