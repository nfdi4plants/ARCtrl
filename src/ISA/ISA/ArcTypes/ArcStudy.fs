namespace ISA

open Fable.Core

[<AttachMembers>]
type ArcStudy = 
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
        Materials : StudyMaterials option
        Sheets : ArcTable list option
        Assays : ArcAssay list option
        Factors : Factor list option
        /// List of all the characteristics categories (or material attributes) defined in the study, used to avoid duplication of their declaration when each material_attribute_value is created. 
        CharacteristicCategories : MaterialAttribute list option
        /// List of all the unitsdefined in the study, used to avoid duplication of their declaration when each value is created.
        UnitCategories : OntologyAnnotation list option
        Comments : Comment list option
    }


    static member make id fileName identifier title description submissionDate publicReleaseDate publications contacts studyDesignDescriptors materials sheets assays factors characteristicCategories unitCategories comments = 
        {
            ID = id
            FileName = fileName
            Identifier = identifier
            Title = title
            Description = description
            SubmissionDate = submissionDate
            PublicReleaseDate = publicReleaseDate
            Publications = publications
            Contacts = contacts
            StudyDesignDescriptors = studyDesignDescriptors
            Materials = materials
            Sheets = sheets
            Assays = assays
            Factors = factors
            CharacteristicCategories = characteristicCategories
            UnitCategories = unitCategories
            Comments = comments
        }

    member this.isEmpty 
        with get() =
            (this.ID = None) &&
            (this.FileName = None) &&
            (this.Identifier = None) &&
            (this.Title = None) &&
            (this.Description = None) &&
            (this.SubmissionDate = None) &&
            (this.PublicReleaseDate = None) &&
            (this.Publications = None) &&
            (this.Contacts = None) &&
            (this.StudyDesignDescriptors = None) &&
            (this.Materials = None) &&
            (this.Sheets = None) &&
            (this.Assays = None) &&
            (this.Factors = None) &&
            (this.CharacteristicCategories = None) &&
            (this.UnitCategories = None) &&
            (this.Comments = None)

    [<NamedParams>]
    static member create (?ID, ?FileName, ?Identifier, ?Title, ?Description, ?SubmissionDate, ?PublicReleaseDate, ?Publications, ?Contacts, ?StudyDesignDescriptors, ?Materials, ?Sheets, ?Assays, ?Factors, ?CharacteristicCategories, ?UnitCategories, ?Comments) = 
        ArcStudy.make ID FileName Identifier Title Description SubmissionDate PublicReleaseDate Publications Contacts StudyDesignDescriptors Materials Sheets Assays Factors CharacteristicCategories UnitCategories Comments

    static member tryGetAssayByID (assayIdentifier : string) (study : Study) : Assay option = 
        raise (System.NotImplementedException())

    static member updateAssayByID (assay : Assay) (assayIdentifier : string) (study : Study) : Study = 
        ArcStudy.tryGetAssayByID assayIdentifier study |> ignore
        raise (System.NotImplementedException())

    static member addAssay (assay : Assay) (study : Study) : Study = 
        raise (System.NotImplementedException())

    static member fromStudy (study : Study) : ArcStudy = 
        raise (System.NotImplementedException())

    static member toStudy (arcStudy : ArcStudy) : Study =
        raise (System.NotImplementedException())
