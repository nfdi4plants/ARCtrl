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

    /// A workflow with an id that defines a type. A step of it runs an inline workflow, and a step of that runs an inline
    /// tool. The tool names the type by its bare name. Schema Salad finds it by walking up the ids of the steps.
    let nestedWorkflowFile = """cwlVersion: v1.2
class: Workflow
id: wf
requirements:
  - class: SchemaDefRequirement
    types:
      - name: Point
        type: record
        fields:
          x: int
  - class: SubworkflowFeatureRequirement
inputs: {}
outputs: {}
steps:
  sub:
    run:
      class: Workflow
      inputs: {}
      outputs: {}
      steps:
        deep:
          run:
            class: CommandLineTool
            baseCommand: x
            inputs:
              p: Point
            outputs: {}
          in:
            p: {default: {x: 1}}
          out: []
    in: {}
    out: []
"""

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

    /// A workflow with an id and its requirement in map syntax. Schema Salad gives its type the id pipeline/Point, and
    /// the workflow names it with that id. The inline tool of the step move finds the type by its bare name and names
    /// it a second time through the id of the workflow. The step shift has a requirement of its own, and its inline
    /// tool names the type of that step. A tool uses one SchemaDefRequirement, so it names the types of the workflow
    /// or the types of its step.
    let workflowFile = """cwlVersion: v1.2
class: Workflow
id: pipeline
requirements:
  SchemaDefRequirement:
    types:
      - name: Point
        type: record
        fields:
          x: int
inputs:
  origin: "#pipeline/Point"
steps:
  move:
    run:
      class: CommandLineTool
      baseCommand: move
      inputs:
        from: Point
        again: "#pipeline/Point"
      outputs: {}
    in:
      from: origin
      again: origin
    out: []
  shift:
    requirements:
      - class: SchemaDefRequirement
        types:
          - name: Offset
            type: record
            fields:
              dx: int
    run:
      class: CommandLineTool
      baseCommand: shift
      inputs:
        by: Offset?
      outputs: {}
    in: {}
    out: []
outputs: {}
"""

    /// A tool whose type is defined in the hints.
    let hintsFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
hints:
  - class: SchemaDefRequirement
    types:
      - name: Tag
        type: enum
        symbols: [a, b]
inputs:
  tag: Tag
outputs: {}
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

/// Documents for the override of a SchemaDefRequirement. The innermost SchemaDefRequirement applies. A requirement of
/// a process or a step overrides the one of the enclosing process, and a requirement overrides a hint.
module SchemaDefOverride =

    /// A tool with two SchemaDefRequirements in its requirements. The second one applies, and the port names its type.
    /// The port y: A names the type of the first one instead.
    let twoRequirementsFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: SchemaDefRequirement
    types:
      - name: A
        type: enum
        symbols: [a]
  - class: SchemaDefRequirement
    types:
      - name: B
        type: enum
        symbols: [b]
inputs:
  y: B
outputs: {}
"""

    /// A tool with the type A in its requirements and the type B in its hints. The requirement overrides the hint, so
    /// the port y: B names a type that does not apply. The port y: A names the type of the requirement.
    let requirementAndHintFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: SchemaDefRequirement
    types:
      - name: A
        type: record
        fields:
          x: int
hints:
  - class: SchemaDefRequirement
    types:
      - name: B
        type: record
        fields:
          a: A
inputs:
  y: B
outputs: {}
"""

    /// A tool whose second SchemaDefRequirement names the type of the first one. Only the second one applies.
    let secondRequirementNamesFirstFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: SchemaDefRequirement
    types:
      - name: A
        type: record
        fields:
          x: int
  - class: SchemaDefRequirement
    types:
      - name: B
        type: record
        fields:
          a: A
inputs:
  y: B
outputs: {}
"""

    /// A workflow with the type Point and a step whose own SchemaDefRequirement names that type. The requirement of
    /// the step overrides the one of the workflow.
    let stepOverridesWorkflowFile = """cwlVersion: v1.2
class: Workflow
id: wf
requirements:
  - class: SchemaDefRequirement
    types:
      - name: Point
        type: record
        fields:
          x: int
