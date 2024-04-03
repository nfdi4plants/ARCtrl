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
    ]


let main = testList "ProcessInput" [
    tests_source
    tests_material
    tests_data
    tests_sample
]
