namespace ARCtrl.CWL

open DynamicObj
open Fable.Core
open System

[<AttachMembers>]
type CWLParameterReference(key : string, ?values: string ResizeArray, ?value: CWLParameterValue, ?type_: CWLType) =
    inherit DynamicObj ()

    let mutable _key = key
    let mutable _value =
        value
        |> Option.orElseWith (fun () ->
            values
            |> Option.bind CWLParameterValue.fromFlatStrings
        )
    let mutable _type = type_

    member this.Key
        with get() = _key
        and set(value) = _key <- value

    member this.Value
        with get() = _value
        and set(value) = _value <- value

    member this.Values
        with get() =
            _value
            |> Option.map CWLParameterValue.toFlatStrings
            |> Option.defaultValue (ResizeArray())
        and set(values) = _value <- CWLParameterValue.fromFlatStrings values

    member this.Type
        with get() = _type
        and set(value) = _type <- value

    override this.GetHashCode() =
        [|
            HashHelpers.hash this.Key |> box
            HashHelpers.boxHashOption this.Value
            HashHelpers.boxHashOption this.Type
            DynamicObjHelpers.hashDynamicProperties this
        |]
        |> HashHelpers.boxHashArray
        |> fun x -> x :?> int

    override this.Equals (obj: obj) : bool = 
        match obj with
        | :? CWLParameterReference as other -> this.StructurallyEquals other
        | _ -> false

    member this.StructurallyEquals (other: CWLParameterReference) : bool =
        this.Key = other.Key &&
        this.Value = other.Value &&
        this.Type = other.Type &&
        DynamicObjHelpers.dynamicPropertiesEqual this other

    member this.ReferenceEquals (other: CWLParameterReference) : bool =
        System.Object.ReferenceEquals(this,other)

    static member KnownFieldNames =
        CWLKnownFieldNames.parameterReference
