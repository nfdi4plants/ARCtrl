module ISAJsonTests

open ISADotNet.Json

open Expecto
open TestingUtils


[<Tests>]
let testProtocolFile =

    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
    let sinkDirectory = System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + @"/TestResult/").FullName
    let referenceInvestigationFilePath = System.IO.Path.Combine(sourceDirectory,"ProtocolTestFile.json")
    let outputInvestigationFilePath = System.IO.Path.Combine(sinkDirectory,"new.ProtocolTestFile.json")

    testList "ProtocolJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Protocol.fromFile referenceInvestigationFilePath |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let p = Protocol.fromFile referenceInvestigationFilePath

            let writingSuccess = 
                try 
                    Protocol.toFile outputInvestigationFilePath p
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "OutputMatchesInput" (fun () ->

            let i = Protocol.fromFile referenceInvestigationFilePath

            let o = Protocol.fromFile outputInvestigationFilePath

            Expect.equal o i "Written protocol file does not match read investigation file"
        )
        |> testSequenced
    ]

[<Tests>]
let testProcessFile =

    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
    let sinkDirectory = System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + @"/TestResult/").FullName
    let referenceInvestigationFilePath = System.IO.Path.Combine(sourceDirectory,"ProcessTestFile.json")
    let outputInvestigationFilePath = System.IO.Path.Combine(sinkDirectory,"new.ProcessTestFile.json")

    testList "ProcessJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Process.fromFile referenceInvestigationFilePath |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let p = Process.fromFile referenceInvestigationFilePath

            let writingSuccess = 
                try 
                    Process.toFile outputInvestigationFilePath p
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
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

            Expect.sequenceEqual o i "Written process file does not match read investigation file"
        )
        |> testSequenced
    ]

[<Tests>]
let testAssayFile =

    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
    let sinkDirectory = System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + @"/TestResult/").FullName
    let referenceInvestigationFilePath = System.IO.Path.Combine(sourceDirectory,"AssayTestFile.json")
    let outputInvestigationFilePath = System.IO.Path.Combine(sinkDirectory,"new.AssayTestFile.json")

    testList "AssayJsonTests" [
        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    Assay.fromFile referenceInvestigationFilePath |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "WriterSuccess" (fun () ->

            let a = Assay.fromFile referenceInvestigationFilePath

            let writingSuccess = 
                try 
                    Assay.toFile outputInvestigationFilePath a
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
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