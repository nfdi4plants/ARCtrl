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

let workflowWithExtendedStepFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    label: Example step
    doc: Step docs
    scatter: input1
    scatterMethod: dotproduct
    in:
      in1:
        source: input1
        loadContents: true
        loadListing: deep_listing
        label: Input label
        linkMerge: merge_nested
    out:
      - id: out"""

let workflowWithInlineRunCommandLineToolFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: inlineStep/out
steps:
  inlineStep:
    run:
      class: CommandLineTool
      cwlVersion: v1.2
      baseCommand: echo
      inputs: {}
      outputs:
        out: string
    in: {}
    out: [out]"""

let workflowWithInputArrayStepFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      - id: in1
        source: input1
        label: Input in array syntax
    out: [out]"""

let workflowWithStepsArrayFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  - id: step1
    run: ./tool.cwl
    when: $(inputs.input1 != null)
    in:
      - id: in1
        source: input1
        pickValue: first_non_null
        doc: Input docs
        default:
          type: string
          value: fallback
    out:
      - id: out"""

let workflowWithInvalidScatterMethodFile = """cwlVersion: v1.2
class: Workflow
inputs: { input1: string }
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    scatter: input1
    scatterMethod: invalid_scatter
    in:
      in1: input1
    out: [out]"""

let workflowWithUnsupportedInlineRunClassFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run:
      class: ExpressionTool
      cwlVersion: v1.2
      inputs: {}
      outputs: {}
      expression: $(null)
    in: {}
    out: [out]"""

let workflowWithNoStepsFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs: {}
steps: {}"""

let workflowWithInlineRunWorkflowFile = """cwlVersion: v1.2
class: Workflow
inputs: {}
outputs:
  result:
    type: string
    outputSource: outer/out
steps:
  outer:
    run:
      class: Workflow
      cwlVersion: v1.2
      inputs: {}
      outputs:
        out:
          type: string
          outputSource: inner/out
      steps:
        inner:
          run: echo.cwl
          in: {}
          out: [out]
    in: {}
    out: [out]"""

let workflowWithInvalidPickValueFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      in1:
        source: input1
        pickValue: invalid_pick
    out: [out]"""

let workflowWithPickValueMethodFile (pickValue: string) = $"""cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      in1:
        source: input1
        pickValue: {pickValue}
    out: [out]"""

let workflowWithScatterMethodFile (scatterMethod: string) = $"""cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    scatter: in1
    scatterMethod: {scatterMethod}
    in:
      in1:
        source: input1
    out: [out]"""

let workflowWithStructuredArrayDefaultFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      in1:
        source: input1
        default: [1, 2, 3]
    out: [out]"""

let workflowWithWhenExpressionFile (whenExpression: string) = $"""cwlVersion: v1.2
class: Workflow
inputs:
  name: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    when: "{whenExpression}"
    in:
      in1: name
    out: [out]"""

let workflowWithScatterAndScatterMethodFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
  input2: int
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    scatter: [in1, in2]
    scatterMethod: flat_crossproduct
    in:
      in1: input1
      in2: input2
    out: [out]"""

let workflowWithStepLevelRequirementsAndHintsFile = """cwlVersion: v1.2
class: Workflow
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      in1: input1
    out: [out]
    hints:
      - class: StepInputExpressionRequirement
    requirements:
      - class: NetworkAccess
        networkAccess: true"""

let workflowWithOutputSourceArrayFile = """cwlVersion: v1.2
class: Workflow
requirements:
  - class: MultipleInputFeatureRequirement
inputs: {}
outputs:
  merged:
    type: string
    outputSource:
      - step1/out
      - step2/out
steps:
  step1:
    run: ./tool-a.cwl
    in: {}
    out: [out]
  step2:
    run: ./tool-b.cwl
    in: {}
    out: [out]"""

let workflowWithInlineRunOperationFile = """cwlVersion: v1.2
class: Workflow
inputs:
  msg: string
outputs:
  echoed:
    type: string
    outputSource: op/out
steps:
  op:
    run:
      class: Operation
      cwlVersion: v1.2
      inputs:
        in:
          type: string
      outputs:
        out:
          type: string
    in:
      in: msg
    out: [out]"""

let advancedWorkflowFile = """#!/usr/bin/env cwl-runner

cwlVersion: v1.2
class: Workflow
id: "CWL_advanced_layout_workflow"

$namespaces:
  cwltool: "http://commonwl.org/cwltool#"
  arc: "https://github.com/nfdi4plants/ARC_ontology"

