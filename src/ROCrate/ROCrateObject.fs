namespace ARCtrl.ROCrate

open DynamicObj
open Thoth.Json.Core
open Fable.Core
open System

type LDContext() = inherit DynamicObj()

/// Base interface implemented by all explicitly known objects in our ROCrate profiles.
type IROCrateObject =
    abstract member SchemaType : string with get, set
    abstract member Id: string
    abstract member AdditionalType: string option with get, set

/// Base class for all explicitly known objects in our ROCrate profiles to inherit from.
/// Basically a DynamicObj that implements the IROPCrateObject interface.
[<AttachMembers>]
type ROCrateObject(id:string, schemaType: string, ?additionalType) =
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

    interface IROCrateObject with

        member this.SchemaType
            with get() = schemaType
            and set(value) = schemaType <- value

        member this.Id = id

        member this.AdditionalType
            with get() = additionalType
            and set(value) = additionalType <- value

    member this.SetContext (context: LDContext) =
        this.SetProperty("@context", context)

    static member setContext (context: LDContext) = fun (roc: #ROCrateObject) -> roc.SetContext(context)

    member this.TryGetContext() = DynObj.tryGetTypedPropertyValue<DynamicObj>("@context") this

    static member tryGetContext () = fun (roc: #ROCrateObject) -> roc.TryGetContext()

    member this.RemoveContext() = this.RemoveProperty("@context")

    static member removeContext () = fun (roc: #ROCrateObject) -> roc.RemoveContext() 