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
    let mutable nodes = defaultArg nodes (ResizeArray [])

    member this.Id
        with get() = id
        and set(v) = id <- v

    member this.Nodes
        with get() = nodes
        and set(v) = nodes <- v

/// Base class for all explicitly known objects in our ROCrate profiles to inherit from.
/// Basically a DynamicObj that implements the ILDNode interface.
and [<AttachMembers>] LDNode(id: string, schemaType: ResizeArray<string>, ?additionalType: ResizeArray<string>) =
    inherit DynamicObj()

    let mutable schemaType = schemaType
    let mutable additionalType = defaultArg additionalType (ResizeArray [])

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = schemaType
        and set(value) = schemaType <- value

    member this.AdditionalType
        with get() = additionalType
        and set(value) = additionalType <- value

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

    member this.SetContext (context: LDContext) =
        this.SetProperty("@context", context)

    static member setContext (context: LDContext) = fun (roc: #LDNode) -> roc.SetContext(context)

    member this.TryGetContext() = DynObj.tryGetTypedPropertyValue<LDContext>("@context") this

    static member tryGetContext () = fun (roc: #LDNode) -> roc.TryGetContext()

    member this.RemoveContext() = this.RemoveProperty("@context")

    member this.Compact_InPlace(?context : LDContext) =
        let context = LDContext.tryCombineOptional context (this.TryGetContext())
        this.GetPropertyHelpers(true)
        |> Seq.iter (fun ph ->
            let newKey =
                match context with
                | Some ctx ->
                    match ctx.TryResolveTerm ph.Name with
                    | Some term -> term
                    | None -> ph.Name
                | None -> ph.Name
            let newValue =
                match ph.GetValue with
                | :? LDNode as n -> n.Compact_InPlace(context)
                | :? System.IEnu
        )

    //member this.Flatten

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
