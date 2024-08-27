namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type LabProtocol(id: string, ?additionalType: string) =
    inherit ROCrateObject(id = id, schemaType = "bioschemas.org/LabProtocol", ?additionalType = additionalType)

    static member create(
        // mandatory
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
    ) =
        let lp = LabProcess(id, ?additionalType = additionalType)

        DynObj.setValueOpt lp (nameof name) name
        DynObj.setValueOpt lp (nameof intendedUse) intendedUse
        DynObj.setValueOpt lp (nameof description) description
        DynObj.setValueOpt lp (nameof url) url
        DynObj.setValueOpt lp (nameof comment) comment
        DynObj.setValueOpt lp (nameof version) version
        DynObj.setValueOpt lp (nameof labEquipment) labEquipment
        DynObj.setValueOpt lp (nameof reagent) reagent
        DynObj.setValueOpt lp (nameof computationalTool) computationalTool

        lp
