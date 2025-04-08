module TestObjects.Spreadsheet.Run

open FsSpreadsheet

module Proteomics = 

    [<Literal>]
    let runIdentifier = "proteomics"

    let runMetadata = 
        let ws = FsWorksheet("isa_run")
        let row1 = ws.Row(1)
        row1.[1].Value <- "RUN"
        let row2 = ws.Row(2)
        row2.[1].Value <- "Run Identifier"
        row2.[2].Value <- $"{runIdentifier}"
        let row3 = ws.Row(3)
        row3.[1].Value <- "Run Title"
        row3.[2].Value <- "Proteome Run"
        let row4 = ws.Row(4)
        row4.[1].Value <- "Run Description"
        row4.[2].Value <- "This is a proteome run"
        let row5 = ws.Row(5)
        row5.[1].Value <- "Run Workflow Identifiers"
        row5.[2].Value <- "Proteomics"
        let row6 = ws.Row(6)
        row6.[1].Value <- "Run Measurement Type"
        row6.[2].Value <- "protein expression profiling"
        let row7 = ws.Row(7)
        row7.[1].Value <- "Run Measurement Type Term Accession Number"
        row7.[2].Value <- "http://purl.obolibrary.org/obo/OBI_0000615"
        let row8 = ws.Row(8)
        row8.[1].Value <- "Run Measurement Type Term Source REF"
        row8.[2].Value <- "OBI"
        let row9 = ws.Row(9)
        row9.[1].Value <- "Run Technology Type"
        row9.[2].Value <- "mass spectrometry"
        let row10 = ws.Row(10)
        row10.[1].Value <- "Run Technology Type Term Accession Number"
        row10.[2].Value <- "http://purl.obolibrary.org/obo/OBI_0000615"
        let row11 = ws.Row(11)
        row11.[1].Value <- "Run Technology Type Term Source REF"
        row11.[2].Value <- "OBI"
        let row12 = ws.Row(12)
        row12.[1].Value <- "Run Technology Platform"
        row12.[2].Value <- "iTRAQ"
        let row13 = ws.Row(13)
        row13.[1].Value <- "Run File Name"
        row13.[2].Value <- $"runs/{runIdentifier}/isa.run.xlsx"
        let row14 = ws.Row(14)
        row14.[1].Value <- "RUN PERFORMERS"
        let row15 = ws.Row(15)
        row15.[1].Value <- "Run Person Last Name"
        row15.[2].Value <- "Weil"
        row15.[3].Value <- "Ott"
        let row16 = ws.Row(16)
        row16.[1].Value <- "Run Person First Name"
        row16.[2].Value <- "Lukas"
        row16.[3].Value <- "Caroline"
        let row17 = ws.Row(17)
        row17.[1].Value <- "Run Person Mid Initials"
        row17.[2].Value <- "H"
        let row18 = ws.Row(18)
        row18.[1].Value <- "Run Person Email"
        let row19 = ws.Row(19)
        row19.[1].Value <- "Run Person Phone"
        let row20 = ws.Row(20)
        row20.[1].Value <- "Run Person Fax"
        let row21 = ws.Row(21)
        row21.[1].Value <- "Run Person Address"
        row21.[2].Value <- "Erwin-Schrödinger-Straße 56, Kaiserslautern, DE"
        row21.[3].Value <- "Erwin-Schrödinger-Straße 56, Kaiserslautern, DE"
        let row22 = ws.Row(22)
        row22.[1].Value <- "Run Person Affiliation"
        row22.[2].Value <- "Department of Biology, University of Kaiserslautern"
        row22.[3].Value <- "Department of Biology, University of Kaiserslautern"
        let row23 = ws.Row(23)
        row23.[1].Value <- "Run Person Roles"
        row23.[2].Value <- "corresponding author"
        row23.[3].Value <- "author"
        let row24 = ws.Row(24)
        row24.[1].Value <- "Run Person Roles Term Accession Number"
        let row25 = ws.Row(25)
        row25.[1].Value <- "Run Person Roles Term Source REF"
        let row26 = ws.Row(26)
        row26.[1].Value <- "Comment[ORCID]"
        row26.[2].Value <- "1234-5678-9012-3456"
        row26.[3].Value <- "9876-5432-1098-7654"
        ws

    let runMetadataCollection =
        let run = ARCtrl.Spreadsheet.ArcRun.fromMetadataSheet runMetadata
        ARCtrl.Spreadsheet.ArcRun.toMetadataCollection run

    let runMetadataEmpty = 
        let ws = FsWorksheet("isa_run")
        let row1 = ws.Row(1)
        row1.[1].Value <- "RUN"
        let row2 = ws.Row(2)
        row2.[1].Value <- "Run Identifier"
        let row3 = ws.Row(3)
        row3.[1].Value <- "Run Title"
        let row4 = ws.Row(4)
        row4.[1].Value <- "Run Description"
        let row5 = ws.Row(5)
        row5.[1].Value <- "Run Measurement Type"
        let row6 = ws.Row(6)
        row6.[1].Value <- "Run Measurement Type Term Accession Number"
        let row7 = ws.Row(7)
        row7.[1].Value <- "Run Measurement Type Term Source REF"
        let row8 = ws.Row(8)
        row8.[1].Value <- "Run Technology Type"
        let row9 = ws.Row(9)
        row9.[1].Value <- "Run Technology Type Term Accession Number"
        let row10 = ws.Row(10)
        row10.[1].Value <- "Run Technology Type Term Source REF"
        let row11 = ws.Row(11)
        row11.[1].Value <- "Run Technology Platform"
        let row12 = ws.Row(12)
        row12.[1].Value <- "Run File Name"
        let row13 = ws.Row(13)
        row13.[1].Value <- "RUN PERFORMERS"    
        let row14 = ws.Row(14)
        row14.[1].Value <- "Run Person Last Name"
        let row15 = ws.Row(15)
        row15.[1].Value <- "Run Person First Name"
        let row16 = ws.Row(16)
        row16.[1].Value <- "Run Person Mid Initials"
        let row17 = ws.Row(17)
        row17.[1].Value <- "Run Person Email"
        let row18 = ws.Row(18)
        row18.[1].Value <- "Run Person Phone"
        let row19 = ws.Row(19)
        row19.[1].Value <- "Run Person Fax"
        let row20 = ws.Row(20)
        row20.[1].Value <- "Run Person Address"
        let row21 = ws.Row(21)
        row21.[1].Value <- "Run Person Affiliation"
        let row22 = ws.Row(22)
        row22.[1].Value <- "Run Person Roles"
        let row23 = ws.Row(23)
        row23.[1].Value <- "Run Person Roles Term Accession Number"
        let row24 = ws.Row(24)
        row24.[1].Value <- "Run Person Roles Term Source REF"
        let row25 = ws.Row(25)
        row25.[1].Value <- "Comment[Worksheet]"
        ws

    let runMetadataWorkbook = 
        let wb = new FsWorkbook()
        wb.AddWorksheet(runMetadata)
        wb

    let runMetadataWorkbookEmpty = 
        let wb = new FsWorkbook()
        wb.AddWorksheet(runMetadataEmpty)
        wb
