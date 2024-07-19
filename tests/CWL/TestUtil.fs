module TestUtil

let outputs ="""outputs:
  output:
    type: File
    outputBinding:
      glob: ./arc/runs/fsResult1/result.csv
  example:
    type: Directory
    outputBinding:
      glob: ./arc/runs/fsResult1/example.csv
  exampleArray1:
    type: File[]
    outputBinding:
      glob: ./arc/runs/fsResult1/example.csv
  exampleArray2:
    type:
      type: array
      items: File
    outputBinding:
      glob: ./arc/runs/fsResult1/example.csv"""

let inputs ="""inputs:
  arcDirectory:
    type: Directory
  firstArg:
    type: File
    inputBinding:
      position: 1
      prefix: --example
  secondArg:
    type: string
    inputBinding:
      position: 2
      separate: false"""

let requirements ="""requirements:
  - class: DockerRequirement
    dockerImageId: "devcontainer"
    dockerFile: {$include: "FSharpArcCapsule/Dockerfile"}
  - class: InitialWorkDirRequirement
    listing:
      - entryname: arc
        entry: $(inputs.arcDirectory)
        writable: true
  - class: EnvVarRequirement
    envDef:
      - envName: DOTNET_NOLOGO
        envValue: "true"
  - class: NetworkAccess
    networkAccess: true"""

let cwl ="""cwlVersion: v1.2
class: CommandLineTool
hints:
  - class: DockerRequirement
    dockerPull: mcr.microsoft.com/dotnet/sdk:6.0
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entryname: script.fsx
        entry:
          $include: script.fsx
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