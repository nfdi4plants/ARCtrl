namespace ISADotNet.InvestigationFile

open System
type TermSource =
    {
    Name : string
    File : string
    Version : string
    Description : string
    Comments : List<string*string>
    }


    static member create name file version description comments =
        {
        Name = name
        File = file
        Version = version
        Description = description
        Comments = comments
        }

    static member NameLabel = "Term Source Name"
    static member FileLabel = "Term Source File"
    static member VersionLabel = "Term Source Version"
    static member DescriptionLabel = "Term Source Description"


type Publication =
    {
    PubMedID : string
    DOI : string
    AuthorList : string
    Title : string
    Status : string
    StatusTermAccessionNumber : string
    StatusTermSourceREF : string
    Comments : List<string*string>
    }


    static member create pubMedID doi authorList title status statusTermAccessionNumber statusTermSourceREF comments =
        {
        PubMedID = pubMedID
        DOI = doi
        AuthorList = authorList
        Title = title
        Status = status
        StatusTermAccessionNumber = statusTermAccessionNumber
        StatusTermSourceREF = statusTermSourceREF
        Comments = comments
        }

    static member PubMedIDLabel = "PubMed ID"
    static member DOILabel = "DOI"
    static member AuthorListLabel = "Author List"
    static member TitleLabel = "Title"
    static member StatusLabel = "Status"
    static member StatusTermAccessionNumberLabel = "Status Term Accession Number"
    static member StatusTermSourceREFLabel = "Status Term Source REF"


type Person =
    {
    LastName : string
    FirstName : string
    MidInitials : string
    Email : string
    Phone : string
    Fax : string
    Address : string
    Affiliation : string
    Roles : string
    RolesTermAccessionNumber : string
    RolesTermSourceREF : string
    Comments : List<string*string>
    }


    static member create lastName firstName midInitials email phone fax address affiliation roles rolesTermAccessionNumber rolesTermSourceREF comments =
        {
        LastName = lastName
        FirstName = firstName
        MidInitials = midInitials
        Email = email
        Phone = phone
        Fax = fax
        Address = address
        Affiliation = affiliation
        Roles = roles
        RolesTermAccessionNumber = rolesTermAccessionNumber
        RolesTermSourceREF = rolesTermSourceREF
        Comments = comments
        }

    static member LastNameLabel = "Last Name"
    static member FirstNameLabel = "First Name"
    static member MidInitialsLabel = "Mid Initials"
    static member EmailLabel = "Email"
    static member PhoneLabel = "Phone"
    static member FaxLabel = "Fax"
    static member AddressLabel = "Address"
    static member AffiliationLabel = "Affiliation"
    static member RolesLabel = "Roles"
    static member RolesTermAccessionNumberLabel = "Roles Term Accession Number"
    static member RolesTermSourceREFLabel = "Roles Term Source REF"


type Design =
    {
    DesignType : string
    TypeTermAccessionNumber : string
    TypeTermSourceREF : string
    Comments : List<string*string>
    }


    static member create designType typeTermAccessionNumber typeTermSourceREF comments =
        {
        DesignType = designType
        TypeTermAccessionNumber = typeTermAccessionNumber
        TypeTermSourceREF = typeTermSourceREF
        Comments = comments
        }

    static member DesignTypeLabel = "Type"
    static member TypeTermAccessionNumberLabel = "Type Term Accession Number"
    static member TypeTermSourceREFLabel = "Type Term Source REF"


type Factor =
    {
    Name : string
    FactorType : string
    TypeTermAccessionNumber : string
    TypeTermSourceREF : string
    Comments : List<string*string>
    }


    static member create name factorType typeTermAccessionNumber typeTermSourceREF comments =
        {
        Name = name
        FactorType = factorType
        TypeTermAccessionNumber = typeTermAccessionNumber
        TypeTermSourceREF = typeTermSourceREF
        Comments = comments
        }

    static member NameLabel = "Name"
    static member FactorTypeLabel = "Type"
    static member TypeTermAccessionNumberLabel = "Type Term Accession Number"
    static member TypeTermSourceREFLabel = "Type Term Source REF"


