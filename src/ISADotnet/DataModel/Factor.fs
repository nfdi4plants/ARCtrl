namespace ISADotNet

open System.Text.Json.Serialization

type Factor = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : string
        [<JsonPropertyName(@"factorName")>]
        Name : string
        [<JsonPropertyName(@"factorType")>]
        FactorType : OntologyAnnotation
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list
    
    }

    static member create id name factorType comments =
        {
            ID      = id
            Name    = name
            FactorType = factorType
            Comments = comments         
        }

    static member NameTab = "Name"
    static member FactorTypeTab = "Type"
    static member TypeTermAccessionNumberTab = "Type Term Accession Number"
    static member TypeTermSourceREFTab = "Type Term Source REF"



type Value =
    | Ontology of OntologyAnnotation
    | Numeric of float
    | Name of string


type FactorValue =
    {
        [<JsonPropertyName(@"@id")>]
        ID : string
        [<JsonPropertyName(@"category")>]
        Category : Factor
        [<JsonPropertyName(@"value")>]
        Value : Value
        [<JsonPropertyName(@"unit")>]
        Unit : OntologyAnnotation
    
    }

    static member create id category value unit =
        {
            ID      = id
            Category = category
            Value = value
            Unit = unit         
        }
