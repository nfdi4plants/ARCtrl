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

    let fileParameterReference = CWLParameterReference.create(key = Inputs.File.inputFileName, values = ResizeArray [filePath])

module String =

    let stringValue = "abcdefg"
    let stringParameterReference = CWLParameterReference.create(key = Inputs.String.inputStringName, values = ResizeArray [stringValue])