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

        DynObj.setValueOpt this (nameof citation) citation
        DynObj.setValueOpt this (nameof comment) comment
        DynObj.setValueOpt this (nameof creator) creator
        DynObj.setValueOpt this (nameof dateCreated) dateCreated
        DynObj.setValueOpt this (nameof dateModified) dateModified
        DynObj.setValueOpt this (nameof datePublished) datePublished
        DynObj.setValueOpt this (nameof hasPart) hasPart
        DynObj.setValueOpt this (nameof headline) headline
        DynObj.setValueOpt this (nameof mentions) mentions
        DynObj.setValueOpt this (nameof url) url
        DynObj.setValueOpt this (nameof description) description

    member this.GetIdentifier() = DynObj.tryGetTypedValue<string> (nameof identifier) this |> Option.get
    static member getIdentifier = fun (inv: Investigation) -> inv.GetIdentifier()