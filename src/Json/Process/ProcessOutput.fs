namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

/// Functions for handling the ProcessOutput Type
module ProcessOutput =

    module ISAJson =

        let encoder idMap (value : ProcessOutput) = 
            match value with
            | ProcessOutput.Sample s -> 
                Sample.ISAJson.encoder idMap s
            | ProcessOutput.Data d -> 
                Data.ISAJson.encoder idMap d
            | ProcessOutput.Material m -> 
                Material.ISAJson.encoder idMap m

        let decoder: Decoder<ProcessOutput> =
            Decode.oneOf [
                Decode.map ProcessOutput.Sample Sample.ISAJson.decoder
                Decode.map ProcessOutput.Data Data.ISAJson.decoder
                Decode.map ProcessOutput.Material Material.ISAJson.decoder
            ]
