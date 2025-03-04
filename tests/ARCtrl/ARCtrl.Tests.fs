module ARCtrl.Tests


open ARCtrl
open TestObjects.Contract.ISA
open TestObjects.Spreadsheet
open TestingUtils
open ARCtrl.Contract
open ARCtrl.Spreadsheet
open ARCtrl.Helper
open FsSpreadsheet
open CrossAsync

let tests_create = testList "create" [
    testCase "empty" <| fun _ ->
        let arc = ARC()
        Expect.isNone arc.CWL "cwl"
        Expect.isNone arc.ISA "isa"
] 

let private tests_fromFilePaths = testList "fromFilePaths" [

    testCase "simple" <| fun _ ->
        let input = 
            [|@"isa.investigation.xlsx"; @".arc\.gitkeep"; @".git\config";
            @".git\description"; @".git\HEAD"; @"assays\.gitkeep"; @"runs\.gitkeep";
            @"studies\.gitkeep"; @"workflows\.gitkeep";
            @".git\hooks\applypatch-msg.sample"; @".git\hooks\commit-msg.sample";
            @".git\hooks\fsmonitor-watchman.sample"; @".git\hooks\post-update.sample";
            @".git\hooks\pre-applypatch.sample"; @".git\hooks\pre-commit.sample";
            @".git\hooks\pre-merge-commit.sample"; @".git\hooks\pre-push.sample";
            @".git\hooks\pre-rebase.sample"; @".git\hooks\pre-receive.sample";
            @".git\hooks\prepare-commit-msg.sample";
            @".git\hooks\push-to-checkout.sample"; @".git\hooks\update.sample";
            @".git\info\exclude"; @"assays\est\isa.assay.xlsx"; @"assays\est\README.md";
            @"assays\TestAssay1\isa.assay.xlsx"; @"assays\TestAssay1\README.md";
            @"studies\est\isa.study.xlsx"; @"studies\est\README.md";
            @"studies\MyStudy\isa.study.xlsx"; @"studies\MyStudy\README.md";
            @"studies\TestAssay1\isa.study.xlsx"; @"studies\TestAssay1\README.md";
            @"assays\est\dataset\.gitkeep"; @"assays\est\protocols\.gitkeep";
            @"assays\TestAssay1\dataset\.gitkeep";
            @"assays\TestAssay1\protocols\.gitkeep"; @"studies\est\protocols\.gitkeep";
            @"studies\est\resources\.gitkeep"; @"studies\MyStudy\protocols\.gitkeep";
            @"studies\MyStudy\resources\.gitkeep";
            @"studies\TestAssay1\protocols\.gitkeep";
            @"studies\TestAssay1\resources\.gitkeep"
            |]
            |> Array.map (fun x -> x.Replace(@"\","/"))
            |> Array.sort
        let arc = ARC.fromFilePaths(input)
        Expect.isNone arc.CWL "cwl"
        Expect.isNone arc.ISA "isa"
        let actualFilePaths = arc.FileSystem.Tree.ToFilePaths() |> Array.sort
        Expect.equal actualFilePaths input "isSome fs"
    testCase "correctContractsFor(.)AtBeginning" <| fun _ ->
        let input =  [|
            @"./isa.investigation.xlsx"
            @"./assays/TestAssay1/isa.assay.xlsx"
            @"./studies/TestAssay1/isa.study.xlsx";|]
        let arc = ARC.fromFilePaths(input)
        let contracts = arc.GetReadContracts()
        Expect.hasLength contracts 3 "should have read 3 contracts"
]

let private simpleISAContracts = 
    [|
        SimpleISA.Investigation.investigationReadContract
        SimpleISA.Study.bII_S_1ReadContract
        SimpleISA.Study.bII_S_2ReadContract
        SimpleISA.Assay.proteomeReadContract
        SimpleISA.Assay.metabolomeReadContract
        SimpleISA.Assay.transcriptomeReadContract
    |]

let private tests_read_contracts = testList "read_contracts" [
    testCase "simpleISA" (fun () -> 
        let arc = ARC()
        arc.SetISAFromContracts simpleISAContracts
        Expect.isSome arc.ISA "isa should be filled out"
        let inv = arc.ISA.Value
        Expect.equal inv.Identifier Investigation.BII_I_1.investigationIdentifier "investigation identifier should have been read from investigation contract"

        Expect.equal inv.Studies.Count 2 "should have read two studies"
        let study1 = inv.Studies.[0]
        Expect.equal study1.Identifier Study.BII_S_1.studyIdentifier "study 1 identifier should have been read from study contract"

        Expect.equal study1.TableCount 2 "study 1 should have the 2 tables. Top level Metadata tables are ignored."
        //Expect.equal study1.TableCount 8 "study 1 should have the 7 tables from investigation plus one extra. One table should be overwritten."
        
        Expect.equal study1.RegisteredAssays.Count 3 "study 1 should have read three assays"
        let assay1 = study1.RegisteredAssays.[0]
        Expect.equal assay1.Identifier Assay.Proteome.assayIdentifier "assay 1 identifier should have been read from assay contract"
        Expect.equal assay1.TableCount 1 "assay 1 should have read one table"
    
    )
    testCase "GetStudyRemoveContractsOnlyRegistered" (fun () -> // set to pending, until performance issues in Study.fromFsWorkbook is resolved.
        let arc = ARC()
        arc.SetISAFromContracts([|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
        |])
        Expect.isSome arc.ISA "isa should be filled out"
        let inv = arc.ISA.Value
        Expect.equal inv.Identifier Investigation.BII_I_1.investigationIdentifier "investigation identifier should have been read from investigation contract"

        Expect.equal inv.Studies.Count 1 "should have read one study"
        Expect.equal inv.RegisteredStudyIdentifierCount 2 "should have read two registered study identifiers"
        Expect.equal inv.VacantStudyIdentifiers.Count 1 "should have one vacant study identifier"
        let study1 = inv.Studies.[0]
        Expect.equal study1.Identifier Study.BII_S_1.studyIdentifier "study 1 identifier should have been read from study contract"
        Expect.equal study1.TableCount 2 "study 1 should have the 2 tables. Top level Metadata tables are ignored."
        //Expect.equal study1.TableCount 8 "study 1 should have the 7 tables from investigation plus one extra. One table should be overwritten."
        
        Expect.equal study1.RegisteredAssays.Count 1 "study 1 should have read one assay"
        Expect.equal study1.RegisteredAssayIdentifierCount 3 "study 1 should have read three registered assay identifiers"
        Expect.equal study1.VacantAssayIdentifiers.Count 2 "study 1 should have two vacant assay identifiers"
        let assay1 = study1.RegisteredAssays.[0]
        Expect.equal assay1.Identifier Assay.Proteome.assayIdentifier "assay 1 identifier should have been read from assay contract"
        Expect.equal assay1.TableCount 1 "assay 1 should have read one table"
    
    )
    // Assay Table protocol get's updated by protocol metadata stored in study
    testCase "assayTableGetsUpdated" (fun () ->
        let iContract = UpdateAssayWithStudyProtocol.investigationReadContract
        let sContract = UpdateAssayWithStudyProtocol.studyReadContract
        let aContract = UpdateAssayWithStudyProtocol.assayReadContract
        let arc = ARC()
        arc.SetISAFromContracts([|iContract; sContract; aContract|])
        Expect.isSome arc.ISA "isa should be filled out"
        let inv = arc.ISA.Value
        Expect.equal inv.Identifier UpdateAssayWithStudyProtocol.investigationIdentifier "investigation identifier should have been read from investigation contract"

        Expect.equal inv.Studies.Count 1 "should have read one study"
        let study = inv.Studies.[0]

        Expect.equal study.TableCount 1 "study should have read one table"
        let studyTable = study.Tables.[0]
        Expect.equal studyTable.ColumnCount 2 "study column number should be unchanged"
        Expect.sequenceEqual
            (studyTable.GetProtocolDescriptionColumn()).Cells
            [CompositeCell.createFreeText UpdateAssayWithStudyProtocol.description]
            "Description value was not kept correctly"
        Expect.sequenceEqual
            (studyTable.GetProtocolNameColumn()).Cells
            [CompositeCell.createFreeText UpdateAssayWithStudyProtocol.protocolName]
            "Protocol ref value was not kept correctly"

        Expect.equal study.RegisteredAssays.Count 1 "study should have read one assay"
        let assay = study.RegisteredAssays.[0]
        Expect.equal assay.TableCount 1 "assay should have read one table"
        let assayTable = assay.Tables.[0]
        Expect.equal assayTable.ColumnCount 3 "assay column number should be updated"
        Expect.sequenceEqual
            (assayTable.GetProtocolNameColumn()).Cells
            (Array.create 2 (CompositeCell.createFreeText UpdateAssayWithStudyProtocol.protocolName))
            "Protocol ref value was not kept correctly"
        Expect.sequenceEqual
            (assayTable.GetColumnByHeader(UpdateAssayWithStudyProtocol.inputHeader)).Cells
            (Array.create 2 UpdateAssayWithStudyProtocol.inputCell)
            "Protocol ref value was not kept correctly"
        Expect.sequenceEqual
            (assayTable.GetProtocolDescriptionColumn()).Cells
            (Array.create 2 (CompositeCell.createFreeText UpdateAssayWithStudyProtocol.description))
            "Description value was not taken correctly"
    )
    testCase "SimpleISA_WithDataset" (fun _ ->
        let contracts = Array.append simpleISAContracts [|SimpleISA.Assay.proteomeDatamapContract|]

        let arc = ARC()
        arc.SetISAFromContracts contracts

        let inv = Expect.wantSome arc.ISA "Arc should have investigation"
        let a1 = inv.GetAssay(SimpleISA.Assay.proteomeIdentifer)
        let datamap = Expect.wantSome a1.DataMap "Proteome Assay was supposed to have datamap"
        
        Expect.equal 2 datamap.DataContexts.Count "Datamap was not read correctly"

        let a2 = inv.GetAssay(SimpleISA.Assay.metabolomeIdentifer)
        Expect.isNone a2.DataMap "Metabolome Assay was not supposed to have datamap"
    
    )

]

let private tests_writeContracts = testList "write_contracts" [
    testCase "empty" (fun _ ->
        let arc = ARC()
        let contracts = arc.GetWriteContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 5 $"Should contain exactly as much contracts as base folders but contained: {contractPathsString}" 
        Expect.exists contracts (fun c -> c.Path = "workflows/.gitkeep") "Contract for workflows folder missing"
        Expect.exists contracts (fun c -> c.Path = "runs/.gitkeep") "Contract for runs folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/.gitkeep") "Contract for assays folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/.gitkeep") "Contract for studies folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx") "Contract for investigation folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Investigation) "Contract for investigation exisiting but has wrong DTO type"
    )
    testCase "simpleISA" (fun _ ->
        let inv = ArcInvestigation("MyInvestigation", "BestTitle")
        inv.InitStudy("MyStudy").InitRegisteredAssay("MyAssay") |> ignore
        let arc = ARC(isa = inv)
        let contracts = arc.GetWriteContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 13 $"Should contain more contracts as base folders but contained: {contractPathsString}"

        // Base 
        Expect.exists contracts (fun c -> c.Path = "workflows/.gitkeep") "Contract for workflows folder missing"
        Expect.exists contracts (fun c -> c.Path = "runs/.gitkeep") "Contract for runs folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/.gitkeep") "Contract for assays folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/.gitkeep") "Contract for studies folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx") "Contract for investigation folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Investigation) "Contract for investigation exisiting but has wrong DTO type"

        // Study folder
        Expect.exists contracts (fun c -> c.Path = "studies/MyStudy/README.md") "study readme missing"
        Expect.exists contracts (fun c -> c.Path = "studies/MyStudy/protocols/.gitkeep") "study protocols folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/MyStudy/resources/.gitkeep") "study resources folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/MyStudy/isa.study.xlsx") "study file missing"
        Expect.exists contracts (fun c -> c.Path = "studies/MyStudy/isa.study.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Study) "study file exisiting but has wrong DTO type"

        // Assay folder
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/README.md") "assay readme missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/protocols/.gitkeep") "assay protocols folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/dataset/.gitkeep") "assay dataset folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/isa.assay.xlsx") "assay file missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/isa.assay.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Assay) "assay file exisiting but has wrong DTO type"

    )
    testCase "assayWithDatamap" (fun _ ->
        let inv = ArcInvestigation("MyInvestigation", "BestTitle")
        let a = inv.InitAssay("MyAssay")
        let dm = DataMap.init()
        a.DataMap <- Some dm
        let arc = ARC(isa = inv)
        let contracts = arc.GetWriteContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 10 $"Should contain more contracts as base folders but contained: {contractPathsString}"

        // Base 
        Expect.exists contracts (fun c -> c.Path = "workflows/.gitkeep") "Contract for workflows folder missing"
        Expect.exists contracts (fun c -> c.Path = "runs/.gitkeep") "Contract for runs folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/.gitkeep") "Contract for assays folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/.gitkeep") "Contract for studies folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx") "Contract for investigation folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Investigation) "Contract for investigation existing but has wrong DTO type"

        // Assay folder
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/README.md") "assay readme missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/protocols/.gitkeep") "assay protocols folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/dataset/.gitkeep") "assay dataset folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/isa.assay.xlsx") "assay file missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/isa.assay.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Assay) "assay file existing but has wrong DTO type"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/isa.datamap.xlsx") "assay datamap file missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/isa.datamap.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Datamap) "assay datamap file existing but has wrong DTO type"
    )
    testCase "sameAssayAndStudyName" (fun _ ->
        let inv = ArcInvestigation("MyInvestigation", "BestTitle")
        inv.InitStudy("MyAssay").InitRegisteredAssay("MyAssay") |> ignore
        let arc = ARC(isa = inv)
        let contracts = arc.GetWriteContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 13 $"Should contain more contracts as base folders but contained: {contractPathsString}"

        // Base 
        Expect.exists contracts (fun c -> c.Path = "workflows/.gitkeep") "Contract for workflows folder missing"
        Expect.exists contracts (fun c -> c.Path = "runs/.gitkeep") "Contract for runs folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/.gitkeep") "Contract for assays folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/.gitkeep") "Contract for studies folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx") "Contract for investigation folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Investigation) "Contract for investigation exisiting but has wrong DTO type"

        // Study folder
        Expect.exists contracts (fun c -> c.Path = "studies/MyAssay/README.md") "study readme missing"
        Expect.exists contracts (fun c -> c.Path = "studies/MyAssay/protocols/.gitkeep") "study protocols folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/MyAssay/resources/.gitkeep") "study resources folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/MyAssay/isa.study.xlsx") "study file missing"
        Expect.exists contracts (fun c -> c.Path = "studies/MyAssay/isa.study.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Study) "study file exisiting but has wrong DTO type"

        // Assay folder
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/README.md") "assay readme missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/protocols/.gitkeep") "assay protocols folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/dataset/.gitkeep") "assay dataset folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/isa.assay.xlsx") "assay file missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/isa.assay.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Assay) "assay file exisiting but has wrong DTO type"

        // Assay file and Study file Contract should be distinct
        let assayDTOType,assayDTO = contracts |> Array.pick (fun c -> if c.Path = "assays/MyAssay/isa.assay.xlsx" then Some (c.DTOType.Value,c.DTO.Value) else None)
        let studyDTOType,studyDTO = contracts |> Array.pick (fun c -> if c.Path = "studies/MyAssay/isa.study.xlsx" then Some (c.DTOType.Value,c.DTO.Value) else None)
        Expect.equal assayDTOType Contract.DTOType.ISA_Assay "DTOType of assay file should be assay file"
        Expect.equal studyDTOType Contract.DTOType.ISA_Study "DTOType of study file should be study file"
        Expect.equal assayDTO assayDTO "Check that same object should equal to itself"
        Expect.notEqual assayDTO studyDTO "assay and study DTO should differ"   
    )
    testCase "sameAssayInDifferentStudies" (fun _ ->
        let inv = ArcInvestigation("MyInvestigation", "BestTitle")
        let assay = ArcAssay("MyAssay")
        inv.InitStudy("Study1").AddRegisteredAssay(assay) |> ignore
        inv.InitStudy("Study2").RegisterAssay(assay.Identifier) |> ignore
        let arc = ARC(isa = inv)
        let contracts = arc.GetWriteContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 17 $"Should contain more contracts as base folders but contained: {contractPathsString}"

        // Base 
        Expect.exists contracts (fun c -> c.Path = "workflows/.gitkeep") "Contract for workflows folder missing"
        Expect.exists contracts (fun c -> c.Path = "runs/.gitkeep") "Contract for runs folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/.gitkeep") "Contract for assays folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/.gitkeep") "Contract for studies folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx") "Contract for investigation folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Investigation) "Contract for investigation exisiting but has wrong DTO type"

        // Study folder
        Expect.exists contracts (fun c -> c.Path = "studies/Study1/README.md") "study readme missing"
        Expect.exists contracts (fun c -> c.Path = "studies/Study1/protocols/.gitkeep") "study protocols folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/Study1/resources/.gitkeep") "study resources folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/Study1/isa.study.xlsx") "study file missing"
        Expect.exists contracts (fun c -> c.Path = "studies/Study1/isa.study.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Study) "study file exisiting but has wrong DTO type"

        // Study folder
        Expect.exists contracts (fun c -> c.Path = "studies/Study2/README.md") "study readme missing"
        Expect.exists contracts (fun c -> c.Path = "studies/Study2/protocols/.gitkeep") "study protocols folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/Study2/resources/.gitkeep") "study resources folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/Study2/isa.study.xlsx") "study file missing"
        Expect.exists contracts (fun c -> c.Path = "studies/Study2/isa.study.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Study) "study file exisiting but has wrong DTO type"

        // Assay folder
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/README.md") "assay readme missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/protocols/.gitkeep") "assay protocols folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/dataset/.gitkeep") "assay dataset folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/isa.assay.xlsx") "assay file missing"
        Expect.exists contracts (fun c -> c.Path = "assays/MyAssay/isa.assay.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Assay) "assay file exisiting but has wrong DTO type"
    )

]

