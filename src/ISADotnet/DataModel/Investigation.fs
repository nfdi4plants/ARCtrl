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
    
    static member create(?Id,?FileName,?Identifier,?Title,?Description,?SubmissionDate,?PublicReleaseDate,?OntologySourceReferences,?Publications,?Contacts,?Studies,?Comments,?Remarks) : Investigation=
        {
            ID                          = Id
            FileName                    = FileName
            Identifier                  = Identifier
            Title                       = Title
            Description                 = Description
            SubmissionDate              = SubmissionDate
            PublicReleaseDate           = PublicReleaseDate
            OntologySourceReferences    = OntologySourceReferences
            Publications                = Publications
            Contacts                    = Contacts
            Studies                     = Studies
            Comments                    = Comments
            Remarks                     = (Option.defaultValue [] Remarks)
        }

    static member empty =
        Investigation.create ()
