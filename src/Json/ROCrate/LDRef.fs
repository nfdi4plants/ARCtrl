namespace ARCtrl.Json

open ARCtrl
open System
open ARCtrl.ROCrate
open Thoth.Json.Core
open DynamicObj

module LDRef =

    let decoder : Decoder<LDRef> =
        Decode.object (fun decoders -> 
            let id = decoders.Required.Field "@id" Decode.string
            LDRef(id)
        )

    let encoder (r: LDRef) =
        [
            "@id", Encode.string r.Id
        ]
        |> Encode.object

