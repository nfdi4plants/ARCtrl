namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO

module Value = 

    module ISAJson = 

        let encoder (idMap : IDTable.IDTableWrite option) (value : PropertyValue) = 
            match value with
            | PropertyValue.Float f -> 
                Encode.float f
            | PropertyValue.Int i -> 
                Encode.int i
            | PropertyValue.Name s -> 
                Encode.string s
            | PropertyValue.Ontology s -> 
                OntologyAnnotation.ISAJson.encoder idMap s

        let decoder : Decoder<PropertyValue> =
            Decode.oneOf [
                Decode.map PropertyValue.Int Decode.int
                Decode.map PropertyValue.Float Decode.float
                Decode.map PropertyValue.Ontology OntologyAnnotation.ISAJson.decoder
                Decode.map PropertyValue.Name Decode.string           
            ]