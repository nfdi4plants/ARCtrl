namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.ROCrate
open ARCtrl.Helper
open ARCtrl.Conversion

module ARC =
    
    /// Functions for serializing and deserializing ARC objects to RO-Crate Root Data Entity
    ///
    /// See https://www.researchobject.org/ro-crate/1.1/root-data-entity.html for more information
    type ROCrate =

        static member getDefaultLicense() =
            //let cw = LDNode("License", ResizeArray ["https://schema.org/CreativeWork"])
            //cw.SetProperty("https://schema.org/name", "ALL RIGHTS RESERVED BY THE AUTHORS")
            //cw.SetProperty("https://schema.org/about",LDRef "./")
            //cw
            "ALL RIGHTS RESERVED BY THE AUTHORS"
            
        static member metadataFileDescriptor =
            let id = "ro-crate-metadata.json"
            let schemaType = ResizeArray ["http://schema.org/CreativeWork"]
            let node = LDNode(id, schemaType)
            node.SetProperty("http://purl.org/dc/terms/conformsTo", LDRef("https://w3id.org/ro/crate/1.1"))
            node.SetProperty("http://schema.org/about", LDRef("./"))
            node

        static member encoder (isa : ArcInvestigation, ?license : obj) =
            let license = match license with
                          | Some license -> license
                          | None -> ROCrate.getDefaultLicense()
            let isa = isa.ToROCrateInvestigation()
            LDDataset.setSDDatePublishedAsDateTime(isa, System.DateTime.Now)
            LDDataset.setLicenseAsCreativeWork(isa, license)
            let graph = isa.Flatten()
            let context = LDContext(baseContexts=ResizeArray[Context.initV1_1();Context.initBioschemasContext()])
            graph.SetContext(context)
            graph.AddNode(ROCrate.metadataFileDescriptor)
            graph.Compact_InPlace()
            LDGraph.encoder graph

        static member decoder : Decoder<ArcInvestigation*string ResizeArray> =
            LDGraph.decoder
            |> Decode.map (fun graph ->
                match graph.TryGetNode("./") with
                | Some node ->
                    let files =
                        graph.Nodes
                        |> ResizeArray.choose (fun n ->
                            if LDFile.validate(n, ?context = graph.TryGetContext()) && n.Id.Contains "#" |> not then
                                Some n.Id
                            else
                                None
                        )
                    ArcInvestigation.fromROCrateInvestigation(node, graph = graph, ?context = graph.TryGetContext()),
                    files
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