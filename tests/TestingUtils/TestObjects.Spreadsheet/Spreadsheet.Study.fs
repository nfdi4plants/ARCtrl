module TestObjects.Spreadsheet.Study

open FsSpreadsheet

let studyMetadataEmpty =
    let ws = FsWorksheet("isa_study")
    let row1 = ws.Row(1)
    row1.[1].Value <- "STUDY"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Study Identifier"
    let row3 = ws.Row(3)
    row3.[1].Value <- "Study Title"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Study Description"
    let row5 = ws.Row(5)
    row5.[1].Value <- "Study Submission Date"
    let row6 = ws.Row(6)
    row6.[1].Value <- "Study Public Release Date"
    let row7 = ws.Row(7)
    row7.[1].Value <- "Study File Name"
    let row8 = ws.Row(8)
    row8.[1].Value <- "STUDY DESIGN DESCRIPTORS"
    let row9 = ws.Row(9)
    row9.[1].Value <- "Study Design Type"
    let row10 = ws.Row(10)
    row10.[1].Value <- "Study Design Type Term Accession Number"
    let row11 = ws.Row(11)
    row11.[1].Value <- "Study Design Type Term Source REF"
    let row12 = ws.Row(12)
    row12.[1].Value <- "STUDY PUBLICATIONS"
    let row13 = ws.Row(13)
    row13.[1].Value <- "Study Publication PubMed ID"
    let row14 = ws.Row(14)
    row14.[1].Value <- "Study Publication DOI"
    let row15 = ws.Row(15)
    row15.[1].Value <- "Study Publication Author List"
    let row16 = ws.Row(16)
    row16.[1].Value <- "Study Publication Title"
    let row17 = ws.Row(17)
    row17.[1].Value <- "Study Publication Status"
    let row18 = ws.Row(18)
    row18.[1].Value <- "Study Publication Status Term Accession Number"
    let row19 = ws.Row(19)
    row19.[1].Value <- "Study Publication Status Term Source REF"
    let row20 = ws.Row(20)
    row20.[1].Value <- "STUDY FACTORS"
    let row21 = ws.Row(21)
    row21.[1].Value <- "Study Factor Name"
    let row22 = ws.Row(22)
    row22.[1].Value <- "Study Factor Type"
    let row23 = ws.Row(23)
    row23.[1].Value <- "Study Factor Type Term Accession Number"
    let row24 = ws.Row(24)
    row24.[1].Value <- "Study Factor Type Term Source REF"
    let row25 = ws.Row(25)
    row25.[1].Value <- "STUDY ASSAYS"
    let row26 = ws.Row(26)
    row26.[1].Value <- "Study Assay Measurement Type"
    let row27 = ws.Row(27)
    row27.[1].Value <- "Study Assay Measurement Type Term Accession Number"
    let row28 = ws.Row(28)
    row28.[1].Value <- "Study Assay Measurement Type Term Source REF"
    let row29 = ws.Row(29)
    row29.[1].Value <- "Study Assay Technology Type"
    let row30 = ws.Row(30)
    row30.[1].Value <- "Study Assay Technology Type Term Accession Number"
    let row31 = ws.Row(31)
    row31.[1].Value <- "Study Assay Technology Type Term Source REF"
    let row32 = ws.Row(32)
    row32.[1].Value <- "Study Assay Technology Platform"
    let row33 = ws.Row(33)
    row33.[1].Value <- "Study Assay File Name"
    let row34 = ws.Row(34)
    row34.[1].Value <- "STUDY PROTOCOLS"
    let row35 = ws.Row(35)
    row35.[1].Value <- "Study Protocol Name"
    let row36 = ws.Row(36)
    row36.[1].Value <- "Study Protocol Type"
    let row37 = ws.Row(37)
    row37.[1].Value <- "Study Protocol Type Term Accession Number"
    let row38 = ws.Row(38)
    row38.[1].Value <- "Study Protocol Type Term Source REF"
    let row39 = ws.Row(39)
    row39.[1].Value <- "Study Protocol Description"
    let row40 = ws.Row(40)
    row40.[1].Value <- "Study Protocol URI"
    let row41 = ws.Row(41)
    row41.[1].Value <- "Study Protocol Version"
    let row42 = ws.Row(42)
    row42.[1].Value <- "Study Protocol Parameters Name"
    let row43 = ws.Row(43)
    row43.[1].Value <- "Study Protocol Parameters Term Accession Number"
    let row44 = ws.Row(44)
    row44.[1].Value <- "Study Protocol Parameters Term Source REF"
    let row45 = ws.Row(45)
    row45.[1].Value <- "Study Protocol Components Name"
    let row46 = ws.Row(46)
    row46.[1].Value <- "Study Protocol Components Type"
    let row47 = ws.Row(47)
    row47.[1].Value <- "Study Protocol Components Type Term Accession Number"
    let row48 = ws.Row(48)
    row48.[1].Value <- "Study Protocol Components Type Term Source REF"
    let row49 = ws.Row(49)
    row49.[1].Value <- "STUDY CONTACTS"
    let row50 = ws.Row(50)
    row50.[1].Value <- "Study Person Last Name"
    let row51 = ws.Row(51)
    row51.[1].Value <- "Study Person First Name"
    let row52 = ws.Row(52)
    row52.[1].Value <- "Study Person Mid Initials"
    let row53 = ws.Row(53)
    row53.[1].Value <- "Study Person Email"
    let row54 = ws.Row(54)
    row54.[1].Value <- "Study Person Phone"
    let row55 = ws.Row(55)
    row55.[1].Value <- "Study Person Fax"
    let row56 = ws.Row(56)
    row56.[1].Value <- "Study Person Address"
    let row57 = ws.Row(57)
    row57.[1].Value <- "Study Person Affiliation"
    let row58 = ws.Row(58)
    row58.[1].Value <- "Study Person Roles"
    let row59 = ws.Row(59)
    row59.[1].Value <- "Study Person Roles Term Accession Number"
    let row60 = ws.Row(60)
    row60.[1].Value <- "Study Person Roles Term Source REF"
    let row61 = ws.Row(61)
    row61.[1].Value <- "Comment[Study Person REF]"
    ws

