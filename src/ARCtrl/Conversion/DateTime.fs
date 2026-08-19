namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

module DateTime =


    let dateModifiedKey = "dateModified"

    let tryFromString (s : string) =
        try Json.Decode.fromJsonString Json.Decode.datetime s |> Some
        with _ -> None

    let toString (d : System.DateTime) =
        Json.Encode.dateTime d
        |> Json.Encode.toJsonString 0

    let compose (s : string) =
        match tryFromString s with
        | Some d -> box d
        | None -> box s

    let tryDecompose (d : obj) =
        match d with
        | :? System.DateTime as dt -> Some (toString dt)
        | :? string as s -> Some s
        | _ -> None