$schemas:
  - "https://raw.githubusercontent.com/nfdi4plants/ARC_ontology/main/ARC_v2.0.owl"
  - "https://raw.githubusercontent.com/common-workflow-language/cwltool/main/cwltool/extensions.yml"

hints:
  - class: DockerRequirement
    dockerImageId: "devcontainer"
    dockerFile:
      $include: "./Dockerfile"
    cwltool:dockerRunOptions:
      - "--gpus=all"
      # - "--memory=120g"
      # - "--memory-swap=128g"
  - class: cwltool:CUDARequirement
    cudaComputeCapability: '8.9'
    cudaDeviceCountMin: 1
    cudaVersionMin: "12.3"
  - class: EnvVarRequirement
    envDef:
      NVIDIA_VISIBLE_DEVICES: "all"
      NVIDIA_DRIVER_CAPABILITIES: "compute,utility"
      VIRTUAL_ENV: /workspace/venv
      PATH: /workspace/venv/bin:/usr/local/nvidia/bin:/usr/local/cuda/bin:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin

requirements:
  # - class: ResourceRequirement
  #   ramMin: 120000   # in megabytes
  #   coresMin: 4
  - class: MultipleInputFeatureRequirement
  - class: StepInputExpressionRequirement
  - class: InitialWorkDirRequirement
    listing:
      - entryname: arc
        #entry: "/workspaces/example-layout
        entry: $(inputs.rootDir)
        writable: true
  - class: NetworkAccess
    networkAccess: true

inputs:

  rootDir:
    type: Directory
    doc: "Root directory of the working area containing all inputs and outputs."

  auxDir:
    type: Directory
    doc: "Directory containing auxiliary runtime resources."

  inputDirA:
    type: Directory
    doc: "Input directory containing source files for processing."

  inputFileA:
    type: File
    doc: "Input table describing the source files to be processed."

  outputDirA:
    type: string
    doc: "Output directory for the first generated directory payload."

  outputDirB:
    type: string
    doc: "Output directory for the second generated directory payload."

  supportDirA:
    type: Directory
    doc: "Directory containing supporting model or configuration files."

  scalarA:
    type: float
    doc: "Threshold value controlling a decision boundary."

  integerA:
    type: int
    doc: "Primary integer setting preserved for scalar layout coverage."

  integerB:
    type: int
    doc: "Secondary integer setting preserved for scalar layout coverage."

  integerC:
    type: int
    doc: "Tertiary integer setting preserved for scalar layout coverage."

  integerD:
    type: int
    doc: "Fourth integer setting preserved for scalar layout coverage."

  integerE:
    type: int
    doc: "Fifth integer setting preserved for scalar layout coverage."

  integerF:
    type: int
    doc: "Sixth integer setting preserved for scalar layout coverage."

  integerArrayA:
    type: int[]
    doc: "Integer array setting preserved to exercise array input layout."

  outputFileA:
    type: string
    doc: "Path where the first generated file payload will be stored."

  outputFileB:
    type: string
    doc: "Path where the second generated file payload will be stored."

  outputDirC:
    type: string
    doc: "Output directory for the third generated directory payload."

  integerG:
    type: int
    doc: "Seventh integer setting preserved for scalar layout coverage."

  integerH:
    type: int
    doc: "Eighth integer setting preserved for scalar layout coverage."

  outputDirD:
    type: string
    doc: "Output directory for the fourth generated directory payload."

  inputDirB:
    type: string
    doc: "Additional input directory used in a later stage."

  outputDirE:
    type: string
    doc: "Output directory for the fifth generated directory payload."

  outputDirF:
    type: string
    doc: "Final output directory name consumed by downstream stages."


