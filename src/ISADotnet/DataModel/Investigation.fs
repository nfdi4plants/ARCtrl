namespace ISADotNet

open System.Text.Json.Serialization


type Investigation = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI
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