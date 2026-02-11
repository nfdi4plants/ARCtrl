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
            CWL.Encode.encodeCWLType t
            |> CWL.Encode.writeYaml
            |> fun s -> s.Trim()

    static member decomposeAdditionalType (t : string) : CWL.CWLType =
        // Try to parse as YAML (handles both shorthand and YAML-formatted complex types)
        let yamlElement = YAMLicious.Reader.read t
        
        // Check if it's a simple string (shorthand like "File[]", "string?")
        match yamlElement with
        | YAMLicious.YAMLiciousTypes.YAMLElement.Value _ ->
            // Shorthand format: parse string directly without YAML context
            // Note: YAMLicious.Decode.object requires a YAMLElement for its API,
            // but cwlTypeStringMatcher only uses the 'get' function and ignores the element.
            // This placeholder satisfies the function without affecting the actual parsing.
            let placeholderElement = YAMLicious.YAMLiciousTypes.YAMLElement.Comment "unused-shorthand-context"
            YAMLicious.Decode.object (fun get ->
                CWL.Decode.cwlTypeStringMatcher t get
            ) placeholderElement
            |> fst
        | YAMLicious.YAMLiciousTypes.YAMLElement.Object [YAMLicious.YAMLiciousTypes.YAMLElement.Sequence items] ->
            // YAML array wrapped in Object (happens when parsing multi-line YAML arrays)
            // Unwrap and treat as Union
            CWL.Decode.cwlTypeDecoder' (YAMLicious.YAMLiciousTypes.YAMLElement.Sequence items)
        | _ ->
            // Complex type in YAML format
            CWL.Decode.cwlTypeDecoder' yamlElement

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
        let valueRequired = inp.Optional |> Option.defaultValue false |> not
        let id = LDFormalParameter.genID(name = inp.Name, ?workflowName = workflowName, ?runName = runName)
        let identifiers =
            inp.InputBinding
            |> Option.bind (fun ib ->
                WorkflowConversion.composeFormalParamInputIdentifiers ib.Prefix ib.Position
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
                ?prefix = prefix,
                ?position = position
            )
            |> Some

    static member decomposeInputFromFormalParameter(inp : LDNode, ?context : LDContext, ?graph : LDGraph) =
        let t = inp.AdditionalType |> Seq.exactlyOne |> WorkflowConversion.decomposeAdditionalType
        let binding = WorkflowConversion.decomposeInputBindings(LDFormalParameter.getIdentifiers(inp, ?context = context, ?graph = graph), ?context = context)
        let optional =
            match LDFormalParameter.tryGetValueRequiredAsBoolean(inp, ?context = context) with
            | Some true -> None
            | _ -> Some true
        let name = LDFormalParameter.getNameAsString(inp, ?context = context)
        CWL.CWLInput(name, t, ?inputBinding = binding, ?optional = optional)

    static member composeFormalParameterOutputIdentifiers (glob : string option) =
        match glob with
        | Some g ->               
            ResizeArray [
                LDPropertyValue.createGlob(g)
            ]
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
        CWL.OutputBinding.create(
            ?glob = Seq.tryPick (fun n -> LDPropertyValue.tryGetAsGlob(n, ?context = context)) identifiers
        )

    static member decomposeOutputFromFormalParameter(inp : LDNode, ?context : LDContext, ?graph : LDGraph) =
        let t = inp.AdditionalType |> Seq.exactlyOne |> WorkflowConversion.decomposeAdditionalType
        let binding = WorkflowConversion.decomposeOutputBindings(LDFormalParameter.getIdentifiers(inp, ?context = context, ?graph = graph), ?context = context)
        let optional = LDFormalParameter.tryGetValueRequiredAsBoolean(inp, ?context = context) |> Option.map (not)
        let name = LDFormalParameter.getNameAsString(inp, ?context = context)
        CWL.CWLOutput(name, t, binding)

    static member composeComputationalTool (tool : Process.Component) =
        let n, na =
            match tool.ComponentType with
            | Some c when c.Name.IsSome -> c.Name.Value, c.TermAccessionNumber
            | _ -> failwith "Component must have a type"
        let v,va =
            match tool.ComponentValue with
            | Some (Value.Name s) -> Some s, None
            | Some (Value.Float f) -> Some (f.ToString()), None
            | Some (Value.Int i) -> Some (i.ToString()), None
            | Some (Value.Ontology oa) -> oa.Name, oa.TermAccessionNumber
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

    static member getInputParametersFromProcessingUnit (pu : CWL.CWLProcessingUnit) =
        match pu with
        | CWL.CommandLineTool tool ->
            match tool.Inputs with
            | Some inputs -> inputs |> ResizeArray.map (fun i -> i)
            | None -> ResizeArray []
        | CWL.Workflow wf -> 
            wf.Inputs |> ResizeArray.map (fun i -> i)
        | CWL.ExpressionTool et ->
            match et.Inputs with
            | Some inputs -> inputs |> ResizeArray.map (fun i -> i)
            | None -> ResizeArray []

    
    static member toolDescriptionTypeName = "ToolDescription"

    static member workflowDescriptionTypeName = "WorkflowDescription"

    static member expressionToolDescriptionTypeName = "ExpressionToolDescription"

    static member private runToIdentifier (run : CWL.WorkflowStepRun) : string =
        match run with
        | CWL.RunString runPath -> runPath
        | CWL.RunCommandLineTool _ -> "inline:CommandLineTool"
        | CWL.RunWorkflow _ -> "inline:Workflow"
        | CWL.RunExpressionTool _ -> "inline:ExpressionTool"

    static member private tryDecodeProcessingUnitFromGraph(resolvedRunPath: string, ?graph: LDGraph, ?context: LDContext) : CWL.CWLProcessingUnit option =
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

    static member composeWorkflowProtocolFromToolDescription (filePath : string, workflow : CWL.CWLToolDescription, ?workflowName : string, ?runName : string) =
        let inputs =
            workflow.Inputs
            |> Option.map (ResizeArray.map (fun i -> WorkflowConversion.composeFormalParameterFromInput(i, ?workflowName = workflowName, ?runName = runName)))
        let outputs =
            workflow.Outputs
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
            outputs = outputs,
            inputs = inputs
        )

    static member composeWorkflowProtocolFromExpressionToolDescription (filePath : string, expressionTool : CWL.CWLExpressionToolDescription, ?workflowName : string, ?runName : string) =
        let inputs =
            expressionTool.Inputs
            |> Option.map (ResizeArray.map (fun i -> WorkflowConversion.composeFormalParameterFromInput(i, ?workflowName = workflowName, ?runName = runName)))
        let outputs =
            expressionTool.Outputs
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
        LDLabProtocol.setDescriptionAsString(protocol, expressionTool.Expression)
        protocol

    static member decomposeWorkflowProtocolToExpressionToolDescription (protocol : LDNode, ?context : LDContext, ?graph : LDGraph) =
        let inputs =
            LDComputationalWorkflow.getInputsAsFormalParameters(protocol, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun i -> WorkflowConversion.decomposeInputFromFormalParameter(i, ?context = context, ?graph = graph))
        let outputs =
            LDComputationalWorkflow.getOutputsAsFormalParameter(protocol, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun o -> WorkflowConversion.decomposeOutputFromFormalParameter(o, ?context = context, ?graph = graph))
        let expression =
            LDLabProtocol.tryGetDescriptionAsString(protocol, ?context = context)
            |> Option.defaultValue "$(null)"
        CWL.CWLExpressionToolDescription(
            outputs = outputs,
            expression = expression,
            inputs = inputs
        )

    static member composeWorkflowStep (step : CWL.WorkflowStep, workflowID) =
        let id = $"WorkflowStep_{workflowID}_{step.Id}"
        let cw = LDComputationalWorkflow.create(id = id, name = step.Id)
        LDDataset.setIdentifierAsString(cw, WorkflowConversion.runToIdentifier step.Run)
        cw

    static member composeWorkflowProtocolFromWorkflow (filePath : string, workflow : CWL.CWLWorkflowDescription, ?workflowName : string, ?runName : string) =
        let inputs =
            workflow.Inputs
            |> ResizeArray.map (fun i -> WorkflowConversion.composeFormalParameterFromInput(i, ?workflowName = workflowName, ?runName = runName))
        let outputs =
            workflow.Outputs
            |> ResizeArray.map (fun o -> WorkflowConversion.composeFormalParameterFromOutput(o, ?workflowName = workflowName, ?runName = runName))
        let steps =
            workflow.Steps
            |> ResizeArray.map (fun s -> WorkflowConversion.composeWorkflowStep(s, filePath))
        LDWorkflowProtocol.create(
            id = filePath,
            name = filePath,
            inputs = inputs,
            outputs = outputs,
            programmingLanguages = ResizeArray.singleton (LDComputerLanguage.createCWL()),
            hasParts = steps,
            additionalType = ResizeArray [WorkflowConversion.workflowDescriptionTypeName]
        )

    static member decomposeWorkflowStep (step : LDNode, ?workflowFilePath: string, ?resolveRunReferences: bool, ?context : LDContext, ?graph : LDGraph) =
        let name = LDComputationalWorkflow.getNameAsString(step, ?context = context)
        let runString = LDDataset.getIdentifierAsString(step, ?context = context)
        let shouldResolveRunReferences = defaultArg resolveRunReferences true
        let run =
            let initialRun = CWL.RunString runString
            if not shouldResolveRunReferences then
                initialRun
            else
                match workflowFilePath with
                | Some path ->
                    let tryResolveRunPath runPath =
                        WorkflowConversion.tryDecodeProcessingUnitFromGraph(runPath, ?graph = graph, ?context = context)
                    CWLRunResolver.resolveWorkflowStepRunFromLookup path initialRun tryResolveRunPath
                | None ->
                    initialRun
        let inputs = ResizeArray()
        let output = ResizeArray<CWL.StepOutput>()
        CWL.WorkflowStep(id = name, in_ = inputs, out_ = output, run = run)


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
            |> ResizeArray.map (fun s ->
                WorkflowConversion.decomposeWorkflowStep(
                    s,
                    workflowFilePath = workflowFilePath,
                    resolveRunReferences = shouldResolveRunReferences,
                    ?context = context,
                    ?graph = graph
                )
            )
        CWL.CWLWorkflowDescription(
            steps = steps,
            inputs = inputs,
            outputs = outputs
        )

    static member composeWorkflowProtocolFromProcessingUnit (filePath, pu : CWL.CWLProcessingUnit, ?workflowName : string, ?runName : string) =
        match pu with
        | CWL.CommandLineTool tool ->
            WorkflowConversion.composeWorkflowProtocolFromToolDescription(filePath, tool, ?workflowName = workflowName, ?runName = runName)
        | CWL.Workflow wf -> 
            WorkflowConversion.composeWorkflowProtocolFromWorkflow(filePath, wf, ?workflowName = workflowName, ?runName = runName)
        | CWL.ExpressionTool et ->
            WorkflowConversion.composeWorkflowProtocolFromExpressionToolDescription(filePath, et, ?workflowName = workflowName, ?runName = runName)

    static member decomposeWorkflowProtocolToProcessingUnit (protocol : LDNode, ?resolveRunReferences: bool, ?context : LDContext, ?graph : LDGraph) =
        let shouldResolveRunReferences = defaultArg resolveRunReferences true
        match LDComputationalWorkflow.getAdditionalTypeAsString(protocol, ?context = context) with
        | s when s = WorkflowConversion.workflowDescriptionTypeName ->
            WorkflowConversion.decomposeWorkflowProtocolToWorkflow(protocol, resolveRunReferences = shouldResolveRunReferences, ?context = context, ?graph = graph)
            |> CWL.Workflow
        | s when s = WorkflowConversion.expressionToolDescriptionTypeName ->
            WorkflowConversion.decomposeWorkflowProtocolToExpressionToolDescription(protocol, ?context = context, ?graph = graph)
            |> CWL.ExpressionTool
        | s ->
            WorkflowConversion.decomposeWorkflowProtocolToToolDescription(protocol, ?context = context, ?graph = graph)
            |> CWL.CommandLineTool              

    static member composeWorkflow (workflow : ArcWorkflow, ?fs : FileSystem) =
        let workflowProtocol =
            let workflowFilePath = Identifier.Workflow.cwlFileNameFromIdentifier workflow.Identifier
            match workflow.CWLDescription with
            | Some pu -> WorkflowConversion.composeWorkflowProtocolFromProcessingUnit(workflowFilePath, pu, workflowName = workflow.Identifier)
            | None -> failwithf "Workflow %s must have a CWL description" workflow.Identifier
        let publisher = LDOrganization.create("DataPLANT")
        let creators =
            workflow.Contacts
            |> ResizeArray.map (fun c -> PersonConversion.composePerson c)
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
        LDDataset.createARCWorkflow(
            identifier = workflow.Identifier,
            mainEntities = ResizeArray.singleton workflowProtocol,
            ?name = workflow.Title,
            ?description = workflow.Description,
            ?creators = creators,
            ?hasParts = hasParts,
            ?comments = comments
        )

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
