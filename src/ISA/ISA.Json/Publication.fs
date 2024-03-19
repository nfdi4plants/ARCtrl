namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA
open System.IO

module Publication =    
    
    let genID (p:Publication) = 
        match p.DOI with
        | Some doi -> doi
        | None -> match p.PubMedID with
                  | Some id -> id
                  | None -> match p.Title with
                            | Some t -> "#Pub_" + t.Replace(" ","_")
                            | None -> "#EmptyPublication"

    let encoder (options : ConverterOptions) (oa : Publication) = 
        let commentEncoder = if options.IsJsonLD then Comment.encoderDisambiguatingDescription else Comment.encoder options
        let authorListEncoder = if options.IsJsonLD then ROCrateHelper.Person.authorListStringEncoder else Encode.string
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            if options.IsJsonLD then 
                "@type", Encode.string "Publication"
            GEncode.tryInclude "pubMedID" Encode.string (oa.PubMedID)
            GEncode.tryInclude "doi" Encode.string (oa.DOI)
            GEncode.tryInclude "authorList" authorListEncoder (oa.Authors)
            GEncode.tryInclude "title" Encode.string (oa.Title)
            GEncode.tryInclude "status" (OntologyAnnotation.encoder options) (oa.Status)
            GEncode.tryIncludeArray "comments" commentEncoder (oa.Comments)
            if options.IsJsonLD then 
                "@context", ROCrateContext.Publication.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object

    let allowedFields = ["@id";"pubMedID";"doi";"authorList";"title";"status";"comments";"@type"; "@context"]

    let decoder (options : ConverterOptions) : Decoder<Publication> =
        GDecode.object allowedFields (fun get ->
            {
                PubMedID = get.Optional.Field "pubMedID" GDecode.uri
                DOI = get.Optional.Field "doi" Decode.string
                Authors = get.Optional.Field "authorList" Decode.string
                Title = get.Optional.Field "title" Decode.string
                Status = get.Optional.Field "status" (OntologyAnnotation.decoder options)
                Comments = get.Optional.Field "comments" (Decode.array (Comment.decoder options))
            }
            
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (p:Publication) = 
        encoder (ConverterOptions()) p
        |> GEncode.toJsonString 2

    /// exports in json-ld format
    let toJsonldString (p:Publication) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) p
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Publication) = 
    //    File.WriteAllText(path,toString p)