let private tests_updateContracts = testList "update_contracts" [
    testCase "empty" (fun _ ->
        let arc = ARC()
        // As the ARC is compeletely empty, GetUpdateContracts should return the same contracts as GetWriteContracts
        let contracts = arc.GetUpdateContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 5 $"Should contain exactly as much contracts as base folders but contained: {contractPathsString}" 
        Expect.exists contracts (fun c -> c.Path = "workflows/.gitkeep") "Contract for workflows folder missing"
        Expect.exists contracts (fun c -> c.Path = "runs/.gitkeep") "Contract for runs folder missing"
        Expect.exists contracts (fun c -> c.Path = "assays/.gitkeep") "Contract for assays folder missing"
        Expect.exists contracts (fun c -> c.Path = "studies/.gitkeep") "Contract for studies folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx") "Contract for investigation folder missing"
        Expect.exists contracts (fun c -> c.Path = "isa.investigation.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Investigation) "Contract for investigation exisiting but has wrong DTO type"
    )
    testCase "empty_addAssay" (fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        let arc = ARC(isa = i)
        arc.GetWriteContracts() |> ignore
        i.InitAssay("MyAssay") |> ignore
        // As the ARC is compeletely empty, GetUpdateContracts should return the same contracts as GetWriteContracts
        let contracts = arc.GetUpdateContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 4 $"Should contain exactly as much contracts as needed for new assay: {contractPathsString}"
        // Assay file contract
        let assayFilePath = "assays/MyAssay/isa.assay.xlsx"
        let assayFileContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = assayFilePath)) $"Assay file contract for {assayFilePath} missing"
        Expect.equal assayFileContract.Operation Operation.CREATE "Operation for assay file contract should be CREATE"
        let assayFileDTOType = Expect.wantSome assayFileContract.DTOType "DTOType for assay file contract missing"
        Expect.equal assayFileDTOType DTOType.ISA_Assay "DTOType for assay file contract should be ISA_Assay"
        let assayFileDTO = Expect.wantSome assayFileContract.DTO "DTO for assay file contract missing"
        let wb = assayFileDTO.AsSpreadsheet() :?> FsWorkbook
        let assay = ArcAssay.fromFsWorkbook wb
        Expect.equal assay.Identifier "MyAssay" "Assay identifier should be set"
        // readme contract
        let readmeFilePath = "assays/MyAssay/README.md"
        let readmeContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = readmeFilePath)) $"Readme contract for {readmeFilePath} missing"
        Expect.equal readmeContract.Operation Operation.CREATE "Operation for readme contract should be CREATE"
        let readmeDTOType = Expect.wantSome readmeContract.DTOType "DTOType for readme contract missing"
        Expect.equal readmeDTOType DTOType.PlainText "DTOType for readme contract should be ISA_Readme"
        Expect.isNone readmeContract.DTO "DTO for readme contract should be None"
        // protocols gitkeep contract
        let protocolsGitkeepFilePath = "assays/MyAssay/protocols/.gitkeep"
        let protocolsGitkeepContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = protocolsGitkeepFilePath)) $"Protocols gitkeep contract for {protocolsGitkeepFilePath} missing"
        Expect.equal protocolsGitkeepContract.Operation Operation.CREATE "Operation for protocols gitkeep contract should be CREATE"
        let protocolsGitkeepDTOType = Expect.wantSome protocolsGitkeepContract.DTOType "DTOType for protocols gitkeep contract missing"
        Expect.equal protocolsGitkeepDTOType DTOType.PlainText "DTOType for protocols gitkeep contract should be GitKeep"
        Expect.isNone protocolsGitkeepContract.DTO "DTO for protocols gitkeep contract should be None"
        // dataset gitkeep contract
        let datasetGitkeepFilePath = "assays/MyAssay/dataset/.gitkeep"
        let datasetGitkeepContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = datasetGitkeepFilePath)) $"Dataset gitkeep contract for {datasetGitkeepFilePath} missing"
        Expect.equal datasetGitkeepContract.Operation Operation.CREATE "Operation for dataset gitkeep contract should be CREATE"
        let datasetGitkeepDTOType = Expect.wantSome datasetGitkeepContract.DTOType "DTOType for dataset gitkeep contract missing"
        Expect.equal datasetGitkeepDTOType DTOType.PlainText "DTOType for dataset gitkeep contract should be GitKeep"
        Expect.isNone datasetGitkeepContract.DTO "DTO for dataset gitkeep contract should be None"
    )
    testCase "empty_addStudy" (fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        let arc = ARC(isa = i)
        arc.GetWriteContracts() |> ignore
        i.InitStudy("MyStudy") |> ignore
        // As the ARC is compeletely empty, GetUpdateContracts should return the same contracts as GetWriteContracts
        let contracts = arc.GetUpdateContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 4 $"Should contain exactly as much contracts as needed for new study: {contractPathsString}"
        // Study file contract
        let studyFilePath = "studies/MyStudy/isa.study.xlsx"
        let studyFileContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = studyFilePath)) $"Study file contract for {studyFilePath} missing"
        Expect.equal studyFileContract.Operation Operation.CREATE "Operation for study file contract should be CREATE"
        let studyFileDTOType = Expect.wantSome studyFileContract.DTOType "DTOType for study file contract missing"
        Expect.equal studyFileDTOType DTOType.ISA_Study "DTOType for study file contract should be ISA_Study"
        let studyFileDTO = Expect.wantSome studyFileContract.DTO "DTO for study file contract missing"
        let wb = studyFileDTO.AsSpreadsheet() :?> FsWorkbook
        let study,_ = ArcStudy.fromFsWorkbook wb
        Expect.equal study.Identifier "MyStudy" "Study identifier should be set"
        // readme contract
        let readmeFilePath = "studies/MyStudy/README.md"
        let readmeContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = readmeFilePath)) $"Readme contract for {readmeFilePath} missing"
        Expect.equal readmeContract.Operation Operation.CREATE "Operation for readme contract should be CREATE"
        let readmeDTOType = Expect.wantSome readmeContract.DTOType "DTOType for readme contract missing"
        Expect.equal readmeDTOType DTOType.PlainText "DTOType for readme contract should be ISA_Readme"
        Expect.isNone readmeContract.DTO "DTO for readme contract should be None"
        // protocols gitkeep contract
        let protocolsGitkeepFilePath = "studies/MyStudy/protocols/.gitkeep"
        let protocolsGitkeepContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = protocolsGitkeepFilePath)) $"Protocols gitkeep contract for {protocolsGitkeepFilePath} missing"
        Expect.equal protocolsGitkeepContract.Operation Operation.CREATE "Operation for protocols gitkeep contract should be CREATE"
        let protocolsGitkeepDTOType = Expect.wantSome protocolsGitkeepContract.DTOType "DTOType for protocols gitkeep contract missing"
        Expect.equal protocolsGitkeepDTOType DTOType.PlainText "DTOType for protocols gitkeep contract should be GitKeep"
        Expect.isNone protocolsGitkeepContract.DTO "DTO for protocols gitkeep contract should be None"
        // resources gitkeep contract
        let resourcesGitkeepFilePath = "studies/MyStudy/resources/.gitkeep"
        let resourcesGitkeepContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = resourcesGitkeepFilePath)) $"Resources gitkeep contract for {resourcesGitkeepFilePath} missing"
        Expect.equal resourcesGitkeepContract.Operation Operation.CREATE "Operation for resources gitkeep contract should be CREATE"
        let resourcesGitkeepDTOType = Expect.wantSome resourcesGitkeepContract.DTOType "DTOType for resources gitkeep contract missing"
        Expect.equal resourcesGitkeepDTOType DTOType.PlainText "DTOType for resources gitkeep contract should be GitKeep"
        Expect.isNone resourcesGitkeepContract.DTO "DTO for resources gitkeep contract should be None"
    )
    testCase "init_simpleISA" (fun _ ->
        let inv = ArcInvestigation("MyInvestigation", "BestTitle")
        inv.InitStudy("MyStudy").InitRegisteredAssay("MyAssay") |> ignore
        let arc = ARC(isa = inv)
        let contracts = arc.GetUpdateContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 13 $"Should contain more contracts as base folders but contained: {contractPathsString}"
    )
    testCase "simpleISA_NoChanges" (fun _ ->
        let arc = ARC()
        arc.SetISAFromContracts simpleISAContracts
        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 0 "Should contain no contracts as there are no changes"
    )
    testCase "simpleISA_AssayChange" (fun _ ->
        let arc = ARC()
        arc.SetISAFromContracts simpleISAContracts
        let isa = arc.ISA.Value
        let testAssayIdentifier = TestObjects.Spreadsheet.Assay.Metabolome.assayIdentifier
        isa.GetAssay(testAssayIdentifier).InitTable("MyNewTestTable") |> ignore
        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 1 $"Should contain only assay change contract"
        let expectedPath = Identifier.Assay.fileNameFromIdentifier testAssayIdentifier
        Expect.equal contracts.[0].Path expectedPath "Should be the assay file"
        let nextContracts = arc.GetUpdateContracts()
        Expect.equal nextContracts.Length 0 "Should contain no contracts as there are no changes"
    )
    testCase "simpleISA_Datamap_NoChanges" (fun _ ->
        let arc = ARC()
        let readContracts = Array.append simpleISAContracts [|SimpleISA.Assay.proteomeDatamapContract|]
        arc.SetISAFromContracts readContracts
        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 0 "Should contain no contracts as there are no changes"
    )
    testCase "simpleISA_Datamap_Changed" (fun _ ->
        let arc = ARC()
        let readContracts = Array.append simpleISAContracts [|SimpleISA.Assay.proteomeDatamapContract|]
        arc.SetISAFromContracts readContracts
        let isa = arc.ISA.Value

        let dm = Expect.wantSome (isa.GetAssay(SimpleISA.Assay.proteomeIdentifer).DataMap) "Assay should have datamap"       
        dm.GetDataContext(1).Name <- Some "Hello"

        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 1 $"Should contain only assay datamap change contract"
        let expectedPath = Identifier.Assay.datamapFileNameFromIdentifier SimpleISA.Assay.proteomeIdentifer
        Expect.equal contracts.[0].Path expectedPath "Should be the assay datamap file"
        let nextContracts = arc.GetUpdateContracts()
        Expect.equal nextContracts.Length 0 "Should contain no contracts as there are no changes"
    )
    testCase "simpleISA_StudyChange" (fun _ ->
        let arc = ARC()
        arc.SetISAFromContracts simpleISAContracts
        let isa = arc.ISA.Value
        let testStudyIdentifier = TestObjects.Spreadsheet.Study.BII_S_1.studyIdentifier
        isa.GetStudy(testStudyIdentifier).InitTable("MyNewTestTable") |> ignore
        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 1 $"Should contain only study change contract"
        let expectedPath = Identifier.Study.fileNameFromIdentifier testStudyIdentifier
        Expect.equal contracts.[0].Path expectedPath "Should be the study file"
        let nextContracts = arc.GetUpdateContracts()
        Expect.equal nextContracts.Length 0 "Should contain no contracts as there are no changes"      
    )
    testCase "simpleISA_InvestigationChange" (fun _ ->
        let arc = ARC()
        arc.SetISAFromContracts simpleISAContracts
        let isa = arc.ISA.Value
        isa.Title <- Some "NewTitle"
        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 1 $"Should contain only investigation change contract"
        let expectedPath = "isa.investigation.xlsx"
        Expect.equal contracts.[0].Path expectedPath "Should be the investigation file"
        let nextContracts = arc.GetUpdateContracts()
        Expect.equal nextContracts.Length 0 "Should contain no contracts as there are no changes"   
    )
]


