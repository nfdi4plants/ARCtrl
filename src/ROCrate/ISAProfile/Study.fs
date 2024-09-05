namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Study(
    id,
    identifier,
    ?about,
    ?citation,
    ?comment,
    ?creator,
    ?dateCreated,
    ?dateModified,
    ?datePublished,
    ?description,
    ?hasPart,
    ?headline,
    ?url
) as this = 
    inherit Dataset(id, "Study")
    do
        DynObj.setValue this (nameof identifier) identifier

        DynObj.setValueOpt this (nameof about) about
        DynObj.setValueOpt this (nameof citation) citation
        DynObj.setValueOpt this (nameof comment) comment
        DynObj.setValueOpt this (nameof creator) creator
        DynObj.setValueOpt this (nameof dateCreated) dateCreated
        DynObj.setValueOpt this (nameof dateModified) dateModified
        DynObj.setValueOpt this (nameof datePublished) datePublished
        DynObj.setValueOpt this (nameof description) description
        DynObj.setValueOpt this (nameof hasPart) hasPart
        DynObj.setValueOpt this (nameof headline) headline
        DynObj.setValueOpt this (nameof url) url

