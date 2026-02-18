module ARCtrl.Contracts.Tests

open TestingUtils

open ARCtrl
open ARCtrl
open ARCtrl.Contract
open ARCtrl.Helper
open FsSpreadsheet

let tests_tryFromContract = testList "tryFromContract" [
    testCase "Exists" <| fun _ ->
        let fswb = TestObjects.Spreadsheet.Investigation.BII_I_1.fullInvestigation
        let contracts = [|
            Contract.create(
                READ, 
                "isa.investigation.xlsx", 
                DTOType.ISA_Investigation, 
                DTO.Spreadsheet fswb
            )
        |]
        let investigation = contracts |> Array.choose ArcInvestigation.tryFromReadContract
        Expect.hasLength investigation 1 ""
    testCase "Datamap" <| fun _ ->
        let fswb = TestObjects.Contract.ISA.SimpleISA.Assay.proteomeDatamapWB
        let assayName = "myAssay"
        let contracts = [|
            Contract.create(
                READ, 
                $"assays/{assayName}/isa.datamap.xlsx", 
                DTOType.ISA_Datamap, 
                DTO.Spreadsheet fswb
            )
        |]
        let datamap = ARCAux.getAssayDatamapFromContracts assayName contracts
        Expect.isSome datamap "Datamap should have been parsed"
    testCase "Datamap_WrongIdentifier" <| fun _ ->
        let fswb = TestObjects.Contract.ISA.SimpleISA.Assay.proteomeDatamapWB
        let assayName = "myAssay"
        let contracts = [|
            Contract.create(
                READ, 
                $"assays/{assayName}/isa.datamap.xlsx", 
                DTOType.ISA_Datamap, 
                DTO.Spreadsheet fswb
            )
        |]
        let datamap = ARCAux.getAssayDatamapFromContracts "wrongAssay" contracts
        Expect.isNone datamap "Datamap should not have been parsed"
    testCase "Datamap_WrongType" <| fun _ ->
        let fswb = TestObjects.Contract.ISA.SimpleISA.Assay.proteomeDatamapWB
        let assayName = "myAssay"
        let contracts = [|
            Contract.create(
                READ, 
                $"assays/{assayName}/isa.datamap.xlsx", 
                DTOType.ISA_Assay, 
                DTO.Spreadsheet fswb
            )
        |]
        let datamap = ARCAux.getAssayDatamapFromContracts assayName contracts
        Expect.isNone datamap "Datamap should not have been parsed"
    testCase "Datamap_WrongPath" <| fun _ ->
        let fswb = TestObjects.Contract.ISA.SimpleISA.Assay.proteomeDatamapWB
        let assayName = "myAssay"
        let contracts = [|
            Contract.create(
                READ, 
                $"studies/{assayName}/isa.datamap.xlsx", 
                DTOType.ISA_Datamap, 
                DTO.Spreadsheet fswb
            )
        |]
        let datamap = ARCAux.getAssayDatamapFromContracts assayName contracts
        Expect.isNone datamap "Datamap should not have been parsed"
    testCase "WorkflowCWL" <| fun _ ->
        let cwl = TestObjects.CWL.CommandLineTool.cwlFile
        let workflowName = "myWorkflow"
        let contracts = [|
            Contract.create(
                READ, 
                $"workflows/{workflowName}/workflow.cwl", 
                DTOType.CWL, 
                DTO.Text cwl
            )
            |]
        let workflow = ARCAux.getWorkflowCWLFromContracts workflowName contracts
        Expect.isSome workflow "Workflow should have been parsed"
    testCase "WorkflowCWL_WrongIdentifier" <| fun _ ->
        let cwl = TestObjects.CWL.CommandLineTool.cwlFile
        let workflowName = "myWorkflow"
        let contracts = [|
            Contract.create(
                READ, 
                $"workflows/{workflowName}/workflow.cwl", 
                DTOType.CWL, 
                DTO.Text cwl
            )
            |]
        let workflow = ARCAux.getWorkflowCWLFromContracts "wrongWorkflow" contracts
        Expect.isNone workflow "Workflow should not have been parsed"
    testCase "WorkflowCWL_WrongType" <| fun _ ->
        let cwl = TestObjects.CWL.CommandLineTool.cwlFile
        let workflowName = "myWorkflow"
        let contracts = [|
            Contract.create(
                READ, 
                $"workflows/{workflowName}/workflow.cwl", 
                DTOType.ISA_Assay, 
                DTO.Text cwl
            )
            |]
        let workflow = ARCAux.getWorkflowCWLFromContracts workflowName contracts
        Expect.isNone workflow "Workflow should not have been parsed"
    testCase "WorkflowCWL_WrongPath" <| fun _ ->
        let cwl = TestObjects.CWL.CommandLineTool.cwlFile
        let workflowName = "myWorkflow"
        let contracts = [|
            Contract.create(
                READ, 
                $"workflows/{workflowName}/workflow.json", 
                DTOType.CWL, 
                DTO.Text cwl
            )
            |]
        let workflow = ARCAux.getWorkflowCWLFromContracts workflowName contracts
        Expect.isNone workflow "Workflow should not have been parsed"
    testCase "RunCWL" <| fun _ ->
        let cwl = TestObjects.CWL.Workflow.workflowFile
        let runName = "myRun"
        let contracts = [|
            Contract.create(
                READ, 
                $"runs/{runName}/run.cwl", 
                DTOType.CWL, 
                DTO.Text cwl
            )
            |]
        let run = ARCAux.getRunCWLFromContracts runName contracts
        Expect.isSome run "Run should have been parsed"
    testCase "RunCWL_WrongIdentifier" <| fun _ ->
        let cwl = TestObjects.CWL.Workflow.workflowFile
        let runName = "myRun"
        let contracts = [|
            Contract.create(
                READ, 
                $"runs/{runName}/run.cwl", 
                DTOType.CWL, 
                DTO.Text cwl
            )
            |]
        let run = ARCAux.getRunCWLFromContracts "wrongRun" contracts
        Expect.isNone run "Run should not have been parsed"
    testCase "WorkflowCWL_OperationContractRoundtrip" <| fun _ ->
        let workflowIdentifier = "operationWorkflow"
        let operationText = TestObjects.CWL.Operation.operationWithRequirementsAndMetadataFile
        let operationProcessingUnit = CWL.Decode.decodeCWLProcessingUnit operationText
        let workflow = ArcWorkflow(identifier = workflowIdentifier, cwlDescription = operationProcessingUnit)

        let cwlPath = Identifier.Workflow.cwlFileNameFromIdentifier workflowIdentifier
        let createContract =
            workflow.ToCreateContract()
            |> Array.tryFind (fun c -> c.Path = cwlPath)
            |> fun c -> Expect.wantSome c "Workflow should create a CWL contract"

        Expect.equal createContract.Operation Operation.CREATE "Contract should be a create contract"
        Expect.equal createContract.DTOType (Some DTOType.CWL) "Contract should be typed as CWL"
        let createdCwlText =
            let dto = Expect.wantSome createContract.DTO "CWL contract should include text payload"
            dto.AsText()

        let readContract = { createContract with Operation = Operation.READ }
        let decoded =
            ArcWorkflow.tryCWLFromReadContract workflowIdentifier readContract
            |> fun cwl -> Expect.wantSome cwl "Workflow operation CWL should decode from read contract"

        Expect.isTrue
            (match decoded with | CWL.Operation _ -> true | _ -> false)
            (sprintf "Expected decoded processing unit to be Operation but got %A" decoded)
        Expect.equal
            (CWL.Encode.encodeProcessingUnit decoded)
            createdCwlText
            "Decoded processing unit should encode to the created CWL payload"
    testCase "RunCWL_OperationContractRoundtrip" <| fun _ ->
        let runIdentifier = "operationRun"
        let operationText = TestObjects.CWL.Operation.operationWithRequirementsAndMetadataFile
        let operationProcessingUnit = CWL.Decode.decodeCWLProcessingUnit operationText
        let run = ArcRun(identifier = runIdentifier, cwlDescription = operationProcessingUnit)

        let cwlPath = Identifier.Run.cwlFileNameFromIdentifier runIdentifier
        let createContract =
            run.ToCreateContract()
            |> Array.tryFind (fun c -> c.Path = cwlPath)
            |> fun c -> Expect.wantSome c "Run should create a CWL contract"

        Expect.equal createContract.Operation Operation.CREATE "Contract should be a create contract"
        Expect.equal createContract.DTOType (Some DTOType.CWL) "Contract should be typed as CWL"
        let createdCwlText =
            let dto = Expect.wantSome createContract.DTO "CWL contract should include text payload"
            dto.AsText()

        let readContract = { createContract with Operation = Operation.READ }
        let decoded =
            ArcRun.tryCWLFromReadContract runIdentifier readContract
            |> fun cwl -> Expect.wantSome cwl "Run operation CWL should decode from read contract"

        Expect.isTrue
            (match decoded with | CWL.Operation _ -> true | _ -> false)
            (sprintf "Expected decoded processing unit to be Operation but got %A" decoded)
        Expect.equal
            (CWL.Encode.encodeProcessingUnit decoded)
            createdCwlText
            "Decoded processing unit should encode to the created CWL payload"
]

