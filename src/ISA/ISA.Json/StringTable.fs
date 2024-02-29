namespace rec ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA

open ARCtrl.ISA.Aux

type StringTableMap = System.Collections.Generic.Dictionary<string,int>

type StringTableArray = array<string>

module StringTable =

    let arrayFromMap (otm : StringTableMap) : StringTableArray=
        otm
        |> Seq.sortBy (fun kv -> kv.Value)
        |> Seq.map (fun kv -> kv.Key)
        |> Seq.toArray

    let encoder (ot: StringTableArray) =
        ot
        |> Array.map Encode.string
        |> Encode.array

    let decoder : Decoder<StringTableArray> =
        Decode.array Decode.string
        
    let encodeString (otm : StringTableMap) (s : obj) =
        let s = s :?> string
        match Dict.tryFind s otm with
        | Some i -> Encode.int i
        | None ->
            let i = otm.Count
            otm.Add(s,i)
            Encode.int i

    let decodeString (ot : StringTableArray) : Decoder<string> = 
        { new Decoder<string> with
            member this.Decode (s,json) = 
                match Decode.int.Decode(s,json) with
                | Ok i -> Ok ot.[i]
                | Error err -> Error err
        }
