module Tests.Comment


open ARCtrl
open ARCtrl.Json
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
]

let main = testList "Comment" [
    tests_DisambiguatingDescription
]

