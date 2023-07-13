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
            let i = ArcInvestigation.create("")
            Expect.isNone i.ID "Should be None"                   
        )
        testCase "CreateEmptyAssay" (fun () ->
            let a = ArcAssay.create("")
            Expect.isNone a.ID "Should be None"                
        )
        testCase "CreateEmptyStudy" (fun () ->
            let s = ArcStudy.create("")
            Expect.isNone s.ID "Should be None"                 
        )
        testCase "MakeEmptyInvestigation" (fun () ->
            let i = ArcInvestigation.make None None None None None None None [] [] [] (ResizeArray()) [] []
            Expect.isNone i.ID "Should be None"
        )
        testCase "InitEmptyInvestigation" (fun () ->
            let i = 
                {
                    ID = None 
                    FileName = None
                    Identifier = None
                    Title = None
                    Description = None
                    SubmissionDate = None
                    PublicReleaseDate = None
                    OntologySourceReferences = None
                    Publications = None
                    Contacts = None
                    Studies = None
                    Comments = None
                    Remarks = []                           
                }
            Expect.isNone i.ID "Should be None"
        )
    ]

let main = 
    testList "Fable" [
        tests_EmptyObjectCreation
    ]