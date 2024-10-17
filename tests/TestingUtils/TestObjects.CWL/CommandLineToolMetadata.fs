module TestObjects.CWL.CommandLineToolMetadata

let cwlFile ="""cwlVersion: v1.2
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
      glob: $(runtime.outdir)/*.csv

arc:has technology type:
  - class: arc:technology type
    arc:annotation value: "Fsharp Devcontainer"

arc:technology platform: ".NET"

arc:performer:
  - class: arc:Person
    arc:first name: "Timo"
    arc:last name: "Mühlhaus"
    arc:has role: 
      - class: arc:role
        arc:term accession: "https://credit.niso.org/contributor-roles/formal-analysis/"
        arc:annotation value: "Formal analysis"

arc:has process sequence:
  - class: arc:process sequence
    arc:name: "script.fsx"
    arc:has input: 
      - class: arc:data
        arc:name: "./arc/assays/measurement1/dataset/table.csv"
    arc:has parameter value:
      - class: arc:process parameter value
        arc:has parameter:
          - class: arc:protocol parameter
            arc:has parameter name:
            - class: arc:parameter name
              arc:term accession: "http://purl.obolibrary.org/obo/NCIT_C43582"
              arc:term source REF: "NCIT"
              arc:annotation value: "Data Transformation"
        arc:value: 
          - class: arc:ontology annotation
            arc:term accession: "http://purl.obolibrary.org/obo/NCIT_C64911"
            arc:term source REF: "NCIT"
            arc:annotation value: "Addition"
"""

let expectedMetadataString ="""?arc:has technology type:
    ?class: arc:technology type
    ?arc:annotation value: Fsharp Devcontainer
?arc:technology platform: .NET
?arc:performer:
    ?class: arc:Person
    ?arc:first name: Timo
    ?arc:last name: Mühlhaus
    ?arc:has role:
        ?class: arc:role
        ?arc:term accession: https://credit.niso.org/contributor-roles/formal-analysis/
        ?arc:annotation value: Formal analysis
?arc:has process sequence:
    ?class: arc:process sequence
    ?arc:name: script.fsx
    ?arc:has input:
        ?class: arc:data
        ?arc:name: ./arc/assays/measurement1/dataset/table.csv
    ?arc:has parameter value:
        ?class: arc:process parameter value
        ?arc:has parameter:
            ?class: arc:protocol parameter
            ?arc:has parameter name:
                ?class: arc:parameter name
                ?arc:term accession: http://purl.obolibrary.org/obo/NCIT_C43582
                ?arc:term source REF: NCIT
                ?arc:annotation value: Data Transformation
        ?arc:value:
            ?class: arc:ontology annotation
            ?arc:term accession: http://purl.obolibrary.org/obo/NCIT_C64911
            ?arc:term source REF: NCIT
            ?arc:annotation value: Addition"""