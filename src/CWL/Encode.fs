namespace ARCtrl.CWL

open System
open DynamicObj
open YAMLicious
/// Encoding helpers and functions mirroring the logic in Decode.fs.
/// Each decode path gets a corresponding encode path that produces
/// YAML key ordering and scalar formatting matching existing fixtures.
module Encode =

    // ------------------------------
    // Type classification helpers - Single source of truth for CWL type categorization
    // ------------------------------
    
    /// Matches the optional type, the list ["null", X]. Schema Salad reads the shorthand X? as exactly this list. A list
    /// with the null behind X is a plain type list, and X? would reorder it.
    let (|OptionalOf|_|) (t: CWLType) : CWLType option =
        match t with
        | Union types ->
            match Seq.toList types with
            | [ Null; otherType ] -> Some otherType
            | _ -> None
        | _ -> None

    /// Active pattern to classify CWL types as primitive (with shorthand) or complex (requiring full YAML)
    let rec (|PrimitiveType|ComplexType|) (t: CWLType) =
        match t with
        // Primitive cases have a direct CWL scalar spelling and can participate
        // in shorthand array syntax.
        | File -> PrimitiveType "File"
        | Directory -> PrimitiveType "Directory"
        | String -> PrimitiveType "string"
        | Int -> PrimitiveType "int"
        | Long -> PrimitiveType "long"
        | Float -> PrimitiveType "float"
        | Double -> PrimitiveType "double"
        | Boolean -> PrimitiveType "boolean"
        | Any -> PrimitiveType "Any"
        | Reference name -> PrimitiveType name
        | Null -> PrimitiveType "null"
        | Stdout -> PrimitiveType "stdout"
        | Stderr -> PrimitiveType "stderr"
        | Stdin -> PrimitiveType "stdin"
        // Schemas and general unions require structured YAML unless a later
        // optional-union special case handles them.
        | Record _ | Enum _ | Union _ | ImportedType _ | IncludedType _ | MixedType _ | RawType _ -> ComplexType
        | Array arraySchema ->
            // Arrays are primitive only if their items are primitive
            match arraySchema.Items with
            | PrimitiveType _ -> PrimitiveType "array"
            | ComplexType -> ComplexType

    /// True when the array schema holds nothing but its items. The shorthand T[] has no place for a label, a doc, a
    /// name, an inputBinding or any other field of the schema.
    let private isPlainArray (schema: InputArraySchema) : bool =
        schema.Label.IsNone
        && schema.Doc.IsNone
        && schema.Name.IsNone
        && Seq.isEmpty (DynamicObjHelpers.dynamicPropertiesExcept InputArraySchema.KnownFieldNames schema)

    /// Try to get shorthand notation for a CWL type (e.g., "File", "string[]", "int[][]")
    /// Returns None for complex types that require full YAML serialization
    let rec tryGetArrayShorthand (cwlType: CWLType) : string option =
        match cwlType with
        | PrimitiveType name when name <> "array" -> Some name
        | Array arraySchema when isPlainArray arraySchema ->
            // Recursively get shorthand for inner type and append []
            tryGetArrayShorthand arraySchema.Items |> Option.map (fun s -> s + "[]")
        | _ -> None

    /// The shorthand of a one-level array in a CWL document, as in File[]. CWL reads one [] after a name, so an array
    /// of arrays has none.
    let private tryGetDocumentArrayShorthand (schema: InputArraySchema) : string option =
        match schema.Items with
        | PrimitiveType name when name <> "array" && isPlainArray schema -> Some (name + "[]")
        | _ -> None

    /// Determine if a type requires full YAML serialization (complex type)
    let rec isComplexType (t: CWLType) : bool =
        match t with
        | Record _ | Enum _ | ImportedType _ | IncludedType _ | MixedType _ | RawType _ -> true
        | Array _ ->
            // Complex if array doesn't have shorthand (array of record/enum, or an array with more than its items)
            tryGetArrayShorthand t |> Option.isNone
        | OptionalOf otherType -> isComplexType otherType
        // Every other union is a type list.
        | Union _ -> true
        | _ -> false

    // ------------------------------
    // Layout helpers for strings and lists
    // ------------------------------

    /// A literal block scalar. The writer keeps a stored chomping that fits the value. Otherwise it takes the
    /// chomping that reads the value back, Clip for one final line feed and Keep for several, so nothing here
    /// derives it.
    let private literalBlock (value: string) : YamlNode =
        YamlNode.scalar (ScalarStyle.Block(BlockScalarStyle.Literal, ChompingMode.Strip, None)) value

    /// A text with a line feed is a literal block scalar. The loader of cwltool reads a NEL, a line separator and a
    /// paragraph separator as line breaks in a block scalar too, so a text that holds one is double-quoted, where the
    /// writer escapes it.
    let private multiLineScalar (value: string) : YamlNode =
        if ScalarQuoting.holdsLineBreakCharacter value then
            YamlNode.scalar ScalarStyle.DoubleQuoted value
        else
            literalBlock value

    /// A string that spans lines is written as a literal block scalar, so a doc text or an expression stays
    /// readable (see multiLineScalar). Any other string goes through Encode.string, which double-quotes text that a core schema
    /// reader takes for a bool, a number or null, and then through ScalarQuoting.keepAsText, which quotes what the loader of
    /// cwltool reads as another type.
    let encodeString (value: string) : YamlNode =
        if value.Contains("\n") then
            multiLineScalar value
        else
            ScalarQuoting.textNode value

    /// The text of a doc or a label. The node that the text was read from, when there is one, takes the current text
    /// with YamlNode.withScalarValue. It comes back unchanged while the text is the same, and an edited text keeps the
    /// style of the node and the lines that still fit. withScalarValue and ScalarQuoting.keepAsText quote an edited text that a reader
    /// would take for another type than a string, so the text stays a string. The node of a doc may be a list of
    /// strings, which comes back unchanged while the text is the concatenation of its items and is a string after an
    /// edit. A text without a node is spelled by encodeString.
    let encodeSourceText (source: YamlNode option) (text: string) : YamlNode =
        match source with
        | Some node ->
            match node.Kind with
            | YamlNodeKind.Scalar _ -> ScalarQuoting.keepAsText (YamlNode.withScalarValue text node)
            | YamlNodeKind.Sequence collection ->
                let concatenated =
                    collection.Items
                    |> List.map (fun item ->
                        match item.Node.Kind with
                        | YamlNodeKind.Scalar scalar -> scalar.Value
                        | _ -> "")
                    |> String.concat ""
                if concatenated = text then ScalarQuoting.normalizeTree node else encodeString text
            | _ -> encodeString text
        | None -> encodeString text

    /// True when the node is a block collection without properties and the trivia stand in front of its first entry
    /// or item. Behind an inline dash such a collection continues the line of the dash, and the trivia would stand in
    /// front of the dash.
    let private startsWithTrivia (node: YamlNode) : bool =
        let hasProperties = node.Properties.Tag.IsSome || node.Properties.Anchor.IsSome
        if hasProperties then
            false
        else
            match node.Kind with
            | YamlNodeKind.Mapping collection when collection.Style = CollectionStyle.Block ->
                match collection.Items with
                | first :: _ -> not first.Before.IsEmpty
                | [] -> false
            | YamlNodeKind.Sequence collection when collection.Style = CollectionStyle.Block ->
                match collection.Items with
                | first :: _ -> not first.Before.IsEmpty
                | [] -> false
            | _ -> false

    /// The dash of an item whose collection starts with trivia. The first entry stands on the next line, behind the
    /// trivia, which stay in the raw value.
    let private lineBreakDash : Separation =
        Separation.NewLine ({ Comment = None; Following = [] } : LineBreak)

    /// An item of a block sequence. The dash is inline, as in `- class: x`, unless the collection of the item
    /// starts with trivia.
    let private blockItem (node: YamlNode) : SequenceItem =
        let item = YamlNode.item node
        if startsWithTrivia node then { item with Dash = Some lineBreakDash } else item

    /// A list of single-line scalars is written inline, as in `baseCommand: [dotnet, fsi]`. Any other list is
    /// a block sequence. An empty list is `[]`.
    let encodeSequence (items: YamlNode list) : YamlNode =
        // A missing value has no text to stand between the commas, and a comment would end the flow line.
        let isInlineScalar (node: YamlNode) =
            match node.Kind with
            | YamlNodeKind.Scalar scalar ->
                match scalar.Style with
                | ScalarStyle.Block _ -> false
                | _ ->
                    not (scalar.Value.Contains("\n"))
                    && node.TrailingComment.IsNone
                    && not (YamlNode.isMissingValue node)
            | _ -> false
        if items |> List.forall isInlineScalar then
            YamlNode.flowSequence (items |> List.map YamlNode.item)
        else
            YamlNode.sequence (items |> List.map blockItem)

    /// A mapping of keys and values. The keys are text of a document, such as the id of a port or the name of a field, so a
    /// key that the loader of cwltool reads as another type than a string, such as 1_000, gets quotes.
    let encodeObject (pairs: (string * YamlNode) list) : YamlNode =
        let node = Encode.object pairs
        match node.Kind with
        | YamlNodeKind.Mapping collection ->
            { node with Kind = YamlNodeKind.Mapping { collection with Items = collection.Items |> List.map (fun entry -> { entry with Key = ScalarQuoting.keepAsText entry.Key }) } }
        | _ -> node

    /// The object of an entry of an id list: the fields of the entry after its id. The id key names the field that holds
    /// the id, and the value key names the field of an entry that is no mapping.
    let private idObject (idKey: string) (valueKey: string) (id: string, node: YamlNode) : YamlNode =
        let fields =
            match node.Kind with
            | YamlNodeKind.Mapping collection -> collection.Items |> List.map (fun entry -> Decode.entryName entry, entry.Value)
            | _ -> [ valueKey, node ]
        encodeObject ((idKey, ScalarQuoting.textNode id) :: fields)

    /// Encode the entries of an id map, such as the step inputs. The map names each entry by its id, and a mapping holds
    /// a key once. Entries with a repeated id are written as a list of objects that carry their ids, so no entry is
    /// lost. The id key names the field that holds the id in such an object, as name does for the fields of a record. The
    /// value key names the field of an entry that is no mapping, as the source of a step input written as its source
    /// alone.
    let encodeIdMapOrList (idKey: string) (valueKey: string) (entries: (string * YamlNode) list) : YamlNode =
        let ids = entries |> List.map fst
        if (List.distinct ids).Length = ids.Length then
            encodeObject entries
        else
            entries |> List.map (idObject idKey valueKey) |> encodeSequence

    // ------------------------------
    // Helper append functions for ordered construction
    // ------------------------------
    /// The YAML of a raw value that a CWL object keeps. The scalars are in the form that cwltool reads as written (see
    /// ScalarQuoting.normalizeScalar), and RawYaml compares that form.
    let rawYamlNode (raw: RawYaml) : YamlNode = ScalarQuoting.normalizeTree raw.Node

    let inline appendOpt name (encoder:'a -> YamlNode) (value:'a option) acc =
        match value with
        | Some v -> acc @ [name, encoder v]
        | None -> acc

    /// Encode preserved DynamicObj extension values when they can be represented as YAML.
    let rec encodeDynamicValue (value: obj) =
        match value with
        | null -> None
        // Direct scalar values map to YAML scalars.
        | :? string as s -> Some (encodeString s)
        | :? bool as b -> Some (Encode.bool b)
        // Fable JavaScript/TypeScript use `number` for int and float; avoid truncating decimal values through Encode.int.
        | :? float as f -> Some (Encode.float f)
        | :? int as i -> Some (Encode.int i)
        | :? int64 as i -> Some (Encode.int64 i)
        // Already-parsed YAML is written back without reinterpretation.
        | :? RawYaml as raw -> Some (rawYamlNode raw)
        | :? YamlNode as y -> Some (ScalarQuoting.normalizeTree y)
        // Nested DynamicObj extension payloads become nested YAML mappings.
        | :? DynamicObj as dynObj -> Some (encodeDynamicObj dynObj)
        | :? System.Collections.IEnumerable as values ->
            // Generic collections are encoded item-by-item, silently dropping values
            // that cannot be represented as YAML.
            values
            |> Seq.cast<obj>
            |> Seq.choose encodeDynamicValue
            |> Seq.toList
            |> encodeSequence
            |> Some
        | _ -> None

    /// Encode all dynamic extension properties on a DynamicObj as a YAML mapping.
    and encodeDynamicObj (dynObj: DynamicObj) =
        DynamicObjHelpers.dynamicPropertiesExcept Set.empty dynObj
        |> Seq.choose (fun kvp -> encodeDynamicValue kvp.Value |> Option.map (fun encoded -> kvp.Key, encoded))
        |> Seq.toList
        |> encodeObject

    /// Append encodable dynamic extension fields while excluding typed CWL fields.
    let appendDynamicPropertiesExcept (knownFieldNames: Set<string>) (dynObj: DynamicObj) acc =
        DynamicObjHelpers.dynamicPropertiesExcept knownFieldNames dynObj
        |> Seq.choose (fun kvp -> encodeDynamicValue kvp.Value |> Option.map (fun encoded -> kvp.Key, encoded))
        // Use a fold that appends to preserve the existing pair order before adding
        // dynamic fields at the end of the current section.
        |> Seq.fold (fun pairs pair -> pairs @ [ pair ]) acc

    let appendDynamicProperties (dynObj: DynamicObj) acc =
        appendDynamicPropertiesExcept Set.empty dynObj acc

    /// Normalize documentation line endings. The doc keeps its final line feed, because a block scalar that ends
    /// in one reads back with it.
    let normalizeDocString (doc:string) =
        doc.Replace("\r\n","\n")

    /// Encode expression payloads with style-aware scalars.
    /// Single-line expressions are double-quoted to protect JS token syntax.
    /// Multi-line expressions are literal block scalars (see multiLineScalar), which keep them readable and keep YAML
    /// from reading the punctuation of the JavaScript. The writer picks the chomping, so the trailing line feeds and
    /// blank lines of the expression survive a write and a read.
    let encodeExpressionScalar (expression: string) : YamlNode =
        let normalized =
            // Normalize line endings once so style selection and emitted text agree.
            if isNull expression then "" else expression.Replace("\r\n", "\n").Replace("\r", "\n")

        if normalized.Contains("\n") then
            multiLineScalar normalized
        else
            // Single-line expressions are quoted for parser safety.
            YamlNode.scalar ScalarStyle.DoubleQuoted normalized

    /// Encode a Schema Salad directive as a flow mapping, as in {$import: path}. The path is written as it was read.
    let directive (key: string) (path: string) : YamlNode =
        YamlNode.flowMapping [ YamlNode.entry (Encode.string key) (encodeString path) ]

    /// Encode an entry of a list or the one object of a field, with the directive as a mapping in the place of an object.
    let encodeListEntry (encodeValue: 'T -> YamlNode) (entry: ListEntry<'T>) : YamlNode =
        match entry with
        | ListEntry.Value value -> encodeValue value
        | ListEntry.Imported path -> directive "$import" path
        | ListEntry.Mixed raw -> raw.Node

    /// Encode the entries of an id list, such as the inputs, the outputs, the steps or the fields of a record. Entries
    /// with a distinct id go into a map, as in encodeIdMapOrList. An import has no id to be a key, so a list that holds
    /// one is written as a list in which the import stands in its place. A field that the document wrote as one
    /// directive gets the directive again while the list holds that one import.
    let encodeIdEntries (idKey: string) (valueKey: string) (whole: bool) (entries: ListEntry<string * YamlNode> list) : YamlNode =
        match whole, entries with
        | true, [ ListEntry.Imported path ] -> directive "$import" path
        | _ ->
            let named = entries |> List.choose ListEntry.tryValue
            if named.Length = entries.Length then
                encodeIdMapOrList idKey valueKey named
            else
                entries
                |> List.map (encodeListEntry (idObject idKey valueKey))
                |> encodeSequence

    /// Encode schema-salad strings as literals or directive mappings. The function encodes the text of a literal.
    let encodeSchemaSaladStringWith (encodeLiteral: string -> YamlNode) (value: SchemaSaladString) : YamlNode =
        match value with
        | SchemaSaladString.Literal text -> encodeLiteral text
        | SchemaSaladString.Include path -> directive "$include" path
        | SchemaSaladString.Import path -> directive "$import" path
        | SchemaSaladString.Mixin path -> directive "$mixin" path
        | SchemaSaladString.Mixed raw -> raw.Node

    /// Encode schema-salad strings as literals or directive mappings.
    let encodeSchemaSaladString (value: SchemaSaladString) : YamlNode =
        encodeSchemaSaladStringWith encodeString value

    /// Encode a boolean field, and a directive as a mapping.
    let encodeSchemaSaladBool (value: SchemaSaladBool) : YamlNode =
        match value with
        | SchemaSaladBool.Value flag -> Encode.bool flag
        | SchemaSaladBool.Imported path -> directive "$import" path
        | SchemaSaladBool.Mixed raw -> raw.Node

    /// Encode the size of a File, and a directive as a mapping.
    let encodeFileSize (size: FileSize) : YamlNode =
        match size with
        | FileSize.Bytes count -> Encode.int64 count
        | FileSize.Imported path -> directive "$import" path
        | FileSize.Mixed raw -> raw.Node

    /// Encode a loadListing symbol, and a directive as a mapping.
    let encodeLoadListing (value: LoadListingEnum) : YamlNode =
        match value with
        | ImportedLoadListing path -> directive "$import" path
        | IncludedLoadListing path -> directive "$include" path
        | MixedLoadListing raw -> raw.Node
        | NoListing
        | ShallowListing
        | DeepListing -> encodeString (LoadListingEnum.toCwlString value)

    /// The value of a field that holds a list of strings. A document that wrote the field as one directive gets the
    /// directive again while the list holds that one directive. Any other list is a sequence.
    let encodeStringList (whole: bool) (entries: ResizeArray<SchemaSaladString>) : YamlNode =
        match whole, Seq.toList entries with
        | true, [ (SchemaSaladString.Import _ | SchemaSaladString.Mixin _ | SchemaSaladString.Mixed _) as single ] -> encodeSchemaSaladString single
        | _ -> entries |> Seq.map encodeSchemaSaladString |> List.ofSeq |> encodeSequence

    /// The label of an object. The node that a literal label was read from keeps its layout, and a directive is written
    /// as a mapping.
    let encodeLabelValue (source: YamlNode option) (label: SchemaSaladString) : YamlNode =
        match label with
        | SchemaSaladString.Literal text -> encodeSourceText source text
        | directive -> encodeSchemaSaladString directive

    let encodeLabel (label: SchemaSaladString) : (string * YamlNode) =
        "label", encodeLabelValue None label

    /// The text of a doc. The line breaks are line feeds, and the node that the text was read from keeps its layout.
    let encodeDocText (source: YamlNode option) (doc: string) : YamlNode =
        encodeSourceText source (normalizeDocString doc)

    /// The doc of an object. The function writes the text of a literal. The node that a literal doc was read from keeps
    /// its layout, a directive is written as a mapping, and the parts of a list are written as a list.
    let private encodeDocWith (encodeText: YamlNode option -> string -> YamlNode) (source: YamlNode option) (doc: DocText) : YamlNode =
        match doc with
        | DocText.Text (SchemaSaladString.Literal text) -> encodeText source text
        | DocText.Text directive -> encodeSchemaSaladString directive
        | DocText.Parts parts -> parts |> Seq.map encodeSchemaSaladString |> List.ofSeq |> encodeSequence

    /// The doc of a port, a step, a schema or a field, written with the characters of the text.
    let encodeDocValue (source: YamlNode option) (doc: DocText) : YamlNode =
        encodeDocWith encodeSourceText source doc

    /// The doc of a process, written with line feeds for the line breaks.
    let encodeProcessDocValue (source: YamlNode option) (doc: DocText) : YamlNode =
        encodeDocWith encodeDocText source doc

    let encodeDoc (doc: DocText) : (string * YamlNode) =
        "doc", encodeDocValue None doc

    /// The value of a field that holds a list of numbers. A document that wrote the field as one directive gets the
    /// directive again while the list holds that one directive. Any other list is a sequence.
    let encodeIntList (whole: bool) (codes: ResizeArray<ListEntry<int>>) : YamlNode =
        match whole, Seq.toList codes with
        | true, [ ListEntry.Imported path ] -> directive "$import" path
        | true, [ ListEntry.Mixed raw ] -> raw.Node
        | _ ->
            codes
            |> Seq.map (encodeListEntry Encode.int)
            |> List.ofSeq
            |> encodeSequence

    /// The intent of a process. The flag tells that the document wrote the field as one directive.
    let encodeIntent (whole: bool) (intent: ResizeArray<SchemaSaladString>) : (string * YamlNode) =
        "intent", encodeStringList whole intent

    let inline appendOptPair pairOpt acc =
        match pairOpt with
        | Some pair -> acc @ [pair]
        | None -> acc

    // ------------------------------
    // CWLType encoder
    // ------------------------------
    /// Encode the fields of a File value, led by the supplied discriminator key and value.
    let encodeFilePairs discriminatorKey discriminatorValue (file: FileInstance) =
        [ discriminatorKey, encodeString discriminatorValue ]
        |> appendOpt "location" encodeSchemaSaladString file.Location
        |> appendOpt "path" encodeSchemaSaladString file.Path
        |> appendOpt "basename" encodeSchemaSaladString file.Basename
        |> appendOpt "dirname" encodeSchemaSaladString file.Dirname
        |> appendOpt "nameroot" encodeSchemaSaladString file.Nameroot
        |> appendOpt "nameext" encodeSchemaSaladString file.Nameext
        |> appendOpt "checksum" encodeSchemaSaladString file.Checksum
        |> appendOpt "size" encodeFileSize file.Size
        |> appendOpt "secondaryFiles" rawYamlNode file.SecondaryFiles
        |> appendOpt "format" encodeSchemaSaladString file.Format
        |> appendOpt "contents" encodeSchemaSaladString file.Contents
        |> appendDynamicPropertiesExcept FileInstance.KnownFieldNames file

    /// Encode the fields of a Directory value, led by the supplied discriminator key and value.
    let encodeDirectoryPairs discriminatorKey discriminatorValue (directory: DirectoryInstance) =
        [ discriminatorKey, encodeString discriminatorValue ]
        |> appendOpt "location" encodeSchemaSaladString directory.Location
        |> appendOpt "path" encodeSchemaSaladString directory.Path
        |> appendOpt "basename" encodeSchemaSaladString directory.Basename
        |> appendOpt "listing" rawYamlNode directory.Listing
        |> appendDynamicPropertiesExcept DirectoryInstance.KnownFieldNames directory

    /// Encode a CWL type for a CWL document. A name, a one-level array of a name and an optional of those are written
    /// as the shorthand (File, File[], File[]?). Every other type is a schema object or a list.
    let rec encodeCWLType (t:CWLType) : YamlNode =
        encodeTypeWith true t

    /// Encode a CWL type as the text of the additional type of an RO-Crate entity. The shorthand nests arrays there
    /// (File[][]), which ARCtrl reads and a CWL document does not.
    and encodeCWLTypeShorthand (t:CWLType) : YamlNode =
        encodeTypeWith false t

    /// Encode a CWL type, as a CWL document when the flag is set.
    and encodeTypeWith (cwlDocument: bool) (t:CWLType) : YamlNode =
        let arrayShorthand (schema: InputArraySchema) =
            if cwlDocument then tryGetDocumentArrayShorthand schema else tryGetArrayShorthand (CWLType.Array schema)
        // A member of a type list reads the shorthand of a type field, in a CWL document as in the RO-Crate text.
        let encodeMember = encodeTypeWith cwlDocument

        match t with
        | File -> encodeString "File"
        | Directory -> encodeString "Directory"
        | String -> encodeString "string"
        | Int -> encodeString "int"
        | Long -> encodeString "long"
        | Float -> encodeString "float"
        | Double -> encodeString "double"
        | Boolean -> encodeString "boolean"
        | Any -> encodeString "Any"
        | Reference name -> encodeString name
        | ImportedType path -> directive "$import" path
        | IncludedType path -> directive "$include" path
        | MixedType raw -> raw.Node
        | RawType raw -> raw.Node
        | Stdout -> encodeString "stdout"
        | Stderr -> encodeString "stderr"
        | Stdin -> encodeString "stdin"
        | Null -> encodeString "null"
        | OptionalOf otherType ->
            // Only the list ["null", X] can use CWL's `?` shorthand.
            match otherType with
            | File -> encodeString "File?"
            | Directory -> encodeString "Directory?"
            | String -> encodeString "string?"
            | Int -> encodeString "int?"
            | Long -> encodeString "long?"
            | Float -> encodeString "float?"
            | Double -> encodeString "double?"
            | Boolean -> encodeString "boolean?"
            | Any -> encodeString "Any?"
            | Reference name -> encodeString (name + "?")
            | Array arraySchema ->
                // Optional array - use shorthand detection
                match arrayShorthand arraySchema with
                | Some shorthand -> encodeString (shorthand + "?")
                | None ->
                    // Complex optional array - use full form
                    encodeSequence [ encodeString "null"; encodeInputArraySchema arraySchema ]
            | _ ->
                // Complex optional type - use array form ["null", type]
                [ Null; otherType ] |> List.map encodeMember |> encodeSequence
        | Union types ->
            // Every other union is a type list.
            types |> Seq.toList |> List.map encodeMember |> encodeSequence
        | Array arraySchema ->
            // Use the short form for arrays. Fall back to a schema object for complex item types.
            match arrayShorthand arraySchema with
            | Some shorthand -> encodeString shorthand
            | None -> encodeInputArraySchema arraySchema
        | Record recordSchema -> encodeInputRecordSchema recordSchema
        | Enum enumSchema -> encodeInputEnumSchema enumSchema

    /// Encode the type under items. CWL reads the shorthand File[] and File? as the whole value of a type field and as
    /// a member of a type list, so under items an array is a schema and an optional type is a list.
    and encodeSchemaType (t:CWLType) : YamlNode =
        match t with
        | Array arraySchema -> encodeInputArraySchema arraySchema
        | Union types -> types |> Seq.map encodeSchemaType |> List.ofSeq |> encodeSequence
        | other -> encodeTypeWith true other

    // ------------------------------
    // InputRecordSchema encoders
    // ------------------------------

    /// Encode a record field as a map entry keyed by its field name.
    and encodeInputRecordField (field:InputRecordField) : (string * YamlNode) =
        let pairs =
            // The field name is emitted as the mapping key, so the nested value only
            // contains field payload such as type/doc/label/extensions.
            [ "type", encodeCWLType field.Type ]
            |> appendOpt "doc" (encodeDocValue field.SourceNodes.Doc) field.Doc
            |> appendOpt "label" (encodeLabelValue field.SourceNodes.Label) field.Label
            |> appendDynamicPropertiesExcept InputRecordField.KnownFieldNames field
        field.Name, encodeObject pairs

    /// Encode record schemas with map-form fields and preserved schema metadata.
    and encodeInputRecordSchema (schema:InputRecordSchema) : YamlNode =
        inputRecordSchemaPairs schema |> encodeObject

    /// The entries of a record schema.
    and inputRecordSchemaPairs (schema:InputRecordSchema) : (string * YamlNode) list =
        let fieldsElement =
            match schema.Fields with
            | Some fs ->
                // Record fields are written in map form for stable names and compact YAML. A list with an import or with
                // a repeated name is written as a list.
                fs
                |> Seq.map (ListEntry.map encodeInputRecordField)
                |> Seq.toList
                |> encodeIdEntries "name" "type" (schema.SourceNodes.WholeFields.Contains "fields")
            | None ->
                // Missing fields are emitted as an empty map to keep the schema object valid.
                encodeObject []
        [ "type", encodeString "record"; "fields", fieldsElement ]
        |> appendOpt "label" (encodeLabelValue schema.SourceNodes.Label) schema.Label
        |> appendOpt "doc" (encodeDocValue schema.SourceNodes.Doc) schema.Doc
        |> appendOpt "name" encodeString schema.Name
        |> appendDynamicPropertiesExcept InputRecordSchema.KnownFieldNames schema

    /// Encode enum schemas while preserving symbol order and schema metadata.
    and encodeInputEnumSchema (schema:InputEnumSchema) : YamlNode =
        inputEnumSchemaPairs schema |> encodeObject

    /// The entries of an enum schema.
    and inputEnumSchemaPairs (schema:InputEnumSchema) : (string * YamlNode) list =
        let pairs =
            // Symbol order is semantically significant, so emit the ResizeArray order.
            [ "type", encodeString "enum"; "symbols", encodeStringList (schema.SourceNodes.WholeFields.Contains "symbols") schema.Symbols ]

        pairs
        |> appendOpt "label" (encodeLabelValue schema.SourceNodes.Label) schema.Label
        |> appendOpt "doc" (encodeDocValue schema.SourceNodes.Doc) schema.Doc
        |> appendOpt "name" encodeString schema.Name
        |> appendDynamicPropertiesExcept InputEnumSchema.KnownFieldNames schema

    /// Encode array schemas with their item type and preserved schema metadata.
    and encodeInputArraySchema (schema:InputArraySchema) : YamlNode =
        inputArraySchemaPairs schema |> encodeObject

    /// The entries of an array schema.
    and inputArraySchemaPairs (schema:InputArraySchema) : (string * YamlNode) list =
        [ "type", encodeString "array"; "items", encodeSchemaType schema.Items ]
        |> appendOpt "label" (encodeLabelValue schema.SourceNodes.Label) schema.Label
        |> appendOpt "doc" (encodeDocValue schema.SourceNodes.Doc) schema.Doc
        |> appendOpt "name" encodeString schema.Name
        |> appendDynamicPropertiesExcept InputArraySchema.KnownFieldNames schema

    // ------------------------------
    // Binding & Port encoders
    // ------------------------------

    /// Encode the glob of an output binding in the form that it holds, so a list of one pattern stays a list.
    let encodeOutputGlob (glob: OutputGlob) : YamlNode =
        match glob with
        | OutputGlob.Single value -> encodeSchemaSaladString value
        | OutputGlob.Multiple values -> values |> Seq.map encodeSchemaSaladString |> List.ofSeq |> encodeSequence

    /// Encode outputBinding fields and preserved extension properties.
    let encodeOutputBinding (ob:OutputBinding) : YamlNode =
        []
        |> appendOpt "glob" encodeOutputGlob ob.Glob
        |> appendOpt "loadContents" encodeSchemaSaladBool ob.LoadContents
        |> appendOpt "loadListing" encodeLoadListing ob.LoadListing
        |> appendOpt "outputEval" encodeSchemaSaladString ob.OutputEval
        |> appendDynamicPropertiesExcept OutputBinding.KnownFieldNames ob
        |> encodeObject

    let rec encodeCWLParameterValue (value: CWLParameterValue) : YamlNode =
        match value with
        | CWLParameterValue.Null -> Encode.nullValue
        | CWLParameterValue.String value -> encodeString value
        | CWLParameterValue.Int value -> Encode.int64 value
        | CWLParameterValue.Float value -> Encode.float value
        | CWLParameterValue.Boolean value -> Encode.bool value
        | CWLParameterValue.File file ->
            encodeFilePairs "class" "File" file
            |> encodeObject
        | CWLParameterValue.Directory directory ->
            encodeDirectoryPairs "class" "Directory" directory
            |> encodeObject
        | CWLParameterValue.Array values ->
            values
            |> Seq.map encodeCWLParameterValue
            |> Seq.toList
            |> encodeSequence
        | CWLParameterValue.Record fields ->
            fields
            |> Seq.map (fun field -> field.Name, encodeCWLParameterValue field.Value)
            |> Seq.toList
            |> encodeObject

    /// A port as a map entry. The type comes first, and a port without other fields is written as the type alone, as
    /// in outputName: File. The map value is the port itself when it is a mapping, so a schema without other fields
    /// stays under type. A port with other fields writes the schema under type in any case, as the CWL schemas
    /// define.
    let private encodePortEntry (name: string) (type_: CWLType option) (otherPairs: (string * YamlNode) list) : string * YamlNode =
        match type_ |> Option.map encodeCWLType, otherPairs with
        | Some typeNode, [] ->
            match typeNode.Kind with
            | YamlNodeKind.Mapping _ -> name, encodeObject [ "type", typeNode ]
            | _ -> name, typeNode
        | typeNode, _ ->
            name, encodeObject (Option.toList (typeNode |> Option.map (fun node -> "type", node)) @ otherPairs)

    /// Encode a workflow or tool output, choosing shorthand only when type is the sole field.
    let encodeCWLOutput (o:CWLOutput) : (string * YamlNode) =
        let outputSourceElement =
            // A single source is a scalar, and a list is a sequence also when it holds one source. A directive on its own is
            // written in the place of the scalar, where it stands for the string or the list of a file.
            match o.OutputSource with
            | Some (OutputSource.Single value) -> Some (encodeSchemaSaladString value)
            | Some (OutputSource.Multiple values) when values.Count > 0 ->
                Some (values |> Seq.map encodeSchemaSaladString |> List.ofSeq |> encodeSequence)
            | _ -> None

        []
        |> appendOpt "label" (encodeLabelValue o.SourceNodes.Label) o.Label
        |> appendOpt "secondaryFiles" rawYamlNode o.SecondaryFiles
        |> appendOpt "streamable" encodeSchemaSaladBool o.Streamable
        |> appendOpt "doc" (encodeDocValue o.SourceNodes.Doc) o.Doc
        |> appendOpt "format" encodeSchemaSaladString o.Format
        |> appendOpt "outputBinding" (encodeListEntry encodeOutputBinding) o.OutputBinding
        |> appendOpt "outputSource" id outputSourceElement
        |> appendDynamicPropertiesExcept CWLOutput.KnownFieldNames o
        |> encodePortEntry o.Name o.Type_

    /// Encode the position of an input binding: a number or an expression, and a directive as a mapping.
    let encodeBindingPosition (position: BindingPosition) : YamlNode =
        match position with
        | BindingPosition.Number value -> Encode.int value
        | BindingPosition.Expression value -> encodeString value
        | BindingPosition.Imported path -> directive "$import" path
        | BindingPosition.Included path -> directive "$include" path
        | BindingPosition.Mixed raw -> raw.Node

    /// Encode inputBinding fields and preserved extension properties.
    let encodeInputBinding (ib:InputBinding) : YamlNode =
        []
        |> appendOpt "loadContents" encodeSchemaSaladBool ib.LoadContents
        |> appendOpt "prefix" encodeSchemaSaladString ib.Prefix
        |> appendOpt "position" encodeBindingPosition ib.Position
        |> appendOpt "itemSeparator" encodeSchemaSaladString ib.ItemSeparator
        |> appendOpt "separate" encodeSchemaSaladBool ib.Separate
        |> appendOpt "valueFrom" encodeSchemaSaladString ib.ValueFrom
        |> appendOpt "shellQuote" encodeSchemaSaladBool ib.ShellQuote
        |> appendDynamicPropertiesExcept InputBinding.KnownFieldNames ib
        |> encodeObject

    /// Encode the format of an input in the form that it holds, so a list of one name stays a list.
    let encodeInputFormat (format: InputFormat) : YamlNode =
        match format with
        | InputFormat.Single value -> encodeSchemaSaladString value
        | InputFormat.Multiple values -> values |> Seq.map encodeSchemaSaladString |> List.ofSeq |> encodeSequence

    /// Encode a workflow or tool input, choosing shorthand only when type is the sole field.
    let encodeCWLInput (i:CWLInput) : (string * YamlNode) =
        // Build object-form fields in CWL's conventional order.
        []
        |> appendOpt "label" (encodeLabelValue i.SourceNodes.Label) i.Label
        |> appendOpt "secondaryFiles" rawYamlNode i.SecondaryFiles
        |> appendOpt "streamable" encodeSchemaSaladBool i.Streamable
        |> appendOpt "doc" (encodeDocValue i.SourceNodes.Doc) i.Doc
        |> appendOpt "format" encodeInputFormat i.Format
        |> appendOpt "loadContents" encodeSchemaSaladBool i.LoadContents
        |> appendOpt "loadListing" encodeLoadListing i.LoadListing
        |> appendOpt "default" rawYamlNode i.DefaultValue
        |> appendOpt "inputBinding" (encodeListEntry encodeInputBinding) i.InputBinding
        |> appendDynamicPropertiesExcept CWLInput.KnownFieldNames i
        |> encodePortEntry i.Name i.Type_

    // ------------------------------
    // Requirement encoder (always extended style)
    // ------------------------------

    /// Encode one SchemaDefRequirement type entry. CWL defines the entry as the schema itself, with type: record or
    /// enum and the fields of the schema. The name leads when the schema has one, and the extension fields of the
    /// schema follow.
    let encodeSchemaDefinition (definition: SchemaDefinition) : YamlNode =
        let schemaPairs =
            match definition with
            | SchemaDefinition.Record schema -> inputRecordSchemaPairs schema
            | SchemaDefinition.Enum schema -> inputEnumSchemaPairs schema
            | SchemaDefinition.Array schema -> inputArraySchemaPairs schema
        let name, others = schemaPairs |> List.partition (fun (key, _) -> key = "name")
        name @ others |> encodeObject

    /// Encode known CWL requirements in extended form while preserving requirement extensions.
    let encodeRequirement (r:Requirement) : YamlNode =
        match r with
        | InlineJavascriptRequirement value ->
            let expressionLib =
                // Empty expressionLib is omitted because the requirement class alone
                // is meaningful and matches CWL's compact style.
                value.ExpressionLib
                |> Option.bind (fun entries -> if entries.Count > 0 then Some entries else None)
            [ "class", encodeString "InlineJavascriptRequirement" ]
            |> appendOpt "expressionLib" (fun entries -> entries |> Seq.map encodeSchemaSaladString |> List.ofSeq |> encodeSequence) expressionLib
            |> appendDynamicPropertiesExcept InlineJavascriptRequirementValue.KnownFieldNames value
            |> encodeObject
        | SchemaDefRequirement types ->
            // SchemaDefRequirement contains schema entries that already preserve their
            // own extension fields. An imported file is a mapping with the $import directive.
            let encodeEntry (entry: SchemaDefEntry) =
                match entry with
                | Defined schema -> encodeSchemaDefinition schema
                | Imported path -> directive "$import" path
                | Mixed raw -> raw.Node
                | Raw raw -> raw.Node
            [ "class", encodeString "SchemaDefRequirement";
              "types", (types |> Seq.map encodeEntry |> List.ofSeq |> encodeSequence) ] |> encodeObject
        | DockerRequirement dr ->
            // Docker fields are optional and emitted only when present.
            [ "class", encodeString "DockerRequirement" ]
            |> appendOpt "dockerPull" encodeSchemaSaladString dr.DockerPull
            |> appendOpt "dockerFile" encodeSchemaSaladString dr.DockerFile
            |> appendOpt "dockerImageId" encodeSchemaSaladString dr.DockerImageId
            |> appendOpt "dockerLoad" encodeSchemaSaladString dr.DockerLoad
            |> appendOpt "dockerImport" encodeSchemaSaladString dr.DockerImport
            |> appendOpt "dockerOutputDirectory" encodeSchemaSaladString dr.DockerOutputDirectory
            |> appendOpt "cwltool:dockerRunOptions" (fun values -> values |> Seq.map encodeString |> List.ofSeq |> encodeSequence) dr.DockerRunOptions
            |> appendDynamicPropertiesExcept DockerRequirement.KnownFieldNames dr
            |> encodeObject
        | SoftwareRequirement pkgs ->
            let encodePkg (p:SoftwarePackage) =
                // Extended package form is used here so version/specs/extensions all
                // have explicit keys.
                []
                |> fun acc -> acc @ [ "package", encodeSchemaSaladString p.Package ]
                |> appendOpt "version" (fun vs -> vs |> Seq.map encodeSchemaSaladString |> List.ofSeq |> encodeSequence) p.Version
                |> appendOpt "specs" (fun vs -> vs |> Seq.map encodeSchemaSaladString |> List.ofSeq |> encodeSequence) p.Specs
                |> appendDynamicPropertiesExcept SoftwarePackage.KnownFieldNames p
                |> encodeObject
            [ "class", encodeString "SoftwareRequirement";
              "packages", (pkgs |> Seq.map (encodeListEntry encodePkg) |> List.ofSeq |> encodeSequence) ] |> encodeObject
        | LoadListingRequirement loadListing ->
            // Always emit the concrete loadListing value instead of relying on decoder defaults.
            [ "class", encodeString "LoadListingRequirement"
              "loadListing", encodeLoadListing loadListing.LoadListing ]
            |> appendDynamicPropertiesExcept LoadListingRequirementValue.KnownFieldNames loadListing
            |> encodeObject
        | InitialWorkDirRequirement listing ->
            // File and Directory listing entries carry their kind in the class field.
            let encodeFileOrDirectory = function
                | FileItem file -> encodeFilePairs "class" "File" file |> encodeObject
                | DirectoryItem directory -> encodeDirectoryPairs "class" "Directory" directory |> encodeObject
                | ImportedItem path -> directive "$import" path
                | MixedItem raw -> raw.Node
                | RawItem raw -> raw.Node
            let encodeInitialWorkDirEntry = function
                | DirentEntry d ->
                    // Dirent entries use their own object form.
                    [ ]
                    |> appendOpt "entryname" encodeSchemaSaladString d.Entryname
                    |> fun acc -> acc @ [ "entry", encodeSchemaSaladString d.Entry ]
                    |> appendOpt "writable" encodeSchemaSaladBool d.Writable
                    |> appendDynamicPropertiesExcept DirentInstance.KnownFieldNames d
                    |> encodeObject
                | StringEntry s ->
                    // String/expression listing entries stay scalar/directive form.
                    encodeSchemaSaladString s
                | FileEntry file -> encodeFileOrDirectory (FileItem file)
                | DirectoryEntry directory -> encodeFileOrDirectory (DirectoryItem directory)
                | NullEntry -> Encode.nullValue
                | FilesEntry items -> items |> Seq.map encodeFileOrDirectory |> List.ofSeq |> encodeSequence
                | RawEntry raw -> raw.Node

            [ "class", encodeString "InitialWorkDirRequirement";
              "listing", (listing |> Seq.map encodeInitialWorkDirEntry |> List.ofSeq |> encodeSequence) ] |> encodeObject
        | InitialWorkDirExpressionRequirement expression ->
            [ "class", encodeString "InitialWorkDirRequirement"
              "listing", encodeSchemaSaladString expression ]
            |> encodeObject
        | EnvVarRequirement envs ->
            let encodeEnv (e:EnvironmentDef) =
                // Env var values are strings, and encodeString quotes values such as true or 1.
                [ "envName", encodeSchemaSaladString e.EnvName; "envValue", encodeSchemaSaladString e.EnvValue ]
                |> appendDynamicPropertiesExcept EnvironmentDef.KnownFieldNames e
                |> encodeObject
            [ "class", encodeString "EnvVarRequirement";
              "envDef", (envs |> Seq.map (encodeListEntry encodeEnv) |> List.ofSeq |> encodeSequence) ] |> encodeObject
        | ShellCommandRequirement -> [ "class", encodeString "ShellCommandRequirement" ] |> encodeObject
        | ResourceRequirement rr ->
            let tryEncodeScalar (key: string) (value: obj) =
                // Resource fields can be numeric, boolean, or expression strings.
                // Only those scalar shapes are emitted from typed or dynamic fields.
                match value with
                // Fable JavaScript/TypeScript use `number` for int and float; preserve decimal resource values.
                | :? float as f -> Some (key, Encode.float f)
                | :? int as i -> Some (key, Encode.int i)
                | :? int64 as i -> Some (key, Encode.int64 i)
                | :? string as s -> Some (key, encodeString s)
                | :? SchemaSaladString as s -> Some (key, encodeSchemaSaladString s)
                | :? bool as b -> Some (key, Encode.bool b)
                | _ -> None

            let knownPairs =
                // Typed fields are emitted first in CWL field order.
                rr.KnownFieldValues
                |> List.choose (fun (key, value) ->
                    value |> Option.bind (tryEncodeScalar key))

            let knownFieldNames =
                ResourceRequirementInstance.KnownFieldNames
                |> Seq.map id
                |> Set.ofSeq

            let dynamicPairs =
                // Then append custom resource fields that look scalar enough to encode.
                DynamicObjHelpers.dynamicPropertiesExcept knownFieldNames rr
                |> Seq.choose (fun kvp ->
                    match kvp.Value with
                    | :? Option<obj> as optionalValue ->
                        optionalValue
                        |> Option.bind (tryEncodeScalar kvp.Key)
                    | directValue ->
                        tryEncodeScalar kvp.Key directValue)
                |> Seq.toList
            [ "class", encodeString "ResourceRequirement" ] @ knownPairs @ dynamicPairs |> encodeObject
        // Canonicalize class names to short CWL forms where applicable.
        | WorkReuseRequirement workReuse ->
            [ "class", encodeString "WorkReuse"
              "enableReuse", Encode.bool workReuse.EnableReuse ]
            |> appendDynamicPropertiesExcept WorkReuseRequirementValue.KnownFieldNames workReuse
            |> encodeObject
        | WorkReuseExpressionRequirement expression ->
            [ "class", encodeString "WorkReuse"
              "enableReuse", encodeSchemaSaladString expression ]
            |> encodeObject
        | NetworkAccessRequirement networkAccess ->
            [ "class", encodeString "NetworkAccess"
              "networkAccess", Encode.bool networkAccess.NetworkAccess ]
            |> appendDynamicPropertiesExcept NetworkAccessRequirementValue.KnownFieldNames networkAccess
            |> encodeObject
        | NetworkAccessExpressionRequirement expression ->
            [ "class", encodeString "NetworkAccess"
              "networkAccess", encodeSchemaSaladString expression ]
            |> encodeObject
        | InplaceUpdateRequirement inplaceUpdate ->
            [ "class", encodeString "InplaceUpdateRequirement"
              "inplaceUpdate", encodeSchemaSaladBool inplaceUpdate.InplaceUpdate ]
            |> appendDynamicPropertiesExcept InplaceUpdateRequirementValue.KnownFieldNames inplaceUpdate
            |> encodeObject
        | ToolTimeLimitRequirement tl ->
            let timelimit =
                match tl with
                | ToolTimeLimitSeconds seconds -> Encode.int64 seconds
                | ToolTimeLimitExpression expression -> encodeSchemaSaladString expression
            [ "class", encodeString "ToolTimeLimit"; "timelimit", timelimit ] |> encodeObject
        | SubworkflowFeatureRequirement -> [ "class", encodeString "SubworkflowFeatureRequirement" ] |> encodeObject
        | ScatterFeatureRequirement -> [ "class", encodeString "ScatterFeatureRequirement" ] |> encodeObject
        | MultipleInputFeatureRequirement -> [ "class", encodeString "MultipleInputFeatureRequirement" ] |> encodeObject
        | StepInputExpressionRequirement -> [ "class", encodeString "StepInputExpressionRequirement" ] |> encodeObject
        | ImportedRequirement path -> directive "$import" path
        | MixedRequirement raw -> raw.Node
        | RawRequirement raw -> raw.Node

    /// Encode known hints as requirements and unknown hints as their original raw YAML.
    let encodeHintEntry (hint: HintEntry) : YamlNode =
        match hint with
        | KnownHint requirement -> encodeRequirement requirement
        | UnknownHint unknownHint -> unknownHint.Raw.Node
        | ImportedHint path -> directive "$import" path
        | MixedHint raw -> raw.Node
        | RawHint raw -> raw.Node

    /// The requirements or the hints of an object as the value of their field. A document that wrote the field as one
    /// directive gets the directive again while the list holds that one import. Any other list is a sequence.
    let private encodeEntryList (whole: bool) (encodeEntry: 'a -> YamlNode) (importedPath: 'a -> string option) (entries: ResizeArray<'a>) : YamlNode =
        match whole, Seq.toList entries with
        | true, [ single ] when (importedPath single).IsSome -> directive "$import" (importedPath single).Value
        | _ -> entries |> Seq.map encodeEntry |> List.ofSeq |> encodeSequence

    let private importedRequirementPath (requirement: Requirement) : string option =
        match requirement with
        | ImportedRequirement path -> Some path
        | _ -> None

    let private importedHintPath (hint: HintEntry) : string option =
        match hint with
        | ImportedHint path -> Some path
        | _ -> None

    /// The value of the hints field of an object with these source nodes.
    let private encodeHintsField (sourceNodes: SourceNodes) (hints: ResizeArray<HintEntry>) : YamlNode =
        encodeEntryList (sourceNodes.WholeFields.Contains "hints") encodeHintEntry importedHintPath hints

    /// The value of the requirements field of an object with these source nodes.
    let private encodeRequirementsField (sourceNodes: SourceNodes) (requirements: ResizeArray<Requirement>) : YamlNode =
        encodeEntryList (sourceNodes.WholeFields.Contains "requirements") encodeRequirement importedRequirementPath requirements

    // ------------------------------
    // Workflow step encoders
    // ------------------------------
    
    /// Encode the sources as either a single string or a sequence
    let encodeSourceArray (sources: OutputSource) : YamlNode =
        match sources with
        | OutputSource.Single source ->
            // CWL source shorthand: source: input_id
            encodeSchemaSaladString source
        | OutputSource.Multiple sources ->
            // A list is always written as a block sequence, also a list of one source.
            sources
            |> Seq.map (fun s -> YamlNode.item (encodeSchemaSaladString s))
            |> List.ofSeq
            |> YamlNode.sequence

    let encodeLinkMergeMethod (linkMerge: LinkMergeMethod) : YamlNode =
        match linkMerge with
        | ImportedLinkMerge path -> directive "$import" path
        | IncludedLinkMerge path -> directive "$include" path
        | MixedLinkMerge raw -> raw.Node
        | MergeNested
        | MergeFlattened -> encodeString linkMerge.AsCwlString

    let encodePickValueMethod (pickValue: PickValueMethod) : YamlNode =
        match pickValue with
        | ImportedPickValue path -> directive "$import" path
        | IncludedPickValue path -> directive "$include" path
        | MixedPickValue raw -> raw.Node
        | FirstNonNull
        | TheOnlyNonNull
        | AllNonNull -> encodeString pickValue.AsCwlString

    let encodeScatterMethod (scatterMethod: ScatterMethod) : YamlNode =
        match scatterMethod with
        | ImportedScatterMethod path -> directive "$import" path
        | IncludedScatterMethod path -> directive "$include" path
        | MixedScatterMethod raw -> raw.Node
        | DotProduct
        | NestedCrossProduct
        | FlatCrossProduct -> encodeString scatterMethod.AsCwlString

    /// Encode a step input using scalar source shorthand only when no other fields exist.
    let encodeStepInput (si:StepInput) : (string * YamlNode) =
        let pairs =
            []
            |> appendOpt "source" encodeSourceArray si.Source
            |> appendOpt "default" rawYamlNode si.DefaultValue
            |> appendOpt "valueFrom" encodeSchemaSaladString si.ValueFrom
            |> appendOpt "linkMerge" encodeLinkMergeMethod si.LinkMerge
            |> appendOpt "pickValue" encodePickValueMethod si.PickValue
            |> appendOpt "doc" (encodeDocValue si.SourceNodes.Doc) si.Doc
            |> appendOpt "loadContents" encodeSchemaSaladBool si.LoadContents
            |> appendOpt "loadListing" encodeLoadListing si.LoadListing
            |> appendOpt "label" (encodeLabelValue si.SourceNodes.Label) si.Label
            |> appendDynamicPropertiesExcept StepInput.KnownFieldNames si
        match pairs with
        | [ ("source", s) ]
            when
                // A directive on its own cannot stand for the value of a map entry, because Schema Salad adds the key to it as a field.
                (match s.Kind with YamlNodeKind.Mapping _ -> false | _ -> true)
                && si.DefaultValue.IsNone
                && si.ValueFrom.IsNone
                && si.LinkMerge.IsNone
                && si.PickValue.IsNone
                && si.Doc.IsNone
                && si.LoadContents.IsNone
                && si.LoadListing.IsNone
                && si.Label.IsNone ->
            si.Id, s
        | _ -> si.Id, encodeObject pairs

    /// Encode all step inputs in map form keyed by step input id. A list with an import is written as a list, as in
    /// encodeIdEntries. The flag tells that the document wrote the field as one directive.
    let encodeStepInputs (whole: bool) (inputs:ResizeArray<ListEntry<StepInput>>) : YamlNode =
        inputs
        |> Seq.map (ListEntry.map encodeStepInput)
        |> Seq.toList
        |> encodeIdEntries "id" "source" whole

    /// Encode record-form step output parameters, including extension fields.
    let encodeStepOutputParameter (so: StepOutputParameter) : YamlNode =
        [ "id", encodeString so.Id ]
        |> appendDynamicPropertiesExcept StepOutputParameter.KnownFieldNames so
        |> encodeObject

    /// Encode step outputs as scalar ids or record-form output parameters. A directive entry stands in its place. A
    /// document that wrote the field as one directive gets the directive again while the list holds that one import.
    let encodeStepOutputs (whole: bool) (outputs: ResizeArray<StepOutput>) : YamlNode =
        match whole, Seq.toList outputs with
        | true, [ StepOutputImport path ] -> directive "$import" path
        | _ ->
            outputs
            |> Seq.map (fun output ->
                match output with
                | StepOutputString id -> encodeString id
                | StepOutputRecord outputParameter -> encodeStepOutputParameter outputParameter
                | StepOutputInclude path -> directive "$include" path
                | StepOutputImport path -> directive "$import" path
                | StepOutputMixed raw -> raw.Node
            )
            |> List.ofSeq
            |> encodeSequence

    /// Encode scatter as scalar shorthand for one item or a sequence for multiple items.
    let encodeScatter (scatter: ResizeArray<SchemaSaladString>) : YamlNode =
        match scatter.Count with
        | 1 -> encodeSchemaSaladString scatter.[0]
        | _ -> scatter |> Seq.map encodeSchemaSaladString |> List.ofSeq |> encodeSequence

    /// The cwlVersion entry of a process. A process that takes the version of its enclosing document writes none.
    let private cwlVersionPairs (includeCwlVersion: bool) (version: string) : (string * YamlNode) list =
        if includeCwlVersion then [ "cwlVersion", encodeString version ] else []

    /// Encode workflow step run targets as paths or inline processing units. The flag tells whether the run is written
    /// inside the $graph of a packed document, where a run of the form #id can refer to a process of that document.
    let rec encodeWorkflowStepRun (inGraph: bool) (run: WorkflowStepRun) : YamlNode =
        match run with
        | RunString runPath ->
            // External run reference.
            encodeString runPath
        | RunImport runPath ->
            // The process of a file that the document imports.
            directive "$import" runPath
        | RunMixed raw ->
            // The process of a file that the document completes with the fields beside the directive.
            raw.Node
        | RunRaw raw ->
            // The process whose class is a directive.
            raw.Node
        | RunCommandLineTool toolObj ->
            // Inline CommandLineTool run; validate the object payload before encoding.
            match WorkflowStepRunOps.tryGetTool run with
            | Some tool -> encodeToolDescriptionElement true tool
            | None ->
                raise (System.ArgumentException($"RunCommandLineTool must contain CWLToolDescription but got %A{toolObj}"))
        | RunWorkflow workflowObj ->
            // Inline nested Workflow run.
            match WorkflowStepRunOps.tryGetWorkflow run with
            | Some workflow -> encodeWorkflowDescriptionElement inGraph true workflow
            | None ->
                raise (System.ArgumentException($"RunWorkflow must contain CWLWorkflowDescription but got %A{workflowObj}"))
        | RunExpressionTool expressionToolObj ->
            // Inline ExpressionTool run.
            match WorkflowStepRunOps.tryGetExpressionTool run with
            | Some expressionTool -> encodeExpressionToolDescriptionElement true expressionTool
            | None ->
                raise (System.ArgumentException($"RunExpressionTool must contain CWLExpressionToolDescription but got %A{expressionToolObj}"))
        | RunOperation operationObj ->
            // Inline Operation run.
            match WorkflowStepRunOps.tryGetOperation run with
            | Some operation -> encodeOperationDescriptionElement true operation
            | None ->
                raise (System.ArgumentException($"RunOperation must contain CWLOperationDescription but got %A{operationObj}"))

    /// Encode a CommandLineTool element with ordered sections and preserved metadata. The flag tells whether the element
    /// writes its cwlVersion.
    and encodeToolDescriptionElement (includeCwlVersion: bool) (td: CWLToolDescription) : YamlNode =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            cwlVersionPairs includeCwlVersion td.CWLVersion
            @ [ "class", encodeString "CommandLineTool" ]
            |> appendOptPair (td.Id |> Option.map (fun id -> "id", encodeString id))
            |> appendOpt "label" (encodeLabelValue td.SourceNodes.Label) td.Label
            |> appendOpt "doc" (encodeProcessDocValue td.SourceNodes.Doc) td.Doc
            |> appendOptPair (td.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map (encodeIntent (td.SourceNodes.WholeFields.Contains "intent")))
        let withHints =
            match td.Hints with
            | Some h when h.Count > 0 ->
                // Hints are emitted before requirements to match existing fixtures.
                basePairs @ [ "hints", (encodeHintsField td.SourceNodes h) ]
            | _ -> basePairs
        let withRequirements =
            match td.Requirements with
            | Some r when r.Count > 0 ->
                // Requirements are omitted when absent or empty.
                withHints @ [ "requirements", (encodeRequirementsField td.SourceNodes r) ]
            | _ -> withHints
        let withBaseCommand =
            match td.BaseCommand with
            | Some bc when bc.Count > 0 ->
                // baseCommand is always emitted as a sequence here.
                withRequirements @ [ "baseCommand", (bc |> Seq.map encodeSchemaSaladString |> List.ofSeq |> encodeSequence) ]
            | _ -> withRequirements
        let withCommandFields =
            // Command-line execution fields belong near baseCommand before ports.
            withBaseCommand
            |> appendOpt "arguments" rawYamlNode td.Arguments
            |> appendOpt "stdin" encodeSchemaSaladString td.Stdin
            |> appendOpt "stderr" encodeSchemaSaladString td.Stderr
            |> appendOpt "stdout" encodeSchemaSaladString td.Stdout
            |> appendOpt "successCodes" (encodeIntList (td.SourceNodes.WholeFields.Contains "successCodes")) td.SuccessCodes
            |> appendOpt "temporaryFailCodes" (encodeIntList (td.SourceNodes.WholeFields.Contains "temporaryFailCodes")) td.TemporaryFailCodes
            |> appendOpt "permanentFailCodes" (encodeIntList (td.SourceNodes.WholeFields.Contains "permanentFailCodes")) td.PermanentFailCodes
        let withInputs =
            // CWL requires inputs, so a tool without inputs writes the empty map, as outputs do.
            let inputs = td.Inputs |> Option.defaultValue (ResizeArray<ListEntry<CWLInput>>())
            withCommandFields @ [ "inputs", (inputs |> Seq.map (ListEntry.map encodeCWLInput) |> Seq.toList |> encodeIdEntries "id" "type" (td.SourceNodes.WholeFields.Contains "inputs")) ]
        let withOutputs =
            // Outputs are required by the model and always emitted.
            withInputs @ [ "outputs", (td.Outputs |> Seq.map (ListEntry.map encodeCWLOutput) |> Seq.toList |> encodeIdEntries "id" "type" (td.SourceNodes.WholeFields.Contains "outputs")) ]
        let withMetadata =
            match td.Metadata with
            | Some md ->
                // Metadata DynamicObj is serialized as top-level extension fields.
                appendDynamicProperties md withOutputs
            | None -> withOutputs
        withMetadata
        // Also include dynamic fields attached directly to the tool object.
        |> appendDynamicPropertiesExcept CWLToolDescription.KnownFieldNames td
        |> encodeObject

    /// Encode an ExpressionTool element with ordered sections and preserved metadata. The flag tells whether the element
    /// writes its cwlVersion.
    and encodeExpressionToolDescriptionElement (includeCwlVersion: bool) (et: CWLExpressionToolDescription) : YamlNode =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            cwlVersionPairs includeCwlVersion et.CWLVersion
            @ [ "class", encodeString "ExpressionTool" ]
            |> appendOptPair (et.Id |> Option.map (fun id -> "id", encodeString id))
            |> appendOpt "label" (encodeLabelValue et.SourceNodes.Label) et.Label
            |> appendOpt "doc" (encodeProcessDocValue et.SourceNodes.Doc) et.Doc
            |> appendOptPair (et.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map (encodeIntent (et.SourceNodes.WholeFields.Contains "intent")))
        let withHints =
            match et.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (encodeHintsField et.SourceNodes h) ]
            | _ -> basePairs
        let withRequirements =
            match et.Requirements with
            | Some r when r.Count > 0 ->
                withHints @ [ "requirements", (encodeRequirementsField et.SourceNodes r) ]
            | _ -> withHints
        let withInputs =
            // CWL requires inputs, so a tool without inputs writes the empty map, as outputs do.
            let inputs = et.Inputs |> Option.defaultValue (ResizeArray<ListEntry<CWLInput>>())
            withRequirements @ [ "inputs", (inputs |> Seq.map (ListEntry.map encodeCWLInput) |> Seq.toList |> encodeIdEntries "id" "type" (et.SourceNodes.WholeFields.Contains "inputs")) ]
        let withOutputs =
            withInputs @ [ "outputs", (et.Outputs |> Seq.map (ListEntry.map encodeCWLOutput) |> Seq.toList |> encodeIdEntries "id" "type" (et.SourceNodes.WholeFields.Contains "outputs")) ]
        let withExpression =
            // Expression is required and written after ports.
            withOutputs @ [ "expression", encodeSchemaSaladStringWith encodeExpressionScalar et.Expression ]
        let withMetadata =
            match et.Metadata with
            | Some md ->
                // Metadata DynamicObj is serialized as top-level extension fields.
                appendDynamicProperties md withExpression
            | None -> withExpression
        withMetadata
        // Include dynamic fields attached directly to the expression tool object.
        |> appendDynamicPropertiesExcept CWLExpressionToolDescription.KnownFieldNames et
        |> encodeObject

    /// Encode an Operation element with ordered sections and preserved metadata. The flag tells whether the element
    /// writes its cwlVersion.
    and encodeOperationDescriptionElement (includeCwlVersion: bool) (op: CWLOperationDescription) : YamlNode =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            cwlVersionPairs includeCwlVersion op.CWLVersion
            @ [ "class", encodeString "Operation" ]
            |> appendOptPair (op.Id |> Option.map (fun id -> "id", encodeString id))
            |> appendOpt "label" (encodeLabelValue op.SourceNodes.Label) op.Label
            |> appendOpt "doc" (encodeProcessDocValue op.SourceNodes.Doc) op.Doc
            |> appendOptPair (op.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map (encodeIntent (op.SourceNodes.WholeFields.Contains "intent")))
        let withHints =
            match op.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (encodeHintsField op.SourceNodes h) ]
            | _ -> basePairs
        let withRequirements =
            match op.Requirements with
            | Some r when r.Count > 0 ->
                withHints @ [ "requirements", (encodeRequirementsField op.SourceNodes r) ]
            | _ -> withHints
        let withInputs =
            // Operation inputs are required by the model.
            withRequirements @ [ "inputs", (op.Inputs |> Seq.map (ListEntry.map encodeCWLInput) |> Seq.toList |> encodeIdEntries "id" "type" (op.SourceNodes.WholeFields.Contains "inputs")) ]
        let withOutputs =
            // Operation outputs are required by the model.
            withInputs @ [ "outputs", (op.Outputs |> Seq.map (ListEntry.map encodeCWLOutput) |> Seq.toList |> encodeIdEntries "id" "type" (op.SourceNodes.WholeFields.Contains "outputs")) ]
        let withMetadata =
            match op.Metadata with
            | Some md ->
                // Metadata DynamicObj is serialized as top-level extension fields.
                appendDynamicProperties md withOutputs
            | None -> withOutputs
        withMetadata
        // Include dynamic fields attached directly to the operation object.
        |> appendDynamicPropertiesExcept CWLOperationDescription.KnownFieldNames op
        |> encodeObject

    /// Encode a Workflow element with ordered sections and preserved metadata. The first flag tells whether the element
    /// is written inside the $graph of a packed document, and the second whether it writes its cwlVersion.
    and encodeWorkflowDescriptionElement (inGraph: bool) (includeCwlVersion: bool) (wd: CWLWorkflowDescription) : YamlNode =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            cwlVersionPairs includeCwlVersion wd.CWLVersion
            @ [ "class", encodeString "Workflow" ]
            |> appendOptPair (wd.Id |> Option.map (fun id -> "id", encodeString id))
            |> appendOpt "label" (encodeLabelValue wd.SourceNodes.Label) wd.Label
            |> appendOpt "doc" (encodeProcessDocValue wd.SourceNodes.Doc) wd.Doc
            |> appendOptPair (wd.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map (encodeIntent (wd.SourceNodes.WholeFields.Contains "intent")))
        let withHints =
            match wd.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (encodeHintsField wd.SourceNodes h) ]
            | _ -> basePairs
        let withRequirements =
            match wd.Requirements with
            | Some r when r.Count > 0 ->
                withHints @ [ "requirements", (encodeRequirementsField wd.SourceNodes r) ]
            | _ -> withHints
        let withInputs =
            // Workflow inputs are required and emitted before steps.
            withRequirements @ [ "inputs", (wd.Inputs |> Seq.map (ListEntry.map encodeCWLInput) |> Seq.toList |> encodeIdEntries "id" "type" (wd.SourceNodes.WholeFields.Contains "inputs")) ]
        let withSteps =
            // Steps are map entries keyed by step id.
            withInputs @ [ "steps", encodeWorkflowSteps inGraph (wd.SourceNodes.WholeFields.Contains "steps") wd.Steps ]
        let withOutputs =
            // Outputs follow steps for scan-friendly workflow YAML.
            withSteps @ [ "outputs", (wd.Outputs |> Seq.map (ListEntry.map encodeCWLOutput) |> Seq.toList |> encodeIdEntries "id" "type" (wd.SourceNodes.WholeFields.Contains "outputs")) ]
        let withMetadata =
            match wd.Metadata with
            | Some md ->
                // Metadata DynamicObj is serialized as top-level extension fields.
                appendDynamicProperties md withOutputs
            | None -> withOutputs
        withMetadata
        // Include dynamic fields attached directly to the workflow object.
        |> appendDynamicPropertiesExcept CWLWorkflowDescription.KnownFieldNames wd
        |> encodeObject

    /// Encode the steps of a workflow as the entries of an id list (see encodeIdEntries). A step without an id has no key
    /// for a map, so a list with such a step is a list of objects, and only the steps with an id write it.
    and encodeWorkflowSteps (inGraph: bool) (whole: bool) (steps: ResizeArray<ListEntry<WorkflowStep>>) : YamlNode =
        let encoded = steps |> Seq.map (ListEntry.map (encodeWorkflowStep inGraph)) |> Seq.toList
        let withoutId = encoded |> List.exists (fun entry -> match entry with | ListEntry.Value (None, _) -> true | _ -> false)
        if withoutId then
            encoded
            |> List.map (encodeListEntry (fun (id, node) -> match id with | Some id -> idObject "id" "run" (id, node) | None -> node))
            |> encodeSequence
        else
            encoded
            |> List.map (ListEntry.map (fun (id, node) -> Option.get id, node))
            |> encodeIdEntries "id" "run" whole

    /// Encode one workflow step as a map entry keyed by step id, and without a key when the step has no id. A run of the
    /// form #id refers to a process of the packed document that holds the step. Outside of a $graph, the file has no such
    /// process, so the step writes its resolved process inline. A step without one cannot be written there.
    and encodeWorkflowStep (inGraph: bool) (ws:WorkflowStep) : (string option * YamlNode) =
        let run =
            match ws.Run, ws.ResolvedRun.Run with
            | RunString reference, resolved when not inGraph && reference.StartsWith "#" ->
                match resolved with
                | Some resolvedRun -> resolvedRun
                | None ->
                    let name = match ws.Id with | Some id -> "step " + id | None -> "step without an id"
                    raise (System.ArgumentException($"The {name} runs {reference}, a process of a packed document, and has no resolved process. A file that is no packed document cannot refer to it."))
            | run, _ -> run
        let basePairs =
            // Required step fields are emitted first.
            [ "run", encodeWorkflowStepRun inGraph run
              "in", encodeStepInputs (ws.SourceNodes.WholeFields.Contains "in") ws.In
              "out", encodeStepOutputs (ws.SourceNodes.WholeFields.Contains "out") ws.Out ]
            |> appendOpt "label" (encodeLabelValue ws.SourceNodes.Label) ws.Label
            |> appendOpt "doc" (encodeDocValue ws.SourceNodes.Doc) ws.Doc
            |> appendOpt "scatter" encodeScatter ws.Scatter
            |> appendOpt "scatterMethod" encodeScatterMethod ws.ScatterMethod
            |> appendOpt "when" encodeSchemaSaladString ws.When_
            |> appendDynamicPropertiesExcept WorkflowStep.KnownFieldNames ws
        let withHints =
            match ws.Hints with
            // Step-local hints are included only when non-empty.
            | Some h when h.Count > 0 -> basePairs @ [ "hints", (encodeHintsField ws.SourceNodes h) ]
            | _ -> basePairs
        let withReq =
            match ws.Requirements with
            // Step-local requirements follow hints.
            | Some r when r.Count > 0 -> withHints @ [ "requirements", (encodeRequirementsField ws.SourceNodes r) ]
            | _ -> withHints
        ws.Id, encodeObject withReq

    // ------------------------------
    // Top-level encoders
    // ------------------------------

    /// A block collection under rebuild by moveEndTrivia.
    type private OpenBlock =
        { Source: YamlNode
          /// True while the collection is on the chain of last nodes of the document.
          IsLast: bool
          Children: YamlNode[]
          Before: Trivia list[]
          Rebuilt: YamlNode[]
          mutable Next: int }

    let private isBlockWithItems (node: YamlNode) : bool =
        match node.Kind with
        | YamlNodeKind.Mapping collection -> collection.Style = CollectionStyle.Block && not collection.Items.IsEmpty
        | YamlNodeKind.Sequence collection -> collection.Style = CollectionStyle.Block && not collection.Items.IsEmpty
        | _ -> false

    /// The values or items of a block collection with the trivia in front of each one.
    let private childrenOf (node: YamlNode) : YamlNode[] * Trivia list[] =
        match node.Kind with
        | YamlNodeKind.Mapping collection ->
            [| for entry in collection.Items -> entry.Value |], [| for entry in collection.Items -> entry.Before |]
        | YamlNodeKind.Sequence collection ->
            [| for item in collection.Items -> item.Node |], [| for item in collection.Items -> item.Before |]
        | _ -> [||], [||]

    let private endOf (node: YamlNode) : Trivia list =
        match node.Kind with
        | YamlNodeKind.Mapping collection -> collection.End
        | YamlNodeKind.Sequence collection -> collection.End
        | _ -> []

    /// The collection with new trivia in front of its children, new children and a new End. An item with an inline
    /// dash gets a dash with a line break when its collection now starts with trivia.
    let private rebuildBlock (node: YamlNode) (before: Trivia list[]) (children: YamlNode[]) (collectionEnd: Trivia list) : YamlNode =
        match node.Kind with
        | YamlNodeKind.Mapping collection ->
            let entries = collection.Items |> List.mapi (fun index entry -> { entry with Before = before.[index]; Value = children.[index] })
            { node with Kind = YamlNodeKind.Mapping { collection with Items = entries; End = collectionEnd } }
        | YamlNodeKind.Sequence collection ->
            let items =
                collection.Items
                |> List.mapi (fun index item ->
                    let dash =
                        match item.Dash with
                        | Some Separation.Inline when startsWithTrivia children.[index] -> Some lineBreakDash
                        | other -> other
                    { item with Before = before.[index]; Dash = dash; Node = children.[index] })
            { node with Kind = YamlNodeKind.Sequence { collection with Items = items; End = collectionEnd } }
        | _ -> node

    /// True when a block collection that is not on the chain of last nodes has trivia in its End. The flag tells
    /// whether the root is the last node of the document.
    let private hasMisplacedEnd (rootIsLast: bool) (root: YamlNode) : bool =
        let pending = ResizeArray<YamlNode * bool>()
        pending.Add((root, rootIsLast))
        let mutable found = false
        while not found && pending.Count > 0 do
            let (node, isLast) = pending.[pending.Count - 1]
            pending.RemoveAt(pending.Count - 1)
            if isBlockWithItems node then
                if not isLast && not (endOf node).IsEmpty then
                    found <- true
                else
                    let children = fst (childrenOf node)
                    children |> Array.iteri (fun index child -> pending.Add((child, isLast && index = children.Length - 1)))
        found

    /// A comment line behind the last entry of a document belongs to the End of the deepest open block collection
    /// (rule R4). A raw value keeps that End. When the encoder puts the value in front of other entries, the End
    /// is no longer at the end of the document, and the writer refuses it. The trivia move in front of the last entry
    /// or item of the same collection. They stay in the raw value, and the next read finds them in the same place.
    /// The blank lines at either end of the moved trivia are dropped, because the comments now stand against the
    /// entry. The flag tells whether the root is the last node of the document. The walk keeps its own stack, so a
    /// value of any depth is safe.
    let private moveEndTrivia (rootIsLast: bool) (root: YamlNode) : YamlNode =
        if not (hasMisplacedEnd rootIsLast root) then
            root
        else
            let unfinished = ResizeArray<OpenBlock>()
            let mutable result = root
            let enter (node: YamlNode) (isLast: bool) =
                let (children, before) = childrenOf node
                unfinished.Add ({ Source = node; IsLast = isLast; Children = children; Before = Array.copy before; Rebuilt = Array.copy children; Next = 0 } : OpenBlock)
            let isBlankLine (line: Trivia) = match line with BlankLine _ -> true | FullLineComment _ -> false
            enter root rootIsLast
            while unfinished.Count > 0 do
                let top = unfinished.[unfinished.Count - 1]
                if top.Next < top.Children.Length then
                    let index = top.Next
                    top.Next <- index + 1
                    let child = top.Children.[index]
                    if isBlockWithItems child then
                        enter child (top.IsLast && index = top.Children.Length - 1)
                else
                    unfinished.RemoveAt(unfinished.Count - 1)
                    let collectionEnd =
                        let trivia = endOf top.Source
                        if top.IsLast || trivia.IsEmpty then
                            trivia
                        else
                            let moved = trivia |> List.skipWhile isBlankLine |> List.rev |> List.skipWhile isBlankLine |> List.rev
                            let last = top.Before.Length - 1
                            top.Before.[last] <- top.Before.[last] @ moved
                            []
                    let rebuilt = rebuildBlock top.Source top.Before top.Rebuilt collectionEnd
                    if unfinished.Count = 0 then
                        result <- rebuilt
                    else
                        let parent = unfinished.[unfinished.Count - 1]
                        parent.Rebuilt.[parent.Next - 1] <- rebuilt
            result

    /// The options of every write. The default options indent by two spaces, which matches the fixtures.
    /// renderTopLevelElement asks the writer about the same options, because they decide the chomping.
    let private writerOptions : WriterOptions = WriterOptions.defaults

    let writeYaml (element: YamlNode) =
        Writer.writeNode writerOptions (moveEndTrivia true element)

    /// Encode an entry of a job file as the key and the plain CWL value. The file has no field for a type.
    let encodeCWLParameterReference (reference: CWLParameterReference) =
        let encodedValue =
            reference.Value
            |> Option.map encodeCWLParameterValue
            |> Option.defaultValue (encodeSequence [])
        reference.Key, encodedValue

    let encodeYAMLParameterFile (references: CWLParameterReference ResizeArray) =
        references
        |> Seq.map encodeCWLParameterReference
        |> Seq.toList
        |> encodeObject
        |> writeYaml

    /// Render top-level CWL sections in a stable order while keeping metadata as a final block. The document is
    /// one mapping, and a blank line separates the sections, except behind a section that ends in a block scalar
    /// with the chomping Keep. The empty lines behind such a scalar are part of its value, and the writer says
    /// which scalars it writes that way. The writer prints the document once, so only the end of the document
    /// ends a text, and the end of a section is no end of a scalar.
    let renderTopLevelElement (baseKeys: string list) (orderedSectionKeys: string list) (element: YamlNode) : string =
        let entries =
            match element.Kind with
            | YamlNodeKind.Mapping collection ->
                collection.Items |> List.map (fun entry -> Decode.entryName entry, entry)
            | _ -> []
        let basePairs =
            // Identity and description keys stay together at the top.
            entries
            |> List.filter (fun (k, _) -> List.contains k baseKeys)

        let knownSections =
            // Ordered sections are emitted one key at a time to keep large CWL blocks
            // visually separated.
            orderedSectionKeys
            |> List.choose (fun sectionKey ->
                entries
                |> List.tryFind (fun (k, _) -> k = sectionKey)
                |> Option.map List.singleton
            )

        let reservedKeys = Set.ofList (baseKeys @ orderedSectionKeys)
        let metadataPairs =
            // Any unreserved key is extension metadata and is placed after typed sections.
            entries
            |> List.filter (fun (k, _) -> reservedKeys.Contains k |> not)

        let sections =
            // Drop empty sections, preserving the requested order for the rest.
            [
                if basePairs.Length > 0 then
                    basePairs
                yield! knownSections
                if metadataPairs.Length > 0 then
                    metadataPairs
            ]
            |> List.map (List.map snd)
            |> List.toArray

        // Another section follows the last value of a section, so the End trivia of a raw value stand in the middle of
        // the document. They move first, because the writer answers from the tree whether the value ends in kept lines.
        let sections =
            sections
            |> Array.mapi (fun index section ->
                if index < sections.Length - 1 then
                    let last = List.last section
                    List.take (section.Length - 1) section @ [ { last with Value = moveEndTrivia false last.Value } ]
                else
                    section)

        let separated =
            sections
            |> Array.mapi (fun index section ->
                match section with
                | first :: rest when index > 0 && not (Writer.endsWithKeptLines writerOptions (List.last sections.[index - 1]).Value) ->
                    // The first key of a section follows a blank line. Behind a block scalar that keeps its empty
                    // lines, a blank line would become part of the value, so the lines of the scalar stand in its place.
                    { first with Before = Trivia.BlankLine None :: first.Before } :: rest
                | _ -> section)
            |> List.concat

        if separated.IsEmpty then
            ""
        else
            YamlNode.mapping separated |> writeYaml

    let encodeToolDescription (td:CWLToolDescription) : string =
        // Tool top-level layout places command fields before ports.
        encodeToolDescriptionElement true td
        |> renderTopLevelElement ["cwlVersion"; "class"; "id"; "label"; "doc"; "intent"] ["hints"; "requirements"; "baseCommand"; "inputs"; "outputs"]

    let encodeWorkflowDescription (wd:CWLWorkflowDescription) : string =
        // Workflow layout keeps inputs, steps, and outputs as separate scan blocks.
        encodeWorkflowDescriptionElement false true wd
        |> renderTopLevelElement ["cwlVersion"; "class"; "id"; "label"; "doc"; "intent"] ["hints"; "requirements"; "inputs"; "steps"; "outputs"]

    let encodeExpressionToolDescription (et:CWLExpressionToolDescription) : string =
        // ExpressionTool layout writes expression after ports.
        encodeExpressionToolDescriptionElement true et
        |> renderTopLevelElement ["cwlVersion"; "class"; "id"; "label"; "doc"; "intent"] ["hints"; "requirements"; "inputs"; "outputs"; "expression"]

    let encodeOperationDescription (op: CWLOperationDescription) : string =
        // Operation layout mirrors workflow/tool metadata and port ordering.
        encodeOperationDescriptionElement true op
        |> renderTopLevelElement ["cwlVersion"; "class"; "id"; "label"; "doc"; "intent"] ["hints"; "requirements"; "inputs"; "outputs"]

    let encodeProcessingUnit (pu : CWLProcessingUnit) :string =
        match pu with
        // Preserve the public processing-unit wrapper dispatch at the string level.
        | CommandLineTool td -> encodeToolDescription td
        | Workflow wd -> encodeWorkflowDescription wd
        | ExpressionTool et -> encodeExpressionToolDescription et
        | Operation op -> encodeOperationDescription op
        // The process is written as it was read, with its own layout.
        | RawProcess raw -> writeYaml raw.Node

    /// Encode a packed document: the cwlVersion of the root and the processes under $graph. The processes use the
    /// version of the root, so they write none of their own. The other keys of the root follow the graph. A step that
    /// runs one of the processes of the graph holds the reference to it, as in run: "#shout", and writes it as it is.
    let encodeCWLGraph (graph: CWLGraph) : string =
        let encodeMember (processingUnit: CWLProcessingUnit) : YamlNode =
            match processingUnit with
            | CommandLineTool td -> encodeToolDescriptionElement false td
            | Workflow wd -> encodeWorkflowDescriptionElement true false wd
            | ExpressionTool et -> encodeExpressionToolDescriptionElement false et
            | Operation op -> encodeOperationDescriptionElement false op
            | RawProcess raw -> raw.Node
        [ "cwlVersion", encodeString graph.CWLVersion
          "$graph", (graph.Processes |> Seq.map encodeMember |> List.ofSeq |> encodeSequence) ]
        |> appendDynamicPropertiesExcept CWLGraph.KnownFieldNames graph
        |> encodeObject
        |> renderTopLevelElement ["cwlVersion"] ["$graph"]

    /// Encode a CWL document, which is a process or a packed document.
    let encodeCWLDocument (document: CWLDocument) : string =
        match document with
        | CWLDocument.Process processingUnit -> encodeProcessingUnit processingUnit
        | CWLDocument.Graph graph -> encodeCWLGraph graph

    /// Encode the process of a file that may hold a packed document. The file is the packed document while the process
    /// is the process main of the graph, so the other processes and the other keys of the root are written as they were
    /// read. In any other case the file holds the process alone. The caller can change the id of the process, or the
    /// graph, after the read, so this decides on each write.
    let encodeProcessingUnitInGraph (graph: CWLGraph option) (processingUnit: CWLProcessingUnit) : string =
        match graph with
        | Some graph when CWLGraph.isMain processingUnit graph -> encodeCWLGraph graph
        | _ -> encodeProcessingUnit processingUnit

    /// Encode a CWLType in flow style, so that the written text has no line break and can be embedded in JSON.
    /// Unions are lists, and schemas are mappings with their fields in the array form.
    let rec encodeCWLTypeFlow (t: CWLType) : YamlNode =
        let flowObject (pairs: (string * YamlNode) list) =
            YamlNode.flowMapping [ for (name, value) in pairs -> YamlNode.entry (ScalarQuoting.textNode name) value ]
        let flowList (nodes: YamlNode list) =
            YamlNode.flowSequence (nodes |> List.map YamlNode.item)
        match t with
        | Union types ->
            types |> Seq.map encodeCWLTypeFlow |> Seq.toList |> flowList
        | Array arraySchema ->
            // Inline schema rendering is used for compact serialization contexts.
            flowObject [ "type", Encode.string "array"; "items", encodeCWLTypeFlow arraySchema.Items ]
        | Record recordSchema ->
            // Preserve field order from the schema's ResizeArray.
            let fields =
                match recordSchema.Fields with
                | Some fs ->
                    fs
                    |> Seq.map (encodeListEntry (fun field -> flowObject [ "name", ScalarQuoting.textNode field.Name; "type", encodeCWLTypeFlow field.Type ]))
                    |> Seq.toList
                | None -> []
            flowObject [ "type", Encode.string "record"; "fields", flowList fields ]
        | Enum enumSchema ->
            // Symbols are emitted in stored order.
            flowObject [ "type", Encode.string "enum"; "symbols", enumSchema.Symbols |> Seq.map (encodeSchemaSaladStringWith ScalarQuoting.textNode) |> Seq.toList |> flowList ]
        | _ ->
            // Simple type - just the type name. The null type comes out quoted, because
            // Encode.string quotes the text null.
            encodeCWLType t

    /// Encode a CWLType to a single-line YAML string using flow/inline style
    /// This produces YAML that doesn't contain newlines and can be embedded in JSON
    let encodeCWLTypeYaml (t: CWLType) : string =
        (encodeCWLTypeFlow t |> writeYaml).Trim()

    /// Convert a CWLType to a YAML-formatted string for use in serialization
    let cwlTypeToYamlString (t: CWLType) : string =
        encodeCWLTypeYaml t

    /// Helper function to format CWLType for display in error messages
    let formatCWLType (type_ : CWLType) =
        encodeCWLType type_
        |> writeYaml
        |> fun s -> s.Trim()

