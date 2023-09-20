module ARCtrl.FileSystemTree.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open ARCtrl.FileSystem
open TestingUtils

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


let newArcFST = 
    Folder("root",[|
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

let private tests_fromFilePaths =
    testList "fromFilePaths" [
        testCase "new arc (2023-07-11)" <| fun _ ->
            // /// ARC paths are read in .NET with
            //let readFilePaths (arcPath: string) = 
            //    System.IO.Directory.EnumerateFiles(arcPath,"*",SearchOption.AllDirectories)
            //    |> Array.ofSeq 
            //    |> Array.map (fun p -> Path.GetRelativePath(rootPath, p))
            let filest = FileSystemTree.fromFilePaths newArcRelativePaths
            // damn... i made this, because i thought equal would somehow not work. But i missed adding "@" in front of paths.
            // i'll leave it for improved error message.
            Expect.testFileSystemTree filest newArcFST 
            Expect.equal filest newArcFST "isEqual"
    ]

let private tests_ToFilePaths =
    testList "ToFilePaths" [
        testCase "new arc (2023-07-11)" <| fun _ ->
            let filest = FileSystemTree.fromFilePaths newArcRelativePaths
            let actual = filest.ToFilePaths()
            // Actual will be created with generic seperator `/` instead of windows `\\`
            let actual_sep = actual |> Array.map (fun p -> p.Replace('/', '\\'))
            let actualDifference = Array.except newArcRelativePaths actual_sep
            Expect.equal actualDifference Array.empty "should be empty"
            let actual_sorted = actual_sep |> Array.sort
            let expected_sorted = newArcRelativePaths |> Array.sort
            Expect.equal actual_sorted expected_sorted "equal"
    ]

let private tests_Filter_Files = 
    testList "FilterFiles" [
        test "new arc (2023-07-11), keep .xlsx files" {
            let expected = Some(
               Folder("root",[|
                    File "isa.investigation.xlsx"; 
                    Folder(".arc", [||]);
                    Folder(".git",[|
                        Folder("hooks",[||]);
                        Folder ("info", [||])
                    |]);
                    Folder("assays",[|
                        Folder("est",[|
                            File "isa.assay.xlsx"
                            Folder ("dataset", [||]);
                            Folder ("protocols", [||])
                        |]);
                        Folder
                            ("TestAssay1",[|
                            File "isa.assay.xlsx"
                            Folder ("dataset", [||]);
                            Folder ("protocols", [||])
                        |])
                    |]);
                    Folder("runs", [||]);
                    Folder("studies",[|
                        Folder("est",[|
                            File "isa.study.xlsx";
                            Folder ("protocols", [||]);
                            Folder ("resources", [||])
                        |]);
                        Folder("MyStudy",[|
                            File "isa.study.xlsx";
                            Folder ("protocols", [||]);
                            Folder ("resources", [||])
                        |]);
                        Folder("TestAssay1",[|
                            File "isa.study.xlsx";
                            Folder ("protocols", [||]);
                            Folder ("resources", [||])
                        |])
                    |]);
                    Folder ("workflows", [||])
                |])
            )
            let filest = FileSystemTree.fromFilePaths newArcRelativePaths
            let actualInstanceMethod = filest.FilterFiles (fun n -> n.EndsWith ".xlsx")
            let actualStaticMember = filest |> FileSystemTree.filterFiles (fun n -> n.EndsWith ".xlsx")
            Expect.equal actualInstanceMethod expected "instance member result incorrect."
            Expect.equal actualStaticMember expected "static member result incorrect."
        }
        test "new arc (2023-07-11), filter not startswith '.'" {
            let expected = Some(
                Folder("root",[|
                    File "isa.investigation.xlsx"; 
                    Folder(".arc", [||]);
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
                        Folder("est",[|
                            File "isa.assay.xlsx"; File "README.md";
                            Folder ("dataset", [||]);
                            Folder ("protocols", [||])
                        |]);
                        Folder
                            ("TestAssay1",[|
                            File "isa.assay.xlsx"; File "README.md";
                            Folder ("dataset", [||]);
                            Folder ("protocols", [||])
                        |])
                    |]);
                    Folder("runs", [||]);
                    Folder("studies",[|
                        Folder("est",[|
                            File "isa.study.xlsx"; File "README.md";
                            Folder ("protocols", [||]);
                            Folder ("resources", [||])
                        |]);
                        Folder("MyStudy",[|
                            File "isa.study.xlsx"; File "README.md";
                            Folder ("protocols", [||]);
                            Folder ("resources", [||])
                        |]);
                        Folder("TestAssay1",[|
                            File "isa.study.xlsx"; File "README.md";
                            Folder ("protocols", [||]);
                            Folder ("resources", [||])
                        |])
                    |]);
                    Folder ("workflows", [||])
                |])
            )
            let filest = FileSystemTree.fromFilePaths newArcRelativePaths
            let actualInstanceMethod = filest.FilterFiles (fun n -> not (n.StartsWith "."))
            let actualStaticMember = filest |> FileSystemTree.filterFiles (fun n -> not (n.StartsWith "."))
            Expect.equal actualInstanceMethod expected "instance member result incorrect."
            Expect.equal actualStaticMember expected "static member result incorrect."
        }
    ]

