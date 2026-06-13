// docs:begin
import {
  ArcAssay,
  DataContext,
  Datamap,
} from "@nfdi4plants/arctrl";

const leafArea = new DataContext(
  undefined,
  "assays/Measurement/dataset/results.csv",
  undefined,
  undefined,
  undefined,
  undefined,
  undefined,
  undefined,
  "leaf area",
);

const datamap = new Datamap([leafArea]);

const assay = new ArcAssay("Measurement");

assay.Datamap = datamap;
// docs:end

// docs:assert
if (assay.Datamap === undefined) {
  throw new Error("Expected datamap to be attached to the assay.");
}

if (assay.Datamap.DataContexts.length !== 1) {
  throw new Error(
    `Expected one attached data context, got ${assay.Datamap.DataContexts.length}`,
  );
}
// docs:endassert
