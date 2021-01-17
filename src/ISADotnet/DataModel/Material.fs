namespace ISADotNet

open System.Text.Json.Serialization

type MaterialAttribute = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI
        [<JsonPropertyName(@"characteristicType")>]
        CharacteristicType : OntologyAnnotation
    
    }

    static member create id characteristicType =
        {
            ID = id
            CharacteristicType = characteristicType     
        }


type MaterialAttributeValue = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI
        [<JsonPropertyName(@"category")>]
        Category : MaterialAttribute
        [<JsonPropertyName(@"value")>]
        Value : Value
        [<JsonPropertyName(@"unit")>]
        Unit : OntologyAnnotation
    
    }

    static member create id category value unit : MaterialAttributeValue =
        {
            ID      = id
            Category = category
            Value = value
            Unit = unit         
        }


[<StringEnumAttribute>]
type MaterialType =
    | [<StringEnumValue("Extract Name")>]           ExtractName // "Extract Name"
    | [<StringEnumValue("Labeled Extract Name")>]   LabeledExtractName // "Labeled Extract Name"

    static member create t =
        if t = "Extract Name" then ExtractName
        elif t = "Labeled Extract Name" then LabeledExtractName
        else failwith "No other value than \"Extract Name\" or \"Labeled Extract Name\" allowed for materialtype"


type Material = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI
        [<JsonPropertyName(@"name")>]
        Name : string
        [<JsonPropertyName(@"type")>]
        MaterialType : MaterialType
        [<JsonPropertyName(@"characteristics")>]
        Characteristics : MaterialAttributeValue list
        [<JsonPropertyName(@"derivesFrom")>]
        DerivesFrom : OntologyAnnotation
    
    }

    static member create id name materialType characteristics derivesFrom : Material=
        {
            ID              = id
            Name            = name
            MaterialType    = materialType
            Characteristics = characteristics     
            DerivesFrom     = derivesFrom       
        }
