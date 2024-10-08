module TestObjects.CWL.Outputs

let outputs ="""outputs:
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