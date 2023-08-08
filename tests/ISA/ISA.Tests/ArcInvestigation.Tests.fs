﻿module ArcInvestigation.Tests

open ARCtrl.ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let private test_create =
    testList "create" [
        testCase "constructor" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Investigation Title"
            let description = "Investigation Description"
            let submissionDate = "2023-07-19"
            let publicReleaseDate = "2023-12-31"
            let ontologySourceReferences = [|OntologySourceReference.create("Reference 1")|]
            let publications = [|Publication.create("Publication 1")|]
            let contacts = [|Person.create(FirstName = "John", LastName = "Doe")|]
            let studies = ResizeArray([|ArcStudy.init("Study 1")|])
            let comments = [|Comment.create("Comment 1")|]
            let remarks = [|Remark.create(1, "Remark 1")|]

            let actual =
                ArcInvestigation(
                    identifier = identifier,
                    title = title,
                    description = description,
                    submissionDate = submissionDate,
                    publicReleaseDate = publicReleaseDate,
                    ontologySourceReferences = ontologySourceReferences,
                    publications = publications,
                    contacts = contacts,
                    studies = studies,
                    comments = comments,
                    remarks = remarks
                )

            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title (Some title) "Title"
            Expect.equal actual.Description (Some description) "Description"
            Expect.equal actual.SubmissionDate (Some submissionDate) "SubmissionDate"
            Expect.equal actual.PublicReleaseDate (Some publicReleaseDate) "PublicReleaseDate"
            Expect.equal actual.OntologySourceReferences ontologySourceReferences "OntologySourceReferences"
            Expect.equal actual.Publications publications "Publications"
            Expect.equal actual.Contacts contacts "Contacts"
            Expect.equal actual.Studies studies "Studies"
            Expect.equal actual.Comments comments "Comments"
            Expect.equal actual.Remarks remarks "Remarks"

        testCase "create" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Investigation Title"
            let description = "Investigation Description"
            let submissionDate = "2023-07-19"
            let publicReleaseDate = "2023-12-31"
            let ontologySourceReferences = [|OntologySourceReference.create("Reference 1")|]
            let publications = [|Publication.create("Publication 1")|]
            let contacts = [|Person.create(FirstName = "John", LastName = "Doe")|]
            let studies = ResizeArray([|ArcStudy.init("Study 1")|])
            let comments = [|Comment.create("Comment 1")|]
            let remarks = [|Remark.create(1, "Remark 1")|]

            let actual = ArcInvestigation.create(
                identifier = identifier,
                title = title,
                description = description,
                submissionDate = submissionDate,
                publicReleaseDate = publicReleaseDate,
                ontologySourceReferences = ontologySourceReferences,
                publications = publications,
                contacts = contacts,
                studies = studies,
                comments = comments,
                remarks = remarks
            )

            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title (Some title) "Title"
            Expect.equal actual.Description (Some description) "Description"
            Expect.equal actual.SubmissionDate (Some submissionDate) "SubmissionDate"
            Expect.equal actual.PublicReleaseDate (Some publicReleaseDate) "PublicReleaseDate"
            Expect.equal actual.OntologySourceReferences ontologySourceReferences "OntologySourceReferences"
            Expect.equal actual.Publications publications "Publications"
            Expect.equal actual.Contacts contacts "Contacts"
            Expect.equal actual.Studies studies "Studies"
            Expect.equal actual.Comments comments "Comments"
            Expect.equal actual.Remarks remarks "Remarks"

        testCase "init" <| fun _ ->
            let identifier = "MyIdentifier"

            let actual = ArcInvestigation.init(identifier)

            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title None "Title"
            Expect.equal actual.Description None "Description"
            Expect.equal actual.SubmissionDate None "SubmissionDate"
            Expect.equal actual.PublicReleaseDate None "PublicReleaseDate"
            Expect.isEmpty actual.OntologySourceReferences "OntologySourceReferences"
            Expect.isEmpty actual.Publications "Publications"
            Expect.isEmpty actual.Contacts "Contacts"
            Expect.isEmpty actual.Studies "Studies"
            Expect.isEmpty actual.Comments "Comments"
            Expect.isEmpty actual.Remarks "Remarks"

        testCase "make" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = Some "Investigation Title"
            let description = Some "Investigation Description"
            let submissionDate = Some "2023-07-19"
            let publicReleaseDate = Some "2023-12-31"
            let ontologySourceReferences = [|OntologySourceReference.create("Reference 1")|]
            let publications = [|Publication.create("Publication 1")|]
            let contacts = [|Person.create(FirstName = "John", LastName = "Doe")|]
            let studies = ResizeArray([|ArcStudy.init("Study 1")|])
            let comments = [|Comment.create("Comment 1")|]
            let remarks = [|Remark.create(1, "Remark 1")|]

            let actual = 
                ArcInvestigation.make
                    identifier
                    title
                    description
                    submissionDate
                    publicReleaseDate
                    ontologySourceReferences
                    publications
                    contacts
                    studies
                    comments
                    remarks

            Expect.equal actual.Identifier identifier "Identifier"
            Expect.equal actual.Title title "Title"
            Expect.equal actual.Description description "Description"
            Expect.equal actual.SubmissionDate submissionDate "SubmissionDate"
            Expect.equal actual.PublicReleaseDate publicReleaseDate "PublicReleaseDate"
            Expect.equal actual.OntologySourceReferences ontologySourceReferences "OntologySourceReferences"
            Expect.equal actual.Publications publications "Publications"
            Expect.equal actual.Contacts contacts "Contacts"
            Expect.equal actual.Studies studies "Studies"
            Expect.equal actual.Comments comments "Comments"
            Expect.equal actual.Remarks remarks "Remarks"
    ]

