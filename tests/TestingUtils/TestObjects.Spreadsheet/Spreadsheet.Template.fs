module TestObjects.Spreadsheet.Template

open FsSpreadsheet

let templateTableName = "plant_growth"

let templateMetadata = 
    let ws = FsWorksheet("isa_template")
    let row1 = ws.Row(1)
    row1.[1].Value <- "TEMPLATE"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Id"
    row2.[2].Value <- "f12e98ee-a4e7-4ada-ba56-1e13cce1a44b"
    let row3 = ws.Row(3)
    row3.[1].Value <- "Name"
    row3.[2].Value <- "Plant growth"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Version"
    row4.[2].Value <- "1.2.0"
    let row5 = ws.Row(5)
    row5.[1].Value <- "Description"
    row5.[2].Value <- "Template to describe a plant growth study as well as sample collection and handling."
    let row6 = ws.Row(6)
    row6.[1].Value <- "Organisation"
    row6.[2].Value <- "DataPLANT"
    let row7 = ws.Row(7)
    row7.[1].Value <- "Table"
    row7.[2].Value <- templateTableName
    let row8 = ws.Row(8)
    row8.[1].Value <- "ERS"
    row8.[2].Value <- ""
    let row9 = ws.Row(9)
    row9.[1].Value <- "ER"
    row9.[2].Value <- "GEO"
    row9.[3].Value <- "METABOLIGHTS"
    row9.[4].Value <- "PRIDE"
    row9.[5].Value <- "BIOSAMPLE"
    row9.[6].Value <- "ENA"
    let row10 = ws.Row(10)
    row10.[1].Value <- "ER Term Accession Number"
    row10.[2].Value <- "http://purl.obolibrary.org/obo/DPBO_1000096"
    row10.[3].Value <- "http://purl.obolibrary.org/obo/NFDI4PSO_1000097"
    row10.[4].Value <- "http://purl.obolibrary.org/obo/NFDI4PSO_1000098"
    row10.[5].Value <- "http://purl.obolibrary.org/obo/NFDI4PSO_0010002"
    row10.[6].Value <- "http://purl.obolibrary.org/obo/DPBO_0010000"
    let row11 = ws.Row(11)
    row11.[1].Value <- "ER Term Source REF"
    row11.[2].Value <- "DPBO"
    row11.[3].Value <- "NFDI4PSO"
    row11.[4].Value <- "NFDI4PSO"
    row11.[5].Value <- "NFDI4PSO"
    row11.[6].Value <- "DPBO"
    let row12 = ws.Row(12)
    row12.[1].Value <- "TAGS"
    row12.[2].Value <- ""
    let row13 = ws.Row(13)
    row13.[1].Value <- "Tags"
    row13.[2].Value <- "Plants"
    row13.[3].Value <- "Sampling Method"
    row13.[4].Value <- "plant growth protocol"
    row13.[5].Value <- "Plant study"
    row13.[6].Value <- "Plant Sample Checklist"
    let row14 = ws.Row(14)
    row14.[1].Value <- "Tags Term Accession Number"
    row14.[2].Value <- ""
    row14.[3].Value <- "http://purl.obolibrary.org/obo/NCIT_C71492"
    row14.[4].Value <- "http://purl.obolibrary.org/obo/DPBO_1000164"
    row14.[5].Value <- ""
    row14.[6].Value <- ""
    let row15 = ws.Row(15)
    row15.[1].Value <- "Tags Term Source REF"
    row15.[2].Value <- ""
    row15.[3].Value <- "NCIT"
    row15.[4].Value <- "DPBO"
    row15.[5].Value <- ""
    row15.[6].Value <- ""
    let row16 = ws.Row(16)
    row16.[1].Value <- "AUTHORS"
    row16.[2].Value <- ""
    let row17 = ws.Row(17)
    row17.[1].Value <- "Author Last Name"
    row17.[2].Value <- "Weil"
    row17.[3].Value <- "Brilhaus"
    row17.[4].Value <- "Maus"
    row17.[5].Value <- "Kuhl"
    row17.[6].Value <- "Zhou"
    let row18 = ws.Row(18)
    row18.[1].Value <- "Author First Name"
    row18.[2].Value <- "Heinrich"
    row18.[3].Value <- "Dominik"
    row18.[4].Value <- "Oliver"
    row18.[5].Value <- "Martin"
    row18.[6].Value <- "Xiaoran"
    let row19 = ws.Row(19)
    row19.[1].Value <- "Author Mid Initials"
    row19.[2].Value <- "L"
    row19.[3].Value <- ""
    row19.[4].Value <- ""
    row19.[5].Value <- ""
    row19.[6].Value <- ""
    let row20 = ws.Row(20)
    row20.[1].Value <- "Author Email"
    row20.[2].Value <- "weil@rptu.de"
    row20.[3].Value <- ""
    row20.[4].Value <- ""
    row20.[5].Value <- ""
    row20.[6].Value <- ""
    let row21 = ws.Row(21)
    row21.[1].Value <- "Author Phone"
    row21.[2].Value <- ""
    row21.[3].Value <- ""
    row21.[4].Value <- ""
    row21.[5].Value <- ""
    row21.[6].Value <- ""
    let row22 = ws.Row(22)
    row22.[1].Value <- "Author Fax"
    row22.[2].Value <- ""
    row22.[3].Value <- ""
    row22.[4].Value <- ""
    row22.[5].Value <- ""
    row22.[6].Value <- ""
    let row23 = ws.Row(23)
    row23.[1].Value <- "Author Address"
    row23.[2].Value <- ""
    row23.[3].Value <- ""
    row23.[4].Value <- ""
    row23.[5].Value <- ""
    row23.[6].Value <- ""
    let row24 = ws.Row(24)
    row24.[1].Value <- "Author Affiliation"
    row24.[2].Value <- ""
    row24.[3].Value <- ""
    row24.[4].Value <- ""
    row24.[5].Value <- ""
    row24.[6].Value <- ""
    let row25 = ws.Row(25)
    row25.[1].Value <- "Author Roles"
    row25.[2].Value <- ""
    row25.[3].Value <- ""
    row25.[4].Value <- ""
    row25.[5].Value <- ""
    row25.[6].Value <- ""
    let row26 = ws.Row(26)
    row26.[1].Value <- "Author Roles Term Accession Number"
    row26.[2].Value <- ""
    row26.[3].Value <- ""
    row26.[4].Value <- ""
    row26.[5].Value <- ""
    row26.[6].Value <- ""
    let row27 = ws.Row(27)
    row27.[1].Value <- "Author Roles Term Source REF"
    row27.[2].Value <- ""
    row27.[3].Value <- ""
    row27.[4].Value <- ""
    row27.[5].Value <- ""
    row27.[6].Value <- ""
    let row28 = ws.Row(28)
    row28.[1].Value <- "Comment[ORCID]"
    row28.[2].Value <- "0000-0003-1945-6342"
    row28.[3].Value <- ""
    row28.[4].Value <- ""
    row28.[5].Value <- ""
    row28.[6].Value <- ""
    ws

