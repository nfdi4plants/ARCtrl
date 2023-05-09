namespace ISADotNet

type AssayMaterials =
    {
        Samples : Sample list option
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
        ID : URI option
        FileName : string option
        MeasurementType : OntologyAnnotation option
        TechnologyType : OntologyAnnotation option
        TechnologyPlatform : string option
        DataFiles : Data list option
        Materials : AssayMaterials option
        /// List of all the characteristics categories (or material attributes) defined in the study, used to avoid duplication of their declaration when each material_attribute_value is created. 
        CharacteristicCategories : MaterialAttribute list option
        /// List of all the unitsdefined in the study, used to avoid duplication of their declaration when each value is created.
        UnitCategories : OntologyAnnotation list option
        ProcessSequence : Process list option
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
