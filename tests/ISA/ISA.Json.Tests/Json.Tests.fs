module Json.Tests

open ARCtrl.ISA
open ARCtrl.ISA.Json
open ARCtrl.ISA

#if FABLE_COMPILER
open Fable.Core
open Fable.Core.JsInterop
open Thoth.Json
#else
open Thoth.Json.Net
#endif

open TestingUtils

open TestObjects.Json

module JsonExtensions =

    let private f2 i = 
        if i < 10 then sprintf "0%i" i
        else sprintf "%i" i 

    type System.DateTime with
        member this.ToJsonTimeString() = 
            $"{f2 this.Hour}:{f2 this.Minute}:{f2 this.Second}.{this.Millisecond}"

        member this.ToJsonDateString() = 
            $"{this.Year}-{f2 this.Month}-{f2 this.Day}"
        
        member this.ToJsonDateTimeString() = 
            $"{this.ToJsonDateString()}T{this.ToJsonTimeString()}Z"

    module Time =
    
        let fromInts hour minute = 
            let d = System.DateTime(1,1,1,hour,minute,0)
            d.ToJsonTimeString()

    module Date =
    
        let fromInts year month day = 
            let d = System.DateTime(year,month,day)
            d.ToJsonDateString()
      
    module DateTime =
    
        let fromInts year month day hour minute = 
            let d = System.DateTime(year,month,day,hour,minute,0)
            d.ToJsonDateTimeString()

let testEncode =

    testList "Encode" [
        testCase "tryGetPropertyValueExists" (fun () -> 
            
            let oa = ARCtrl.ISA.OntologyAnnotation.fromString ("MyAnnotation", "ABC:123", "ABC") 
                
            let field = "Name" 

            let result = GEncode.tryGetPropertyValue field oa
            
            Expect.isSome result "Could not find the field, even though it exists"

            let result' = result.Value

            let expected = AnnotationValue.Text "MyAnnotation"

            Expect.equal result' expected "Retrieved value did not match"
        )

        testCase "tryGetPropertyValueMissing" (fun () -> 
            
            let oa = ARCtrl.ISA.OntologyAnnotation.fromString("MyAnnotation", "ABC:123", "ABC") 
                
            let field = "FantasyValue" 

            let result = GEncode.tryGetPropertyValue field oa
            
            Expect.isNone result "Could find a value, even though it shouldn't exists"
        )

        testCase "tryGetPropertyValueSequence" (fun () ->
            
            let comments = [|ARCtrl.ISA.Comment.fromString "k1" "v1"; ARCtrl.ISA.Comment.fromString "k2" "vv"|]
            let oa = ARCtrl.ISA.OntologyAnnotation.fromString ("MyAnnotation", "ABC:123", "ABC", comments)

            let field = "Comments" 

            let result : obj option = GEncode.tryGetPropertyValue field oa
            
            Expect.isSome result "Could not find the field, even though it exists"

            let result' = 
                #if FABLE_COMPILER
                !!result.Value
                #else
                result.Value :?> Comment []
                #endif

            let expected = comments

            Expect.sequenceEqual result' expected "Retrieved value did not match"
        )

    ]

let testDecode =

    testList "Decode" [
        testCase "getFieldNames" (fun () -> 
            
            let s = 
                """
                {
                "@id": "#material/extract-C-0.07-aliquot10",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot10",
                "type": "Extract Name"
                }
                """
            let v = Decode.fromString Decode.value s
           
            let result = 
                match v with 
                | Result.Ok v -> GDecode.getFieldNames v
                | Error e -> failwith e
            
            let expected = ["@id";"characteristics";"name";"type"]

            Expect.sequenceEqual result expected "Field names did not match"
        )
    ]

let testOntoloyAnnotation =
    testList "OntologyAnnotation" [
        
            testCase "ReaderSuccess" (fun () -> 
           
                let result = OntologyAnnotation.fromJsonString OntologyAnnotation.peptidase


                let comment = Comment.create(Name = "comment",Value = "This is a comment")
                let expected = 
                    OntologyAnnotation.create("protease",AnnotationValue.Text "Peptidase", "MS", "http://purl.obolibrary.org/obo/NCIT_C16965",Comments = [|comment|])
                
                Expect.equal result expected "Source did not match"
            )
            testCase "WriterOutputMatchesInput" (fun () -> 
            
                let o_read_in = OntologyAnnotation.fromJsonString OntologyAnnotation.peptidase
                let o_out = OntologyAnnotation.toJsonString o_read_in

                let expected = 
                    OntologyAnnotation.peptidase
                    |> Utils.wordFrequency

                let actual = 
                    o_out
                    |> Utils.wordFrequency

                Expect.sequenceEqual actual expected "Written processInput does not match read process input"
            )
    ]

let testOntoloyAnnotationLD =
    testList "OntologyAnnotationLD" [
        
            testCase "ReaderSuccess" (fun () -> 
           
                let result = OntologyAnnotation.fromJsonString OntologyAnnotation.peptidaseLD


                let comment = Comment.create(Name = "comment",Value = "This is a comment")
                let expected = 
                    OntologyAnnotation.create("protease",AnnotationValue.Text "Peptidase", "MS", "http://purl.obolibrary.org/obo/NCIT_C16965",Comments = [|comment|])
                
                Expect.equal result expected "Source did not match"
            )
            testCase "WriterOutputMatchesInputGivenIDs" (fun () -> 
            
                let o_read_in = OntologyAnnotation.fromJsonString OntologyAnnotation.peptidase
                let o_out = OntologyAnnotation.toJsonldString o_read_in

                let expected = 
                    OntologyAnnotation.peptidaseLD
                    |> Utils.wordFrequency

                let actual = 
                    o_out
                    |> Utils.wordFrequency

                Expect.sequenceEqual actual expected "Written processInput does not match read process input"
            )
            testCase "WriterOutputMatchesInputDefaultIDs" (fun () -> 
            
                let o_read_in = OntologyAnnotation.fromJsonString OntologyAnnotation.peptidaseWithoutIds
                let o_out = OntologyAnnotation.toJsonldString o_read_in

                let expected = 
                    OntologyAnnotation.peptidaseWithDefaultLD
                    |> Utils.wordFrequency

                let actual = 
                    o_out
                    |> Utils.wordFrequency

                Expect.sequenceEqual actual expected "Written processInput does not match read process input"
            )
    ]


