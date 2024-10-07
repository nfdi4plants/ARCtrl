namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl

module IOType =
  let encoder (io:IOType) = Encode.string <| io.ToString()

  let decoder : Decoder<IOType> = Decode.string |> Decode.andThen (
      fun s -> IOType.ofString s |> Decode.succeed
    )