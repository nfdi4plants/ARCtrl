module ArcInvestigation.Tests

open ARCtrl.ISA

open TestingUtils

let private assay_Identifier = "MyAssay"
let private assay_MeasurementType = OntologyAnnotation.fromString("My Measurement Type", "MST", "MST:42424242")
let private create_ExampleAssay() = ArcAssay.create(assay_Identifier,assay_MeasurementType)
let private create_ExampleAssays() = ResizeArray([create_ExampleAssay()])

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
            let assays = create_ExampleAssays()
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
                    assays = assays,
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
            Expect.equal actual.Assays assays "Assays"
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
            let assays = create_ExampleAssays()
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
                assays = assays,
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
            Expect.equal actual.Assays assays "Assays"
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
            Expect.isEmpty actual.Assays "Assays"
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
            let assays = create_ExampleAssays()
            let studies = ResizeArray([|ArcStudy.init("Study 1")|])
            let registeredStudyIdentifiers = ResizeArray(["Study 1"])
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
                    assays
                    studies
                    registeredStudyIdentifiers
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
            Expect.equal actual.Assays assays "Assays"
            Expect.equal actual.Studies studies "Studies"
            Expect.equal actual.Comments comments "Comments"
            Expect.equal actual.Remarks remarks "Remarks"
        testCase "constructorAppliesReference" <| fun _ ->
            let assayName = "MyAssay"
            let studyName = "MyStudy"
            let tableName = "MyTable"
            let investigationName = "MyInvestigation"

            let myAssay = ArcAssay.init(assayName)
            myAssay.InitTable(tableName) |> ignore
            let myStudy = ArcStudy.init(studyName)
            myStudy.RegisterAssay(assayName)

            let assays = ResizeArray([myAssay])
            let studies = ResizeArray([|myStudy|])
            let registeredStudyIdentifiers = ResizeArray([studyName])

            let myInvestigation = 
                ArcInvestigation(
                    investigationName,
                    assays = assays,
                    studies = studies,
                    registeredStudyIdentifiers = registeredStudyIdentifiers
                )

            let result = myStudy.GetRegisteredAssay(assayName)

            Expect.equal result myAssay "Retrieved assay should be equal"

            result.InitTable("MySecondTable") |> ignore

            Expect.equal result.TableCount 2 "Table count should be 2"
            Expect.equal myAssay.TableCount 2 "Table count should also be 2"
    ]

