module Data.Tests

open ARCtrl

open TestingUtils

let private tests_GetHashCode = testList "GetHashCode" [
    testCase "equal" <| fun _ ->
        let d1 = Data("MyID","MyName",Process.DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
        let d2 = Data("MyID","MyName",Process.DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
        let hash1 = d1.GetHashCode()
        let hash2 = d2.GetHashCode()
        Expect.equal d1 d2 "Should be equal"
        Expect.equal hash1 hash2 "HashCode should be equal"
    testCase "unequal, different name" <| fun _ ->
        let d1 = Data("MyID","MyName",Process.DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
        let d2 = Data("MyID","MyName2",Process.DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
        let hash1 = d1.GetHashCode()
        let hash2 = d2.GetHashCode()
        Expect.notEqual d1 d2 "Should not be equal"
        Expect.notEqual hash1 hash2 "HashCode should not be equal"
    ]


let main = 
    testList "Data" [
        tests_GetHashCode       
    ]