inputs: {}
outputs: {}
steps:
  move:
    requirements:
      - class: SchemaDefRequirement
        types:
          - name: Seg
            type: record
            fields:
              a: Point
    run:
      class: CommandLineTool
      baseCommand: x
      inputs:
        p: Seg?
      outputs: {}
    in: {}
    out: []
"""

    /// A workflow with the type A in its hints and an inline tool with the type B in its hints. The hint of the tool
    /// overrides the hint of the workflow, so the port p: A? names a type that does not apply. The port p: run/B?
    /// names the type of the tool.
    let innerHintFile = """cwlVersion: v1.2
class: Workflow
id: wf
hints:
  - class: SchemaDefRequirement
    types:
      - name: A
        type: enum
        symbols: [a]
inputs: {}
outputs: {}
steps:
  s:
    run:
      class: CommandLineTool
      baseCommand: x
      hints:
        - class: SchemaDefRequirement
          types:
            - name: B
              type: enum
              symbols: [b]
      inputs:
        p: A?
      outputs: {}
    in: {}
    out: []
"""

    /// A workflow with the type A in its requirements and an inline tool with the type B in its hints. The requirement
    /// of the workflow overrides the hint of the tool, so the port p: run/B? names a type that does not apply. The port
    /// p: A? names the type of the workflow.
    let outerRequirementInnerHintFile = """cwlVersion: v1.2
class: Workflow
id: wf
requirements:
  - class: SchemaDefRequirement
    types:
      - name: A
        type: enum
        symbols: [a]
inputs: {}
outputs: {}
steps:
  s:
    run:
      class: CommandLineTool
      baseCommand: x
      hints:
        - class: SchemaDefRequirement
          types:
            - name: B
              type: enum
              symbols: [b]
      inputs:
        p: run/B?
      outputs: {}
    in: {}
    out: []
"""

    /// A workflow without a SchemaDefRequirement whose port names the type of a step. The type belongs to the step only.
    let workflowPortStepTypeFile = """cwlVersion: v1.2
class: Workflow
id: wf
inputs:
  o: "#wf/s/Offset?"
outputs: {}
steps:
  s:
    requirements:
      - class: SchemaDefRequirement
        types:
          - name: Offset
            type: enum
            symbols: [a]
    run:
      class: CommandLineTool
      baseCommand: x
      inputs:
        p: Offset?
      outputs: {}
    in: {}
    out: []
"""

    /// A workflow whose step defines the type Offset, and whose inline tool defines the type Foo. The requirement of
    /// the tool overrides the one of the step, so the port p: "#wf/s/Offset?" names a type that does not apply. The
    /// port p: run/Foo? names the type of the tool.
    let runOverridesStepFile = """cwlVersion: v1.2
class: Workflow
id: wf
inputs: {}
outputs: {}
steps:
  s:
    requirements:
      - class: SchemaDefRequirement
        types:
          - name: Offset
            type: enum
            symbols: [a]
    run:
      class: CommandLineTool
      baseCommand: x
      requirements:
        - class: SchemaDefRequirement
          types:
            - name: Foo
              type: enum
              symbols: [f]
      inputs:
        p: "#wf/s/Offset?"
      outputs: {}
    in: {}
    out: []
"""

    /// A tool that imports shapes.yml in its first SchemaDefRequirement. The second requirement overrides the first, so
    /// the import does not apply to the port.
    let importInOverriddenRequirementFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: SchemaDefRequirement
    types:
      - $import: shapes.yml
  - class: SchemaDefRequirement
    types:
      - name: B
        type: enum
        symbols: [b]
inputs:
  x: shapes.yml#Circle?
outputs: {}
"""

    /// A tool that imports shapes.yml in its hints. The requirement overrides the hint, so the import does not apply.
    let importInOverriddenHintFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
hints:
  - class: SchemaDefRequirement
    types:
      - $import: shapes.yml
requirements:
  - class: SchemaDefRequirement
    types:
      - name: B
        type: enum
        symbols: [b]
inputs:
  x: shapes.yml#Circle?
