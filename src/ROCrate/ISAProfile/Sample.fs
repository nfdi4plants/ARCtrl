namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Sample(id: string, ?additionalType: string) =
    inherit ROCrateObject(id = id, schemaType = "bioschemas.org/Sample", ?additionalType = additionalType)

    static member create(
        // mandatory
        id,
        name,
        ?additionalType,
        ?additionalProperty,
        ?derivesFrom
    ) =
        let s = Sample(id, ?additionalType = additionalType)

        DynObj.setValue s (nameof name) name

        DynObj.setValueOpt s (nameof additionalProperty) additionalProperty
        DynObj.setValueOpt s (nameof derivesFrom) derivesFrom

        s