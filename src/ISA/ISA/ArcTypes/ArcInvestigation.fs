namespace ISA

open Fable.Core
open ISA.Aux

module ArcInvestigationAux =
    module SanityChecks = 
        let inline validateUniqueStudyIdentifier (study: ArcStudy) (existingStudies: seq<ArcStudy>) =
            match existingStudies |> Seq.tryFindIndex (fun x -> x.Identifier = study.Identifier) with
            | Some i ->
                failwith $"Cannot create study with name {study.Identifier}, as study names must be unique and study at index {i} has the same name."
            | None ->
                ()

[<AttachMembers>]
type ArcInvestigation(identifier : string, ?title : string, ?description : string, ?submissionDate : string, ?publicReleaseDate : string, ?ontologySourceReferences : OntologySourceReference list, ?publications : Publication list, ?contacts : Person list, ?studies : ResizeArray<ArcStudy>, ?comments : Comment list, ?remarks : Remark list) = 

    let ontologySourceReferences = defaultArg ontologySourceReferences []
    let publications = defaultArg publications []
    let contacts = defaultArg contacts []
    let studies = defaultArg studies (ResizeArray())
    let comments = defaultArg comments []
    let remarks = defaultArg remarks []

    let mutable identifier = identifier
    /// Must be unique in one investigation
    member this.Identifier 
        with get() = identifier
        and internal set(i) = identifier <- i

    member val Title : string option = title with get, set
    member val Description : string option = description with get, set
    member val SubmissionDate : string option = submissionDate with get, set
    member val PublicReleaseDate : string option = publicReleaseDate with get, set
    member val OntologySourceReferences : OntologySourceReference list = ontologySourceReferences with get, set
    member val Publications : Publication list = publications with get, set
    member val Contacts : Person list = contacts with get, set
    member val Studies : ResizeArray<ArcStudy> = studies with get, set
    member val Comments : Comment list = comments with get, set
    member val Remarks : Remark list = remarks with get, set

    static member FileName = ARCtrl.Path.InvestigationFileName

    static member init(identifier: string) = ArcInvestigation identifier
    static member create(identifier : string, ?title : string, ?description : string, ?submissionDate : string, ?publicReleaseDate : string, ?ontologySourceReferences : OntologySourceReference list, ?publications : Publication list, ?contacts : Person list, ?studies : ResizeArray<ArcStudy>, ?comments : Comment list, ?remarks : Remark list) = 
        ArcInvestigation(identifier, ?title = title, ?description = description, ?submissionDate = submissionDate, ?publicReleaseDate = publicReleaseDate, ?ontologySourceReferences = ontologySourceReferences, ?publications = publications, ?contacts = contacts, ?studies = studies, ?comments = comments, ?remarks = remarks)

    static member make (identifier : string) (title : string option) (description : string option) (submissionDate : string option) (publicReleaseDate : string option) (ontologySourceReferences : OntologySourceReference list) (publications : Publication list) (contacts : Person list) (studies : ResizeArray<ArcStudy>) (comments : Comment list) (remarks : Remark list) : ArcInvestigation =
        ArcInvestigation(identifier, ?title = title, ?description = description, ?submissionDate = submissionDate, ?publicReleaseDate = publicReleaseDate, ontologySourceReferences = ontologySourceReferences, publications = publications, contacts = contacts, studies = studies, comments = comments, remarks = remarks)


    member this.StudyCount 
        with get() = this.Studies.Count

    member this.StudyIdentifiers
        with get() = this.Studies |> Seq.map (fun (x:ArcStudy) -> x.Identifier)

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
    member this.SetAssay(studyIdentifier: string, assayIdentifier: string, assay: ArcAssay) =
        let study = this.GetStudy(studyIdentifier)
        let index = study.GetAssayIndex assayIdentifier
        study.SetAssayAt (index, assay)

    static member setAssay(studyIdentifier: string, assayIdentifier: string, assay: ArcAssay) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.SetAssay(studyIdentifier, assayIdentifier, assay)

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
        ArcInvestigation(
            this.Identifier,
            ?title = this.Title,
            ?description = this.Description,
            ?submissionDate = this.SubmissionDate,
            ?publicReleaseDate = this.PublicReleaseDate,
            ontologySourceReferences = this.OntologySourceReferences,
            publications = this.Publications,
            contacts = this.Contacts,
            studies = newStudies, // correct mutable behaviour is tested on this field
            comments = this.Comments,
            remarks = this.Remarks
        )


    