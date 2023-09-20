module TestObjects.Spreadsheet.Assay

open FsSpreadsheet

[<Literal>]
let assayIdentifier = "proteome"

let assayMetadata = 
    let ws = FsWorksheet("isa_assay")
    let row1 = ws.Row(1)
    row1.[1].Value <- "ASSAY"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Assay Measurement Type"
    row2.[2].Value <- "protein expression profiling"
    let row3 = ws.Row(3)
    row3.[1].Value <- "Assay Measurement Type Term Accession Number"
    row3.[2].Value <- "http://purl.obolibrary.org/obo/OBI_0000615"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Assay Measurement Type Term Source REF"
    row4.[2].Value <- "OBI"
    let row5 = ws.Row(5)
    row5.[1].Value <- "Assay Technology Type"
    row5.[2].Value <- "mass spectrometry"
    let row6 = ws.Row(6)
    row6.[1].Value <- "Assay Technology Type Term Accession Number"
    let row7 = ws.Row(7)
    row7.[1].Value <- "Assay Technology Type Term Source REF"
    row7.[2].Value <- "OBI"
    let row8 = ws.Row(8)
    row8.[1].Value <- "Assay Technology Platform"
    row8.[2].Value <- "iTRAQ"
    let row9 = ws.Row(9)
    row9.[1].Value <- "Assay File Name"
    row9.[2].Value <- $"assays/{assayIdentifier}/isa.assay.xlsx"
    let row10 = ws.Row(10)
    row10.[1].Value <- "ASSAY PERFORMERS"    
    let row11 = ws.Row(11)
    row11.[1].Value <- "Assay Person Last Name"
    row11.[2].Value <- "Oliver"
    row11.[3].Value <- "Juan"
    row11.[4].Value <- "Leo"
    let row12 = ws.Row(12)
    row12.[1].Value <- "Assay Person First Name"
    row12.[2].Value <- "Stephen"
    row12.[3].Value <- "Castrillo"
    row12.[4].Value <- "Zeef"
    let row13 = ws.Row(13)
    row13.[1].Value <- "Assay Person Mid Initials"
    row13.[2].Value <- "G"
    row13.[3].Value <- "I"
    row13.[4].Value <- "A"
    let row14 = ws.Row(14)
    row14.[1].Value <- "Assay Person Email"
    let row15 = ws.Row(15)
    row15.[1].Value <- "Assay Person Phone"
    let row16 = ws.Row(16)
    row16.[1].Value <- "Assay Person Fax"
    let row17 = ws.Row(17)
    row17.[1].Value <- "Assay Person Address"
    row17.[2].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row17.[3].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row17.[4].Value <- "Oxford Road, Manchester M13 9PT, UK"
    let row18 = ws.Row(18)
    row18.[1].Value <- "Assay Person Affiliation"
    row18.[2].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row18.[3].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row18.[4].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    let row19 = ws.Row(19)
    row19.[1].Value <- "Assay Person Roles"
    row19.[2].Value <- "corresponding author"
    row19.[3].Value <- "author"
    row19.[4].Value <- "author"
    let row20 = ws.Row(20)
    row20.[1].Value <- "Assay Person Roles Term Accession Number"
    let row21 = ws.Row(21)
    row21.[1].Value <- "Assay Person Roles Term Source REF"
    let row22 = ws.Row(22)
    row22.[1].Value <- "Comment[Worksheet]"
    row22.[2].Value <- "Sheet1;Sheet2"
    row22.[3].Value <- "Sheet2"
    row22.[4].Value <- "Sheet3"
    let row23 = ws.Row(23)
    row23.[1].Value <- "Comment[ORCID]"
    row23.[2].Value <- "0000-0002-1825-0097"
    row23.[4].Value <- "0000-0002-1825-0098"
    ws

