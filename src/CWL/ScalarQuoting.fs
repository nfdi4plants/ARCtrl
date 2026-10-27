/// How the writer spells a scalar so that cwltool reads it as written.
///
/// cwltool loads documents and job files with the YAML loader of ruamel.yaml. That loader reads more plain scalars as
/// other types than the core schema of YAML 1.2.2 does. Measured with cwltool, it types the markers = and <<, numbers
/// with underscores (1_000, 1_0.5), binary ints (0b101, 0b_1) and hex ints with an underscore (0x1_F, 0x_1F). It reads
/// a float with a dot in front of its digits and an exponent without a sign (.5e3) as a string, and it fails on
/// spellings such as ._ and 0b_. A string that the core schema or this loader would read as another type is written in
/// double quotes, so cwltool reads it as a string.
///
/// The loader reads the words yes, no, on and off, the sexagesimal numbers (1:30) and the dates as strings. A reader of
/// YAML 1.1 reads the words as bools and the sexagesimal numbers as numbers, so the writer quotes them too. It quotes more
/// than the core schema needs on purpose, so that a string stays a string for both kinds of reader. The reader follows
/// the core schema only.
module ARCtrl.CWL.ScalarQuoting

open System.Text.RegularExpressions
open YAMLicious

let private boolWords = Regex("^(?:y|Y|yes|Yes|YES|n|N|no|No|NO|on|On|ON|off|Off|OFF)$")

let private intSpellings =
    Regex("^[-+]?(?:0b[0-1_]+|0o?[0-7_]+|[0-9_]+|0x[0-9a-fA-F_]+|[1-9][0-9_]*(?::[0-5]?[0-9])+)$")

let private floatSpellings =
    Regex(
        "^(?:[-+]?(?:[0-9][0-9_]*\\.[0-9_]*(?:[eE][-+]?[0-9]+)?|[0-9][0-9_]*[eE][-+]?[0-9]+|\\.[0-9_]+(?:[eE][-+][0-9]+)?|[0-9][0-9_]*(?::[0-5]?[0-9])+\\.[0-9_]*|\\.(?:inf|Inf|INF))|\\.(?:nan|NaN|NAN))$"
    )

/// A float of the core schema that has a dot in front of its digits and an exponent without a sign. The loader of
/// cwltool reads it as a string.
let private dotFloatWithUnsignedExponent = Regex("^[-+]?\\.[0-9]+[eE][0-9]+$")

/// A text with a line break never matches, so `$` cannot match in front of a final line break.
let private matches (pattern: Regex) (text: string) : bool =
    not (text.Contains("\n")) && pattern.IsMatch text

/// True when the loader of cwltool or a reader of YAML 1.1 reads the text, written as a plain scalar, as another type than
/// a string, and the core schema reads it as a string. The loader types the markers, the numbers with underscores and the
/// binary ints (see the description of the module). The words yes, no, on and off and the sexagesimal numbers are in the
/// match for the readers of YAML 1.1, because the loader reads them as strings. The match is on the whole text and it is
/// case-sensitive. A spelling that the loader recognizes but cannot construct, such as ._ and 0b_, counts too, because
/// the loader fails on it.
let readsAsAnotherTypeThanCore (text: string) : bool =
    text = "="
    || text = "<<"
    || matches boolWords text
    || (text.Length > 0 && "-+0123456789".IndexOf text.[0] >= 0 && matches intSpellings text)
    || matches floatSpellings text

/// True when the loader of cwltool reads the text, written as a plain scalar, as another string than the core schema
/// does, or fails on it. The loader fails on a tab inside a plain scalar. It reads a NEL (U+0085), a line separator
/// (U+2028) and a paragraph separator (U+2029) as line breaks, which fold or vanish at the ends of the text. A ? at the
/// start opens a key in a flow collection. The writer cannot see in which collection a scalar stands, so it quotes the
/// text wherever it stands.
let private readsAsAnotherString (text: string) : bool =
    text.StartsWith "?" || text |> Seq.exists (fun c -> c = '\t' || c = '\u0085' || c = '\u2028' || c = '\u2029')

/// True when a string needs double quotes to stay a string for the core schema or for the loader of cwltool.
let readsAsAnotherType (text: string) : bool =
    CoreSchema.resolvesToNonString text || readsAsAnotherTypeThanCore text || readsAsAnotherString text

/// A plain scalar that a reader would take for another type than a string gets double quotes, so the text stays a
/// string. A scalar in any other style is a string already, and so is a scalar with an explicit tag.
let keepAsText (node: YamlNode) : YamlNode =
    match node.Kind with
    | YamlNodeKind.Scalar scalar when node.Properties.Tag.IsNone ->
        match scalar.Style with
        | ScalarStyle.Plain when readsAsAnotherType scalar.Value ->
            { node with Kind = YamlNodeKind.Scalar { scalar with Style = ScalarStyle.DoubleQuoted } }
        | _ -> node
    | _ -> node

