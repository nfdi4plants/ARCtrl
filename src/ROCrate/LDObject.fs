namespace ARCtrl.ROCrate

open DynamicObj
open Thoth.Json.Core
open Fable.Core
open System

type LDContext() = inherit DynamicObj()

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
            dynObj.DeepCopyPropertiesTo(roc)
            roc.TryGetDynamicPropertyHelper("@id").Value.RemoveValue()
            roc.TryGetDynamicPropertyHelper("@type").Value.RemoveValue()
            if at.IsSome then roc.TryGetDynamicPropertyHelper("additionalType").Value.RemoveValue()
            Some roc

        | _ -> None
