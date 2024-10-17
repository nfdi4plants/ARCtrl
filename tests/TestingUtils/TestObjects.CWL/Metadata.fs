module TestObjects.CWL.Metadata

let metadataFileContent ="""arc:has technology type:
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