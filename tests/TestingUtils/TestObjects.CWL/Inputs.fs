module TestObjects.CWL.Inputs

let inputsFileContent ="""inputs:
  arcDirectory: Directory
  firstArg:
    type: File
    inputBinding:
      position: 1
      prefix: --example
  secondArg:
    type: string
    inputBinding:
      position: 2
      separate: false"""