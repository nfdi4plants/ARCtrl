module TestObjects.CWL.YAMLParameterFile

open ARCtrl.CWL

let yamlParameterFileContent ="""exampleKey: 1234
exampleKeyString: "abcdefg"
exampleFile:
  class: File
  path: ../examplePath
exampleDir:
  class: Directory
  path: ../examplePathDir
exampleList:
  - foo.txt
  - bar.dat
  - baz.txt"""

module File =
    let filePath = "data/examplePath"
    let fileClass = "File"
    let fileType = CWLType.file()

    let fileParameterReference = CWLParameterReference(key = Inputs.File.inputFileName, values = ResizeArray [filePath])

    let fileParameterReferenceWithType = CWLParameterReference(key = Inputs.File.inputFileName, values = ResizeArray [filePath], type_ = fileType)

module String =

    let stringValue = "abcdefg"
    let stringParameterReference = CWLParameterReference(key = Inputs.String.inputStringName, values = ResizeArray [stringValue])