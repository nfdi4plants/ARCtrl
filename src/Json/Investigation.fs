namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Helper

module Investigation =
    
    let encoder (inv : ArcInvestigation) = 
        Encode.object [ 
            "Identifier", Encode.string inv.Identifier
            Encode.tryInclude "Title" Encode.string inv.Title
            Encode.tryInclude "Description" Encode.string inv.Description
            Encode.tryInclude "SubmissionDate" Encode.string inv.SubmissionDate
            Encode.tryInclude "PublicReleaseDate" Encode.string inv.PublicReleaseDate
            Encode.tryIncludeSeq "OntologySourceReferences" OntologySourceReference.encoder inv.OntologySourceReferences
            Encode.tryIncludeSeq "Publications" Publication.encoder inv.Publications
            Encode.tryIncludeSeq "Contacts" Person.encoder inv.Contacts
            Encode.tryIncludeSeq "Assays" Assay.encoder inv.Assays
            Encode.tryIncludeSeq "Studies" Study.encoder inv.Studies
            Encode.tryIncludeSeq "RegisteredStudyIdentifiers" Encode.string inv.RegisteredStudyIdentifiers
            Encode.tryIncludeSeq "Comments" Comment.encoder inv.Comments
            // remarks are ignored for whatever reason
        ]

    let decoder : Decoder<ArcInvestigation> =
        Decode.object (fun get ->
            ArcInvestigation(
                get.Required.Field("Identifier") Decode.string,
                ?title = get.Optional.Field "Title" Decode.string,
                ?description = get.Optional.Field "Description" Decode.string,
                ?submissionDate = get.Optional.Field "SubmissionDate" Decode.string,
                ?publicReleaseDate = get.Optional.Field "PublicReleaseDate" Decode.string,
                ?ontologySourceReferences = get.Optional.Field "OntologySourceReferences" (Decode.resizeArray OntologySourceReference.decoder),
                ?publications = get.Optional.Field "Publications" (Decode.resizeArray Publication.decoder),
                ?contacts = get.Optional.Field "Contacts" (Decode.resizeArray Person.decoder),
                ?assays = get.Optional.Field "Assays" (Decode.resizeArray Assay.decoder),
                ?studies = get.Optional.Field "Studies" (Decode.resizeArray Study.decoder),
                ?registeredStudyIdentifiers = get.Optional.Field "RegisteredStudyIdentifiers" (Decode.resizeArray Decode.string),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            ) 
        )

    open OATable
    open CellTable
    open StringTable

    let encoderCompressed (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (inv : ArcInvestigation) = 
        Encode.object [ 
            "Identifier", Encode.string inv.Identifier
            Encode.tryInclude "Title" Encode.string inv.Title
            Encode.tryInclude "Description" Encode.string inv.Description
            Encode.tryInclude "SubmissionDate" Encode.string inv.SubmissionDate
            Encode.tryInclude "PublicReleaseDate" Encode.string inv.PublicReleaseDate
            Encode.tryIncludeSeq "OntologySourceReferences" OntologySourceReference.encoder inv.OntologySourceReferences
            Encode.tryIncludeSeq "Publications" Publication.encoder inv.Publications
            Encode.tryIncludeSeq "Contacts" Person.encoder inv.Contacts
            Encode.tryIncludeSeq "Assays" (Assay.encoderCompressed stringTable oaTable cellTable) inv.Assays
            Encode.tryIncludeSeq "Studies" (Study.encoderCompressed stringTable oaTable cellTable) inv.Studies
            Encode.tryIncludeSeq "RegisteredStudyIdentifiers" Encode.string inv.RegisteredStudyIdentifiers
            Encode.tryIncludeSeq "Comments" Comment.encoder inv.Comments
            // remarks are ignored for whatever reason
        ]

    let decoderCompressed (stringTable) (oaTable) (cellTable) : Decoder<ArcInvestigation> =
        Decode.object (fun get ->
            ArcInvestigation(
                get.Required.Field("Identifier") Decode.string,
                ?title = get.Optional.Field "Title" Decode.string,
                ?description = get.Optional.Field "Description" Decode.string,
                ?submissionDate = get.Optional.Field "SubmissionDate" Decode.string,
                ?publicReleaseDate = get.Optional.Field "PublicReleaseDate" Decode.string,
                ?ontologySourceReferences = get.Optional.Field "OntologySourceReferences" (Decode.resizeArray OntologySourceReference.decoder),
                ?publications = get.Optional.Field "Publications" (Decode.resizeArray Publication.decoder),
                ?contacts = get.Optional.Field "Contacts" (Decode.resizeArray Person.decoder),
                ?assays = get.Optional.Field "Assays" (Decode.resizeArray <| Assay.decoderCompressed stringTable oaTable cellTable),
                ?studies = get.Optional.Field "Studies" (Decode.resizeArray <| Study.decoderCompressed stringTable oaTable cellTable),
                ?registeredStudyIdentifiers = get.Optional.Field "RegisteredStudyIdentifiers" (Decode.resizeArray Decode.string),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            ) 
        )

    module ROCrate = 
        let genID (i:ArcInvestigation) : string = 
            "./"
            // match i.ID with
            // | Some id -> URI.toString id
            // | None -> match i.FileName with
            //           | Some n -> "#Study_" + n.Replace(" ","_")
            //           | None -> match i.Identifier with
            //                     | Some id -> "#Study_" + id.Replace(" ","_")
            //                     | None -> match i.Title with
            //                               | Some t -> "#Study_" + t.Replace(" ","_")
            //                               | None -> "#EmptyStudy"

        //let encoder (options : ConverterOptions) (oa : Investigation) = 
        //    [
        //        if options.SetID then 
        //            "@id", Encode.string (oa |> genID)
        //        else 
        //            Encode.tryInclude "@id" Encode.string (oa.ID)
        //        if options.IsJsonLD then 
        //            "@type", Encode.string "Investigation"
        //            "additionalType", Encode.string "Investigation"
        //        Encode.tryInclude "filename" Encode.string (oa.FileName)
        //        Encode.tryInclude "identifier" Encode.string (oa.Identifier)
        //        Encode.tryInclude "title" Encode.string (oa.Title)
        //        Encode.tryInclude "description" Encode.string (oa.Description)
        //        Encode.tryInclude "submissionDate" Encode.string (oa.SubmissionDate)
        //        Encode.tryInclude "publicReleaseDate" Encode.string (oa.PublicReleaseDate)
        //        Encode.tryIncludeList "ontologySourceReferences" (OntologySourceReference.encoder options) (oa.OntologySourceReferences)
        //        Encode.tryIncludeList "publications" (Publication.encoder options) (oa.Publications)
        //        Encode.tryIncludeList "people" (Person.encoder options) (oa.Contacts)
        //        Encode.tryIncludeList "studies" (Study.encoder options) (oa.Studies)
        //        Encode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
        //        if options.IsJsonLD then
        //            "@context", ROCrateContext.Investigation.context_jsonvalue
        //    ]
        //    |> Encode.choose
        //    |> Encode.object

        //let encodeRoCrate (options : ConverterOptions) (oa : Investigation) = 
        //    [
        //        Encode.tryInclude "@type" Encode.string (Some "CreativeWork")
        //        Encode.tryInclude "@id" Encode.string (Some "ro-crate-metadata.json")
        //        Encode.tryInclude "about" (encoder options) (Some oa)
        //        "conformsTo", ROCrateContext.ROCrate.conformsTo_jsonvalue
        //        if options.IsJsonLD then
        //            "@context", ROCrateContext.ROCrate.context_jsonvalue
        //        ]
        //    |> Encode.choose
        //    |> Encode.object

    module ISAJson =

        let allowedFields = ["@id";"filename";"identifier";"title";"description";"submissionDate";"publicReleaseDate";"ontologySourceReferences";"publications";"people";"studies";"comments";"@type";"@context"]

        let encoder (inv: ArcInvestigation) = 
            [
                "filename", Encode.string ArcInvestigation.FileName
                "identifier", Encode.string (inv.Identifier)
                Encode.tryInclude "title" Encode.string (inv.Title)
                Encode.tryInclude "description" Encode.string (inv.Description)
                Encode.tryInclude "submissionDate" Encode.string (inv.SubmissionDate)
                Encode.tryInclude "publicReleaseDate" Encode.string (inv.PublicReleaseDate)
                Encode.tryIncludeSeq "ontologySourceReferences" OntologySourceReference.ISAJson.encoder inv.OntologySourceReferences
                Encode.tryIncludeSeq "publications" Publication.ISAJson.encoder inv.Publications
                Encode.tryIncludeSeq "people" Person.ISAJson.encoder inv.Contacts
                Encode.tryIncludeSeq "studies" Study.ISAJson.encoder inv.Studies
                Encode.tryIncludeSeq "comments" Comment.ISAJson.encoder inv.Comments
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<ArcInvestigation> =
            Decode.objectNoAdditionalProperties allowedFields (fun get ->
                let identifer = 
                    match get.Optional.Field("identifier") Decode.string with
                    | Some i -> i
                    | None -> Identifier.createMissingIdentifier()
                let studiesRaw, assaysRaw =
                    get.Optional.Field "studies" (Decode.list Study.ISAJson.decoder)
                    |> Option.defaultValue []
                    |> List.unzip
                let assays = assaysRaw |> Seq.concat |> Seq.distinctBy (fun a -> a.Identifier) |> ResizeArray 
                let studies = ResizeArray(studiesRaw)
                let studyIdentifiers = studiesRaw |> Seq.map (fun a -> a.Identifier) |> ResizeArray
                ArcInvestigation(
                    identifer,
                    ?title = get.Optional.Field "title" Decode.string,
                    ?description = get.Optional.Field "description" Decode.string,
                    ?submissionDate = get.Optional.Field "submissionDate" Decode.string,
                    ?publicReleaseDate = get.Optional.Field "publicReleaseDate" Decode.string,
                    ?ontologySourceReferences = get.Optional.Field "ontologySourceReferences" (Decode.resizeArray OntologySourceReference.ISAJson.decoder),
                    ?publications = get.Optional.Field "publications" (Decode.resizeArray Publication.ISAJson.decoder),
                    ?contacts = get.Optional.Field "people" (Decode.resizeArray Person.ISAJson.decoder),
                    assays = assays,
                    studies = studies,
                    registeredStudyIdentifiers = studyIdentifiers,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ISAJson.decoder)
                )
            )

[<AutoOpen>]
module InvestigationExtensions =

    type ArcInvestigation with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Investigation.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:ArcInvestigation) ->
                Investigation.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)                  

        static member fromCompressedJsonString (s: string) =
            try Decode.fromJsonString (Compression.decode Investigation.decoderCompressed) s with
            | e -> failwithf "Error. Unable to parse json string to ArcStudy: %s" e.Message

        static member toCompressedJsonString(?spaces) =
            fun (obj:ArcInvestigation) ->
                let spaces = defaultArg spaces 0
                Encode.toJsonString spaces (Compression.encode Investigation.encoderCompressed obj)

        //static member fromROCrateJsonString (s:string) = 
        //    Decode.fromJsonString Investigation.ROCrate.decoder s

        ///// exports in json-ld format
        //static member toROCrateJsonString(?spaces) =
        //    fun (obj:ArcInvestigation) ->
        //        Investigation.ROCrate.encoder obj
        //        |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member toISAJsonString(?spaces) =
            fun (obj:ArcInvestigation) ->
                Investigation.ISAJson.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Investigation.ISAJson.decoder s