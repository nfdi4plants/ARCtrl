namespace ARCtrl

open ARCtrl.Helper
open Fable.Core

[<AttachMembers>]
type Publication(?pubMedID, ?doi, ?authors, ?title, ?status, ?comments) =

    let mutable _pubMedID : URI option = pubMedID
    let mutable _doi : string option = doi
    let mutable _authors : string option = authors
    let mutable _title : string option = title
    let mutable _status : OntologyAnnotation option = status
    let mutable _comments : ResizeArray<Comment> = Option.defaultValue (ResizeArray()) comments

    member this.PubMedID
        with get() = _pubMedID
        and set(pubMedID) = _pubMedID <- pubMedID

    member this.DOI
        with get() = _doi
        and set(doi) = _doi <- doi

    member this.Authors
        with get() = _authors
        and set(authors) = _authors <- authors

    member this.Title
        with get() = _title
        and set(title) = _title <- title

    member this.Status
        with get() = _status
        and set(status) = _status <- status

    member this.Comments
        with get() = _comments
        and set(comments) = _comments <- comments

    static member make pubMedID doi authors title status comments =
        Publication(?pubMedID=pubMedID, ?doi=doi, ?authors=authors, ?title=title, ?status=status, comments=comments)

    static member create(?pubMedID,?doi,?authors,?title,?status,?comments) : Publication =
       Publication.make pubMedID doi authors title status (Option.defaultValue (ResizeArray()) comments)

    static member empty() =
        Publication.create()

    member this.Copy() =
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        Publication.make this.PubMedID this.DOI this.Authors this.Title this.Status nextComments

    override this.GetHashCode() =
        [|
            HashCodes.boxHashOption this.DOI
            HashCodes.boxHashOption this.Title
            HashCodes.boxHashOption this.Authors
            HashCodes.boxHashOption this.PubMedID
            HashCodes.boxHashOption this.Status
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray 
        |> fun x -> x :?> int

    override this.Equals(obj) =
        match obj with
        | :? Publication as p -> 
            this.GetHashCode() = p.GetHashCode()
        | _ -> false

    //let mutable _pubMedID : URI option = pubMedID
    //let mutable _doi : string option = doi
    //let mutable _authors : string option = authors
    //let mutable _title : string option = title
    //let mutable _status : OntologyAnnotation option = status
    //let mutable _comments : ResizeArray<Comment> = Option.defaultValue (ResizeArray()) comments

    override this.ToString() =
        let sb = System.Text.StringBuilder()
        sb.Append("Publication {\n\t") |> ignore
        [
            "PubMedID", this.PubMedID
            "DOI", this.DOI
            "Authors", this.Authors
            "Title", this.Title
            "Status", 
                this.Status
                |> Option.map (sprintf "%A")
            "Comments", 
                if Seq.length this.Comments > 0 then 
                    this.Comments
                    |> sprintf "%A"
                    |> Some
                else
                    None
        ] 
        |> List.choose (fun (s,opt) -> opt |> Option.map (fun o -> s,o))
        |> List.map (fun (s,v) -> sprintf "%s = %A" s v)
        |> String.concat ",\n\t"
        |> sb.Append
        |> ignore
        sb.Append("\n}") |> ignore
        sb.ToString()