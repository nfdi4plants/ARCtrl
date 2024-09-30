namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type PropertyValue(
    id,
    name,
    value,
    ?propertyID,
    ?unitCode,
    ?unitText,
    ?valueReference,
    ?additionalType
) as this =
    inherit ROCrateObject(id = id, schemaType = "schema.org/PropertyValue", ?additionalType = additionalType)
    do

        DynObj.setProperty (nameof name) name this
        DynObj.setProperty (nameof value) value this

        DynObj.setOptionalProperty (nameof propertyID) propertyID         this
        DynObj.setOptionalProperty (nameof unitCode) unitCode             this
        DynObj.setOptionalProperty (nameof unitText) unitText             this
        DynObj.setOptionalProperty (nameof valueReference) valueReference this
