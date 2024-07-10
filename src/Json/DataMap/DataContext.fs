namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper
open Conversion

module DataContext = 

    let encoder (dc:DataContext) = 
        [ 
            "data", Data.encoder dc
            Encode.tryInclude "explication" OntologyAnnotation.encoder dc.Explication 
            Encode.tryInclude "unit" OntologyAnnotation.encoder dc.Unit
            Encode.tryInclude "objectType" OntologyAnnotation.encoder dc.ObjectType
            Encode.tryInclude "description" Encode.string dc.Description
            Encode.tryInclude "generatedBy" Encode.string dc.GeneratedBy
            Encode.tryInclude "label" Encode.string dc.Label
        ]
        |> Encode.choose
        |> Encode.object

    let decoder : Decoder<DataContext> =
        Decode.object (fun get ->
            let data = get.Required.Field "data" Data.decoder
            DataContext(
                ?id = data.ID,
                ?name = data.Name,
                ?dataType = data.DataType,
                ?format = data.Format,
                ?selectorFormat = data.SelectorFormat,
                ?explication = get.Optional.Field "explication" OntologyAnnotation.decoder,
                ?unit = get.Optional.Field "unit" OntologyAnnotation.decoder,
                ?objectType = get.Optional.Field "objectType" OntologyAnnotation.decoder,
                ?description = get.Optional.Field "description" Decode.string,
                ?generatedBy = get.Optional.Field "generatedBy" Decode.string,
                ?label = get.Optional.Field "label" Decode.string,
                comments = data.Comments
            )
        )