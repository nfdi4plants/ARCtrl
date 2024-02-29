namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA

module Investigation =
    
    
    let genID (i:Investigation) : string = 
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
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IncludeType then 
                "@type", Encode.string "Investigation"
            GEncode.tryInclude "filename" Encode.string (oa.FileName)
            GEncode.tryInclude "identifier" Encode.string (oa.Identifier)
            GEncode.tryInclude "title" Encode.string (oa.Title)
            GEncode.tryInclude "description" Encode.string (oa.Description)
            GEncode.tryInclude "submissionDate" Encode.string (oa.SubmissionDate)
            GEncode.tryInclude "publicReleaseDate" Encode.string (oa.PublicReleaseDate)
            GEncode.tryIncludeList "ontologySourceReferences" (OntologySourceReference.encoder options) (oa.OntologySourceReferences)
            GEncode.tryIncludeList "publications" (Publication.encoder options) (oa.Publications)
            GEncode.tryIncludeList "people" (Person.encoder options) (oa.Contacts)
            GEncode.tryIncludeList "studies" (Study.encoder options) (oa.Studies)
            GEncode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
            if options.IncludeContext then
                "@context", ROCrateContext.Investigation.context_jsonvalue
        ]
        |> GEncode.choose
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

    let toJsonString (p:Investigation) = 
        encoder (ConverterOptions()) p
        |> GEncode.toJsonString 2

    /// exports in json-ld format
    let toJsonldString (i:Investigation) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) i
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (i:Investigation) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true,IncludeContext=true)) i
        |> GEncode.toJsonString 2

    let toRoCrateString (i:Investigation) = 
        encodeRoCrate (ConverterOptions(SetID=true,IncludeType=true,IncludeContext=true,IsRoCrate=true)) i
        |> GEncode.toJsonString 2