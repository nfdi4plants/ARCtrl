// docs:begin
import {
  ArcInvestigation,
  JsonController,
} from "@nfdi4plants/arctrl";

const investigation = new ArcInvestigation(
  "RoundtripInvestigation",
  "Roundtrip Example",
);

const json = JsonController.Investigation.toJsonString(investigation, 2);
const investigationAgain = JsonController.Investigation.fromJsonString(json);
// docs:end

// docs:assert
if (investigationAgain.Identifier !== "RoundtripInvestigation") {
  throw new Error(
    `Expected investigation identifier to survive JSON roundtrip, got ${investigationAgain.Identifier}`,
  );
}

if (investigationAgain.Title !== "Roundtrip Example") {
  throw new Error("Expected investigation title to survive JSON roundtrip.");
}
// docs:endassert
