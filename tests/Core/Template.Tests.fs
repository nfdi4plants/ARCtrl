module Template.Tests

open Thoth.Json.Core


open ARCtrl.Json
open ARCtrl

open TestingUtils

let create_TestTemplate() =
    let table = ArcTable.init("My Table")
    table.AddColumn(CompositeHeader.Input IOType.Source, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Source {i}")|])
    table.AddColumn(CompositeHeader.Output IOType.Data, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Output {i}")|])
    let guid = System.Guid(String.init 32 (fun _ -> "d"))
    Template.make 
        guid 
        table 
        "My Template" 
        "My Template is great"
        DataPLANT 
        "1.0.3" 
        (ResizeArray [|Person.create(firstName="John", lastName="Doe")|])
        (ResizeArray [|OntologyAnnotation "My oa rep"|])
        (ResizeArray [|OntologyAnnotation "My oa tag"|])
        (System.DateTime(2023,09,19))


let private tests_equality = testList "equality" [

    testList "override equality" [
        testCase "equal" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            Expect.equal template1 template2 "equal"
        testCase "not equal" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            template2.Name <- "New Name"
            Expect.notEqual template1 template2 "not equal"
    ]
    testList "structural equality" [
        testCase "equal" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            let equals = template1.StructurallyEquals(template2)
            Expect.isTrue equals "equal"
        testCase "not equal" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            template2.Name <- "New Name"
            let equals = template1.StructurallyEquals(template2)
            Expect.isFalse equals "not equal"
    ]
    testList "reference equality" [
        testCase "not same object" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            let equals = template1.ReferenceEquals(template2)
            Expect.isFalse equals ""
        testCase "same object" <| fun _ ->
            let template1 = create_TestTemplate()
            let equals = template1.ReferenceEquals(template1)
            Expect.isTrue equals ""
    ]
]

let private tests_HashCode = 
    testList "hashcode" [
        testCase "equal" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            let hash1 = template1.GetHashCode()
            let hash2 = template2.GetHashCode()
            Expect.equal hash1 hash2 "equal"
        testCase "not equal" <| fun _ ->
            let template1 = create_TestTemplate()
            let template2 = create_TestTemplate()
            template2.Name <- "New Name"
            let hash1 = template1.GetHashCode()
            let hash2 = template2.GetHashCode()
            Expect.notEqual hash1 hash2 "not equal"
    ]


let private tests_filters = testList "filters" [
    let create_TestTemplate() =
        let guid = System.Guid(String.init 32 (fun _ -> "d"))
        Template.make 
            guid 
            (ArcTable.init("TestTable")) 
            "My Template" 
            "My Template is great"
            DataPLANT 
            "1.0.3" 
            (ResizeArray [|Person(firstName="John", lastName="Doe")|])
            (ResizeArray [|OntologyAnnotation "PRIDE";|])
            (ResizeArray [|OntologyAnnotation "Protein"; OntologyAnnotation "DNA";|])
            (System.DateTime(2023,09,19))
    // this testList is representative to filterByEndpointRepositories
    testList "filterByTags" [
        testCase "OR, contains all" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "DNA"|]
            let actual = Templates.filterByTags(queryTags) templates
            let expected = 1
            Expect.equal actual.Count expected ""
        testCase "OR, contains different" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "DNA"; OntologyAnnotation "RNA"|]
            let actual = Templates.filterByTags(queryTags) templates
            let expected = 1
            Expect.equal actual.Count expected ""
        testCase "AND, contains some" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "DNA"|]
            let actual = Templates.filterByTags(queryTags, true) templates
            let expected = 1
            Expect.equal actual.Count expected ""
        testCase "AND, contains all" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "DNA"; OntologyAnnotation "Protein";|]
            let actual = Templates.filterByTags(queryTags, true) templates
            let expected = 1
            Expect.equal actual.Count expected ""
        testCase "AND, contains different" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "DNA"; OntologyAnnotation "RNA"|]
            let actual = Templates.filterByTags(queryTags, true) templates
            let expected = 0
            Expect.equal actual.Count expected ""
    ]
    testList "filterByOntologyAnnotations" [
        testCase "OR, contains tag" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "DNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags) templates
            let expected = 1
            Expect.equal actual.Count expected ""
        testCase "OR, contains er" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "PRIDE"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags) templates
            let expected = 1
            Expect.equal actual.Count expected ""
        testCase "OR, contains combined" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "PRIDE"; OntologyAnnotation "DNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags) templates
            let expected = 1
            Expect.equal actual.Count expected ""
        testCase "OR, contains different" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "DNA"; OntologyAnnotation "RNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags) templates
            let expected = 1
            Expect.equal actual.Count expected ""
        testCase "AND, contains tag" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "DNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags, true) templates
            let expected = 1
            Expect.equal actual.Count expected ""
        testCase "AND, contains er" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "PRIDE"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags, true) templates
            let expected = 1
            Expect.equal actual.Count expected ""
        testCase "AND, contains combined" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "PRIDE"; OntologyAnnotation "DNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags, true) templates
            let expected = 1
            Expect.equal actual.Count expected ""
        testCase "AND, contains different" <| fun _ -> 
            let templates = ResizeArray [|create_TestTemplate()|]
            let queryTags = ResizeArray [|OntologyAnnotation "DNA"; OntologyAnnotation "RNA"|]
            let actual = Templates.filterByOntologyAnnotation(queryTags, true) templates
            let expected = 0
            Expect.equal actual.Count expected ""
    ]
]

let private tests_copy = testList "Copy" [
    testCase "DefaultEquality"<| fun _ ->
        let template = create_TestTemplate()
        let copy = template.Copy()
        Expect.equal template copy "Templates should be equal"
    testCase "StructuralEquality" <| fun _ ->
        let template = create_TestTemplate()
        let copy = template.Copy()
        let equals = template.StructurallyEquals(copy)
        Expect.isTrue equals "Structural equality should be true"
    testCase "ReferenceEquality" <| fun _ ->
        let template = create_TestTemplate()
        let copy = template.Copy()
        let equals = template.ReferenceEquals(copy)
        Expect.isFalse equals "Reference equality should be false"
    testCase "UpdatePerson" <| fun _ ->
        let template = create_TestTemplate()
        let copy = template.Copy()
        Expect.equal template copy "Templates should be equal before change"
        copy.Authors.[0].FirstName <- Some "Jane"
        Expect.equal template.Authors.[0].FirstName (Some "John") "Name should not have been updated"
        Expect.notEqual copy template "Templates should not be equal after change"
    testCase "AddTableColumn" <| fun _ ->
        let template = create_TestTemplate()
        let copy = template.Copy()
        Expect.equal template copy "Templates should be equal before change"
        copy.Table.AddColumn(CompositeHeader.Parameter (OntologyAnnotation("VeryImportant")))
        Expect.notEqual copy template "Templates should not be equal after change"
]


let main = testList "Templates" [
    tests_equality
    tests_HashCode
    tests_filters
    tests_copy
]