namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type LabProtocol(
    id,
    ?additionalType,
    ?name,
    ?intendedUse,
    ?description,
    ?url,
    ?comment,
    ?version,
    ?labEquipment,
    ?reagent,
    ?computationalTool
) as this =
    inherit ROCrateObject(id = id, schemaType = "bioschemas.org/LabProtocol", ?additionalType = additionalType)
    do
        DynObj.setOptionalProperty (nameof name) name                            this
        DynObj.setOptionalProperty (nameof intendedUse) intendedUse              this
        DynObj.setOptionalProperty (nameof description) description              this
        DynObj.setOptionalProperty (nameof url) url                              this
        DynObj.setOptionalProperty (nameof comment) comment                      this
        DynObj.setOptionalProperty (nameof version) version                      this
        DynObj.setOptionalProperty (nameof labEquipment) labEquipment            this
        DynObj.setOptionalProperty (nameof reagent) reagent                      this
        DynObj.setOptionalProperty (nameof computationalTool) computationalTool  this
