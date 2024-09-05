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
        DynObj.setValue this (nameof name) name

        DynObj.setValueOpt this (nameof additionalProperty) additionalProperty
        DynObj.setValueOpt this (nameof derivesFrom) derivesFrom
