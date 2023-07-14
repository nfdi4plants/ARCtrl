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
    testList "AddStudy" [
        testCase "add" <| fun _ ->
            let i = ArcInvestigation.createEmpty("MyInvestigation")
            let s = ArcStudy.createEmpty
            Expect.isTrue true ""
    ]
]

let tests_Assay = testList "CRUD Assay" [
    testCase "placeholder" <| fun _ ->
        Expect.isTrue true ""
]


let main = 
    testList "ArcInvestigation" [
        tests_MutableFields
        tests_Copy
        tests_Study
        tests_Assay
    ]