#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: CommandLineTool
hints:
  DockerRequirement:
    dockerImageId: "proteomiqon"
    dockerFile: {$include: "Dockerfile"}

baseCommand: [/tools/proteomiqon-alignmentbasedquantification]
arguments: ["-mf"]
inputs:
  instrumentOutput:
    type: Directory
    inputBinding:
      position: 1
      prefix: -i
  alignedPeptides:
    type: Directory
    inputBinding:
      position: 2
      prefix: -ii
  alignmentMetrics:
    type: Directory
    inputBinding:
      position: 3
      prefix: -iii
  quantifiedPeptides:
    type: Directory
    inputBinding:
      position: 4
      prefix: -iv
  database:
    type: File
    inputBinding:
      position: 5
      prefix: -d
  outputDirectory:
    type: Directory
    inputBinding:
      position: 6
      prefix: -o
  params:
    type: File
    inputBinding:
      position: 7
      prefix: -p
  parallelismLevel:
    type: int
    inputBinding:
      position: 8
      prefix: -c
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entry: $(inputs.instrumentOutput)
        writable: true
      - entry: $(inputs.alignedPeptides)
        writable: true
      - entry: $(inputs.alignmentMetrics)
        writable: true
      - entry: $(inputs.quantifiedPeptides)
        writable: true
      - entry: $(inputs.outputDirectory)
        writable: true
outputs:
  dir:
    type: Directory
    outputBinding:
      glob: $(inputs.outputDirectory.basename)

        
        
        
