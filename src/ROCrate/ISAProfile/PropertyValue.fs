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
    inherit LDObject(
        id = id,
        schemaType = ResizeArray[|"schema.org/PropertyValue"|],
        additionalType = defaultArg additionalType (ResizeArray[||])
    )
    do

        DynObj.setProperty (nameof name) name this
        DynObj.setProperty (nameof value) value this

        DynObj.setOptionalProperty (nameof propertyID) propertyID         this
        DynObj.setOptionalProperty (nameof unitCode) unitCode             this
        DynObj.setOptionalProperty (nameof unitText) unitText             this
        DynObj.setOptionalProperty (nameof valueReference) valueReference this

    member this.GetName() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "PropertyValue" (nameof name) this
    static member getName = fun (lp: PropertyValue) -> lp.GetName()

    member this.GetValue() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "PropertyValue" (nameof name) this
    static member getValue = fun (lp: PropertyValue) -> lp.GetValue()
