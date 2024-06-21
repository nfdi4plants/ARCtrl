namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module ProcessParameterValue =
    
    module ROCrate =

        let encoder : ProcessParameterValue -> Json= 
            PropertyValue.ROCrate.encoder<ProcessParameterValue>

        let decoder : Decoder<ProcessParameterValue> =
            PropertyValue.ROCrate.decoder<ProcessParameterValue> (ProcessParameterValue.createAsPV)

    module ISAJson =

        let genID (oa : ProcessParameterValue) = 
            failwith "Not implemented"

        let encoder (idMap : IDTable.IDTableWrite option) (oa : ProcessParameterValue) = 
            let f (oa : ProcessParameterValue) =
                [
                    Encode.tryInclude "category" (ProtocolParameter.ISAJson.encoder idMap) oa.Category
                    Encode.tryInclude "value" (Value.ISAJson.encoder idMap) oa.Value
                    Encode.tryInclude "unit" (OntologyAnnotation.ISAJson.encoder idMap) oa.Unit
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f oa
            | Some idMap -> IDTable.encode genID f oa idMap

        let decoder : Decoder<ProcessParameterValue> =
            Decode.object (fun get ->
                {
                    Category = get.Optional.Field "category" ProtocolParameter.ISAJson.decoder
                    Value = get.Optional.Field "value" Value.ISAJson.decoder
                    Unit = get.Optional.Field "unit" OntologyAnnotation.ISAJson.decoder
                }
            )

[<AutoOpen>]
module ProcessParameterValueExtensions =
    
    type ProcessParameterValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString ProcessParameterValue.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:ProcessParameterValue) ->
                ProcessParameterValue.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            ProcessParameterValue.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this

        static member fromROCrateJsonString (s:string) =
            Decode.fromJsonString ProcessParameterValue.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:ProcessParameterValue) ->
                ProcessParameterValue.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) =
            ProcessParameterValue.toROCrateJsonString(?spaces=spaces) this