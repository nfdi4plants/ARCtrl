module TestObjects.CWL.CommandLineTool

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