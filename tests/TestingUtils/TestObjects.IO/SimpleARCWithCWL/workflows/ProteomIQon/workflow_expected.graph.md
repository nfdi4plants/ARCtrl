```mermaid
flowchart TD
    subgraph wg_initial_inputs[Initial Inputs]
        port_unit_workflow__in__cores(cores)
        port_unit_workflow__in__inputMzML(inputMzML)
        port_unit_workflow__in__inputPeptideDB(inputPeptideDB)
        port_unit_workflow__in__outputAlignment(outputAlignment)
        port_unit_workflow__in__outputAlignmentQuant(outputAlignmentQuant)
        port_unit_workflow__in__outputAlignmentStats(outputAlignmentStats)
        port_unit_workflow__in__outputDB(outputDB)
        port_unit_workflow__in__outputLabeledProt(outputLabeledProt)
        port_unit_workflow__in__outputMzML(outputMzML)
        port_unit_workflow__in__outputPSM(outputPSM)
        port_unit_workflow__in__outputPSMStats(outputPSMStats)
        port_unit_workflow__in__outputProt(outputProt)
        port_unit_workflow__in__outputProtDeduced(outputProtDeduced)
        port_unit_workflow__in__outputQuant(outputQuant)
        port_unit_workflow__in__outputQuantAndProt(outputQuantAndProt)
        port_unit_workflow__in__paramsAlignmentBasedQuant(paramsAlignmentBasedQuant)
        port_unit_workflow__in__paramsAlignmentBasedQuantStats(paramsAlignmentBasedQuantStats)
        port_unit_workflow__in__paramsDB(paramsDB)
        port_unit_workflow__in__paramsLabeledProteinQuant(paramsLabeledProteinQuant)
        port_unit_workflow__in__paramsMzML(paramsMzML)
        port_unit_workflow__in__paramsPSM(paramsPSM)
        port_unit_workflow__in__paramsPSMBasedQuant(paramsPSMBasedQuant)
        port_unit_workflow__in__paramsPSMStats(paramsPSMStats)
        port_unit_workflow__in__paramsProt(paramsProt)
    end
    unit_workflow__AddDeducedPeptides__run{{proteomiqon-adddeducedpeptides}}
    unit_workflow__AlignmentBasedQuantStatistics__run{{proteomiqon-alignmentbasedquantstatistics}}
    unit_workflow__AlignmentBasedQuantification__run{{proteomiqon-alignmentbasedquantification}}
    unit_workflow__JoinQuantPepIonsWithProteins__run{{proteomiqon-joinquantpepionswithproteins}}
    unit_workflow__LabeledProteinQuantification__run{{proteomiqon-labeledproteinquantification}}
    unit_workflow__MzMLToMzlite__run{{proteomiqon-mzmltomzlite}}
    unit_workflow__PSMBasedQuantification__run{{proteomiqon-psmbasedquantification}}
    unit_workflow__PSMStatistics__run{{proteomiqon-psmstatistics}}
    unit_workflow__PeptideDB__run{{proteomiqon-peptidedb}}
    unit_workflow__PeptideSpectrumMatching__run{{proteomiqon-peptidespectrummatching}}
    unit_workflow__ProteinInference__run{{proteomiqon-proteininference}}
    unit_workflow__QuantBasedAlignment__run{{proteomiqon-quantbasedalignment}}
    subgraph wg_final_outputs[Final Outputs]
        port_unit_workflow__out__alignment([alignment])
        port_unit_workflow__out__alignmentQuant([alignmentQuant])
        port_unit_workflow__out__alignmentStats([alignmentStats])
        port_unit_workflow__out__db([db])
        port_unit_workflow__out__labeledProteins([labeledProteins])
        port_unit_workflow__out__mzml([mzml])
        port_unit_workflow__out__prot([prot])
        port_unit_workflow__out__protDeduced([protDeduced])
        port_unit_workflow__out__psm([psm])
        port_unit_workflow__out__psmstats([psmstats])
        port_unit_workflow__out__quant([quant])
        port_unit_workflow__out__quantAndProt([quantAndProt])
    end
    port_unit_workflow__in__cores-->|parallelismLevel|unit_workflow__AlignmentBasedQuantification__run
    port_unit_workflow__in__cores-->|parallelismLevel|unit_workflow__JoinQuantPepIonsWithProteins__run
    port_unit_workflow__in__cores-->|parallelismLevel|unit_workflow__MzMLToMzlite__run
    port_unit_workflow__in__cores-->|parallelismLevel|unit_workflow__PSMBasedQuantification__run
    port_unit_workflow__in__cores-->|parallelismLevel|unit_workflow__PSMStatistics__run
    port_unit_workflow__in__cores-->|parallelismLevel|unit_workflow__PeptideSpectrumMatching__run
    port_unit_workflow__in__cores-->|parallelismLevel|unit_workflow__QuantBasedAlignment__run
    port_unit_workflow__in__inputMzML-->|inputDirectory|unit_workflow__MzMLToMzlite__run
    port_unit_workflow__in__inputPeptideDB-->|inputFile|unit_workflow__PeptideDB__run
    port_unit_workflow__in__outputAlignment-->|outputDirectory|unit_workflow__QuantBasedAlignment__run
    port_unit_workflow__in__outputAlignmentQuant-->|outputDirectory|unit_workflow__AlignmentBasedQuantification__run
    port_unit_workflow__in__outputAlignmentStats-->|outputDirectory|unit_workflow__AlignmentBasedQuantStatistics__run
    port_unit_workflow__in__outputDB-->|outputDirectory|unit_workflow__PeptideDB__run
    port_unit_workflow__in__outputLabeledProt-->|outputDirectory|unit_workflow__LabeledProteinQuantification__run
    port_unit_workflow__in__outputMzML-->|outputDirectory|unit_workflow__MzMLToMzlite__run
    port_unit_workflow__in__outputPSM-->|outputDirectory|unit_workflow__PeptideSpectrumMatching__run
    port_unit_workflow__in__outputPSMStats-->|outputDirectory|unit_workflow__PSMStatistics__run
    port_unit_workflow__in__outputProt-->|outputDirectory|unit_workflow__ProteinInference__run
    port_unit_workflow__in__outputProtDeduced-->|outputDirectory|unit_workflow__AddDeducedPeptides__run
    port_unit_workflow__in__outputQuant-->|outputDirectory|unit_workflow__PSMBasedQuantification__run
    port_unit_workflow__in__outputQuantAndProt-->|outputDirectory|unit_workflow__JoinQuantPepIonsWithProteins__run
    port_unit_workflow__in__paramsAlignmentBasedQuant-->|params|unit_workflow__AlignmentBasedQuantification__run
    port_unit_workflow__in__paramsAlignmentBasedQuantStats-->|params|unit_workflow__AlignmentBasedQuantStatistics__run
    port_unit_workflow__in__paramsDB-->|params|unit_workflow__PeptideDB__run
    port_unit_workflow__in__paramsLabeledProteinQuant-->|params|unit_workflow__LabeledProteinQuantification__run
    port_unit_workflow__in__paramsMzML-->|params|unit_workflow__MzMLToMzlite__run
    port_unit_workflow__in__paramsPSM-->|params|unit_workflow__PeptideSpectrumMatching__run
    port_unit_workflow__in__paramsPSMBasedQuant-->|params|unit_workflow__PSMBasedQuantification__run
    port_unit_workflow__in__paramsPSMStats-->|params|unit_workflow__PSMStatistics__run
    port_unit_workflow__in__paramsProt-->|params|unit_workflow__ProteinInference__run
    unit_workflow__AddDeducedPeptides__run-->port_unit_workflow__out__protDeduced
    unit_workflow__AlignmentBasedQuantStatistics__run-->port_unit_workflow__out__alignmentStats
    unit_workflow__AlignmentBasedQuantification__run-->port_unit_workflow__out__alignmentQuant
    unit_workflow__JoinQuantPepIonsWithProteins__run-->port_unit_workflow__out__quantAndProt
    unit_workflow__LabeledProteinQuantification__run-->port_unit_workflow__out__labeledProteins
    unit_workflow__MzMLToMzlite__run-->port_unit_workflow__out__mzml
    unit_workflow__PSMBasedQuantification__run-->port_unit_workflow__out__quant
    unit_workflow__PSMStatistics__run-->port_unit_workflow__out__psmstats
    unit_workflow__PeptideDB__run-->port_unit_workflow__out__db
    unit_workflow__PeptideSpectrumMatching__run-->port_unit_workflow__out__psm
    unit_workflow__ProteinInference__run-->port_unit_workflow__out__prot
    unit_workflow__QuantBasedAlignment__run-->port_unit_workflow__out__alignment
    unit_workflow__AddDeducedPeptides__run==>|inputDirectoryProt|unit_workflow__JoinQuantPepIonsWithProteins__run
    unit_workflow__AlignmentBasedQuantStatistics__run==>|inputDirectoryQuant|unit_workflow__JoinQuantPepIonsWithProteins__run
    unit_workflow__AlignmentBasedQuantification__run==>|quantDirectory|unit_workflow__AddDeducedPeptides__run
    unit_workflow__AlignmentBasedQuantification__run==>|alignedQuantDirectory|unit_workflow__AlignmentBasedQuantStatistics__run
    unit_workflow__AlignmentBasedQuantification__run==>|alignedQuantLearnDirectory|unit_workflow__AlignmentBasedQuantStatistics__run
    unit_workflow__JoinQuantPepIonsWithProteins__run==>|quantAndProtDirectory|unit_workflow__LabeledProteinQuantification__run
    unit_workflow__MzMLToMzlite__run==>|instrumentOutput|unit_workflow__AlignmentBasedQuantification__run
    unit_workflow__MzMLToMzlite__run==>|inputDirectoryI|unit_workflow__PSMBasedQuantification__run
    unit_workflow__MzMLToMzlite__run==>|inputDirectory|unit_workflow__PeptideSpectrumMatching__run
    unit_workflow__PSMBasedQuantification__run==>|quantDirectory|unit_workflow__AlignmentBasedQuantStatistics__run
    unit_workflow__PSMBasedQuantification__run==>|quantLearnDirectory|unit_workflow__AlignmentBasedQuantStatistics__run
    unit_workflow__PSMBasedQuantification__run==>|quantifiedPeptides|unit_workflow__AlignmentBasedQuantification__run
    unit_workflow__PSMBasedQuantification__run==>|inputSources|unit_workflow__QuantBasedAlignment__run
    unit_workflow__PSMBasedQuantification__run==>|inputTargets|unit_workflow__QuantBasedAlignment__run
    unit_workflow__PSMStatistics__run==>|inputDirectoryII|unit_workflow__PSMBasedQuantification__run
    unit_workflow__PSMStatistics__run==>|inputDirectory|unit_workflow__ProteinInference__run
    unit_workflow__PeptideDB__run==>|database|unit_workflow__AlignmentBasedQuantification__run
    unit_workflow__PeptideDB__run==>|database|unit_workflow__PSMBasedQuantification__run
    unit_workflow__PeptideDB__run==>|database|unit_workflow__PSMStatistics__run
    unit_workflow__PeptideDB__run==>|database|unit_workflow__PeptideSpectrumMatching__run
    unit_workflow__PeptideDB__run==>|database|unit_workflow__ProteinInference__run
    unit_workflow__PeptideSpectrumMatching__run==>|inputDirectory|unit_workflow__PSMStatistics__run
    unit_workflow__ProteinInference__run==>|proteinDirectory|unit_workflow__AddDeducedPeptides__run
    unit_workflow__QuantBasedAlignment__run==>|alignmentDirectory|unit_workflow__AlignmentBasedQuantStatistics__run
    unit_workflow__QuantBasedAlignment__run==>|alignmentLearnDirectory|unit_workflow__AlignmentBasedQuantStatistics__run
    unit_workflow__QuantBasedAlignment__run==>|alignedPeptides|unit_workflow__AlignmentBasedQuantification__run
    unit_workflow__QuantBasedAlignment__run==>|alignmentMetrics|unit_workflow__AlignmentBasedQuantification__run
    classDef wg_workflow fill:#dff4ff,stroke:#246fa8,stroke-width:2px;
    classDef wg_tool fill:#e8f5e9,stroke:#2e7d32,stroke-width:2px;
    classDef wg_expression fill:#fff3e0,stroke:#e65100,stroke-width:2px;
    classDef wg_unresolved fill:#ffebee,stroke:#c62828,stroke-width:2px,stroke-dasharray:5 5;
    classDef wg_initial_input fill:#e1f5fe,stroke:#0288d1,stroke-width:2px;
    classDef wg_final_output fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px;
    class unit_workflow__AddDeducedPeptides__run,unit_workflow__AlignmentBasedQuantStatistics__run,unit_workflow__AlignmentBasedQuantification__run,unit_workflow__JoinQuantPepIonsWithProteins__run,unit_workflow__LabeledProteinQuantification__run,unit_workflow__MzMLToMzlite__run,unit_workflow__PSMBasedQuantification__run,unit_workflow__PSMStatistics__run,unit_workflow__PeptideDB__run,unit_workflow__PeptideSpectrumMatching__run,unit_workflow__ProteinInference__run,unit_workflow__QuantBasedAlignment__run wg_tool;
    class port_unit_workflow__in__cores,port_unit_workflow__in__inputMzML,port_unit_workflow__in__inputPeptideDB,port_unit_workflow__in__outputAlignment,port_unit_workflow__in__outputAlignmentQuant,port_unit_workflow__in__outputAlignmentStats,port_unit_workflow__in__outputDB,port_unit_workflow__in__outputLabeledProt,port_unit_workflow__in__outputMzML,port_unit_workflow__in__outputPSM,port_unit_workflow__in__outputPSMStats,port_unit_workflow__in__outputProt,port_unit_workflow__in__outputProtDeduced,port_unit_workflow__in__outputQuant,port_unit_workflow__in__outputQuantAndProt,port_unit_workflow__in__paramsAlignmentBasedQuant,port_unit_workflow__in__paramsAlignmentBasedQuantStats,port_unit_workflow__in__paramsDB,port_unit_workflow__in__paramsLabeledProteinQuant,port_unit_workflow__in__paramsMzML,port_unit_workflow__in__paramsPSM,port_unit_workflow__in__paramsPSMBasedQuant,port_unit_workflow__in__paramsPSMStats,port_unit_workflow__in__paramsProt wg_initial_input;
    class port_unit_workflow__out__alignment,port_unit_workflow__out__alignmentQuant,port_unit_workflow__out__alignmentStats,port_unit_workflow__out__db,port_unit_workflow__out__labeledProteins,port_unit_workflow__out__mzml,port_unit_workflow__out__prot,port_unit_workflow__out__protDeduced,port_unit_workflow__out__psm,port_unit_workflow__out__psmstats,port_unit_workflow__out__quant,port_unit_workflow__out__quantAndProt wg_final_output;

```