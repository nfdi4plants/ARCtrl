namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open System.IO

module Publication = 

    let encoder (oa : Publication) = 
        [
            Encode.tryInclude "pubMedID" Encode.string (oa.PubMedID)
            Encode.tryInclude "doi" Encode.string (oa.DOI)
            Encode.tryInclude "authorList" Encode.string (oa.Authors)
            Encode.tryInclude "title" Encode.string (oa.Title)
            Encode.tryInclude "status" OntologyAnnotation.encoder (oa.Status)
            Encode.tryIncludeSeq "comments" Comment.encoder oa.Comments
        ]
        |> Encode.choose
        |> Encode.object

    let decoder : Decoder<Publication> =
        Decode.object (fun get ->
            Publication(
                ?pubMedID = get.Optional.Field "pubMedID" Decode.uri,
                ?doi= get.Optional.Field "doi" Decode.string,
                ?authors = get.Optional.Field "authorList" Decode.string,
                ?title = get.Optional.Field "title" Decode.string,
                ?status = get.Optional.Field "status" OntologyAnnotation.decoder,
                ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.decoder)
            )
        )

    let genID (p:Publication) = 
        match p.DOI with
        | Some doi -> doi
        | None -> 
            match p.PubMedID with
            | Some id -> id
            | None -> 
                match p.Title with
                | Some t -> "#Pub_" + t.Replace(" ","_")
                | None -> "#EmptyPublication"

    module ISAJson =
        
        let encoder (idMap : IDTable.IDTableWrite option) (oa : Publication) = 
            [
                Encode.tryInclude "pubMedID" Encode.string oa.PubMedID
                Encode.tryInclude "doi" Encode.string (oa.DOI)
                Encode.tryInclude "authorList" Encode.string oa.Authors
                Encode.tryInclude "title" Encode.string (oa.Title)
                Encode.tryInclude "status" OntologyAnnotation.encoder oa.Status
                Encode.tryIncludeSeq "comments" (Comment.ISAJson.encoder idMap) oa.Comments
            ]
            |> Encode.choose
            |> Encode.object

        let allowedFields = ["pubMedID";"doi";"authorList";"title";"status";"comments";]

        let decoder : Decoder<Publication> =
            decoder 
            |> Decode.noAdditionalProperties allowedFields
