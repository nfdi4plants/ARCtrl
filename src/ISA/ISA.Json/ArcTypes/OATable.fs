namespace rec ARCtrl.ISA.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ARCtrl.ISA

open ARCtrl.ISA.Aux

type OATableMap = System.Collections.Generic.Dictionary<OntologyAnnotation,int>

type OATableArray = array<OntologyAnnotation>

module OATable =

    let arrayFromMap (otm : OATableMap) : OATableArray=
        otm
        |> Seq.sortBy (fun kv -> kv.Value)
        |> Seq.map (fun kv -> kv.Key)
        |> Seq.toArray

    let encoder (stringTable : StringTableMap) (ot: OATableArray) =
        ot
        |> Array.map (OntologyAnnotation.compressedEncoder stringTable (ConverterOptions()))
        |> Encode.array

    let decoder stringTable : Decoder<OATableArray> =
        Decode.array (OntologyAnnotation.compressedDecoder stringTable (ConverterOptions())) 
        
    let encodeOA (otm : OATableMap) (oa : OntologyAnnotation) =
        match Dict.tryFind oa otm with
        | Some i -> Encode.int i
        | None ->
            let i = otm.Count
            otm.Add(oa,i)
            Encode.int i

    let decodeOA (ot : OATableArray) : Decoder<OntologyAnnotation> = 
        fun s o -> 
            match Decode.int s o with
            | Ok i -> Ok ot.[i]
            | Error err -> Error err