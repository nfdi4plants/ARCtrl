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
        YAMLElement.Value { Value = (if b then "true" else "false"); Comment = None }

    // ------------------------------
    // Helper to build YAML mappings preserving order
    // ------------------------------
    let yMap (pairs: (string * YAMLElement) list) =
        // Represent a mapping as an Object containing Mapping nodes preserving order.
        // Avoid wrapping scalar/sequence values inside an extra Object layer so YAMLicious prints 'key: value'.
        let normalize = function
            | YAMLElement.Object [single] -> single // unwrap single wrapped value (legacy helper usage)
            | other -> other
        pairs
        |> List.map (fun (k,v) -> YAMLElement.Mapping ({ Value = k; Comment = None }, normalize v))
        |> YAMLElement.Object

    // ------------------------------
    // Helper append functions for ordered construction
    // ------------------------------
    let inline private appendOpt name (encoder:'a -> YAMLElement) (value:'a option) acc =
        match value with
        | Some v -> acc @ [name, encoder v]
        | None -> acc

    let normalizeDocString (doc:string) =
        doc.Replace("\r\n","\n").TrimEnd('\n').TrimEnd('\r')

    let encodeSchemaSaladString (value: SchemaSaladString) : YAMLElement =
        match value with
        | Literal text -> Encode.string text
        | Include path -> yMap [ "$include", Encode.string path ]
        | Import path -> yMap [ "$import", Encode.string path ]

    let encodeLabel (label:string) : (string * YAMLElement) =
        "label", Encode.string label

    let encodeDoc (doc:string) : (string * YAMLElement) =
        "doc", Encode.string (normalizeDocString doc)

    let inline private appendOptPair pairOpt acc =
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
        
        let pairs =
            []
            |> appendOpt "type" id typeElement
            |> appendOpt "outputBinding" encodeOutputBinding o.OutputBinding
            |> appendOpt "outputSource" Encode.string o.OutputSource
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
        | InlineJavascriptRequirement -> [ "class", Encode.string "InlineJavascriptRequirement" ] |> yMap
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
              "loadListing", Encode.string loadListing.LoadListing ]
            |> yMap
        | InitialWorkDirRequirement listing ->
            let encodeInitialWorkDirEntry = function
                | DirentEntry d ->
                    [ ]
                    |> appendOpt "entryname" encodeSchemaSaladString d.Entryname
                    |> fun acc -> acc @ [ "entry", encodeSchemaSaladString d.Entry ]
                    |> appendOpt "writable" yBool d.Writable
                    |> yMap
                | StringEntry s ->
                    encodeSchemaSaladString s

            [ "class", Encode.string "InitialWorkDirRequirement";
              "listing", (listing |> Seq.map encodeInitialWorkDirEntry |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | EnvVarRequirement envs ->
            let encodeEnv (e:EnvironmentDef) =
                let v = if e.EnvValue = "true" || e.EnvValue = "false" then "\"" + e.EnvValue + "\"" else e.EnvValue
                [ "envName", Encode.string e.EnvName; "envValue", Encode.string v ] |> yMap
            [ "class", Encode.string "EnvVarRequirement";
              "envDef", (envs |> Seq.map encodeEnv |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | ShellCommandRequirement -> [ "class", Encode.string "ShellCommandRequirement" ] |> yMap
        | ResourceRequirement rr ->
            let dynamicPairs =
                rr.GetProperties(false)
                |> Seq.choose (fun kvp ->
                    match kvp.Value with
                    | :? int as i -> Some (kvp.Key, Encode.int i)
                    | :? float as f -> Some (kvp.Key, Encode.float f)
                    | :? string as s -> Some (kvp.Key, Encode.string s)
                    | :? bool as b -> Some (kvp.Key, yBool b)
                    | _ -> None)
                |> Seq.toList
            [ "class", Encode.string "ResourceRequirement" ] @ dynamicPairs |> yMap
        // Canonicalize class names to short CWL forms where applicable.
        | WorkReuseRequirement workReuse ->
            [ "class", Encode.string "WorkReuse"
              "enableReuse", yBool workReuse.EnableReuse ]
            |> yMap
        | NetworkAccessRequirement networkAccess ->
            [ "class", Encode.string "NetworkAccess"
              "networkAccess", yBool networkAccess.NetworkAccess ]
            |> yMap
        | InplaceUpdateRequirement inplaceUpdate ->
            [ "class", Encode.string "InplaceUpdateRequirement"
              "inplaceUpdate", yBool inplaceUpdate.InplaceUpdate ]
            |> yMap
        | ToolTimeLimitRequirement tl ->
            let timelimit =
                match tl with
                | ToolTimeLimitSeconds seconds -> Encode.float seconds
                | ToolTimeLimitExpression expression -> Encode.string expression
            [ "class", Encode.string "ToolTimeLimit"; "timelimit", timelimit ] |> yMap
        | SubworkflowFeatureRequirement -> [ "class", Encode.string "SubworkflowFeatureRequirement" ] |> yMap
        | ScatterFeatureRequirement -> [ "class", Encode.string "ScatterFeatureRequirement" ] |> yMap
        | MultipleInputFeatureRequirement -> [ "class", Encode.string "MultipleInputFeatureRequirement" ] |> yMap
        | StepInputExpressionRequirement -> [ "class", Encode.string "StepInputExpressionRequirement" ] |> yMap

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
            |> Seq.map (fun s -> YAMLElement.Object [YAMLElement.Value { Value = s; Comment = None }])
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

    and encodeToolDescriptionElement (td: CWLToolDescription) : YAMLElement =
        let basePairs =
            [ "cwlVersion", Encode.string td.CWLVersion
              "class", Encode.string "CommandLineTool" ]
            |> appendOptPair (td.Label |> Option.map encodeLabel)
            |> appendOptPair (td.Doc |> Option.map encodeDoc)
        let withHints =
            match td.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (h |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
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
        let withHints =
            match et.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (h |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
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
            withOutputs @ [ "expression", Encode.string et.Expression ]
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

    and encodeWorkflowDescriptionElement (wd: CWLWorkflowDescription) : YAMLElement =
        let basePairs =
            [ "cwlVersion", Encode.string wd.CWLVersion
              "class", Encode.string "Workflow" ]
            |> appendOptPair (wd.Label |> Option.map encodeLabel)
            |> appendOptPair (wd.Doc |> Option.map encodeDoc)
        let withHints =
            match wd.Hints with
            | Some h when h.Count > 0 ->
                basePairs @ [ "hints", (h |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
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
            | Some h when h.Count > 0 -> basePairs @ [ "hints", (h |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
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

    let private getObjectPairs (element: YAMLElement) : (string * YAMLElement) list =
        match element with
        | YAMLElement.Object mappings ->
            mappings
            |> List.choose (function
                | YAMLElement.Mapping (k, v) -> Some (k.Value, v)
                | _ -> None
            )
        | _ -> []

    let private renderTopLevelElement (baseKeys: string list) (orderedSectionKeys: string list) (element: YAMLElement) : string =
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
        |> renderTopLevelElement ["cwlVersion"; "class"; "label"; "doc"] ["hints"; "requirements"; "baseCommand"; "inputs"; "outputs"]

    let encodeWorkflowDescription (wd:CWLWorkflowDescription) : string =
        encodeWorkflowDescriptionElement wd
        |> renderTopLevelElement ["cwlVersion"; "class"; "label"; "doc"] ["hints"; "requirements"; "inputs"; "steps"; "outputs"]

    let encodeExpressionToolDescription (et:CWLExpressionToolDescription) : string =
        encodeExpressionToolDescriptionElement et
        |> renderTopLevelElement ["cwlVersion"; "class"; "label"; "doc"] ["hints"; "requirements"; "inputs"; "outputs"; "expression"]


    let encodeProcessingUnit (pu : CWLProcessingUnit) :string =
        match pu with
        | CommandLineTool td -> encodeToolDescription td
        | Workflow wd -> encodeWorkflowDescription wd
        | ExpressionTool et -> encodeExpressionToolDescription et

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

