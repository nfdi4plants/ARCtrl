namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO

module StudyMaterials = 

    module ISAJson = 
        let encoder idMap (ps : Process list) = 
            let source = ProcessSequence.getSources ps
            let samples = ProcessSequence.getSamples ps
            let materials = ProcessSequence.getMaterials ps
            [
                Encode.tryIncludeList "sources" (Source.ISAJson.encoder idMap) source
                Encode.tryIncludeList "samples" (Sample.ISAJson.encoder idMap) samples
                Encode.tryIncludeList "otherMaterials" (Material.ISAJson.encoder idMap) materials
            ]  
            |> Encode.choose
            |> Encode.object 

    // No decoder, as this is a write-only operation

    //let allowedFields = ["sources";"samples";"otherMaterials"]

    //let decoder (options : ConverterOptions) : Decoder<StudyMaterials> =
    //    GDecode.object allowedFields (fun get ->
    //        {
    //            Sources = get.Optional.Field "sources" (Decode.list (Source.decoder options))
    //            Samples = get.Optional.Field "samples" (Decode.list (Sample.decoder options))
    //            OtherMaterials = get.Optional.Field "otherMaterials" (Decode.list (Material.decoder options))
    //        }
    //    )