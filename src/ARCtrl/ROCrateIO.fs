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
    module ROCrate = 
       
        let metadataFileDescriptor =
            let id = "ro-crate-metadata.json"
            let schemaType = ResizeArray ["http://schema.org/CreativeWork"]
            let node = LDNode(id, schemaType)
            node.SetProperty("http://purl.org/dc/terms/conformsTo", LDRef("https://w3id.org/ro/crate/1.1"))
            node.SetProperty("http://schema.org/about", LDRef("./"))
            node

        let encoder (isa : ArcInvestigation) =         
            let isa = isa.ToROCrateInvestigation()
            let graph = isa.Flatten()
            let context = LDContext(baseContexts=ResizeArray[Context.initV1_1();Context.initBioschemasContext()])
            graph.SetContext(context)
            graph.AddNode(metadataFileDescriptor)
            graph.Compact_InPlace()
            LDGraph.encoder graph

        let decoder : Decoder<ArcInvestigation option> =
            LDGraph.decoder
            |> Decode.map (fun graph ->
                match graph.TryGetNode("./") with
                | Some node ->
                    let isa = ArcInvestigation.fromROCrateInvestigation(node, graph = graph, ?context = graph.TryGetContext())
                    Some isa
                | None -> None
            )