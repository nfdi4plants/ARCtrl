namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Helper

module OATable =

    type OATableMap = System.Collections.Generic.Dictionary<OntologyAnnotation,int>

    type OATableArray = array<OntologyAnnotation>

    let arrayFromMap (otm : OATableMap) : OATableArray=
        otm
        |> Seq.sortBy (fun kv -> kv.Value)
        |> Seq.map (fun kv -> kv.Key)
        |> Seq.toArray

    let encoder (stringTable : StringTableMap) (ot: OATableArray) =
        ot
        |> Array.map (OntologyAnnotation.compressedEncoder stringTable)
        |> Encode.array

    let decoder stringTable : Decoder<OATableArray> =
        Decode.array (OntologyAnnotation.compressedDecoder stringTable) 
        
    let encodeOA (otm : OATableMap) (oa : OntologyAnnotation) =
        match Dictionary.tryFind oa otm with
        | Some i -> Encode.int i
        | None ->
            let i = otm.Count
            otm.Add(oa,i)
            Encode.int i

    let decodeOA (ot : OATableArray) : Decoder<OntologyAnnotation> = 
        Decode.object (fun get ->
            let i = get.Required.Raw Decode.int
            ot.[i]
        )
        