let private tests_updateFileSystem = testList "update_Filesystem" [
    testCase "empty noChanges" (fun () ->
        let arc = ARC()
        let oldFS = arc.FileSystem.Copy()
        arc.UpdateFileSystem()
        let newFS = arc.FileSystem
        Expect.equal oldFS.Tree newFS.Tree "Tree should be equal"
    )
    testCase "empty addInvestigationWithStudy" (fun () ->
        let arc = ARC()
        let oldFS = arc.FileSystem.Copy()
        let study = ArcStudy("MyStudy")
        let inv = ArcInvestigation("MyInvestigation")
        inv.AddStudy(study)
        arc.ISA <- Some (inv)
        arc.UpdateFileSystem()
        let newFS = arc.FileSystem
        Expect.notEqual oldFS.Tree newFS.Tree "Tree should be unequal"
    )
    testCase "simple noChanges" (fun () ->
        let study = ArcStudy("MyStudy")
        let inv = ArcInvestigation("MyInvestigation")
        inv.AddStudy(study)
        let arc = ARC(isa = inv)
        let oldFS = arc.FileSystem.Copy()   
        arc.UpdateFileSystem()
        let newFS = arc.FileSystem
        Expect.equal oldFS.Tree newFS.Tree "Tree should be equal"
    )
    testCase "simple addAssayToStudy" (fun () ->
        let study = ArcStudy("MyStudy")
        let inv = ArcInvestigation("MyInvestigation")
        inv.AddStudy(study)
        let arc = ARC(isa = inv)
        let oldFS = arc.FileSystem.Copy()   
        let assay = ArcAssay("MyAssay")
        study.AddRegisteredAssay(assay)
        arc.UpdateFileSystem()
        let newFS = arc.FileSystem
        Expect.notEqual oldFS.Tree newFS.Tree "Tree should be unequal"
    )
    testCase "set ISA" <| fun () ->
        let arc = new ARC()
        let paths = arc.FileSystem.Tree.ToFilePaths()
        let expected_paths = [|"isa.investigation.xlsx"; "workflows/.gitkeep"; "runs/.gitkeep"; "assays/.gitkeep"; "studies/.gitkeep"|]
        Expect.sequenceEqual paths expected_paths "paths"
        let i = ArcInvestigation.init("My Investigation") 
        let a = i.InitAssay("My Assay")
        ()
        arc.ISA <- Some i
        let paths2 = arc.FileSystem.Tree.ToFilePaths()
        let expected_paths2 = [|
            "isa.investigation.xlsx"; "workflows/.gitkeep"; "runs/.gitkeep";
            "assays/.gitkeep"; "assays/My Assay/isa.assay.xlsx";
            "assays/My Assay/README.md"; "assays/My Assay/dataset/.gitkeep";
            "assays/My Assay/protocols/.gitkeep"; "studies/.gitkeep"
        |]
        Expect.equal paths2 expected_paths2 "paths2"
    testCase "setFileSystem" <| fun () ->
        let initial_paths = [|"isa.investigation.xlsx"; "workflows/.gitkeep"; "runs/.gitkeep"; "assays/.gitkeep"; "studies/.gitkeep"|]
        let updated_paths = [|"isa.investigation.xlsx"; "workflows/.gitkeep"; "runs/.gitkeep"; "assays/.gitkeep"; "studies/.gitkeep"; "studies/testFile.txt"|]
        let arc = ARC.fromFilePaths(initial_paths)
        let paths = arc.FileSystem.Tree.ToFilePaths()
        Expect.sequenceEqual paths initial_paths "paths"
        arc.SetFilePaths(updated_paths)
        let paths2 = arc.FileSystem.Tree.ToFilePaths()
        Expect.sequenceEqual paths2 updated_paths "paths2"        
]

