namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type PropertyValue(id: string, ?additionalType: string) =
    inherit DynamicObj()

    let mutable _schemaType = "schema.org/PropertyValue"
    let mutable _additionalType = additionalType

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = _schemaType
        and set(value) = _schemaType <- value

    member this.AdditionalType
        with get() = _additionalType
        and set(value) = _additionalType <- value

     //interface implementations
    interface IROCrateObject with 
        member this.Id with get () = this.Id
        member this.SchemaType with get (): string = this.SchemaType
        member this.AdditionalType with get (): string option = this.AdditionalType

    static member create(
        // mandatory
        id,
        name,
        value,
        ?propertyID,
        ?unitCode,
        ?unitText,
        ?valueReference,
        ?additionalType
    ) =
        let pv = PropertyValue(id, ?additionalType = additionalType)

        DynObj.setValue pv (nameof name) name
        DynObj.setValue pv (nameof value) value

        DynObj.setValueOpt pv (nameof propertyID) propertyID
        DynObj.setValueOpt pv (nameof unitCode) unitCode
        DynObj.setValueOpt pv (nameof unitText) unitText
        DynObj.setValueOpt pv (nameof valueReference) valueReference
        
        pv