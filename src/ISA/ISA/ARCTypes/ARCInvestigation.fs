namespace ISA

open Fable.Core

[<AttachMembers>]
type ARCInvestigation = 

    {Studies : Study array option}

    [<NamedParams>]
    static member create (?Studies : Study []) = 
        {Studies = Studies}

    static member tryGetStudyByID (studyIdentifier : string) (investigation : Investigation) : Study option = 
        raise (System.NotImplementedException())

    static member updateStudyByID (study : Study) (studyIdentifier : string) (investigation : Investigation) : Investigation = 
        ARCInvestigation.tryGetStudyByID studyIdentifier investigation |> ignore
        raise (System.NotImplementedException())

    static member addStudy (study : Study) (investigation : Investigation) : Investigation = 
        raise (System.NotImplementedException())

    static member addAssay (assay : Assay) (studyIdentifier : string) (investigation : Investigation) : Investigation = 
        match ARCInvestigation.tryGetStudyByID studyIdentifier investigation with
        | Some s ->
             ARCStudy.addAssay |> ignore
             ARCInvestigation.updateStudyByID |> ignore

        | None ->
             Study.create |> ignore
             ARCInvestigation.addStudy |> ignore
        raise (System.NotImplementedException())


    