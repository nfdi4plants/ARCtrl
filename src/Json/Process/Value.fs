namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO

module Value = 

    module ISAJson = 

        let encoder (idMap : IDTable.IDTableWrite option) (value : Value) = 
            match value with
            | Value.Float f -> 
                Encode.float f
            | Value.Int i -> 
                Encode.int i
            | Value.Name s -> 
                Encode.string s
            | Value.Ontology s -> 
                OntologyAnnotation.ISAJson.encoder idMap s

        let decoder : Decoder<Value> =
            Decode.oneOf [
                Decode.map Value.Int Decode.int
                Decode.map Value.Float Decode.float
                Decode.map Value.Ontology OntologyAnnotation.ISAJson.decoder
                Decode.map Value.Name Decode.string           
            ]