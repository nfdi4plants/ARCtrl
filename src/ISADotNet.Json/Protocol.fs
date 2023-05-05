namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode
module ProtocolParameter =

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            tryInclude "@id" GEncode.string (oa |> tryGetPropertyValue "ID")
            tryInclude "name" GEncode.string (oa |> tryGetPropertyValue "Name")
            tryInclude "factorType" (OntologyAnnotation.encoder options) (oa |> tryGetPropertyValue "FactorType")
            tryInclude "comments" GEncode.string (oa |> tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<Factor> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "name" Decode.string
                FactorType = get.Optional.Field "factorType" (OntologyAnnotation.decoder options)
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))               
            }
        )

    let fromString (s:string) = 
        JsonSerializer.Deserialize<ProtocolParameter>(s,JsonExtensions.options)

    let toString (p:ProtocolParameter) = 
        JsonSerializer.Serialize<ProtocolParameter>(p,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:ProtocolParameter) = 
        File.WriteAllText(path,toString p)

module Protocol =  
  
    let fromString (s:string) = 
        JsonSerializer.Deserialize<Protocol>(s,JsonExtensions.options)

    let toString (p:Protocol) = 
        JsonSerializer.Serialize<Protocol>(p,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:Protocol) = 
        File.WriteAllText(path,toString p)