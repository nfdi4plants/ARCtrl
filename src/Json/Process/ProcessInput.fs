namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

/// Functions for handling the ProcessInput Type
module ProcessInput =

    module ISAJson =

        let encoder idMap (value : ProcessInput) = 
            match value with
            | ProcessInput.Source s-> 
                Source.ISAJson.encoder idMap s
            | ProcessInput.Sample s -> 
                Sample.ISAJson.encoder idMap s
            | ProcessInput.Data d -> 
                Data.ISAJson.encoder idMap d
            | ProcessInput.Material m -> 
                Material.ISAJson.encoder idMap m

        let decoder: Decoder<ProcessInput> =
            Decode.oneOf [
                Decode.map ProcessInput.Source Source.ISAJson.decoder
                Decode.map ProcessInput.Sample Sample.ISAJson.decoder
                Decode.map ProcessInput.Data Data.ISAJson.decoder
                Decode.map ProcessInput.Material Material.ISAJson.decoder          
            ]
