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

        DynObj.setProperty (nameof headline) headline     this
        DynObj.setProperty (nameof identifier) identifier this

        DynObj.setOptionalProperty (nameof author) author this
        DynObj.setOptionalProperty (nameof url) url this
        DynObj.setOptionalProperty (nameof creativeWorkStatus) creativeWorkStatus this
        DynObj.setOptionalProperty (nameof disambiguatingDescription) disambiguatingDescription this

    member this.GetHeadline() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "ScholarlyArticle" (nameof headline) this
    static member getHeadline = fun (s: ScholarlyArticle) -> s.GetHeadline()

    member this.GetIdentifier() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "ScholarlyArticle" (nameof identifier) this
    static member getIdentifier = fun (s: ScholarlyArticle) -> s.GetIdentifier()