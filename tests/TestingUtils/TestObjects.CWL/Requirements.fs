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

    let dockerRequirement = DockerRequirement.create (dockerImageId = SchemaSaladString.Literal "devcontainer", dockerFileReference = Include "FSharpArcCapsule/Dockerfile")

    let requirement = Requirement.DockerRequirement dockerRequirement


let initialWorkDirListingTypedFileContent = """requirements:
  - class: InitialWorkDirRequirement
    listing:
      - checksum: sha1$abc
        class: File
        path: /tmp/in.txt
        basename: in.txt
        size: 42
        arc:file note: keep file overflow
      - class: Directory
        listing: [out.txt]
        path: /tmp/out
        basename: out
        arc:dir note: keep dir overflow"""


module SchemaDefTypes =

    /// A tool with two records. The second record names the first by a bare name and with a leading #, and it holds an
    /// array of the first as well. The ports name both records.
    let toolFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: draw
requirements:
  - class: SchemaDefRequirement
    types:
      - name: Point
        type: record
        fields:
          x: int
          y: int
      - name: Segment
        type: record
        fields:
          from: Point
          to: "#Point"
          waypoints: Point[]
inputs:
  start: Point
  anchor: "#Point"
  route: Segment
  corners: Point[]
  spare: Point?
outputs:
  bend:
    type: Segment
"""

    /// A tool whose only requirement defines one record, with no reference to any other type.
    let plainRecordFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: SchemaDefRequirement
    types:
      - name: Point
        type: record
        fields:
          x: int
inputs: {}
outputs: {}
"""

module SchemaDefIds =

    /// A tool whose SchemaDefRequirement holds a named record and two unnamed schemas, an enum and a record.
    let unnamedDefinitionsFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: SchemaDefRequirement
    types:
      - name: Point
        type: record
        fields:
          x: int
      - type: enum
        symbols: [a]
      - type: record
        fields:
          p: Point
inputs:
  x: Point?
outputs: []
"""
