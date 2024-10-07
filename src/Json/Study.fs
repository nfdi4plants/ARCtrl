namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper
open Conversion

module Study =
    
    module Helper =
        /// <summary>
        /// Get registered assays or get assays from `assays` if IsSome
        /// </summary>
        /// <param name="assays"></param>
        /// <param name="study"></param>
        let getAssayInformation (assays: ArcAssay list option) (study: ArcStudy) =
            if assays.IsSome then
                study.RegisteredAssayIdentifiers 
                |> ResizeArray.map (fun assayId ->
                    assays.Value 
                    |> List.tryFind (fun a -> a.Identifier = assayId)
                    |> Option.defaultValue (ArcAssay.init(assayId))
                )
            else
                study.GetRegisteredAssaysOrIdentifier()

    let encoder (study:ArcStudy) = 
        [ 
            "Identifier", Encode.string study.Identifier  |> Some
            Encode.tryInclude "Title" Encode.string study.Title
            Encode.tryInclude "Description" Encode.string study.Description
            Encode.tryInclude "SubmissionDate" Encode.string study.SubmissionDate
            Encode.tryInclude "PublicReleaseDate" Encode.string study.PublicReleaseDate
            Encode.tryIncludeSeq "Publications" Publication.encoder study.Publications
            Encode.tryIncludeSeq "Contacts" Person.encoder study.Contacts
            Encode.tryIncludeSeq "StudyDesignDescriptors" OntologyAnnotation.encoder study.StudyDesignDescriptors
            Encode.tryIncludeSeq "Tables" ArcTable.encoder study.Tables
            Encode.tryInclude "DataMap" DataMap.encoder study.DataMap
            Encode.tryIncludeSeq "RegisteredAssayIdentifiers" Encode.string study.RegisteredAssayIdentifiers
            Encode.tryIncludeSeq "Comments" Comment.encoder study.Comments
        ]
        |> Encode.choose
        |> Encode.object
  
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
                ?tables = get.Optional.Field "Tables" (Decode.resizeArray ArcTable.decoder),
                ?datamap = get.Optional.Field "DataMap" DataMap.decoder,
                ?registeredAssayIdentifiers = get.Optional.Field "RegisteredAssayIdentifiers" (Decode.resizeArray Decode.string),
                ?comments = get.Optional.Field "Comments" (Decode.resizeArray Comment.decoder)
            ) 
    )

    open OATable
    open CellTable
    open StringTable

    let encoderCompressed (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (study:ArcStudy) =
        [ 
            "Identifier", Encode.string study.Identifier  |> Some
            Encode.tryInclude "Title" Encode.string study.Title
            Encode.tryInclude "Description" Encode.string study.Description
            Encode.tryInclude "SubmissionDate" Encode.string study.SubmissionDate
            Encode.tryInclude "PublicReleaseDate" Encode.string study.PublicReleaseDate
            Encode.tryIncludeSeq "Publications" Publication.encoder study.Publications
            Encode.tryIncludeSeq "Contacts" Person.encoder study.Contacts
            Encode.tryIncludeSeq "StudyDesignDescriptors" OntologyAnnotation.encoder study.StudyDesignDescriptors
            Encode.tryIncludeSeq "Tables" (ArcTable.encoderCompressed stringTable oaTable cellTable) study.Tables
            Encode.tryInclude "DataMap" (DataMap.encoderCompressed stringTable oaTable cellTable) study.DataMap
            Encode.tryIncludeSeq "RegisteredAssayIdentifiers" Encode.string study.RegisteredAssayIdentifiers
            Encode.tryIncludeSeq "Comments" Comment.encoder study.Comments
        ]
        |> Encode.choose
        |> Encode.object

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
                ?datamap = get.Optional.Field "DataMap" (DataMap.decoderCompressed stringTable oaTable cellTable),
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
    
        let encoder (assays: ArcAssay list option) (s : ArcStudy) = 
            let fileName = Identifier.Study.tryFileNameFromIdentifier s.Identifier
            let processes = s.GetProcesses()
            let assays = Helper.getAssayInformation assays s
            [
                "@id", Encode.string (s |> genID) |> Some
                "@type", (Encode.list [Encode.string "Study"])  |> Some
                "additionalType", Encode.string "Study" |> Some
                "identifier", Encode.string (s.Identifier) |> Some
                Encode.tryInclude "filename" Encode.string fileName
                Encode.tryInclude "title" Encode.string (s.Title)
                Encode.tryInclude "description" Encode.string (s.Description)
                Encode.tryIncludeSeq "studyDesignDescriptors" OntologyAnnotation.ROCrate.encoderDefinedTerm (s.StudyDesignDescriptors)
                Encode.tryInclude "submissionDate" Encode.string (s.SubmissionDate)
                Encode.tryInclude "publicReleaseDate" Encode.string (s.PublicReleaseDate)
                Encode.tryIncludeSeq "publications" Publication.ROCrate.encoder s.Publications
                Encode.tryIncludeSeq "people" Person.ROCrate.encoder s.Contacts
                Encode.tryIncludeList "processSequence" (Process.ROCrate.encoder (Some s.Identifier) None) processes
                Encode.tryIncludeSeq "assays" (Assay.ROCrate.encoder (Some s.Identifier)) assays           
                Encode.tryIncludeSeq "comments" Comment.ROCrate.encoder s.Comments
                "@context", ROCrateContext.Study.context_jsonvalue |> Some
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<ArcStudy*ArcAssay list> =
            Decode.object (fun get ->             
                let identifier = 
                    get.Optional.Field "filename" Decode.string
                    |> Option.bind Identifier.Study.tryIdentifierFromFileName
                    |> Option.defaultValue (Identifier.createMissingIdentifier())
                let assays = 
                    get.Optional.Field "assays" (Decode.list Assay.ROCrate.decoder)
                let assayIdentifiers = 
                    assays 
                    |> Option.map (List.map (fun a -> a.Identifier) >> ResizeArray)
                let tables = 
                    get.Optional.Field "processSequence" (Decode.list Process.ROCrate.decoder)
                    |> Option.map (fun ps -> ArcTables.fromProcesses(ps).Tables)
                ArcStudy(
                    identifier,
                    ?title = get.Optional.Field "title" Decode.string,
                    ?description = get.Optional.Field "description" Decode.string,
                    ?submissionDate = get.Optional.Field "submissionDate" Decode.string,
                    ?publicReleaseDate = get.Optional.Field "publicReleaseDate" Decode.string,
                    ?publications = get.Optional.Field "publications" (Decode.resizeArray Publication.ROCrate.decoder),
                    ?contacts = get.Optional.Field "people" (Decode.resizeArray Person.ROCrate.decoder),
                    ?studyDesignDescriptors = get.Optional.Field "studyDesignDescriptors" (Decode.resizeArray OntologyAnnotation.ROCrate.decoderDefinedTerm),
                    ?tables = tables,
                    ?registeredAssayIdentifiers = assayIdentifiers,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ROCrate.decoder)
                ), assays |> Option.defaultValue [] 
            )

    module ISAJson =

        /// <summary>
        /// If assays.IsSome then try to get registered assays from external list, otherwise try to access investigation or create empty defaults.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="assays"></param>
        let encoder idMap (assays: ArcAssay list option) (s : ArcStudy) = 
            let f (s : ArcStudy) =
                let study = s.Copy(true)
                let fileName = Identifier.Study.fileNameFromIdentifier study.Identifier
                let assaysRaw = Helper.getAssayInformation assays study
                let assays = 
                    let n = ResizeArray()
                    for a in assaysRaw do 
                        let assay = a.Copy()
                        // Move persons to study
                        for person in assay.Performers do
                            // set source assay identifier as comment
                            let person = Process.Conversion.Person.setSourceAssayComment person assay.Identifier
                            study.Contacts.Add(person)
                        assay.Performers <- ResizeArray()
                        n.Add(assay)
                    n
                let processes = study.GetProcesses()
                let encodedUnits = 
                    ProcessSequence.getUnits processes
                    |> Encode.tryIncludeList "unitCategories" (OntologyAnnotation.ISAJson.encoder idMap) 
                let encodedFactors = 
                    ProcessSequence.getFactors processes
                    |> Encode.tryIncludeList "factors" (Factor.ISAJson.encoder idMap)
                let encodedCharacteristics =
                    ProcessSequence.getCharacteristics processes
                    |> Encode.tryIncludeList "characteristicCategories" (MaterialAttribute.ISAJson.encoder idMap)
                let encodedMaterials = 
                    Encode.tryInclude "materials" (StudyMaterials.ISAJson.encoder idMap) (Option.fromValueWithDefault [] processes)
                let encodedProtocols = 
                    ProcessSequence.getProtocols processes
                    |> Encode.tryIncludeList "protocols" (Protocol.ISAJson.encoder (Some s.Identifier) None None idMap)
                [
                    "@id", Encode.string (study |> ROCrate.genID) |> Some
                    "filename", Encode.string fileName |> Some
                    "identifier", Encode.string study.Identifier |> Some
                    Encode.tryInclude "title" Encode.string study.Title
                    Encode.tryInclude "description" Encode.string study.Description
                    Encode.tryInclude "submissionDate" Encode.string study.SubmissionDate
                    Encode.tryInclude "publicReleaseDate" Encode.string study.PublicReleaseDate
                    Encode.tryIncludeSeq "publications" (Publication.ISAJson.encoder idMap) study.Publications
                    Encode.tryIncludeSeq "people" (Person.ISAJson.encoder idMap) study.Contacts
                    Encode.tryIncludeSeq "studyDesignDescriptors" (OntologyAnnotation.ISAJson.encoder idMap) study.StudyDesignDescriptors
                    encodedProtocols
                    encodedMaterials
                    Encode.tryIncludeList "processSequence" (Process.ISAJson.encoder (Some s.Identifier) None idMap) processes
                    Encode.tryIncludeSeq "assays" (Assay.ISAJson.encoder (Some s.Identifier) idMap) assays
                    encodedFactors
                    encodedCharacteristics
                    encodedUnits
                    Encode.tryIncludeSeq "comments" (Comment.ISAJson.encoder idMap) study.Comments
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f s
            | Some idMap -> IDTable.encode (fun s -> ROCrate.genID s) f s idMap

        let allowedFields = ["@id";"filename";"identifier";"title";"description";"submissionDate";"publicReleaseDate";"publications";"people";"studyDesignDescriptors";"protocols";"materials";"assays";"factors";"characteristicCategories";"unitCategories";"processSequence";"comments";"@type"; "@context"]
        
        let decoder : Decoder<ArcStudy*ArcAssay list> =
            Decode.objectNoAdditionalProperties allowedFields (fun get ->
                let identifier = 
                    get.Optional.Field "identifier" Decode.string
                    |> Option.defaultWith (fun () ->
                        get.Optional.Field "filename" Decode.string 
                        |> Option.bind Identifier.Study.tryIdentifierFromFileName
                        |> Option.defaultValue (Identifier.createMissingIdentifier())
                    )
                let tables = 
                    get.Optional.Field "processSequence" (Decode.list Process.ISAJson.decoder)
                    |> Option.map (ArcTables.fromProcesses >> (fun a -> a.Tables))
                let assays = 
                    get.Optional.Field "assays" (Decode.list Assay.ISAJson.decoder)
                let personsRaw = get.Optional.Field "people" (Decode.resizeArray Person.ISAJson.decoder)
                // Set
                let persons = ResizeArray()
                if personsRaw.IsSome then
                    for person in personsRaw.Value do
                        // Try to find if any person, written by ARCtrl, was originally part of ArcAssay
                        let sourceAssays = Process.Conversion.Person.getSourceAssayIdentifiersFromComments person
                        // For every found assay identifier ...
                        for assayIdentifier in sourceAssays do
                            // ... we add the person to ArcAssay.Perfomers if identifiers match.
                            for assay in assays.Value do
                                if assay.Identifier = assayIdentifier then 
                                    assay.Performers.Add person
                        // After completing the backtracking to source assays we remove all comments on the person, which start with the defined AssayIdentifierPrefix
                        person.Comments <- Process.Conversion.Person.removeSourceAssayComments person
                        // If no source assay identifiers are found, we assume the person was orginally part of the study and we add the person to the future study.contacts array
                        if Seq.isEmpty sourceAssays then persons.Add person
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
                    ?contacts = (if persons.Count = 0 then None else Some persons),
                    ?studyDesignDescriptors = get.Optional.Field "studyDesignDescriptors" (Decode.resizeArray OntologyAnnotation.ISAJson.decoder),
                    ?tables = tables,
                    ?registeredAssayIdentifiers = assayIdentifiers,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ISAJson.decoder)
                ), assays |> Option.defaultValue []
            )