#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: CommandLineTool
hints:
  DockerRequirement:
    dockerImageId: "proteomiqon"
    dockerFile: {$include: "Dockerfile"}

baseCommand: [/tools/proteomiqon-adddeducedpeptides]
inputs:
  quantDirectory:
    type: Directory
    inputBinding:
      position: 1
      prefix: -i
  proteinDirectory:
    type: Directory
    inputBinding:
      position: 2
      prefix: -ii
  outputDirectory:
    type: Directory
    inputBinding:
      position: 3
      prefix: -o
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entry: $(inputs.quantDirectory)
        writable: true
      - entry: $(inputs.proteinDirectory)
        writable: true
      - entry: $(inputs.outputDirectory)
        writable: true
outputs:
  dir:
    type: Directory
    outputBinding:
      glob: $(inputs.outputDirectory.basename)

        
        
        