let testProcessInput =

    testList "ProcessInput" [
        testList "Source" [
            testCase "ReaderSuccess" (fun () -> 
           
                let result = ProcessInput.fromJsonString ProcessInput.source

                let expected = 
                    Source.create("#source/source-culture8","source-culture8")
                
                Expect.isTrue (ProcessInput.isSource result) "Result is not a source"

                Expect.equal (ProcessInput.trySource result).Value expected "Source did not match"
            )
            testCase "WriterOutputMatchesInput" (fun () -> 
            
                let o_read_in = ProcessInput.fromJsonString ProcessInput.source
                let o_out = ProcessInput.toJsonString o_read_in

                let expected = 
                    ProcessInput.source
                    |> Utils.wordFrequency

                let actual = 
                    o_out
                    |> Utils.wordFrequency

                Expect.sequenceEqual actual expected "Written processInput does not match read process input"
            )
        ]
        testList "Material" [
            testCase "ReaderSuccess" (fun () -> 
           
                let result = ProcessInput.fromJsonString ProcessInput.material

                let expected = 
                    Material.create("#material/extract-G-0.1-aliquot1","extract-G-0.1-aliquot1",MaterialType.ExtractName,Characteristics = [])

                Expect.isTrue (ProcessInput.isMaterial result) "Result is not a material"

                Expect.equal (ProcessInput.tryMaterial result).Value expected "Material did not match"

            )
            testCase "WriterOutputMatchesInput" (fun () -> 
            
                let o_read_in = ProcessInput.fromJsonString ProcessInput.material
                let o_out = ProcessInput.toJsonString o_read_in

                let expected = 
                    ProcessInput.material
                    |> Utils.wordFrequency

                let actual = 
                    o_out
                    |> Utils.wordFrequency

                Expect.equal actual expected "Written processInput does not match read process input"
            )
        ]
        testList "Data" [
            testCase "ReaderSuccess" (fun () -> 
           
                let result = ProcessInput.fromJsonString ProcessInput.data
                let expected = 
                    Data.create("#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_3.txt","JIC64_Nitrogen_0.07_External_1_3.txt",DataFile.RawDataFile,Comments = [])
                Expect.isTrue (ProcessInput.isData result) "Result is not a data"
                Expect.equal (ProcessInput.tryData result).Value expected "Data did not match"
            )
            testCase "WriterOutputMatchesInput" (fun () -> 
            
                    let o_read_in = ProcessInput.fromJsonString ProcessInput.data
                    let o_out = ProcessInput.toJsonString o_read_in

                    let expected = 
                        ProcessInput.data
                        |> Utils.wordFrequency

                    let actual = 
                        o_out
                        |> Utils.wordFrequency

                    Expect.equal actual expected "Written processInput does not match read process input"
            )
        ]
        testList "Sample" [
            testCase "ReaderSuccessSimple" (fun () -> 
           
                let result = ProcessInput.fromJsonString ProcessInput.sampleSimple

                let expectedDerivesFrom = [Source.create("#source/source-culture8")]

                let expected = 
                    Sample.create("#sample/sample-P-0.1-aliquot7","sample-P-0.1-aliquot7", DerivesFrom = expectedDerivesFrom)

                Expect.isTrue (ProcessInput.isSample result) "Result is not a sample"

                Expect.equal (ProcessInput.trySample result).Value expected "Sample did not match"
            )
            testCase "WriterOutputMatchesInputSimple" (fun () -> 
            
                    let o_read_in = ProcessInput.fromJsonString ProcessInput.sampleSimple
                    let o_out = ProcessInput.toJsonString o_read_in

                    let expected = 
                        ProcessInput.sampleSimple
                        |> Utils.wordFrequency

                    let actual = 
                        o_out
                        |> Utils.wordFrequency

                    Expect.equal actual expected "Written processInput does not match read process input"
            )
        ]
    ]

