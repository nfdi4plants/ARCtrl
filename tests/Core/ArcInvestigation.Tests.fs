module ArcInvestigation.Tests

open ARCtrl

open TestingUtils

let private assay_Identifier = "MyAssay"
let private assay_MeasurementType = OntologyAnnotation("My Measurement Type", "MST", "MST:42424242")
let private create_ExampleAssay() = ArcAssay.create(assay_Identifier,measurementType = assay_MeasurementType)
let private create_ExampleAssays() = ResizeArray([create_ExampleAssay()])

let private tests_create =
    testList "create" [
        testCase "constructor" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Investigation Title"
            let description = "Investigation Description"
            let submissionDate = "2023-07-19"
            let publicReleaseDate = "2023-12-31"
            let ontologySourceReferences = ResizeArray [|OntologySourceReference.create("Reference 1")|]
            let publications = ResizeArray [|Publication.create("Publication 1")|]
            let contacts = ResizeArray [|Person.create(firstName = "John", lastName = "Doe")|]
            let assays = create_ExampleAssays()
            let studies = ResizeArray([|ArcStudy.init("Study 1")|])
            let workflows = ResizeArray([|ArcWorkflow.init("Workflow 1")|])
            let runs = ResizeArray([ArcRun.init("Run 1")])
            let comments = ResizeArray [|Comment.create("Comment 1")|]
            let remarks = ResizeArray [|Remark.create(1, "Remark 1")|]

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
                    workflows = workflows,
                    runs = runs,
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
            Expect.equal actual.Workflows workflows "Workflows"
            Expect.equal actual.Runs runs "Runs"
            Expect.equal actual.Comments comments "Comments"
            Expect.equal actual.Remarks remarks "Remarks"

        testCase "create" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Investigation Title"
            let description = "Investigation Description"
            let submissionDate = "2023-07-19"
            let publicReleaseDate = "2023-12-31"
            let ontologySourceReferences = ResizeArray [|OntologySourceReference.create("Reference 1")|]
            let publications = ResizeArray [|Publication.create("Publication 1")|]
            let contacts = ResizeArray [|Person.create(firstName = "John", lastName = "Doe")|]
            let assays = create_ExampleAssays()
            let studies = ResizeArray [|ArcStudy.init("Study 1")|]
            let workflows = ResizeArray [|ArcWorkflow.init("Workflow 1")|]
            let runs = ResizeArray [|ArcRun.init("Run 1")|]
            let comments = ResizeArray [|Comment.create("Comment 1")|]
            let remarks = ResizeArray [|Remark.create(1, "Remark 1")|]

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
                workflows = workflows,
                runs = runs,
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
            Expect.equal actual.Workflows workflows "Workflows"
            Expect.equal actual.Runs runs "Runs"
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
            Expect.isEmpty actual.Workflows "Workflows"
            Expect.isEmpty actual.Runs "Runs"
            Expect.isEmpty actual.Comments "Comments"
            Expect.isEmpty actual.Remarks "Remarks"

        testCase "make" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = Some "Investigation Title"
            let description = Some "Investigation Description"
            let submissionDate = Some "2023-07-19"
            let publicReleaseDate = Some "2023-12-31"
            let ontologySourceReferences = ResizeArray [|OntologySourceReference.create("Reference 1")|]
            let publications = ResizeArray [|Publication.create("Publication 1")|]
            let contacts = ResizeArray [|Person.create(firstName = "John", lastName = "Doe")|]
            let assays = create_ExampleAssays()
            let studies = ResizeArray([|ArcStudy.init("Study 1")|])
            let workflows = ResizeArray([|ArcWorkflow.init("Workflow 1")|])
            let runs = ResizeArray([|ArcRun.init("Run 1")|])
            let registeredStudyIdentifiers = ResizeArray(["Study 1"])
            let comments = ResizeArray [|Comment.create("Comment 1")|]
            let remarks = ResizeArray [|Remark.create(1, "Remark 1")|]

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
                    workflows
                    runs
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
            Expect.equal actual.Workflows workflows "Workflows"
            Expect.equal actual.Runs runs "Runs"
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

let tests_RegisteredAssays = testList "RegisteredAssays" [
    testCase "Investigation.RegisterAssay" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        i.RegisterAssay(s.Identifier, a.Identifier)
        Expect.equal s.RegisteredAssayIdentifierCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
    testCase "Study.RegisterAssay" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        s.RegisterAssay(a.Identifier)
        Expect.equal s.RegisteredAssayIdentifierCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
    testCase "Investigation.DeregisterAssay" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        i.RegisterAssay(s.Identifier, a.Identifier)
        Expect.equal s.RegisteredAssayIdentifierCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
        i.DeregisterAssay(s.Identifier,a.Identifier)
        Expect.equal s.RegisteredAssayIdentifierCount 0 "registered assay count 2"
    testCase "Study.DeregisterAssay" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        s.RegisterAssay(a.Identifier)
        Expect.equal s.RegisteredAssayIdentifierCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
        s.DeregisterAssay(a.Identifier)
        Expect.equal s.RegisteredAssayIdentifierCount 0 "registered assay count 2"
    testCase "Remove registered assay from investigation" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        Expect.equal i.AssayCount 1 "assay count"
        Expect.equal i.StudyCount 1 "study count"
        s.RegisterAssay(a.Identifier)
        Expect.equal s.RegisteredAssayIdentifierCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
        i.RemoveAssayAt 0 
        Expect.equal i.AssayCount 0 "assay count 2"
        Expect.equal s.RegisteredAssayIdentifierCount 0 "registered assay count 2"
    testCase "Remove registered assay from investigation, without removing other missing assays" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let a = i.InitAssay("MyAssay")
        let s = i.InitStudy("MyStudy")
        s.RegisterAssay(a.Identifier)
        Expect.equal s.RegisteredAssayIdentifierCount 1 "registered assay count"
        Expect.equal s.RegisteredAssayIdentifiers.[0] a.Identifier "identifier"
        s.RegisteredAssayIdentifiers.Add("Any Assay That is missing")
        Expect.equal s.RegisteredAssayIdentifierCount 2 "registered assay count 2"
        i.RemoveAssayAt 0
        Expect.equal i.AssayCount 0 "assay count"
        Expect.equal s.RegisteredAssayIdentifierCount 1 "registered assay count 3"
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
        Expect.equal s.RegisteredAssayIdentifierCount 1 "registered assay count 2"
        i.DeregisterMissingAssays()
        Expect.equal s.RegisteredAssayIdentifierCount 0 "registered assay count 3"
]

let tests_RegisteredStudies = testList "RegisteredStudies" [
    testCase "Remove Study" <| fun _ ->
        let i = ArcInvestigation.init("My Investigation")
        let s = ArcStudy.init("My Study")
        i.AddRegisteredStudy(s)
        Expect.hasLength i.RegisteredStudies 1 "Registered Studies count"
        Expect.equal i.StudyCount 1 "Studies count"
        i.RemoveStudy(s.Identifier)
        Expect.hasLength i.RegisteredStudies 0 "Registered Studies count 2"
        Expect.equal i.StudyCount 0 "Studies count 2"
    testCase "Delete Study" <| fun _ ->
        let i = ArcInvestigation.init("My Investigation")
        let s = ArcStudy.init("My Study")
        i.AddRegisteredStudy(s)
        Expect.hasLength i.RegisteredStudies 1 "Registered Studies count"
        Expect.equal i.StudyCount 1 "Studies count"
        i.DeleteStudy(s.Identifier)
        Expect.hasLength i.RegisteredStudyIdentifiers 1 "Registered Study identifiers count 2"
        Expect.hasLength i.RegisteredStudies 0 "Registered Studies count 2"
        Expect.equal i.StudyCount 0 "Studies count 2"
]

let tests_MutableFields = testList "MutableFields" [
    testCase "ensure investigation" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        Expect.equal i.Description None ""
    testCase "test mutable fields" <| fun _ ->
        let i = ArcInvestigation.init("MyInvestigation")
        let persons = ResizeArray [|Person.create(firstName="Kevin", lastName="Frey")|]
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
        let persons = ResizeArray [|Person.create(firstName="Kevin", lastName="Frey")|]
        i.Description <- Some "MyName"
        i.Contacts <- persons
        i.Title <- Some "Awesome Title"
        Expect.equal i.Description (Some "MyName") "FileName"
        Expect.equal i.Contacts persons "Contacts"
        Expect.equal i.Title (Some "Awesome Title") "Title"
        let copy = i.Copy()
        let nextPersons = ResizeArray [|Person.create(firstName="Pascal", lastName="Gevangen")|]
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
        let persons = ResizeArray [|Person.create(firstName="Kevin", lastName="Frey")|]
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
        let nextPersons = ResizeArray [|Person.create(firstName="Pascal", lastName="Gevangen")|]
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
        testCase "not existing, throws" <| fun _ ->
            let i = createExampleInvestigation()
            let func = fun () -> i.GetStudy("LOREM IPSUM; DOLOR") |> ignore
            Expect.throws func "Should throw on non existing study"
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
            let assay_techPlatform = OntologyAnnotation("Assay Tech")
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
            let assay_techPlatform = OntologyAnnotation("Assay Tech")
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
            let assay_techPlatform = OntologyAnnotation("Assay Tech")
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
            let assay_techPlatform = OntologyAnnotation("Assay Tech","ABC","ABC:123")
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
            let assay_techPlatform = OntologyAnnotation("Assay Tech")
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
        testCase "not existing, throws" <| fun _ ->
            let i = createExampleInvestigation()
            let func = fun () -> i.GetAssay("LOREM IPSUM; DOLOR") |> ignore
            Expect.throws func "Should throw on non existing assay"
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
            let tech = Some (OntologyAnnotation("New Tech Stuff"))
            let a = i.GetAssayAt(0)
            Expect.equal a.Identifier "Assay 1" "FileName"
            Expect.equal a.TechnologyPlatform None "TechnologyPlatform"
            a.TechnologyPlatform <- tech
            Expect.equal a.TechnologyPlatform tech "TechnologyPlatform, a"
            Expect.equal i.Assays.[0].TechnologyPlatform tech "TechnologyPlatform, direct"
        testCase "mutable propagation, copy" <| fun _ ->
            let i = createExampleInvestigation()
            let copy = createExampleInvestigation()
            let tech = Some (OntologyAnnotation("New Tech Stuff"))
            let a = i.GetAssayAt(0)
            Expect.equal a.Identifier "Assay 1" "FileName"
            Expect.equal a.TechnologyPlatform None "TechnologyPlatform"
            a.TechnologyPlatform <- tech
            Expect.equal a.TechnologyPlatform tech "TechnologyPlatform, a"
            Expect.equal i.Assays.[0].TechnologyPlatform tech "TechnologyPlatform, direct"
            Expect.equal copy.Assays.[0].TechnologyPlatform None "TechnologyPlatform, copy direct"
    ]
]

