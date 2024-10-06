module TestObjects.CWL.WorkflowSteps

let workflowSteps ="""steps:
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
        source: MzMLToMzlite/dir
      parallelismLevel:
        default: 8
      outputDirectory:
        valueFrom: "output" 
    out: [dir1, dir2]"""

