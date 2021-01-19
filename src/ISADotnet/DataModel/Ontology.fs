namespace ISADotNet

open System.Text.Json.Serialization

[<AnyOf>]
type AnnotationValue = 
    | [<SerializationOrder(2)>]Text of string
    | [<SerializationOrder(1)>]Float of float
    | [<SerializationOrder(0)>]Int of int

type OntologyAnnotation =
    {
        [<JsonPropertyName("@id")>]
        ID : URI
        [<JsonPropertyName("annotationValue")>]
        Name : AnnotationValue
        [<JsonPropertyName("termSource")>]
        TermSourceREF : string
        [<JsonPropertyName("termAccession")>]
        TermAccessionNumber : URI
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

