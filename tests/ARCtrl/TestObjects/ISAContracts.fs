module TestObjects.ISAContracts
    
open FsSpreadsheet
open ARCtrl.Contract
open ARCtrl
open ARCtrl.ISA
open ARCtrl.ISA.Spreadsheet

module SimpleISA = 
    let assayMetadataWorksheet = TestObjects.Assay.assayMetadata
    let assayWsName = "Measurement"
    let assayTable =
        ArcTable.initWorksheet assayWsName
            [
                ArcTable.Protocol.REF.appendLolColumn 4          
                ArcTable.Protocol.Type.appendCollectionColumn 2
                ArcTable.Parameter.appendMixedTemperatureColumn 2 2
                ArcTable.Parameter.appendInstrumentColumn 2 
                ArcTable.Characteristic.appendOrganismColumn 3
                ArcTable.Factor.appendTimeColumn 0
            ]
    let assayWB = 
        let wb = new FsWorkbook()
        wb.AddWorksheet(assayMetadataWorksheet)
        wb.AddWorksheet(assayTable)
        wb
    let assayReadContract = 
        Contract.create(
            Operation.READ, 
            path = Identifier.Assay.fileNameFromIdentifier Assay.assayIdentifier,
            dtoType = DTOType.ISA_Assay,
            dto = DTO.Spreadsheet assayWB)


    let studyMetadataWorksheet = TestObjects.Study.studyMetadata
    let studyNewWsName = "preparation"
    let studyNewTable =
        ArcTable.initWorksheet studyNewWsName
            [
                ArcTable.Protocol.REF.appendLolColumn 4          
                ArcTable.Protocol.Type.appendCollectionColumn 2
                ArcTable.Parameter.appendMixedTemperatureColumn 2 2
                ArcTable.Parameter.appendInstrumentColumn 2 
                ArcTable.Characteristic.appendOrganismColumn 3
                ArcTable.Factor.appendTimeColumn 0
            ]
    let studyExistingWSName = "growth protocol"
    let studyExistingTable =
        ArcTable.initWorksheet studyExistingWSName
            [
                ArcTable.Protocol.REF.appendLolColumn 4          
                ArcTable.Protocol.Type.appendCollectionColumn 2
                ArcTable.Parameter.appendMixedTemperatureColumn 2 2
                ArcTable.Parameter.appendInstrumentColumn 2 
                ArcTable.Characteristic.appendOrganismColumn 3
                ArcTable.Factor.appendTimeColumn 0
            ]
    let studyWB = 
        let wb = new FsWorkbook()
        wb.AddWorksheet(studyMetadataWorksheet)
        wb.AddWorksheet(studyNewTable)
        wb.AddWorksheet(studyExistingTable)
        wb
    let studyReadContract = 
        Contract.create(
            Operation.READ, 
            path = Identifier.Study.fileNameFromIdentifier Study.studyIdentifier,
            dtoType = DTOType.ISA_Study,
            dto = DTO.Spreadsheet studyWB)


    let investigationReadContract =
        Contract.create(
            Operation.READ,
            path = Path.InvestigationFileName,
            dtoType = DTOType.ISA_Investigation,
            dto = DTO.Spreadsheet Investigation.fullInvestigation)


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