let tests_gitContracts = testList "gitContracts" [
    testCase "init_basic" <| fun _ ->
        let arc = ARC("MyARC")
        let contracts = arc.GetGitInitContracts()
        Expect.equal contracts.Length 1 "Should be one contract"
        /// Check init contract
        Expect.equal contracts.[0].Operation Operation.EXECUTE "Should be an execute operation"
        Expect.isSome contracts.[0].DTOType "Should have a DTO type"
        Expect.equal contracts.[0].DTOType.Value DTOType.CLI "Should be a CLI DTO"
        Expect.equal contracts.[0].Path "" "Should have an empty path"
        let dto = Expect.wantSome contracts.[0].DTO "Should have a DTO"
        Expect.isTrue dto.isCLITool "Should be a CLI tool"
        let cli = dto.AsCLITool()
        Expect.equal cli.Name "git" "Should be git"
        Expect.equal cli.Arguments.Length 3 "Should have three arguments"
        Expect.sequenceEqual cli.Arguments [|"init";"-b";"main"|] "Should be init"
        /// Check gitattributes contract
        //Expect.equal contracts.[1].Operation Operation.CREATE "Should be an create operation"
        //let dtoType = Expect.wantSome contracts.[1].DTOType "Should have a DTO type"
        //Expect.equal dtoType DTOType.PlainText "Should be a plain text"
        //Expect.equal contracts.[1].Path ".gitattributes" "Should have a path"
        //let dto = Expect.wantSome contracts.[1].DTO "Gitattributes contract should have a DTO"
        //Expect.isTrue dto.isText "Should be text"
        //let text = dto.AsText()
        //Expect.equal text "**/dataset/**" "Should have the correct text"

    testCase "init_Branch" <| fun _ ->
        let arc = ARC("MyARC")
        let branchName = "myBranch"
        let contracts = arc.GetGitInitContracts(branch = branchName)
        Expect.equal contracts.Length 1 "Should be one contract"
        let dto = Expect.wantSome contracts.[0].DTO "Should have a DTO"
        let cli = dto.AsCLITool()
        Expect.sequenceEqual cli.Arguments [|"init";"-b";branchName|] "Should have new branchname"

    testCase "init_remoteRepository" <| fun _ ->
        let arc = ARC("MyARC")
        let remote = @"www.fantasyGit.net/MyAccount/MyRepo"
        let contracts = arc.GetGitInitContracts(repositoryAddress = remote)
        Expect.equal contracts.Length 2 "Should be two contracts"
        let dto = Expect.wantSome contracts.[1].DTO "Should have a DTO"
        let cli = dto.AsCLITool()
        Expect.sequenceEqual cli.Arguments [|"remote";"add";"origin";remote|] "Should correctly set new remote"

    testCase "init_GitIgnore" <| fun _ ->
        let arc = ARC("MyARC")
        let contracts = arc.GetGitInitContracts(defaultGitignore = true)
        Expect.equal contracts.Length 2 "Should be two contracts"
        Expect.equal contracts.[1].Operation Operation.CREATE "Should be an create operation"
        let dto = Expect.wantSome contracts.[1].DTO "Should have a DTO"
        Expect.isTrue dto.isText "Should be text"
        Expect.equal contracts.[1].Path ".gitignore" "Should be a gitignore"
    testCase "init_GitAttributes" <| fun _ ->
        let arc = ARC("MyARC")
        let contracts = arc.GetGitInitContracts(defaultGitattributes = true)
        Expect.equal contracts.Length 2 "Should be two contracts"
        Expect.equal contracts.[1].Operation Operation.CREATE "Should be an create operation"
        let dtoType = Expect.wantSome contracts.[1].DTOType "Should have a DTO type"
        Expect.equal dtoType DTOType.PlainText "Should be a plain text"
        Expect.equal contracts.[1].Path ".gitattributes" "Should have a path"
        let dto = Expect.wantSome contracts.[1].DTO "Gitattributes contract should have a DTO"
        Expect.isTrue dto.isText "Should be text"
        let text = dto.AsText()
        Expect.equal text "**/dataset/** filter=lfs diff=lfs merge=lfs -text" "Should have the correct text"
    testCase "clone_AllOptions" <| fun _ ->
        let remoteURL = @"https://git.fantasyGit.net/MyAccount/MyRepo"
        let user = "Lukas"
        let token = "12345"
        let tokenFormattedURL = @$"https://{user}:{token}@git.fantasyGit.net/MyAccount/MyRepo"
        let branch = "myBranch"
        let noLFSConfig = "-c \"filter.lfs.smudge = git-lfs smudge --skip -- %f\" -c \"filter.lfs.process = git-lfs filter-process --skip\""
        let contract = ARC.getCloneContract(remoteURL,merge = true,branch = branch,token = (user,token),nolfs = true)
        let dto = contract.DTO.Value
        let cli = dto.AsCLITool()
        Expect.sequenceEqual cli.Arguments [|"clone";noLFSConfig;"-b";branch;tokenFormattedURL;"."|] "some option was wrong"
    ]


let main = testList "Contracts" [
    tests_tryFromContract
    tests_gitContracts
]
