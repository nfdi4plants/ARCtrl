module TestObjects.Spreadsheet.Investigation

open FsSpreadsheet.DSL
open FsSpreadsheet 

let emptyInvestigation = 
    let wb = new FsWorkbook()
    let ws = wb.InitWorksheet("isa_investigation")
    let row1 = ws.Row(1)
    row1.[1].Value <- "ONTOLOGY SOURCE REFERENCE"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Term Source Name"
    let row3 = ws.Row(3)
    row3.[1].Value <- "Term Source File"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Term Source Version"
    let row5 = ws.Row(5)
    row5.[1].Value <- "Term Source Description"
    let row6 = ws.Row(6)
    row6.[1].Value <- "INVESTIGATION"
    let row7 = ws.Row(7)
    row7.[1].Value <- "Investigation Identifier"
    let row8 = ws.Row(8)
    row8.[1].Value <- "Investigation Title"
    let row9 = ws.Row(9)
    row9.[1].Value <- "Investigation Description"
    let row10 = ws.Row(10)
    row10.[1].Value <- "Investigation Submission Date"
    let row11 = ws.Row(11)
    row11.[1].Value <- "Investigation Public Release Date"
    let row12 = ws.Row(12)
    row12.[1].Value <- "INVESTIGATION PUBLICATIONS"
    let row13 = ws.Row(13)
    row13.[1].Value <- "Investigation Publication PubMed ID"
    let row14 = ws.Row(14)
    row14.[1].Value <- "Investigation Publication DOI"
    let row15 = ws.Row(15)
    row15.[1].Value <- "Investigation Publication Author List"
    let row16 = ws.Row(16)
    row16.[1].Value <- "Investigation Publication Title"
    let row17 = ws.Row(17)
    row17.[1].Value <- "Investigation Publication Status"
    let row18 = ws.Row(18)
    row18.[1].Value <- "Investigation Publication Status Term Accession Number"
    let row19 = ws.Row(19)
    row19.[1].Value <- "Investigation Publication Status Term Source REF"
    let row20 = ws.Row(20)
    row20.[1].Value <- "INVESTIGATION CONTACTS"
    let row21 = ws.Row(21)
    row21.[1].Value <- "Investigation Person Last Name"
    let row22 = ws.Row(22)
    row22.[1].Value <- "Investigation Person First Name"
    let row23 = ws.Row(23)
    row23.[1].Value <- "Investigation Person Mid Initials"
    let row24 = ws.Row(24)
    row24.[1].Value <- "Investigation Person Email"
    let row25 = ws.Row(25)
    row25.[1].Value <- "Investigation Person Phone"
    let row26 = ws.Row(26)
    row26.[1].Value <- "Investigation Person Fax"
    let row27 = ws.Row(27)
    row27.[1].Value <- "Investigation Person Address"
    let row28 = ws.Row(28)
    row28.[1].Value <- "Investigation Person Affiliation"
    let row29 = ws.Row(29)
    row29.[1].Value <- "Investigation Person Roles"
    let row30 = ws.Row(30)
    row30.[1].Value <- "Investigation Person Roles Term Accession Number"
    let row31 = ws.Row(31)
    row31.[1].Value <- "Investigation Person Roles Term Source REF"
    let row32 = ws.Row(32)
    row32.[1].Value <- "STUDY"
    let row33 = ws.Row(33)
    row33.[1].Value <- "Study Identifier"
    let row34 = ws.Row(34)
    row34.[1].Value <- "Study Title"
    let row35 = ws.Row(35)
    row35.[1].Value <- "Study Description"
    let row36 = ws.Row(36)
    row36.[1].Value <- "Study Submission Date"
    let row37 = ws.Row(37)
    row37.[1].Value <- "Study Public Release Date"
    let row38 = ws.Row(38)
    row38.[1].Value <- "Study File Name"
    let row39 = ws.Row(39)
    row39.[1].Value <- "STUDY DESIGN DESCRIPTORS"
    let row40 = ws.Row(40)
    row40.[1].Value <- "Study Design Type"
    let row41 = ws.Row(41)
    row41.[1].Value <- "Study Design Type Term Accession Number"
    let row42 = ws.Row(42)
    row42.[1].Value <- "Study Design Type Term Source REF"
    let row43 = ws.Row(43)
    row43.[1].Value <- "STUDY PUBLICATIONS"
    let row44 = ws.Row(44)
    row44.[1].Value <- "Study Publication PubMed ID"
    let row45 = ws.Row(45)
    row45.[1].Value <- "Study Publication DOI"
    let row46 = ws.Row(46)
    row46.[1].Value <- "Study Publication Author List"
    let row47 = ws.Row(47)
    row47.[1].Value <- "Study Publication Title"
    let row48 = ws.Row(48)
    row48.[1].Value <- "Study Publication Status"
    let row49 = ws.Row(49)
    row49.[1].Value <- "Study Publication Status Term Accession Number"
    let row50 = ws.Row(50)
    row50.[1].Value <- "Study Publication Status Term Source REF"
    let row51 = ws.Row(51)
    row51.[1].Value <- "STUDY FACTORS"
    let row52 = ws.Row(52)
    row52.[1].Value <- "Study Factor Name"
    let row53 = ws.Row(53)
    row53.[1].Value <- "Study Factor Type"
    let row54 = ws.Row(54)
    row54.[1].Value <- "Study Factor Type Term Accession Number"
    let row55 = ws.Row(55)
    row55.[1].Value <- "Study Factor Type Term Source REF"
    let row56 = ws.Row(56)
    row56.[1].Value <- "STUDY ASSAYS"
    let row57 = ws.Row(57)
    row57.[1].Value <- "Study Assay Measurement Type"
    let row58 = ws.Row(58)
    row58.[1].Value <- "Study Assay Measurement Type Term Accession Number"
    let row59 = ws.Row(59)
    row59.[1].Value <- "Study Assay Measurement Type Term Source REF"
    let row60 = ws.Row(60)
    row60.[1].Value <- "Study Assay Technology Type"
    let row61 = ws.Row(61)
    row61.[1].Value <- "Study Assay Technology Type Term Accession Number"
    let row62 = ws.Row(62)
    row62.[1].Value <- "Study Assay Technology Type Term Source REF"
    let row63 = ws.Row(63)
    row63.[1].Value <- "Study Assay Technology Platform"
    let row64 = ws.Row(64)
    row64.[1].Value <- "Study Assay File Name"
    let row65 = ws.Row(65)
    row65.[1].Value <- "STUDY PROTOCOLS"
    let row66 = ws.Row(66)
    row66.[1].Value <- "Study Protocol Name"
    let row67 = ws.Row(67)
    row67.[1].Value <- "Study Protocol Type"
    let row68 = ws.Row(68)
    row68.[1].Value <- "Study Protocol Type Term Accession Number"
    let row69 = ws.Row(69)
    row69.[1].Value <- "Study Protocol Type Term Source REF"
    let row70 = ws.Row(70)
    row70.[1].Value <- "Study Protocol Description"
    let row71 = ws.Row(71)
    row71.[1].Value <- "Study Protocol URI"
    let row72 = ws.Row(72)
    row72.[1].Value <- "Study Protocol Version"
    let row73 = ws.Row(73)
    row73.[1].Value <- "Study Protocol Parameters Name"
    let row74 = ws.Row(74)
    row74.[1].Value <- "Study Protocol Parameters Term Accession Number"
    let row75 = ws.Row(75)
    row75.[1].Value <- "Study Protocol Parameters Term Source REF"
    let row76 = ws.Row(76)
    row76.[1].Value <- "Study Protocol Components Name"
    let row77 = ws.Row(77)
    row77.[1].Value <- "Study Protocol Components Type"
    let row78 = ws.Row(78)
    row78.[1].Value <- "Study Protocol Components Type Term Accession Number"
    let row79 = ws.Row(79)
    row79.[1].Value <- "Study Protocol Components Type Term Source REF"
    let row80 = ws.Row(80)
    row80.[1].Value <- "STUDY CONTACTS"
    let row81 = ws.Row(81)
    row81.[1].Value <- "Study Person Last Name"
    let row82 = ws.Row(82)
    row82.[1].Value <- "Study Person First Name"
    let row83 = ws.Row(83)
    row83.[1].Value <- "Study Person Mid Initials"
    let row84 = ws.Row(84)
    row84.[1].Value <- "Study Person Email"
    let row85 = ws.Row(85)
    row85.[1].Value <- "Study Person Phone"
    let row86 = ws.Row(86)
    row86.[1].Value <- "Study Person Fax"
    let row87 = ws.Row(87)
    row87.[1].Value <- "Study Person Address"
    let row88 = ws.Row(88)
    row88.[1].Value <- "Study Person Affiliation"
    let row89 = ws.Row(89)
    row89.[1].Value <- "Study Person Roles"
    let row90 = ws.Row(90)
    row90.[1].Value <- "Study Person Roles Term Accession Number"
    let row91 = ws.Row(91)
    row91.[1].Value <- "Study Person Roles Term Source REF"
    wb

