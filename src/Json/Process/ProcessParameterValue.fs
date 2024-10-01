namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module ProcessParameterValue =
    
    module ROCrate =

        let encoder : ProcessParameterValue -> IEncodable = 
            PropertyValue.ROCrate.encoder

        let decoder : Decoder<ProcessParameterValue> =
            PropertyValue.ROCrate.decoder<ProcessParameterValue> (ProcessParameterValue.createAsPV)

    module ISAJson =

        let genID (oa : ProcessParameterValue) = 
            failwith "Not implemented"

        let encoder (idMap : IDTable.IDTableWrite option) (oa : ProcessParameterValue) = 
            [
                Encode.tryInclude "category" (ProtocolParameter.ISAJson.encoder idMap) oa.Category
                Encode.tryInclude "value" (Value.ISAJson.encoder idMap) oa.Value
                Encode.tryInclude "unit" (OntologyAnnotation.ISAJson.encoder idMap) oa.Unit
            ]
            |> Encode.choose
            |> Encode.object
            

        let decoder : Decoder<ProcessParameterValue> =
            Decode.object (fun get ->
                {
                    Category = get.Optional.Field "category" ProtocolParameter.ISAJson.decoder
                    Value = get.Optional.Field "value" Value.ISAJson.decoder
                    Unit = get.Optional.Field "unit" OntologyAnnotation.ISAJson.decoder
                }
            )