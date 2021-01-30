namespace ISADotNet

open System.Text.Json
open System.Text.Json.Serialization

[<StringEnum>]
type DataFile =

    | [<StringEnumValue("Raw Data File")>]      RawDataFile // "Raw Data File"
    | [<StringEnumValue("Derived Data File")>]  DerivedDataFile // "Derived Data File"
    | [<StringEnumValue("Image File")>]         ImageFile // "Image File"

    static member RawDataFileJson       = "Raw Data File"
    static member DerivedDataFileJson   = "Derived Data File"
    static member ImageFileJson         = "Image File"


type Data = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name : string option
        [<JsonPropertyName(@"type")>]
        DataType : DataFile option
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list option
    }

    static member create id name dataType comments =
        {
            ID      = id
            Name    = name
            DataType = dataType
            Comments = comments         
        }

    static member empty =
        Data.create None None None None

type Source = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name : string option
        [<JsonPropertyName(@"characteristics")>]
        Characteristics : MaterialAttributeValue list option
    }

    static member create id name characteristics : Source=
        {
            ID              = id
            Name            = name
            Characteristics = characteristics          
        }

    static member empty =
        Source.create None None None

type Sample = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name : string option
        [<JsonPropertyName(@"characteristics")>]
        Characteristics : MaterialAttributeValue list option
        [<JsonPropertyName(@"factorValues")>]
        FactorValues : FactorValue list option
        [<JsonPropertyName(@"derivesFrom")>]
        DerivesFrom : Source list option
    }

    static member create id name characteristics factorValues derivesFrom : Sample=
        {
            ID              = id
            Name            = name
            Characteristics = characteristics     
            FactorValues    = factorValues
            DerivesFrom     = derivesFrom       
        }

    static member empty =
        Sample.create None None None None None