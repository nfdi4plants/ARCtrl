namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Data(
    id,
    name,
    ?additionalType,
    ?comment,
    ?encodingFormat,
    ?disambiguatingDescription
) as this =
    inherit ROCrateObject(id = id, schemaType = "schema.org/MediaObject", ?additionalType = additionalType)
    do
        DynObj.setValue this (nameof name) name

        DynObj.setValueOpt this (nameof comment) comment
        DynObj.setValueOpt this (nameof encodingFormat) encodingFormat
        DynObj.setValueOpt this (nameof disambiguatingDescription) disambiguatingDescription

