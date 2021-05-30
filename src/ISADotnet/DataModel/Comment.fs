namespace ISADotNet

open System.Text.Json.Serialization

type EMail = string

type Comment = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name : string option
        [<JsonPropertyName(@"value")>]
        Value : string option
    }
  
    static member create id name value = 
        {
            ID = id
            Name = name 
            Value = value      
        }

    static member fromStrings name value =
        let name = Option.fromValueWithDefault "" name
        let value = Option.fromValueWithDefault "" value
        Comment.create None name value
    
    static member toStrings (comment : Comment) =
        Option.defaultValue "" comment.Name, Option.defaultValue "" comment.Value

    static member Create(?Id,?Name,?Value) =
        Comment.create Id Name Value


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