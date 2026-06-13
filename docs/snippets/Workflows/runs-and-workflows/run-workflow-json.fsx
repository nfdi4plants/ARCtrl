#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let workflow =
    ArcWorkflow(
        "Workflow-001",
        title = "Image analysis workflow"
    )

let run =
    ArcRun(
        "Run-001",
        title = "Image analysis run",
        workflowIdentifiers = ResizeArray [| workflow.Identifier |]
    )

let runJson =
    JsonController.Run.toJsonString(run, 2)

let runAgain =
    JsonController.Run.fromJsonString(runJson)
// docs:end

// docs:assert
if runAgain.Identifier <> "Run-001" then
    failwithf "Expected run identifier to survive JSON roundtrip, got %s" runAgain.Identifier

if runAgain.WorkflowIdentifierCount <> 1 then
    failwithf "Expected one workflow identifier, got %i" runAgain.WorkflowIdentifierCount
// docs:endassert
