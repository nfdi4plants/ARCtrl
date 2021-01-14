namespace ISADotNet

open System.Text.Json.Serialization

type Publication = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI
        [<JsonPropertyName(@"pubMedID")>]
        PubMedID : string
        [<JsonPropertyName(@"doi")>]
        DOI : string
        [<JsonPropertyName(@"authorList")>]
        Authors : string
        [<JsonPropertyName(@"title")>]
        Title : string
        [<JsonPropertyName(@"status")>]
        Status : OntologyAnnotation
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list
    }

    static member create id pubMedID doi authors title status comments =
        {
            ID          = id
            PubMedID    = pubMedID
            DOI         = doi
            Authors     = authors
            Title       = title
            Status      = status
            Comments    = comments
        }


    static member PubMedIDTab = "PubMed ID"
    static member DOITab = "DOI"
    static member AuthorListTab = "Author List"
    static member TitleTab = "Title"
    static member StatusTab = "Status"
    static member StatusTermAccessionNumberTab = "Status Term Accession Number"
    static member StatusTermSourceREFTab = "Status Term Source REF"

