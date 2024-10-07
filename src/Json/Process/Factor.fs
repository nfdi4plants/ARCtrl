namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO


module Factor =  

    module ISAJson = 

        let genID (f : Factor) = 
            match f.Name with
            | Some name -> $"#Factor/{name}"
            | None -> 
                match f.FactorType with
                | Some factorType -> 
                    $"#Factor/{OntologyAnnotation.ROCrate.genID factorType}"
                | None -> "#EmptyFactor"

        let encoder (idMap : IDTable.IDTableWrite option) (value : Factor) = 
            let f (value : Factor) =
                [
                    Encode.tryInclude "@id" Encode.string (value |> genID |> Some)
                    Encode.tryInclude "factorName" Encode.string value.Name
                    Encode.tryInclude "factorType" (OntologyAnnotation.ISAJson.encoder idMap) value.FactorType
                    Encode.tryIncludeSeq "comments" (Comment.ISAJson.encoder idMap) (value.Comments |> Option.defaultValue (ResizeArray()))
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f value
            | Some idMap -> IDTable.encode genID f value idMap

        let decoder : Decoder<Factor> =
            Decode.object (fun get ->
                {                    
                    Name = get.Optional.Field "factorName" Decode.string
                    FactorType = get.Optional.Field "factorType" (OntologyAnnotation.ISAJson.decoder)
                    Comments = get.Optional.Field "comments" (Decode.resizeArray (Comment.ISAJson.decoder))
                }
            )
