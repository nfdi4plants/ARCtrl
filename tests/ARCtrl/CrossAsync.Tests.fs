module ARCtrl.CrossAsync.Tests

open TestingUtils
open ARCtrl
open CrossAsync

let catchWith = 
    testList "CatchWith" [
        testCaseCrossAsync "simple" (crossAsync {
            let internalAsync =
                crossAsync {
                    failwith "This is an error"
                    return "Just strolling around"
                }
                |> catchWith (fun e -> "Caught error")
            let! internalResult = internalAsync
            Expect.equal internalResult "Caught error" "Error was not caught"
        })
    ]

let main = 
    testList "CrossAsyncTests" [
        catchWith
    ]
