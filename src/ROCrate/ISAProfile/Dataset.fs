namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Dataset (id: string, ?additionalType: ResizeArray<string>) =
    inherit LDObject(
        id = id,
        schemaType = ResizeArray[|"schema.org/Dataset"|],
        additionalType = defaultArg additionalType (ResizeArray[||])
    )
