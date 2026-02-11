#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: CommandLineTool
hints:
  DockerRequirement:
    dockerImageId: "proteomiqon"
    dockerFile: {$include: "Dockerfile"}

baseCommand: [/tools/proteomiqon-psmbasedquantification]
arguments: ["-mf"]
inputs:
  inputDirectoryI:
    type: Directory
    inputBinding:
      position: 1
      prefix: -i
  inputDirectoryII:
    type: Directory
    inputBinding:
      position: 2
      prefix: -ii
  database:
    type: File
    inputBinding:
      position: 3
      prefix: -d
  params:
    type: File
    inputBinding:
      position: 4
      prefix: -p
  outputDirectory:
    type: Directory
    inputBinding:
      position: 5
      prefix: -o
  parallelismLevel:
    type: int
    inputBinding:
      position: 6
      prefix: -c
requirements:
  - class: InitialWorkDirRequirement
    listing:
      - entry: $(inputs.inputDirectoryI)
        writable: true
      - entry: $(inputs.inputDirectoryII)
        writable: true
      - entry: $(inputs.outputDirectory)
        writable: true
outputs:
  dir:
    type: Directory
    outputBinding:
      glob: $(inputs.outputDirectory.basename)

        
        
        
