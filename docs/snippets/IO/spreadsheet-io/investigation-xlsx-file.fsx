#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let investigation =
    ArcInvestigation(
        "Investigation-XLSX",
        title = "Spreadsheet IO example"
    )

let xlsxPath =
    System.IO.Path.Combine(__SOURCE_DIRECTORY__, "isa.investigation.xlsx")

XlsxController.Investigation.toXlsxFile(xlsxPath, investigation)

let investigationAgain =
    XlsxController.Investigation.fromXlsxFile(xlsxPath)
// docs:end

// docs:assert
if investigationAgain.Identifier <> "Investigation-XLSX" then
    failwithf "Expected identifier to survive xlsx file roundtrip, got %s" investigationAgain.Identifier
// docs:endassert
