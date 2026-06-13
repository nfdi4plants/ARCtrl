# docs:begin
from pathlib import Path

from arctrl import DataContext, Datamap, XlsxController

leaf_area = DataContext(
    name="assays/Measurement/dataset/results.csv",
    label="leaf area",
)

datamap = Datamap([leaf_area])

xlsx_path = Path(__file__).with_name("datamap-file-roundtrip.xlsx")

XlsxController.Datamap().to_xlsx_file(str(xlsx_path), datamap)

datamap_again = XlsxController.Datamap().from_xlsx_file(str(xlsx_path))
# docs:end

# docs:assert
if len(datamap_again.DataContexts) != 1:
    raise Exception(
        "Expected one data context after xlsx file roundtrip, "
        f"got {len(datamap_again.DataContexts)}"
    )
# docs:endassert
