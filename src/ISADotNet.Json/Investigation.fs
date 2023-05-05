namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

module Investigation =
    
    let encoder (options : ConverterOptions) (oa : obj) = 
        [

            tryInclude "@id" GEncode.string (oa |> tryGetPropertyValue "ID")
            tryInclude "filename" GEncode.string (oa |> tryGetPropertyValue "FileName")
            tryInclude "identifier" GEncode.string (oa |> tryGetPropertyValue "Identifier")
            tryInclude "title" GEncode.string (oa |> tryGetPropertyValue "Title")
            tryInclude "description" GEncode.string (oa |> tryGetPropertyValue "Description")
            tryInclude "submissionDate" GEncode.string (oa |> tryGetPropertyValue "SubmissionDate")
            tryInclude "publicReleaseDate" GEncode.string (oa |> tryGetPropertyValue "PublicReleaseDate")
            tryInclude "ontologySourceReferences" (OntologySourceReference.encoder options) (oa |> tryGetPropertyValue "OntologySourceReferences")
            tryInclude "publications" (Publication.encoder options) (oa |> tryGetPropertyValue "Publications")
            tryInclude "contacts" (Person.encoder options) (oa |> tryGetPropertyValue "Contacts")
            tryInclude "studies" (Study.encoder options) (oa |> tryGetPropertyValue "Studies")
            tryInclude "comments" (Comment.encoder options) (oa |> tryGetPropertyValue "Comments")
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
                Contacts = get.Optional.Field "contacts" (Decode.list (Person.decoder options))
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

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:Investigation) = 
        File.WriteAllText(path,toString p)