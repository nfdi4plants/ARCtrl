namespace ARCtrl.ISA.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ARCtrl.ISA
open System.IO

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
            if options.SetID then "@id", GEncode.string (oa :?> Investigation |> genID)
                else GEncode.tryInclude "@id" GEncode.string (oa |> GEncode.tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.string "Investigation"
            GEncode.tryInclude "filename" GEncode.string (oa |> GEncode.tryGetPropertyValue "FileName")
            GEncode.tryInclude "identifier" GEncode.string (oa |> GEncode.tryGetPropertyValue "Identifier")
            GEncode.tryInclude "title" GEncode.string (oa |> GEncode.tryGetPropertyValue "Title")
            GEncode.tryInclude "description" GEncode.string (oa |> GEncode.tryGetPropertyValue "Description")
            GEncode.tryInclude "submissionDate" GEncode.string (oa |> GEncode.tryGetPropertyValue "SubmissionDate")
            GEncode.tryInclude "publicReleaseDate" GEncode.string (oa |> GEncode.tryGetPropertyValue "PublicReleaseDate")
            GEncode.tryInclude "ontologySourceReferences" (OntologySourceReference.encoder options) (oa |> GEncode.tryGetPropertyValue "OntologySourceReferences")
            GEncode.tryInclude "publications" (Publication.encoder options) (oa |> GEncode.tryGetPropertyValue "Publications")
            GEncode.tryInclude "people" (Person.encoder options) (oa |> GEncode.tryGetPropertyValue "Contacts")
            GEncode.tryInclude "studies" (Study.encoder options) (oa |> GEncode.tryGetPropertyValue "Studies")
            GEncode.tryInclude "comments" (Comment.encoder options) (oa |> GEncode.tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<Investigation> =
        Decode.object (fun get ->
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

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (p:Investigation) = 
        encoder (ConverterOptions()) p
        |> Encode.toString 2

    /// exports in json-ld format
    let toStringLD (i:Investigation) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) i
        |> Encode.toString 2

module ArcInvestigation = 

    open Investigation

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s
        |> ArcInvestigation.fromInvestigation

    let toString (a:ArcInvestigation) = 
        encoder (ConverterOptions()) (a.ToInvestigation())
        |> Encode.toString 2

    /// exports in json-ld format
    let toStringLD (a:ArcInvestigation) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) (a.ToInvestigation())
        |> Encode.toString 2