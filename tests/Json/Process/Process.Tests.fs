module Tests.Process.Process

open ARCtrl
open ARCtrl.Process
open ARCtrl.Json
open TestingUtils
open TestObjects.Json

let main = testList "Process" [
    testCase "ReaderSuccess" <| fun () -> 
        let readingSuccess = 
            try 
                Process.fromISAJsonString Process.process' |> ignore
                Result.Ok "DidRun"
            with
            | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)
        Expect.isOk readingSuccess (Result.getMessage readingSuccess)

    testCase "WriterSuccess" (fun () ->

        let p = Process.fromISAJsonString Process.process'

        let writingSuccess = 
            try 
                Process.toISAJsonString () p |> ignore
                Result.Ok "DidRun"
            with
            | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

        Expect.isOk writingSuccess (Result.getMessage writingSuccess)
    )
    #if !FABLE_COMPILER_PYTHON
    testAsync "WriterSchemaCorrectness" {

        let p = Process.fromISAJsonString Process.process'

        let s = Process.toISAJsonString () p

        let! validation = Validation.validateProcess s

        Expect.isTrue validation.Success $"Process did not match schema: {validation.GetErrors()}"
    }
    #endif
    testCase "OutputMatchesInput" (fun () ->
        let o =
            Process.fromISAJsonString Process.process'
            |> Process.toISAJsonString ()
        let expected = Process.process'
        let actual = o
        Expect.stringEqual actual expected "Written process file does not match read process file"
    )
]
