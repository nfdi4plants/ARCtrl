namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO


module FactorValue =
    
    module ROCrate =

        let encoder : FactorValue -> Json= 
            PropertyValue.ROCrate.encoder<FactorValue>

        let decoder : Decoder<FactorValue> =
            PropertyValue.ROCrate.decoder<FactorValue> (FactorValue.createAsPV)

    module ISAJson = 
        
        let genID (fv : FactorValue) = 
            PropertyValue.ROCrate.genID fv

        let encoder (idMap : IDTable.IDTableWrite option) (fv : FactorValue) = 
            let f (fv : FactorValue) =
                [
                    // Is this required for ISA-JSON? The FactorValue type has an @id field
                    Encode.tryInclude "@id" Encode.string (fv |> genID |> Some)
                    Encode.tryInclude "category" (Factor.ISAJson.encoder idMap) fv.Category
                    Encode.tryInclude "value" (Value.ISAJson.encoder idMap) fv.Value
                    Encode.tryInclude "unit" (OntologyAnnotation.ISAJson.encoder idMap) fv.Unit
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f fv
            | Some idMap -> IDTable.encode genID f fv idMap

        let decoder: Decoder<FactorValue> =
            Decode.object (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Category = get.Optional.Field "category" Factor.ISAJson.decoder
                    Value = get.Optional.Field "value" Value.ISAJson.decoder
                    Unit = get.Optional.Field "unit" OntologyAnnotation.ISAJson.decoder
                }
            )

[<AutoOpen>]
module FactorValueExtensions =
    
    type FactorValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString FactorValue.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:FactorValue) ->
                FactorValue.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            FactorValue.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this