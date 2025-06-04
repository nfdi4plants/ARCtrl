module TestObjects.CWL.YAMLParameterFile

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