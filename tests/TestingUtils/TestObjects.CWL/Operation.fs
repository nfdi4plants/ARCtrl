module TestObjects.CWL.Operation

let minimalOperationFile = """cwlVersion: v1.2
class: Operation
inputs:
  input: string
outputs:
  output: string"""

let operationWithRequirementsAndMetadataFile = """cwlVersion: v1.2
class: Operation
label: Operation label
doc: Operation docs
hints:
  - class: StepInputExpressionRequirement
requirements:
  - class: InlineJavascriptRequirement
inputs:
  input:
    type: string
outputs:
  output:
    type: string
customKey: custom-value"""
