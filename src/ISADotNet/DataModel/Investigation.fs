namespace ISADotNet

type Investigation = 
    {
        ID : URI option
        FileName : string option
        Identifier : string option
        Title : string option
        Description : string option
        SubmissionDate : string option
        PublicReleaseDate : string option
        OntologySourceReferences : OntologySourceReference list option
        Publications : Publication list option
        Contacts : Person list option
        Studies : Study list option
        Comments : Comment list option
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