let test_RegisteredAssays = testList "RegisteredAssays" [
    testCase "Investigation.RegisterAssay" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        i.RegisterAssay(s.Identifier, a.Identifier)
        Expect.equal s.RegisteredAssayCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
    testCase "Study.RegisterAssay" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        s.RegisterAssay(a.Identifier)
        Expect.equal s.RegisteredAssayCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
    testCase "Investigation.DeregisterAssay" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        i.RegisterAssay(s.Identifier, a.Identifier)
        Expect.equal s.RegisteredAssayCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
        i.DeregisterAssay(s.Identifier,a.Identifier)
        Expect.equal s.RegisteredAssayCount 0 "registered assay count 2"
    testCase "Study.DeregisterAssay" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        s.RegisterAssay(a.Identifier)
        Expect.equal s.RegisteredAssayCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
        s.DeregisterAssay(a.Identifier)
        Expect.equal s.RegisteredAssayCount 0 "registered assay count 2"
    testCase "Remove registered assay from investigation" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        s.RegisterAssay(a.Identifier)
        Expect.equal s.RegisteredAssayCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
        i.RemoveAssayAt 0 
        Expect.equal i.AssayCount 0 "assay count 2"
        Expect.equal s.RegisteredAssayCount 0 "registered assay count 2"
    testCase "Investigation.DeregisterMissingAssays" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        s.RegisterAssay(a.Identifier)
        Expect.equal s.RegisteredAssayCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
        i.Assays <- (ResizeArray())
        Expect.equal i.AssayCount 0 "assay count 2"
        Expect.equal s.RegisteredAssayCount 1 "registered assay count 2"
        i.DeregisterMissingAssays()
        Expect.equal s.RegisteredAssayCount 0 "registered assay count 3"
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
    testCase "test mutable fields on registered assay" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let assay = create_ExampleAssay()
        let study_identifier = "MyStudy"
        i.AddAssay(assay)
        let study = i.InitStudy(study_identifier)
        study.RegisterAssay(assay.Identifier)
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        Expect.equal study.RegisteredAssayCount 1 "registered assay count"
        Expect.equal assay.TableCount 0 "assay table count"
        Expect.equal study.RegisteredAssayIdentifiers.[0] assay.Identifier "registered assay identifier"
        let registeredAssay = study.RegisteredAssays.[0]
        Expect.equal registeredAssay.Identifier assay.Identifier "full registered assay identifier"
        let table = registeredAssay.InitTable("My New Table")
        Expect.equal assay.TableCount 1 "table count to init table"
        Expect.equal i.Assays.[0].TableCount 1 "table count from investigation"
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
            TestingUtils.Expect.sequenceEqual i.StudyIdentifiers (seq {"Study 1"; "Study 2"}) "StudyIdentifiers"
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
        let s2 = ArcStudy.create("Study 2",title="Study 2 Title")
        let a = ArcAssay.init("Assay 1")
        let a2 = ArcAssay.init("Assay 2")
        i.AddStudy s
        i.AddStudy s2
        i.AddAssay(a)
        i.AddAssay(a2)
        i.RegisterAssay(s.Identifier, a.Identifier)
        i.RegisterAssay(s.Identifier, a2.Identifier)
        i
    testCase "ensure example" <| fun _ ->
        let i = createExampleInvestigation()
        Expect.equal i.StudyCount 2 "StudyCount"
        Expect.equal i.AssayCount 2 "AssayCount"
        TestingUtils.Expect.sequenceEqual i.StudyIdentifiers (seq {"Study 1"; "Study 2"}) "StudyIdentifiers"
        TestingUtils.Expect.sequenceEqual i.AssayIdentifiers (seq {"Assay 1"; "Assay 2"}) "AssayIdentifiers"
        let s = i.Studies.Item 0 
        Expect.equal s.RegisteredAssayCount 2 "Registered AssayCount"
        let s2 = i.Studies.Item 1 
        Expect.equal s2.Title (Some "Study 2 Title") "Study 2 Title"
    testList "AddAssay" [ 
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let assay_ident = "New Assay"
            let assay_techPlatform = OntologyAnnotation.fromString("Assay Tech")
            let expected = ArcAssay(assay_ident, technologyPlatform = assay_techPlatform)
            i.AddAssay(expected)
            i.RegisterAssayAt(0, expected.Identifier)
            Expect.equal i.StudyCount 2 "StudyCount"
            let s = i.Studies.Item 0 
            Expect.equal s.RegisteredAssayCount 3 "AssayCount"
            let actual = i.GetAssayAt 2
            Expect.equal actual expected "equal"
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let assay_ident = "New Assay"
            let assay_techPlatform = OntologyAnnotation.fromString("Assay Tech")
            let expected = ArcAssay(assay_ident, technologyPlatform = assay_techPlatform)
            i.AddAssay(expected)
            i.RegisterAssay("Study 1", expected.Identifier)
            Expect.equal i.StudyCount 2 "StudyCount"
            let s = i.Studies.Item 0 
            Expect.equal s.RegisteredAssayCount 3 "AssayCount"
            let actual = i.GetAssayAt 2
            Expect.equal actual expected "equal"
    ]
    testList "SetAssay" [ 
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let assay_ident = "New Assay"
            let assay_techPlatform = OntologyAnnotation.fromString("Assay Tech")
            let expected = ArcAssay(assay_ident, technologyPlatform = assay_techPlatform)
            i.SetAssayAt(0, expected)
            Expect.equal i.StudyCount 2 "StudyCount"
            Expect.equal i.AssayCount 2 "AssayCount"
            Expect.equal i.Assays.[0] expected "equal"
            let s = i.Studies.Item 0 
            Expect.equal s.RegisteredAssayCount 1 "Registered AssayCount. should be 1 less due to different identifier"
        testCase "by index tpOntology" <| fun _ ->
            let i = createExampleInvestigation()
            let assay_ident = "New Assay"
            let assay_techPlatform = OntologyAnnotation.fromString("Assay Tech","ABC","ABC:123")
            let expected = ArcAssay(assay_ident, technologyPlatform = assay_techPlatform)
            i.SetAssayAt(0, expected)
            Expect.equal i.StudyCount 2 "StudyCount"
            Expect.equal i.AssayCount 2 "AssayCount"
            let s = i.Studies.Item 0 
            Expect.equal s.RegisteredAssayCount 1 "Registered AssayCount. should be 1 less due to different identifier"
            Expect.equal i.Assays.[0] expected "equal"
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let assay_ident = "New Assay"
            let assay_techPlatform = OntologyAnnotation.fromString("Assay Tech")
            let expected = ArcAssay(assay_ident, technologyPlatform = assay_techPlatform)
            i.SetAssay("Assay 2", expected)
            Expect.equal i.StudyCount 2 "StudyCount"
            Expect.equal i.AssayCount 2 "AssayCount"
            let s = i.Studies.Item 0 
            Expect.equal s.RegisteredAssayCount 1 "Registered AssayCount. should be 1 less due to different identifier"
            let actual = i.Assays.[1]
            Expect.equal actual expected "equal"
    ]
    testList "GetAssay" [ 
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let a = i.GetAssayAt(0)
            Expect.equal a.Identifier "Assay 1" "FileName"
        testCase "by ident" <| fun _ ->
            let i = createExampleInvestigation()
            let a = i.GetAssay("Assay 2")
            Expect.equal a.Identifier "Assay 2" "FileName"
        testCase "mutable propagation" <| fun _ ->
            let i = createExampleInvestigation()
            let tech = Some (OntologyAnnotation.fromString("New Tech Stuff"))
            let a = i.GetAssayAt(0)
            Expect.equal a.Identifier "Assay 1" "FileName"
            Expect.equal a.TechnologyPlatform None "TechnologyPlatform"
            a.TechnologyPlatform <- tech
            Expect.equal a.TechnologyPlatform tech "TechnologyPlatform, a"
            Expect.equal i.Assays.[0].TechnologyPlatform tech "TechnologyPlatform, direct"
        testCase "mutable propagation, copy" <| fun _ ->
            let i = createExampleInvestigation()
            let copy = createExampleInvestigation()
            let tech = Some (OntologyAnnotation.fromString("New Tech Stuff"))
            let a = i.GetAssayAt(0)
            Expect.equal a.Identifier "Assay 1" "FileName"
            Expect.equal a.TechnologyPlatform None "TechnologyPlatform"
            a.TechnologyPlatform <- tech
            Expect.equal a.TechnologyPlatform tech "TechnologyPlatform, a"
            Expect.equal i.Assays.[0].TechnologyPlatform tech "TechnologyPlatform, direct"
            Expect.equal copy.Assays.[0].TechnologyPlatform None "TechnologyPlatform, copy direct"
    ]
]

