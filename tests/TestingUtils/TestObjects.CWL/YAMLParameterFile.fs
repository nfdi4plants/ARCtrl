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

    let fileParameterReference = CWLParameterReference(key = Inputs.File.inputFileName, value = CWLParameterValue.File (FileInstance(path = filePath)))

module String =

    let stringValue = "abcdefg"
    let stringParameterReference = CWLParameterReference(key = Inputs.String.inputStringName, values = ResizeArray [stringValue])

module Structured =

    let nestedFileArray = """sampleRecordFiles:
  - - class: File
      path: ../../assays/RNASeq/dataset/DB_097.fastq.gz
      format: edam:format_1930
  - - class: File
      path: ../../assays/RNASeq/dataset/DB_163.fastq.gz
      format: edam:format_1930"""

    let arrayOfRecords = """sampleRecords:
  - name: DB_097
    reads:
      - class: File
        path: ../../assays/RNASeq/dataset/DB_097.fastq.gz
  - name: DB_163
    reads:
      - class: File
        path: ../../assays/RNASeq/dataset/DB_163.fastq.gz"""

    let emptyArray = """sampleRecordFiles: []"""

module CoreSchemaScalars =

    let quotedAndPlainScalars = """quotedBool: "true"
plainBool: true
quotedInt: '12'
plainInt: 12
quotedNull: "null"
plainNull: null
quotedFloat: "1.5"
plainFloat: 1.5
plainInfinity: .inf
capitalBool: True
upperBool: FALSE
tilde: ~
capitalNull: Null
nanWord: NaN
infinityWord: Infinity
"""
