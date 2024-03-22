namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Data = 

    module ISAJson =
    
        let encoder (oa : Data) = 
            [
                Encode.tryInclude "@id" Encode.string oa.ID
                Encode.tryInclude "name" Encode.string oa.Name
                Encode.tryInclude "type" DataFile.ISAJson.encoder oa.DataType
                Encode.tryIncludeListOpt "comments" Comment.ISAJson.encoder oa.Comments
            ]
            |> Encode.choose
            |> Encode.object

        let allowedFields = ["@id";"name";"type";"comments";"@type"; "@context"]

        let decoder: Decoder<Data> =
            Decode.objectNoAdditionalProperties allowedFields (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Name = get.Optional.Field "name" Decode.string
                    DataType = get.Optional.Field "type" DataFile.ISAJson.decoder
                    Comments = get.Optional.Field "comments" (Decode.list Comment.ISAJson.decoder)
                }
            )

    //let genID (d:Data) : string = 
    //        match d.ID with
    //        | Some id -> URI.toString id
    //        | None -> match d.Name with
    //                  | Some n -> n.Replace(" ","_")
    //                  | None -> "#EmptyData"
    
    //    let rec encoder (options : ConverterOptions) (oa : Data) = 
    //        [
    //            if options.SetID then 
    //                "@id", Encode.string (oa |> genID)
    //            else 
    //                Encode.tryInclude "@id" Encode.string (oa.ID)
    //            if options.IsJsonLD then 
    //                "@type", (Encode.list [Encode.string "Data"])
    //            Encode.tryInclude "name" Encode.string (oa.Name)
    //            Encode.tryInclude "type" (DataFile.encoder options) (oa.DataType)
    //            Encode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
    //            if options.IsJsonLD then
    //                "@context", ROCrateContext.Data.context_jsonvalue
    //        ]
    //        |> Encode.choose
    //        |> Encode.object

    //    let allowedFields = ["@id";"name";"type";"comments";"@type"; "@context"]

    //    let rec decoder (options : ConverterOptions) : Decoder<Data> =
    //        GDecode.object allowedFields (fun get ->
    //            {
    //                ID = get.Optional.Field "@id" GDecode.uri
    //                Name = get.Optional.Field "name" Decode.string
    //                DataType = get.Optional.Field "type" (DataFile.decoder options)
    //                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
    //            }
            
    //        )

[<AutoOpen>]
module DataExtensions =
    
    type Data with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Data.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Data) ->
                Data.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)