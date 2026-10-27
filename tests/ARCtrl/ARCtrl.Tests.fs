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

let private simpleISAWithWRContracts_WithCWL = 
    [|
        SimpleISA.Investigation.investigationReadContract
        SimpleISA.Study.bII_S_1ReadContract
        SimpleISA.Assay.proteomeReadContract
        SimpleISA.Workflow.proteomicsReadContract
        SimpleISA.Workflow.proteomicsCWLContract
        SimpleISA.Run.proteomicsReadContract
        SimpleISA.Run.proteomicsCWLContract
    |]

let private tests_read_contracts = testList "read_contracts" [
    testCase "simpleISAWithWR_NoCWL" (fun () -> 
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
    testCase "simpleISAWithWR_WithCWL" (fun () -> 
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
            @"workflows\TestWorkflow\workflow.cwl";
            @"workflows\TestWorkflow\README.md";
            // run
            @"runs\TestRun\isa.run.xlsx";
            @"runs\TestRun\run.cwl";
            @"runs\TestRun\README.md";
            |]
            |> Array.map (fun x -> x.Replace(@"\","/"))
            |> Array.sort
        let arc = ARC.fromFilePaths(input)
        let contracts = arc.GetReadContracts()
        Expect.hasLength contracts 7 "should have read 7 read contracts"

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

        let workflowCWLContractOpt = contracts |> Array.tryFind (fun c -> c.Path = "workflows/TestWorkflow/workflow.cwl")
        let workflowCWLContract = Expect.wantSome workflowCWLContractOpt "workflow cwl contract should be present"
        Expect.equal workflowCWLContract.Operation Operation.READ "workflow cwl contract should be a read contract"
        Expect.equal workflowCWLContract.DTOType (Some DTOType.CWL) "workflow cwl contract should have the correct DTO type"

        let runContractOpt = contracts |> Array.tryFind (fun c -> c.Path = "runs/TestRun/isa.run.xlsx")
        let runContract = Expect.wantSome runContractOpt "run contract should be present"
        Expect.equal runContract.Operation Operation.READ "run contract should be a read contract"
        Expect.equal runContract.DTOType (Some DTOType.ISA_Run) "run contract should have the correct DTO type"

        let runCWLContractOpt = contracts |> Array.tryFind (fun c -> c.Path = "runs/TestRun/run.cwl")
        let runCWLContract = Expect.wantSome runCWLContractOpt "run cwl contract should be present"
        Expect.equal runCWLContract.Operation Operation.READ "run cwl contract should be a read contract"
        Expect.equal runCWLContract.DTOType (Some DTOType.CWL) "run cwl contract should have the correct DTO type"
    )
    ]

/// The text that the write contracts of the ARC hold for a file.
let private writtenCwlText (arc: ARC) (path: string) : string =
    let contract = arc.GetWriteContracts() |> Array.find (fun c -> c.Path = path)
    match contract.DTO with
    | Some (DTO.Text text) -> text
    | other -> failwith $"Expected the text of {path} but got %A{other}"

/// An ARC that reads the given text as the CWL file of its run.
let private arcWithRunCwl (cwlText: string) : ARC =
    let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier Run.Proteomics.runIdentifier
    let contracts = [|
        SimpleISA.Investigation.investigationReadContract
        SimpleISA.Study.bII_S_1ReadContract
        SimpleISA.Assay.proteomeReadContract
        SimpleISA.Workflow.proteomicsReadContract
        SimpleISA.Run.proteomicsReadContract
        Contract.create(Operation.READ, path = runCwlPath, dtoType = DTOType.CWL, dto = DTO.Text cwlText)
    |]
    let arc = ARC("MyIdentifier")
    arc.SetISAFromContracts contracts
    arc

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
    testCase "simpleISAWithWR_NoCWL" (fun () -> 
        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts simpleISAWithWRContracts
        Expect.equal arc.Identifier Investigation.BII_I_1.investigationIdentifier "investigation identifier should have been read from investigation contract"

        Expect.equal arc.StudyCount 1 "should have read two studies"
        Expect.equal  arc.Studies.[0].Identifier Study.BII_S_1.studyIdentifier "Study Identifier"

        Expect.equal arc.AssayCount 1 "should have read one assay"
        Expect.equal arc.Assays.[0].Identifier Assay.Proteome.assayIdentifier "Assay Identifier"

        Expect.equal arc.WorkflowCount 1 "should have read one workflow"
        Expect.equal arc.Workflows.[0].Identifier Workflow.Proteomics.workflowIdentifier "Workflow Identifier"
        Expect.isNone arc.Workflows.[0].CWLDescription "Workflow CWL should not be present"

        Expect.equal arc.RunCount 1 "should have read one run"
        Expect.equal arc.Runs.[0].Identifier Run.Proteomics.runIdentifier "Run Identifier"
        Expect.isNone arc.Runs.[0].CWLDescription "Run CWL should not be present"
    )
    testCase "simpleISAWithWR_WithCWL" (fun () -> 
        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts simpleISAWithWRContracts_WithCWL
        Expect.equal arc.Identifier Investigation.BII_I_1.investigationIdentifier "investigation identifier should have been read from investigation contract"

        Expect.equal arc.StudyCount 1 "should have read two studies"
        Expect.equal  arc.Studies.[0].Identifier Study.BII_S_1.studyIdentifier "Study Identifier"

        Expect.equal arc.AssayCount 1 "should have read one assay"
        Expect.equal arc.Assays.[0].Identifier Assay.Proteome.assayIdentifier "Assay Identifier"

        Expect.equal arc.WorkflowCount 1 "should have read one workflow"
        Expect.equal arc.Workflows.[0].Identifier Workflow.Proteomics.workflowIdentifier "Workflow Identifier"
        Expect.isSome arc.Workflows.[0].CWLDescription "Workflow CWL should be present"

        Expect.equal arc.RunCount 1 "should have read one run"
        Expect.equal arc.Runs.[0].Identifier Run.Proteomics.runIdentifier "Run Identifier"
        Expect.isSome arc.Runs.[0].CWLDescription "Run CWL should be present"
    )
    testCase "simpleISAWithWR_WithOperationCWL_LoadsOperationDescriptions" (fun () ->
        let workflowIdentifier = Workflow.Proteomics.workflowIdentifier
        let runIdentifier = Run.Proteomics.runIdentifier
        let workflowCwlPath = Identifier.Workflow.cwlFileNameFromIdentifier workflowIdentifier
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier runIdentifier
        let operationCwlText = TestObjects.CWL.Operation.minimalOperationFile

        let contracts = [|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Workflow.proteomicsReadContract
            Contract.create(Operation.READ, path = workflowCwlPath, dtoType = DTOType.CWL, dto = DTO.Text operationCwlText)
            SimpleISA.Run.proteomicsReadContract
            Contract.create(Operation.READ, path = runCwlPath, dtoType = DTOType.CWL, dto = DTO.Text operationCwlText)
        |]

        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts contracts

        let workflowDescription = Expect.wantSome arc.Workflows.[0].CWLDescription "Workflow CWL should be present"
        let runDescription = Expect.wantSome arc.Runs.[0].CWLDescription "Run CWL should be present"

        Expect.isTrue
            (match workflowDescription with | ARCtrl.CWL.Operation _ -> true | _ -> false)
            (sprintf "Expected workflow CWL description to be Operation but got %A" workflowDescription)
        Expect.isTrue
            (match runDescription with | ARCtrl.CWL.Operation _ -> true | _ -> false)
            (sprintf "Expected run CWL description to be Operation but got %A" runDescription)
    )
    testCase "simpleISAWithWR_WithCWL_ResolvesRunWorkflowReference" (fun () ->
        let workflowIdentifier = Workflow.Proteomics.workflowIdentifier
        let runIdentifier = Run.Proteomics.runIdentifier
        let workflowCwlPath = Identifier.Workflow.cwlFileNameFromIdentifier workflowIdentifier
        let workflowToolPath = ArcPathHelper.combineMany [|ArcPathHelper.WorkflowsFolderName; workflowIdentifier; "tool.cwl"|]
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier runIdentifier

        let workflowCwlText = TestObjects.CWL.RunWorkflowReference.workflowCwlText
        let workflowToolText = TestObjects.CWL.RunWorkflowReference.workflowToolText
        let runCwlText = TestObjects.CWL.RunWorkflowReference.runCwlText

        let contracts = [|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Workflow.proteomicsReadContract
            Contract.create(Operation.READ, path = workflowCwlPath, dtoType = DTOType.CWL, dto = DTO.Text workflowCwlText)
            Contract.create(Operation.READ, path = workflowToolPath, dtoType = DTOType.CWL, dto = DTO.Text workflowToolText)
            SimpleISA.Run.proteomicsReadContract
            Contract.create(Operation.READ, path = runCwlPath, dtoType = DTOType.CWL, dto = DTO.Text runCwlText)
        |]

        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts contracts

        let run = arc.Runs.[0]
        let runCwlDescription = Expect.wantSome run.CWLDescription "Run CWL description should be present."
        Expect.isTrue
            (match runCwlDescription with | ARCtrl.CWL.Workflow _ -> true | _ -> false)
            (sprintf "Expected run CWL description to be workflow but got %A" runCwlDescription)

        let runWorkflow =
            match runCwlDescription with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | _ -> Unchecked.defaultof<ARCtrl.CWL.CWLWorkflowDescription>

        let runStep = (ARCtrl.CWL.ListEntry.values runWorkflow.Steps).[0]
        Expect.isTrue
            (match ARCtrl.CWL.WorkflowStep.getResolvedRun runStep with | ARCtrl.CWL.RunWorkflow _ -> true | _ -> false)
            (sprintf "Expected run step to resolve to RunWorkflow but got %A" (ARCtrl.CWL.WorkflowStep.getResolvedRun runStep))

        let workflowObj: obj =
            match ARCtrl.CWL.WorkflowStep.getResolvedRun runStep with
            | ARCtrl.CWL.RunWorkflow workflowObj -> workflowObj
            | _ -> Unchecked.defaultof<obj>

        Expect.isTrue
            (workflowObj :? ARCtrl.CWL.CWLWorkflowDescription)
            (sprintf "Expected run step to resolve to CWLWorkflowDescription but got %A" workflowObj)

        let referencedWorkflow = workflowObj :?> ARCtrl.CWL.CWLWorkflowDescription
        let referencedStep = (ARCtrl.CWL.ListEntry.values referencedWorkflow.Steps).[0]
        Expect.isTrue
            (match ARCtrl.CWL.WorkflowStep.getResolvedRun referencedStep with | ARCtrl.CWL.RunCommandLineTool _ -> true | _ -> false)
            (sprintf "Expected referenced workflow step run to resolve to RunCommandLineTool but got %A" (ARCtrl.CWL.WorkflowStep.getResolvedRun referencedStep))

        let toolObj: obj =
            match ARCtrl.CWL.WorkflowStep.getResolvedRun referencedStep with
            | ARCtrl.CWL.RunCommandLineTool toolObj -> toolObj
            | _ -> Unchecked.defaultof<obj>
        let tool = toolObj :?> ARCtrl.CWL.CWLToolDescription
        Expect.equal tool.BaseCommand.Value.[0] (CWL.SchemaSaladString.Literal "echo") "Referenced workflow step should resolve to command line tool"
    )
    testCase "simpleISAWithWR_WithCWL_ResolvesRunWorkflowReference_CacheHitForSharedToolPath" (fun () ->
        let workflowIdentifier = Workflow.Proteomics.workflowIdentifier
        let runIdentifier = Run.Proteomics.runIdentifier
        let workflowCwlPath = Identifier.Workflow.cwlFileNameFromIdentifier workflowIdentifier
        let workflowToolPath = ArcPathHelper.combineMany [|ArcPathHelper.WorkflowsFolderName; workflowIdentifier; "tool.cwl"|]
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier runIdentifier

        let workflowCwlText = TestObjects.CWL.RunWorkflowReference.SharedToolPath.workflowCwlText
        let workflowToolText = TestObjects.CWL.RunWorkflowReference.workflowToolText
        let runCwlText = TestObjects.CWL.RunWorkflowReference.SharedToolPath.runCwlText

        let contracts = [|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Workflow.proteomicsReadContract
            Contract.create(Operation.READ, path = workflowCwlPath, dtoType = DTOType.CWL, dto = DTO.Text workflowCwlText)
            Contract.create(Operation.READ, path = workflowToolPath, dtoType = DTOType.CWL, dto = DTO.Text workflowToolText)
            SimpleISA.Run.proteomicsReadContract
            Contract.create(Operation.READ, path = runCwlPath, dtoType = DTOType.CWL, dto = DTO.Text runCwlText)
        |]

        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts contracts

        let run = arc.Runs.[0]
        let runWorkflow =
            match Expect.wantSome run.CWLDescription "Run CWL description should be present" with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other ->
                Expect.isTrue false (sprintf "Expected run CWL to be workflow but got %A" other)
                Unchecked.defaultof<ARCtrl.CWL.CWLWorkflowDescription>

        let referencedWorkflow =
            match ARCtrl.CWL.WorkflowStep.getResolvedRun (ARCtrl.CWL.ListEntry.values runWorkflow.Steps).[0] with
            | ARCtrl.CWL.RunWorkflow workflowObj -> workflowObj :?> ARCtrl.CWL.CWLWorkflowDescription
            | other ->
                Expect.isTrue false (sprintf "Expected run step to resolve to workflow but got %A" other)
                Unchecked.defaultof<ARCtrl.CWL.CWLWorkflowDescription>

        Expect.equal (ARCtrl.CWL.ListEntry.values referencedWorkflow.Steps).Count 2 "Referenced workflow should expose both tool steps."

        for i in 0 .. (ARCtrl.CWL.ListEntry.values referencedWorkflow.Steps).Count - 1 do
            match ARCtrl.CWL.WorkflowStep.getResolvedRun (ARCtrl.CWL.ListEntry.values referencedWorkflow.Steps).[i] with
            | ARCtrl.CWL.RunCommandLineTool toolObj ->
                let tool = toolObj :?> ARCtrl.CWL.CWLToolDescription
                Expect.equal tool.BaseCommand.Value.[0] (CWL.SchemaSaladString.Literal "echo") $"Referenced step {i} should resolve to command line tool."
            | other ->
                Expect.isTrue false (sprintf "Expected referenced step %d to resolve to RunCommandLineTool but got %A" i other)
    )
    testCase "simpleISAWithWR_WithCWL_ResolvesRunWorkflowReference_CacheHitAcrossDeepNesting" (fun () ->
        let runIdentifier = Run.Proteomics.runIdentifier
        let workflowIdentifier = Workflow.Proteomics.workflowIdentifier
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier runIdentifier

        let workflowAPath = ArcPathHelper.combineMany [|ArcPathHelper.WorkflowsFolderName; workflowIdentifier; "workflow-a.cwl"|]
        let workflowBPath = ArcPathHelper.combineMany [|ArcPathHelper.WorkflowsFolderName; workflowIdentifier; "workflow-b.cwl"|]
        let workflowCPath = ArcPathHelper.combineMany [|ArcPathHelper.WorkflowsFolderName; workflowIdentifier; "workflow-c.cwl"|]
        let workflowToolPath = ArcPathHelper.combineMany [|ArcPathHelper.WorkflowsFolderName; workflowIdentifier; "tool.cwl"|]

        let runProcessingUnit = ARCtrl.CWL.Decode.decodeCWLProcessingUnit TestObjects.CWL.RunWorkflowReference.DeepSharedToolPath.runCwlText
        let workflowAProcessingUnit = ARCtrl.CWL.Decode.decodeCWLProcessingUnit TestObjects.CWL.RunWorkflowReference.DeepSharedToolPath.workflowACwlText
        let workflowBProcessingUnit = ARCtrl.CWL.Decode.decodeCWLProcessingUnit TestObjects.CWL.RunWorkflowReference.DeepSharedToolPath.workflowBCwlText
        let workflowCProcessingUnit = ARCtrl.CWL.Decode.decodeCWLProcessingUnit TestObjects.CWL.RunWorkflowReference.DeepSharedToolPath.workflowCCwlText
        let toolProcessingUnit = ARCtrl.CWL.Decode.decodeCWLProcessingUnit TestObjects.CWL.RunWorkflowReference.workflowToolText

        let mutable toolResolveCalls = 0

        let tryResolveRunPath (path: string) =
            let key = ArcPathHelper.normalizePathKey path
            if key = ArcPathHelper.normalizePathKey workflowAPath then
                Some workflowAProcessingUnit
            elif key = ArcPathHelper.normalizePathKey workflowBPath then
                Some workflowBProcessingUnit
            elif key = ArcPathHelper.normalizePathKey workflowCPath then
                Some workflowCProcessingUnit
            elif key = ArcPathHelper.normalizePathKey workflowToolPath then
                toolResolveCalls <- toolResolveCalls + 1
                Some toolProcessingUnit
            else
                None

        let resolvedRun =
            ARCtrl.CWLRunResolver.resolveRunReferencesFromLookup runCwlPath runProcessingUnit tryResolveRunPath

        let getStepById (id: string) (workflow: ARCtrl.CWL.CWLWorkflowDescription) =
            let stepOpt =
                (ARCtrl.CWL.ListEntry.values workflow.Steps)
                |> Seq.tryFind (fun s -> s.Id = Some id)
            Expect.wantSome stepOpt (sprintf "Expected step '%s' to exist." id)

        let requireWorkflowRun (label: string) (run: ARCtrl.CWL.WorkflowStepRun) : ARCtrl.CWL.CWLWorkflowDescription =
            match run with
            | ARCtrl.CWL.RunWorkflow workflowObj -> workflowObj :?> ARCtrl.CWL.CWLWorkflowDescription
            | other ->
                Expect.isTrue false (sprintf "Expected %s to resolve to RunWorkflow but got %A" label other)
                Unchecked.defaultof<ARCtrl.CWL.CWLWorkflowDescription>

        let requireToolRun (label: string) (run: ARCtrl.CWL.WorkflowStepRun) : ARCtrl.CWL.CWLToolDescription =
            match run with
            | ARCtrl.CWL.RunCommandLineTool toolObj -> toolObj :?> ARCtrl.CWL.CWLToolDescription
            | other ->
                Expect.isTrue false (sprintf "Expected %s to resolve to RunCommandLineTool but got %A" label other)
                Unchecked.defaultof<ARCtrl.CWL.CWLToolDescription>

        let runWorkflow =
            match resolvedRun with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other ->
                Expect.isTrue false (sprintf "Expected resolved run to be Workflow but got %A" other)
                Unchecked.defaultof<ARCtrl.CWL.CWLWorkflowDescription>

        let workflowA = getStepById "Nested" runWorkflow |> fun step -> requireWorkflowRun "Nested" (ARCtrl.CWL.WorkflowStep.getResolvedRun step)
        let workflowB = getStepById "NestedB" workflowA |> fun step -> requireWorkflowRun "NestedB" (ARCtrl.CWL.WorkflowStep.getResolvedRun step)
        let workflowC = getStepById "NestedC" workflowB |> fun step -> requireWorkflowRun "NestedC" (ARCtrl.CWL.WorkflowStep.getResolvedRun step)

        let deepTool : ARCtrl.CWL.CWLToolDescription =
            getStepById "SharedTool" workflowC
            |> fun step -> requireToolRun "SharedTool" (ARCtrl.CWL.WorkflowStep.getResolvedRun step)
        let directTool : ARCtrl.CWL.CWLToolDescription =
            getStepById "DirectTool" runWorkflow
            |> fun step -> requireToolRun "DirectTool" (ARCtrl.CWL.WorkflowStep.getResolvedRun step)

        Expect.equal deepTool.BaseCommand.Value.[0] (CWL.SchemaSaladString.Literal "echo") "Deeply nested shared tool should resolve correctly."
        Expect.equal directTool.BaseCommand.Value.[0] (CWL.SchemaSaladString.Literal "echo") "Top-level shared tool should resolve correctly."
        Expect.equal toolResolveCalls 1 "Shared tool should be resolved exactly once and reused via cache across nesting depths."
    )
    testCase "simpleISAWithWR_WithCWL_ResolvesRunWorkflowReference_CycleSafe" (fun () ->
        let runIdentifier = Run.Proteomics.runIdentifier
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier runIdentifier
        let workflowACwlPath = ArcPathHelper.combineMany [|ArcPathHelper.RunsFolderName; runIdentifier; "a.cwl"|]
        let workflowBCwlPath = ArcPathHelper.combineMany [|ArcPathHelper.RunsFolderName; runIdentifier; "b.cwl"|]

        let workflowACwlText = TestObjects.CWL.RunWorkflowReference.CycleSafe.workflowACwlText
        let workflowBCwlText = TestObjects.CWL.RunWorkflowReference.CycleSafe.workflowBCwlText
        let runCwlText = TestObjects.CWL.RunWorkflowReference.CycleSafe.runCwlText

        let contracts = [|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Workflow.proteomicsReadContract
            SimpleISA.Run.proteomicsReadContract
            Contract.create(Operation.READ, path = runCwlPath, dtoType = DTOType.CWL, dto = DTO.Text runCwlText)
            Contract.create(Operation.READ, path = workflowACwlPath, dtoType = DTOType.CWL, dto = DTO.Text workflowACwlText)
            Contract.create(Operation.READ, path = workflowBCwlPath, dtoType = DTOType.CWL, dto = DTO.Text workflowBCwlText)
        |]

        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts contracts

        let run = arc.Runs.[0]
        let runWorkflow =
            match Expect.wantSome run.CWLDescription "Run CWL description should be present" with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other ->
                Expect.isTrue false (sprintf "Expected run CWL to be workflow but got %A" other)
                Unchecked.defaultof<ARCtrl.CWL.CWLWorkflowDescription>

        let stepA =
            match ARCtrl.CWL.WorkflowStep.getResolvedRun (ARCtrl.CWL.ListEntry.values runWorkflow.Steps).[0] with
            | ARCtrl.CWL.RunWorkflow workflowObj -> workflowObj :?> ARCtrl.CWL.CWLWorkflowDescription
            | other ->
                Expect.isTrue false (sprintf "Expected first run step to resolve to workflow but got %A" other)
                Unchecked.defaultof<ARCtrl.CWL.CWLWorkflowDescription>

        let stepB =
            match ARCtrl.CWL.WorkflowStep.getResolvedRun (ARCtrl.CWL.ListEntry.values stepA.Steps).[0] with
            | ARCtrl.CWL.RunWorkflow workflowObj -> workflowObj :?> ARCtrl.CWL.CWLWorkflowDescription
            | other ->
                Expect.isTrue false (sprintf "Expected nested step to resolve to workflow but got %A" other)
                Unchecked.defaultof<ARCtrl.CWL.CWLWorkflowDescription>

        match ARCtrl.CWL.WorkflowStep.getResolvedRun (ARCtrl.CWL.ListEntry.values stepB.Steps).[0] with
        | ARCtrl.CWL.RunString unresolved ->
            Expect.equal unresolved "./a.cwl" "Cycle should stop and keep unresolved string reference"
        | other ->
            Expect.isTrue false (sprintf "Expected cycle edge to remain RunString but got %A" other)
    )
    testCase "simpleISAWithWR_WithCWL_FilesThatRunEachOtherHoldNoCycleWhenAWorkflowAndARunReachThem" (fun () ->
        let runIdentifier = Run.Proteomics.runIdentifier
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier runIdentifier
        let workflowCwlPath = Identifier.Workflow.cwlFileNameFromIdentifier Workflow.Proteomics.workflowIdentifier
        let workflowACwlPath = ArcPathHelper.combineMany [|ArcPathHelper.RunsFolderName; runIdentifier; "a.cwl"|]
        let workflowBCwlPath = ArcPathHelper.combineMany [|ArcPathHelper.RunsFolderName; runIdentifier; "b.cwl"|]
        // The workflow runs b.cwl and the run runs a.cwl. Both files come from one lookup, so the second resolution meets
        // the objects that the first one resolved.
        let workflowCwlText = TestObjects.CWL.RunWorkflowReference.CycleSafe.runCwlText.Replace("run: ./a.cwl", $"run: ../../runs/{runIdentifier}/b.cwl")
        let contracts = [|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Workflow.proteomicsReadContract
            SimpleISA.Run.proteomicsReadContract
            Contract.create(Operation.READ, path = workflowCwlPath, dtoType = DTOType.CWL, dto = DTO.Text workflowCwlText)
            Contract.create(Operation.READ, path = runCwlPath, dtoType = DTOType.CWL, dto = DTO.Text TestObjects.CWL.RunWorkflowReference.CycleSafe.runCwlText)
            Contract.create(Operation.READ, path = workflowACwlPath, dtoType = DTOType.CWL, dto = DTO.Text TestObjects.CWL.RunWorkflowReference.CycleSafe.workflowACwlText)
            Contract.create(Operation.READ, path = workflowBCwlPath, dtoType = DTOType.CWL, dto = DTO.Text TestObjects.CWL.RunWorkflowReference.CycleSafe.workflowBCwlText)
        |]

        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts contracts

        let workflowOf (description: ARCtrl.CWL.CWLProcessingUnit option) =
            match Expect.wantSome description "The CWL description should be present" with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other -> failwith $"Expected a workflow but got %A{other}"
        let resolvedWorkflowOf (workflow: ARCtrl.CWL.CWLWorkflowDescription) =
            ARCtrl.CWL.WorkflowStepRunOps.tryGetWorkflow (ARCtrl.CWL.WorkflowStep.getResolvedRun (ARCtrl.CWL.ListEntry.values workflow.Steps).[0])
        // Each workflow of the files holds one step, so the resolved runs form a chain that ends or comes back.
        let chainLength (start: ARCtrl.CWL.CWLWorkflowDescription) =
            let rec walk (seen: ARCtrl.CWL.CWLWorkflowDescription list) (workflow: ARCtrl.CWL.CWLWorkflowDescription) =
                if seen |> List.exists (fun other -> obj.ReferenceEquals(other, workflow)) then failwith "The resolved runs hold a cycle."
                match resolvedWorkflowOf workflow with
                | Some next -> walk (workflow :: seen) next
                | None -> List.length seen + 1
            walk [] start
        let workflow = workflowOf arc.Workflows.[0].CWLDescription
        let run = workflowOf arc.Runs.[0].CWLDescription
        Expect.equal (chainLength workflow) 3 "The workflow reaches b.cwl and a.cwl, and the run of a.cwl back to b.cwl stays a reference."
        Expect.equal (chainLength run) 2 "The run reaches a.cwl, whose run of b.cwl leads back to a.cwl and stays a reference."
        let a = Expect.wantSome (resolvedWorkflowOf run) "The run resolves a.cwl."
        Expect.equal (ARCtrl.CWL.ListEntry.values a.Steps).[0].Run (ARCtrl.CWL.RunString "./b.cwl") "The step keeps the reference as written."
    )
    testCase "resolveRunReferencesFromLookup_ProcessThatHoldsTheWorkflowOrItsStepStaysOutOfTheResolvedRun" (fun () ->
        let workflowOf (text: string) =
            match ARCtrl.CWL.Decode.decodeCWLProcessingUnit text with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other -> failwith $"Expected a workflow but got %A{other}"
        let text = "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  s:\n    run: other.cwl\n    in: {}\n    out: []\n"
        // A lookup that hands out the workflow itself for another path.
        let root = workflowOf text
        ARCtrl.CWLRunResolver.resolveRunReferencesFromLookup "runs/r/run.cwl" (ARCtrl.CWL.Workflow root) (fun _ -> Some (ARCtrl.CWL.Workflow root)) |> ignore
        Expect.isNone (ARCtrl.CWL.ListEntry.values root.Steps).[0].ResolvedRun.Run "The workflow is no process of its own step."
        // A step whose run holds the workflow inline: the resolver does not enter it a second time.
        let inlineHolder = workflowOf text
        (ARCtrl.CWL.ListEntry.values inlineHolder.Steps).[0].Run <- ARCtrl.CWL.WorkflowStepRunOps.fromWorkflow inlineHolder
        ARCtrl.CWLRunResolver.resolveRunReferencesFromLookup "runs/r/run.cwl" (ARCtrl.CWL.Workflow inlineHolder) (fun _ -> None) |> ignore
        // A single step whose lookup hands out the workflow that holds the step.
        let owner = workflowOf text
        let step = (ARCtrl.CWL.ListEntry.values owner.Steps).[0]
        ARCtrl.CWLRunResolver.resolveWorkflowStepFromLookup "runs/r/run.cwl" step (fun _ -> Some (ARCtrl.CWL.Workflow owner))
        Expect.isNone step.ResolvedRun.Run "The workflow that holds the step is no process of the step."
        let other = workflowOf text
        ARCtrl.CWLRunResolver.resolveWorkflowStepFromLookup "runs/r/run.cwl" step (fun _ -> Some (ARCtrl.CWL.Workflow other))
        Expect.isSome step.ResolvedRun.Run "Another workflow is the process of the step."
    )
    testCase "simpleISAWithWR_WithCWL_ResolvesRunWorkflowReference_PartialResolutionKeepsString" (fun () ->
        let runIdentifier = Run.Proteomics.runIdentifier
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier runIdentifier
        let runCwlText = TestObjects.CWL.RunWorkflowReference.PartialResolution.runCwlText

        let contracts = [|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Workflow.proteomicsReadContract
            SimpleISA.Run.proteomicsReadContract
            Contract.create(Operation.READ, path = runCwlPath, dtoType = DTOType.CWL, dto = DTO.Text runCwlText)
        |]

        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts contracts

        let run = arc.Runs.[0]
        let runWorkflow =
            match Expect.wantSome run.CWLDescription "Run CWL description should be present" with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other ->
                Expect.isTrue false (sprintf "Expected run CWL to be workflow but got %A" other)
                Unchecked.defaultof<ARCtrl.CWL.CWLWorkflowDescription>

        match (ARCtrl.CWL.ListEntry.values runWorkflow.Steps).[0].Run with
        | ARCtrl.CWL.RunString unresolved ->
            Expect.equal unresolved "./missing.cwl" "Missing referenced workflow should remain unresolved string"
        | other ->
            Expect.isTrue false (sprintf "Expected missing reference to remain RunString but got %A" other)
        Expect.isNone (ARCtrl.CWL.ListEntry.values runWorkflow.Steps).[0].ResolvedRun.Run "A reference that finds no file resolves to nothing."
    )
    testCase "simpleISAWithWR_WithCWL_WritesTheRunReferencesItRead" (fun () ->
        let workflowIdentifier = Workflow.Proteomics.workflowIdentifier
        let workflowCwlPath = Identifier.Workflow.cwlFileNameFromIdentifier workflowIdentifier
        let shoutPath = ArcPathHelper.combineMany [|ArcPathHelper.WorkflowsFolderName; workflowIdentifier; "shout.cwl"|]
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let contracts = [|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Workflow.proteomicsReadContract
            Contract.create(Operation.READ, path = workflowCwlPath, dtoType = DTOType.CWL, dto = DTO.Text TestObjects.CWL.RunWorkflowReference.StandardLayout.workflowCwlText)
            Contract.create(Operation.READ, path = shoutPath, dtoType = DTOType.CWL, dto = DTO.Text TestObjects.CWL.RunWorkflowReference.StandardLayout.shoutCwlText)
            SimpleISA.Run.proteomicsReadContract
            Contract.create(Operation.READ, path = runCwlPath, dtoType = DTOType.CWL, dto = DTO.Text TestObjects.CWL.RunWorkflowReference.StandardLayout.runCwlText)
        |]

        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts contracts

        let writtenRun = writtenCwlText arc runCwlPath
        Expect.stringContains writtenRun "run: ../../workflows/Proteomics/workflow.cwl" "The run file refers to the workflow file as it did."
        Expect.isFalse (writtenRun.Contains "baseCommand") "The run file holds no process of another file."
        Expect.equal
            (ARCtrl.CWL.Decode.decodeCWLProcessingUnit writtenRun)
            (ARCtrl.CWL.Decode.decodeCWLProcessingUnit TestObjects.CWL.RunWorkflowReference.StandardLayout.runCwlText)
            "The run file reads back as the file that was read."
        let writtenWorkflow = writtenCwlText arc workflowCwlPath
        Expect.stringContains writtenWorkflow "run: shout.cwl" "The workflow file refers to its tool as it did."
        Expect.isFalse (writtenWorkflow.Contains "InitialWorkDirRequirement") "The workflow file holds no part of the tool."
        Expect.isFalse (arc.GetWriteContracts() |> Array.exists (fun c -> c.Path = shoutPath)) "The tool file belongs to no run or workflow of the ARC, so the ARC writes nothing for it."

        let runDescription = Expect.wantSome arc.Runs.[0].CWLDescription "Run CWL description should be present."
        Expect.equal
            runDescription
            (ARCtrl.CWL.Decode.decodeCWLProcessingUnit TestObjects.CWL.RunWorkflowReference.StandardLayout.runCwlText)
            "The resolved processes of the steps do not count for the comparison of the description."
        let runWorkflow =
            match runDescription with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other -> failwith $"Expected a workflow but got %A{other}"
        let workflow =
            match ARCtrl.CWL.WorkflowStep.getResolvedRun (ARCtrl.CWL.ListEntry.values runWorkflow.Steps).[0] with
            | ARCtrl.CWL.RunWorkflow workflow -> workflow :?> ARCtrl.CWL.CWLWorkflowDescription
            | other -> failwith $"Expected the step to resolve to the workflow but got %A{other}"
        let tool =
            match ARCtrl.CWL.WorkflowStep.getResolvedRun (ARCtrl.CWL.ListEntry.values workflow.Steps).[0] with
            | ARCtrl.CWL.RunCommandLineTool tool -> tool :?> ARCtrl.CWL.CWLToolDescription
            | other -> failwith $"Expected the step to resolve to the tool but got %A{other}"
        Expect.equal tool.BaseCommand.Value.[0] (CWL.SchemaSaladString.Literal "echo") "Consumers still find the tool behind both references."
    )
    testCase "simpleISAWithWR_WithCWL_ResolvesTheSameRelativePathInTwoDirectoriesToTwoFiles" (fun () ->
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let fixtures = TestObjects.CWL.RunWorkflowReference.SameRelativePathInTwoDirectories.workflowCwlText
        let readContract (path: string) (text: string) =
            Contract.create(Operation.READ, path = path, dtoType = DTOType.CWL, dto = DTO.Text text)
        let contracts = [|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Workflow.proteomicsReadContract
            readContract "workflows/First/workflow.cwl" fixtures
            readContract "workflows/First/shout.cwl" TestObjects.CWL.RunWorkflowReference.SameRelativePathInTwoDirectories.echoCwlText
            readContract "workflows/Second/workflow.cwl" fixtures
            readContract "workflows/Second/shout.cwl" TestObjects.CWL.RunWorkflowReference.SameRelativePathInTwoDirectories.catCwlText
            SimpleISA.Run.proteomicsReadContract
            readContract runCwlPath TestObjects.CWL.RunWorkflowReference.SameRelativePathInTwoDirectories.runCwlText
        |]

        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts contracts

        let runWorkflow =
            match Expect.wantSome arc.Runs.[0].CWLDescription "Run CWL description should be present." with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other -> failwith $"Expected a workflow but got %A{other}"
        let commandOf (step: ARCtrl.CWL.WorkflowStep) =
            let workflow =
                match ARCtrl.CWL.WorkflowStep.getResolvedRun step with
                | ARCtrl.CWL.RunWorkflow workflow -> workflow :?> ARCtrl.CWL.CWLWorkflowDescription
                | other -> failwith $"Expected the step to resolve to a workflow but got %A{other}"
            match ARCtrl.CWL.WorkflowStep.getResolvedRun (ARCtrl.CWL.ListEntry.values workflow.Steps).[0] with
            | ARCtrl.CWL.RunCommandLineTool tool -> (tool :?> ARCtrl.CWL.CWLToolDescription).BaseCommand.Value.[0]
            | other -> failwith $"Expected the step to resolve to a tool but got %A{other}"
        Expect.equal (commandOf (ARCtrl.CWL.ListEntry.values runWorkflow.Steps).[0]) (CWL.SchemaSaladString.Literal "echo") "The first workflow runs the shout.cwl next to it."
        Expect.equal (commandOf (ARCtrl.CWL.ListEntry.values runWorkflow.Steps).[1]) (CWL.SchemaSaladString.Literal "cat") "The second workflow runs the shout.cwl next to it."
    )
    testCase "simpleISAWithWR_WithRunYml_WritesTheParameterFileAndReadsItBack" (fun () ->
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let ymlPath = Identifier.Run.ymlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let ymlText = """msg: hello
reads:
  - class: File
    path: ../../assays/RNASeq/dataset/a.fastq.gz
"""
        let readContracts = [|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Workflow.proteomicsReadContract
            SimpleISA.Run.proteomicsReadContract
            Contract.create(Operation.READ, path = runCwlPath, dtoType = DTOType.CWL, dto = DTO.Text TestObjects.CWL.RunWorkflowReference.StandardLayout.runCwlText)
            Contract.create(Operation.READ, path = ymlPath, dtoType = DTOType.YAML, dto = DTO.Text ymlText)
        |]

        let arc = ARC("MyIdentifier")
        arc.SetISAFromContracts readContracts
        Expect.equal arc.Runs.[0].CWLInput.Count 2 "The run reads both parameter references."

        let ymlContract =
            arc.GetWriteContracts()
            |> Array.tryFind (fun c -> c.Path = ymlPath)
            |> fun c -> Expect.wantSome c "The ARC writes the parameter file of the run."
        Expect.equal ymlContract.Operation Operation.CREATE "The parameter file is a create contract."
        Expect.equal ymlContract.DTOType (Some DTOType.YAML) "The parameter file is typed as YAML."

        let readBack = ARC("MyIdentifier")
        let readContractsOfWrite =
            Array.append
                (readContracts |> Array.filter (fun c -> c.Path <> ymlPath))
                [| { ymlContract with Operation = Operation.READ } |]
        readBack.SetISAFromContracts readContractsOfWrite
        Expect.sequenceEqual readBack.Runs.[0].CWLInput arc.Runs.[0].CWLInput "The parameter references read back equal."
    )
    testCase "simpleISAWithWR_WithPackedRunCWL_FailsWithTheFile" (fun () ->
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let packed = "cwlVersion: v1.2\n$graph:\n  - id: main\n    class: CommandLineTool\n    baseCommand: echo\n    inputs: []\n    outputs: []\n"
        match (try arcWithRunCwl packed |> ignore; None with YAMLicious.YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.stringContains error.Message "Packed CWL documents ($graph) are not supported." "The error says that the document is packed."
            Expect.stringContains error.Message runCwlPath "The error names the file."
            Expect.equal (YAMLicious.DecodeError.pathText error) "" "The error stands at the root."
            Expect.isSome error.Span "The error keeps its place in the document."
        | None -> failwith "A packed run file must fail."
    )
    testCase "simpleISAWithWR_WithRunCWLOfUnknownClass_IsWrittenAsItWasRead" (fun () ->
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let text = "cwlVersion: v1.2\nclass: {$import: s.yml}\nbaseCommand: echo\ninputs: []\noutputs: []\n"
        let arc = arcWithRunCwl text
        match Expect.wantSome arc.Runs.[0].CWLDescription "Run CWL description should be present" with
        | ARCtrl.CWL.RawProcess raw -> Expect.equal raw { ARCtrl.CWL.RawYaml.Node = YAMLicious.Reader.readRoot text } "The run holds the process as written."
        | other -> failwith $"Expected a process of unknown class but got %A{other}"
        Expect.equal (writtenCwlText arc runCwlPath) text "The file is written as it was read."
    )
    testCase "simpleISAWithWR_WithPlainRunCWL_FragmentReferenceIsWrittenAsItWasRead" (fun () ->
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let text = "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  shout:\n    run: \"#shout\"\n    in: {}\n    out: []\n"
        let arc = arcWithRunCwl text
        let workflow =
            match Expect.wantSome arc.Runs.[0].CWLDescription "Run CWL description should be present" with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other -> failwith $"Expected a workflow but got %A{other}"
        Expect.equal (ARCtrl.CWL.ListEntry.values workflow.Steps).[0].Run (ARCtrl.CWL.RunString "#shout") "The step keeps the reference as written."
        Expect.isNone (ARCtrl.CWL.ListEntry.values workflow.Steps).[0].ResolvedRun.Run "Nothing resolves a reference of the form #id."
        let written = writtenCwlText arc runCwlPath
        Expect.stringContains written "run: \"#shout\"" "The reference is written as it was read."
        Expect.equal (ARCtrl.CWL.Decode.decodeCWLProcessingUnit written) (ARCtrl.CWL.Workflow workflow) "The written file reads back equal."
    )
    testCase "simpleISAWithWR_WithFailingRunCWL_FailsWithTheFileAndKeepsThePlaceInTheDocument" (fun () ->
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let badText = "cwlVersion: v1.2\nclass: Workflow\ninputs: []\noutputs: []\nsteps: []\nsteps: []\n"
        let decodeError (read: unit -> unit) =
            match (try read (); None with YAMLicious.YamlDecodeException error -> Some error) with
            | Some error -> error
            | None -> failwith "The file does not decode, so reading it must fail."
        let alone = decodeError (fun () -> ARCtrl.CWL.Decode.decodeCWLProcessingUnit badText |> ignore)
        let inArc = decodeError (fun () -> arcWithRunCwl badText |> ignore)
        Expect.stringContains inArc.Message runCwlPath "The error names the file of the run."
        Expect.stringContains inArc.Message alone.Message "The error keeps the message of the decoder."
        Expect.equal inArc.Path alone.Path "The error keeps its path in the document."
        Expect.equal inArc.Span alone.Span "The error keeps its span in the document."
    )
    testCase "simpleISAWithWR_WithFailingWorkflowCWL_FailsWithTheFile" (fun () ->
        let workflowCwlPath = Identifier.Workflow.cwlFileNameFromIdentifier Workflow.Proteomics.workflowIdentifier
        let contracts = [|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Workflow.proteomicsReadContract
            Contract.create(Operation.READ, path = workflowCwlPath, dtoType = DTOType.CWL, dto = DTO.Text "cwlVersion: v1.2\nclass: Workflow\ninputs: []\noutputs: []\nsteps: []\nsteps: []\n")
        |]
        match (try ARC("MyIdentifier").SetISAFromContracts contracts; None with YAMLicious.YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.stringContains error.Message workflowCwlPath "The error names the file of the workflow."
            Expect.equal error.Path [ "steps" ] "The error points at the repeated key."
            Expect.isSome error.Span "The error has a span."
        | None -> failwith "A workflow file that does not decode must fail the read."
    )
    testCase "simpleISAWithWR_WithFailingToolCWL_FailsWithTheFile" (fun () ->
        let toolPath = ArcPathHelper.combineMany [|ArcPathHelper.WorkflowsFolderName; Workflow.Proteomics.workflowIdentifier; "tool.cwl"|]
        let contracts = [|
            SimpleISA.Investigation.investigationReadContract
            SimpleISA.Study.bII_S_1ReadContract
            SimpleISA.Assay.proteomeReadContract
            SimpleISA.Workflow.proteomicsReadContract
            Contract.create(Operation.READ, path = toolPath, dtoType = DTOType.CWL, dto = DTO.Text "cwlVersion: v1.2\nclass: Workflow\ninputs: []\noutputs: []\nsteps: []\nsteps: []\n")
        |]
        match (try ARC("MyIdentifier").SetISAFromContracts contracts; None with YAMLicious.YamlDecodeException error -> Some error) with
        | Some error -> Expect.stringContains error.Message toolPath "The error names the file of the tool, which no run or workflow owns."
        | None -> failwith "A CWL file that does not decode must fail the read."
    )
    testCase "simpleISAWithWR_WithUnparsableCWLAndParameterFile_FailsWithTheFile" (fun () ->
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let ymlPath = Identifier.Run.ymlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let readWith (path: string) (dtoType: DTOType) (text: string) =
            let contracts = [|
                SimpleISA.Investigation.investigationReadContract
                SimpleISA.Study.bII_S_1ReadContract
                SimpleISA.Assay.proteomeReadContract
                SimpleISA.Workflow.proteomicsReadContract
                SimpleISA.Run.proteomicsReadContract
                Contract.create(Operation.READ, path = path, dtoType = dtoType, dto = DTO.Text text)
            |]
            ARC("MyIdentifier").SetISAFromContracts contracts
        let parseError (read: unit -> unit) =
            match (try read (); None with YAMLicious.YamlParseException error -> Some error) with
            | Some error -> error
            | None -> failwith "The text is no YAML, so reading it must fail."
        let aloneCwl = parseError (fun () -> ARCtrl.CWL.Decode.decodeCWLProcessingUnit "cwlVersion: [v1.2\n" |> ignore)
        let inArcCwl = parseError (fun () -> readWith runCwlPath DTOType.CWL "cwlVersion: [v1.2\n")
        Expect.stringContains inArcCwl.Message runCwlPath "The error names the CWL file."
        Expect.equal inArcCwl.Span aloneCwl.Span "The error keeps its span."
        Expect.equal inArcCwl.Code aloneCwl.Code "The error keeps its code."
        let inArcYml = parseError (fun () -> readWith ymlPath DTOType.YAML "msg: [1, 2\n")
        Expect.stringContains inArcYml.Message ymlPath "The error names the parameter file."
    )
    testCase "resolveRunReferencesFromLookup_RunOfAFileWithAProcessOfUnknownClassStaysAReference" (fun () ->
        let workflow =
            match ARCtrl.CWL.Decode.decodeCWLProcessingUnit "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  s:\n    run: tool.cwl\n    in: {}\n    out: [out]\n" with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other -> failwith $"Expected a workflow but got %A{other}"
        let unknown = ARCtrl.CWL.Decode.decodeCWLProcessingUnit "cwlVersion: v1.2\nclass: {$import: s.yml}\nbaseCommand: echo\ninputs: []\noutputs: []\n"
        let toolKey = ArcPathHelper.normalizePathKey "runs/r/tool.cwl"
        let lookup (path: string) = if ArcPathHelper.normalizePathKey path = toolKey then Some unknown else None
        ARCtrl.CWLRunResolver.resolveRunReferencesFromLookup "runs/r/run.cwl" (ARCtrl.CWL.Workflow workflow) lookup |> ignore
        Expect.equal (ARCtrl.CWL.ListEntry.values workflow.Steps).[0].Run (ARCtrl.CWL.RunString "tool.cwl") "The step keeps the reference as written."
        Expect.isNone (ARCtrl.CWL.ListEntry.values workflow.Steps).[0].ResolvedRun.Run "The file holds a process of unknown class, so the step resolves to nothing."
        Expect.equal (ARCtrl.CWLRunResolver.resolveRunReferencesFromLookup "runs/r/run.cwl" unknown lookup) unknown "A process of unknown class has no steps to resolve."
    )
    testCase "resolveRunReferencesFromLookup_ImportedRunResolvesToTheProcessOfTheSamePath" (fun () ->
        let workflowWith (run: string) =
            match ARCtrl.CWL.Decode.decodeCWLProcessingUnit ("cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  s:\n    run: " + run + "\n    in: {}\n    out: [out]\n") with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other -> failwith $"Expected a workflow but got %A{other}"
        let tool = ARCtrl.CWL.Decode.decodeCommandLineTool TestObjects.CWL.CommandLineTool.cwlFile
        let toolKey = ArcPathHelper.normalizePathKey "runs/r/tool.cwl"
        let lookup (path: string) = if ArcPathHelper.normalizePathKey path = toolKey then Some (ARCtrl.CWL.CommandLineTool tool) else None
        let imported = workflowWith "{$import: tool.cwl}"
        let plain = workflowWith "tool.cwl"
        ARCtrl.CWLRunResolver.resolveRunReferencesFromLookup "runs/r/run.cwl" (ARCtrl.CWL.Workflow imported) lookup |> ignore
        ARCtrl.CWLRunResolver.resolveRunReferencesFromLookup "runs/r/run.cwl" (ARCtrl.CWL.Workflow plain) lookup |> ignore
        Expect.equal (ARCtrl.CWL.ListEntry.values imported.Steps).[0].Run (ARCtrl.CWL.RunImport "tool.cwl") "The step keeps the import as written."
        let resolvedTool (workflow: ARCtrl.CWL.CWLWorkflowDescription) =
            Expect.wantSome (ARCtrl.CWL.WorkflowStepRunOps.tryGetTool (ARCtrl.CWL.WorkflowStep.getResolvedRun (ARCtrl.CWL.ListEntry.values workflow.Steps).[0])) "The step resolves to a tool."
        Expect.isTrue (obj.ReferenceEquals(resolvedTool imported, tool)) "The import resolves to the process of the path."
        Expect.isTrue (obj.ReferenceEquals(resolvedTool imported, resolvedTool plain)) "The import and the path resolve to one process."
        Expect.isTrue ((ARCtrl.CWL.Encode.encodeWorkflowDescription imported).Contains "run: {$import: tool.cwl}") "The step writes the import again."
    )
    testCase "resolveRunReferencesFromLookup_RunThatLeadsBackToItsFileStaysAReference" (fun () ->
        let workflow =
            match ARCtrl.CWL.Decode.decodeCWLProcessingUnit TestObjects.CWL.RunWorkflowReference.selfReferenceText with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other -> failwith $"Expected a workflow but got %A{other}"
        let ownPathKey = ArcPathHelper.normalizePathKey "runs/r/run.cwl"
        let lookup (path: string) = if ArcPathHelper.normalizePathKey path = ownPathKey then Some (ARCtrl.CWL.Workflow workflow) else None
        ARCtrl.CWLRunResolver.resolveRunReferencesFromLookup "runs/r/run.cwl" (ARCtrl.CWL.Workflow workflow) lookup |> ignore
        Expect.equal (ARCtrl.CWL.ListEntry.values workflow.Steps).[0].Run (ARCtrl.CWL.RunString "run.cwl") "The step that runs its own file is not resolved."
        Expect.isNone (ARCtrl.CWL.ListEntry.values workflow.Steps).[0].ResolvedRun.Run "The step that runs its own file has no resolved run."
    )
    testCase "getRunNodeCandidates_OnlyAPathToACWLFileHasCandidates" (fun () ->
        Expect.sequenceEqual
            (ARCtrl.CWLRunResolver.getRunNodeCandidates "workflows/w/workflow.cwl" "./sub/../tools.cwl")
            [| "./sub/../tools.cwl"; "tools.cwl"; "workflows/w/tools.cwl" |]
            "A path to a CWL file is looked up as written, normalized and in the directory of the workflow."
        for reference in [ "#shout"; "tools.cwl#shout"; "notes.txt"; "" ] do
            Expect.isEmpty (ARCtrl.CWLRunResolver.getRunNodeCandidates "workflows/w/workflow.cwl" reference) $"{reference} names no CWL file, so it stays a reference."
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
        let datamap = Expect.wantSome a1.Datamap "Proteome Assay was supposed to have datamap"
        
        Expect.equal 2 datamap.DataContexts.Count "Datamap was not read correctly"

        let a2 = arc.GetAssay(SimpleISA.Assay.metabolomeIdentifer)
        Expect.isNone a2.Datamap "Metabolome Assay was not supposed to have datamap"
    
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
        let dm = Expect.wantSome arc.Assays.[0].Datamap "Assay should have a datamap assigned"
        Expect.hasLength dm.DataContexts 2 "Datamap should have two data contexts"

        Expect.isNone arc.Studies.[0].Datamap "Study should not have a datamap assigned"
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
        Expect.isNone arc.Assays.[0].Datamap "Assay should not have a datamap assigned"

        let dm = Expect.wantSome arc.Studies.[0].Datamap "Study should have a datamap assigned"
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
        let dm = Datamap.init()
        a.Datamap <- Some dm
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
    // https://github.com/nfdi4plants/ARCtrl/issues/641
    testCase "duplicateCommentNames_ErrorSurfaces" (fun _ ->
        let inv = ArcInvestigation("MyInvestigation", "BestTitle", comments = ResizeArray [Comment("note", "first"); Comment("note", "second")])
        let arc = ARC.fromArcInvestigation(isa = inv)
        let writingResult =
            try
                arc.GetWriteContracts() |> ignore
                Result.Ok "DidRun"
            with
            | err -> Result.Error err.Message
        let message = Expect.wantError writingResult "Getting write contracts for an investigation with duplicate comment names should fail"
        Expect.stringContains message "Comment[note]" "Error should name the duplicate comment"
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
        let wb = assayFileDTO.AsSpreadsheet()
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
        let wb = studyFileDTO.AsSpreadsheet()
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
        let wb = workflowFileDTO.AsSpreadsheet()
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
        let wb = runFileDTO.AsSpreadsheet()
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

        let dm = Expect.wantSome (arc.GetAssay(SimpleISA.Assay.proteomeIdentifer).Datamap) "Assay should have datamap"       
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
    testList "CWL files of runs and workflows" [
        let workflowCwlPath = Identifier.Workflow.cwlFileNameFromIdentifier Workflow.Proteomics.workflowIdentifier
        let shoutPath = ArcPathHelper.combineMany [|ArcPathHelper.WorkflowsFolderName; Workflow.Proteomics.workflowIdentifier; "shout.cwl"|]
        let runCwlPath = Identifier.Run.cwlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let ymlPath = Identifier.Run.ymlFileNameFromIdentifier Run.Proteomics.runIdentifier
        let readContract path dtoType text = Contract.create(Operation.READ, path = path, dtoType = dtoType, dto = DTO.Text text)
        /// An ARC with a run and a workflow that have CWL files, and the run with a parameter file when asked.
        let readARC (withParameterFile: bool) =
            let contracts = [|
                SimpleISA.Investigation.investigationReadContract
                SimpleISA.Study.bII_S_1ReadContract
                SimpleISA.Assay.proteomeReadContract
                SimpleISA.Workflow.proteomicsReadContract
                readContract workflowCwlPath DTOType.CWL TestObjects.CWL.RunWorkflowReference.StandardLayout.workflowCwlText
                readContract shoutPath DTOType.CWL TestObjects.CWL.RunWorkflowReference.StandardLayout.shoutCwlText
                SimpleISA.Run.proteomicsReadContract
                readContract runCwlPath DTOType.CWL TestObjects.CWL.RunWorkflowReference.StandardLayout.runCwlText
                if withParameterFile then readContract ymlPath DTOType.YAML "msg: hello\n"
            |]
            let arc = ARC("MyIdentifier")
            arc.SetISAFromContracts contracts
            arc
        let contractAt (path: string) (contracts: Contract []) = contracts |> Array.tryFind (fun c -> c.Path = path)
        let textOf (contract: Contract) =
            match contract.DTO with
            | Some (DTO.Text text) -> text
            | other -> failwith $"Expected text but got %A{other}"
        let workflowOf (description: ARCtrl.CWL.CWLProcessingUnit option) =
            match Expect.wantSome description "The CWL description should be present." with
            | ARCtrl.CWL.Workflow workflow -> workflow
            | other -> failwith $"Expected a workflow but got %A{other}"

        testCase "an ARC without changes updates none of the CWL files" (fun () ->
            let arc = readARC true
            let contracts = arc.GetUpdateContracts()
            for path in [ runCwlPath; ymlPath; workflowCwlPath ] do
                Expect.isNone (contractAt path contracts) $"{path} did not change."
        )
        testCase "an edited run description and parameter file are updated" (fun () ->
            let arc = readARC true
            let run = arc.Runs.[0]
            (workflowOf run.CWLDescription).Label <- Some (CWL.SchemaSaladString.Literal "Edited")
            run.CWLInput.[0].Value <- Some (ARCtrl.CWL.CWLParameterValue.String "changed")
            let contracts = arc.GetUpdateContracts()
            let cwlContract = Expect.wantSome (contractAt runCwlPath contracts) "The edited run.cwl should be updated."
            Expect.equal cwlContract.Operation Operation.UPDATE "run.cwl is updated."
            Expect.equal cwlContract.DTOType (Some DTOType.CWL) "run.cwl is typed as CWL."
            Expect.stringContains (textOf cwlContract) "label: Edited" "run.cwl holds the edit."
            Expect.stringContains (textOf cwlContract) "run: ../../workflows/Proteomics/workflow.cwl" "run.cwl keeps its reference."
            let ymlContract = Expect.wantSome (contractAt ymlPath contracts) "The edited run.yml should be updated."
            Expect.equal ymlContract.Operation Operation.UPDATE "run.yml is updated."
            Expect.equal ymlContract.DTOType (Some DTOType.YAML) "run.yml is typed as YAML."
            Expect.stringContains (textOf ymlContract) "msg: changed" "run.yml holds the edit."
            Expect.isNone (contractAt workflowCwlPath contracts) "The workflow did not change."
            Expect.isNone (contractAt (Identifier.Run.fileNameFromIdentifier run.Identifier) contracts) "The xlsx of the run did not change."
            Expect.equal (run.GetLightHashCode()) run.StaticHash "The hash of the xlsx ignores the CWL files."
            let again = arc.GetUpdateContracts()
            Expect.isNone (contractAt runCwlPath again) "A second call has nothing to update in run.cwl."
            Expect.isNone (contractAt ymlPath again) "A second call has nothing to update in run.yml."
        )
        testCase "an edited workflow description is updated" (fun () ->
            let arc = readARC false
            (workflowOf arc.Workflows.[0].CWLDescription).Label <- Some (CWL.SchemaSaladString.Literal "Edited")
            let contracts = arc.GetUpdateContracts()
            let cwlContract = Expect.wantSome (contractAt workflowCwlPath contracts) "The edited workflow.cwl should be updated."
            Expect.equal cwlContract.Operation Operation.UPDATE "workflow.cwl is updated."
            Expect.stringContains (textOf cwlContract) "label: Edited" "workflow.cwl holds the edit."
            Expect.isNone (contractAt runCwlPath contracts) "The run did not change."
            Expect.isNone (contractAt (Identifier.Workflow.fileNameFromIdentifier arc.Workflows.[0].Identifier) contracts) "The xlsx of the workflow did not change."
        )
        testCase "a new parameter file is created and files that the run lost are deleted" (fun () ->
            let arc = readARC false
            let run = arc.Runs.[0]
            run.CWLInput.Add(ARCtrl.CWL.CWLParameterReference(key = "msg", value = ARCtrl.CWL.CWLParameterValue.String "hello"))
            let created = Expect.wantSome (contractAt ymlPath (arc.GetUpdateContracts())) "The new run.yml should be created."
            Expect.equal created.Operation Operation.CREATE "run.yml is new."
            Expect.stringContains (textOf created) "msg: hello" "run.yml holds the reference."
            run.CWLInput.Clear()
            run.CWLDescription <- None
            let contracts = arc.GetUpdateContracts()
            Expect.equal (contractAt ymlPath contracts) (Some (Contract.createDelete ymlPath)) "A run without parameter references loses run.yml."
            Expect.equal (contractAt runCwlPath contracts) (Some (Contract.createDelete runCwlPath)) "A run without a description loses run.cwl."
        )
        testCase "contracts that were written are the state for the next update" (fun () ->
            let arc = ARC("MyARC", title = "MyTitle")
            let run = ArcRun("MyRun", cwlDescription = ARCtrl.CWL.Decode.decodeCWLProcessingUnit TestObjects.CWL.RunWorkflowReference.StandardLayout.runCwlText)
            arc.AddRun(run)
            arc.GetWriteContracts() |> ignore
            let path = Identifier.Run.cwlFileNameFromIdentifier "MyRun"
            Expect.isNone (contractAt path (arc.GetUpdateContracts())) "Nothing changed since the write."
            (workflowOf run.CWLDescription).Label <- Some (CWL.SchemaSaladString.Literal "Edited")
            let contract = Expect.wantSome (contractAt path (arc.GetUpdateContracts())) "The edit should be updated."
            Expect.equal contract.Operation Operation.UPDATE "run.cwl exists already."
        )
        testCase "an edited input of a workflow description is updated" (fun () ->
            let arc = readARC false
            let workflow = workflowOf arc.Workflows.[0].CWLDescription
            match workflow.Inputs.[0] with
            | CWL.ListEntry.Value input -> input.Type_ <- Some CWL.CWLType.Int
            | other -> failwith $"Expected an input but got %A{other}"
            let contract = Expect.wantSome (contractAt workflowCwlPath (arc.GetUpdateContracts())) "The edited input should update workflow.cwl."
            Expect.equal contract.Operation Operation.UPDATE "workflow.cwl is updated."
            Expect.stringContains (textOf contract) "msg: int" "workflow.cwl holds the edit."
        )
        testCase "an input of a tool description that is edited through getOrCreateInputs is updated" (fun () ->
            let tool = ARCtrl.CWL.Decode.decodeCommandLineTool "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n  msg: string\noutputs: {}\n"
            let arc = ARC("MyARC", title = "MyTitle")
            arc.AddRun(ArcRun("MyRun", cwlDescription = ARCtrl.CWL.CommandLineTool tool))
            arc.GetWriteContracts() |> ignore
            let path = Identifier.Run.cwlFileNameFromIdentifier "MyRun"
            Expect.isNone (contractAt path (arc.GetUpdateContracts())) "Nothing changed since the write."
            for edit in [ (fun (input: CWL.CWLInput) -> input.Name <- "renamed"); (fun input -> input.Type_ <- Some CWL.CWLType.Int); (fun input -> input.Doc <- Some (CWL.DocText.literal "docs")) ] do
                match (CWL.CWLToolDescription.getOrCreateInputs tool).[0] with
                | CWL.ListEntry.Value input -> edit input
                | other -> failwith $"Expected an input but got %A{other}"
                let contract = Expect.wantSome (contractAt path (arc.GetUpdateContracts())) "The edited input should update run.cwl."
                Expect.equal contract.Operation Operation.UPDATE "run.cwl exists already."
        )
    ]
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
        let wb = updateDTO.AsSpreadsheet()
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
        let wb = updateDTO.AsSpreadsheet()
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
        let wb = studyUpdateDTO.AsSpreadsheet()
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
        let wb = assayUpdateDTO.AsSpreadsheet()
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
        let wb = updateDTO.AsSpreadsheet()
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
        let wb = invUpdateDTO.AsSpreadsheet()
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
        let wb = studyUpdateDTO.AsSpreadsheet()
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
        let parsedRun = XlsxController.Run.fromFsWorkbook (updateDTO.AsSpreadsheet())
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
        let parsedWorkflow = XlsxController.Workflow.fromFsWorkbook (updateDTO.AsSpreadsheet())
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
        let wb = updateDTO.AsSpreadsheet()
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
        let wb = runUpdateDTO.AsSpreadsheet()
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
        let wb = workflowUpdateDTO.AsSpreadsheet()
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
        let wb = updateDTO.AsSpreadsheet()
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
        // https://github.com/nfdi4plants/ARCtrl/issues/620
        testCaseCrossAsync "zeroByteWorkbooks" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_Load_ZeroByteWorkbooks"
            do! FileSystemHelper.deleteFileOrDirectoryAsync p
            let inv = ArcInvestigation("MyInvestigation", "BestTitle")
            inv.InitAssay("MyAssay") |> ignore
            inv.InitStudy("MyStudy") |> ignore
            inv.InitWorkflow("MyWorkflow") |> ignore
            inv.InitRun("MyRun") |> ignore
            let! writeResult = ARC.fromArcInvestigation(isa = inv).TryWriteAsync(p)
            Expect.wantOk writeResult "ARC should write successfully" |> ignore
            // Simulate interrupted entity creations, which leave zero-byte workbooks behind
            do! FileSystemHelper.writeFileTextAsync (ArcPathHelper.combine p "assays/MyAssay/isa.assay.xlsx") ""
            do! FileSystemHelper.writeFileTextAsync (ArcPathHelper.combine p "studies/MyStudy/isa.study.xlsx") ""
            do! FileSystemHelper.writeFileTextAsync (ArcPathHelper.combine p "workflows/MyWorkflow/isa.workflow.xlsx") ""
            do! FileSystemHelper.writeFileTextAsync (ArcPathHelper.combine p "runs/MyRun/isa.run.xlsx") ""

            let! result = ARC.tryLoadAsync(p)
            let result = Expect.wantOk result "ARC with zero-byte workbooks should load successfully"
            Expect.sequenceEqual result.AssayIdentifiers ["MyAssay"] "Assay should be recovered from its folder"
            Expect.sequenceEqual result.StudyIdentifiers ["MyStudy"] "Study should be recovered from its folder"
            Expect.sequenceEqual result.WorkflowIdentifiers ["MyWorkflow"] "Workflow should be recovered from its folder"
            Expect.sequenceEqual result.RunIdentifiers ["MyRun"] "Run should be recovered from its folder"

            // Updating the ARC should write the recovered workbooks to disk
            let! updateResult = result.TryUpdateAsync(p)
            Expect.wantOk updateResult "ARC should update successfully" |> ignore
            let! _ = FileSystemHelper.readFileXlsxAsync (ArcPathHelper.combine p "assays/MyAssay/isa.assay.xlsx")
            let! _ = FileSystemHelper.readFileXlsxAsync (ArcPathHelper.combine p "studies/MyStudy/isa.study.xlsx")
            let! _ = FileSystemHelper.readFileXlsxAsync (ArcPathHelper.combine p "workflows/MyWorkflow/isa.workflow.xlsx")
            let! _ = FileSystemHelper.readFileXlsxAsync (ArcPathHelper.combine p "runs/MyRun/isa.run.xlsx")
            ()
            }
        )
        // https://github.com/nfdi4plants/ARCtrl/issues/620
        testCaseCrossAsync "nonEmptyCorruptWorkbookFails" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_Load_CorruptWorkbook"
            do! FileSystemHelper.deleteFileOrDirectoryAsync p
            let inv = ArcInvestigation("MyInvestigation", "BestTitle")
            inv.InitAssay("MyAssay") |> ignore
            let! writeResult = ARC.fromArcInvestigation(isa = inv).TryWriteAsync(p)
            Expect.wantOk writeResult "ARC should write successfully" |> ignore
            do! FileSystemHelper.writeFileTextAsync (ArcPathHelper.combine p "assays/MyAssay/isa.assay.xlsx") "This is not a workbook"

            let! result = ARC.tryLoadAsync(p)
            Expect.isError result "Loading should fail for a non-empty corrupt workbook"
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
        // https://github.com/nfdi4plants/ARCtrl/issues/622
        testCaseCrossAsync "empty_AdditionalContracts" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_Write_AdditionalContracts"
            do! FileSystemHelper.deleteFileOrDirectoryAsync p
            let a = ARC("MyInvestigation")
            let additionalContracts = [|
                Contract.createCreate(".gitignore", DTOType.PlainText, DTO.Text "*.tmp")
                Contract.createCreate("notes/.gitkeep", DTOType.PlainText)
            |]

            let! result = a.TryWriteAsync(p, additionalContracts = additionalContracts)

            Expect.wantOk result "ARC should write successfully" |> ignore

            let expectedPaths =
                [
                    "/isa.investigation.xlsx";
                    "/assays/.gitkeep";
                    "/studies/.gitkeep";
                    "/runs/.gitkeep";
                    "/workflows/.gitkeep";
                    "/.gitignore";
                    "/notes/.gitkeep"
                ]
                |> List.sort

            let! paths =
                FileSystemHelper.getAllFilePathsAsync p
                |> map Seq.sort

            Expect.sequenceEqual paths expectedPaths "Additional files were not created correctly."
            let! gitignore = FileSystemHelper.readFileTextAsync (ArcPathHelper.combine p ".gitignore")
            Expect.equal gitignore "*.tmp" "Additional file content was not written correctly."
        })
        // https://github.com/nfdi4plants/ARCtrl/issues/622
        testCaseCrossAsync "empty_AdditionalContracts_WriteAsync" (crossAsync {
            let p = ArcPathHelper.combine TestObjects.IO.testResultsFolder "ARC_Write_AdditionalContracts_WriteAsync"
            do! FileSystemHelper.deleteFileOrDirectoryAsync p
            let a = ARC("MyInvestigation")
            let additionalContracts = [|
                Contract.createCreate(".gitignore", DTOType.PlainText, DTO.Text "*.tmp")
            |]

            do! a.WriteAsync(p, additionalContracts = additionalContracts)

            let! gitignoreExists = FileSystemHelper.fileExistsAsync (ArcPathHelper.combine p ".gitignore")
            Expect.isTrue gitignoreExists "Additional file was not created."
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
                    "/assays/.gitkeep";
                    $"/assays/{existingAssayName}/isa.assay.xlsx"
                    $"/assays/{existingAssayName}/README.md"
                    $"/assays/{existingAssayName}/isa.datamap.xlsx"
                    $"/assays/{existingAssayName}/protocols/.gitkeep"
                    $"/assays/{existingAssayName}/dataset/.gitkeep"
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

let private createSingleRowTableNameTables () =
    let headers =
        ResizeArray [
            CompositeHeader.Input IOType.Source
            CompositeHeader.ProtocolREF
            CompositeHeader.Output IOType.Sample
        ]
    let createRow (source : string) (protocol : string) (sample : string) =
        ResizeArray [
            CompositeCell.createFreeText source
            CompositeCell.createFreeText protocol
            CompositeCell.createFreeText sample
        ]
    ResizeArray [
        ArcTable.createFromRows("my-sheet", headers, ResizeArray [createRow "source1" "My protocol" "sample1"])
        ArcTable.createFromRows(
            "my-sheet-2rows",
            headers,
            ResizeArray [
                createRow "source5" "My protocol" "sample5"
                createRow "source6" "My protocol" "sample6"
            ]
        )
        ArcTable.createFromRows("my-sheet_1", headers, ResizeArray [createRow "source7" "My protocol" "sample7"])
        ArcTable.createFromRows(
            "two-rows",
            headers,
            ResizeArray [
                createRow "source2" "Other protocol" "sample2"
                createRow "source3" "Other protocol" "sample3"
            ]
        )
        ArcTable.createFromRows(
            "step_2",
            ResizeArray [CompositeHeader.Input IOType.Source; CompositeHeader.Output IOType.Sample],
            ResizeArray [ResizeArray [CompositeCell.createFreeText "source4"; CompositeCell.createFreeText "sample4"]]
        )
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
            // https://github.com/nfdi4plants/ARCtrl/issues/651
            testCase "Full" <| fun _ ->
                let createDatamap() =
                    Datamap(ResizeArray [
                        ROCrateConversion.Tests.Helper.create_dataContextOfRow "MyFile" 1
                        ROCrateConversion.Tests.Helper.create_dataContextOfRow "MyFile" 2
                    ])
                let investigation = ROCrateConversion.Tests.Helper.create_investigation_toplevel()
                let study = ROCrateConversion.Tests.Helper.create_study_full()
                study.Datamap <- Some (createDatamap())
                // The full test objects use json encoded dates, which are not read from xlsx, so plain date strings are used instead
                investigation.SubmissionDate <- Some "2024-01-01T12:00:00"
                investigation.PublicReleaseDate <- Some "2024-02-01T12:00:00"
                study.SubmissionDate <- Some "2024-01-01T12:00:00"
                study.PublicReleaseDate <- Some "2024-02-01T12:00:00"
                let assay = ROCrateConversion.Tests.Helper.create_assay_full()
                assay.Datamap <- Some (createDatamap())
                let workflow = ROCrateConversion.Tests.Helper.create_workflow_full()
                workflow.Datamap <- Some (createDatamap())
                let run = ROCrateConversion.Tests.Helper.create_run_full()
                run.Datamap <- Some (createDatamap())
                investigation.AddStudy(study)
                investigation.AddAssay(assay, registerIn = ResizeArray [study])
                investigation.AddWorkflow(workflow)
                investigation.AddRun(run)
                let arcExpected = ARC.fromArcInvestigation(investigation, license = License.initFulltext "CC-BY-4.0")
                let json = arcExpected.ToROCrateJsonString()
                let arcActual = ARC.fromROCrateJsonString(json)
                // Data paths are read with leading ./, see https://github.com/nfdi4plants/ARCtrl/issues/615
                for datamap in [study.Datamap; assay.Datamap; workflow.Datamap; run.Datamap] do
                    datamap.Value.DataContexts |> Seq.iter (fun dc -> dc.FilePath <- Some ("./" + dc.FilePath.Value))
                Expect.equal arcActual.Identifier arcExpected.Identifier "Identifier should be equal"
                Expect.equal arcActual.Title arcExpected.Title "Title should be equal"
                Expect.equal arcActual.Description arcExpected.Description "Description should be equal"
                Expect.equal arcActual.SubmissionDate arcExpected.SubmissionDate "SubmissionDate should be equal"
                Expect.equal arcActual.PublicReleaseDate arcExpected.PublicReleaseDate "PublicReleaseDate should be equal"
                Expect.sequenceEqual arcActual.Publications arcExpected.Publications "Publications should be equal"
                Expect.sequenceEqual arcActual.Contacts arcExpected.Contacts "Contacts should be equal"
                Expect.sequenceEqual arcActual.Comments arcExpected.Comments "Comments should be equal"
                Expect.equal arcActual.License arcExpected.License "License should be equal"
                Expect.sequenceEqual arcActual.Studies arcExpected.Studies "Studies should be equal"
                Expect.sequenceEqual arcActual.Assays arcExpected.Assays "Assays should be equal"
                Expect.sequenceEqual arcActual.Workflows arcExpected.Workflows "Workflows should be equal"
                Expect.sequenceEqual arcActual.Runs arcExpected.Runs "Runs should be equal"
            testCase "NoLicense" <| fun _ ->
                let arcExpected = ARC("MyARC", title = "MyTitle", description = "MyDescription")
                let json = arcExpected.ToROCrateJsonString()
                let arcActual = ARC.fromROCrateJsonString(json)
                Expect.equal arcActual.Identifier arcExpected.Identifier "Identifier should be equal"
                Expect.equal arcActual.Title arcExpected.Title "Title should be equal"
                Expect.equal arcActual.Description arcExpected.Description "Description should be equal"
                Expect.isNone arcActual.License "License should be None"
            testCase "SingleRowTableNames" <| fun _ ->
                let assay = ArcAssay("my-assay", tables = createSingleRowTableNameTables ())
                let arcExpected = ARC("MyARC", title = "MyTitle", assays = ResizeArray [assay])
                let json = arcExpected.ToROCrateJsonString()
                let arcActual = ARC.fromROCrateJsonString(json)
                Expect.equal arcActual.AssayCount 1 "ARC should contain 1 assay"
                let actualAssay = arcActual.GetAssay("my-assay")
                let names = actualAssay.Tables |> Seq.map (fun t -> t.Name) |> List.ofSeq
                Expect.equal names ["my-sheet"; "my-sheet-2rows"; "my-sheet_1"; "two-rows"; "step_2"] "Table names should survive the round trip"
        ]
                
        testCase "CanRead_Deprecated" <| fun _ ->
            let arc = ARC.fromDeprecatedROCrateJsonString(TestObjects.ROCrate.ArcPrototypeDeprecated.ed123499)
            let nonDeprecatedARC = ARC.fromROCrateJsonString(TestObjects.ROCrate.ArcPrototype.ed123499)
            Expect.equal arc.Identifier nonDeprecatedARC.Identifier "Investigation should have correct identifier"
            Expect.equal arc.Title nonDeprecatedARC.Title "Investigation should have correct title"
            Expect.equal arc.Description nonDeprecatedARC.Description "Investigation should have correct description"
            Expect.equal arc.Contacts.[1] nonDeprecatedARC.Contacts.[1] "Investigation should have correct contacts"
            nonDeprecatedARC.Studies.[1].Comments <- ResizeArray() // remove comments for comparison as dateModified is now being preserved through comments
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
        // to-do: check in again
        ptestCase "ReadWriteEquality" <| fun _ ->
            let arc = ARC.fromROCrateJsonString(TestObjects.ROCrate.ArcPrototype.ed123499)
            let roCrate = arc.ToROCrateJsonString(2)
            Expect.equal roCrate TestObjects.ROCrate.ArcPrototype.ed123499 "RO-Crate JSON should be equal"
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
            // Data paths are read with leading ./, see https://github.com/nfdi4plants/ARCtrl/issues/615
            table.UpdateCellAt(0, 0, CompositeCell.createDataFromString("./assays/MyAssay/dataset/MyData.csv"))
            table.UpdateCellAt(0, 1, CompositeCell.createDataFromString("./assays/MyAssay/dataset/ABC.D"))
            Expect.testFileSystemTree arc'.FileSystem.Tree arc.FileSystem.Tree
            Expect.equal arc'.Assays[0] arc.Assays[0] "Assays should be equal"
        // https://github.com/nfdi4plants/ARCtrl/issues/650
        testCase "ExternalFilesNotInFilesystem" <| fun _ ->
            let arc = ARC("MyARC", title = "MyTitle", description = "MyDescription")
            let assay = arc.InitAssay("MyAssay")
            arc.UpdateFileSystem()
            arc.FileSystem <- arc.FileSystem.AddFile("assays/MyAssay/dataset/MyData.csv")
            let table = assay.InitTable("MyTable")
            table.AddColumn(
                CompositeHeader.Input IOType.Data,
                ResizeArray [
                    CompositeCell.createDataFromString("assays/MyAssay/dataset/MyData.csv")
                    CompositeCell.createDataFromString("https://www.ncbi.nlm.nih.gov/datasets/genome/GCF_001406875.1/")
                ]
            ) |> ignore
            let roCrate = arc.ToROCrateJsonString()
            let arc' = ARC.fromROCrateJsonString(roCrate)
            let paths = arc'.FileSystem.Tree.ToFilePaths(true)
            Expect.isFalse (paths |> Array.exists (fun p -> p.StartsWith "https:")) "External file should not be part of the filesystem"
            Expect.testFileSystemTree arc'.FileSystem.Tree arc.FileSystem.Tree
            // Data paths are read with leading ./, see https://github.com/nfdi4plants/ARCtrl/issues/615
            table.UpdateCellAt(0, 0, CompositeCell.createDataFromString("./assays/MyAssay/dataset/MyData.csv"))
            Expect.equal arc'.Assays[0] arc.Assays[0] "Assays should be equal, including the external file reference"
        // https://github.com/nfdi4plants/ARCtrl/issues/593
        testCase "EmptyDataCellNotMadeAbsolute" <| fun _ ->
            let arc = ARC("MyARC", title = "MyTitle")
            let assay = arc.InitAssay("MyAssay")
            assay.Title <- Some "My Assay"
            let table = assay.InitTable("MyTable")
            table.AddColumn(
                CompositeHeader.Input IOType.Data,
                ResizeArray [
                    CompositeCell.createDataFromString("assays/MyAssay/dataset/MyData.csv")
                    CompositeCell.createDataFromString("")
                ]
            ) |> ignore
            let roCrate = arc.ToROCrateJsonString()
            Expect.equal (table.GetCellAt(0, 1).AsData.FilePath) (Some "") "Empty data cell should not be changed by writing"
            let graph = Json.Decode.fromJsonString Json.LDGraph.decoder roCrate
            Expect.isNone (graph.TryGetNode "./assays/MyAssay/dataset/") "Empty data cell should not result in a dataset folder entity"
            Expect.isNone (graph.TryGetNode "assays/MyAssay/dataset/") "Empty data cell should not result in a dataset folder entity"
        // https://github.com/nfdi4plants/ARCtrl/issues/615
        testCase "NormalizeDataEntityIds" <| fun _ ->
            let arc = ARC("MyARC", title = "MyTitle")
            let assay = arc.InitAssay("MyAssay")
            assay.Title <- Some "My Assay"
            let table = assay.InitTable("MyTable")
            table.AddColumn(
                CompositeHeader.Input IOType.Data,
                ResizeArray [CompositeCell.createDataFromString("assays/MyAssay/dataset/MyData.csv")]
            ) |> ignore
            let roCrate = arc.ToROCrateJsonString()
            // Decode without normalization, to check the written ids
            let graph = Json.Decode.fromJsonString Json.LDGraph.decoder roCrate
            let context = graph.TryGetContext()
            Expect.isNone (graph.TryGetNode "assays/MyAssay/dataset/MyData.csv") "File without leading ./ should not be written"
            let file = Expect.wantSome (graph.TryGetNode "./assays/MyAssay/dataset/MyData.csv") "File should be written with leading ./"
            Expect.equal (ROCrate.LDFile.getNameAsString(file, ?context = context)) "./assays/MyAssay/dataset/MyData.csv" "File name should be written with leading ./"
            Expect.isNone (graph.TryGetNode "assays/MyAssay/") "Assay dataset without leading ./ should not be written"
            let assayNode = Expect.wantSome (graph.TryGetNode "./assays/MyAssay/") "Assay dataset should be written with leading ./"
            Expect.equal (ROCrate.LDDataset.getNameAsString(assayNode, ?context = context)) "My Assay" "Assay name should not be changed"
        testCase "PersonGetsMergedOneORCID" <| fun _ ->
            let pWithORCID = Person(firstName = "John", lastName = "Doe", orcid = "0000-0003-3925-6778", email = "john.doe@example.com")
            let pWithoutORCID = Person(firstName = "John", lastName = "Doe", phone = "123456")
            let arc = ARC("MyInvestigation", title = "MyTitle")
            let assay = arc.InitAssay("MyAssay")
            assay.Performers.Add(pWithORCID)
            arc.Contacts.Add(pWithoutORCID)
            let roCrate = arc.ToROCrateJsonString()
            let graph = JsonController.LDGraph.fromROCrateJsonString(roCrate)
            let p1O = graph.TryGetNode(ROCrate.LDPerson.genId(givenName = "John", familyName = "Doe", orcid = "0000-0003-3925-6778"))
            let p1 = Expect.wantSome p1O "Person with ORCID should be present in graph"
            let email = Expect.wantSome (ROCrate.LDPerson.tryGetEmailAsString(p1, ?context = graph.TryGetContext())) "Person with ORCID should have email"
            Expect.equal email "john.doe@example.com" "Person with ORCID should have correct email"
            let phone = Expect.wantSome (ROCrate.LDPerson.tryGetTelephoneAsString(p1, ?context = graph.TryGetContext())) "Person with ORCID should have phone"
            Expect.equal phone "123456" "Person with ORCID should have correct phone"
            let p2 = graph.TryGetNode(ROCrate.LDPerson.genId(givenName = "John", familyName = "Doe"))
            Expect.isNone p2 "Person without ORCID should not be present in graph"
        testCase "PersonDoesNotGetMergedWithPublication" <| fun _ ->
            let p = Person(firstName = "John", lastName = "Doe")
            let pub = Publication(title = "MyPublication", authors = "John Doe")
            let arc = ARC("MyInvestigation", title = "MyTitle")
            arc.Contacts.Add(p)
            arc.Publications.Add(pub)
            let roCrate = arc.ToROCrateJsonString()
            let graph = JsonController.LDGraph.fromROCrateJsonString(roCrate)
            let p1O = graph.TryGetNode(ROCrate.LDPerson.genId(givenName = "John", familyName = "Doe"))
            let p1 = Expect.wantSome p1O "Person should be present in graph"
            let givenNames = p1.GetPropertyValues(ROCrate.LDPerson.givenName, ?context = graph.TryGetContext())
            Expect.hasLength givenNames 1 "Person should have one given name"
            let gn = try givenNames.[0] :?> string |> Some with | _ -> None
            Expect.equal gn (Some "John") "Person should have correct given name"
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


