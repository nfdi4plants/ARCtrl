module AssayTests

open Expecto
open System.Text.Json

//[<Tests>]
let testComponentCasting =

    testList "Test" [
        testCase "WillFail" (fun () -> 
            Expect.isTrue true "Test if the test will test."
            
        )
    ]