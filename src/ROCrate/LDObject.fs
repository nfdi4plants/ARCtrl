namespace ARCtrl.ROCrate

open DynamicObj
open Thoth.Json.Core
open Fable.Core
open System

module ActivePattern = 
    #if !FABLE_COMPILER
    let (|SomeObj|_|) =
        // create generalized option type
        let ty = typedefof<option<_>>
        fun (a:obj) ->
            // Check for nulls otherwise 'a.GetType()' would fail
            if isNull a 
            then 
                None 
            else
                let aty = a.GetType()
                // Get option'.Value
                let v = aty.GetProperty("Value")
                if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then
                    // return value if existing
                    Some(v.GetValue(a, [| |]))
                else 
                    None
    #endif
    let (|NonStringEnumerable|_|) (o : obj) =
        match o with
        | :? string as s -> None
        | :? System.Collections.IEnumerable as e -> Some e
        | _ -> None
        



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

[<AttachMembers>]
type LDValue(value : obj, ?valueType : string) =
    
    let mutable valueType = defaultArg valueType "string"
    let mutable value = value

    member this.Value
        with get() = value
        and set(v) = value <- v

    member this.ValueType
        with get() = valueType
        and set(v) = valueType <- v

    override this.Equals(other : obj) =
        match other with
        | :? LDValue as other ->            
            this.Value = other.Value
        | _ -> false

    override this.GetHashCode() =
        HashCodes.mergeHashes (123) (this.Value.GetHashCode())

and [<AttachMembers>] LDRef(id : string) =
    let mutable id = id

    member this.Id
        with get() = id
        and set(v) = id <- v

    override this.Equals(other : obj) =
        match other with
        | :? LDRef as other ->            
            this.Id = other.Id
        | _ -> false

    override this.GetHashCode() =
       HashCodes.mergeHashes (123) (this.Id.GetHashCode())

