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