let tests_Workflow = testList "CRUD_Workflow" [
    let createExampleInvestigation() =
        let i = ArcInvestigation.init("MyInvestigation")
        let w = ArcWorkflow.init("Workflow 1")
        let w2 = ArcWorkflow.create("Workflow 2",title="Workflow 2 Title")
        i.AddWorkflow w
        i.AddWorkflow w2
        i
    testCase "Ensure example" <| fun _ ->
        let i = createExampleInvestigation()
        Expect.equal i.WorkflowCount 2 "WorkflowCount"
        TestingUtils.Expect.sequenceEqual i.WorkflowIdentifiers (seq {"Workflow 1"; "Workflow 2"}) "WorkflowIdentifiers"
        let w = i.Workflows.Item 0 
        Expect.equal w.Title None "Workflow 1 Title"
        let w2 = i.Workflows.Item 1 
        Expect.equal w2.Title (Some "Workflow 2 Title") "Workflow 2 Title"
    testList "AddWorkflow" [
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let workflow_ident = "New Workflow"
            let expected = ArcWorkflow(workflow_ident, version = "0.0.1")
            i.AddWorkflow(expected)
            Expect.equal i.WorkflowCount 3 "Workflow"
            let actual = i.GetWorkflowAt 2
            Expect.equal actual expected "equal"
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let workflow_ident = "New Workflow"
            let expected = ArcWorkflow(workflow_ident, version = "0.0.1")
            i.AddWorkflow(expected)
            Expect.equal i.WorkflowCount 3 "Workflow"
            let actual = i.GetWorkflowAt 2
            Expect.equal actual expected "equal"
    ]
    testList "SetWorkflow" [
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let workflow_ident = "New Workflow"
            let expected = ArcWorkflow(workflow_ident, version = "0.0.1")
            i.SetWorkflowAt(0, expected)
            Expect.equal i.WorkflowCount 2 "Workflow"
            Expect.equal i.Workflows.[0] expected "equal"
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let workflow_ident = "New Workflow"
            let expected = ArcWorkflow(workflow_ident, version = "0.0.1")
            i.SetWorkflow("Workflow 2", expected)
            Expect.equal i.WorkflowCount 2 "Workflow"
            let actual = i.Workflows.[1]
            Expect.equal actual expected "equal"
    ]
    testList "GetWorkflow" [
        testCase "not existing, throws" <| fun _ ->
            let i = createExampleInvestigation()
            let func = fun () -> i.GetWorkflow("LOREM IPSUM; DOLOR") |> ignore
            Expect.throws func "Should throw on non existing workflow"
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let w = i.GetWorkflowAt(0)
            Expect.equal w.Identifier "Workflow 1" "FileName"
        testCase "by ident" <| fun _ ->
            let i = createExampleInvestigation()
            let w = i.GetWorkflow("Workflow 2")
            Expect.equal w.Identifier "Workflow 2" "FileName"
        testCase "mutable propagation" <| fun _ ->
            let i = createExampleInvestigation()
            let w = i.GetWorkflowAt(0)
            Expect.equal w.Identifier "Workflow 1" "FileName"
            Expect.equal w.Title None "Title"
            w.Title <- Some "New Title"
            Expect.equal w.Title (Some "New Title") "Title, w"
            Expect.equal i.Workflows.[0].Title (Some "New Title") "Title, direct"
        testCase "mutable propagation, copy" <| fun _ ->
            let i = createExampleInvestigation()
            let copy = createExampleInvestigation()
            let w = i.GetWorkflowAt(0)
            Expect.equal w.Identifier "Workflow 1" "FileName"
            Expect.equal w.Title None "Title"
            w.Title <- Some "New Title"
            Expect.equal w.Title (Some "New Title") "Title, w"
            Expect.equal i.Workflows.[0].Title (Some "New Title") "Title, direct"
            Expect.equal copy.Workflows.[0].Title None "Title, copy direct"
    ]
    
]

