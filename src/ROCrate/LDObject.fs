namespace ARCtrl.ROCrate

open DynamicObj
open Thoth.Json.Core
open Fable.Core
open System
[<AutoOpen>]
module DynamicObjExtensions =

    type DynamicObj with

        member this.HasProperty(propertyName : string) =
            this.TryGetPropertyValue(propertyName) |> Option.isSome


/// Base interface implemented by all explicitly known objects in our ROCrate profiles.
type ILDObject =
    abstract member SchemaType : ResizeArray<string> with get, set
    abstract member Id: string
    abstract member AdditionalType: ResizeArray<string> with get, set


type LDValue(value : obj, ?valueType : string) =
    
    let mutable valueType = defaultArg valueType "string"
    let mutable value = value

    member this.Value
        with get() = value
        and set(v) = value <- v

    member this.ValueType
        with get() = valueType
        and set(v) = valueType <- v


and [<AttachMembers>] LDRef(id : string) =
    let mutable id = id

    member this.Id
        with get() = id
        and set(v) = id <- v

and [<AttachMembers>] LDGraph(?id : string, ?nodes : ResizeArray<LDNode>, ?context : LDContext) =

    let mutable id = id
    let mappings = System.Collections.Generic.Dictionary()

    do
        match nodes with
        | Some nodes ->
            nodes
            |> Seq.iter (fun node ->
                mappings.Add(node.Id, node)
            )              
        | None -> ()
      
    member this.Id
        with get() = id
        and set(v) = id <- v

    member this.Nodes
        with get() = mappings.Values |> ResizeArray

    member this.ContainsNode(id : string) =
        mappings.ContainsKey(id)

    member this.GetNode(id : string) =
        mappings.Item(id)

    member this.TryGetNode(id : string) =
        match mappings.TryGetValue(id) with
        | true, node -> Some node
        | _ -> None

    member this.AddNode(node : LDNode) =
        mappings.Add(node.Id, node)

