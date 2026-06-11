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
