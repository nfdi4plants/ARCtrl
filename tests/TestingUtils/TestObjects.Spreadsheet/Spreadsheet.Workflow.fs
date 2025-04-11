module TestObjects.Spreadsheet.Workflow

open FsSpreadsheet

module Proteomics = 

    [<Literal>]
    let workflowIdentifier = "Proteomics"

    let workflowMetadata = 
        let ws = FsWorksheet("isa_workflow")
        let row1 = ws.Row(1)
        row1.[1].Value <- "WORKFLOW"
        let row2 = ws.Row(2)
        row2.[1].Value <- "Workflow Identifier"
        row2.[2].Value <- $"{workflowIdentifier}"
        let row3 = ws.Row(3)
        row3.[1].Value <- "Workflow Title"
        row3.[2].Value <- "Proteomics Workflow"
        let row4 = ws.Row(4)
        row4.[1].Value <- "Workflow Description"
        row4.[2].Value <- "This is a proteomics workflow"
        let row5 = ws.Row(5)
        row5.[1].Value <- "Workflow Type"
        row5.[2].Value <- "Proteomics"
        let row6 = ws.Row(6)
        row6.[1].Value <- "Workflow Type Term Accession Number"
        row6.[2].Value <- "http://purl.obolibrary.org/obo/OMIT_0023015"
        let row7 = ws.Row(7)
        row7.[1].Value <- "Workflow Type Term Source REF"
        row7.[2].Value <- "OMIT"
        let row8 = ws.Row(8)
        row8.[1].Value <- "Workflow Sub Workflow Identifiers"
        row8.[2].Value <- "PeptideSpectrumMatching;Quantification"
        let row9 = ws.Row(9)
        row9.[1].Value <- "Workflow URI"
        row9.[2].Value <- "http://example.com/workflow"
        let row10 = ws.Row(10)
        row10.[1].Value <- "Workflow Version"
        row10.[2].Value <- "0.1.2"
        let row11 = ws.Row(11)
        row11.[1].Value <- "Workflow Parameters Name"
        row11.[2].Value <- "test statistic"
        let row12 = ws.Row(12)
        row12.[1].Value <- "Workflow Parameters Term Accession Number"
        row12.[2].Value <- $"http://purl.obolibrary.org/obo/OBCS_0000013"
        let row13 = ws.Row(13)
        row13.[1].Value <- "Workflow Parameters Term Source REF"
        row13.[2].Value <- "OBCS"
        let row14 = ws.Row(14)
        row14.[1].Value <- "Workflow Components Name"
        row14.[2].Value <- "ProteomIQon"
        let row15 = ws.Row(15)
        row15.[1].Value <- "Workflow Components Type"
        row15.[2].Value <- "software"
        let row16 = ws.Row(16)
        row16.[1].Value <- "Workflow Components Type Term Accession Number"
        row16.[2].Value <- "http://purl.obolibrary.org/obo/IAO_0000010"
        let row17 = ws.Row(17)
        row17.[1].Value <- "Workflow Components Type Term Source REF"
        row17.[2].Value <- "IAO"
        let row18 = ws.Row(18)
        row18.[1].Value <- "Workflow File Name"
        row18.[2].Value <- $"workflows/{workflowIdentifier}/isa.workflow.xlsx"
        let row19 = ws.Row(19)
        row19.[1].Value <- "WORKFLOW CONTACTS"
        let row20 = ws.Row(20)
        row20.[1].Value <- "Workflow Person Last Name"
        row20.[2].Value <- "Oliver"
        row20.[3].Value <- "Juan"
        row20.[4].Value <- "Leo"
        let row21 = ws.Row(21)
        row21.[1].Value <- "Workflow Person First Name"
        row21.[2].Value <- "Stephen"
        row21.[3].Value <- "Castrillo"
        row21.[4].Value <- "Zeef"
        let row22 = ws.Row(22)
        row22.[1].Value <- "Workflow Person Mid Initials"
        row22.[2].Value <- "G"
        row22.[3].Value <- "I"
        row22.[4].Value <- "A"
        let row23 = ws.Row(23)
        row23.[1].Value <- "Workflow Person Email"
        let row24 = ws.Row(24)
        row24.[1].Value <- "Workflow Person Phone"
        let row25 = ws.Row(25)
        row25.[1].Value <- "Workflow Person Fax"
        let row26 = ws.Row(26)
        row26.[1].Value <- "Workflow Person Address"
        row26.[2].Value <- "Oxford Road, Manchester M13 9PT, UK"
        row26.[3].Value <- "Oxford Road, Manchester M13 9PT, UK"
        row26.[4].Value <- "Oxford Road, Manchester M13 9PT, UK"
        let row27 = ws.Row(27)
        row27.[1].Value <- "Workflow Person Affiliation"
        row27.[2].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
        row27.[3].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
        row27.[4].Value <- "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
        let row28 = ws.Row(28)
        row28.[1].Value <- "Workflow Person Roles"
        row28.[2].Value <- "corresponding author"
        row28.[3].Value <- "author"
        row28.[4].Value <- "author"
        let row29 = ws.Row(29)
        row29.[1].Value <- "Workflow Person Roles Term Accession Number"
        let row30 = ws.Row(30)
        row30.[1].Value <- "Workflow Person Roles Term Source REF"
        let row32 = ws.Row(32)
        row32.[1].Value <- "Comment[ORCID]"
        row32.[2].Value <- "0000-0002-1825-0097"
        row32.[4].Value <- "0000-0002-1825-0098"
        ws
         
    let workflowMetadataMetadataCollection =
        let workflow = ARCtrl.Spreadsheet.ArcWorkflow.fromMetadataSheet workflowMetadata
        ARCtrl.Spreadsheet.ArcWorkflow.toMetadataCollection workflow


    let workflowMetadataWorkbook = 
        let wb = new FsWorkbook()
        wb.AddWorksheet(workflowMetadata)
        wb
