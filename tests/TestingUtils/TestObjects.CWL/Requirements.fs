module TestObjects.CWL.Requirements

open ARCtrl.CWL

let requirementsClassFileContent ="""requirements:
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

let requirementsMappingFileContent ="""requirements:
  DockerRequirement:
    dockerImageId: "devcontainer"
    dockerFile: {$include: "FSharpArcCapsule/Dockerfile"}
  InitialWorkDirRequirement:
    listing:
      - entryname: arc
        entry: $(inputs.arcDirectory)
        writable: true
      - entry: $(inputs.outputDirectory)
        writable: true
  EnvVarRequirement:
    envDef:
      - envName: DOTNET_NOLOGO
        envValue: "true"
      - envName: TEST
        envValue: "false"
  SoftwareRequirement:
    packages:
      - package: interproscan
        specs: [ "https://identifiers.org/rrid/RRID:SCR_005829" ]
        version: [ "5.21-60" ]
  NetworkAccess:
    networkAccess: true"""

let requirementsJSONFileContent ="""
requirements: {
  DockerRequirement: {
    dockerImageId: "devcontainer",
    dockerFile: { $include: "FSharpArcCapsule/Dockerfile" }
  },
  InitialWorkDirRequirement: {
    listing: [
      { entryname: "arc", entry: "$(inputs.arcDirectory)", writable: true },
      { entry: "$(inputs.outputDirectory)", writable: true }
    ]
  },
  EnvVarRequirement: {
    envDef: [
      { envName: "DOTNET_NOLOGO", envValue: "true" },
      { envName: "TEST", envValue: "false" }
    ]
  },
  SoftwareRequirement: {
    packages: [
      {
        package: "interproscan",
        specs: [ "https://identifiers.org/rrid/RRID:SCR_005829" ],
        version: [ "5.21-60" ]
      }
    ]
  },
  NetworkAccess: { networkAccess: true }
}
"""

let requirementsArraySyntax = """
requirements:
  - class: SubworkflowFeatureRequirement
"""
let requirementsMappingSyntax = """
requirements:
  SubworkflowFeatureRequirement: {}
"""

let requirementsJSONSyntax = """
requirements: { SubworkflowFeatureRequirement: {} }
"""

module Docker =

    let dockerFileMap = Map.ofList ["$include","FSharpArcCapsule/Dockerfile"]

    let dockerRequirement = DockerRequirement.createFromLegacyMap (dockerImageId = "devcontainer", dockerFileMap = dockerFileMap)

    let requirement = Requirement.DockerRequirement dockerRequirement
