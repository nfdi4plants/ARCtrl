namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper
open Conversion

open CellTable
open OATable
open StringTable


module DataMap = 

    let encoder (dm: DataMap) =
        Encode.object [
            "dataContexts", dm.DataContexts |> Seq.map DataContext.encoder |> Encode.seq
        ]

    let decoder : Decoder<DataMap> =
        Decode.object (fun get ->
            let dataContexts = get.Required.Field "dataContexts" (Decode.resizeArray DataContext.decoder)
            DataMap(dataContexts)
        )   


    let encoderCompressed (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (dm: DataMap)  =
        encoder dm

    let decoderCompressed (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray)  : Decoder<DataMap> =
        decoder
        //Decode.map (fun (t : ArcTable) -> DataMap(t.Headers,t.Values)) (ArcTable.decoderCompressed stringTable oaTable cellTable)

    //module ROCrate =

    //    let encodeVariableM