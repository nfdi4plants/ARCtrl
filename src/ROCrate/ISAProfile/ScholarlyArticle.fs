namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type ScholarlyArticle(
    id,
    headline,
    identifier,
    ?additionalType,
    ?author,
    ?url,
    ?creativeWorkStatus,
    ?disambiguatingDescription

) as this =
    inherit ROCrateObject(id = id, schemaType = "schema.org/ScholarlyArticle", ?additionalType = additionalType)
    do

        DynObj.setValue this (nameof headline) headline
        DynObj.setValue this (nameof identifier) identifier

        DynObj.setValueOpt this (nameof author) author
        DynObj.setValueOpt this (nameof url) url
        DynObj.setValueOpt this (nameof creativeWorkStatus) creativeWorkStatus
        DynObj.setValueOpt this (nameof disambiguatingDescription) disambiguatingDescription