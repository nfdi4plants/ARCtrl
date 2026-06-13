// docs:begin
import { ArcRun, ArcWorkflow, JsonController } from "@nfdi4plants/arctrl";

const workflow = new ArcWorkflow(
  "Workflow-001",
  "Image analysis workflow",
);

const run = new ArcRun(
  "Run-001",
  "Image analysis run",
  undefined,
  undefined,
  undefined,
  undefined,
  [workflow.Identifier],
);

const runJson = JsonController.Run.toJsonString(run, 2);

const runAgain = JsonController.Run.fromJsonString(runJson);
// docs:end

// docs:assert
if (runAgain.Identifier !== "Run-001") {
  throw new Error(`Expected run identifier to survive JSON roundtrip, got ${runAgain.Identifier}`);
}

if (runAgain.WorkflowIdentifierCount !== 1) {
  throw new Error(`Expected one workflow identifier, got ${runAgain.WorkflowIdentifierCount}`);
}
// docs:endassert
