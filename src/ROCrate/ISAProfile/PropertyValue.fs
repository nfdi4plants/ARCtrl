namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type PropertyValue(id: string, ?additionalType: string) =
    inherit ROCrateObject(id = id, schemaType = "schema.org/PropertyValue", ?additionalType = additionalType)

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