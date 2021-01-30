namespace ISADotNet

open System.Text.Json.Serialization

type MaterialAttribute = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"characteristicType")>]
        CharacteristicType : OntologyAnnotation option
    
    }

    static member create id characteristicType =
        {
            ID = id
            CharacteristicType = characteristicType     
        }

    static member empty =
        MaterialAttribute.create None None 

type MaterialAttributeValue = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"category")>]
        Category : MaterialAttribute option
        [<JsonPropertyName(@"value")>]
        Value : Value option
        [<JsonPropertyName(@"unit")>]
        Unit : OntologyAnnotation option
    
    }

    static member create id category value unit : MaterialAttributeValue =
        {
            ID      = id
            Category = category
            Value = value
            Unit = unit         
        }

    static member empty =
        MaterialAttributeValue.create None None None None

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
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name : string option
        [<JsonPropertyName(@"type")>]
        MaterialType : MaterialType option
        [<JsonPropertyName(@"characteristics")>]
        Characteristics : MaterialAttributeValue list option
        [<JsonPropertyName(@"derivesFrom")>]
        DerivesFrom : OntologyAnnotation option
    
    }

    static member create id name materialType characteristics derivesFrom : Material=
        {
            ID              = id
            Name            = name
            MaterialType    = materialType
            Characteristics = characteristics     
            DerivesFrom     = derivesFrom       
        }

    static member empty =
        Material.create None None None None None