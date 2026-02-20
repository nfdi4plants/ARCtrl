module TestObjects.CWL.ExpressionTool

let minimalExpressionToolFile = """cwlVersion: v1.2
class: ExpressionTool
inputs: {}
outputs: {}
expression: $(null)"""

let expressionToolWithIntentFile = """cwlVersion: v1.2
class: ExpressionTool
intent:
  - feature-generation
  - post-processing
inputs: {}
outputs:
  out: string
expression: $(null)"""

let expressionToolWithRequirementsFile ="""cwlVersion: v1.2
class: ExpressionTool
requirements:
  - class: InlineJavascriptRequirement
inputs:
  i: string
outputs:
  output: int
expression: "$({'output': (inputs.i == 'the-default' ? 1 : 2)})"
"""

let expressionToolWithMetadataFile = """cwlVersion: v1.2
class: ExpressionTool
label: Metadata test
doc: An expression tool with metadata
inputs:
  i: string
outputs:
  out: string
expression: "$({'out': inputs.i})"
customKey: custom-value"""

let expressionToolWithDefaultInputFile ="""cwlVersion: v1.2
class: ExpressionTool
requirements:
  - class: InlineJavascriptRequirement
inputs:
  i1:
    type: string
    default: "the-default"
outputs:
  output: int
expression: "$({'output': (inputs.i1 == 'the-default' ? 1 : 2)})"
"""

let expressionToolArrayOutputFile = """cwlVersion: v1.2
class: ExpressionTool
requirements:
  InlineJavascriptRequirement: {}
inputs:
  i:
    type: int
outputs:
  o:
    type: int[]
expression: >
  ${return {'o': Array.apply(null, {length: inputs.i}).map(Number.call, Number)};}"""

let expressionToolLoadContentsFile = """cwlVersion: v1.2
class: ExpressionTool
requirements:
  InlineJavascriptRequirement: {}
inputs:
  my_number:
    type: File
    loadContents: true
outputs:
  my_int: int
expression: |
  ${ return { "my_int": parseInt(inputs.my_number.contents) }; }"""

let expressionToolPoolOutRoundtripFile = """#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: ExpressionTool
id: "V_pool_out"
label: Returns the output directory named after "analysis", containing all input files and directories.
requirements:
  InlineJavascriptRequirement: {}
inputs:
  mount_dir:
    type: Directory
  file_single:
    type: File?
  file_array:
    type: File[]?
  directory_single:
    type: Directory?
  directory_array:
    type: Directory[]?
  newname:
    type: string?
outputs:
  pool_DIR:
    type: Directory
    doc: "Final analysis output folder"
expression: >
  ${ return (function() {
    function sanitize(entry) {
      var allowedFields = ['class', 'basename', 'location', 'listing'];
      var sanitized = {};
      for (var i = 0; i < allowedFields.length; i++) {
        var key = allowedFields[i];
        if (entry[key] !== undefined) sanitized[key] = entry[key];
      }
      return sanitized.class && sanitized.basename ? sanitized : null;
      return name.replace(/\.tiff$/, "").replace(/\.tif$/, "");
    }

    var outputList = [];
    if (inputs.directory_single) outputList.push(sanitize(inputs.directory_single));
    if (inputs.file_single) outputList.push(sanitize(inputs.file_single));

    return {
      pool_DIR: { class: "Directory", basename: inputs.newname || "analysis", listing: outputList }
    };
  })(); }"""
let workflowWithInlineExpressionToolChainFile = """cwlVersion: v1.2
class: Workflow
requirements:
  InlineJavascriptRequirement: {}
inputs:
  i: int
outputs:
  o:
    type: int
    outputSource: step3/o
steps:
  step1:
    in:
      i: i
    out: [o]
    run:
      class: ExpressionTool
      inputs:
        i:
          type: int
      outputs:
        o:
          type: int[]
      expression: >
        ${return {'o': Array.apply(null, {length: inputs.i}).map(Number.call, Number)};}
  step2:
    in:
      i:
        source: step1/o
    out: [o]
    run:
      class: ExpressionTool
      inputs:
        i:
          type: int[]
      outputs:
        o:
          type: int[]
      expression: >
        ${return {'o': inputs.i.map(function(x) { return (x + 1) * 2; })};}
  step3:
    in:
      i:
        source: step2/o
    out: [o]
    run:
      class: ExpressionTool
      inputs:
        i:
          type: int[]
      outputs:
        o:
          type: int
      expression: >
        ${return {'o': inputs.i.reduce(function(a, b) { return a + b; })};}"""

let workflowWithLoadContentsExpressionToolFile = """cwlVersion: v1.2
class: Workflow
requirements:
  StepInputExpressionRequirement: {}
  InlineJavascriptRequirement: {}
inputs:
  my_file: File
steps:
  one:
    run:
      class: ExpressionTool
      requirements:
        InlineJavascriptRequirement: {}
      inputs:
        my_number: int
      outputs:
        my_int: int
      expression: |
        ${ return { "my_int": inputs.my_number }; }
    in:
      my_number:
        source: my_file
        loadContents: true
        valueFrom: $(parseInt(self.contents))
    out: [my_int]
outputs:
  my_int:
    type: int
    outputSource: one/my_int"""

let missingExpressionFieldFile = """cwlVersion: v1.2
class: ExpressionTool
inputs: {}
outputs: {}"""

let malformedExpressionToolClassFile = """cwlVersion: v1.2
class: ExpressionToolXYZ
inputs: {}
outputs: {}
expression: $(null)"""

let workflowWithMixedToolAndExpressionStepFile = """cwlVersion: v1.2
class: Workflow
inputs:
  x: string
outputs:
  result:
    type: string
    outputSource: expr/out
steps:
  tool:
    run: ./tool.cwl
    in:
      input1: x
    out: [out]
  expr:
    run:
      class: ExpressionTool
      cwlVersion: v1.2
      inputs:
        y: string
      outputs:
        out: string
      expression: "$({'out': inputs.y})"
    in:
      y: tool/out
    out: [out]"""
