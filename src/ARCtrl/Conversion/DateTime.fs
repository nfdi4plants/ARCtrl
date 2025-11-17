namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

module DateTime =

    let tryFromString (s : string) =
        try Json.Decode.fromJsonString Json.Decode.datetime s |> Some
        with _ -> None

    let toString (d : System.DateTime) =
        Json.Encode.dateTime d
        |> Json.Encode.toJsonString 0