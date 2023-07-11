﻿module TestObjects.Assay

open FsSpreadsheet

let assayMetadata = 
    let ws = FsWorksheet("Assay")
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
    row9.[2].Value <- "MyAssay.xlsx"
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
    ws


let assayMetadataEmpty = 
    let ws = FsWorksheet("Assay")
    let row1 = ws.Row(1)
    row1.[1].Value <- "ASSAY"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Measurement Type"
    let row3 = ws.Row(3)
    row3.[1].Value <- "Measurement Type Term Accession Number"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Measurement Type Term Source REF"
    let row5 = ws.Row(5)
    row5.[1].Value <- "Technology Type"
    let row6 = ws.Row(6)
    row6.[1].Value <- "Technology Type Term Accession Number"
    let row7 = ws.Row(7)
    row7.[1].Value <- "Technology Type Term Source REF"
    let row8 = ws.Row(8)
    row8.[1].Value <- "Technology Platform"
    let row9 = ws.Row(9)
    row9.[1].Value <- "File Name"
    let row10 = ws.Row(10)
    row10.[1].Value <- "ASSAY PERFORMERS"    
    let row11 = ws.Row(11)
    row11.[1].Value <- "Last Name"
    let row12 = ws.Row(12)
    row12.[1].Value <- "First Name"
    let row13 = ws.Row(13)
    row13.[1].Value <- "Mid Initials"
    let row14 = ws.Row(14)
    row14.[1].Value <- "Email"
    let row15 = ws.Row(15)
    row15.[1].Value <- "Phone"
    let row16 = ws.Row(16)
    row16.[1].Value <- "Fax"
    let row17 = ws.Row(17)
    row17.[1].Value <- "Address"
    let row18 = ws.Row(18)
    row18.[1].Value <- "Affiliation"
    let row19 = ws.Row(19)
    row19.[1].Value <- "Roles"
    let row20 = ws.Row(20)
    row20.[1].Value <- "Roles Term Accession Number"
    let row21 = ws.Row(21)
    row21.[1].Value <- "Roles Term Source REF"
    let row22 = ws.Row(22)
    row22.[1].Value <- "Comment[Worksheet]"
    ws