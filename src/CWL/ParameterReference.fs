namespace ARCtrl.CWL

open Fable.Core
open System

/// An entry of a job file: the key of a process input and the CWL value that the file gives it. The value is a plain
/// CWL value, so the file has no place for the type of the input. The type belongs to the process that runs.
[<AttachMembers>]
type CWLParameterReference(key : string, ?values: string ResizeArray, ?value: CWLParameterValue) =

    let mutable _key = key
    let mutable _value =
        value
        |> Option.orElseWith (fun () ->
            values
            |> Option.bind CWLParameterValue.fromFlatStrings
        )

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

    override this.GetHashCode() =
        [|
            HashHelpers.hash this.Key |> box
            HashHelpers.boxHashOption this.Value
        |]
        |> HashHelpers.boxHashArray
        |> fun x -> x :?> int

    override this.Equals (obj: obj) : bool =
        match obj with
        | :? CWLParameterReference as other -> this.StructurallyEquals other
        | _ -> false

    member this.StructurallyEquals (other: CWLParameterReference) : bool =
        this.Key = other.Key &&
        this.Value = other.Value

    member this.ReferenceEquals (other: CWLParameterReference) : bool =
        System.Object.ReferenceEquals(this,other)