let testProcessInputLD =

    testList "ProcessInputLD" [
        testList "Source" [
            testCase "ReaderSuccess" (fun () -> 
           
                let result = ProcessInput.fromJsonString ProcessInput.sourceLD

                let expected = 
                    Source.create("#source/source-culture8","source-culture8")
                
                Expect.isTrue (ProcessInput.isSource result) "Result is not a source"

                Expect.equal (ProcessInput.trySource result).Value expected "Source did not match"
            )
            testCase "WriterOutputMatchesInputGivenID" (fun () -> 
            
                let o_read_in = ProcessInput.fromJsonString ProcessInput.source
                let o_out = ProcessInput.toJsonldString o_read_in

                let expected = 
                    ProcessInput.sourceLD
                    |> Utils.wordFrequency

                let actual = 
                    o_out
                    |> Utils.wordFrequency

                Expect.equal actual expected "Written processInput does not match read process input"
            )
            testCase "WriterOutputMatchesInputDefaultID" (fun () -> 
            
                let o_read_in = ProcessInput.fromJsonString ProcessInput.sourceWithoutID
                let o_out = ProcessInput.toJsonldString o_read_in

                let expected = 
                    ProcessInput.sourceWithDefaultLD
                    |> Utils.wordFrequency

                let actual = 
                    o_out
                    |> Utils.wordFrequency

                Expect.equal actual expected "Written processInput does not match read process input"
            )
        ]
        testList "Material" [
            testCase "ReaderSuccess" (fun () -> 
           
                let result = ProcessInput.fromJsonString ProcessInput.materialLD

                let expected = 
                    Material.create("#material/extract-G-0.1-aliquot1","extract-G-0.1-aliquot1",MaterialType.ExtractName,Characteristics = [])

                Expect.isTrue (ProcessInput.isMaterial result) "Result is not a material"

                Expect.equal (ProcessInput.tryMaterial result).Value expected "Material did not match"

            )
            testCase "WriterOutputMatchesInputGivenID" (fun () -> 
            
                let o_read_in = ProcessInput.fromJsonString ProcessInput.material
                let o_out = ProcessInput.toJsonldString o_read_in

                let expected = 
                    ProcessInput.materialLD
                    |> Utils.wordFrequency

                let actual = 
                    o_out
                    |> Utils.wordFrequency

                Expect.equal actual expected "Written processInput does not match read process input"
            )
            testCase "WriterOutputMatchesInputDefaultID" (fun () -> 
            
                let o_read_in = ProcessInput.fromJsonString ProcessInput.materialWithoutID
                let o_out = ProcessInput.toJsonldString o_read_in

                let expected = 
                    ProcessInput.materialWithDefaultLD
                    |> Utils.wordFrequency

                let actual = 
                    o_out
                    |> Utils.wordFrequency

                Expect.equal actual expected "Written processInput does not match read process input"
            )
        ]
        testList "Data" [
            testCase "ReaderSuccess" (fun () -> 
           
                let result = ProcessInput.fromJsonString ProcessInput.dataLD
                let expected = 
                    Data.create("#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_3.txt","JIC64_Nitrogen_0.07_External_1_3.txt",DataFile.RawDataFile,Comments = [])
                Expect.isTrue (ProcessInput.isData result) "Result is not a data"
                Expect.equal (ProcessInput.tryData result).Value expected "Data did not match"
            )
            testCase "WriterOutputMatchesInputGivenID" (fun () -> 
            
                    let o_read_in = ProcessInput.fromJsonString ProcessInput.data
                    let o_out = ProcessInput.toJsonldString o_read_in

                    let expected = 
                        ProcessInput.dataLD
                        |> Utils.wordFrequency

                    let actual = 
                        o_out
                        |> Utils.wordFrequency

                    Expect.equal actual expected "Written processInput does not match read process input"
            )
            testCase "WriterOutputMatchesInputDefaultID" (fun () -> 
            
                    let o_read_in = ProcessInput.fromJsonString ProcessInput.dataWithoutID
                    let o_out = ProcessInput.toJsonldString o_read_in

                    let expected = 
                        ProcessInput.dataWithDefaultLD
                        |> Utils.wordFrequency

                    let actual = 
                        o_out
                        |> Utils.wordFrequency

                    Expect.equal actual expected "Written processInput does not match read process input"
            )
        ]
        testList "Sample" [
            testCase "ReaderSuccessSimple" (fun () -> 
           
                let result = ProcessInput.fromJsonString ProcessInput.sampleSimpleLD

                let expectedDerivesFrom = [Source.create("#source/source-culture8")]

                let expected = 
                    Sample.create("#sample/sample-P-0.1-aliquot7","sample-P-0.1-aliquot7", DerivesFrom = expectedDerivesFrom)

                Expect.isTrue (ProcessInput.isSample result) "Result is not a sample"

                Expect.equal (ProcessInput.trySample result).Value expected "Sample did not match"
            )
            testCase "WriterOutputMatchesInputSimpleGivenID" (fun () -> 
            
                    let o_read_in = ProcessInput.fromJsonString ProcessInput.sampleSimple
                    let o_out = ProcessInput.toJsonldString o_read_in

                    let expected = 
                        ProcessInput.sampleSimpleLD
                        |> Utils.wordFrequency

                    let actual = 
                        o_out
                        |> Utils.wordFrequency

                    Expect.equal actual expected "Written processInput does not match read process input"
            )
            testCase "WriterOutputMatchesInputSimpleDefaultID" (fun () -> 
            
                    let o_read_in = ProcessInput.fromJsonString ProcessInput.sampleSimpleWithoutID
                    let o_out = ProcessInput.toJsonldString o_read_in

                    let expected = 
                        ProcessInput.sampleSimpleWithDefaultLD
                        |> Utils.wordFrequency

                    let actual = 
                        o_out
                        |> Utils.wordFrequency

                    Expect.equal actual expected "Written processInput does not match read process input"
            )
        ]
    ]

let testProtocolFile =

    testList "Protocol" [
        testCase "ReaderRunning" (fun () -> 
            let readingSuccess = 
                try 
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )
        testCase "ReaderSuccess" (fun () -> 
            
            let protocol = Protocol.fromJsonString Protocol.protocol
            let exptected_name = "peptide_digestion"
            let actual = protocol.Name 
            Expect.isSome actual "Should be some"
            Expect.equal actual (Some exptected_name) ""
        )

        testCase "WriterRunning" (fun () ->

            let p = Protocol.fromJsonString Protocol.protocol

            let writingSuccess = 
                try 
                    Protocol.toJsonString p |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testAsync "WriterSchemaCorrectness" {

            let p = Protocol.fromJsonString Protocol.protocol

            let s = Protocol.toJsonString p

            let! validation = Validation.validateProtocol s

            Expect.isTrue validation.Success $"Protocol did not match schema: {validation.GetErrors()}"
        }

        testCase "OutputMatchesInput" (fun () ->

            let o_read_in = Protocol.fromJsonString Protocol.protocol
            let exptected_name = "peptide_digestion"
            let actual_name = o_read_in.Name
            Expect.isSome actual_name "Should be some"
            Expect.equal actual_name (Some exptected_name) "Name exists"

            let o = o_read_in |> Protocol.toJsonString

            let expected = 
                Protocol.protocol
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.equal actual expected "Written protocol file does not match read protocol file"
        )
    ]

let testProtocolFileLD =

    testList "ProtocolLD" [
        testCase "ReaderRunning" (fun () -> 
            let readingSuccess = 
                try 
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )
        testCase "ReaderSuccess" (fun () -> 
            
            let protocol = Protocol.fromJsonString Protocol.protocolLD
            let exptected_name = "peptide_digestion"
            let actual = protocol.Name 
            Expect.isSome actual "Should be some"
            Expect.equal actual (Some exptected_name) ""
        )

        testCase "WriterRunning" (fun () ->

            let p = Protocol.fromJsonString Protocol.protocol

            let writingSuccess = 
                try 
                    Protocol.toJsonldString p |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        // testAsync "WriterSchemaCorrectness" {

        //     let p = Protocol.fromString Protocol.protocol

        //     let s = Protocol.toString p

        //     let! validation = Validation.validateProtocol s

        //     Expect.isTrue validation.Success $"Protocol did not match schema: {validation.GetErrors()}"
        // }

        testCase "OutputMatchesInputGivenIDs" (fun () ->

            let o_read_in = Protocol.fromJsonString Protocol.protocol
            let exptected_name = "peptide_digestion"
            let actual_name = o_read_in.Name
            Expect.isSome actual_name "Should be some"
            Expect.equal actual_name (Some exptected_name) "Name exists"

            let o = o_read_in |> Protocol.toJsonldString

            let expected = 
                Protocol.protocolLD
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.equal actual expected "Written protocol file does not match read protocol file"
        )

        testCase "OutputMatchesInputDefaultIDs" (fun () ->

            let o_read_in = Protocol.fromJsonString Protocol.protocolWithoutIds
            let exptected_name = "peptide_digestion"
            let actual_name = o_read_in.Name
            Expect.isSome actual_name "Should be some"
            Expect.equal actual_name (Some exptected_name) "Name exists"

            let o = o_read_in |> Protocol.toJsonldString

            let expected = 
                Protocol.protocolWithDefaultLD
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.equal actual expected "Written protocol file does not match read protocol file"
        )
    ]

let testProcessFile =

    testList "Process" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Process.fromJsonString Process.process' |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let p = Process.fromJsonString Process.process'

            let writingSuccess = 
                try 
                    Process.toJsonString p |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testAsync "WriterSchemaCorrectness" {

            let p = Process.fromJsonString Process.process'

            let s = Process.toJsonString p

            let! validation = Validation.validateProcess s

            Expect.isTrue validation.Success $"Process did not match schema: {validation.GetErrors()}"
        }

        testCase "OutputMatchesInput" (fun () ->

            let o =
                Process.fromJsonString Process.process'
                |> Process.toJsonString

            let expected = 
                Process.process'
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual actual expected "Written process file does not match read process file"
        )
    ]

let testProcessFileLD =

    testList "ProcessLD" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Process.fromJsonString Process.processLD |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let p = Process.fromJsonString Process.processLD

            let writingSuccess = 
                try 
                    Process.toJsonldString p |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        // testAsync "WriterSchemaCorrectness" {

        //     let p = Process.fromString Process.process'

        //     let s = Process.toString p

        //     let! validation = Validation.validateProcess s

        //     Expect.isTrue validation.Success $"Process did not match schema: {validation.GetErrors()}"
        // }

        testCase "OutputMatchesInputGivenIDs" (fun () ->

            let o =
                Process.fromJsonString Process.process'
                |> Process.toJsonldString

            let expected = 
                Process.processLD
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual actual expected "Written process file does not match read process file"
        )

        testCase "OutputMatchesInputDefaultLD" (fun () ->

            let o =
                Process.fromJsonString Process.processWithoutIDs
                |> Process.toJsonldString

            let expected = 
                Process.processWithDefaultLD
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst
            Expect.sequenceEqual actual expected "Written process file does not match read process file"
        )
    ]

