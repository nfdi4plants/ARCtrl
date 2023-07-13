namespace ISA

open Fable.Core
open ISA.Aux

module ArcInvestigationAux =
    module SanityChecks = 
        let inline validateUniqueStudyIdentifier (study: ArcStudy) (existingStudies: seq<ArcStudy>) =
            match Seq.tryFindIndex (fun x -> x.Identifier = study.Identifier) existingStudies with
            | Some i ->
                failwith $"Cannot create study with name {study.Identifier}, as study names must be unique and study at index {i} has the same name."
            | None ->
                ()

[<AttachMembers>]
type ArcInvestigation = 

    {
        ID : URI option
        mutable FileName : string option
        Identifier : string option
        mutable Title : string option
        mutable Description : string option
        mutable SubmissionDate : string option
        mutable PublicReleaseDate : string option
        mutable OntologySourceReferences : OntologySourceReference list
        mutable Publications : Publication list
        mutable Contacts : Person list
        Studies : ResizeArray<ArcStudy>
        mutable Comments : Comment list
        mutable Remarks : Remark list
    }

    static member make (id : URI option) (filename : string option) (identifier : string option) (title : string option) (description : string option) (submissionDate : string option) (publicReleaseDate : string option) (ontologySourceReference : OntologySourceReference list) (publications : Publication list) (contacts : Person list) (studies : ResizeArray<ArcStudy>) (comments : Comment list) (remarks : Remark list) : ArcInvestigation =
        {ID = id; FileName = filename; Identifier = identifier; Title = title; Description = description; SubmissionDate = submissionDate; PublicReleaseDate = publicReleaseDate; OntologySourceReferences = ontologySourceReference; Publications = publications; Contacts = contacts; Studies = studies; Comments = comments; Remarks = remarks}

    [<NamedParams>]
    static member create (identifier : string, ?id : URI, ?fileName : string, ?title : string, ?description : string, ?submissionDate : string, ?publicReleaseDate : string, ?ontologySourceReferences : OntologySourceReference list, ?publications : Publication list, ?contacts : Person list, ?studies : ResizeArray<ArcStudy>, ?comments : Comment list, ?remarks : Remark list) : ArcInvestigation =
        let ontologySourceReferences = defaultArg ontologySourceReferences []
        let publications = defaultArg publications []
        let contacts = defaultArg contacts []
        let studies = defaultArg studies (ResizeArray())
        let comments = defaultArg comments []
        let remarks = defaultArg remarks []
        ArcInvestigation.make id fileName (Option.fromValueWithDefault "" identifier) title description submissionDate publicReleaseDate ontologySourceReferences publications contacts studies comments remarks

    static member createEmpty() =
        ArcInvestigation.make None None None None None None None [] [] [] (ResizeArray()) [] []

    //static member tryGetStudyByID (studyIdentifier : string) (investigation : Investigation) : Study option = 
    //    raise (System.NotImplementedException())

    //static member updateStudyByID (study : Study) (studyIdentifier : string) (investigation : Investigation) : Investigation = 
    //    ArcInvestigation.tryGetStudyByID studyIdentifier investigation |> ignore
    //    raise (System.NotImplementedException())

    // - Study API - CRUD //
    member this.AddStudy(study: ArcStudy) =
        ArcInvestigationAux.SanityChecks.validateUniqueStudyIdentifier study this.Studies
        this.Studies.Add(study)

    static member addStudy(study: ArcStudy) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.AddStudy(study)
            copy

    // - Study API - CRUD //
    member this.AddStudyEmpty(studyName: string) =
        let study = ArcStudy.create(studyName)
        this.AddStudy(study)

    static member addStudyEmpty(studyName: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            let study = ArcStudy.create(studyName)
            copy.AddStudy(study)
            copy

    // - Study API - CRUD //
    member this.RemoveStudyAt(index: int) =
        this.Studies.RemoveAt(index)

    static member removeStudyAt(index: int) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.RemoveStudyAt(index)
            newInv

    // - Study API - CRUD //
    member this.SetStudyAt(index: int, study: ArcStudy) =
        ArcInvestigationAux.SanityChecks.validateUniqueStudyIdentifier study (this.Studies |> Seq.removeAt index)
        this.Studies.[index] <- study

    static member setStudyAt(index: int, study: ArcStudy) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.SetStudyAt(index, study)
            newInv

    // - Study API - CRUD //
    member this.GetStudyAt(index: int) : ArcStudy =
        this.Studies.[index]

    static member getStudyAt(index: int) : ArcInvestigation -> ArcStudy =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.GetStudyAt(index)

    // - Study API - CRUD //
    member this.GetStudy(studyIdentifier: string) : ArcStudy =
        this.Studies.Find (fun s -> s.Identifier = Some studyIdentifier)

    static member getStudy(studyIdentifier: string) : ArcInvestigation -> ArcStudy =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.GetStudy(studyIdentifier)

    // - Study API - CRUD //
    member this.AddAssay(studyIdentifier: string, assay: ArcAssay) =
        let study = this.GetStudy(studyIdentifier)
        ArcStudyAux.SanityChecks.validateUniqueAssayIdentifier assay study.Assays
        study.AddAssay(assay)

    static member addAssay(studyIdentifier: string, assay: ArcAssay) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.AddAssay(studyIdentifier, assay)
            copy

    // - Study API - CRUD //
    member this.AddAssayEmpty(studyIdentifier: string, assayName: string) =
        let assay = ArcAssay.create(assayName)
        this.AddAssay(studyIdentifier, assay)

    static member addAssayEmpty(studyIdentifier: string, assayName: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.AddAssayEmpty(studyIdentifier, assayName)
            copy

    // - Study API - CRUD //
    member this.RemoveAssayAt(studyIdentifier: string, index: int) =
        let study = this.GetStudy(studyIdentifier)
        study.Assays.RemoveAt(index)

    static member removeAssayAt(studyIdentifier: string, index: int) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.RemoveAssayAt(studyIdentifier, index)
            newInv

    // - Study API - CRUD //
    member this.SetAssayAt(studyIdentifier: string, index: int, assay: ArcAssay) =
        let study = this.GetStudy(studyIdentifier)
        ArcStudyAux.SanityChecks.validateUniqueAssayIdentifier assay (study.Assays |> Seq.removeAt index)
        this.Studies.[index] <- study

    static member setAssayAt(studyIdentifier: string, index: int, assay: ArcAssay) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.SetAssayAt(studyIdentifier, index, assay)
            newInv

    // - Study API - CRUD //
    member this.GetAssayAt(studyIdentifier: string, index: int) : ArcAssay =
        let study = this.GetStudy(studyIdentifier)
        study.GetAssayAt index

    static member getAssayAt(studyIdentifier: string, index: int) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.GetAssayAt(studyIdentifier, index)

    member this.Copy() : ArcInvestigation =
        let newStudies = ResizeArray()
        for study in this.Studies do
            let copy = study.Copy()
            newStudies.Add(copy)
        { this with 
            Studies = newStudies
        }
    //static member addAssay (assay : ArcAssay) (studyIdentifier : string) (investigation : Investigation) : Investigation = 
    //    match ArcInvestigation.tryGetStudyByID studyIdentifier investigation with
    //    | Some s ->
    //         ArcStudy.addAssay (assay) |> ignore
    //         ArcInvestigation.updateStudyByID |> ignore

    //    | None ->
    //         Study.create |> ignore
    //         ArcInvestigation.addStudy |> ignore
    //    raise (System.NotImplementedException())


    