namespace ISADotNet


type Comment = 
    {
        ID : string
        Name : string
        Value : string
    }
  
    static member NameJson = "name"
    static member ValueJson = "value"

type REF<'T> =
    | ID of string
    | Item of 'T

type OntologyAnnotation =
    {
        Name : string
        TermSourceREF : string
        TermAccessionNumber : string
        Comments : REF<Comment> []
    }

    static member NameJson = "annotationValue"
    static member TermSourceREFJson = "termSource"
    static member TermAccessionNumberJson = "termAccession"
    static member CommentsJson = "comments"




type Value =
    | Ontology of REF<OntologyAnnotation>
    | Numeric of float
    | Name of string

type DataFile =
    | RawDataFile // "Raw Data File"
    | DerivedDataFile // "Derived Data File"
    | ImageFile // "Image File"

    static member RawDataFileJson       = "Raw Data File"
    static member DerivedDataFileJson   = "Derived Data File"
    static member ImageFileJson         = "Image File"


type Data = 
    {
        ID : string
        Name : string
        DataType : DataFile
        Comments : REF<Comment> list  
    }

    static member IDJson        = "@id"
    static member NameJson      = "name"
    static member DataTypeJson  = "type"
    static member CommentsJson  = "comments"

type Component = 
    {
        ComponentName : string
        ComponentType : REF<OntologyAnnotation>
    }

    static member NameJson = "componentName"
    static member TypeJson = "componentType"

type Factor = 
    {
        ID : string
        Name : string
        FactorType : REF<OntologyAnnotation>
        Comments : REF<Comment> list
    
    }

    static member IDJson = "@id"
    static member NameJson =   "factorName"
    static member FactorTypeJson = "factorType"
    static member CommentsJson = "comments"

type FactorValue =
    {
        ID : string
        Category : REF<Factor>
        Value : Value
        Unit : REF<OntologyAnnotation> 
    
    }

    static member IDJson = "@id"
    static member CategoryJson = "category"
    static member ValueJson = "value"
    static member UnitJson = "unit"




type MaterialAttribute = 
    {
        ID : string
        CharacteristicType : REF<OntologyAnnotation>
    
    }

    static member IDJson = "@id"
    static member CharacteristicTypeJson = "characteristicType"

type MaterialAttributeValue = 
    {
        ID : string
        Category : REF<MaterialAttribute>
        Value : REF<Value>
        Unit : REF<OntologyAnnotation>
    
    }

    static member IDJson = "@id"
    static member CategoryJson = "category"
    static member ValueJson = "value"
    static member UnitJson = "unit"

type MaterialType =
    | ExtractName // "Extract Name"
    | LabeledExtractName // "Labeled Extract Name"

    static member ExtractNameJson           = "Extract Name"
    static member LabeledExtractNameJson    = "Labeled Extract Name"

type Material = 
    {
        ID : string
        Name : string
        MaterialType : MaterialType
        Characteristics : REF<MaterialAttributeValue> list
        DerivesFrom : REF<OntologyAnnotation>
    
    }

    static member IDJson                = "@id"
    static member NameJson              = "name"
    static member MaterialTypeJson      = "type"
    static member CharacteristicsJson   = "characteristics"
    static member DerivesFromJson       = "derivesFrom"

type Source = 
    {
        ID : string 
        Name : string
        Characteristics : REF<MaterialAttributeValue>
    }

    static member IDJson = "@id"
    static member NameJson = "name"
    static member CharacteristicsJson = "characteristics"


type Sample = 
    {
        ID : string
        Name : string
        Characteristics : REF<MaterialAttributeValue> list
        FactorValues : REF<FactorValue> list
        DerivesFrom : REF<Source>    
    }

    static member IDJson = "@id"
    static member NameJson = "name"
    static member CharacteristicsJson = "characteristics"
    static member FactorValuesJson = "factorValues"
    static member DerivesFromJson = "derivesFrom"

type ProtocolParameter = 
    {
        ID : string
        ParameterName : REF<OntologyAnnotation>
    }

    static member IDJson = "@id"
    static member ParameterNameJson = "parameterName"

type Protocol =
    {       
        Name :          string
        ProtocolType :  REF<OntologyAnnotation> Option
        Description :   string
        Uri :           string
        Version :       string
        Parameters :    REF<ProtocolParameter> list
        Components :    REF<Component> list
        Comments :      Comment []
    }

    static member NameJson =           "name"
    static member TypeJson =           "protocolType"
    static member DescriptionJson =    "description"
    static member UriJson =            "uri"
    static member VersionJson =        "version"
    static member ParametersJson =     "parameters"
    static member ComponentsJson =     "components"
    static member CommentsJson =       "comments"

type ProcessParameterValue =
    {
        Category    : REF<ProtocolParameter>
        Value       : Value
        Unit        : OntologyAnnotation option   
    }

    static member CategoryJson = "category"
    static member ValueJson = "value"
    static member UnitJson = "unit"




type Process = 
    {
        Name : string
        ExecutesProtocol : REF<Protocol>
        ParameterValues : REF<ProcessParameterValue> list
        Performer : string
        Date : string
        PreviousProcess : REF<Process> 
        NextProcess : REF<Process>
        Inputs : string list
        Outputs : string list
        Comments : Comment list
    }

    static member NameJson = "name"
    static member ExecutesProtocolJson = "executesProtocol"
    static member ParameterValuesJson = "parameterValues"
    static member PerformerJson = "performer"
    static member DateJson = "date"
    static member PreviousProcessJson = "previousProcess"
    static member NextProcessJson = "nextProcess"
    static member InputsJson = "inputs"
    static member OutputsJson = "outputs"
    static member CommentsJson = "comments"