open ARCtrl.FileSystem

let private ``payload_file_filters`` = 
    
    let orderFST (fs : FileSystemTree) = 
        fs
        |> FileSystemTree.toFilePaths()
        |> Array.sort
        |> FileSystemTree.fromFilePaths

    testList "payload file filters" [
        let inv = ArcInvestigation("MyInvestigation", "BestTitle")

        let assay = ArcAssay("registered_assay")
        let assayTable = assay.InitTable("MyAssayTable")
        assayTable.AppendColumn(CompositeHeader.Input (IOType.Data), [|CompositeCell.createFreeText "registered_assay_input.txt"|])
        assayTable.AppendColumn(CompositeHeader.ProtocolREF, [|CompositeCell.createFreeText "assay_protocol.rtf"|])
        assayTable.AppendColumn(CompositeHeader.Output (IOType.Data), [|CompositeCell.createFreeText "registered_assay_output.txt"|])

        let study = ArcStudy("registered_study")
        inv.AddRegisteredStudy(study)
        let studyTable = study.InitTable("MyStudyTable")
        studyTable.AppendColumn(CompositeHeader.Input (IOType.Sample), [|CompositeCell.createFreeText "some_study_input_material"|])
        studyTable.AppendColumn(CompositeHeader.FreeText "Some File", [|CompositeCell.createFreeText "xd/some_file_that_lies_in_slashxd.txt"|])
        studyTable.AppendColumn(CompositeHeader.ProtocolREF, [|CompositeCell.createFreeText "study_protocol.pdf"|])
        studyTable.AppendColumn(CompositeHeader.Output (IOType.Data), [|CompositeCell.createFreeText "registered_study_output.txt"|])
        study.AddRegisteredAssay(assay)


        let fs = 
            Folder("root",[|
                File "isa.investigation.xlsx"; // this should be included
                File "README.md"; // this should be included
                Folder("xd", [|File "some_file_that_lies_in_slashxd.txt"|]); // this should be included
                Folder(".arc", [|File ".gitkeep"|]);
                Folder(".git",[|
                    File "config"; File "description"; File "HEAD";
                    Folder("hooks",[|
                        File "applypatch-msg.sample"; File "commit-msg.sample";
                        File "fsmonitor-watchman.sample"; File "post-update.sample";
                        File "pre-applypatch.sample"; File "pre-commit.sample";
                        File "pre-merge-commit.sample"; File "pre-push.sample";
                        File "pre-rebase.sample"; File "pre-receive.sample";
                        File "prepare-commit-msg.sample";
                        File "push-to-checkout.sample"; File "update.sample"
                    |]);
                    Folder ("info", [|File "exclude"|])
                |]);
                Folder("assays",[|
                    File ".gitkeep";
                    Folder("registered_assay",[|
                        File "isa.assay.xlsx"; // this should be included
                        File "README.md"; // this should be included
                        Folder ("dataset", [|
                            File "registered_assay_input.txt" // this should be included
                            File "registered_assay_output.txt" // this should be included
                            File "unregistered_file.txt"
                        |]; ); 
                        Folder ("protocols", [|File "assay_protocol.rtf"|]) // this should be included
                    |]);
                    Folder
                        ("unregistered_assay",[|
                        File "isa.assay.xlsx"; File "README.md";
                        Folder ("dataset", [|File ".gitkeep"|]);
                        Folder ("protocols", [|File ".gitkeep"|])
                    |])
                |]);
                Folder("runs", [|File ".gitkeep"|]); // this folder should be included (empty)
                Folder("studies",[|
                    File ".gitkeep";
                    Folder("registered_study",[|
                        File "isa.study.xlsx"; // this should be included
                        File "README.md"; // this should be included
                        Folder ("protocols", [|File "study_protocol.pdf"|]); // this should be included
                        Folder ("resources", [|File "registered_study_output.txt"|]) // this should be included
                    |]);
                    Folder("unregistered_study",[|
                        File "isa.study.xlsx"; File "README.md";
                        Folder ("protocols", [|File ".gitkeep"|]);
                        Folder ("resources", [|File ".gitkeep"|])
                    |]);
                |]);
                Folder ("workflows", [|File ".gitkeep"|]) // this folder should be included (empty)
            |])
       
        let arc = ARC(isa = inv, fs = FileSystem.create(fs))

        test "GetRegisteredPayload" {
            let expected = 
                Folder("root",[|
                    File "isa.investigation.xlsx"; // this should be included
                    File "README.md"; // this should be included
                    Folder("xd", [|File "some_file_that_lies_in_slashxd.txt"|]); // this should be included
                    Folder("assays",[|
                        Folder("registered_assay",[|
                            File "isa.assay.xlsx"; // this should be included
                            File "README.md"; // this should be included
                            Folder ("dataset", [|
                                File "registered_assay_input.txt" // this should be included
                                File "registered_assay_output.txt" // this should be included
                            |]; ); 
                            Folder ("protocols", [|File "assay_protocol.rtf"|]) // this should be included
                        |]);
                    |]);
                    Folder("runs", [||]); // this folder should be included (empty)
                    Folder("studies",[|
                        Folder("registered_study",[|
                            File "isa.study.xlsx"; // this should be included
                            File "README.md"; // this should be included
                            Folder ("protocols", [|File "study_protocol.pdf"|]); // this should be included
                            Folder ("resources", [|File "registered_study_output.txt"|]) // this should be included
                        |]);
                    |]);
                    Folder ("workflows", [||]) // this folder should be included (empty)
                |])

            let actual = arc.GetRegisteredPayload()
            Expect.equal (orderFST actual) (orderFST expected) "incorrect payload."
        }
        test "GetAdditionalPayload" {
            let expected = 
                Folder("root",[|
                    Folder("assays",[|
                        Folder("registered_assay",[|
                            Folder ("dataset", [|
                                File "unregistered_file.txt"
                            |]; ); 
                        |]);
                        Folder
                            ("unregistered_assay",[|
                            File "isa.assay.xlsx"; File "README.md";
                            Folder ("dataset", [||]);
                            Folder ("protocols", [||])
                        |])
                    |]);
                    Folder("studies",[|
                        Folder("unregistered_study",[|
                            File "isa.study.xlsx"; File "README.md";
                            Folder ("protocols", [||]);
                            Folder ("resources", [||])
                        |]);
                    |]);
                |])
            let actual = arc.GetAdditionalPayload()
            Expect.equal (orderFST actual) (orderFST expected) "incorrect payload."
        }
    ]

