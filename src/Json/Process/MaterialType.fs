namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process


module MaterialType =

    module ISAJson = 

        let encoder (options : ConverterOptions) (value : MaterialType) = 
            match value with
            | MaterialType.ExtractName -> 
                Encode.string "Extract Name"
            | MaterialType.LabeledExtractName -> 
                Encode.string "Labeled Extract Name"

        let decoder (options : ConverterOptions) : Decoder<MaterialType> =
            { new Decoder<MaterialType> with
                member this.Decode (s,json) = 
                    match Decode.string.Decode(s,json) with
                    | Ok "Extract Name" -> Ok MaterialType.ExtractName
                    | Ok "Labeled Extract Name" -> Ok MaterialType.LabeledExtractName
                    | Ok s -> Error (DecoderError($"Could not parse {s}No other value than \"Extract Name\" or \"Labeled Extract Name\" allowed for materialtype", ErrorReason.BadPrimitive(s,json)))
                    | Error e -> Error e
        
            }