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
        let arc = ARC("MyIdentifier")
        //Expect.isNone arc.CWL "cwl"
        Expect.equal arc.Identifier "MyIdentifier" "Identifier should be set"
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
            @"studies\TestAssay1\resources\.gitkeep";
            |]
            |> Array.map (fun x -> x.Replace(@"\","/"))
            |> Array.sort
        let arc = ARC.fromFilePaths(input)
        //Expect.isNone arc.CWL "cwl"
        Expect.isTrue (Helper.Identifier.isMissingIdentifier arc.Identifier) "nothing should have been parsed from isa"
        let actualFilePaths = arc.ToFilePaths() |> Array.sort
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

let private simpleISAWithWRContracts = 
    [|
        SimpleISA.Investigation.investigationReadContract
        SimpleISA.Study.bII_S_1ReadContract
        SimpleISA.Assay.proteomeReadContract
        SimpleISA.Workflow.proteomicsReadContract
        SimpleISA.Run.proteomicsReadContract
    |]

let private tests_read_contracts = testList "read_contracts" [
    testCase "simpleISAWithWR" (fun () -> 
        let input = 
            [|@"isa.investigation.xlsx"; @".arc\.gitkeep"; @".git\config";
            // assay
            @"assays\TestAssay\isa.assay.xlsx";
            @"assays\TestAssay\README.md";
            @"assays\TestAssay\dataset\.gitkeep";
            @"assays\TestAssay\protocols\.gitkeep";
            // study
            @"studies\TestStudy\isa.study.xlsx";
            @"studies\TestStudy\README.md";
            @"studies\TestStudy\isa.study.xlsx";
            @"studies\TestStudy\protocols\.gitkeep";
            // workflow
            @"workflows\TestWorkflow\isa.workflow.xlsx";
            @"workflows\TestWorkflow\README.md";
            // run
            @"runs\TestRun\isa.run.xlsx";
            @"runs\TestRun\README.md";
            |]
            |> Array.map (fun x -> x.Replace(@"\","/"))
            |> Array.sort
        let arc = ARC.fromFilePaths(input)
        let contracts = arc.GetReadContracts()
        Expect.hasLength contracts 5 "should have read 5 read contracts"

        let investigationContractOpt = contracts |> Array.tryFind (fun c -> c.Path = "isa.investigation.xlsx")
        let investigationContract = Expect.wantSome investigationContractOpt "investigation contract should be present"
        Expect.equal investigationContract.Operation Operation.READ "investigation contract should be a read contract"
        Expect.equal investigationContract.DTOType (Some DTOType.ISA_Investigation) "investigation contract should have the correct DTO type"

        let studyContractOpt = contracts |> Array.tryFind (fun c -> c.Path = "studies/TestStudy/isa.study.xlsx")
        let studyContract = Expect.wantSome studyContractOpt "study contract should be present"
        Expect.equal studyContract.Operation Operation.READ "study contract should be a read contract"
        Expect.equal studyContract.DTOType (Some DTOType.ISA_Study) "study contract should have the correct DTO type"

        let assayContractOpt = contracts |> Array.tryFind (fun c -> c.Path = "assays/TestAssay/isa.assay.xlsx")
        let assayContract = Expect.wantSome assayContractOpt "assay contract should be present"
        Expect.equal assayContract.Operation Operation.READ "assay contract should be a read contract"
        Expect.equal assayContract.DTOType (Some DTOType.ISA_Assay) "assay contract should have the correct DTO type"

        let workflowContractOpt = contracts |> Array.tryFind (fun c -> c.Path = "workflows/TestWorkflow/isa.workflow.xlsx")
        let workflowContract = Expect.wantSome workflowContractOpt "workflow contract should be present"
        Expect.equal workflowContract.Operation Operation.READ "workflow contract should be a read contract"
        Expect.equal workflowContract.DTOType (Some DTOType.ISA_Workflow) "workflow contract should have the correct DTO type"

        let runContractOpt = contracts |> Array.tryFind (fun c -> c.Path = "runs/TestRun/isa.run.xlsx")
        let runContract = Expect.wantSome runContractOpt "run contract should be present"
        Expect.equal runContract.Operation Operation.READ "run contract should be a read contract"
        Expect.equal runContract.DTOType (Some DTOType.ISA_Run) "run contract should have the correct DTO type"      
    )
    testCase "License" (fun () ->
        let input = 
            [|
            @"isa.investigation.xlsx"; @".arc\.gitkeep"; @".git\config";
            @"LICENSE"
            |]
            |> Array.map (fun x -> x.Replace(@"\","/"))
            |> Array.sort
        let arc = ARC.fromFilePaths(input)
        let contracts = arc.GetReadContracts()
        Expect.hasLength contracts 2 "should have read 2 read contracts"
        let licenseContractOpt = contracts |> Array.tryFind (fun c -> c.Path = "LICENSE")
        let licenseContract = Expect.wantSome licenseContractOpt "license contract should be present"
        Expect.equal licenseContract.Operation Operation.READ "license contract should be a read contract"
        Expect.equal licenseContract.DTOType (Some DTOType.PlainText) "license contract should have the correct DTO type"
    )
    testCase "License_AltPath" (fun () ->
        let licensePath = "LICENSE.txt"
        let input = 
            [|
            @"isa.investigation.xlsx"; @".arc\.gitkeep"; @".git\config";
            licensePath
            |]
            |> Array.map (fun x -> x.Replace(@"\","/"))
            |> Array.sort
        let arc = ARC.fromFilePaths(input)
        let contracts = arc.GetReadContracts()
        Expect.hasLength contracts 2 "should have read 2 read contracts"
        let licenseContractOpt = contracts |> Array.tryFind (fun c -> c.Path = licensePath)
        let licenseContract = Expect.wantSome licenseContractOpt "license contract should be present"
        Expect.equal licenseContract.Operation Operation.READ "license contract should be a read contract"
        Expect.equal licenseContract.DTOType (Some DTOType.PlainText) "license contract should have the correct DTO type"
    )

    ]

let private tests_SetISAFromContracts = testList "SetISAFromContracts" [
    testCase "simpleISA" (fun () -> 
        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts simpleISAContracts
        Expect.equal arc.Identifier Investigation.BII_I_1.investigationIdentifier "investigation identifier should have been read from investigation contract"

        Expect.equal arc.Studies.Count 2 "should have read two studies"
        let study1 = arc.Studies.[0]
        Expect.equal study1.Identifier Study.BII_S_1.studyIdentifier "study 1 identifier should have been read from study contract"

        Expect.equal study1.TableCount 2 "study 1 should have the 2 tables. Top level Metadata tables are ignored."
        //Expect.equal study1.TableCount 8 "study 1 should have the 7 tables from investigation plus one extra. One table should be overwritten."
        
        Expect.equal study1.RegisteredAssays.Count 3 "study 1 should have read three assays"
        let assay1 = study1.RegisteredAssays.[0]
        Expect.equal assay1.Identifier Assay.Proteome.assayIdentifier "assay 1 identifier should have been read from assay contract"
        Expect.equal assay1.TableCount 1 "assay 1 should have read one table"   
    )
    testCase "simpleISAWithWR" (fun () -> 
        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts simpleISAWithWRContracts
        Expect.equal arc.Identifier Investigation.BII_I_1.investigationIdentifier "investigation identifier should have been read from investigation contract"

        Expect.equal arc.StudyCount 1 "should have read two studies"
        Expect.equal  arc.Studies.[0].Identifier Study.BII_S_1.studyIdentifier "Study Identifier"

        Expect.equal arc.AssayCount 1 "should have read one assay"
        Expect.equal arc.Assays.[0].Identifier Assay.Proteome.assayIdentifier "Assay Identifier"

        Expect.equal arc.WorkflowCount 1 "should have read one workflow"
        Expect.equal arc.Workflows.[0].Identifier Workflow.Proteomics.workflowIdentifier "Workflow Identifier"

        Expect.equal arc.RunCount 1 "should have read one run"
        Expect.equal arc.Runs.[0].Identifier Run.Proteomics.runIdentifier "Run Identifier"   
    )
    testCase "GetStudyRemoveContractsOnlyRegistered" (fun () -> // set to pending, until performance issues in Study.fromFsWorkbook is resolved.
        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts([|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
        |])
        Expect.equal arc.Identifier Investigation.BII_I_1.investigationIdentifier "investigation identifier should have been read from investigation contract"

        Expect.equal arc.Studies.Count 1 "should have read one study"
        Expect.equal arc.RegisteredStudyIdentifierCount 2 "should have read two registered study identifiers"
        Expect.equal arc.VacantStudyIdentifiers.Count 1 "should have one vacant study identifier"
        let study1 = arc.Studies.[0]
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
        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts([|iContract; sContract; aContract|])
        Expect.equal arc.Identifier UpdateAssayWithStudyProtocol.investigationIdentifier "investigation identifier should have been read from investigation contract"

        Expect.equal arc.Studies.Count 1 "should have read one study"
        let study = arc.Studies.[0]

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

        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts contracts

        let a1 = arc.GetAssay(SimpleISA.Assay.proteomeIdentifer)
        let datamap = Expect.wantSome a1.DataMap "Proteome Assay was supposed to have datamap"
        
        Expect.equal 2 datamap.DataContexts.Count "Datamap was not read correctly"

        let a2 = arc.GetAssay(SimpleISA.Assay.metabolomeIdentifer)
        Expect.isNone a2.DataMap "Metabolome Assay was not supposed to have datamap"
    
    )
    testCase "studySameNameAsAssayWithDatamap" (fun () ->
        // https://github.com/nfdi4plants/ARCtrl/issues/538
        // This test checks that a study with the same name as an assay with a datamap does not faultily also gets the datamap assigned as a child.
        let arc = ARC("MyIdentifier")
        let contracts = [|
            Contract.create (
                Operation.READ,
                path = ArcPathHelper.InvestigationFileName,
                dtoType = DTOType.ISA_Investigation,
                dto = DTO.Spreadsheet (ArcInvestigation.init("MyInvestigation").ToFsWorkbook())
            )
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Assay.proteomeDatamapContract
            Contract.create (
                Operation.READ,
                path = Identifier.Study.fileNameFromIdentifier Assay.Proteome.assayIdentifier,
                dtoType = DTOType.ISA_Study,
                dto = DTO.Spreadsheet (ArcStudy.init(Assay.Proteome.assayIdentifier).ToFsWorkbook())
            )
        |]

        arc.SetISAFromContracts contracts
        Expect.equal arc.Identifier "MyInvestigation" "investigation identifier should have been read from investigation contract"

        Expect.equal arc.StudyCount 1 "should have read two studies"
        Expect.equal arc.Studies.[0].Identifier Assay.Proteome.assayIdentifier "Study Identifier"

        Expect.equal arc.AssayCount 1 "should have read one assay"
        Expect.equal arc.Assays.[0].Identifier Assay.Proteome.assayIdentifier "Assay Identifier"

        // Assay has datamap, but study should not have it
        let dm = Expect.wantSome arc.Assays.[0].DataMap "Assay should have a datamap assigned"
        Expect.hasLength dm.DataContexts 2 "Datamap should have two data contexts"

        Expect.isNone arc.Studies.[0].DataMap "Study should not have a datamap assigned"
    )
    testCase "assaySameNameAsStudyWithDatamap" (fun () ->
        // https://github.com/nfdi4plants/ARCtrl/issues/538
        // This test checks that a assay with the same name as an study with a datamap does not faultily also gets the datamap assigned as a child.
        let arc = ARC("MyIdentifier")
        let contracts = [|
            Contract.create (
                Operation.READ,
                path = ArcPathHelper.InvestigationFileName,
                dtoType = DTOType.ISA_Investigation,
                dto = DTO.Spreadsheet (ArcInvestigation.init("MyInvestigation").ToFsWorkbook())
            )
            SimpleISA.Assay.proteomeReadContract
            Contract.create (
                Operation.READ,
                path = Identifier.Study.datamapFileNameFromIdentifier Assay.Proteome.assayIdentifier,
                dtoType = DTOType.ISA_Datamap,
                dto = DTO.Spreadsheet (SimpleISA.Assay.proteomeDatamapWB)
            )
            Contract.create (
                Operation.READ,
                path = Identifier.Study.fileNameFromIdentifier Assay.Proteome.assayIdentifier,
                dtoType = DTOType.ISA_Study,
                dto = DTO.Spreadsheet (ArcStudy.init(Assay.Proteome.assayIdentifier).ToFsWorkbook())
            )
        |]

        arc.SetISAFromContracts contracts
        Expect.equal arc.Identifier "MyInvestigation" "investigation identifier should have been read from investigation contract"

        Expect.equal arc.StudyCount 1 "should have read two studies"
        Expect.equal arc.Studies.[0].Identifier Assay.Proteome.assayIdentifier "Study Identifier"

        Expect.equal arc.AssayCount 1 "should have read one assay"
        Expect.equal arc.Assays.[0].Identifier Assay.Proteome.assayIdentifier "Assay Identifier"

        // Study has datamap, but assay should not have it
        Expect.isNone arc.Assays.[0].DataMap "Assay should not have a datamap assigned"

        let dm = Expect.wantSome arc.Studies.[0].DataMap "Study should have a datamap assigned"
        Expect.hasLength dm.DataContexts 2 "Datamap should have two data contexts"
    )
    testCase "license" (fun () ->
        let licenseText = "This is my license"
        let licenseContract = 
            Contract.create(
                Operation.READ,
                path = "LICENSE",
                dtoType = DTOType.PlainText,
                dto = DTO.Text licenseText
            )
        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts [|SimpleISA.Investigation.investigationReadContract; licenseContract|]
        let expectedLicense = License(LicenseContentType.Fulltext, content = licenseText)
        let actualLicense = Expect.wantSome arc.License "License was not set"
        Expect.equal actualLicense expectedLicense "License was not set correctly"
    )
    testCase "license_altPath" (fun () ->
        let licenseText = "This is my license"
        let licensePath = "LICENSE.md"
        let licenseContract = 
            Contract.create(
                Operation.READ,
                path = licensePath,
                dtoType = DTOType.PlainText,
                dto = DTO.Text licenseText
            )
        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts [|SimpleISA.Investigation.investigationReadContract; licenseContract|]
        let expectedLicense = License(LicenseContentType.Fulltext, content = licenseText, path = licensePath)
        let actualLicense = Expect.wantSome arc.License "License was not set"
        Expect.equal actualLicense expectedLicense "License was not set correctly"
    )
]

let private tests_writeContracts = testList "write_contracts" [
    testCase "empty" (fun _ ->
        let arc = ARC("MyIdentifier")
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
        let arc = ARC.fromArcInvestigation(isa = inv)
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
    testCase "simpleISAWithWR" (fun _ ->
        let arc = ARC("MyInvestigation", title = "BestTitle")
        arc.InitStudy("MyStudy").InitRegisteredAssay("MyAssay") |> ignore
        arc.InitWorkflow("MyWorkflow") |> ignore
        arc.InitRun("MyRun") |> ignore
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

        // Workflow folder
        Expect.exists contracts (fun c -> c.Path = "workflows/MyWorkflow/README.md") "workflow readme missing"
        Expect.exists contracts (fun c -> c.Path = "workflows/MyWorkflow/isa.workflow.xlsx") "workflow file missing"
        Expect.exists contracts (fun c -> c.Path = "workflows/MyWorkflow/isa.workflow.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Workflow) "workflow file exisiting but has wrong DTO type"

        // Run folder
        Expect.exists contracts (fun c -> c.Path = "runs/MyRun/README.md") "run readme missing"
        Expect.exists contracts (fun c -> c.Path = "runs/MyRun/isa.run.xlsx") "run file missing"
        Expect.exists contracts (fun c -> c.Path = "runs/MyRun/isa.run.xlsx" && c.DTOType.IsSome && c.DTOType.Value = Contract.DTOType.ISA_Run) "run file exisiting but has wrong DTO type"
    )
    testCase "assayWithDatamap" (fun _ ->
        let inv = ArcInvestigation("MyInvestigation", "BestTitle")
        let a = inv.InitAssay("MyAssay")
        let dm = DataMap.init()
        a.DataMap <- Some dm
        let arc = ARC.fromArcInvestigation(isa = inv)
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
        let arc = ARC.fromArcInvestigation(isa = inv)
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
        let arc = ARC.fromArcInvestigation(isa = inv)
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

    testCase "license" (fun _ ->
        let p = "LICENSE"
        let arc = ARC("MyARC")
        let licenseFullText = "This is my license"
        arc.SetLicenseFulltext (licenseFullText)
        let contracts = arc.GetWriteContracts()
        let licenseContract = contracts |> Array.tryFind (fun c -> c.Path = p)
        let actualContract = Expect.wantSome licenseContract "There should be a license contract"
        let expectedContract = Contract.createCreate(p, DTOType.PlainText, DTO.Text licenseFullText)
        Expect.equal actualContract expectedContract "should be equal"
    )
    testCase "license_altPath" (fun _ ->
        let p = "LICENSE.txt"
        let arc = ARC("MyARC")
        let licenseFullText = "This is my license"
        arc.SetLicenseFulltext (licenseFullText, path = p)
        let contracts = arc.GetWriteContracts()
        let licenseContract = contracts |> Array.tryFind (fun c -> c.Path = p)
        let actualContract = Expect.wantSome licenseContract "There should be a license contract"
        let expectedContract = Contract.createCreate(p, DTOType.PlainText, DTO.Text licenseFullText)
        Expect.equal actualContract expectedContract "should be equal"
    )
]

let private tests_updateContracts = testList "update_contracts" [
    testCase "empty" (fun _ ->
        let arc = ARC("MyIdentifier")
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
        let arc = ARC("MyInvestigation")
        arc.GetWriteContracts() |> ignore
        arc.InitAssay("MyAssay") |> ignore
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
        let arc = ARC("MyInvestigation")
        arc.GetWriteContracts() |> ignore
        arc.InitStudy("MyStudy") |> ignore
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
    testCase "empty_addWorkflow" (fun () ->
        let arc = ARC("MyInvestigation")
        arc.GetWriteContracts() |> ignore
        arc.InitWorkflow("MyWorkflow") |> ignore
        // As the ARC is compeletely empty, GetUpdateContracts should return the same contracts as GetWriteContracts
        let contracts = arc.GetUpdateContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 2 $"Should contain exactly as much contracts as needed for new workflow: {contractPathsString}"
        // Workflow file contract
        let workflowFilePath = "workflows/MyWorkflow/isa.workflow.xlsx"
        let workflowFileContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = workflowFilePath)) $"Workflow file contract for {workflowFilePath} missing"
        Expect.equal workflowFileContract.Operation Operation.CREATE "Operation for workflow file contract should be CREATE"
        let workflowFileDTOType = Expect.wantSome workflowFileContract.DTOType "DTOType for workflow file contract missing"
        Expect.equal workflowFileDTOType DTOType.ISA_Workflow "DTOType for workflow file contract should be ISA_Workflow"
        let workflowFileDTO = Expect.wantSome workflowFileContract.DTO "DTO for workflow file contract missing"
        let wb = workflowFileDTO.AsSpreadsheet() :?> FsWorkbook
        let workflow = ArcWorkflow.fromFsWorkbook wb
        Expect.equal workflow.Identifier "MyWorkflow" "Workflow identifier should be set"
        // readme contract
        let readmeFilePath = "workflows/MyWorkflow/README.md"
        let readmeContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = readmeFilePath)) $"Readme contract for {readmeFilePath} missing"
        Expect.equal readmeContract.Operation Operation.CREATE "Operation for readme contract should be CREATE"
        let readmeDTOType = Expect.wantSome readmeContract.DTOType "DTOType for readme contract missing"
        Expect.equal readmeDTOType DTOType.PlainText "DTOType for readme contract should be ISA_Readme"
        Expect.isNone readmeContract.DTO "DTO for readme contract should be None"
    )
    testCase "empty_changeWorkflow" (fun () ->
        let arc = ARC("MyInvestigation")
        arc.GetWriteContracts() |> ignore
        let workflow = arc.InitWorkflow("MyWorkflow")
        // As the ARC is compeletely empty, GetUpdateContracts should return the same contracts as GetWriteContracts
        let contracts = arc.GetUpdateContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.hasLength contracts 2 $"Should contain exactly as much contracts as needed for new workflow: {contractPathsString}"
        workflow.Description <- Some "MyDescription"
        let changeContracts = arc.GetUpdateContracts()
        Expect.hasLength changeContracts 1 "Should contain only one contract for the change"
        let expectedPath = "workflows/MyWorkflow/isa.workflow.xlsx"
        Expect.equal changeContracts.[0].Path expectedPath "Should be the workflow file"
        Expect.equal changeContracts.[0].Operation Operation.UPDATE "Operation for workflow file contract should be UPDATE"
        let workflowFileDTOType = Expect.wantSome changeContracts.[0].DTOType "DTOType for workflow file contract missing"
        Expect.equal workflowFileDTOType DTOType.ISA_Workflow "DTOType for workflow file contract should be ISA_Workflow"
    )
    testCase "empty_addRun" (fun () ->
        let arc = ARC("MyInvestigation")
        arc.GetWriteContracts() |> ignore
        arc.InitRun("MyRun") |> ignore
        // As the ARC is compeletely empty, GetUpdateContracts should return the same contracts as GetWriteContracts
        let contracts = arc.GetUpdateContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 2 $"Should contain exactly as much contracts as needed for new run: {contractPathsString}"
        // Run file contract
        let runFilePath = "runs/MyRun/isa.run.xlsx"
        let runFileContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = runFilePath)) $"Run file contract for {runFilePath} missing"
        Expect.equal runFileContract.Operation Operation.CREATE "Operation for run file contract should be CREATE"
        let runFileDTOType = Expect.wantSome runFileContract.DTOType "DTOType for run file contract missing"
        Expect.equal runFileDTOType DTOType.ISA_Run "DTOType for run file contract should be ISA_Run"
        let runFileDTO = Expect.wantSome runFileContract.DTO "DTO for run file contract missing"
        let wb = runFileDTO.AsSpreadsheet() :?> FsWorkbook
        let run = ArcRun.fromFsWorkbook wb
        Expect.equal run.Identifier "MyRun" "Run identifier should be set"
        // readme contract
        let readmeFilePath = "runs/MyRun/README.md"
        let readmeContract = Expect.wantSome (contracts |> Array.tryFind (fun c -> c.Path = readmeFilePath)) $"Readme contract for {readmeFilePath} missing"
        Expect.equal readmeContract.Operation Operation.CREATE "Operation for readme contract should be CREATE"
        let readmeDTOType = Expect.wantSome readmeContract.DTOType "DTOType for readme contract missing"
        Expect.equal readmeDTOType DTOType.PlainText "DTOType for readme contract should be ISA_Readme"
        Expect.isNone readmeContract.DTO "DTO for readme contract should be None"
    )
    testCase "empty_changeRun" (fun () ->
        let arc = ARC("MyInvestigation")
        arc.GetWriteContracts() |> ignore
        let run = arc.InitRun("MyRun")
        // As the ARC is compeletely empty, GetUpdateContracts should return the same contracts as GetWriteContracts
        let contracts = arc.GetUpdateContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.hasLength contracts 2 $"Should contain exactly as much contracts as needed for new run: {contractPathsString}"
        run.Description <- Some "MyDescription"
        let changeContracts = arc.GetUpdateContracts()
        Expect.hasLength changeContracts 1 "Should contain only one contract for the change"
        let expectedPath = "runs/MyRun/isa.run.xlsx"
        Expect.equal changeContracts.[0].Path expectedPath "Should be the run file"
        Expect.equal changeContracts.[0].Operation Operation.UPDATE "Operation for run file contract should be UPDATE"
        let runFileDTOType = Expect.wantSome changeContracts.[0].DTOType "DTOType for run file contract missing"
        Expect.equal runFileDTOType DTOType.ISA_Run "DTOType for run file contract should be ISA_Run"
    )
    testCase "full_CallingTwice" (fun _ ->
        let arc = ARC("MyInvestigation")
        arc.GetWriteContracts() |> ignore
        arc.InitStudy("MyStudy") |> ignore
        arc.InitAssay("MyAssay") |> ignore
        arc.InitWorkflow("MyWorkflow") |> ignore
        arc.InitRun("MyRun") |> ignore
        // As the ARC is compeletely empty, GetUpdateContracts should return the same contracts as GetWriteContracts
        let contracts = arc.GetUpdateContracts()
        Expect.isNotEmpty contracts "Should contain contracts"
        let secondContracts = arc.GetUpdateContracts()
        let contractsAsString =
            secondContracts
            |> Array.map (fun c -> c.Path)
            |> String.concat ", "
        Expect.isEmpty secondContracts $"Should contain no contracts as there are no changes, but returned contracts: \n {contractsAsString}"
    )
    testCase "init_simpleISA" (fun _ ->
        let arc = ARC("MyInvestigation", "BestTitle")
        arc.InitStudy("MyStudy").InitRegisteredAssay("MyAssay") |> ignore
        let contracts = arc.GetUpdateContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 13 $"Should contain more contracts as base folders but contained: {contractPathsString}"
    )
    testCase "simpleISA_NoChanges" (fun _ ->
        let arc = ARC("MyInvestigation")
        arc.SetISAFromContracts simpleISAContracts
        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 0 "Should contain no contracts as there are no changes"
    )
    testCase "simpleISA_AssayChange" (fun _ ->
        let arc = ARC("MyInvestigation")
        arc.SetISAFromContracts simpleISAContracts
        let testAssayIdentifier = TestObjects.Spreadsheet.Assay.Metabolome.assayIdentifier
        arc.GetAssay(testAssayIdentifier).InitTable("MyNewTestTable") |> ignore
        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 1 $"Should contain only assay change contract"
        let expectedPath = Identifier.Assay.fileNameFromIdentifier testAssayIdentifier
        Expect.equal contracts.[0].Path expectedPath "Should be the assay file"
        let nextContracts = arc.GetUpdateContracts()
        Expect.equal nextContracts.Length 0 "Should contain no contracts as there are no changes"
    )
    testCase "simpleISA_Datamap_NoChanges" (fun _ ->
        let arc = ARC("MyInvestigation")
        let readContracts = Array.append simpleISAContracts [|SimpleISA.Assay.proteomeDatamapContract|]
        arc.SetISAFromContracts readContracts
        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 0 "Should contain no contracts as there are no changes"
    )
    testCase "simpleISA_Datamap_Changed" (fun _ ->
        let arc = ARC("MyInvestigation")
        let readContracts = Array.append simpleISAContracts [|SimpleISA.Assay.proteomeDatamapContract|]
        arc.SetISAFromContracts readContracts

        let dm = Expect.wantSome (arc.GetAssay(SimpleISA.Assay.proteomeIdentifer).DataMap) "Assay should have datamap"       
        dm.GetDataContext(1).Name <- Some "Hello"

        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 1 $"Should contain only assay datamap change contract"
        let expectedPath = Identifier.Assay.datamapFileNameFromIdentifier SimpleISA.Assay.proteomeIdentifer
        Expect.equal contracts.[0].Path expectedPath "Should be the assay datamap file"
        let nextContracts = arc.GetUpdateContracts()
        Expect.equal nextContracts.Length 0 "Should contain no contracts as there are no changes"
    )
    testCase "simpleISA_StudyChange" (fun _ ->
        let arc = ARC("MyInvestigation")
        arc.SetISAFromContracts simpleISAContracts
        let testStudyIdentifier = TestObjects.Spreadsheet.Study.BII_S_1.studyIdentifier
        arc.GetStudy(testStudyIdentifier).InitTable("MyNewTestTable") |> ignore
        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 1 $"Should contain only study change contract"
        let expectedPath = Identifier.Study.fileNameFromIdentifier testStudyIdentifier
        Expect.equal contracts.[0].Path expectedPath "Should be the study file"
        let nextContracts = arc.GetUpdateContracts()
        Expect.equal nextContracts.Length 0 "Should contain no contracts as there are no changes"      
    )
    testCase "simpleISA_InvestigationChange" (fun _ ->
        let arc = ARC("MyInvestigation")
        arc.SetISAFromContracts simpleISAContracts
        arc.Title <- Some "NewTitle"
        let contracts = arc.GetUpdateContracts()
        Expect.equal contracts.Length 1 $"Should contain only investigation change contract"
        let expectedPath = "isa.investigation.xlsx"
        Expect.equal contracts.[0].Path expectedPath "Should be the investigation file"
        let nextContracts = arc.GetUpdateContracts()
        Expect.equal nextContracts.Length 0 "Should contain no contracts as there are no changes"   
    )
    testCase "license" (fun _ ->
        let p = "LICENSE"
        let initLicenseTxt = "This is my license"
        let nextLicenseTxt = "This is my new license"
        let arc = ARC("MyARC", license = License.initFulltext initLicenseTxt)
        arc.GetWriteContracts() |> ignore // to simulate that the license was written
        arc.SetLicenseFulltext (nextLicenseTxt)
        let contracts = arc.GetUpdateContracts()
        let licenseContract = contracts |> Array.tryFind (fun c -> c.Path = p)
        let actualContract = Expect.wantSome licenseContract "There should be a license contract"
        let expectedContract = Contract.createUpdate(p, DTOType.PlainText, DTO.Text nextLicenseTxt)
        Expect.equal actualContract expectedContract "should be equal"
    )
    testCase "license_altPath" (fun _ ->
        let p = "LICENSE.txt"
        let arc = ARC("MyARC")
        let initLicenseTxt = "This is my license"
        let nextLicenseTxt = "This is my new license"
        arc.SetLicenseFulltext (initLicenseTxt, path = p)
        arc.GetWriteContracts() |> ignore // to simulate that the license was written
        arc.SetLicenseFulltext (nextLicenseTxt, path = p)
        let contracts = arc.GetUpdateContracts()
        let licenseContract = contracts |> Array.tryFind (fun c -> c.Path = p)
        let actualContract = Expect.wantSome licenseContract "There should be a license contract"
        let expectedContract = Contract.createUpdate(p, DTOType.PlainText, DTO.Text nextLicenseTxt)
        Expect.equal actualContract expectedContract "should be equal"
    )
]


let private tests_updateFileSystem = testList "update_Filesystem" [
    testCase "empty noChanges" (fun () ->
        let arc = ARC("MyInvestigation")
        let oldFS = arc.FileSystem.Copy()
        arc.UpdateFileSystem()
        let newFS = arc.FileSystem
        Expect.equal oldFS.Tree newFS.Tree "Tree should be equal"
    )
    testCase "empty addInvestigationWithStudy" (fun () ->
        let arc = ARC("MyInvestigation")
        let oldFS = arc.FileSystem.Copy()
        let study = ArcStudy("MyStudy")
        arc.AddStudy(study)
        arc.UpdateFileSystem()
        let newFS = arc.FileSystem
        Expect.notEqual oldFS.Tree newFS.Tree "Tree should be unequal"
    )
    testCase "simple noChanges" (fun () ->
        let study = ArcStudy("MyStudy")
        let inv = ArcInvestigation("MyInvestigation")
        inv.AddStudy(study)
        let arc = ARC.fromArcInvestigation(isa = inv)
        let oldFS = arc.FileSystem.Copy()   
        arc.UpdateFileSystem()
        let newFS = arc.FileSystem
        Expect.equal oldFS.Tree newFS.Tree "Tree should be equal"
    )
    testCase "simple addAssayToStudy" (fun () ->
        let study = ArcStudy("MyStudy")
        let inv = ArcInvestigation("MyInvestigation")
        inv.AddStudy(study)
        let arc = ARC.fromArcInvestigation(isa = inv)
        let oldFS = arc.FileSystem.Copy()   
        let assay = ArcAssay("MyAssay")
        study.AddRegisteredAssay(assay)
        arc.UpdateFileSystem()
        let newFS = arc.FileSystem
        Expect.notEqual oldFS.Tree newFS.Tree "Tree should be unequal"
    )
    testCase "set ISA" <| fun () ->
        let arc = new ARC("My Investigation")
        let paths = arc.ToFilePaths() |> Seq.sort
        let expected_paths = [|"isa.investigation.xlsx"; "workflows/.gitkeep"; "runs/.gitkeep"; "assays/.gitkeep"; "studies/.gitkeep"|] |> Seq.sort
        Expect.sequenceEqual paths expected_paths "paths"
        let a = arc.InitAssay("My Assay")
        let paths2 = arc.ToFilePaths() |> Seq.sort
        let expected_paths2 =
            [|
            "isa.investigation.xlsx"; "workflows/.gitkeep"; "runs/.gitkeep";
            "assays/.gitkeep"; "assays/My Assay/isa.assay.xlsx";
            "assays/My Assay/README.md"; "assays/My Assay/dataset/.gitkeep";
            "assays/My Assay/protocols/.gitkeep"; "studies/.gitkeep"
            |]
            |> Seq.sort
        Expect.sequenceEqual paths2 expected_paths2 "paths2"
    testCase "setFileSystem" <| fun () ->
        let initial_paths = [|"isa.investigation.xlsx"; "workflows/.gitkeep"; "runs/.gitkeep"; "assays/.gitkeep"; "studies/.gitkeep"|] 
        let updated_paths = [|"isa.investigation.xlsx"; "workflows/.gitkeep"; "runs/.gitkeep"; "assays/.gitkeep"; "studies/.gitkeep"; "studies/testFile.txt"|]
        let arc = ARC.fromFilePaths(initial_paths)
        let paths = arc.ToFilePaths() |> Seq.sort
        Expect.sequenceEqual paths (initial_paths |> Seq.sort) "paths"
        arc.SetFilePaths(updated_paths)
        let paths2 = arc.ToFilePaths() |> Seq.sort
        Expect.sequenceEqual paths2 (updated_paths |> Seq.sort) "paths2"        
]

open ARCtrl.FileSystem

let private ``payload_file_filters`` = 
    
    let orderFST (fs : FileSystemTree) = 
        fs
        |> FileSystemTree.toFilePaths()
        |> Array.sort
        |> FileSystemTree.fromFilePaths

    testList "payload file filters" [
        let arc = ARC("MyInvestigation", "BestTitle")

        let assay = ArcAssay("registered_assay")
        let assayTable = assay.InitTable("MyAssayTable")
        assayTable.AppendColumn(CompositeHeader.Input (IOType.Data), ResizeArray [|CompositeCell.createFreeText "registered_assay_input.txt"|])
        assayTable.AppendColumn(CompositeHeader.ProtocolREF, ResizeArray [|CompositeCell.createFreeText "assay_protocol.rtf"|])
        assayTable.AppendColumn(CompositeHeader.Output (IOType.Data), ResizeArray [|CompositeCell.createFreeText "registered_assay_output.txt"|])

        let study = ArcStudy("registered_study")
        arc.AddRegisteredStudy(study)
        let studyTable = study.InitTable("MyStudyTable")
        studyTable.AppendColumn(CompositeHeader.Input (IOType.Sample), ResizeArray [|CompositeCell.createFreeText "some_study_input_material"|])
        studyTable.AppendColumn(CompositeHeader.FreeText "Some File", ResizeArray [|CompositeCell.createFreeText "xd/some_file_that_lies_in_slashxd.txt"|])
        studyTable.AppendColumn(CompositeHeader.ProtocolREF, ResizeArray [|CompositeCell.createFreeText "study_protocol.pdf"|])
        studyTable.AppendColumn(CompositeHeader.Output (IOType.Data), ResizeArray [|CompositeCell.createFreeText "registered_study_output.txt"|])
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
       
        arc.FileSystem <- FileSystem.create(fs)

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
        let arc = ARC("My Investigation")
        let assayIdentifier = "My Assay"
        arc.InitAssay(assayIdentifier) |> ignore
        Expect.equal arc.AssayCount 1 "ensure assay count"
        arc.GetWriteContracts() |> ignore
        let actual = arc.GetAssayRemoveContracts(assayIdentifier)
        let expected = [
            Contract.createDelete (ArcPathHelper.getAssayFolderPath assayIdentifier)
            arc.ToUpdateContract()
        ]
        Expect.sequenceEqual actual expected "we do not have correct FsWorkbook equality helper functions"
    testCase "not registered" <| fun _ ->
        let arc = ARC("My Investigation")
        let assayIdentifier = "My Assay"
        arc.InitAssay(assayIdentifier) |> ignore
        Expect.equal arc.AssayCount 1 "ensure assay count"
        arc.GetWriteContracts() |> ignore
        let actual = arc.GetAssayRemoveContracts(assayIdentifier)
        Expect.hasLength actual 1 "contract count"
        Expect.equal actual.[0].Path (ArcPathHelper.getAssayFolderPath assayIdentifier) "assay contract path"
        Expect.equal actual.[0].Operation DELETE "assay contract cmd"
    testCase "registered in multiple studies" <| fun _ ->
        let arc = ARC("My Investigation")
        let assayIdentifier = "My Assay"
        let s1 = arc.InitStudy("Study 1")
        let s2 = arc.InitStudy("Study 2")
        let a = arc.InitAssay(assayIdentifier)
        s1.RegisterAssay(assayIdentifier)
        s2.RegisterAssay(assayIdentifier)
        Expect.equal arc.AssayCount 1 "ensure assay count"
        Expect.equal arc.StudyCount 2 "ensure study count"
        Expect.hasLength a.StudiesRegisteredIn 2 "ensure studies registered in - count"
        arc.GetWriteContracts() |> ignore
        let actual = arc.GetAssayRemoveContracts(assayIdentifier)
        Expect.hasLength actual 3 "contract count"
        Expect.equal actual.[0].Path (ArcPathHelper.getAssayFolderPath assayIdentifier) "assay contract path"
        Expect.equal actual.[0].Operation DELETE "assay contract cmd"
        Expect.equal actual.[1].Path (Identifier.Study.fileNameFromIdentifier "Study 1") "study 1 contract path"
        Expect.equal actual.[1].Operation UPDATE "study 1 contract cmd"
        Expect.equal actual.[2].Path (Identifier.Study.fileNameFromIdentifier "Study 2") "study 2 contract path"
        Expect.equal actual.[2].Operation UPDATE "study 2 contract cmd"
]

let tests_GetAssayRenameContracts = testList "GetAssayRenameContracts" [
    testCase "not existing" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitAssay("OtherAssayName") |> ignore
        let arc = ARC.fromArcInvestigation(isa = i)
        
        let assayMoveF = 
            fun () -> arc.GetAssayRenameContracts("MyOldAssay","MyNewAssay") |> ignore

        Expect.throws assayMoveF "Should fail as arc does not contan assay with given name"
    testCase "Basic" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitAssay("MyOldAssay") |> ignore
        let arc = ARC.fromArcInvestigation(isa = i)
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
        let arc = ARC.fromArcInvestigation(isa = i)
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
        let arc = ARC.fromArcInvestigation(isa = i)
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
        let arc = ARC("MyInvestigation")
        arc.InitStudy("OtherStudyName") |> ignore
        
        let studyMoveF = 
            fun () -> arc.GetStudyRenameContracts("MyOldStudy","MyNewStudy") |> ignore

        Expect.throws studyMoveF "Should fail as arc does not contan study with given name"
    testCase "Basic" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitStudy("MyOldStudy") |> ignore
        let arc = ARC.fromArcInvestigation(isa = i)
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
        let arc = ARC.fromArcInvestigation(isa = i)
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

let tests_GetWorkflowRemoveContracts = testList "GetWorkflowRemoveContracts" [
    testCase "not existing" <| fun _ ->
        let arc = ARC("My Investigation")        
        let workflowRemoveF = 
            fun () -> arc.GetWorkflowRemoveContracts("MyWorkflow") |> ignore
        Expect.throws workflowRemoveF "Should fail as arc does not contan workflow with given name"
    testCase "Basic" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitWorkflow("MyWorkflow") |> ignore
        let arc = ARC.fromArcInvestigation(isa = i)
        Expect.equal arc.WorkflowCount 1 "ensure workflow count"
        arc.GetWriteContracts() |> ignore
        let actual = arc.GetWorkflowRemoveContracts("MyWorkflow")
        let expected = [
            Contract.createDelete (ArcPathHelper.getWorkflowFolderPath "MyWorkflow")
        ]
        Expect.sequenceEqual actual expected "Should contain exactly the delete contract"
    testCase "RegisteredInRun" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        let w = i.InitWorkflow("MyWorkflow")
        let r = i.InitRun("MyRun")
        r.WorkflowIdentifiers.Add(w.Identifier)
        Expect.isTrue (r.WorkflowIdentifiers.Contains("MyWorkflow")) "ensure run references workflow"
        let arc = ARC.fromArcInvestigation(isa = i)
        Expect.equal arc.WorkflowCount 1 "ensure workflow count"
        arc.GetWriteContracts() |> ignore
        let actual = arc.GetWorkflowRemoveContracts("MyWorkflow")
        Expect.isFalse (r.WorkflowIdentifiers.Contains("MyWorkflow")) "After getting remove contracts, run should no longer reference workflow"
        Expect.hasLength actual 2 "Should contain delete and run update contracts"
        let delete = Expect.wantSome (actual |> Array.tryFind (fun c -> c.Operation = DELETE)) "Should contain delete contract"
        Expect.equal delete.Path (ArcPathHelper.getWorkflowFolderPath "MyWorkflow") "Delete contract path"
        let update = Expect.wantSome (actual |> Array.tryFind (fun c -> c.Operation = UPDATE)) "Should contain run update contract"
        Expect.equal update.Path (Identifier.Run.fileNameFromIdentifier "MyRun") "Run update contract path"
        let updateDTO = Expect.wantSome update.DTO "Run update contract should have DTO"
        let parsedRun = XlsxController.Run.fromFsWorkbook (updateDTO.AsSpreadsheet() :?> FsWorkbook)
        Expect.isFalse (parsedRun.WorkflowIdentifiers.Contains("MyWorkflow")) "Parsed Run should no longer reference the removed workflow"
    testCase "RegisteredInWorkflow" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        let w1 = i.InitWorkflow("MySubWorkflow")
        let w2 = i.InitWorkflow("MyWorkflow")
        w2.SubWorkflowIdentifiers.Add("MySubWorkflow")
        Expect.isTrue (w2.SubWorkflowIdentifiers.Contains("MySubWorkflow")) "ensure workflow references sub-workflow"
        let arc = ARC.fromArcInvestigation(isa = i)
        Expect.equal arc.WorkflowCount 2 "ensure workflow count"
        arc.GetWriteContracts() |> ignore
        let actual = arc.GetWorkflowRemoveContracts("MySubWorkflow")
        Expect.isFalse (w2.SubWorkflowIdentifiers.Contains("MySubWorkflow")) "Workflow should no longer reference the removed sub-workflow"
        Expect.hasLength actual 2 "Should contain delete and workflow update contracts"
        let delete = Expect.wantSome (actual |> Array.tryFind (fun c -> c.Operation = DELETE)) "Should contain delete contract"
        Expect.equal delete.Path (ArcPathHelper.getWorkflowFolderPath "MySubWorkflow") "Delete contract path"
        let update = Expect.wantSome (actual |> Array.tryFind (fun c -> c.Operation = UPDATE)) "Should contain workflow update contract"
        Expect.equal update.Path (Identifier.Workflow.fileNameFromIdentifier "MyWorkflow") "Workflow update contract path"
        let updateDTO = Expect.wantSome update.DTO "Workflow update contract should have DTO"
        let parsedWorkflow = XlsxController.Workflow.fromFsWorkbook (updateDTO.AsSpreadsheet() :?> FsWorkbook)
        Expect.isFalse (parsedWorkflow.SubWorkflowIdentifiers.Contains("MySubWorkflow")) "Parsed Workflow should no longer reference the removed sub-workflow"
    ]

let tests_GetRunRemoveContracts = testList "GetRunRemoveContracts" [
    testCase "not existing" <| fun _ ->
        let arc = ARC("My Investigation")        
        let runRemoveF = 
            fun () -> arc.GetRunRemoveContracts("MyRun") |> ignore
        Expect.throws runRemoveF "Should fail as arc does not contan run with given name"
    testCase "Basic" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitRun("MyRun") |> ignore
        let arc = ARC.fromArcInvestigation(isa = i)
        Expect.equal arc.RunCount 1 "ensure run count"
        arc.GetWriteContracts() |> ignore
        let actual = arc.GetRunRemoveContracts("MyRun")
        let expected = [
            Contract.createDelete (ArcPathHelper.getRunFolderPath "MyRun")
        ]
        Expect.sequenceEqual actual expected "Should contain exactly the delete contract"
    ]

let tests_GetWorkflowRenameContracts = testList "GetWorkflowRenameContracts" [
    testCase "not existing" <| fun _ ->
        let arc = ARC("My Investigation")        
        let workflowMoveF = 
            fun () -> arc.GetWorkflowRenameContracts("MyOldWorkflow","MyNewWorkflow") |> ignore
        Expect.throws workflowMoveF "Should fail as arc does not contan workflow with given name"
    testCase "Basic" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitWorkflow("MyOldWorkflow") |> ignore
        let arc = ARC.fromArcInvestigation(isa = i)
        arc.GetWriteContracts() |> ignore
        let contracts = arc.GetWorkflowRenameContracts("MyOldWorkflow","MyNewWorkflow")
        Expect.hasLength contracts 2 "Contract count is wrong"
        // Rename contract
        let renameContract = contracts.[0]
        Expect.equal renameContract.Operation Operation.RENAME "Rename contract operation"
        Expect.equal "workflows/MyOldWorkflow" renameContract.Path "Rename contract path"
        let renameDTO = Expect.wantSome renameContract.DTO "Rename contract dto"
        let expectedRenameDTO = DTO.Text "workflows/MyNewWorkflow"
        Expect.equal renameDTO expectedRenameDTO "Rename contract dto"
        // Update workflow contract
        let updateContract = contracts.[1]
        Expect.equal updateContract.Operation Operation.UPDATE "Update contract operation"
        Expect.equal updateContract.Path "workflows/MyNewWorkflow/isa.workflow.xlsx" "Update contract path"
        let updateDTO = Expect.wantSome updateContract.DTO "Update contract dto"
        Expect.isTrue updateDTO.isSpreadsheet "Update contract dto"
        let wb = updateDTO.AsSpreadsheet() :?> FsWorkbook
        let updatedWorkflow = ArcWorkflow.fromFsWorkbook wb
        Expect.equal updatedWorkflow.Identifier "MyNewWorkflow" "Update contract Workflow Identifier"
    testCase "RegisteredInRun" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        let w = i.InitWorkflow("MyOldWorkflow")
        let r = i.InitRun("MyRun")
        r.WorkflowIdentifiers.Add(w.Identifier)
        Expect.isTrue (r.WorkflowIdentifiers.Contains("MyOldWorkflow")) "ensure run references workflow"
        let arc = ARC.fromArcInvestigation(isa = i)
        arc.GetWriteContracts() |> ignore
        let contracts = arc.GetWorkflowRenameContracts("MyOldWorkflow","MyNewWorkflow")
        Expect.sequenceEqual r.WorkflowIdentifiers ["MyNewWorkflow"] "After getting rename contracts, run should reference new workflow name"
        Expect.hasLength contracts 3 "Contract count is wrong"
        // Update run contract
        let runUpdateContract = contracts.[2]
        Expect.equal runUpdateContract.Operation Operation.UPDATE "Update contract operation"
        Expect.equal runUpdateContract.Path "runs/MyRun/isa.run.xlsx" "Update contract path"
        let runUpdateDTO = Expect.wantSome runUpdateContract.DTO "Update contract dto"
        Expect.isTrue runUpdateDTO.isSpreadsheet "Update contract dto"
        let wb = runUpdateDTO.AsSpreadsheet() :?> FsWorkbook
        let updatedRun = ArcRun.fromFsWorkbook wb
        Expect.equal updatedRun.Identifier "MyRun" "Update contract Run Identifier"
        Expect.hasLength updatedRun.WorkflowIdentifiers 1 "Update contract Run Workflow count"
        Expect.equal updatedRun.WorkflowIdentifiers.[0] "MyNewWorkflow" "Update contract Run Workflow Identifier"
    testCase "RegisteredInWorkflow" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        let w1 = i.InitWorkflow("MyOldSubWorkflow")
        let w2 = i.InitWorkflow("MyWorkflow")
        w2.SubWorkflowIdentifiers.Add("MyOldSubWorkflow")
        Expect.isTrue (w2.SubWorkflowIdentifiers.Contains("MyOldSubWorkflow")) "ensure workflow references sub-workflow"
        let arc = ARC.fromArcInvestigation(isa = i)
        arc.GetWriteContracts() |> ignore
        let contracts = arc.GetWorkflowRenameContracts("MyOldSubWorkflow","MyNewSubWorkflow")
        Expect.sequenceEqual w2.SubWorkflowIdentifiers ["MyNewSubWorkflow"] "After getting rename contracts, workflow should still reference sub-workflow"
        Expect.hasLength contracts 3 "Contract count is wrong"
        // Update workflow contract
        let workflowUpdateContract = contracts.[2]
        Expect.equal workflowUpdateContract.Operation Operation.UPDATE "Update contract operation"
        Expect.equal workflowUpdateContract.Path "workflows/MyWorkflow/isa.workflow.xlsx" "Update contract path"
        let workflowUpdateDTO = Expect.wantSome workflowUpdateContract.DTO "Update contract dto"
        Expect.isTrue workflowUpdateDTO.isSpreadsheet "Update contract dto"
        let wb = workflowUpdateDTO.AsSpreadsheet() :?> FsWorkbook
        let updatedWorkflow = ArcWorkflow.fromFsWorkbook wb
        Expect.equal updatedWorkflow.Identifier "MyWorkflow" "Update contract Workflow Identifier"
        Expect.hasLength updatedWorkflow.SubWorkflowIdentifiers 1 "Updated contract still has Sub workflow"
        Expect.equal updatedWorkflow.SubWorkflowIdentifiers.[0] "MyNewSubWorkflow" "Updated contract Sub workflow Identifier"
]

let tests_GetRunRenameContracts = testList "GetRunRenameContracts" [
    testCase "not existing" <| fun _ ->
        let arc = ARC("My Investigation")        
        let runMoveF = 
            fun () -> arc.GetRunRenameContracts("MyOldRun","MyNewRun") |> ignore
        Expect.throws runMoveF "Should fail as arc does not contan run with given name"
    testCase "Basic" <| fun _ ->
        let i = ArcInvestigation("MyInvestigation")
        i.InitRun("MyOldRun") |> ignore
        let arc = ARC.fromArcInvestigation(isa = i)
        arc.GetWriteContracts() |> ignore
        let contracts = arc.GetRunRenameContracts("MyOldRun","MyNewRun")
        Expect.hasLength contracts 2 "Contract count is wrong"
        // Rename contract
        let renameContract = contracts.[0]
        Expect.equal renameContract.Operation Operation.RENAME "Rename contract operation"
        Expect.equal "runs/MyOldRun" renameContract.Path "Rename contract path"
        let renameDTO = Expect.wantSome renameContract.DTO "Rename contract dto"
        let expectedRenameDTO = DTO.Text "runs/MyNewRun"
        Expect.equal renameDTO expectedRenameDTO "Rename contract dto"
        // Update run contract
        let updateContract = contracts.[1]
        Expect.equal updateContract.Operation Operation.UPDATE "Update contract operation"
        Expect.equal updateContract.Path "runs/MyNewRun/isa.run.xlsx" "Update contract path"
        let updateDTO = Expect.wantSome updateContract.DTO "Update contract dto"
        Expect.isTrue updateDTO.isSpreadsheet "Update contract dto"
        let wb = updateDTO.AsSpreadsheet() :?> FsWorkbook
        let updatedRun = ArcRun.fromFsWorkbook wb
        Expect.equal updatedRun.Identifier "MyNewRun" "Update contract Run Identifier"
]

let tests_load =

    testList "Load" [
        testCaseCrossAsync "simpleARC" (crossAsync {
            let p = TestObjects.IO.testSimpleARC
            let! result = ARC.tryLoadAsync(p)
            let result = Expect.wantOk result "ARC should load successfully"

            //Expect.isNone result.CWL "Should not contain a CWL part"

            Expect.equal result.StudyCount 1 "Should contain 1 study"
            Expect.equal result.AssayCount 1 "Should contain 1 assay"
            Expect.equal result.RegisteredStudies.Count 1 "Should contain 1 registered study"
            
            let s = result.Studies.[0]
            Expect.equal s.RegisteredAssayCount 1 "Should contain 1 registered assay"
            Expect.equal s.TableCount 3 "Study should contain 3 tables"

            let a = s.RegisteredAssays.[0]
            Expect.equal a.TableCount 4 "Assay should contain 4 tables"
            }
            
        )
        testCaseCrossAsync "simpleARCWithWR" (crossAsync {
            let p = TestObjects.IO.testSimpleARCWithWR
            let! result = ARC.tryLoadAsync(p)
            let result = Expect.wantOk result "ARC should load successfully"

            //Expect.isNone result.CWL "Should not contain a CWL part"

            Expect.equal result.StudyCount 1 "Should contain 1 study"
            Expect.equal result.AssayCount 1 "Should contain 1 assay"
            Expect.equal result.WorkflowCount 1 "Should contain 1 workflow"
            Expect.equal result.RunCount 1 "Should contain 1 run"
            Expect.equal result.RegisteredStudies.Count 1 "Should contain 1 registered study"
            
            let s = result.Studies.[0]
            Expect.equal s.RegisteredAssayCount 1 "Should contain 1 registered assay"
            Expect.equal s.TableCount 3 "Study should contain 3 tables"

            let a = s.RegisteredAssays.[0]
            Expect.equal a.TableCount 4 "Assay should contain 4 tables"

            let w = result.Workflows.[0]
            Expect.equal w.Identifier "Proteomics" "Workflow identifer does not match"
            Expect.equal w.Version (Some "0.1.2") "Workflow version does not match"
            Expect.hasLength w.Contacts 3 "Workflow should have 3 contacts"

            let r = result.Runs.[0]
            Expect.equal r.Identifier "Proteomics" "Run identifer does not match"
            Expect.hasLength r.Performers 2 "Run should have 2 performers"
            }          
        )
    ]


let tests_write =

    testList "Write" [
        testCaseCrossAsync "empty" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_Write_Empty"
            let a = ARC("MyInvestigation")

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
            let arc = ARC("MyInvestigation")

            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            arc.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
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
        testCaseCrossAsync "SimpleARCWithWR" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_Write_SimpleARCWithWR"
            let arc = ARC("MyInvestigation")

            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            arc.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
            let workflowName = "MyWorkflow"
            let w = arc.InitWorkflow(workflowName)
            let runName = "MyRun"
            let r = arc.InitRun(runName)

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
                    $"/runs/{runName}/isa.run.xlsx"
                    $"/runs/{runName}/README.md"
                    "/workflows/.gitkeep"
                    $"/workflows/{workflowName}/isa.workflow.xlsx"
                    $"/workflows/{workflowName}/README.md"
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

            let existingStudyName = "experiment1_material"
            let existingAssayName = "measurement1"

            let assayName = "YourAssay"
            arc.InitAssay(assayName) |> ignore

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
            let arc = ARC("MyInvestigation")

            // setup arc
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            arc.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
            arc.UpdateFileSystem()

            let! writeResult = arc.TryWriteAsync(p)

            Expect.wantOk writeResult "ARC should write successfully" |> ignore

            // add assay
            let newAssayName = "MyNewAssay"
            arc.InitAssay(newAssayName) |> ignore
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
            do! FileSystemHelper.deleteFileOrDirectoryAsync p
            let arc = ARC("MyInvestigation")

            // setup arc
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            arc.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
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
            do! FileSystemHelper.deleteFileOrDirectoryAsync p
            let arc = ARC("MyInvestigation")

            // setup arc
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            arc.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)            
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
            let arc = ARC("MyInvestigation")

            // setup arc
            let i = ArcInvestigation("MyInvestigation")
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            arc.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
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
            let arc = ARC("MyInvestigation")

            // setup arc
            let studyName = "MyStudy"
            let s = ArcStudy(studyName)
            arc.AddRegisteredStudy(s)
            let assayName = "MyAssay"
            let a = ArcAssay(assayName)
            s.AddRegisteredAssay(a)
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
        testList "Roundtrip" [
            testCase "License" <| fun _ ->
                let arcExpected = ARC("MyARC", title = "MyTitle", description = "MyDescription", license = License.initFulltext "CC-BY-4.0")
                let json = arcExpected.ToROCrateJsonString()
                let arcActual = ARC.fromROCrateJsonString(json)
                Expect.equal arcActual.Identifier arcExpected.Identifier "Identifier should be equal"
                Expect.equal arcActual.Title arcExpected.Title "Title should be equal"
                Expect.equal arcActual.Description arcExpected.Description "Description should be equal"
                Expect.equal arcActual.License arcExpected.License "License should be equal"
            testCase "License_DifferentPath" <| fun _ ->
                let license = License(contentType = LicenseContentType.Fulltext, content = "CC-BY-4.0", path = "LICENSE.txt")
                let arcExpected = ARC("MyARC", title = "MyTitle", description = "MyDescription", license = license)
                let json = arcExpected.ToROCrateJsonString()
                let arcActual = ARC.fromROCrateJsonString(json)
                Expect.equal arcActual.Identifier arcExpected.Identifier "Identifier should be equal"
                Expect.equal arcActual.Title arcExpected.Title "Title should be equal"
                Expect.equal arcActual.Description arcExpected.Description "Description should be equal"
                Expect.equal arcActual.License arcExpected.License "License should be equal"
            testCase "NoLicense" <| fun _ ->
                let arcExpected = ARC("MyARC", title = "MyTitle", description = "MyDescription")
                let json = arcExpected.ToROCrateJsonString()
                let arcActual = ARC.fromROCrateJsonString(json)
                Expect.equal arcActual.Identifier arcExpected.Identifier "Identifier should be equal"
                Expect.equal arcActual.Title arcExpected.Title "Title should be equal"
                Expect.equal arcActual.Description arcExpected.Description "Description should be equal"
                Expect.isNone arcActual.License "License should be None"
        ]
                
        testCase "CanRead_Deprecated" <| fun _ ->
            let arc = ARC.fromDeprecatedROCrateJsonString(TestObjects.ROCrate.ArcPrototypeDeprecated.ed123499)
            let nonDeprecatedARC = ARC.fromROCrateJsonString(TestObjects.ROCrate.ArcPrototype.ed123499)
            Expect.equal arc.Identifier nonDeprecatedARC.Identifier "Investigation should have correct identifier"
            Expect.equal arc.Title nonDeprecatedARC.Title "Investigation should have correct title"
            Expect.equal arc.Description nonDeprecatedARC.Description "Investigation should have correct description"
            Expect.equal arc.Contacts.[1] nonDeprecatedARC.Contacts.[1] "Investigation should have correct contacts"
            Expect.equal arc.Studies.[1] nonDeprecatedARC.Studies.[1] "Investigation should have correct studies"
        testCase "CanRead" <| fun _ ->
            let arc = ARC.fromROCrateJsonString(TestObjects.ROCrate.ArcPrototype.ed123499)
            Expect.equal arc.Identifier "ArcPrototype" "Investigation should have correct identifier"
            let title = Expect.wantSome arc.Title "Investigation should have title"
            Expect.equal title "ArcPrototype" "Investigation should have correct title"
            let description = Expect.wantSome arc.Description "Investigation should have description"
            Expect.equal description "A prototypic ARC that implements all specification standards accordingly" "Investigation should have correct description"
            /// Contacts
            Expect.hasLength arc.Contacts 3 "Investigation should have 3 contacts"
            let first = arc.Contacts.[0]
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
            Expect.equal arc.StudyCount 2 "ARC should contain 2 studies"
            let secondStudy = arc.GetStudy("MaterialPreparation")
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
            // Assays
            Expect.equal arc.AssayCount 2 "ARC should contain 2 assays"
        testCase "IncludeFilesystem" <| fun _ ->
            let arc = ARC("MyARC", title = "MyTitle", description = "MyDescription")
            let assay = arc.InitAssay("MyAssay")
            arc.UpdateFileSystem()
            let fs = 
                arc.FileSystem.AddFile("assays/MyAssay/dataset/MyData.csv")
                    .AddFile("assays/MyAssay/dataset/ABC.D/SubFile.txt")
                    .AddFile("assays/MyAssay/dataset/ABC.D/SubFolder/SubSubFile.txt")
            arc.FileSystem <- fs
            let table = assay.InitTable("MyTable")
            table.AddColumn(
                CompositeHeader.Input IOType.Data,
                ResizeArray [
                    CompositeCell.createDataFromString("assays/MyAssay/dataset/MyData.csv")
                    CompositeCell.createDataFromString("assays/MyAssay/dataset/ABC.D");
                ]                
            ) |> ignore
            let roCrate = arc.ToROCrateJsonString()
            let arc' = ARC.fromROCrateJsonString(roCrate)
            Expect.testFileSystemTree arc'.FileSystem.Tree arc.FileSystem.Tree 
            Expect.equal arc'.Assays[0] arc.Assays[0] "Assays should be equal"
                    
        
    ]



let main = testList "ARCtrl" [
    tests_create
    tests_fromFilePaths
    tests_updateFileSystem
    tests_read_contracts
    tests_SetISAFromContracts
    tests_writeContracts
    tests_updateContracts
    tests_GetAssayRemoveContracts
    tests_GetAssayRenameContracts
    tests_GetStudyRenameContracts
    tests_GetWorkflowRemoveContracts
    tests_GetWorkflowRenameContracts
    tests_GetRunRemoveContracts
    tests_GetRunRenameContracts
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