let private tests_GetAssayRemoveContracts = testList "GetAssayRemoveContracts" [
    ptestCase "not registered, fsworkbook equal" <| fun _ ->
        let arc = ARC()
        let i = ArcInvestigation.init("My Investigation")
        arc.ISA <- Some i
        let assayIdentifier = "My Assay"
        i.InitAssay(assayIdentifier) |> ignore
        Expect.equal i.AssayCount 1 "ensure assay count"
        let actual = arc.GetAssayRemoveContracts(assayIdentifier)
        let expected = [
            Contract.createDelete (ArcPathHelper.getAssayFolderPath assayIdentifier)
            i.ToUpdateContract()
        ]
        Expect.sequenceEqual actual expected "we do not have correct FsWorkbook equality helper functions"
    testCase "not registered" <| fun _ ->
        let arc = ARC()
        let i = ArcInvestigation.init("My Investigation")
        arc.ISA <- Some i
        let assayIdentifier = "My Assay"
        i.InitAssay(assayIdentifier) |> ignore
        Expect.equal i.AssayCount 1 "ensure assay count"
        let actual = arc.GetAssayRemoveContracts(assayIdentifier)
        Expect.hasLength actual 2 "contract count"
        Expect.equal actual.[0].Path (ArcPathHelper.getAssayFolderPath assayIdentifier) "assay contract path"
        Expect.equal actual.[0].Operation DELETE "assay contract cmd"
        Expect.equal actual.[1].Path (ArcPathHelper.InvestigationFileName) "inv contract path"
        Expect.equal actual.[1].Operation UPDATE "inve contract cmd"
        Expect.isSome actual.[1].DTO "has DTO"
        let dtoType = Expect.wantSome actual.[1].DTOType "has DTOType"
        Expect.equal dtoType DTOType.ISA_Investigation "dto type"
    testCase "registered in multiple studies" <| fun _ ->
        let arc = ARC()
        let i = ArcInvestigation.init("My Investigation")
        arc.ISA <- Some i
        let assayIdentifier = "My Assay"
        let s1 = i.InitStudy("Study 1")
        let s2 = i.InitStudy("Study 2")
        let a = i.InitAssay(assayIdentifier)
        s1.RegisterAssay(assayIdentifier)
        s2.RegisterAssay(assayIdentifier)
        Expect.equal i.AssayCount 1 "ensure assay count"
        Expect.equal i.StudyCount 2 "ensure study count"
        Expect.hasLength a.StudiesRegisteredIn 2 "ensure studies registered in - count"
        let actual = arc.GetAssayRemoveContracts(assayIdentifier)
        Expect.hasLength actual 4 "contract count"
        Expect.equal actual.[0].Path (ArcPathHelper.getAssayFolderPath assayIdentifier) "assay contract path"
        Expect.equal actual.[0].Operation DELETE "assay contract cmd"
        Expect.equal actual.[1].Path (ArcPathHelper.InvestigationFileName) "inv contract path"
        Expect.equal actual.[1].Operation UPDATE "inv contract cmd"
        Expect.equal actual.[2].Path (Identifier.Study.fileNameFromIdentifier "Study 1") "study 1 contract path"
        Expect.equal actual.[2].Operation UPDATE "study 1 contract cmd"
        Expect.equal actual.[3].Path (Identifier.Study.fileNameFromIdentifier "Study 2") "study 2 contract path"
        Expect.equal actual.[3].Operation UPDATE "study 2 contract cmd"
]

