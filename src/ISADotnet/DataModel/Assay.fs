namespace ISADotNet

open System.Text.Json.Serialization

type AssayMaterials =
    {
        [<JsonPropertyName(@"samples")>]
        Samples : Sample list option
        [<JsonPropertyName(@"otherMaterials")>]
        OtherMaterials : Material list option
    }

    static member create samples otherMaterials =
        {
            Samples = samples
            OtherMaterials = otherMaterials       
        }

    static member empty =
        AssayMaterials.create None None

type Assay = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"filename")>]
        FileName : string option
        [<JsonPropertyName(@"measurementType")>]
        MeasurementType : OntologyAnnotation option
        [<JsonPropertyName(@"technologyType")>]
        TechnologyType : OntologyAnnotation option
        [<JsonPropertyName(@"technologyPlatform")>]
        TechnologyPlatform : string option
        [<JsonPropertyName(@"dataFiles")>]
        DataFiles : Data list option
        [<JsonPropertyName(@"materials")>]
        Materials : AssayMaterials option
        /// List of all the characteristics categories (or material attributes) defined in the study, used to avoid duplication of their declaration when each material_attribute_value is created. 
        [<JsonPropertyName(@"characteristicCategories")>]
        CharacteristicCategories : MaterialAttribute list option
        /// List of all the unitsdefined in the study, used to avoid duplication of their declaration when each value is created.
        [<JsonPropertyName(@"unitCategories")>]
        UnitCategories : OntologyAnnotation list option
        [<JsonPropertyName(@"processSequence")>]
        ProcessSequence : Process list option
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list option
    }


    static member create id fileName measurementType technologyType technologyPlatform dataFiles materials characteristicCategories unitCategories processSequence comments =
        {
            ID                          = id
            FileName                    = fileName
            MeasurementType             = measurementType
            TechnologyType              = technologyType
            TechnologyPlatform          = technologyPlatform
            DataFiles                   = dataFiles
            Materials                   = materials
            CharacteristicCategories    = characteristicCategories
            UnitCategories              = unitCategories
            ProcessSequence             = processSequence
            Comments                    = comments
        }

    static member empty =
        Assay.create None None None None None None None None None None None