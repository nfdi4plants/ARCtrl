namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Source =
    
    module ISAJson = 
    
        let rec encoder (oa : Source) = 
            [
                Encode.tryInclude "@id" Encode.string (oa.ID)
                Encode.tryInclude "name" Encode.string (oa.Name)
                Encode.tryIncludeListOpt "characteristics" MaterialAttributeValue.ISAJson.encoder oa.Characteristics  
            ]
            |> Encode.choose
            |> Encode.object

        let allowedFields = ["@id";"name";"characteristics";"@type"; "@context"]

        let decoder: Decoder<Source> =     
            Decode.objectNoAdditionalProperties allowedFields (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Name = get.Optional.Field "name" Decode.string
                    Characteristics = get.Optional.Field "characteristics" (Decode.list MaterialAttributeValue.ISAJson.decoder)
                } 
            )

    //let genID (s:Source) : string = 
    //    match s.ID with
    //    | Some id -> URI.toString id
    //    | None -> match s.Name with
    //              | Some n -> "#Source_" + n.Replace(" ","_")
    //              | None -> "#EmptySource"
    
    //let rec encoder (options : ConverterOptions) (oa : Source) = 
    //    [
    //        if options.SetID then 
    //            "@id", Encode.string (oa |> genID)
    //        else 
    //            Encode.tryInclude "@id" Encode.string (oa.ID)
    //        if options.IsJsonLD then 
    //            "@type", (Encode.list [ Encode.string "Source"])
    //        Encode.tryInclude "name" Encode.string (oa.Name)
    //        Encode.tryIncludeList "characteristics" (MaterialAttributeValue.encoder options) (oa.Characteristics)      
    //        if options.IsJsonLD then
    //            "@context", ROCrateContext.Source.context_jsonvalue
    //        ]
    //    |> Encode.choose
    //    |> Encode.object

    //let allowedFields = ["@id";"name";"characteristics";"@type"; "@context"]

    //let rec decoder (options : ConverterOptions) : Decoder<Source> =     
    //    GDecode.object allowedFields (fun get ->
            
    //            {
    //                ID = get.Optional.Field "@id" GDecode.uri
    //                Name = get.Optional.Field "name" Decode.string
    //                Characteristics = get.Optional.Field "characteristics" (Decode.list (MaterialAttributeValue.decoder options))
    //            } 
            
    //    )

[<AutoOpen>]
module SourceExtensions =
    
    type Source with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Source.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Source) ->
                Source.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)
