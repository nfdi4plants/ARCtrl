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

let workflowWithStepsArrayFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  - id: step1
    run: ./tool.cwl
    when: $(inputs.input1 != null)
    in:
      - id: in1
        source: input1
        pickValue: first_non_null
        doc: Input docs
        default:
          type: string
          value: fallback
    out:
      - id: out"""

let workflowWithInvalidScatterMethodFile = """cwlVersion: v1.2
class: Workflow
inputs: { input1: string }
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    scatter: input1
    scatterMethod: invalid_scatter
    in:
      in1: input1
    out: [out]"""

let workflowWithInlineExpressionToolFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run:
      class: ExpressionTool
      cwlVersion: v1.2
      inputs: {}
      outputs: {}
      expression: $(null)
    in: {}
    out: [out]"""

let workflowWithNoStepsFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps: {}"""

let workflowWithInlineRunWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: outer/out
steps:
  outer:
    run:
      class: Workflow
      cwlVersion: v1.2
      inputs: {}
      outputs:
        out:
          type: string
          outputSource: inner/out
      steps:
        inner:
          run: echo.cwl
          in: {}
          out: [out]
    in: {}
    out: [out]"""

let workflowWithInvalidPickValueFile = """cwlVersion: v1.2
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
      in1:
        source: input1
        pickValue: invalid_pick
    out: [out]"""

let workflowWithPickValueMethodFile (pickValue: string) = $"""cwlVersion: v1.2
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
      in1:
        source: input1
        pickValue: {pickValue}
    out: [out]"""

let workflowWithScatterMethodFile (scatterMethod: string) = $"""cwlVersion: v1.2
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
    scatter: in1
    scatterMethod: {scatterMethod}
    in:
      in1:
        source: input1
    out: [out]"""

let workflowWithStructuredArrayDefaultFile = """cwlVersion: v1.2
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
      in1:
        source: input1
        default: [1, 2, 3]
    out: [out]"""

let workflowWithWhenExpressionFile (whenExpression: string) = $"""cwlVersion: v1.2
class: Workflow
inputs:
  name: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    when: "{whenExpression}"
    in:
      in1: name
    out: [out]"""

let workflowWithScatterAndScatterMethodFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
  input2: int
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    scatter: [in1, in2]
    scatterMethod: flat_crossproduct
    in:
      in1: input1
      in2: input2
    out: [out]"""

let workflowWithStepLevelRequirementsAndHintsFile = """cwlVersion: v1.2
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
      in1: input1
    out: [out]
    hints:
      - class: StepInputExpressionRequirement
    requirements:
      - class: NetworkAccess
        networkAccess: true"""

