module TestObjects.Contract.ISA
    
open FsSpreadsheet
open ARCtrl.Contract
open ARCtrl
open ARCtrl
open ARCtrl.Spreadsheet
open ARCtrl.Helper

open TestObjects.Spreadsheet

module SimpleISA = 
    
    module Assay = 

        let proteomeMetadataWorksheet = Assay.Proteome.assayMetadata
        let proteomeWsName = "Measurement"
        let proteomeTable =
            ArcTable.initWorksheet proteomeWsName
                [
                    ArcTable.Protocol.REF.appendLolColumn 4          
                    ArcTable.Protocol.Type.appendCollectionColumn 2
                    ArcTable.Parameter.appendMixedTemperatureColumn 2 2
                    ArcTable.Parameter.appendInstrumentColumn 2 
                    ArcTable.Characteristic.appendOrganismColumn 3
                    ArcTable.Factor.appendTimeColumn 0
                ]
        let proteomeWB = 
            let wb = new FsWorkbook()
            wb.AddWorksheet(proteomeMetadataWorksheet)
            wb.AddWorksheet(proteomeTable)
            wb
        let proteomeReadContract = 
            Contract.create(
                Operation.READ, 
                path = Identifier.Assay.fileNameFromIdentifier Assay.Proteome.assayIdentifier,
                dtoType = DTOType.ISA_Assay,
                dto = DTO.Spreadsheet proteomeWB)

        let metabolomeMetadataWorksheet = Assay.Metabolome.assayMetadata

        let metabolomeWB = 
            let wb = new FsWorkbook()
            wb.AddWorksheet(metabolomeMetadataWorksheet)
            wb
        let metabolomeReadContract = 
            Contract.create(
                Operation.READ, 
                path = Identifier.Assay.fileNameFromIdentifier Assay.Metabolome.assayIdentifier,
                dtoType = DTOType.ISA_Assay,
                dto = DTO.Spreadsheet metabolomeWB)

        
        let transcriptomeMetadataWorksheet = Assay.Transcriptome.assayMetadata

        let transcriptomeWB = 
            let wb = new FsWorkbook()
            wb.AddWorksheet(transcriptomeMetadataWorksheet)
            wb
        let transcriptomeReadContract = 
            Contract.create(
                Operation.READ, 
                path = Identifier.Assay.fileNameFromIdentifier Assay.Transcriptome.assayIdentifier,
                dtoType = DTOType.ISA_Assay,
                dto = DTO.Spreadsheet transcriptomeWB)

    module Study =
    
    
        let bII_S_1MetadataWorksheet = Study.BII_S_1.studyMetadata
        let bII_S_1NewWsName = "preparation"
        let bII_S_1NewTable =
            ArcTable.initWorksheet bII_S_1NewWsName
                [
                    ArcTable.Protocol.REF.appendLolColumn 4          
                    ArcTable.Protocol.Type.appendCollectionColumn 2
                    ArcTable.Parameter.appendMixedTemperatureColumn 2 2
                    ArcTable.Parameter.appendInstrumentColumn 2 
                    ArcTable.Characteristic.appendOrganismColumn 3
                    ArcTable.Factor.appendTimeColumn 0
                ]
        let bII_S_1ExistingWSName = "growth protocol"
        let bII_S_1ExistingTable =
            ArcTable.initWorksheet bII_S_1ExistingWSName
                [
                    ArcTable.Protocol.REF.appendLolColumn 4          
                    ArcTable.Protocol.Type.appendCollectionColumn 2
                    ArcTable.Parameter.appendMixedTemperatureColumn 2 2
                    ArcTable.Parameter.appendInstrumentColumn 2 
                    ArcTable.Characteristic.appendOrganismColumn 3
                    ArcTable.Factor.appendTimeColumn 0
                ]
        let bII_S_1WB = 
            let wb = new FsWorkbook()
            wb.AddWorksheet(bII_S_1MetadataWorksheet)
            wb.AddWorksheet(bII_S_1NewTable)
            wb.AddWorksheet(bII_S_1ExistingTable)
            wb
        let bII_S_1ReadContract = 
            Contract.create(
                Operation.READ, 
                path = Identifier.Study.fileNameFromIdentifier Study.BII_S_1.studyIdentifier,
                dtoType = DTOType.ISA_Study,
                dto = DTO.Spreadsheet bII_S_1WB)

        let bII_S_2MetadataWorksheet = Study.BII_S_2.studyMetadata

        let bII_S_2WB = 
            let wb = new FsWorkbook()
            wb.AddWorksheet(bII_S_2MetadataWorksheet)
            wb
        let bII_S_2ReadContract = 
            Contract.create(
                Operation.READ, 
                path = Identifier.Study.fileNameFromIdentifier Study.BII_S_2.studyIdentifier,
                dtoType = DTOType.ISA_Study,
                dto = DTO.Spreadsheet bII_S_2WB)

    module Investigation = 

        let investigationReadContract =
            Contract.create(
                Operation.READ,
                path = Path.InvestigationFileName,
                dtoType = DTOType.ISA_Investigation,
                dto = DTO.Spreadsheet Investigation.BII_I_1.fullInvestigation)


module UpdateAssayWithStudyProtocol = 
    
    let assayIdentifier = "MyAssay"
    let studyIdentifier = "MyStudy"
    let investigationIdentifier = "MyInvestigation"

    let protocolName = "MyProtocol"
    let inputHeader = CompositeHeader.Input IOType.Sample
    let inputCell = (CompositeCell.createFreeText "inputValueName")
    let description = "MyDescription"

    let assay = ArcAssay(assayIdentifier)
    let study = ArcStudy(studyIdentifier)
    let investigation = ArcInvestigation(investigationIdentifier)
    investigation.AddRegisteredStudy(study)
    study.AddRegisteredAssay assay

    let t = ArcTable.init("AssayProtocol")
    t.AddProtocolNameColumn(Array.create 2 protocolName)
    t.AddColumn(inputHeader, Array.create 2 inputCell)
    assay.AddTable t

    let refT = ArcTable.init("StudyProtocol")
    refT.AddProtocolNameColumn(Array.create 1 protocolName)
    refT.AddProtocolDescriptionColumn(Array.create 1 description)
    study.AddTable refT

    let assayWB = ArcAssay.toFsWorkbook assay

    let retAssay = ArcAssay.fromFsWorkbook assayWB

    let assayReadContract = 
        Contract.create(
            Operation.READ, 
            path = Identifier.Assay.fileNameFromIdentifier assayIdentifier,
            dtoType = DTOType.ISA_Assay,
            dto = DTO.Spreadsheet assayWB)

    let studyWB = ArcStudy.toFsWorkbook study
    let studyReadContract = 
        Contract.create(
            Operation.READ, 
            path = Identifier.Study.fileNameFromIdentifier studyIdentifier,
            dtoType = DTOType.ISA_Study,
            dto = DTO.Spreadsheet studyWB)

    let investigationWB = ArcInvestigation.toFsWorkbook investigation
    let investigationReadContract =
        Contract.create(
            Operation.READ,
            path = Path.InvestigationFileName,
            dtoType = DTOType.ISA_Investigation,
            dto = DTO.Spreadsheet investigationWB)