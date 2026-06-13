// docs:begin
import { ArcInvestigation, XlsxController } from "@nfdi4plants/arctrl";

const investigation = new ArcInvestigation(
  "Investigation-XLSX",
  "Spreadsheet IO example",
);

const xlsxPath =
  "docs/generated/snippets/IO/spreadsheet-io/isa.investigation.xlsx";

await XlsxController.Investigation.toXlsxFileAsync(xlsxPath, investigation);

const investigationAgain =
  await XlsxController.Investigation.fromXlsxFileAsync(xlsxPath);
// docs:end

// docs:assert
if (investigationAgain.Identifier !== "Investigation-XLSX") {
  throw new Error(
    `Expected identifier to survive xlsx file roundtrip, got ${investigationAgain.Identifier}`,
  );
}
// docs:endassert
