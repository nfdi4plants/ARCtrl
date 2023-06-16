namespace ISA

open Fable.Core

[<AttachMembers>]
type ArcInvestigation = 

    {Studies : Study array option}

    [<NamedParams>]
    static member create (?Studies : Study []) = 
        {Studies = Studies}

    static member tryGetStudyByID (studyIdentifier : string) (investigation : Investigation) : Study option = 
        raise (System.NotImplementedException())

    static member updateStudyByID (study : Study) (studyIdentifier : string) (investigation : Investigation) : Investigation = 
        ArcInvestigation.tryGetStudyByID studyIdentifier investigation |> ignore
        raise (System.NotImplementedException())

    static member addStudy (study : Study) (investigation : Investigation) : Investigation = 
        raise (System.NotImplementedException())

    static member addAssay (assay : Assay) (studyIdentifier : string) (investigation : Investigation) : Investigation = 
        match ArcInvestigation.tryGetStudyByID studyIdentifier investigation with
        | Some s ->
             ArcStudy.addAssay |> ignore
             ArcInvestigation.updateStudyByID |> ignore

        | None ->
             Study.create |> ignore
             ArcInvestigation.addStudy |> ignore
        raise (System.NotImplementedException())


    