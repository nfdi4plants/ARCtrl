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

    let inputFileType = CWLType.File

    let inputFilePrefix = "-f"

    let inputFilePosition = 1

    let inputFileBinding = InputBinding.create(position = BindingPosition.Number inputFilePosition, prefix = inputFilePrefix)

    let inputFirstArg = CWLInput(inputFileName, inputFileType, inputBinding = inputFileBinding)


module String =

    let inputStringName = "secondArg"

    let inputStringType = CWLType.String

    let inputStringPosition = 2

    let inputStringBinding = InputBinding.create(position = BindingPosition.Number inputStringPosition)

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
    

/// A tool with the forms of the format field of an input: a name, a list of names, a list of one name and the empty
/// list. The names use the prefix edam, which the document declares as a namespace.
let formatFormsFileContent = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
$namespaces:
  edam: http://edamontology.org/
inputs:
  single:
    type: File
    format: edam:format_2330
  several:
    type: File
    format: [edam:format_2330, edam:format_1930]
  one_in_list:
    type: File
    format: [edam:format_2330]
  empty:
    type: File
    format: []
outputs: []"""
