namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module MaterialAttribute = 
    
    module ISAJson = 

        let genID (m : MaterialAttribute) = 
            match m.CharacteristicType with
            | Some mType -> 
                $"#MaterialAttribute/{OntologyAnnotation.ROCrate.genID mType}"
            | None -> "#EmptyFactor"

        let encoder (idMap : IDTable.IDTableWrite option) (value : MaterialAttribute) = 
            let f (value : MaterialAttribute) =
                [
                    Encode.tryInclude "@id" Encode.string (value |> genID |> Some)
                    Encode.tryInclude "characteristicType" (OntologyAnnotation.ISAJson.encoder idMap) value.CharacteristicType
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f value
            | Some idMap -> IDTable.encode genID f value idMap

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
    
            static member toISAJsonString(?spaces, ?useIDReferencing) =
                let useIDReferencing = Option.defaultValue false useIDReferencing
                let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
               
                fun (v:MaterialAttribute) ->
                    MaterialAttribute.ISAJson.encoder idMap v
                    |> Encode.toJsonString (Encode.defaultSpaces spaces)

            member this.ToJsonString(?spaces, ?useIDReferencing) =
                MaterialAttribute.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing  ) this