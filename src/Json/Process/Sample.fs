namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Sample =

    let genID (s:Sample) : string = 
        match s.ID with
        | Some id -> id
        | None -> match s.Name with
                    | Some n -> "#Sample_" + n.Replace(" ","_")
                    | None -> "#EmptySample"

    module ISAJson =

        let encoder (idMap : IDTable.IDTableWrite option) (oa : Sample) = 
            let f (oa : Sample) =
                [
                    Encode.tryInclude "@id" Encode.string (oa |> genID |> Some)
                    Encode.tryInclude "name" Encode.string oa.Name
                    Encode.tryIncludeListOpt "characteristics" (MaterialAttributeValue.ISAJson.encoder idMap) oa.Characteristics
                    Encode.tryIncludeListOpt "factorValues" (FactorValue.ISAJson.encoder idMap) oa.FactorValues
                    Encode.tryIncludeListOpt "derivesFrom" (Source.ISAJson.encoder idMap) oa.DerivesFrom
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f oa
            | Some idMap -> IDTable.encode genID f oa idMap


        let allowedFields = ["@id";"name";"characteristics";"factorValues";"derivesFrom";"@type"; "@context"]

        let decoder: Decoder<Sample> =       
            Decode.objectNoAdditionalProperties allowedFields (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Name = get.Optional.Field "name" Decode.string
                    Characteristics = get.Optional.Field "characteristics" (Decode.list MaterialAttributeValue.ISAJson.decoder)
                    FactorValues = get.Optional.Field "factorValues" (Decode.list FactorValue.ISAJson.decoder)
                    DerivesFrom = get.Optional.Field "derivesFrom" (Decode.list Source.ISAJson.decoder)
                }
            )