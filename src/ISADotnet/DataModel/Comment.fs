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

    static member create(?Id,?Name,?Value) : Comment =
        {
            ID      = Id
            Name    = Name
            Value   = Value
        }

    static member fromStrings name value =
        Comment.create (Name=name,Value=value)
    
    static member toStrings (comment : Comment) =
        Option.defaultValue "" comment.Name, Option.defaultValue "" comment.Value



type Remark = 
    {
        Line : int 
        Value : string
    }
    
    static member create(line,value) : Remark = 
        {
            Line = line 
            Value = value      
        }

    static member toTuple (remark : Remark ) =
        remark.Line, remark.Value