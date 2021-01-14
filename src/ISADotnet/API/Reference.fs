namespace ISADotNet

type Comment = 
    {
        ID : string
        Name : string
        Value : string
    }
  
    static member create id name value = 
        {
            ID = id
            Name = name 
            Value = value      
        }
        
    static member NameJson = "name"
    static member ValueJson = "value"

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

type REF<'T> =
    | ID of string
    | Item of 'T

    static member toID ref =
        match ref with
        | ID s -> s
        | Item i -> failwith "REF contained item, and not id"

    static member toItem ref =
        match ref with
        | ID s -> failwith "REF contained id, and not item"
        | Item i -> i