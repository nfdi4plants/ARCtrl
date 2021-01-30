namespace ISADotNet

open System.Text.Json.Serialization

[<AnyOf>]
type AnnotationValue = 
    | [<SerializationOrder(2)>]Text of string
    | [<SerializationOrder(1)>]Float of float
    | [<SerializationOrder(0)>]Int of int

    static member empty = Text ""

type OntologyAnnotation =
    {
        [<JsonPropertyName("@id")>]
        ID : URI option
        [<JsonPropertyName("annotationValue")>]
        Name : AnnotationValue option
        [<JsonPropertyName("termSource")>]
        TermSourceREF : string option
        [<JsonPropertyName("termAccession")>]
        TermAccessionNumber : URI option
        [<JsonPropertyName("comments")>]
        Comments : Comment list option
    }

    static member create id name termAccessionNumber termSourceREF comments= 
        {
            ID = id
            Name = name 
            TermSourceREF = termSourceREF
            TermAccessionNumber = termAccessionNumber  
            Comments = comments
        }

    static member empty =
        OntologyAnnotation.create None None None None None

type OntologySourceReference =
    {
        [<JsonPropertyName("description")>]
        Description : string option
        [<JsonPropertyName("file")>]
        File : string option
        [<JsonPropertyName("name")>]
        Name : string option
        [<JsonPropertyName("version")>]
        Version : string option
        [<JsonPropertyName("comments")>]
        Comments : Comment list option
    }

    static member create description file name version comments  =
        {

            Description = description
            File        = file
            Name        = name
            Version     = version
            Comments    = comments
        }

    static member empty =
        OntologySourceReference.create None None None None None