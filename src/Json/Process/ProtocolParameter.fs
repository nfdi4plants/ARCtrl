namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper

module ProtocolParameter =

    let encoder (value : ProtocolParameter) = 
        value.ParameterName
        |> Option.defaultValue (OntologyAnnotation())
        |> OntologyAnnotation.encoder

    let decoder : Decoder<ProtocolParameter> =
        OntologyAnnotation.decoder
        |> Decode.map (fun oa ->
            let oa = Option.fromValueWithDefault (OntologyAnnotation()) oa
            ProtocolParameter.create(?ParameterName = oa)
        )

    module ISAJson = 

        let genID (p : ProtocolParameter) = 
            match p.ParameterName with
            | Some name -> $"#ProtocolParameter/{OntologyAnnotation.ROCrate.genID name}"
            | None -> "#EmptyProtocolParameter"

        let encoder (idMap : IDTable.IDTableWrite option) (value : ProtocolParameter) = 
            let f (value : ProtocolParameter) =
                [
                    Encode.tryInclude "@id" Encode.string (value |> genID |> Some)
                    Encode.tryInclude "parameterName" (OntologyAnnotation.ISAJson.encoder idMap) value.ParameterName
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f value
            | Some idMap -> IDTable.encode genID f value idMap 

        let decoder : Decoder<ProtocolParameter> =
            Decode.object (fun get ->
                {         
                    ID = None
                    ParameterName = get.Optional.Field "parameterName" (OntologyAnnotation.ISAJson.decoder)
                }
            )

