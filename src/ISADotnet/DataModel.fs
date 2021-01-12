namespace ISADotNet


type Comment = 
    {
        ID : string
        Name : string
        Value : string
    }
  
    static member create id name value = 
        {
            ID = id
            Name = name 
            Value = value      
        }
        
    static member NameJson = "name"
    static member ValueJson = "value"
    
type REF<'T> =
    | ID of string
    | Item of 'T

    static member toID ref =
        match ref with
        | ID s -> s
        | Item i -> failwith "REF contained item, and not id"

    static member toItem ref =
        match ref with
        | ID s -> failwith "REF contained id, and not item"
        | Item i -> i



type OntologyAnnotation =
    {
        Name : string
        TermSourceREF : string
        TermAccessionNumber : string
        Comments : Comment list
    }

    static member create name termAccessionNumber termSourceREF comments= 
        {
            Name = name 
            TermSourceREF = termSourceREF
            TermAccessionNumber = termAccessionNumber  
            Comments = comments
        }

    static member NameJson = "annotationValue"
    static member TermSourceREFJson = "termSource"
    static member TermAccessionNumberJson = "termAccession"
    static member CommentsJson = "comments"

    static member NameTab = "Term Source Name"
    static member FileTab = "Term Source File"
    static member VersionTab = "Term Source Version"
    static member DescriptionTab = "Term Source Description"



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
        Comments : Comment list  
    }

    static member create id name dataType comments =
        {
            ID      = id
            Name    = name
            DataType = dataType
            Comments = comments         
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

    static member create name componentType =
        {
            ComponentName = name
            ComponentType = componentType
        }

    static member NameJson = "componentName"
    static member TypeJson = "componentType"

type Factor = 
    {
        ID : string
        Name : string
        FactorType : REF<OntologyAnnotation>
        Comments : Comment list
    
    }

    static member create id name factorType comments =
        {
            ID      = id
            Name    = name
            FactorType = factorType
            Comments = comments         
        }

    static member IDJson = "@id"
    static member NameJson =   "factorName"
    static member FactorTypeJson = "factorType"
    static member CommentsJson = "comments"

    static member NameTab = "Name"
    static member FactorTypeTab = "Type"
    static member TypeTermAccessionNumberTab = "Type Term Accession Number"
    static member TypeTermSourceREFTab = "Type Term Source REF"

type FactorValue =
    {
        ID : string
        Category : REF<Factor>
        Value : Value
        Unit : REF<OntologyAnnotation> 
    
    }

    static member create id category value unit =
        {
            ID      = id
            Category = category
            Value = value
            Unit = unit         
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

    static member create id characteristicType =
        {
            ID = id
            CharacteristicType = characteristicType     
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

    static member create id category value unit : MaterialAttributeValue =
        {
            ID      = id
            Category = category
            Value = value
            Unit = unit         
        }

    static member IDJson = "@id"
    static member CategoryJson = "category"
    static member ValueJson = "value"
    static member UnitJson = "unit"

type MaterialType =
    | ExtractName // "Extract Name"
    | LabeledExtractName // "Labeled Extract Name"

    static member create t =
        if t = "Extract Name" then ExtractName
        elif t = "Labeled Extract Name" then LabeledExtractName
        else failwith "No other value than \"Extract Name\" or \"Labeled Extract Name\" allowed for materialtype"

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

    static member create id name materialType characteristics derivesFrom : Material=
        {
            ID              = id
            Name            = name
            MaterialType    = materialType
            Characteristics = characteristics     
            DerivesFrom     = derivesFrom       
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

    static member create id name characteristics : Source=
        {
            ID              = id
            Name            = name
            Characteristics = characteristics          
        }

    static member IDJson                = "@id"
    static member NameJson              = "name"
    static member CharacteristicsJson   = "characteristics"


type Sample = 
    {
        ID : string
        Name : string
        Characteristics : REF<MaterialAttributeValue> list
        FactorValues : REF<FactorValue> list
        DerivesFrom : REF<Source>    
    }

    static member create id name characteristics factorValues derivesFrom : Sample=
        {
            ID              = id
            Name            = name
            Characteristics = characteristics     
            FactorValues    = factorValues
            DerivesFrom     = derivesFrom       
        }

    static member IDJson                = "@id"
    static member NameJson              = "name"
    static member CharacteristicsJson   = "characteristics"
    static member FactorValuesJson      = "factorValues"
    static member DerivesFromJson       = "derivesFrom"

type ProtocolParameter = 
    {
        ID : string
        ParameterName : REF<OntologyAnnotation>
    }

    static member create id parameterName : ProtocolParameter= 
        {
            ID = id
            ParameterName = parameterName
        
        }

    static member IDJson            = "@id"
    static member ParameterNameJson = "parameterName"


type Protocol =
    {       
        Name :          string
        ProtocolType :  REF<OntologyAnnotation>
        Description :   string
        Uri :           string
        Version :       string
        Parameters :    REF<ProtocolParameter> list
        Components :    REF<Component> list
        Comments :      Comment list
    }

    static member create name protocolType description uri version parameters components comments : Protocol= 
        {       
            Name            = name
            ProtocolType    = protocolType
            Description     = description
            Uri             = uri
            Version         = version
            Parameters      = parameters
            Components      = components
            Comments        = comments
        }

    static member NameJson          = "name"
    static member TypeJson          = "protocolType"
    static member DescriptionJson   = "description"
    static member UriJson           = "uri"
    static member VersionJson       = "version"
    static member ParametersJson    = "parameters"
    static member ComponentsJson    = "components"
    static member CommentsJson      = "comments"

    static member NameTab = "Name"
    static member ProtocolTypeTab = "Type"
    static member TypeTermAccessionNumberTab = "Type Term Accession Number"
    static member TypeTermSourceREFTab = "Type Term Source REF"
    static member DescriptionTab = "Description"
    static member URITab = "URI"
    static member VersionTab = "Version"
    static member ParametersNameTab = "Parameters Name"
    static member ParametersTermAccessionNumberTab = "Parameters Term Accession Number"
    static member ParametersTermSourceREFTab = "Parameters Term Source REF"
    static member ComponentsNameTab = "Components Name"
    static member ComponentsTypeTab = "Components Type"
    static member ComponentsTypeTermAccessionNumberTab = "Components Type Term Accession Number"
    static member ComponentsTypeTermSourceREFTab = "Components Type Term Source REF"

type ProcessParameterValue =
    {
        Category    : REF<ProtocolParameter>
        Value       : Value
        Unit        : REF<OntologyAnnotation> option   
    }

    static member create category value unit : ProcessParameterValue = 
        {
            Category = category
            Value = value
            Unit = unit
        }

    static member CategoryJson  = "category"
    static member ValueJson     = "value"
    static member UnitJson      = "unit"


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

    static member create name executesProtocol parameterValues performer date previousProcess nextProcess inputs outputs comments : Process= 
        {       
            Name                = name
            ExecutesProtocol    = executesProtocol
            ParameterValues     = parameterValues
            Performer           = performer
            Date                = date
            PreviousProcess     = previousProcess
            NextProcess         = nextProcess
            Inputs              = inputs
            Outputs             = outputs
            Comments            = comments       
        }

    static member NameJson              = "name"
    static member ExecutesProtocolJson  = "executesProtocol"
    static member ParameterValuesJson   = "parameterValues"
    static member PerformerJson         = "performer"
    static member DateJson              = "date"
    static member PreviousProcessJson   = "previousProcess"
    static member NextProcessJson       = "nextProcess"
    static member InputsJson            = "inputs"
    static member OutputsJson           = "outputs"
    static member CommentsJson          = "comments"

type Person = 
    {
        ID : string
        LastName : string
        FirstName : string
        MidInitials : string
        EMail : string
        Phone : string
        Fax : string
        Address : string
        Affiliation : string
        Roles : REF<OntologyAnnotation> list
        Comments : Comment list  
    }

    static member create id lastName firstName midInitials email phone fax address affiliation roles comments : Person =
        {
            ID = id
            LastName = lastName
            FirstName = firstName
            MidInitials = midInitials
            EMail = email
            Phone = phone
            Fax = fax
            Address = address
            Affiliation = affiliation
            Roles = roles
            Comments = comments
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

    static member LastNameTab = "Last Name"
    static member FirstNameTab = "First Name"
    static member MidInitialsTab = "Mid Initials"
    static member EmailTab = "Email"
    static member PhoneTab = "Phone"
    static member FaxTab = "Fax"
    static member AddressTab = "Address"
    static member AffiliationTab = "Affiliation"
    static member RolesTab = "Roles"
    static member RolesTermAccessionNumberTab = "Roles Term Accession Number"
    static member RolesTermSourceREFTab = "Roles Term Source REF"

type Publication = 
    {
        PubMedID : string
        DOI : string
        Authors : string
        Title : string
        Status : REF<OntologyAnnotation>
        Comments : Comment list
    }

    static member create pubMedID doi authors title status comments =
        {
            PubMedID    = pubMedID
            DOI         = doi
            Authors     = authors
            Title       = title
            Status      = status
            Comments    = comments
        }

    static member PubMedIDJson  = "pubMedID"
    static member DOIJson       = "doi"
    static member AuthorsJson   = "authorList"
    static member TitleJson     = "title" 
    static member StatusJson    = "status" 
    static member CommentsJson  = "comments" 

    static member PubMedIDTab = "PubMed ID"
    static member DOITab = "DOI"
    static member AuthorListTab = "Author List"
    static member TitleTab = "Title"
    static member StatusTab = "Status"
    static member StatusTermAccessionNumberTab = "Status Term Accession Number"
    static member StatusTermSourceREFTab = "Status Term Source REF"

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

    static member MeasurementTypeTab = "Measurement Type"
    static member MeasurementTypeTermAccessionNumberTab = "Measurement Type Term Accession Number"
    static member MeasurementTypeTermSourceREFTab = "Measurement Type Term Source REF"
    static member TechnologyTypeTab = "Technology Type"
    static member TechnologyTypeTermAccessionNumberTab = "Technology Type Term Accession Number"
    static member TechnologyTypeTermSourceREFTab = "Technology Type Term Source REF"
    static member TechnologyPlatformTab = "Technology Platform"
    static member FileNameTab = "File Name"


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

type OntologySourceReference =
    {
        Description : string
        File : string
        Name : string
        Version : string
        Comments : Comment list    
    }

    static member create description file name version comments  =
        {
            Description = description
            File        = file
            Name        = name
            Version     = version
            Comments    = comments
        }


    static member DescriptionJson   = "description"
    static member FileJson          = "file"
    static member NameJson          = "name"
    static member VersionJson       = "version"
    static member CommentsJson      = "comments"

    static member NameTab = "Term Source Name"
    static member FileTab = "Term Source File"
    static member VersionTab = "Term Source Version"
    static member DescriptionTab = "Term Source Description"

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

    static member create id filename identifier title description submissionDate publicReleaseDate ontologySourceReference publications contacts studies comments : Investigation=
        {
            ID                          = id
            FileName                    = filename
            Identifier                  = identifier
            Title                       = title
            Description                 = description
            SubmissionDate              = submissionDate
            PublicReleaseDate           = publicReleaseDate
            OntologySourceReferences    = ontologySourceReference
            Publications                = publications
            Contacts                    = contacts
            Studies                     = studies
            Comments                    = comments
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



    static member IdentifierTab = "Investigation Identifier"
    static member TitleTab = "Investigation Title"
    static member DescriptionTab = "Investigation Description"
    static member SubmissionDateTab = "Investigation Submission Date"
    static member PublicReleaseDateTab = "Investigation Public Release Date"

    static member InvestigationTab = "INVESTIGATION"
    static member OntologySourceReferenceTab = "ONTOLOGY SOURCE REFERENCE"
    static member PublicationsTab = "INVESTIGATION PUBLICATIONS"
    static member ContactsTab = "INVESTIGATION CONTACTS"
    static member StudyTab = "STUDY"

    static member PublicationsTabPrefix = "Investigation Publication"
    static member ContactsTabPrefix = "Investigation Person"