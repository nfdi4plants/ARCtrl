module ArcStudy.Tests

open ARCtrl.ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let createExampleAssays() = ResizeArray([|ArcAssay.init("Assay 1")|])

let getAssayIdentifiers(assays: ResizeArray<ArcAssay>) = assays |> Seq.map (fun a -> a.Identifier) |> ResizeArray

let private test_create =
    testList "create" [
        testCase "constructor" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Study Title"
            let description = "Study Description"
            let submissionDate = "2023-07-19"
            let publicReleaseDate = "2023-12-31"
            let publications = [|Publication.create("Publication 1")|]
            let contacts = [|Person.create(FirstName = "John", LastName = "Doe")|]
            let studyDesignDescriptors = [|OntologyAnnotation.fromString("Design Descriptor")|]
            let tables = ResizeArray([|ArcTable.init("Table 1")|])
            let assays = createExampleAssays()
            let assay_identifiers = getAssayIdentifiers assays
            let factors = [|Factor.create("Factor 1")|]
            let comments = [|Comment.create("Comment 1")|]

            let actual =
                ArcStudy(
                    identifier = identifier,
                    title = title,
                    description = description,
                    submissionDate = submissionDate,
                    publicReleaseDate = publicReleaseDate,
                    publications = publications,
                    contacts = contacts,
                    studyDesignDescriptors = studyDesignDescriptors,
                    tables = tables,
                    assays = assay_identifiers,
                    factors = factors,
                    comments = comments
                )

            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title (Some title) "Title"
            Expect.equal actual.Description (Some description) "Description"
            Expect.equal actual.SubmissionDate (Some submissionDate) "SubmissionDate"
            Expect.equal actual.PublicReleaseDate (Some publicReleaseDate) "PublicReleaseDate"
            Expect.equal actual.Publications publications "Publications"
            Expect.equal actual.Contacts contacts "Contacts"
            Expect.equal actual.StudyDesignDescriptors studyDesignDescriptors "StudyDesignDescriptors"
            Expect.equal actual.Tables tables "Tables"
            Expect.equal actual.Assays assay_identifiers "Assays"
            Expect.equal actual.Factors factors "Factors"
            Expect.equal actual.Comments comments "Comments"

        testCase "create" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Study Title"
            let description = "Study Description"
            let submissionDate = "2023-07-19"
            let publicReleaseDate = "2023-12-31"
            let publications = [|Publication.create("Publication 1")|]
            let contacts = [|Person.create(FirstName = "John", LastName = "Doe")|]
            let studyDesignDescriptors = [|OntologyAnnotation.fromString("Design Descriptor")|]
            let tables = ResizeArray([|ArcTable.init("Table 1")|])
            let assays = createExampleAssays()
            let assay_identifiers = getAssayIdentifiers assays
            let factors = [|Factor.create("Factor 1")|]
            let comments = [|Comment.create("Comment 1")|]

            let actual = ArcStudy.create(
                identifier = identifier,
                title = title,
                description = description,
                submissionDate = submissionDate,
                publicReleaseDate = publicReleaseDate,
                publications = publications,
                contacts = contacts,
                studyDesignDescriptors = studyDesignDescriptors,
                tables = tables,
                assays = assay_identifiers,
                factors = factors,
                comments = comments
            )

            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title (Some title) "Title"
            Expect.equal actual.Description (Some description) "Description"
            Expect.equal actual.SubmissionDate (Some submissionDate) "SubmissionDate"
            Expect.equal actual.PublicReleaseDate (Some publicReleaseDate) "PublicReleaseDate"
            Expect.equal actual.Publications publications "Publications"
            Expect.equal actual.Contacts contacts "Contacts"
            Expect.equal actual.StudyDesignDescriptors studyDesignDescriptors "StudyDesignDescriptors"
            Expect.equal actual.Tables tables "Tables"
            Expect.equal actual.Assays assay_identifiers "Assays"
            Expect.equal actual.Factors factors "Factors"
            Expect.equal actual.Comments comments "Comments"

        testCase "init" <| fun _ ->
            let identifier = "MyIdentifier"
            let actual = ArcStudy.init(identifier)
            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title None "Title"
            Expect.equal actual.Description None "Description"
            Expect.equal actual.SubmissionDate None "SubmissionDate"
            Expect.equal actual.PublicReleaseDate None "PublicReleaseDate"
            Expect.isEmpty actual.Publications "Publications"
            Expect.isEmpty actual.Contacts "Contacts"
            Expect.isEmpty actual.StudyDesignDescriptors "StudyDesignDescriptors"
            Expect.isEmpty actual.Tables "Tables"
            Expect.isEmpty actual.Assays "Assays"
            Expect.isEmpty actual.Factors "Factors"
            Expect.isEmpty actual.Comments "Comments"
        testCase "make" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = Some "Study Title"
            let description = Some "Study Description"
            let submissionDate = Some "2023-07-19"
            let publicReleaseDate = Some "2023-12-31"
            let publications = [|Publication.create("Publication 1")|]
            let contacts = [|Person.create(FirstName = "John", LastName = "Doe")|]
            let studyDesignDescriptors = [|OntologyAnnotation.fromString("Design Descriptor")|]
            let tables = ResizeArray([|ArcTable.init("Table 1")|])
            let assays = createExampleAssays()
            let assay_identifiers = getAssayIdentifiers assays
            let factors = [|Factor.create("Factor 1")|]
            let comments = [|Comment.create("Comment 1")|]

            let actual = 
                ArcStudy.make
                    identifier
                    title
                    description
                    submissionDate
                    publicReleaseDate
                    publications
                    contacts
                    studyDesignDescriptors
                    tables
                    assay_identifiers
                    factors
                    comments

            Expect.equal actual.Identifier identifier "Identifier"
            Expect.equal actual.Title title "Title"
            Expect.equal actual.Description description "Description"
            Expect.equal actual.SubmissionDate submissionDate "SubmissionDate"
            Expect.equal actual.PublicReleaseDate publicReleaseDate "PublicReleaseDate"
            Expect.equal actual.Publications publications "Publications"
            Expect.equal actual.Contacts contacts "Contacts"
            Expect.equal actual.StudyDesignDescriptors studyDesignDescriptors "StudyDesignDescriptors"
            Expect.equal actual.Tables tables "Tables"
            Expect.equal actual.Assays assay_identifiers "Assays"
            Expect.equal actual.Factors factors "Factors"
            Expect.equal actual.Comments comments "Comments"
    ]