and [<AttachMembers>] LDGraph(?id : string, ?nodes : ResizeArray<LDNode>, ?context : LDContext) as this =

    inherit DynamicObj()

    let mutable id = id
    let mappings = System.Collections.Generic.Dictionary()

    do
        match context with
        | Some ctx -> this.SetContext(ctx)
        | None -> ()

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
        let id = node.Id
        match this.TryGetNode(id) with
        | Some existingNode -> node.MergeAppendInto_InPlace(existingNode, flattenTo = this)
        | None -> mappings.Add(id, node)

    member this.Compact_InPlace(?context : LDContext) =
        let context = LDContext.tryCombineOptional context (this.TryGetContext())
        this.Nodes
        |> Seq.iter (fun node -> node.Compact_InPlace(?context = context))

    member this.SetContext (context: LDContext) =
        this.SetProperty("@context", context)

    static member setContext (context: LDContext) = fun (roc: #LDNode) -> roc.SetContext(context)

    member this.TryGetContext() = DynObj.tryGetTypedPropertyValue<LDContext>("@context") this

    static member tryGetContext () = fun (roc: #LDNode) -> roc.TryGetContext()

    member this.RemoveContext() = this.RemoveProperty("@context")

    member this.GetDynamicPropertyHelpers() =
        
        this.GetPropertyHelpers(false)
        |> Seq.filter (fun ph ->
            (ph.Name.StartsWith "init@" || ph.Name.Equals("mappings"))
            |> not
        )

    member this.GetDynamicPropertyNames(?context : LDContext) =
        let context = LDContext.tryCombineOptional context (this.TryGetContext())
        this.GetDynamicPropertyHelpers()
        |> Seq.choose (fun ph ->
            let name = 
                match context with
                | Some ctx ->
                    match ctx.TryResolveTerm ph.Name with
                    | Some term -> term
                    | None -> ph.Name
                | None -> ph.Name
            if name = "@context" then None else Some name
        )

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

    member this.HasType(schemaType:string, ?context : LDContext) =
        let context = LDContext.tryCombineOptional context (this.TryGetContext())
        this.SchemaType
        |> Seq.exists (fun st ->
            if st = schemaType then true
            else
                match context with
                | Some ctx ->
                    match ctx.TryResolveTerm st, ctx.TryResolveTerm schemaType with
                    | Some st, Some schemaType -> st = schemaType
                    | Some st, None -> st = schemaType
                    | None, Some schemaType -> st = schemaType
                    | _ -> false
                | None -> false
        ) 

    member this.TryGetProperty(propertyName : string, ?context : LDContext) =
        match this.TryGetPropertyValue(propertyName) with
        | Some value -> Some value
        | None ->                       
            match LDContext.tryCombineOptional context (this.TryGetContext()) with
            | Some ctx ->
                match ctx.TryResolveTerm propertyName with
                | Some term -> this.TryGetPropertyValue term
                | None ->
                    // Or instead of compact search term
                    match ctx.TryGetTerm propertyName with
                    | Some term -> this.TryGetPropertyValue term
                    | None -> None
            | None -> None

    member this.TryGetPropertyAsSingleton(propertyName : string, ?context : LDContext) : obj option =
        match this.TryGetProperty(propertyName, ?context = context) with
        | Some (:? string as s) -> Some s
        | Some (:? System.Collections.IEnumerable as e) ->
            let en = e.GetEnumerator()
            if en.MoveNext() then Some en.Current else None
        | Some o -> Some o
        | _ -> None

    member this.GetPropertyValues(propertyName : string, ?filter : obj -> LDContext option -> bool, ?context) =
        let filter = defaultArg filter (fun _ _ -> true)
        match this.TryGetProperty(propertyName, ?context = context) with
        | Some (:? string as s) ->
            if filter s context then
                ResizeArray [box s]
            else
                ResizeArray []
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

    member this.TryGetPropertyAsSingleNode(propertyName : string, ?graph : LDGraph, ?context : LDContext) =
        match this.TryGetPropertyAsSingleton(propertyName, ?context = context) with
        | Some (:? LDNode as n) -> Some n
        | Some (:? LDRef as r) when graph.IsSome ->
            match graph.Value.TryGetNode(r.Id) with
            | Some n -> Some n
            | None -> None
        | _ -> None

    member this.GetPropertyNodes(propertyName : string, ?filter : LDNode -> LDContext option -> bool, ?graph : LDGraph, ?context) =
        let context = LDContext.tryCombineOptional context (this.TryGetContext())
        this.GetPropertyValues(propertyName, ?context = context)
        |> Seq.choose (fun o ->
            match o with
            | :? LDRef as r when graph.IsSome ->
                match graph.Value.TryGetNode(r.Id) with
                | Some n ->
                    match filter with
                    | Some f when f n context -> Some n
                    | None -> Some n
                    | _ -> None
                | None -> None
            | :? LDNode as n ->
                match filter with
                | Some f when f n context -> Some n
                | None -> Some n
                | _ -> None
            | _ -> None

        )
        |> ResizeArray

    member this.GetDynamicPropertyHelpers() =       
        this.GetPropertyHelpers(false)
        |> Seq.filter (fun ph ->
            #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
            (ph.Name.StartsWith "init@" || ph.Name.Equals("id"))
            |> not
            #endif
            #if FABLE_COMPILER_PYTHON
            (ph.Name.StartsWith "init_" || ph.Name.Equals("id"))
            |> not
            #endif
            #if !FABLE_COMPILER
            true
            #endif
        )

    member this.GetPropertyNames(?context : LDContext) =
        let context = LDContext.tryCombineOptional context (this.TryGetContext())
        this.GetDynamicPropertyHelpers()
        |> Seq.choose (fun ph ->
            let name = 
                match context with
                | Some ctx ->
                    match ctx.TryResolveTerm ph.Name with
                    | Some term -> term
                    | None -> ph.Name
                | None -> ph.Name
            if name = "@context" then None else Some name
        )

    member this.SetProperty(propertyName : string, value : obj, ?context : LDContext) =
        let context = LDContext.tryCombineOptional context (this.TryGetContext())
        let propertyName =
            this.GetPropertyNames()
            |> Seq.tryFind (fun pn ->
                match context with
                | Some c -> c.PropertyNamesMatch(pn,propertyName)
                | None -> pn = propertyName
            )
            |> Option.defaultValue propertyName
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        Fable.Core.JsInterop.emitJsStatement (propertyName, value)  "super.SetProperty($0,$1)"
        #endif
        #if FABLE_COMPILER_PYTHON
        Fable.Core.PyInterop.emitPyStatement (propertyName, value)  "super().SetProperty($0,$1)"
        #endif
        #if !FABLE_COMPILER
        (this :> DynamicObj).SetProperty(propertyName, value)
        #endif        
        //DynObj.setProperty propertyName value this 

    member this.SetOptionalProperty(propertyName : string, value : #obj option, ?context : LDContext) =
        match value with
        | Some v -> this.SetProperty(propertyName, v, ?context = context)
        | None -> ()
        //this.RemoveProperty(propertyName) |> ignore
        //let propertyName =
        //    match LDContext.tryCombineOptional context (this.TryGetContext()) with
        //    | Some ctx ->
        //        match ctx.TryResolveTerm propertyName with
        //        | Some term -> term
        //        | None -> propertyName
        //    | None -> propertyName
        //this.SetProperty(propertyName,value)

    member this.HasProperty(propertyName : string, ?context : LDContext) =
        let v = this.TryGetProperty(propertyName, ?context = context)
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

    member this.MergeAppendInto_InPlace(other : LDNode, ?flattenTo : LDGraph) =
        let flattenTo_Singleton : obj -> obj =
            match flattenTo with
            | Some graph ->
                let rec f (o : obj) : obj =
                    match o with
                    #if !FABLE_COMPILER
                    | ActivePattern.SomeObj o -> f o
                    #endif
                    | :? LDNode as n ->
                        n.Flatten(graph) |> ignore
                        LDRef(n.Id)
                    | _ -> o
                f
            | None -> Operators.id
        let flattenTo_RA : ResizeArray<obj> -> ResizeArray<obj>=
            match flattenTo with
            | Some graph ->
                ARCtrl.Helper.ResizeArray.map flattenTo_Singleton
            | None -> Operators.id
        let flattenToAny : obj -> obj =
            match flattenTo with
            | Some graph ->
                let rec f (o : obj) : obj =
                    match o with
                    #if !FABLE_COMPILER
                    | ActivePattern.SomeObj o -> f o
                    #endif
                    | :? LDNode as n ->
                        n.Flatten(graph) |> ignore
                        LDRef(n.Id)
                    | ActivePattern.NonStringEnumerable e ->
                        [for v in e do f v] |> ResizeArray |> box
                    | _ -> o
                f
            | None -> Operators.id
        let rec toEqualitor (o : obj) : obj =
            match o with
            #if !FABLE_COMPILER
            | ActivePattern.SomeObj o -> toEqualitor o
            #endif
            | :? LDNode as n -> n.Id
            | :? LDRef as r -> r.Id
            | _ -> o
        this.GetPropertyNames()
        |> Seq.iter (fun pn -> 
            match other.TryGetProperty(pn) with
            | Some otherVal ->
                let thisVal = this.TryGetProperty(pn).Value
                match (thisVal, otherVal) with
                | ActivePattern.NonStringEnumerable e1, ActivePattern.NonStringEnumerable e2 ->
                    let l =
                        [
                            for v in e2 do
                                v
                            for v in e1 do
                                v
                        ]
                        |> List.distinctBy toEqualitor
                        |> ResizeArray
                        |> flattenTo_RA
                    other.SetProperty(pn, l)
                | ActivePattern.NonStringEnumerable theseVals, otherVal ->
                    let mutable isContained = false
                    let l = ResizeArray [
                        for thisVal in theseVals do
                            if toEqualitor thisVal = toEqualitor otherVal then
                                isContained <- true
                                flattenTo_Singleton thisVal
                            else thisVal
                    ]
                    if not isContained then
                        l.Add(otherVal)                   
                        other.SetProperty(pn, l)
                | thisVal, ActivePattern.NonStringEnumerable otherVals ->
                    let mutable isContained = false
                    let l = ResizeArray [
                        for otherVal in otherVals do
                            if toEqualitor thisVal = toEqualitor otherVal then
                                isContained <- true
                            otherVal
                    ]
                    if not isContained then
                        l.Add(flattenTo_Singleton thisVal)                   
                        other.SetProperty(pn, l)            
                | thisVal, otherVal ->
                    if toEqualitor thisVal = toEqualitor otherVal then ()
                    else
                        let l = ResizeArray [flattenTo_Singleton thisVal; otherVal]
                        other.SetProperty(pn, l)
            | None ->
                let v = this.TryGetProperty(pn).Value |> flattenToAny 
                other.SetProperty(pn, v)

        )
        


    member this.Compact_InPlace(?context : LDContext,?setContext : bool) =
        let setContext = defaultArg setContext false
        let context = LDContext.tryCombineOptional context (this.TryGetContext())
        if context.IsSome then
            let context = context.Value
            if setContext then this.SetContext(context)
            let newTypes = ResizeArray [
                for st in this.SchemaType do
                    match context.TryGetTerm st with
                    | Some term -> term
                    | None -> st
            ]
            this.SchemaType <- newTypes
        let rec compactValue_inPlace (o : obj) : obj =
            match o with
            | :? LDNode as n ->
                n.Compact_InPlace(?context = context)
                n
            | :? string as s ->
                s
            | :? System.Collections.IEnumerable as e ->
                let en = e.GetEnumerator()
                let l = ResizeArray [
                    while en.MoveNext() do
                        compactValue_inPlace en.Current
                ]
                if l.Count = 1 then l.[0] else l
            | :? LDValue as v -> v.Value
            | x -> x

        this.GetDynamicPropertyHelpers()
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
        //let graph, graphContext =
        //    match graph with
        //    | Some g -> g, g.TryGetContext()
        //    | None -> new LDGraph(?context = this.TryGetContext()), None
        let graph =
            match graph with
            | Some g -> g
            | None -> new LDGraph(?context = this.TryGetContext())
        let rec flattenValue (o : obj) : obj =
            match o with
            | :? LDNode as n ->
                n.Flatten(graph) |> ignore
                LDRef(n.Id)
            | :? string as s -> s
            | :? System.Collections.IEnumerable as e ->
                let en = e.GetEnumerator()
                let l = ResizeArray [
                    while en.MoveNext() do
                        flattenValue en.Current
                ]
                l
            | x -> x
        this.GetDynamicPropertyHelpers()
        |> Seq.iter (fun ph ->
            let newValue = flattenValue (ph.GetValue(this))
            ph.SetValue this newValue
        )
        graph.AddNode this
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
        this.GetDynamicPropertyHelpers()
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

            roc.GetDynamicPropertyHelpers()
            |> Seq.iter (fun ph ->
                if ph.IsDynamic && (ph.Name = "@id" || ph.Name = "@type" || ph.Name = "additionalType"(* || ph.Name = "id"*)) then
                    ph.RemoveValue(roc)
            )
            Some roc

        | _ -> None