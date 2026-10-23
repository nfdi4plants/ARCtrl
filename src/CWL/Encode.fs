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
    
    /// Matches the optional type, the list [null, X]. Schema Salad reads the shorthand X? as exactly this list. A list
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
        | File _ -> PrimitiveType "File"
        | Directory _ -> PrimitiveType "Directory"
        | Dirent _ -> PrimitiveType "Dirent"
        | String -> PrimitiveType "string"
        | Int -> PrimitiveType "int"
        | Long -> PrimitiveType "long"
        | Float -> PrimitiveType "float"
        | Double -> PrimitiveType "double"
        | Boolean -> PrimitiveType "boolean"
        | Any -> PrimitiveType "Any"
        | Null -> PrimitiveType "null"
        | Stdout -> PrimitiveType "stdout"
        | Stderr -> PrimitiveType "stderr"
        | Stdin -> PrimitiveType "stdin"
        // Schemas and general unions require structured YAML unless a later
        // optional-union special case handles them.
        | Record _ | Enum _ | Union _ -> ComplexType
        | Array arraySchema ->
            // Arrays are primitive only if their items are primitive
            match arraySchema.Items with
            | PrimitiveType _ -> PrimitiveType "array"
            | ComplexType -> ComplexType

    /// Try to get shorthand notation for a CWL type (e.g., "File", "string[]", "int[][]")
    /// Returns None for complex types that require full YAML serialization
    let rec tryGetArrayShorthand (cwlType: CWLType) : string option =
        match cwlType with
        | PrimitiveType name when name <> "array" -> Some name
        | Array arraySchema ->
            // Recursively get shorthand for inner type and append []
            tryGetArrayShorthand arraySchema.Items |> Option.map (fun s -> s + "[]")
        | _ -> None

    /// The shorthand of the items of a one-level array in a CWL document, as in File[]. CWL reads one [] after a name,
    /// so an array of arrays has none.
    let private tryGetItemShorthand (items: CWLType) : string option =
        match items with
        | PrimitiveType name when name <> "array" -> Some name
        | _ -> None

    /// Determine if a type requires full YAML serialization (complex type)
    let rec isComplexType (t: CWLType) : bool =
        match t with
        | Record _ | Enum _ -> true
        | Array arraySchema ->
            // Complex if array doesn't have shorthand (array of record/enum)
            tryGetArrayShorthand arraySchema.Items |> Option.isNone
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

    /// A string that spans lines is written as a literal block scalar, so a doc text or an expression stays
    /// readable. Any other string goes through Encode.string, which double-quotes text that a core schema
    /// reader takes for a bool, a number or null.
    let encodeString (value: string) : YamlNode =
        if value.Contains("\n") then
            literalBlock value
        else
            Encode.string value

    /// The text of a doc or a label. The node that the text was read from, when there is one, takes the current text
    /// with YamlNode.withScalarValue. It comes back unchanged while the text is the same, and an edited text keeps the
    /// style of the node and the lines that still fit. withScalarValue quotes an edited text that a core schema reader
    /// would take for another type than a string, so the text stays a string. The node of a doc may be a list of
    /// strings, which comes back unchanged while the text is the concatenation of its items and is a string after an
    /// edit. A text without a node is spelled by encodeString.
    let encodeSourceText (source: YamlNode option) (text: string) : YamlNode =
        match source with
        | Some node ->
            match node.Kind with
            | YamlNodeKind.Sequence collection ->
                let concatenated =
                    collection.Items
                    |> List.map (fun item ->
                        match item.Node.Kind with
                        | YamlNodeKind.Scalar scalar -> scalar.Value
                        | _ -> "")
                    |> String.concat ""
                if concatenated = text then node else encodeString text
            | _ -> YamlNode.withScalarValue text node
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

    // ------------------------------
    // Helper append functions for ordered construction
    // ------------------------------
    /// The YAML of a raw value that a CWL object keeps.
    let rawYamlNode (raw: RawYaml) : YamlNode = raw.Node

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
        | :? RawYaml as raw -> Some raw.Node
        | :? YamlNode as y -> Some y
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
        |> Encode.object

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
    /// Multi-line expressions are literal block scalars, which keep them readable and keep YAML from reading the
    /// punctuation of the JavaScript. The writer picks the chomping, so the trailing line feeds and blank lines
    /// of the expression survive a write and a read.
    let encodeExpressionScalar (expression: string) : YamlNode =
        let normalized =
            // Normalize line endings once so style selection and emitted text agree.
            if isNull expression then "" else expression.Replace("\r\n", "\n").Replace("\r", "\n")

        if normalized.Contains("\n") then
            literalBlock normalized
        else
            // Single-line expressions are quoted for parser safety.
            YamlNode.scalar ScalarStyle.DoubleQuoted normalized

    /// Encode schema-salad strings as literals or directive mappings.
    let encodeSchemaSaladString (value: SchemaSaladString) : YamlNode =
        match value with
        | SchemaSaladString.Literal text -> encodeString text
        | SchemaSaladString.Include path -> Encode.object [ "$include", encodeString path ]
        | SchemaSaladString.Import path -> Encode.object [ "$import", encodeString path ]

    /// Encode EnvVarRequirement using compact map shorthand (envName -> envValue).
    let encodeEnvVarRequirementCompactMap (envs: ResizeArray<EnvironmentDef>) : YamlNode =
        let envDefMap =
            // Compact map form writes env names as keys. Env values are strings, and encodeString
            // quotes the ones that a YAML reader would take for a bool, a number or null.
            envs
            |> Seq.map (fun env -> env.EnvName, encodeString env.EnvValue)
            |> Seq.toList
            |> Encode.object
        [ "class", encodeString "EnvVarRequirement"
          "envDef", envDefMap ]
        |> Encode.object

    /// Encode SoftwareRequirement using compact map shorthand.
    let encodeSoftwareRequirementCompactMap (packages: ResizeArray<SoftwarePackage>) : YamlNode =
        let encodePackageValue (package: SoftwarePackage) =
            match package.Version, package.Specs with
            // Empty object means package present with no version/specs constraints.
            | None, None -> Encode.object []
            // Specs-only packages can use sequence shorthand.
            | None, Some specs -> specs |> Seq.map encodeString |> Seq.toList |> encodeSequence
            | _ ->
                // Full package object keeps version and specs separate.
                []
                |> appendOpt "version" (fun values -> values |> Seq.map encodeString |> Seq.toList |> encodeSequence) package.Version
                |> appendOpt "specs" (fun values -> values |> Seq.map encodeString |> Seq.toList |> encodeSequence) package.Specs
                |> Encode.object

        let packagesMap =
            packages
            |> Seq.map (fun package -> package.Package, encodePackageValue package)
            |> Seq.toList
            |> Encode.object

        [ "class", encodeString "SoftwareRequirement"
          "packages", packagesMap ]
        |> Encode.object

    let encodeLabel (label:string) : (string * YamlNode) =
        "label", encodeString label

    /// The text of a doc. The line breaks are line feeds, and the node that the text was read from keeps its layout.
    let encodeDocText (source: YamlNode option) (doc: string) : YamlNode =
        encodeSourceText source (normalizeDocString doc)

    let encodeDoc (doc:string) : (string * YamlNode) =
        "doc", encodeDocText None doc

    let encodeIntent (intent: ResizeArray<string>) : (string * YamlNode) =
        "intent", (intent |> Seq.map encodeString |> List.ofSeq |> encodeSequence)

    let inline appendOptPair pairOpt acc =
        match pairOpt with
        | Some pair -> acc @ [pair]
        | None -> acc

    // ------------------------------
    // CWLType encoder
    // ------------------------------
    /// Encode File fields with the supplied discriminator key (`type` or `class`).
    let encodeFilePairs discriminatorKey discriminatorValue (file: FileInstance) =
        [ discriminatorKey, encodeString discriminatorValue ]
        |> appendOpt "location" encodeString file.Location
        |> appendOpt "path" encodeString file.Path
        |> appendOpt "basename" encodeString file.Basename
        |> appendOpt "dirname" encodeString file.Dirname
        |> appendOpt "nameroot" encodeString file.Nameroot
        |> appendOpt "nameext" encodeString file.Nameext
        |> appendOpt "checksum" encodeString file.Checksum
        |> appendOpt "size" Encode.int64 file.Size
        |> appendOpt "secondaryFiles" rawYamlNode file.SecondaryFiles
        |> appendOpt "format" encodeString file.Format
        |> appendOpt "contents" encodeString file.Contents
        |> appendDynamicPropertiesExcept FileInstance.KnownFieldNames file

    /// Encode Directory fields with the supplied discriminator key (`type` or `class`).
    let encodeDirectoryPairs discriminatorKey discriminatorValue (directory: DirectoryInstance) =
        [ discriminatorKey, encodeString discriminatorValue ]
        |> appendOpt "location" encodeString directory.Location
        |> appendOpt "path" encodeString directory.Path
        |> appendOpt "basename" encodeString directory.Basename
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
        let arrayShorthand = if cwlDocument then tryGetItemShorthand else tryGetArrayShorthand
        // A member of a type list reads the shorthand of a type field, in a CWL document as in the RO-Crate text.
        let encodeMember = encodeTypeWith cwlDocument
        let hasFileFields (file: FileInstance) =
            // encodeFilePairs always includes the discriminator, so more than one
            // pair means metadata is present and scalar File shorthand would lose it.
            (encodeFilePairs "type" "File" file).Length > 1

        let hasDirectoryFields (directory: DirectoryInstance) =
            // Same rule as File: any metadata requires object form.
            (encodeDirectoryPairs "type" "Directory" directory).Length > 1

        match t with
        | File file when hasFileFields file ->
            // Preserve File metadata such as secondaryFiles or format.
            encodeFilePairs "type" "File" file |> Encode.object
        | File _ -> encodeString "File"
        | Directory directory when hasDirectoryFields directory ->
            // Preserve Directory metadata such as listing.
            encodeDirectoryPairs "type" "Directory" directory |> Encode.object
        | Directory _ -> encodeString "Directory"
        | Dirent d ->
            // Dirent is always structured because it has required entry plus optional
            // entryname/writable fields.
            [ "entry", encodeSchemaSaladString d.Entry ]
            |> appendOpt "entryname" encodeSchemaSaladString d.Entryname
            |> appendOpt "writable" (fun b -> Encode.bool b) d.Writable
            |> appendDynamicPropertiesExcept DirentInstance.KnownFieldNames d
            |> Encode.object
        | String -> encodeString "string"
        | Int -> encodeString "int"
        | Long -> encodeString "long"
        | Float -> encodeString "float"
        | Double -> encodeString "double"
        | Boolean -> encodeString "boolean"
        | Any -> encodeString "Any"
        | Stdout -> encodeString "stdout"
        | Stderr -> encodeString "stderr"
        | Stdin -> encodeString "stdin"
        | Null -> encodeString "null"
        | OptionalOf otherType ->
            // Only the list [null, X] can use CWL's `?` shorthand.
            match otherType with
            | File _ -> encodeString "File?"
            | Directory _ -> encodeString "Directory?"
            | String -> encodeString "string?"
            | Int -> encodeString "int?"
            | Long -> encodeString "long?"
            | Float -> encodeString "float?"
            | Double -> encodeString "double?"
            | Boolean -> encodeString "boolean?"
            | Any -> encodeString "Any?"
            | Array arraySchema ->
                // Optional array - use shorthand detection
                match arrayShorthand arraySchema.Items with
                | Some shorthand -> encodeString (shorthand + "[]?")
                | None ->
                    // Complex optional array - use full form
                    encodeSequence [ encodeString "null"; encodeInputArraySchema arraySchema ]
            | _ ->
                // Complex optional type - use array form [null, type]
                [ Null; otherType ] |> List.map encodeMember |> encodeSequence
        | Union types ->
            // Every other union is a type list.
            types |> Seq.toList |> List.map encodeMember |> encodeSequence
        | Array arraySchema ->
            // Try to use short form for arrays; fall back to schema object for complex item types.
            match arrayShorthand arraySchema.Items with
            | Some shorthand -> encodeString (shorthand + "[]")
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
            |> appendOpt "doc" (encodeSourceText field.SourceNodes.Doc) field.Doc
            |> appendOpt "label" (encodeSourceText field.SourceNodes.Label) field.Label
            |> appendDynamicPropertiesExcept InputRecordField.KnownFieldNames field
        field.Name, Encode.object pairs

    /// Encode record schemas with map-form fields and preserved schema metadata.
    and encodeInputRecordSchema (schema:InputRecordSchema) : YamlNode =
        let fieldsElement =
            match schema.Fields with
            | Some fs ->
                // Record fields are written in map form for stable names and compact YAML.
                let fieldPairs = fs |> Seq.map encodeInputRecordField |> Seq.toList
                Encode.object fieldPairs
            | None ->
                // Missing fields are emitted as an empty map to keep the schema object valid.
                Encode.object []
        [ "type", encodeString "record"; "fields", fieldsElement ]
        |> appendOpt "label" (encodeSourceText schema.SourceNodes.Label) schema.Label
        |> appendOpt "doc" (encodeSourceText schema.SourceNodes.Doc) schema.Doc
        |> appendOpt "name" encodeString schema.Name
        |> appendDynamicPropertiesExcept InputRecordSchema.KnownFieldNames schema
        |> Encode.object

    /// Encode enum schemas while preserving symbol order and schema metadata.
    and encodeInputEnumSchema (schema:InputEnumSchema) : YamlNode =
        let pairs =
            // Symbol order is semantically significant, so emit the ResizeArray order.
            [ "type", encodeString "enum" ]
            @ [ "symbols", (schema.Symbols |> Seq.map encodeString |> List.ofSeq |> encodeSequence) ]

        pairs
        |> appendOpt "label" (encodeSourceText schema.SourceNodes.Label) schema.Label
        |> appendOpt "doc" (encodeSourceText schema.SourceNodes.Doc) schema.Doc
        |> appendOpt "name" encodeString schema.Name
        |> appendDynamicPropertiesExcept InputEnumSchema.KnownFieldNames schema
        |> Encode.object

    /// Encode array schemas with their item type and preserved schema metadata.
    and encodeInputArraySchema (schema:InputArraySchema) : YamlNode =
        [ "type", encodeString "array"; "items", encodeSchemaType schema.Items ]
        |> appendOpt "label" (encodeSourceText schema.SourceNodes.Label) schema.Label
        |> appendOpt "doc" (encodeSourceText schema.SourceNodes.Doc) schema.Doc
        |> appendOpt "name" encodeString schema.Name
        |> appendDynamicPropertiesExcept InputArraySchema.KnownFieldNames schema
        |> Encode.object

    // ------------------------------
    // Binding & Port encoders
    // ------------------------------

    /// Encode outputBinding fields and preserved extension properties.
    let encodeOutputBinding (ob:OutputBinding) : YamlNode =
        [ ob.Glob |> Option.map (fun g -> "glob", encodeString g) ]
        |> List.choose id
        |> appendOpt "loadContents" Encode.bool ob.LoadContents
        |> appendOpt "loadListing" (LoadListingEnum.toCwlString >> encodeString) ob.LoadListing
        |> appendOpt "outputEval" encodeString ob.OutputEval
        |> appendDynamicPropertiesExcept OutputBinding.KnownFieldNames ob
        |> Encode.object

    /// Encode one source value as a scalar and multiple source values as a sequence.
    let encodeStringArrayOrScalar (values: ResizeArray<string>) : YamlNode =
        if values.Count = 1 then
            encodeString values.[0]
        else
            values
            |> Seq.map encodeString
            |> List.ofSeq
            |> encodeSequence

    let rec encodeCWLParameterValue (value: CWLParameterValue) : YamlNode =
        match value with
        | CWLParameterValue.Null -> Encode.nullValue
        | CWLParameterValue.String value -> encodeString value
        | CWLParameterValue.Int value -> Encode.int64 value
        | CWLParameterValue.Float value -> Encode.float value
        | CWLParameterValue.Boolean value -> Encode.bool value
        | CWLParameterValue.File file ->
            encodeFilePairs "class" "File" file
            |> Encode.object
        | CWLParameterValue.Directory directory ->
            encodeDirectoryPairs "class" "Directory" directory
            |> Encode.object
        | CWLParameterValue.Array values ->
            values
            |> Seq.map encodeCWLParameterValue
            |> Seq.toList
            |> encodeSequence
        | CWLParameterValue.Record fields ->
            fields
            |> Seq.map (fun field -> field.Name, encodeCWLParameterValue field.Value)
            |> Seq.toList
            |> Encode.object

    /// Encode a workflow or tool output, choosing shorthand only when type is the sole field.
    let encodeCWLOutput (o:CWLOutput) : (string * YamlNode) =
        let typeElement = o.Type_ |> Option.map (fun t ->
            // Output type encoding mirrors input type encoding, but wraps complex
            // schema forms under `type` when the port itself is an object.
            match t with
            | Array arraySchema ->
                // Check if we can use short form
                match arraySchema.Items with
                | File _ | Directory _ | Dirent _ | String | Int | Long | Float | Double | Boolean | Any ->
                    encodeCWLType t
                | _ ->
                    // Complex array - need full schema form wrapped in "type"
                    Encode.object [ "type", encodeInputArraySchema arraySchema ]
            | Record recordSchema ->
                // Record needs full schema form wrapped in "type"
                Encode.object [ "type", encodeInputRecordSchema recordSchema ]
            | Enum enumSchema ->
                // Enum needs full schema form wrapped in "type"
                Encode.object [ "type", encodeInputEnumSchema enumSchema ]
            | _ ->
                // Simple types and unions
                encodeCWLType t
        )

        let outputSourceElement =
            // Keep scalar outputSource compact, but use a sequence when multiple
            // upstream step outputs feed this workflow output.
            match o.OutputSource with
            | Some (OutputSource.Single value) -> Some (encodeString value)
            | Some (OutputSource.Multiple values) when values.Count > 0 -> Some (encodeStringArrayOrScalar values)
            | _ -> None
        
        let pairs =
            // Build object-form fields in CWL's conventional order.
            []
            |> appendOpt "type" id typeElement
            |> appendOpt "label" (encodeSourceText o.SourceNodes.Label) o.Label
            |> appendOpt "secondaryFiles" rawYamlNode o.SecondaryFiles
            |> appendOpt "streamable" Encode.bool o.Streamable
            |> appendOpt "doc" (encodeSourceText o.SourceNodes.Doc) o.Doc
            |> appendOpt "format" encodeString o.Format
            |> appendOpt "outputBinding" encodeOutputBinding o.OutputBinding
            |> appendOpt "outputSource" id outputSourceElement
            |> appendDynamicPropertiesExcept CWLOutput.KnownFieldNames o
        match pairs with
        | [ ("type", t) ] ->
            // Map shorthand: outputName: File
            o.Name, t
        | _ ->
            // Always extended form when additional fields like outputSource/outputBinding present
            o.Name, (Encode.object pairs)

    /// Encode inputBinding fields and preserved extension properties.
    let encodeInputBinding (ib:InputBinding) : YamlNode =
        []
        |> appendOpt "loadContents" Encode.bool ib.LoadContents
        |> appendOpt "prefix" encodeString ib.Prefix
        |> appendOpt "position" (fun p -> Encode.int p) ib.Position
        |> appendOpt "itemSeparator" encodeString ib.ItemSeparator
        |> appendOpt "separate" Encode.bool ib.Separate
        |> appendOpt "valueFrom" encodeString ib.ValueFrom
        |> appendOpt "shellQuote" Encode.bool ib.ShellQuote
        |> appendDynamicPropertiesExcept InputBinding.KnownFieldNames ib
        |> Encode.object

    /// Encode the format of an input in the form that it holds, so a list of one name stays a list.
    let encodeInputFormat (format: InputFormat) : YamlNode =
        match format with
        | InputFormat.Single value -> encodeString value
        | InputFormat.Multiple values -> values |> Seq.map encodeString |> List.ofSeq |> encodeSequence

    /// Encode a workflow or tool input, choosing shorthand only when type is the sole field.
    let encodeCWLInput (i:CWLInput) : (string * YamlNode) =
        let typeElement = i.Type_ |> Option.map (fun t ->
            // Input type object wrapping follows the same shorthand/full-schema rules
            // as outputs to avoid losing complex schema metadata.
            match t with
            | Array arraySchema ->
                // Check if we can use short form
                match arraySchema.Items with
                | File _ | Directory _ | Dirent _ | String | Int | Long | Float | Double | Boolean | Any ->
                    encodeCWLType t
                | _ ->
                    // Complex array - need full schema form wrapped in "type"
                    Encode.object [ "type", encodeInputArraySchema arraySchema ]
            | Record recordSchema ->
                // Record needs full schema form wrapped in "type"
                Encode.object [ "type", encodeInputRecordSchema recordSchema ]
            | Enum enumSchema ->
                // Enum needs full schema form wrapped in "type"
                Encode.object [ "type", encodeInputEnumSchema enumSchema ]
            | _ ->
                // Simple types and unions
                encodeCWLType t
        )

        let pairs =
            // Build object-form fields in CWL's conventional order; optional is
            // encoded through the type union shorthand rather than as a separate field.
            []
            |> appendOpt "type" id typeElement
            |> appendOpt "label" (encodeSourceText i.SourceNodes.Label) i.Label
            |> appendOpt "secondaryFiles" rawYamlNode i.SecondaryFiles
            |> appendOpt "streamable" Encode.bool i.Streamable
            |> appendOpt "doc" (encodeSourceText i.SourceNodes.Doc) i.Doc
            |> appendOpt "format" encodeInputFormat i.Format
            |> appendOpt "loadContents" Encode.bool i.LoadContents
            |> appendOpt "loadListing" (LoadListingEnum.toCwlString >> encodeString) i.LoadListing
            |> appendOpt "default" rawYamlNode i.DefaultValue
            |> appendOpt "inputBinding" encodeInputBinding i.InputBinding
            |> appendDynamicPropertiesExcept CWLInput.KnownFieldNames i
        match pairs with
        | [ ("type", t) ] ->
            // Map shorthand: inputName: string
            i.Name, t
        | _ ->
            // Extended form is needed whenever metadata/default/binding/extensions exist.
            i.Name, Encode.object pairs

    // ------------------------------
    // Requirement encoder (always extended style)
    // ------------------------------

    /// Encode one SchemaDefRequirement type entry and preserved schema extension fields.
    let encodeSchemaDefRequirementType (s:SchemaDefRequirementType) : YamlNode =
        [
            "name", encodeString s.Name
            "type", encodeCWLType s.Type_
        ]
        |> appendDynamicPropertiesExcept SchemaDefRequirementType.KnownFieldNames s
        |> Encode.object

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
            |> appendOpt "expressionLib" (fun entries -> entries |> Seq.map encodeString |> List.ofSeq |> encodeSequence) expressionLib
            |> appendDynamicPropertiesExcept InlineJavascriptRequirementValue.KnownFieldNames value
            |> Encode.object
        | SchemaDefRequirement types ->
            // SchemaDefRequirement contains schema entries that already preserve their
            // own extension fields.
            [ "class", encodeString "SchemaDefRequirement";
              "types", (types |> Seq.map encodeSchemaDefRequirementType |> List.ofSeq |> encodeSequence) ] |> Encode.object
        | DockerRequirement dr ->
            // Docker fields are optional and emitted only when present.
            [ "class", encodeString "DockerRequirement" ]
            |> appendOpt "dockerPull" encodeString dr.DockerPull
            |> appendOpt "dockerFile" encodeSchemaSaladString dr.DockerFile
            |> appendOpt "dockerImageId" encodeString dr.DockerImageId
            |> appendOpt "dockerLoad" encodeString dr.DockerLoad
            |> appendOpt "dockerImport" encodeString dr.DockerImport
            |> appendOpt "dockerOutputDirectory" encodeString dr.DockerOutputDirectory
            |> appendOpt "cwltool:dockerRunOptions" (fun values -> values |> Seq.map encodeString |> List.ofSeq |> encodeSequence) dr.DockerRunOptions
            |> appendDynamicPropertiesExcept DockerRequirement.KnownFieldNames dr
            |> Encode.object
        | SoftwareRequirement pkgs ->
            let encodePkg (p:SoftwarePackage) =
                // Extended package form is used here so version/specs/extensions all
                // have explicit keys.
                []
                |> fun acc -> acc @ [ "package", encodeString p.Package ]
                |> appendOpt "version" (fun vs -> vs |> Seq.map encodeString |> List.ofSeq |> encodeSequence) p.Version
                |> appendOpt "specs" (fun vs -> vs |> Seq.map encodeString |> List.ofSeq |> encodeSequence) p.Specs
                |> appendDynamicPropertiesExcept SoftwarePackage.KnownFieldNames p
                |> Encode.object
            [ "class", encodeString "SoftwareRequirement";
              "packages", (pkgs |> Seq.map encodePkg |> List.ofSeq |> encodeSequence) ] |> Encode.object
        | LoadListingRequirement loadListing ->
            // Always emit the concrete loadListing value instead of relying on decoder defaults.
            [ "class", encodeString "LoadListingRequirement"
              "loadListing", encodeString (LoadListingEnum.toCwlString loadListing.LoadListing) ]
            |> appendDynamicPropertiesExcept LoadListingRequirementValue.KnownFieldNames loadListing
            |> Encode.object
        | InitialWorkDirRequirement listing ->
            let encodeInitialWorkDirEntry = function
                | DirentEntry d ->
                    // Dirent entries use their own object form.
                    [ ]
                    |> appendOpt "entryname" encodeSchemaSaladString d.Entryname
                    |> fun acc -> acc @ [ "entry", encodeSchemaSaladString d.Entry ]
                    |> appendOpt "writable" Encode.bool d.Writable
                    |> appendDynamicPropertiesExcept DirentInstance.KnownFieldNames d
                    |> Encode.object
                | StringEntry s ->
                    // String/expression listing entries stay scalar/directive form.
                    encodeSchemaSaladString s
                | FileEntry file ->
                    // File/Directory listing entries use class, not type, as discriminator.
                    encodeFilePairs "class" "File" file |> Encode.object
                | DirectoryEntry directory ->
                    encodeDirectoryPairs "class" "Directory" directory |> Encode.object

            [ "class", encodeString "InitialWorkDirRequirement";
              "listing", (listing |> Seq.map encodeInitialWorkDirEntry |> List.ofSeq |> encodeSequence) ] |> Encode.object
        | EnvVarRequirement envs ->
            let encodeEnv (e:EnvironmentDef) =
                // Env var values are strings, and encodeString quotes values such as true or 1.
                [ "envName", encodeString e.EnvName; "envValue", encodeString e.EnvValue ]
                |> appendDynamicPropertiesExcept EnvironmentDef.KnownFieldNames e
                |> Encode.object
            [ "class", encodeString "EnvVarRequirement";
              "envDef", (envs |> Seq.map encodeEnv |> List.ofSeq |> encodeSequence) ] |> Encode.object
        | ShellCommandRequirement -> [ "class", encodeString "ShellCommandRequirement" ] |> Encode.object
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
            [ "class", encodeString "ResourceRequirement" ] @ knownPairs @ dynamicPairs |> Encode.object
        // Canonicalize class names to short CWL forms where applicable.
        | WorkReuseRequirement workReuse ->
            [ "class", encodeString "WorkReuse"
              "enableReuse", Encode.bool workReuse.EnableReuse ]
            |> appendDynamicPropertiesExcept WorkReuseRequirementValue.KnownFieldNames workReuse
            |> Encode.object
        | WorkReuseExpressionRequirement expression ->
            [ "class", encodeString "WorkReuse"
              "enableReuse", encodeString expression ]
            |> Encode.object
        | NetworkAccessRequirement networkAccess ->
            [ "class", encodeString "NetworkAccess"
              "networkAccess", Encode.bool networkAccess.NetworkAccess ]
            |> appendDynamicPropertiesExcept NetworkAccessRequirementValue.KnownFieldNames networkAccess
            |> Encode.object
        | NetworkAccessExpressionRequirement expression ->
            [ "class", encodeString "NetworkAccess"
              "networkAccess", encodeString expression ]
            |> Encode.object
        | InplaceUpdateRequirement inplaceUpdate ->
            [ "class", encodeString "InplaceUpdateRequirement"
              "inplaceUpdate", Encode.bool inplaceUpdate.InplaceUpdate ]
            |> appendDynamicPropertiesExcept InplaceUpdateRequirementValue.KnownFieldNames inplaceUpdate
            |> Encode.object
        | ToolTimeLimitRequirement tl ->
            let timelimit =
                match tl with
                | ToolTimeLimitSeconds seconds -> Encode.int64 seconds
                | ToolTimeLimitExpression expression ->
                    // The decoder takes a float as an expression, because the time limit is a long or a string. A float
                    // that stays a plain scalar is the number that the file held. Every other text is a string.
                    match YamlNode.plain expression with
                    | Decode.CoreFloat _ -> YamlNode.plain expression
                    | _ -> encodeString expression
            [ "class", encodeString "ToolTimeLimit"; "timelimit", timelimit ] |> Encode.object
        | SubworkflowFeatureRequirement -> [ "class", encodeString "SubworkflowFeatureRequirement" ] |> Encode.object
        | ScatterFeatureRequirement -> [ "class", encodeString "ScatterFeatureRequirement" ] |> Encode.object
        | MultipleInputFeatureRequirement -> [ "class", encodeString "MultipleInputFeatureRequirement" ] |> Encode.object
        | StepInputExpressionRequirement -> [ "class", encodeString "StepInputExpressionRequirement" ] |> Encode.object

    /// Encode known hints as requirements and unknown hints as their original raw YAML.
    let encodeHintEntry (hint: HintEntry) : YamlNode =
        match hint with
        | KnownHint requirement -> encodeRequirement requirement
        | UnknownHint unknownHint -> unknownHint.Raw.Node

    // ------------------------------
    // Workflow step encoders
    // ------------------------------
    
    /// Encode a ResizeArray<string> as either a single string or a sequence
    let encodeSourceArray (sources:ResizeArray<string>) : YamlNode =
        match sources.Count with
        | 1 ->
            // CWL source shorthand: source: input_id
            encodeString sources.[0]
        | _ -> 
            // Several sources are always written as a block sequence.
            sources
            |> Seq.map (fun s -> YamlNode.item (encodeString s))
            |> List.ofSeq
            |> YamlNode.sequence

    let encodeLinkMergeMethod (linkMerge: LinkMergeMethod) : YamlNode =
        encodeString linkMerge.AsCwlString

    let encodePickValueMethod (pickValue: PickValueMethod) : YamlNode =
        encodeString pickValue.AsCwlString

    let encodeScatterMethod (scatterMethod: ScatterMethod) : YamlNode =
        encodeString scatterMethod.AsCwlString

    /// Encode a step input using scalar source shorthand only when no other fields exist.
    let encodeStepInput (si:StepInput) : (string * YamlNode) =
        let pairs =
            []
            |> appendOpt "source" encodeSourceArray si.Source
            |> appendOpt "default" rawYamlNode si.DefaultValue
            |> appendOpt "valueFrom" encodeString si.ValueFrom
            |> appendOpt "linkMerge" encodeLinkMergeMethod si.LinkMerge
            |> appendOpt "pickValue" encodePickValueMethod si.PickValue
            |> appendOpt "doc" (encodeSourceText si.SourceNodes.Doc) si.Doc
            |> appendOpt "loadContents" Encode.bool si.LoadContents
            |> appendOpt "loadListing" encodeString si.LoadListing
            |> appendOpt "label" (encodeSourceText si.SourceNodes.Label) si.Label
            |> appendDynamicPropertiesExcept StepInput.KnownFieldNames si
        match pairs with
        | [ ("source", s) ]
            when
                si.DefaultValue.IsNone
                && si.ValueFrom.IsNone
                && si.LinkMerge.IsNone
                && si.PickValue.IsNone
                && si.Doc.IsNone
                && si.LoadContents.IsNone
                && si.LoadListing.IsNone
                && si.Label.IsNone ->
            si.Id, s
        | _ -> si.Id, Encode.object pairs

    /// Encode all step inputs in map form keyed by step input id.
    let encodeStepInputs (inputs:ResizeArray<StepInput>) : YamlNode =
        inputs
        |> Seq.map encodeStepInput
        |> Seq.toList
        |> Encode.object

    /// Encode record-form step output parameters, including extension fields.
    let encodeStepOutputParameter (so: StepOutputParameter) : YamlNode =
        [ "id", encodeString so.Id ]
        |> appendDynamicPropertiesExcept StepOutputParameter.KnownFieldNames so
        |> Encode.object

    /// Encode step outputs as either scalar ids or record-form output parameters.
    let encodeStepOutputs (outputs: ResizeArray<StepOutput>) : YamlNode =
        outputs
        |> Seq.map (fun output ->
            match output with
            | StepOutputString id -> encodeString id
            | StepOutputRecord outputParameter -> encodeStepOutputParameter outputParameter
        )
        |> List.ofSeq
        |> encodeSequence

    /// Encode scatter as scalar shorthand for one item or a sequence for multiple items.
    let encodeScatter (scatter: ResizeArray<string>) : YamlNode =
        match scatter.Count with
        | 1 -> encodeString scatter.[0]
        | _ -> scatter |> Seq.map encodeString |> List.ofSeq |> encodeSequence

    /// The cwlVersion entry of a process. A process that takes the version of its enclosing document writes none.
    let private cwlVersionPairs (includeCwlVersion: bool) (version: string) : (string * YamlNode) list =
        if includeCwlVersion then [ "cwlVersion", encodeString version ] else []

    /// Encode workflow step run targets as paths or inline processing units.
    let rec encodeWorkflowStepRun (run: WorkflowStepRun) : YamlNode =
        match run with
        | RunString runPath ->
            // External run reference.
            encodeString runPath
        | RunCommandLineTool toolObj ->
            // Inline CommandLineTool run; validate the object payload before encoding.
            match WorkflowStepRunOps.tryGetTool run with
            | Some tool -> encodeToolDescriptionElement true tool
            | None ->
                raise (System.ArgumentException($"RunCommandLineTool must contain CWLToolDescription but got %A{toolObj}"))
        | RunWorkflow workflowObj ->
            // Inline nested Workflow run.
            match WorkflowStepRunOps.tryGetWorkflow run with
            | Some workflow -> encodeWorkflowDescriptionElement true workflow
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
            |> appendOpt "label" (encodeSourceText td.SourceNodes.Label) td.Label
            |> appendOpt "doc" (encodeDocText td.SourceNodes.Doc) td.Doc
            |> appendOptPair (td.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map encodeIntent)
        let withHints =
            match td.Hints with
            | Some h when h.Count > 0 ->
                // Hints are emitted before requirements to match existing fixtures.
                basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> encodeSequence) ]
            | _ -> basePairs
        let withRequirements =
            match td.Requirements with
            | Some r when r.Count > 0 ->
                // Requirements are omitted when absent or empty.
                withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> encodeSequence) ]
            | _ -> withHints
        let withBaseCommand =
            match td.BaseCommand with
            | Some bc when bc.Count > 0 ->
                // baseCommand is always emitted as a sequence here.
                withRequirements @ [ "baseCommand", (bc |> Seq.map encodeString |> List.ofSeq |> encodeSequence) ]
            | _ -> withRequirements
        let withCommandFields =
            // Command-line execution fields belong near baseCommand before ports.
            withBaseCommand
            |> appendOpt "arguments" rawYamlNode td.Arguments
            |> appendOpt "stdin" encodeString td.Stdin
            |> appendOpt "stderr" encodeString td.Stderr
            |> appendOpt "stdout" encodeString td.Stdout
            |> appendOpt "successCodes" (fun codes -> codes |> Seq.map Encode.int |> List.ofSeq |> encodeSequence) td.SuccessCodes
            |> appendOpt "temporaryFailCodes" (fun codes -> codes |> Seq.map Encode.int |> List.ofSeq |> encodeSequence) td.TemporaryFailCodes
            |> appendOpt "permanentFailCodes" (fun codes -> codes |> Seq.map Encode.int |> List.ofSeq |> encodeSequence) td.PermanentFailCodes
        let withInputs =
            // CWL requires inputs, so a tool without inputs writes the empty map, as outputs do.
            let inputs = td.Inputs |> Option.defaultValue (ResizeArray<CWLInput>())
            withCommandFields @ [ "inputs", (inputs |> Seq.map encodeCWLInput |> Seq.toList |> Encode.object) ]
        let withOutputs =
            // Outputs are required by the model and always emitted.
            withInputs @ [ "outputs", (td.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> Encode.object) ]
        let withMetadata =
            match td.Metadata with
            | Some md ->
                // Metadata DynamicObj is serialized as top-level extension fields.
                appendDynamicProperties md withOutputs
            | None -> withOutputs
        withMetadata
        // Also include dynamic fields attached directly to the tool object.
        |> appendDynamicPropertiesExcept CWLToolDescription.KnownFieldNames td
        |> Encode.object

    /// Encode an ExpressionTool element with ordered sections and preserved metadata. The flag tells whether the element
    /// writes its cwlVersion.
    and encodeExpressionToolDescriptionElement (includeCwlVersion: bool) (et: CWLExpressionToolDescription) : YamlNode =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            cwlVersionPairs includeCwlVersion et.CWLVersion
            @ [ "class", encodeString "ExpressionTool" ]
            |> appendOptPair (et.Id |> Option.map (fun id -> "id", encodeString id))
            |> appendOpt "label" (encodeSourceText et.SourceNodes.Label) et.Label
            |> appendOpt "doc" (encodeDocText et.SourceNodes.Doc) et.Doc
            |> appendOptPair (et.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map encodeIntent)
        let withHints =
            match et.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> encodeSequence) ]
            | _ -> basePairs
        let withRequirements =
            match et.Requirements with
            | Some r when r.Count > 0 ->
                withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> encodeSequence) ]
            | _ -> withHints
        let withInputs =
            // CWL requires inputs, so a tool without inputs writes the empty map, as outputs do.
            let inputs = et.Inputs |> Option.defaultValue (ResizeArray<CWLInput>())
            withRequirements @ [ "inputs", (inputs |> Seq.map encodeCWLInput |> Seq.toList |> Encode.object) ]
        let withOutputs =
            withInputs @ [ "outputs", (et.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> Encode.object) ]
        let withExpression =
            // Expression is required and written after ports.
            withOutputs @ [ "expression", encodeExpressionScalar et.Expression ]
        let withMetadata =
            match et.Metadata with
            | Some md ->
                // Metadata DynamicObj is serialized as top-level extension fields.
                appendDynamicProperties md withExpression
            | None -> withExpression
        withMetadata
        // Include dynamic fields attached directly to the expression tool object.
        |> appendDynamicPropertiesExcept CWLExpressionToolDescription.KnownFieldNames et
        |> Encode.object

    /// Encode an Operation element with ordered sections and preserved metadata. The flag tells whether the element
    /// writes its cwlVersion.
    and encodeOperationDescriptionElement (includeCwlVersion: bool) (op: CWLOperationDescription) : YamlNode =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            cwlVersionPairs includeCwlVersion op.CWLVersion
            @ [ "class", encodeString "Operation" ]
            |> appendOptPair (op.Id |> Option.map (fun id -> "id", encodeString id))
            |> appendOpt "label" (encodeSourceText op.SourceNodes.Label) op.Label
            |> appendOpt "doc" (encodeDocText op.SourceNodes.Doc) op.Doc
            |> appendOptPair (op.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map encodeIntent)
        let withHints =
            match op.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> encodeSequence) ]
            | _ -> basePairs
        let withRequirements =
            match op.Requirements with
            | Some r when r.Count > 0 ->
                withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> encodeSequence) ]
            | _ -> withHints
        let withInputs =
            // Operation inputs are required by the model.
            withRequirements @ [ "inputs", (op.Inputs |> Seq.map encodeCWLInput |> Seq.toList |> Encode.object) ]
        let withOutputs =
            // Operation outputs are required by the model.
            withInputs @ [ "outputs", (op.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> Encode.object) ]
        let withMetadata =
            match op.Metadata with
            | Some md ->
                // Metadata DynamicObj is serialized as top-level extension fields.
                appendDynamicProperties md withOutputs
            | None -> withOutputs
        withMetadata
        // Include dynamic fields attached directly to the operation object.
        |> appendDynamicPropertiesExcept CWLOperationDescription.KnownFieldNames op
        |> Encode.object

    /// Encode a Workflow element with ordered sections and preserved metadata. The flag tells whether the element
    /// writes its cwlVersion.
    and encodeWorkflowDescriptionElement (includeCwlVersion: bool) (wd: CWLWorkflowDescription) : YamlNode =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            cwlVersionPairs includeCwlVersion wd.CWLVersion
            @ [ "class", encodeString "Workflow" ]
            |> appendOptPair (wd.Id |> Option.map (fun id -> "id", encodeString id))
            |> appendOpt "label" (encodeSourceText wd.SourceNodes.Label) wd.Label
            |> appendOpt "doc" (encodeDocText wd.SourceNodes.Doc) wd.Doc
            |> appendOptPair (wd.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map encodeIntent)
        let withHints =
            match wd.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> encodeSequence) ]
            | _ -> basePairs
        let withRequirements =
            match wd.Requirements with
            | Some r when r.Count > 0 ->
                withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> encodeSequence) ]
            | _ -> withHints
        let withInputs =
            // Workflow inputs are required and emitted before steps.
            withRequirements @ [ "inputs", (wd.Inputs |> Seq.map encodeCWLInput |> Seq.toList |> Encode.object) ]
        let withSteps =
            // Steps are map entries keyed by step id.
            withInputs @ [ "steps", (wd.Steps |> Seq.map encodeWorkflowStep |> Seq.toList |> Encode.object) ]
        let withOutputs =
            // Outputs follow steps for scan-friendly workflow YAML.
            withSteps @ [ "outputs", (wd.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> Encode.object) ]
        let withMetadata =
            match wd.Metadata with
            | Some md ->
                // Metadata DynamicObj is serialized as top-level extension fields.
                appendDynamicProperties md withOutputs
            | None -> withOutputs
        withMetadata
        // Include dynamic fields attached directly to the workflow object.
        |> appendDynamicPropertiesExcept CWLWorkflowDescription.KnownFieldNames wd
        |> Encode.object

    /// Encode one workflow step as a map entry keyed by step id.
    and encodeWorkflowStep (ws:WorkflowStep) : (string * YamlNode) =
        let basePairs =
            // Required step fields are emitted first.
            [ "run", encodeWorkflowStepRun ws.Run
              "in", encodeStepInputs ws.In
              "out", encodeStepOutputs ws.Out ]
            |> appendOpt "label" (encodeSourceText ws.SourceNodes.Label) ws.Label
            |> appendOpt "doc" (encodeSourceText ws.SourceNodes.Doc) ws.Doc
            |> appendOpt "scatter" encodeScatter ws.Scatter
            |> appendOpt "scatterMethod" encodeScatterMethod ws.ScatterMethod
            |> appendOpt "when" encodeString ws.When_
            |> appendDynamicPropertiesExcept WorkflowStep.KnownFieldNames ws
        let withHints =
            match ws.Hints with
            // Step-local hints are included only when non-empty.
            | Some h when h.Count > 0 -> basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> encodeSequence) ]
            | _ -> basePairs
        let withReq =
            match ws.Requirements with
            // Step-local requirements follow hints.
            | Some r when r.Count > 0 -> withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> encodeSequence) ]
            | _ -> withHints
        ws.Id, Encode.object withReq

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

    let encodeCWLParameterReference (reference: CWLParameterReference) =
        let encodedValue =
            reference.Value
            |> Option.map encodeCWLParameterValue
            |> Option.defaultValue (encodeSequence [])

        let dynamicPairs =
            DynamicObjHelpers.dynamicPropertiesExcept
                CWLParameterReference.KnownFieldNames
                reference
            |> Seq.choose (fun property ->
                encodeDynamicValue property.Value
                |> Option.map (fun encoded -> property.Key, encoded)
            )
            |> Seq.toList

        let inferredType =
            reference.Value
            |> Option.bind CWLParameterValue.tryInferType

        let explicitTypePair =
            match reference.Type with
            | Some type_ when inferredType <> Some type_ ->
                Some ("type", encodeCWLType type_)
            | _ -> None

        let encodedReference =
            match explicitTypePair, dynamicPairs, encodedValue.Kind with
            | None, [], _ -> encodedValue
            | None, overflow, YamlNodeKind.Mapping collection ->
                // A record value takes the extension fields next to its own fields.
                let valuePairs =
                    collection.Items
                    |> List.map (fun entry -> Decode.entryName entry, entry.Value)
                let valueKeys = valuePairs |> Seq.map fst |> Set.ofSeq
                let uniqueOverflow =
                    overflow
                    |> List.filter (fun (key, _) -> not (Set.contains key valueKeys))
                Encode.object (valuePairs @ uniqueOverflow)
            | typePair, overflow, _ ->
                [
                    yield! typePair |> Option.toList
                    yield "value", encodedValue
                    yield! overflow
                ]
                |> Encode.object

        reference.Key, encodedReference

    let encodeYAMLParameterFile (references: CWLParameterReference ResizeArray) =
        references
        |> Seq.map encodeCWLParameterReference
        |> Seq.toList
        |> Encode.object
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
        encodeWorkflowDescriptionElement true wd
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

    /// Encode a CWLType in flow style, so that the written text has no line break and can be embedded in JSON.
    /// Unions are lists, and schemas are mappings with their fields in the array form.
    let rec encodeCWLTypeFlow (t: CWLType) : YamlNode =
        let flowObject (pairs: (string * YamlNode) list) =
            YamlNode.flowMapping [ for (name, value) in pairs -> YamlNode.entry (Encode.string name) value ]
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
                    |> Seq.map (fun field -> flowObject [ "name", Encode.string field.Name; "type", encodeCWLTypeFlow field.Type ])
                    |> Seq.toList
                | None -> []
            flowObject [ "type", Encode.string "record"; "fields", flowList fields ]
        | Enum enumSchema ->
            // Symbols are emitted in stored order.
            flowObject [ "type", Encode.string "enum"; "symbols", enumSchema.Symbols |> Seq.map Encode.string |> Seq.toList |> flowList ]
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

