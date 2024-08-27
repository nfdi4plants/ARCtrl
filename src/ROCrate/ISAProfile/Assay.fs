namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Assay(id: string) =
    // inheritance
    inherit Dataset(id, "Assay")
    static member create(
        // mandatory
        id,
        // Properties from Thing
        identifier,
        // optional
        // Properties from CreativeWork
        // Properties from Dataset
        ?measurementMethod,
        ?measurementTechnique,
        ?variableMeasured,
        // Properties from CreativeWork
        ?about,
        ?comment,
        ?creator,
        ?hasPart,
        ?url
    ) =
        let ds = Assay(id = id)

        // Properties from Dataset
        DynObj.setValueOpt ds (nameof measurementMethod) measurementMethod
        DynObj.setValueOpt ds (nameof measurementTechnique) measurementTechnique
        DynObj.setValueOpt ds (nameof variableMeasured) variableMeasured

        // Properties from CreativeWork
        DynObj.setValueOpt ds (nameof about) about
        DynObj.setValueOpt ds (nameof comment) comment
        DynObj.setValueOpt ds (nameof creator) creator
        DynObj.setValueOpt ds (nameof hasPart) hasPart
        DynObj.setValueOpt ds (nameof url) url

        // Properties from Thing
        DynObj.setValue ds (nameof identifier) identifier

        ds