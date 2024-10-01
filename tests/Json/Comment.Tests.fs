module Tests.Comment


open ARCtrl
open ARCtrl.Json
open Thoth.Json.Core
open ARCtrl.Process
open TestingUtils

let private tests_DisambiguatingDescription = testList "DisambiguatingDescription" [
    testCase "Full" <| fun _ ->
        let comment = Comment.create(name="My, cool  comment wiht = lots; of special <> chars", value="STARTING VALUE")
        let textString = Comment.ROCrate.encoderDisambiguatingDescription comment |> Encode.toJsonString 0
        let actual = Decode.fromJsonString Comment.ROCrate.decoderDisambiguatingDescription textString
        Expect.equal actual comment ""
    testCase "None" <| fun _ ->
        let comment = Comment.create()
        let textString = Comment.ROCrate.encoderDisambiguatingDescription comment |> Encode.toJsonString 0
        let actual = Decode.fromJsonString Comment.ROCrate.decoderDisambiguatingDescription textString
        Expect.equal actual comment ""
    testCase "Write" <| fun _ ->
        let c = Comment.create(name="My, cool  comment wiht = lots; of special <> chars", value="STARTING VALUE")
        let actual = Comment.ROCrate.encoderDisambiguatingDescription c |> Encode.toJsonString 0
        //let expected = """ "{\"@id\":\"#Comment_My,_cool__comment_wiht_=_lots;_of_special_<>_chars_STARTING_VALUE\",\"@type\":\"Comment\",\"name\":\"My, cool  comment wiht = lots; of special <> chars\",\"value\":\"STARTING VALUE\",\"@context\":{\"sdo\":\"http://schema.org/\",\"Comment\":\"sdo:Comment\",\"name\":\"sdo:name\",\"value\":\"sdo:text\"}}" """
        let expected = c.ToString() |> Encode.string |> Encode.toJsonString 0
        Expect.stringEqual actual expected ""
]

let private tests_ROCrate = testList "ROCrate" [
    testCase "Write" <| fun _ ->
        let c = Comment.create(name="My, cool  comment wiht = lots; of special <> chars", value="STARTING VALUE")
        let actual = Comment.toROCrateJsonString () c 
        let expected = TestObjects.Json.ROCrate.comment
        Expect.stringEqual actual expected ""
]

let main = testList "Comment" [
    tests_DisambiguatingDescription
    tests_ROCrate
]

