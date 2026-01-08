namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.FileSystem
open ARCtrl.ROCrate
open ARCtrl.Helper
open ARCtrl.Conversion

module ARC =

    [<Literal>]
    let SubstituteLicenseID = "#LICENSE"

    /// Functions for serializing and deserializing ARC objects to RO-Crate Root Data Entity
    ///
    /// See https://www.researchobject.org/ro-crate/1.1/root-data-entity.html for more information
    type ROCrate =
            
        static member metadataFileDescriptor =
            let id = "ro-crate-metadata.json"
            let schemaType = ResizeArray ["http://schema.org/CreativeWork"]
            let node = LDNode(id, schemaType)
            node.SetProperty("http://purl.org/dc/terms/conformsTo", LDRef("https://w3id.org/ro/crate/1.2"))
            node.SetProperty("http://schema.org/about", LDRef("./"))
            node

        static member metadataFileDescriptorWfRun =
            let id = "ro-crate-metadata.json"
            let schemaType = ResizeArray ["http://schema.org/CreativeWork"]
            let node = LDNode(id, schemaType)
            node.SetProperty("http://purl.org/dc/terms/conformsTo", [LDRef("https://w3id.org/ro/crate/1.1");LDRef("https://w3id.org/workflowhub/workflow-ro-crate/1.0")])
            node.SetProperty("http://schema.org/about", LDRef("./"))
            node

        static member createLicenseNode (license : License option) =
            match license with
                | Some license ->
                    let text = 
                        match license.Type with
                        | LicenseContentType.Fulltext -> license.Content
                    LDCreativeWork.create(license.Path, text = text)
                | None ->
                    LDCreativeWork.create(SubstituteLicenseID, text = ARCtrl.FileSystem.DefaultLicense.dl)

        static member getLicense (license : LDNode, ?context : LDContext) =
            let text = LDCreativeWork.tryGetTextAsString(license, ?context = context)
            match license.Id, text with
            | SubstituteLicenseID, None 
            | SubstituteLicenseID, Some ARCtrl.FileSystem.DefaultLicense.dl -> None
            | SubstituteLicenseID, Some text -> Some (License(contentType = Fulltext,content = text))
            | path, Some text -> Some (License(contentType = Fulltext,content = text, path = path))
            | path, None -> Some (License(contentType = Fulltext,content = "", path = path))

        static member encoder (isa : ArcInvestigation, ?license : License, ?fs : FileSystem) =
            let license = ROCrate.createLicenseNode(license)
            let isa = isa.ToROCrateInvestigation(?fs = fs)
            LDDataset.setSDDatePublishedAsDateTime(isa, System.DateTime.Now)
            LDDataset.setLicenseAsCreativeWork(isa, license)
            let graph = isa.Flatten()
            let context = LDContext(baseContexts=ResizeArray[Context.initV1_2();Context.initBioschemasContext()])
            graph.SetContext(context)
            graph.AddNode(ROCrate.metadataFileDescriptor)
            graph.Compact_InPlace()
            LDGraph.encoder graph

        /// Returns ArcInvestigation, list of file Ids, and optional License
        static member decoder : Decoder<ArcInvestigation*string ResizeArray * License option> =
            LDGraph.decoder
            |> Decode.map (fun graph ->
                match graph.TryGetNode("./") with
                | Some node ->
                    let files =
                        graph.Nodes
                        |> ResizeArray.choose (fun n ->
                            if LDFile.validate(n, ?context = graph.TryGetContext()) && n.Id.Contains "#" |> not && n.HasType(LDDataset.schemaType, ?context = graph.TryGetContext()) |> not then
                                Some n.Id
                            else
                                None
                        )
                    let license =
                        LDDataset.tryGetLicenseAsCreativeWork(node, graph = graph, ?context = graph.TryGetContext())
                        |> Option.bind (fun n -> ROCrate.getLicense(n, ?context = graph.TryGetContext()))

                    ArcInvestigation.fromROCrateInvestigation(node, graph = graph, ?context = graph.TryGetContext()),
                    files,
                    license
                | None ->
                    failwith "RO-Crate graph did not contain root data Entity"
            )

        static member packDatasetAsCrate (dataset : LDNode, datasetName:string, datasetDescription:string, metadataFileDescriptor: option<LDNode>, license : option<License>) =
            let metadataFileDescriptor = defaultArg metadataFileDescriptor ROCrate.metadataFileDescriptor
            let license = ROCrate.createLicenseNode(license)
            // Create the main entity node with all properties from the dataset, but with @id "./"
            let mainEntity = LDDataset.create(id = "./", name = datasetName, description = datasetDescription)
            dataset.GetProperties(false)
            |> Seq.iter (fun kv ->
                printfn "%A" kv
                mainEntity.SetProperty(kv.Key, kv.Value)
            )
            // Set additional properties
            LDDataset.setSDDatePublishedAsDateTime(mainEntity, System.DateTime.Now)
            LDDataset.setLicenseAsCreativeWork(mainEntity, license)
            // LDDataset.setHasParts(mainEntity,(dataset.GetPropertyValues"http://schema.org/mainEntity") |> Seq.map (fun x -> x :?> LDNode) |> ResizeArray)
            // Flatten the dataset into a graph
            let graph = mainEntity.Flatten()
            // Add context 
            let context = LDContext(baseContexts=ResizeArray[Context.initV1_2();Context.initBioschemasContext()])
            graph.SetContext(context)
            // Add file descriptor
            graph.AddNode(metadataFileDescriptor)
            // Compact nodes according to context (e.g. "https://schema.org/CreativeWork" -> "CreativeWork")
            graph.Compact_InPlace()
            graph

        static member writeRunAsCrate(arcRun:ArcRun, fileSystem: FileSystem, datasetName: string, datasetDescription: string, ?license : License) =
            let runDataset = arcRun.ToROCrateRun(fs = fileSystem)
            ROCrate.packDatasetAsCrate(runDataset, datasetName, datasetDescription, Some ROCrate.metadataFileDescriptorWfRun,license)
            |> LDGraph.toROCrateJsonString(2)

        static member writeWorkflowAsCrate(arcWorkflow:ArcWorkflow, fileSystem: FileSystem, datasetName: string, datasetDescription: string, ?license : License) =
            let workflowDataset = arcWorkflow.ToROCrateWorkflow(fs = fileSystem)
            ROCrate.packDatasetAsCrate(workflowDataset, datasetName, datasetDescription, Some ROCrate.metadataFileDescriptorWfRun, license)
            |> LDGraph.toROCrateJsonString(2)
            
        static member decoderDeprecated : Decoder<ArcInvestigation> =
            LDNode.decoder
            |> Decode.map (fun ldnode ->
                ldnode
                |> LDDataset.getAbouts
                |> Seq.exactlyOne
                |> fun node -> ArcInvestigation.fromROCrateInvestigation(node, context = Context.initV1_1())
            )