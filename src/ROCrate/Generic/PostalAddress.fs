namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
///
[<AttachMembers>]
type PostalAddress =

    static member schemaType = "http://schema.org/PostalAddress"

    static member validate(o : LDNode, ?context : LDContext) =
        o.HasType(PostalAddress.schemaType, ?context = context)