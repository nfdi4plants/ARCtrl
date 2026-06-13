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

const xlsxPath =
  "docs/generated/snippets/Datamaps/datamap/datamap-file-roundtrip.xlsx";

await XlsxController.Datamap.toXlsxFileAsync(xlsxPath, datamap);

const datamapAgain = await XlsxController.Datamap.fromXlsxFileAsync(xlsxPath);
// docs:end

// docs:assert
if (datamapAgain.DataContexts.length !== 1) {
  throw new Error(
    `Expected one data context after xlsx file roundtrip, got ${datamapAgain.DataContexts.length}`,
  );
}
// docs:endassert
