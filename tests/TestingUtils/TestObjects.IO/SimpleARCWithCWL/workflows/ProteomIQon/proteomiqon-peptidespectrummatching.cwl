#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: CommandLineTool
hints:
  DockerRequirement:
    dockerImageId: "proteomiqon"
    dockerFile: {$include: "Dockerfile"}
  InitialWorkDirRequirement:
    listing:
      - entry: $(inputs.inputDirectory)
        writable: true
      - entry: $(inputs.outputDirectory)
        writable: true

baseCommand: [/tools/proteomiqon-peptidespectrummatching]
inputs:
  inputDirectory:
    type: Directory
    inputBinding:
      position: 1
      prefix: -i
  database:
    type: File
    inputBinding:
      position: 2
      prefix: -d
  params:
    type: File
    inputBinding:
      position: 3
      prefix: -p
  outputDirectory:
    type: Directory
    inputBinding:
      position: 4
      prefix: -o
  parallelismLevel:
    type: int
    inputBinding:
      position: 5
      prefix: -c
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entry: $(inputs.inputDirectory)
        writable: true
      # stageDirectory entry intentionally disabled
      - entry: $(inputs.outputDirectory)
        writable: true
outputs:
  dir:
    type: Directory
    outputBinding:
      glob: $(inputs.outputDirectory.basename)
        
        
        
