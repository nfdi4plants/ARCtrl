namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO


module FactorValue =
    
    module ROCrate =

        let encoder : FactorValue -> IEncodable = 
            PropertyValue.ROCrate.encoder

        let decoder : Decoder<FactorValue> =
            PropertyValue.ROCrate.decoder<FactorValue> (FactorValue.createAsPV)

    module ISAJson = 
        
        let genID (fv : FactorValue) = 
            PropertyValue.ROCrate.genID fv

        let encoder (idMap : IDTable.IDTableWrite option) (fv : FactorValue) = 
            let f (fv : FactorValue) =
                [
                    // Is this required for ISA-JSON? The FactorValue type has an @id field
                    Encode.tryInclude "@id" Encode.string (fv |> genID |> Some)
                    Encode.tryInclude "category" (Factor.ISAJson.encoder idMap) fv.Category
                    Encode.tryInclude "value" (Value.ISAJson.encoder idMap) fv.Value
                    Encode.tryInclude "unit" (OntologyAnnotation.ISAJson.encoder idMap) fv.Unit
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f fv
            | Some idMap -> IDTable.encode genID f fv idMap

        let decoder: Decoder<FactorValue> =
            Decode.object (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Category = get.Optional.Field "category" Factor.ISAJson.decoder
                    Value = get.Optional.Field "value" Value.ISAJson.decoder
                    Unit = get.Optional.Field "unit" OntologyAnnotation.ISAJson.decoder
                }
            )