[<Literal>]
let studyIdentifier = "BII-S-1"

let studyMetadata =
    let ws = FsWorksheet("isa_study")
    let row1 = ws.Row(1)
    row1.[1].Value <- "STUDY"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Study Identifier"
    row2.[2].Value <- studyIdentifier
    let row3 = ws.Row(3)
    row3.[1].Value <- "Study Title"
    row3.[2].Value <- "Study of the impact of changes in flux on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae under different nutrient limitations"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Study Description"
    row4.[2].Value <- "We wished to study the impact of growth rate on the total complement of mRNA molecules, proteins, and metabolites in S. cerevisiae, independent of any nutritional or other physiological effects. To achieve this, we carried out our analyses on yeast grown in steady-state chemostat culture under four different nutrient limitations (glucose, ammonium, phosphate, and sulfate) at three different dilution (that is, growth) rates (D = u = 0.07, 0.1, and 0.2/hour, equivalent to population doubling times (Td) of 10 hours, 7 hours, and 3.5 hours, respectively; u = specific growth rate defined as grams of biomass generated per gram of biomass present per unit time)."
    let row5 = ws.Row(5)
    row5.[1].Value <- "Study Submission Date"
    row5.[2].Value <- "2007-04-30"
    let row6 = ws.Row(6)
    row6.[1].Value <- "Study Public Release Date"
    row6.[2].Value <- "2009-03-10"
    let row7 = ws.Row(7)
    row7.[1].Value <- "Study File Name"
    row7.[2].Value <- $"studies/{studyIdentifier}/isa.study.xlsx"
    let row8 = ws.Row(8)
    row8.[1].Value <- "STUDY DESIGN DESCRIPTORS"
    let row9 = ws.Row(9)
    row9.[1].Value <- "Study Design Type"
    row9.[2].Value <- "intervention design"
    let row10 = ws.Row(10)
    row10.[1].Value <- "Study Design Type Term Accession Number"
    row10.[2].Value <- "http://purl.obolibrary.org/obo/OBI_0000115"
    let row11 = ws.Row(11)
    row11.[1].Value <- "Study Design Type Term Source REF"
    row11.[2].Value <- "OBI"
    let row12 = ws.Row(12)
    row12.[1].Value <- "STUDY PUBLICATIONS"
    let row13 = ws.Row(13)
    row13.[1].Value <- "Study Publication PubMed ID"
    row13.[2].Value <- "17439666"
    let row14 = ws.Row(14)
    row14.[1].Value <- "Study Publication DOI"
    row14.[2].Value <- "doi:10.1186/jbiol54"
    let row15 = ws.Row(15)
    row15.[1].Value <- "Study Publication Author List"
    row15.[2].Value <- "Castrillo JI, Zeef LA, Hoyle DC, Zhang N, Hayes A, Gardner DC, Cornell MJ, Petty J, Hakes L, Wardleworth L, Rash B, Brown M, Dunn WB, Broadhurst D, O'Donoghue K, Hester SS, Dunkley TP, Hart SR, Swainston N, Li P, Gaskell SJ, Paton NW, Lilley KS, Kell DB, Oliver SG."
    let row16 = ws.Row(16)
    row16.[1].Value <- "Study Publication Title"
    row16.[2].Value <- "Growth control of the eukaryote cell: a systems biology study in yeast."
    let row17 = ws.Row(17)
    row17.[1].Value <- "Study Publication Status"
    row17.[2].Value <- "published"
    let row18 = ws.Row(18)
    row18.[1].Value <- "Study Publication Status Term Accession Number"
    let row19 = ws.Row(19)
    row19.[1].Value <- "Study Publication Status Term Source REF"
    let row20 = ws.Row(20)
    row20.[1].Value <- "STUDY FACTORS"
    let row21 = ws.Row(21)
    row21.[1].Value <- "Study Factor Name"
    row21.[2].Value <- "limiting nutrient"
    row21.[3].Value <- "rate"
    let row22 = ws.Row(22)
    row22.[1].Value <- "Study Factor Type"
    row22.[2].Value <- "chemical compound"
    row22.[3].Value <- "rate"
    let row23 = ws.Row(23)
    row23.[1].Value <- "Study Factor Type Term Accession Number"
    row23.[3].Value <- "http://purl.obolibrary.org/obo/PATO_0000161"
    let row24 = ws.Row(24)
    row24.[1].Value <- "Study Factor Type Term Source REF"
    row24.[3].Value <- "PATO"
    let row25 = ws.Row(25)
    row25.[1].Value <- "STUDY ASSAYS"
    let row26 = ws.Row(26)
    row26.[1].Value <- "Study Assay Measurement Type"
    row26.[2].Value <- "protein expression profiling"
    row26.[3].Value <- "metabolite profiling"
    row26.[4].Value <- "transcription profiling"
    let row27 = ws.Row(27)
    row27.[1].Value <- "Study Assay Measurement Type Term Accession Number"
    row27.[2].Value <- "http://purl.obolibrary.org/obo/OBI_0000615"
    row27.[3].Value <- "http://purl.obolibrary.org/obo/OBI_0000366"
    row27.[4].Value <- "424"
    let row28 = ws.Row(28)
    row28.[1].Value <- "Study Assay Measurement Type Term Source REF"
    row28.[2].Value <- "OBI"
    row28.[3].Value <- "OBI"
    row28.[4].Value <- "OBI"
    let row29 = ws.Row(29)
    row29.[1].Value <- "Study Assay Technology Type"
    row29.[2].Value <- "mass spectrometry"
    row29.[3].Value <- "mass spectrometry"
    row29.[4].Value <- "DNA microarray"
    let row30 = ws.Row(30)
    row30.[1].Value <- "Study Assay Technology Type Term Accession Number"
    row30.[4].Value <- "http://purl.obolibrary.org/obo/OBI_0400148"
    let row31 = ws.Row(31)
    row31.[1].Value <- "Study Assay Technology Type Term Source REF"
    row31.[2].Value <- "OBI"
    row31.[3].Value <- "OBI"
    row31.[4].Value <- "OBI"
    let row32 = ws.Row(32)
    row32.[1].Value <- "Study Assay Technology Platform"
    row32.[2].Value <- "iTRAQ"
    row32.[3].Value <- "LC-MS/MS"
    row32.[4].Value <- "Affymetrix"
    let row33 = ws.Row(33)
    row33.[1].Value <- "Study Assay File Name"
    row33.[2].Value <- $"assays/{Assay.assayIdentifier}/isa.assay.xlsx"
    row33.[3].Value <- "assays/metabolome/isa.assay.xlsx"
    row33.[4].Value <- "assays/transcriptome/isa.assay.xlsx"
    let row34 = ws.Row(34)
    row34.[1].Value <- "STUDY PROTOCOLS"
    let row35 = ws.Row(35)
    row35.[1].Value <- "Study Protocol Name"
    row35.[2].Value <- "growth protocol"
    row35.[3].Value <- "mRNA extraction"
    row35.[4].Value <- "protein extraction"
    row35.[5].Value <- "biotin labeling"
    row35.[6].Value <- "ITRAQ labeling"
    row35.[7].Value <- "EukGE-WS4"
    row35.[8].Value <- "metabolite extraction"
    let row36 = ws.Row(36)
    row36.[1].Value <- "Study Protocol Type"
    row36.[2].Value <- "growth"
    row36.[3].Value <- "mRNA extraction"
    row36.[4].Value <- "protein extraction"
    row36.[5].Value <- "labeling"
    row36.[6].Value <- "labeling"
    row36.[7].Value <- "hybridization"
    row36.[8].Value <- "extraction"
    let row37 = ws.Row(37)
    row37.[1].Value <- "Study Protocol Type Term Accession Number"
    row37.[8].Value <- "http://purl.obolibrary.org/obo/OBI_0302884"
    let row38 = ws.Row(38)
    row38.[1].Value <- "Study Protocol Type Term Source REF"
    row38.[8].Value <- "OBI"
    let row39 = ws.Row(39)
    row39.[1].Value <- "Study Protocol Description"
    row39.[2].Value <- "1. Biomass samples (45 ml) were taken via the sample port of the Applikon fermenters. The cells were pelleted by centrifugation for 5 min at 5000 rpm. The supernatant was removed and the RNA pellet resuspended in the residual medium to form a slurry. This was added in a dropwise manner directly into a 5 ml Teflon flask (B. Braun Biotech, Germany) containing liquid nitrogen and a 7 mm-diameter tungsten carbide ball. After allowing evaporation of the liquid nitrogen the flask was reassembled and the cells disrupted by agitation at 1500 rpm for 2 min in a Microdismembranator U (B. Braun Biotech, Germany) 2. The frozen powder was then dissolved in 1 ml of TriZol reagent (Sigma-Aldrich, UK), vortexed for 1 min, and then kept at room temperature for a further 5min. 3. Chloroform extraction was performed by addition of 0.2 ml chloroform, shaking vigorously or 15 s, then 5min incubation at room temperature. 4. Following centrifugation at 12,000 rpm for 5 min, the RNA (contained in the aqueous phase) was precipitated with 0.5 vol of 2-propanol at room temperature for 15 min. 5. After further centrifugation (12,000 rpm for 10 min at 4 C) the RNA pellet was washed twice with 70 % (v/v) ethanol, briefly air-dried, and redissolved in 0.5 ml diethyl pyrocarbonate (DEPC)-treated water. 6. The single-stranded RNA was precipitated once more by addition of 0.5 ml of LiCl buffer (4 M LiCl, 20 mM Tris-HCl, pH 7.5, 10 mM EDTA), thus removing tRNA and DNA from the sample. 7. After precipitation (20 C for 1h) and centrifugation (12,000 rpm, 30 min, 4 C), the RNA was washed twice in 70 % (v/v) ethanol prior to being dissolved in a minimal volume of DEPC-treated water. 8. Total RNA quality was checked using the RNA 6000 Nano Assay, and analysed on an Agilent 2100 Bioanalyser (Agilent Technologies). RNA was quantified using the Nanodrop ultra low volume spectrophotometer (Nanodrop Technologies)."
    row39.[3].Value <- "1. Biomass samples (45 ml) were taken via the sample port of the Applikon fermenters. The cells were pelleted by centrifugation for 5 min at 5000 rpm. The supernatant was removed and the RNA pellet resuspended in the residual medium to form a slurry. This was added in a dropwise manner directly into a 5 ml Teflon flask (B. Braun Biotech, Germany) containing liquid nitrogen and a 7 mm-diameter tungsten carbide ball. After allowing evaporation of the liquid nitrogen the flask was reassembled and the cells disrupted by agitation at 1500 rpm for 2 min in a Microdismembranator U (B. Braun Biotech, Germany) 2. The frozen powder was then dissolved in 1 ml of TriZol reagent (Sigma-Aldrich, UK), vortexed for 1 min, and then kept at room temperature for a further 5min. 3. Chloroform extraction was performed by addition of 0.2 ml chloroform, shaking vigorouslyor 15 s, then 5min incubation at room temperature. 4. Following centrifugation at 12,000 rpm for 5 min, the RNA (contained in the aqueous phase) was precipitated with 0.5 vol of 2-propanol at room temperature for 15 min. 5. After further centrifugation (12,000 rpm for 10 min at 4 C) the RNA pellet was washed twice with 70 % (v/v) ethanol, briefly air-dried, and redissolved in 0.5 ml diethyl pyrocarbonate (DEPC)-treated water. 6. The single-stranded RNA was precipitated once more by addition of 0.5 ml of LiCl bffer (4 M LiCl, 20 mM Tris-HCl, pH 7.5, 10 mM EDTA), thus removing tRNA and DNA from the sample. 7. After precipitation (20 C for 1 h) and centrifugation (12,000 rpm, 30 min, 4 C), the RNA was washed twice in 70 % (v/v) ethanol prior to being dissolved in a minimal volume of DEPC-treated water. 8. Total RNA quality was checked using the RNA 6000 Nano Assay, and analysed on an Agilent 2100 Bioanalyser (Agilent Technologies). RNA was quantified using the Nanodrop ultra low volume spectrophotometer (Nanodrop Technologies)."
    row39.[5].Value <- "This was done using Enzo BioArrayTM HighYieldTM RNA transcript labelling kit (T7) with 5 ul cDNA. The resultant cRNA was again purified using the GeneChip Sample Clean Up Module. The column was eluted in the first instance using 10 l RNase-free water, and for a second time using 11 ul RNase-free water. cRNA was quantified using the Nanodrop spectrophotometer. A total of 15 ug of cRNA (required for hybridisation) was fragmented. Fragmentation was carried out by using 2 ul of fragmentation buffer for every 8 ul cRNA."
    row39.[7].Value <- "For each target, a hybridisation cocktail was made using the standard array recipe as described in the GeneChip Expression Analysis technical manual. GeneChip control oligonucleotide and 20x eukaryotic hybridisation controls were used. Hybridisation buffer was made as detailed in the GeneChip manual and the BSA and herring sperm DNA was purchased from Invitrogen. The cocktail was heated to 99 C for 5 min, transferred to 45 C for 5 min and then spun for 5 min to remove any insoluble material. Affymetrix Yeast Yg_s98 S. cerevisiae arrays were pre-hybridised with 200 ul 1x hybridisation buffer and incubated at 45 C for 10 min. 200 ul of the hybridisation cocktail was loaded onto the arrays. The probe array was incubated in a rotisserie at 45 C, rotating at 60 rpm. Following hybridisation, for 16 hr, chips were loaded onto a Fluidics station for washing and staining using the EukGe WS2v4 programme controlled using Microarray Suite 5 software."
    let row40 = ws.Row(40)
    row40.[1].Value <- "Study Protocol URI"
    let row41 = ws.Row(41)
    row41.[1].Value <- "Study Protocol Version"
    let row42 = ws.Row(42)
    row42.[1].Value <- "Study Protocol Parameters Name"
    row42.[8].Value <- "sample volume;standard volume"
    let row43 = ws.Row(43)
    row43.[1].Value <- "Study Protocol Parameters Term Accession Number"
    row43.[8].Value <- ";"
    let row44 = ws.Row(44)
    row44.[1].Value <- "Study Protocol Parameters Term Source REF"
    row44.[8].Value <- ";"
    let row45 = ws.Row(45)
    row45.[1].Value <- "Study Protocol Components Name"
    let row46 = ws.Row(46)
    row46.[1].Value <- "Study Protocol Components Type"
    let row47 = ws.Row(47)
    row47.[1].Value <- "Study Protocol Components Type Term Accession Number"
    let row48 = ws.Row(48)
    row48.[1].Value <- "Study Protocol Components Type Term Source REF"
    let row49 = ws.Row(49)
    row49.[1].Value <- "STUDY CONTACTS"
    let row50 = ws.Row(50)
    row50.[1].Value <- "Study Person Last Name"
    row50.[2].Value <- "Oliver"
    row50.[3].Value <- "Juan"
    row50.[4].Value <- "Leo"
    let row51 = ws.Row(51)
    row51.[1].Value <- "Study Person First Name"
    row51.[2].Value <- "Stephen"
    row51.[3].Value <- "Castrillo"
    row51.[4].Value <- "Zeef"
    let row52 = ws.Row(52)
    row52.[1].Value <- "Study Person Mid Initials"
    row52.[2].Value <- "G"
    row52.[3].Value <- "I"
    row52.[4].Value <- "A"
    let row53 = ws.Row(53)
    row53.[1].Value <- "Study Person Email"
    let row54 = ws.Row(54)
    row54.[1].Value <- "Study Person Phone"
    let row55 = ws.Row(55)
    row55.[1].Value <- "Study Person Fax"
    let row56 = ws.Row(56)
    row56.[1].Value <- "Study Person Address"
    row56.[2].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row56.[3].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row56.[4].Value <- "Oxford Road, Manchester M13 9PT, UK"
    let row57 = ws.Row(57)
    row57.[1].Value <- "Study Person Affiliation"
    row57.[2].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row57.[3].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row57.[4].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    let row58 = ws.Row(58)
    row58.[1].Value <- "Study Person Roles"
    row58.[2].Value <- "corresponding author"
    row58.[3].Value <- "author"
    row58.[4].Value <- "author"
    let row59 = ws.Row(59)
    row59.[1].Value <- "Study Person Roles Term Accession Number"
    let row60 = ws.Row(60)
    row60.[1].Value <- "Study Person Roles Term Source REF"
    let row61 = ws.Row(61)
    row61.[1].Value <- "Comment[Study Person REF]"
    ws

let studyMetadataEmptyObsoleteSheetName =
    let cp = studyMetadataEmpty.Copy()
    cp.Name <- "Study"
    cp