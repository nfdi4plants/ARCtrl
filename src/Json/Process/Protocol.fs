namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Protocol =   
    
    module ROCrate =

        let genID (studyName:string Option) (assayName:string Option) (processName:string Option) (p:Protocol): string = 
            match p.Uri with
            | Some u -> u
            | None -> 
                match p.Name with
                | Some n -> "#Protocol_" + n.Replace(" ","_")
                | None -> 
                    match (studyName,assayName,processName) with
                    | (Some sn, Some an, Some pn) -> "#Protocol_" + sn.Replace(" ","_") + "_" + an.Replace(" ","_") + "_" + pn.Replace(" ","_")
                    | (Some sn, None, Some pn) -> "#Protocol_" + sn.Replace(" ","_") + "_" + pn.Replace(" ","_")
                    | (None, None, Some pn) -> "#Protocol_" + pn.Replace(" ","_")
                    | _ -> "#EmptyProtocol" 

        let encoder (studyName:string Option) (assayName:string Option) (processName:string Option) (oa : Protocol) = 
            [
                "@id", Encode.string (genID studyName assayName processName oa)
                "@type", (Encode.list [Encode.string "Protocol"])
                Encode.tryInclude "name" Encode.string (oa.Name)
                Encode.tryInclude "protocolType" OntologyAnnotation.ROCrate.encoder (oa.ProtocolType)
                Encode.tryInclude "description" Encode.string (oa.Description)
                Encode.tryInclude "uri" Encode.string (oa.Uri)
                Encode.tryInclude "version" Encode.string (oa.Version)
                Encode.tryIncludeListOpt "components" Component.ROCrate.encoder oa.Components
                Encode.tryIncludeListOpt "comments" Comment.ROCrate.encoder oa.Comments
                "@context", ROCrateContext.Protocol.context_jsonvalue
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<Protocol> =
            Decode.object (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Name = get.Optional.Field "name" Decode.string
                    ProtocolType = get.Optional.Field "protocolType" OntologyAnnotation.ROCrate.decoder
                    Description = get.Optional.Field "description" Decode.string
                    Uri = get.Optional.Field "uri" Decode.uri
                    Parameters = None
                    Version = get.Optional.Field "version" Decode.string
                    Components = get.Optional.Field "components" (Decode.list Component.ROCrate.decoder)
                    Comments = get.Optional.Field "comments" (Decode.list Comment.ROCrate.decoder)
                }
            )

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


[<AutoOpen>]
module ProtocolExtensions =
    
    type Protocol with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Protocol.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Protocol) ->
                Protocol.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromROCrateString (s:string) =
            Decode.fromJsonString (Protocol.ROCrate.decoder) s

        static member toROCrateString (studyName:string Option) (assayName:string Option) (processName:string Option) (?spaces) =
            fun (f:Protocol) ->
                Protocol.ROCrate.encoder studyName assayName processName f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)