let tests_Run = testList "CRUD Run" [
    let createExampleInvestigation() =
        let i = ArcInvestigation.init("MyInvestigation")
        let w = ArcRun.init("Run 1")
        let w2 = ArcRun.create("Run 2",title="Run 2 Title")
        i.AddRun w
        i.AddRun w2
        i
    testCase "Ensure example" <| fun _ ->
        let i = createExampleInvestigation()
        Expect.equal i.RunCount 2 "RunCount"
        TestingUtils.Expect.sequenceEqual i.RunIdentifiers (seq {"Run 1"; "Run 2"}) "RunIdentifiers"
        let w = i.Runs.Item 0
        Expect.equal w.Title None "Run 1 Title"
        let w2 = i.Runs.Item 1
        Expect.equal w2.Title (Some "Run 2 Title") "Run 2 Title"
    testList "AddRun" [
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let run_ident = "New Run"
            let expected = ArcRun(run_ident, description = "So good")
            i.AddRun(expected)
            Expect.equal i.RunCount 3 "Run"
            let actual = i.GetRunAt 2
            Expect.equal actual expected "equal"
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let run_ident = "New Run"
            let expected = ArcRun(run_ident, description = "So good")
            i.AddRun(expected)
            Expect.equal i.RunCount 3 "Run"
            let actual = i.GetRunAt 2
            Expect.equal actual expected "equal"
    ]
    testList "SetRun" [
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let run_ident = "New Run"
            let expected = ArcRun(run_ident, description = "So good")
            i.SetRunAt(0, expected)
            Expect.equal i.RunCount 2 "Run"
            Expect.equal i.Runs.[0] expected "equal"
        testCase "by identifier" <| fun _ ->
            let i = createExampleInvestigation()
            let run_ident = "New Run"
            let expected = ArcRun(run_ident, description = "So good")
            i.SetRun("Run 2", expected)
            Expect.equal i.RunCount 2 "Run"
            let actual = i.Runs.[1]
            Expect.equal actual expected "equal"
    ]
    testList "GetRun" [
        testCase "not existing, throws" <| fun _ ->
            let i = createExampleInvestigation()
            let func = fun () -> i.GetRun("LOREM IPSUM; DOLOR") |> ignore
            Expect.throws func "Should throw on non existing run"
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let w = i.GetRunAt(0)
            Expect.equal w.Identifier "Run 1" "FileName"
        testCase "by ident" <| fun _ ->
            let i = createExampleInvestigation()
            let w = i.GetRun("Run 2")
            Expect.equal w.Identifier "Run 2" "FileName"
        testCase "mutable propagation" <| fun _ ->
            let i = createExampleInvestigation()
            let w = i.GetRunAt(0)
            Expect.equal w.Identifier "Run 1" "FileName"
            Expect.equal w.Title None "Title"
            w.Title <- Some "New Title"
            Expect.equal w.Title (Some "New Title") "Title, w"
            Expect.equal i.Runs.[0].Title (Some "New Title") "Title, direct"
        testCase "mutable propagation, copy" <| fun _ ->
            let i = createExampleInvestigation()
            let copy = createExampleInvestigation()
            let w = i.GetRunAt(0)
            Expect.equal w.Identifier "Run 1" "FileName"
            Expect.equal w.Title None "Title"
            w.Title <- Some "New Title"
            Expect.equal w.Title (Some "New Title") "Title, w"
            Expect.equal i.Runs.[0].Title (Some "New Title") "Title, direct"
            Expect.equal copy.Runs.[0].Title None "Title, copy direct"
    ]

]

