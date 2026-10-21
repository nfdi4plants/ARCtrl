module ARCtrl.CWL.HashHelpers

open YAMLicious

let mergeHashes (hash1 : int) (hash2 : int) : int =
    0x9e3779b9 + hash2 + (hash1 <<< 6) + (hash1 >>> 2)

let hashDateTime (dt : System.DateTime) : int =
    let mutable acc = 0
    acc <- mergeHashes acc dt.Year
    acc <- mergeHashes acc dt.Month
    acc <- mergeHashes acc dt.Day
    acc <- mergeHashes acc dt.Hour
    acc <- mergeHashes acc dt.Minute
    acc <- mergeHashes acc dt.Second
    acc
    

let hash obj =
    obj.GetHashCode()

let boxHashOption (a: 'a option) : obj =
    if a.IsSome then a.Value.GetHashCode() else (0).GetHashCode()
    |> box

let boxHashArray (a: 'a []) : obj =
    a 
    // from https://stackoverflow.com/a/53507559
    |> Array.fold (fun acc o -> 
        hash o
        |> mergeHashes acc) 0
    |> box

let boxHashSeq (a: seq<'a>) : obj =
    a 
    // from https://stackoverflow.com/a/53507559
    |> Seq.fold (fun acc o -> 
        hash o
        |> mergeHashes acc) 0
    |> box


/// Two raw YAML nodes of a CWL object are equal when they hold the same data. They have the same shape, the same
/// keys in the same order and the same scalar texts. A plain scalar differs from a quoted one, because the core
/// schema reads only the plain one as a number, a bool or null, and an explicit tag counts too. Comments, anchors,
/// collection styles, quote styles and spans do not count, so a node survives a write and a read unchanged.
let rec yamlNodeEqual (left: YamlNode) (right: YamlNode) : bool =
    let tagOf (node: YamlNode) = node.Properties.Tag |> Option.map YamlTag.identifier
    let isPlain (scalar: YamlScalar) =
        match scalar.Style with
        | ScalarStyle.Plain -> true
        | _ -> false
    tagOf left = tagOf right
    && (
        match left.Kind, right.Kind with
        | YamlNodeKind.Scalar leftScalar, YamlNodeKind.Scalar rightScalar ->
            leftScalar.Value = rightScalar.Value && isPlain leftScalar = isPlain rightScalar
        | YamlNodeKind.Alias leftName, YamlNodeKind.Alias rightName ->
            leftName = rightName
        | YamlNodeKind.Sequence leftCollection, YamlNodeKind.Sequence rightCollection ->
            leftCollection.Items.Length = rightCollection.Items.Length
            && List.forall2 (fun (leftItem: SequenceItem) (rightItem: SequenceItem) -> yamlNodeEqual leftItem.Node rightItem.Node) leftCollection.Items rightCollection.Items
        | YamlNodeKind.Mapping leftCollection, YamlNodeKind.Mapping rightCollection ->
            leftCollection.Items.Length = rightCollection.Items.Length
            && List.forall2
                (fun (leftEntry: MappingEntry) (rightEntry: MappingEntry) ->
                    yamlNodeEqual leftEntry.Key rightEntry.Key && yamlNodeEqual leftEntry.Value rightEntry.Value)
                leftCollection.Items
                rightCollection.Items
        | _ -> false
    )

/// A hash that agrees with yamlNodeEqual. It reads the shape of the node and its scalar texts, so two equal nodes
/// always have the same hash.
let rec hashYamlNode (node: YamlNode) : int =
    match node.Kind with
    | YamlNodeKind.Scalar scalar -> mergeHashes 1 (scalar.Value.GetHashCode())
    | YamlNodeKind.Alias name -> mergeHashes 2 (name.GetHashCode())
    | YamlNodeKind.Sequence collection ->
        collection.Items
        |> List.fold (fun acc item -> mergeHashes acc (hashYamlNode item.Node)) 3
    | YamlNodeKind.Mapping collection ->
        collection.Items
        |> List.fold (fun acc entry -> mergeHashes (mergeHashes acc (hashYamlNode entry.Key)) (hashYamlNode entry.Value)) 4
