namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Data(id: string, ?additionalType: string) =
    inherit ROCrateObject(id = id, schemaType = "schema.org/MediaObject", ?additionalType = additionalType)

    static member create(
        // mandatory
        id,
        name,
        ?additionalType,
        ?comment,
        ?encodingFormat,
        ?disambiguatingDescription
    ) =
        let d = Data(id, ?additionalType = additionalType)

        DynObj.setValue d (nameof name) name

        DynObj.setValueOpt d (nameof comment) comment
        DynObj.setValueOpt d (nameof encodingFormat) encodingFormat
        DynObj.setValueOpt d (nameof disambiguatingDescription) disambiguatingDescription

        d