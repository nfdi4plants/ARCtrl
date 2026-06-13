// docs:begin
import { DataContext, Datamap, XlsxController } from "@nfdi4plants/arctrl";

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

const workbook = XlsxController.Datamap.toFsWorkbook(datamap);

const datamapAgain = XlsxController.Datamap.fromFsWorkbook(workbook);
// docs:end

// docs:assert
if (datamapAgain.DataContexts.length !== 1) {
  throw new Error(
    `Expected one data context after xlsx workbook roundtrip, got ${datamapAgain.DataContexts.length}`,
  );
}
// docs:endassert
