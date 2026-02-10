#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: CommandLineTool
hints:
  DockerRequirement:
    dockerImageId: "proteomiqon"
    dockerFile: {$include: "Dockerfile"}
baseCommand: [/tools/proteomiqon-peptidedb]
inputs:
  inputFile:
    type: File
    inputBinding:
      position: 1
      prefix: -i
  params:
    type: File
    inputBinding:
      position: 2
      prefix: -p
  outputDirectory:
    type: Directory
    inputBinding:
      position: 3
      prefix: -o
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entry: $(inputs.outputDirectory)
        writable: true
outputs:
  db:
    type: File
    outputBinding:
      glob: "*/*.db"
  dbFolder:
    type: Directory
    outputBinding:
      glob: $(inputs.outputDirectory.basename)
        
        
        
