module ArcWorkflow.Tests

open ARCtrl

open TestingUtils

module Helper =
    let TableName = "Test"
    let oa_species = OntologyAnnotation("species", "GO", "GO:0123456")
    let oa_chlamy = OntologyAnnotation("Chlamy", "NCBI", "NCBI:0123456")
    let oa_instrumentModel = OntologyAnnotation("instrument model", "MS", "MS:0123456")
    let oa_SCIEXInstrumentModel = OntologyAnnotation("SCIEX instrument model", "MS", "MS:654321")
    let oa_temperature = OntologyAnnotation("temperature","NCIT","NCIT:0123210")
    let oa_time = OntologyAnnotation("time","PATO","PATO:0000165")

    let component_instrument = Process.Component.create(
        value = Value.Ontology oa_SCIEXInstrumentModel,
        componentType = oa_instrumentModel
    )

    let oa_proteomics = OntologyAnnotation("Proteomics", "MS", "MS:123456")

    let create_exampleWorkflowWithIDentifier (identifier : string) =
        let title = "Best Workflow On Earth"
        let description = "Workflow to be used for being the best"
        let workflowType = oa_proteomics
        let uri = "http://example.com/MyWorkflow"
        let version = "1.0.0"
        let subWorkflowIdentifiers = ResizeArray [|"SubWorkflow"|]
        let parameters = ResizeArray [|oa_temperature; oa_time|]
        let components = ResizeArray [|component_instrument|]
        let contacts = ResizeArray [|Person(firstName = "Kevin", lastName = "Frey")|]
        let comments = ResizeArray [|Comment.create("Comment Name")|]
        ArcWorkflow(identifier, title = title, description = description, workflowType = workflowType, uri = uri, version = version, subWorkflowIdentifiers = subWorkflowIdentifiers, parameters = parameters, components = components, contacts = contacts, comments = comments)

    let create_exampleWorkflow() =
        create_exampleWorkflowWithIDentifier "MyIdentifier"


open Helper

