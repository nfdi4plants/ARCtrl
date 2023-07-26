module ARCtrl.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open ARCtrl

let private test_model = testList "model" [
    testCase "create" <| fun _ ->
        let arc = ARC.create()
        Expect.isNone arc.CWL "cwl"
        Expect.isNone arc.ISA "isa"
        Expect.isNone arc.FileSystem "fs"
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
        Expect.isSome arc.FileSystem "isSome fs"
        let actualFilePaths = arc.FileSystem.Value.Tree.ToFilePaths() |> Array.sort
        Expect.equal actualFilePaths input "isSome fs"
]

let main = testList "main" [
    test_model
]