let tests_RegisteredAssays = testList "RegisteredAssays" [
    let _study_identifier = "My Study"
    let _study_description = Some "Lorem Ipsum"
    let _assay_identifier = "My Assay"
    let createTestStudy() =
        let s = ArcStudy(_study_identifier)
        s.Description <- _study_description
        s
    testCase "RegisterAssay, no parent" <| fun _ ->
        let study = createTestStudy()
        study.RegisterAssay(_assay_identifier)
        Expect.equal study.AssayCount 1 "registered assay count"
    testCase "GetRegisteredAssay, no parent" <| fun _ ->
        let study = createTestStudy()
        study.RegisterAssay(_assay_identifier)
        let eval() = study.GetRegisteredAssay(_assay_identifier) |> ignore
        Expect.throws eval "throws as single study has no parent, therefore no access to full assays."
    testCase "GetRegisteredAssays, no parent" <| fun _ ->
        let study = createTestStudy()
        study.RegisterAssay(_assay_identifier)
        let eval() = study.GetRegisteredAssays() |> ignore
        Expect.throws eval "throws as single study has no parent, therefore no access to full assays."
    testCase "DeregisterAssay, no parent" <| fun _ ->
        let study = createTestStudy()
        study.RegisterAssay(_assay_identifier)
        Expect.equal study.AssayCount 1 "registered assay count"
        study.DeregisterAssay(_assay_identifier)
        Expect.equal study.AssayCount 0 "registered assay count 2"
    testCase "InitAssay, no parent" <| fun _ ->
        let study = createTestStudy()
        let eval() = study.InitAssay(_assay_identifier) |> ignore
        Expect.throws eval "throws as single study has no parent, therefore no access to full assays."
    testCase "AddAssay, no parent" <| fun _ ->
        let study = createTestStudy()
        let eval() = study.AddAssay(ArcAssay(_assay_identifier)) |> ignore
        Expect.throws eval "throws as single study has no parent, therefore no access to full assays."
]

let tests_copy = 
    testList "copy" [
        let _study_identifier = "My Study"
        let _study_description = Some "Lorem Ipsum"
        let _assay_identifier = "My Assay"
        let createTestStudy() =
            let s = ArcStudy(_study_identifier)
            s.Description <- _study_description
            s.RegisterAssay(_assay_identifier)
            s
        testCase "ensure test study" <| fun _ -> 
            let study = createTestStudy()
            Expect.equal study.Identifier _study_identifier "_study_identifier"
            Expect.equal study.Description _study_description "_study_description"
            Expect.equal study.AssayCount 1 "AssayCount"
            let assayIdentifier = study.AssayIdentifiers.[0]
            Expect.equal assayIdentifier _assay_identifier "_assay_identifier"
        testCase "test mutable fields" <| fun _ -> 
            let newDesciption = Some "New Description"
            let newPublicReleaseDate = Some "01.01.2000"
            let study = createTestStudy()
            let copy = study.Copy()
            copy.Description <- newDesciption
            copy.PublicReleaseDate <- newPublicReleaseDate
            let checkSourceStudy =
                Expect.equal study.Identifier _study_identifier "_study_identifier"
                Expect.equal study.Description _study_description "_study_description"
                Expect.equal study.PublicReleaseDate None "PublicReleaseDate"
                Expect.equal study.AssayCount 1 "AssayCount"
                let assayIdentifier = study.AssayIdentifiers.[0]
                Expect.equal assayIdentifier _assay_identifier "_assay_identifier"
            let checkCopy =
                Expect.equal copy.Identifier _study_identifier "copy _study_identifier"
                Expect.equal copy.Description newDesciption "copy _study_description"
                Expect.equal copy.PublicReleaseDate newPublicReleaseDate "copy PublicReleaseDate"
                Expect.equal copy.AssayCount 1 "copy AssayCount"
                let assayIdentifier = study.AssayIdentifiers.[0]
                Expect.equal assayIdentifier _assay_identifier "copy _assay_identifier"
            ()
    ]


let main = 
    testList "ArcStudy" [
        tests_copy
        tests_RegisteredAssays
        test_create
    ]



