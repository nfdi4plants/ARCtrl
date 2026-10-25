module Tests.Descriptions

open System
open ARCtrl.CWL
open YAMLicious
open TestingUtils
open TestingUtils.CWL

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

/// The tool that a top-level field of a document belongs to.
let toolWith (fields: string) : CWLToolDescription =
    Decode.decodeCommandLineTool ("cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs: []\noutputs: []\n" + fields + "\n")

let decodeTests =
    testList "Decode" [
        testCase "label" <| fun _ ->
            let actual = (toolWith TestObjects.CWL.Descriptions.label).Label
            Expect.equal actual (Some (SchemaSaladString.Literal "An example tool demonstrating metadata.")) ""
        testCase "doc simple" <| fun _ ->
            let actual = (toolWith TestObjects.CWL.Descriptions.docSimple).Doc
            let expected = "Note that this is an example and the metadata is not necessarily consistent."
            Expect.equal actual (Some (DocText.literal expected)) ""
        testCase "doc block" <| fun _ ->
            let actual =
                (toolWith TestObjects.CWL.Descriptions.descriptionFileContentComplex).Doc
                |> Option.bind (fun doc -> doc.TryText)
                |> Option.map normalizeDocBlockContent
            let expected =
                TestObjects.CWL.Descriptions.descriptionFileContentComplexDecoded
                |> normalizeDocBlockContent
            Expect.equal actual (Some expected) ""
        testCase "intent" <| fun _ ->
            let actual =
                TestObjects.CWL.Descriptions.intent
                |> Decode.readYaml
                |> Decode.intentDecoder
            let expected = literals [ "classification"; "quality-control" ]
            let parsed = Expect.wantSome actual "Intent should decode as string array."
            Expect.sequenceEqual parsed expected ""
    ]

let encodeToString pair =
    [pair]
    |> YAMLicious.Encode.object
    |> Encode.writeYaml
    |> normalizeNewLines

let encodeTests =
    testList "Encode" [
        testCase "label" <| fun _ ->
            let expected = normalizeNewLines TestObjects.CWL.Descriptions.label
            let actual = Encode.encodeLabel (SchemaSaladString.Literal "An example tool demonstrating metadata.") |> encodeToString
            Expect.equal actual expected ""
        testCase "doc simple" <| fun _ ->
            let expected = normalizeNewLines TestObjects.CWL.Descriptions.docSimple
            let actual = Encode.encodeDoc (DocText.literal "Note that this is an example and the metadata is not necessarily consistent.") |> encodeToString
            Expect.equal actual expected ""
        testCase "doc block" <| fun _ ->
            let complexDoc = stripDocContent TestObjects.CWL.Descriptions.descriptionFileContentComplex
            let actual = Encode.encodeDoc (DocText.literal complexDoc) |> encodeToString
            let decoded =
                (toolWith actual).Doc
                |> Option.bind (fun doc -> doc.TryText)
                |> Option.map normalizeDocBlockContent
            let expected = normalizeDocBlockContent complexDoc
            Expect.equal decoded (Some expected) "Encoded block doc should decode back to original content."
            Expect.stringContains actual "doc:" "Encoded block doc should emit doc field."
        testCase "intent roundtrip" <| fun _ ->
            let encoded =
                literals [ "classification"; "quality-control" ]
                |> Encode.encodeIntent false
                |> encodeToString
            let decoded =
                encoded
                |> Decode.readYaml
                |> Decode.intentDecoder
            let parsed = Expect.wantSome decoded "Intent should decode after encode."
            Expect.sequenceEqual parsed (literals [ "classification"; "quality-control" ]) ""
    ]

let main =
    testList "Descriptions" [
        decodeTests
        encodeTests
    ]
