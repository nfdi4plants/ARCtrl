namespace ISADotNet

open System.Text.Json.Serialization


type Investigation = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"filename")>]
        FileName : string option
        [<JsonPropertyName(@"identifier")>]
        Identifier : string option
        [<JsonPropertyName(@"title")>]
        Title : string option
        [<JsonPropertyName(@"description")>]
        Description : string option
        [<JsonPropertyName(@"submissionDate")>]
        SubmissionDate : string option
        [<JsonPropertyName(@"publicReleaseDate")>]
        PublicReleaseDate : string option
        [<JsonPropertyName(@"ontologySourceReferences")>]
        OntologySourceReferences : OntologySourceReference list option
        [<JsonPropertyName(@"publications")>]
        Publications : Publication list option
        [<JsonPropertyName(@"people")>]
        Contacts : Person list option
        [<JsonPropertyName(@"studies")>]
        Studies : Study list option
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list option
        [<JsonIgnore>]
        Remarks     : Remark list
    }
    
    static member make id filename identifier title description submissionDate publicReleaseDate ontologySourceReference publications contacts studies comments remarks : Investigation=
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

    static member create(?Id,?FileName,?Identifier,?Title,?Description,?SubmissionDate,?PublicReleaseDate,?OntologySourceReferences,?Publications,?Contacts,?Studies,?Comments,?Remarks) : Investigation=
        Investigation.make Id FileName Identifier Title Description SubmissionDate PublicReleaseDate OntologySourceReferences Publications Contacts Studies Comments (Remarks |> Option.defaultValue [])

    static member empty =
        Investigation.create ()
