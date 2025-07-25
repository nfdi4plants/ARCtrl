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
    ]


let main = 
    testList "Data" [
        tests_GetHashCode
        tests_AbsoluteFilePath
    ]