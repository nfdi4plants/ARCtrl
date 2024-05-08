namespace ARCtrl

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper 

type Data = 
    {
        ID : URI option
        Name : string option
        DataType : DataFile option
        Format : string option
        SelectorFormat : URI option
        Comments : Comment list option
    }

    static member make id name dataType format selectorFormat comments =
        {
            ID      = id
            Name    = name
            DataType = dataType
            Format  = format
            SelectorFormat = selectorFormat
            Comments = comments         
        }

    static member create (?Id,?Name,?DataType,?Format,?SelectorFormat,?Comments) = 
        Data.make Id Name DataType Format SelectorFormat Comments

    static member empty =
        Data.create()

    member this.NameText =
        this.Name
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            match this.DataType with
            | Some t ->
                sprintf "%s [%s]" this.NameText t.AsString 
            | None -> sprintf "%s" this.NameText
