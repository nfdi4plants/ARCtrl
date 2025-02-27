namespace ARCtrl.ROCrate

open System.Collections.Generic
open ARCtrl.Helper
open Fable.Core

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

module IRIHelper =

    open ARCtrl.Helper.Regex

    let compactIRIRegex = """(?<prefix>.*):(?<suffix>[^\/][^\/].*)"""

    let (|CompactIri|_|) (termDefition : string) =
        match termDefition with
        | ActivePatterns.Regex compactIRIRegex result ->
            let prefix = result.Groups.["prefix"].Value
            let suffix = result.Groups.["suffix"].Value
            Some(prefix,suffix)
        | _ -> None

    let combine (baseIRI : string) (relative : string) =
        if relative.StartsWith("http://") || relative.StartsWith("https://") then
            relative
        else
            let baseUri = new System.Uri(baseIRI)
            let relativeUri = new System.Uri(baseUri,relative)
            relativeUri.ToString()

    let combineOptional (baseIRI : string option) (relative : string option) =
        match baseIRI, relative with
        | Some b, Some r -> Some (combine b r)
        | Some b, None -> Some b
        | None, Some r -> Some r
        | _ -> None

// Add second dictionary which maps from definition to term?
// Make LDContext to be nested hierarchical tree? Like this you can iterate through the tree and stop at the first match, kind of like a shadowing mechanism
[<AttachMembers>]
type LDContext(?mappings : Dictionary<string,string>, ?baseContexts : ResizeArray<LDContext>) =

    let mutable baseContexts = Option.defaultValue (ResizeArray []) baseContexts
    let mutable name : string option = None

    let mappings : Dictionary<string,string> =
        match mappings with
        | Some m -> m
        | None -> Dictionary()

    let reverseMappings : Dictionary<string,string> =
        let dict = Dictionary()
        for kvp in mappings do
            Dictionary.addOrUpdate kvp.Value kvp.Key dict
        dict

    let rec tryFindTerm (term : string) : string option =
        let definition = 
            match Dictionary.tryFind term mappings with
            | Some v -> Some v
            | None ->
                baseContexts
                |> Seq.tryPick (fun ctx -> ctx.TryResolveTerm term)
        match definition with
        | Some (IRIHelper.CompactIri (prefix,suffix)) ->
            let prefix = if prefix = term then prefix else tryFindTerm prefix |> Option.defaultValue prefix
            let suffix = if suffix = term then suffix else tryFindTerm suffix |> Option.defaultValue suffix
            IRIHelper.combine prefix suffix
            |> Some
        | Some d -> Some d
        | None -> None

    let tryFindIri (iri : string) =
        match Dictionary.tryFind iri reverseMappings with
        | Some v -> Some v
        | None -> 
            baseContexts
            |> Seq.tryPick (fun ctx -> ctx.TryGetTerm iri)

    let tryCompactIRI (iri : string) =
        failwith "TryCompactIRI is Not implemented yet"

    member this.Mappings
        with get() = mappings

    member this.BaseContexts
        with get() = baseContexts
        and internal set(value) = baseContexts <- value

    member this.Name
        with get() = name
        and set(value) = name <- value

    member this.AddMapping(term,definition) =
        Dictionary.addOrUpdate term definition mappings
        Dictionary.addOrUpdate definition term reverseMappings
        
    member this.TryResolveTerm(term : string) =
        // Handle compact IRI
        if term.Contains(":") then
            term.Split(':')
            |> Seq.map tryFindTerm
            |> Seq.reduce IRIHelper.combineOptional
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

    member this.ShallowCopy() =
        let newMappings = Dictionary()
        for kvp in mappings do
            newMappings.Add(kvp.Key,kvp.Value)
        LDContext(mappings = newMappings, baseContexts = baseContexts)

    member this.DeepCopy() =
        let newMappings = Dictionary()
        for kvp in mappings do
            newMappings.Add(kvp.Key,kvp.Value)
        let newBaseContexts = ResizeArray()
        for ctx in baseContexts do
            newBaseContexts.Add(ctx.DeepCopy())
        LDContext(mappings = newMappings, baseContexts = newBaseContexts)

    interface System.ICloneable with
        member this.Clone() =
            this.DeepCopy()

    member this.StructurallyEquals(other : LDContext) =
        this.GetHashCode() = other.GetHashCode()

    member this.ReferenceEquals(other : LDContext) =
        System.Object.ReferenceEquals(this,other)

    override this.Equals(other : obj) =
        match other with
        | :? LDContext as other -> this.StructurallyEquals(other)
        | _ -> false

    override this.GetHashCode() =
        let mappingsHash = 
            this.Mappings.Keys
            |> Seq.sort
            |> Seq.map (fun k -> HashCodes.mergeHashes (HashCodes.hash k) (HashCodes.hash this.Mappings.[k]))
            |> HashCodes.boxHashSeq
            |> fun v -> v :?> int
        let nameHash = 
            match this.Name with
            | Some n -> n.GetHashCode()
            | None -> 0
        let baseContextsHash =
            if baseContexts.Count = 0 then 0
            else
                baseContexts
                |> Seq.map (fun ctx -> ctx.GetHashCode())
                |> Seq.reduce HashCodes.mergeHashes
        HashCodes.mergeHashes (HashCodes.mergeHashes mappingsHash nameHash) baseContextsHash