# docs:begin
from arctrl import ArcAssay, DataContext, Datamap

leaf_area = DataContext(
    name="assays/Measurement/dataset/results.csv",
    label="leaf area",
)

datamap = Datamap([leaf_area])

assay = ArcAssay("Measurement")

assay.Datamap = datamap
# docs:end

# docs:assert
if assay.Datamap is None:
    raise Exception("Expected datamap to be attached to the assay.")

if len(assay.Datamap.DataContexts) != 1:
    raise Exception(
        f"Expected one attached data context, got {len(assay.Datamap.DataContexts)}"
    )
# docs:endassert