let private test_create =
    testList "create" [
        testCase "constructor" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Best Workflow On Earth"
            let description = "Workflow to be used for being the best"
            let workflowType = oa_proteomics
            let uri = "http://example.com/MyWorkflow"
            let version = "1.0.0"
            let subWorkflowIdentifiers = ResizeArray [|"SubWorkflow"|]
            let parameters = ResizeArray [|oa_temperature; oa_time|]
            let components = ResizeArray [|component_instrument|]
            let contacts = ResizeArray [|Person(firstName = "Kevin", lastName = "Frey")|]
            let comments = ResizeArray [|Comment.create("Comment Name")|]
            let actual = create_exampleWorkflow()
            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title (Some title) "title"
            Expect.equal actual.Description (Some description) "description"
            Expect.equal actual.WorkflowType (Some workflowType) "workflowType"
            Expect.equal actual.URI (Some uri) "uri"
            Expect.equal actual.Version (Some version) "version"
            Expect.sequenceEqual actual.SubWorkflowIdentifiers subWorkflowIdentifiers "subWorkflowIdentifiers"
            Expect.sequenceEqual actual.Parameters parameters "parameters"
            Expect.sequenceEqual actual.Components components "components"
            Expect.sequenceEqual actual.Contacts contacts "contacts"
            Expect.sequenceEqual actual.Comments comments "Comments"       
        testCase "create" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Best Workflow On Earth"
            let description = "Workflow to be used for being the best"
            let workflowType = oa_proteomics
            let uri = "http://example.com/MyWorkflow"
            let version = "1.0.0"
            let subWorkflowIdentifiers = ResizeArray [|"SubWorkflow"|]
            let parameters = ResizeArray [|oa_temperature; oa_time|]
            let components = ResizeArray [|component_instrument|]
            let contacts = ResizeArray [|Person(firstName = "Kevin", lastName = "Frey")|]
            let comments = ResizeArray [|Comment.create("Comment Name")|]
            let actual = ArcWorkflow.create(identifier, title = title, description = description, workflowType = workflowType, uri = uri, version = version, subWorkflowIdentifiers = subWorkflowIdentifiers, parameters = parameters, components = components, contacts = contacts, comments = comments)
            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title (Some title) "title"
            Expect.equal actual.Description (Some description) "description"
            Expect.equal actual.WorkflowType (Some workflowType) "workflowType"
            Expect.equal actual.URI (Some uri) "uri"
            Expect.equal actual.Version (Some version) "version"
            Expect.sequenceEqual actual.SubWorkflowIdentifiers subWorkflowIdentifiers "subWorkflowIdentifiers"
            Expect.sequenceEqual actual.Parameters parameters "parameters"
            Expect.sequenceEqual actual.Components components "components"
            Expect.sequenceEqual actual.Contacts contacts "contacts"
            Expect.sequenceEqual actual.Comments comments "Comments"
        testCase "init" <| fun _ ->
            let identifier = "MyIdentifier"
            let actual = ArcWorkflow.init identifier
            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title None "title"
            Expect.equal actual.Description None "description"
            Expect.equal actual.URI None "uri"
            Expect.equal actual.Version None "version"
            Expect.isEmpty actual.SubWorkflowIdentifiers "subWorkflowIdentifiers"
            Expect.isEmpty actual.Parameters "parameters"
            Expect.isEmpty actual.Contacts "contacts"
            Expect.isEmpty actual.Comments "Comments"
        testCase "make" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = Some "Best Workflow On Earth"
            let description = Some "Workflow to be used for being the best"
            let workflowType = Some oa_proteomics
            let uri = Some "http://example.com/MyWorkflow"
            let version = Some "1.0.0"
            let subWorkflowIdentifiers = ResizeArray [|"SubWorkflow"|]
            let parameters = ResizeArray [|oa_temperature; oa_time|]
            let components = ResizeArray [|component_instrument|]
            let contacts = ResizeArray [|Person.create(firstName = "John", lastName = "Doe")|]
            let comments = ResizeArray [|Comment.create("Comment 1")|]

            let actual = 
                ArcWorkflow.make
                    identifier
                    title
                    description
                    workflowType
                    uri
                    version
                    subWorkflowIdentifiers
                    parameters
                    components
                    None
                    contacts
                    None
                    comments

            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title  title "title"
            Expect.equal actual.Description description "description"
            Expect.equal actual.WorkflowType workflowType "workflowType"
            Expect.equal actual.URI uri "uri"
            Expect.equal actual.Version version "version"
            Expect.sequenceEqual actual.SubWorkflowIdentifiers subWorkflowIdentifiers "subWorkflowIdentifiers"
            Expect.sequenceEqual actual.Parameters parameters "parameters"
            Expect.sequenceEqual actual.Components components "components"
            Expect.sequenceEqual actual.Contacts contacts "contacts"
            Expect.sequenceEqual actual.Comments comments "Comments"       
        testCase "failsForInvalidCharacters" <| fun _ ->
            let createWorkflow =
                fun () -> ArcWorkflow("My{Workflow") |> ignore
            Expect.throws createWorkflow "throws, invalid characters"
        testCase "whiteSpaceTrimmed" <| fun _ ->
            let workflow = ArcWorkflow(" MyWorkflow ")
            Expect.equal workflow.Identifier "MyWorkflow" "Identifier"
    ]

