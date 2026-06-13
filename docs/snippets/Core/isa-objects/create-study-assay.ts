// docs:begin
import { ArcInvestigation } from "@nfdi4plants/arctrl";

const investigation = new ArcInvestigation(
  "Investigation-001",
  "Growth experiment",
);

const study = investigation.InitStudy("Study-001");

const assay = investigation.InitAssay("Assay-001", [study]);

study.Title = "Plant growth study";
assay.Title = "Phenotyping assay";
// docs:end

// docs:assert
if (investigation.StudyCount !== 1) {
  throw new Error(`Expected one study, got ${investigation.StudyCount}`);
}

if (investigation.AssayCount !== 1) {
  throw new Error(`Expected one assay, got ${investigation.AssayCount}`);
}

if (study.RegisteredAssayCount !== 1) {
  throw new Error(`Expected one registered assay, got ${study.RegisteredAssayCount}`);
}
// docs:endassert
