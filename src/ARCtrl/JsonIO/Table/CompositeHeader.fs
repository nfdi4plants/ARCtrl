namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module CompositeHeaderExtensions =

    type CompositeHeader with

        static member fromJsonString (s:string)  = 
            Decode.fromJsonString CompositeHeader.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:CompositeHeader) ->
                CompositeHeader.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToJsonString(?spaces) =
            CompositeHeader.toJsonString(?spaces=spaces) this