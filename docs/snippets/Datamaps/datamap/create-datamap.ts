// docs:begin
import { DataContext, Datamap } from "@nfdi4plants/arctrl";

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
  "Leaf area measurements exported from image analysis.",
);

const datamap = new Datamap([leafArea]);

const firstContext = datamap.GetDataContext(0);
// docs:end

// docs:assert
if (datamap.DataContexts.length !== 1) {
  throw new Error(`Expected one data context, got ${datamap.DataContexts.length}`);
}

if (firstContext === undefined) {
  throw new Error("Expected the first data context to be readable.");
}
// docs:endassert
