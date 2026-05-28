namespace ARCtrl.CWL

open System
open DynamicObj
open YAMLicious
open YAMLicious.YAMLiciousTypes
open YAMLicious.Writer
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
    // Basic boolean encoder with lowercase letters
    // ------------------------------
    let yBool (b:bool) =
        YAMLElement.Value (YAMLContent.create (if b then "true" else "false"))

    /// Encode floats with round-trip formatting on .NET while keeping Fable output usable.
    let yFloat (value: float) =
#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        YAMLElement.Value (YAMLContent.create (value.ToString()))
#else
        YAMLElement.Value (YAMLContent.create (value.ToString("R", System.Globalization.CultureInfo.InvariantCulture)))
#endif

    // ------------------------------
    // Helper to build YAML mappings preserving order
    // ------------------------------
    let yMap (pairs: (string * YAMLElement) list) =
        // Represent a mapping as an Object containing Mapping nodes preserving order.
        let normalize = function
            // Preserve wrapped scalars and aliases because YAMLicious uses these
            // wrappers to retain style information.
            | (YAMLElement.Object [YAMLElement.Value _]) as wrapped -> wrapped
            | (YAMLElement.Object [YAMLElement.Alias _]) as wrapped -> wrapped
            // Legacy helpers often return Object [single] for nested values; unwrap
            // those to avoid one unnecessary object layer in emitted YAML.
            | YAMLElement.Object [single] -> single // unwrap single wrapped value (legacy helper usage)
            | other -> other
        pairs
        |> List.map (fun (k,v) -> YAMLElement.Mapping (YAMLContent.create k, normalize v))
        |> YAMLElement.Object

    // ------------------------------
    // Helper append functions for ordered construction
    // ------------------------------
    let inline appendOpt name (encoder:'a -> YAMLElement) (value:'a option) acc =
        match value with
        | Some v -> acc @ [name, encoder v]
        | None -> acc

    /// Encode preserved DynamicObj extension values when they can be represented as YAML.
    let rec encodeDynamicValue (value: obj) =
        match value with
        | null -> None
        // Direct scalar values map to YAML scalars.
        | :? string as s -> Some (Encode.string s)
        | :? bool as b -> Some (yBool b)
        // Fable JavaScript/TypeScript use `number` for int and float; avoid truncating decimal values through Encode.int.
        | :? float as f -> Some (yFloat f)
        | :? int as i -> Some (Encode.int i)
        | :? int64 as i -> Some (Encode.string (string i))
        // Already-parsed YAML is written back without reinterpretation.
        | :? YAMLElement as y -> Some y
        // Nested DynamicObj extension payloads become nested YAML mappings.
        | :? DynamicObj as dynObj -> Some (encodeDynamicObj dynObj)
        | :? System.Collections.IEnumerable as values ->
            // Generic collections are encoded item-by-item, silently dropping values
            // that cannot be represented as YAML.
            values
            |> Seq.cast<obj>
            |> Seq.choose encodeDynamicValue
            |> Seq.toList
            |> YAMLElement.Sequence
            |> Some
        | _ -> None

    /// Encode all dynamic extension properties on a DynamicObj as a YAML mapping.
    and encodeDynamicObj (dynObj: DynamicObj) =
        DynamicObjHelpers.dynamicPropertiesExcept Set.empty dynObj
        |> Seq.choose (fun kvp -> encodeDynamicValue kvp.Value |> Option.map (fun encoded -> kvp.Key, encoded))
        |> Seq.toList
        |> yMap

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
    /// Multi-line expressions are emitted as literal block scalars with clip chomping
    /// so trailing newlines and blank lines survive decode/encode roundtrips.
    let encodeExpressionScalar (expression: string) : YAMLElement =
        let normalized =
            // Normalize line endings once so style selection and emitted text agree.
            if isNull expression then "" else expression.Replace("\r\n", "\n").Replace("\r", "\n")

        let style =
            // Literal block style keeps multi-line JS expressions readable and avoids
            // YAML interpreting punctuation inside expressions.
            if normalized.Contains("\n") then
                ScalarStyle.Block(BlockScalarStyle.Literal, ChompingMode.Clip, None)
            else
                // Single-line expressions are quoted for parser safety.
                ScalarStyle.DoubleQuoted

        YAMLElement.Value (YAMLContent.create(normalized, style = style))

    /// Encode schema-salad strings as literals or directive mappings.
    let encodeSchemaSaladString (value: SchemaSaladString) : YAMLElement =
        match value with
        | SchemaSaladString.Literal text -> Encode.string text
        | SchemaSaladString.Include path -> yMap [ "$include", Encode.string path ]
        | SchemaSaladString.Import path -> yMap [ "$import", Encode.string path ]

    /// Quote boolean-looking EnvVar values so YAML does not coerce them on decode.
    let normalizeEnvValueForEncode (envValue: string) =
        if envValue = "true" || envValue = "false" then "\"" + envValue + "\"" else envValue

    /// Encode EnvVarRequirement using compact map shorthand (envName -> envValue).
    let encodeEnvVarRequirementCompactMap (envs: ResizeArray<EnvironmentDef>) : YAMLElement =
        let envDefMap =
            // Compact map form writes env names as keys.
            envs
            |> Seq.map (fun env -> env.EnvName, Encode.string (normalizeEnvValueForEncode env.EnvValue))
            |> Seq.toList
            |> yMap
        [ "class", Encode.string "EnvVarRequirement"
          "envDef", envDefMap ]
        |> yMap

    /// Encode SoftwareRequirement using compact map shorthand.
    let encodeSoftwareRequirementCompactMap (packages: ResizeArray<SoftwarePackage>) : YAMLElement =
        let encodePackageValue (package: SoftwarePackage) =
            match package.Version, package.Specs with
            // Empty object means package present with no version/specs constraints.
            | None, None -> yMap []
            // Specs-only packages can use sequence shorthand.
            | None, Some specs -> specs |> Seq.map Encode.string |> Seq.toList |> YAMLElement.Sequence
            | _ ->
                // Full package object keeps version and specs separate.
                []
                |> appendOpt "version" (fun values -> values |> Seq.map Encode.string |> Seq.toList |> YAMLElement.Sequence) package.Version
                |> appendOpt "specs" (fun values -> values |> Seq.map Encode.string |> Seq.toList |> YAMLElement.Sequence) package.Specs
                |> yMap

        let packagesMap =
            packages
            |> Seq.map (fun package -> package.Package, encodePackageValue package)
            |> Seq.toList
            |> yMap

        [ "class", Encode.string "SoftwareRequirement"
          "packages", packagesMap ]
        |> yMap

    let encodeLabel (label:string) : (string * YAMLElement) =
        "label", Encode.string label

    let encodeDoc (doc:string) : (string * YAMLElement) =
        "doc", Encode.string (normalizeDocString doc)

    let encodeIntent (intent: ResizeArray<string>) : (string * YAMLElement) =
        "intent", (intent |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence)

    let inline appendOptPair pairOpt acc =
        match pairOpt with
        | Some pair -> acc @ [pair]
        | None -> acc

    // ------------------------------
    // CWLType encoder
    // ------------------------------
    /// Encode File fields with the supplied discriminator key (`type` or `class`).
    let encodeFilePairs discriminatorKey discriminatorValue (file: FileInstance) =
        [ discriminatorKey, Encode.string discriminatorValue ]
        |> appendOpt "location" Encode.string file.Location
        |> appendOpt "path" Encode.string file.Path
        |> appendOpt "basename" Encode.string file.Basename
        |> appendOpt "dirname" Encode.string file.Dirname
        |> appendOpt "nameroot" Encode.string file.Nameroot
        |> appendOpt "nameext" Encode.string file.Nameext
        |> appendOpt "checksum" Encode.string file.Checksum
        |> appendOpt "size" (fun size -> YAMLElement.Value (YAMLContent.create (string size))) file.Size
        |> appendOpt "secondaryFiles" id file.SecondaryFiles
        |> appendOpt "format" Encode.string file.Format
        |> appendOpt "contents" Encode.string file.Contents
        |> appendDynamicPropertiesExcept FileInstance.KnownFieldNames file

    /// Encode Directory fields with the supplied discriminator key (`type` or `class`).
    let encodeDirectoryPairs discriminatorKey discriminatorValue (directory: DirectoryInstance) =
        [ discriminatorKey, Encode.string discriminatorValue ]
        |> appendOpt "location" Encode.string directory.Location
        |> appendOpt "path" Encode.string directory.Path
        |> appendOpt "basename" Encode.string directory.Basename
        |> appendOpt "listing" id directory.Listing
        |> appendDynamicPropertiesExcept DirectoryInstance.KnownFieldNames directory

    /// Encode CWL types using shorthand when possible and full schema objects when required.
    let rec encodeCWLType (t:CWLType) : YAMLElement =
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
            encodeFilePairs "type" "File" file |> yMap
        | File _ -> Encode.string "File"
        | Directory directory when hasDirectoryFields directory ->
            // Preserve Directory metadata such as listing.
            encodeDirectoryPairs "type" "Directory" directory |> yMap
        | Directory _ -> Encode.string "Directory"
        | Dirent d ->
            // Dirent is always structured because it has required entry plus optional
            // entryname/writable fields.
            [ "entry", encodeSchemaSaladString d.Entry ]
            |> appendOpt "entryname" encodeSchemaSaladString d.Entryname
            |> appendOpt "writable" (fun b -> yBool b) d.Writable
            |> appendDynamicPropertiesExcept DirentInstance.KnownFieldNames d
            |> yMap
        | String -> Encode.string "string"
        | Int -> Encode.string "int"
        | Long -> Encode.string "long"
        | Float -> Encode.string "float"
        | Double -> Encode.string "double"
        | Boolean -> Encode.string "boolean"
        | Stdout -> Encode.string "stdout"
        | Null -> Encode.string "null"
        | Union types ->
            // Check if this is an optional type (union of null and one other type).
            // Only those unions can use CWL's `?` shorthand.
            let typesList = types |> Seq.toList
            match typesList with
            | [Null; otherType] | [otherType; Null] ->
                // Optional type - use short form with "?"
                match otherType with
                | File _ -> Encode.string "File?"
                | Directory _ -> Encode.string "Directory?"
                | String -> Encode.string "string?"
                | Int -> Encode.string "int?"
                | Long -> Encode.string "long?"
                | Float -> Encode.string "float?"
                | Double -> Encode.string "double?"
                | Boolean -> Encode.string "boolean?"
                | Array arraySchema ->
                    // Optional array - use recursive shorthand detection
                    match tryGetArrayShorthand arraySchema.Items with
                    | Some shorthand -> Encode.string (shorthand + "[]?")
                    | None ->
                        // Complex optional array - use full form
                        YAMLElement.Sequence [ Encode.string "null"; encodeInputArraySchema arraySchema ]
                | _ ->
                    // Complex optional type - use array form [null, type]
                    typesList |> List.map encodeCWLType |> YAMLElement.Sequence
            | _ ->
                // General union - use array form
                typesList |> List.map encodeCWLType |> YAMLElement.Sequence
        | Array arraySchema ->
            // Try to use short form for arrays (handles arbitrary nesting depth
            // recursively); fall back to schema object for complex item types.
            match tryGetArrayShorthand arraySchema.Items with
            | Some shorthand -> Encode.string (shorthand + "[]")
            | None -> encodeInputArraySchema arraySchema
        | Record recordSchema -> encodeInputRecordSchema recordSchema
        | Enum enumSchema -> encodeInputEnumSchema enumSchema

    // ------------------------------
    // InputRecordSchema encoders
    // ------------------------------

    /// Encode a record field as a map entry keyed by its field name.
    and encodeInputRecordField (field:InputRecordField) : (string * YAMLElement) =
        let pairs =
            // The field name is emitted as the mapping key, so the nested value only
            // contains field payload such as type/doc/label/extensions.
            [ "type", encodeCWLType field.Type ]
            |> appendOpt "doc" Encode.string field.Doc
            |> appendOpt "label" Encode.string field.Label
            |> appendDynamicPropertiesExcept InputRecordField.KnownFieldNames field
        field.Name, yMap pairs

    /// Encode record schemas with map-form fields and preserved schema metadata.
    and encodeInputRecordSchema (schema:InputRecordSchema) : YAMLElement =
        let fieldsElement =
            match schema.Fields with
            | Some fs ->
                // Record fields are written in map form for stable names and compact YAML.
                let fieldPairs = fs |> Seq.map encodeInputRecordField |> Seq.toList
                yMap fieldPairs
            | None ->
                // Missing fields are emitted as an empty map to keep the schema object valid.
                yMap []
        [ "type", Encode.string "record"; "fields", fieldsElement ]
        |> appendOpt "label" Encode.string schema.Label
        |> appendOpt "doc" Encode.string schema.Doc
        |> appendOpt "name" Encode.string schema.Name
        |> appendDynamicPropertiesExcept InputRecordSchema.KnownFieldNames schema
        |> yMap

    /// Encode enum schemas while preserving symbol order and schema metadata.
    and encodeInputEnumSchema (schema:InputEnumSchema) : YAMLElement =
        let pairs =
            // Symbol order is semantically significant, so emit the ResizeArray order.
            [ "type", Encode.string "enum" ]
            @ [ "symbols", (schema.Symbols |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) ]

        pairs
        |> appendOpt "label" Encode.string schema.Label
        |> appendOpt "doc" Encode.string schema.Doc
        |> appendOpt "name" Encode.string schema.Name
        |> appendDynamicPropertiesExcept InputEnumSchema.KnownFieldNames schema
        |> yMap

    /// Encode array schemas with their item type and preserved schema metadata.
    and encodeInputArraySchema (schema:InputArraySchema) : YAMLElement =
        [ "type", Encode.string "array"; "items", encodeCWLType schema.Items ]
        |> appendOpt "label" Encode.string schema.Label
        |> appendOpt "doc" Encode.string schema.Doc
        |> appendOpt "name" Encode.string schema.Name
        |> appendDynamicPropertiesExcept InputArraySchema.KnownFieldNames schema
        |> yMap

    // ------------------------------
    // Binding & Port encoders
    // ------------------------------

    /// Encode outputBinding fields and preserved extension properties.
    let encodeOutputBinding (ob:OutputBinding) : YAMLElement =
        [ ob.Glob |> Option.map (fun g -> "glob", Encode.string g) ]
        |> List.choose id
        |> appendOpt "loadContents" yBool ob.LoadContents
        |> appendOpt "loadListing" (LoadListingEnum.toCwlString >> Encode.string) ob.LoadListing
        |> appendOpt "outputEval" Encode.string ob.OutputEval
        |> appendDynamicPropertiesExcept OutputBinding.KnownFieldNames ob
        |> yMap

    /// Encode one source value as a scalar and multiple source values as a sequence.
    let encodeStringArrayOrScalar (values: ResizeArray<string>) : YAMLElement =
        if values.Count = 1 then
            Encode.string values.[0]
        else
            values
            |> Seq.map Encode.string
            |> List.ofSeq
            |> YAMLElement.Sequence

    /// Encode a workflow or tool output, choosing shorthand only when type is the sole field.
    let encodeCWLOutput (o:CWLOutput) : (string * YAMLElement) =
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
                    yMap [ "type", encodeInputArraySchema arraySchema ]
            | Record recordSchema ->
                // Record needs full schema form wrapped in "type"
                yMap [ "type", encodeInputRecordSchema recordSchema ]
            | Enum enumSchema ->
                // Enum needs full schema form wrapped in "type"
                yMap [ "type", encodeInputEnumSchema enumSchema ]
            | _ ->
                // Simple types
                encodeCWLType t
        )

        let outputSourceElement =
            // Keep scalar outputSource compact, but use a sequence when multiple
            // upstream step outputs feed this workflow output.
            match o.OutputSource with
            | Some (OutputSource.Single value) -> Some (Encode.string value)
            | Some (OutputSource.Multiple values) when values.Count > 0 -> Some (encodeStringArrayOrScalar values)
            | _ -> None
        
        let pairs =
            // Build object-form fields in CWL's conventional order.
            []
            |> appendOpt "type" id typeElement
            |> appendOpt "label" Encode.string o.Label
            |> appendOpt "secondaryFiles" id o.SecondaryFiles
            |> appendOpt "streamable" yBool o.Streamable
            |> appendOpt "doc" Encode.string o.Doc
            |> appendOpt "format" Encode.string o.Format
            |> appendOpt "outputBinding" encodeOutputBinding o.OutputBinding
            |> appendOpt "outputSource" id outputSourceElement
            |> appendDynamicPropertiesExcept CWLOutput.KnownFieldNames o
        match pairs with
        | [ ("type", t) ] ->
            // Map shorthand: outputName: File
            o.Name, t
        | _ ->
            // Always extended form when additional fields like outputSource/outputBinding present
            o.Name, (yMap pairs)

    /// Encode inputBinding fields and preserved extension properties.
    let encodeInputBinding (ib:InputBinding) : YAMLElement =
        []
        |> appendOpt "loadContents" yBool ib.LoadContents
        |> appendOpt "prefix" Encode.string ib.Prefix
        |> appendOpt "position" (fun p -> Encode.int p) ib.Position
        |> appendOpt "itemSeparator" Encode.string ib.ItemSeparator
        |> appendOpt "separate" yBool ib.Separate
        |> appendOpt "valueFrom" Encode.string ib.ValueFrom
        |> appendOpt "shellQuote" yBool ib.ShellQuote
        |> appendDynamicPropertiesExcept InputBinding.KnownFieldNames ib
        |> yMap

    /// Encode a workflow or tool input, choosing shorthand only when type is the sole field.
    let encodeCWLInput (i:CWLInput) : (string * YAMLElement) =
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
                    yMap [ "type", encodeInputArraySchema arraySchema ]
            | Record recordSchema ->
                // Record needs full schema form wrapped in "type"
                yMap [ "type", encodeInputRecordSchema recordSchema ]
            | Enum enumSchema ->
                // Enum needs full schema form wrapped in "type"
                yMap [ "type", encodeInputEnumSchema enumSchema ]
            | _ ->
                // Simple types
                encodeCWLType t
        )
        
        let pairs =
            // Build object-form fields in CWL's conventional order; optional is
            // encoded through the type union shorthand rather than as a separate field.
            []
            |> appendOpt "type" id typeElement
            |> appendOpt "label" Encode.string i.Label
            |> appendOpt "secondaryFiles" id i.SecondaryFiles
            |> appendOpt "streamable" yBool i.Streamable
            |> appendOpt "doc" Encode.string i.Doc
            |> appendOpt "format" Encode.string i.Format
            |> appendOpt "loadContents" yBool i.LoadContents
            |> appendOpt "loadListing" (LoadListingEnum.toCwlString >> Encode.string) i.LoadListing
            |> appendOpt "default" id i.DefaultValue
            |> appendOpt "inputBinding" encodeInputBinding i.InputBinding
            |> appendDynamicPropertiesExcept CWLInput.KnownFieldNames i
        match pairs with
        | [ ("type", t) ] ->
            // Map shorthand: inputName: string
            i.Name, t
        | _ ->
            // Extended form is needed whenever metadata/default/binding/extensions exist.
            i.Name, yMap pairs

    // ------------------------------
    // Requirement encoder (always extended style)
    // ------------------------------

    /// Encode one SchemaDefRequirement type entry and preserved schema extension fields.
    let encodeSchemaDefRequirementType (s:SchemaDefRequirementType) : YAMLElement =
        [
            "name", Encode.string s.Name
            "type", encodeCWLType s.Type_
        ]
        |> appendDynamicPropertiesExcept SchemaDefRequirementType.KnownFieldNames s
        |> yMap

    /// Encode known CWL requirements in extended form while preserving requirement extensions.
    let encodeRequirement (r:Requirement) : YAMLElement =
        match r with
        | InlineJavascriptRequirement value ->
            let expressionLib =
                // Empty expressionLib is omitted because the requirement class alone
                // is meaningful and matches CWL's compact style.
                value.ExpressionLib
                |> Option.bind (fun entries -> if entries.Count > 0 then Some entries else None)
            [ "class", Encode.string "InlineJavascriptRequirement" ]
            |> appendOpt "expressionLib" (fun entries -> entries |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) expressionLib
            |> appendDynamicPropertiesExcept InlineJavascriptRequirementValue.KnownFieldNames value
            |> yMap
        | SchemaDefRequirement types ->
            // SchemaDefRequirement contains schema entries that already preserve their
            // own extension fields.
            [ "class", Encode.string "SchemaDefRequirement";
              "types", (types |> Seq.map encodeSchemaDefRequirementType |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | DockerRequirement dr ->
            // Docker fields are optional and emitted only when present.
            [ "class", Encode.string "DockerRequirement" ]
            |> appendOpt "dockerPull" Encode.string dr.DockerPull
            |> appendOpt "dockerFile" encodeSchemaSaladString dr.DockerFile
            |> appendOpt "dockerImageId" Encode.string dr.DockerImageId
            |> appendOpt "dockerLoad" Encode.string dr.DockerLoad
            |> appendOpt "dockerImport" Encode.string dr.DockerImport
            |> appendOpt "dockerOutputDirectory" Encode.string dr.DockerOutputDirectory
            |> appendOpt "cwltool:dockerRunOptions" (fun values -> values |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) dr.DockerRunOptions
            |> appendDynamicPropertiesExcept DockerRequirement.KnownFieldNames dr
            |> yMap
        | SoftwareRequirement pkgs ->
            let encodePkg (p:SoftwarePackage) =
                // Extended package form is used here so version/specs/extensions all
                // have explicit keys.
                []
                |> fun acc -> acc @ [ "package", Encode.string p.Package ]
                |> appendOpt "version" (fun vs -> vs |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) p.Version
                |> appendOpt "specs" (fun vs -> vs |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) p.Specs
                |> appendDynamicPropertiesExcept SoftwarePackage.KnownFieldNames p
                |> yMap
            [ "class", Encode.string "SoftwareRequirement";
              "packages", (pkgs |> Seq.map encodePkg |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | LoadListingRequirement loadListing ->
            // Always emit the concrete loadListing value instead of relying on decoder defaults.
            [ "class", Encode.string "LoadListingRequirement"
              "loadListing", Encode.string (LoadListingEnum.toCwlString loadListing.LoadListing) ]
            |> appendDynamicPropertiesExcept LoadListingRequirementValue.KnownFieldNames loadListing
            |> yMap
        | InitialWorkDirRequirement listing ->
            let encodeInitialWorkDirEntry = function
                | DirentEntry d ->
                    // Dirent entries use their own object form.
                    [ ]
                    |> appendOpt "entryname" encodeSchemaSaladString d.Entryname
                    |> fun acc -> acc @ [ "entry", encodeSchemaSaladString d.Entry ]
                    |> appendOpt "writable" yBool d.Writable
                    |> appendDynamicPropertiesExcept DirentInstance.KnownFieldNames d
                    |> yMap
                | StringEntry s ->
                    // String/expression listing entries stay scalar/directive form.
                    encodeSchemaSaladString s
                | FileEntry file ->
                    // File/Directory listing entries use class, not type, as discriminator.
                    encodeFilePairs "class" "File" file |> yMap
                | DirectoryEntry directory ->
                    encodeDirectoryPairs "class" "Directory" directory |> yMap

            [ "class", Encode.string "InitialWorkDirRequirement";
              "listing", (listing |> Seq.map encodeInitialWorkDirEntry |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | EnvVarRequirement envs ->
            let encodeEnv (e:EnvironmentDef) =
                // Env var values remain strings; quote boolean-looking values.
                let v = normalizeEnvValueForEncode e.EnvValue
                [ "envName", Encode.string e.EnvName; "envValue", Encode.string v ]
                |> appendDynamicPropertiesExcept EnvironmentDef.KnownFieldNames e
                |> yMap
            [ "class", Encode.string "EnvVarRequirement";
              "envDef", (envs |> Seq.map encodeEnv |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | ShellCommandRequirement -> [ "class", Encode.string "ShellCommandRequirement" ] |> yMap
        | ResourceRequirement rr ->
            let tryEncodeScalar (key: string) (value: obj) =
                // Resource fields can be numeric, boolean, or expression strings.
                // Only those scalar shapes are emitted from typed or dynamic fields.
                match value with
                // Fable JavaScript/TypeScript use `number` for int and float; preserve decimal resource values.
                | :? float as f -> Some (key, yFloat f)
                | :? int as i -> Some (key, Encode.int i)
                | :? int64 as i -> Some (key, YAMLElement.Value (YAMLContent.create (string i)))
                | :? string as s -> Some (key, Encode.string s)
                | :? bool as b -> Some (key, yBool b)
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
            [ "class", Encode.string "ResourceRequirement" ] @ knownPairs @ dynamicPairs |> yMap
        // Canonicalize class names to short CWL forms where applicable.
        | WorkReuseRequirement workReuse ->
            [ "class", Encode.string "WorkReuse"
              "enableReuse", yBool workReuse.EnableReuse ]
            |> appendDynamicPropertiesExcept WorkReuseRequirementValue.KnownFieldNames workReuse
            |> yMap
        | WorkReuseExpressionRequirement expression ->
            [ "class", Encode.string "WorkReuse"
              "enableReuse", Encode.string expression ]
            |> yMap
        | NetworkAccessRequirement networkAccess ->
            [ "class", Encode.string "NetworkAccess"
              "networkAccess", yBool networkAccess.NetworkAccess ]
            |> appendDynamicPropertiesExcept NetworkAccessRequirementValue.KnownFieldNames networkAccess
            |> yMap
        | NetworkAccessExpressionRequirement expression ->
            [ "class", Encode.string "NetworkAccess"
              "networkAccess", Encode.string expression ]
            |> yMap
        | InplaceUpdateRequirement inplaceUpdate ->
            [ "class", Encode.string "InplaceUpdateRequirement"
              "inplaceUpdate", yBool inplaceUpdate.InplaceUpdate ]
            |> appendDynamicPropertiesExcept InplaceUpdateRequirementValue.KnownFieldNames inplaceUpdate
            |> yMap
        | ToolTimeLimitRequirement tl ->
            let timelimit =
                match tl with
                | ToolTimeLimitSeconds seconds -> YAMLElement.Value (YAMLContent.create (string seconds))
                | ToolTimeLimitExpression expression -> Encode.string expression
            [ "class", Encode.string "ToolTimeLimit"; "timelimit", timelimit ] |> yMap
        | SubworkflowFeatureRequirement -> [ "class", Encode.string "SubworkflowFeatureRequirement" ] |> yMap
        | ScatterFeatureRequirement -> [ "class", Encode.string "ScatterFeatureRequirement" ] |> yMap
        | MultipleInputFeatureRequirement -> [ "class", Encode.string "MultipleInputFeatureRequirement" ] |> yMap
        | StepInputExpressionRequirement -> [ "class", Encode.string "StepInputExpressionRequirement" ] |> yMap

    /// Encode known hints as requirements and unknown hints as their original raw YAML.
    let encodeHintEntry (hint: HintEntry) : YAMLElement =
        match hint with
        | KnownHint requirement -> encodeRequirement requirement
        | UnknownHint unknownHint -> unknownHint.Raw

    // ------------------------------
    // Workflow step encoders
    // ------------------------------
    
    /// Encode a ResizeArray<string> as either a single string or a sequence
    let encodeSourceArray (sources:ResizeArray<string>) : YAMLElement =
        match sources.Count with
        | 1 ->
            // CWL source shorthand: source: input_id
            Encode.string sources.[0]
        | _ -> 
            // Wrap scalar items to keep nested `source` arrays in block-sequence form.
            sources 
            |> Seq.map (fun s -> YAMLElement.Object [YAMLElement.Value (YAMLContent.create s)])
            |> List.ofSeq
            |> YAMLElement.Sequence

    let encodeLinkMergeMethod (linkMerge: LinkMergeMethod) : YAMLElement =
        Encode.string linkMerge.AsCwlString

    let encodePickValueMethod (pickValue: PickValueMethod) : YAMLElement =
        Encode.string pickValue.AsCwlString

    let encodeScatterMethod (scatterMethod: ScatterMethod) : YAMLElement =
        Encode.string scatterMethod.AsCwlString

    /// Encode a step input using scalar source shorthand only when no other fields exist.
    let encodeStepInput (si:StepInput) : (string * YAMLElement) =
        let pairs =
            []
            |> appendOpt "source" encodeSourceArray si.Source
            |> appendOpt "default" id si.DefaultValue
            |> appendOpt "valueFrom" Encode.string si.ValueFrom
            |> appendOpt "linkMerge" encodeLinkMergeMethod si.LinkMerge
            |> appendOpt "pickValue" encodePickValueMethod si.PickValue
            |> appendOpt "doc" Encode.string si.Doc
            |> appendOpt "loadContents" yBool si.LoadContents
            |> appendOpt "loadListing" Encode.string si.LoadListing
            |> appendOpt "label" Encode.string si.Label
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
        | _ -> si.Id, yMap pairs

    /// Encode all step inputs in map form keyed by step input id.
    let encodeStepInputs (inputs:ResizeArray<StepInput>) : YAMLElement =
        inputs
        |> Seq.map encodeStepInput
        |> Seq.toList
        |> yMap

    /// Encode record-form step output parameters, including extension fields.
    let encodeStepOutputParameter (so: StepOutputParameter) : YAMLElement =
        [ "id", Encode.string so.Id ]
        |> appendDynamicPropertiesExcept StepOutputParameter.KnownFieldNames so
        |> yMap

    /// Encode step outputs as either scalar ids or record-form output parameters.
    let encodeStepOutputs (outputs: ResizeArray<StepOutput>) : YAMLElement =
        outputs
        |> Seq.map (fun output ->
            match output with
            | StepOutputString id -> Encode.string id
            | StepOutputRecord outputParameter -> encodeStepOutputParameter outputParameter
        )
        |> List.ofSeq
        |> YAMLElement.Sequence

    /// Encode scatter as scalar shorthand for one item or a sequence for multiple items.
    let encodeScatter (scatter: ResizeArray<string>) : YAMLElement =
        match scatter.Count with
        | 1 -> Encode.string scatter.[0]
        | _ -> scatter |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence

    /// Encode workflow step run targets as paths or inline processing units.
    let rec encodeWorkflowStepRun (run: WorkflowStepRun) : YAMLElement =
        match run with
        | RunString runPath ->
            // External run reference.
            Encode.string runPath
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
    and encodeToolDescriptionElement (td: CWLToolDescription) : YAMLElement =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            [ "cwlVersion", Encode.string td.CWLVersion
              "class", Encode.string "CommandLineTool" ]
            |> appendOptPair (td.Id |> Option.map (fun id -> "id", Encode.string id))
            |> appendOptPair (td.Label |> Option.map encodeLabel)
            |> appendOptPair (td.Doc |> Option.map encodeDoc)
            |> appendOptPair (td.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map encodeIntent)
        let withHints =
            match td.Hints with
            | Some h when h.Count > 0 ->
                // Hints are emitted before requirements to match existing fixtures.
                basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> basePairs
        let withRequirements =
            match td.Requirements with
            | Some r when r.Count > 0 ->
                // Requirements are omitted when absent or empty.
                withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> withHints
        let withBaseCommand =
            match td.BaseCommand with
            | Some bc when bc.Count > 0 ->
                // baseCommand is always emitted as a sequence here.
                withRequirements @ [ "baseCommand", (bc |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> withRequirements
        let withCommandFields =
            // Command-line execution fields belong near baseCommand before ports.
            withBaseCommand
            |> appendOpt "arguments" id td.Arguments
            |> appendOpt "stdin" Encode.string td.Stdin
            |> appendOpt "stderr" Encode.string td.Stderr
            |> appendOpt "stdout" Encode.string td.Stdout
            |> appendOpt "successCodes" (fun codes -> codes |> Seq.map Encode.int |> List.ofSeq |> YAMLElement.Sequence) td.SuccessCodes
            |> appendOpt "temporaryFailCodes" (fun codes -> codes |> Seq.map Encode.int |> List.ofSeq |> YAMLElement.Sequence) td.TemporaryFailCodes
            |> appendOpt "permanentFailCodes" (fun codes -> codes |> Seq.map Encode.int |> List.ofSeq |> YAMLElement.Sequence) td.PermanentFailCodes
        let withInputs =
            match td.Inputs with
            | Some i when i.Count > 0 ->
                // Inputs are optional for CommandLineTool.
                withCommandFields @ [ "inputs", (i |> Seq.map encodeCWLInput |> Seq.toList |> yMap) ]
            | _ -> withCommandFields
        let withOutputs =
            // Outputs are required by the model and always emitted.
            withInputs @ [ "outputs", (td.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> yMap) ]
        let withMetadata =
            match td.Metadata with
            | Some md ->
                // Metadata DynamicObj is serialized as top-level extension fields.
                appendDynamicProperties md withOutputs
            | None -> withOutputs
        withMetadata
        // Also include dynamic fields attached directly to the tool object.
        |> appendDynamicPropertiesExcept CWLToolDescription.KnownFieldNames td
        |> yMap

    /// Encode an ExpressionTool element with ordered sections and preserved metadata.
    and encodeExpressionToolDescriptionElement (et: CWLExpressionToolDescription) : YAMLElement =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            [ "cwlVersion", Encode.string et.CWLVersion
              "class", Encode.string "ExpressionTool" ]
            |> appendOptPair (et.Id |> Option.map (fun id -> "id", Encode.string id))
            |> appendOptPair (et.Label |> Option.map encodeLabel)
            |> appendOptPair (et.Doc |> Option.map encodeDoc)
            |> appendOptPair (et.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map encodeIntent)
        let withHints =
            match et.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> basePairs
        let withRequirements =
            match et.Requirements with
            | Some r when r.Count > 0 ->
                withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> withHints
        let withInputs =
            match et.Inputs with
            | Some i when i.Count > 0 ->
                withRequirements @ [ "inputs", (i |> Seq.map encodeCWLInput |> Seq.toList |> yMap) ]
            | _ -> withRequirements
        let withOutputs =
            withInputs @ [ "outputs", (et.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> yMap) ]
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
        |> yMap

    /// Encode an Operation element with ordered sections and preserved metadata.
    and encodeOperationDescriptionElement (op: CWLOperationDescription) : YAMLElement =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            [ "cwlVersion", Encode.string op.CWLVersion
              "class", Encode.string "Operation" ]
            |> appendOptPair (op.Id |> Option.map (fun id -> "id", Encode.string id))
            |> appendOptPair (op.Label |> Option.map encodeLabel)
            |> appendOptPair (op.Doc |> Option.map encodeDoc)
            |> appendOptPair (op.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map encodeIntent)
        let withHints =
            match op.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> basePairs
        let withRequirements =
            match op.Requirements with
            | Some r when r.Count > 0 ->
                withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> withHints
        let withInputs =
            // Operation inputs are required by the model.
            withRequirements @ [ "inputs", (op.Inputs |> Seq.map encodeCWLInput |> Seq.toList |> yMap) ]
        let withOutputs =
            // Operation outputs are required by the model.
            withInputs @ [ "outputs", (op.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> yMap) ]
        let withMetadata =
            match op.Metadata with
            | Some md ->
                // Metadata DynamicObj is serialized as top-level extension fields.
                appendDynamicProperties md withOutputs
            | None -> withOutputs
        withMetadata
        // Include dynamic fields attached directly to the operation object.
        |> appendDynamicPropertiesExcept CWLOperationDescription.KnownFieldNames op
        |> yMap

    /// Encode a Workflow element with ordered sections and preserved metadata.
    and encodeWorkflowDescriptionElement (wd: CWLWorkflowDescription) : YAMLElement =
        let basePairs =
            // Base identity fields stay in the first top-level section.
            [ "cwlVersion", Encode.string wd.CWLVersion
              "class", Encode.string "Workflow" ]
            |> appendOptPair (wd.Id |> Option.map (fun id -> "id", Encode.string id))
            |> appendOptPair (wd.Label |> Option.map encodeLabel)
            |> appendOptPair (wd.Doc |> Option.map encodeDoc)
            |> appendOptPair (wd.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map encodeIntent)
        let withHints =
            match wd.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> basePairs
        let withRequirements =
            match wd.Requirements with
            | Some r when r.Count > 0 ->
                withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> withHints
        let withInputs =
            // Workflow inputs are required and emitted before steps.
            withRequirements @ [ "inputs", (wd.Inputs |> Seq.map encodeCWLInput |> Seq.toList |> yMap) ]
        let withSteps =
            // Steps are map entries keyed by step id.
            withInputs @ [ "steps", (wd.Steps |> Seq.map encodeWorkflowStep |> Seq.toList |> yMap) ]
        let withOutputs =
            // Outputs follow steps for scan-friendly workflow YAML.
            withSteps @ [ "outputs", (wd.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> yMap) ]
        let withMetadata =
            match wd.Metadata with
            | Some md ->
                // Metadata DynamicObj is serialized as top-level extension fields.
                appendDynamicProperties md withOutputs
            | None -> withOutputs
        withMetadata
        // Include dynamic fields attached directly to the workflow object.
        |> appendDynamicPropertiesExcept CWLWorkflowDescription.KnownFieldNames wd
        |> yMap

    /// Encode one workflow step as a map entry keyed by step id.
    and encodeWorkflowStep (ws:WorkflowStep) : (string * YAMLElement) =
        let basePairs =
            // Required step fields are emitted first.
            [ "run", encodeWorkflowStepRun ws.Run
              "in", encodeStepInputs ws.In
              "out", encodeStepOutputs ws.Out ]
            |> appendOpt "label" Encode.string ws.Label
            |> appendOpt "doc" Encode.string ws.Doc
            |> appendOpt "scatter" encodeScatter ws.Scatter
            |> appendOpt "scatterMethod" encodeScatterMethod ws.ScatterMethod
            |> appendOpt "when" Encode.string ws.When_
            |> appendDynamicPropertiesExcept WorkflowStep.KnownFieldNames ws
        let withHints =
            match ws.Hints with
            // Step-local hints are included only when non-empty.
            | Some h when h.Count > 0 -> basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> basePairs
        let withReq =
            match ws.Requirements with
            // Step-local requirements follow hints.
            | Some r when r.Count > 0 -> withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> withHints
        ws.Id, yMap withReq

    // ------------------------------
    // Top-level encoders
    // ------------------------------

    let writeYaml (element:YAMLElement) =
        // Use whitespace=2 to match fixtures (assumed)
        YAMLicious.Writer.write element (Some (fun c -> { c with Whitespace = 2 }))

    /// Extract object mappings in order, dropping non-mapping presentation nodes.
    let getObjectPairs (element: YAMLElement) : (string * YAMLElement) list =
        match element with
        | YAMLElement.Object mappings ->
            mappings
            |> List.choose (function
                | YAMLElement.Mapping (k, v) -> Some (k.Value, v)
                | _ -> None
            )
        | _ -> []

    /// Render top-level CWL sections in a stable order while keeping metadata as a final block.
    let renderTopLevelElement (baseKeys: string list) (orderedSectionKeys: string list) (element: YAMLElement) : string =
        let section (pairs:(string*YAMLElement) list) =
            // Render each logical section independently so blank lines can separate
            // high-level CWL sections.
            pairs
            |> yMap
            |> writeYaml
            |> fun s -> s.Replace("\r\n","\n").TrimEnd('\n')

        let pairs = getObjectPairs element
        let basePairs =
            // Identity and description keys stay together at the top.
            pairs
            |> List.filter (fun (k, _) -> List.contains k baseKeys)

        let knownSections =
            // Ordered sections are emitted one key at a time to keep large CWL blocks
            // visually separated.
            orderedSectionKeys
            |> List.choose (fun sectionKey ->
                pairs
                |> List.tryFind (fun (k, _) -> k = sectionKey)
                |> Option.map List.singleton
            )

        let reservedKeys = Set.ofList (baseKeys @ orderedSectionKeys)
        let metadataPairs =
            // Any unreserved key is extension metadata and is placed after typed sections.
            pairs
            |> List.filter (fun (k, _) -> reservedKeys.Contains k |> not)

        let sections =
            // Drop empty sections, preserving the requested order for the rest.
            [
                if basePairs.Length > 0 then
                    section basePairs
                yield! knownSections |> List.map section
                if metadataPairs.Length > 0 then
                    section metadataPairs
            ]

        let output = sections |> String.concat "\r\n\r\n"
        let lines = output.Split([|"\r\n"|], StringSplitOptions.None) |> Array.toList
        let rec merge (acc:string list) (remaining:string list) =
            match remaining with
            | a::b::rest when a.Trim() = "-" && b.TrimStart().Contains(":") ->
                // YAMLicious can render a sequence item marker on its own line before
                // a mapping; merge it to the conventional `- key: value` form.
                let merged = a + " " + b.Trim()
                merge (merged::acc) rest
            | l::rest -> merge (l::acc) rest
            | [] -> List.rev acc
        merge [] lines |> String.concat "\r\n"

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

    /// Encode a CWLType to a single-line YAML string using flow/inline style
    /// This produces YAML that doesn't contain newlines and can be embedded in JSON
    let rec encodeCWLTypeYaml (t: CWLType) : string =
        match t with
        | Union types ->
            // Union - use YAML flow array notation [type1, type2]
            let encodedTypes = 
                types 
                |> Seq.map encodeCWLTypeYaml
                |> String.concat ", "
            "[" + encodedTypes + "]"
        | Array arraySchema ->
            // Inline schema rendering is used for compact serialization contexts.
            encodeInputArraySchemaYaml arraySchema
        | Record recordSchema -> encodeInputRecordSchemaYaml recordSchema
        | Enum enumSchema -> encodeInputEnumSchemaYaml enumSchema
        | Null ->
            // Null needs to be quoted in YAML to distinguish from null value
            "\"null\""
        | _ -> 
            // Simple type - just the type name
            let yamlForm = encodeCWLType t |> writeYaml
            yamlForm.Trim()

    and encodeInputRecordFieldYaml (field: InputRecordField) : string =
        // Flow-style helper used only for single-line type serialization.
        let typeYaml = encodeCWLTypeYaml field.Type
        $"{{name: {field.Name}, type: {typeYaml}}}"

    and encodeInputRecordSchemaYaml (schema: InputRecordSchema) : string =
        let fieldsYaml =
            match schema.Fields with
            | Some fs when fs.Count > 0 -> 
                // Preserve field order from the schema's ResizeArray.
                fs 
                |> Seq.map encodeInputRecordFieldYaml
                |> String.concat ", "
            | _ -> ""
        
        if fieldsYaml = "" then
            // Empty records still need an explicit fields array in flow form.
            "{type: record, fields: []}"
        else
            $"{{type: record, fields: [{fieldsYaml}]}}"

    and encodeInputEnumSchemaYaml (schema: InputEnumSchema) : string =
        // Symbols are emitted in stored order.
        let symbolsYaml = 
            schema.Symbols 
            |> String.concat ", "
        $"{{type: enum, symbols: [{symbolsYaml}]}}"

    and encodeInputArraySchemaYaml (schema: InputArraySchema) : string =
        // Array flow form delegates recursively for nested or complex item types.
        let itemsYaml = encodeCWLTypeYaml schema.Items
        $"{{type: array, items: {itemsYaml}}}"

    /// Convert a CWLType to a YAML-formatted string for use in serialization
    let cwlTypeToYamlString (t: CWLType) : string =
        encodeCWLTypeYaml t