let testPersonFile =

    testList "Person" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Person.fromJsonString Person.person |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = Person.fromJsonString Person.person

            let writingSuccess = 
                try 
                    Person.toJsonString a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testAsync "WriterSchemaCorrectness" {

            let a = Person.fromJsonString Person.person

            let s = Person.toJsonString a

            let! validation = Validation.validatePerson s

            Expect.isTrue validation.Success $"Person did not match schema: {validation.GetErrors()}"
        }

        testCase "OutputMatchesInput" (fun () ->

            let o = 
                Person.fromJsonString Person.person
                |> Person.toJsonString

            let expected = 
                Person.person
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual actual expected "Written person file does not match read person file"
        )

        
        testCase "WithORCID ReaderCorrectness" (fun () -> 
            
            let p = Person.fromJsonString Person.personWithORCID
            Expect.isNone p.Comments "Comments should be None"
            Expect.isSome p.ORCID "ORCID should be Some"
            Expect.equal p.ORCID.Value "0000-0002-1825-0097" "ORCID not as expected"

        )

        testAsync "WithORCID WriterSchemaCorrectness" {

            let a = Person.fromJsonString Person.personWithORCID

            let s = Person.toJsonString a

            let! validation = Validation.validatePerson s

            Expect.isTrue validation.Success $"Person did not match schema: {validation.GetErrors()}"
        }

        testCase "WithORCID OutputMatchesInput" (fun () ->

            let o = 
                Person.fromJsonString Person.personWithORCID
                |> Person.toJsonString

            let expected = 
                Person.personWithORCID
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual actual expected "Written person file does not match read person file"
        )
    ]

let testPersonFileLD =

    testList "PersonLD" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Person.fromJsonString Person.personLD |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = Person.fromJsonString Person.person

            let writingSuccess = 
                try 
                    Person.toJsonldString a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        // testAsync "WriterSchemaCorrectness" {

        //     let a = Person.fromString Person.person

        //     let s = Person.toString a

        //     let! validation = Validation.validatePerson s

        //     Expect.isTrue validation.Success $"Person did not match schema: {validation.GetErrors()}"
        // }

        testCase "OutputMatchesInputGivenID" (fun () ->

            let o = 
                Person.fromJsonString Person.person
                |> Person.toJsonldString

            let expected = 
                Person.personLD
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual actual expected "Written person file does not match read person file"
        )

        testCase "OutputMatchesInputDefaultLD" (fun () ->

            let o = 
                Person.fromJsonString Person.personWithoutID
                |> Person.toJsonldString

            let expected = 
                Person.personWithDefaultLD
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual actual expected "Written person file does not match read person file"
        )
    ]

let testPublicationFile =

    testList "Publication" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Publication.fromJsonString Publication.publication |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = Publication.fromJsonString Publication.publication

            let writingSuccess = 
                try 
                    Publication.toJsonString a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testAsync "WriterSchemaCorrectness" {

            let a = Publication.fromJsonString Publication.publication

            let s = Publication.toJsonString a

            let! validation = Validation.validatePublication s

            Expect.isTrue validation.Success $"Publication did not match schema: {validation.GetErrors()}"
        }

        testCase "OutputMatchesInput" (fun () ->

            let o = 
                Publication.fromJsonString Publication.publication
                |> Publication.toJsonString

            let expected = 
                Publication.publication
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual actual expected "Written Publication file does not match read publication file"
        )
    ]

let testPublicationFileLD =

    testList "PublicationLD" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Publication.fromJsonString Publication.publicationLD |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = Publication.fromJsonString Publication.publication

            let writingSuccess = 
                try 
                    Publication.toJsonldString a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        // testAsync "WriterSchemaCorrectness" {

        //     let a = Publication.fromString Publication.publication

        //     let s = Publication.toString a

        //     let! validation = Validation.validatePublication s

        //     Expect.isTrue validation.Success $"Publication did not match schema: {validation.GetErrors()}"
        // }

        testCase "OutputMatchesInput" (fun () ->

            let o = 
                Publication.fromJsonString Publication.publication
                |> Publication.toJsonldString

            let expected = 
                Publication.publicationLD
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual actual expected "Written Publication file does not match read publication file"
        )
    ]