let tests_MutableFields = testList "MutableFields" [
    testCase "ensure investigation" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        Expect.equal i.Description None ""
    testCase "test mutable fields" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let persons = [|Person.create(FirstName="Kevin", LastName="Frey")|]
        i.Description <- Some "MyName"
        i.Contacts <- persons
        i.Title <- Some "Awesome Title"
        Expect.equal i.Description (Some "MyName") "FileName"
        Expect.equal i.Contacts persons "Contacts"
        Expect.equal i.Title (Some "Awesome Title") "Title"
]

let tests_Copy = testList "Copy" [
    testCase "test mutable fields" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let persons = [|Person.create(FirstName="Kevin", LastName="Frey")|]
        i.Description <- Some "MyName"
        i.Contacts <- persons
        i.Title <- Some "Awesome Title"
        Expect.equal i.Description (Some "MyName") "FileName"
        Expect.equal i.Contacts persons "Contacts"
        Expect.equal i.Title (Some "Awesome Title") "Title"
        let copy = i.Copy()
        let nextPersons = [|Person.create(FirstName="Pascal", LastName="Gevangen")|]
        copy.Description <- Some "Next FileName"
        copy.Contacts <- nextPersons
        copy.Title <- Some "Next Title"
        Expect.equal i.Description (Some "MyName") "FileName, after copy"
        Expect.equal i.Contacts persons "Contacts, after copy"
        Expect.equal i.Title (Some "Awesome Title") "Title, after copy"
        Expect.equal copy.Description (Some "Next FileName") "copy FileName"
        Expect.equal copy.Contacts nextPersons "copy Contacts"
        Expect.equal copy.Title (Some "Next Title") "copy Title"
    testCase "test mutable fields on study" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let persons = [|Person.create(FirstName="Kevin", LastName="Frey")|]
        i.Description <- Some "MyName"
        i.Contacts <- persons
        i.Title <- Some "Awesome Title"
        i.AddStudy(ArcStudy.init("Study 1"))
        let s = i.GetStudyAt(0)
        s.Description <- Some "My Test Desciption"
        Expect.equal i.Description (Some "MyName") "FileName"
        Expect.equal i.Contacts persons "Contacts"
        Expect.equal i.Title (Some "Awesome Title") "Title"
        Expect.equal i.StudyCount 1 "StudyCount"
        let sNext = i.GetStudyAt(0)
        Expect.equal sNext.Description (Some "My Test Desciption") "study description"
        // Create `copy` and change params. Then `i` should still be the same while `copy` must be changed
        let copy = i.Copy()
        let nextPersons = [|Person.create(FirstName="Pascal", LastName="Gevangen")|]
        copy.Description <- Some "Next FileName"
        copy.Contacts <- nextPersons
        copy.Title <- Some "Next Title"
        let copyStudy = copy.GetStudyAt(0)
        copyStudy.Description <- Some "My New Desciption"
        Expect.equal i.Description (Some "MyName") "FileName, after copy"
        Expect.equal i.Contacts persons "Contacts, after copy"
        Expect.equal i.Title (Some "Awesome Title") "Title, after copy"
        Expect.equal i.StudyCount 1 "StudyCount, after copy"
        let s_postCopy = i.GetStudyAt(0)
        Expect.equal s_postCopy.Description (Some "My Test Desciption") "study description, after copy"
        Expect.equal copy.Description (Some "Next FileName") "copy FileName"
        Expect.equal copy.Contacts nextPersons "copy Contacts"
        Expect.equal copy.Title (Some "Next Title") "copy Title"
        Expect.equal copy.StudyCount 1 "copy StudyCount"
        let nextcopyStudy = copy.GetStudyAt(0)
        Expect.equal nextcopyStudy.Description (Some "My New Desciption") "copy study desciption"
]

