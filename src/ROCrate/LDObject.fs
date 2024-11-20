namespace ARCtrl.ROCrate

open DynamicObj
open Thoth.Json.Core
open Fable.Core
open System

type LDContext() = inherit DynamicObj()

/// Base interface implemented by all explicitly known objects in our ROCrate profiles.
type ILDObject =
    abstract member SchemaType : string with get, set
    abstract member Id: string
    abstract member AdditionalType: string option with get, set

/// Base class for all explicitly known objects in our ROCrate profiles to inherit from.
/// Basically a DynamicObj that implements the ILDObject interface.
[<AttachMembers>]
type LDObject(id:string, schemaType: string, ?additionalType) =
    inherit DynamicObj()

    let mutable schemaType = schemaType
    let mutable additionalType = additionalType

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

    member this.TryGetContext() = DynObj.tryGetTypedPropertyValue<DynamicObj>("@context") this

    static member tryGetContext () = fun (roc: #LDObject) -> roc.TryGetContext()

    member this.RemoveContext() = this.RemoveProperty("@context")

    static member removeContext () = fun (roc: #LDObject) -> roc.RemoveContext()

    static member tryFromDynamicObj (dynObj: DynamicObj) =
        match
            DynObj.tryGetTypedPropertyValue<string>("@type") dynObj,
            DynObj.tryGetTypedPropertyValue<string>("@id") dynObj,
            DynObj.tryGetTypedPropertyValue<string>("additionalType") dynObj
        with
        | (Some schemaType), (Some id), at ->
            let roc = new LDObject(id, schemaType, ?additionalType = at)
            match DynObj.tryGetTypedPropertyValue<LDContext>("@context") dynObj with
            | Some context -> roc.SetContext(context)
            | _ -> ()
            Some roc
        | _ -> None
        