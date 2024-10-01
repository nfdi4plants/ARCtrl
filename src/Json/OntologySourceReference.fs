namespace ARCtrl.Json

open Thoth.Json.Core
open ARCtrl
open ARCtrl.Helper
open ARCtrl.Json

module OntologySourceReference =

    let encoder (osr : OntologySourceReference) = 
        [
            Encode.tryInclude "description" Encode.string (osr.Description)
            Encode.tryInclude "file" Encode.string (osr.File)
            Encode.tryInclude "name" Encode.string (osr.Name)
            Encode.tryInclude "version" Encode.string (osr.Version)
            Encode.tryIncludeSeq "comments" Comment.encoder (osr.Comments)
        ]
        |> Encode.choose
        |> Encode.object

    let decoder : Decoder<OntologySourceReference> =
        Decode.object (fun get ->
            OntologySourceReference(
                ?description = get.Optional.Field "description" Decode.uri,
                ?file = get.Optional.Field "file" Decode.string,
                ?name = get.Optional.Field "name" Decode.string,
                ?version = get.Optional.Field "version" Decode.string,
                ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.decoder)
            )
        )

    module ROCrate =
        
        let genID (o:OntologySourceReference) = 
            match o.File with
            | Some f -> f
            | None -> 
                match o.Name with
                | Some n -> "#OntologySourceRef_" + n.Replace(" ","_")
                | None -> "#DummyOntologySourceRef"


        let encoder (osr : OntologySourceReference) = 
            [
                "@id", Encode.string (osr |> genID) |> Some
                "@type", Encode.string "OntologySourceReference" |> Some
                Encode.tryInclude "description" Encode.string (osr.Description)
                Encode.tryInclude "file" Encode.string (osr.File)
                Encode.tryInclude "name" Encode.string (osr.Name)
                Encode.tryInclude "version" Encode.string (osr.Version)
                Encode.tryIncludeSeq "comments" Comment.encoder (osr.Comments)
                "@context", ROCrateContext.OntologySourceReference.context_jsonvalue |> Some
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<OntologySourceReference> =
            Decode.object (fun get ->
                OntologySourceReference(
                    ?description = get.Optional.Field "description" Decode.uri,
                    ?file = get.Optional.Field "file" Decode.string,
                    ?name = get.Optional.Field "name" Decode.string,
                    ?version = get.Optional.Field "version" Decode.string,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.decoder)               
                )
            )

    module ISAJson =
        let encoder (idMap : IDTable.IDTableWrite option) (osr : OntologySourceReference) =         
            [
                Encode.tryInclude "description" Encode.string (osr.Description)
                Encode.tryInclude "file" Encode.string (osr.File)
                Encode.tryInclude "name" Encode.string (osr.Name)
                Encode.tryInclude "version" Encode.string (osr.Version)
                Encode.tryIncludeSeq "comments" (Comment.ISAJson.encoder idMap) (osr.Comments)
            ]
            |> Encode.choose
            |> Encode.object
            

        let decoder = decoder