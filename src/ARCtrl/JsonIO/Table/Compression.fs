namespace ARCtrl.Json


open Thoth.Json.Core
open ARCtrl
open ARCtrl.Helper
open ARCtrl.Process
open System.Collections.Generic

/// This module is used to generalize json compression helpers
module Compression =

    let encode (encoder: Dictionary<string, int> -> Dictionary<OntologyAnnotation, int> -> Dictionary<CompositeCell, int> -> 'A -> IEncodable) (obj: 'A) = 
        let stringTable = Dictionary.init()
        let oaTable = Dictionary.init()
        let cellTable = Dictionary.init()
        let object = encoder stringTable oaTable cellTable obj
        object |> Encode.toJsonString 0 |> ignore
        let encodedCellTable = CellTable.arrayFromMap cellTable |> CellTable.encoder stringTable oaTable
        let encodedOATable = OATable.arrayFromMap oaTable |> OATable.encoder stringTable
        let encodedStringTable = StringTable.arrayFromMap stringTable |> StringTable.encoder

        Encode.object [
            "cellTable", encodedCellTable
            "oaTable", encodedOATable
            "stringTable", encodedStringTable
            "object", object
        ] 

    let decode (decoder) =
        Decode.object(fun get ->
            let stringTable = get.Required.Field "stringTable" (StringTable.decoder)
            let oaTable = get.Required.Field "oaTable" (OATable.decoder stringTable)
            let cellTable = get.Required.Field "cellTable" (CellTable.decoder stringTable oaTable)
            get.Required.Field "object" (decoder stringTable oaTable cellTable)
        )