namespace ISADotNet

open System.Text.Json.Serialization

type AssayMaterials =
    {
        [<JsonPropertyName(@"samples")>]
        Samples : Sample list option
        [<JsonPropertyName(@"otherMaterials")>]
        OtherMaterials : Material list option
    }

    static member make samples otherMaterials =
        {
            Samples = samples
            OtherMaterials = otherMaterials       
        }

    static member create(?Samples,?OtherMaterials) : AssayMaterials =
        AssayMaterials.make Samples OtherMaterials

    static member empty =
        AssayMaterials.create()


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

    static member make id fileName measurementType technologyType technologyPlatform dataFiles materials characteristicCategories unitCategories processSequence comments =
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

    static member create (?Id,?FileName,?MeasurementType,?TechnologyType,?TechnologyPlatform,?DataFiles,?Materials,?CharacteristicCategories,?UnitCategories,?ProcessSequence,?Comments) : Assay =
        Assay.make Id FileName MeasurementType TechnologyType TechnologyPlatform DataFiles Materials CharacteristicCategories UnitCategories ProcessSequence Comments

    static member empty =
        Assay.create()
