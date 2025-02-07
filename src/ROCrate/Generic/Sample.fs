namespace ARCtrl.ROCrate.Generic

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Sample =   
    do
        DynObj.setProperty (nameof name) name this

        DynObj.setOptionalProperty (nameof additionalProperty) additionalProperty this
        DynObj.setOptionalProperty (nameof derivesFrom) derivesFrom               this

    member this.GetName() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "Sample" (nameof name) this
    static member getName = fun (s: Sample) -> s.GetName()


    static member 