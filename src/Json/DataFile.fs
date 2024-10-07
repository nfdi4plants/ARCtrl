namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO

module DataFile = 

    module ROCrate =

        let encoder (value : DataFile) = 
            match value with
            | DataFile.RawDataFile -> 
                Encode.string "Raw Data File"
            | DataFile.DerivedDataFile  -> 
                Encode.string "Derived Data File"
            | DataFile.ImageFile  -> 
                Encode.string "Image File"

        let decoder : Decoder<DataFile> =
            { new Decoder<DataFile> with
                member this.Decode (s,json) = 
                    match Decode.string.Decode(s,json) with
                    | Ok "Raw Data File" -> 
                        Ok DataFile.RawDataFile
                    | Ok "Derived Data File" -> 
                        Ok DataFile.DerivedDataFile
                    | Ok "Image File" -> 
                        Ok DataFile.ImageFile
                    | Ok s -> Error (DecoderError($"Could not parse {s}.", ErrorReason.BadPrimitive(s,json)))
                    | Error e -> Error e
            }

    module ISAJson =

        let encoder = ROCrate.encoder

        let decoder = ROCrate.decoder

