module TestObjects.CWL.Requirements

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
      - envName: TEST
        envValue: "false"
  - class: NetworkAccess
    networkAccess: true"""