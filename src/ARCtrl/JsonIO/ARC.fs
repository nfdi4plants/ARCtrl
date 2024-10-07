namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Helper

module ARC =
    
    /// Functions for serializing and deserializing ARC objects to RO-Crate Root Data Entity
    ///
    /// See https://www.researchobject.org/ro-crate/1.1/root-data-entity.html for more information
    module ROCrate = 
        
        let encoder (isa : ArcInvestigation) = 
            [
                Encode.tryInclude "@type" Encode.string (Some "CreativeWork")
                Encode.tryInclude "@id" Encode.string (Some "ro-crate-metadata.json")
                Encode.tryInclude "about" Investigation.ROCrate.encoder (Some isa)
                "conformsTo", ROCrateContext.ROCrate.conformsTo_jsonvalue |> Some
                "@context", ROCrateContext.ROCrate.context_jsonvalue |> Some
                ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<ArcInvestigation option> = 
            Decode.object (fun get ->
                let isa = get.Optional.Field "about" Investigation.ROCrate.decoder
                isa
            )