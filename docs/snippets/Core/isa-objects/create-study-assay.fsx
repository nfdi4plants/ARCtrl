#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let investigation =
    ArcInvestigation(
        "Investigation-001",
        title = "Growth experiment"
    )

let study =
    investigation.InitStudy("Study-001")

let assay =
    investigation.InitAssay(
        "Assay-001",
        registerIn = ResizeArray [| study |]
    )

study.Title <- Some "Plant growth study"
assay.Title <- Some "Phenotyping assay"
// docs:end

// docs:assert
if investigation.StudyCount <> 1 then
    failwithf "Expected one study, got %i" investigation.StudyCount

if investigation.AssayCount <> 1 then
    failwithf "Expected one assay, got %i" investigation.AssayCount

if study.RegisteredAssayCount <> 1 then
    failwithf "Expected one registered assay, got %i" study.RegisteredAssayCount
// docs:endassert
