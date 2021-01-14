namespace ISADotNet

open System.Text.Json.Serialization

type OntologyAnnotation =
    {
        [<JsonPropertyName("@id")>]
        ID : URI
        [<JsonPropertyName("annotationValue")>]
        Name : string
        [<JsonPropertyName("termSource")>]
        TermSourceREF : string
        [<JsonPropertyName("termAccession")>]
        TermAccessionNumber : string
        [<JsonPropertyName("comments")>]
        Comments : Comment list
    }

    static member create id name termAccessionNumber termSourceREF comments= 
        {
            ID = id
            Name = name 
            TermSourceREF = termSourceREF
            TermAccessionNumber = termAccessionNumber  
            Comments = comments
        }

    static member NameTab = "Term Source Name"
    static member FileTab = "Term Source File"
    static member VersionTab = "Term Source Version"
    static member DescriptionTab = "Term Source Description"


type OntologySourceReference =
    {
        [<JsonPropertyName("description")>]
        Description : string
        [<JsonPropertyName("file")>]
        File : string
        [<JsonPropertyName("name")>]
        Name : string
        [<JsonPropertyName("version")>]
        Version : string
        [<JsonPropertyName("comments")>]
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

    static member NameTab = "Term Source Name"
    static member FileTab = "Term Source File"
    static member VersionTab = "Term Source Version"
    static member DescriptionTab = "Term Source Description"
