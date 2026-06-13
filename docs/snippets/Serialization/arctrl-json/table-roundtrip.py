# docs:begin
from arctrl import ArcInvestigation, JsonController

investigation = ArcInvestigation(
    "RoundtripInvestigation",
    "Roundtrip Example",
)

json = JsonController.Investigation().to_json_string(investigation, 2)
investigation_again = JsonController.Investigation().from_json_string(json)
# docs:end

# docs:assert
if investigation_again.Identifier != "RoundtripInvestigation":
    raise Exception(
        "Expected investigation identifier to survive JSON roundtrip, "
        f"got {investigation_again.Identifier}"
    )

if investigation_again.Title != "Roundtrip Example":
    raise Exception("Expected investigation title to survive JSON roundtrip.")
# docs:endassert
