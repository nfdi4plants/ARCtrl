namespace ISA

type Study = 
    {
        ID : URI option
        FileName : string option
        Identifier : string option
        Title : string option
        Description : string option
        SubmissionDate : string option
        PublicReleaseDate : string option
        Publications : Publication list option
        Contacts : Person list option
        StudyDesignDescriptors : OntologyAnnotation list option
        Protocols : Protocol list option
        Materials : StudyMaterials option
        ProcessSequence : Process list option
        Assays : Assay list option
        Factors : Factor list option
        /// List of all the characteristics categories (or material attributes) defined in the study, used to avoid duplication of their declaration when each material_attribute_value is created. 
        CharacteristicCategories : MaterialAttribute list option
        /// List of all the unitsdefined in the study, used to avoid duplication of their declaration when each value is created.
        UnitCategories : OntologyAnnotation list option
        Comments : Comment list option
    }

    static member make id filename identifier title description submissionDate publicReleaseDate publications contacts studyDesignDescriptors protocols materials processSequence assays factors characteristicCategories unitCategories comments : Study=
        {
            ID                          = id
            FileName                    = filename
            Identifier                  = identifier
            Title                       = title
            Description                 = description
            SubmissionDate              = submissionDate
            PublicReleaseDate           = publicReleaseDate
            Publications                = publications
            Contacts                    = contacts
            StudyDesignDescriptors      = studyDesignDescriptors
            Protocols                   = protocols
            Materials                   = materials
            ProcessSequence             = processSequence
            Assays                      = assays
            Factors                     = factors
            CharacteristicCategories    = characteristicCategories
            UnitCategories              = unitCategories
            Comments                    = comments
        }

    static member create(?Id,?FileName,?Identifier,?Title,?Description,?SubmissionDate,?PublicReleaseDate,?Publications,?Contacts,?StudyDesignDescriptors,?Protocols,?Materials,?ProcessSequence,?Assays,?Factors,?CharacteristicCategories,?UnitCategories,?Comments) : Study=
        Study.make Id FileName Identifier Title Description SubmissionDate PublicReleaseDate Publications Contacts StudyDesignDescriptors Protocols Materials ProcessSequence Assays Factors CharacteristicCategories UnitCategories Comments

    static member empty =
        Study.create ()
