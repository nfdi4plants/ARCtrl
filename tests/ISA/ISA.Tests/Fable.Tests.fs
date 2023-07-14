module Fable.Tests

open ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let private tests_EmptyObjectCreation = 
    testList "EmptyObjectCreationTests" [
        testCase "CreateEmptyInvestigation" (fun () ->
            let i = ArcInvestigation.create("My Identifier")
            Expect.isNone i.Title "Should be None"                   
        )
        testCase "CreateEmptyAssay" (fun () ->
            let a = ArcAssay.create("")
            Expect.isNone a.MeasurementType "Should be None"                
        )
        testCase "CreateEmptyStudy" (fun () ->
            let s = ArcStudy.create("MY Study")
            Expect.isNone s.FileName "Should be None"                 
        )
        testCase "MakeEmptyInvestigation" (fun () ->
            let i = ArcInvestigation.make None "My Identifier" None None None None [] [] [] (ResizeArray()) [] []
            Expect.isNone i.FileName "Should be None"
            Expect.equal i.Identifier "My Identifier" "Should be None"
        )
        testCase "InitEmptyInvestigation" (fun () ->
            let i : ArcInvestigation = 
                {
                    FileName = None
                    Identifier = System.Guid.NewGuid().ToString()
                    Title = None
                    Description = None
                    SubmissionDate = None
                    PublicReleaseDate = None
                    OntologySourceReferences = []
                    Publications = []
                    Contacts = []
                    Studies = ResizeArray()
                    Comments = []
                    Remarks = []                           
                }
            Expect.isNone i.FileName "Should be None"
        )
    ]

let main = 
    testList "Fable" [
        tests_EmptyObjectCreation
    ]