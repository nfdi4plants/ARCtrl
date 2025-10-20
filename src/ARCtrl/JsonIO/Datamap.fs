namespace ARCtrl.Json

open ARCtrl
open ARCtrl.Helper

[<AutoOpen>]
module DatamapExtensions =

    type DataMap with

        static member fromJsonString (s:string)  = 
            Decode.fromJsonString DataMap.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:DataMap) ->
                DataMap.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToJsonString(?spaces) = 
            DataMap.toJsonString(?spaces=spaces) this

        static member fromCompressedJsonString (s: string) =
            try Decode.fromJsonString (Compression.decode DataMap.decoderCompressed) s with
            | e -> failwithf "Error. Unable to parse json string to Datamap: %s" e.Message

        static member toCompressedJsonString(?spaces) =
            fun (obj:DataMap) ->
                let spaces = defaultArg spaces 0
                Encode.toJsonString spaces (Compression.encode DataMap.encoderCompressed obj)

        member this.ToCompressedJsonString(?spaces) = 
            DataMap.toCompressedJsonString(?spaces=spaces) this
