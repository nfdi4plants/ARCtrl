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

/// Base class for all explicitly known objects in our ROCrate profiles to inherit from.
/// Basically a DynamicObj that implements the ILDObject interface.
[<AttachMembers>]
type LDObject(id: string, schemaType: ResizeArray<string>, ?additionalType: ResizeArray<string>) =
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

    interface ILDObject with

        member this.SchemaType
            with get() = schemaType
            and set(value) = schemaType <- value

        member this.Id = id

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

    static member setContext (context: LDContext) = fun (roc: #LDObject) -> roc.SetContext(context)

    member this.TryGetContext() = DynObj.tryGetTypedPropertyValue<LDContext>("@context") this

    static member tryGetContext () = fun (roc: #LDObject) -> roc.TryGetContext()

    member this.RemoveContext() = this.RemoveProperty("@context")

    static member removeContext () = fun (roc: #LDObject) -> roc.RemoveContext()

    static member tryFromDynamicObj (dynObj: DynamicObj) =

        let original_id = DynObj.tryGetTypedPropertyValue<string> "@id" dynObj
        let original_type = DynObj.tryGetTypedPropertyValueAsResizeArray<string> "@type" dynObj
        match original_id, original_type with
        | (Some id), (Some st)->
            // initialize with extracted static members only
            let at = DynObj.tryGetTypedPropertyValueAsResizeArray<string> "additionalType" dynObj
            let roc = new LDObject(id = id, schemaType = st, ?additionalType = at)


            // Currently commented out, as @context is set as a dynamic property
            //match DynObj.tryGetTypedPropertyValue<LDContext>("@context") dynObj with
            //| Some context -> roc.SetContext(context)
            //| _ -> ()

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
