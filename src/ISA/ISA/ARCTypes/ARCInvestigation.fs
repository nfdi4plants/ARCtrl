namespace ISA

open Fable.Core

[<AttachMembers>]
type Investigation = 

    {Studies : Study array option}

    [<NamedParams>]
    static member create (?Studies : Study []) = 
        {Studies = Studies}

    static member tryGetStudyByID (studyIdentifier : string) (investigation : Investigation) : Study option = 
        raise (System.NotImplementedException())

    static member updateStudyByID (study : Study) (studyIdentifier : string) (investigation : Investigation) : Investigation = 
        Investigation.tryGetStudyByID studyIdentifier investigation |> ignore
        raise (System.NotImplementedException())

    static member addStudy (study : Study) (investigation : Investigation) : Investigation = 
        raise (System.NotImplementedException())

    static member addAssay (assay : Assay) (studyIdentifier : string) (investigation : Investigation) : Investigation = 
        match Investigation.tryGetStudyByID studyIdentifier investigation with
        | Some s ->
             Study.addAssay |> ignore
             Investigation.updateStudyByID |> ignore

        | None ->
             Study.create |> ignore
             Investigation.addStudy |> ignore
        raise (System.NotImplementedException())


    