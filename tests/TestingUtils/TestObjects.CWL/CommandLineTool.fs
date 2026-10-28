module TestObjects.CWL.CommandLineTool

open ARCtrl.CWL

let cwlFile ="""cwlVersion: v1.2
class: CommandLineTool
hints:
  - class: DockerRequirement
    dockerPull: mcr.microsoft.com/dotnet/sdk:6.0
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entryname: script.fsx
        entry: {$include: script.fsx}
  - class: EnvVarRequirement
    envDef:
      - envName: DOTNET_NOLOGO
        envValue: "true"
  - class: NetworkAccess
    networkAccess: true
baseCommand: [dotnet, fsi, script.fsx]
inputs:
  firstArg:
    type: File
    inputBinding:
      position: 1
  secondArg:
    type: string
    inputBinding:
      position: 2

outputs:
  output:
    type: Directory
    outputBinding:
      glob: $(runtime.outdir)/.nuget
  output2:
    type: File
    outputBinding:
      glob: $(runtime.outdir)/*.csv"""

let cwlFileWithIntent = """cwlVersion: v1.2
class: CommandLineTool
intent: [classification, quality-control]
baseCommand: [echo]
inputs: {}
outputs:
  out: string"""


module Basic =

    let baseCommand = ResizeArray [(SchemaSaladString.Literal "dotnet"); (SchemaSaladString.Literal "fsi"); (SchemaSaladString.Literal "script.fsx")]

    let cwlTool =
        CWLToolDescription(
            outputs = ListEntry.ofValues [Outputs.CSV.outputCSV],
            inputs = ListEntry.ofValues [Inputs.File.inputFirstArg; Inputs.String.inputSecondArg])

    let processingUnit = CWLProcessingUnit.CommandLineTool cwlTool

module UnionArrayItems =

    let blockStyleCwlFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  mixed_input:
    type:
      type: array
      items:
        - string
        - File
outputs: {}
"""

module StreamTypes =

    /// A tool whose input has the type stdin and whose outputs have the types stdout and stderr.
    let cwlFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: sort
stdout: sorted.txt
stderr: sort.log
inputs:
  unsorted: stdin
outputs:
  sorted: stdout
  log:
    type: stderr
    doc: The standard error of the tool.
"""

/// Array types with fields besides their items.
module ArraySchemas =

    /// Arrays with a label, a doc, a name and an item separator, in type lists and as the whole type.
    let withMetadata = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  names:
    type:
      - "null"
      - type: array
        items: string
        label: names
        inputBinding:
          itemSeparator: ","
  counts:
    type:
      type: array
      items: int
      doc: the counts
  tags:
    type:
      - string
      - type: array
        items: string
        name: Tags
  plain: string[]
  maybe: int[]?
outputs: []
"""
