namespace ISADotNet

open System.Text.Json.Serialization

type AssayMaterials =
    {
        [<JsonPropertyName(@"samples")>]
        Samples : Sample list option
        [<JsonPropertyName(@"otherMaterials")>]
        OtherMaterials : Material list option
    }

    static member create(?Samples,?OtherMaterials) : AssayMaterials =
        {
            Samples         = Samples
            OtherMaterials  = OtherMaterials       
        }

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


    static member create (?Id,?FileName,?MeasurementType,?TechnologyType,?TechnologyPlatform,?DataFiles,?Materials,?CharacteristicCategories,?UnitCategories,?ProcessSequence,?Comments) : Assay =
        {
            ID                          = Id
            FileName                    = FileName
            MeasurementType             = MeasurementType
            TechnologyType              = TechnologyType
            TechnologyPlatform          = TechnologyPlatform
            DataFiles                   = DataFiles
            Materials                   = Materials
            CharacteristicCategories    = CharacteristicCategories
            UnitCategories              = UnitCategories
            ProcessSequence             = ProcessSequence
            Comments                    = Comments
        }

    static member empty =
        Assay.create()
