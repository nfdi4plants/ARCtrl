namespace rec ARCtrl.Json

open Thoth.Json.Core

open ARCtrl

open ARCtrl.Aux

type CellTableMap = System.Collections.Generic.Dictionary<CompositeCell,int>

type CellTableArray = array<CompositeCell>

module CellTable =

    let [<Literal>] CellType = "celltype"
    let [<Literal>] CellValues = "values"

    let arrayFromMap (otm : CellTableMap) : CellTableArray=
        otm
        |> Seq.sortBy (fun kv -> kv.Value)
        |> Seq.map (fun kv -> kv.Key)
        |> Seq.toArray

    let encoder (stringTable : StringTableMap) (oaTable : OATableMap) (ot: CellTableArray) =
        ot
        |> Array.map (CompositeCell.compressedEncoder stringTable oaTable)
        |> Encode.array

    let decoder (stringTable : StringTableArray) (oaTable : OATableArray) : Decoder<CellTableArray> =
        Decode.array (CompositeCell.compressedDecoder stringTable oaTable)
        
    let encodeCell (otm : CellTableMap) (cc : CompositeCell) =
        match Dict.tryFind cc otm with
        | Some i -> Encode.int i
        | None ->
            let i = otm.Count
            otm.Add(cc,i)
            Encode.int i

    let decodeCell (ot : CellTableArray) : Decoder<CompositeCell> = 
        Decode.object (fun get ->
            let i = get.Required.Raw Decode.int
            ot.[i]
        )