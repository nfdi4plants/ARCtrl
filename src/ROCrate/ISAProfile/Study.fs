namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Study(id: string) =
    // inheritance
    inherit Dataset(id, "Study")
    static member create(
        // mandatory
        id,
        // Properties from Thing
        identifier,
        // optional
        // Properties from CreativeWork
        ?about,
        ?citation,
        ?comment,
        ?creator,
        ?dateCreated,
        ?dateModified,
        ?datePublished,
        ?hasPart,
        ?headline,
        ?url,
        // Properties from Thing
        ?description
    ) =
        let ds = Study(id = id)

        // Properties from CreativeWork
        DynObj.setValueOpt ds (nameof about) about
        DynObj.setValueOpt ds (nameof citation) citation
        DynObj.setValueOpt ds (nameof comment) comment
        DynObj.setValueOpt ds (nameof creator) creator
        DynObj.setValueOpt ds (nameof dateCreated) dateCreated
        DynObj.setValueOpt ds (nameof dateModified) dateModified
        DynObj.setValueOpt ds (nameof datePublished) datePublished
        DynObj.setValueOpt ds (nameof hasPart) hasPart
        DynObj.setValueOpt ds (nameof headline) headline
        DynObj.setValueOpt ds (nameof url) url

        // Properties from Thing
        DynObj.setValueOpt ds (nameof description) description
        DynObj.setValue ds (nameof identifier) identifier

        ds