type Assay =
    {
    MeasurementType : string
    MeasurementTypeTermAccessionNumber : string
    MeasurementTypeTermSourceREF : string
    TechnologyType : string
    TechnologyTypeTermAccessionNumber : string
    TechnologyTypeTermSourceREF : string
    TechnologyPlatform : string
    FileName : string
    Comments : List<string*string>
    }


    static member create measurementType measurementTypeTermAccessionNumber measurementTypeTermSourceREF technologyType technologyTypeTermAccessionNumber technologyTypeTermSourceREF technologyPlatform fileName comments =
        {
        MeasurementType = measurementType
        MeasurementTypeTermAccessionNumber = measurementTypeTermAccessionNumber
        MeasurementTypeTermSourceREF = measurementTypeTermSourceREF
        TechnologyType = technologyType
        TechnologyTypeTermAccessionNumber = technologyTypeTermAccessionNumber
        TechnologyTypeTermSourceREF = technologyTypeTermSourceREF
        TechnologyPlatform = technologyPlatform
        FileName = fileName
        Comments = comments
        }

    static member MeasurementTypeLabel = "Measurement Type"
    static member MeasurementTypeTermAccessionNumberLabel = "Measurement Type Term Accession Number"
    static member MeasurementTypeTermSourceREFLabel = "Measurement Type Term Source REF"
    static member TechnologyTypeLabel = "Technology Type"
    static member TechnologyTypeTermAccessionNumberLabel = "Technology Type Term Accession Number"
    static member TechnologyTypeTermSourceREFLabel = "Technology Type Term Source REF"
    static member TechnologyPlatformLabel = "Technology Platform"
    static member FileNameLabel = "File Name"


type Protocol =
    {
    Name : string
    ProtocolType : string
    TypeTermAccessionNumber : string
    TypeTermSourceREF : string
    Description : string
    URI : string
    Version : string
    ParametersName : string
    ParametersTermAccessionNumber : string
    ParametersTermSourceREF : string
    ComponentsName : string
    ComponentsType : string
    ComponentsTypeTermAccessionNumber : string
    ComponentsTypeTermSourceREF : string
    Comments : List<string*string>
    }


    static member create name protocolType typeTermAccessionNumber typeTermSourceREF description uri version parametersName parametersTermAccessionNumber parametersTermSourceREF componentsName componentsType componentsTypeTermAccessionNumber componentsTypeTermSourceREF comments =
        {
        Name = name
        ProtocolType = protocolType
        TypeTermAccessionNumber = typeTermAccessionNumber
        TypeTermSourceREF = typeTermSourceREF
        Description = description
        URI = uri
        Version = version
        ParametersName = parametersName
        ParametersTermAccessionNumber = parametersTermAccessionNumber
        ParametersTermSourceREF = parametersTermSourceREF
        ComponentsName = componentsName
        ComponentsType = componentsType
        ComponentsTypeTermAccessionNumber = componentsTypeTermAccessionNumber
        ComponentsTypeTermSourceREF = componentsTypeTermSourceREF
        Comments = comments
        }

    static member NameLabel = "Name"
    static member ProtocolTypeLabel = "Type"
    static member TypeTermAccessionNumberLabel = "Type Term Accession Number"
    static member TypeTermSourceREFLabel = "Type Term Source REF"
    static member DescriptionLabel = "Description"
    static member URILabel = "URI"
    static member VersionLabel = "Version"
    static member ParametersNameLabel = "Parameters Name"
    static member ParametersTermAccessionNumberLabel = "Parameters Term Accession Number"
    static member ParametersTermSourceREFLabel = "Parameters Term Source REF"
    static member ComponentsNameLabel = "Components Name"
    static member ComponentsTypeLabel = "Components Type"
    static member ComponentsTypeTermAccessionNumberLabel = "Components Type Term Accession Number"
    static member ComponentsTypeTermSourceREFLabel = "Components Type Term Source REF"


