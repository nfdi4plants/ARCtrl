namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.FileSystem
open ARCtrl.ROCrate
open ARCtrl.Helper
open ARCtrl.Conversion

module ARC =
    
    /// Functions for serializing and deserializing ARC objects to RO-Crate Root Data Entity
    ///
    /// See https://www.researchobject.org/ro-crate/1.1/root-data-entity.html for more information
    type ROCrate =
            
        static member metadataFileDescriptor =
            let id = "ro-crate-metadata.json"
            let schemaType = ResizeArray ["http://schema.org/CreativeWork"]
            let node = LDNode(id, schemaType)
            node.SetProperty("http://purl.org/dc/terms/conformsTo", LDRef("https://w3id.org/ro/crate/1.1"))
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
                    LDCreativeWork.create("#LICENSE", text = ARCtrl.FileSystem.DefaultLicense.dl)

        static member getLicense (license : LDNode, ?context : LDContext) =
            let text = LDCreativeWork.tryGetTextAsString(license, ?context = context)
            match license.Id, text with
            | "#LICENSE", None 
            | "#LICENSE", Some ARCtrl.FileSystem.DefaultLicense.dl -> None
            | "#LICENSE", Some text -> Some (License(contentType = Fulltext,content = text))
            | path, Some text -> Some (License(contentType = Fulltext,content = text, path = path))
            | path, None -> Some (License(contentType = Fulltext,content = "", path = path))

        static member encoder (isa : ArcInvestigation, ?license : License, ?fs : FileSystem) =
            let license = ROCrate.createLicenseNode(license)
            let isa = isa.ToROCrateInvestigation(?fs = fs)
            LDDataset.setSDDatePublishedAsDateTime(isa, System.DateTime.Now)
            LDDataset.setLicenseAsCreativeWork(isa, license)
            let graph = isa.Flatten()
            let context = LDContext(baseContexts=ResizeArray[Context.initV1_1();Context.initBioschemasContext()])
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
                        |> Option.bind ROCrate.getLicense

                    ArcInvestigation.fromROCrateInvestigation(node, graph = graph, ?context = graph.TryGetContext()),
                    files,
                    license
                | None ->
                    failwith "RO-Crate graph did not contain root data Entity"
            )

        static member decoderDeprecated : Decoder<ArcInvestigation> =
            LDNode.decoder
            |> Decode.map (fun ldnode ->
                ldnode
                |> LDDataset.getAbouts
                |> Seq.exactlyOne
                |> fun node -> ArcInvestigation.fromROCrateInvestigation(node, context = Context.initV1_1())
            )