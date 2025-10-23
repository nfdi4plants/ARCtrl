module Tests.Run

open ARCtrl
open ARCtrl.Json
open TestingUtils

module Helper =
    let create_filled_run() = 
        ArcRun.create(
            identifier = "My Cool Run",
            title = "Best Run",
            description = "This is a test run",
            measurementType = OntologyAnnotation("MT", "MS", "MS:424242", comments = ResizeArray [Comment.create("ByeBye","Space")]), 
            technologyType = OntologyAnnotation("TT", "MS", "MS:696969"), 
            technologyPlatform = OntologyAnnotation("TP", "MS", "MS:123456", comments = ResizeArray [Comment.create("Hello","Space")]),
            workflowIdentifiers = ResizeArray [| "Workflow 1"; "Workflow 2"|],
            tables = ResizeArray([Tests.ArcTable.Helper.create_filled(); ArcTable.init("My Second Table")]),
            datamap = DataMap.Helper.create_filled(),
            performers = ResizeArray [|Person.create(firstName="Kevin", lastName="Frey")|],
            comments = ResizeArray [|Comment.create("Hello", "World")|]
        )

    let compare =
        fun (r1: ArcRun) (r2: ArcRun) ->
            Expect.equal r1.Identifier r2.Identifier "Identifier"
            Expect.equal r1.Title r2.Title "Title"
            Expect.equal r1.Description r2.Description "Description"
            Expect.equal r1.MeasurementType r2.MeasurementType "MeasurementType"
            Expect.equal r1.TechnologyType r2.TechnologyType "TechnologyType"
            Expect.equal r1.TechnologyPlatform r2.TechnologyPlatform "TechnologyPlatform"
            Expect.equal r1.DataMap r2.DataMap "DataMap"
            Expect.sequenceEqual r1.WorkflowIdentifiers r2.WorkflowIdentifiers "WorkflowIdentifiers"
            Expect.sequenceEqual r1.Tables r2.Tables "Tables"
            Expect.sequenceEqual r1.Performers r2.Performers "Performers"
            Expect.sequenceEqual r1.Comments r2.Comments "Comments"
        |> Some

open Helper

let private test_runJsonTesting =
    createBaseJsonTests
        "run-json"
        create_filled_run
        ArcRun.toJsonString
        ArcRun.fromJsonString
        None
        compare

let private test_runCompressedJsonTesting =
    createBaseJsonTests
        "run-compressed-json"
        create_filled_run
        ArcRun.toCompressedJsonString
        ArcRun.fromCompressedJsonString
        None
        compare

let main = testList "Run" [
    test_runJsonTesting
    test_runCompressedJsonTesting
]
