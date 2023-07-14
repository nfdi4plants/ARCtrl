module ArcInvestigation.Tests

open ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let tests_MutableFields = testList "MutableFields" [
    testCase "ensure investigation" <| fun _ ->
        let i = ArcInvestigation.createEmpty("MyInvestigation")
        Expect.equal i.FileName None ""
    testCase "test mutable fields" <| fun _ ->
        let i = ArcInvestigation.createEmpty("MyInvestigation")
        let persons = [Person.create(FirstName="Kevin", LastName="Frey")]
        i.FileName <- Some "MyName"
        i.Contacts <- persons
        i.Title <- Some "Awesome Title"
        Expect.equal i.FileName (Some "MyName") "FileName"
        Expect.equal i.Contacts persons "Contacts"
        Expect.equal i.Title (Some "Awesome Title") "Title"
]

let tests_Copy = testList "Copy" [
    testCase "test mutable fields" <| fun _ ->
        let i = ArcInvestigation.createEmpty("MyInvestigation")
        let persons = [Person.create(FirstName="Kevin", LastName="Frey")]
        i.FileName <- Some "MyName"
        i.Contacts <- persons
        i.Title <- Some "Awesome Title"
        Expect.equal i.FileName (Some "MyName") "FileName"
        Expect.equal i.Contacts persons "Contacts"
        Expect.equal i.Title (Some "Awesome Title") "Title"
        let copy = i.Copy()
        let nextPersons = [Person.create(FirstName="Pascal", LastName="Gevangen")]
        copy.FileName <- Some "Next FileName"
        copy.Contacts <- nextPersons
        copy.Title <- Some "Next Title"
        Expect.equal i.FileName (Some "MyName") "FileName, after copy"
        Expect.equal i.Contacts persons "Contacts, after copy"
        Expect.equal i.Title (Some "Awesome Title") "Title, after copy"
        Expect.equal copy.FileName (Some "Next FileName") "copy FileName"
        Expect.equal copy.Contacts nextPersons "copy Contacts"
        Expect.equal copy.Title (Some "Next Title") "copy Title"
        
]

let tests_Study = testList "CRUD Study" [
    let createExampleInvestigation() =
        let i = ArcInvestigation.createEmpty("MyInvestigation")
        let s = ArcStudy.createEmpty("Study 1")
        let s2 = ArcStudy.create("Study 2",title="Study 2 Title")
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
        let i = ArcInvestigation.createEmpty("MyInvestigation")
        let s = ArcStudy.createEmpty("Study 1")
        let a = ArcAssay.createEmpty("Assay 1")
        let a2 = ArcAssay.createEmpty("Assay 2")
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
            let expected = ArcAssay.create(assay_ident, technologyPlatform = assay_techPlatform)
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
            let expected = ArcAssay.create(assay_ident, technologyPlatform = assay_techPlatform)
            i.AddAssay("Study 1", expected)
            Expect.equal i.StudyCount 2 "StudyCount"
            let s = i.Studies.Item 0 
            Expect.equal s.AssayCount 3 "AssayCount"
            let actual = s.GetAssayAt 2
            Expect.equal actual expected "equal"
    ]
    testList "GetAssay" [ 
        testCase "by index" <| fun _ ->
            let i = createExampleInvestigation()
            let a = i.GetAssayAt("Study 1", 0)
            Expect.equal a.FileName "Assay 1" "FileName"
        testCase "by ident" <| fun _ ->
            let i = createExampleInvestigation()
            let a = i.GetAssay("Study 1", "Assay 2")
            Expect.equal a.FileName "Assay 2" "FileName"
        testCase "mutable propagation" <| fun _ ->
            let i = createExampleInvestigation()
            let tech = Some "New Tech Stuff"
            let a = i.GetAssayAt("Study 1", 0)
            Expect.equal a.FileName "Assay 1" "FileName"
            Expect.equal a.TechnologyPlatform None "TechnologyPlatform"
            a.TechnologyPlatform <- tech
            Expect.equal a.TechnologyPlatform tech "TechnologyPlatform, a"
            Expect.equal i.Studies.[0].Assays.[0].TechnologyPlatform tech "TechnologyPlatform, direct"
        testCase "mutable propagation, copy" <| fun _ ->
            let i = createExampleInvestigation()
            let copy = createExampleInvestigation()
            let tech = Some "New Tech Stuff"
            let a = i.GetAssayAt("Study 1", 0)
            Expect.equal a.FileName "Assay 1" "FileName"
            Expect.equal a.TechnologyPlatform None "TechnologyPlatform"
            a.TechnologyPlatform <- tech
            Expect.equal a.TechnologyPlatform tech "TechnologyPlatform, a"
            Expect.equal i.Studies.[0].Assays.[0].TechnologyPlatform tech "TechnologyPlatform, direct"
            Expect.equal copy.Studies.[0].Assays.[0].TechnologyPlatform None "TechnologyPlatform, copy direct"
    ]
]


let main = 
    testList "ArcInvestigation" [
        tests_MutableFields
        tests_Copy
        tests_Study
        tests_Assay
    ]