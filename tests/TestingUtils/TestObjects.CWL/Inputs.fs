module TestObjects.CWL.Inputs

open ARCtrl.CWL

let inputsFileContent ="""inputs:
  arcDirectory: Directory
  firstArg:
    type: File
    inputBinding:
      position: 1
      prefix: --example
  argOptional:
    type: File?
  argOptionalMap: File[]?
  secondArg:
    type: string
    inputBinding:
      position: 2
      separate: false"""

module File =

    let inputFileName = "firstArg"

    let inputFileType = CWLType.file()

    let inputFilePrefix = "-f"

    let inputFilePosition = 1

    let inputFileBinding = InputBinding.create(position = inputFilePosition, prefix = inputFilePrefix)

    let inputFirstArg = CWLInput(inputFileName, inputFileType, inputBinding = inputFileBinding)


module String =

    let inputStringName = "secondArg"

    let inputStringType = CWLType.String

    let inputStringPosition = 2

    let inputStringBinding = InputBinding.create(position = inputStringPosition)

    let inputSecondArg = CWLInput(inputStringName, inputStringType, inputBinding = inputStringBinding)

    