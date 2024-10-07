namespace ARCtrl

type EMail = string

open Fable.Core
open ARCtrl.Helper
open System.Text.RegularExpressions

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

    override this.ToString() =
        let sb = System.Text.StringBuilder()
        sb.Append("Comment {") |> ignore
        [
            "Name", this.Name
            "Value", this.Value
        ] 
        |> List.choose (fun (s,opt) -> opt |> Option.map (fun o -> s,o))
        |> List.map (fun (s,v) -> sprintf "%s = \"%s\"" s v)
        |> String.concat ", "
        |> sb.Append
        |> ignore
        sb.Append("}") |> ignore
        sb.ToString()

    // Reverse function to ToString() override
    static member fromString(s) =


        // Buggy because of fable

        //let namePattern = Regex.Pattern.handleGroupPatterns """(?<=Name = ")[^"]*(?=")"""
        ////let nameResult = Regex(namePattern).Match(s)
        //let nameResult = Regex.Match(s, namePattern)

        //let valuePattern = Regex.Pattern.handleGroupPatterns """(?<=Value = ")[^"]*(?=")"""
        ////let valueResult = Regex(valuePattern).Match(s)
        //let valueResult = Regex.Match(s, valuePattern)

        //let name = if nameResult.Success then Some nameResult.Value else None
        //let value = if valueResult.Success then Some valueResult.Value else None



        let namePattern = Regex.Pattern.handleGroupPatterns "Name = \"[^\"]*\""
        //let nameResult = Regex(namePattern).Match(s)
        let nameResult = Regex.Match(s, namePattern)

        let valuePattern = Regex.Pattern.handleGroupPatterns "Value = \"[^\"]*\""
        //let valueResult = Regex(valuePattern).Match(s)
        let valueResult = Regex.Match(s, valuePattern)

        let name = if nameResult.Success then Some (nameResult.Value.Replace("Name = ","").Replace("\"","")) else None
        let value = if valueResult.Success then Some (valueResult.Value.Replace("Value = ","").Replace("\"","")) else None









        Comment(?name=name, ?value=value)


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