[<Literal>]
let investigationIdentifier = "BII-I-1"

let fullInvestigation =
    let wb = new FsWorkbook()
    let ws = wb.InitWorksheet("isa_investigation")
    let row1 = ws.Row(1)
    row1.[1].Value <- "ONTOLOGY SOURCE REFERENCE"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Term Source Name"
    row2.[2].Value <- "OBI"
    row2.[3].Value <- "BTO"
    row2.[4].Value <- "NEWT"
    row2.[5].Value <- "UO"
    row2.[6].Value <- "CHEBI"
    row2.[7].Value <- "PATO"
    row2.[8].Value <- "EFO"
    let row3 = ws.Row(3)
    row3.[1].Value <- "#TestRemark1"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Term Source File"
    row4.[2].Value <- "http://bioportal.bioontology.org/ontologies/1123"
    row4.[3].Value <- "ArrayExpress Experimental Factor Ontology"
    let row5 = ws.Row(5)
    row5.[1].Value <- "Term Source Version"
    row5.[2].Value <- "47893"
    row5.[3].Value <- "v 1.26"
    row5.[4].Value <- "v 1.26"
    row5.[5].Value <- "v 1.26"
    row5.[6].Value <- "v 1.26"
    row5.[7].Value <- "v 1.26"
    row5.[8].Value <- "v 1.26"
    let row6 = ws.Row(6)
    row6.[1].Value <- "Term Source Description"
    row6.[2].Value <- "Ontology for Biomedical Investigations"
    row6.[3].Value <- "BRENDA tissue / enzyme source"
    row6.[4].Value <- "NEWT UniProt Taxonomy Database"
    row6.[5].Value <- "Unit Ontology"
    row6.[6].Value <- "Chemical Entities of Biological Interest"
    row6.[7].Value <- "Phenotypic qualities (properties)"
    row6.[8].Value <- "ArrayExpress Experimental Factor Ontology"
    let row7 = ws.Row(7)
    row7.[1].Value <- "INVESTIGATION"
    let row8 = ws.Row(8)
    row8.[1].Value <- "Investigation Identifier"
    row8.[2].Value <- investigationIdentifier
    let row9 = ws.Row(9)
    row9.[1].Value <- "Investigation Title"
    row9.[2].Value <- "Growth control of the eukaryote cell: a systems biology study in yeast"
    let row10 = ws.Row(10)
    row10.[1].Value <- "Investigation Description"
    row10.[2].Value <- "Background Cell growth underlies many key cellular and developmental processes, yet a limited number of studies have been carried out on cell-growth regulation. Comprehensive studies at the transcriptional, proteomic and metabolic levels under defined controlled conditions are currently lacking. Results Metabolic control analysis is being exploited in a systems biology study of the eukaryotic cell. Using chemostat culture, we have measured the impact of changes in flux (growth rate) on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae. Each functional genomic level shows clear growth-rate-associated trends and discriminates between carbon-sufficient and carbon-limited conditions. Genes consistently and significantly upregulated with increasing growth rate are frequently essential and encode evolutionarily conserved proteins of known function that participate in many protein-protein interactions. In contrast, more unknown, and fewer essential, genes are downregulated with increasing growth rate; their protein products rarely interact with one another. A large proportion of yeast genes under positive growth-rate control share orthologs with other eukaryotes, including humans. Significantly, transcription of genes encoding components of the TOR complex (a major controller of eukaryotic cell growth) is not subject to growth-rate regulation. Moreover, integrative studies reveal the extent and importance of post-transcriptional control, patterns of control of metabolic fluxes at the level of enzyme synthesis, and the relevance of specific enzymatic reactions in the control of metabolic fluxes during cell growth. Conclusion This work constitutes a first comprehensive systems biology study on growth-rate control in the eukaryotic cell. The results have direct implications for advanced studies on cell growth, in vivo regulation of metabolic fluxes for comprehensive metabolic engineering, and for the design of genome-scale systems biology models of the eukaryotic cell."
    let row11 = ws.Row(11)
    row11.[1].Value <- "Investigation Submission Date"
    row11.[2].Value <- "2007-04-30"
    let row12 = ws.Row(12)
    row12.[1].Value <- "Investigation Public Release Date"
    row12.[2].Value <- "2009-03-10"
    let row13 = ws.Row(13)
    row13.[1].Value <- "Comment[Created With Configuration]"
    let row14 = ws.Row(14)
    row14.[1].Value <- "Comment[Last Opened With Configuration]"
    row14.[2].Value <- "isaconfig-default_v2013-02-13"
    let row15 = ws.Row(15)
    row15.[1].Value <- "INVESTIGATION PUBLICATIONS"
    let row16 = ws.Row(16)
    row16.[1].Value <- "Investigation Publication PubMed ID"
    row16.[2].Value <- "17439666"
    let row17 = ws.Row(17)
    row17.[1].Value <- "Investigation Publication DOI"
    row17.[2].Value <- "doi:10.1186/jbiol54"
    let row18 = ws.Row(18)
    row18.[1].Value <- "Investigation Publication Author List"
    row18.[2].Value <- "Castrillo JI, Zeef LA, Hoyle DC, Zhang N, Hayes A, Gardner DC, Cornell MJ, Petty J, Hakes L, Wardleworth L, Rash B, Brown M, Dunn WB, Broadhurst D, O'Donoghue K, Hester SS, Dunkley TP, Hart SR, Swainston N, Li P, Gaskell SJ, Paton NW, Lilley KS, Kell DB, Oliver SG."
    let row19 = ws.Row(19)
    row19.[1].Value <- "Investigation Publication Title"
    row19.[2].Value <- "Growth control of the eukaryote cell: a systems biology study in yeast."
    let row20 = ws.Row(20)
    row20.[1].Value <- "Investigation Publication Status"
    row20.[2].Value <- "indexed in Pubmed"
    let row21 = ws.Row(21)
    row21.[1].Value <- "Investigation Publication Status Term Accession Number"
    let row22 = ws.Row(22)
    row22.[1].Value <- "Investigation Publication Status Term Source REF"
    let row23 = ws.Row(23)
    row23.[1].Value <- "INVESTIGATION CONTACTS"
    let row24 = ws.Row(24)
    row24.[1].Value <- "Investigation Person Last Name"
    row24.[2].Value <- "Stephen"
    row24.[3].Value <- "Castrillo"
    row24.[4].Value <- "Zeef"
    let row25 = ws.Row(25)
    row25.[1].Value <- "Investigation Person First Name"
    row25.[2].Value <- "Oliver"
    row25.[3].Value <- "Juan"
    row25.[4].Value <- "Leo"
    let row26 = ws.Row(26)
    row26.[1].Value <- "Investigation Person Mid Initials"
    row26.[2].Value <- "G"
    row26.[3].Value <- "I"
    row26.[4].Value <- "A"
    let row27 = ws.Row(27)
    row27.[1].Value <- "Investigation Person Email"
    let row28 = ws.Row(28)
    row28.[1].Value <- "Investigation Person Phone"
    let row29 = ws.Row(29)
    row29.[1].Value <- "Investigation Person Fax"
    let row30 = ws.Row(30)
    row30.[1].Value <- "Investigation Person Address"
    row30.[2].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row30.[3].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row30.[4].Value <- "Oxford Road, Manchester M13 9PT, UK"
    let row31 = ws.Row(31)
    row31.[1].Value <- "Investigation Person Affiliation"
    row31.[2].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row31.[3].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row31.[4].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    let row32 = ws.Row(32)
    row32.[1].Value <- "Investigation Person Roles"
    row32.[2].Value <- "corresponding author"
    row32.[3].Value <- "author"
    row32.[4].Value <- "author"
    let row33 = ws.Row(33)
    row33.[1].Value <- "Investigation Person Roles Term Accession Number"
    let row34 = ws.Row(34)
    row34.[1].Value <- "Investigation Person Roles Term Source REF"
    let row35 = ws.Row(35)
    row35.[1].Value <- "Comment[Investigation Person REF]"
    let row36 = ws.Row(36)
    row36.[1].Value <- "STUDY"
    let row37 = ws.Row(37)
    row37.[1].Value <- "Study Identifier"
    row37.[2].Value <- Study.studyIdentifier
    let row38 = ws.Row(38)
    row38.[1].Value <- "Study Title"
    row38.[2].Value <- "Study of the impact of changes in flux on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae under different nutrient limitations"
    let row39 = ws.Row(39)
    row39.[1].Value <- "Study Description"
    row39.[2].Value <- "We wished to study the impact of growth rate on the total complement of mRNA molecules, proteins, and metabolites in S. cerevisiae, independent of any nutritional or other physiological effects. To achieve this, we carried out our analyses on yeast grown in steady-state chemostat culture under four different nutrient limitations (glucose, ammonium, phosphate, and sulfate) at three different dilution (that is, growth) rates (D = u = 0.07, 0.1, and 0.2/hour, equivalent to population doubling times (Td) of 10 hours, 7 hours, and 3.5 hours, respectively; u = specific growth rate defined as grams of biomass generated per gram of biomass present per unit time)."
    let row40 = ws.Row(40)
    row40.[1].Value <- "Study Submission Date"
    row40.[2].Value <- "2007-04-30"
    let row41 = ws.Row(41)
    row41.[1].Value <- "Study Public Release Date"
    row41.[2].Value <- "2009-03-10"
    let row42 = ws.Row(42)
    row42.[1].Value <- "Study File Name"
    row42.[2].Value <- $"studies/{Study.studyIdentifier}/isa.study.xlsx"
    let row43 = ws.Row(43)
    row43.[1].Value <- "Comment[Study Grant Number]"
    let row44 = ws.Row(44)
    row44.[1].Value <- "Comment[Study Funding Agency]"
    let row45 = ws.Row(45)
    row45.[1].Value <- "STUDY DESIGN DESCRIPTORS"
    let row46 = ws.Row(46)
    row46.[1].Value <- "Study Design Type"
    row46.[2].Value <- "intervention design"
    let row47 = ws.Row(47)
    row47.[1].Value <- "#TestRemark2"
    let row48 = ws.Row(48)
    row48.[1].Value <- "Study Design Type Term Accession Number"
    row48.[2].Value <- "http://purl.obolibrary.org/obo/OBI_0000115"
    let row49 = ws.Row(49)
    row49.[1].Value <- "Study Design Type Term Source REF"
    row49.[2].Value <- "OBI"
    let row50 = ws.Row(50)
    row50.[1].Value <- "STUDY PUBLICATIONS"
    let row51 = ws.Row(51)
    row51.[1].Value <- "Study Publication PubMed ID"
    row51.[2].Value <- "17439666"
    let row52 = ws.Row(52)
    row52.[1].Value <- "Study Publication DOI"
    row52.[2].Value <- "doi:10.1186/jbiol54"
    let row53 = ws.Row(53)
    row53.[1].Value <- "Study Publication Author List"
    row53.[2].Value <- "Castrillo JI, Zeef LA, Hoyle DC, Zhang N, Hayes A, Gardner DC, Cornell MJ, Petty J, Hakes L, Wardleworth L, Rash B, Brown M, Dunn WB, Broadhurst D, O'Donoghue K, Hester SS, Dunkley TP, Hart SR, Swainston N, Li P, Gaskell SJ, Paton NW, Lilley KS, Kell DB, Oliver SG."
    let row54 = ws.Row(54)
    row54.[1].Value <- "Study Publication Title"
    row54.[2].Value <- "Growth control of the eukaryote cell: a systems biology study in yeast."
    let row55 = ws.Row(55)
    row55.[1].Value <- "Study Publication Status"
    row55.[2].Value <- "published"
    let row56 = ws.Row(56)
    row56.[1].Value <- "Study Publication Status Term Accession Number"
    let row57 = ws.Row(57)
    row57.[1].Value <- "Study Publication Status Term Source REF"
    let row58 = ws.Row(58)
    row58.[1].Value <- "STUDY FACTORS"
    let row59 = ws.Row(59)
    row59.[1].Value <- "Study Factor Name"
    row59.[2].Value <- "limiting nutrient"
    row59.[3].Value <- "rate"
    let row60 = ws.Row(60)
    row60.[1].Value <- "Study Factor Type"
    row60.[2].Value <- "chemical compound"
    row60.[3].Value <- "rate"
    let row61 = ws.Row(61)
    row61.[1].Value <- "Study Factor Type Term Accession Number"
    row61.[3].Value <- "http://purl.obolibrary.org/obo/PATO_0000161"
    let row62 = ws.Row(62)
    row62.[1].Value <- "Study Factor Type Term Source REF"
    row62.[3].Value <- "PATO"
    let row63 = ws.Row(63)
    row63.[1].Value <- "STUDY ASSAYS"
    let row64 = ws.Row(64)
    row64.[1].Value <- "Study Assay Measurement Type"
    row64.[2].Value <- "protein expression profiling"
    row64.[3].Value <- "metabolite profiling"
    row64.[4].Value <- "transcription profiling"
    let row65 = ws.Row(65)
    row65.[1].Value <- "Study Assay Measurement Type Term Accession Number"
    row65.[2].Value <- "http://purl.obolibrary.org/obo/OBI_0000615"
    row65.[3].Value <- "http://purl.obolibrary.org/obo/OBI_0000366"
    row65.[4].Value <- "424"
    let row66 = ws.Row(66)
    row66.[1].Value <- "Study Assay Measurement Type Term Source REF"
    row66.[2].Value <- "OBI"
    row66.[3].Value <- "OBI"
    row66.[4].Value <- "OBI"
    let row67 = ws.Row(67)
    row67.[1].Value <- "Study Assay Technology Type"
    row67.[2].Value <- "mass spectrometry"
    row67.[3].Value <- "mass spectrometry"
    row67.[4].Value <- "DNA microarray"
    let row68 = ws.Row(68)
    row68.[1].Value <- "Study Assay Technology Type Term Accession Number"
    row68.[4].Value <- "http://purl.obolibrary.org/obo/OBI_0400148"
    let row69 = ws.Row(69)
    row69.[1].Value <- "Study Assay Technology Type Term Source REF"
    row69.[2].Value <- "OBI"
    row69.[3].Value <- "OBI"
    row69.[4].Value <- "OBI"
    let row70 = ws.Row(70)
    row70.[1].Value <- "Study Assay Technology Platform"
    row70.[2].Value <- "iTRAQ"
    row70.[3].Value <- "LC-MS/MS"
    row70.[4].Value <- "Affymetrix"
    let row71 = ws.Row(71)
    row71.[1].Value <- "Study Assay File Name"
    row71.[2].Value <- $"assays/{Assay.assayIdentifier}/isa.assay.xlsx"
    row71.[3].Value <- "assays/metabolome/isa.assay.xlsx"
    row71.[4].Value <- "assays/transcriptome/isa.assay.xlsx"
    let row72 = ws.Row(72)
    row72.[1].Value <- "STUDY PROTOCOLS"
    let row73 = ws.Row(73)
    row73.[1].Value <- "Study Protocol Name"
    row73.[2].Value <- "growth protocol"
    row73.[3].Value <- "mRNA extraction"
    row73.[4].Value <- "protein extraction"
    row73.[5].Value <- "biotin labeling"
    row73.[6].Value <- "ITRAQ labeling"
    row73.[7].Value <- "EukGE-WS4"
    row73.[8].Value <- "metabolite extraction"
    let row74 = ws.Row(74)
    row74.[1].Value <- "Study Protocol Type"
    row74.[2].Value <- "growth"
    row74.[3].Value <- "mRNA extraction"
    row74.[4].Value <- "protein extraction"
    row74.[5].Value <- "labeling"
    row74.[6].Value <- "labeling"
    row74.[7].Value <- "hybridization"
    row74.[8].Value <- "extraction"
    let row75 = ws.Row(75)
    row75.[1].Value <- "Study Protocol Type Term Accession Number"
    row75.[8].Value <- "http://purl.obolibrary.org/obo/OBI_0302884"
    let row76 = ws.Row(76)
    row76.[1].Value <- "Study Protocol Type Term Source REF"
    row76.[8].Value <- "OBI"
    let row77 = ws.Row(77)
    row77.[1].Value <- "Study Protocol Description"
    row77.[2].Value <- "1. Biomass samples (45 ml) were taken via the sample port of the Applikon fermenters. The cells were pelleted by centrifugation for 5 min at 5000 rpm. The supernatant was removed and the RNA pellet resuspended in the residual medium to form a slurry. This was added in a dropwise manner directly into a 5 ml Teflon flask (B. Braun Biotech, Germany) containing liquid nitrogen and a 7 mm-diameter tungsten carbide ball. After allowing evaporation of the liquid nitrogen the flask was reassembled and the cells disrupted by agitation at 1500 rpm for 2 min in a Microdismembranator U (B. Braun Biotech, Germany) 2. The frozen powder was then dissolved in 1 ml of TriZol reagent (Sigma-Aldrich, UK), vortexed for 1 min, and then kept at room temperature for a further 5min. 3. Chloroform extraction was performed by addition of 0.2 ml chloroform, shaking vigorously or 15 s, then 5min incubation at room temperature. 4. Following centrifugation at 12,000 rpm for 5 min, the RNA (contained in the aqueous phase) was precipitated with 0.5 vol of 2-propanol at room temperature for 15 min. 5. After further centrifugation (12,000 rpm for 10 min at 4 C) the RNA pellet was washed twice with 70 % (v/v) ethanol, briefly air-dried, and redissolved in 0.5 ml diethyl pyrocarbonate (DEPC)-treated water. 6. The single-stranded RNA was precipitated once more by addition of 0.5 ml of LiCl buffer (4 M LiCl, 20 mM Tris-HCl, pH 7.5, 10 mM EDTA), thus removing tRNA and DNA from the sample. 7. After precipitation (20 C for 1h) and centrifugation (12,000 rpm, 30 min, 4 C), the RNA was washed twice in 70 % (v/v) ethanol prior to being dissolved in a minimal volume of DEPC-treated water. 8. Total RNA quality was checked using the RNA 6000 Nano Assay, and analysed on an Agilent 2100 Bioanalyser (Agilent Technologies). RNA was quantified using the Nanodrop ultra low volume spectrophotometer (Nanodrop Technologies)."
    row77.[3].Value <- "1. Biomass samples (45 ml) were taken via the sample port of the Applikon fermenters. The cells were pelleted by centrifugation for 5 min at 5000 rpm. The supernatant was removed and the RNA pellet resuspended in the residual medium to form a slurry. This was added in a dropwise manner directly into a 5 ml Teflon flask (B. Braun Biotech, Germany) containing liquid nitrogen and a 7 mm-diameter tungsten carbide ball. After allowing evaporation of the liquid nitrogen the flask was reassembled and the cells disrupted by agitation at 1500 rpm for 2 min in a Microdismembranator U (B. Braun Biotech, Germany) 2. The frozen powder was then dissolved in 1 ml of TriZol reagent (Sigma-Aldrich, UK), vortexed for 1 min, and then kept at room temperature for a further 5min. 3. Chloroform extraction was performed by addition of 0.2 ml chloroform, shaking vigorouslyor 15 s, then 5min incubation at room temperature. 4. Following centrifugation at 12,000 rpm for 5 min, the RNA (contained in the aqueous phase) was precipitated with 0.5 vol of 2-propanol at room temperature for 15 min. 5. After further centrifugation (12,000 rpm for 10 min at 4 C) the RNA pellet was washed twice with 70 % (v/v) ethanol, briefly air-dried, and redissolved in 0.5 ml diethyl pyrocarbonate (DEPC)-treated water. 6. The single-stranded RNA was precipitated once more by addition of 0.5 ml of LiCl bffer (4 M LiCl, 20 mM Tris-HCl, pH 7.5, 10 mM EDTA), thus removing tRNA and DNA from the sample. 7. After precipitation (20 C for 1 h) and centrifugation (12,000 rpm, 30 min, 4 C), the RNA was washed twice in 70 % (v/v) ethanol prior to being dissolved in a minimal volume of DEPC-treated water. 8. Total RNA quality was checked using the RNA 6000 Nano Assay, and analysed on an Agilent 2100 Bioanalyser (Agilent Technologies). RNA was quantified using the Nanodrop ultra low volume spectrophotometer (Nanodrop Technologies)."
    row77.[5].Value <- "This was done using Enzo BioArrayTM HighYieldTM RNA transcript labelling kit (T7) with 5 ul cDNA. The resultant cRNA was again purified using the GeneChip Sample Clean Up Module. The column was eluted in the first instance using 10 l RNase-free water, and for a second time using 11 ul RNase-free water. cRNA was quantified using the Nanodrop spectrophotometer. A total of 15 ug of cRNA (required for hybridisation) was fragmented. Fragmentation was carried out by using 2 ul of fragmentation buffer for every 8 ul cRNA."
    row77.[7].Value <- "For each target, a hybridisation cocktail was made using the standard array recipe as described in the GeneChip Expression Analysis technical manual. GeneChip control oligonucleotide and 20x eukaryotic hybridisation controls were used. Hybridisation buffer was made as detailed in the GeneChip manual and the BSA and herring sperm DNA was purchased from Invitrogen. The cocktail was heated to 99 C for 5 min, transferred to 45 C for 5 min and then spun for 5 min to remove any insoluble material. Affymetrix Yeast Yg_s98 S. cerevisiae arrays were pre-hybridised with 200 ul 1x hybridisation buffer and incubated at 45 C for 10 min. 200 ul of the hybridisation cocktail was loaded onto the arrays. The probe array was incubated in a rotisserie at 45 C, rotating at 60 rpm. Following hybridisation, for 16 hr, chips were loaded onto a Fluidics station for washing and staining using the EukGe WS2v4 programme controlled using Microarray Suite 5 software."
    let row78 = ws.Row(78)
    row78.[1].Value <- "Study Protocol URI"
    let row79 = ws.Row(79)
    row79.[1].Value <- "Study Protocol Version"
    let row80 = ws.Row(80)
    row80.[1].Value <- "Study Protocol Parameters Name"
    row80.[8].Value <- "sample volume;standard volume"
    let row81 = ws.Row(81)
    row81.[1].Value <- "Study Protocol Parameters Term Accession Number"
    row81.[8].Value <- ";"
    let row82 = ws.Row(82)
    row82.[1].Value <- "Study Protocol Parameters Term Source REF"
    row82.[8].Value <- ";"
    let row83 = ws.Row(83)
    row83.[1].Value <- "Study Protocol Components Name"
    let row84 = ws.Row(84)
    row84.[1].Value <- "Study Protocol Components Type"
    let row85 = ws.Row(85)
    row85.[1].Value <- "Study Protocol Components Type Term Accession Number"
    let row86 = ws.Row(86)
    row86.[1].Value <- "Study Protocol Components Type Term Source REF"
    let row87 = ws.Row(87)
    row87.[1].Value <- "STUDY CONTACTS"
    let row88 = ws.Row(88)
    row88.[1].Value <- "Study Person Last Name"
    row88.[2].Value <- "Oliver"
    row88.[3].Value <- "Juan"
    row88.[4].Value <- "Leo"
    let row89 = ws.Row(89)
    row89.[1].Value <- "Study Person First Name"
    row89.[2].Value <- "Stephen"
    row89.[3].Value <- "Castrillo"
    row89.[4].Value <- "Zeef"
    let row90 = ws.Row(90)
    row90.[1].Value <- "Study Person Mid Initials"
    row90.[2].Value <- "G"
    row90.[3].Value <- "I"
    row90.[4].Value <- "A"
    let row91 = ws.Row(91)
    row91.[1].Value <- "Study Person Email"
    let row92 = ws.Row(92)
    row92.[1].Value <- "Study Person Phone"
    let row93 = ws.Row(93)
    row93.[1].Value <- "Study Person Fax"
    let row94 = ws.Row(94)
    row94.[1].Value <- "Study Person Address"
    row94.[2].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row94.[3].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row94.[4].Value <- "Oxford Road, Manchester M13 9PT, UK"
    let row95 = ws.Row(95)
    row95.[1].Value <- "Study Person Affiliation"
    row95.[2].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row95.[3].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row95.[4].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    let row96 = ws.Row(96)
    row96.[1].Value <- "Study Person Roles"
    row96.[2].Value <- "corresponding author"
    row96.[3].Value <- "author"
    row96.[4].Value <- "author"
    let row97 = ws.Row(97)
    row97.[1].Value <- "Study Person Roles Term Accession Number"
    let row98 = ws.Row(98)
    row98.[1].Value <- "Study Person Roles Term Source REF"
    let row99 = ws.Row(99)
    row99.[1].Value <- "Comment[Study Person REF]"
    let row100 = ws.Row(100)
    row100.[1].Value <- "STUDY"
    let row101 = ws.Row(101)
    row101.[1].Value <- "#TestRemark3"
    let row102 = ws.Row(102)
    row102.[1].Value <- "Study Identifier"
    row102.[2].Value <- "BII-S-2"
    let row103 = ws.Row(103)
    row103.[1].Value <- "Study Title"
    row103.[2].Value <- "A time course analysis of transcription response in yeast treated with rapamycin, a specific inhibitor of the TORC1 complex: impact on yeast growth"
    let row104 = ws.Row(104)
    row104.[1].Value <- "Study Description"
    row104.[2].Value <- "Comprehensive high-throughput analyses at the levels of mRNAs, proteins, and metabolites, and studies on gene expression patterns are required for systems biology studies of cell growth [4,26-29]. Although such comprehensive data sets are lacking, many studies have pointed to a central role for the target-of-rapamycin (TOR) signal transduction pathway in growth control. TOR is a serine/threonine kinase that has been conserved from yeasts to mammals; it integrates signals from nutrients or growth factors to regulate cell growth and cell-cycle progression coordinately. Although such comprehensive data sets are lacking, many studies have pointed to a central role for the target-of-rapamycin (TOR) signal transduction pathway in growth control. TOR is a serine/threonine kinase that has been conserved from yeasts to mammals; it integrates signals from nutrients or growth factors to regulate cell growth and cell-cycle progression coordinately. The effect of rapamycin were studied as follows: a culture growing at mid-exponential phase was divided into two. Rapamycin (200 ng/ml) was added to one half, and the drug's solvent to the other, as the control. Samples were taken at 0, 1, 2 and 4 h after treatment. Gene expression at the mRNA level was investigated by transcriptome analysis using Affymetrix hybridization arrays."
    let row105 = ws.Row(105)
    row105.[1].Value <- "Study Submission Date"
    row105.[2].Value <- "2007-04-30"
    let row106 = ws.Row(106)
    row106.[1].Value <- "Study Public Release Date"
    row106.[2].Value <- "2009-03-10"
    let row107 = ws.Row(107)
    row107.[1].Value <- "Study File Name"
    row107.[2].Value <- "studies/BII-S-2/isa.study.xlsx"
    let row108 = ws.Row(108)
    row108.[1].Value <- "Comment[Study Grant Number]"
    let row109 = ws.Row(109)
    row109.[1].Value <- "Comment[Study Funding Agency]"
    let row110 = ws.Row(110)
    row110.[1].Value <- "STUDY DESIGN DESCRIPTORS"
    let row111 = ws.Row(111)
    row111.[1].Value <- "Study Design Type"
    row111.[2].Value <- "time series design"
    let row112 = ws.Row(112)
    row112.[1].Value <- "Study Design Type Term Accession Number"
    row112.[2].Value <- "http://purl.obolibrary.org/obo/OBI_0500020"
    let row113 = ws.Row(113)
    row113.[1].Value <- "Study Design Type Term Source REF"
    row113.[2].Value <- "OBI"
    let row114 = ws.Row(114)
    row114.[1].Value <- "STUDY PUBLICATIONS"
    let row115 = ws.Row(115)
    row115.[1].Value <- "Study Publication PubMed ID"
    row115.[2].Value <- "17439666"
    let row116 = ws.Row(116)
    row116.[1].Value <- "Study Publication DOI"
    let row117 = ws.Row(117)
    row117.[1].Value <- "Study Publication Author List"
    let row118 = ws.Row(118)
    row118.[1].Value <- "Study Publication Title"
    row118.[2].Value <- "Growth control of the eukaryote cell: a systems biology study in yeast."
    let row119 = ws.Row(119)
    row119.[1].Value <- "Study Publication Status"
    row119.[2].Value <- "indexed in Pubmed"
    let row120 = ws.Row(120)
    row120.[1].Value <- "Study Publication Status Term Accession Number"
    let row121 = ws.Row(121)
    row121.[1].Value <- "Study Publication Status Term Source REF"
    let row122 = ws.Row(122)
    row122.[1].Value <- "STUDY FACTORS"
    let row123 = ws.Row(123)
    row123.[1].Value <- "Study Factor Name"
    row123.[2].Value <- "compound"
    row123.[3].Value <- "exposure time"
    row123.[4].Value <- "dose"
    let row124 = ws.Row(124)
    row124.[1].Value <- "Study Factor Type"
    row124.[2].Value <- "compound"
    row124.[3].Value <- "time"
    row124.[4].Value <- "dose"
    let row125 = ws.Row(125)
    row125.[1].Value <- "Study Factor Type Term Accession Number"
    row125.[3].Value <- "http://www.ebi.ac.uk/efo/EFO_0000721"
    row125.[4].Value <- "http://www.ebi.ac.uk/efo/EFO_0000428"
    let row126 = ws.Row(126)
    row126.[1].Value <- "Study Factor Type Term Source REF"
    row126.[3].Value <- "EFO"
    row126.[4].Value <- "EFO"
    let row127 = ws.Row(127)
    row127.[1].Value <- "STUDY ASSAYS"
    let row128 = ws.Row(128)
    row128.[1].Value <- "Study Assay Measurement Type"
    row128.[2].Value <- "transcription profiling"
    let row129 = ws.Row(129)
    row129.[1].Value <- "Study Assay Measurement Type Term Accession Number"
    row129.[2].Value <- "424"
    let row130 = ws.Row(130)
    row130.[1].Value <- "Study Assay Measurement Type Term Source REF"
    row130.[2].Value <- "OBI"
    let row131 = ws.Row(131)
    row131.[1].Value <- "Study Assay Technology Type"
    row131.[2].Value <- "DNA microarray"
    let row132 = ws.Row(132)
    row132.[1].Value <- "Study Assay Technology Type Term Accession Number"
    row132.[2].Value <- "http://purl.obolibrary.org/obo/OBI_0400148"
    let row133 = ws.Row(133)
    row133.[1].Value <- "Study Assay Technology Type Term Source REF"
    row133.[2].Value <- "OBI"
    let row134 = ws.Row(134)
    row134.[1].Value <- "Study Assay Technology Platform"
    row134.[2].Value <- "Affymetrix"
    let row135 = ws.Row(135)
    row135.[1].Value <- "Study Assay File Name"
    row135.[2].Value <- "assays/microarray/isa.assay.xlsx"
    let row136 = ws.Row(136)
    row136.[1].Value <- "STUDY PROTOCOLS"
    let row137 = ws.Row(137)
    row137.[1].Value <- "Study Protocol Name"
    row137.[2].Value <- "EukGE-WS4"
    row137.[3].Value <- "growth"
    row137.[4].Value <- "mRNA extraction"
    row137.[5].Value <- "biotin labeling"
    let row138 = ws.Row(138)
    row138.[1].Value <- "Study Protocol Type"
    row138.[2].Value <- "hybridization"
    row138.[3].Value <- "growth"
    row138.[4].Value <- "mRNA extraction"
    row138.[5].Value <- "labeling"
    let row139 = ws.Row(139)
    row139.[1].Value <- "Study Protocol Type Term Accession Number"
    let row140 = ws.Row(140)
    row140.[1].Value <- "Study Protocol Type Term Source REF"
    let row141 = ws.Row(141)
    row141.[1].Value <- "Study Protocol Description"
    row141.[2].Value <- "For each target, a hybridisation cocktail was made using the standard array recipe as described in the GeneChip Expression Analysis technical manual. GeneChip control oligonucleotide and 20x eukaryotic hybridisation controls were used. Hybridisation buffer was made as detailed in the GeneChip manual and the BSA and herring sperm DNA was purchased from Invitrogen. The cocktail was heated to 99 C for 5mins, transferred to 45 C for 5 min and then spun for 5 min to remove any insoluble material. Affymetrix Yeast Yg_s98 S. cerevisiae arrays were pre-hybridised with 200 l 1x hybridisation buffer and incubated at 45 C for 10 min. 200 l of the hybridisation cocktail was loaded onto the arrays. The probe array was incubated in a rotisserie at 45 C, rotating at 60 rpm. Following hybridisation, for 16hr, chips were loaded onto a Fluidics station for washing and staining using the EukGe WS2v4 programme controlled using Microarray Suite 5 software."
    row141.[3].Value <- "The culture was grown in YMB minimum media + 2% glucose + supplement to early exponential growth (OD ~0.32)"
    row141.[4].Value <- "1. Biomass samples (45ml) were taken via the sample port of the Applikon fermenters. The cells were pelleted by centrifugation for 5min at 5000 rpm. The supernatant was removed and the RNA pellet resuspended in the residual medium to form a slurry. This was added in a dropwise manner directly into a 5ml Teflon flask (B. Braun Biotech, Germany) containing liquid nitrogen and a 7 mm-diameter tungsten carbide ball. After allowing evaporation of the liquid nitrogen the flask was reassembled and the cells disrupted by agitation at 1500 rpm for 2 min in a Microdismembranator U (B. Braun Biotech, Germany) 2. The frozen powder was then dissolved in 1 ml of TriZol reagent (Sigma-Aldrich, UK), vortexed for 1 min, and then kept at room temperature for a further 5 min. 3. Chloroform extraction was performed by addition of 0.2 ml chloroform, shaking vigorouslyor 15 s, then 5 min incubation at room temperature. 4. Following centrifugation at 12,000 rpm for 5 min, the RNA (contained in the aqueous phase) was precipitated with 0.5 vol of 2-propanol at room temperature for 15 min. 5. After further centrifugation (12,000 rpm for 10 min at 4 C) the RNA pellet was washed twice with 70 % (v/v) ethanol, briefly air-dried, and redissolved in 0.5 ml diethyl pyrocarbonate (DEPC)-treated water. 6. The single-stranded RNA was precipitated once more by addition of 0.5 ml of LiCl bffer (4 M LiCl, 20 mM Tris-HCl, pH 7.5, 10 mM EDTA), thus removing tRNA and DNA from the sample. 7. After precipitation (20 C for 1h) and centrifugation (12,000 rpm, 30 min, 4 C), the RNA was washed twice in 70 % (v/v) ethanol prior to being dissolved in a minimal volume of DEPC-treated water. 8. Total RNA quality was checked using the RNA 6000 Nano Assay, and analysed on an Agilent 2100 Bioanalyser (Agilent Technologies). RNA was quantified using the Nanodrop ultra low volume spectrophotometer (Nanodrop Technologies)."
    row141.[5].Value <- "This was done using Enzo BioArrayTM HighYieldTM RNA transcript labelling kit (T7) with 5 ul cDNA. The resultant cRNA was again purified using the GeneChip Sample Clean Up Module. The column was eluted in the first instance using 10 l RNase-free water, and for a second time using 11 l RNase-free water. cRNA was quantified using the Nanodrop spectrophotometer. A total of 15 ug of cRNA (required for hybridisation) was fragmented. Fragmentation was carried out by using 2 ul of fragmentation buffer for every 8 ul cRNA."
    let row142 = ws.Row(142)
    row142.[1].Value <- "Study Protocol URI"
    let row143 = ws.Row(143)
    row143.[1].Value <- "Study Protocol Version"
    let row144 = ws.Row(144)
    row144.[1].Value <- "Study Protocol Parameters Name"
    let row145 = ws.Row(145)
    row145.[1].Value <- "Study Protocol Parameters Term Accession Number"
    let row146 = ws.Row(146)
    row146.[1].Value <- "Study Protocol Parameters Term Source REF"
    let row147 = ws.Row(147)
    row147.[1].Value <- "Study Protocol Components Name"
    let row148 = ws.Row(148)
    row148.[1].Value <- "Study Protocol Components Type"
    let row149 = ws.Row(149)
    row149.[1].Value <- "Study Protocol Components Type Term Accession Number"
    let row150 = ws.Row(150)
    row150.[1].Value <- "Study Protocol Components Type Term Source REF"
    let row151 = ws.Row(151)
    row151.[1].Value <- "STUDY CONTACTS"
    let row152 = ws.Row(152)
    row152.[1].Value <- "Study Person Last Name"
    row152.[2].Value <- "Oliver"
    row152.[3].Value <- "Juan"
    row152.[4].Value <- "Leo"
    let row153 = ws.Row(153)
    row153.[1].Value <- "Study Person First Name"
    row153.[2].Value <- "Stephen"
    row153.[3].Value <- "Castrillo"
    row153.[4].Value <- "Zeef"
    let row154 = ws.Row(154)
    row154.[1].Value <- "Study Person Mid Initials"
    row154.[2].Value <- "G"
    row154.[3].Value <- "I"
    row154.[4].Value <- "A"
    let row155 = ws.Row(155)
    row155.[1].Value <- "Study Person Email"
    let row156 = ws.Row(156)
    row156.[1].Value <- "Study Person Phone"
    let row157 = ws.Row(157)
    row157.[1].Value <- "Study Person Fax"
    let row158 = ws.Row(158)
    row158.[1].Value <- "Study Person Address"
    row158.[2].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row158.[3].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row158.[4].Value <- "Oxford Road, Manchester M13 9PT, UK"
    let row159 = ws.Row(159)
    row159.[1].Value <- "Study Person Affiliation"
    row159.[2].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row159.[3].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row159.[4].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    let row160 = ws.Row(160)
    row160.[1].Value <- "Study Person Roles"
    row160.[2].Value <- "corresponding author"
    row160.[3].Value <- "author"
    row160.[4].Value <- "author"
    let row161 = ws.Row(161)
    row161.[1].Value <- "Study Person Roles Term Accession Number"
    let row162 = ws.Row(162)
    row162.[1].Value <- "Study Person Roles Term Source REF"
    let row163 = ws.Row(163)
    row163.[1].Value <- "Comment[Study Person REF]"
    let row164 = ws.Row(164)
    row164.[1].Value <- "#TestRemark4"
    let row165 = ws.Row(165)
    row165.[1].Value <- "#TestRemark5"
    wb

let fullInvestigationObsoleteSheetName =
    let cp =  (fullInvestigation.GetWorksheetByName "isa_investigation").Copy()
    cp.Name <- "Investigation"
    let wb = new FsWorkbook()
    wb.AddWorksheet cp
    wb

let fullInvestigationWrongSheetName =
    let cp =  (fullInvestigation.GetWorksheetByName "isa_investigation").Copy()
    cp.Name <- "Gibberish"
    let wb = new FsWorkbook()
    wb.AddWorksheet cp
    wb