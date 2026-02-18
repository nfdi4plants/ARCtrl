cwlVersion: v1.2
class: Workflow

requirements:
  - class: MultipleInputFeatureRequirement
  - class: ResourceRequirement
    coresMin: 20
    ramMin: 81920

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
  MzMLToMzlite:
    label: MzML to mzLite conversion
    doc: Converts mzML files to mzLite format with optional peak picking and filtering of mass spectra
    run: ./proteomiqon-mzmltomzlite.cwl
    in:
      inputDirectory: inputMzML
      params: paramsMzML
      outputDirectory: outputMzML
      parallelismLevel: cores
    out: [dir]
  PeptideDB:
    label: Peptide database creation
    doc: Creates a peptide database by in silico digestion of proteins from FASTA format, storing peptide-protein relationships in SQLite
    run: ./proteomiqon-peptidedb.cwl
    in:
      inputFile: inputPeptideDB
      params: paramsDB
      outputDirectory: outputDB
    out: [db, dbFolder]
  PeptideSpectrumMatching:
    label: Peptide spectrum matching
    doc: Identifies MS/MS spectra by comparing them against peptides in the reference database using SEQUEST, Andromeda, and XTandem scoring
    run: ./proteomiqon-peptidespectrummatching.cwl
    in:
      inputDirectory: MzMLToMzlite/dir
      database: PeptideDB/db
      params: paramsPSM
      outputDirectory: outputPSM
      parallelismLevel: cores
    out: [dir]
  PSMStatistics:
    label: PSM statistics and FDR control
    doc: Uses semi-supervised machine learning to integrate search engine scores into a consensus score and calculates local and global false discovery rates
    run: ./proteomiqon-psmstatistics.cwl
    in:
      inputDirectory: PeptideSpectrumMatching/dir
      database: PeptideDB/db
      params: paramsPSMStats
      outputDirectory: outputPSMStats
      parallelismLevel: cores
    out: [dir]
  PSMBasedQuantification:
    label: PSM-based quantification
    doc: Performs XIC extraction and quantification of identified peptide ions using wavelet-based peak detection and Gaussian peak fitting, supporting both label-free and labeled quantification
    run: ./proteomiqon-psmbasedquantification.cwl
    in:
      inputDirectoryI: MzMLToMzlite/dir
      inputDirectoryII: PSMStatistics/dir
      database: PeptideDB/db
      params: paramsPSMBasedQuant
      outputDirectory: outputQuant
      parallelismLevel: cores
    out: [dir]
  ProteinInference:
    label: Protein inference
    doc: Maps identified peptides to protein groups, handling one-to-many peptide-protein relationships and reporting peptide evidence classes
    run: ./proteomiqon-proteininference.cwl
    in:
      inputDirectory: PSMStatistics/dir
      database: PeptideDB/db
      params: paramsProt
      outputDirectory: outputProt
    out: [dir]
  QuantBasedAlignment:
    label: Quantification-based alignment
    doc: Performs run alignment using smoothing splines to map retention times between source and target runs, enabling peptide ion transfer across runs
    run: ./proteomiqon-quantbasedalignment.cwl
    in:
      inputTargets: PSMBasedQuantification/dir
      inputSources: PSMBasedQuantification/dir
      outputDirectory: outputAlignment
      parallelismLevel: cores
    out: [dir]
  AlignmentBasedQuantification:
    label: Alignment-based quantification
    doc: Quantifies peptides transferred from other runs using alignment predictions with local dynamic time warping refinement and XIC extraction
    run: ./proteomiqon-alignmentbasedquantification.cwl
    in:
      instrumentOutput: MzMLToMzlite/dir
      alignedPeptides: QuantBasedAlignment/dir
      alignmentMetrics: QuantBasedAlignment/dir
      quantifiedPeptides: PSMBasedQuantification/dir
      database: PeptideDB/db
      outputDirectory: outputAlignmentQuant
      params: paramsAlignmentBasedQuant
      parallelismLevel: cores
    out: [dir]
  AlignmentBasedQuantStatistics:
    label: Alignment-based quantification statistics
    doc: Computes statistics and quality metrics for alignment-based quantification results
    run: ./proteomiqon-alignmentbasedquantstatistics.cwl
    in:
      quantDirectory: PSMBasedQuantification/dir
      alignmentDirectory: QuantBasedAlignment/dir
      alignedQuantDirectory: AlignmentBasedQuantification/dir
      quantLearnDirectory: PSMBasedQuantification/dir
      alignmentLearnDirectory: QuantBasedAlignment/dir
      alignedQuantLearnDirectory: AlignmentBasedQuantification/dir
      outputDirectory: outputAlignmentStats
      params: paramsAlignmentBasedQuantStats
    out: [dir]
  AddDeducedPeptides:
    label: Add deduced peptides
    doc: Assigns protein inference information to peptides quantified via alignment, creating updated protein inference results for each quantification file
    run: ./proteomiqon-adddeducedpeptides.cwl
    in:
      quantDirectory: AlignmentBasedQuantification/dir
      proteinDirectory: ProteinInference/dir
      outputDirectory: outputProtDeduced
    out: [dir]
  JoinQuantPepIonsWithProteins:
    label: Join quantified peptide ions with proteins
    doc: Combines peptide quantification information with detailed protein inference results including q-values
    run: ./proteomiqon-joinquantpepionswithproteins.cwl
    in:
      inputDirectoryQuant: AlignmentBasedQuantStatistics/dir
      inputDirectoryProt: AddDeducedPeptides/dir
      outputDirectory: outputQuantAndProt
      parallelismLevel: cores
    out: [dir]
  LabeledProteinQuantification:
    label: Labeled protein quantification
    doc: Aggregates peptide-level quantification to protein-level, calculating ratios between light and heavy labeled peptides with optional charge and modification aggregation
    run: ./proteomiqon-labeledproteinquantification.cwl
    in:
      quantAndProtDirectory: JoinQuantPepIonsWithProteins/dir
      outputDirectory: outputLabeledProt
      params: paramsLabeledProteinQuant
    out: [dir]

