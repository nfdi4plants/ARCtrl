namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Sample =

    module ROCrate = 

        let genID (s:Sample) : string = 
            match s.ID with
            | Some id -> id
            | None -> match s.Name with
                      | Some n -> "#Sample_" + n.Replace(" ","_")
                      | None -> "#EmptySample"
    
        let encoder (oa : Sample) = 
            [
                "@id", Encode.string (oa |> genID)
                "@type", (Encode.list [ Encode.string "Sample"])
                Encode.tryInclude "name" Encode.string (oa.Name)
                Encode.tryIncludeList "characteristics" MaterialAttributeValue.ROCrate.encoder (oa.Characteristics)
                Encode.tryIncludeList "factorValues" FactorValue.ROCrate.encoder (oa.FactorValues)
                "@context", ROCrateContext.Sample.context_jsonvalue
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<Sample> =       
            Decode.object (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Name = get.Optional.Field "name" Decode.string
                    Characteristics = get.Optional.Field "characteristics" (Decode.list MaterialAttributeValue.ROCrate.decoder)
                    FactorValues = get.Optional.Field "factorValues" (Decode.list FactorValue.ROCrate.decoder)
                    DerivesFrom = get.Optional.Field "derivesFrom" (Decode.list Source.ROCrate.decoder)
                }
            )

    module ISAJson =
    
        let encoder (oa : Sample) = 
            [
                Encode.tryInclude "@id" Encode.string (oa.ID)
                Encode.tryInclude "name" Encode.string (oa.Name)
                Encode.tryIncludeListOpt "characteristics" MaterialAttributeValue.ISAJson.encoder oa.Characteristics
                Encode.tryIncludeListOpt "factorValues" FactorValue.ISAJson.encoder oa.FactorValues
                Encode.tryIncludeListOpt "derivesFrom" Source.ISAJson.encoder oa.DerivesFrom
            ]
            |> Encode.choose
            |> Encode.object

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

[<AutoOpen>]
module SampleExtensions =
    
    type Sample with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Sample.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Sample) ->
                Sample.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromROCrateString (s:string) =
            Decode.fromJsonString Sample.ROCrate.decoder s

        static member toROCrateString(?spaces) =
            fun (f:Sample) ->
                Sample.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)
