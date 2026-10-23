module TestObjects.CWL.Operation

let minimalOperationFile = """cwlVersion: v1.2
class: Operation
inputs:
  input: string
outputs:
  output: string"""

let operationWithIntentFile = """cwlVersion: v1.2
class: Operation
intent:
  - orchestration
  - wiring
inputs:
  input:
    type: string
outputs:
  output:
    type: string"""

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

/// Ports of the type Any in every place that a type name stands.
module AnyType =

    let cwlFile = """cwlVersion: v1.2
class: Operation
inputs:
  anything: Any
  maybe_anything: Any?
  anything_list: Any[]
  anything_or_text: [Any, string]
  listed:
    type:
      type: array
      items: Any
outputs:
  anything_out: Any
  maybe_out: Any?
  list_out: Any[]
"""