outputs: {}
"""

/// Documents for the ids of the types that a SchemaDefRequirement defines.
module SchemaDefIds =

    /// A tool whose record Outer holds named record and enum schemas in a field, in the items of an array, in a
    /// list of types and below another named record, and a record Listed with its fields in list form. Each port names
    /// one of them by its absolute id, which Schema Salad builds from the ids of the fields.
    let nestedNamedFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
requirements:
  - class: SchemaDefRequirement
    types:
      - name: Outer
        type: record
        fields:
          inner:
            type:
              type: enum
              name: Color
              symbols: [red]
          list:
            type:
              type: array
              items:
                type: enum
                name: Shade
                symbols: [light]
          deep:
            type:
              type: record
              name: Mid
              fields:
                leaf:
                  type:
                    type: enum
                    name: Leaf
                    symbols: [d]
          pick:
            type:
              - "null"
              - type: enum
                name: Pick
                symbols: [x]
      - name: Listed
        type: record
        fields:
          - name: item
            type:
              type: enum
              name: Kind
              symbols: [k]
inputs:
  color: "#Outer/inner/Color?"
  shade: "#Outer/list/Shade?"
  leaf: "#Outer/deep/Mid/leaf/Leaf?"
  mid: "#Outer/deep/Mid?"
  pick: "#Outer/pick/Pick?"
  kind: "#Listed/item/Kind?"
  outer: Outer?
outputs: []
"""

    /// A tool with an id whose record holds a named enum schema in a field. The id of the enum starts with the id of the
    /// tool.
    let nestedNamedInProcessWithIdFile = """cwlVersion: v1.2
class: CommandLineTool
id: t
baseCommand: echo
requirements:
  - class: SchemaDefRequirement
    types:
      - name: Outer
        type: record
        fields:
          inner:
            type:
              type: enum
              name: Color
              symbols: [red]
inputs:
  c: "#t/Outer/inner/Color?"
outputs: []
"""

    /// A workflow whose inline tool has the id inner and defines the type Point. The id of an inline run lies below the
    /// segment run, so the type is #wf/s/run/inner/Point.
    let inlineRunWithIdFile = """cwlVersion: v1.2
class: Workflow
id: wf
inputs: {}
outputs: {}
steps:
  s:
    run:
      class: CommandLineTool
      id: inner
      baseCommand: x
      requirements:
        - class: SchemaDefRequirement
          types:
            - name: Point
              type: enum
              symbols: [a]
      inputs:
        x: "#wf/s/run/inner/Point?"
      outputs: {}
    in: {}
    out: []
"""

    /// A tool whose SchemaDefRequirement holds a named record and two unnamed schemas, an enum and a record. An unnamed
    /// schema adds no id.
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

/// Documents with SchemaDefRequirements at several levels of nested workflows. cwltool hands the steps of a workflow the
/// first SchemaDefRequirement of the requirements that it holds, so the outermost one reaches a tool below nested
/// workflows, and a requirement of the tool replaces it.
module SchemaDefNested =

    /// A workflow with the type Outer, a nested workflow with the type Inner, and a tool below both that names the type
    /// through the {0} placeholder.
    let nestedWorkflowsFile = """cwlVersion: v1.2
class: Workflow
id: wf
requirements:
  - class: SubworkflowFeatureRequirement
  - class: SchemaDefRequirement
    types:
      - name: Outer
        type: enum
        symbols: [a]
inputs: {}
outputs: {}
steps:
  s:
    run:
      class: Workflow
      requirements:
        - class: SchemaDefRequirement
          types:
            - name: Inner
              type: enum
              symbols: [a]
      inputs: {}
      outputs: {}
      steps:
        t:
          run:
            class: CommandLineTool
            baseCommand: x
            inputs:
              p: "{0}"
            outputs: {}
          in:
            p:
              default: a
          out: []
    in: {}
    out: []
"""

    /// A tool with a port in the type of an inline enum, and a second port that names the enum by its id.
    let portSchemaFile = """cwlVersion: v1.2
class: CommandLineTool
baseCommand: echo
inputs:
  a:
    type:
      type: enum
      name: Color
      symbols: [red]
    default: red
  b:
    type: "{0}"
    default: red
outputs:
  o:
    type: "{1}"
    outputBinding:
      glob: out.txt
"""
