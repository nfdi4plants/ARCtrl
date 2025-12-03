namespace ARCtrl.Json

open ARCtrl
open ARCtrl.Helper

[<AutoOpen>]
module DatamapExtensions =

    type Datamap with

        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Datamap.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:Datamap) ->
                Datamap.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToJsonString(?spaces) = 
            Datamap.toJsonString(?spaces=spaces) this

        //static member fromCompressedJsonString (s: string) =
        //    try Decode.fromJsonString (Compression.decode Datamap.decoderCompressed) s with
        //    | e -> failwithf "Error. Unable to parse json string to Datamap: %s" e.Message

        //static member toCompressedJsonString(?spaces) =
        //    fun (obj:Datamap) ->
        //        let spaces = defaultArg spaces 0
        //        Encode.toJsonString spaces (Compression.encode Datamap.encoderCompressed obj)

        //member this.ToCompressedJsonString(?spaces) = 
        //    Datamap.toCompressedJsonString(?spaces=spaces) this
