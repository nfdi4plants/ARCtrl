namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO

module MaterialAttributeValue =

    module ISAJson =
        
        let genID (mv : MaterialAttributeValue) = 
            match mv.Category,mv.Value,mv.Unit with
            | Some t, Some v, Some u -> $"#Characteristic_{t.NameText}={v.Text}{u.NameText}"
            | Some t, Some v, None-> $"#Characteristic_{t.NameText}={v.Text}"
            | _ -> $"#EmptyCharacteristic"

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
