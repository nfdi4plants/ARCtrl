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

    // ------------------------------
    // Helper to build YAML mappings preserving order
    // ------------------------------
    let yMap (pairs: (string * YAMLElement) list) =
        // Represent a mapping as an Object containing Mapping nodes preserving order.
        // Avoid wrapping scalar/sequence values inside an extra Object layer so YAMLicious prints 'key: value'.
        let normalize = function
            // YAMLicious emits `key:` (null) for empty object values unless we force inline `{}`.
            | YAMLElement.Object [] -> YAMLElement.Value (YAMLContent.create "{}")
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

    let normalizeDocString (doc:string) =
        doc.Replace("\r\n","\n").TrimEnd('\n').TrimEnd('\r')

    /// Encode expression payloads with style-aware scalars.
    /// Single-line expressions are double-quoted to protect JS token syntax.
    /// Multi-line expressions are emitted as literal block scalars with clip chomping
    /// so trailing newlines and blank lines survive decode/encode roundtrips.
    let encodeExpressionScalar (expression: string) : YAMLElement =
        let normalized =
            if isNull expression then "" else expression.Replace("\r\n", "\n").Replace("\r", "\n")

        let style =
            if normalized.Contains("\n") then
                ScalarStyle.Block(BlockScalarStyle.Literal, ChompingMode.Clip, None)
            else
                ScalarStyle.DoubleQuoted

        YAMLElement.Value (YAMLContent.create(normalized, style = style))

    let encodeSchemaSaladString (value: SchemaSaladString) : YAMLElement =
        match value with
        | SchemaSaladString.Literal text -> Encode.string text
        | SchemaSaladString.Include path -> yMap [ "$include", Encode.string path ]
        | SchemaSaladString.Import path -> yMap [ "$import", Encode.string path ]

    let normalizeEnvValueForEncode (envValue: string) =
        if envValue = "true" || envValue = "false" then "\"" + envValue + "\"" else envValue

    /// Encode EnvVarRequirement using compact map shorthand (envName -> envValue).
    let encodeEnvVarRequirementCompactMap (envs: ResizeArray<EnvironmentDef>) : YAMLElement =
        let envDefMap =
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
            | None, None -> yMap []
            | None, Some specs -> specs |> Seq.map Encode.string |> Seq.toList |> YAMLElement.Sequence
            | _ ->
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
    let rec encodeCWLType (t:CWLType) : YAMLElement =
        match t with
        | File _ -> Encode.string "File"
        | Directory _ -> Encode.string "Directory"
        | Dirent d ->
            [ "entry", encodeSchemaSaladString d.Entry ]
            |> appendOpt "entryname" encodeSchemaSaladString d.Entryname
            |> appendOpt "writable" (fun b -> yBool b) d.Writable
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
            // Check if this is an optional type (union of null and one other type)
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
            // Try to use short form for arrays (handles arbitrary nesting depth recursively)
            match tryGetArrayShorthand arraySchema.Items with
            | Some shorthand -> Encode.string (shorthand + "[]")
            | None -> encodeInputArraySchema arraySchema
        | Record recordSchema -> encodeInputRecordSchema recordSchema
        | Enum enumSchema -> encodeInputEnumSchema enumSchema

    // ------------------------------
    // InputRecordSchema encoders
    // ------------------------------

    and encodeInputRecordField (field:InputRecordField) : (string * YAMLElement) =
        field.Name, yMap [ "type", encodeCWLType field.Type ]

    and encodeInputRecordSchema (schema:InputRecordSchema) : YAMLElement =
        let fieldsElement =
            match schema.Fields with
            | Some fs ->
                let fieldPairs = fs |> Seq.map encodeInputRecordField |> Seq.toList
                yMap fieldPairs
            | None -> yMap []
        
        yMap [ "type", Encode.string "record"; "fields", fieldsElement ]

    and encodeInputEnumSchema (schema:InputEnumSchema) : YAMLElement =
        let pairs =
            [ "type", Encode.string "enum" ]
            @ [ "symbols", (schema.Symbols |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) ]
        
        yMap pairs

    and encodeInputArraySchema (schema:InputArraySchema) : YAMLElement =
        yMap [ "type", Encode.string "array"; "items", encodeCWLType schema.Items ]

    // ------------------------------
    // Binding & Port encoders
    // ------------------------------

    let encodeOutputBinding (ob:OutputBinding) : YAMLElement =
        [ ob.Glob |> Option.map (fun g -> "glob", Encode.string g) ]
        |> List.choose id
        |> yMap

    let encodeStringArrayOrScalar (values: ResizeArray<string>) : YAMLElement =
        if values.Count = 1 then
            Encode.string values.[0]
        else
            values
            |> Seq.map Encode.string
            |> List.ofSeq
            |> YAMLElement.Sequence

    let encodeCWLOutput (o:CWLOutput) : (string * YAMLElement) =
        let typeElement = o.Type_ |> Option.map (fun t ->
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
            match o.OutputSource with
            | Some (OutputSource.Single value) -> Some (Encode.string value)
            | Some (OutputSource.Multiple values) when values.Count > 0 -> Some (encodeStringArrayOrScalar values)
            | _ -> None
        
        let pairs =
            []
            |> appendOpt "type" id typeElement
            |> appendOpt "outputBinding" encodeOutputBinding o.OutputBinding
            |> appendOpt "outputSource" id outputSourceElement
        match pairs with
        | [ ("type", t) ] -> o.Name, t // only type specified
        | _ ->
            // Always extended form when additional fields like outputSource/outputBinding present
            o.Name, (yMap pairs)

    let encodeInputBinding (ib:InputBinding) : YAMLElement =
        []
        |> appendOpt "prefix" Encode.string ib.Prefix
        |> appendOpt "position" (fun p -> Encode.int p) ib.Position
        |> appendOpt "itemSeparator" Encode.string ib.ItemSeparator
        |> appendOpt "separate" yBool ib.Separate
        |> yMap

    let encodeCWLInput (i:CWLInput) : (string * YAMLElement) =
        let typeElement = i.Type_ |> Option.map (fun t ->
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
            []
            |> appendOpt "type" id typeElement
            |> appendOpt "inputBinding" encodeInputBinding i.InputBinding
            |> appendOpt "optional" yBool i.Optional
        match pairs with
        | [ ("type", t) ] -> i.Name, t
        | _ -> i.Name, yMap pairs

    // ------------------------------
    // Requirement encoder (always extended style)
    // ------------------------------

    let encodeSchemaDefRequirementType (s:SchemaDefRequirementType) : YAMLElement =
        yMap [
            "name", Encode.string s.Name
            "type", encodeCWLType s.Type_
        ]

    let encodeRequirement (r:Requirement) : YAMLElement =
        match r with
        | InlineJavascriptRequirement value ->
            let expressionLib =
                value.ExpressionLib
                |> Option.bind (fun entries -> if entries.Count > 0 then Some entries else None)
            [ "class", Encode.string "InlineJavascriptRequirement" ]
            |> appendOpt "expressionLib" (fun entries -> entries |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) expressionLib
            |> yMap
        | SchemaDefRequirement types ->
            [ "class", Encode.string "SchemaDefRequirement";
              "types", (types |> Seq.map encodeSchemaDefRequirementType |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | DockerRequirement dr ->
            [ "class", Encode.string "DockerRequirement" ]
            |> appendOpt "dockerPull" Encode.string dr.DockerPull
            |> appendOpt "dockerFile" encodeSchemaSaladString dr.DockerFile
            |> appendOpt "dockerImageId" Encode.string dr.DockerImageId
            |> appendOpt "dockerLoad" Encode.string dr.DockerLoad
            |> appendOpt "dockerImport" Encode.string dr.DockerImport
            |> appendOpt "dockerOutputDirectory" Encode.string dr.DockerOutputDirectory
            |> yMap
        | SoftwareRequirement pkgs ->
            let encodePkg (p:SoftwarePackage) =
                []
                |> fun acc -> acc @ [ "package", Encode.string p.Package ]
                |> appendOpt "version" (fun vs -> vs |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) p.Version
                |> appendOpt "specs" (fun vs -> vs |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) p.Specs
                |> yMap
            [ "class", Encode.string "SoftwareRequirement";
              "packages", (pkgs |> Seq.map encodePkg |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | LoadListingRequirement loadListing ->
            [ "class", Encode.string "LoadListingRequirement"
              "loadListing", Encode.string (LoadListingEnum.toCwlString loadListing.LoadListing) ]
            |> yMap
        | InitialWorkDirRequirement listing ->
            let encodeDynamicObjWithClass (className: string) (dynObj: DynamicObj) =
                let dynamicPairs =
                    dynObj.GetProperties(false)
                    |> Seq.choose (fun kvp ->
                        match kvp.Value with
                        | :? string as s -> Some (kvp.Key, Encode.string s)
                        | :? bool as b -> Some (kvp.Key, yBool b)
                        | :? int as i -> Some (kvp.Key, Encode.int i)
                        | :? int64 as i -> Some (kvp.Key, Encode.string (string i))
                        | :? float as f -> Some (kvp.Key, Encode.float f)
                        | :? YAMLElement as y -> Some (kvp.Key, y)
                        | _ -> None
                    )
                    |> Seq.toList

                let hasClass = dynamicPairs |> List.exists (fun (k, _) -> k = "class")
                if hasClass then
                    yMap dynamicPairs
                else
                    yMap (("class", Encode.string className) :: dynamicPairs)

            let encodeInitialWorkDirEntry = function
                | DirentEntry d ->
                    [ ]
                    |> appendOpt "entryname" encodeSchemaSaladString d.Entryname
                    |> fun acc -> acc @ [ "entry", encodeSchemaSaladString d.Entry ]
                    |> appendOpt "writable" yBool d.Writable
                    |> yMap
                | StringEntry s ->
                    encodeSchemaSaladString s
                | FileEntry file ->
                    encodeDynamicObjWithClass "File" file
                | DirectoryEntry directory ->
                    encodeDynamicObjWithClass "Directory" directory

            [ "class", Encode.string "InitialWorkDirRequirement";
              "listing", (listing |> Seq.map encodeInitialWorkDirEntry |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | EnvVarRequirement envs ->
            let encodeEnv (e:EnvironmentDef) =
                let v = normalizeEnvValueForEncode e.EnvValue
                [ "envName", Encode.string e.EnvName; "envValue", Encode.string v ] |> yMap
            [ "class", Encode.string "EnvVarRequirement";
              "envDef", (envs |> Seq.map encodeEnv |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | ShellCommandRequirement -> [ "class", Encode.string "ShellCommandRequirement" ] |> yMap
        | ResourceRequirement rr ->
            let tryEncodeScalar (key: string) (value: obj) =
                match value with
                | :? int as i -> Some (key, Encode.int i)
                | :? int64 as i -> Some (key, YAMLElement.Value (YAMLContent.create (string i)))
                | :? float as f -> Some (key, Encode.float f)
                | :? string as s -> Some (key, Encode.string s)
                | :? bool as b -> Some (key, yBool b)
                | _ -> None

            let dynamicPairs =
                rr.GetProperties(false)
                |> Seq.choose (fun kvp ->
                    match kvp.Value with
                    | :? Option<obj> as optionalValue ->
                        optionalValue
                        |> Option.bind (tryEncodeScalar kvp.Key)
                    | directValue ->
                        tryEncodeScalar kvp.Key directValue)
                |> Seq.toList
            [ "class", Encode.string "ResourceRequirement" ] @ dynamicPairs |> yMap
        // Canonicalize class names to short CWL forms where applicable.
        | WorkReuseRequirement workReuse ->
            [ "class", Encode.string "WorkReuse"
              "enableReuse", yBool workReuse.EnableReuse ]
            |> yMap
        | WorkReuseExpressionRequirement expression ->
            [ "class", Encode.string "WorkReuse"
              "enableReuse", Encode.string expression ]
            |> yMap
        | NetworkAccessRequirement networkAccess ->
            [ "class", Encode.string "NetworkAccess"
              "networkAccess", yBool networkAccess.NetworkAccess ]
            |> yMap
        | NetworkAccessExpressionRequirement expression ->
            [ "class", Encode.string "NetworkAccess"
              "networkAccess", Encode.string expression ]
            |> yMap
        | InplaceUpdateRequirement inplaceUpdate ->
            [ "class", Encode.string "InplaceUpdateRequirement"
              "inplaceUpdate", yBool inplaceUpdate.InplaceUpdate ]
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
        | 1 -> Encode.string sources.[0]
        | _ -> 
            // Create sequence with each item as an Object[Value] to force block-style rendering
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

    let encodeStepInputs (inputs:ResizeArray<StepInput>) : YAMLElement =
        inputs
        |> Seq.map encodeStepInput
        |> Seq.toList
        |> yMap

    let encodeStepOutputParameter (so: StepOutputParameter) : YAMLElement =
        yMap [ "id", Encode.string so.Id ]

    let encodeStepOutputs (outputs: ResizeArray<StepOutput>) : YAMLElement =
        outputs
        |> Seq.map (fun output ->
            match output with
            | StepOutputString id -> Encode.string id
            | StepOutputRecord outputParameter -> encodeStepOutputParameter outputParameter
        )
        |> List.ofSeq
        |> YAMLElement.Sequence

    let encodeScatter (scatter: ResizeArray<string>) : YAMLElement =
        match scatter.Count with
        | 1 -> Encode.string scatter.[0]
        | _ -> scatter |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence

    let rec encodeWorkflowStepRun (run: WorkflowStepRun) : YAMLElement =
        match run with
        | RunString runPath -> Encode.string runPath
        | RunCommandLineTool toolObj ->
            match WorkflowStepRunOps.tryGetTool run with
            | Some tool -> encodeToolDescriptionElement tool
            | None ->
                raise (System.ArgumentException($"RunCommandLineTool must contain CWLToolDescription but got %A{toolObj}"))
        | RunWorkflow workflowObj ->
            match WorkflowStepRunOps.tryGetWorkflow run with
            | Some workflow -> encodeWorkflowDescriptionElement workflow
            | None ->
                raise (System.ArgumentException($"RunWorkflow must contain CWLWorkflowDescription but got %A{workflowObj}"))
        | RunExpressionTool expressionToolObj ->
            match WorkflowStepRunOps.tryGetExpressionTool run with
            | Some expressionTool -> encodeExpressionToolDescriptionElement expressionTool
            | None ->
                raise (System.ArgumentException($"RunExpressionTool must contain CWLExpressionToolDescription but got %A{expressionToolObj}"))
        | RunOperation operationObj ->
            match WorkflowStepRunOps.tryGetOperation run with
            | Some operation -> encodeOperationDescriptionElement operation
            | None ->
                raise (System.ArgumentException($"RunOperation must contain CWLOperationDescription but got %A{operationObj}"))

    and encodeToolDescriptionElement (td: CWLToolDescription) : YAMLElement =
        let basePairs =
            [ "cwlVersion", Encode.string td.CWLVersion
              "class", Encode.string "CommandLineTool" ]
            |> appendOptPair (td.Label |> Option.map encodeLabel)
            |> appendOptPair (td.Doc |> Option.map encodeDoc)
            |> appendOptPair (td.Intent |> Option.filter (fun intent -> intent.Count > 0) |> Option.map encodeIntent)
        let withHints =
            match td.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> basePairs
        let withRequirements =
            match td.Requirements with
            | Some r when r.Count > 0 ->
                withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> withHints
        let withBaseCommand =
            match td.BaseCommand with
            | Some bc when bc.Count > 0 ->
                withRequirements @ [ "baseCommand", (bc |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> withRequirements
        let withInputs =
            match td.Inputs with
            | Some i when i.Count > 0 ->
                withBaseCommand @ [ "inputs", (i |> Seq.map encodeCWLInput |> Seq.toList |> yMap) ]
            | _ -> withBaseCommand
        let withOutputs =
            withInputs @ [ "outputs", (td.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> yMap) ]
        let withMetadata =
            match td.Metadata with
            | Some md ->
                md.GetProperties(false)
                |> Seq.fold (fun acc kvp ->
                    let encodedValue =
                        match kvp.Value with
                        | :? string as s -> Encode.string s
                        | :? bool as b -> yBool b
                        | :? int as i -> Encode.int i
                        | :? float as f -> Encode.float f
                        | :? YAMLElement as y -> y
                        | _ -> Encode.string (string kvp.Value)
                    acc @ [ kvp.Key, encodedValue ]
                ) withOutputs
            | None -> withOutputs
        yMap withMetadata

    and encodeExpressionToolDescriptionElement (et: CWLExpressionToolDescription) : YAMLElement =
        let basePairs =
            [ "cwlVersion", Encode.string et.CWLVersion
              "class", Encode.string "ExpressionTool" ]
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
            withOutputs @ [ "expression", encodeExpressionScalar et.Expression ]
        let withMetadata =
            match et.Metadata with
            | Some md ->
                md.GetProperties(false)
                |> Seq.fold (fun acc kvp ->
                    let encodedValue =
                        match kvp.Value with
                        | :? string as s -> Encode.string s
                        | :? bool as b -> yBool b
                        | :? int as i -> Encode.int i
                        | :? float as f -> Encode.float f
                        | :? YAMLElement as y -> y
                        | _ -> Encode.string (string kvp.Value)
                    acc @ [ kvp.Key, encodedValue ]
                ) withExpression
            | None -> withExpression
        yMap withMetadata

    and encodeOperationDescriptionElement (op: CWLOperationDescription) : YAMLElement =
        let basePairs =
            [ "cwlVersion", Encode.string op.CWLVersion
              "class", Encode.string "Operation" ]
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
            withRequirements @ [ "inputs", (op.Inputs |> Seq.map encodeCWLInput |> Seq.toList |> yMap) ]
        let withOutputs =
            withInputs @ [ "outputs", (op.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> yMap) ]
        let withMetadata =
            match op.Metadata with
            | Some md ->
                md.GetProperties(false)
                |> Seq.fold (fun acc kvp ->
                    let encodedValue =
                        match kvp.Value with
                        | :? string as s -> Encode.string s
                        | :? bool as b -> yBool b
                        | :? int as i -> Encode.int i
                        | :? float as f -> Encode.float f
                        | :? YAMLElement as y -> y
                        | _ -> Encode.string (string kvp.Value)
                    acc @ [ kvp.Key, encodedValue ]
                ) withOutputs
            | None -> withOutputs
        yMap withMetadata

    and encodeWorkflowDescriptionElement (wd: CWLWorkflowDescription) : YAMLElement =
        let basePairs =
            [ "cwlVersion", Encode.string wd.CWLVersion
              "class", Encode.string "Workflow" ]
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
        let withInputs = withRequirements @ [ "inputs", (wd.Inputs |> Seq.map encodeCWLInput |> Seq.toList |> yMap) ]
        let withSteps = withInputs @ [ "steps", (wd.Steps |> Seq.map encodeWorkflowStep |> Seq.toList |> yMap) ]
        let withOutputs = withSteps @ [ "outputs", (wd.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> yMap) ]
        let withMetadata =
            match wd.Metadata with
            | Some md ->
                md.GetProperties(false)
                |> Seq.fold (fun acc kvp ->
                    let encodedValue =
                        match kvp.Value with
                        | :? string as s -> Encode.string s
                        | :? bool as b -> yBool b
                        | :? int as i -> Encode.int i
                        | :? float as f -> Encode.float f
                        | :? YAMLElement as y -> y
                        | _ -> Encode.string (string kvp.Value)
                    acc @ [ kvp.Key, encodedValue ]
                ) withOutputs
            | None -> withOutputs
        yMap withMetadata

    and encodeWorkflowStep (ws:WorkflowStep) : (string * YAMLElement) =
        let basePairs =
            [ "run", encodeWorkflowStepRun ws.Run
              "in", encodeStepInputs ws.In
              "out", encodeStepOutputs ws.Out ]
            |> appendOpt "label" Encode.string ws.Label
            |> appendOpt "doc" Encode.string ws.Doc
            |> appendOpt "scatter" encodeScatter ws.Scatter
            |> appendOpt "scatterMethod" encodeScatterMethod ws.ScatterMethod
            |> appendOpt "when" Encode.string ws.When_
        let withHints =
            match ws.Hints with
            | Some h when h.Count > 0 -> basePairs @ [ "hints", (h |> Seq.map encodeHintEntry |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> basePairs
        let withReq =
            match ws.Requirements with
            | Some r when r.Count > 0 -> withHints @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> withHints
        ws.Id, yMap withReq

    // ------------------------------
    // Top-level encoders
    // ------------------------------

    let writeYaml (element:YAMLElement) =
        // Use whitespace=2 to match fixtures (assumed)
        YAMLicious.Writer.write element (Some (fun c -> { c with Whitespace = 2 }))

    let getObjectPairs (element: YAMLElement) : (string * YAMLElement) list =
        match element with
        | YAMLElement.Object mappings ->
            mappings
            |> List.choose (function
                | YAMLElement.Mapping (k, v) -> Some (k.Value, v)
                | _ -> None
            )
        | _ -> []

    let renderTopLevelElement (baseKeys: string list) (orderedSectionKeys: string list) (element: YAMLElement) : string =
        let section (pairs:(string*YAMLElement) list) =
            pairs
            |> yMap
            |> writeYaml
            |> fun s -> s.Replace("\r\n","\n").TrimEnd('\n')

        let pairs = getObjectPairs element
        let basePairs =
            pairs
            |> List.filter (fun (k, _) -> List.contains k baseKeys)

        let knownSections =
            orderedSectionKeys
            |> List.choose (fun sectionKey ->
                pairs
                |> List.tryFind (fun (k, _) -> k = sectionKey)
                |> Option.map List.singleton
            )

        let reservedKeys = Set.ofList (baseKeys @ orderedSectionKeys)
        let metadataPairs =
            pairs
            |> List.filter (fun (k, _) -> reservedKeys.Contains k |> not)

        let sections =
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
                let merged = a + " " + b.Trim()
                merge (merged::acc) rest
            | l::rest -> merge (l::acc) rest
            | [] -> List.rev acc
        merge [] lines |> String.concat "\r\n"

    let encodeToolDescription (td:CWLToolDescription) : string =
        encodeToolDescriptionElement td
        |> renderTopLevelElement ["cwlVersion"; "class"; "label"; "doc"; "intent"] ["hints"; "requirements"; "baseCommand"; "inputs"; "outputs"]

    let encodeWorkflowDescription (wd:CWLWorkflowDescription) : string =
        encodeWorkflowDescriptionElement wd
        |> renderTopLevelElement ["cwlVersion"; "class"; "label"; "doc"; "intent"] ["hints"; "requirements"; "inputs"; "steps"; "outputs"]

    let encodeExpressionToolDescription (et:CWLExpressionToolDescription) : string =
        encodeExpressionToolDescriptionElement et
        |> renderTopLevelElement ["cwlVersion"; "class"; "label"; "doc"; "intent"] ["hints"; "requirements"; "inputs"; "outputs"; "expression"]

    let encodeOperationDescription (op: CWLOperationDescription) : string =
        encodeOperationDescriptionElement op
        |> renderTopLevelElement ["cwlVersion"; "class"; "label"; "doc"; "intent"] ["hints"; "requirements"; "inputs"; "outputs"]

    let encodeProcessingUnit (pu : CWLProcessingUnit) :string =
        match pu with
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
        let typeYaml = encodeCWLTypeYaml field.Type
        $"{{name: {field.Name}, type: {typeYaml}}}"

    and encodeInputRecordSchemaYaml (schema: InputRecordSchema) : string =
        let fieldsYaml =
            match schema.Fields with
            | Some fs when fs.Count > 0 -> 
                fs 
                |> Seq.map encodeInputRecordFieldYaml
                |> String.concat ", "
            | _ -> ""
        
        if fieldsYaml = "" then
            "{type: record, fields: []}"
        else
            $"{{type: record, fields: [{fieldsYaml}]}}"

    and encodeInputEnumSchemaYaml (schema: InputEnumSchema) : string =
        let symbolsYaml = 
            schema.Symbols 
            |> String.concat ", "
        $"{{type: enum, symbols: [{symbolsYaml}]}}"

    and encodeInputArraySchemaYaml (schema: InputArraySchema) : string =
        let itemsYaml = encodeCWLTypeYaml schema.Items
        $"{{type: array, items: {itemsYaml}}}"

    /// Convert a CWLType to a YAML-formatted string for use in serialization
    let cwlTypeToYamlString (t: CWLType) : string =
        encodeCWLTypeYaml t

