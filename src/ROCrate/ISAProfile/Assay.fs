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

        DynObj.setOptionalProperty (nameof measurementMethod) measurementMethod this
        DynObj.setOptionalProperty (nameof measurementTechnique) measurementTechnique this
        DynObj.setOptionalProperty (nameof variableMeasured) variableMeasured this
        DynObj.setOptionalProperty (nameof about) about this
        DynObj.setOptionalProperty (nameof comment) comment this
        DynObj.setOptionalProperty (nameof creator) creator this
        DynObj.setOptionalProperty (nameof hasPart) hasPart this
        DynObj.setOptionalProperty (nameof url) url this

    member this.GetIdentifier() = DynObj.tryGetTypedPropertyValue<string> (nameof identifier) this |> Option.get
    static member getIdentifier = fun (ass: Assay) -> ass.GetIdentifier()