namespace ARCtrl.ROCrate

open DynamicObj

/// Base interface implemented by all explicitly known objects in our ROCrate profiles.
type IROCrateObject =
    abstract member SchemaType : string
    abstract member Id: string
    abstract member AdditionalType: string option

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
        member this.SchemaType = schemaType
        member this.Id = id
        member this.AdditionalType = additionalType