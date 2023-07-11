module FileSystemTree.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open FileSystem

let private newArcRelativePaths = [|
    @"isa.investigation.xlsx"; @".arc\.gitkeep"; @".git\config";
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
    @"studies\TestAssay1\resources\.gitkeep"|]


let private tests_fromFilePaths =
    testList "fromFilePaths" [
        testCase "new arc (2023-07-11)" <| fun _ ->
            // /// ARC paths are read in .NET with
            //let readFilePaths (arcPath: string) = 
            //    System.IO.Directory.EnumerateFiles(arcPath,"*",SearchOption.AllDirectories)
            //    |> Array.ofSeq 
            //    |> Array.map (fun p -> Path.GetRelativePath(rootPath, p))
            let filest = FileSystemTree.fromFilePaths newArcRelativePaths
            let expected = Folder("root",[|
                File "isa.investigation.xlsx"; 
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
                    Folder("est",[|
                        File "isa.assay.xlsx"; File "README.md";
                        Folder ("dataset", [|File ".gitkeep"|]);
                        Folder ("protocols", [|File ".gitkeep"|])
                    |]);
                    Folder
                      ("TestAssay1",[|
                        File "isa.assay.xlsx"; File "README.md";
                        Folder ("dataset", [|File ".gitkeep"|]);
                        Folder ("protocols", [|File ".gitkeep"|])
                    |])
                |]);
                Folder("runs", [|File ".gitkeep"|]);
                Folder("studies",[|
                    File ".gitkeep";
                    Folder("est",[|
                        File "isa.study.xlsx"; File "README.md";
                        Folder ("protocols", [|File ".gitkeep"|]);
                        Folder ("resources", [|File ".gitkeep"|])
                    |]);
                    Folder("MyStudy",[|
                        File "isa.study.xlsx"; File "README.md";
                        Folder ("protocols", [|File ".gitkeep"|]);
                        Folder ("resources", [|File ".gitkeep"|])
                    |]);
                    Folder("TestAssay1",[|
                        File "isa.study.xlsx"; File "README.md";
                        Folder ("protocols", [|File ".gitkeep"|]);
                        Folder ("resources", [|File ".gitkeep"|])
                    |])
                |]);
                Folder ("workflows", [|File ".gitkeep"|])
            |])
            // damn... i made this, because i thought equal would somehow not work. But i missed adding "@" in front of paths.
            // i'll leave it for improved error message.
            TestingUtils.testFileSystemTree filest expected 
            Expect.equal filest expected "isEqual"
    ]

let main = testList "FileSystemTree" [
    tests_fromFilePaths
]
