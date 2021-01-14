namespace ISADotNet

open System.Text.Json.Serialization


type Investigation = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : string
        [<JsonPropertyName(@"filename")>]
        FileName : string
        [<JsonPropertyName(@"identifier")>]
        Identifier : string
        [<JsonPropertyName(@"title")>]
        Title : string 
        [<JsonPropertyName(@"description")>]
        Description : string
        [<JsonPropertyName(@"submissionDate")>]
        SubmissionDate : string
        [<JsonPropertyName(@"publicReleaseDate")>]
        PublicReleaseDate : string
        [<JsonPropertyName(@"ontologySourceReferences")>]
        OntologySourceReferences : OntologySourceReference list
        [<JsonPropertyName(@"publications")>]
        Publications : Publication list
        [<JsonPropertyName(@"people")>]
        Contacts : Person list
        [<JsonPropertyName(@"studies")>]
        Studies : Study list
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list
        [<JsonIgnore>]
        Remarks     : Remark list
    }
    
    static member create id filename identifier title description submissionDate publicReleaseDate ontologySourceReference publications contacts studies comments remarks : Investigation=
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
            Remarks                     = remarks
        }

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