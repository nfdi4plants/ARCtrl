namespace ARCtrl

type EMail = string

open Fable.Core
open ARCtrl.Helper

[<AttachMembers>]
type Comment(?name, ?value) =
    
    let mutable _name : string option = name
    let mutable _value : string option = value

    member this.Name 
        with get() = _name
        and set(name) = _name <- name

    member this.Value
        with get() = _value
        and set(value) = _value <- value

    static member make name value = Comment(?name=name, ?value=value)

    static member create(?name,?value) : Comment =
        Comment.make name value
    
    static member toString (comment : Comment) =
        Option.defaultValue "" comment.Name, Option.defaultValue "" comment.Value

    member this.Copy() =
        Comment.make this.Name this.Value

    override this.Equals(obj) =
        match obj with
        | :? Comment as c -> c.Name = this.Name && c.Value = this.Value
        | _ -> false

    override this.GetHashCode() =        
        [|
            HashCodes.boxHashOption this.Name
            HashCodes.boxHashOption this.Value
        |]
        |> HashCodes.boxHashArray
        |> fun x -> x :?> int


[<AttachMembers>]
type Remark = 
    {
        Line : int 
        Value : string
    }
    
    static member make line value  : Remark = 
        {
            Line = line 
            Value = value      
        }

    static member create(line,value) : Remark = 
        Remark.make line value

    static member toTuple (remark : Remark ) =
        remark.Line, remark.Value

    member this.Copy() =
        Remark.make this.Line this.Value