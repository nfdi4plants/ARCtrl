namespace ARCtrl.ROCrate.Generic

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type PropertyValue =
    
    static member name = "name"

    static member value = "value"

    static member propertyID = "propertyID"

    static member unitCode = "unitCode"

    static member unitText = "unitText"

    static member valueReference = "valueReference"

    member this.GetName() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "PropertyValue" (nameof name) this

    static member getName = fun (lp: PropertyValue) -> lp.GetName()

    member this.GetValue() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "PropertyValue" (nameof name) this
    static member getValue = fun (lp: PropertyValue) -> lp.GetValue()

    static member create