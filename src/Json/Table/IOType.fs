namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl

module IOType =
  let encoder (io:IOType) = Encode.string <| io.ToString()

  let decoder : Decoder<IOType> = Decode.string |> Decode.andThen (
      fun s -> IOType.ofString s |> Decode.succeed
    )


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