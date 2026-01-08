module TestObjects.CWL.WorkflowSteps

let workflowStepsFileContent ="""steps:
  MzMLToMzlite:
    run: ./runs/MzMLToMzlite/proteomiqon-mzmltomzlite.cwl
    in:
      stageDirectory: stage
      inputDirectory:
        source: inputMzML
    out: [dir]
  PeptideSpectrumMatching:
    run: ./runs/PeptideSpectrumMatching/proteomiqon-peptidespectrummatching.cwl
    in:
      stageDirectory:
        source: stage
      inputDirectory:
        source:
          - MzMLToMzlite/dir1
          - MzMLToMzlite/dir2
        linkMerge: merge_flattened
      parallelismLevel:
        default: 8
      outputDirectory:
        valueFrom: "output" 
    out: [dir1, dir2]"""

