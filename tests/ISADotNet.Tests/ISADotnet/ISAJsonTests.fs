module ISAJsonTests

open ISADotNet.Json

open Expecto
open TestingUtils
open JsonSchemaValidation

[<Tests>]
let testProtocolFile =

    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
    let sinkDirectory = System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + @"/TestResult/").FullName
    let referenceProtocolFilePath = System.IO.Path.Combine(sourceDirectory,"ProtocolTestFile.json")
    let outputProtocolFilePath = System.IO.Path.Combine(sinkDirectory,"new.ProtocolTestFile.json")

    testList "ProtocolJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Protocol.fromFile referenceProtocolFilePath |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let p = Protocol.fromFile referenceProtocolFilePath



            let writingSuccess = 
                try 
                    Protocol.toFile outputProtocolFilePath p
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSchemaCorrectness" (fun () ->

            let p = Protocol.fromFile referenceProtocolFilePath

            let s = Protocol.toString p

            Expect.matchingProtocol s
        )

        testCase "OutputMatchesInput" (fun () ->

            let i = Protocol.fromFile referenceProtocolFilePath

            let o = Protocol.fromFile outputProtocolFilePath

            Expect.equal o i "Written protocol file does not match read protocol file"
        )
        |> testSequenced
    ]

[<Tests>]
let testProcessFile =

    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
    let sinkDirectory = System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + @"/TestResult/").FullName
    let referenceProcessFilePath = System.IO.Path.Combine(sourceDirectory,"ProcessTestFile.json")
    let outputProcessFilePath = System.IO.Path.Combine(sinkDirectory,"new.ProcessTestFile.json")

    testList "ProcessJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Process.fromFile referenceProcessFilePath |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let p = Process.fromFile referenceProcessFilePath

            let writingSuccess = 
                try 
                    Process.toFile outputProcessFilePath p
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSchemaCorrectness" (fun () ->

            let p = Process.fromFile referenceProcessFilePath

            let s = Process.toString p

            Expect.matchingProcess s
        )

        testCase "OutputMatchesInput" (fun () ->

            let extractWords (json:string) = 
                json.Split([|'{';'}';'[';']';',';':'|])
                |> Array.map (fun s -> s.Trim())
                |> Array.filter ((<>) "")

            let i = 
                System.IO.File.ReadAllText referenceProcessFilePath 
                |> extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let o = 
                System.IO.File.ReadAllText outputProcessFilePath 
                |> extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual o i "Written process file does not match read process file"
        )
        |> testSequenced
    ]

[<Tests>]
let testAssayFile =

    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
    let sinkDirectory = System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + @"/TestResult/").FullName
    let referenceAssayFilePath = System.IO.Path.Combine(sourceDirectory,"AssayTestFile.json")
    let outputAssayFilePath = System.IO.Path.Combine(sinkDirectory,"new.AssayTestFile.json")

    testList "AssayJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Assay.fromFile referenceAssayFilePath |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = Assay.fromFile referenceAssayFilePath

            let writingSuccess = 
                try 
                    Assay.toFile outputAssayFilePath a
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSchemaCorrectness" (fun () ->

            let a = Assay.fromFile referenceAssayFilePath

            let s = Assay.toString a

            Expect.matchingAssay s
        )

        testCase "OutputMatchesInput" (fun () ->

            let extractWords (json:string) = 
                json.Split([|'{';'}';'[';']';',';':'|])
                |> Array.map (fun s -> s.Trim())
                |> Array.filter ((<>) "")

            let i = 
                System.IO.File.ReadAllText referenceAssayFilePath 
                |> extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let o = 
                System.IO.File.ReadAllText outputAssayFilePath 
                |> extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual o i "Written assay file does not match read investigation file"
        )
        |> testSequenced
    ]

[<Tests>]
let testInvestigationFile = 

    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
    let sinkDirectory = System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + @"/TestResult/").FullName
    let referenceInvestigationFilePath = System.IO.Path.Combine(sourceDirectory,"InvestigationTestFile.json")
    let outputInvestigationFilePath = System.IO.Path.Combine(sinkDirectory,"new.InvestigationTestFile.json")

    testList "InvestigationJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Investigation.fromFile referenceInvestigationFilePath |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )

        testCase "WriterSuccess" (fun () ->

            let i = Investigation.fromFile referenceInvestigationFilePath

            let writingSuccess = 
                try 
                    Investigation.toFile outputInvestigationFilePath i
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterSchemaCorrectness" (fun () ->

            let i = Investigation.fromFile referenceInvestigationFilePath

            let s = Investigation.toString i

            Expect.matchingInvestigation s
        )

        testCase "OutputMatchesInput" (fun () ->

            let extractWords (json:string) = 
                json.Split([|'{';'}';'[';']';',';':'|])
                |> Array.map (fun s -> s.Trim())
                |> Array.filter ((<>) "")

            let i = 
                System.IO.File.ReadAllText referenceInvestigationFilePath 
                |> extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            let o = 
                System.IO.File.ReadAllText outputInvestigationFilePath 
                |> extractWords
                |> Array.countBy id
                |> Array.sortBy fst

            Expect.sequenceEqual o i "Written investigation file does not match read investigation file"
        )
        |> testSequenced
    ]