# docs:begin
from arctrl import DataContext, Datamap

leaf_area = DataContext(
    None,
    "assays/Measurement/dataset/results.csv",
    None,
    None,
    None,
    None,
    None,
    None,
    "leaf area",
    "Leaf area measurements exported from image analysis.",
)

datamap = Datamap([leaf_area])

first_context = datamap.GetDataContext(0)
# docs:end

# docs:assert
if len(datamap.DataContexts) != 1:
    raise Exception(f"Expected one data context, got {len(datamap.DataContexts)}")

if first_context.Name != "assays/Measurement/dataset/results.csv":
    raise Exception("Expected the data context path to be stored as its name.")
# docs:endassert
