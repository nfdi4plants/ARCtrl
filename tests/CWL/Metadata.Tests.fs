module Tests.Metadata

open ARCtrl.CWL
open ARCtrl.CWL.Decode
open DynamicObj
open YAMLicious
open TestingUtils

let decodeMetadata =
    TestObjects.CWL.Metadata.metadataFileContent
    |> Decode.read

let overflowDictionary =
    decodeMetadata
    |> Decode.object (fun get -> get.Overflow.FieldList [])

let dynObj =
    overflowDecoder (new DynamicObj()) overflowDictionary

let testMetadata =
    testList "Decode" [
        testCase "Overflow Dictionary Keys" <| fun _ ->
            let expected = ["arc:has technology type"; "arc:technology platform"; "arc:performer"; "arc:has process sequence"]
            let actual = overflowDictionary.Keys |> List.ofSeq
            Expect.equal actual expected ""
        testCase "DynObj Keys" <| fun _ ->
            let expected = ["arc:has process sequence"; "arc:has technology type"; "arc:performer"; "arc:technology platform"]
            let actual = dynObj |> DynamicObjHelpers.dynamicPropertiesSnapshot |> List.map fst
            Expect.equal actual expected ""
        testCase "DynObj setProperty Value check" <| fun _ ->
            let expectedValue = ".NET"
            let actualValue = dynObj |> DynObj.tryGetTypedPropertyValue<string> "arc:technology platform"
            Expect.equal actualValue.Value expectedValue ""
        testCase "multiple sequence entries stay distinct in overflow" <| fun _ ->
            let yaml = """arc:items:
  - class: arc:First
    arc:name: one
  - class: arc:Second
    arc:name: two"""
            let decoded =
                yaml
                |> Decode.read
                |> Decode.object (fun get -> get.Overflow.FieldList [])
                |> overflowDecoder (DynamicObj())
            let items = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "arc:items" decoded) "Sequence overflow should decode as a collection."
            Expect.equal items.Count 2 "Both sequence entries should survive."
            let first = items.[0] :?> DynamicObj
            let second = items.[1] :?> DynamicObj
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "class" first) (Some "arc:First") "First item should keep its own class."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:name" first) (Some "one") "First item should keep its own payload."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "class" second) (Some "arc:Second") "Second item should keep its own class."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:name" second) (Some "two") "Second item should keep its own payload."
    ]


let main = 
    testList "DynamicObj Metadata" [
        testMetadata
    ]