/// Base class for all explicitly known objects in our ROCrate profiles to inherit from.
/// Basically a DynamicObj that implements the ILDNode interface.
and [<AttachMembers>] LDNode(id: string, schemaType: ResizeArray<string>, ?additionalType: ResizeArray<string>, ?context : LDContext) as this =
    inherit DynamicObj()

    let mutable schemaType = schemaType
    let mutable additionalType = defaultArg additionalType (ResizeArray [])

    do
        match context with
        | Some ctx -> this.SetContext(ctx)
        | None -> ()

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = schemaType
        and set(value) = schemaType <- value

    member this.AdditionalType
        with get() = additionalType
        and set(value) = additionalType <- value

    member this.ContainsContextualizedType(schemaType:string, ?context : LDContext) =
        let context = LDContext.tryCombineOptional context (this.TryGetContext())
        this.SchemaType
        |> Seq.exists (fun st ->
            if st = schemaType then true
            else
                match context with
                | Some ctx ->
                    ctx.TryResolveTerm st = Some schemaType
                | None -> false
        ) 

    member this.TryGetContextualizedProperty(propertyName : string, ?context : LDContext) =
        match this.TryGetPropertyValue(propertyName) with
        | Some value -> Some value
        | None ->                       
            match LDContext.tryCombineOptional context (this.TryGetContext()) with
            | Some ctx ->
                match ctx.TryResolveTerm propertyName with
                | Some term -> this.TryGetPropertyValue term
                | None -> None
            | None -> None

    member this.GetPropertyValues(propertyName : string, ?filter : obj -> LDContext option -> bool, ?context) =
        let filter = defaultArg filter (fun _ _ -> true)
        match this.TryGetContextualizedProperty(propertyName, ?context = context) with
        | Some (:? System.Collections.IEnumerable as e) ->
            let en = e.GetEnumerator()
            [
                while en.MoveNext() do
                    if filter en.Current context then
                        yield en.Current
            ]
            |> ResizeArray
        | Some o when filter o context->
            ResizeArray [o]
        | _ ->
            ResizeArray []

    member this.GetPropertyNodes(propertyName : string, ?filter : LDNode -> LDContext option -> bool, ?context) =
        let filter (o : obj) context =
            match o with
            | :? LDNode as n ->
                match filter with
                | Some f -> f n context
                | None -> true
            | _ -> false
        this.GetPropertyValues(propertyName, filter = filter, ?context = context)

    member this.SetContextualizedPropertyValue(propertyName : string, value : obj, ?context : LDContext) =
        this.RemoveProperty(propertyName) |> ignore
        let propertyName =
            match LDContext.tryCombineOptional context (this.TryGetContext()) with
            | Some ctx ->
                match ctx.TryResolveTerm propertyName with
                | Some term -> term
                | None -> propertyName
            | None -> propertyName
        this.SetProperty(propertyName,value)

    member this.ContainsContextualizedPropertyValue(propertyName : string, ?context : LDContext) =
        let v = this.TryGetContextualizedProperty(propertyName, ?context = context)
        match v with
        | None -> false
        | Some v when v = null -> false
        //| Some (:? string as s) -> s <> "" // Caught by next rule?
        | Some (:? System.Collections.IEnumerable as e) -> e.GetEnumerator().MoveNext()
        | _ -> true

    member this.SetContext (context: LDContext) =
        this.SetProperty("@context", context)

    static member setContext (context: LDContext) = fun (roc: #LDNode) -> roc.SetContext(context)

    member this.TryGetContext() = DynObj.tryGetTypedPropertyValue<LDContext>("@context") this

    static member tryGetContext () = fun (roc: #LDNode) -> roc.TryGetContext()

    member this.RemoveContext() = this.RemoveProperty("@context")

    member this.Compact_InPlace(?context : LDContext) =
        let context = LDContext.tryCombineOptional context (this.TryGetContext())
        let rec compactValue_inPlace (o : obj) : obj =
            match o with
            | :? LDNode as n ->
                n.Compact_InPlace(?context = context)
                n
            | :? System.Collections.IEnumerable as e ->
                let en = e.GetEnumerator()
                let l = ResizeArray [
                    while en.MoveNext() do
                        compactValue_inPlace en.Current
                ]
                if l.Count = 1 then l.[0] else l
            | :? LDValue as v -> v.Value
            | x -> x
        this.GetPropertyHelpers(true)
        |> Seq.iter (fun ph ->
            let newKey =
                match context with
                | Some ctx ->
                    match ctx.TryGetTerm ph.Name with
                    | Some term -> Some term
                    | None -> None
                | None -> None
            let newValue =
                compactValue_inPlace (ph.GetValue(this))
            match newKey with
            | Some key when key <> ph.Name ->
                this.RemoveProperty(ph.Name) |> ignore
                this.SetProperty(key, newValue)
            | _ -> ph.SetValue this newValue
        )

    member this.Flatten(?graph : LDGraph) : LDGraph =
        let graph = defaultArg graph (new LDGraph(?context = this.TryGetContext()))
        let rec flattenValue (o : obj) : obj =
            match o with
            | :? LDNode as n ->
                n.Flatten(graph) |> ignore
                LDRef(n.Id)
            | :? System.Collections.IEnumerable as e ->
                let en = e.GetEnumerator()
                let l = ResizeArray [
                    while en.MoveNext() do
                        flattenValue en.Current
                ]
                l
            | x -> x
        this.GetPropertyHelpers(true)
        |> Seq.iter (fun ph ->
            let newValue = flattenValue (ph.GetValue(this))
            ph.SetValue this newValue
        )
        graph

    member this.Unflatten(graph : LDGraph) =
        let rec unflattenValue (o : obj) : obj =
            match o with
            | :? LDRef as r ->
                match graph.TryGetNode(r.Id) with
                | Some n -> n
                | None -> r
            | :? LDNode as n ->
                n.Unflatten(graph)
                n
            | :? System.Collections.IEnumerable as e ->
                let en = e.GetEnumerator()
                let l = ResizeArray [
                    while en.MoveNext() do
                        unflattenValue en.Current
                ]
                l
            | x -> x
        this.GetPropertyHelpers(true)
        |> Seq.iter (fun ph ->
            let newValue = unflattenValue (ph.GetValue(this))
            ph.SetValue this newValue
        )

    static member removeContext () = fun (roc: #LDNode) -> roc.RemoveContext()

    static member tryFromDynamicObj (dynObj: DynamicObj) =

        let original_id = DynObj.tryGetTypedPropertyValue<string> "@id" dynObj
        let original_type = DynObj.tryGetTypedPropertyValueAsResizeArray<string> "@type" dynObj
        match original_id, original_type with
        | (Some id), (Some st)->
            // initialize with extracted static members only
            let at = DynObj.tryGetTypedPropertyValueAsResizeArray<string> "additionalType" dynObj
            let roc = new LDNode(id = id, schemaType = st, ?additionalType = at)

            // copy dynamic properties!
            dynObj.DeepCopyPropertiesTo(roc, includeInstanceProperties = false)


            // ----- Commented out as implementation has not been finalized -----
            //printfn "dynobj"
            //dynObj.GetPropertyHelpers(true)           
            //|> Seq.iter (fun p -> printfn "isDynamic:%b, Name: %s" p.IsDynamic p.Name)
            //printfn "roc"
            //roc.GetPropertyHelpers(true)           
            //|> Seq.iter (fun p -> printfn "isDynamic:%b, Name: %s" p.IsDynamic p.Name)      
            //roc.TryGetDynamicPropertyHelper("@id").Value.RemoveValue()
            //roc.TryGetDynamicPropertyHelper("@type").Value.RemoveValue()
            //if at.IsSome then roc.TryGetDynamicPropertyHelper("additionalType").Value.RemoveValue()

            roc.GetPropertyHelpers(true)
            |> Seq.iter (fun ph ->
                if ph.IsDynamic && (ph.Name = "@id" || ph.Name = "@type" || ph.Name = "additionalType"(* || ph.Name = "id"*)) then
                    ph.RemoveValue(roc)
            )
            Some roc

        | _ -> None
