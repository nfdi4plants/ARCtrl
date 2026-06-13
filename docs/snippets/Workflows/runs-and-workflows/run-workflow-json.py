# docs:begin
from arctrl import ArcRun, ArcWorkflow, JsonController

workflow = ArcWorkflow(
    "Workflow-001",
    "Image analysis workflow",
)

run = ArcRun(
    "Run-001",
    "Image analysis run",
    None,
    None,
    None,
    None,
    [workflow.Identifier],
)

run_json = JsonController.Run().to_json_string(run, 2)

run_again = JsonController.Run().from_json_string(run_json)
# docs:end

# docs:assert
if run_again.Identifier != "Run-001":
    raise Exception(
        f"Expected run identifier to survive JSON roundtrip, got {run_again.Identifier}"
    )

if run_again.WorkflowIdentifierCount != 1:
    raise Exception(
        f"Expected one workflow identifier, got {run_again.WorkflowIdentifierCount}"
    )
# docs:endassert
