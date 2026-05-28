module ARCtrl.CWL.DynamicObjHelpers

open System
open DynamicObj

let toCompilerBackingName (propertyName: string) =
    let propertyName =
        if String.IsNullOrEmpty propertyName then
            propertyName
        elif propertyName.EndsWith("_", StringComparison.Ordinal) then
            propertyName.Substring(0, propertyName.Length - 1)
        else
            propertyName

    let chars = propertyName.ToCharArray()
    let mutable index = 0
    let mutable keepLowering = true
    while keepLowering && index < chars.Length && Char.IsUpper chars.[index] do
        let nextIsEnd = index + 1 = chars.Length
        let nextIsUpper = not nextIsEnd && Char.IsUpper chars.[index + 1]
        if index = 0 || nextIsEnd || nextIsUpper then
            chars.[index] <- Char.ToLowerInvariant chars.[index]
            index <- index + 1
        else
            keepLowering <- false

    "_" + String(chars)

let typedBackingFieldNames (dynObj: DynamicObj) =
    dynObj.GetPropertyHelpers(true)
    |> Seq.filter (fun property -> not property.IsDynamic)
    |> Seq.collect (fun property ->
        seq {
            yield "_" + property.Name
            yield toCompilerBackingName property.Name
        })
    |> Set.ofSeq

let dynamicPropertiesExcept (knownFieldNames: seq<string>) (dynObj: DynamicObj) =
    let knownFieldSet = knownFieldNames |> Set.ofSeq
    let typedBackingFields = typedBackingFieldNames dynObj
    dynObj.GetProperties(false)
    |> Seq.filter (fun kv ->
        not (Set.contains kv.Key knownFieldSet) &&
        not (Set.contains kv.Key typedBackingFields))

let dynamicPropertiesSnapshotExcept (knownFieldNames: seq<string>) (dynObj: DynamicObj) =
    dynamicPropertiesExcept knownFieldNames dynObj
    |> Seq.map (fun kv -> kv.Key, kv.Value)
    |> Seq.sortBy fst
    |> Seq.toList

let dynamicPropertiesSnapshot (dynObj: DynamicObj) =
    dynamicPropertiesSnapshotExcept Seq.empty dynObj

let dynamicPropertiesEqual (left: DynamicObj) (right: DynamicObj) =
    dynamicPropertiesSnapshot left = dynamicPropertiesSnapshot right

let hashDynamicProperties (dynObj: DynamicObj) =
    dynamicPropertiesSnapshot dynObj
    |> Operators.hash
