module TestObjects.Investigation

open FsSpreadsheet.DSL
open FsSpreadsheet 

let emptyInvestigation = 
    workbook {
        sheet "Investigation" {
             row {
                  "ONTOLOGY SOURCE REFERENCE"
             }
             row {
                  "Term Source Name"
             }
             row {
                  "Term Source File"
             }
             row {
                  "Term Source Version"
             }
             row {
                  "Term Source Description"
             }
             row {
                  "INVESTIGATION"
             }
             row {
                  "Investigation Identifier"
             }
             row {
                  "Investigation Title"
             }
             row {
                  "Investigation Description"
             }
             row {
                  "Investigation Submission Date"
             }
             row {
                  "Investigation Public Release Date"
             }
             row {
                  "INVESTIGATION PUBLICATIONS"
             }
             row {
                  "Investigation Publication PubMed ID"
             }
             row {
                  "Investigation Publication DOI"
             }
             row {
                  "Investigation Publication Author List"
             }
             row {
                  "Investigation Publication Title"
             }
             row {
                  "Investigation Publication Status"
             }
             row {
                  "Investigation Publication Status Term Accession Number"
             }
             row {
                  "Investigation Publication Status Term Source REF"
             }
             row {
                  "INVESTIGATION CONTACTS"
             }
             row {
                  "Investigation Person Last Name"
             }
             row {
                  "Investigation Person First Name"
             }
             row {
                  "Investigation Person Mid Initials"
             }
             row {
                  "Investigation Person Email"
             }
             row {
                  "Investigation Person Phone"
             }
             row {
                  "Investigation Person Fax"
             }
             row {
                  "Investigation Person Address"
             }
             row {
                  "Investigation Person Affiliation"
             }
             row {
                  "Investigation Person Roles"
             }
             row {
                  "Investigation Person Roles Term Accession Number"
             }
             row {
                  "Investigation Person Roles Term Source REF"
             }
             row {
                  "STUDY"
             }
             row {
                  "Study Identifier"
             }
             row {
                  "Study Title"
             }
             row {
                  "Study Description"
             }
             row {
                  "Study Submission Date"
             }
             row {
                  "Study Public Release Date"
             }
             row {
                  "Study File Name"
             }
             row {
                  "STUDY DESIGN DESCRIPTORS"
             }
             row {
                  "Study Design Type"
             }
             row {
                  "Study Design Type Term Accession Number"
             }
             row {
                  "Study Design Type Term Source REF"
             }
             row {
                  "STUDY PUBLICATIONS"
             }
             row {
                  "Study Publication PubMed ID"
             }
             row {
                  "Study Publication DOI"
             }
             row {
                  "Study Publication Author List"
             }
             row {
                  "Study Publication Title"
             }
             row {
                  "Study Publication Status"
             }
             row {
                  "Study Publication Status Term Accession Number"
             }
             row {
                  "Study Publication Status Term Source REF"
             }
             row {
                  "STUDY FACTORS"
             }
             row {
                  "Study Factor Name"
             }
             row {
                  "Study Factor Type"
             }
             row {
                  "Study Factor Type Term Accession Number"
             }
             row {
                  "Study Factor Type Term Source REF"
             }
             row {
                  "STUDY ASSAYS"
             }
             row {
                  "Study Assay Measurement Type"
             }
             row {
                  "Study Assay Measurement Type Term Accession Number"
             }
             row {
                  "Study Assay Measurement Type Term Source REF"
             }
             row {
                  "Study Assay Technology Type"
             }
             row {
                  "Study Assay Technology Type Term Accession Number"
             }
             row {
                  "Study Assay Technology Type Term Source REF"
             }
             row {
                  "Study Assay Technology Platform"
             }
             row {
                  "Study Assay File Name"
             }
             row {
                  "STUDY PROTOCOLS"
             }
             row {
                  "Study Protocol Name"
             }
             row {
                  "Study Protocol Type"
             }
             row {
                  "Study Protocol Type Term Accession Number"
             }
             row {
                  "Study Protocol Type Term Source REF"
             }
             row {
                  "Study Protocol Description"
             }
             row {
                  "Study Protocol URI"
             }
             row {
                  "Study Protocol Version"
             }
             row {
                  "Study Protocol Parameters Name"
             }
             row {
                  "Study Protocol Parameters Term Accession Number"
             }
             row {
                  "Study Protocol Parameters Term Source REF"
             }
             row {
                  "Study Protocol Components Name"
             }
             row {
                  "Study Protocol Components Type"
             }
             row {
                  "Study Protocol Components Type Term Accession Number"
             }
             row {
                  "Study Protocol Components Type Term Source REF"
             }
             row {
                  "STUDY CONTACTS"
             }
             row {
                  "Study Person Last Name"
             }
             row {
                  "Study Person First Name"
             }
             row {
                  "Study Person Mid Initials"
             }
             row {
                  "Study Person Email"
             }
             row {
                  "Study Person Phone"
             }
             row {
                  "Study Person Fax"
             }
             row {
                  "Study Person Address"
             }
             row {
                  "Study Person Affiliation"
             }
             row {
                  "Study Person Roles"
             }
             row {
                  "Study Person Roles Term Accession Number"
             }
             row {
                  "Study Person Roles Term Source REF"
             }
         }
    }
    |> fun wb -> wb.Value.Parse()

