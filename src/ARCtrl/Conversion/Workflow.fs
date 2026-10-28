namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
//open ColumnIndex

open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns


type WorkflowConversion =

    static member composeAdditionalType (t : CWL.CWLType) : string =
        // Use JSON format for complex types, YAML shorthand for simple types
        if CWL.Encode.isComplexType t then
            CWL.Encode.cwlTypeToYamlString t
        else
            CWL.Encode.encodeCWLTypeShorthand t
            |> CWL.Encode.writeYaml
            |> fun s -> s.Trim()

    static member decomposeAdditionalType (t : string) : CWL.CWLType =
        // The shorthand of the text nests arrays (File[][]), which a CWL document does not read.
        CWL.Decode.cwlTypeFromAdditionalType t

    static member composeFormalParamInputIdentifiers (prefix : string option) (position : int option) =
        match prefix, position with
        | Some pr, Some po ->               
            ResizeArray [
                LDPropertyValue.createPosition(po)
                LDPropertyValue.createPrefix(pr)
            ]
            |> Some
        | Some pr, None ->               
            ResizeArray [
                LDPropertyValue.createPrefix(pr)
            ]
            |> Some
        | None, Some po ->               
            ResizeArray [
                LDPropertyValue.createPosition(po)
            ]
            |> Some
        | None, None -> None

    static member composeFormalParameterFromInput (inp : CWL.CWLInput, ?workflowName : string, ?runName) =
        
        let additionalType =
            match inp.Type_ with
            | Some t -> WorkflowConversion.composeAdditionalType t
            | None -> failwith "Input must have a type"
        let valueRequired = not inp.Optional
        let id = LDFormalParameter.genID(name = inp.Name, ?workflowName = workflowName, ?runName = runName)
        let identifiers =
            inp.InputBinding
            |> Option.bind CWL.ListEntry.tryValue
            |> Option.bind (fun ib ->
                // The identifiers hold a number only, so a position that is an expression or a directive has no place in them.
                let position =
                    ib.Position
                    |> Option.bind (function
                        | CWL.BindingPosition.Number number -> Some number
                        | CWL.BindingPosition.Expression _
                        | CWL.BindingPosition.Imported _
                        | CWL.BindingPosition.Included _
                        | CWL.BindingPosition.Mixed _ -> None)
                WorkflowConversion.composeFormalParamInputIdentifiers (ib.Prefix |> Option.bind CWL.SchemaSaladString.tryLiteral) position
            )
        LDFormalParameter.create(
            additionalType = additionalType,
            id = id,
            name = inp.Name,
            valueRequired = valueRequired,
            ?identifiers = identifiers
        )

    static member decomposeInputBindings(identifiers : ResizeArray<LDNode>, ?context : LDContext) =
        let prefix = Seq.tryPick (fun n -> LDPropertyValue.tryGetAsPrefix(n, ?context = context)) identifiers
        let position = Seq.tryPick (fun n -> LDPropertyValue.tryGetAsPosition(n, ?context = context)) identifiers
        match prefix, position with
        | None, None -> None
        | _ ->
            CWL.InputBinding.create(
                ?prefix = (prefix |> Option.map CWL.SchemaSaladString.Literal),
                ?position = (position |> Option.map CWL.BindingPosition.Number)
            )
            |> Some

    static member decomposeInputFromFormalParameter(inp : LDNode, ?context : LDContext, ?graph : LDGraph) =
        let t = inp.AdditionalType |> Seq.exactlyOne |> WorkflowConversion.decomposeAdditionalType
        let binding = WorkflowConversion.decomposeInputBindings(LDFormalParameter.getIdentifiers(inp, ?context = context, ?graph = graph), ?context = context)
        let required = LDFormalParameter.tryGetValueRequiredAsBoolean(inp, ?context = context) = Some true
        let name = LDFormalParameter.getNameAsString(inp, ?context = context)
        CWL.CWLInput(name, (if required then t else CWL.CWLType.makeOptional t), ?inputBinding = (binding |> Option.map CWL.ListEntry.Value))

    static member composeFormalParameterOutputIdentifiers (glob : CWL.OutputGlob option) =
        match glob with
        | Some globs ->
            globs.AsValues()
            |> Seq.choose CWL.SchemaSaladString.tryLiteral
            |> Seq.map (fun g -> LDPropertyValue.createGlob(g))
            |> ResizeArray
            |> Some
        | None -> None

    static member composeFormalParameterFromOutput (out : CWL.CWLOutput, ?workflowName : string, ?runName) =
        let additionalType =
            match out.Type_ with
            | Some t -> WorkflowConversion.composeAdditionalType t
            | None -> failwith "Output must have a type"
        let id = LDFormalParameter.genID(name = out.Name, ?workflowName = workflowName, ?runName = runName)
        let identifiers =
            out.OutputBinding
            |> Option.bind CWL.ListEntry.tryValue
            |> Option.bind (fun ob ->
                WorkflowConversion.composeFormalParameterOutputIdentifiers ob.Glob
            )
        LDFormalParameter.create(
            additionalType = additionalType,
            id = id,
            name = out.Name,
            valueRequired = true,
            ?identifiers = identifiers
        )

    static member decomposeOutputBindings(identifiers : ResizeArray<LDNode>, ?context : LDContext) =
        let globs = identifiers |> Seq.choose (fun n -> LDPropertyValue.tryGetAsGlob(n, ?context = context)) |> ResizeArray
        match globs.Count with
        | 0 -> CWL.OutputBinding.create()
        | 1 -> CWL.OutputBinding.create(glob = CWL.OutputGlob.Single (CWL.SchemaSaladString.Literal globs.[0]))
        | _ -> CWL.OutputBinding.create(glob = CWL.OutputGlob.Multiple (globs |> ResizeArray.map CWL.SchemaSaladString.Literal))

    static member decomposeOutputFromFormalParameter(outp : LDNode, ?context : LDContext, ?graph : LDGraph) =
        let t = outp.AdditionalType |> Seq.exactlyOne |> WorkflowConversion.decomposeAdditionalType
        let binding = WorkflowConversion.decomposeOutputBindings(LDFormalParameter.getIdentifiers(outp, ?context = context, ?graph = graph), ?context = context)
        let optional = LDFormalParameter.tryGetValueRequiredAsBoolean(outp, ?context = context) |> Option.map (not)
        let name = LDFormalParameter.getNameAsString(outp, ?context = context)
        CWL.CWLOutput(name, t, CWL.ListEntry.Value binding)

    static member composeComputationalTool (tool : Process.Component) =
        let n, na =
            match tool.ComponentType with
            | Some c when c.Name.IsSome -> c.Name.Value, c.TermAccessionNumber
            | _ -> failwith "Component must have a type"
        let v,va =
            match tool.ComponentValue with
            | Some (ScalarValue.Name s) -> Some s, None
            | Some (ScalarValue.Float f) -> Some (f.ToString()), None
            | Some (ScalarValue.Int i) -> Some (i.ToString()), None
            | Some (ScalarValue.Ontology oa) -> oa.Name, oa.TermAccessionNumber
            | None -> None, None
        let u,ua =
            match tool.ComponentUnit with
            | Some oa -> oa.Name, oa.TermAccessionNumber
            | None -> None, None
        LDPropertyValue.createComponent(n, ?value = v, ?propertyID = na, ?valueReference = va, ?unitCode = ua, ?unitText = u)

    static member decomposeComputationalTool(tool : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let h,v,u = BaseTypes.decomposePropertyValue(tool, ?context = context)
        Process.Component.create(
            ?value = v,
            ?unit = u,
            componentType = h
        )

    /// <summary>
    /// Returns the normalized input parameters for a CWL processing unit.
    /// Supports CommandLineTool, Workflow, and ExpressionTool variants.
    /// </summary>
    /// <param name="pu">The processing unit to inspect.</param>
    static member getInputParametersFromProcessingUnit (pu : CWL.CWLProcessingUnit) =
        CWL.CWLProcessingUnit.getInputValues pu

    
    static member toolDescriptionTypeName = "ToolDescription"

    static member workflowDescriptionTypeName = "WorkflowDescription"

    static member expressionToolDescriptionTypeName = "ExpressionToolDescription"

    static member operationDescriptionTypeName = "OperationDescription"

    /// True when a path from the root of the ARC lies in one of its folders: assays, studies, workflows or runs.
    static member private isInArcFolder (path : string) : bool =
        let segments = ArcPathHelper.split path
        segments.Length > 1
        && (segments.[0] = ArcPathHelper.AssaysFolderName
            || segments.[0] = ArcPathHelper.StudiesFolderName
            || segments.[0] = ArcPathHelper.WorkflowsFolderName
            || segments.[0] = ArcPathHelper.RunsFolderName)

    /// <summary>
    /// Converts a workflow step run payload to a storable dataset identifier.
    /// A run that names a file in a folder of the ARC is resolved against the folder of the CWL file and written as a path
    /// from the root of the crate with a leading "./", like the ids of the files in the crate. Any other run that names a
    /// file is written as it is. Inline runs are mapped to stable inline markers. A process whose class is a directive has
    /// no path and no marker, so it has no identifier.
    /// </summary>
    /// <param name="cwlFilePath">The path of the CWL file that holds the step, from the root of the ARC.</param>
    /// <param name="run">Workflow step run payload.</param>
    static member runToIdentifier (cwlFilePath : string, run : CWL.WorkflowStepRun) : string option =
        let ofPath (runPath : string) =
            match WorkflowConversion.tryResolveFileReference cwlFilePath runPath with
            | Some path when WorkflowConversion.isInArcFolder path -> "./" + path
            | _ -> runPath
        match run with
        | CWL.RunString runPath
        | CWL.RunImport runPath -> Some (ofPath runPath)
        | CWL.RunMixed raw -> Some (ofPath raw.MixinPath)
        | CWL.RunRaw _ -> None
        | CWL.RunCommandLineTool _ -> Some "inline:CommandLineTool"
        | CWL.RunWorkflow _ -> Some "inline:Workflow"
        | CWL.RunExpressionTool _ -> Some "inline:ExpressionTool"
        | CWL.RunOperation _ -> Some "inline:Operation"

    /// <summary>
    /// Converts the identifier of a workflow step back to its run as the CWL file writes it. An identifier that is a path
    /// from the root of the crate into a folder of the ARC becomes the path relative to the folder of the CWL file. Any
    /// other identifier stays as it is: an inline marker, or a run as written, which is what older crates hold.
    /// </summary>
    /// <param name="cwlFilePath">The path of the CWL file that holds the step, from the root of the crate.</param>
    /// <param name="identifier">The identifier of the step.</param>
    static member identifierToRun (cwlFilePath : string, identifier : string) : string =
        if identifier.StartsWith "./" && WorkflowConversion.isInArcFolder identifier then
            ArcPathHelper.getRelativePathFromFile cwlFilePath identifier
        else
            identifier

    /// <summary>
    /// Attempts to decode a CWL processing unit from an LDGraph node addressed by a resolved run path.
    /// Supports direct WorkflowProtocol nodes and ARC Workflow/Run datasets that reference a main workflow protocol.
    /// </summary>
    /// <param name="resolvedRunPath">Resolved run reference to look up in the graph.</param>
    /// <param name="graph">Optional LDGraph containing workflow protocol nodes.</param>
    /// <param name="context">Optional LDContext used for node validation and property access.</param>
    static member tryDecodeProcessingUnitFromGraph(resolvedRunPath: string, ?graph: LDGraph, ?context: LDContext) : CWL.CWLProcessingUnit option =
        let tryDecodeWorkflowProtocol (protocol: LDNode) =
            try
                Some (
                    WorkflowConversion.decomposeWorkflowProtocolToProcessingUnit(
                        protocol,
                        ?context = context,
                        ?graph = graph,
                        resolveRunReferences = false
                    )
                )
            with _ ->
                None

        match graph with
        | None -> None
        | Some graph ->
            // Normalize via FileSystem path helpers so lookup works with mixed "./" and normalized identifiers.
            let normalizedResolvedRunPath = ArcPathHelper.normalize resolvedRunPath
            let candidateIds =
                ResizeArray [
                    resolvedRunPath
                    normalizedResolvedRunPath
                    $"./{normalizedResolvedRunPath}"
                ]
                |> Seq.distinct
                |> Seq.toArray
            let candidateNode =
                candidateIds
                |> Array.tryPick graph.TryGetNode
            // Accept either direct workflow protocol nodes or datasets that contain one as main entity.
            match candidateNode with
            | Some node when LDWorkflowProtocol.validate(node, ?context = context) ->
                tryDecodeWorkflowProtocol node
            | Some node when LDDataset.validateARCWorkflow(node, graph = graph, ?context = context) ->
                LDDataset.tryGetMainEntityAsWorkflowProtocol(node, graph = graph, ?context = context)
                |> Option.bind tryDecodeWorkflowProtocol
            | Some node when LDDataset.validateARCRun(node, ?context = context) ->
                LDDataset.tryGetMainEntityAsWorkflowProtocol(node, graph = graph, ?context = context)
                |> Option.bind tryDecodeWorkflowProtocol
            | _ ->
                None

    /// True when a reference is a relative path, which names a file in relation to the CWL file that holds it. A
    /// reference with a URI scheme, as in https://example.org/tool.cwl, and an absolute path name no file of the ARC.
    static member private isRelativeFileReference (reference: string) : bool =
        not (System.String.IsNullOrWhiteSpace reference)
        && not (System.Text.RegularExpressions.Regex.IsMatch(reference, "^[A-Za-z][A-Za-z0-9+.\\-]+:"))
        && (fst (ArcPathHelper.splitWithPrefix reference)).IsNone

    /// The path from the root of the ARC that a relative reference names, resolved against the folder of the CWL file
    /// that holds it, and none for any other reference and for a path outside the ARC.
    static member private tryResolveFileReference (cwlFilePath : string) (reference : string) : string option =
        if WorkflowConversion.isRelativeFileReference reference then
            let path = ArcPathHelper.resolvePathFromFile cwlFilePath reference
            if path = ".." || path.StartsWith "../" then None else Some path
        else
            None

    /// The file references in the YAML tree of a CWL document, in the order of the document: the run of every step that
    /// is a text, the text target of every $import, $include and $mixin directive, and the location or path of every
    /// File and Directory literal. Each reference comes with true when it names a directory.
    static member private fileReferencesOfYaml (root: YAMLicious.YamlNode) : ResizeArray<string * bool> =
        let references = ResizeArray<string * bool>()
        let tryScalarText (node: YAMLicious.YamlNode) =
            match node.Kind with
            | YAMLicious.YamlNodeKind.Scalar scalar when not (YAMLicious.YamlNode.isMissingValue node) -> Some scalar.Value
            | _ -> None
        let isDirective (key: string) = key = "$import" || key = "$include" || key = "$mixin"
        let rec visit (isStep: bool) (node: YAMLicious.YamlNode) =
            match node.Kind with
            | YAMLicious.YamlNodeKind.Mapping collection ->
                let fieldText (name: string) =
                    collection.Items
                    |> List.tryPick (fun entry -> if tryScalarText entry.Key = Some name then tryScalarText entry.Value else None)
                match fieldText "class" with
                | Some literalClass when literalClass = "File" || literalClass = "Directory" ->
                    fieldText "location"
                    |> Option.orElse (fieldText "path")
                    |> Option.iter (fun path -> references.Add((path, literalClass = "Directory")))
                | _ -> ()
                for entry in collection.Items do
                    let key = tryScalarText entry.Key |> Option.defaultValue ""
                    match tryScalarText entry.Value with
                    | Some path when isDirective key || (isStep && key = "run") -> references.Add((path, false))
                    | _ when key = "steps" -> visitSteps entry.Value
                    | _ -> visit false entry.Value
            | YAMLicious.YamlNodeKind.Sequence collection ->
                for item in collection.Items do
                    visit false item.Node
            | _ -> ()
        // The steps are the values of a mapping or the items of a list. A directive can stand for them.
        and visitSteps (node: YAMLicious.YamlNode) =
            match node.Kind with
            | YAMLicious.YamlNodeKind.Mapping collection ->
                for entry in collection.Items do
                    let key = tryScalarText entry.Key |> Option.defaultValue ""
                    match tryScalarText entry.Value with
                    | Some path when isDirective key -> references.Add((path, false))
                    | _ -> visit true entry.Value
            | YAMLicious.YamlNodeKind.Sequence collection ->
                for item in collection.Items do
                    visit true item.Node
            | _ -> ()
        visit false root
        references

    /// The processes that the steps of a process resolved their runs to, each with the run as it is written. The steps of
    /// an inline workflow are steps of the same file, and each inline workflow is entered once.
    static member private resolvedRunsOf (processingUnit: CWL.CWLProcessingUnit) : ResizeArray<string * CWL.CWLProcessingUnit> =
        let resolved = ResizeArray<string * CWL.CWLProcessingUnit>()
        let entered = ResizeArray<CWL.CWLWorkflowDescription>()
        let rec visit (workflow: CWL.CWLWorkflowDescription) =
            if not (entered |> Seq.exists (fun other -> System.Object.ReferenceEquals(other, workflow))) then
                entered.Add workflow
                for step in CWL.CWLWorkflowDescription.getStepValues workflow do
                    match step.Run with
                    | CWL.RunString runPath
                    | CWL.RunImport runPath ->
                        step.ResolvedRun.Run
                        |> Option.bind CWL.WorkflowStepRunOps.tryGetProcessingUnit
                        |> Option.iter (fun target -> resolved.Add((runPath, target)))
                    | CWL.RunWorkflow _ ->
                        CWL.WorkflowStepRunOps.tryGetWorkflow step.Run |> Option.iter visit
                    | _ -> ()
        match processingUnit with
        | CWL.Workflow workflow -> visit workflow
        | _ -> ()
        resolved

    /// <summary>
    /// Returns the local files that a CWL file needs, as paths from the root of the ARC. These are the run of every step
    /// that names a file, the target of every $import, $include and $mixin directive and the location or path of every
    /// File literal, each resolved against the folder of the CWL file that holds it. A Directory literal adds every file
    /// under its directory. A file that a step runs adds the files that it needs in turn, when the step holds the process
    /// that its run resolved to. References with a URI scheme, absolute paths, paths outside the ARC and paths that are
    /// not in the file system are left out. Each CWL file is visited once, so a cycle of references ends.
    /// </summary>
    /// <param name="cwlFilePath">The path of the CWL file from the root of the ARC.</param>
    /// <param name="processingUnit">The process that the CWL file describes.</param>
    /// <param name="fs">The file system of the ARC.</param>
    static member getReferencedFilePaths (cwlFilePath: string, processingUnit: CWL.CWLProcessingUnit, fs: FileSystem) : ResizeArray<string> =
        let mainPath = ArcPathHelper.normalize cwlFilePath
        let paths = ResizeArray<string>()
        let visited = System.Collections.Generic.HashSet<string>()
        let isFile (path: string) =
            match fs.Tree.TryGetPath path with
            | Some (FileSystemTree.File _) -> true
            | _ -> false
        let add (path: string) =
            if path <> mainPath && not (paths.Contains path) then paths.Add path
        let rec visit (filePath: string) (processingUnit: CWL.CWLProcessingUnit) =
            if visited.Add filePath then
                for (reference, isDirectory) in WorkflowConversion.fileReferencesOfYaml (CWL.Encode.encodeProcessingUnitElement processingUnit) do
                    match WorkflowConversion.tryResolveFileReference filePath reference with
                    | Some path when isDirectory ->
                        match fs.Tree.TryGetPath path with
                        | Some (FileSystemTree.Folder _ as folder) ->
                            for file in folder.ToFilePaths(true) do
                                add (ArcPathHelper.combine path file)
                        | _ -> ()
                    | Some path when isFile path -> add path
                    | _ -> ()
                for (runPath, target) in WorkflowConversion.resolvedRunsOf processingUnit do
                    WorkflowConversion.tryResolveFileReference filePath runPath |> Option.iter (fun path -> visit path target)
        visit mainPath processingUnit
        paths

    static member composeWorkflowProtocolFromToolDescription (filePath : string, workflow : CWL.CWLToolDescription, ?workflowName : string, ?runName : string) =
        let inputs =
            workflow.Inputs
            |> Option.map (CWL.ListEntry.values >> ResizeArray.map (fun i -> WorkflowConversion.composeFormalParameterFromInput(i, ?workflowName = workflowName, ?runName = runName)))
        let outputs =
            CWL.ListEntry.values workflow.Outputs
            |> ResizeArray.map (fun o -> WorkflowConversion.composeFormalParameterFromOutput(o, ?workflowName = workflowName, ?runName = runName))
        LDWorkflowProtocol.create(
            id = filePath,
            name = filePath,
            ?inputs = inputs,
            programmingLanguages = ResizeArray.singleton (LDComputerLanguage.createCWL()),
            outputs = outputs,
            additionalType = ResizeArray [WorkflowConversion.toolDescriptionTypeName]
        )

    static member decomposeWorkflowProtocolToToolDescription (protocol : LDNode, ?context : LDContext, ?graph : LDGraph) =
        let inputs =
            LDComputationalWorkflow.getInputsAsFormalParameters(protocol, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun i -> WorkflowConversion.decomposeInputFromFormalParameter(i, ?context = context, ?graph = graph))
        let outputs =
            LDComputationalWorkflow.getOutputsAsFormalParameter(protocol, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun o -> WorkflowConversion.decomposeOutputFromFormalParameter(o, ?context = context, ?graph = graph))
        CWL.CWLToolDescription(
            outputs = CWL.ListEntry.ofValues outputs,
            inputs = CWL.ListEntry.ofValues inputs
        )

    /// <summary>
    /// Converts an ExpressionTool CWL description into an LD workflow protocol node.
    /// Persists the expression text in schema:description to keep roundtrip fidelity in the current RO-Crate projection.
    /// </summary>
    /// <param name="filePath">Identifier/path used as protocol id and name.</param>
    /// <param name="expressionTool">ExpressionTool description to compose.</param>
    /// <param name="workflowName">Optional workflow name used for generated formal parameter IDs.</param>
    /// <param name="runName">Optional run name used for generated formal parameter IDs.</param>
    static member composeWorkflowProtocolFromExpressionToolDescription (filePath : string, expressionTool : CWL.CWLExpressionToolDescription, ?workflowName : string, ?runName : string) =
        let inputs =
            expressionTool.Inputs
            |> Option.map (CWL.ListEntry.values >> ResizeArray.map (fun i -> WorkflowConversion.composeFormalParameterFromInput(i, ?workflowName = workflowName, ?runName = runName)))
        let outputs =
            CWL.ListEntry.values expressionTool.Outputs
            |> ResizeArray.map (fun o -> WorkflowConversion.composeFormalParameterFromOutput(o, ?workflowName = workflowName, ?runName = runName))
        let protocol =
            LDWorkflowProtocol.create(
                id = filePath,
                name = filePath,
                ?inputs = inputs,
                programmingLanguages = ResizeArray.singleton (LDComputerLanguage.createCWL()),
                outputs = outputs,
                additionalType = ResizeArray [WorkflowConversion.expressionToolDescriptionTypeName]
            )
        // Persist expression on schema:description to keep roundtrip fidelity without introducing
        // a custom LD predicate in the current RO-Crate projection.
        LDLabProtocol.setDescriptionAsString(protocol, expressionTool.Expression.AsDirectiveString)
        protocol

    /// <summary>
    /// Converts an LD workflow protocol node into an ExpressionTool CWL description.
    /// Falls back to <c>$(null)</c> if no expression text is stored.
    /// </summary>
    /// <param name="protocol">Workflow protocol node to decompose.</param>
    /// <param name="context">Optional LDContext used for property reads.</param>
    /// <param name="graph">Optional LDGraph used to resolve linked nodes.</param>
    static member decomposeWorkflowProtocolToExpressionToolDescription (protocol : LDNode, ?context : LDContext, ?graph : LDGraph) =
        let inputs =
            LDComputationalWorkflow.getInputsAsFormalParameters(protocol, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun i -> WorkflowConversion.decomposeInputFromFormalParameter(i, ?context = context, ?graph = graph))
        let outputs =
            LDComputationalWorkflow.getOutputsAsFormalParameter(protocol, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun o -> WorkflowConversion.decomposeOutputFromFormalParameter(o, ?context = context, ?graph = graph))
        let expression =
            LDLabProtocol.tryGetDescriptionAsString(protocol, ?context = context)
            // Keep deserialization resilient for externally produced or incomplete crates.
            |> Option.defaultValue "$(null)"
        CWL.CWLExpressionToolDescription(
            outputs = CWL.ListEntry.ofValues outputs,
            expression = CWL.SchemaSaladString.Literal expression,
            inputs = CWL.ListEntry.ofValues inputs
        )

    static member composeWorkflowProtocolFromOperationDescription (filePath : string, operation : CWL.CWLOperationDescription, ?workflowName : string, ?runName : string) =
        let inputs =
            CWL.ListEntry.values operation.Inputs
            |> ResizeArray.map (fun i -> WorkflowConversion.composeFormalParameterFromInput(i, ?workflowName = workflowName, ?runName = runName))
        let outputs =
            CWL.ListEntry.values operation.Outputs
            |> ResizeArray.map (fun o -> WorkflowConversion.composeFormalParameterFromOutput(o, ?workflowName = workflowName, ?runName = runName))
        LDWorkflowProtocol.create(
            id = filePath,
            name = filePath,
            inputs = inputs,
            programmingLanguages = ResizeArray.singleton (LDComputerLanguage.createCWL()),
            outputs = outputs,
            additionalType = ResizeArray [WorkflowConversion.operationDescriptionTypeName]
        )

    static member decomposeWorkflowProtocolToOperationDescription (protocol : LDNode, ?context : LDContext, ?graph : LDGraph) =
        let inputs =
            LDComputationalWorkflow.getInputsAsFormalParameters(protocol, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun i -> WorkflowConversion.decomposeInputFromFormalParameter(i, ?context = context, ?graph = graph))
        let outputs =
            LDComputationalWorkflow.getOutputsAsFormalParameter(protocol, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun o -> WorkflowConversion.decomposeOutputFromFormalParameter(o, ?context = context, ?graph = graph))
        CWL.CWLOperationDescription(
            inputs = CWL.ListEntry.ofValues inputs,
            outputs = CWL.ListEntry.ofValues outputs
        )

    /// A step with an id is named by it. A step without an id has no name in the crate and gets an identifier from its
    /// position, which names the node in the crate only and never becomes an id of the step. The file path is the path of
    /// the CWL file that holds the step, from the root of the ARC.
    static member composeWorkflowStep (step : CWL.WorkflowStep, filePath : string, ?position : int) =
        let id =
            match step.Id, position with
            | Some stepId, _ -> Some $"WorkflowStep_{filePath}_{stepId}"
            | None, Some position -> Some $"WorkflowStep_{filePath}?position={position}"
            | None, None -> None
        let cw = LDComputationalWorkflow.create(?id = id, ?name = step.Id)
        // Persist run info as identifier so decompose can reconstruct RunString or inline markers.
        WorkflowConversion.runToIdentifier(filePath, step.Run) |> Option.iter (fun identifier -> LDDataset.setIdentifierAsString(cw, identifier))
        cw

    static member composeWorkflowProtocolFromWorkflow (filePath : string, workflow : CWL.CWLWorkflowDescription, ?workflowName : string, ?runName : string) =
        let inputs =
            CWL.ListEntry.values workflow.Inputs
            |> ResizeArray.map (fun i -> WorkflowConversion.composeFormalParameterFromInput(i, ?workflowName = workflowName, ?runName = runName))
        let outputs =
            CWL.ListEntry.values workflow.Outputs
            |> ResizeArray.map (fun o -> WorkflowConversion.composeFormalParameterFromOutput(o, ?workflowName = workflowName, ?runName = runName))
        let steps =
            CWL.ListEntry.values workflow.Steps
            |> Seq.mapi (fun position s -> WorkflowConversion.composeWorkflowStep(s, filePath, position))
            |> ResizeArray
        LDWorkflowProtocol.create(
            id = filePath,
            name = filePath,
            inputs = inputs,
            outputs = outputs,
            programmingLanguages = ResizeArray.singleton (LDComputerLanguage.createCWL()),
            hasParts = steps,
            additionalType = ResizeArray [WorkflowConversion.workflowDescriptionTypeName]
        )

    /// <summary>
    /// Converts a workflow-step LD node into a CWL WorkflowStep.
    /// Can optionally resolve the run reference of the step via the graph. The step keeps the reference as its run and
    /// holds the resolved process in its ResolvedRun.
    /// </summary>
    /// <param name="step">Workflow step LD node.</param>
    /// <param name="workflowFilePath">Optional workflow file path used for turning the identifier back into the run as written and for resolving relative run references.</param>
    /// <param name="resolveRunReferences">If true and a workflow file path is given, the step resolves its run through the graph. Otherwise the step has no resolved run.</param>
    /// <param name="context">Optional LDContext for node property access.</param>
    /// <param name="graph">Optional LDGraph used for run reference resolution.</param>
    static member decomposeWorkflowStep (step : LDNode, ?workflowFilePath: string, ?resolveRunReferences: bool, ?context : LDContext, ?graph : LDGraph) =
        let name = LDComputationalWorkflow.tryGetNameAsString(step, ?context = context)
        let identifier = LDDataset.getIdentifierAsString(step, ?context = context)
        let runString =
            match workflowFilePath with
            | Some path -> WorkflowConversion.identifierToRun(path, identifier)
            | None -> identifier
        let shouldResolveRunReferences = defaultArg resolveRunReferences true
        let inputs = ResizeArray()
        let output = ResizeArray<CWL.StepOutput>()
        let workflowStep = CWL.WorkflowStep(id = name, in_ = inputs, out_ = output, run = CWL.RunString runString)
        // Without a workflow path we cannot resolve relative references safely.
        match workflowFilePath with
        | Some path when shouldResolveRunReferences ->
            // Use graph-backed lookup so run paths resolve to CWL objects of the crate.
            let tryResolveRunPath runPath =
                WorkflowConversion.tryDecodeProcessingUnitFromGraph(runPath, ?graph = graph, ?context = context)
            CWLRunResolver.resolveWorkflowStepFromLookup path workflowStep tryResolveRunPath
        | _ -> ()
        workflowStep


    /// <summary>
    /// Converts an LD workflow protocol node to a CWL workflow description.
    /// Optionally resolves step run references using the containing graph.
    /// </summary>
    /// <param name="protocol">Workflow protocol node to convert.</param>
    /// <param name="resolveRunReferences">If true, attempts to resolve run strings to inline processing units.</param>
    /// <param name="context">Optional LDContext for property access.</param>
    /// <param name="graph">Optional LDGraph used for linked node and run-resolution lookups.</param>
    static member decomposeWorkflowProtocolToWorkflow (protocol : LDNode, ?resolveRunReferences: bool, ?context : LDContext, ?graph : LDGraph) =
        let workflowFilePath =
            LDDataset.tryGetIdentifierAsString(protocol, ?context = context)
            |> Option.defaultValue protocol.Id
        let shouldResolveRunReferences = defaultArg resolveRunReferences true
        let inputs =
            LDComputationalWorkflow.getInputsAsFormalParameters(protocol, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun i -> WorkflowConversion.decomposeInputFromFormalParameter(i, ?context = context, ?graph = graph))
        let outputs =
            LDComputationalWorkflow.getOutputsAsFormalParameter(protocol, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun o -> WorkflowConversion.decomposeOutputFromFormalParameter(o, ?context = context, ?graph = graph))
        let steps =
            LDComputationalWorkflow.getHasPart(protocol, ?graph = graph, ?context = context)
            // A step without an identifier held a process whose class is a directive, which the crate cannot give back.
            |> Seq.filter (fun s -> (LDDataset.tryGetIdentifierAsString(s, ?context = context)).IsSome)
            |> ResizeArray
            |> ResizeArray.map (fun s ->
                // Forward workflow path and options to keep step-level run resolution consistent.
                WorkflowConversion.decomposeWorkflowStep(
                    s,
                    workflowFilePath = workflowFilePath,
                    resolveRunReferences = shouldResolveRunReferences,
                    ?context = context,
                    ?graph = graph
                )
            )
        CWL.CWLWorkflowDescription(
            steps = CWL.ListEntry.ofValues steps,
            inputs = CWL.ListEntry.ofValues inputs,
            outputs = CWL.ListEntry.ofValues outputs
        )

    /// <summary>
    /// Converts a generic CWL processing unit into a workflow protocol representation.
    /// Dispatches by processing-unit variant.
    /// </summary>
    /// <param name="filePath">Identifier/path used as protocol id and name.</param>
    /// <param name="pu">Processing unit to compose.</param>
    /// <param name="workflowName">Optional workflow name used for generated formal parameter IDs.</param>
    /// <param name="runName">Optional run name used for generated formal parameter IDs.</param>
    static member composeWorkflowProtocolFromProcessingUnit (filePath, pu : CWL.CWLProcessingUnit, ?workflowName : string, ?runName : string) =
        match pu with
        | CWL.CommandLineTool tool ->
            WorkflowConversion.composeWorkflowProtocolFromToolDescription(filePath, tool, ?workflowName = workflowName, ?runName = runName)
        | CWL.Workflow wf -> 
            WorkflowConversion.composeWorkflowProtocolFromWorkflow(filePath, wf, ?workflowName = workflowName, ?runName = runName)
        | CWL.ExpressionTool et ->
            WorkflowConversion.composeWorkflowProtocolFromExpressionToolDescription(filePath, et, ?workflowName = workflowName, ?runName = runName)
        | CWL.Operation op ->
            WorkflowConversion.composeWorkflowProtocolFromOperationDescription(filePath, op, ?workflowName = workflowName, ?runName = runName)
        // The class of the process is a directive, so the crate cannot tell its kind or its ports. It holds the process as a
        // tool without ports, which is what the decomposition makes of a protocol of unknown kind.
        | CWL.RawProcess _ ->
            WorkflowConversion.composeWorkflowProtocolFromToolDescription(filePath, CWL.CWLToolDescription(outputs = ResizeArray()), ?workflowName = workflowName, ?runName = runName)

    /// <summary>
    /// Converts a workflow protocol node back into the appropriate CWL processing unit variant.
    /// Uses <c>additionalType</c> to discriminate Workflow, ExpressionTool, and Operation descriptions.
    /// </summary>
    /// <param name="protocol">Workflow protocol node to decompose.</param>
    /// <param name="resolveRunReferences">If true, run references inside workflows are resolved through graph lookup.</param>
    /// <param name="context">Optional LDContext for property access.</param>
    /// <param name="graph">Optional LDGraph for linked node lookup and run resolution.</param>
    static member decomposeWorkflowProtocolToProcessingUnit (protocol : LDNode, ?resolveRunReferences: bool, ?context : LDContext, ?graph : LDGraph) =
        let shouldResolveRunReferences = defaultArg resolveRunReferences true
        // Keep default branch as CommandLineTool for backward compatibility with legacy crates.
        match LDComputationalWorkflow.getAdditionalTypeAsString(protocol, ?context = context) with
        | s when s = WorkflowConversion.workflowDescriptionTypeName ->
            WorkflowConversion.decomposeWorkflowProtocolToWorkflow(protocol, resolveRunReferences = shouldResolveRunReferences, ?context = context, ?graph = graph)
            |> CWL.Workflow
        | s when s = WorkflowConversion.expressionToolDescriptionTypeName ->
            WorkflowConversion.decomposeWorkflowProtocolToExpressionToolDescription(protocol, ?context = context, ?graph = graph)
            |> CWL.ExpressionTool
        | s when s = WorkflowConversion.operationDescriptionTypeName ->
            WorkflowConversion.decomposeWorkflowProtocolToOperationDescription(protocol, ?context = context, ?graph = graph)
            |> CWL.Operation
        | s ->
            WorkflowConversion.decomposeWorkflowProtocolToToolDescription(protocol, ?context = context, ?graph = graph)
            |> CWL.CommandLineTool              

    static member composeWorkflow (workflow : ArcWorkflow, ?fs : FileSystem) =
        BaseTypes.warnIfTitleEmpty("Workflow", workflow.Identifier, workflow.Title)
        let workflowProtocol =
            let workflowFilePath = Identifier.Workflow.cwlFileNameFromIdentifier workflow.Identifier
            match workflow.CWLDescription with
            | Some pu -> WorkflowConversion.composeWorkflowProtocolFromProcessingUnit(workflowFilePath, pu, workflowName = workflow.Identifier)
            | None -> failwithf "Workflow %s must have a CWL description" workflow.Identifier
        let publisher = LDOrganization.create("DataPLANT")
        let persons = workflow.Investigation |> Option.map (fun inv -> seq(inv.GetAllPersons()))
        let creators =
            workflow.Contacts
            |> ResizeArray.map (fun c -> PersonConversion.composePerson(c, ?persons = persons))
            |> Option.fromSeq
        let dateCreated = System.DateTime.UtcNow
        if creators.IsSome then
            LDComputationalWorkflow.setCreators(workflowProtocol, creators.Value)
        LDComputationalWorkflow.setSdPublisher(workflowProtocol, publisher)
        LDComputationalWorkflow.setDateCreatedAsDateTime(workflowProtocol, dateCreated)
        if workflow.Version.IsSome then
            LDLabProtocol.setVersionAsString(workflowProtocol, workflow.Version.Value)
        if workflow.URI.IsSome then
            LDLabProtocol.setUrl(workflowProtocol, workflow.URI.Value)
        if workflow.WorkflowType.IsSome then
            let intendedUse = BaseTypes.composeDefinedTerm(workflow.WorkflowType.Value)
            LDLabProtocol.setIntendedUseAsDefinedTerm(workflowProtocol, intendedUse)
        //if workflow.Parameters.Count > 0 then
        //    let parameters = 
        //        workflow.Parameters
        //        |> ResizeArray.choose (fun p ->
        //            p.ParameterName |> Option.map BaseTypes.composeDefinedTerm
        //        )
        //    LDLabProtocol.set(workflowProtocol, inputs)
        let fragmentDescriptors =
            workflow.Datamap
            |> Option.map DatamapConversion.composeFragmentDescriptors
        let dataFiles =
            fragmentDescriptors
            |> Option.defaultValue (ResizeArray [])
            |> ResizeArray.choose (fun df -> LDPropertyValue.tryGetSubjectOf(df))
        if workflow.Components.Count > 0 then
            let softwareTools =
                workflow.Components
                |> ResizeArray.map WorkflowConversion.composeComputationalTool
            LDLabProtocol.setComputationalTools(workflowProtocol, softwareTools)
        let hasParts =
            if dataFiles.Count = 0 then
                ResizeArray.singleton workflowProtocol |> Some
            else
                ResizeArray.appendSingleton workflowProtocol dataFiles |> Some
        let comments =
            workflow.Comments
            |> ResizeArray.map (fun c -> BaseTypes.composeComment c)
            |> Option.fromSeq
        let arcWorkflow =
            LDDataset.createARCWorkflow(
                identifier = workflow.Identifier,
                mainEntities = ResizeArray.singleton workflowProtocol,
                ?name = workflow.Title,
                ?description = workflow.Description,
                ?creators = creators,
                ?hasParts = hasParts,
                ?comments = comments
            )
        fragmentDescriptors |> Option.iter (fun fds -> LDDataset.setVariableMeasuredAsPropertyValues(arcWorkflow, fds))
        arcWorkflow

    static member decomposeWorkflow (workflow : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let workflowProtocol =
            LDDataset.getMainEntities(workflow, ?graph = graph, ?context = context)
            |> Seq.find (fun me ->
                LDComputationalWorkflow.validate(me, ?context = context)
            )
        let cwlDescription = 
            WorkflowConversion.decomposeWorkflowProtocolToProcessingUnit(workflowProtocol, ?context = context, ?graph = graph)
        let version =
            LDLabProtocol.tryGetVersionAsString(workflowProtocol, ?context = context)
        let uri =
            LDLabProtocol.tryGetUrl(workflowProtocol, ?context = context)
        let workflowType =
            LDLabProtocol.tryGetIntendedUseAsDefinedTerm(workflowProtocol, ?graph = graph, ?context = context)
            |> Option.map (fun iu -> BaseTypes.decomposeDefinedTerm(iu, ?context = context))
        let components =
            LDLabProtocol.getComputationalTools(workflowProtocol, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun ct -> WorkflowConversion.decomposeComputationalTool(ct, ?graph = graph, ?context = context))
        let contacts =
            LDDataset.getCreators(workflow, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> PersonConversion.decomposePerson(c, ?graph = graph, ?context = context))
        let comments =
            LDDataset.getComments(workflow, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> BaseTypes.decomposeComment(c, ?context = context))
        let datamap = 
            LDDataset.getVariableMeasuredAsFragmentDescriptors(workflow, ?graph = graph, ?context = context)
            |> fun fds -> DatamapConversion.decomposeFragmentDescriptors(fds, ?graph = graph, ?context = context)
            |> Option.fromValueWithDefault (Datamap.init())

        ArcWorkflow.create(
            identifier = LDDataset.getIdentifierAsString(workflow, ?context = context),
            ?title = LDDataset.tryGetNameAsString(workflow, ?context = context),
            ?description = LDDataset.tryGetDescriptionAsString(workflow, ?context = context),
            ?version = version,
            ?uri = uri,
            ?workflowType = workflowType,
            cwlDescription = cwlDescription,
            components = components,
            ?datamap = datamap,
            contacts = contacts,
            comments = comments
        )
