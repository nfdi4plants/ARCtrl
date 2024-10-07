namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module CompositeCellExtensions =

    type CompositeCell with

        static member fromJsonString (s:string)  = 
            Decode.fromJsonString CompositeCell.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:CompositeCell) ->
                CompositeCell.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToJsonString(?spaces) =
            CompositeCell.toJsonString(?spaces=spaces) this