module TestObjects.Study

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
    row2.[1].Value <- "Study Factor Type"
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
    ws

let studyMetadataEmptyObsoleteSheetName =
    let cp = studyMetadataEmpty.Copy()
    cp.Name <- "Study"
    cp