namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper
open Conversion

open CellTable
open OATable
open StringTable


module Datamap = 

    let encoder (dm: Datamap) =
        Encode.object [
            "dataContexts", dm.DataContexts |> Seq.map DataContext.encoder |> Encode.seq
        ]

    let decoder : Decoder<Datamap> =
        Decode.object (fun get ->
            let dataContexts = get.Required.Field "dataContexts" (Decode.resizeArray DataContext.decoder)
            Datamap(dataContexts)
        )   


    let encoderCompressed (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (dm: Datamap)  =
        encoder dm

    let decoderCompressed (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray)  : Decoder<Datamap> =
        decoder
        //Decode.map (fun (t : ArcTable) -> Datamap(t.Headers,t.Values)) (ArcTable.decoderCompressed stringTable oaTable cellTable)

    //module ROCrate =

    //    let encodeVariableM