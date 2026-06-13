# docs:begin
from arctrl import ArcInvestigation

investigation = ArcInvestigation(
    "Investigation-001",
    "Growth experiment",
)

study = investigation.InitStudy("Study-001")

assay = investigation.InitAssay("Assay-001", [study])

study.Title = "Plant growth study"
assay.Title = "Phenotyping assay"
# docs:end

# docs:assert
if investigation.StudyCount != 1:
    raise Exception(f"Expected one study, got {investigation.StudyCount}")

if investigation.AssayCount != 1:
    raise Exception(f"Expected one assay, got {investigation.AssayCount}")

if study.RegisteredAssayCount != 1:
    raise Exception(f"Expected one registered assay, got {study.RegisteredAssayCount}")
# docs:endassert
