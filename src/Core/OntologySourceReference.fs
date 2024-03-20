namespace ARCtrl

open ARCtrl.Helper
open Update
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

    /// Updates all ontology source references for which the predicate returns true with the given ontology source reference values
    static member updateBy (predicate : OntologySourceReference -> bool) (updateOption : UpdateOptions) (ontologySourceReference : OntologySourceReference) (ontologies : OntologySourceReference list) =
        if List.exists predicate ontologies then
            ontologies
            |> List.map (fun t -> if predicate t then updateOption.updateRecordType t ontologySourceReference else t) 
        else 
            ontologies

    /// If an ontology source reference with the same name as the given name exists in the investigation, updates it with the given ontology source reference
    static member updateByName (updateOption:UpdateOptions) (ontologySourceReference : OntologySourceReference) (ontologies:OntologySourceReference list) =
        OntologySourceReference.updateBy (fun t -> t.Name = ontologySourceReference.Name) updateOption ontologySourceReference ontologies

    member this.Copy() =
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        OntologySourceReference.make this.Description this.File this.Name this.Version nextComments