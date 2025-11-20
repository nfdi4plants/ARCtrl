namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns



type RunConversion = 

    /// File paths in CWL files are relative to the file itself. In RO-Crate, we use relative paths from the root of the crate.
    ///
    /// This function replaces the relative paths in the CWL input file with paths relative to the root of the crate.
    static member composeCWLInputFilePath (path : string, runName : string) =
        if path.StartsWith("../..") then
            path.Replace("../../","").Replace("../..","")
        else
            ArcPathHelper.combineMany [| "runs"; runName ; path|]

    static member decomposeCWLInputFilePath (path : string, runName : string) =
        let prefix = ArcPathHelper.combineMany [| "runs"; runName|]
        if path.StartsWith(prefix) then
            path.Replace(prefix, "").TrimStart('/')
        else
            ArcPathHelper.combine "../.." path

    static member composeCWLInputValue (inputValue : CWL.CWLParameterReference, exampleOfWork : LDNode, inputParam : CWL.CWLInput, runName : string) =
        if inputParam.Type_.IsNone then
            failwith $"Cannot convert param values \"{inputValue.Values}\" as Input parameter \"{inputParam.Name}\" has no type."
        let type_ = inputParam.Type_.Value
        if inputValue.Type.IsSome then
            if inputValue.Type.Value <> type_ then
                failwith $"Type ({inputValue.Type.Value.ToString()}) of yml input value \"{inputValue.Key}\" does not match type of workflow input parameter ({type_.ToString()})."
        match type_ with
        | CWL.CWLType.File _ when inputValue.Values.Count = 1 ->
            let path = RunConversion.composeCWLInputFilePath(inputValue.Values[0], runName)
            LDFile.createCWLParameter(path, exampleOfWork = exampleOfWork)
        | _ when type_.ToString().ToLower().Contains("array") ->
            let separator =
                inputParam.InputBinding
                |> Option.bind (fun ib -> ib.ItemSeparator)
                |> Option.defaultValue ","
            let values = String.concat separator inputValue.Values
            LDPropertyValue.createCWLParameter(
                exampleOfWork,
                inputValue.Key,
                ResizeArray.singleton values
            )
        | _ ->
            LDPropertyValue.createCWLParameter(
                exampleOfWork,
                inputValue.Key,
                inputValue.Values
            )

    static member decomposeCWLInputValue (inputValue : LDNode, runName : string, ?context : LDContext, ?graph : LDGraph) =
        let exampleOfWork =
            match LDFile.tryGetExampleOfWorkAsFormalParameter(inputValue, ?graph = graph, ?context = context) with
            | Some eow -> eow
            | None -> failwithf "Input value %s of run %s must have an exampleOfWork property pointing to a CWL formal parameter." inputValue.Id runName
        let key = LDFormalParameter.getNameAsString(exampleOfWork, ?context = context)
        if LDFile.validateCWLParameter(inputValue, ?context = context) then
            let path = RunConversion.decomposeCWLInputFilePath(inputValue.Id, runName)
            CWL.CWLParameterReference(
                key = key,
                values = ResizeArray [path],
                type_ = CWL.CWLType.file()
            )
        else if LDPropertyValue.validateCWLParameter(inputValue, ?context = context) then
            let values = LDPropertyValue.getValuesAsString(inputValue, ?context = context)
            CWL.CWLParameterReference(
                key = key,
                values = values
            )
        else
            failwithf "Input value %s of run %s is neither a CWL File nor a CWL Parameter." inputValue.Id runName


    static member composeWorkflowInvocationFromArcRun (run : ArcRun, ?fs : FileSystem) =
        let workflowProtocol =
            let workflowFilePath = Identifier.Run.cwlFileNameFromIdentifier run.Identifier
            match run.CWLDescription with
            | Some pu -> WorkflowConversion.composeWorkflowProtocolFromProcessingUnit(workflowFilePath, pu, runName = run.Identifier)
            | None -> failwithf "Run %s must have a CWL description" run.Identifier
        let inputParams =
            LDComputationalWorkflow.getInputsAsFormalParameters(workflowProtocol, ?context = workflowProtocol.TryGetContext())
            |> ResizeArray.zip (WorkflowConversion.getInputParametersFromProcessingUnit run.CWLDescription.Value)
            |> ResizeArray.map (fun (i, ldI) ->
                let name = LDFormalParameter.getNameAsString(ldI, ?context = workflowProtocol.TryGetContext())
                let paramRef =
                    run.CWLInput
                    |> Seq.tryPick (fun i ->
                        if i.Key = name then Some i
                        else None
                    )
                match paramRef with
                | Some pr ->
                    RunConversion.composeCWLInputValue(pr, exampleOfWork = ldI, inputParam = i, runName = run.Identifier)
                | None ->
                    failwith $"Could not create workflow invocation for run \"{run.Identifier}\": Workflow parameter \"name\" had no assigned value."
            )
        let processSequence =
            ArcTables(run.Tables).GetProcesses(?fs = fs)
            |> ResizeArray
        let mainInvocation =
            LDWorkflowInvocation.create(
                name = run.Identifier,
                instrument = workflowProtocol,
                objects = inputParams,
                executesLabProtocol = workflowProtocol
            )
        if processSequence.Count = 0 then
            mainInvocation
            |> ResizeArray.singleton
        else
            processSequence
            |> ResizeArray.map (fun p ->
                let id = p.Id.Replace("Process", $"WorkflowInvocation_{run.Identifier}")
                let name = LDLabProcess.getNameAsString(p, ?context = p.TryGetContext())
                let inputs = LDLabProcess.getObjects(p) // |> ResizeArray.append inputParams // Merge process from isa and cwl?
                let results = LDLabProcess.getResults(p) |> Option.fromSeq
                let protocol = LDLabProcess.tryGetExecutesLabProtocol(p) // |> Option.defaultValue workflowProtocol
                let parameterValues = LDLabProcess.getParameterValues(p) |> Option.fromSeq
                let agents = LDLabProcess.tryGetAgent(p) |> Option.map ResizeArray.singleton
                let disambiguatingDescriptions = LDLabProcess.getDisambiguatingDescriptionsAsString(p) |> Option.fromSeq
                LDWorkflowInvocation.create(
                    name = name,
                    id = id,
                    instrument = workflowProtocol,
                    ?executesLabProtocol = protocol,
                    objects = inputs,
                    ?results = results,
                    ?parameterValues = parameterValues,
                    ?agents = agents,
                    ?disambiguatingDescriptions = disambiguatingDescriptions
                )
            )
            |> ResizeArray.appendSingleton mainInvocation

    static member decomposeMainWorkflowInvocation (workflowInvocation : LDNode, runName : string, ?context : LDContext, ?graph : LDGraph) : CWL.CWLProcessingUnit*CWL.CWLParameterReference ResizeArray=
        let cwlDescription =
            match LDLabProcess.tryGetExecutesLabProtocol(workflowInvocation, ?graph = graph, ?context = context) with
            | Some wn ->
                WorkflowConversion.decomposeWorkflowProtocolToProcessingUnit(wn, ?context = context, ?graph = graph)
            | None -> failwith $"Could not decompose workflow invocation for run \"{runName}\": Workflow parameter \"name\" had no assigned value."
        let parameterRefs =
            LDLabProcess.getObjects(workflowInvocation, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun iv ->
                RunConversion.decomposeCWLInputValue(iv, runName = runName, ?context = context, ?graph = graph)
            )
        cwlDescription, parameterRefs

    static member composeRun (run : ArcRun, ?fs : FileSystem) =
        let workflowInvocations =
            RunConversion.composeWorkflowInvocationFromArcRun(run, ?fs = fs)
            |> Option.fromSeq
        let measurementMethod = run.TechnologyType |> Option.map BaseTypes.composeDefinedTerm
        let measurementTechnique = run.TechnologyPlatform |> Option.map BaseTypes.composeDefinedTerm
        let variableMeasured = run.MeasurementType |> Option.map BaseTypes.composePropertyValueFromOA
        let creators = 
            run.Performers
            |> ResizeArray.map (fun c -> PersonConversion.composePerson c)
            |> Option.fromSeq
        let fragmentDescriptors =
            run.DataMap
            |> Option.map DatamapConversion.composeFragmentDescriptors
        let dataFiles = 
            workflowInvocations
            |> Option.map (fun ps -> AssayConversion.getDataFilesFromProcesses(ps, ?fragmentDescriptors = fragmentDescriptors))
        let variableMeasureds =
            match variableMeasured, fragmentDescriptors with
            | Some vm, Some fds -> ResizeArray.appendSingleton vm fds |> Some
            | Some vm, None -> ResizeArray.singleton vm |> Some
            | None, Some fds -> fds |> Some
            | None, None -> None
        let comments = 
            run.Comments
            |> ResizeArray.map (fun c -> BaseTypes.composeComment c)
            |> Option.fromSeq
        LDDataset.createARCRun(
            identifier = run.Identifier,
            ?name = run.Title,
            ?description = run.Description, 
            ?creators = creators,
            ?hasParts = dataFiles,
            ?measurementMethod = measurementMethod,
            ?measurementTechnique = measurementTechnique,
            ?variableMeasureds = variableMeasureds,
            ?abouts = workflowInvocations,
            ?mentions = workflowInvocations,
            ?comments = comments
        )

    static member decomposeRun (run : LDNode, ?graph : LDGraph, ?context : LDContext) : ArcRun=
        let mainWorkflowInvocation =
            LDDataset.getAboutsAsWorkflowInvocation(run, ?graph = graph, ?context = context)
            |> Seq.find (fun wi ->
                LDLabProcess.getObjects(wi, ?graph = graph, ?context = context)
                |> Seq.exists (fun i -> LDFile.tryGetExampleOfWorkAsFormalParameter(i, ?graph = graph, ?context = context).IsSome)
            )
        let cwlDescription, parameterRefs =
            RunConversion.decomposeMainWorkflowInvocation(mainWorkflowInvocation, LDDataset.getIdentifierAsString(run, ?context = context), ?context = context, ?graph = graph)
        let measurementMethod = 
            LDDataset.tryGetMeasurementMethodAsDefinedTerm(run, ?graph = graph, ?context = context)
            |> Option.map (fun m -> BaseTypes.decomposeDefinedTerm(m, ?context = context))
        let measurementTechnique = 
            LDDataset.tryGetMeasurementTechniqueAsDefinedTerm(run, ?graph = graph, ?context = context)
            |> Option.map (fun m -> BaseTypes.decomposeDefinedTerm(m, ?context = context))
        let variableMeasured = 
            LDDataset.tryGetVariableMeasuredAsMeasurementType(run, ?graph = graph, ?context = context)
            |> Option.map (fun v -> BaseTypes.decomposePropertyValueToOA(v, ?context = context))
        let contacts =
            LDDataset.getCreators(run, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> PersonConversion.decomposePerson(c, ?graph = graph, ?context = context))
        let comments =
            LDDataset.getComments(run, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> BaseTypes.decomposeComment(c, ?context = context))
        let dataMap = 
            LDDataset.getVariableMeasuredAsFragmentDescriptors(run, ?graph = graph, ?context = context)
            |> fun fds -> DatamapConversion.decomposeFragmentDescriptors(fds, ?graph = graph, ?context = context)
            |> Option.fromValueWithDefault (DataMap.init())
        let tables = 
            LDDataset.getAboutsAsLabProcess(run, ?graph = graph, ?context = context)
            |> ResizeArray.filter (fun wi -> wi.Id <> mainWorkflowInvocation.Id)
            |> fun ps -> ArcTables.fromProcesses(List.ofSeq ps, ?graph = graph, ?context = context)
        ArcRun.create(
            identifier = LDDataset.getIdentifierAsString(run, ?context = context),
            ?title = LDDataset.tryGetNameAsString(run, ?context = context),
            ?description = LDDataset.tryGetDescriptionAsString(run, ?context = context),
            cwlDescription = cwlDescription,
            cwlInput = parameterRefs,
            ?measurementType = variableMeasured,
            ?technologyType = measurementMethod,
            ?technologyPlatform = measurementTechnique,
            ?datamap = dataMap,
            performers = contacts,
            tables = tables.Tables,
            comments = comments
        )
