module Tests.Process.Protocol

open ARCtrl
open ARCtrl.Process
open ARCtrl.Json
open TestingUtils
open TestObjects.Json

let main = testList "Protocol" [
    testCase "ReaderRunning" (fun () -> 
        let readingSuccess = 
            try 
                Result.Ok "DidRun"
            with
            | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)
        Expect.isOk readingSuccess (Result.getMessage readingSuccess)
    )
    testCase "ReaderSuccess" (fun () -> 
            
        let protocol = Protocol.fromISAJsonString Protocol.protocol
        let exptected_name = "peptide_digestion"
        let actual = protocol.Name 
        Expect.isSome actual "Should be some"
        Expect.equal actual (Some exptected_name) ""
    )

    testCase "WriterRunning" (fun () ->

        let p = Protocol.fromISAJsonString Protocol.protocol

        let writingSuccess = 
            try 
                Protocol.toISAJsonString () p |> ignore
                Result.Ok "DidRun"
            with
            | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

        Expect.isOk writingSuccess (Result.getMessage writingSuccess)
    )
    #if !FABLE_COMPILER_PYTHON
    testAsync "WriterSchemaCorrectness" {

        let p = Protocol.fromISAJsonString Protocol.protocol

        let s = Protocol.toISAJsonString () p

        let! validation = Validation.validateProtocol s

        Expect.isTrue validation.Success $"Protocol did not match schema: {validation.GetErrors()}"
    }
    #endif

    testCase "OutputMatchesInput" (fun () ->

        let o_read_in = Protocol.fromISAJsonString Protocol.protocol
        let exptected_name = "peptide_digestion"
        let actual_name = o_read_in.Name
        Expect.isSome actual_name "Should be some"
        Expect.equal actual_name (Some exptected_name) "Name exists"

        let actual = o_read_in |> Protocol.toISAJsonString ()
        let expected = Protocol.protocol
        Expect.stringEqual actual expected "Written protocol file does not match read protocol file"
    )
]