let tests_Study = testList "CRUD Study" [
    let createExampleInvestigation() =
        let i = ArcInvestigation.init("MyInvestigation")
        let s = ArcStudy.init("Study 1")
        let s2 = ArcStudy("Study 2",title="Study 2 Title")
        i.AddStudy s
        i.AddStudy s2
        i
    testList "AddStudy" [
        testCase "add multiple" <| fun _ ->
            let i = createExampleInvestigation()
            Expect.equal i.StudyCount 2 "StudyCount"
            TestingUtils.mySequenceEqual i.StudyIdentifiers (seq {"Study 1"; "Study 2"}) "StudyIdentifiers"
            let s2 = i.Studies.Item 1 
            Expect.equal s2.Title (Some "Study 2 Title") "Study 2 Title"
        testCase "add with same name, throws" <| fun _ ->
            let i = createExampleInvestigation()
            let eval() = i.AddStudy(ArcStudy.create("Study 1"))
            Expect.throws eval "Identifier already exists"
    ]
    testList "GetStudy" [
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let s = i.GetStudy("Study 1")
            Expect.equal s.Identifier "Study 1" "get success"
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let s = i.GetStudyAt(1)
            Expect.equal s.Identifier "Study 2" "get success"
        testCase "mutable propagation" <| fun _ ->
            let i = createExampleInvestigation()
            let s = i.GetStudyAt(1)
            Expect.equal s.Identifier "Study 2" "identifier"
            Expect.equal s.Title (Some "Study 2 Title") "identifier"
            s.Title <- None
            let s_post = i.GetStudyAt(1)
            Expect.equal s_post.Title None "identifier, s_post"
            Expect.equal s.Title None "identifier, s"
            Expect.equal (i.Studies.[1].Title) None "identifier, direct"
    ]
    testList "RemoveStudy" [
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let s = i.RemoveStudy("Study 1")
            Expect.equal i.StudyCount 1 "StudyCount"
            Expect.equal (i.Studies.[0].Identifier) "Study 2" "Correct study removed"
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            i.RemoveStudyAt(1)
            Expect.equal i.StudyCount 1 "StudyCount"
            Expect.equal (i.Studies.[0].Identifier) "Study 1" "Correct study removed"
    ]
    testList "SetStudy" [
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let study_identifier = "New Awesome Study"
            let study_title = "New Study"
            let expected = ArcStudy.create(study_identifier, title = study_title)
            i.SetStudy("Study 1", expected)
            Expect.equal i.StudyCount 2 "StudyCount"
            Expect.equal (i.Studies.[0].Identifier) study_identifier "Correct study set, ident"
            Expect.equal (i.Studies.[0].Title) (Some study_title) "Correct study set, title"
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let study_identifier = "New Awesome Study"
            let study_title = "New Study"
            let expected = ArcStudy.create(study_identifier, title = study_title)
            i.SetStudyAt(1, expected)
            Expect.equal i.StudyCount 2 "StudyCount"
            Expect.equal (i.Studies.[1].Identifier) study_identifier "Correct study set, ident"
            Expect.equal (i.Studies.[1].Title) (Some study_title) "Correct study set, title"
    ]
]