outputs:
  db:
    type: Directory
    label: Database files
    doc: Created database
    outputSource: PeptideDB/dbFolder
  mzml:
    type: Directory
    label: mzLite files
    doc: Processed mzLite files
    outputSource: MzMLToMzlite/dir
  psm:
    type: Directory
    label: Peptide-spectrum matches
    doc: Peptide-spectrum match results
    outputSource: PeptideSpectrumMatching/dir
  psmstats:
    type: Directory
    label: PSM statistics
    doc: Statistical analysis of peptide-spectrum matches
    outputSource: PSMStatistics/dir
  quant:
    type: Directory
    label: Quantification results
    doc: PSM-based quantification results
    outputSource: PSMBasedQuantification/dir
  prot:
    type: Directory
    label: Protein identification results
    doc: Protein inference and identification results
    outputSource: ProteinInference/dir
  alignment:
    type: Directory
    label: Alignment results
    doc: Feature alignment results across samples
    outputSource: QuantBasedAlignment/dir
  alignmentQuant:
    type: Directory
    label: Alignment-based quantification
    doc: Quantification results based on feature alignment
    outputSource: AlignmentBasedQuantification/dir
  alignmentStats:
    type: Directory
    label: Alignment statistics
    doc: Statistical analysis of alignment-based quantification
    outputSource: AlignmentBasedQuantStatistics/dir
  protDeduced:
    type: Directory
    label: Deduced proteins
    doc: Deduced protein results from alignment-based quantification
    outputSource: AddDeducedPeptides/dir
  quantAndProt:
    type: Directory
    label: Combined quantification and protein results
    doc: Combined quantification and protein inference results
    outputSource: JoinQuantPepIonsWithProteins/dir
  labeledProteins:
    type: Directory
    label: Labeled protein quantification
    doc: Labeled protein quantification results
    outputSource: LabeledProteinQuantification/dir