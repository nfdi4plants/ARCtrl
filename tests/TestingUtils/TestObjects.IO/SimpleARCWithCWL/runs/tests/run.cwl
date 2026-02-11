cwlVersion: v1.2
class: Workflow

requirements:
  - class: MultipleInputFeatureRequirement
  - class: SubworkflowFeatureRequirement

inputs:
  cores:
    type: int
    label: CPU cores
    doc: Number of CPU cores to use for processing
  outputMzML:
    type: Directory
    label: MzML conversion output directory
    doc: Output directory for mzLite files
  outputDB:
    type: Directory
    label: Database output directory
    doc: Output directory for database files
  outputPSM:
    type: Directory
    label: PSM output directory
    doc: Output directory for peptide-spectrum match files
  outputPSMStats:
    type: Directory
    label: PSM statistics output directory
    doc: Output directory for PSM statistics
  outputQuant:
    type: Directory
    label: Quantification output directory
    doc: Output directory for quantification results
  outputProt:
    type: Directory
    label: Protein output directory
    doc: Output directory for protein identification results
  outputAlignment:
    type: Directory
    label: Alignment output directory
    doc: Output directory for alignment results
  outputAlignmentQuant:
    type: Directory
    label: Alignment quantification output directory
    doc: Output directory for alignment-based quantification
  outputAlignmentStats:
    type: Directory
    label: Alignment statistics output directory
    doc: Output directory for alignment statistics
  outputProtDeduced:
    type: Directory
    label: Deduced protein output directory
    doc: Output directory for deduced protein results
  outputQuantAndProt:
    type: Directory
    label: Combined quantification and protein output directory
    doc: Output directory for combined quantification and protein results
  outputLabeledProt:
    type: Directory
    label: Labeled protein output directory
    doc: Output directory for labeled protein quantification
  inputMzML:
    type: Directory
    label: MzML input directory
    doc: Input directory containing mzML files
  inputPeptideDB:
    type: File
    label: Peptide database file
    doc: Input peptide database file
  paramsMzML:
    type: File
    label: MzML parameters
    doc: Parameter file for mzML processing
  paramsDB:
    type: File
    label: Database parameters
    doc: Parameter file for database creation
  paramsPSM:
    type: File
    label: PSM parameters
    doc: Parameter file for PSM analysis
  paramsPSMStats:
    type: File
    label: PSM statistics parameters
    doc: Parameter file for PSM statistics
  paramsPSMBasedQuant:
    type: File
    label: PSM-based quantification parameters
    doc: Parameter file for PSM-based quantification
  paramsAlignmentBasedQuant:
    type: File
    label: Alignment-based quantification parameters
    doc: Parameter file for alignment-based quantification
  paramsAlignmentBasedQuantStats:
    type: File
    label: Alignment quantification statistics parameters
    doc: Parameter file for alignment-based quantification statistics
  paramsProt:
    type: File
    label: Protein inference parameters
    doc: Parameter file for protein inference
  paramsLabeledProteinQuant:
    type: File
    label: Labeled protein quantification parameters
    doc: Parameter file for labeled protein quantification

steps:
  Workflow:
    label: ProteomIQon
    doc: The ProteomIQon is a collection of open source computational proteomics tools to build pipelines for the evaluation of MS derived proteomics data written
    run: ../../workflows/ProteomIQon/workflow.cwl
    in:
      cores: cores
      outputMzML: outputMzML
      outputDB: outputDB
      outputPSM: outputPSM
      outputPSMStats: outputPSMStats
      outputQuant: outputQuant
      outputProt: outputProt
      outputAlignment: outputAlignment
      outputAlignmentQuant: outputAlignmentQuant
      outputAlignmentStats: outputAlignmentStats
      outputProtDeduced: outputProtDeduced
      outputQuantAndProt: outputQuantAndProt
      outputLabeledProt: outputLabeledProt
      inputMzML: inputMzML
      inputPeptideDB: inputPeptideDB
      paramsMzML: paramsMzML
      paramsDB: paramsDB
      paramsPSM: paramsPSM
      paramsPSMStats: paramsPSMStats
      paramsPSMBasedQuant: paramsPSMBasedQuant
      paramsAlignmentBasedQuant: paramsAlignmentBasedQuant
      paramsAlignmentBasedQuantStats: paramsAlignmentBasedQuantStats
      paramsProt: paramsProt
      paramsLabeledProteinQuant: paramsLabeledProteinQuant
    out: [db, mzml, psm, psmstats, quant, prot, alignment, alignmentQuant, alignmentStats, protDeduced, quantAndProt, labeledProteins]

outputs:
  db:
    type: Directory
    label: Database files
    doc: Created database
    outputSource: Workflow/db
  mzml:
    type: Directory
    label: mzLite files
    doc: Processed mzLite files
    outputSource: Workflow/mzml
  psm:
    type: Directory
    label: Peptide-spectrum matches
    doc: Peptide-spectrum match results
    outputSource: Workflow/psm
  psmstats:
    type: Directory
    label: PSM statistics
    doc: Statistical analysis of peptide-spectrum matches
    outputSource: Workflow/psmstats
  quant:
    type: Directory
    label: Quantification results
    doc: PSM-based quantification results
    outputSource: Workflow/quant
  prot:
    type: Directory
    label: Protein identification results
    doc: Protein inference and identification results
    outputSource: Workflow/prot
  alignment:
    type: Directory
    label: Alignment results
    doc: Feature alignment results across samples
    outputSource: Workflow/alignment
  alignmentQuant:
    type: Directory
    label: Alignment-based quantification
    doc: Quantification results based on feature alignment
    outputSource: Workflow/alignmentQuant
  alignmentStats:
    type: Directory
    label: Alignment statistics
    doc: Statistical analysis of alignment-based quantification
    outputSource: Workflow/alignmentStats
  protDeduced:
    type: Directory
    label: Deduced proteins
    doc: Deduced protein results from alignment-based quantification
    outputSource: Workflow/protDeduced
  quantAndProt:
    type: Directory
    label: Combined quantification and protein results
    doc: Combined quantification and protein inference results
    outputSource: Workflow/quantAndProt
  labeledProteins:
    type: Directory
    label: Labeled protein quantification
    doc: Labeled protein quantification results
    outputSource: Workflow/labeledProteins