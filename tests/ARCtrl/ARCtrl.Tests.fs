module ARCtrl.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open ARCtrl
open ARCtrl.ISA

let private test_model = testList "model" [
    testCase "create" <| fun _ ->
        let arc = ARC()
        Expect.isNone arc.CWL "cwl"
        Expect.isNone arc.ISA "isa"
    testCase "fromFilePath" <| fun _ ->
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
]

let private test_writeContracts = testList "write_contracts" [
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
        inv.InitStudy("MyStudy").InitAssay("MyAssay") |> ignore
        let arc = ARC(isa = inv)
        let contracts = arc.GetWriteContracts()
        let contractPathsString = contracts |> Array.map (fun c -> c.Path) |> String.concat ", "
        Expect.equal contracts.Length 13 $"Should contain exactly as much contracts as base folders but contained: {contractPathsString}"

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
]

let main = testList "main" [
    test_model
    test_writeContracts
]