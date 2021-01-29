namespace ISADotNet

open System.Text.Json.Serialization

type StudyMaterials =
    {   [<JsonPropertyName(@"sources")>]
        Sources : Source list option
        [<JsonPropertyName(@"samples")>]
        Samples : Sample list option
        [<JsonPropertyName(@"otherMaterials")>]
        OtherMaterials : Material list option
    }

    static member create sources samples otherMaterials =
        {
            Sources = sources
            Samples = samples
            OtherMaterials = otherMaterials           
        }

    static member Empty =
        StudyMaterials.create None None None


type Study = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"filename")>]
        FileName : string option
        [<JsonPropertyName(@"identifier")>]
        Identifier : string option
        [<JsonPropertyName(@"title")>]
        Title : string option
        [<JsonPropertyName(@"description")>]
        Description : string option
        [<JsonPropertyName(@"submissionDate")>]
        SubmissionDate : string option
        [<JsonPropertyName(@"publicReleaseDate")>]
        PublicReleaseDate : string option
        [<JsonPropertyName(@"publications")>]
        Publications : Publication list option
        [<JsonPropertyName(@"people")>]
        Contacts : Person list option
        [<JsonPropertyName(@"studyDesignDescriptors")>]
        StudyDesignDescriptors : OntologyAnnotation list option
        [<JsonPropertyName(@"protocols")>]
        Protocols : Protocol list option
        [<JsonPropertyName(@"materials")>]
        Materials : StudyMaterials option
        [<JsonPropertyName(@"processSequence")>]
        ProcessSequence : Process list option
        [<JsonPropertyName(@"assays")>]
        Assays : Assay list option
        [<JsonPropertyName(@"factors")>]
        Factors : Factor list option
        /// List of all the characteristics categories (or material attributes) defined in the study, used to avoid duplication of their declaration when each material_attribute_value is created. 
        [<JsonPropertyName(@"characteristicCategories")>]
        CharacteristicCategories : MaterialAttribute list option
        /// List of all the unitsdefined in the study, used to avoid duplication of their declaration when each value is created.
        [<JsonPropertyName(@"unitCategories")>]
        UnitCategories : OntologyAnnotation list option
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list option
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



