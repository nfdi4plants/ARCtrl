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

    static member private tryDynamicString name (value: DynamicObj) =
        DynObj.tryGetTypedPropertyValue<string> name value

    static member private getPathOrLocation (value: DynamicObj) =
        RunConversion.tryDynamicString "path" value
        |> Option.orElse (RunConversion.tryDynamicString "location" value)
        |> Option.defaultValue ""

    static member cwlTypesEqual (left : CWL.CWLType) (right : CWL.CWLType) =
        match left, right with
        | CWL.CWLType.File _, CWL.CWLType.File _
        | CWL.CWLType.Directory _, CWL.CWLType.Directory _ -> true
        | CWL.CWLType.Array left, CWL.CWLType.Array right ->
            RunConversion.cwlTypesEqual left.Items right.Items
        | CWL.CWLType.Union left, CWL.CWLType.Union right ->
            left.Count = right.Count &&
            Seq.forall2 RunConversion.cwlTypesEqual left right
        | CWL.CWLType.Record left, CWL.CWLType.Record right ->
            match left.Fields, right.Fields with
            | None, None -> true
            | Some leftFields, Some rightFields ->
                leftFields.Count = rightFields.Count &&
                Seq.forall2 (fun (leftField: CWL.InputRecordField) (rightField: CWL.InputRecordField) ->
                    leftField.Name = rightField.Name &&
                    RunConversion.cwlTypesEqual leftField.Type rightField.Type
                ) leftFields rightFields
            | _ -> false
        | _ -> left = right

    /// Helper function to format CWLType for display in error messages
    static member formatCWLType (type_ : CWL.CWLType) =
        CWL.Encode.encodeCWLType type_
        |> CWL.Encode.writeYaml
        |> fun s -> s.Trim()

    /// Helper function to check if a CWLType is or contains an Array type
    static member isArrayType (type_ : CWL.CWLType) =
        match type_ with
        | CWL.CWLType.Array _ -> true
        | CWL.CWLType.Union types -> types |> Seq.exists (function CWL.CWLType.Array _ -> true | _ -> false)
        | _ -> false

    static member tryGetArrayItemType (type_ : CWL.CWLType) =
        match type_ with
        | CWL.CWLType.Array schema -> Some schema.Items
        | CWL.CWLType.Union types ->
            types
            |> Seq.tryPick (function
                | CWL.CWLType.Array schema -> Some schema.Items
                | _ -> None)
        | _ -> None

    static member tryGetNonNullUnionType (type_ : CWL.CWLType) =
        match type_ with
        | CWL.CWLType.Union types ->
            types
            |> Seq.tryFind (function
                | CWL.CWLType.Null -> false
                | _ -> true)
        | _ -> Some type_

    static member private tryParseInt64 (value: obj) =
        let tryConvert (convert: unit -> int64) =
            try Some (convert()) with _ -> None
        match value with
        | :? int as value -> Some (int64 value)
        | :? int64 as value -> Some value
        | :? decimal as value ->
            tryConvert (fun () ->
                let converted = int64 value
                if decimal converted = value then converted
                else failwith "Expected an integral decimal value."
            )
        | :? float as value ->
            tryConvert (fun () ->
                let converted = int64 value
                if float converted = value then converted
                else failwith "Expected an integral floating-point value."
            )
        | :? string as value ->
            match System.Int64.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture) with
            | true, parsed -> Some parsed
            | false, _ -> None
        | _ -> None

    static member private tryParseFloat (value: obj) =
        match value with
        | :? float as value -> Some value
        | :? decimal as value -> Some (float value)
        | :? int as value -> Some (float value)
        | :? int64 as value -> Some (float value)
        | :? string as value ->
            match System.Double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) with
            | true, parsed -> Some parsed
            | false, _ -> None
        | _ -> None

    static member private tryGetRecordFieldType (name: string) (schema: CWL.InputRecordSchema) =
        schema.Fields
        |> Option.bind (fun fields ->
            fields
            |> Seq.tryFind (fun field -> field.Name = name)
            |> Option.map (fun field -> field.Type)
        )

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
                |> RunConversion.getPathOrLocation
                |> fun path -> RunConversion.composeCWLInputFilePath(path, runName)
            let encodingFormat = RunConversion.tryDynamicString "format" file
            LDFile.create(path, ?encodingFormat = encodingFormat, ?context = context) :> obj
        | CWL.CWLParameterValue.Directory directory ->
            let path =
                directory
                |> RunConversion.getPathOrLocation
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
            if not (RunConversion.cwlTypesEqual inputValue.Type.Value type_) then
                let typeStr = RunConversion.formatCWLType inputValue.Type.Value
                let paramTypeStr = RunConversion.formatCWLType type_
                failwith $"Type ({typeStr}) of yml input value \"{inputValue.Key}\" does not match type of workflow input parameter ({paramTypeStr})."
        match type_ with
        | CWL.CWLType.File _ when inputValue.Values.Count = 1 ->
            let path = RunConversion.composeCWLInputFilePath(inputValue.Values[0], runName)
            let file = LDFile.createCWLParameter(path, exampleOfWork = exampleOfWork)
            match inputValue.Value with
            | Some (CWL.CWLParameterValue.File fileValue) ->
                RunConversion.tryDynamicString "format" fileValue
                |> Option.iter (fun format -> LDFile.setEncodingFormatAsString(file, format))
            | _ -> ()
            file
        | _ when RunConversion.isArrayType type_ ->
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
        let expectedType = expectedType |> Option.bind RunConversion.tryGetNonNullUnionType

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
            let file = CWL.FileInstance()
            match value with
            | :? LDNode as node when LDFile.validate(node) ->
                DynObj.setProperty "class" "File" file
                DynObj.setProperty "path" (RunConversion.decomposeCWLInputFilePath(node.Id, runName)) file
                LDFile.tryGetEncodingFormatAsString node
                |> Option.iter (fun format -> DynObj.setProperty "format" format file)
                CWL.CWLParameterValue.File file
            | :? string as path ->
                DynObj.setProperty "class" "File" file
                DynObj.setProperty "path" (RunConversion.decomposeCWLInputFilePath(path, runName)) file
                CWL.CWLParameterValue.File file
            | _ -> CWL.CWLParameterValue.String (string value)
        | Some (CWL.CWLType.Directory _) ->
            let directory = CWL.DirectoryInstance()
            match value with
            | :? LDNode as node when LDFile.validate(node) ->
                DynObj.setProperty "class" "Directory" directory
                DynObj.setProperty "path" (RunConversion.decomposeCWLInputFilePath(node.Id, runName)) directory
                CWL.CWLParameterValue.Directory directory
            | :? string as path ->
                DynObj.setProperty "class" "Directory" directory
                DynObj.setProperty "path" (RunConversion.decomposeCWLInputFilePath(path, runName)) directory
                CWL.CWLParameterValue.Directory directory
            | _ -> CWL.CWLParameterValue.String (string value)
        | Some CWL.CWLType.String ->
            match value with
            | null -> CWL.CWLParameterValue.Null
            | :? string as value -> CWL.CWLParameterValue.String value
            | _ -> CWL.CWLParameterValue.String (string value)
        | Some CWL.CWLType.Int
        | Some CWL.CWLType.Long ->
            RunConversion.tryParseInt64 value
            |> Option.map CWL.CWLParameterValue.Int
            |> Option.defaultWith (fun () -> CWL.CWLParameterValue.String (string value))
        | Some CWL.CWLType.Float
        | Some CWL.CWLType.Double ->
            RunConversion.tryParseFloat value
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
                    let expectedFieldType = RunConversion.tryGetRecordFieldType kvp.Key recordSchema
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
                let file = CWL.FileInstance()
                DynObj.setProperty "class" "File" file
                DynObj.setProperty "path" (RunConversion.decomposeCWLInputFilePath(node.Id, runName)) file
                LDFile.tryGetEncodingFormatAsString node
                |> Option.iter (fun format -> DynObj.setProperty "format" format file)
                CWL.CWLParameterValue.File file
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
        let creators = 
            run.Performers
            |> ResizeArray.map (fun c -> PersonConversion.composePerson c)
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
