namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA

module IOType =
  let encoder (io:IOType) = Encode.string <| io.ToString()

  let decoder : Decoder<IOType> = Decode.string |> Decode.andThen (
      fun s -> IOType.ofString s |> Decode.succeed
    )

[<AutoOpen>]
module IOTypeExtensions =

    type IOType with
        static member fromJsonString (jsonString: string) : IOType = 
            GDecode.fromJsonString IOType.decoder jsonString

        member this.ToJsonString(?spaces) : string =
            let spaces = defaultArg spaces 0
            GEncode.toJsonString spaces (IOType.encoder this)

        static member toJsonString(a:IOType) = a.ToJsonString()