type Person = 
    {
        ID : string
        LastName : string
        FirstName : string
        MidInitials : string
        EMail : string
        Phone : string
        Fax : string
        Adress : string
        Affiliation : string
        Roles : REF<OntologyAnnotation> list
        Comments : Comment list  
    }

    static member IDJson            = "@id"
    static member LastNameJson      = "lastName"
    static member FirstNameJson     = "firstName"
    static member MidInitialsJson   = "midInitials" 
    static member EMailJson         = "email" 
    static member PhoneJson         = "phone" 
    static member FaxJson           = "fax"
    static member AdressJson        = "address"
    static member AffiliationJson   = "affiliation"
    static member RolesJson         = "roles" 
    static member CommentsJson      = "comments"

type Publication = 
    {
        PubMedID : string
        DOI : string
        Authors : string
        Title : string
        Status : REF<OntologyAnnotation>
        Comments : Comment list
    }

    static member PubMedIDJson  = "pubMedID"
    static member DOIJson       = "doi"
    static member AuthorsJson   = "authorList"
    static member TitleJson     = "title" 
    static member StatusJson    = "status" 
    static member CommentsJson  = "comments" 


type Assay = 
    {
        ID : string
        FileName : string
        MeasurementType : REF<OntologyAnnotation>
        TechnologyType : REF<OntologyAnnotation>
        TechnologyPlatform : string
        DataFiles : REF<Data> list
        Materials : REF<Sample> list * REF<Material> list
        /// List of all the characteristics categories (or material attributes) defined in the study, used to avoid duplication of their declaration when each material_attribute_value is created. 
        CharacteristicCategories : REF<MaterialAttribute> list
        /// List of all the unitsdefined in the study, used to avoid duplication of their declaration when each value is created.
        UnitCategories : REF<OntologyAnnotation> list
        ProcessSequence : REF<Process> list
        Comments : REF<Comment> list
    }

    static member IDJson                        = "@id"
    static member FileNameJson                  = "filename"
    static member MeasurementTypeJson           = "measurementType"
    static member TechnologyTypeJson            = "technologyType"
    static member TechnologyPlatformJson        = "technologyPlatform"
    static member DataFilesJson                 = "dataFiles"
    static member MaterialsJson                 = "materials"
    static member CharacteristicCategoriesJson  = "characteristicCategories"
    static member UnitCategoriesJson            = "unitCategories"
    static member ProcessSequenceJson           = "processSequence"
    static member CommentsJson                  = "comments"

type Study = 
    {
        ID : string
        FileName : string
        Identifier : string
        Title : string 
        Description : string
        SubmissionDate : string
        PublicReleaseDate : string
        Publications : REF<Publication> list
        Contacts : REF<Person> list
        StudyDesignDescriptors : REF<OntologyAnnotation> list
        Protocols : REF<Protocol> list
        Materials : REF<Source> list * REF<Sample> list * REF<Material> list
        ProcessSequence : REF<Process> list
        Assays : REF<Assay> list
        Factors : REF<Factor> list
        /// List of all the characteristics categories (or material attributes) defined in the study, used to avoid duplication of their declaration when each material_attribute_value is created. 
        CharacteristicCategories : REF<MaterialAttribute> list
        /// List of all the unitsdefined in the study, used to avoid duplication of their declaration when each value is created.
        UnitCategories : REF<OntologyAnnotation> list
        Comments : Comment list
    }

    static member IDJson                        = "@id"
    static member FileNameJson                  = "filename"
    static member IdentifierJson                = "identifier"
    static member TitleJson                     = "title"
    static member DescriptionJson               = "description"
    static member SubmissionDateJson            = "submissionDate"
    static member PublicReleaseDateJson         = "publicReleaseDate"
    static member PublicationsJson              = "publications"
    static member ContactsJson                  = "people"
    static member StudyDesignDescriptorsJson    = "studyDesignDescriptors"
    static member ProtocolsJson                 = "protocols"
    static member MaterialsJson                 = "materials"
    static member ProcessSequenceJson           = "processSequence"
    static member AssaysJson                    = "assays"
    static member FactorsJson                   = "factors"
    static member CharacteristicCategoriesJson  = "characteristicCategories"
    static member UnitCategoriesJson            = "unitCategories"
    static member CommentsJson                  = "comments"

type OntologySourceReference =
    {
        Description : string
        File : string
        Name : string
        Version : string
        Comments : Comment list    
    }

    static member DescriptionJson   = "description"
    static member FileJson          = "file"
    static member NameJson          = "name"
    static member VersionJson       = "version"
    static member CommentsJson      = "comments"


type Investigation = 
    {
        ID : string
        FileName : string
        Identifier : string
        Title : string 
        Description : string
        SubmissionDate : string
        PublicReleaseDate : string
        OntologySourceReferences : REF<OntologySourceReference> list
        Publications : REF<Publication> list
        Contacts : REF<Person> list
        Studies : REF<Study> list
        Comments : Comment list
    }

    static member IDJson                        = "@id"
    static member FileNameJson                  = "filename"
    static member IdentifierJson                = "identifier"
    static member TitleJson                     = "title"
    static member DescriptionJson               = "description"
    static member SubmissionDateJson            = "submissionDate"
    static member PublicReleaseDateJson         = "publicReleaseDate"
    static member OntologySourceReferencesJson  = "ontologySourceReferences"
    static member PublicationsJson              = "publications"
    static member ContactsJson                  = "people"
    static member StudiesJson                   = "studies"
    static member CommentsJson                  = "comments"
