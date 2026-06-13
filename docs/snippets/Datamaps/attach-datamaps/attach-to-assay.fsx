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

let assay =
    ArcAssay("Measurement")

assay.Datamap <- Some datamap
// docs:end

// docs:assert
match assay.Datamap with
| Some attached when attached.DataContexts.Count = 1 -> ()
| Some attached -> failwithf "Expected one attached data context, got %i" attached.DataContexts.Count
| None -> failwith "Expected datamap to be attached to the assay."
// docs:endassert
