namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper
open Conversion
open System.IO

module Study =
    
    let encoder (study:ArcStudy) = 
        Encode.object [ 
            "Identifier", Encode.string study.Identifier
            Encode.tryInclude "Title" Encode.string study.Title
            Encode.tryInclude "Description" Encode.string study.Description
            Encode.tryInclude "SubmissionDate" Encode.string study.SubmissionDate
            Encode.tryInclude "PublicReleaseDate" Encode.string study.PublicReleaseDate
            Encode.tryIncludeSeq "Publications" Publication.encoder study.Publications
            Encode.tryIncludeSeq "Contacts" Person.encoder study.Contacts
            Encode.tryIncludeSeq "StudyDesignDescriptors" OntologyAnnotation.encoder study.StudyDesignDescriptors
            Encode.tryIncludeSeq "Tables" ArcTable.encoder study.Tables
            Encode.tryIncludeSeq "RegisteredAssayIdentifiers" Encode.string study.RegisteredAssayIdentifiers
            Encode.tryIncludeSeq "Comments" Comment.encoder study.Comments
        ]
  
    let decoder : Decoder<ArcStudy> =
        Decode.object (fun get ->
            ArcStudy(
                get.Required.Field "Identifier" Decode.string,
                ?title = get.Optional.Field "Title" Decode.string,
                ?description = get.Optional.Field "Description" Decode.string,
                ?submissionDate = get.Optional.Field "SubmissionDate" Decode.string,
                ?publicReleaseDate = get.Optional.Field "PublicReleaseDate" Decode.string,
                ?publications = get.Optional.Field "Publications" (Decode.resizeArray Publication.decoder),
                ?contacts = get.Optional.Field "Contacts" (Decode.resizeArray Person.decoder),
                ?studyDesignDescriptors = get.Optional.Field "StudyDesignDescriptors" (Decode.resizeArray OntologyAnnotation.decoder),
                ?tables = get.Optional.Field "Tables" (Decode.resizeArray ArcTable.decoder) ,
                ?registeredAssayIdentifiers = get.Optional.Field "RegisteredAssayIdentifiers" (Decode.resizeArray Decode.string),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            ) 
    )

    open OATable
    open CellTable

    let encoderCompressed (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (study:ArcStudy) =
        Encode.object [ 
            "Identifier", Encode.string study.Identifier
            Encode.tryInclude "Title" Encode.string study.Title
            Encode.tryInclude "Description" Encode.string study.Description
            Encode.tryInclude "SubmissionDate" Encode.string study.SubmissionDate
            Encode.tryInclude "PublicReleaseDate" Encode.string study.PublicReleaseDate
            Encode.tryIncludeSeq "Publications" Publication.encoder study.Publications
            Encode.tryIncludeSeq "Contacts" Person.encoder study.Contacts
            Encode.tryIncludeSeq "StudyDesignDescriptors" OntologyAnnotation.encoder study.StudyDesignDescriptors
            Encode.tryIncludeSeq "Tables" (ArcTable.encoderCompressed stringTable oaTable cellTable) study.Tables
            Encode.tryIncludeSeq "RegisteredAssayIdentifiers" Encode.string study.RegisteredAssayIdentifiers
            Encode.tryIncludeSeq "Comments" Comment.encoder study.Comments
        ]

    let decoderCompressed (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray) : Decoder<ArcStudy> =
        Decode.object (fun get ->
            ArcStudy(
                get.Required.Field "Identifier" Decode.string,
                ?title = get.Optional.Field "Title" Decode.string,
                ?description = get.Optional.Field "Description" Decode.string,
                ?submissionDate = get.Optional.Field "SubmissionDate" Decode.string,
                ?publicReleaseDate = get.Optional.Field "PublicReleaseDate" Decode.string,
                ?publications = get.Optional.Field "Publications" (Decode.resizeArray Publication.decoder),
                ?contacts = get.Optional.Field "Contacts" (Decode.resizeArray Person.decoder),
                ?studyDesignDescriptors = get.Optional.Field "StudyDesignDescriptors" (Decode.resizeArray OntologyAnnotation.decoder),
                ?tables = get.Optional.Field "Tables" (Decode.resizeArray <| ArcTable.decoderCompressed stringTable oaTable cellTable),
                ?registeredAssayIdentifiers = get.Optional.Field "RegisteredAssayIdentifiers" (Decode.resizeArray Decode.string),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            ) 
        )

    module ROCrate = 

        let genID (a:ArcStudy) : string = 
            match a.Identifier with
            | "" -> "#EmptyStudy"
            | i -> 
                let identifier = i.Replace(" ","_")
                $"#study/{identifier}"
    
    //    let encoder (options : ConverterOptions) (s : Study) = 
    //        [
    //            if options.SetID then 
    //                "@id", Encode.string (s |> genID)
    //            else 
    //                Encode.tryInclude "@id" Encode.string (s.ID)
    //            if options.IsJsonLD then 
    //                "@type", (Encode.list [Encode.string "Study"])
    //                "additionalType", Encode.string "Study"
    //            Encode.tryInclude "filename" Encode.string (s.FileName)
    //            Encode.tryInclude "identifier" Encode.string (s.Identifier)
    //            Encode.tryInclude "title" Encode.string (s.Title)
    //            Encode.tryInclude "description" Encode.string (s.Description)
    //            Encode.tryInclude "submissionDate" Encode.string (s.SubmissionDate)
    //            Encode.tryInclude "publicReleaseDate" Encode.string (s.PublicReleaseDate)
    //            Encode.tryIncludeList "publications" (Publication.encoder options) (s.Publications)
    //            Encode.tryIncludeList "people" (Person.encoder options) (s.Contacts)
    //            if not options.IsJsonLD then
    //                Encode.tryIncludeList "studyDesignDescriptors" (OntologyAnnotation.encoder options) (s.StudyDesignDescriptors) 
    //                Encode.tryIncludeList "protocols" (Protocol.encoder options None None None) (s.Protocols)
    //                Encode.tryInclude "materials" (StudyMaterials.encoder options) (s.Materials)
    //                Encode.tryIncludeList "factors" (Factor.encoder options) (s.Factors)
    //                Encode.tryIncludeList "characteristicCategories" (MaterialAttribute.encoder options) (s.CharacteristicCategories)            
    //                Encode.tryIncludeList "unitCategories" (OntologyAnnotation.encoder options) (s.UnitCategories)
    //            // if options.IsJsonLD then
    //            //     match s.Materials with
    //            //     | Some m -> 
    //            //         Encode.tryIncludeList "samples" (Sample.encoder options) (m.Samples)
    //            //         Encode.tryIncludeList "sources" (Source.encoder options) (m.Sources)
    //            //         Encode.tryIncludeList "materials" (Material.encoder options) (m.OtherMaterials)
    //            //     | None -> ()
    //            Encode.tryIncludeList "processSequence" (Process.encoder options s.Identifier None) (s.ProcessSequence)
    //            Encode.tryIncludeList "assays" (Assay.encoder options s.Identifier) (s.Assays)            
    //            Encode.tryIncludeList "comments" (Comment.encoder options) (s.Comments)
    //            if options.IsJsonLD then 
    //                "@context", ROCrateContext.Study.context_jsonvalue
    //        ]
    //        |> Encode.choose
    //        |> Encode.object

    //    let allowedFields = ["@id";"filename";"identifier";"title";"description";"submissionDate";"publicReleaseDate";"publications";"people";"studyDesignDescriptors";"protocols";"materials";"assays";"factors";"characteristicCategories";"unitCategories";"processSequence";"comments";"@type"; "@context"]

    //    let decoder (options : ConverterOptions) : Decoder<Study> =
    //        GDecode.object allowedFields (fun get ->
    //            {
    //                ID = get.Optional.Field "@id" GDecode.uri
    //                FileName = get.Optional.Field "filename" Decode.string
    //                Identifier = get.Optional.Field "identifier" Decode.string
    //                Title = get.Optional.Field "title" Decode.string
    //                Description = get.Optional.Field "description" Decode.string
    //                SubmissionDate = get.Optional.Field "submissionDate" Decode.string
    //                PublicReleaseDate = get.Optional.Field "publicReleaseDate" Decode.string
    //                Publications = get.Optional.Field "publications" (Decode.list (Publication.decoder options))
    //                Contacts = get.Optional.Field "people" (Decode.list (Person.decoder options))
    //                StudyDesignDescriptors = get.Optional.Field "studyDesignDescriptors" (Decode.list (OntologyAnnotation.decoder options))
    //                Protocols = get.Optional.Field "protocols" (Decode.list (Protocol.decoder options))
    //                Materials = get.Optional.Field "materials" (StudyMaterials.decoder options)
    //                Assays = get.Optional.Field "assays" (Decode.list (Assay.decoder options))
    //                Factors = get.Optional.Field "factors" (Decode.list (Factor.decoder options))
    //                CharacteristicCategories = get.Optional.Field "characteristicCategories" (Decode.list (MaterialAttribute.decoder options))
    //                UnitCategories = get.Optional.Field "unitCategories" (Decode.list (OntologyAnnotation.decoder options))
    //                ProcessSequence = get.Optional.Field "processSequence" (Decode.list (Process.decoder options))
    //                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
    //            }
    //        )

    module ISAJson =

        let encoder (s : ArcStudy) = 
            let fileName = Identifier.Assay.fileNameFromIdentifier s.Identifier
            let processes = s.GetProcesses()
            let protocols = ProcessSequence.getProtocols processes
            let factors = ProcessSequence.getFactors processes
            let characteristics = ProcessSequence.getCharacteristics processes
            let units = ProcessSequence.getUnits processes
            [
                "filename", Encode.string fileName
                "identifier", Encode.string s.Identifier
                Encode.tryInclude "title" Encode.string (s.Title)
                Encode.tryInclude "description" Encode.string (s.Description)
                Encode.tryInclude "submissionDate" Encode.string (s.SubmissionDate)
                Encode.tryInclude "publicReleaseDate" Encode.string (s.PublicReleaseDate)
                Encode.tryIncludeSeq "publications" Publication.ISAJson.encoder s.Publications
                Encode.tryIncludeSeq "people" Person.ISAJson.encoder s.Contacts
                Encode.tryIncludeSeq "studyDesignDescriptors" OntologyAnnotation.ISAJson.encoder (s.StudyDesignDescriptors) 
                Encode.tryIncludeList "protocols" Protocol.ISAJson.encoder protocols
                Encode.tryInclude "materials" StudyMaterials.ISAJson.encoder (Option.fromValueWithDefault [] processes)
                Encode.tryIncludeList "factors" Factor.ISAJson.encoder factors
                Encode.tryIncludeList "characteristicCategories" MaterialAttribute.ISAJson.encoder characteristics     
                Encode.tryIncludeList "unitCategories" OntologyAnnotation.ISAJson.encoder units
                Encode.tryIncludeList "processSequence" Process.ISAJson.encoder processes
                Encode.tryIncludeSeq "assays" Assay.ISAJson.encoder s.RegisteredAssays       
                Encode.tryIncludeSeq "comments" Comment.ISAJson.encoder s.Comments
            ]
            |> Encode.choose
            |> Encode.object

        let allowedFields = ["@id";"filename";"identifier";"title";"description";"submissionDate";"publicReleaseDate";"publications";"people";"studyDesignDescriptors";"protocols";"materials";"assays";"factors";"characteristicCategories";"unitCategories";"processSequence";"comments";"@type"; "@context"]
        
        let decoder : Decoder<ArcStudy*ArcAssay list> =
            Decode.objectNoAdditionalProperties allowedFields (fun get ->
                let identifier = 
                    get.Optional.Field "filename" Decode.string
                    |> Option.bind Identifier.Study.tryIdentifierFromFileName
                    |> Option.defaultValue (Identifier.createMissingIdentifier())
                let tables = 
                    get.Optional.Field "processSequence" (Decode.list Process.ISAJson.decoder)
                    |> Option.map (ArcTables.fromProcesses >> (fun a -> a.Tables))
                let assays = 
                    get.Optional.Field "assays" (Decode.list Assay.ISAJson.decoder)
                let assayIdentifiers = 
                    assays
                    |> Option.map (List.map (fun a -> a.Identifier) >> ResizeArray)
                ArcStudy(
                    identifier,
                    ?title = get.Optional.Field "title" Decode.string,
                    ?description = get.Optional.Field "description" Decode.string,
                    ?submissionDate = get.Optional.Field "submissionDate" Decode.string,
                    ?publicReleaseDate = get.Optional.Field "publicReleaseDate" Decode.string,
                    ?publications = get.Optional.Field "publications" (Decode.resizeArray Publication.ISAJson.decoder),
                    ?contacts = get.Optional.Field "people" (Decode.resizeArray Person.ISAJson.decoder),
                    ?studyDesignDescriptors = get.Optional.Field "studyDesignDescriptors" (Decode.resizeArray OntologyAnnotation.ISAJson.decoder),
                    ?tables = tables,
                    ?registeredAssayIdentifiers = assayIdentifiers,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ISAJson.decoder)
                ), assays |> Option.defaultValue []
            )
