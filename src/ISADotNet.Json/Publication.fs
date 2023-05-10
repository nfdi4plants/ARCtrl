namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

module Publication =    
    
    let genID (p:Publication) = 
        match p.DOI with
        | Some doi -> doi
        | None -> match p.PubMedID with
                  | Some id -> id
                  | None -> match p.Title with
                            | Some t -> "#Pub_" + t
                            | None -> "#EmptyPublication"

    let rec encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.string (oa :?> Publication |> genID)
            if options.IncludeType then "@type", GEncode.string "Publication"
            tryInclude "pubMedID" GEncode.string (oa |> tryGetPropertyValue "PubMedID")
            tryInclude "doi" GEncode.string (oa |> tryGetPropertyValue "DOI")
            tryInclude "authorList" GEncode.string (oa |> tryGetPropertyValue "Authors")
            tryInclude "title" GEncode.string (oa |> tryGetPropertyValue "Title")
            tryInclude "status" (OntologyAnnotation.encoder options) (oa |> tryGetPropertyValue "Status")
            tryInclude "comments" (Comment.encoder options) (oa |> tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let rec decoder (options : ConverterOptions) : Decoder<Publication> =
        Decode.object (fun get ->
            {
                PubMedID = get.Optional.Field "pubMedID" GDecode.uri
                DOI = get.Optional.Field "doi" Decode.string
                Authors = get.Optional.Field "authorList" Decode.string
                Title = get.Optional.Field "title" Decode.string
                Status = get.Optional.Field "status" (OntologyAnnotation.decoder options)
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
            }
            
        )

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (p:Publication) = 
        encoder (ConverterOptions()) p
        |> Encode.toString 2

    /// exports in json-ld format
    let toStringLD (p:Publication) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) p
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Publication) = 
    //    File.WriteAllText(path,toString p)