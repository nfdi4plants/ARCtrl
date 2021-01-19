namespace ISADotNet

open System.Text.Json.Serialization

type Factor = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI
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

[<AnyOf>]
type Value =
    | [<SerializationOrder(0)>] Ontology of OntologyAnnotation
    | [<SerializationOrder(1)>] Int of int
    | [<SerializationOrder(2)>] Float of float
    | [<SerializationOrder(3)>] Name of string


type FactorValue =
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI
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