let tests_GetAssayRenameContracts = testList "GetAssayRenameContracts" [
    testCase "not existing" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitAssay("OtherAssayName") |> ignore
        let arc = ARC(isa = i)
        
        let assayMoveF = 
            fun () -> arc.GetAssayRenameContracts("MyOldAssay","MyNewAssay") |> ignore

        Expect.throws assayMoveF "Should fail as arc does not contan assay with given name"
    testCase "Basic" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitAssay("MyOldAssay") |> ignore
        let arc = ARC(isa = i)
        arc.GetWriteContracts() |> ignore
        let contracts = arc.GetAssayRenameContracts("MyOldAssay","MyNewAssay")
        Expect.hasLength contracts 2 "Contract count is wrong"
        let renameContract = contracts.[0]
        Expect.equal renameContract.Operation Operation.RENAME "Rename contract operation"
        Expect.equal "assays/MyOldAssay" renameContract.Path "Rename contract path"
        let renameDTO = Expect.wantSome renameContract.DTO "Rename contract dto"
        let expectedRenameDTO = DTO.Text "assays/MyNewAssay"
        Expect.equal renameDTO expectedRenameDTO "Rename contract dto"
        let updateContract = contracts.[1]
        Expect.equal updateContract.Operation Operation.UPDATE "Update contract operation"
        Expect.equal updateContract.Path "assays/MyNewAssay/isa.assay.xlsx" "Update contract path"
        let updateDTO = Expect.wantSome updateContract.DTO "Update contract dto"
        Expect.isTrue updateDTO.isSpreadsheet "Update contract dto"
        let wb = updateDTO.AsSpreadsheet() :?> FsWorkbook
        let updatedAssay = ArcAssay.fromFsWorkbook wb
        Expect.equal updatedAssay.Identifier "MyNewAssay" "Update contract Assay Identifier"
    testCase "NotRegisteredInStudy" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitStudy("MyStudy") |> ignore
        i.InitAssay("MyOldAssay") |> ignore
        let arc = ARC(isa = i)
        arc.GetWriteContracts() |> ignore
        let contracts = arc.GetAssayRenameContracts("MyOldAssay","MyNewAssay")
        Expect.hasLength contracts 2 "Contract count is wrong"
        let renameContract = contracts.[0]
        Expect.equal renameContract.Operation Operation.RENAME "Rename contract operation"
        Expect.equal "assays/MyOldAssay" renameContract.Path "Rename contract path"
        let renameDTO = Expect.wantSome renameContract.DTO "Rename contract dto"
        let expectedRenameDTO = DTO.Text "assays/MyNewAssay"
        Expect.equal renameDTO expectedRenameDTO "Rename contract dto"
        let updateContract = contracts.[1]
        Expect.equal updateContract.Operation Operation.UPDATE "Update contract operation"
        Expect.equal updateContract.Path "assays/MyNewAssay/isa.assay.xlsx" "Update contract path"
        let updateDTO = Expect.wantSome updateContract.DTO "Update contract dto"
        Expect.isTrue updateDTO.isSpreadsheet "Update contract dto"
        let wb = updateDTO.AsSpreadsheet() :?> FsWorkbook
        let updatedAssay = ArcAssay.fromFsWorkbook wb
        Expect.equal updatedAssay.Identifier "MyNewAssay" "Update contract Assay Identifier"
    testCase "RegisteredInStudy" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        let s = i.InitStudy("MyStudy")
        s.InitRegisteredAssay("MyOldAssay") |> ignore
        let arc = ARC(isa = i)
        arc.GetWriteContracts() |> ignore
        let contracts = arc.GetAssayRenameContracts("MyOldAssay","MyNewAssay")
        Expect.hasLength contracts 3 "Contract count is wrong"
        // Rename contract
        let renameContract = contracts.[0]
        Expect.equal renameContract.Operation Operation.RENAME "Rename contract operation"
        Expect.equal "assays/MyOldAssay" renameContract.Path "Rename contract path"
        let renameDTO = Expect.wantSome renameContract.DTO "Rename contract dto"
        let expectedRenameDTO = DTO.Text "assays/MyNewAssay"
        Expect.equal renameDTO expectedRenameDTO "Rename contract dto"
        // Study Update contract
        let StudyUpdateContract = contracts.[1]
        Expect.equal StudyUpdateContract.Operation Operation.UPDATE "Update contract operation"
        Expect.equal StudyUpdateContract.Path "studies/MyStudy/isa.study.xlsx" "Update contract path"
        let studyUpdateDTO = Expect.wantSome StudyUpdateContract.DTO "Update contract dto"
        Expect.isTrue studyUpdateDTO.isSpreadsheet "Update contract dto"
        let wb = studyUpdateDTO.AsSpreadsheet() :?> FsWorkbook
        let updatedStudy,_ = ArcStudy.fromFsWorkbook wb
        Expect.equal updatedStudy.Identifier "MyStudy" "Update contract Study Identifier"
        Expect.hasLength updatedStudy.RegisteredAssayIdentifiers 1 "Update contract Study Assay count"
        Expect.equal updatedStudy.RegisteredAssayIdentifiers.[0] "MyNewAssay" "Update contract Study Assay Identifier"
        // Assay Update contract
        let assayUpdateContract = contracts.[2]
        Expect.equal assayUpdateContract.Operation Operation.UPDATE "Update contract operation"
        Expect.equal assayUpdateContract.Path "assays/MyNewAssay/isa.assay.xlsx" "Update contract path"
        let assayUpdateDTO = Expect.wantSome assayUpdateContract.DTO "Update contract dto"
        Expect.isTrue assayUpdateDTO.isSpreadsheet "Update contract dto"
        let wb = assayUpdateDTO.AsSpreadsheet() :?> FsWorkbook
        let updatedAssay = ArcAssay.fromFsWorkbook wb
        Expect.equal updatedAssay.Identifier "MyNewAssay" "Update contract Assay Identifier"
]

open ARCtrl.Spreadsheet

let tests_GetStudyRenameContracts = testList "GetStudyRenameContracts" [
    testCase "not existing" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitStudy("OtherStudyName") |> ignore
        let arc = ARC(isa = i)
        
        let studyMoveF = 
            fun () -> arc.GetStudyRenameContracts("MyOldStudy","MyNewStudy") |> ignore

        Expect.throws studyMoveF "Should fail as arc does not contan study with given name"
    testCase "Basic" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitStudy("MyOldStudy") |> ignore
        let arc = ARC(isa = i)
        arc.GetWriteContracts() |> ignore
        let contracts = arc.GetStudyRenameContracts("MyOldStudy","MyNewStudy")
        Expect.hasLength contracts 2 "Contract count is wrong"
        // Rename contract
        let renameContract = contracts.[0]
        Expect.equal renameContract.Operation Operation.RENAME "Rename contract operation"
        Expect.equal "studies/MyOldStudy" renameContract.Path "Rename contract path"
        let renameDTO = Expect.wantSome renameContract.DTO "Rename contract dto"
        let expectedRenameDTO = DTO.Text "studies/MyNewStudy"
        Expect.equal renameDTO expectedRenameDTO "Rename contract dto"
        // Update study contract
        let updateContract = contracts.[1]
        Expect.equal updateContract.Operation Operation.UPDATE "Update contract operation"
        Expect.equal updateContract.Path "studies/MyNewStudy/isa.study.xlsx" "Update contract path"
        let updateDTO = Expect.wantSome updateContract.DTO "Update contract dto"
        Expect.isTrue updateDTO.isSpreadsheet "Update contract dto"
        let wb = updateDTO.AsSpreadsheet() :?> FsWorkbook
        let updatedStudy,_ = ArcStudy.fromFsWorkbook wb
        Expect.equal updatedStudy.Identifier "MyNewStudy" "Update contract Study Identifier"
    testCase "RegisteredInInvestigation" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitStudy("MyOldStudy") |> ignore
        i.RegisterStudy("MyOldStudy") |> ignore
        let arc = ARC(isa = i)
        arc.GetWriteContracts() |> ignore
        let contracts = arc.GetStudyRenameContracts("MyOldStudy","MyNewStudy")
        Expect.hasLength contracts 3 "Contract count is wrong"
        // Rename contract
        let renameContract = contracts.[0]
        Expect.equal renameContract.Operation Operation.RENAME "Rename contract operation"
        Expect.equal "studies/MyOldStudy" renameContract.Path "Rename contract path"
        let renameDTO = Expect.wantSome renameContract.DTO "Rename contract dto"
        let expectedRenameDTO = DTO.Text "studies/MyNewStudy"
        Expect.equal renameDTO expectedRenameDTO "Rename contract dto"
        // Investigation Update contract
        let invUpdateContract = contracts.[1]
        Expect.equal invUpdateContract.Operation Operation.UPDATE "Update contract operation"
        Expect.equal invUpdateContract.Path "isa.investigation.xlsx" "Update contract path"
        let invUpdateDTO = Expect.wantSome invUpdateContract.DTO "Update contract dto"
        Expect.isTrue invUpdateDTO.isSpreadsheet "Update contract dto"
        let wb = invUpdateDTO.AsSpreadsheet() :?> FsWorkbook
        let updatedInvestigation = ArcInvestigation.fromFsWorkbook wb
        Expect.equal updatedInvestigation.Identifier "MyInvestigation" "Update contract Investigation Identifier"
        Expect.hasLength updatedInvestigation.RegisteredStudyIdentifiers 1 "Update contract Investigation Study count"
        Expect.equal updatedInvestigation.RegisteredStudyIdentifiers.[0] "MyNewStudy" "Update contract Investigation Study Identifier"
        // Study update contract
        let studyUpdateContract = contracts.[2]
        Expect.equal studyUpdateContract.Operation Operation.UPDATE "Update contract operation"
        Expect.equal studyUpdateContract.Path "studies/MyNewStudy/isa.study.xlsx" "Update contract path"
        let studyUpdateDTO = Expect.wantSome studyUpdateContract.DTO "Update contract dto"
        Expect.isTrue studyUpdateDTO.isSpreadsheet "Update contract dto"
        let wb = studyUpdateDTO.AsSpreadsheet() :?> FsWorkbook
        let updatedStudy,_ = ArcStudy.fromFsWorkbook wb
        Expect.equal updatedStudy.Identifier "MyNewStudy" "Update contract Study Identifier"

] 


let tests_load =

    testList "Load" [
        testCaseCrossAsync "simpleARC" (crossAsync {
            let p = TestObjects.IO.testSimpleARC
            let! result = ARC.tryLoadAsync(p)
            let result = Expect.wantOk result "ARC should load successfully"

            Expect.isSome result.ISA "Should contain an ISA part"
            Expect.isNone result.CWL "Should not contain a CWL part"

            let isa = result.ISA.Value
            Expect.equal isa.StudyCount 1 "Should contain 1 study"
            Expect.equal isa.AssayCount 1 "Should contain 1 assay"
            Expect.equal isa.RegisteredStudies.Count 1 "Should contain 1 registered study"
            
            let s = isa.Studies.[0]
            Expect.equal s.RegisteredAssayCount 1 "Should contain 1 registered assay"
            Expect.equal s.TableCount 3 "Study should contain 3 tables"

            let a = s.RegisteredAssays.[0]
            Expect.equal a.TableCount 4 "Assay should contain 4 tables"
            }
            
        )
    ]


