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
        let tryGetResizeArrayOrSingleton (name : string) (obj : DynamicObj) =
            match obj.TryGetPropertyValue(name) with
            | Some (:? ResizeArray<string> as ra) -> Some ra
            | Some (:?string as s) -> Some (ResizeArray [s])
            | _ -> None
        match
            tryGetResizeArrayOrSingleton "@type" dynObj,
            DynObj.tryGetTypedPropertyValue<string>("@id") dynObj
        with
        | (Some st), (Some id)->
            // initialize with extracted static members only
            let at = tryGetResizeArrayOrSingleton "additionalType" dynObj
            let roc = new LDObject(id = id, schemaType = st, ?additionalType = at)

            // copy dynamic properties!
            match DynObj.tryGetTypedPropertyValue<LDContext>("@context") dynObj with
            | Some context -> roc.SetContext(context)
            | _ -> ()
            dynObj.CopyDynamicPropertiesTo(roc)
            Some roc
        | _ -> None
        