let private tests_Copy = 
    testList "Copy" [
        testCase "Ensure mutability" (fun () ->
            let workflow = create_exampleWorkflow()
            Expect.hasLength workflow.Parameters 2 "There are 2 Parameters"
            workflow.Parameters.RemoveAt(1)
            Expect.hasLength workflow.Parameters 1 "There is 1 Parameter"
        )
        testCase "CopyInheritsValues" (fun () ->
            let workflow = create_exampleWorkflow()
            let copy = workflow.Copy()
            Expect.equal workflow.Identifier copy.Identifier "Identifier"
            Expect.equal workflow.Title copy.Title "Title"
            Expect.equal workflow.Description copy.Description "Description"
            Expect.equal workflow.WorkflowType copy.WorkflowType "WorkflowType"
            Expect.equal workflow.URI copy.URI "URI"
            Expect.equal workflow.Version copy.Version "Version"
            Expect.sequenceEqual workflow.SubWorkflowIdentifiers copy.SubWorkflowIdentifiers "SubWorkflowIdentifiers"
            Expect.sequenceEqual workflow.Parameters copy.Parameters "Parameters"
            Expect.sequenceEqual workflow.Components copy.Components "Components"
            Expect.sequenceEqual workflow.Contacts copy.Contacts "Contacts"
            Expect.sequenceEqual workflow.Comments copy.Comments "Comments"
        )
        testCase "IndependentMutability" (fun () ->
            let workflow = create_exampleWorkflow()
            let copy = workflow.Copy()
            copy.Title <- Some "New Title"
            copy.Description <- Some "New Description"
            copy.WorkflowType <- Some (OntologyAnnotation("NewWorkflowType"))
            copy.URI <- Some "http://example.com/NewWorkflow"
            copy.Version <- Some "2.0.0"
            copy.SubWorkflowIdentifiers.Add("NewSubWorkflow")
            copy.Parameters.Add(OntologyAnnotation("NewParameter"))
            copy.Components.Add(Process.Component.create(Value.Ontology (OntologyAnnotation("NewComponent")), OntologyAnnotation("NewComponentType")))
            copy.Contacts.Add(Person(firstName = "New", lastName = "Person"))
            copy.Comments.Add(Comment.create("New Comment"))
            let expected = create_exampleWorkflow()
            Expect.equal workflow.Identifier expected.Identifier "Identifier"
            Expect.equal workflow.Title expected.Title "Title"
            Expect.equal workflow.Description expected.Description "Description"
            Expect.equal workflow.WorkflowType expected.WorkflowType "WorkflowType"
            Expect.equal workflow.URI expected.URI "URI"
            Expect.equal workflow.Version expected.Version "Version"
            Expect.sequenceEqual workflow.SubWorkflowIdentifiers expected.SubWorkflowIdentifiers "SubWorkflowIdentifiers"
            Expect.sequenceEqual workflow.Parameters expected.Parameters "Parameters"
            Expect.sequenceEqual workflow.Components expected.Components "Components"
            Expect.sequenceEqual workflow.Contacts expected.Contacts "Contacts"
            Expect.sequenceEqual workflow.Comments expected.Comments "Comments"
        )
        
    ]

let private tests_GetHashCode = testList "GetHashCode" [    
    testCase "passing" <| fun _ ->
        let actual = ArcStudy.init("MyStudy")
        Expect.isSome (actual.GetHashCode() |> Some) ""
    testCase "equal minimal" <| fun _ -> 
        let workflow = ArcStudy.init("MyStudy")
        let copy = workflow.Copy()
        let workflow2 = ArcStudy.init("MyStudy")
        Expect.equal workflow copy "equal"
        Expect.equal (workflow.GetHashCode()) (copy.GetHashCode()) "copy hash equal"
        Expect.equal (workflow.GetHashCode()) (workflow2.GetHashCode()) "workflow2 hash equal"
    testCase "equal" <| fun _ -> 
        let workflow = create_exampleWorkflow()
        let copy = workflow.Copy()
        Expect.equal (workflow.GetHashCode()) (copy.GetHashCode()) ""
    testCase "notEqual" <| fun _ ->
        let x1 = ArcWorkflow.init("My workflow")
        let x2 = ArcWorkflow.init("My other workflow")
        Expect.notEqual x1 x2 "not equal"
        Expect.notEqual (x1.GetHashCode()) (x2.GetHashCode()) "not equal hash"
    testCase "unequalWorkflowIdentifiers" <| fun _ ->
        let workflow1 = create_exampleWorkflowWithIDentifier "MyWorkflow"
        let workflow2 = create_exampleWorkflowWithIDentifier "MyWorkflow"
        workflow2.SubWorkflowIdentifiers.Add("Workflow3")
        Expect.notEqual (workflow1.GetHashCode()) (workflow2.GetHashCode()) "unequal hash"
]

let main = 
    testList "ArcWorkflow" [
        test_create
        tests_Copy
        tests_GetHashCode
    ]