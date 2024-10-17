module TestObjects.CWL.Requirements

let requirementsFileContent ="""requirements:
  - class: DockerRequirement
    dockerImageId: "devcontainer"
    dockerFile: {$include: "FSharpArcCapsule/Dockerfile"}
  - class: InitialWorkDirRequirement
    listing:
      - entryname: arc
        entry: $(inputs.arcDirectory)
        writable: true
      - entry: $(inputs.outputDirectory)
        writable: true
  - class: EnvVarRequirement
    envDef:
      - envName: DOTNET_NOLOGO
        envValue: "true"
      - envName: TEST
        envValue: "false"
  - class: SoftwareRequirement
    packages:
      - package: interproscan
        specs: [ "https://identifiers.org/rrid/RRID:SCR_005829" ]
        version: [ "5.21-60" ]
  - class: NetworkAccess
    networkAccess: true"""