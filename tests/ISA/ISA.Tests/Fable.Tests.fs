module Fable.Tests

open ARCtrl.ISA

open TestingUtils

let private tests_EmptyObjectCreation = 
    testList "EmptyObjectCreationTests" [
        testCase "CreateEmptyInvestigation" (fun () ->
            let i = ArcInvestigation("My Identifier")
            Expect.isNone i.Title "Should be None"                   
        )
        testCase "CreateEmptyAssay" (fun () ->
            let a = ArcAssay("")
            Expect.isNone a.MeasurementType "Should be None"                
        )
        testCase "CreateEmptyStudy" (fun () ->
            let s = ArcStudy("MY Study")
            Expect.isNone s.Description "Should be None"                 
        )
        testCase "MakeEmptyInvestigation" (fun () ->
            let i = ArcInvestigation.make "My Identifier" None None None None [||] [||] [||] (ResizeArray()) (ResizeArray()) (ResizeArray()) [||] [||]
            Expect.isNone i.Description "Should be None"
            Expect.equal i.Identifier "My Identifier" "Should be None"
        )
    ]

let main = 
    testList "Fable" [
        tests_EmptyObjectCreation
    ]