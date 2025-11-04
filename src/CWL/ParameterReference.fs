namespace ARCtrl.CWL

open Fable.Core
open System

[<AttachMembers>]
type CWLParameterReference(key : string, ?values: string ResizeArray, ?type_: CWLType) =
    let mutable _key = key
    let mutable _values = defaultArg values (ResizeArray<string>())
    let mutable _type = type_

    member this.Key
        with get() = _key
        and set(value) = _key <- value

    member this.Values
        with get() = _values
        and set(value) = _values <- value

    member this.Type
        with get() = _type
        and set(value) = _type <- value

    override this.GetHashCode() =
        [|
            HashHelpers.boxHashSeq this.Values
            HashHelpers.hash this.Key
            HashHelpers.boxHashOption this.Type
        |]
        |> HashHelpers.boxHashArray
        |> fun x -> x :?> int

    override this.Equals (obj: obj) : bool = 
        this.StructurallyEquals (obj :?> CWLParameterReference)

    member this.StructurallyEquals (other: CWLParameterReference) : bool =
        this.GetHashCode() = other.GetHashCode()

    member this.ReferenceEquals (other: CWLParameterReference) : bool =
        System.Object.ReferenceEquals(this,other)