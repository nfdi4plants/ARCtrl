module ARCtrl.Contracts.Tests

open TestingUtils

open ARCtrl
open ARCtrl
open ARCtrl.Contract
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
    testCase "DataMap" <| fun _ ->
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
        let datamap = ARCAux.getAssayDataMapFromContracts assayName contracts
        Expect.isSome datamap "Datamap should have been parsed"
    testCase "DataMap_WrongIdentifier" <| fun _ ->
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
        let datamap = ARCAux.getAssayDataMapFromContracts "wrongAssay" contracts
        Expect.isNone datamap "Datamap should not have been parsed"
    testCase "DataMap_WrongType" <| fun _ ->
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
        let datamap = ARCAux.getAssayDataMapFromContracts assayName contracts
        Expect.isNone datamap "Datamap should not have been parsed"
    testCase "DataMap_WrongPath" <| fun _ ->
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
        let datamap = ARCAux.getAssayDataMapFromContracts assayName contracts
        Expect.isNone datamap "Datamap should not have been parsed"
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