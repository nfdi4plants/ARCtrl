namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Investigation(
    id: string,
    identifier: string,
    ?citation,
    ?comment,
    ?creator,
    ?dateCreated,
    ?dateModified,
    ?datePublished,
    ?hasPart,
    ?headline,
    ?mentions,
    ?url,
    ?description
) as this =
    inherit Dataset(id, "Investigation")
    do 
        DynObj.setProperty (nameof identifier) identifier this

        DynObj.setOptionalProperty (nameof citation) citation this
        DynObj.setOptionalProperty (nameof comment) comment this
        DynObj.setOptionalProperty (nameof creator) creator this
        DynObj.setOptionalProperty (nameof dateCreated) dateCreated this
        DynObj.setOptionalProperty (nameof dateModified) dateModified this
        DynObj.setOptionalProperty (nameof datePublished) datePublished this
        DynObj.setOptionalProperty (nameof hasPart) hasPart this
        DynObj.setOptionalProperty (nameof headline) headline this
        DynObj.setOptionalProperty (nameof mentions) mentions this
        DynObj.setOptionalProperty (nameof url) url this
        DynObj.setOptionalProperty (nameof description) description this

    member this.GetIdentifier() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "Investigation" (nameof identifier) this
    static member getIdentifier = fun (inv: Investigation) -> inv.GetIdentifier()