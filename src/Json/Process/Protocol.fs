namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl

module Protocol =   
    
    let genID (studyName:string Option) (assayName:string Option) (processName:string Option) (p:Protocol): string = 
        match p.ID with
        | Some id -> URI.toString id 
        | None -> match p.Uri with
                  | Some u -> u
                  | None -> match p.Name with
                            | Some n -> "#Protocol_" + n.Replace(" ","_")
                            | None -> match (studyName,assayName,processName) with
                                      | (Some sn, Some an, Some pn) -> "#Protocol_" + sn.Replace(" ","_") + "_" + an.Replace(" ","_") + "_" + pn.Replace(" ","_")
                                      | (Some sn, None, Some pn) -> "#Protocol_" + sn.Replace(" ","_") + "_" + pn.Replace(" ","_")
                                      | (None, None, Some pn) -> "#Protocol_" + pn.Replace(" ","_")
                                      | _ -> "#EmptyProtocol" 

    let encoder (options : ConverterOptions) (studyName:string Option) (assayName:string Option) (processName:string Option) (oa : Protocol) = 
        [
            if options.SetID then 
                "@id", Encode.string (genID studyName assayName processName oa)
            else 
                Encode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [Encode.string "Protocol"])
            Encode.tryInclude "name" Encode.string (oa.Name)
            Encode.tryInclude "protocolType" (OntologyAnnotation.encoder options) (oa.ProtocolType)
            Encode.tryInclude "description" Encode.string (oa.Description)
            Encode.tryInclude "uri" Encode.string (oa.Uri)
            Encode.tryInclude "version" Encode.string (oa.Version)
            if not options.IsJsonLD then 
                Encode.tryIncludeList "parameters" (ProtocolParameter.encoder options) (oa.Parameters)
            Encode.tryIncludeList "components" (Component.encoder options) (oa.Components)
            Encode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
            if options.IsJsonLD then 
                "@context", ROCrateContext.Protocol.context_jsonvalue
        ]
        |> Encode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<Protocol> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "name" Decode.string
                ProtocolType = get.Optional.Field "protocolType" (OntologyAnnotation.decoder options)
                Description = get.Optional.Field "description" Decode.string
                Uri = get.Optional.Field "uri" GDecode.uri
                Version = get.Optional.Field "version" Decode.string
                Parameters = get.Optional.Field "parameters" (Decode.list (ProtocolParameter.decoder options))
                Components = get.Optional.Field "components" (Decode.list (Component.decoder options))
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
            }
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (p:Protocol) = 
        encoder (ConverterOptions()) None None None p
        |> Encode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (p:Protocol) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None None None p
        |> Encode.toJsonString 2

    let toJsonldStringWithContext (a:Protocol) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None None None a
        |> Encode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Protocol) = 
    //    File.WriteAllText(path,toString p)