namespace ARCtrl.Json


open Thoth.Json.Core
open ARCtrl
open ARCtrl.Process
open System.Collections.Generic

/// This module is used to generalize json compression helpers
module Compression =

    let encode (encoder: Dictionary<string, int> -> Dictionary<OntologyAnnotation, int> -> Dictionary<CompositeCell, int> -> 'A -> Json) (obj: 'A) = 
        let stringTable = Dictionary()
        let oaTable = Dictionary()
        let cellTable = Dictionary()
        let arcStudy = encoder stringTable oaTable cellTable obj 
        Encode.object [
            "cellTable", CellTable.arrayFromMap cellTable |> CellTable.encoder stringTable oaTable
            "oaTable", OATable.arrayFromMap oaTable |> OATable.encoder stringTable
            "stringTable", StringTable.arrayFromMap stringTable |> StringTable.encoder
            "object", arcStudy
        ] 

    let decode (decoder) =
        Decode.object(fun get ->
            let stringTable = get.Required.Field "stringTable" (StringTable.decoder)
            let oaTable = get.Required.Field "oaTable" (OATable.decoder stringTable)
            let cellTable = get.Required.Field "cellTable" (CellTable.decoder stringTable oaTable)
            get.Required.Field "object" (decoder stringTable oaTable cellTable)
        )