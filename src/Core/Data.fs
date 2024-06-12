namespace ARCtrl

open ARCtrl
open ARCtrl.Helper 
open Fable.Core

module DataAux = 
    
    let nameFromPathAndSelector (path : string) (selector : string) =
        sprintf "%s#%s" path selector

    let pathAndSelectorFromName (name : string) =
        let parts = name.Split('#')
        if parts.Length = 2 then
            parts.[0], Some parts.[1]
        else
            name, None

[<AttachMembers>]
type Data(?id,?name : string,?dataType,?format,?selectorFormat,?comments) =

    let mutable _id : URI option = id
    let mutable _filePath,_selector  = 
        match name with
        | Some n -> 
            let p,s = DataAux.pathAndSelectorFromName n
            Some p, s
        | None -> None, None
    let mutable _dataType : DataFile option = dataType
    let mutable _format : string option = format
    let mutable _selectorFormat : URI option = selectorFormat
    let mutable _comments : Comment ResizeArray = Option.defaultValue (ResizeArray()) comments
    
    member this.ID
        with get() = _id
        and set(id) = _id <- id

    member this.Name
        with get() = 
            match _filePath,_selector with
            | Some p, Some s -> DataAux.nameFromPathAndSelector p s |> Some
            | Some p, None -> p |> Some
            | None, _ -> None 
        and set(name) = 
            match name with
            | Some n -> 
                let p,s = DataAux.pathAndSelectorFromName n
                _filePath <- Some p
                _selector <- s
            | None -> 
                _filePath <- None
                _selector <- None

    member this.FilePath 
        with get() = _filePath
        and set(filePath) = _filePath <- filePath

    member this.Selector
        with get() = _selector
        and set(selector) = _selector <- selector

    member this.DataType
        with get() = _dataType
        and set(dataType) = _dataType <- dataType

    member this.Format
        with get() = _format
        and set(format) = _format <- format

    member this.SelectorFormat
        with get() = _selectorFormat
        and set(selectorFormat) = _selectorFormat <- selectorFormat

    member this.Comments
        with get() = _comments
        and set(comments) = _comments <- comments    

    static member make id name dataType format selectorFormat comments =
        Data(?id=id,?name=name,?dataType=dataType,?format=format,?selectorFormat=selectorFormat,?comments=comments)

    static member create (?Id,?Name,?DataType,?Format,?SelectorFormat,?Comments) = 
        Data.make Id Name DataType Format SelectorFormat Comments

    static member empty =
        Data.create()

    member this.NameText =
        this.Name
        |> Option.defaultValue ""

    member this.Copy() =
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        Data(?id=this.ID,?name=this.Name,?dataType=this.DataType,?format=this.Format,?selectorFormat=this.SelectorFormat,comments=nextComments)

    override this.GetHashCode() =
        [|
            HashCodes.boxHashOption this.ID
            HashCodes.boxHashOption this.Name
            HashCodes.boxHashOption this.DataType
            HashCodes.boxHashOption this.Format
            HashCodes.boxHashOption this.SelectorFormat
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray
        |> fun x -> x :?> int

    override this.Equals(obj) =
        HashCodes.hash this = HashCodes.hash obj

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            match this.DataType with
            | Some t ->
                sprintf "%s [%s]" this.NameText t.AsString 
            | None -> sprintf "%s" this.NameText
