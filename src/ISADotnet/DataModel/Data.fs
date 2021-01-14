namespace ISADotNet

open System.Text.Json.Serialization

type DataFile =

    | RawDataFile // "Raw Data File"
    | DerivedDataFile // "Derived Data File"
    | ImageFile // "Image File"

    static member RawDataFileJson       = "Raw Data File"
    static member DerivedDataFileJson   = "Derived Data File"
    static member ImageFileJson         = "Image File"


type Data = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : string
        [<JsonPropertyName(@"name")>]
        Name : string
        [<JsonPropertyName(@"type")>]
        DataType : DataFile
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list  
    }

    static member create id name dataType comments =
        {
            ID      = id
            Name    = name
            DataType = dataType
            Comments = comments         
        }


type Source = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : string 
        [<JsonPropertyName(@"name")>]
        Name : string
        [<JsonPropertyName(@"characteristics")>]
        Characteristics : MaterialAttributeValue
    }

    static member create id name characteristics : Source=
        {
            ID              = id
            Name            = name
            Characteristics = characteristics          
        }


type Sample = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : string
        [<JsonPropertyName(@"name")>]
        Name : string
        [<JsonPropertyName(@"characteristics")>]
        Characteristics : MaterialAttributeValue list
        [<JsonPropertyName(@"factorValues")>]
        FactorValues : FactorValue list
        [<JsonPropertyName(@"derivesFrom")>]
        DerivesFrom : Source
    }

    static member create id name characteristics factorValues derivesFrom : Sample=
        {
            ID              = id
            Name            = name
            Characteristics = characteristics     
            FactorValues    = factorValues
            DerivesFrom     = derivesFrom       
        }