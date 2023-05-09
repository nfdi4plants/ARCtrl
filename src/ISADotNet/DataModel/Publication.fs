namespace ISADotNet


type Publication = 
    {
        PubMedID : URI option
        DOI : string option
        Authors : string option
        Title : string option
        Status : OntologyAnnotation option
        Comments : Comment list option
    }

    static member make pubMedID doi authors title status comments =
        {
            PubMedID    = pubMedID
            DOI         = doi
            Authors     = authors
            Title       = title
            Status      = status
            Comments    = comments
        }

    static member create (?PubMedID,?Doi,?Authors,?Title,?Status,?Comments) : Publication =
       Publication.make PubMedID Doi Authors Title Status Comments

    static member empty =
        Publication.create()

