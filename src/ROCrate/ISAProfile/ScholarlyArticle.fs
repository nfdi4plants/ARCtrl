namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type ScholarlyArticle(id: string, ?additionalType: string) =
    inherit ROCrateObject(id = id, schemaType = "schema.org/ScholarlyArticle", ?additionalType = additionalType)

    static member create(
        // mandatory
        id,
        headline,
        identifier,
        ?additionalType,
        ?author,
        ?url,
        ?creativeWorkStatus,
        ?disambiguatingDescription
    ) =
        let sa = ScholarlyArticle(id, ?additionalType = additionalType)

        DynObj.setValue sa (nameof headline) headline
        DynObj.setValue sa (nameof identifier) identifier

        DynObj.setValueOpt sa (nameof author) author
        DynObj.setValueOpt sa (nameof url) url
        DynObj.setValueOpt sa (nameof creativeWorkStatus) creativeWorkStatus
        DynObj.setValueOpt sa (nameof disambiguatingDescription) disambiguatingDescription

        sa