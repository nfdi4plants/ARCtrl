module TestObjects.CWL.RunWorkflowReference

let workflowCwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: Tool/out
steps:
  Tool:
    run: ./tool.cwl
    in: {}
    out: [out]"""

let workflowToolText = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs:
  out: string"""

let runCwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: Workflow/out
steps:
  Workflow:
    run: ../../workflows/Proteomics/workflow.cwl
    in: {}
    out: [out]"""

/// A workflow whose only step runs the file of the workflow itself.
let selfReferenceText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  Again:
    run: run.cwl
    in: {}
    out: []"""

module CycleSafe =

    let workflowACwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  out:
    type: string
    outputSource: B/out
steps:
  B:
    run: ./b.cwl
    in: {}
    out: [out]"""

    let workflowBCwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  out:
    type: string
    outputSource: A/out
steps:
  A:
    run: ./a.cwl
    in: {}
    out: [out]"""

    let runCwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: A/out
steps:
  A:
    run: ./a.cwl
    in: {}
    out: [out]"""

module PartialResolution =

    let runCwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: Missing/out
steps:
  Missing:
    run: ./missing.cwl
    in: {}
    out: [out]"""

module SharedToolPath =

    let workflowCwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  combined:
    type: string
    outputSource: StepA/out
steps:
  StepA:
    run: ./tool.cwl
    in: {}
    out: [out]
  StepB:
    run: ./tool.cwl
    in: {}
    out: [out]"""

    let runCwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: Workflow/combined
steps:
  Workflow:
    run: ../../workflows/Proteomics/workflow.cwl
    in: {}
    out: [combined]"""

module DeepSharedToolPath =

    let workflowACwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  out:
    type: string
    outputSource: NestedB/out
steps:
  NestedB:
    run: ./workflow-b.cwl
    in: {}
    out: [out]"""

    let workflowBCwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  out:
    type: string
    outputSource: NestedC/out
steps:
  NestedC:
    run: ./workflow-c.cwl
    in: {}
    out: [out]"""

    let workflowCCwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  out:
    type: string
    outputSource: SharedTool/out
steps:
  SharedTool:
    run: ./tool.cwl
    in: {}
    out: [out]"""

    let runCwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: DirectTool/out
steps:
  Nested:
    run: ../../workflows/Proteomics/workflow-a.cwl
    in: {}
    out: [out]
  DirectTool:
    run: ../../workflows/Proteomics/tool.cwl
    in: {}
    out: [out]"""


/// The layout of an ARC with a run that runs a workflow, which runs a tool. The tool reads a script file with $include.
module StandardLayout =

    let runCwlText = """cwlVersion: v1.2
class: Workflow
requirements:
  - class: SubworkflowFeatureRequirement
inputs:
  msg: string
outputs: []
steps:
  main:
    run: ../../workflows/Proteomics/workflow.cwl
    in:
      msg: msg
    out: []"""

    let workflowCwlText = """cwlVersion: v1.2
class: Workflow
inputs:
  msg: string
outputs: []
steps:
  shout:
    run: shout.cwl
    in:
      msg: msg
    out: []"""

    let shoutCwlText = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entryname: script.sh
        entry:
          $include: script.sh
inputs:
  msg: string
outputs: []"""

/// Two workflows in different directories whose steps run a file of the same relative path, and a run that runs both.
module SameRelativePathInTwoDirectories =

    let workflowCwlText = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps:
  shout:
    run: shout.cwl
    in: {}
    out: []"""

    let echoCwlText = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs: {}"""

    let catCwlText = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: cat
inputs: {}
outputs: {}"""

    let runCwlText = """cwlVersion: v1.2
class: Workflow
requirements:
  - class: SubworkflowFeatureRequirement
inputs: {}
outputs: {}
steps:
  first:
    run: ../../workflows/First/workflow.cwl
    in: {}
    out: []
  second:
    run: ../../workflows/Second/workflow.cwl
    in: {}
    out: []"""
