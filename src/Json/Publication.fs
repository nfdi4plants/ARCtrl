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

    module ROCrate =

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

        let encoder (oa : Publication) = 
            [
                "@id", Encode.string (oa |> genID)
                "@type", Encode.string "Publication"
                Encode.tryInclude "pubMedID" Encode.string oa.PubMedID
                Encode.tryInclude "doi" Encode.string (oa.DOI)
                Encode.tryInclude "authorList" Person.ROCrate.encodeAuthorListString oa.Authors
                Encode.tryInclude "title" Encode.string (oa.Title)
                Encode.tryInclude "status" OntologyAnnotation.ROCrate.encoderDefinedTerm oa.Status
                Encode.tryIncludeSeq "comments" Comment.ROCrate.encoderDisambiguatingDescription oa.Comments
                "@context", ROCrateContext.Publication.context_jsonvalue
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<Publication> =
            Decode.object (fun get ->
                Publication(
                    ?pubMedID = get.Optional.Field "pubMedID" Decode.uri,
                    ?doi= get.Optional.Field "doi" Decode.string,
                    ?authors = get.Optional.Field "authorList" Person.ROCrate.decodeAuthorListString,
                    ?title = get.Optional.Field "title" Decode.string,
                    ?status = get.Optional.Field "status" OntologyAnnotation.ROCrate.decoderDefinedTerm,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ROCrate.decoderDisambiguatingDescription)
                )           
            )

    module ISAJson =
        
        let encoder (idMap : IDTable.IDTableWrite option) (oa : Publication) = 
            let f (oa : Publication) =
                [
                    Encode.tryInclude "@id" Encode.string (ROCrate.genID oa |> Some)
                    Encode.tryInclude "pubMedID" Encode.string oa.PubMedID
                    Encode.tryInclude "doi" Encode.string (oa.DOI)
                    Encode.tryInclude "authorList" Encode.string oa.Authors
                    Encode.tryInclude "title" Encode.string (oa.Title)
                    Encode.tryInclude "status" OntologyAnnotation.encoder oa.Status
                    Encode.tryIncludeSeq "comments" (Comment.ISAJson.encoder idMap) oa.Comments
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f oa
            | Some idMap -> IDTable.encode ROCrate.genID f oa idMap


        let allowedFields = ["pubMedID";"doi";"authorList";"title";"status";"comments";]

        let decoder : Decoder<Publication> =
            decoder 
            |> Decode.noAdditionalProperties allowedFields

[<AutoOpen>]
module PublicationExtensions =

    type Publication with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Publication.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:Publication) ->
                Publication.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)                  

        member this.ToJsonString(?spaces) =
            Publication.toJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString Publication.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:Publication) ->
                Publication.ROCrate.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) =
            Publication.toROCrateJsonString(?spaces=spaces) this

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Publication.ISAJson.decoder s
       
        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (obj:Publication) ->
                Publication.ISAJson.encoder idMap obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            Publication.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this