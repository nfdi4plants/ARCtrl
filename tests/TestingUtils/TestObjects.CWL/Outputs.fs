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

    let outputCSVType = CWLType.file()

    let outputCSVGlobStr = "*.csv"

    let outputCSVGlob = OutputBinding.create(glob = outputCSVGlobStr)

    let outputCSV = CWLOutput(outputCSVName, outputCSVType, outputBinding = outputCSVGlob)


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