let tests_write =

    testList "Write" [
        testCaseCrossAsync "empty" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_Write_Empty"
            let a = ARC()

            let! result = a.TryWriteAsync(p)

            Expect.wantOk result "ARC should write successfully" |> ignore

            let expectedPaths = 
                [
                    "/isa.investigation.xlsx";
                    "/assays/.gitkeep";
                    "/studies/.gitkeep";
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep"          
                ]
                |> List.sort


            let! paths = 
                FileSystemHelper.getAllFilePathsAsync p
                |> map Seq.sort

            Expect.sequenceEqual paths expectedPaths "Files were not created correctly."                  
        })
        testCaseCrossAsync "SimpleARC" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_Write_SimpleARC"
            let arc = ARC()

            let i = ArcInvestigation("MyInvestigation")
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            i.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
            arc.ISA <- Some i
            arc.UpdateFileSystem()

            let! result = arc.TryWriteAsync(p)
            Expect.wantOk result "ARC should write successfully" |> ignore

            let expectedPaths = 
                [
                    "/isa.investigation.xlsx";
                    "/studies/.gitkeep";
                    $"/studies/{studyName}/isa.study.xlsx"
                    $"/studies/{studyName}/README.md"
                    $"/studies/{studyName}/protocols/.gitkeep";
                    $"/studies/{studyName}/resources/.gitkeep";
                    "/assays/.gitkeep";
                    $"/assays/{assayName}/isa.assay.xlsx"
                    $"/assays/{assayName}/README.md"
                    $"/assays/{assayName}/protocols/.gitkeep"
                    $"/assays/{assayName}/dataset/.gitkeep"
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep"          
                ]
                |> List.sort


            let! paths = 
                FileSystemHelper.getAllFilePathsAsync p
                |> map Seq.sort

            Expect.sequenceEqual paths expectedPaths "Files were not created correctly."            
        })
        // This test reads a preexisting assay with data and everything, data content is not copied though but just the 
        testCaseCrossAsync "LoadSimpleARCAndAddAssay" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_Write_SimpleARCWithAssay"

            let! readResult = ARC.tryLoadAsync(TestObjects.IO.testSimpleARC)
            let arc = Expect.wantOk readResult "ARC should load correctly"

            let i = arc.ISA.Value

            let existingStudyName = "experiment1_material"
            let existingAssayName = "measurement1"

            let assayName = "YourAssay"
            i.InitAssay(assayName) |> ignore
            arc.ISA <- Some i

            arc.UpdateFileSystem()

            let! writeResult = arc.TryWriteAsync(p)

            Expect.wantOk writeResult "ARC should write successfully" |> ignore

            let expectedPaths = 
                [
                    "/isa.investigation.xlsx";
                    "/studies/.gitkeep";
                    $"/studies/{existingStudyName}/isa.study.xlsx"
                    $"/studies/{existingStudyName}/README.md"
                    $"/studies/{existingStudyName}/protocols/.gitkeep";
                    $"/studies/{existingStudyName}/resources/.gitkeep";
                    $"/studies/{existingStudyName}/resources/Sample5_84c37b60-2342-4226-a36c-4b8dfe84ebe9.png"
                    $"/studies/{existingStudyName}/resources/Sample6_208df064-4b1c-4da0-a1f8-6412e1fb2284.png"
                    $"/studies/{existingStudyName}/resources/Sample1_e36ca6b8-19ba-4504-aa82-d4781765873d.png"
                    $"/studies/{existingStudyName}/resources/Sample2_714ca2b7-22b7-4f69-b83d-9165f624da25.png"
                    $"/studies/{existingStudyName}/resources/Sample3_66fac760-acc7-4ed4-ba21-2cb67fa36e4d.png"
                    $"/studies/{existingStudyName}/resources/Sample4_cba5f40c-fc05-44d6-a589-b0e3dafaeefe.png"
                    "/assays/.gitkeep";
                    $"/.arc/.gitkeep"
                    $"/assays/{existingAssayName}/isa.assay.xlsx"
                    $"/assays/{existingAssayName}/README.md"
                    $"/assays/{existingAssayName}/isa.datamap.xlsx"
                    $"/assays/{existingAssayName}/protocols/.gitkeep"
                    $"/assays/{existingAssayName}/protocols/extractionProtocol.txt"
                    $"/assays/{existingAssayName}/dataset/.gitkeep"
                    $"/assays/{existingAssayName}/dataset/table.csv"
                    $"/assays/{existingAssayName}/dataset/proteomics_result.csv"
                    $"/assays/{existingAssayName}/dataset/sample1.raw"
                    $"/assays/{existingAssayName}/dataset/sample2.raw"
                    $"/assays/{existingAssayName}/dataset/sample3.raw"
                    $"/assays/{existingAssayName}/dataset/sample4.raw"
                    $"/assays/{existingAssayName}/dataset/sample5.raw"
                    $"/assays/{existingAssayName}/dataset/sample6.raw"
                    $"/assays/{existingAssayName}/dataset/sample7.raw"
                    $"/assays/{assayName}/isa.assay.xlsx"
                    $"/assays/{assayName}/README.md"
                    $"/assays/{assayName}/protocols/.gitkeep"
                    $"/assays/{assayName}/dataset/.gitkeep"
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep"          
                ]
                |> List.sort


            let! paths = 
                FileSystemHelper.getAllFilePathsAsync p
                |> map Seq.sort

            Expect.sequenceEqual paths expectedPaths "Files were not created correctly."            
        })
        |> testSequenced
    ]

let tests_Update =
    testList "Update" [
        testCaseCrossAsync "AddedAssay" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_Update_AddedAssay"
            let arc = ARC()

            // setup arc
            let i = ArcInvestigation("MyInvestigation")
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            i.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
            arc.ISA <- Some i
            arc.UpdateFileSystem()

            let! writeResult = arc.TryWriteAsync(p)

            Expect.wantOk writeResult "ARC should write successfully" |> ignore

            // add assay
            let newAssayName = "MyNewAssay"
            i.InitAssay(newAssayName) |> ignore
            arc.ISA <- Some i
            arc.UpdateFileSystem()

            let! updateResult = arc.TryUpdateAsync(p)

            Expect.wantOk updateResult "ARC should update successfully" |> ignore

            let expectedPaths = 
                [
                    "/isa.investigation.xlsx";
                    "/studies/.gitkeep";
                    $"/studies/{studyName}/isa.study.xlsx"
                    $"/studies/{studyName}/README.md"
                    $"/studies/{studyName}/protocols/.gitkeep";
                    $"/studies/{studyName}/resources/.gitkeep";
                    "/assays/.gitkeep";
                    $"/assays/{assayName}/isa.assay.xlsx"
                    $"/assays/{assayName}/README.md"
                    $"/assays/{assayName}/protocols/.gitkeep"
                    $"/assays/{assayName}/dataset/.gitkeep"
                    $"/assays/{newAssayName}/isa.assay.xlsx"
                    $"/assays/{newAssayName}/README.md"
                    $"/assays/{newAssayName}/protocols/.gitkeep"
                    $"/assays/{newAssayName}/dataset/.gitkeep"
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep"          
                ]
                |> List.sort

            let! paths = 
                FileSystemHelper.getAllFilePathsAsync p
                |> map Seq.sort

            Expect.sequenceEqual paths expectedPaths "Files were not created correctly."
        })


    ]

let tests_renameAssay =
    testList "RenameAssay" [
        testCaseCrossAsync "SimpleARC" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_RenameAssay_SimpleARC"
            let arc = ARC()

            // setup arc
            let i = ArcInvestigation("MyInvestigation")
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            i.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
            arc.ISA <- Some i
            arc.UpdateFileSystem()

            let! updateResult = arc.TryWriteAsync(p)

            Expect.wantOk updateResult "ARC should write successfully" |> ignore

            // rename assay
            let newAssayName = "MyNewAssay"

            let! renameResult = arc.TryRenameAssayAsync(p,assayName, newAssayName)
            Expect.wantOk renameResult "Assay should be renamed successfully" |> ignore

            let expectedPaths = 
                [
                    "/isa.investigation.xlsx";
                    "/studies/.gitkeep";
                    $"/studies/{studyName}/isa.study.xlsx"
                    $"/studies/{studyName}/README.md"
                    $"/studies/{studyName}/protocols/.gitkeep";
                    $"/studies/{studyName}/resources/.gitkeep";
                    "/assays/.gitkeep";
                    $"/assays/{newAssayName}/isa.assay.xlsx"
                    $"/assays/{newAssayName}/README.md"
                    $"/assays/{newAssayName}/protocols/.gitkeep"
                    $"/assays/{newAssayName}/dataset/.gitkeep"
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep"          
                ]
                |> List.sort

            let! paths = 
                FileSystemHelper.getAllFilePathsAsync p
                |> map Seq.sort

            Expect.sequenceEqual paths expectedPaths "Files were not created correctly."            
        })
    ]

