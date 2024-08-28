namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Assay(
    id,
    identifier,
    ?about,
    ?comment,
    ?creator,
    ?hasPart,
    ?measurementMethod,
    ?measurementTechnique,
    ?url,
    ?variableMeasured
) as this =
    inherit Dataset(id, "Assay")
    do
        DynObj.setValue this (nameof identifier) identifier

        DynObj.setValueOpt this (nameof measurementMethod) measurementMethod
        DynObj.setValueOpt this (nameof measurementTechnique) measurementTechnique
        DynObj.setValueOpt this (nameof variableMeasured) variableMeasured
        DynObj.setValueOpt this (nameof about) about
        DynObj.setValueOpt this (nameof comment) comment
        DynObj.setValueOpt this (nameof creator) creator
        DynObj.setValueOpt this (nameof hasPart) hasPart
        DynObj.setValueOpt this (nameof url) url