type InvestigationInfo =
    {
    Identifier : string
    Title : string
    Description : string
    SubmissionDate : string
    PublicReleaseDate : string
    Comments : List<string*string>
    }


    static member create identifier title description submissionDate publicReleaseDate comments =
        {
        Identifier = identifier
        Title = title
        Description = description
        SubmissionDate = submissionDate
        PublicReleaseDate = publicReleaseDate
        Comments = comments
        }

    static member IdentifierLabel = "Investigation Identifier"
    static member TitleLabel = "Investigation Title"
    static member DescriptionLabel = "Investigation Description"
    static member SubmissionDateLabel = "Investigation Submission Date"
    static member PublicReleaseDateLabel = "Investigation Public Release Date"


type StudyInfo =
    {
    Identifier : string
    Title : string
    Description : string
    SubmissionDate : string
    PublicReleaseDate : string
    FileName : string
    Comments : List<string*string>
    }


    static member create identifier title description submissionDate publicReleaseDate fileName comments =
        {
        Identifier = identifier
        Title = title
        Description = description
        SubmissionDate = submissionDate
        PublicReleaseDate = publicReleaseDate
        FileName = fileName
        Comments = comments
        }

    static member IdentifierLabel = "Study Identifier"
    static member TitleLabel = "Study Title"
    static member DescriptionLabel = "Study Description"
    static member SubmissionDateLabel = "Study Submission Date"
    static member PublicReleaseDateLabel = "Study Public Release Date"
    static member FileNameLabel = "Study File Name"

type Study = 
    {
    Info : StudyInfo
    DesignDescriptors : Design list
    Publications : Publication list
    Factors : Factor list
    Assays : Assay list
    Protocols : Protocol list
    Contacts : Person list
    }

    static member create studyInfo designDescriptors publications factors assays protocols contacts =
        {
        Info = studyInfo
        DesignDescriptors = designDescriptors
        Publications = publications
        Factors = factors
        Assays = assays
        Protocols = protocols
        Contacts = contacts
        }
    
    static member DesignDescriptorsLabel = "STUDY DESIGN DESCRIPTORS"
    static member PublicationsLabel = "STUDY PUBLICATIONS"
    static member FactorsLabel = "STUDY FACTORS"
    static member AssaysLabel = "STUDY ASSAYS"
    static member ProtocolsLabel = "STUDY PROTOCOLS"
    static member ContactsLabel = "STUDY CONTACTS"

    static member DesignDescriptorsPrefix = "Study Design"
    static member PublicationsPrefix = "Study Publication"
    static member FactorsPrefix = "Study Factor"
    static member AssaysPrefix = "Study Assay"
    static member ProtocolsPrefix = "Study Protocol"
    static member ContactsPrefix = "Study Person"

type Investigation =
    {
    OntologySourceReference : TermSource list
    Info : InvestigationInfo
    Publications : Publication list
    Contacts : Person list
    Studies : Study list
    Remarks : (int*string) list
    }

    static member create ontologySourceReference investigationInfo publications contacts studies remarks =
        {
        Info = investigationInfo
        OntologySourceReference = ontologySourceReference
        Publications = publications
        Contacts = contacts
        Studies = studies
        Remarks = remarks
        }

        static member InvestigationLabel = "INVESTIGATION"
        static member OntologySourceReferenceLabel = "ONTOLOGY SOURCE REFERENCE"
        static member PublicationsLabel = "INVESTIGATION PUBLICATIONS"
        static member ContactsLabel = "INVESTIGATION CONTACTS"
        static member StudyLabel = "STUDY"

        static member PublicationsPrefix = "Investigation Publication"
        static member ContactsPrefix = "Investigation Person"