/// The scalar for a text of a document that stays a string, such as a key or an id.
let textNode (text: string) : YamlNode =
    keepAsText (Encode.string text)

/// A scalar of a value that the document keeps as it was written, in the form that the writer spells. A plain scalar
/// that the core schema reads as a string and that the loader of cwltool reads as another type gets double quotes. A
/// plain float with a dot in front of its digits and an exponent without a sign gets a zero in front of the dot,
/// because the loader reads it as a string. Every other scalar stays as it is, so the function is idempotent. The result
/// is none for a scalar that stays as it is.
let tryNormalizeScalar (scalar: YamlScalar) : YamlScalar option =
    match scalar.Style with
    | ScalarStyle.Plain ->
        match CoreSchema.resolveKind scalar.Value with
        | CoreKind.Str when readsAsAnotherTypeThanCore scalar.Value -> Some { scalar with Style = ScalarStyle.DoubleQuoted }
        | CoreKind.Float when matches dotFloatWithUnsignedExponent scalar.Value ->
            let sign = if scalar.Value.[0] = '-' || scalar.Value.[0] = '+' then scalar.Value.Substring(0, 1) else ""
            Some { scalar with Value = sign + "0" + scalar.Value.Substring sign.Length }
        | _ -> None
    | _ -> None

/// The scalar in the form that the writer spells (see tryNormalizeScalar).
let normalizeScalar (scalar: YamlScalar) : YamlScalar =
    defaultArg (tryNormalizeScalar scalar) scalar

/// A collection of the tree whose nodes below it are rebuilt.
type private RebuildFrame =
    { Original: YamlNode
      Children: YamlNode[]
      Rebuilt: ResizeArray<YamlNode>
      mutable Changed: bool }

/// The nodes directly below a node in the order of the text: the key and the value of each entry of a mapping, and the
/// node of each item of a sequence.
let private childrenOf (node: YamlNode) : YamlNode[] =
    match node.Kind with
    | YamlNodeKind.Mapping collection ->
        [| for entry in collection.Items do
               yield entry.Key
               yield entry.Value |]
    | YamlNodeKind.Sequence collection -> [| for item in collection.Items -> item.Node |]
    | _ -> [||]

/// The node with the nodes below it replaced, in the order of childrenOf.
let private withChildren (node: YamlNode) (children: ResizeArray<YamlNode>) : YamlNode =
    match node.Kind with
    | YamlNodeKind.Mapping collection ->
        let items =
            collection.Items
            |> List.mapi (fun index entry -> { entry with Key = children.[2 * index]; Value = children.[2 * index + 1] })
        { node with Kind = YamlNodeKind.Mapping { collection with Items = items } }
    | YamlNodeKind.Sequence collection ->
        let items = collection.Items |> List.mapi (fun index item -> { item with Node = children.[index] })
        { node with Kind = YamlNodeKind.Sequence { collection with Items = items } }
    | _ -> node

/// The tree with every scalar, key or value, in the form that the writer spells (see normalizeScalar). A node
/// that needs no change stays the same object. The walk keeps its own stack, so a tree of any depth is safe.
let normalizeTree (root: YamlNode) : YamlNode =
    let normalizeLeaf (node: YamlNode) : YamlNode * bool =
        match node.Kind with
        | YamlNodeKind.Scalar scalar ->
            match tryNormalizeScalar scalar with
            | Some normalized -> { node with Kind = YamlNodeKind.Scalar normalized }, true
            | None -> node, false
        | _ -> node, false
    let frames = ResizeArray<RebuildFrame>()
    let enter (node: YamlNode) : (YamlNode * bool) option =
        let children = childrenOf node
        if children.Length = 0 then
            Some (normalizeLeaf node)
        else
            frames.Add { Original = node; Children = children; Rebuilt = ResizeArray<YamlNode>(); Changed = false }
            None
    let mutable result = root, false
    match enter root with
    | Some leaf -> result <- leaf
    | None ->
        while frames.Count > 0 do
            let top = frames.[frames.Count - 1]
            if top.Rebuilt.Count < top.Children.Length then
                let child = top.Children.[top.Rebuilt.Count]
                match enter child with
                | Some (node, changed) ->
                    top.Rebuilt.Add node
                    if changed then top.Changed <- true
                | None -> ()
            else
                frames.RemoveAt(frames.Count - 1)
                let rebuilt = if top.Changed then withChildren top.Original top.Rebuilt else top.Original
                if frames.Count = 0 then
                    result <- rebuilt, top.Changed
                else
                    let parent = frames.[frames.Count - 1]
                    parent.Rebuilt.Add rebuilt
                    if top.Changed then parent.Changed <- true
    fst result
