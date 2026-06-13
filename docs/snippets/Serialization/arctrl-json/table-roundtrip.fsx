#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let investigation =
    ArcInvestigation(
        "RoundtripInvestigation",
        title = "Roundtrip Example"
    )

let json = JsonController.Investigation.toJsonString(investigation, 2)
let investigationAgain = JsonController.Investigation.fromJsonString(json)
// docs:end

// docs:assert
if investigationAgain.Identifier <> "RoundtripInvestigation" then
    failwithf "Expected investigation identifier to survive JSON roundtrip, got %s" investigationAgain.Identifier

if investigationAgain.Title <> Some "Roundtrip Example" then
    failwith "Expected investigation title to survive JSON roundtrip."
// docs:endassert
