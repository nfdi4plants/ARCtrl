module TestObjects.CWL.Workflow

let workflowFile ="""cwlVersion: v1.2
class: Workflow

requirements:
  - class: MultipleInputFeatureRequirement

inputs:
  cores: int
  db: File
  stage: Directory
  outputMzML: Directory
  outputPSM: Directory
  inputMzML: Directory
  paramsMzML: File
  paramsPSM: File
  sampleRecord:
    type:
      type: array
      items:
        type: record
        fields:
          readsOfOneSample:
            type: File[]
          sampleName:
            type: string?

steps:
  MzMLToMzlite:
    run: ./runs/MzMLToMzlite/proteomiqon-mzmltomzlite.cwl
    in:
      stageDirectory: stage
      inputDirectory: inputMzML
      params: paramsMzML
      outputDirectory: outputMzML
      parallelismLevel: cores
    out: [dir]
  PeptideSpectrumMatching:
    run: ./runs/PeptideSpectrumMatching/proteomiqon-peptidespectrummatching.cwl
    in:
      stageDirectory: stage
      inputDirectory:
        source:
          - MzMLToMzlite/dir1
          - MzMLToMzlite/dir2
        linkMerge: merge_flattened
      database: db
      params: paramsPSM
      outputDirectory: outputPSM
      parallelismLevel: cores
    out: [dir]

outputs:
  mzlite:
    type: Directory
    outputSource: MzMLToMzlite/dir
  psm:
    type: Directory
    outputSource: PeptideSpectrumMatching/dir"""

let workflowWithExtendedStepFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    label: Example step
    doc: Step docs
    scatter: input1
    scatterMethod: dotproduct
    in:
      in1:
        source: input1
        loadContents: true
        loadListing: deep_listing
        label: Input label
        linkMerge: merge_nested
    out:
      - id: out"""

let workflowWithInlineRunCommandLineToolFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: inlineStep/out
steps:
  inlineStep:
    run:
      class: CommandLineTool
      cwlVersion: v1.2
      baseCommand: echo
      inputs: {}
      outputs:
        out: string
    in: {}
    out: [out]"""

let workflowWithInputArrayStepFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      - id: in1
        source: input1
        label: Input in array syntax
    out: [out]"""