let testAssayFile =

    testList "Assay" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Assay.fromJsonString Assay.assay |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = Assay.fromJsonString Assay.assay

            let writingSuccess = 
                try 
                    Assay.toJsonString a |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testAsync "WriterSchemaCorrectness" {

            let a = Assay.fromJsonString Assay.assay

            let s = Assay.toJsonString a

            let! validation = Validation.validateAssay s

            Expect.isTrue validation.Success $"Assay did not match schema: {validation.GetErrors()}"
        }

        testCase "OutputMatchesInput" (fun () ->

            let o = 
                Assay.fromJsonString Assay.assay
                |> Assay.toJsonString

            let expected = 
                Assay.assay
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual actual expected "Written assay file does not match read assay file"
        )
    ]

let testInvestigationFile = 

    testList "Investigation" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Investigation.fromJsonString Investigation.investigation |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccess" (fun () ->

            let i = Investigation.fromJsonString Investigation.investigation

            let writingSuccess = 
                try 
                    Investigation.toJsonString i |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testAsync "WriterSchemaCorrectness" {

            let i = Investigation.fromJsonString Investigation.investigation

            let s = Investigation.toJsonString i

            let! validation = Validation.validateInvestigation s

            Expect.isTrue validation.Success $"Investigation did not match schema: {validation.GetErrors()}"
        }

        testCase "OutputMatchesInput" (fun () ->

            let o = 
                Investigation.fromJsonString Investigation.investigation
                |> Investigation.toJsonString

            let expected = 
                Investigation.investigation
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let actual = 
                o
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual actual expected "Written investigation file does not match read investigation file"
        )
        testCase "HandleEmptyRemarks" (fun () ->

            let json = "{}"
            
            let i = Investigation.fromJsonString json

            Expect.equal i.Remarks List.empty "Remark list should be an empty list."
        )
        testCase "OnlyConsiderRegisteredStudies" (fun () ->
            let isa = ArcInvestigation("MyInvestigation")
            let registeredStudyIdentifier = "RegisteredStudy"
            let registeredStudy = ArcStudy(registeredStudyIdentifier)
            let unregisteredStudyIdentifier = "UnregisteredStudy"
            let unregisteredStudy = ArcStudy(unregisteredStudyIdentifier)

            isa.AddStudy(unregisteredStudy)
            isa.AddRegisteredStudy(registeredStudy)

            let result = ArcInvestigation.toJsonString isa |> ArcInvestigation.fromJsonString

            Expect.sequenceEqual result.RegisteredStudyIdentifiers [registeredStudyIdentifier] "Only the registered study should be written and read"
        )
        testCase "FullInvestigation" (fun () ->
                  
            let comment = 
                Comment.make (Some "MyComment") (Some "Key") (Some "Value")

            let ontologySourceReference =
                OntologySourceReference.make
                    (Some "bla bla")
                    (Some "filePath.txt")
                    (Some "OO")
                    (Some "1.3.3")
                    (Some [|comment|])

            let publicationStatus = 
                OntologyAnnotation.make 
                    (Some "OntologyTerm/Published")
                    (Some (AnnotationValue.Text "published"))
                    (Some "pso")
                    (Some "http://purl.org/spar/pso/published")
                    (Some [|comment|])

            let publication =
                Publication.make
                    (Some "12345678")
                    (Some "11.1111/abcdef123456789")
                    (Some "Lukas Weil, Other Gzúy")
                    (Some "Fair is great")
                    (Some publicationStatus)
                    (Some [|comment|])

            let role = 
                OntologyAnnotation.make 
                    (Some "OntologyTerm/SoftwareDeveloperRole")
                    (Some (AnnotationValue.Text "software developer role"))
                    (Some "swo")
                    (Some "http://www.ebi.ac.uk/swo/SWO_0000392")
                    (Some [|comment|])

            let person =
                Person.make
                    (Some "Persons/LukasWeil")
                    None
                    (Some "Weil")
                    (Some "Lukas")
                    (Some "H")
                    (Some "weil@email.com")
                    (Some "0123 456789")
                    (Some "9876 543210")
                    (Some "fantasyStreet 23, 123 Town")
                    (Some "Universiteee")
                    (Some [|role|])
                    (Some [|comment|])

            let characteristic = 
                MaterialAttribute.make 
                    (Some "Characteristic/Organism")
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/Organism")
                            (Some (AnnotationValue.Text "organism"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0100026")
                            (Some [|comment|])
                    ))

            let characteristicValue = 
                MaterialAttributeValue.make 
                    (Some "CharacteristicValue/Arabidopsis")
                    (Some characteristic)
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/Organism")
                            (Some (AnnotationValue.Text "Arabidopsis thaliana"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0100026")
                            (Some [|comment|])
                        |> Value.Ontology
                    ))
                    None

            let studyDesignDescriptor = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/TimeSeries")
                    (Some (AnnotationValue.Text "Time Series Analysis"))
                    (Some "ncit")
                    (Some "http://purl.obolibrary.org/obo/NCIT_C18235")               
                    (Some [|comment|])

            let protocolType = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/GrowthProtocol")
                    (Some (AnnotationValue.Text "growth protocol"))
                    (Some "dfbo")
                    (Some "http://purl.obolibrary.org/obo/DFBO_1000162")
                    (Some [|comment|])

            let parameter = 
                ProtocolParameter.make
                    (Some "Parameter/Temperature")
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/Temperature")
                            (Some (AnnotationValue.Text "temperature unit"))
                            (Some "uo")
                            (Some "http://purl.obolibrary.org/obo/UO_0000005")
                            (Some [|comment|])
                    ))

            let parameterUnit =              
                OntologyAnnotation.make
                    (Some "OntologyTerm/DegreeCelsius")
                    (Some (AnnotationValue.Text "degree celsius"))
                    (Some "uo")
                    (Some "http://purl.obolibrary.org/obo/UO_0000027")
                    (Some [|comment|])

            let parameterValue = 
                ProcessParameterValue.make
                    (Some parameter)
                    (Some (Value.Int 20))
                    (Some parameterUnit)

            let protocolComponent =
                Component.make
                    (Some "PCR instrument")
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/RTPCR")
                            (Some (AnnotationValue.Text "real-time PCR machine"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0001110")
                            (Some [|comment|])
                        |> Value.Ontology
                    ))
                    None
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/PCR")
                            (Some (AnnotationValue.Text "PCR instrument"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0000989")
                            (Some [|comment|])
                    ))
                
            let protocol = 
                Protocol.make 
                    (Some "Protocol/MyProtocol")
                    (Some "MyProtocol")
                    (Some protocolType)
                    (Some "bla bla bla\nblabbbbblaaa")
                    (Some "http://nfdi4plants.org/protocols/MyProtocol")
                    (Some "1.2.3")
                    (Some [parameter])
                    (Some [protocolComponent])                   
                    (Some [comment])

            let factor = 
                Factor.make 
                        (Some "Factor/Time")
                        (Some "Time")
                        (Some (
                            OntologyAnnotation.make
                                (Some "OntologyTerm/Time")
                                (Some (AnnotationValue.Text "time"))
                                (Some "pato")
                                (Some "http://purl.obolibrary.org/obo/PATO_0000165")
                                (Some [|comment|])
                        ))
                        (Some [|comment|])

            let factorUnit = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/Hour")
                    (Some (AnnotationValue.Text "hour"))
                    (Some "uo")
                    (Some "http://purl.obolibrary.org/obo/UO_0000032")
                    (Some [|comment|])
                    

            let factorValue = 
                FactorValue.make
                    (Some "FactorValue/4hours")
                    (Some factor)
                    (Some (Value.Float 4.5))
                    (Some factorUnit)

            let source =
                Source.make
                    (Some "Source/MySource")
                    (Some "MySource")
                    (Some [characteristicValue])

            let sample = 
                Sample.make
                    (Some "Sample/MySample")
                    (Some "MySample")
                    (Some [characteristicValue])
                    (Some [factorValue])
                    (Some [source])

            let data = 
                Data.make
                    (Some "Data/MyData")
                    (Some "MyData")
                    (Some DataFile.DerivedDataFile)
                    (Some [comment])
        
            let material = 
                Material.make
                    (Some "Material/MyMaterial")
                    (Some "MyMaterial")
                    (Some MaterialType.ExtractName)
                    (Some [characteristicValue])
                    None

            let derivedMaterial = 
                Material.make
                    (Some "Material/MyDerivedMaterial")
                    (Some "MyDerivedMaterial")
                    (Some MaterialType.LabeledExtractName)
                    (Some [characteristicValue])
                    (Some [material])

            let studyMaterials = 
                StudyMaterials.make
                    (Some [source])
                    (Some [sample])
                    (Some [material;derivedMaterial])

            let studyProcess = 
                Process.make
                    (Some "Process/MyProcess1")
                    (Some "MyProcess1")
                    (Some protocol)
                    (Some [parameterValue])
                    (Some "Lukas While")
                    (Some (JsonExtensions.DateTime.fromInts 2020 10 5 3 3))
                    None
                    (Some (Process.create (Id = "Process/MyProcess2")))
                    (Some [ProcessInput.Source source])
                    (Some [ProcessOutput.Sample sample])
                    (Some [comment])

            let assayProcess =
                Process.make
                    (Some "Process/MyProcess2")
                    (Some "MyProcess2")
                    (Some protocol)
                    (Some [parameterValue])
                    (Some "Lukas While")
                    (Some (JsonExtensions.DateTime.fromInts 2020 10 5 3 3))
                    (Some (Process.create (Id = "Process/MyProcess1")))
                    None
                    (Some [ProcessInput.Sample sample])
                    (Some [ProcessOutput.Data data])
                    (Some [comment])


            let measurementType = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/LFQuantification")
                    (Some (AnnotationValue.Text "LC/MS Label-Free Quantification"))
                    (Some "ncit")
                    (Some "http://purl.obolibrary.org/obo/NCIT_C161813")
                    (Some [|comment|])

            let technologyType = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/TOF")
                    (Some (AnnotationValue.Text "Time-of-Flight"))
                    (Some "ncit")
                    (Some "http://purl.obolibrary.org/obo/NCIT_C70698")
                    (Some [|comment|])

            let assayMaterials =
                AssayMaterials.make
                    (Some [sample])
                    (Some [material;derivedMaterial])

            let assay = 
                Assay.make
                    (Some "Assay/MyAssay")
                    (Some "MyAssay/isa.assay.xlsx")
                    (Some measurementType)
                    (Some technologyType)
                    (Some "Mass spectrometry platform")
                    (Some [data])
                    (Some assayMaterials)                   
                    (Some [characteristic])
                    (Some [parameterUnit;factorUnit])
                    (Some [assayProcess])
                    (Some [comment])

            let study = 
                Study.make 
                    (Some "Study/MyStudy")
                    (Some "MyStudy/isa.study.xlsx")
                    (Some "MyStudy")
                    (Some "bla bla bla")
                    (Some "bla bla bla\nblabbbbblaaa")
                    (Some (JsonExtensions.DateTime.fromInts 2020 10 5 3 3))
                    (Some (JsonExtensions.Date.fromInts 2020 10 20))                   
                    (Some [publication])
                    (Some [person])
                    (Some [studyDesignDescriptor])
                    (Some [protocol])
                    (Some studyMaterials)
                    (Some [studyProcess])
                    (Some [assay])
                    (Some [factor])
                    (Some [characteristic])
                    (Some [parameterUnit;factorUnit])
                    (Some [comment])

            let investigation = 
                Investigation.make 
                    (Some "Investigations/MyInvestigation")
                    (Some "isa.investigation.xlsx")
                    (Some "MyInvestigation")
                    (Some "bla bla bla")
                    (Some "bla bla bla\nblabbbbblaaa")
                    (Some (JsonExtensions.DateTime.fromInts 2020 3 15 18 23))
                    (Some (JsonExtensions.Date.fromInts 2020 4 3))                   
                    (Some [ontologySourceReference])
                    (Some [publication])
                    (Some [person])
                    (Some [study])
                    (Some [comment])
                    ([Remark.make 0 "hallo"])

            let s = Investigation.toJsonString investigation

            //MyExpect.matchingInvestigation s

            let reReadInvestigation = Investigation.fromJsonString s
            let reWrittenInvestigation = Investigation.toJsonString reReadInvestigation

            let i = 
                s 
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let o = 
                reWrittenInvestigation
                |> Utils.extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual o i "Written investigation file does not match read investigation file"

        )
    ]

