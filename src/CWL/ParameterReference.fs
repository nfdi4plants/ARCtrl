namespace ARCtrl.CWL

open DynamicObj
open Fable.Core
open System

[<AttachMembers>]
type CWLParameterReference(key : string, ?values: string ResizeArray, ?type_: CWLType) =
    inherit DynamicObj ()

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
            HashHelpers.hashDynamicProperties this
        |]
        |> HashHelpers.boxHashArray
        |> fun x -> x :?> int

    override this.Equals (obj: obj) : bool = 
        match obj with
        | :? CWLParameterReference as other -> this.StructurallyEquals other
        | _ -> false

    member this.StructurallyEquals (other: CWLParameterReference) : bool =
        this.Key = other.Key &&
        this.Values.Count = other.Values.Count &&
        Seq.forall2 (=) this.Values other.Values &&
        this.Type = other.Type &&
        HashHelpers.dynamicPropertiesEqual this other

    member this.ReferenceEquals (other: CWLParameterReference) : bool =
        System.Object.ReferenceEquals(this,other)

    static member KnownFieldNames =
        ResizeArray [| "class"; "path"; "location"; "type"; "value" |]
