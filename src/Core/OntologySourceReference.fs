namespace ARCtrl

open ARCtrl.Helper
open Fable.Core

[<AttachMembers>]
type OntologySourceReference(?description, ?file, ?name, ?version, ?comments) =
    
    let mutable _description : string option = description
    let mutable _file : string option = file
    let mutable _name : string option = name
    let mutable _version : string option = version
    let mutable _comments : ResizeArray<Comment> = Option.defaultValue (ResizeArray()) comments

    member this.Description
        with get() = _description
        and set(description) = _description <- description

    member this.File
        with get() = _file
        and set(file) = _file <- file

    member this.Name
        with get() = _name
        and set(name) = _name <- name

    member this.Version
        with get() = _version
        and set(version) = _version <- version

    member this.Comments
        with get() = _comments
        and set(comments) = _comments <- comments


    static member make description file name version comments = 
        OntologySourceReference(?description=description, ?file=file, ?name=name, ?version=version, comments=comments)

    static member create(?description,?file,?name,?version,?comments) : OntologySourceReference =
        OntologySourceReference.make description file name version (Option.defaultValue (ResizeArray()) comments)

    static member empty =
        OntologySourceReference.create()

    member this.Copy() =
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        OntologySourceReference.make this.Description this.File this.Name this.Version nextComments

    override this.GetHashCode() : int = 
        [|
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.File
            HashCodes.boxHashOption this.Name
            HashCodes.boxHashOption this.Version
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray 
        |> fun x -> x :?> int

    override this.Equals(obj) =
        match obj with
        | :? OntologySourceReference as t -> this.GetHashCode() = t.GetHashCode()
        | _ -> false