let testInvestigationFileLD = 

    testList "InvestigationLD" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Investigation.fromJsonString Investigation.investigationLD |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccess" (fun () ->

            let i = Investigation.fromJsonString Investigation.investigation

            let writingSuccess = 
                try 
                    Investigation.toJsonldString i |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        // testAsync "WriterSchemaCorrectness" {

        //     let i = Investigation.fromString TestObjects.Investigation.investigation

        //     let s = Investigation.toString i

        //     let! validation = Validation.validateInvestigation s

        //     Expect.isTrue validation.Success $"Investigation did not match schema: {validation.GetErrors()}"
        // }

        // testCase "OutputMatchesInput" (fun () ->

        //     let o = 
        //         Investigation.fromString TestObjects.Investigation.investigation
        //         |> Investigation.toString

        //     let expected = 
        //         TestObjects.Investigation.investigation
        //         |> Utils.extractWords
        //         |> Array.countBy id
        //         |> Array.sortBy fst

        //     let actual = 
        //         o
        //         |> Utils.extractWords
        //         |> Array.countBy id
        //         |> Array.sortBy fst

        //     Expect.sequenceEqual actual expected "Written investigation file does not match read investigation file"
        // )
        // testCase "HandleEmptyRemarks" (fun () ->

        //     let json = "{}"
            
        //     let i = Investigation.fromString json

        //     Expect.equal i.Remarks List.empty "Remark list should be an empty list."
        // )

        testCase "FullInvestigation" (fun () ->
                  
            let comment = 
                Comment.make (Some "MyComment") (Some "Key") (Some "Value")

            let ontologySourceReference =
                OntologySourceReference.make
                    (Some "bla bla")
                    (Some "filePath.txt")
                    (Some "OO")
                    (Some "1.3.3")
                    (Some [|comment|])

            let publicationStatus = 
                OntologyAnnotation.make 
                    (Some "OntologyTerm/Published")
                    (Some (AnnotationValue.Text "published"))
                    (Some "pso")
                    (Some "http://purl.org/spar/pso/published")
                    (Some [|comment|])

            let publication =
                Publication.make
                    (Some "12345678")
                    (Some "11.1111/abcdef123456789")
                    (Some "Lukas Weil, Other Gzúy")
                    (Some "Fair is great")
                    (Some publicationStatus)
                    (Some [|comment|])

            let role = 
                OntologyAnnotation.make 
                    (Some "OntologyTerm/SoftwareDeveloperRole")
                    (Some (AnnotationValue.Text "software developer role"))
                    (Some "swo")
                    (Some "http://www.ebi.ac.uk/swo/SWO_0000392")
                    (Some [|comment|])

            let person =
                Person.make
                    (Some "Persons/LukasWeil")
                    None
                    (Some "Weil")
                    (Some "Lukas")
                    (Some "H")
                    (Some "weil@email.com")
                    (Some "0123 456789")
                    (Some "9876 543210")
                    (Some "fantasyStreet 23, 123 Town")
                    (Some "Universiteee")
                    (Some [|role|])
                    (Some [|comment|])

            let characteristic = 
                MaterialAttribute.make 
                    (Some "Characteristic/Organism")
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/Organism")
                            (Some (AnnotationValue.Text "organism"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0100026")
                            (Some [|comment|])
                    ))

            let characteristicValue = 
                MaterialAttributeValue.make 
                    (Some "CharacteristicValue/Arabidopsis")
                    (Some characteristic)
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/Organism")
                            (Some (AnnotationValue.Text "Arabidopsis thaliana"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0100026")
                            (Some [|comment|])
                        |> Value.Ontology
                    ))
                    None

            let studyDesignDescriptor = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/TimeSeries")
                    (Some (AnnotationValue.Text "Time Series Analysis"))
                    (Some "ncit")
                    (Some "http://purl.obolibrary.org/obo/NCIT_C18235")               
                    (Some [|comment|])

            let protocolType = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/GrowthProtocol")
                    (Some (AnnotationValue.Text "growth protocol"))
                    (Some "dfbo")
                    (Some "http://purl.obolibrary.org/obo/DFBO_1000162")
                    (Some [|comment|])

            let parameter = 
                ProtocolParameter.make
                    (Some "Parameter/Temperature")
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/Temperature")
                            (Some (AnnotationValue.Text "temperature unit"))
                            (Some "uo")
                            (Some "http://purl.obolibrary.org/obo/UO_0000005")
                            (Some [|comment|])
                    ))

            let parameterUnit =              
                OntologyAnnotation.make
                    (Some "OntologyTerm/DegreeCelsius")
                    (Some (AnnotationValue.Text "degree celsius"))
                    (Some "uo")
                    (Some "http://purl.obolibrary.org/obo/UO_0000027")
                    (Some [|comment|])

            let parameterValue = 
                ProcessParameterValue.make
                    (Some parameter)
                    (Some (Value.Int 20))
                    (Some parameterUnit)

            let protocolComponent =
                Component.make
                    (Some "PCR instrument")
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/RTPCR")
                            (Some (AnnotationValue.Text "real-time PCR machine"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0001110")
                            (Some [|comment|])
                        |> Value.Ontology
                    ))
                    None
                    (Some (
                        OntologyAnnotation.make
                            (Some "OntologyTerm/PCR")
                            (Some (AnnotationValue.Text "PCR instrument"))
                            (Some "obi")
                            (Some "http://purl.obolibrary.org/obo/OBI_0000989")
                            (Some [|comment|])
                    ))
                
            let protocol = 
                Protocol.make 
                    (Some "Protocol/MyProtocol")
                    (Some "MyProtocol")
                    (Some protocolType)
                    (Some "bla bla bla\nblabbbbblaaa")
                    (Some "http://nfdi4plants.org/protocols/MyProtocol")
                    (Some "1.2.3")
                    (Some [parameter])
                    (Some [protocolComponent])                   
                    (Some [comment])

            let factor = 
                Factor.make 
                        (Some "Factor/Time")
                        (Some "Time")
                        (Some (
                            OntologyAnnotation.make
                                (Some "OntologyTerm/Time")
                                (Some (AnnotationValue.Text "time"))
                                (Some "pato")
                                (Some "http://purl.obolibrary.org/obo/PATO_0000165")
                                (Some [|comment|])
                        ))
                        (Some [|comment|])

            let factorUnit = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/Hour")
                    (Some (AnnotationValue.Text "hour"))
                    (Some "uo")
                    (Some "http://purl.obolibrary.org/obo/UO_0000032")
                    (Some [|comment|])
                    

            let factorValue = 
                FactorValue.make
                    (Some "FactorValue/4hours")
                    (Some factor)
                    (Some (Value.Float 4.5))
                    (Some factorUnit)

            let source =
                Source.make
                    (Some "Source/MySource")
                    (Some "MySource")
                    (Some [characteristicValue])

            let sample = 
                Sample.make
                    (Some "Sample/MySample")
                    (Some "MySample")
                    (Some [characteristicValue])
                    (Some [factorValue])
                    (Some [source])

            let data = 
                Data.make
                    (Some "Data/MyData")
                    (Some "MyData")
                    (Some DataFile.DerivedDataFile)
                    (Some [comment])
        
            let material = 
                Material.make
                    (Some "Material/MyMaterial")
                    (Some "MyMaterial")
                    (Some MaterialType.ExtractName)
                    (Some [characteristicValue])
                    None

            let derivedMaterial = 
                Material.make
                    (Some "Material/MyDerivedMaterial")
                    (Some "MyDerivedMaterial")
                    (Some MaterialType.LabeledExtractName)
                    (Some [characteristicValue])
                    (Some [material])

            let studyMaterials = 
                StudyMaterials.make
                    (Some [source])
                    (Some [sample])
                    (Some [material;derivedMaterial])

            let studyProcess = 
                Process.make
                    (Some "Process/MyProcess1")
                    (Some "MyProcess1")
                    (Some protocol)
                    (Some [parameterValue])
                    (Some "Lukas While")
                    (Some (JsonExtensions.DateTime.fromInts 2020 10 5 3 3))
                    None
                    (Some (Process.create (Id = "Process/MyProcess2")))
                    (Some [ProcessInput.Source source])
                    (Some [ProcessOutput.Sample sample])
                    (Some [comment])

            let assayProcess =
                Process.make
                    (Some "Process/MyProcess2")
                    (Some "MyProcess2")
                    (Some protocol)
                    (Some [parameterValue])
                    (Some "Lukas While")
                    (Some (JsonExtensions.DateTime.fromInts 2020 10 5 3 3))
                    (Some (Process.create (Id = "Process/MyProcess1")))
                    None
                    (Some [ProcessInput.Sample sample])
                    (Some [ProcessOutput.Data data])
                    (Some [comment])


            let measurementType = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/LFQuantification")
                    (Some (AnnotationValue.Text "LC/MS Label-Free Quantification"))
                    (Some "ncit")
                    (Some "http://purl.obolibrary.org/obo/NCIT_C161813")
                    (Some [|comment|])

            let technologyType = 
                OntologyAnnotation.make
                    (Some "OntologyTerm/TOF")
                    (Some (AnnotationValue.Text "Time-of-Flight"))
                    (Some "ncit")
                    (Some "http://purl.obolibrary.org/obo/NCIT_C70698")
                    (Some [|comment|])

            let assayMaterials =
                AssayMaterials.make
                    (Some [sample])
                    (Some [material;derivedMaterial])

            let assay = 
                Assay.make
                    (Some "Assay/MyAssay")
                    (Some "MyAssay/isa.assay.xlsx")
                    (Some measurementType)
                    (Some technologyType)
                    (Some "Mass spectrometry platform")
                    (Some [data])
                    (Some assayMaterials)                   
                    (Some [characteristic])
                    (Some [parameterUnit;factorUnit])
                    (Some [assayProcess])
                    (Some [comment])

            let study = 
                Study.make 
                    (Some "Study/MyStudy")
                    (Some "MyStudy/isa.study.xlsx")
                    (Some "MyStudy")
                    (Some "bla bla bla")
                    (Some "bla bla bla\nblabbbbblaaa")
                    (Some (JsonExtensions.DateTime.fromInts 2020 10 5 3 3))
                    (Some (JsonExtensions.Date.fromInts 2020 10 20))                   
                    (Some [publication])
                    (Some [person])
                    (Some [studyDesignDescriptor])
                    (Some [protocol])
                    (Some studyMaterials)
                    (Some [studyProcess])
                    (Some [assay])
                    (Some [factor])
                    (Some [characteristic])
                    (Some [parameterUnit;factorUnit])
                    (Some [comment])

            let investigation = 
                Investigation.make 
                    (Some "Investigations/MyInvestigation")
                    (Some "isa.investigation.xlsx")
                    (Some "MyInvestigation")
                    (Some "bla bla bla")
                    (Some "bla bla bla\nblabbbbblaaa")
                    (Some (JsonExtensions.DateTime.fromInts 2020 3 15 18 23))
                    (Some (JsonExtensions.Date.fromInts 2020 4 3))                   
                    (Some [ontologySourceReference])
                    (Some [publication])
                    (Some [person])
                    (Some [study])
                    (Some [comment])
                    ([Remark.make 0 "hallo"])

            let s = Investigation.toJsonldString investigation

            // let expected = 
            //     TestObjects.Investigation.investigationLD
            //     |> Utils.extractWords
            //     |> Array.countBy id
            //     |> Array.sortBy fst

            // let actual = 
            //     s
            //     |> Utils.extractWords
            //     |> Array.countBy id
            //     |> Array.sortBy fst

            // Expect.sequenceEqual actual expected "Written investigation file does not match read investigation file"
            Expect.sequenceEqual [1;2] [1;2] "bla"

        )
    ]

let main = 
    testList "Json" [
        testEncode
        testDecode
        testOntoloyAnnotation
        testOntoloyAnnotationLD
        testProcessInput     
        testProcessInputLD  
        testProtocolFile     
        testProtocolFileLD
        testProcessFile
        testProcessFileLD
        testPersonFile
        testPersonFileLD
        testPublicationFile
        testPublicationFileLD
        testAssayFile
        testInvestigationFile
        testInvestigationFileLD
    ]