module Tests.Process.ProcessInput

open ARCtrl
open ARCtrl.Process
open ARCtrl.Json
open TestingUtils
open TestObjects.Json

let private tests_source =
    testList "Source" [
        testCase "ReaderSuccess" (fun () -> 
            let result = ProcessInput.fromISAJsonString ProcessInput.source
            let expected = 
                Source.create("#source/source-culture8","source-culture8")  
            Expect.isTrue (ProcessInput.isSource result) "Result is not a source"
            Expect.equal (ProcessInput.trySource result).Value expected "Source did not match"
        )
        testCase "WriterOutputMatchesInput" (fun () -> 
            let o_read_in = ProcessInput.fromISAJsonString ProcessInput.source
            let o_out = ProcessInput.toISAJsonString () o_read_in
            let expected = ProcessInput.source
            let actual =  o_out
            Expect.stringEqual actual expected "Written processInput does not match read process input"
        )
        testCase "LD_WriteReadWithCharacteristics" (fun () ->
            let charaHeader = Process.MaterialAttribute.create(CharacteristicType = OntologyAnnotation.create("MyAnnotation","NCIT","http://purl.obolibrary.org/obo/NCIT_C42781"))
            let charaValue = OntologyAnnotation.create("MyAnnotationValue","NCIT","http://purl.obolibrary.org/obo/NCIT_C42782")
            let chara = Process.MaterialAttributeValue.create(Category = charaHeader, Value = Value.Ontology charaValue)
            let source = Source.create(Name = "#sample/sample-P-0.1-aliquot7", Characteristics = [chara])
            let o_out = ProcessInput.ROCrate.encoder (ProcessInput.Source source) |> Encode.toJsonString 0
            let inputPI = Decode.fromJsonString ProcessInput.ROCrate.decoder o_out
            let inputSourceOpt = ProcessInput.trySource inputPI
            let inputSource = Expect.wantSome inputSourceOpt "Input is not a sample"
            Expect.equal inputSource.Name source.Name "Sample name did not match"
            let characteristics = Expect.wantSome inputSource.Characteristics "No characteristics found"
            Expect.hasLength characteristics 1 "Sample characteristics length did not match"
            let inputChara = characteristics.[0]
            Expect.equal inputChara chara "Sample characteristic did not match"
        )
    ]

let private tests_material =
    testList "Material" [
        testCase "ReaderSuccess" (fun () ->           
            let result = ProcessInput.fromISAJsonString ProcessInput.material
            let expected = 
                Material.create("#material/extract-G-0.1-aliquot1","extract-G-0.1-aliquot1",MaterialType.ExtractName)
            Expect.isTrue (ProcessInput.isMaterial result) "Result is not a material"
            Expect.equal (ProcessInput.tryMaterial result).Value expected "Material did not match"

        )
        testCase "WriterOutputMatchesInput" (fun () -> 
            let o_read_in = ProcessInput.fromISAJsonString ProcessInput.material
            let o_out = ProcessInput.toISAJsonString () o_read_in
            let expected = ProcessInput.material
            let actual = o_out
            Expect.stringEqual actual expected "Written processInput does not match read process input"
        )
    ]
let private tests_data =
    testList "Data" [
        testCase "ReaderSuccess" (fun () -> 
            let result = ProcessInput.fromISAJsonString ProcessInput.data
            let expected = 
                Data.create("#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_3.txt","JIC64_Nitrogen_0.07_External_1_3.txt",DataFile.RawDataFile)
            Expect.isTrue (ProcessInput.isData result) "Result is not a data"
            Expect.equal (ProcessInput.tryData result).Value expected "Data did not match"
        )
        testCase "WriterOutputMatchesInput" (fun () -> 
            let o_read_in = ProcessInput.fromISAJsonString ProcessInput.data
            let o_out = ProcessInput.toISAJsonString () o_read_in
            let expected = ProcessInput.data
            let actual = o_out
            Expect.stringEqual actual expected "Written processInput does not match read process input"
        )
    ]
let private tests_sample =
    testList "Sample" [
        testCase "ReaderSuccessSimple" (fun () -> 
            let result = ProcessInput.fromISAJsonString ProcessInput.sampleSimple
            let expectedDerivesFrom = [Source.create("#source/source-culture8")]
            let expected = 
                Sample.create("#sample/sample-P-0.1-aliquot7","sample-P-0.1-aliquot7", DerivesFrom = expectedDerivesFrom)
            Expect.isTrue (ProcessInput.isSample result) "Result is not a sample"
            Expect.equal (ProcessInput.trySample result).Value expected "Sample did not match"
        )
        testCase "WriterOutputMatchesInputSimple" (fun () -> 
            let o_read_in = ProcessInput.fromISAJsonString ProcessInput.sampleSimple
            let o_out = ProcessInput.toISAJsonString () o_read_in
            let expected = ProcessInput.sampleSimple
            let actual = o_out
            Expect.stringEqual actual expected "Written processInput does not match read process input"
        )
        testCase "LD_WriteReadWithCharacteristics" (fun () ->
            let charaHeader = Process.MaterialAttribute.create(CharacteristicType = OntologyAnnotation.create("MyAnnotation","NCIT","http://purl.obolibrary.org/obo/NCIT_C42781"))
            let charaValue = OntologyAnnotation.create("MyAnnotationValue","NCIT","http://purl.obolibrary.org/obo/NCIT_C42782")
            let chara = Process.MaterialAttributeValue.create(Category = charaHeader, Value = Value.Ontology charaValue)
            let sample = Sample.create(Name = "#sample/sample-P-0.1-aliquot7", Characteristics = [chara])
            let o_out = ProcessInput.ROCrate.encoder (ProcessInput.Sample sample) |> Encode.toJsonString 0
            let inputPI = Decode.fromJsonString ProcessInput.ROCrate.decoder o_out
            let inputSampleOpt = ProcessInput.trySample inputPI
            let inputSample = Expect.wantSome inputSampleOpt "Input is not a sample"
            Expect.equal inputSample.Name sample.Name "Sample name did not match"
            let characteristics = Expect.wantSome inputSample.Characteristics "No characteristics found"
            Expect.hasLength characteristics 1 "Sample characteristics length did not match"
            let inputChara = characteristics.[0]
            Expect.equal inputChara chara "Sample characteristic did not match"
        )
    ]


let main = testList "ProcessInput" [
    tests_source
    tests_material
    tests_data
    tests_sample
]
