namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
 
///
[<AttachMembers>]
type Dataset (id: string, ?additionalType: string) =
    inherit ROCrateObject(id = id, schemaType = "schema.org/Dataset", ?additionalType = additionalType)
