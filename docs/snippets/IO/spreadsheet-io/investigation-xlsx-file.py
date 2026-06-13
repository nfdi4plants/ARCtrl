# docs:begin
from pathlib import Path

from arctrl import ArcInvestigation, XlsxController

investigation = ArcInvestigation(
    "Investigation-XLSX",
    "Spreadsheet IO example",
)

xlsx_path = Path(__file__).with_name("isa.investigation.xlsx")

XlsxController.Investigation().to_xlsx_file(str(xlsx_path), investigation)

investigation_again = XlsxController.Investigation().from_xlsx_file(str(xlsx_path))
# docs:end

# docs:assert
if investigation_again.Identifier != "Investigation-XLSX":
    raise Exception(
        "Expected identifier to survive xlsx file roundtrip, "
        f"got {investigation_again.Identifier}"
    )
# docs:endassert
