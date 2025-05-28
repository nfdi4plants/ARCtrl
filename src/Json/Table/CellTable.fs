namespace rec ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open StringTable

open ARCtrl.Helper

module CellTable =

    open OATable

    type CellTableMap = System.Collections.Generic.Dictionary<CompositeCell,int>
    type CellTableArray = array<CompositeCell>

    let [<Literal>] CellType = "celltype"
    let [<Literal>] CellValues = "values"

    let arrayFromMap (otm : CellTableMap) : CellTableArray=
        otm
        |> Dictionary.items
        |> Seq.sortBy (fun kv -> kv.Value)
        |> Seq.map (fun kv -> kv.Key)
        |> Seq.toArray

    let encoder (stringTable : StringTableMap) (oaTable : OATableMap) (ot: CellTableArray) =
        ot
        |> Array.map (CompositeCell.encoderCompressed stringTable oaTable)
        |> Encode.array

    let decoder (stringTable : StringTableArray) (oaTable : OATableArray) : Decoder<CellTableArray> =
        Decode.array (CompositeCell.decoderCompressed stringTable oaTable)
        
    let encodeCell (otm : CellTableMap) (cc : CompositeCell) =
        match Dictionary.tryFind cc otm with
        | Some i -> Encode.int i
        | None ->
            let i = otm.Count
            otm.Add(cc,i)
            Encode.int i

    let decodeCell (ot : CellTableArray) : Decoder<CompositeCell> = 
        Decode.object (fun get ->
            let i = get.Required.Raw Decode.int
            ot.[i].Copy()
        )