let assayMetadataDeprecatedKeys = 
    let ws = FsWorksheet("isa_assay")
    let row1 = ws.Row(1)
    row1.[1].Value <- "ASSAY"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Measurement Type"
    row2.[2].Value <- "protein expression profiling"
    let row3 = ws.Row(3)
    row3.[1].Value <- "Measurement Type Term Accession Number"
    row3.[2].Value <- "http://purl.obolibrary.org/obo/OBI_0000615"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Measurement Type Term Source REF"
    row4.[2].Value <- "OBI"
    let row5 = ws.Row(5)
    row5.[1].Value <- "Technology Type"
    row5.[2].Value <- "mass spectrometry"
    let row6 = ws.Row(6)
    row6.[1].Value <- "Technology Type Term Accession Number"
    let row7 = ws.Row(7)
    row7.[1].Value <- "Technology Type Term Source REF"
    row7.[2].Value <- "OBI"
    let row8 = ws.Row(8)
    row8.[1].Value <- "Technology Platform"
    row8.[2].Value <- "iTRAQ"
    let row9 = ws.Row(9)
    row9.[1].Value <- "File Name"
    row9.[2].Value <- $"assays/{assayIdentifier}/isa.assay.xlsx"
    let row10 = ws.Row(10)
    row10.[1].Value <- "ASSAY PERFORMERS"    
    let row11 = ws.Row(11)
    row11.[1].Value <- "Last Name"
    row11.[2].Value <- "Oliver"
    row11.[3].Value <- "Juan"
    row11.[4].Value <- "Leo"
    let row12 = ws.Row(12)
    row12.[1].Value <- "First Name"
    row12.[2].Value <- "Stephen"
    row12.[3].Value <- "Castrillo"
    row12.[4].Value <- "Zeef"
    let row13 = ws.Row(13)
    row13.[1].Value <- "Mid Initials"
    row13.[2].Value <- "G"
    row13.[3].Value <- "I"
    row13.[4].Value <- "A"
    let row14 = ws.Row(14)
    row14.[1].Value <- "Email"
    let row15 = ws.Row(15)
    row15.[1].Value <- "Phone"
    let row16 = ws.Row(16)
    row16.[1].Value <- "Fax"
    let row17 = ws.Row(17)
    row17.[1].Value <- "Address"
    row17.[2].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row17.[3].Value <- "Oxford Road, Manchester M13 9PT, UK"
    row17.[4].Value <- "Oxford Road, Manchester M13 9PT, UK"
    let row18 = ws.Row(18)
    row18.[1].Value <- "Affiliation"
    row18.[2].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row18.[3].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    row18.[4].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    let row19 = ws.Row(19)
    row19.[1].Value <- "Roles"
    row19.[2].Value <- "corresponding author"
    row19.[3].Value <- "author"
    row19.[4].Value <- "author"
    let row20 = ws.Row(20)
    row20.[1].Value <- "Roles Term Accession Number"
    let row21 = ws.Row(21)
    row21.[1].Value <- "Roles Term Source REF"
    let row22 = ws.Row(22)
    row22.[1].Value <- "Comment[Worksheet]"
    row22.[2].Value <- "Sheet1;Sheet2"
    row22.[3].Value <- "Sheet2"
    row22.[4].Value <- "Sheet3"
    let row23 = ws.Row(23)
    row23.[1].Value <- "Comment[Assay ORCID]"
    row23.[2].Value <- "0000-0002-1825-0097"
    row23.[4].Value <- "0000-0002-1825-0098"
    ws

let assayMetadataObsoleteSheetName = 
    let cp = assayMetadata.Copy()
    cp.Name <- "Assay"
    cp


let assayMetadataEmpty = 
    let ws = FsWorksheet("isa_assay")
    let row1 = ws.Row(1)
    row1.[1].Value <- "ASSAY"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Assay Measurement Type"
    let row3 = ws.Row(3)
    row3.[1].Value <- "Assay Measurement Type Term Accession Number"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Assay Measurement Type Term Source REF"
    let row5 = ws.Row(5)
    row5.[1].Value <- "Assay Technology Type"
    let row6 = ws.Row(6)
    row6.[1].Value <- "Assay Technology Type Term Accession Number"
    let row7 = ws.Row(7)
    row7.[1].Value <- "Assay Technology Type Term Source REF"
    let row8 = ws.Row(8)
    row8.[1].Value <- "Assay Technology Platform"
    let row9 = ws.Row(9)
    row9.[1].Value <- "Assay File Name"
    let row10 = ws.Row(10)
    row10.[1].Value <- "ASSAY PERFORMERS"    
    let row11 = ws.Row(11)
    row11.[1].Value <- "Assay Person Last Name"
    let row12 = ws.Row(12)
    row12.[1].Value <- "Assay Person First Name"
    let row13 = ws.Row(13)
    row13.[1].Value <- "Assay Person Mid Initials"
    let row14 = ws.Row(14)
    row14.[1].Value <- "Assay Person Email"
    let row15 = ws.Row(15)
    row15.[1].Value <- "Assay Person Phone"
    let row16 = ws.Row(16)
    row16.[1].Value <- "Assay Person Fax"
    let row17 = ws.Row(17)
    row17.[1].Value <- "Assay Person Address"
    let row18 = ws.Row(18)
    row18.[1].Value <- "Assay Person Affiliation"
    let row19 = ws.Row(19)
    row19.[1].Value <- "Assay Person Roles"
    let row20 = ws.Row(20)
    row20.[1].Value <- "Assay Person Roles Term Accession Number"
    let row21 = ws.Row(21)
    row21.[1].Value <- "Assay Person Roles Term Source REF"
    let row22 = ws.Row(22)
    row22.[1].Value <- "Comment[Worksheet]"
    ws


