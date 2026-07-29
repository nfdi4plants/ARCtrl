namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
open DynamicObj
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

    static member composeCWLParameterValue(value: CWL.CWLParameterValue, runName: string, ?context: LDContext) : obj =
        match value with
        | CWL.CWLParameterValue.File file ->
            let path =
                file
                |> CWL.FileInstance.pathOrLocation
                |> fun path -> RunConversion.composeCWLInputFilePath(path, runName)
            let encodingFormat = file.Format
            LDFile.create(path, ?encodingFormat = encodingFormat, ?context = context) :> obj
        | CWL.CWLParameterValue.Directory directory ->
            let path =
                directory
                |> CWL.DirectoryInstance.pathOrLocation
                |> fun path -> RunConversion.composeCWLInputFilePath(path, runName)
            LDFile.create(path, ?context = context) :> obj
        | CWL.CWLParameterValue.Array values ->
            values
            |> ResizeArray.map (fun value -> RunConversion.composeCWLParameterValue(value, runName, ?context = context))
            :> obj
        | CWL.CWLParameterValue.Record fields ->
            let record = DynamicObj()
            fields
            |> Seq.iter (fun field ->
                DynObj.setProperty field.Name (RunConversion.composeCWLParameterValue(field.Value, runName, ?context = context)) record
            )
            record :> obj
        | CWL.CWLParameterValue.String value -> value :> obj
        | CWL.CWLParameterValue.Int value -> value :> obj
        | CWL.CWLParameterValue.Float value -> value :> obj
        | CWL.CWLParameterValue.Boolean value -> value :> obj
        | CWL.CWLParameterValue.Null -> null

    static member private composeLegacyValue (inputValue: CWL.CWLParameterReference) =
        inputValue.Value
        |> Option.defaultWith (fun () ->
            inputValue.Values
            |> Seq.map CWL.CWLParameterValue.String
            |> ResizeArray
            |> CWL.CWLParameterValue.Array
        )

    static member composeCWLInputValue (inputValue : CWL.CWLParameterReference, exampleOfWork : LDNode, inputParam : CWL.CWLInput, runName : string) =
        if inputParam.Type_.IsNone then
            failwith $"Cannot convert param values \"{inputValue.Values}\" as Input parameter \"{inputParam.Name}\" has no type."
        let type_ = inputParam.Type_.Value
        if inputValue.Type.IsSome then
            if not (CWL.CWLType.typesEqual inputValue.Type.Value type_) then
                let typeStr = CWL.Encode.formatCWLType inputValue.Type.Value
                let paramTypeStr = CWL.Encode.formatCWLType type_
                failwith $"Type ({typeStr}) of yml input value \"{inputValue.Key}\" does not match type of workflow input parameter ({paramTypeStr})."
        match type_ with
        | CWL.CWLType.File _ when inputValue.Values.Count = 1 ->
            let path = RunConversion.composeCWLInputFilePath(inputValue.Values[0], runName)
            let file = LDFile.createCWLParameter(path, exampleOfWork = exampleOfWork)
            match inputValue.Value with
            | Some (CWL.CWLParameterValue.File fileValue) ->
                fileValue.Format
                |> Option.iter (fun format -> LDFile.setEncodingFormatAsString(file, format))
            | _ -> ()
            file
        | _ when CWL.CWLType.isArrayType type_ ->
            LDPropertyValue.createCWLParameter(
                exampleOfWork,
                inputValue.Key,
                ResizeArray()
            )
            |> fun pv ->
                inputValue.Value
                |> Option.defaultValue (RunConversion.composeLegacyValue inputValue)
                |> fun value -> RunConversion.composeCWLParameterValue(value, runName)
                |> fun value -> LDPropertyValue.setValueObjects(pv, value)
                pv
        | _ ->
            let pv =
                LDPropertyValue.createCWLParameter(
                    exampleOfWork,
                    inputValue.Key,
                    ResizeArray()
                )
            let value =
                inputValue.Value
                |> Option.defaultWith (fun () ->
                    match inputValue.Values.Count with
                    | 0 -> CWL.CWLParameterValue.Array (ResizeArray())
                    | 1 -> CWL.CWLParameterValue.String inputValue.Values.[0]
                    | _ -> inputValue.Values |> ResizeArray.map CWL.CWLParameterValue.String |> CWL.CWLParameterValue.Array
                )
                |> fun value -> RunConversion.composeCWLParameterValue(value, runName)
            LDPropertyValue.setValueObjects(pv, value)
            pv

    static member private resolveLDValue(value: obj, ?graph: LDGraph) =
        match value with
        | :? LDRef as ref when graph.IsSome ->
            graph.Value.TryGetNode ref.Id
            |> Option.map (fun node -> node :> obj)
            |> Option.defaultValue value
        | _ -> value

    static member decomposeCWLParameterValue(value: obj, runName: string, ?expectedType: CWL.CWLType, ?graph: LDGraph) : CWL.CWLParameterValue =
        let value = RunConversion.resolveLDValue(value, ?graph = graph)
        let expectedType = expectedType |> Option.bind CWL.CWLType.tryGetNonNullUnionType

        match expectedType with
        | Some (CWL.CWLType.Array arraySchema) ->
            match value with
            | :? string as value ->
                CWL.CWLParameterValue.Array (ResizeArray [CWL.CWLParameterValue.String value])
            | :? System.Collections.IEnumerable as values ->
                values
                |> Seq.cast<obj>
                |> Seq.map (fun value ->
                    RunConversion.decomposeCWLParameterValue(value, runName, expectedType = arraySchema.Items, ?graph = graph)
                )
                |> ResizeArray
                |> CWL.CWLParameterValue.Array
            | _ ->
                CWL.CWLParameterValue.Array (ResizeArray [RunConversion.decomposeCWLParameterValue(value, runName, expectedType = arraySchema.Items, ?graph = graph)])
        | Some (CWL.CWLType.File _) ->
            match value with
            | :? LDNode as node when LDFile.validate(node) ->
                let path = RunConversion.decomposeCWLInputFilePath(node.Id, runName)
                let format = LDFile.tryGetEncodingFormatAsString node
                CWL.CWLParameterValue.File (CWL.FileInstance(path = path, ?format = format))
            | :? string as path ->
                let path = RunConversion.decomposeCWLInputFilePath(path, runName)
                CWL.CWLParameterValue.File (CWL.FileInstance(path = path))
            | _ -> CWL.CWLParameterValue.String (string value)
        | Some (CWL.CWLType.Directory _) ->
            match value with
            | :? LDNode as node when LDFile.validate(node) ->
                let path = RunConversion.decomposeCWLInputFilePath(node.Id, runName)
                CWL.CWLParameterValue.Directory (CWL.DirectoryInstance(path = path))
            | :? string as path ->
                let path = RunConversion.decomposeCWLInputFilePath(path, runName)
                CWL.CWLParameterValue.Directory (CWL.DirectoryInstance(path = path))
            | _ -> CWL.CWLParameterValue.String (string value)
        | Some CWL.CWLType.String ->
            match value with
            | null -> CWL.CWLParameterValue.Null
            | :? string as value -> CWL.CWLParameterValue.String value
            | _ -> CWL.CWLParameterValue.String (string value)
        | Some CWL.CWLType.Int
        | Some CWL.CWLType.Long ->
            CWL.CWLParameterValue.tryParseInt64 value
            |> Option.map CWL.CWLParameterValue.Int
            |> Option.defaultWith (fun () -> CWL.CWLParameterValue.String (string value))
        | Some CWL.CWLType.Float
        | Some CWL.CWLType.Double ->
            CWL.CWLParameterValue.tryParseFloat value
            |> Option.map CWL.CWLParameterValue.Float
            |> Option.defaultWith (fun () -> CWL.CWLParameterValue.String (string value))
        | Some CWL.CWLType.Boolean ->
            match value with
            | :? bool as value -> CWL.CWLParameterValue.Boolean value
            | :? string as value ->
                match System.Boolean.TryParse value with
                | true, parsed -> CWL.CWLParameterValue.Boolean parsed
                | false, _ -> CWL.CWLParameterValue.String value
            | _ -> CWL.CWLParameterValue.String (string value)
        | Some CWL.CWLType.Null ->
            CWL.CWLParameterValue.Null
        | Some (CWL.CWLType.Enum _) ->
            match value with
            | null -> CWL.CWLParameterValue.Null
            | :? string as value -> CWL.CWLParameterValue.String value
            | _ -> CWL.CWLParameterValue.String (string value)
        | Some (CWL.CWLType.Record recordSchema) ->
            match value with
            | :? DynamicObj as record ->
                record.GetProperties(false)
                |> Seq.map (fun kvp ->
                    let expectedFieldType = CWL.InputRecordSchema.tryGetFieldType kvp.Key recordSchema
                    CWL.CWLParameterRecordField(kvp.Key, RunConversion.decomposeCWLParameterValue(kvp.Value, runName, ?expectedType = expectedFieldType, ?graph = graph))
                )
                |> ResizeArray
                |> CWL.CWLParameterValue.Record
            | _ -> CWL.CWLParameterValue.String (string value)
        | _ ->
            match value with
            | null -> CWL.CWLParameterValue.Null
            | :? string as value -> CWL.CWLParameterValue.String value
            | :? int as value -> CWL.CWLParameterValue.Int (int64 value)
            | :? int64 as value -> CWL.CWLParameterValue.Int value
            | :? decimal as value -> CWL.CWLParameterValue.Float (float value)
            | :? float as value -> CWL.CWLParameterValue.Float value
            | :? bool as value -> CWL.CWLParameterValue.Boolean value
            | :? LDNode as node when LDFile.validate(node) ->
                let path = RunConversion.decomposeCWLInputFilePath(node.Id, runName)
                let format = LDFile.tryGetEncodingFormatAsString node
                CWL.CWLParameterValue.File (CWL.FileInstance(path = path, ?format = format))
            | :? System.Collections.IEnumerable as values ->
                values
                |> Seq.cast<obj>
                |> Seq.map (fun value -> RunConversion.decomposeCWLParameterValue(value, runName, ?graph = graph))
                |> ResizeArray
                |> CWL.CWLParameterValue.Array
            | :? DynamicObj as record ->
                record.GetProperties(false)
                |> Seq.map (fun kvp ->
                    CWL.CWLParameterRecordField(kvp.Key, RunConversion.decomposeCWLParameterValue(kvp.Value, runName, ?graph = graph))
                )
                |> ResizeArray
                |> CWL.CWLParameterValue.Record
            | _ -> CWL.CWLParameterValue.String (string value)

    static member private tryGetExpectedTypeFromFormalParameter(exampleOfWork: LDNode, ?context: LDContext, ?graph: LDGraph) =
        try
            let input = WorkflowConversion.decomposeInputFromFormalParameter(exampleOfWork, ?context = context, ?graph = graph)
            input.Type_
        with _ ->
            None

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
            let valueObject = LDPropertyValue.getValueObject(inputValue, ?context = context)
            let expectedType = RunConversion.tryGetExpectedTypeFromFormalParameter(exampleOfWork, ?context = context, ?graph = graph)
            let value = RunConversion.decomposeCWLParameterValue(valueObject, runName, ?expectedType = expectedType, ?graph = graph)
            CWL.CWLParameterReference(
                key = key,
                value = value
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
                    failwith $"Could not create workflow invocation for run \"{run.Identifier}\": Workflow parameter \"{name}\" had no assigned value."
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
        let workflowProtocol =
            let workflowFilePath = Identifier.Run.cwlFileNameFromIdentifier run.Identifier
            match run.CWLDescription with
            | Some pu -> WorkflowConversion.composeWorkflowProtocolFromProcessingUnit(workflowFilePath, pu, runName = run.Identifier)
            | None -> failwithf "Run %s must have a CWL description" run.Identifier
        let workflowInvocations =
            RunConversion.composeWorkflowInvocationFromArcRun(run, ?fs = fs)
            |> Option.fromSeq
        let measurementMethod = run.TechnologyType |> Option.map BaseTypes.composeDefinedTerm
        let measurementTechnique = run.TechnologyPlatform |> Option.map BaseTypes.composeDefinedTerm
        let variableMeasured = run.MeasurementType |> Option.map BaseTypes.composePropertyValueFromOA
        let persons = run.Investigation |> Option.map (fun i -> seq (i.GetAllPersons()))
        let creators = 
            run.Performers
            |> ResizeArray.map (fun c -> PersonConversion.composePerson(c, ?persons = persons))
            |> Option.fromSeq
        let publisher = LDOrganization.create("DataPLANT")
        let dateCreated = System.DateTime.UtcNow
        if creators.IsSome then
            LDComputationalWorkflow.setCreators(workflowProtocol, creators.Value)
        LDComputationalWorkflow.setSdPublisher(workflowProtocol, publisher)
        LDComputationalWorkflow.setDateCreatedAsDateTime(workflowProtocol, dateCreated)
        let fragmentDescriptors =
            run.Datamap
            |> Option.map DatamapConversion.composeFragmentDescriptors
        let dataFiles = 
            workflowInvocations
            |> Option.map (fun ps -> AssayConversion.getDataFilesFromProcesses(ps, ?fragmentDescriptors = fragmentDescriptors))
        let hasParts =
            match dataFiles with
            | Some df -> ResizeArray.appendSingleton workflowProtocol df |> Some
            | None -> ResizeArray.singleton workflowProtocol |> Some
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
            mainEntities = ResizeArray.singleton workflowProtocol,
            ?name = run.Title,
            ?description = run.Description, 
            ?creators = creators,
            ?hasParts = hasParts,
            ?measurementMethod = measurementMethod,
            ?measurementTechnique = measurementTechnique,
            ?variableMeasureds = variableMeasureds,
            ?abouts = workflowInvocations,
            ?mentions = workflowInvocations,
            ?comments = comments
        )

    static member decomposeRun (run : LDNode, ?graph : LDGraph, ?context : LDContext) : ArcRun=
        let workflowProtocol = LDDataset.tryGetMainEntityAsWorkflowProtocol(run, ?graph = graph, ?context = context)
        let mainWorkflowInvocation =
            LDDataset.getAboutsAsWorkflowInvocation(run, ?graph = graph, ?context = context)
            |> Seq.find (fun wi ->
                match LDLabProcess.tryGetExecutesLabProtocol(wi, ?graph = graph, ?context = context) with
                | Some lp when LDWorkflowProtocol.validate(lp, ?context = context) -> true
                | _ -> false
            )
        let cwlDescription, parameterRefs =
            match workflowProtocol with
            | Some wp ->
                WorkflowConversion.decomposeWorkflowProtocolToProcessingUnit(wp, ?context = context, ?graph = graph),
                LDLabProcess.getObjects(mainWorkflowInvocation, ?graph = graph, ?context = context)
                |> ResizeArray.map (fun iv ->
                    RunConversion.decomposeCWLInputValue(iv, runName = LDDataset.getIdentifierAsString(run, ?context = context), ?context = context, ?graph = graph)
                )
            | None ->
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
        let datamap = 
            LDDataset.getVariableMeasuredAsFragmentDescriptors(run, ?graph = graph, ?context = context)
            |> fun fds -> DatamapConversion.decomposeFragmentDescriptors(fds, ?graph = graph, ?context = context)
            |> Option.fromValueWithDefault (Datamap.init())
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
            ?datamap = datamap,
            performers = contacts,
            tables = tables.Tables,
            comments = comments
        )
