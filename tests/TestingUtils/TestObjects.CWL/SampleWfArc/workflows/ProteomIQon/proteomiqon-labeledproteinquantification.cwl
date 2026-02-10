#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: CommandLineTool
hints:
  DockerRequirement:
    dockerImageId: "proteomiqon"
    dockerFile: {$include: "Dockerfile"}

baseCommand: [/tools/proteomiqon-labeledproteinquantification]
inputs:
  quantAndProtDirectory:
    type: Directory
    inputBinding:
      position: 1
      prefix: -i
  outputDirectory:
    type: Directory
    inputBinding:
      position: 2
      prefix: -o
  params:
    type: File
    inputBinding:
      position: 3
      prefix: -p
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entry: $(inputs.quantAndProtDirectory)
        writable: true
      - entry: $(inputs.outputDirectory)
        writable: true
outputs:
  dir:
    type: Directory
    outputBinding:
      glob: $(inputs.outputDirectory.basename)

        
        
        
