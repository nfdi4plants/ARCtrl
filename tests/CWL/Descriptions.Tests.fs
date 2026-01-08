module Tests.Descriptions

open System
open ARCtrl.CWL
open YAMLicious
open TestingUtils

let normalizeNewLines (text:string) =
    text.Replace("\r\n","\n").TrimEnd('\n')

let stripDocContent (docBlock:string) =
    let normalized = normalizeNewLines docBlock
    let lines = normalized.Split('\n') |> Array.toList
    match lines with
    | first::rest when first.StartsWith("doc:") ->
        if List.isEmpty rest then
            first.Substring("doc:".Length).TrimStart()
        else
            rest
            |> List.map (fun line -> if line.StartsWith("  ") then line.Substring(2) else line)
            |> String.concat "\n"
            |> fun s -> s.TrimEnd()
    | _ -> normalized

let decodeTests =
    testList "Decode" [
        testCase "label" <| fun _ ->
            let actual =
                TestObjects.CWL.Descriptions.label
                |> Decode.read
                |> Decode.labelDecoder
            Expect.equal actual (Some "An example tool demonstrating metadata.") ""
        testCase "doc simple" <| fun _ ->
            let actual =
                TestObjects.CWL.Descriptions.docSimple
                |> Decode.read
                |> Decode.docDecoder
            let expected = "Note that this is an example and the metadata is not necessarily consistent."
            Expect.equal actual (Some expected) ""
        testCase "doc block" <| fun _ ->
            let actual =
                TestObjects.CWL.Descriptions.descriptionFileContentComplex
                |> Decode.read
                |> Decode.docDecoder
                |> Option.map normalizeNewLines
            let expected =
                TestObjects.CWL.Descriptions.descriptionFileContentComplexDecoded
                |> normalizeNewLines
            Expect.equal actual (Some expected) ""
    ]

let encodeToString pair =
    [pair]
    |> Encode.yMap
    |> Encode.writeYaml
    |> normalizeNewLines

let encodeTests =
    testList "Encode" [
        testCase "label" <| fun _ ->
            let expected = normalizeNewLines TestObjects.CWL.Descriptions.label
            let actual = Encode.encodeLabel "An example tool demonstrating metadata." |> encodeToString
            Expect.equal actual expected ""
        testCase "doc simple" <| fun _ ->
            let expected = normalizeNewLines TestObjects.CWL.Descriptions.docSimple
            let actual = Encode.encodeDoc "Note that this is an example and the metadata is not necessarily consistent." |> encodeToString
            Expect.equal actual expected ""
        testCase "doc block" <| fun _ ->
            let expected = normalizeNewLines TestObjects.CWL.Descriptions.descriptionFileContentComplexEncoded
            let complexDoc = stripDocContent TestObjects.CWL.Descriptions.descriptionFileContentComplex
            let actual = Encode.encodeDoc complexDoc |> encodeToString
            Expect.equal actual expected ""
    ]

let main =
    testList "Descriptions" [
        decodeTests
        encodeTests
    ]