let assayMetadataEmptyObsoleteSheetName = 
    let cp = assayMetadataEmpty.Copy()
    cp.Name <- "Assay"
    cp

let assayMetadataEmptyStrings = 
    let ws = FsWorksheet("isa_assay")
    let row1 = ws.Row(1)
    row1.[1].Value <- "ASSAY"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Assay Measurement Type"
    row2.[2].Value <- ""
    let row3 = ws.Row(3)
    row3.[1].Value <- "Assay Measurement Type Term Accession Number"
    row3.[2].Value <- ""
    let row4 = ws.Row(4)
    row4.[1].Value <- "Assay Measurement Type Term Source REF"
    row4.[2].Value <- ""
    let row5 = ws.Row(5)
    row5.[1].Value <- "Assay Technology Type"
    row5.[2].Value <- ""
    let row6 = ws.Row(6)
    row6.[1].Value <- "Assay Technology Type Term Accession Number"
    row6.[2].Value <- ""
    let row7 = ws.Row(7)
    row7.[1].Value <- "Assay Technology Type Term Source REF"
    row7.[2].Value <- ""
    let row8 = ws.Row(8)
    row8.[1].Value <- "Assay Technology Platform"
    row8.[2].Value <- ""
    let row9 = ws.Row(9)
    row9.[1].Value <- "Assay File Name"
    row9.[2].Value <- $"assays/{assayIdentifier}/isa.assay.xlsx"
    let row10 = ws.Row(10)
    row10.[1].Value <- "ASSAY PERFORMERS"    
    let row11 = ws.Row(11)
    row11.[1].Value <- "Assay Person Last Name"
    row11.[2].Value <- ""
    row11.[3].Value <- ""
    row11.[4].Value <- ""
    let row12 = ws.Row(12)
    row12.[1].Value <- "Assay Person First Name"
    row12.[2].Value <- ""
    row12.[3].Value <- ""
    row12.[4].Value <- ""
    let row13 = ws.Row(13)
    row13.[1].Value <- "Assay Person Mid Initials"
    row13.[2].Value <- ""
    row13.[3].Value <- ""
    row13.[4].Value <- ""
    let row14 = ws.Row(14)
    row14.[1].Value <- "Assay Person Email"
    row14.[2].Value <- ""
    row14.[3].Value <- ""
    row14.[4].Value <- ""
    let row15 = ws.Row(15)
    row15.[1].Value <- "Assay Person Phone"
    row15.[2].Value <- ""
    row15.[3].Value <- ""
    row15.[4].Value <- ""
    let row16 = ws.Row(16)
    row16.[1].Value <- "Assay Person Fax"
    row16.[2].Value <- ""
    row16.[3].Value <- ""
    row16.[4].Value <- ""
    let row17 = ws.Row(17)
    row17.[1].Value <- "Assay Person Address"
    row17.[2].Value <- ""
    row17.[3].Value <- ""
    row17.[4].Value <- ""
    let row18 = ws.Row(18)
    row18.[1].Value <- "Assay Person Affiliation"
    row18.[2].Value <- ""
    row18.[3].Value <- ""
    row18.[4].Value <- ""
    let row19 = ws.Row(19)
    row19.[1].Value <- "Assay Person Roles"
    row19.[2].Value <- ""
    row19.[3].Value <- ""
    row19.[4].Value <- ""
    let row20 = ws.Row(20)
    row20.[1].Value <- "Assay Person Roles Term Accession Number"
    row20.[2].Value <- ""
    row20.[3].Value <- ""
    row20.[4].Value <- ""
    let row21 = ws.Row(21)
    row21.[1].Value <- "Assay Person Roles Term Source REF"
    row21.[2].Value <- ""
    row21.[3].Value <- ""
    row21.[4].Value <- ""
    ws

let assayMetadataWorkbook = 
    let wb = new FsWorkbook()
    wb.AddWorksheet(assayMetadata)
    wb

let assayMetadataWorkbookObsoleteSheetName = 
    let wb = new FsWorkbook()
    wb.AddWorksheet(assayMetadataObsoleteSheetName)
    wb

let assayMetadataWorkbookEmpty = 
    let wb = new FsWorkbook()
    wb.AddWorksheet(assayMetadataEmpty)
    wb

let assayMetadataWorkbookEmptyObsoleteSheetName = 
    let wb = new FsWorkbook()
    wb.AddWorksheet(assayMetadataEmptyObsoleteSheetName)
    wb