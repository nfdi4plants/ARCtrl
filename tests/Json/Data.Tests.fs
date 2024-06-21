module Tests.Data

open TestingUtils
open ARCtrl
open ARCtrl.Json

let basic_tests = testList "BasicJson" [
    testCase "AllFields" <| fun _ ->
        let d = Data("MyID","MyName",DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
        let json = Data.encoder d |> Encode.toJsonString 2
        let d2 = Decode.fromJsonString Data.decoder json
        Expect.equal d2 d "Different after write and read"
    ]

let isa_tests = testList "ISAJson" [
    testCase "NativeISAFieldsIO" <| fun _ ->
        let d = Data("MyID","MyName",DataFile.RawDataFile,comments = ResizeArray [Comment.create("MyKey","MyValue")])
        let json = Data.ISAJson.encoder None d |> Encode.toJsonString 2
        let d2 = Decode.fromJsonString Data.ISAJson.decoder json
        Expect.equal d2 d "Different after write and read"
    ptestCase "AllFieldsLossless" <| fun _ ->
        let d = Data("MyID","MyName",DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
        let json = Data.ISAJson.encoder None d |> Encode.toJsonString 2
        let d2 = Decode.fromJsonString Data.ISAJson.decoder json
        Expect.equal d2 d "Different after write and read"
    #if !FABLE_COMPILER_PYTHON
    testAsync "WriterSchemaCorrectness" {
        let d = Data("MyID","MyName",DataFile.RawDataFile, "text/csv", "MySelector", ResizeArray [Comment.create("MyKey","MyValue")])
        let json = Data.ISAJson.encoder None d |> Encode.toJsonString 2
        let! validation = Validation.validateData json
        Expect.isTrue validation.Success $"Data did not match schema: {validation.GetErrors()}"
    }
    #endif
]

let rocrate_tests = testList "RO-CrateJson" [
    testCase "AllFields" <| fun _ ->
        let d = Data("MyID","MyName",DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
        let json = Data.ROCrate.encoder d |> Encode.toJsonString 2
        let d2 = Decode.fromJsonString Data.ROCrate.decoder json
        Expect.equal d2 d "Different after write and read"
    ]

let compressed_tests =
    testList "CompressedJson" [
        testCase "AllFields" <| fun _ ->
            let d = Data("MyID","MyName",DataFile.RawDataFile,"text/csv","MySelector",ResizeArray [Comment.create("MyKey","MyValue")])
            let stringTable = StringTable.StringTableMap()
            let json = Data.compressedEncoder stringTable d |> Encode.toJsonString 2
            let d2 = Decode.fromJsonString (Data.compressedDecoder (StringTable.arrayFromMap stringTable)) json
            Expect.equal d2 d "Different after write and read"
    ]

let main = testList "Data" [
    basic_tests
    isa_tests
    rocrate_tests
    compressed_tests
]