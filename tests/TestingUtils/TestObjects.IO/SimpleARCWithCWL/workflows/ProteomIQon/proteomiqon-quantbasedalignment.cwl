#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: CommandLineTool
hints:
  DockerRequirement:
    dockerImageId: "proteomiqon"
    dockerFile: {$include: "Dockerfile"}

baseCommand: [/tools/proteomiqon-quantbasedalignment]
inputs:
  inputTargets:
    type: Directory
    inputBinding:
      position: 1
      prefix: -i
  inputSources:
    type: Directory
    inputBinding:
      position: 2
      prefix: -ii
  outputDirectory:
    type: Directory
    inputBinding:
      position: 3
      prefix: -o
  parallelismLevel:
    type: int
    inputBinding:
      position: 4
      prefix: -c
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entry: $(inputs.inputTargets)
        writable: true
      - entry: $(inputs.inputSources)
        writable: true
      - entry: $(inputs.outputDirectory)
        writable: true
outputs:
  dir:
    type: Directory
    outputBinding:
      glob: $(inputs.outputDirectory.basename)

        
        
        
