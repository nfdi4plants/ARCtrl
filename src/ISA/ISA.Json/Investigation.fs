namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA

module Investigation =
    
    
    let genID (i:Investigation) : string = 
        match i.ID with
        | Some id -> URI.toString id
        | None -> match i.FileName with
                  | Some n -> "#Study_" + n.Replace(" ","_")
                  | None -> match i.Identifier with
                            | Some id -> "#Study_" + id.Replace(" ","_")
                            | None -> match i.Title with
                                      | Some t -> "#Study_" + t.Replace(" ","_")
                                      | None -> "#EmptyStudy"
    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.includeString (oa :?> Investigation |> genID)
                else GEncode.tryInclude "@id" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.includeString "Investigation"
            GEncode.tryInclude "filename" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "FileName")
            GEncode.tryInclude "identifier" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "Identifier")
            GEncode.tryInclude "title" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "Title")
            GEncode.tryInclude "description" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "Description")
            GEncode.tryInclude "submissionDate" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "SubmissionDate")
            GEncode.tryInclude "publicReleaseDate" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "PublicReleaseDate")
            GEncode.tryInclude "ontologySourceReferences" (OntologySourceReference.encoder options) (oa |> GEncode.tryGetPropertyValue "OntologySourceReferences")
            GEncode.tryInclude "publications" (Publication.encoder options) (oa |> GEncode.tryGetPropertyValue "Publications")
            GEncode.tryInclude "people" (Person.encoder options) (oa |> GEncode.tryGetPropertyValue "Contacts")
            GEncode.tryInclude "studies" (Study.encoder options) (oa |> GEncode.tryGetPropertyValue "Studies")
            GEncode.tryInclude "comments" (Comment.encoder options) (oa |> GEncode.tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let allowedFields = ["@id";"filename";"identifier";"title";"description";"submissionDate";"publicReleaseDate";"ontologySourceReferences";"publications";"people";"studies";"comments";"@type"]

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
    let toStringLD (i:Investigation) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) i
        |> GEncode.toJsonString 2