let private tests_GetHashCode = testList "GetHashCode" [
    testCase "passing" <| fun _ ->
        let actual = ArcInvestigation.init("Test")
        Expect.isSome (actual.GetHashCode() |> Some) ""
    testCase "equal minimal" <| fun _ -> 
        let actual = ArcInvestigation.init("Test")
        let copy = actual.Copy()
        let actual2 = ArcInvestigation.init("Test")
        Expect.equal actual copy "equal"
        Expect.equal (actual.GetHashCode()) (copy.GetHashCode()) "copy hash equal"
        Expect.equal (actual.GetHashCode()) (actual2.GetHashCode()) "assay2 hash equal"
    testCase "equal" <| fun _ ->
        let actual = 
            ArcInvestigation.make 
                "Inv"
                (Some "My Inv Title")
                (Some "My Inv Description")
                (Some "My Inv SubmissionDate")
                (Some "My Inv PRD")
                [|OntologySourceReference.create("Some Lorem ipsum description", Name="Descriptore"); OntologySourceReference.empty|]
                [|Publication.empty; Publication.create(Title="Some nice title")|]
                ([|Person.create(FirstName="John",LastName="Doe"); Person.create(FirstName="Jane",LastName="Doe")|])
                (ResizeArray([ArcAssay.init("Registered Assay1"); ArcAssay.init("Registered Assay2")]))
                (ResizeArray([ArcStudy.init("Registered Study1"); ArcStudy.init("Registered Study2")]))
                (ResizeArray(["Registered Study1"; "Registered Study2"]))
                ([|Comment.create("Hello", "World"); Comment.create("ByeBye", "World") |])
                [|Remark.create(12,"Test"); Remark.create(42, "The answer")|]
        let copy = actual.Copy()
        Expect.equal (actual.GetHashCode()) (copy.GetHashCode()) ""
    testCase "not equal" <| fun _ ->
        let x1 = ArcInvestigation.init "Test"
        let x2 = ArcInvestigation.init "Other Test"
        Expect.notEqual x1 x2 "not equal"
        Expect.notEqual (x1.GetHashCode()) (x2.GetHashCode()) "not equal hash"
]