let tests_Assay = testList "CRUD Assay" [
    let createExampleInvestigation() =
        let i = ArcInvestigation.init("MyInvestigation")
        let s = ArcStudy.init("Study 1")
        let a = ArcAssay.init("Assay 1")
        let a2 = ArcAssay.init("Assay 2")
        s.AddAssay(a)
        s.AddAssay(a2)
        let s2 = ArcStudy.create("Study 2",title="Study 2 Title")
        i.AddStudy s
        i.AddStudy s2
        i
    testCase "ensure example" <| fun _ ->
        let i = createExampleInvestigation()
        Expect.equal i.StudyCount 2 "StudyCount"
        TestingUtils.mySequenceEqual i.StudyIdentifiers (seq {"Study 1"; "Study 2"}) "StudyCount"
        let s = i.Studies.Item 0 
        Expect.equal s.AssayCount 2 "AssayCount"
        TestingUtils.mySequenceEqual s.AssayIdentifiers (seq {"Assay 1"; "Assay 2"}) "AssayIdentifiers"
        let s2 = i.Studies.Item 1 
        Expect.equal s2.Title (Some "Study 2 Title") "Study 2 Title"
    testList "AddAssay" [ 
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let assay_ident = "New Assay"
            let assay_techPlatform = "Assay Tech"
            let expected = ArcAssay(assay_ident, technologyPlatform = assay_techPlatform)
            i.AddAssayAt(0, expected)
            Expect.equal i.StudyCount 2 "StudyCount"
            let s = i.Studies.Item 0 
            Expect.equal s.AssayCount 3 "AssayCount"
            let actual = s.GetAssayAt 2
            Expect.equal actual expected "equal"
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let assay_ident = "New Assay"
            let assay_techPlatform = "Assay Tech"
            let expected = ArcAssay(assay_ident, technologyPlatform = assay_techPlatform)
            i.AddAssay("Study 1", expected)
            Expect.equal i.StudyCount 2 "StudyCount"
            let s = i.Studies.Item 0 
            Expect.equal s.AssayCount 3 "AssayCount"
            let actual = s.GetAssayAt 2
            Expect.equal actual expected "equal"
    ]
    testList "SetAssay" [ 
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let assay_ident = "New Assay"
            let assay_techPlatform = "Assay Tech"
            let expected = ArcAssay(assay_ident, technologyPlatform = assay_techPlatform)
            i.SetAssayAt("Study 1", 0, expected)
            Expect.equal i.StudyCount 2 "StudyCount"
            let s = i.Studies.Item 0 
            Expect.equal s.AssayCount 2 "AssayCount"
            let actual = s.GetAssayAt 0
            Expect.equal actual expected "equal"
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let assay_ident = "New Assay"
            let assay_techPlatform = "Assay Tech"
            let expected = ArcAssay(assay_ident, technologyPlatform = assay_techPlatform)
            i.SetAssay("Study 1", "Assay 2", expected)
            Expect.equal i.StudyCount 2 "StudyCount"
            let s = i.Studies.Item 0 
            Expect.equal s.AssayCount 2 "AssayCount"
            let actual = s.GetAssayAt 1
            Expect.equal actual expected "equal"
    ]
    testList "GetAssay" [ 
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let a = i.GetAssayAt("Study 1", 0)
            Expect.equal a.Identifier "Assay 1" "FileName"
        testCase "by ident" <| fun _ ->
            let i = createExampleInvestigation()
            let a = i.GetAssay("Study 1", "Assay 2")
            Expect.equal a.Identifier "Assay 2" "FileName"
        testCase "mutable propagation" <| fun _ ->
            let i = createExampleInvestigation()
            let tech = Some "New Tech Stuff"
            let a = i.GetAssayAt("Study 1", 0)
            Expect.equal a.Identifier "Assay 1" "FileName"
            Expect.equal a.TechnologyPlatform None "TechnologyPlatform"
            a.TechnologyPlatform <- tech
            Expect.equal a.TechnologyPlatform tech "TechnologyPlatform, a"
            Expect.equal i.Studies.[0].Assays.[0].TechnologyPlatform tech "TechnologyPlatform, direct"
        testCase "mutable propagation, copy" <| fun _ ->
            let i = createExampleInvestigation()
            let copy = createExampleInvestigation()
            let tech = Some "New Tech Stuff"
            let a = i.GetAssayAt("Study 1", 0)
            Expect.equal a.Identifier "Assay 1" "FileName"
            Expect.equal a.TechnologyPlatform None "TechnologyPlatform"
            a.TechnologyPlatform <- tech
            Expect.equal a.TechnologyPlatform tech "TechnologyPlatform, a"
            Expect.equal i.Studies.[0].Assays.[0].TechnologyPlatform tech "TechnologyPlatform, direct"
            Expect.equal copy.Studies.[0].Assays.[0].TechnologyPlatform None "TechnologyPlatform, copy direct"
    ]
    testList "FindAssay" [
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let a = i.FindAssay("Assay 1")
            Expect.equal a.Identifier "Assay 1" "Identifier"
        testCase "mutability" <| fun _ ->
            let i = createExampleInvestigation()
            let a = i.FindAssay("Assay 1")
            let t = a.InitTable("NewTable")
            t.AddColumn(CompositeHeader.Input IOType.Sample,[|for i in 1 .. 10 do yield CompositeCell.createFreeText $"My awesome probe {i}"|])
            Expect.equal t.Name "NewTable" "table name"
            Expect.equal t.ColumnCount 1 "column count"
            Expect.equal t.RowCount 10 "row count"
            let aNew = i.FindAssay("Assay 1")
            Expect.equal aNew.TableCount 1 "table count"
            Expect.equal aNew.Tables.[0].Name "NewTable" "next table name"
            Expect.equal aNew.Tables.[0].ColumnCount 1 "next column count"
            Expect.equal aNew.Tables.[0].RowCount 10 "next row count"
    ]
]


let main = 
    testList "ArcInvestigation" [
        test_create
        tests_MutableFields
        tests_Copy
        tests_Study
        tests_Assay
    ]