#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let leafArea =
    DataContext(
        name = "assays/Measurement/dataset/results.csv",
        label = "leaf area",
        description = "Leaf area measurements exported from image analysis."
    )

let datamap =
    Datamap(ResizeArray [| leafArea |])

let firstContext = datamap.GetDataContext(0)
// docs:end

// docs:assert
if datamap.DataContexts.Count <> 1 then
    failwithf "Expected one data context, got %i" datamap.DataContexts.Count

if firstContext.Name <> Some "assays/Measurement/dataset/results.csv" then
    failwith "Expected the data context path to be stored as its name."
// docs:endassert
