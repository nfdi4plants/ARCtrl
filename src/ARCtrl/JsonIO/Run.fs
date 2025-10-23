namespace ARCtrl.Json

open ARCtrl
open ARCtrl.Helper

[<AutoOpen>]
module RunExtensions =

    type ArcRun with

        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Run.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:ArcRun) ->
                Run.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToJsonString(?spaces) = 
            ArcRun.toJsonString(?spaces=spaces) this

        static member fromCompressedJsonString (s:string)  =
            try Decode.fromJsonString (Compression.decode Run.decoderCompressed) s with
            | e -> failwithf "Error. Unable to parse json string to ArcRun: %s" e.Message

        static member toCompressedJsonString(?spaces) =
            fun (obj:ArcRun) ->
                let spaces = defaultArg spaces 0
                Encode.toJsonString spaces (Compression.encode Run.encoderCompressed obj)

        member this.ToCompressedJsonString(?spaces) = 
            ArcRun.toCompressedJsonString(?spaces=spaces) this