let private tests_Filter_Folders = 
    testList "FilterFolders" [
        test "new arc (2023-07-11), filter not startswith '.'" {
            let expected = Some(
                Folder("root",[|
                    File "isa.investigation.xlsx"; 
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
            )
            let filest = FileSystemTree.fromFilePaths newArcRelativePaths
            let actualInstanceMethod = filest.FilterFolders (fun n -> not (n.StartsWith "."))
            let actualStaticMember = filest |> FileSystemTree.filterFolders (fun n -> not (n.StartsWith "."))
            Expect.equal actualInstanceMethod expected "instance member result incorrect."
            Expect.equal actualStaticMember expected "static member result incorrect."
        }
    ]

let private tests_Filter = 
    testList "Filter" [
        test "new arc (2023-07-11), filter not startswith '.'" {
            let expected = Some(
                Folder("root",[|
                    File "isa.investigation.xlsx"; 
                    Folder("assays",[|
                        Folder("est",[|
                            File "isa.assay.xlsx"; 
                            File "README.md";
                        |]);
                        Folder
                            ("TestAssay1",[|
                            File "isa.assay.xlsx"; 
                            File "README.md";
                        |])
                    |]);
                    Folder("studies",[|
                        Folder("est",[|
                            File "isa.study.xlsx"; 
                            File "README.md";
                        |]);
                        Folder("MyStudy",[|
                            File "isa.study.xlsx"; 
                            File "README.md";
                        |]);
                        Folder("TestAssay1",[|
                            File "isa.study.xlsx"; 
                            File "README.md";
                        |])
                    |]);
                |])
            )
            let filest = FileSystemTree.fromFilePaths newArcRelativePaths
            let actualInstanceMethod = filest.Filter (fun n -> not (n.StartsWith "."))
            let actualStaticMember = filest |> FileSystemTree.filter (fun n -> not (n.StartsWith "."))
            Expect.equal actualInstanceMethod expected "instance member result incorrect."
            Expect.equal actualStaticMember expected "static member result incorrect."
        }
    ]

let private tests_AddFile = 
    testList "AddFile" [
        testCase "new arc, add nested file" <| fun _ ->
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
                        Folder ("resources", [|File "MyAwesomeRessource.pdf"; File ".gitkeep"|])
                    |])
                |]);
                Folder ("workflows", [|File ".gitkeep"|])
            |])
            let newPath = @"studies\TestAssay1\resources\MyAwesomeRessource.pdf"
            let actual = newArcFST.AddFile newPath
            Expect.testFileSystemTree actual expected // use this instead of equal to ensure order of Folder children does not matter
        testCase "new arc, add root file" <| fun _ ->
            let expected = Folder("root",[|
                File "isa.investigation.xlsx"; File "Test.md";
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
            let newPath = @"Test.md"
            let actual = newArcFST.AddFile newPath
            Expect.testFileSystemTree actual expected // use this instead of equal to ensure order of Folder children does not matter
        testCase "new arc, add new folder" <| fun _ ->
            let expected = Folder("root",[|
                File "isa.investigation.xlsx"; 
                Folder ("MyNewFolder", [|File "README.md"|]);
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
            let newPath = @"MyNewFolder/README.md"
            let actual = newArcFST.AddFile newPath
            Expect.testFileSystemTree actual expected // use this instead of equal to ensure order of Folder children does not matter
    ]

let main = testList "FileSystemTree" [
    tests_fromFilePaths
    tests_ToFilePaths
    tests_Filter_Files
    tests_Filter_Folders
    tests_Filter
    tests_AddFile
]
