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

        DynObj.setValue this (nameof name) name
        DynObj.setValue this (nameof value) value

        DynObj.setValueOpt this (nameof propertyID) propertyID
        DynObj.setValueOpt this (nameof unitCode) unitCode
        DynObj.setValueOpt this (nameof unitText) unitText
        DynObj.setValueOpt this (nameof valueReference) valueReference
