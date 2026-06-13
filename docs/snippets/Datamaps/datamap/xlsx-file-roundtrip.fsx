#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let leafArea =
    DataContext(
        name = "assays/Measurement/dataset/results.csv",
        label = "leaf area"
    )

let datamap =
    Datamap(ResizeArray [| leafArea |])

let xlsxPath =
    System.IO.Path.Combine(__SOURCE_DIRECTORY__, "datamap-file-roundtrip.xlsx")

XlsxController.Datamap.toXlsxFile(xlsxPath, datamap)

let datamapAgain =
    XlsxController.Datamap.fromXlsxFile(xlsxPath)
// docs:end

// docs:assert
if datamapAgain.DataContexts.Count <> 1 then
    failwithf "Expected one data context after xlsx file roundtrip, got %i" datamapAgain.DataContexts.Count
// docs:endassert
