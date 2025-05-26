namespace rec ARCtrl.Json

open Thoth.Json.Core

open ARCtrl

open ARCtrl.Helper

module StringTable =

    type StringTableMap = System.Collections.Generic.Dictionary<string,int>

    type StringTableArray = array<string>

    let arrayFromMap (otm : StringTableMap) : StringTableArray =
        let a = Array.zeroCreate<string> otm.Count
        otm
        |> Seq.iter (fun kv -> a.[kv.Value] <- kv.Key)
        a

    let encoder (ot: StringTableArray) =
        ot
        |> Array.map Encode.string
        |> Encode.array

    let decoder : Decoder<StringTableArray> =
        Decode.array Decode.string
        
    let encodeString (otm : StringTableMap) (s : string) =
        match StringDictionary.tryFind s otm with
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
