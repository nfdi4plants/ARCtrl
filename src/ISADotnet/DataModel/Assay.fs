namespace ISADotNet

open System.Text.Json.Serialization

type AssayMaterials =
    {
        [<JsonPropertyName(@"sources")>]
        Samples : Sample list
        [<JsonPropertyName(@"otherMaterials")>]
        OtherMaterials : Material list    
    }

type Assay = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : string
        [<JsonPropertyName(@"filename")>]
        FileName : string
        [<JsonPropertyName(@"measurementType")>]
        MeasurementType : OntologyAnnotation
        [<JsonPropertyName(@"technologyType")>]
        TechnologyType : OntologyAnnotation
        [<JsonPropertyName(@"technologyPlatform")>]
        TechnologyPlatform : string
        [<JsonPropertyName(@"dataFiles")>]
        DataFiles : Data list
        [<JsonPropertyName(@"materials")>]
        Materials : AssayMaterials
        /// List of all the characteristics categories (or material attributes) defined in the study, used to avoid duplication of their declaration when each material_attribute_value is created. 
        [<JsonPropertyName(@"characteristicCategories")>]
        CharacteristicCategories : MaterialAttribute list
        /// List of all the unitsdefined in the study, used to avoid duplication of their declaration when each value is created.
        [<JsonPropertyName(@"unitCategories")>]
        UnitCategories : OntologyAnnotation list
        [<JsonPropertyName(@"processSequence")>]
        ProcessSequence : Process list
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list
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



    static member MeasurementTypeTab = "Measurement Type"
    static member MeasurementTypeTermAccessionNumberTab = "Measurement Type Term Accession Number"
    static member MeasurementTypeTermSourceREFTab = "Measurement Type Term Source REF"
    static member TechnologyTypeTab = "Technology Type"
    static member TechnologyTypeTermAccessionNumberTab = "Technology Type Term Accession Number"
    static member TechnologyTypeTermSourceREFTab = "Technology Type Term Source REF"
    static member TechnologyPlatformTab = "Technology Platform"
    static member FileNameTab = "File Name"