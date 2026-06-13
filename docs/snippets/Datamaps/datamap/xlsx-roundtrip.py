# docs:begin
from arctrl import DataContext, Datamap, XlsxController

leaf_area = DataContext(
    name="assays/Measurement/dataset/results.csv",
    label="leaf area",
)

datamap = Datamap([leaf_area])

workbook = XlsxController.Datamap().to_fs_workbook(datamap)

datamap_again = XlsxController.Datamap().from_fs_workbook(workbook)
# docs:end

# docs:assert
if len(datamap_again.DataContexts) != 1:
    raise Exception(
        "Expected one data context after xlsx workbook roundtrip, "
        f"got {len(datamap_again.DataContexts)}"
    )
# docs:endassert
