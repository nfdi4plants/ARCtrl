namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module IOTypeExtensions =

    type IOType with

        static member fromJsonString (s:string)  = 
            Decode.fromJsonString IOType.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:IOType) ->
                IOType.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToJsonString(?spaces) =
            IOType.toJsonString(?spaces=spaces) this