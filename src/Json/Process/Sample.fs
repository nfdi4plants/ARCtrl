namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Sample =

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

    //let genID (s:Sample) : string = 
    //    match s.ID with
    //    | Some id -> id
    //    | None -> match s.Name with
    //              | Some n -> "#Sample_" + n.Replace(" ","_")
    //              | None -> "#EmptySample"
    
    //let encoder (options : ConverterOptions) (oa : Sample) = 
    //    [
    //        if options.SetID then 
    //            "@id", Encode.string (oa |> genID)
    //        else 
    //            Encode.tryInclude "@id" Encode.string (oa.ID)
    //        if options.IsJsonLD then 
    //            "@type", (Encode.list [ Encode.string "Sample"])
    //        Encode.tryInclude "name" Encode.string (oa.Name)
    //        Encode.tryIncludeList "characteristics" (MaterialAttributeValue.encoder options) (oa.Characteristics)
    //        Encode.tryIncludeList "factorValues" (FactorValue.encoder options) (oa.FactorValues)
    //        if not options.IsJsonLD then 
    //            Encode.tryIncludeList "derivesFrom" (Source.encoder options) (oa.DerivesFrom)
    //        if options.IsJsonLD then
    //            "@context", ROCrateContext.Sample.context_jsonvalue
    //    ]
    //    |> Encode.choose
    //    |> Encode.object

    //let allowedFields = ["@id";"name";"characteristics";"factorValues";"derivesFrom";"@type"; "@context"]

    //let decoder (options : ConverterOptions) : Decoder<Sample> =       
    //    GDecode.object allowedFields (fun get ->
    //        {
    //            ID = get.Optional.Field "@id" GDecode.uri
    //            Name = get.Optional.Field "name" Decode.string
    //            Characteristics = get.Optional.Field "characteristics" (Decode.list (MaterialAttributeValue.decoder options))
    //            FactorValues = get.Optional.Field "factorValues" (Decode.list (FactorValue.decoder options))
    //            DerivesFrom = get.Optional.Field "derivesFrom" (Decode.list (Source.decoder options))
    //        }
    //    )

[<AutoOpen>]
module SampleExtensions =
    
    type Sample with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Sample.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Sample) ->
                Sample.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)