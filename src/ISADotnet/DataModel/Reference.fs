namespace ISADotNet

open System.Text.Json.Serialization

type URI = string

type EMail = string

type Comment = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI
        [<JsonPropertyName(@"name")>]
        Name : string
        [<JsonPropertyName(@"value")>]
        Value : string
    }
  
    static member create id name value = 
        {
            ID = id
            Name = name 
            Value = value      
        }
        

type Remark = 
    {
        Line : int
        Value : string
    }
    
    static member create line value = 
        {
            Line = line 
            Value = value      
        }

    static member toTuple (remark : Remark ) =
        remark.Line,remark.Value

