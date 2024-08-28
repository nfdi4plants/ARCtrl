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
        DynObj.setValueOpt this (nameof name) name
        DynObj.setValueOpt this (nameof intendedUse) intendedUse
        DynObj.setValueOpt this (nameof description) description
        DynObj.setValueOpt this (nameof url) url
        DynObj.setValueOpt this (nameof comment) comment
        DynObj.setValueOpt this (nameof version) version
        DynObj.setValueOpt this (nameof labEquipment) labEquipment
        DynObj.setValueOpt this (nameof reagent) reagent
        DynObj.setValueOpt this (nameof computationalTool) computationalTool
