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

    static member create (?Sources,?Samples,?OtherMaterials) : StudyMaterials =
        {
            Sources         = Sources
            Samples         = Samples
            OtherMaterials  = OtherMaterials           
        }

    static member empty =
        StudyMaterials.create ()


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

    static member create(?Id,?FileName,?Identifier,?Title,?Description,?SubmissionDate,?PublicReleaseDate,?Publications,?Contacts,?StudyDesignDescriptors,?Protocols,?Materials,?ProcessSequence,?Assays,?Factors,?CharacteristicCategories,?UnitCategories,?Comments) : Study=
        {
            ID                          = Id
            FileName                    = FileName
            Identifier                  = Identifier
            Title                       = Title
            Description                 = Description
            SubmissionDate              = SubmissionDate
            PublicReleaseDate           = PublicReleaseDate
            Publications                = Publications
            Contacts                    = Contacts
            StudyDesignDescriptors      = StudyDesignDescriptors
            Protocols                   = Protocols
            Materials                   = Materials
            ProcessSequence             = ProcessSequence
            Assays                      = Assays
            Factors                     = Factors
            CharacteristicCategories    = CharacteristicCategories
            UnitCategories              = UnitCategories
            Comments                    = Comments
        }

    static member empty =
        Study.create ()
