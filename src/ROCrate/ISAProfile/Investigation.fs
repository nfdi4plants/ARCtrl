namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Investigation(
    id,
    // Properties from Thing
    identifier,
    // optional
    // Properties from CreativeWork
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
    // Properties from Thing
    ?description
) as this =
    inherit Dataset(id, "Investigation")
    do 
        // Properties from CreativeWork
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

        // Properties from Thing
        DynObj.setValueOpt this (nameof description) description
        DynObj.setValue this (nameof identifier) identifier