let fullInvestigation =
    workbook {
        sheet "Investigation" {
             row {
                  "ONTOLOGY SOURCE REFERENCE"
             }
             row {
                  "Term Source Name"
                  "OBI"
                  "BTO"
                  "NEWT"
                  "UO"
                  "CHEBI"
                  "PATO"
                  "EFO"
             }
             row {
                  "#TestRemark1"
             }
             row {
                  "Term Source File"
                  "http://bioportal.bioontology.org/ontologies/1123"
                  "ArrayExpress Experimental Factor Ontology"
             }
             row {
                  "Term Source Version"
                  "47893"
                  "v 1.26"
                  "v 1.26"
                  "v 1.26"
                  "v 1.26"
                  "v 1.26"
                  "v 1.26"
             }
             row {
                  "Term Source Description"
                  "Ontology for Biomedical Investigations"
                  "BRENDA tissue / enzyme source"
                  "NEWT UniProt Taxonomy Database"
                  "Unit Ontology"
                  "Chemical Entities of Biological Interest"
                  "Phenotypic qualities (properties)"
                  "ArrayExpress Experimental Factor Ontology"
             }
             row {
                  "INVESTIGATION"
             }
             row {
                  "Investigation Identifier"
                  "BII-I-1"
             }
             row {
                  "Investigation Title"
                  "Growth control of the eukaryote cell: a systems biology study in yeast"
             }
             row {
                  "Investigation Description"
                  "Background Cell growth underlies many key cellular and developmental processes, yet a limited number of studies have been carried out on cell-growth regulation. Comprehensive studies at the transcriptional, proteomic and metabolic levels under defined controlled conditions are currently lacking. Results Metabolic control analysis is being exploited in a systems biology study of the eukaryotic cell. Using chemostat culture, we have measured the impact of changes in flux (growth rate) on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae. Each functional genomic level shows clear growth-rate-associated trends and discriminates between carbon-sufficient and carbon-limited conditions. Genes consistently and significantly upregulated with increasing growth rate are frequently essential and encode evolutionarily conserved proteins of known function that participate in many protein-protein interactions. In contrast, more unknown, and fewer essential, genes are downregulated with increasing growth rate; their protein products rarely interact with one another. A large proportion of yeast genes under positive growth-rate control share orthologs with other eukaryotes, including humans. Significantly, transcription of genes encoding components of the TOR complex (a major controller of eukaryotic cell growth) is not subject to growth-rate regulation. Moreover, integrative studies reveal the extent and importance of post-transcriptional control, patterns of control of metabolic fluxes at the level of enzyme synthesis, and the relevance of specific enzymatic reactions in the control of metabolic fluxes during cell growth. Conclusion This work constitutes a first comprehensive systems biology study on growth-rate control in the eukaryotic cell. The results have direct implications for advanced studies on cell growth, in vivo regulation of metabolic fluxes for comprehensive metabolic engineering, and for the design of genome-scale systems biology models of the eukaryotic cell."
             }
             row {
                  "Investigation Submission Date"
                  "2007-04-30"
             }
             row {
                  "Investigation Public Release Date"
                  "2009-03-10"
             }
             row {
                  "Comment[<Created With Configuration>]"
             }
             row {
                  "Comment[<Last Opened With Configuration>]"
                  "isaconfig-default_v2013-02-13"
             }
             row {
                  "INVESTIGATION PUBLICATIONS"
             }
             row {
                  "Investigation Publication PubMed ID"
                  "17439666"
             }
             row {
                  "Investigation Publication DOI"
                  "doi:10.1186/jbiol54"
             }
             row {
                  "Investigation Publication Author List"
                  "Castrillo JI, Zeef LA, Hoyle DC, Zhang N, Hayes A, Gardner DC, Cornell MJ, Petty J, Hakes L, Wardleworth L, Rash B, Brown M, Dunn WB, Broadhurst D, O'Donoghue K, Hester SS, Dunkley TP, Hart SR, Swainston N, Li P, Gaskell SJ, Paton NW, Lilley KS, Kell DB, Oliver SG."
             }
             row {
                  "Investigation Publication Title"
                  "Growth control of the eukaryote cell: a systems biology study in yeast."
             }
             row {
                  "Investigation Publication Status"
                  "indexed in Pubmed"
             }
             row {
                  "Investigation Publication Status Term Accession Number"
             }
             row {
                  "Investigation Publication Status Term Source REF"
             }
             row {
                  "INVESTIGATION CONTACTS"
             }
             row {
                  "Investigation Person Last Name"
                  "Stephen"
                  "Castrillo"
                  "Zeef"
             }
             row {
                  "Investigation Person First Name"
                  "Oliver"
                  "Juan"
                  "Leo"
             }
             row {
                  "Investigation Person Mid Initials"
                  "G"
                  "I"
                  "A"
             }
             row {
                  "Investigation Person Email"
             }
             row {
                  "Investigation Person Phone"
             }
             row {
                  "Investigation Person Fax"
             }
             row {
                  "Investigation Person Address"
                  "Oxford Road, Manchester M13 9PT, UK"
                  "Oxford Road, Manchester M13 9PT, UK"
                  "Oxford Road, Manchester M13 9PT, UK"
             }
             row {
                  "Investigation Person Affiliation"
                  "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
                  "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
                  "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
             }
             row {
                  "Investigation Person Roles"
                  "corresponding author"
                  "author"
                  "author"
             }
             row {
                  "Investigation Person Roles Term Accession Number"
             }
             row {
                  "Investigation Person Roles Term Source REF"
             }
             row {
                  "Comment[<Investigation Person REF>]"
             }
             row {
                  "STUDY"
             }
             row {
                  "Study Identifier"
                  "BII-S-1"
             }
             row {
                  "Study Title"
                  "Study of the impact of changes in flux on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae under different nutrient limitations"
             }
             row {
                  "Study Description"
                  "We wished to study the impact of growth rate on the total complement of mRNA molecules, proteins, and metabolites in S. cerevisiae, independent of any nutritional or other physiological effects. To achieve this, we carried out our analyses on yeast grown in steady-state chemostat culture under four different nutrient limitations (glucose, ammonium, phosphate, and sulfate) at three different dilution (that is, growth) rates (D = u = 0.07, 0.1, and 0.2/hour, equivalent to population doubling times (Td) of 10 hours, 7 hours, and 3.5 hours, respectively; u = specific growth rate defined as grams of biomass generated per gram of biomass present per unit time)."
             }
             row {
                  "Study Submission Date"
                  "2007-04-30"
             }
             row {
                  "Study Public Release Date"
                  "2009-03-10"
             }
             row {
                  "Study File Name"
                  "s_BII-S-1.txt"
             }
             row {
                  "Comment[<Study Grant Number>]"
             }
             row {
                  "Comment[<Study Funding Agency>]"
             }
             row {
                  "STUDY DESIGN DESCRIPTORS"
             }
             row {
                  "Study Design Type"
                  "intervention design"
             }
             row {
                  "#TestRemark2"
             }
             row {
                  "Study Design Type Term Accession Number"
                  "http://purl.obolibrary.org/obo/OBI_0000115"
             }
             row {
                  "Study Design Type Term Source REF"
                  "OBI"
             }
             row {
                  "STUDY PUBLICATIONS"
             }
             row {
                  "Study Publication PubMed ID"
                  "17439666"
             }
             row {
                  "Study Publication DOI"
                  "doi:10.1186/jbiol54"
             }
             row {
                  "Study Publication Author List"
                  "Castrillo JI, Zeef LA, Hoyle DC, Zhang N, Hayes A, Gardner DC, Cornell MJ, Petty J, Hakes L, Wardleworth L, Rash B, Brown M, Dunn WB, Broadhurst D, O'Donoghue K, Hester SS, Dunkley TP, Hart SR, Swainston N, Li P, Gaskell SJ, Paton NW, Lilley KS, Kell DB, Oliver SG."
             }
             row {
                  "Study Publication Title"
                  "Growth control of the eukaryote cell: a systems biology study in yeast."
             }
             row {
                  "Study Publication Status"
                  "published"
             }
             row {
                  "Study Publication Status Term Accession Number"
             }
             row {
                  "Study Publication Status Term Source REF"
             }
             row {
                  "STUDY FACTORS"
             }
             row {
                  "Study Factor Name"
                  "limiting nutrient"
                  "rate"
             }
             row {
                  "Study Factor Type"
                  "chemical compound"
                  "rate"
             }
             row {
                  "Study Factor Type Term Accession Number"
                  "http://purl.obolibrary.org/obo/PATO_0000161"
             }
             row {
                  "Study Factor Type Term Source REF"
                  "PATO"
             }
             row {
                  "STUDY ASSAYS"
             }
             row {
                  "Study Assay Measurement Type"
                  "protein expression profiling"
                  "metabolite profiling"
                  "transcription profiling"
             }
             row {
                  "Study Assay Measurement Type Term Accession Number"
                  "http://purl.obolibrary.org/obo/OBI_0000615"
                  "http://purl.obolibrary.org/obo/OBI_0000366"
                  "424"
             }
             row {
                  "Study Assay Measurement Type Term Source REF"
                  "OBI"
                  "OBI"
                  "OBI"
             }
             row {
                  "Study Assay Technology Type"
                  "mass spectrometry"
                  "mass spectrometry"
                  "DNA microarray"
             }
             row {
                  "Study Assay Technology Type Term Accession Number"
                  "http://purl.obolibrary.org/obo/OBI_0400148"
             }
             row {
                  "Study Assay Technology Type Term Source REF"
                  "OBI"
                  "OBI"
                  "OBI"
             }
             row {
                  "Study Assay Technology Platform"
                  "iTRAQ"
                  "LC-MS/MS"
                  "Affymetrix"
             }
             row {
                  "Study Assay File Name"
                  "a_proteome.txt"
                  "a_metabolome.txt"
                  "a_transcriptome.txt"
             }
             row {
                  "STUDY PROTOCOLS"
             }
             row {
                  "Study Protocol Name"
                  "growth protocol"
                  "mRNA extraction"
                  "protein extraction"
                  "biotin labeling"
                  "ITRAQ labeling"
                  "EukGE-WS4"
                  "metabolite extraction"
             }
             row {
                  "Study Protocol Type"
                  "growth"
                  "mRNA extraction"
                  "protein extraction"
                  "labeling"
                  "labeling"
                  "hybridization"
                  "extraction"
             }
             row {
                  "Study Protocol Type Term Accession Number"
                  "http://purl.obolibrary.org/obo/OBI_0302884"
             }
             row {
                  "Study Protocol Type Term Source REF"
                  "OBI"
             }
             row {
                  "Study Protocol Description"
                  "1. Biomass samples (45 ml) were taken via the sample port of the Applikon fermenters. The cells were pelleted by centrifugation for 5 min at 5000 rpm. The supernatant was removed and the RNA pellet resuspended in the residual medium to form a slurry. This was added in a dropwise manner directly into a 5 ml Teflon flask (B. Braun Biotech, Germany) containing liquid nitrogen and a 7 mm-diameter tungsten carbide ball. After allowing evaporation of the liquid nitrogen the flask was reassembled and the cells disrupted by agitation at 1500 rpm for 2 min in a Microdismembranator U (B. Braun Biotech, Germany) 2. The frozen powder was then dissolved in 1 ml of TriZol reagent (Sigma-Aldrich, UK), vortexed for 1 min, and then kept at room temperature for a further 5min. 3. Chloroform extraction was performed by addition of 0.2 ml chloroform, shaking vigorously or 15 s, then 5min incubation at room temperature. 4. Following centrifugation at 12,000 rpm for 5 min, the RNA (contained in the aqueous phase) was precipitated with 0.5 vol of 2-propanol at room temperature for 15 min. 5. After further centrifugation (12,000 rpm for 10 min at 4 C) the RNA pellet was washed twice with 70 % (v/v) ethanol, briefly air-dried, and redissolved in 0.5 ml diethyl pyrocarbonate (DEPC)-treated water. 6. The single-stranded RNA was precipitated once more by addition of 0.5 ml of LiCl buffer (4 M LiCl, 20 mM Tris-HCl, pH 7.5, 10 mM EDTA), thus removing tRNA and DNA from the sample. 7. After precipitation (20 C for 1h) and centrifugation (12,000 rpm, 30 min, 4 C), the RNA was washed twice in 70 % (v/v) ethanol prior to being dissolved in a minimal volume of DEPC-treated water. 8. Total RNA quality was checked using the RNA 6000 Nano Assay, and analysed on an Agilent 2100 Bioanalyser (Agilent Technologies). RNA was quantified using the Nanodrop ultra low volume spectrophotometer (Nanodrop Technologies)."
                  "1. Biomass samples (45 ml) were taken via the sample port of the Applikon fermenters. The cells were pelleted by centrifugation for 5 min at 5000 rpm. The supernatant was removed and the RNA pellet resuspended in the residual medium to form a slurry. This was added in a dropwise manner directly into a 5 ml Teflon flask (B. Braun Biotech, Germany) containing liquid nitrogen and a 7 mm-diameter tungsten carbide ball. After allowing evaporation of the liquid nitrogen the flask was reassembled and the cells disrupted by agitation at 1500 rpm for 2 min in a Microdismembranator U (B. Braun Biotech, Germany) 2. The frozen powder was then dissolved in 1 ml of TriZol reagent (Sigma-Aldrich, UK), vortexed for 1 min, and then kept at room temperature for a further 5min. 3. Chloroform extraction was performed by addition of 0.2 ml chloroform, shaking vigorouslyor 15 s, then 5min incubation at room temperature. 4. Following centrifugation at 12,000 rpm for 5 min, the RNA (contained in the aqueous phase) was precipitated with 0.5 vol of 2-propanol at room temperature for 15 min. 5. After further centrifugation (12,000 rpm for 10 min at 4 C) the RNA pellet was washed twice with 70 % (v/v) ethanol, briefly air-dried, and redissolved in 0.5 ml diethyl pyrocarbonate (DEPC)-treated water. 6. The single-stranded RNA was precipitated once more by addition of 0.5 ml of LiCl bffer (4 M LiCl, 20 mM Tris-HCl, pH 7.5, 10 mM EDTA), thus removing tRNA and DNA from the sample. 7. After precipitation (20 C for 1 h) and centrifugation (12,000 rpm, 30 min, 4 C), the RNA was washed twice in 70 % (v/v) ethanol prior to being dissolved in a minimal volume of DEPC-treated water. 8. Total RNA quality was checked using the RNA 6000 Nano Assay, and analysed on an Agilent 2100 Bioanalyser (Agilent Technologies). RNA was quantified using the Nanodrop ultra low volume spectrophotometer (Nanodrop Technologies)."
                  "This was done using Enzo BioArrayTM HighYieldTM RNA transcript labelling kit (T7) with 5 ul cDNA. The resultant cRNA was again purified using the GeneChip Sample Clean Up Module. The column was eluted in the first instance using 10 l RNase-free water, and for a second time using 11 ul RNase-free water. cRNA was quantified using the Nanodrop spectrophotometer. A total of 15 ug of cRNA (required for hybridisation) was fragmented. Fragmentation was carried out by using 2 ul of fragmentation buffer for every 8 ul cRNA."
                  "For each target, a hybridisation cocktail was made using the standard array recipe as described in the GeneChip Expression Analysis technical manual. GeneChip control oligonucleotide and 20x eukaryotic hybridisation controls were used. Hybridisation buffer was made as detailed in the GeneChip manual and the BSA and herring sperm DNA was purchased from Invitrogen. The cocktail was heated to 99 C for 5 min, transferred to 45 C for 5 min and then spun for 5 min to remove any insoluble material. Affymetrix Yeast Yg_s98 S. cerevisiae arrays were pre-hybridised with 200 ul 1x hybridisation buffer and incubated at 45 C for 10 min. 200 ul of the hybridisation cocktail was loaded onto the arrays. The probe array was incubated in a rotisserie at 45 C, rotating at 60 rpm. Following hybridisation, for 16 hr, chips were loaded onto a Fluidics station for washing and staining using the EukGe WS2v4 programme controlled using Microarray Suite 5 software."
             }
             row {
                  "Study Protocol URI"
             }
             row {
                  "Study Protocol Version"
             }
             row {
                  "Study Protocol Parameters Name"
                  "sample volume;standard volume"
             }
             row {
                  "Study Protocol Parameters Term Accession Number"
                  ";"
             }
             row {
                  "Study Protocol Parameters Term Source REF"
                  ";"
             }
             row {
                  "Study Protocol Components Name"
             }
             row {
                  "Study Protocol Components Type"
             }
             row {
                  "Study Protocol Components Type Term Accession Number"
             }
             row {
                  "Study Protocol Components Type Term Source REF"
             }
             row {
                  "STUDY CONTACTS"
             }
             row {
                  "Study Person Last Name"
                  "Oliver"
                  "Juan"
                  "Leo"
             }
             row {
                  "Study Person First Name"
                  "Stephen"
                  "Castrillo"
                  "Zeef"
             }
             row {
                  "Study Person Mid Initials"
                  "G"
                  "I"
                  "A"
             }
             row {
                  "Study Person Email"
             }
             row {
                  "Study Person Phone"
             }
             row {
                  "Study Person Fax"
             }
             row {
                  "Study Person Address"
                  "Oxford Road, Manchester M13 9PT, UK"
                  "Oxford Road, Manchester M13 9PT, UK"
                  "Oxford Road, Manchester M13 9PT, UK"
             }
             row {
                  "Study Person Affiliation"
                  "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
                  "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
                  "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
             }
             row {
                  "Study Person Roles"
                  "corresponding author"
                  "author"
                  "author"
             }
             row {
                  "Study Person Roles Term Accession Number"
             }
             row {
                  "Study Person Roles Term Source REF"
             }
             row {
                  "Comment[<Study Person REF>]"
             }
             row {
                  "STUDY"
             }
             row {
                  "#TestRemark3"
             }
             row {
                  "Study Identifier"
                  "BII-S-2"
             }
             row {
                  "Study Title"
                  "A time course analysis of transcription response in yeast treated with rapamycin, a specific inhibitor of the TORC1 complex: impact on yeast growth"
             }
             row {
                  "Study Description"
                  "Comprehensive high-throughput analyses at the levels of mRNAs, proteins, and metabolites, and studies on gene expression patterns are required for systems biology studies of cell growth [4,26-29]. Although such comprehensive data sets are lacking, many studies have pointed to a central role for the target-of-rapamycin (TOR) signal transduction pathway in growth control. TOR is a serine/threonine kinase that has been conserved from yeasts to mammals; it integrates signals from nutrients or growth factors to regulate cell growth and cell-cycle progression coordinately. Although such comprehensive data sets are lacking, many studies have pointed to a central role for the target-of-rapamycin (TOR) signal transduction pathway in growth control. TOR is a serine/threonine kinase that has been conserved from yeasts to mammals; it integrates signals from nutrients or growth factors to regulate cell growth and cell-cycle progression coordinately. The effect of rapamycin were studied as follows: a culture growing at mid-exponential phase was divided into two. Rapamycin (200 ng/ml) was added to one half, and the drug's solvent to the other, as the control. Samples were taken at 0, 1, 2 and 4 h after treatment. Gene expression at the mRNA level was investigated by transcriptome analysis using Affymetrix hybridization arrays."
             }
             row {
                  "Study Submission Date"
                  "2007-04-30"
             }
             row {
                  "Study Public Release Date"
                  "2009-03-10"
             }
             row {
                  "Study File Name"
                  "s_BII-S-2.txt"
             }
             row {
                  "Comment[<Study Grant Number>]"
             }
             row {
                  "Comment[<Study Funding Agency>]"
             }
             row {
                  "STUDY DESIGN DESCRIPTORS"
             }
             row {
                  "Study Design Type"
                  "time series design"
             }
             row {
                  "Study Design Type Term Accession Number"
                  "http://purl.obolibrary.org/obo/OBI_0500020"
             }
             row {
                  "Study Design Type Term Source REF"
                  "OBI"
             }
             row {
                  "STUDY PUBLICATIONS"
             }
             row {
                  "Study Publication PubMed ID"
                  "17439666"
             }
             row {
                  "Study Publication DOI"
             }
             row {
                  "Study Publication Author List"
             }
             row {
                  "Study Publication Title"
                  "Growth control of the eukaryote cell: a systems biology study in yeast."
             }
             row {
                  "Study Publication Status"
                  "indexed in Pubmed"
             }
             row {
                  "Study Publication Status Term Accession Number"
             }
             row {
                  "Study Publication Status Term Source REF"
             }
             row {
                  "STUDY FACTORS"
             }
             row {
                  "Study Factor Name"
                  "compound"
                  "exposure time"
                  "dose"
             }
             row {
                  "Study Factor Type"
                  "compound"
                  "time"
                  "dose"
             }
             row {
                  "Study Factor Type Term Accession Number"
                  "http://www.ebi.ac.uk/efo/EFO_0000721"
                  "http://www.ebi.ac.uk/efo/EFO_0000428"
             }
             row {
                  "Study Factor Type Term Source REF"
                  "EFO"
                  "EFO"
             }
             row {
                  "STUDY ASSAYS"
             }
             row {
                  "Study Assay Measurement Type"
                  "transcription profiling"
             }
             row {
                  "Study Assay Measurement Type Term Accession Number"
                  "424"
             }
             row {
                  "Study Assay Measurement Type Term Source REF"
                  "OBI"
             }
             row {
                  "Study Assay Technology Type"
                  "DNA microarray"
             }
             row {
                  "Study Assay Technology Type Term Accession Number"
                  "http://purl.obolibrary.org/obo/OBI_0400148"
             }
             row {
                  "Study Assay Technology Type Term Source REF"
                  "OBI"
             }
             row {
                  "Study Assay Technology Platform"
                  "Affymetrix"
             }
             row {
                  "Study Assay File Name"
                  "a_microarray.txt"
             }
             row {
                  "STUDY PROTOCOLS"
             }
             row {
                  "Study Protocol Name"
                  "EukGE-WS4"
                  "growth"
                  "mRNA extraction"
                  "biotin labeling"
             }
             row {
                  "Study Protocol Type"
                  "hybridization"
                  "growth"
                  "mRNA extraction"
                  "labeling"
             }
             row {
                  "Study Protocol Type Term Accession Number"
             }
             row {
                  "Study Protocol Type Term Source REF"
             }
             row {
                  "Study Protocol Description"
                  "For each target, a hybridisation cocktail was made using the standard array recipe as described in the GeneChip Expression Analysis technical manual. GeneChip control oligonucleotide and 20x eukaryotic hybridisation controls were used. Hybridisation buffer was made as detailed in the GeneChip manual and the BSA and herring sperm DNA was purchased from Invitrogen. The cocktail was heated to 99 C for 5mins, transferred to 45 C for 5 min and then spun for 5 min to remove any insoluble material. Affymetrix Yeast Yg_s98 S. cerevisiae arrays were pre-hybridised with 200 l 1x hybridisation buffer and incubated at 45 C for 10 min. 200 l of the hybridisation cocktail was loaded onto the arrays. The probe array was incubated in a rotisserie at 45 C, rotating at 60 rpm. Following hybridisation, for 16hr, chips were loaded onto a Fluidics station for washing and staining using the EukGe WS2v4 programme controlled using Microarray Suite 5 software."
                  "The culture was grown in YMB minimum media + 2% glucose + supplement to early exponential growth (OD ~0.32)"
                  "1. Biomass samples (45ml) were taken via the sample port of the Applikon fermenters. The cells were pelleted by centrifugation for 5min at 5000 rpm. The supernatant was removed and the RNA pellet resuspended in the residual medium to form a slurry. This was added in a dropwise manner directly into a 5ml Teflon flask (B. Braun Biotech, Germany) containing liquid nitrogen and a 7 mm-diameter tungsten carbide ball. After allowing evaporation of the liquid nitrogen the flask was reassembled and the cells disrupted by agitation at 1500 rpm for 2 min in a Microdismembranator U (B. Braun Biotech, Germany) 2. The frozen powder was then dissolved in 1 ml of TriZol reagent (Sigma-Aldrich, UK), vortexed for 1 min, and then kept at room temperature for a further 5 min. 3. Chloroform extraction was performed by addition of 0.2 ml chloroform, shaking vigorouslyor 15 s, then 5 min incubation at room temperature. 4. Following centrifugation at 12,000 rpm for 5 min, the RNA (contained in the aqueous phase) was precipitated with 0.5 vol of 2-propanol at room temperature for 15 min. 5. After further centrifugation (12,000 rpm for 10 min at 4 C) the RNA pellet was washed twice with 70 % (v/v) ethanol, briefly air-dried, and redissolved in 0.5 ml diethyl pyrocarbonate (DEPC)-treated water. 6. The single-stranded RNA was precipitated once more by addition of 0.5 ml of LiCl bffer (4 M LiCl, 20 mM Tris-HCl, pH 7.5, 10 mM EDTA), thus removing tRNA and DNA from the sample. 7. After precipitation (20 C for 1h) and centrifugation (12,000 rpm, 30 min, 4 C), the RNA was washed twice in 70 % (v/v) ethanol prior to being dissolved in a minimal volume of DEPC-treated water. 8. Total RNA quality was checked using the RNA 6000 Nano Assay, and analysed on an Agilent 2100 Bioanalyser (Agilent Technologies). RNA was quantified using the Nanodrop ultra low volume spectrophotometer (Nanodrop Technologies)."
                  "This was done using Enzo BioArrayTM HighYieldTM RNA transcript labelling kit (T7) with 5 ul cDNA. The resultant cRNA was again purified using the GeneChip Sample Clean Up Module. The column was eluted in the first instance using 10 l RNase-free water, and for a second time using 11 l RNase-free water. cRNA was quantified using the Nanodrop spectrophotometer. A total of 15 ug of cRNA (required for hybridisation) was fragmented. Fragmentation was carried out by using 2 ul of fragmentation buffer for every 8 ul cRNA."
             }
             row {
                  "Study Protocol URI"
             }
             row {
                  "Study Protocol Version"
             }
             row {
                  "Study Protocol Parameters Name"
             }
             row {
                  "Study Protocol Parameters Term Accession Number"
             }
             row {
                  "Study Protocol Parameters Term Source REF"
             }
             row {
                  "Study Protocol Components Name"
             }
             row {
                  "Study Protocol Components Type"
             }
             row {
                  "Study Protocol Components Type Term Accession Number"
             }
             row {
                  "Study Protocol Components Type Term Source REF"
             }
             row {
                  "STUDY CONTACTS"
             }
             row {
                  "Study Person Last Name"
                  "Oliver"
                  "Juan"
                  "Leo"
             }
             row {
                  "Study Person First Name"
                  "Stephen"
                  "Castrillo"
                  "Zeef"
             }
             row {
                  "Study Person Mid Initials"
                  "G"
                  "I"
                  "A"
             }
             row {
                  "Study Person Email"
             }
             row {
                  "Study Person Phone"
             }
             row {
                  "Study Person Fax"
             }
             row {
                  "Study Person Address"
                  "Oxford Road, Manchester M13 9PT, UK"
                  "Oxford Road, Manchester M13 9PT, UK"
                  "Oxford Road, Manchester M13 9PT, UK"
             }
             row {
                  "Study Person Affiliation"
                  "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
                  "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
                  "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
             }
             row {
                  "Study Person Roles"
                  "corresponding author"
                  "author"
                  "author"
             }
             row {
                  "Study Person Roles Term Accession Number"
             }
             row {
                  "Study Person Roles Term Source REF"
             }
             row {
                  "Comment[<Study Person REF>]"
             }
             row {
                  "#TestRemark4"
             }
             row {
                  "#TestRemark5"
             }
             }
    }
    |> fun wb -> wb.Value.Parse()