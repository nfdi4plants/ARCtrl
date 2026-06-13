#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let investigation =
    ArcInvestigation(
        "Investigation-ISAJSON",
        title = "ISA-JSON example"
    )

let isaJson =
    JsonController.Investigation.toISAJsonString(investigation, 2)

let investigationAgain =
    JsonController.Investigation.fromISAJsonString(isaJson)
// docs:end

// docs:assert
if investigationAgain.Identifier <> "Investigation-ISAJSON" then
    failwithf "Expected identifier to survive ISA-JSON roundtrip, got %s" investigationAgain.Identifier
// docs:endassert
