namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module MaterialAttribute = 
    
    let genID (m : MaterialAttribute) = 
        match m.CharacteristicType with
        | Some mType -> 
            $"#MaterialAttribute/{OntologyAnnotation.genID mType}"
        | None -> "#EmptyFactor"

    module ISAJson = 

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
