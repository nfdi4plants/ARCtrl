namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Sample(
    id,
    name,
    ?additionalType,
    ?additionalProperty,
    ?derivesFrom
) as this =
    inherit ROCrateObject(id = id, schemaType = "bioschemas.org/Sample", ?additionalType = additionalType)
    do
        DynObj.setProperty (nameof name) name this

        DynObj.setOptionalProperty (nameof additionalProperty) additionalProperty this
        DynObj.setOptionalProperty (nameof derivesFrom) derivesFrom               this

    member this.GetName() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "Sample" (nameof name) this
    static member getName = fun (s: Sample) -> s.GetName()
