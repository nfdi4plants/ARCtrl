namespace ARCtrl.Json

open ARCtrl
open System
open ARCtrl.ROCrate
open Thoth.Json.Core
open DynamicObj

module LDValue =

    let genericDecoder =
        Decode.oneOf [
            Decode.map box Decode.string
            Decode.map box Decode.int
            Decode.map box Decode.decimal
        ]

    let genericEncoder (value : obj) =
        match value with
        | :? string as s -> Encode.string s
        | :? int as i -> Encode.int i
        | :? bool as b -> Encode.bool b
        | :? float as f -> Encode.float f
        | :? DateTime as d -> Encode.dateTime d
        | _ -> failwith "Unknown type"

    let decoder : Decoder<LDValue> =
        Decode.object (fun decoders -> 
            let value = decoders.Required.Field "@value" genericDecoder
            let valueType = decoders.Optional.Field "@type" Decode.string
            LDValue(value, ?valueType = valueType)
        )

    let encoder (v: LDValue) =
        [
            "@value", genericEncoder v.Value
            "@type", Encode.string v.ValueType
        ]
        |> Encode.object