steps:
  step_alpha:
    run: ./cwl_tools/step_alpha.cwl
    in:
      rootDir:                       rootDir
      inputDirA:                     inputDirA
      inputFileA:                    inputFileA
      outputDirA:                    outputDirA
    out:
      - out_dir_alpha
      # - out_dir_alpha_children
      # - out_dir_alpha_files
      - out_mount

  step_beta:
    run: ./cwl_tools/step_beta.cwl
    in:
      rootDir:                       step_alpha/out_mount
      outputDirA:                    step_alpha/out_dir_alpha
      inputFileA:                    inputFileA
      outputDirB:                    outputDirB
      auxDir:                        auxDir
      supportDirA:                   supportDirA
      scalarA:                       scalarA
      integerA:                      integerA
      integerB:                      integerB
      integerC:                      integerC
      integerD:                      integerD
      integerE:                      integerE
      integerF:                      integerF
      integerArrayA:                 integerArrayA
    out:
      - out_dir_beta
      # - out_dir_beta_children
      # - out_dir_beta_files
      - out_mount

  step_gamma:
    run: ./cwl_tools/step_gamma.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir:                       step_beta/out_mount
      outputDirA:                    step_alpha/out_dir_alpha
      inputFileA:                    inputFileA
      outputDirB:                    step_beta/out_dir_beta
      outputFileA:                   outputFileA
    out:
      - out_file_alpha
      - out_mount

  step_delta:
    run: ./cwl_tools/step_delta.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir: step_gamma/out_mount
      inputFileB: step_gamma/out_file_alpha
      outputDirC: outputDirC
    out:
      - out_dir_gamma
      # - out_dir_gamma_plot
      # - out_dir_gamma_table
      - out_mount

  step_epsilon:
    run: ./cwl_tools/step_epsilon.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir: step_delta/out_mount
      outputDirA:                    step_alpha/out_dir_alpha
      inputFileA:                    inputFileA
      outputDirB:                    step_beta/out_dir_beta
      outputFileB:                   outputFileB
    out:
      - out_file_beta
      - out_mount

  step_zeta:
    run: ./cwl_tools/step_zeta.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir: step_epsilon/out_mount
      inputFileC: step_gamma/out_file_alpha
      integerG:                     integerG
      integerH:                     integerH
      outputDirD:                   outputDirD
    out:
      - out_dir_delta
      # - ot_distance_matrix
      # - out_file_pairwise
      # - out_file_global
      # - out_file_condition
      # - permanova_results
      # - mds_eigenvalues
      # - mds_eigenvalues_summary
      # - mds_2d
      - out_mount

  step_eta:
    run: ./cwl_tools/step_eta.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir: step_zeta/out_mount
      inputDirB:                    inputDirB
      outputDirE :                  outputDirE
    out:
      - out_dir_epsilon
      # - ot_distance_matrix_html
      # - permanova_pvalues_log_html
      # - ot_mds_scatter_html
      # - out_file_global_html
      # - out_file_condition_html
      - out_mount

  step_theta:
    run: ./cwl_tools/step_theta.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir:                       step_eta/out_mount
      inputFileD:                    step_gamma/out_file_alpha
      outputDirF:                    outputDirF
    out:
      - out_file_gamma
      - out_file_delta
      - out_file_epsilon
      - out_mount

  step_iota:
    run: ./cwl_tools/step_iota.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir:                       step_theta/out_mount
      inputFileE:                    step_gamma/out_file_alpha
      outputDirF:                    outputDirF
    out:
      - out_file_zeta
      - out_mount

  step_kappa:
    run: ./cwl_tools/step_kappa.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      rootDir:                       step_iota/out_mount
      inputFileF:                    step_gamma/out_file_alpha
      outputDirF:                    outputDirF
    out:
      - out_file_eta
      - out_mount

  step_lambda:
    run: ./cwl_tools/step_lambda.cwl
    in:
      # direct reference to the output of previous step. This creates a dependency.
      in_mount:
        source:
          - step_iota/out_mount # step_kappa/out_mount
      in_dirs:
        # linkMerge: merge_nested
        source:
          - step_alpha/out_dir_alpha
          - step_beta/out_dir_beta
          - step_delta/out_dir_gamma
          - step_zeta/out_dir_delta
          - step_eta/out_dir_epsilon
      in_files:
        linkMerge: merge_flattened
        source:
          - step_gamma/out_file_alpha
          - step_epsilon/out_file_beta
          - step_theta/out_file_gamma
          - step_theta/out_file_delta
          - step_theta/out_file_epsilon
          - step_iota/out_file_zeta
          - step_kappa/out_file_eta
      in_name:
        valueFrom: $(inputs.outputDirF)


    out:
      - final_dir

outputs:
  final_dir:
    type: Directory
    doc: "Folder containing all final results"
    outputSource: step_lambda/final_dir

arc:has technology type:
  - class: arc:technology type
    arc:annotation value: "Docker Container"

arc:technology platform: "Python"

arc:performer:
  - class: arc:Person
    arc:first name: "Example"
    arc:last name: "Researcher"
    arc:email: "workflow.author@example.org"
    arc:affiliation: "Example Research Institute"
    arc:has role:
      - class: arc:role
        arc:term accession: "https://credit.niso.org/contributor-roles/formal-analysis/"
        arc:annotation value: "Formal analysis" """

let workflowWithIntentFile = """cwlVersion: v1.2
class: Workflow
intent:
  - primary-analysis
  - quality-control
inputs:
  input1: string
outputs:
  result:
    type: string
    outputSource: step1/out
steps:
  step1:
    run: ./tool.cwl
    in:
      in1: input1
    out: [out]"""

