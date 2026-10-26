module Data.Tests

open ARCtrl

open TestingUtils

let private tests_GetHashCode = testList "GetHashCode" [
    testCase "equal" <| fun _ ->
        let d1 = Data("MyID","MyName",DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
        let d2 = Data("MyID","MyName",DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
        let hash1 = d1.GetHashCode()
        let hash2 = d2.GetHashCode()
        Expect.equal d1 d2 "Should be equal"
        Expect.equal hash1 hash2 "HashCode should be equal"
    testCase "unequal, different name" <| fun _ ->
        let d1 = Data("MyID","MyName",DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
        let d2 = Data("MyID","MyName2",DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
        let hash1 = d1.GetHashCode()
        let hash2 = d2.GetHashCode()
        Expect.notEqual d1 d2 "Should not be equal"
        Expect.notEqual hash1 hash2 "HashCode should not be equal"
    ]

let private tests_AbsoluteFilePath = testList "AbsoluteFilePath" [
    testCase "Study_Relative" <| fun _ ->
        let fileName = "MyFile"
        let studyName = "MyStudy"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForStudy(studyName)
        let expected = $"studies/{studyName}/resources/{fileName}"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Study_Absolute" <| fun _ ->
        let fileName = "studies/MyStudy/resources/MyFile"
        let studyName = "MyStudy"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForStudy(studyName)
        let expected = $"studies/{studyName}/resources/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Study_Absolute_/" <| fun _ ->
        let fileName = "/studies/MyStudy/resources/MyFile"
        let studyName = "MyStudy"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForStudy(studyName)
        let expected = $"studies/{studyName}/resources/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Study_Absolute_./" <| fun _ ->
        let fileName = "./studies/MyStudy/resources/MyFile"
        let studyName = "MyStudy"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForStudy(studyName)
        let expected = $"studies/{studyName}/resources/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Assay_Relative" <| fun _ ->
        let fileName = "MyFile"
        let assayName = "MyAssay"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForAssay(assayName)
        let expected = $"assays/{assayName}/dataset/{fileName}"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Assay_Absolute" <| fun _ ->
        let fileName = "assays/MyAssay/dataset/MyFile"
        let assayName = "MyAssay"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForAssay(assayName)
        let expected = $"assays/{assayName}/dataset/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Assay_Absolute_/" <| fun _ ->
        let fileName = "/assays/MyAssay/dataset/MyFile"
        let assayName = "MyAssay"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForAssay(assayName)
        let expected = $"assays/{assayName}/dataset/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Assay_Absolute_./" <| fun _ ->
        let fileName = "./assays/MyAssay/dataset/MyFile"
        let assayName = "MyAssay"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForAssay(assayName)
        let expected = $"assays/{assayName}/dataset/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    // https://github.com/nfdi4plants/ARCtrl/issues/593
    testCase "Study_EmptyPath" <| fun _ ->
        let d = Data(name = "")
        let absolute = d.GetAbsolutePathForStudy("MyStudy")
        Expect.equal absolute "" "Empty path should not be resolved to the resources folder"
    testCase "Assay_EmptyPath" <| fun _ ->
        let d = Data(name = "")
        let absolute = d.GetAbsolutePathForAssay("MyAssay")
        Expect.equal absolute "" "Empty path should not be resolved to the dataset folder"
    testCase "Assay_WhitespacePath" <| fun _ ->
        let d = Data(name = "  ")
        let absolute = d.GetAbsolutePathForAssay("MyAssay")
        Expect.equal absolute "  " "Whitespace path should not be resolved into the dataset folder"
    // https://github.com/nfdi4plants/ARCtrl/issues/629
    testCase "Workflow_Relative" <| fun _ ->
        let fileName = "MyFile"
        let workflowName = "MyWorkflow"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForWorkflow(workflowName)
        let expected = $"workflows/{workflowName}/{fileName}"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Workflow_Absolute" <| fun _ ->
        let fileName = "workflows/MyWorkflow/MyFile"
        let workflowName = "MyWorkflow"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForWorkflow(workflowName)
        let expected = $"workflows/{workflowName}/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Workflow_Absolute_/" <| fun _ ->
        let fileName = "/workflows/MyWorkflow/MyFile"
        let workflowName = "MyWorkflow"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForWorkflow(workflowName)
        let expected = $"workflows/{workflowName}/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Workflow_Absolute_./" <| fun _ ->
        let fileName = "./workflows/MyWorkflow/MyFile"
        let workflowName = "MyWorkflow"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForWorkflow(workflowName)
        let expected = $"workflows/{workflowName}/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    // https://github.com/nfdi4plants/ARCtrl/issues/629
    testCase "Run_Relative" <| fun _ ->
        let fileName = "MyFile"
        let runName = "MyRun"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForRun(runName)
        let expected = $"runs/{runName}/{fileName}"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Run_Absolute" <| fun _ ->
        let fileName = "runs/MyRun/MyFile"
        let runName = "MyRun"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForRun(runName)
        let expected = $"runs/{runName}/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Run_Absolute_/" <| fun _ ->
        let fileName = "/runs/MyRun/MyFile"
        let runName = "MyRun"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForRun(runName)
        let expected = $"runs/{runName}/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    testCase "Run_Absolute_./" <| fun _ ->
        let fileName = "./runs/MyRun/MyFile"
        let runName = "MyRun"
        let d = Data(name = fileName)
        let absolute = d.GetAbsolutePathForRun(runName)
        let expected = $"runs/{runName}/MyFile"
        Expect.equal absolute expected "Absolute path should match expected path"
    ]


// https://github.com/nfdi4plants/ARCtrl/issues/630
let private tests_isEmpty = testList "isEmpty" [
    testCase "NoName" <| fun _ ->
        let d = Data()
        Expect.isTrue (d.isEmpty()) "Data without any values should be empty"
    testCase "EmptyName" <| fun _ ->
        let d = Data(name = "")
        Expect.isTrue (d.isEmpty()) "Data with empty name should be empty"
    testCase "BlankName" <| fun _ ->
        let d = Data(name = "  ")
        Expect.isTrue (d.isEmpty()) "Data with blank name should be empty"
    testCase "BlankNameWithSelector" <| fun _ ->
        let d = Data(name = "  #row=1")
        Expect.isFalse (d.isEmpty()) "Data with selector should not be empty"
    testCase "Name" <| fun _ ->
        let d = Data(name = "MyFile.csv")
        Expect.isFalse (d.isEmpty()) "Data with name should not be empty"
    testCase "OnlyComment" <| fun _ ->
        let d = Data(comments = ResizeArray [Comment.create("MyKey","MyValue")])
        Expect.isFalse (d.isEmpty()) "Data with comment should not be empty"
    ]

let main = 
    testList "Data" [
        tests_GetHashCode
        tests_AbsoluteFilePath
        tests_isEmpty
    ]