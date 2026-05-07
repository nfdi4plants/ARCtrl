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


let specInputFieldsDecodeFileContent = """inputs:
  sample:
    label: Sample
    inputBinding:
      valueFrom: $(self.path)
      shellQuote: false
      prefix: --sample
      loadContents: true
      position: 1
    default: sample.txt
    type: File
    doc: Sample input docs
    loadListing: shallow_listing
    loadContents: true
    streamable: false
    format: edam:format_2330
    secondaryFiles: .bai"""
    