let templateMetadata_deprecatedKeys = 
    let ws = FsWorksheet("SwateTemplateMetadata")
    let row1 = ws.Row(1)
    row1.[1].Value <- "Id"
    row1.[2].Value <- "f12e98ee-a4e7-4ada-ba56-1e13cce1a44b"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Name"
    row2.[2].Value <- "Plant growth"
    let row3 = ws.Row(3)
    row3.[1].Value <- "Version"
    row3.[2].Value <- "1.2.0"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Description"
    row4.[2].Value <- "Template to describe a plant growth study as well as sample collection and handling."
    let row5 = ws.Row(5)
    row5.[1].Value <- "Organisation"
    row5.[2].Value <- "DataPLANT"
    let row6 = ws.Row(6)
    row6.[1].Value <- "Table"
    row6.[2].Value <- "annotationTableUnluckyVampirebat89"
    let row7 = ws.Row(7)
    row7.[1].Value <- "#ER list"
    row7.[2].Value <- ""
    let row8 = ws.Row(8)
    row8.[1].Value <- "ER"
    row8.[2].Value <- "GEO"
    row8.[3].Value <- "METABOLIGHTS"
    row8.[4].Value <- "PRIDE"
    row8.[5].Value <- "BIOSAMPLE"
    row8.[6].Value <- "ENA"
    let row9 = ws.Row(9)
    row9.[1].Value <- "ER Term Accession Number"
    row9.[2].Value <- "http://purl.obolibrary.org/obo/DPBO_1000096"
    row9.[3].Value <- "NFDI4PSO:1000097"
    row9.[4].Value <- "NFDI4PSO:1000098"
    row9.[5].Value <- "NFDI4PSO:0010002"
    row9.[6].Value <- "DPBO:0010000"
    let row10 = ws.Row(10)
    row10.[1].Value <- "ER Term Source REF"
    row10.[2].Value <- "DPBO"
    row10.[3].Value <- "NFDI4PSO"
    row10.[4].Value <- "NFDI4PSO"
    row10.[5].Value <- "NFDI4PSO"
    row10.[6].Value <- "DPBO"
    let row11 = ws.Row(11)
    row11.[1].Value <- "#TAGS list"
    row11.[2].Value <- ""
    let row12 = ws.Row(12)
    row12.[1].Value <- "Tags"
    row12.[2].Value <- "Plants"
    row12.[3].Value <- "Sampling Method"
    row12.[4].Value <- "plant growth protocol"
    row12.[5].Value <- "Plant study"
    row12.[6].Value <- "Plant Sample Checklist"
    let row13 = ws.Row(13)
    row13.[1].Value <- "Tags Term Accession Number"
    row13.[2].Value <- ""
    row13.[3].Value <- "http://purl.obolibrary.org/obo/NCIT_C71492"
    row13.[4].Value <- "http://purl.obolibrary.org/obo/DPBO_1000164"
    row13.[5].Value <- ""
    row13.[6].Value <- ""
    let row14 = ws.Row(14)
    row14.[1].Value <- "Tags Term Source REF"
    row14.[2].Value <- ""
    row14.[3].Value <- "NCIT"
    row14.[4].Value <- "DPBO"
    row14.[5].Value <- ""
    row14.[6].Value <- ""
    let row15 = ws.Row(15)
    row15.[1].Value <- "#AUTHORS list"
    row15.[2].Value <- ""
    let row16 = ws.Row(16)
    row16.[1].Value <- "Authors Last Name"
    row16.[2].Value <- "Weil"
    row16.[3].Value <- "Brilhaus"
    row16.[4].Value <- "Maus"
    row16.[5].Value <- "Kuhl"
    row16.[6].Value <- "Zhou"
    let row17 = ws.Row(17)
    row17.[1].Value <- "Authors First Name"
    row17.[2].Value <- "Heinrich"
    row17.[3].Value <- "Dominik"
    row17.[4].Value <- "Oliver"
    row17.[5].Value <- "Martin"
    row17.[6].Value <- "Xiaoran"
    let row18 = ws.Row(18)
    row18.[1].Value <- "Authors Mid Initials"
    row18.[2].Value <- "L"
    row18.[3].Value <- ""
    row18.[4].Value <- ""
    row18.[5].Value <- ""
    row18.[6].Value <- ""
    let row19 = ws.Row(19)
    row19.[1].Value <- "Authors Email"
    row19.[2].Value <- "weil@rptu.de"
    row19.[3].Value <- ""
    row19.[4].Value <- ""
    row19.[5].Value <- ""
    row19.[6].Value <- ""
    let row20 = ws.Row(20)
    row20.[1].Value <- "Authors Phone"
    row20.[2].Value <- ""
    row20.[3].Value <- ""
    row20.[4].Value <- ""
    row20.[5].Value <- ""
    row20.[6].Value <- ""
    let row21 = ws.Row(21)
    row21.[1].Value <- "Authors Fax"
    row21.[2].Value <- ""
    row21.[3].Value <- ""
    row21.[4].Value <- ""
    row21.[5].Value <- ""
    row21.[6].Value <- ""
    let row22 = ws.Row(22)
    row22.[1].Value <- "Authors Address"
    row22.[2].Value <- ""
    row22.[3].Value <- ""
    row22.[4].Value <- ""
    row22.[5].Value <- ""
    row22.[6].Value <- ""
    let row23 = ws.Row(23)
    row23.[1].Value <- "Authors Affiliation"
    row23.[2].Value <- ""
    row23.[3].Value <- ""
    row23.[4].Value <- ""
    row23.[5].Value <- ""
    row23.[6].Value <- ""
    let row24 = ws.Row(24)
    row24.[1].Value <- "Authors ORCID"
    row24.[2].Value <- "0000-0003-1945-6342"
    row24.[3].Value <- ""
    row24.[4].Value <- ""
    row24.[5].Value <- ""
    row24.[6].Value <- ""
    let row25 = ws.Row(25)
    row25.[1].Value <- "Authors Role"
    row25.[2].Value <- ""
    row25.[3].Value <- ""
    row25.[4].Value <- ""
    row25.[5].Value <- ""
    row25.[6].Value <- ""
    let row26 = ws.Row(26)
    row26.[1].Value <- "Authors Role Term Accession Number"
    row26.[2].Value <- ""
    row26.[3].Value <- ""
    row26.[4].Value <- ""
    row26.[5].Value <- ""
    row26.[6].Value <- ""
    let row27 = ws.Row(27)
    row27.[1].Value <- "Authors Role Term Source REF"
    row27.[2].Value <- ""
    row27.[3].Value <- ""
    row27.[4].Value <- ""
    row27.[5].Value <- ""
    row27.[6].Value <- ""
    ws


