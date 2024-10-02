namespace ARCtrl.Json

open ARCtrl
open System.Collections.Generic
open Thoth.Json.Core

[<AutoOpen>]
module ArcTableExtensions =

    type ArcTable with
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString ArcTable.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:ArcTable) ->
                ArcTable.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToJsonString(?spaces) =
            ArcTable.toJsonString(?spaces=spaces) this

        static member fromCompressedJsonString (jsonString: string) : ArcTable = 
            let decoder = 
                Decode.object(fun get ->
                    let stringTable = get.Required.Field "stringTable" (StringTable.decoder)
                    let oaTable = get.Required.Field "oaTable" (OATable.decoder stringTable)
                    let cellTable = get.Required.Field "cellTable" (CellTable.decoder stringTable oaTable)
                    get.Required.Field "table" (ArcTable.decoderCompressed stringTable oaTable cellTable)
                )
            Decode.fromJsonString decoder jsonString

        member this.ToCompressedJsonString(?spaces) : string =
            let spaces = Encode.defaultSpaces spaces
            let stringTable = Dictionary()
            let oaTable = Dictionary()
            let cellTable = Dictionary()
            let arcTable = ArcTable.encoderCompressed stringTable oaTable cellTable this
            let jObject = 
                Encode.object [
                    "cellTable", CellTable.arrayFromMap cellTable |> CellTable.encoder stringTable oaTable
                    "oaTable", OATable.arrayFromMap oaTable |> OATable.encoder stringTable
                    "stringTable", StringTable.arrayFromMap stringTable |> StringTable.encoder
                    "table", arcTable
                ] 
            Encode.toJsonString spaces jObject

        static member toCompressedJsonString(?spaces) = 
            fun (obj:ArcTable) ->
                obj.ToCompressedJsonString(?spaces=spaces)