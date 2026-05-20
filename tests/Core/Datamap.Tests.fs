module Datamap.Tests

open ARCtrl
open ARCtrl.Helper
open TestingUtils

let private tests_hashcode =
    testList "HashCode" [
        testCase "empty does not retun 0" <| fun _ ->
            let dm = Datamap(ResizeArray())
            let hash = dm.GetHashCode()
            Expect.notEqual hash 0 "Hash code of empty datamap should not be 0"
            
    ]


let main = 
    testList "Datamap" [
        tests_hashcode
    ]