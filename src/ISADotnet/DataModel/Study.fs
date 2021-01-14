namespace ISADotNet

open System.Text.Json.Serialization

type StudyMaterials =
    {   [<JsonPropertyName(@"sources")>]
        Sources : Source list
        [<JsonPropertyName(@"samples")>]
        Samples : Sample list
        [<JsonPropertyName(@"otherMaterials")>]
        OtherMaterials : Material list    
    }

    static member create sources samples otherMaterials =
        {
            Sources = sources
            Samples = samples
            OtherMaterials = otherMaterials           
        }


type Study = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : string
        [<JsonPropertyName(@"filename")>]
        FileName : string
        [<JsonPropertyName(@"identifier")>]
        Identifier : string
        [<JsonPropertyName(@"title")>]
        Title : string 
        [<JsonPropertyName(@"description")>]
        Description : string
        [<JsonPropertyName(@"submissionDate")>]
        SubmissionDate : string
        [<JsonPropertyName(@"publicReleaseDate")>]
        PublicReleaseDate : string
        [<JsonPropertyName(@"publications")>]
        Publications : Publication list
        [<JsonPropertyName(@"people")>]
        Contacts : Person list
        [<JsonPropertyName(@"studyDesignDescriptors")>]
        StudyDesignDescriptors : OntologyAnnotation list
        [<JsonPropertyName(@"protocols")>]
        Protocols : Protocol list
        [<JsonPropertyName(@"materials")>]
        Materials : StudyMaterials
        [<JsonPropertyName(@"processSequence")>]
        ProcessSequence : Process list
        [<JsonPropertyName(@"assays")>]
        Assays : Assay list
        [<JsonPropertyName(@"factors")>]
        Factors : Factor list
        /// List of all the characteristics categories (or material attributes) defined in the study, used to avoid duplication of their declaration when each material_attribute_value is created. 
        [<JsonPropertyName(@"characteristicCategories")>]
        CharacteristicCategories : MaterialAttribute list
        /// List of all the unitsdefined in the study, used to avoid duplication of their declaration when each value is created.
        [<JsonPropertyName(@"unitCategories")>]
        UnitCategories : OntologyAnnotation list
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list
    }

    static member create id filename identifier title description submissionDate publicReleaseDate publications contacts studyDesignDescriptors protocols materials processSequence assays factors characteristicCategories unitCategories comments : Study=
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

    static member IdentifierTab = "Study Identifier"
    static member TitleTab = "Study Title"
    static member DescriptionTab = "Study Description"
    static member SubmissionDateTab = "Study Submission Date"
    static member PublicReleaseDateTab = "Study Public Release Date"
    static member FileNameTab = "Study File Name"

    static member DesignDescriptorsTab = "STUDY DESIGN DESCRIPTORS"
    static member PublicationsTab = "STUDY PUBLICATIONS"
    static member FactorsTab = "STUDY FACTORS"
    static member AssaysTab = "STUDY ASSAYS"
    static member ProtocolsTab = "STUDY PROTOCOLS"
    static member ContactsTab = "STUDY CONTACTS"

    static member DesignDescriptorsTabPrefix = "Study Design"
    static member PublicationsTabPrefix = "Study Publication"
    static member FactorsTabPrefix = "Study Factor"
    static member AssaysTabPrefix = "Study Assay"
    static member ProtocolsTabPrefix = "Study Protocol"
    static member ContactsTabPrefix = "Study Person"

    static member DesignTypeTab = "Type"
    static member DesignTypeTermAccessionNumberTab = "Type Term Accession Number"
    static member DesignTypeTermSourceREFTab = "Type Term Source REF"
