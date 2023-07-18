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
        mutable FileName : string option
        Identifier : string
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

    static member make (filename : string option) (identifier : string) (title : string option) (description : string option) (submissionDate : string option) (publicReleaseDate : string option) (ontologySourceReference : OntologySourceReference list) (publications : Publication list) (contacts : Person list) (studies : ResizeArray<ArcStudy>) (comments : Comment list) (remarks : Remark list) : ArcInvestigation =
        {FileName = filename; Identifier = identifier; Title = title; Description = description; SubmissionDate = submissionDate; PublicReleaseDate = publicReleaseDate; OntologySourceReferences = ontologySourceReference; Publications = publications; Contacts = contacts; Studies = studies; Comments = comments; Remarks = remarks}

    member this.StudyCount 
        with get() = this.Studies.Count

    member this.StudyIdentifiers
        with get() = this.Studies |> Seq.map (fun (x:ArcStudy) -> x.Identifier)

    [<NamedParams>]
    static member create (identifier : string, ?fileName : string, ?title : string, ?description : string, ?submissionDate : string, ?publicReleaseDate : string, ?ontologySourceReferences : OntologySourceReference list, ?publications : Publication list, ?contacts : Person list, ?studies : ResizeArray<ArcStudy>, ?comments : Comment list, ?remarks : Remark list) : ArcInvestigation =
        let ontologySourceReferences = defaultArg ontologySourceReferences []
        let publications = defaultArg publications []
        let contacts = defaultArg contacts []
        let studies = defaultArg studies (ResizeArray())
        let comments = defaultArg comments []
        let remarks = defaultArg remarks []
        ArcInvestigation.make fileName identifier title description submissionDate publicReleaseDate ontologySourceReferences publications contacts studies comments remarks

    static member init(identifier: string) = ArcInvestigation.create identifier

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
    member this.InitStudy (studyName: string) =
        let study = ArcStudy.init(studyName)
        this.AddStudy(study)

    static member initStudy(studyName: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.InitStudy(studyName)
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
    member this.RemoveStudy(studyIdentifier: string) =
        this.GetStudy(studyIdentifier)
        |> this.Studies.Remove
        |> ignore

    static member removeStudy(studyIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.RemoveStudy(studyIdentifier)
            copy

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
    member this.SetStudy(studyIdentifier: string, study: ArcStudy) =
        let index = this.GetStudyIndex studyIdentifier
        ArcInvestigationAux.SanityChecks.validateUniqueStudyIdentifier study (this.Studies |> Seq.removeAt index)
        this.Studies.[index] <- study

    static member setStudy(studyIdentifier: string, study: ArcStudy) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.SetStudy(studyIdentifier, study)
            newInv

    // - Study API - CRUD //
    member this.GetStudyIndex(studyIdentifier: string) : int =
        let index = this.Studies.FindIndex (fun s -> s.Identifier = studyIdentifier)
        if index = -1 then failwith $"Unable to find study with specified identifier '{studyIdentifier}'!"
        index

    // - Study API - CRUD //
    static member getStudyIndex(studyIdentifier: string) : ArcInvestigation -> int =
        fun (inv: ArcInvestigation) -> inv.GetStudyIndex (studyIdentifier)

    // - Study API - CRUD //
    member this.GetStudyAt(index: int) : ArcStudy =
        this.Studies.[index]

    static member getStudyAt(index: int) : ArcInvestigation -> ArcStudy =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.GetStudyAt(index)

    // - Study API - CRUD //
    member this.GetStudy(studyIdentifier: string) : ArcStudy =
        this.Studies.Find (fun s -> s.Identifier = studyIdentifier)

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
    member this.AddAssayAt(studyIndex: int, assay: ArcAssay) =
        let study = this.GetStudyAt(studyIndex)
        ArcStudyAux.SanityChecks.validateUniqueAssayIdentifier assay study.Assays
        study.AddAssay(assay)

    static member addAssayAt(studyIndex: int, assay: ArcAssay) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.AddAssayAt(studyIndex, assay)
            copy

    // - Study API - CRUD //
    member this.InitAssay(studyIdentifier: string, assayName: string) =
        let assay = ArcAssay.init(assayName)
        this.AddAssay(studyIdentifier, assay)

    static member initAssay(studyIdentifier: string, assayName: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.InitAssay(studyIdentifier, assayName)
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
        study.SetAssayAt(index, assay)
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

    // - Study API - CRUD //
    member this.GetAssay(studyIdentifier: string, assayIdentifier: string) : ArcAssay =
        let study = this.GetStudy(studyIdentifier)
        let index = study.GetAssayIndex assayIdentifier
        study.GetAssayAt index

    static member getAssay(studyIdentifier: string, assayIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.GetAssay(studyIdentifier, assayIdentifier)

    //member this.TryFindStudyForAssay(assayIdentifier: string) =
    //    let idents = this.Studies |> Seq.map (fun s -> s.Identifier, s.AssayIdentifiers)
    //    idents |> Seq.tryFind (fun (s, aArr) ->
    //        aArr |> Seq.contains assayIdentifier
    //    )

    member this.Copy() : ArcInvestigation =
        let newStudies = ResizeArray()
        for study in this.Studies do
            let copy = study.Copy()
            newStudies.Add(copy)
        { this with 
            Studies = newStudies
        }


    