let tests_UpdateIOTypeByEntityIDTypes = testList "UpdateIOTypeByEntityIDType" [
    testList "SameAssay" [ 
        testCase "nothingToUpdate" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let a = i.InitAssay("MyAssay")
            let t1 = a.InitTable("MyTable")
            let t2 = a.InitTable("MyTable2")
            t1.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Source, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Source %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample %i" i)))
            |]
            t2.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Source, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Source_Alt %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample_Alt %i" i)))
            |]
            let a_Copy = a.Copy()
            i.UpdateIOTypeByEntityID()
            Expect.sequenceEqual a.Tables a_Copy.Tables "Tables should be unchanged"
        testCase "updateOutputByNextInput" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let a = i.InitAssay("MyAssay")
            let t1 = a.InitTable("MyTable")
            let t2 = a.InitTable("MyTable2")
            t1.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Source, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Source %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample %i" i)))
            |]
            t2.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Source, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample_Alt %i" i)))
            |]
            i.UpdateIOTypeByEntityID()
            Expect.sequenceEqual t1.Headers [CompositeHeader.Input IOType.Source; CompositeHeader.Output IOType.Sample] "Headers should be updated"
            Expect.sequenceEqual t2.Headers [CompositeHeader.Input IOType.Sample; CompositeHeader.Output IOType.Sample] "Headers should be updated"
        testCase "failBecauseClashing" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let a = i.InitAssay("MyAssay")
            let t1 = a.InitTable("MyTable")
            let t2 = a.InitTable("MyTable2")
            t1.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Source, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Source %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample %i" i)))
            |]
            t2.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Data, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample_Alt %i" i)))
            |]
            Expect.throws (fun () -> i.UpdateIOTypeByEntityID()) "Update should fail as sample and data can not be updated against each other."
    ]
    testList "AssayAndStudy" [ 
        testCase "nothingToUpdate" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let s = i.InitStudy("MyStudy")
            let a = i.InitAssay("MyAssay")
            let t1 = s.InitTable("MyTable")
            let t2 = a.InitTable("MyTable2")
            t1.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Source, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Source %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample %i" i)))
            |]
            t2.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Source, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Source_Alt %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample_Alt %i" i)))
            |]
            let a_Copy = a.Copy()
            let s_Copy = s.Copy()
            i.UpdateIOTypeByEntityID()
            Expect.sequenceEqual a.Tables a_Copy.Tables "Tables should be unchanged"
            Expect.sequenceEqual s.Tables s_Copy.Tables "Tables should be unchanged"
        testCase "updateOutputByNextInput" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let s = i.InitStudy("MyStudy")
            let a = i.InitAssay("MyAssay")
            let t1 = s.InitTable("MyTable")
            let t2 = a.InitTable("MyTable2")
            t1.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Source, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Source %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample %i" i)))
            |]
            t2.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Source, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample_Alt %i" i)))
            |]
            i.UpdateIOTypeByEntityID()
            Expect.sequenceEqual t1.Headers [CompositeHeader.Input IOType.Source; CompositeHeader.Output IOType.Sample] "Headers should be updated"
            Expect.sequenceEqual t2.Headers [CompositeHeader.Input IOType.Sample; CompositeHeader.Output IOType.Sample] "Headers should be updated"
        testCase "failBecauseClashing" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let s = i.InitStudy("MyStudy")
            let a = i.InitAssay("MyAssay")
            let t1 = s.InitTable("MyTable")
            let t2 = a.InitTable("MyTable2")
            t1.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Source, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Source %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample %i" i)))
            |]
            t2.AddColumns [|
                CompositeColumn.create (CompositeHeader.Input IOType.Data, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample %i" i)))
                CompositeColumn.create (CompositeHeader.Output IOType.Sample, Array.init 3 (fun i -> CompositeCell.createFreeText (sprintf "Sample_Alt %i" i)))
            |]
            Expect.throws (fun () -> i.UpdateIOTypeByEntityID()) "Update should fail as sample and data can not be updated against each other."
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
                (ResizeArray  [|OntologySourceReference("Some Lorem ipsum description", name="Descriptore"); OntologySourceReference.empty|])
                (ResizeArray [|Publication(); Publication(title="Some nice title")|])
                (ResizeArray [|Person(firstName="John",lastName="Doe"); Person(firstName="Jane",lastName="Doe")|])
                (ResizeArray ([ArcAssay.init("Registered Assay1"); ArcAssay.init("Registered Assay2")]))
                (ResizeArray ([ArcStudy.init("Registered Study1"); ArcStudy.init("Registered Study2")]))
                (ResizeArray([ArcWorkflow.init("My Workflow")]))
                (ResizeArray([ArcRun.init("My Run")]))
                (ResizeArray (["Registered Study1"; "Registered Study2"]))
                (ResizeArray [|Comment("Hello", "World"); Comment("ByeBye", "World") |])
                (ResizeArray [|Remark.create(12,"Test"); Remark.create(42, "The answer")|])
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
        tests_create
        tests_RegisteredAssays
        tests_RegisteredStudies
        tests_MutableFields
        tests_Copy
        tests_Study
        tests_Assay
        tests_Workflow
        tests_Run
        tests_GetHashCode
        tests_UpdateIOTypeByEntityIDTypes
        // tests_UpdateBy
    ]