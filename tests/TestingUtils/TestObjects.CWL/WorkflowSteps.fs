module TestObjects.CWL.WorkflowSteps

let workflowStepsFileContent ="""steps:
  MzMLToMzlite:
    run: ./runs/MzMLToMzlite/proteomiqon-mzmltomzlite.cwl
    in:
      stageDirectory: stage
      inputDirectory:
        source: inputMzML
    out: [dir]
  PeptideSpectrumMatching:
    run: ./runs/PeptideSpectrumMatching/proteomiqon-peptidespectrummatching.cwl
    in:
      stageDirectory:
        source: stage
      inputDirectory:
        source:
          - MzMLToMzlite/dir1
          - MzMLToMzlite/dir2
        linkMerge: merge_flattened
      parallelismLevel:
        default: 8
      outputDirectory:
        valueFrom: "output" 
    out: [dir1, dir2]"""


/// A workflow with a step whose input sets every field of a step input, for the tests of equality and hash.
module AllFields =

    let workflowFile = """cwlVersion: v1.2
class: Workflow
requirements:
  StepInputExpressionRequirement: {}
  MultipleInputFeatureRequirement: {}
inputs: {}
outputs: {}
steps:
  s:
    run: t.cwl
    in:
      x:
        source: [a, b]
        default: dflt
        valueFrom: $(self)
        linkMerge: merge_flattened
        pickValue: first_non_null
        doc: first
        loadContents: false
        loadListing: no_listing
        label: lab
        arc:note: n1
    out: []
"""
