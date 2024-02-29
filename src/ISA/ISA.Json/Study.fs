namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA
open System.IO

module StudyMaterials = 

    let encoder (options : ConverterOptions) (oa : StudyMaterials) = 
        [
            GEncode.tryIncludeList "sources" (Source.encoder options) (oa.Sources)
            GEncode.tryIncludeList "samples" (Sample.encoder options) (oa.Samples)
            GEncode.tryIncludeList "otherMaterials" (Material.encoder options) (oa.OtherMaterials)
        ]
        |> GEncode.choose
        |> Encode.object
    
    let allowedFields = ["sources";"samples";"otherMaterials"]

    let decoder (options : ConverterOptions) : Decoder<StudyMaterials> =
        GDecode.object allowedFields (fun get ->
            {
                Sources = get.Optional.Field "sources" (Decode.list (Source.decoder options))
                Samples = get.Optional.Field "samples" (Decode.list (Sample.decoder options))
                OtherMaterials = get.Optional.Field "otherMaterials" (Decode.list (Material.decoder options))
            }
        )


module Study =
    
    let genID (s:Study) : string = 
        match s.ID with
        | Some id -> URI.toString id
        | None -> match s.FileName with
                  | Some n -> n.Replace(" ","_").Remove(0,1 + (max (n.LastIndexOf('/')) (n.LastIndexOf('\\'))))
                  | None -> match s.Identifier with
                            | Some id -> "#Study_" + id.Replace(" ","_")
                            | None -> match s.Title with
                                      | Some t -> "#Study_" + t.Replace(" ","_")
                                      | None -> "#EmptyStudy"
    
    let encoder (options : ConverterOptions) (oa : Study) = 
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IncludeType then 
                "@type", (Encode.list [GEncode.toJsonString "Study"; GEncode.toJsonString "ArcStudy"])
            GEncode.tryInclude "filename" Encode.string (oa.FileName)
            GEncode.tryInclude "identifier" Encode.string (oa.Identifier)
            GEncode.tryInclude "title" Encode.string (oa.Title)
            GEncode.tryInclude "description" Encode.string (oa.Description)
            GEncode.tryInclude "submissionDate" Encode.string (oa.SubmissionDate)
            GEncode.tryInclude "publicReleaseDate" Encode.string (oa.PublicReleaseDate)
            GEncode.tryIncludeList "publications" (Publication.encoder options) (oa.Publications)
            GEncode.tryIncludeList "people" (Person.encoder options) (oa.Contacts)
            GEncode.tryIncludeList "studyDesignDescriptors" (OntologyAnnotation.encoder options) (oa.StudyDesignDescriptors)
            if not options.IsRoCrate then 
                GEncode.tryInclude "protocols" (Protocol.encoder options None None None) (oa.Protocols)
            if options.IsRoCrate then
                match study.Materials with
                | Some m -> 
                    GEncode.tryInclude "samples" (Sample.encoder options) (m.Samples)
                    GEncode.tryInclude "sources" (Source.encoder options) (m.Sources)
                    GEncode.tryInclude "materials" (Material.encoder options) (m.OtherMaterials)
                | None -> ()
            
            if not options.IsRoCrate then 
                (GEncode.tryInclude "materials" (StudyMaterials.encoder options) (oa.Materials)
            GEncode.tryIncludeList "processSequence" (Process.encoder options) (oa.ProcessSequence)
            GEncode.tryIncludeList "assays" (Assay.encoder options) (oa.Assays)            
            GEncode.tryIncludeList "factors" (Factor.encoder options) (oa.Factors)
            GEncode.tryIncludeList "characteristicCategories" (MaterialAttribute.encoder options) (oa.CharacteristicCategories)            
            GEncode.tryIncludeList "unitCategories" (OntologyAnnotation.encoder options) (oa.UnitCategories)
            GEncode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
            if options.IncludeContext then 
                "@context", ROCrateContext.Study.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object

    let allowedFields = ["@id";"filename";"identifier";"title";"description";"submissionDate";"publicReleaseDate";"publications";"people";"studyDesignDescriptors";"protocols";"materials";"assays";"factors";"characteristicCategories";"unitCategories";"processSequence";"comments";"@type"]

    let decoder (options : ConverterOptions) : Decoder<Study> =
        GDecode.object allowedFields (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                FileName = get.Optional.Field "filename" Decode.string
                Identifier = get.Optional.Field "identifier" Decode.string
                Title = get.Optional.Field "title" Decode.string
                Description = get.Optional.Field "description" Decode.string
                SubmissionDate = get.Optional.Field "submissionDate" Decode.string
                PublicReleaseDate = get.Optional.Field "publicReleaseDate" Decode.string
                Publications = get.Optional.Field "publications" (Decode.list (Publication.decoder options))
                Contacts = get.Optional.Field "people" (Decode.list (Person.decoder options))
                StudyDesignDescriptors = get.Optional.Field "studyDesignDescriptors" (Decode.list (OntologyAnnotation.decoder options))
                Protocols = get.Optional.Field "protocols" (Decode.list (Protocol.decoder options))
                Materials = get.Optional.Field "materials" (StudyMaterials.decoder options)
                Assays = get.Optional.Field "assays" (Decode.list (Assay.decoder options))
                Factors = get.Optional.Field "factors" (Decode.list (Factor.decoder options))
                CharacteristicCategories = get.Optional.Field "characteristicCategories" (Decode.list (MaterialAttribute.decoder options))
                UnitCategories = get.Optional.Field "unitCategories" (Decode.list (OntologyAnnotation.decoder options))
                ProcessSequence = get.Optional.Field "processSequence" (Decode.list (Process.decoder options))
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
            }
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s

    let toJsonString (p:Study) = 
        encoder (ConverterOptions()) p
        |> GEncode.toJsonString 2

    /// exports in json-ld format
    let toJsonldString (s:Study) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) s
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:Study) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true,IncludeContext=true)) a
        |> GEncode.toJsonString 2