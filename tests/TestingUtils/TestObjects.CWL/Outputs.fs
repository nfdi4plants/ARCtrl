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

    let outputCSVGlob = {Glob = Some outputCSVGlobStr}

    let outputCSV = CWLOutput(outputCSVName, outputCSVType, outputBinding = outputCSVGlob)
