namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
open Newtonsoft.Json.Linq
#endif

open ISADotNet
open Fable.Core

module GDecode =

    let isURI (s : string) = 
        true
        //s.StartsWith("http://") || s.StartsWith("https://")

    let uri s json : Result<URI,DecoderError>= 
        match Decode.string s json with
        | Ok s when isURI s -> Ok s
        | Ok s -> Error (DecoderError(s,ErrorReason.FailMessage (sprintf "Expected URI, got %s" s)))
        | Error e -> Error e

    let fromString (decoder : Decoder<'a>) (s : string) : 'a = 
        match Decode.fromString decoder s with
        | Ok a -> a
        | Error e -> failwith (sprintf "Error decoding string: %s" e)
   
    let getFieldNames (json : JsonValue) = 
        match json with
        | :? JObject as json -> 
            json.Properties()
            |> Seq.map (fun x -> x.Name)
        | _ -> Seq.empty

    let hasUnknownFields (knownFields : string list) (json : JsonValue) = 
        getFieldNames json
        |> Seq.exists (fun x -> not (knownFields |> Seq.contains x))