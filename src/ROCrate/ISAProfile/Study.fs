namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Study(
    id: string,
    identifier: string,
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
        DynObj.setProperty (nameof identifier) identifier this
        DynObj.setOptionalProperty (nameof about) about                 this 
        DynObj.setOptionalProperty (nameof citation) citation           this 
        DynObj.setOptionalProperty (nameof comment) comment             this 
        DynObj.setOptionalProperty (nameof creator) creator             this 
        DynObj.setOptionalProperty (nameof dateCreated) dateCreated     this 
        DynObj.setOptionalProperty (nameof dateModified) dateModified   this 
        DynObj.setOptionalProperty (nameof datePublished) datePublished this 
        DynObj.setOptionalProperty (nameof description) description     this 
        DynObj.setOptionalProperty (nameof hasPart) hasPart             this 
        DynObj.setOptionalProperty (nameof headline) headline           this 
        DynObj.setOptionalProperty (nameof url) url                     this 

    member this.GetIdentifier() = DynObj.tryGetTypedPropertyValue<string> (nameof identifier) this |> Option.get
    static member getIdentifier = fun (inv: Investigation) -> inv.GetIdentifier()
