namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Protocol =   
    
    module ISAJson =

        let encoder (oa : Protocol) = 
            [
                Encode.tryInclude "@id" Encode.string (oa.ID)
                Encode.tryInclude "name" Encode.string (oa.Name)
                Encode.tryInclude "protocolType" OntologyAnnotation.ISAJson.encoder (oa.ProtocolType)
                Encode.tryInclude "description" Encode.string (oa.Description)
                Encode.tryInclude "uri" Encode.string (oa.Uri)
                Encode.tryInclude "version" Encode.string (oa.Version)
                Encode.tryIncludeListOpt "parameters" ProtocolParameter.ISAJson.encoder oa.Parameters
                Encode.tryIncludeListOpt "components" Component.ISAJson.encoder oa.Components
                Encode.tryIncludeListOpt "comments" Comment.ISAJson.encoder oa.Comments
            ]
            |> Encode.choose
            |> Encode.object

        let decoder: Decoder<Protocol> =
            Decode.object (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Name = get.Optional.Field "name" Decode.string
                    ProtocolType = get.Optional.Field "protocolType" OntologyAnnotation.ISAJson.decoder
                    Description = get.Optional.Field "description" Decode.string
                    Uri = get.Optional.Field "uri" Decode.uri
                    Version = get.Optional.Field "version" Decode.string
                    Parameters = get.Optional.Field "parameters" (Decode.list ProtocolParameter.ISAJson.decoder)
                    Components = get.Optional.Field "components" (Decode.list Component.ISAJson.decoder)
                    Comments = get.Optional.Field "comments" (Decode.list Comment.ISAJson.decoder)
                }
            )

    //let genID (studyName:string Option) (assayName:string Option) (processName:string Option) (p:Protocol): string = 
    //    match p.ID with
    //    | Some id -> URI.toString id 
    //    | None -> match p.Uri with
    //              | Some u -> u
    //              | None -> match p.Name with
    //                        | Some n -> "#Protocol_" + n.Replace(" ","_")
    //                        | None -> match (studyName,assayName,processName) with
    //                                  | (Some sn, Some an, Some pn) -> "#Protocol_" + sn.Replace(" ","_") + "_" + an.Replace(" ","_") + "_" + pn.Replace(" ","_")
    //                                  | (Some sn, None, Some pn) -> "#Protocol_" + sn.Replace(" ","_") + "_" + pn.Replace(" ","_")
    //                                  | (None, None, Some pn) -> "#Protocol_" + pn.Replace(" ","_")
    //                                  | _ -> "#EmptyProtocol" 

    //let encoder (options : ConverterOptions) (studyName:string Option) (assayName:string Option) (processName:string Option) (oa : Protocol) = 
    //    [
    //        if options.SetID then 
    //            "@id", Encode.string (genID studyName assayName processName oa)
    //        else 
    //            Encode.tryInclude "@id" Encode.string (oa.ID)
    //        if options.IsJsonLD then 
    //            "@type", (Encode.list [Encode.string "Protocol"])
    //        Encode.tryInclude "name" Encode.string (oa.Name)
    //        Encode.tryInclude "protocolType" (OntologyAnnotation.encoder options) (oa.ProtocolType)
    //        Encode.tryInclude "description" Encode.string (oa.Description)
    //        Encode.tryInclude "uri" Encode.string (oa.Uri)
    //        Encode.tryInclude "version" Encode.string (oa.Version)
    //        if not options.IsJsonLD then 
    //            Encode.tryIncludeList "parameters" (ProtocolParameter.encoder options) (oa.Parameters)
    //        Encode.tryIncludeList "components" (Component.encoder options) (oa.Components)
    //        Encode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
    //        if options.IsJsonLD then 
    //            "@context", ROCrateContext.Protocol.context_jsonvalue
    //    ]
    //    |> Encode.choose
    //    |> Encode.object

    //let decoder (options : ConverterOptions) : Decoder<Protocol> =
    //    Decode.object (fun get ->
    //        {
    //            ID = get.Optional.Field "@id" GDecode.uri
    //            Name = get.Optional.Field "name" Decode.string
    //            ProtocolType = get.Optional.Field "protocolType" (OntologyAnnotation.decoder options)
    //            Description = get.Optional.Field "description" Decode.string
    //            Uri = get.Optional.Field "uri" GDecode.uri
    //            Version = get.Optional.Field "version" Decode.string
    //            Parameters = get.Optional.Field "parameters" (Decode.list (ProtocolParameter.decoder options))
    //            Components = get.Optional.Field "components" (Decode.list (Component.decoder options))
    //            Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
    //        }
    //    )

[<AutoOpen>]
module ProtocolExtensions =
    
    type Protocol with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Protocol.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Protocol) ->
                Protocol.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)