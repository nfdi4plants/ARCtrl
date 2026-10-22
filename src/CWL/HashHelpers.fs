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


/// Two sequences are equal when they have the same length and equal elements in the same order. The equality of a
/// ResizeArray compares references, so the objects of the CWL model compare their lists with this.
let seqEqual (left: seq<'a>) (right: seq<'a>) : bool when 'a : equality =
    let leftItems = Seq.toArray left
    let rightItems = Seq.toArray right
    leftItems.Length = rightItems.Length && Array.forall2 (=) leftItems rightItems

/// Two optional lists are equal when both are missing or both hold equal elements in the same order.
let optionSeqEqual (left: ResizeArray<'a> option) (right: ResizeArray<'a> option) : bool when 'a : equality =
    match left, right with
    | None, None -> true
    | Some leftItems, Some rightItems -> seqEqual leftItems rightItems
    | _ -> false

/// A hash of the elements in order, to go with seqEqual.
let hashSeq (items: seq<'a>) : int =
    items |> Seq.fold (fun acc item -> mergeHashes acc (hash item)) 0

/// A hash of an optional list, to go with optionSeqEqual.
let hashOptionSeq (items: ResizeArray<'a> option) : int =
    match items with
    | Some items -> mergeHashes 1 (hashSeq items)
    | None -> 0

/// Two raw YAML nodes of a CWL object are equal when they hold the same data. They have the same shape and the same
/// tags, and their keys and scalar texts agree in order. A plain scalar differs from a quoted one, because the core
/// schema reads only the plain one as a number, a bool or null. Comments, blank lines, anchors, collection styles,
/// the kind of quote and spans do not count, so the comparison ignores what a write and a read can change.
let yamlNodeEqual (left: YamlNode) (right: YamlNode) : bool =
    let tagOf (node: YamlNode) = node.Properties.Tag |> Option.map YamlTag.identifier
    let isPlain (scalar: YamlScalar) =
        match scalar.Style with
        | ScalarStyle.Plain -> true
        | _ -> false
    // The pairs that are still to compare. The walk keeps its own stack, so a node of any depth is safe.
    let pending = ResizeArray<YamlNode * YamlNode>()
    pending.Add((left, right))
    let mutable equal = true
    while equal && pending.Count > 0 do
        let (leftNode, rightNode) = pending.[pending.Count - 1]
        pending.RemoveAt(pending.Count - 1)
        if tagOf leftNode <> tagOf rightNode then
            equal <- false
        else
            match leftNode.Kind, rightNode.Kind with
            | YamlNodeKind.Scalar leftScalar, YamlNodeKind.Scalar rightScalar ->
                equal <- leftScalar.Value = rightScalar.Value && isPlain leftScalar = isPlain rightScalar
            | YamlNodeKind.Alias leftName, YamlNodeKind.Alias rightName ->
                equal <- leftName = rightName
            | YamlNodeKind.Sequence leftCollection, YamlNodeKind.Sequence rightCollection ->
                if leftCollection.Items.Length <> rightCollection.Items.Length then
                    equal <- false
                else
                    List.iter2
                        (fun (leftItem: SequenceItem) (rightItem: SequenceItem) -> pending.Add((leftItem.Node, rightItem.Node)))
                        leftCollection.Items
                        rightCollection.Items
            | YamlNodeKind.Mapping leftCollection, YamlNodeKind.Mapping rightCollection ->
                if leftCollection.Items.Length <> rightCollection.Items.Length then
                    equal <- false
                else
                    List.iter2
                        (fun (leftEntry: MappingEntry) (rightEntry: MappingEntry) ->
                            pending.Add((leftEntry.Key, rightEntry.Key))
                            pending.Add((leftEntry.Value, rightEntry.Value)))
                        leftCollection.Items
                        rightCollection.Items
            | _ -> equal <- false
    equal

/// A collection whose hash is still being folded.
type private OpenCollection =
    { Children: YamlNode[]
      mutable Next: int
      mutable Acc: int }

/// A hash that agrees with yamlNodeEqual. It reads the shape of the node and its scalar texts, so two equal nodes
/// always have the same hash.
let hashYamlNode (root: YamlNode) : int =
    // A collection has a seed and the nodes that it folds in, a mapping as key and value one after the other.
    // Every other node has a hash of its own.
    let seedAndChildren (node: YamlNode) : (int * YamlNode[]) option =
        match node.Kind with
        | YamlNodeKind.Sequence collection ->
            Some (3, [| for item in collection.Items -> item.Node |])
        | YamlNodeKind.Mapping collection ->
            Some (4, [| for entry in collection.Items do
                            yield entry.Key
                            yield entry.Value |])
        | _ -> None
    let leafHash (node: YamlNode) : int =
        match node.Kind with
        | YamlNodeKind.Scalar scalar -> mergeHashes 1 (scalar.Value.GetHashCode())
        | YamlNodeKind.Alias name -> mergeHashes 2 (name.GetHashCode())
        | _ -> 0
    // The walk keeps its own stack, so a node of any depth is safe.
    let unfinished = ResizeArray<OpenCollection>()
    let enter (node: YamlNode) : bool =
        match seedAndChildren node with
        | Some (seed, children) ->
            unfinished.Add { Children = children; Next = 0; Acc = seed }
            true
        | None -> false
    let mutable result = 0
    if not (enter root) then
        result <- leafHash root
    while unfinished.Count > 0 do
        let top = unfinished.[unfinished.Count - 1]
        if top.Next < top.Children.Length then
            let child = top.Children.[top.Next]
            top.Next <- top.Next + 1
            if not (enter child) then
                top.Acc <- mergeHashes top.Acc (leafHash child)
        else
            unfinished.RemoveAt(unfinished.Count - 1)
            if unfinished.Count = 0 then
                result <- top.Acc
            else
                let parent = unfinished.[unfinished.Count - 1]
                parent.Acc <- mergeHashes parent.Acc top.Acc
    result
