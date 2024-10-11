module Tests.Metadata

open ARCtrl.CWL
open ARCtrl.CWL.CWLTypes
open ARCtrl.CWL.Requirements
open ARCtrl.CWL.Inputs
open ARCtrl.CWL.Outputs
open ARCtrl.CWL.Decode
open DynamicObj
open YAMLicious
open TestingUtils

let decodeMetadata =
    TestObjects.CWL.Metadata.metadata
    |> Decode.read

let overflowDictionary =
    decodeMetadata
    |> Decode.object (fun get -> get.Overflow.FieldList [])

let dynObj =
    overflowDecoder (new DynamicObj()) overflowDictionary

let testMetadata =
    testList "CWL Metadata" [
        testCase "Overflow Dictionary Keys" <| fun _ ->
            let expected = ["arc:has technology type"; "arc:technology platform"; "arc:performer"; "arc:has process sequence"]
            let actual = overflowDictionary.Keys |> List.ofSeq
            Expect.equal actual expected
                $"Expected: {expected}\nActual: {actual}"
        testCase "DynObj Keys" <| fun _ ->
            let expected = ["arc:has technology type"; "arc:technology platform"; "arc:performer"; "arc:has process sequence"]
            let actual = dynObj.GetProperties(false) |> List.ofSeq |> List.map (fun x -> x.Key)
            Expect.equal actual expected
                $"Expected: {expected}\nActual: {actual}"
        testCase "DynObj setProperty Value check" <| fun _ ->
            let expectedValue = ".NET"
            let actualValue = dynObj |> DynObj.tryGetTypedPropertyValue<string> "arc:technology platform"
            Expect.equal actualValue.Value expectedValue
                $"Expected: {expectedValue}\nActual: {actualValue}"
    ]