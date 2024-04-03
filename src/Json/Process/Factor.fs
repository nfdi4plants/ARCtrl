namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO


module Factor =  

    module ISAJson = 

        let encoder (value : Factor) = 
            [
                Encode.tryInclude "factorName" Encode.string value.Name
                Encode.tryInclude "factorType" OntologyAnnotation.ISAJson.encoder value.FactorType
                Encode.tryIncludeSeq "comments" Comment.ISAJson.encoder (value.Comments |> Option.defaultValue (ResizeArray()))
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<Factor> =
            Decode.object (fun get ->
                {                    
                    Name = get.Optional.Field "factorName" Decode.string
                    FactorType = get.Optional.Field "factorType" (OntologyAnnotation.ISAJson.decoder)
                    Comments = get.Optional.Field "comments" (Decode.resizeArray (Comment.ISAJson.decoder))
                }
            )

[<AutoOpen>]
module FactorExtensions =
    
    type Factor with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Factor.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Factor) ->
                Factor.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)
