module Tests.Workflow

open ARCtrl
open ARCtrl.Json
open TestingUtils

module Helper =

    let create_parameters() =
        ResizeArray [
            OntologyAnnotation("Explication", "MS", "MS:123456");
            OntologyAnnotation("Unit", "MS", "MS:123456")
        ]

    let create_component(i) =
        Process.Component.create(
            Value.Int i,
            OntologyAnnotation("Unit", "MS", "MS:123456"),
            OntologyAnnotation("Explication", "MS", "MS:123456")
        )

    let create_components(amount) =
        ResizeArray [
            for i in 1 .. amount do
                create_component i
        ]

    let create_Datacontext (i:int) =
        DataContext(
            $"id_string_{i}",
            "My Name",
            DataFile.DerivedDataFile,
            "My Format",
            "My Selector Format",
            OntologyAnnotation("Explication", "MS", "MS:123456"),
            OntologyAnnotation("Unit", "MS", "MS:123456"),
            OntologyAnnotation("ObjectType", "MS", "MS:123456"),
            "My Label",
            "My Description",
            "Kevin F",
            (ResizeArray [Comment.create("Hello", "World")])
        )

    let create_datamap(amount) = 
        Datamap(ResizeArray [
            for i in 1 .. amount do
                create_Datacontext i
        ])

    let create_Workflow () =
        ArcWorkflow(
            identifier = "identifier",
            title = "workflow",
            description = "test workflow",
            workflowType = OntologyAnnotation("ObjectType", "MS", "MS:123456"),
            uri = "www.test.de",
            version = "0.1-beta",
            subWorkflowIdentifiers = ResizeArray ["1"; "2"; "3"],
            parameters = create_parameters(),
            components = create_components 3,
            datamap = create_datamap 3,
            contacts = ResizeArray [|Person.create(firstName="Kevin", lastName="Frey")|],
            comments = (ResizeArray [Comment.create("Hello", "World")])
        )

    let compare =
        fun (w1: ArcWorkflow) (w2: ArcWorkflow) ->
            Expect.equal w1.Identifier w2.Identifier "Identifier"
            Expect.equal w1.Title w2.Title "Title"
            Expect.equal w1.Description w2.Description "Description"
            Expect.equal w1.WorkflowType w2.WorkflowType "WorkflowType"
            Expect.equal w1.URI w2.URI "URI"
            Expect.equal w1.Version w2.Version "Version"
            Expect.sequenceEqual w1.SubWorkflowIdentifiers w2.SubWorkflowIdentifiers "SubWorkflowIdentifiers"
            Expect.sequenceEqual w1.Parameters w2.Parameters "Parameters"
            Expect.sequenceEqual w1.Components w2.Components "Components"
            Expect.equal w1.Datamap w2.Datamap "Datamap"
            Expect.sequenceEqual w1.Contacts w2.Contacts "Contacts"
            Expect.sequenceEqual w1.Comments w2.Comments "Comments"
        |> Some

open Helper

let private test_workflowJsonTesting =
    createBaseJsonTests
        "workflow-json"
        create_Workflow
        ArcWorkflow.toJsonString
        ArcWorkflow.fromJsonString
        None
        compare


let private test_workflowCompressedJsonTesting =
    createBaseJsonTests
        "workflow-compressed-json"
        create_Workflow
        ArcWorkflow.toCompressedJsonString
        ArcWorkflow.fromCompressedJsonString
        None
        compare

let main = testList "Workflow" [
    test_workflowJsonTesting
    test_workflowCompressedJsonTesting
]
