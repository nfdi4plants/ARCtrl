namespace ISA

open Fable.Core

[<AttachMembers>]
type ArcInvestigation = 

    {
        ID : URI option
        FileName : string option
        Identifier : string option
        Title : string option
        Description : string option
        SubmissionDate : string option
        PublicReleaseDate : string option
        OntologySourceReferences : OntologySourceReference list option
        Publications : Publication list option
        Contacts : Person list option
        Studies : ArcStudy list option
        Comments : Comment list option
        Remarks     : Remark list
    }

    static member make (id : URI option) (filename : string option) (identifier : string option) (title : string option) (description : string option) (submissionDate : string option) (publicReleaseDate : string option) (ontologySourceReference : OntologySourceReference list option) (publications : Publication list option) (contacts : Person list option) (studies : ArcStudy list option) (comments : Comment list option) (remarks : Remark list) : ArcInvestigation =
        {ID = id; FileName = filename; Identifier = identifier; Title = title; Description = description; SubmissionDate = submissionDate; PublicReleaseDate = publicReleaseDate; OntologySourceReferences = ontologySourceReference; Publications = publications; Contacts = contacts; Studies = studies; Comments = comments; Remarks = remarks}

    [<NamedParams>]
    static member create (?Id : URI, ?FileName : string, ?Identifier : string, ?Title : string, ?Description : string, ?SubmissionDate : string, ?PublicReleaseDate : string, ?OntologySourceReferences : OntologySourceReference list, ?Publications : Publication list, ?Contacts : Person list, ?Studies : ArcStudy list, ?Comments : Comment list, ?Remarks : Remark list) : ArcInvestigation =
        ArcInvestigation.make Id FileName Identifier Title Description SubmissionDate PublicReleaseDate OntologySourceReferences Publications Contacts Studies Comments (Remarks |> Option.defaultValue [])

    static member tryGetStudyByID (studyIdentifier : string) (investigation : Investigation) : Study option = 
        raise (System.NotImplementedException())

    static member updateStudyByID (study : Study) (studyIdentifier : string) (investigation : Investigation) : Investigation = 
        ArcInvestigation.tryGetStudyByID studyIdentifier investigation |> ignore
        raise (System.NotImplementedException())

    static member addStudy (study : Study) (investigation : Investigation) : Investigation = 
        raise (System.NotImplementedException())

    static member addAssay (assay : ArcAssay) (studyIdentifier : string) (investigation : Investigation) : Investigation = 
        match ArcInvestigation.tryGetStudyByID studyIdentifier investigation with
        | Some s ->
             ArcStudy.addAssay (assay) |> ignore
             ArcInvestigation.updateStudyByID |> ignore

        | None ->
             Study.create |> ignore
             ArcInvestigation.addStudy |> ignore
        raise (System.NotImplementedException())


    