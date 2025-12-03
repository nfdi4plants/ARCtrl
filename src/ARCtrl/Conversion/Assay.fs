namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns



type AssayConversion =

    static member getDataFilesFromProcesses (processes : LDNode ResizeArray, ?fragmentDescriptors : LDNode ResizeArray, ?graph : LDGraph, ?context : LDContext) =
        let dataFromFragmentDescriptors =
            fragmentDescriptors
            |> Option.defaultValue (ResizeArray [])
            |> ResizeArray.choose (fun df -> LDPropertyValue.tryGetSubjectOf(df, ?graph = graph, ?context = context))
        let data = 
            processes
            |> ResizeArray.collect (fun p -> 
                let inputs = LDLabProcess.getObjectsAsData(p, ?graph = graph, ?context = context)
                let outputs = LDLabProcess.getResultsAsData(p, ?graph = graph, ?context = context)
                ResizeArray.append inputs outputs
            )
            |> ResizeArray.append dataFromFragmentDescriptors
            |> ResizeArray.distinct
        let files =
            data
            |> ResizeArray.filter (fun d -> 
                DataAux.pathAndSelectorFromName d.Id |> snd |> Option.isNone
            )
        let filesFromfragments = 
            data
            |> ResizeArray.filter (fun d -> 
                DataAux.pathAndSelectorFromName d.Id |> snd |> Option.isSome
            )
            |> ResizeArray.groupBy (fun d ->
                DataAux.pathAndSelectorFromName d.Id |> fst
            )
            |> ResizeArray.map (fun (path,fragments) ->
                let file =
                    match files |> ResizeArray.tryFind (fun d -> d.Id = path) with
                    | Some f -> f
                    | None ->
                        let comments = 
                            LDFile.getComments(fragments.[0], ?graph = graph, ?context = context)
                            |> Option.fromSeq
                        LDFile.create(
                            id = path,
                            name = path,
                            ?comments = comments,
                            ?disambiguatingDescription = LDFile.tryGetDisambiguatingDescriptionAsString(fragments.[0], ?context = context),
                            ?encodingFormat = LDFile.tryGetEncodingFormatAsString(fragments.[0], ?context = context),
                            ?context = fragments.[0].TryGetContext()
                        )
                LDDataset.setHasParts(file, fragments,?context = context)
                file            
            )
        ResizeArray.append files filesFromfragments

    static member composeAssay (assay : ArcAssay, ?fs : FileSystem) =
        let measurementMethod = assay.TechnologyType |> Option.map BaseTypes.composeDefinedTerm
        let measurementTechnique = assay.TechnologyPlatform |> Option.map BaseTypes.composeDefinedTerm
        let variableMeasured = assay.MeasurementType |> Option.map BaseTypes.composePropertyValueFromOA
        let creators = 
            assay.Performers
            |> ResizeArray.map (fun c -> PersonConversion.composePerson c)
            |> Option.fromSeq
        let processSequence = 
            ArcTables(assay.Tables).GetProcesses(assayName = assay.Identifier, ?fs = fs)
            |> ResizeArray
            |> Option.fromSeq
        let fragmentDescriptors =
            assay.Datamap
            |> Option.map DatamapConversion.composeFragmentDescriptors
        let dataFiles = 
            processSequence
            |> Option.map (fun ps -> AssayConversion.getDataFilesFromProcesses(ps, ?fragmentDescriptors = fragmentDescriptors))
        let variableMeasureds =
            match variableMeasured, fragmentDescriptors with
            | Some vm, Some fds -> ResizeArray.appendSingleton vm fds |> Some
            | Some vm, None -> ResizeArray.singleton vm |> Some
            | None, Some fds -> fds |> Some
            | None, None -> None
        let comments = 
            assay.Comments
            |> ResizeArray.map (fun c -> BaseTypes.composeComment c)
            |> Option.fromSeq
        LDDataset.createAssay(
            identifier = assay.Identifier,
            ?name = assay.Title,
            ?description = assay.Description, 
            ?creators = creators,
            ?hasParts = dataFiles,
            ?measurementMethod = measurementMethod,
            ?measurementTechnique = measurementTechnique,
            ?variableMeasureds = variableMeasureds,
            ?abouts = processSequence,
            ?comments = comments
        )

    static member decomposeAssay (assay : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let measurementMethod = 
            LDDataset.tryGetMeasurementMethodAsDefinedTerm(assay, ?graph = graph, ?context = context)
            |> Option.map (fun m -> BaseTypes.decomposeDefinedTerm(m, ?context = context))
        let measurementTechnique = 
            LDDataset.tryGetMeasurementTechniqueAsDefinedTerm(assay, ?graph = graph, ?context = context)
            |> Option.map (fun m -> BaseTypes.decomposeDefinedTerm(m, ?context = context))
        let variableMeasured = 
            LDDataset.tryGetVariableMeasuredAsMeasurementType(assay, ?graph = graph, ?context = context)
            |> Option.map (fun v -> BaseTypes.decomposePropertyValueToOA(v, ?context = context))
        let perfomers = 
            LDDataset.getCreators(assay, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> PersonConversion.decomposePerson(c, ?graph = graph, ?context = context))
        let datamap = 
            LDDataset.getVariableMeasuredAsFragmentDescriptors(assay, ?graph = graph, ?context = context)
            |> fun fds -> DatamapConversion.decomposeFragmentDescriptors(fds, ?graph = graph, ?context = context)
            |> Option.fromValueWithDefault (Datamap.init())
        let tables = 
            LDDataset.getAboutsAsLabProcess(assay, ?graph = graph, ?context = context)
            |> fun ps -> ArcTables.fromProcesses(List.ofSeq ps, ?graph = graph, ?context = context)
        let comments =
            LDDataset.getComments(assay, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> BaseTypes.decomposeComment(c, ?context = context))
        ArcAssay.create(
            identifier = LDDataset.getIdentifierAsString(assay, ?context = context),
            ?title = LDDataset.tryGetNameAsString(assay, ?context = context),
            ?description = LDDataset.tryGetDescriptionAsString(assay, ?context = context),
            ?measurementType = variableMeasured,
            ?technologyType = measurementMethod,
            ?technologyPlatform = measurementTechnique,
            tables = tables.Tables,
            ?datamap = datamap,
            performers = perfomers,
            comments = comments
        )