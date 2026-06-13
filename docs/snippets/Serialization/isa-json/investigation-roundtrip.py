# docs:begin
from arctrl import ArcInvestigation, JsonController

investigation = ArcInvestigation(
    "Investigation-ISAJSON",
    "ISA-JSON example",
)

isa_json = JsonController.Investigation().to_isajson_string(investigation, 2)

investigation_again = JsonController.Investigation().from_isajson_string(isa_json)
# docs:end

# docs:assert
if investigation_again.Identifier != "Investigation-ISAJSON":
    raise Exception(
        "Expected identifier to survive ISA-JSON roundtrip, "
        f"got {investigation_again.Identifier}"
    )
# docs:endassert
