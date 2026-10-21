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
        | Null -> PrimitiveType "null"
        | Stdout -> PrimitiveType "stdout"
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

    /// Determine if a type requires full YAML serialization (complex type)
    let rec isComplexType (t: CWLType) : bool =
        match t with
        | Record _ | Enum _ -> true
        | Array arraySchema ->
            // Complex if array doesn't have shorthand (array of record/enum)
            tryGetArrayShorthand arraySchema.Items |> Option.isNone
        | Union types ->
            // Complex if not a simple optional type
            let typesList = types |> Seq.toList
            match typesList with
            | [Null; otherType] | [otherType; Null] ->
                isComplexType otherType
            | _ -> true // Multi-type union is complex
        | _ -> false

    // ------------------------------
    // Layout helpers for strings and lists
    // ------------------------------

    /// A literal block scalar. The stored chomping is a preference for a value without a final line feed. The
    /// writer takes the chomping that reads the text back as the value whenever the value ends in line feeds,
    /// so nothing here derives it.
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
        let sequenceItems = items |> List.map YamlNode.item
        if items |> List.forall isInlineScalar then
            YamlNode.flowSequence sequenceItems
        else
            YamlNode.sequence sequenceItems

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

    /// Normalize documentation line endings and trim only trailing newline markers.
    let normalizeDocString (doc:string) =
        doc.Replace("\r\n","\n").TrimEnd('\n').TrimEnd('\r')

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

    let encodeDoc (doc:string) : (string * YamlNode) =
        "doc", encodeString (normalizeDocString doc)

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

    /// Encode CWL types using shorthand when possible and full schema objects when required.
    let rec encodeCWLType (t:CWLType) : YamlNode =
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
        | Stdout -> encodeString "stdout"
        | Null -> encodeString "null"
        | Union types ->
            // Check if this is an optional type (union of null and one other type).
            // Only those unions can use CWL's `?` shorthand.
            let typesList = types |> Seq.toList
            match typesList with
            | [Null; otherType] | [otherType; Null] ->
                // Optional type - use short form with "?"
                match otherType with
                | File _ -> encodeString "File?"
                | Directory _ -> encodeString "Directory?"
                | String -> encodeString "string?"
                | Int -> encodeString "int?"
                | Long -> encodeString "long?"
                | Float -> encodeString "float?"
                | Double -> encodeString "double?"
                | Boolean -> encodeString "boolean?"
                | Array arraySchema ->
                    // Optional array - use recursive shorthand detection
                    match tryGetArrayShorthand arraySchema.Items with
                    | Some shorthand -> encodeString (shorthand + "[]?")
                    | None ->
                        // Complex optional array - use full form
                        encodeSequence [ encodeString "null"; encodeInputArraySchema arraySchema ]
                | _ ->
                    // Complex optional type - use array form [null, type]
                    typesList |> List.map encodeCWLType |> encodeSequence
            | _ ->
                // General union - use array form
                typesList |> List.map encodeCWLType |> encodeSequence
        | Array arraySchema ->
            // Try to use short form for arrays (handles arbitrary nesting depth
            // recursively); fall back to schema object for complex item types.
            match tryGetArrayShorthand arraySchema.Items with
            | Some shorthand -> encodeString (shorthand + "[]")
            | None -> encodeInputArraySchema arraySchema
        | Record recordSchema -> encodeInputRecordSchema recordSchema
        | Enum enumSchema -> encodeInputEnumSchema enumSchema

    // ------------------------------
    // InputRecordSchema encoders
    // ------------------------------

    /// Encode a record field as a map entry keyed by its field name.
    and encodeInputRecordField (field:InputRecordField) : (string * YamlNode) =
        let pairs =
            // The field name is emitted as the mapping key, so the nested value only
            // contains field payload such as type/doc/label/extensions.
            [ "type", encodeCWLType field.Type ]
            |> appendOpt "doc" encodeString field.Doc
            |> appendOpt "label" encodeString field.Label
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
        |> appendOpt "label" encodeString schema.Label
        |> appendOpt "doc" encodeString schema.Doc
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
        |> appendOpt "label" encodeString schema.Label
        |> appendOpt "doc" encodeString schema.Doc
        |> appendOpt "name" encodeString schema.Name
        |> appendDynamicPropertiesExcept InputEnumSchema.KnownFieldNames schema
        |> Encode.object

    /// Encode array schemas with their item type and preserved schema metadata.
    and encodeInputArraySchema (schema:InputArraySchema) : YamlNode =
        [ "type", encodeString "array"; "items", encodeCWLType schema.Items ]
        |> appendOpt "label" encodeString schema.Label
        |> appendOpt "doc" encodeString schema.Doc
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
            | Union types ->
                // Check if this is a simple optional (encodeCWLType handles the short form)
                let typesList = types |> Seq.toList
                match typesList with
                | [Null; otherType] | [otherType; Null] ->
                    // Simple optional or optional simple array - use short form
                    match otherType with
                    | File _ | Directory _ | String | Int | Long | Float | Double | Boolean ->
                        encodeCWLType t
                    | Array arraySchema ->
                        match arraySchema.Items with
                        | File _ | Directory _ | String | Int | Long | Float | Double | Boolean ->
                            encodeCWLType t
                        | _ ->
                            // Complex optional array
                            encodeCWLType t
                    | _ ->
                        // Complex optional type
                        encodeCWLType t
                | _ ->
                    // General union
                    encodeCWLType t
            | Array arraySchema ->
                // Check if we can use short form
                match arraySchema.Items with
                | File _ | Directory _ | Dirent _ | String | Int | Long | Float | Double | Boolean ->
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
                // Simple types
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
            |> appendOpt "label" encodeString o.Label
            |> appendOpt "secondaryFiles" rawYamlNode o.SecondaryFiles
            |> appendOpt "streamable" Encode.bool o.Streamable
            |> appendOpt "doc" encodeString o.Doc
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

    /// Encode a workflow or tool input, choosing shorthand only when type is the sole field.
    let encodeCWLInput (i:CWLInput) : (string * YamlNode) =
        let typeElement = i.Type_ |> Option.map (fun t ->
            // Input type object wrapping follows the same shorthand/full-schema rules
            // as outputs to avoid losing complex schema metadata.
            match t with
            | Union types ->
                // Check if this is a simple optional (encodeCWLType handles the short form)
                let typesList = types |> Seq.toList
                match typesList with
                | [Null; otherType] | [otherType; Null] ->
                    // Simple optional or optional simple array - use short form
                    match otherType with
                    | File _ | Directory _ | String | Int | Long | Float | Double | Boolean ->
                        encodeCWLType t
                    | Array arraySchema ->
                        match arraySchema.Items with
                        | File _ | Directory _ | String | Int | Long | Float | Double | Boolean ->
                            encodeCWLType t
                        | _ ->
                            // Complex optional array
                            encodeCWLType t
                    | _ ->
                        // Complex optional type
                        encodeCWLType t
                | _ ->
                    // General union
                    encodeCWLType t
            | Array arraySchema ->
                // Check if we can use short form
                match arraySchema.Items with
                | File _ | Directory _ | Dirent _ | String | Int | Long | Float | Double | Boolean ->
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
                // Simple types
                encodeCWLType t
        )
        
        let pairs =
            // Build object-form fields in CWL's conventional order; optional is
            // encoded through the type union shorthand rather than as a separate field.
            []
            |> appendOpt "type" id typeElement
            |> appendOpt "label" encodeString i.Label
            |> appendOpt "secondaryFiles" rawYamlNode i.SecondaryFiles
            |> appendOpt "streamable" Encode.bool i.Streamable
            |> appendOpt "doc" encodeString i.Doc
            |> appendOpt "format" encodeString i.Format
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
                | ToolTimeLimitExpression expression -> encodeString expression
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
            |> appendOpt "doc" encodeString si.Doc
            |> appendOpt "loadContents" Encode.bool si.LoadContents
            |> appendOpt "loadListing" encodeString si.LoadListing
            |> appendOpt "label" encodeString si.Label
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

    /// Encode workflow step run targets as paths or inline processing units.
    let rec encodeWorkflowStepRun (run: WorkflowStepRun) : YamlNode =
        match run with
        | RunString runPath ->
            // External run reference.
            encodeString runPath
        | RunCommandLineTool toolObj ->
            // Inline CommandLineTool run; validate the object payload before encoding.
            match WorkflowStepRunOps.tryGetTool run with
            | Some tool -> encodeToolDescriptionElement tool
            | None ->
                raise (System.ArgumentException($"RunCommandLineTool must contain CWLToolDescription but got %A{toolObj}"))
        | RunWorkflow workflowObj ->
            // Inline nested Workflow run.
            match WorkflowStepRunOps.tryGetWorkflow run with
            | Some workflow -> encodeWorkflowDescriptionElement workflow
            | None ->
                raise (System.ArgumentException($"RunWorkflow must contain CWLWorkflowDescription but got %A{workflowObj}"))
        | RunExpressionTool expressionToolObj ->
            // Inline ExpressionTool run.
            match WorkflowStepRunOps.tryGetExpressionTool run with
            | Some expressionTool -> encodeExpressionToolDescriptionElement expressionTool
            | None ->
                raise (System.ArgumentException($"RunExpressionTool must contain CWLExpressionToolDescription but got %A{expressionToolObj}"))
        | RunOperation operationObj ->
            // Inline Operation run.
            match WorkflowStepRunOps.tryGetOperation run with
            | Some operation -> encodeOperationDescriptionElement operation
            | None ->
                raise (System.ArgumentException($"RunOperation must contain CWLOperationDescription but got %A{operationObj}"))

    /// Encode a CommandLineTool element with ordered sections and preserved metadata.
    and encodeToolDescriptionElement (td: CWLToolDescription) : YamlNode =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            [ "cwlVersion", encodeString td.CWLVersion
              "class", encodeString "CommandLineTool" ]
            |> appendOptPair (td.Id |> Option.map (fun id -> "id", encodeString id))
            |> appendOptPair (td.Label |> Option.map encodeLabel)
            |> appendOptPair (td.Doc |> Option.map encodeDoc)
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
            match td.Inputs with
            | Some i when i.Count > 0 ->
                // Inputs are optional for CommandLineTool.
                withCommandFields @ [ "inputs", (i |> Seq.map encodeCWLInput |> Seq.toList |> Encode.object) ]
            | _ -> withCommandFields
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

    /// Encode an ExpressionTool element with ordered sections and preserved metadata.
    and encodeExpressionToolDescriptionElement (et: CWLExpressionToolDescription) : YamlNode =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            [ "cwlVersion", encodeString et.CWLVersion
              "class", encodeString "ExpressionTool" ]
            |> appendOptPair (et.Id |> Option.map (fun id -> "id", encodeString id))
            |> appendOptPair (et.Label |> Option.map encodeLabel)
            |> appendOptPair (et.Doc |> Option.map encodeDoc)
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
            match et.Inputs with
            | Some i when i.Count > 0 ->
                withRequirements @ [ "inputs", (i |> Seq.map encodeCWLInput |> Seq.toList |> Encode.object) ]
            | _ -> withRequirements
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

    /// Encode an Operation element with ordered sections and preserved metadata.
    and encodeOperationDescriptionElement (op: CWLOperationDescription) : YamlNode =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            [ "cwlVersion", encodeString op.CWLVersion
              "class", encodeString "Operation" ]
            |> appendOptPair (op.Id |> Option.map (fun id -> "id", encodeString id))
            |> appendOptPair (op.Label |> Option.map encodeLabel)
            |> appendOptPair (op.Doc |> Option.map encodeDoc)
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

    /// Encode a Workflow element with ordered sections and preserved metadata.
    and encodeWorkflowDescriptionElement (wd: CWLWorkflowDescription) : YamlNode =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            [ "cwlVersion", encodeString wd.CWLVersion
              "class", encodeString "Workflow" ]
            |> appendOptPair (wd.Id |> Option.map (fun id -> "id", encodeString id))
            |> appendOptPair (wd.Label |> Option.map encodeLabel)
            |> appendOptPair (wd.Doc |> Option.map encodeDoc)
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
            |> appendOpt "label" encodeString ws.Label
            |> appendOpt "doc" encodeString ws.Doc
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

    let writeYaml (element: YamlNode) =
        // Two spaces match the fixtures.
        Writer.writeNode { WriterOptions.defaults with Indent = 2 } element

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

    /// True when the last thing that the writer prints for the node is a block scalar that ends in a blank line.
    /// A block scalar with the chomping keep reads every blank line behind its body as part of its value, and a
    /// value that ends in a blank line is written with keep. A blank line may therefore not follow such a node.
    let rec private endsInBlankLine (node: YamlNode) : bool =
        match node.Kind with
        | YamlNodeKind.Scalar scalar ->
            match scalar.Style with
            | ScalarStyle.Block (_, chomping, _) ->
                scalar.Value.EndsWith("\n")
                && (chomping = ChompingMode.Keep || scalar.Value = "\n" || scalar.Value.EndsWith("\n\n"))
            | _ -> false
        | YamlNodeKind.Mapping collection when collection.Style = CollectionStyle.Block ->
            collection.Items |> List.tryLast |> Option.exists (fun entry -> endsInBlankLine entry.Value)
        | YamlNodeKind.Sequence collection when collection.Style = CollectionStyle.Block ->
            collection.Items |> List.tryLast |> Option.exists (fun item -> endsInBlankLine item.Node)
        | _ -> false

    /// Render top-level CWL sections in a stable order while keeping metadata as a final block. The document is
    /// one mapping, and a blank line separates the sections. The writer prints it once, so only the end of the
    /// document ends a text, and the end of a section is no end of a scalar.
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

        let separated =
            sections
            |> Array.mapi (fun index section ->
                match section with
                | first :: rest when index > 0 && not (endsInBlankLine (List.last sections.[index - 1]).Value) ->
                    // The first key of a section follows a blank line. After a block scalar that keeps its blank
                    // lines, the lines of the scalar are that separator.
                    { first with Before = Trivia.BlankLine None :: first.Before } :: rest
                | _ -> section)
            |> List.concat

        if separated.IsEmpty then
            ""
        else
            YamlNode.mapping separated |> writeYaml

    let encodeToolDescription (td:CWLToolDescription) : string =
        // Tool top-level layout places command fields before ports.
        encodeToolDescriptionElement td
        |> renderTopLevelElement ["cwlVersion"; "class"; "id"; "label"; "doc"; "intent"] ["hints"; "requirements"; "baseCommand"; "inputs"; "outputs"]

    let encodeWorkflowDescription (wd:CWLWorkflowDescription) : string =
        // Workflow layout keeps inputs, steps, and outputs as separate scan blocks.
        encodeWorkflowDescriptionElement wd
        |> renderTopLevelElement ["cwlVersion"; "class"; "id"; "label"; "doc"; "intent"] ["hints"; "requirements"; "inputs"; "steps"; "outputs"]

    let encodeExpressionToolDescription (et:CWLExpressionToolDescription) : string =
        // ExpressionTool layout writes expression after ports.
        encodeExpressionToolDescriptionElement et
        |> renderTopLevelElement ["cwlVersion"; "class"; "id"; "label"; "doc"; "intent"] ["hints"; "requirements"; "inputs"; "outputs"; "expression"]

    let encodeOperationDescription (op: CWLOperationDescription) : string =
        // Operation layout mirrors workflow/tool metadata and port ordering.
        encodeOperationDescriptionElement op
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

