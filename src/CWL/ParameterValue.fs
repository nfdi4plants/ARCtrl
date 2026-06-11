namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

[<AttachMembers>]
type CWLParameterRecordField(name: string, value: CWLParameterValue) =

    member val Name = name with get, set

    member val Value = value with get, set

    override this.Equals(other: obj) =
        match other with
        | :? CWLParameterRecordField as other ->
            this.Name = other.Name && this.Value = other.Value
        | _ -> false

    override this.GetHashCode() =
        hash (this.Name, this.Value)

and [<CustomEquality; NoComparison; RequireQualifiedAccess; AttachMembers>] CWLParameterValue =
    | Null
    | String of string
    | Int of int64
    | Float of float
    | Boolean of bool
    | File of FileInstance
    | Directory of DirectoryInstance
    | Array of ResizeArray<CWLParameterValue>
    | Record of ResizeArray<CWLParameterRecordField>

    override this.Equals(other: obj) =
        let resizeArrayEqual (left: ResizeArray<'T>) (right: ResizeArray<'T>) =
            left.Count = right.Count && Seq.forall2 (=) left right

        match other with
        | :? CWLParameterValue as other ->
            match this, other with
            | Null, Null -> true
            | String left, String right -> left = right
            | Int left, Int right -> left = right
            | Float left, Float right -> left = right
            | Boolean left, Boolean right -> left = right
            | File left, File right -> left.Equals right
            | Directory left, Directory right -> left.Equals right
            | Array left, Array right -> resizeArrayEqual left right
            | Record left, Record right -> resizeArrayEqual left right
            | _ -> false
        | _ -> false

    override this.GetHashCode() =
        match this with
        | Null -> hash 0
        | String value -> hash (1, value)
        | Int value -> hash (2, value)
        | Float value -> hash (3, value)
        | Boolean value -> hash (4, value)
        | File value -> hash (5, value)
        | Directory value -> hash (6, value)
        | Array values -> hash (7, values |> Seq.map hash |> Seq.toArray)
        | Record fields -> hash (8, fields |> Seq.map hash |> Seq.toArray)

module CWLParameterValue =

    let private tryDynamicString name (value: DynamicObj) =
        DynObj.tryGetTypedPropertyValue<string> name value

    let rec private inferredTypesEqual left right =
        match left, right with
        | CWLType.File _, CWLType.File _
        | CWLType.Directory _, CWLType.Directory _ -> true
        | CWLType.Array left, CWLType.Array right -> inferredTypesEqual left.Items right.Items
        | _ -> left = right

    let private getPathOrLocation (value: DynamicObj) =
        tryDynamicString "path" value
        |> Option.orElse (tryDynamicString "location" value)
        |> Option.defaultValue ""

    let rec toFlatStrings value =
        match value with
        | CWLParameterValue.Null -> ResizeArray()
        | CWLParameterValue.String value -> ResizeArray [| value |]
        | CWLParameterValue.Int value -> ResizeArray [| string value |]
        | CWLParameterValue.Float value ->
            ResizeArray [| value.ToString("R", System.Globalization.CultureInfo.InvariantCulture) |]
        | CWLParameterValue.Boolean value -> ResizeArray [| if value then "true" else "false" |]
        | CWLParameterValue.File file -> ResizeArray [| getPathOrLocation file |]
        | CWLParameterValue.Directory directory -> ResizeArray [| getPathOrLocation directory |]
        | CWLParameterValue.Array values ->
            values
            |> Seq.collect (fun value -> toFlatStrings value :> seq<string>)
            |> ResizeArray
        | CWLParameterValue.Record fields ->
            fields
            |> Seq.collect (fun field -> toFlatStrings field.Value :> seq<string>)
            |> ResizeArray

    let fromFlatStrings (values: ResizeArray<string>) =
        match values.Count with
        | 0 -> None
        | 1 -> Some (CWLParameterValue.String values.[0])
        | _ ->
            values
            |> Seq.map CWLParameterValue.String
            |> ResizeArray
            |> CWLParameterValue.Array
            |> Some

    let rec tryInferType value =
        match value with
        | CWLParameterValue.File _ -> Some (CWLType.file())
        | CWLParameterValue.Directory _ -> Some (CWLType.directory())
        | CWLParameterValue.Array values when values.Count > 0 ->
            let inferredItemTypes =
                values
                |> Seq.map tryInferType
                |> Seq.toArray

            match inferredItemTypes.[0] with
            | Some firstType
                when inferredItemTypes
                     |> Array.forall (function
                         | Some itemType -> inferredTypesEqual firstType itemType
                         | None -> false) ->
                Some (CWLType.Array { Items = firstType; Label = None; Doc = None; Name = None })
            | _ -> None
        | _ -> None
