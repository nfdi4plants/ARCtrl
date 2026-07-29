namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO

module Value = 

    module ISAJson = 

        let encoder (idMap : IDTable.IDTableWrite option) (value : ScalarValue) = 
            match value with
            | ScalarValue.Float f -> 
                Encode.float f
            | ScalarValue.Int i -> 
                Encode.int i
            | ScalarValue.Name s -> 
                Encode.string s
            | ScalarValue.Ontology s -> 
                OntologyAnnotation.ISAJson.encoder idMap s

        let decoder : Decoder<ScalarValue> =
            Decode.oneOf [
                Decode.map ScalarValue.Int Decode.int
                Decode.map ScalarValue.Float Decode.float
                Decode.map ScalarValue.Ontology OntologyAnnotation.ISAJson.decoder
                Decode.map ScalarValue.Name Decode.string           
            ]