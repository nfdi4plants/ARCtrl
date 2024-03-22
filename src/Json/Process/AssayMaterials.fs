namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO

module AssayMaterials = 

    module ISAJson = 

        let encoder (ps : Process list) = 
            let samples = ProcessSequence.getSamples ps
            let materials = ProcessSequence.getMaterials ps
            [
                Encode.tryIncludeList "samples" Sample.ISAJson.encoder samples
                Encode.tryIncludeList "otherMaterials" Material.ISAJson.encoder materials
            ]
           
            |> Encode.choose
            |> Encode.object
    
        // No decoder, as this is a write-only operation

        //let allowedFields = ["samples";"otherMaterials"]

        //let decoder (options : ConverterOptions) : Decoder<AssayMaterials> =
        //    Decode.object allowedFields (fun get ->
        //        {
        //            Samples = get.Optional.Field "samples" (Decode.list (Sample.decoder options))
        //            OtherMaterials = get.Optional.Field "otherMaterials" (Decode.list (Material.decoder options))
        //        }
        //    )