//let tests_UpdateBy = testList "UpdateBy" [
//    let create_testInvestigation(assay) =
//        ArcInvestigation.create(
//            "MyInvestigation",
//            "MyTitle",
//            "MyDescription",
//            "MySubmissionDate",
//            "MyPublicReleaseDate",
//            [|OntologySourceReference.create(Name="MyOntologySourceReference")|],
//            [|Publication.create(Title="MyPublication")|],
//            [|Person.create(FirstName="Kevin", LastName="Frey")|],
//            ResizeArray([assay]),
//            ResizeArray([ArcStudy.init("MyStudy")]),
//            ResizeArray(["MyStudy"]),
//            [|Comment.create(Name="MyComment")|],
//            [|Remark.create(1,"MyRemark")|]
//        )
//    let create_testInvestigationNextEmpty() =
//        ArcInvestigation.init("NextEmptyInvestigation")
//    let create_testInvestigationNext(assay) =
//        ArcInvestigation.create(
//            "NextInvestigation",
//            "NextTitle",
//            "NextDescription",
//            "NextSubmissionDate",
//            "NextPublicReleaseDate",
//            [|OntologySourceReference.create(Name="NextOntologySourceReference")|],
//            [|Publication.create(Title="NextPublication")|],
//            [|Person.create(FirstName="Kevin", LastName="Frey")|],
//            ResizeArray([assay]),
//            ResizeArray([ArcStudy.init("NextStudy")]),
//            ResizeArray(["NextStudy"]),
//            [|Comment.create(Name="NextComment")|],
//            [|Remark.create(12, "NextRemark")|]
//        )
//    testCase "UpdateBy, full replace" <| fun _ ->
//        let myassay = ArcAssay.init("MyAssays")
//        let nextassay = ArcAssay.init("nextassay")
//        let actual = create_testInvestigation(myassay)
//        let next = create_testInvestigationNext(nextassay)
//        actual.UpdateBy(next)
//        Expect.notEqual actual.Identifier next.Identifier "Identifier"
//        Expect.equal actual.Title next.Title "Title"
//        Expect.equal actual.Description next.Description "Description"
//        Expect.equal actual.SubmissionDate next.SubmissionDate "SubmissionDate"
//        Expect.equal actual.PublicReleaseDate next.PublicReleaseDate "PublicReleaseDate"
//        Expect.equal actual.OntologySourceReferences next.OntologySourceReferences "OntologySourceReferences"
//        Expect.equal actual.Publications next.Publications "Publications"
//        Expect.equal actual.Contacts next.Contacts "Contacts"
//        Expect.equal actual.Assays next.Assays "Assays"
//        Expect.equal actual.Studies next.Studies "Studies"
//        Expect.equal actual.RegisteredStudyIdentifiers next.RegisteredStudyIdentifiers "RegisteredStudyIdentifiers"
//        Expect.equal actual.Comments next.Comments "Comments"
//        Expect.equal actual.Remarks next.Remarks "Remarks"
//    testCase "UpdateBy, full replace empty" <| fun _ ->
//        let myassay = ArcAssay.init("MyAssays")
//        let actual = create_testInvestigation(myassay)
//        let next = create_testInvestigationNextEmpty()
//        actual.UpdateBy(next)
//        Expect.notEqual actual.Identifier next.Identifier "Identifier"
//        Expect.equal actual.Title next.Title "Title"
//        Expect.equal actual.Description next.Description "Description"
//        Expect.equal actual.SubmissionDate next.SubmissionDate "SubmissionDate"
//        Expect.equal actual.PublicReleaseDate next.PublicReleaseDate "PublicReleaseDate"
//        Expect.equal actual.OntologySourceReferences next.OntologySourceReferences "OntologySourceReferences"
//        Expect.equal actual.Publications next.Publications "Publications"
//        Expect.equal actual.Contacts next.Contacts "Contacts"
//        Expect.equal actual.Assays next.Assays "Assays"
//        Expect.equal actual.Studies next.Studies "Studies"
//        Expect.equal actual.RegisteredStudyIdentifiers next.RegisteredStudyIdentifiers "RegisteredStudyIdentifiers"
//        Expect.equal actual.Comments next.Comments "Comments"
//        Expect.equal actual.Remarks next.Remarks "Remarks"
//    testCase "UpdateBy, replace existing" <| fun _ ->
//        let myassay = ArcAssay.init("MyAssays")
//        let nextassay = ArcAssay.init("nextassay")
//        let actual = create_testInvestigation(myassay)
//        let next = create_testInvestigationNext(nextassay)
//        actual.UpdateBy(next, true)
//        Expect.notEqual actual.Identifier next.Identifier "Identifier"
//        Expect.equal actual.Title next.Title "Title"
//        Expect.equal actual.Description next.Description "Description"
//        Expect.equal actual.SubmissionDate next.SubmissionDate "SubmissionDate"
//        Expect.equal actual.PublicReleaseDate next.PublicReleaseDate "PublicReleaseDate"
//        Expect.equal actual.OntologySourceReferences next.OntologySourceReferences "OntologySourceReferences"
//        Expect.equal actual.Publications next.Publications "Publications"
//        Expect.equal actual.Contacts next.Contacts "Contacts"
//        Expect.equal actual.Assays next.Assays "Assays"
//        Expect.equal actual.Studies next.Studies "Studies"
//        Expect.equal actual.RegisteredStudyIdentifiers next.RegisteredStudyIdentifiers "RegisteredStudyIdentifiers"
//        Expect.equal actual.Comments next.Comments "Comments"
//        Expect.equal actual.Remarks next.Remarks "Remarks"
//    testCase "UpdateBy, replace existing empty" <| fun _ ->
//        let myassay = ArcAssay.init("MyAssays")
//        let actual = create_testInvestigation(myassay)
//        let next = create_testInvestigationNextEmpty()
//        let expected = create_testInvestigation(myassay)
//        actual.UpdateBy(next, true)
//        Expect.notEqual actual.Identifier next.Identifier "Identifier"
//        Expect.equal actual.Title expected.Title "Title"
//        Expect.equal actual.Description expected.Description "Description"
//        Expect.equal actual.SubmissionDate expected.SubmissionDate "SubmissionDate"
//        Expect.equal actual.PublicReleaseDate expected.PublicReleaseDate "PublicReleaseDate"
//        Expect.equal actual.OntologySourceReferences expected.OntologySourceReferences "OntologySourceReferences"
//        Expect.equal actual.Publications expected.Publications "Publications"
//        Expect.equal actual.Contacts expected.Contacts "Contacts"
//        Expect.equal actual.Assays.Count 1 "Count 1"
//        Expect.equal expected.Assays.Count 1 "Count 2"
//        TestingUtils.Expect.sequenceEqual actual.Assays expected.Assays "Assays"
//        TestingUtils.Expect.sequenceEqual actual.Studies expected.Studies "Studies"
//        Expect.equal actual.RegisteredStudyIdentifiers expected.RegisteredStudyIdentifiers "RegisteredStudyIdentifiers"
//        Expect.equal actual.Comments expected.Comments "Comments"
//        Expect.equal actual.Remarks expected.Remarks "Remarks"
//]

let main = 
    testList "ArcInvestigation" [
        test_create
        test_RegisteredAssays
        tests_MutableFields
        tests_Copy
        tests_Study
        tests_Assay
        tests_GetHashCode
        // tests_UpdateBy
    ]