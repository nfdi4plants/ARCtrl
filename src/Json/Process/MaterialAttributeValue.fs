namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO

module MaterialAttributeValue =

    module ROCrate =

        let encoder : MaterialAttributeValue -> Json= 
            PropertyValue.ROCrate.encoder

        let decoder : Decoder<MaterialAttributeValue> =
            PropertyValue.ROCrate.decoder<MaterialAttributeValue> (MaterialAttributeValue.createAsPV)

    module ISAJson =
        
        let genID (oa : MaterialAttributeValue) = 
            PropertyValue.ROCrate.genID oa

        let encoder (idMap : IDTable.IDTableWrite option) (oa : MaterialAttributeValue) = 
            let f (oa : MaterialAttributeValue) =
                [
                    Encode.tryInclude "@id" Encode.string (oa |> genID |> Some)
                    Encode.tryInclude "category" (MaterialAttribute.ISAJson.encoder idMap) oa.Category
                    Encode.tryInclude "value" (Value.ISAJson.encoder idMap) oa.Value
                    Encode.tryInclude "unit" (OntologyAnnotation.ISAJson.encoder idMap) oa.Unit
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f oa
            | Some idMap -> IDTable.encode genID f oa idMap

        let decoder : Decoder<MaterialAttributeValue> =
            Decode.object (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Category = get.Optional.Field "category" MaterialAttribute.ISAJson.decoder
                    Value = get.Optional.Field "value" Value.ISAJson.decoder
                    Unit = get.Optional.Field "unit" OntologyAnnotation.ISAJson.decoder
                }
            )

[<AutoOpen>]
module MaterialAttributeValueExtensions =
    
    type MaterialAttributeValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString MaterialAttributeValue.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:MaterialAttributeValue) ->
                MaterialAttributeValue.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            MaterialAttributeValue.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString MaterialAttributeValue.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:MaterialAttributeValue) ->
                MaterialAttributeValue.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) =
            MaterialAttributeValue.toROCrateJsonString(?spaces=spaces) this