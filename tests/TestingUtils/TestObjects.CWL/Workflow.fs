module TestObjects.CWL.Workflow

let workflowFile ="""cwlVersion: v1.2
class: Workflow

requirements:
  - class: MultipleInputFeatureRequirement

inputs:
  cores: int
  db: File
  stage: Directory
  outputMzML: Directory
  outputPSM: Directory
  inputMzML: Directory
  paramsMzML: File
  paramsPSM: File
  sampleRecord:
    type:
      type: array
      items:
        type: record
        fields:
          readsOfOneSample:
            type: File[]
          sampleName:
            type: string?

steps:
  MzMLToMzlite:
    run: ./runs/MzMLToMzlite/proteomiqon-mzmltomzlite.cwl
    in:
      stageDirectory: stage
      inputDirectory: inputMzML
      params: paramsMzML
      outputDirectory: outputMzML
      parallelismLevel: cores
    out: [dir]
  PeptideSpectrumMatching:
    run: ./runs/PeptideSpectrumMatching/proteomiqon-peptidespectrummatching.cwl
    in:
      stageDirectory: stage
      inputDirectory:
        source:
          - MzMLToMzlite/dir1
          - MzMLToMzlite/dir2
        linkMerge: merge_flattened
      database: db
      params: paramsPSM
      outputDirectory: outputPSM
      parallelismLevel: cores
    out: [dir]

outputs:
  mzlite:
    type: Directory
    outputSource: MzMLToMzlite/dir
  psm:
    type: Directory
    outputSource: PeptideSpectrumMatching/dir"""

