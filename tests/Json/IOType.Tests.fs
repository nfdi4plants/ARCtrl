module Tests.IOType

open ARCtrl
open ARCtrl.Json
open TestingUtils

let tests_extended = testList "extended" [
  let io_freetext = IOType.FreeText "Hello World"
  let io_freetext_jsonString =  "\"Hello World\"" 
  let io_sample = IOType.Sample
  let io_sample_jsonString =  "\"Sample Name\"" 
  testList "encoder" [
    testCase "FreeText" <| fun _ ->
      let actual = Encode.toJsonString  0 <| IOType.encoder io_freetext
      let expected = io_freetext_jsonString
      Expect.equal actual expected ""
    testCase "Sample" <| fun _ ->
      let actual = Encode.toJsonString  0 <| IOType.encoder io_sample
      let expected = io_sample_jsonString
      Expect.equal actual expected ""
  ]
  testList "decoder" [
    testCase "FreeText" <| fun _ ->
      let actual = Decode.fromJsonString IOType.decoder io_freetext_jsonString 
      let expected = io_freetext
      Expect.equal actual expected ""
    testCase "Sample" <| fun _ ->
      let actual = Decode.fromJsonString IOType.decoder io_sample_jsonString
      let expected = io_sample
      Expect.equal actual expected ""
  ]
]

let main = testList "IOType" [
    tests_extended
]