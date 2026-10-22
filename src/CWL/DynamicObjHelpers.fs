module ARCtrl.CWL.DynamicObjHelpers

open System
open DynamicObj
open Fable.Core
open Fable.Core.PyInterop

/// Read a dynamically stored collection consistently across runtimes.
///
/// Fable Python represents ResizeArray as a native list, so its generated
/// ResizeArray type test always fails unless handled explicitly.
let inline tryGetTypedPropertyValueAsResizeArray<'T> (name: string) (dynObj: DynamicObj) =
    match dynObj.TryGetPropertyValue(name) with
#if FABLE_COMPILER_PYTHON
    | Some value when emitPyExpr value "isinstance($0, list)" ->
        Some (value :?> ResizeArray<'T>)
#else
    | Some (:? ResizeArray<'T> as values) -> Some values
#endif
    | Some (:? 'T as singleton) -> Some (ResizeArray [ singleton ])
    | _ -> None

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

/// The items of the value of a dynamic property when it is a list. The decoder builds a ResizeArray of objects and a
/// program may store a ResizeArray of any element type. A string is no list. On .NET the list interface finds every
/// kind of list. Python holds a ResizeArray as a native list, which its generated type test never finds, and
/// JavaScript holds it as an array whatever the element type.
let private tryItems (value: obj) : ResizeArray<obj> option =
#if FABLE_COMPILER_PYTHON
    if emitPyExpr value "isinstance($0, list)" then Some (value :?> ResizeArray<obj>) else None
#else
#if FABLE_COMPILER
    match value with
    | :? ResizeArray<obj> as items -> Some items
    | _ -> None
#else
    match value with
    | :? System.Collections.IList as items -> Some (ResizeArray (Seq.cast<obj> items))
    | _ -> None
#endif
#endif

/// The values of two dynamic properties are equal when they are equal objects. A ResizeArray compares references, so
/// lists compare by their elements, down to the lists inside them.
let rec dynamicValuesEqual (left: obj) (right: obj) : bool =
    match tryItems left, tryItems right with
    | Some leftItems, Some rightItems ->
        leftItems.Count = rightItems.Count && Seq.forall2 dynamicValuesEqual leftItems rightItems
    | _ -> left = right

/// A hash of the value of a dynamic property that goes with dynamicValuesEqual.
let rec hashDynamicValue (value: obj) : int =
    match tryItems value with
    | Some items -> items |> Seq.fold (fun acc item -> HashHelpers.mergeHashes acc (hashDynamicValue item)) 0
    | None -> Operators.hash value

/// Compare only preserved dynamic extension properties between two CWL objects.
let dynamicPropertiesEqual (left: DynamicObj) (right: DynamicObj) =
    let leftProperties = dynamicPropertiesSnapshot left
    let rightProperties = dynamicPropertiesSnapshot right
    leftProperties.Length = rightProperties.Length
    && List.forall2
        (fun (leftName: string, leftValue: obj) (rightName: string, rightValue: obj) ->
            leftName = rightName && dynamicValuesEqual leftValue rightValue)
        leftProperties
        rightProperties

/// Hash preserved dynamic extension properties in the same stable order used
/// by dynamicPropertiesEqual.
let hashDynamicProperties (dynObj: DynamicObj) =
    dynamicPropertiesSnapshot dynObj
    |> List.map (fun (name, value) -> name, hashDynamicValue value)
    |> Operators.hash
