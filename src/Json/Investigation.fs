namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl

module Investigation =
    
    let encoder (inv : ArcInvestigation) = 
        Encode.object [ 
            "Identifier", Encode.string inv.Identifier
            Encode.tryInclude "Title" Encode.string (inv.Title)
            Encode.tryInclude "Description" Encode.string (inv.Description)
            Encode.tryInclude "SubmissionDate" Encode.string (inv.SubmissionDate)
            Encode.tryInclude "PublicReleaseDate" Encode.string (inv.PublicReleaseDate)
            Encode.tryIncludeSeq "OntologySourceReferences" OntologySourceReference.encoder (inv.OntologySourceReferences)
            //if inv.Title.IsSome then
            //    "Title", Encode.string inv.Title.Value
            //if inv.Description.IsSome then
            //    "Description", Encode.string inv.Description.Value
            //if inv.SubmissionDate.IsSome then
            //    "SubmissionDate", Encode.string inv.SubmissionDate.Value
            //if inv.PublicReleaseDate.IsSome then
            //    "PublicReleaseDate", Encode.string inv.PublicReleaseDate.Value
            //if inv.OntologySourceReferences.Length <> 0 then
            //    "OntologySourceReferences", EncoderOntologySourceReferences inv.OntologySourceReferences
            if inv.Publications.Length <> 0 then
                "Publications", EncoderPublications inv.Publications
            if inv.Contacts.Length <> 0 then
                "Contacts", EncoderPersons inv.Contacts
            if inv.Assays.Count <> 0 then
                "Assays", Encode.seq (Seq.map ArcAssay.encoder inv.Assays) 
            if inv.Studies.Count <> 0 then
                "Studies", Encode.seq (Seq.map ArcStudy.encoder inv.Studies)
            if inv.RegisteredStudyIdentifiers.Count <> 0 then
                "RegisteredStudyIdentifiers", Encode.seq (Seq.map Encode.string inv.RegisteredStudyIdentifiers)
            if inv.Comments.Length <> 0 then
                "Comments", EncoderComments inv.Comments
            // remarks are ignored for whatever reason
        ]

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

    let encoder (options : ConverterOptions) (oa : Investigation) = 
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                Encode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", Encode.string "Investigation"
                "additionalType", Encode.string "Investigation"
            Encode.tryInclude "filename" Encode.string (oa.FileName)
            Encode.tryInclude "identifier" Encode.string (oa.Identifier)
            Encode.tryInclude "title" Encode.string (oa.Title)
            Encode.tryInclude "description" Encode.string (oa.Description)
            Encode.tryInclude "submissionDate" Encode.string (oa.SubmissionDate)
            Encode.tryInclude "publicReleaseDate" Encode.string (oa.PublicReleaseDate)
            Encode.tryIncludeList "ontologySourceReferences" (OntologySourceReference.encoder options) (oa.OntologySourceReferences)
            Encode.tryIncludeList "publications" (Publication.encoder options) (oa.Publications)
            Encode.tryIncludeList "people" (Person.encoder options) (oa.Contacts)
            Encode.tryIncludeList "studies" (Study.encoder options) (oa.Studies)
            Encode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
            if options.IsJsonLD then
                "@context", ROCrateContext.Investigation.context_jsonvalue
        ]
        |> Encode.choose
        |> Encode.object

    let encodeRoCrate (options : ConverterOptions) (oa : Investigation) = 
        [
            Encode.tryInclude "@type" Encode.string (Some "CreativeWork")
            Encode.tryInclude "@id" Encode.string (Some "ro-crate-metadata.json")
            Encode.tryInclude "about" (encoder options) (Some oa)
            "conformsTo", ROCrateContext.ROCrate.conformsTo_jsonvalue
            if options.IsJsonLD then
                "@context", ROCrateContext.ROCrate.context_jsonvalue
            ]
        |> Encode.choose
        |> Encode.object


    let allowedFields = ["@id";"filename";"identifier";"title";"description";"submissionDate";"publicReleaseDate";"ontologySourceReferences";"publications";"people";"studies";"comments";"@type";"@context"]

    let decoder (options : ConverterOptions) : Decoder<Investigation> =
        GDecode.object allowedFields (fun get ->
            {
                ID = get.Optional.Field "@id" Decode.string
                FileName = get.Optional.Field "filename" Decode.string
                Identifier = get.Optional.Field "identifier" Decode.string
                Title = get.Optional.Field "title" Decode.string
                Description = get.Optional.Field "description" Decode.string
                SubmissionDate = get.Optional.Field "submissionDate" Decode.string
                PublicReleaseDate = get.Optional.Field "publicReleaseDate" Decode.string
                OntologySourceReferences = get.Optional.Field "ontologySourceReferences" (Decode.list (OntologySourceReference.decoder options))
                Publications = get.Optional.Field "publications" (Decode.list (Publication.decoder options))
                Contacts = get.Optional.Field "people" (Decode.list (Person.decoder options))
                Studies = get.Optional.Field "studies" (Decode.list (Study.decoder options))
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
                Remarks = []
            }
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (p:Investigation) = 
        encoder (ConverterOptions()) p
        |> Encode.toJsonString 2

    /// exports in json-ld format
    let toJsonldString (i:Investigation) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) i
        |> Encode.toJsonString 2

    let toJsonldStringWithContext (i:Investigation) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) i
        |> Encode.toJsonString 2

    let toRoCrateString (i:Investigation) = 
        encodeRoCrate (ConverterOptions(SetID=true,IsJsonLD=true)) i
        |> Encode.toJsonString 2