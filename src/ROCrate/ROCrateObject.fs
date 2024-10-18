namespace ARCtrl.ROCrate

open DynamicObj
open Thoth.Json.Core
open System

/// Base interface implemented by all explicitly known objects in our ROCrate profiles.
type IROCrateObject =
    abstract member SchemaType : string with get, set
    abstract member Id: string
    abstract member AdditionalType: string option with get, set

/// Base class for all explicitly known objects in our ROCrate profiles to inherit from.
/// Basically a DynamicObj that implements the IROPCrateObject interface.
type ROCrateObject(id:string, schemaType: string, ?additionalType) =
    inherit DynamicObj()

    let mutable _schemaType = schemaType
    let mutable _additionalType = additionalType

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = _schemaType
        and set(value) = _schemaType <- value

    member this.AdditionalType
        with get() = _additionalType
        and set(value) = _additionalType <- value

    interface IROCrateObject with

        member this.SchemaType
            with get() = _schemaType
            and set(value) = _schemaType <- value

        member this.Id = id

        member this.AdditionalType
            with get() = _additionalType
            and set(value) = _additionalType <- value

    member this.SetContext (context: #DynamicObj) =
        this.SetProperty("@context", context)

    static member setContext (context: #DynamicObj) = fun (roc: #ROCrateObject) -> roc.SetContext(context)

    member this.TryGetContext() =
        DynObj.tryGetTypedPropertyValue<DynamicObj>("@context") this

    static member tryGetContext () = fun (roc: #ROCrateObject) -> roc.TryGetContext()

    member this.RemoveContext() =
        this.RemoveProperty("@context")

    static member removeContext () = fun (roc: #ROCrateObject) -> roc.RemoveContext()