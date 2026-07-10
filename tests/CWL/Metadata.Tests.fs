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
        testCase "single sequence entry stays a collection in overflow" <| fun _ ->
            let yaml = "arc:list: [only]"
            let decoded =
                yaml
                |> Decode.read
                |> Decode.object (fun get -> get.Overflow.FieldList [])
                |> overflowDecoder (DynamicObj())
            let items = Expect.wantSome (DynObj.tryGetTypedPropertyValue<ResizeArray<obj>> "arc:list" decoded) "Singleton sequence overflow should remain a collection."
            Expect.equal items.Count 1 "Singleton sequence should keep one entry."
            Expect.equal (items.[0] :?> string) "only" "Singleton sequence entry should be preserved."
        testCase "nested overflow decodes unquoted YAML primitive scalars with their types" <| fun _ ->
            let yaml = """arc:nested:
  enabled: true
  count: 1
  threshold: 2.5
  quotedFlag: 'true'"""
            let decoded =
                yaml
                |> Decode.read
                |> Decode.object (fun get -> get.Overflow.FieldList [])
                |> overflowDecoder (DynamicObj())
            let nested = Expect.wantSome (DynObj.tryGetTypedPropertyValue<DynamicObj> "arc:nested" decoded) "Nested overflow should decode as an object."
            Expect.equal (DynObj.tryGetTypedPropertyValue<bool> "enabled" nested) (Some true) "Plain booleans should retain their YAML type."
            Expect.equal (DynObj.tryGetTypedPropertyValue<int64> "count" nested) (Some 1L) "Plain integers should retain their YAML type."
            Expect.equal (DynObj.tryGetTypedPropertyValue<float> "threshold" nested) (Some 2.5) "Plain floats should retain their YAML type."
            Expect.equal (DynObj.tryGetTypedPropertyValue<string> "quotedFlag" nested) (Some "true") "Quoted scalar text should remain text."
    ]


let main = 
    testList "DynamicObj Metadata" [
        testMetadata
    ]
