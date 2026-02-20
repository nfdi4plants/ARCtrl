module Tests.Descriptions

open System
open ARCtrl.CWL
open YAMLicious
open TestingUtils

let normalizeNewLines (text:string) =
    text.Replace("\r\n","\n").TrimEnd('\n')

let normalizeDocBlockContent (text: string) =
    text
    |> normalizeNewLines
    |> fun normalized ->
        normalized.Split('\n')
        |> Array.map (fun line -> line.TrimStart())
        |> Array.filter (fun line -> line.Trim() <> "")
        |> String.concat "\n"

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
                |> Option.map normalizeDocBlockContent
            let expected =
                TestObjects.CWL.Descriptions.descriptionFileContentComplexDecoded
                |> normalizeDocBlockContent
            Expect.equal actual (Some expected) ""
        testCase "intent" <| fun _ ->
            let actual =
                TestObjects.CWL.Descriptions.intent
                |> Decode.read
                |> Decode.intentDecoder
            let expected = ResizeArray [|"classification"; "quality-control"|]
            let parsed = Expect.wantSome actual "Intent should decode as string array."
            Expect.sequenceEqual parsed expected ""
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
            let complexDoc = stripDocContent TestObjects.CWL.Descriptions.descriptionFileContentComplex
            let actual = Encode.encodeDoc complexDoc |> encodeToString
            let decoded =
                actual
                |> Decode.read
                |> Decode.docDecoder
                |> Option.map normalizeDocBlockContent
            let expected = normalizeDocBlockContent complexDoc
            Expect.equal decoded (Some expected) "Encoded block doc should decode back to original content."
            Expect.stringContains actual "doc:" "Encoded block doc should emit doc field."
        testCase "intent roundtrip" <| fun _ ->
            let encoded =
                ResizeArray [|"classification"; "quality-control"|]
                |> Encode.encodeIntent
                |> encodeToString
            let decoded =
                encoded
                |> Decode.read
                |> Decode.intentDecoder
            let parsed = Expect.wantSome decoded "Intent should decode after encode."
            Expect.sequenceEqual parsed (ResizeArray [|"classification"; "quality-control"|]) ""
    ]

let main =
    testList "Descriptions" [
        decodeTests
        encodeTests
    ]
