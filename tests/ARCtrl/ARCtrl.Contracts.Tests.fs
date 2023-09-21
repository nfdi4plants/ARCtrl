module ARCtrl.Contracts.Tests

open TestingUtils

open ARCtrl
open ARCtrl.Contract
open FsSpreadsheet

let tests_tryFromContract = testList "tryFromContract" [
    testCase "Exists" <| fun _ ->
        let fswb = new FsWorkbook()
        let contracts = [|
            Contract.create(
                READ, 
                "isa.investigation.xlsx", 
                DTOType.ISA_Investigation, 
                DTO.Spreadsheet fswb
            )
        |]
        let investigation = contracts |> Array.choose ArcInvestigation.tryFromContract
        Expect.hasLength investigation 1 ""
]

let tests_gitContracts = testList "gitContracts" [
    testCase "init_basic" <| fun _ ->
        let arc = ARC()
        let contracts = arc.GetGitInitContracts()
        Expect.equal contracts.Length 1 "Should be one contract"
        Expect.equal contracts.[0].Operation Operation.EXECUTE "Should be an execute operation"
        Expect.isSome contracts.[0].DTOType "Should have a DTO type"
        Expect.equal contracts.[0].DTOType.Value DTOType.CLI "Should be a CLI DTO"
        Expect.equal contracts.[0].Path "" "Should have an empty path"
        Expect.isSome contracts.[0].DTO "Should have a DTO"
        let dto = contracts.[0].DTO.Value
        Expect.isTrue dto.isCLITool "Should be a CLI tool"
        let cli = dto.AsCLITool()
        Expect.equal cli.Name "git" "Should be git"
        Expect.equal cli.Arguments.Length 3 "Should have two arguments"
        Expect.sequenceEqual cli.Arguments [|"init";"-b";"main"|] "Should be init"

    testCase "init_Branch" <| fun _ ->
        let arc = ARC()
        let branchName = "myBranch"
        let contracts = arc.GetGitInitContracts(branch = branchName)
        Expect.equal contracts.Length 1 "Should be one contract"
        let dto = contracts.[0].DTO.Value
        let cli = dto.AsCLITool()
        Expect.sequenceEqual cli.Arguments [|"init";"-b";branchName|] "Should have new branchname"

    testCase "init_remoteRepository" <| fun _ ->
        let arc = ARC()
        let remote = @"www.fantasyGit.net/MyAccount/MyRepo"
        let contracts = arc.GetGitInitContracts(repositoryAddress = remote)
        Expect.equal contracts.Length 2 "Should be two contracts"
        let dto = contracts.[1].DTO.Value
        let cli = dto.AsCLITool()
        Expect.sequenceEqual cli.Arguments [|"remote";"add";"origin";remote|] "Should correctly set new remote"

    testCase "init_GitIgnore" <| fun _ ->
        let arc = ARC()
        let contracts = arc.GetGitInitContracts(defaultGitignore = true)
        Expect.equal contracts.Length 2 "Should be two contracts"
        Expect.equal contracts.[1].Operation Operation.CREATE "Should be an create operation"
        let dto = contracts.[1].DTO.Value
        Expect.isTrue dto.isText "Should be text"
        Expect.equal contracts.[1].Path ".gitignore" "Should be a gitignore"
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