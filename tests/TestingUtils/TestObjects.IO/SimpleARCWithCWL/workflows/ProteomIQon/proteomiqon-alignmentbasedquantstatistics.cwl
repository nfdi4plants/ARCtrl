#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: CommandLineTool
hints:
  DockerRequirement:
    dockerImageId: "proteomiqon"
    dockerFile: {$include: "Dockerfile"}

baseCommand: [/tools/proteomiqon-alignmentbasedquantstatistics]
inputs:
  quantDirectory:
    type: Directory
    inputBinding:
      position: 1
      prefix: -i
  alignmentDirectory:
    type: Directory
    inputBinding:
      position: 2
      prefix: -ii
  alignedQuantDirectory:
    type: Directory
    inputBinding:
      position: 3
      prefix: -iii
  quantLearnDirectory:
    type: Directory
    inputBinding:
      position: 4
      prefix: -l
  alignmentLearnDirectory:
    type: Directory
    inputBinding:
      position: 5
      prefix: -ll
  alignedQuantLearnDirectory:
    type: Directory
    inputBinding:
      position: 6
      prefix: -lll
  outputDirectory:
    type: Directory
    inputBinding:
      position: 7
      prefix: -o
  params:
    type: File
    inputBinding:
      position: 8
      prefix: -p
  parallelismLevel:
    type: int?
    inputBinding:
      position: 9
      prefix: -c
  testSet:
    type: boolean?
    inputBinding:
      position: 10
      prefix: -ts
      valueFrom: ""
  diagnosticCharts:
    type: boolean?
    inputBinding:
      position: 11
      prefix: -dc
      valueFrom: ""
  matchFiles:
    type: boolean?
    inputBinding:
      position: 12
      prefix: -mf
      valueFrom: ""
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entry: $(inputs.quantDirectory)
        writable: true
      - entry: $(inputs.alignmentDirectory)
        writable: true
      - entry: $(inputs.alignedQuantDirectory)
        writable: true
      - entry: $(inputs.quantLearnDirectory)
        writable: true
      - entry: $(inputs.alignmentLearnDirectory)
        writable: true
      - entry: $(inputs.alignedQuantLearnDirectory)
        writable: true
      - entry: $(inputs.outputDirectory)
        writable: true
outputs:
  dir:
    type: Directory
    outputBinding:
      glob: $(inputs.outputDirectory.basename)

        
        
        
