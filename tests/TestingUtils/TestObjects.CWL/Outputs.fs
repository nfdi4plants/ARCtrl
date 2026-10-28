module TestObjects.CWL.Outputs

open ARCtrl.CWL

let outputsFileContent ="""outputs:
  output:
    type: File
    outputBinding:
      glob: ./arc/runs/fsResult1/result.csv
  example1:
    type: Directory
    outputBinding:
      glob: ./arc/runs/fsResult1/example.csv
  example2: Directory
  exampleArray1:
    type: File[]
    outputBinding:
      glob: ./arc/runs/fsResult1/example.csv
  exampleArray2:
    type:
      type: array
      items: File
    outputBinding:
      glob: ./arc/runs/fsResult1/example.csv"""


module CSV =

    let outputCSVName = "output"

    let outputCSVType = CWLType.File

    let outputCSVGlobStr = "*.csv"

    let outputCSVGlob = OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal outputCSVGlobStr))

    let outputCSV = CWLOutput(outputCSVName, outputCSVType, outputBinding = ListEntry.Value outputCSVGlob)


let specOutputFieldsDecodeFileContent = """outputs:
  result:
    label: Result
    outputBinding:
      outputEval: $(self[0])
      glob: "*.txt"
      loadListing: deep_listing
      loadContents: true
    secondaryFiles: .idx
    type: File
    outputSource: step/out
    doc: Result docs
    streamable: true
    format: edam:format_2330"""

/// A tool and a workflow with one output that sets every field of an output, for the tests of equality and hash.
module AllFields =

    let toolFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs: {}
outputs:
  o:
    type: File
    outputBinding:
      glob: "*.txt"
    label: lab
    secondaryFiles: .idx
    streamable: true
    doc: first
    format: edam:format_1
    arc:note: n1
"""

    let workflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  o:
    type: File
    outputSource: step/out
steps: {}
"""
