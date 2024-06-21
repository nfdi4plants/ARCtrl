﻿namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process


module ProtocolParameter =
    
    module ISAJson = 

        let genID (p : ProtocolParameter) = 
            match p.ParameterName with
            | Some name -> $"#ProtocolParameter/{name}"
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

[<AutoOpen>]
module ProtocolParameterExtensions =
    
    type ProtocolParameter with
    
        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString ProtocolParameter.ISAJson.decoder s   
    
        static member toISAJsonString(?spaces) =
            fun (v:ProtocolParameter) ->
                ProtocolParameter.ISAJson.encoder None v
                |> Encode.toJsonString (Encode.defaultSpaces spaces)
                
        member this.ToISAJsonString(?spaces) =
            ProtocolParameter.toISAJsonString(?spaces=spaces) this