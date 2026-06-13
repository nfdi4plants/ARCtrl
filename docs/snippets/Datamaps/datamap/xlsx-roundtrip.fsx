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

let workbook =
    XlsxController.Datamap.toFsWorkbook(datamap)

let datamapAgain =
    XlsxController.Datamap.fromFsWorkbook(workbook)
// docs:end

// docs:assert
if datamapAgain.DataContexts.Count <> 1 then
    failwithf "Expected one data context after xlsx workbook roundtrip, got %i" datamapAgain.DataContexts.Count
// docs:endassert
