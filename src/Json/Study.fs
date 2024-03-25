namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper
open Conversion

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
    open StringTable

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
    
        let encoder (s : ArcStudy) = 
            let fileName = Identifier.Study.tryFileNameFromIdentifier s.Identifier
            let processes = s.GetProcesses()
            [
                "@id", Encode.string (s |> genID)               
                "@type", (Encode.list [Encode.string "Study"])
                "additionalType", Encode.string "Study"
                "identifier", Encode.string (s.Identifier)
                Encode.tryInclude "filename" Encode.string fileName
                Encode.tryInclude "title" Encode.string (s.Title)
                Encode.tryInclude "description" Encode.string (s.Description)
                //Encode.tryIncludeSeq "studyDesignDescriptors" OntologyAnnotation.ROCrate.encodeDefinedTerm (s.StudyDesignDescriptors)
                Encode.tryInclude "submissionDate" Encode.string (s.SubmissionDate)
                Encode.tryInclude "publicReleaseDate" Encode.string (s.PublicReleaseDate)
                Encode.tryIncludeSeq "publications" Publication.ROCrate.encoder s.Publications
                Encode.tryIncludeSeq "people" Person.ROCrate.encoder s.Contacts
                Encode.tryIncludeList "processSequence" (Process.ROCrate.encoder (Some s.Identifier) None) processes
                Encode.tryIncludeSeq "assays" (Assay.ROCrate.encoder (Some s.Identifier)) s.RegisteredAssays           
                Encode.tryIncludeSeq "comments" Comment.ROCrate.encoder s.Comments
                "@context", ROCrateContext.Study.context_jsonvalue
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<ArcStudy*ResizeArray<ArcAssay>> =
            Decode.object (fun get ->             
                let identifier = 
                    get.Optional.Field "filename" Decode.string
                    |> Option.bind Identifier.Study.tryIdentifierFromFileName
                    |> Option.defaultValue (Identifier.createMissingIdentifier())
                let assays = 
                    get.Optional.Field "assays" (Decode.resizeArray Assay.ROCrate.decoder)
                    |> Option.defaultValue (ResizeArray [])
                let assayIdentifiers = 
                    assays
                    |> ResizeArray.map (fun a -> a.Identifier)
                let tables = 
                    get.Optional.Field "tables" (Decode.list Process.ROCrate.decoder)
                    |> Option.map (fun ps -> ArcTables.fromProcesses(ps).Tables)
                ArcStudy(
                    identifier,
                    ?title = get.Optional.Field "title" Decode.string,
                    ?description = get.Optional.Field "description" Decode.string,
                    ?submissionDate = get.Optional.Field "submissionDate" Decode.string,
                    ?publicReleaseDate = get.Optional.Field "publicReleaseDate" Decode.string,
                    ?publications = get.Optional.Field "publications" (Decode.resizeArray Publication.ROCrate.decoder),
                    ?contacts = get.Optional.Field "people" (Decode.resizeArray Person.ROCrate.decoder),
                    //?studyDesignDescriptors = get.Optional.Field "studyDesignDescriptors" (Decode.resizeArray OntologyAnnotation.ROCrate.decoderDefinedTerm),
                    ?tables = tables,
                    registeredAssayIdentifiers = assayIdentifiers,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ROCrate.decoder)
                ),
                assays
            )

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


[<AutoOpen>]
module StudyExtensions =

    open System.Collections.Generic

    type ArcStudy with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Study.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:ArcStudy) ->
                Study.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromCompressedJsonString (s: string) =
            try Decode.fromJsonString (Compression.decode Study.decoderCompressed) s with
            | e -> failwithf "Error. Unable to parse json string to ArcStudy: %s" e.Message

        static member toCompressedJsonString(?spaces) =
            fun (obj:ArcStudy) ->
                let spaces = defaultArg spaces 0
                Encode.toJsonString spaces (Compression.encode Study.encoderCompressed obj)

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString Study.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:ArcStudy) ->
                Study.ROCrate.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member toISAJsonString(?spaces) =
            fun (obj:ArcStudy) ->
                Study.ISAJson.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Study.ISAJson.decoder s