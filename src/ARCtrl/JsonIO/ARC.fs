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
            let schemaType = ResizeArray ["https://schema.org/CreativeWork"]
            let node = LDNode(id, schemaType)
            node.SetProperty("https://schema.org/conformsTo", LDRef("https://w3id.org/ro/crate/1.1"))
            node.SetProperty("https://schema.org/about", LDRef("./"))
            node

        let encoder (isa : ArcInvestigation) =         
            let isa = isa.ToROCrateInvestigation()
            let graph = isa.Flatten()
            graph.AddNode(metadataFileDescriptor)
            graph.Compact_InPlace()
            LDGraph.encoder graph

        let decoder : Decoder<ArcInvestigation option> = 
            Decode.object (fun get ->
                let isa = get.Optional.Field "about" Investigation.ROCrate.decoder
                isa
            )