let private templateTableCols = 
    [
            TestObjects.Spreadsheet.ArcTable.Input.appendSampleColumn 4
            TestObjects.Spreadsheet.ArcTable.Parameter.appendInstrumentColumn 4
            TestObjects.Spreadsheet.ArcTable.Output.appendRawDataColumn 4           
    ]

let templateTable = 
    TestObjects.Spreadsheet.ArcTable.initWorksheet templateTableName templateTableCols
   
let templateTableMatchingByTableName = TestObjects.Spreadsheet.ArcTable.initWorksheetWithTableName "Other Random Name" "annotationTableUnluckyVampirebat89" templateTableCols

let template = 
    
    let wb = new FsWorkbook()
    wb.AddWorksheet(templateMetadata)
    wb.AddWorksheet(templateTable)
    wb

let template_deprecatedMetadataSheetName = 
    let ws = templateMetadata.Copy()
    ws.Name <- ARCtrl.Template.Spreadsheet.Template.obsoletemetaDataSheetName
    let wb = new FsWorkbook()
    wb.AddWorksheet(ws)
    wb.AddWorksheet(templateTable)
    wb

let template_wrongTemplateTableName = 
    let t = templateTable.Copy()
    t.Name <- "Other Random Name"
    let wb = new FsWorkbook()
    wb.AddWorksheet(templateMetadata)
    wb.AddWorksheet(t)
    wb

let template_matchingXLSXTableName = 

    let wb = new FsWorkbook()
    wb.AddWorksheet(templateMetadata_deprecatedKeys)
    wb.AddWorksheet(templateTableMatchingByTableName)
    wb