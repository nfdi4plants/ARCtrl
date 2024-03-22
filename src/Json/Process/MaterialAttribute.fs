namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module MaterialAttribute = 
    
    module ISAJson = 

        let encoder (value : MaterialAttribute) = 
            [
                Encode.tryInclude "characteristicType" OntologyAnnotation.ISAJson.encoder value.CharacteristicType
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<MaterialAttribute> =
            Decode.object (fun get ->
                {         
                    ID = None
                    CharacteristicType = get.Optional.Field "characteristicType" (OntologyAnnotation.ISAJson.decoder)
                }
            )

[<AutoOpen>]
module MaterialAttributeExtensions =
    
        type MaterialAttribute with
    
            static member fromISAJsonString (s:string) = 
                Decode.fromJsonString MaterialAttribute.ISAJson.decoder s   
    
            static member toISAJsonString(?spaces) =
                fun (v:MaterialAttribute) ->
                    MaterialAttribute.ISAJson.encoder v
                    |> Encode.toJsonString (Encode.defaultSpaces spaces)