let tests_RenameStudy =
    testList "RenameStudy" [
        testCaseCrossAsync "SimpleARC" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_RenameStudy_SimpleARC"
            let arc = ARC()

            // setup arc
            let i = ArcInvestigation("MyInvestigation")
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            i.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
            arc.ISA <- Some i
            arc.UpdateFileSystem()

            let! writeResult = arc.TryWriteAsync(p)

            Expect.wantOk writeResult "ARC should write successfully" |> ignore

            // rename study
            let newStudyName = "MyNewStudy"

            let! renameResult = arc.TryRenameStudyAsync(p,studyName, newStudyName)
            Expect.wantOk renameResult "Study should be renamed successfully" |> ignore

            let expectedPaths = 
                [
                    "/isa.investigation.xlsx";
                    "/studies/.gitkeep";
                    $"/studies/{newStudyName}/isa.study.xlsx"
                    $"/studies/{newStudyName}/README.md"
                    $"/studies/{newStudyName}/protocols/.gitkeep";
                    $"/studies/{newStudyName}/resources/.gitkeep";
                    "/assays/.gitkeep";
                    $"/assays/{assayName}/isa.assay.xlsx"
                    $"/assays/{assayName}/README.md"
                    $"/assays/{assayName}/protocols/.gitkeep"
                    $"/assays/{assayName}/dataset/.gitkeep"
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep"          
                ]
                |> List.sort

            let! paths = 
                FileSystemHelper.getAllFilePathsAsync p
                |> map Seq.sort

            Expect.sequenceEqual paths expectedPaths "Files were not created correctly."

        })
    ]

let tests_RemoveAssay =
    testList "RemoveAssay" [
        testCaseCrossAsync "SimpleARC" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_RemoveAssay_SimpleARC"
            let arc = ARC()

            // setup arc
            let i = ArcInvestigation("MyInvestigation")
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            i.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
            arc.ISA <- Some i
            arc.UpdateFileSystem()

            let! writeResult = arc.TryWriteAsync(p)

            Expect.wantOk writeResult "ARC should write successfully" |> ignore

            // remove assay

            let! removeResult = arc.TryRemoveAssayAsync(p,assayName)
            Expect.wantOk removeResult "Assay should be removed successfully" |> ignore

            let expectedPaths = 
                [
                    "/isa.investigation.xlsx";
                    "/studies/.gitkeep";
                    $"/studies/{studyName}/isa.study.xlsx"
                    $"/studies/{studyName}/README.md"
                    $"/studies/{studyName}/protocols/.gitkeep";
                    $"/studies/{studyName}/resources/.gitkeep";
                    "/assays/.gitkeep";
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep"          
                ]
                |> List.sort

            let! paths = 
                FileSystemHelper.getAllFilePathsAsync p
                |> map Seq.sort

            Expect.sequenceEqual paths expectedPaths "Files were not created correctly."
        })
    ]

let tests_RemoveStudy =
    testList "RemoveStudy" [
        testCaseCrossAsync "SimpleARC" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_RemoveStudy_SimpleARC"
            let arc = ARC()

            // setup arc
            let i = ArcInvestigation("MyInvestigation")
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            i.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
            arc.ISA <- Some i
            arc.UpdateFileSystem()

            let! writeResult = arc.TryWriteAsync(p)

            Expect.wantOk writeResult "ARC should write successfully" |> ignore

            // remove study

            let! removeResult = arc.TryRemoveStudyAsync(p,studyName)
            Expect.wantOk removeResult "Study should be removed successfully" |> ignore

            let expectedPaths = 
                [
                    "/isa.investigation.xlsx";
                    "/studies/.gitkeep";
                    "/assays/.gitkeep";
                    $"/assays/{assayName}/isa.assay.xlsx"
                    $"/assays/{assayName}/README.md"
                    $"/assays/{assayName}/protocols/.gitkeep"
                    $"/assays/{assayName}/dataset/.gitkeep"
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep"          
                ]
                |> List.sort

            let! paths = 
                FileSystemHelper.getAllFilePathsAsync p
                |> map Seq.sort

            Expect.sequenceEqual paths expectedPaths "Files were not created correctly."
        })
    ]

let tests_ROCrate =
    testList "RO-Crate" [
        testCase "CanRead_Deprecated" <| fun _ ->
            let arc = ARC.fromDeprecatedROCrateJsonString(TestObjects.ROCrate.ArcPrototypeDeprecated.ed123499)
            let isa = Expect.wantSome arc.ISA "ARC should contain an ISA part"
            let nonDeprecatedARC = ARC.fromROCrateJsonString(TestObjects.ROCrate.ArcPrototype.ed123499)
            let nonDeprecatedISA = Expect.wantSome nonDeprecatedARC.ISA "ARC should contain an ISA part"
            Expect.equal isa.Identifier nonDeprecatedISA.Identifier "Investigation should have correct identifier"
            Expect.equal isa.Title nonDeprecatedISA.Title "Investigation should have correct title"
            Expect.equal isa.Description nonDeprecatedISA.Description "Investigation should have correct description"
            Expect.equal isa.Contacts.[1] nonDeprecatedISA.Contacts.[1] "Investigation should have correct contacts"
            Expect.equal isa.Studies.[1] nonDeprecatedISA.Studies.[1] "Investigation should have correct studies"
        testCase "CanRead" <| fun _ ->
            let arc = ARC.fromROCrateJsonString(TestObjects.ROCrate.ArcPrototype.ed123499)
            let isa = Expect.wantSome arc.ISA "ARC should contain an ISA part"
            Expect.equal isa.Identifier "ArcPrototype" "Investigation should have correct identifier"
            let title = Expect.wantSome isa.Title "Investigation should have title"
            Expect.equal title "ArcPrototype" "Investigation should have correct title"
            let description = Expect.wantSome isa.Description "Investigation should have description"
            Expect.equal description "A prototypic ARC that implements all specification standards accordingly" "Investigation should have correct description"
            /// Contacts
            Expect.hasLength isa.Contacts 3 "Investigation should have 3 contacts"
            let first = isa.Contacts.[0]
            let firstName = Expect.wantSome first.FirstName "First contact should have name"
            Expect.equal firstName "Timo" "First contact should have correct name"
            let lastName = Expect.wantSome first.LastName "First contact should have last name"
            Expect.equal lastName "Mühlhaus" "First contact should have correct last name"
            Expect.isNone first.MidInitials "First contact should not have middle initials"
            let orcid = Expect.wantSome first.ORCID "First contact should have ORCID"
            Expect.equal orcid "0000-0003-3925-6778" "First contact should have correct ORCID"
            let affiliation = Expect.wantSome first.Affiliation "First contact should have affiliation"
            Expect.equal affiliation "RPTU University of Kaiserslautern" "First contact should have correct affiliation"
            let address = Expect.wantSome first.Address "First contact should have address"
            Expect.equal address "RPTU University of Kaiserslautern, Paul-Ehrlich-Str. 23 , 67663 Kaiserslautern" "First contact should have correct address"
            Expect.hasLength first.Roles 1 "First contact should have roles"
            let firstRole = first.Roles.[0]
            Expect.equal firstRole.NameText "principal investigator" "First contact should have correct role"
            /// Studies 
            Expect.equal isa.StudyCount 2 "ARC should contain 2 studies"
            let secondStudy = isa.GetStudy("MaterialPreparation")
            let secondStudyTitle = Expect.wantSome secondStudy.Title "Second study should have title"
            Expect.equal secondStudyTitle "Prototype for experimental data" "Second study should have correct title"
            let secondStudyDescription = Expect.wantSome secondStudy.Description "Second study should have description"
            Expect.equal secondStudyDescription "In this a devised study to have an exemplary experimental material description." "Second study should have correct description"
            Expect.isEmpty secondStudy.Contacts "Second study should have no contacts"
            Expect.hasLength secondStudy.Tables 2 "Second study should have 2 tables"
            let firstTable = secondStudy.Tables.[0]
            Expect.equal firstTable.Name "CellCultivation" "First table should have correct name"
            Expect.equal firstTable.RowCount 6 "First table should have correct row count"
            Expect.equal firstTable.ColumnCount 5 "First table should have correct column count"
            let inputCol = Expect.wantSome (firstTable.TryGetInputColumn()) "First table should have input column"
            let expectedHeader = CompositeHeader.Input IOType.Source
            Expect.equal inputCol.Header expectedHeader "First table input column should have correct header"
            let expectedCells = [for i = 1 to 6 do CompositeCell.FreeText $"Source{i}"]
            Expect.sequenceEqual inputCol.Cells expectedCells "First table input column should have correct cells"
            /// Assays
            Expect.equal isa.AssayCount 2 "ARC should contain 2 assays"
    ]



let main = testList "ARCtrl" [
    tests_create
    tests_fromFilePaths
    tests_updateFileSystem
    tests_read_contracts
    tests_writeContracts
    tests_updateContracts
    tests_GetAssayRemoveContracts
    tests_GetAssayRenameContracts
    tests_GetStudyRenameContracts
    payload_file_filters
    tests_load
    tests_write
    tests_Update
    tests_renameAssay
    tests_RenameStudy
    tests_RemoveAssay
    tests_RemoveStudy
    tests_ROCrate
]


