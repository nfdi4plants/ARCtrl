module TestObjects.CWL.WorkflowGraph

let twoStepWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs:
  x: string
outputs:
  y:
    type: string
    outputSource: stepB/out
steps:
  stepA:
    run: ./a.cwl
    in:
      in1: x
    out: [out]
  stepB:
    run: ./b.cwl
    in:
      in1: stepA/out
    out: [out]"""

let fanInWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  stepA:
    run: ./a.cwl
    in: {}
    out: [out1]
  stepB:
    run: ./b.cwl
    in: {}
    out: [out2]
  stepC:
    run: ./c.cwl
    in:
      in1:
        source: [stepA/out1, stepB/out2]
    out: [out]"""

let singleStepToolRunWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  step1:
    run: ./tool.cwl
    in: {}
    out: [out]"""

let sharedRunPathWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  outA:
    type: string
    outputSource: stepA/out
  outB:
    type: string
    outputSource: stepB/out
steps:
  stepA:
    run: ./tool.cwl
    in: {}
    out: [out]
  stepB:
    run: ./tool.cwl
    in: {}
    out: [out]"""

let singleStepMissingRunWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  step1:
    run: ./missing.cwl
    in: {}
    out: [out]"""

let singleStepFragmentRunWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  step1:
    run: ./tools/my-tool.cwl#main
    in: {}
    out: [out]"""

let singleStepQueryRunWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  step1:
    run: ./tools/my-tool.cwl?version=1
    in: {}
    out: [out]"""

let stepOutputMixedWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  step1:
    run: ./tool.cwl
    in: {}
    out:
      - out1
      - id: out2"""

let malformedSourceWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  step1:
    run: ./tool.cwl
    in:
      in1: //
    out: [out]"""

let missingStepOutputReferenceWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  step1:
    run: ./tool.cwl
    in:
      in1: NonExistentStep/out
    out: [out]"""

let invalidWorkflowOutputSourceWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  out:
    type: string
    outputSource: BadStep/badPort
steps:
  step1:
    run: ./tool.cwl
    in: {}
    out: [out]"""

let visualizationSimpleWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs:
  x: string
outputs:
  y:
    type: string
    outputSource: step2/out
steps:
  step1:
    run: ./tool1.cwl
    in:
      in1: x
    out: [out]
  step2:
    run: ./tool2.cwl
    in:
      in1: step1/out
    out: [out]"""

let visualizationSingleStepResolvedWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs:
  x: string
outputs:
  y:
    type: string
    outputSource: s/out
steps:
  s:
    run: ./tool.cwl
    in:
      in1: x
    out: [out]"""

let passThroughOutputWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs:
  x: string
outputs:
  y:
    type: string
    outputSource: x
steps: {}"""


/// A packed document in the form of cwltool --pack: every id is written in full behind the id of its process.
let packedWithFullIdsFile = """cwlVersion: v1.2
$graph:
  - class: CommandLineTool
    id: "#shout.cwl"
    baseCommand: echo
    stdout: shout.txt
    inputs:
      - id: "#shout.cwl/msg"
        type: string
        inputBinding:
          position: 1
    outputs:
      - id: "#shout.cwl/out"
        type: File
        outputBinding:
          glob: shout.txt
  - class: Workflow
    id: "#main"
    requirements:
      - class: StepInputExpressionRequirement
    inputs:
      - id: "#main/msg"
        type: string
    outputs:
      - id: "#main/result"
        type: File
        outputSource: "#main/second/out"
    steps:
      - id: "#main/first"
        run: "#shout.cwl"
        in:
          - id: "#main/first/msg"
            source: "#main/msg"
        out:
          - "#main/first/out"
      - id: "#main/second"
        run: "#shout.cwl"
        in:
          - id: "#main/second/msg"
            source: "#main/first/out"
            valueFrom: done
        out:
          - "#main/second/out"
"""
