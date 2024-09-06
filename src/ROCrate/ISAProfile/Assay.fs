namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Assay(
    id: string,
    identifier: string,
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
        DynObj.setProperty (nameof identifier) identifier this

        DynObj.setValueOpt this (nameof measurementMethod) measurementMethod
        DynObj.setValueOpt this (nameof measurementTechnique) measurementTechnique
        DynObj.setValueOpt this (nameof variableMeasured) variableMeasured
        DynObj.setValueOpt this (nameof about) about
        DynObj.setValueOpt this (nameof comment) comment
        DynObj.setValueOpt this (nameof creator) creator
        DynObj.setValueOpt this (nameof hasPart) hasPart
        DynObj.setValueOpt this (nameof url) url

    member this.GetIdentifier() = DynObj.tryGetTypedValue<string> (nameof identifier) this |> Option.get
    static member getIdentifier = fun (ass: Assay) -> ass.GetIdentifier()