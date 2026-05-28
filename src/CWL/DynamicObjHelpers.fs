module ARCtrl.CWL.DynamicObjHelpers

open System
open DynamicObj

/// Convert a public F# property name to the private backing-field name that
/// DynamicObj exposes for typed members compiled by Fable or .NET.
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

/// Return both common backing-field spellings for all typed properties so they
/// can be excluded from dynamic CWL extension fields.
let typedBackingFieldNames (dynObj: DynamicObj) =
    dynObj.GetPropertyHelpers(true)
    |> Seq.filter (fun property -> not property.IsDynamic)
    |> Seq.collect (fun property ->
        seq {
            yield "_" + property.Name
            yield toCompilerBackingName property.Name
        })
    |> Set.ofSeq

/// Enumerate only true dynamic properties, excluding known CWL fields and
/// compiler-generated storage for typed members.
let dynamicPropertiesExcept (knownFieldNames: Set<string>) (dynObj: DynamicObj) =
    let typedBackingFields = typedBackingFieldNames dynObj
    dynObj.GetProperties(false)
    |> Seq.filter (fun kv ->
        not (Set.contains kv.Key knownFieldNames) &&
        not (Set.contains kv.Key typedBackingFields))

/// Create a stable, sorted dynamic-property snapshot for equality and hashing.
let dynamicPropertiesSnapshotExcept (knownFieldNames: Set<string>) (dynObj: DynamicObj) =
    dynamicPropertiesExcept knownFieldNames dynObj
    |> Seq.map (fun kv -> kv.Key, kv.Value)
    |> Seq.sortBy fst
    |> Seq.toList
    
/// Snapshot all dynamic extension properties on an object.
let dynamicPropertiesSnapshot (dynObj: DynamicObj) =
    dynamicPropertiesSnapshotExcept Set.empty dynObj

/// Compare only preserved dynamic extension properties between two CWL objects.
let dynamicPropertiesEqual (left: DynamicObj) (right: DynamicObj) =
    dynamicPropertiesSnapshot left = dynamicPropertiesSnapshot right

/// Hash preserved dynamic extension properties in the same stable order used
/// by dynamicPropertiesEqual.
let hashDynamicProperties (dynObj: DynamicObj) =
    dynamicPropertiesSnapshot dynObj
    |> Operators.hash
