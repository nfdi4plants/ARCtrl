// docs:begin
import { ArcInvestigation, JsonController } from "@nfdi4plants/arctrl";

const investigation = new ArcInvestigation(
  "Investigation-ISAJSON",
  "ISA-JSON example",
);

const isaJson =
  JsonController.Investigation.toISAJsonString(investigation, 2);

const investigationAgain =
  JsonController.Investigation.fromISAJsonString(isaJson);
// docs:end

// docs:assert
if (investigationAgain.Identifier !== "Investigation-ISAJSON") {
  throw new Error(
    `Expected identifier to survive ISA-JSON roundtrip, got ${investigationAgain.Identifier}`,
  );
}
// docs:endassert
