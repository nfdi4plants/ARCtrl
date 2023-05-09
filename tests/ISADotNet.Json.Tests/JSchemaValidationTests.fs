module JSchemaValidationTests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif


open ISADotNet.Validation

let jsInteropTests = testList "FableValidation" [ 
    testCase "Hello World" (fun () -> 
        let actual = Fable.JsonValidation.helloWorld()
        let expected = "Hello World"
        Expect.equal actual expected "Test if js validator is correctly referenced with hello world example"
    )
    //testAsync "Minimal example" (fun () -> 
    //    let commentSchemaURL = "https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/comment_schema.json"
    //    let commentInstance = """{
    //        "name": "velit amet",
    //        "value": "minim ut reprehenderit cillum commodo"
    //    }"""
    //    ISADotNet.Fable.print("Hit before")
    //    let actual = Fable.validate commentSchemaURL commentInstance
    //    ISADotNet.Fable.print("Hit after")
    //    ISADotNet.Fable.print(actual)
    //    Expect.equal 0 0 "Test if js validator is correctly referenced with hello world example"
    //)
    testAsync "Minimal example" { 
        let commentSchemaURL = "https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/comment_schema.json"
        let commentInstance = """{
            "name": "velit amet",
            "value": "minim ut reprehenderit cillum commodo"
        }"""
        let! actual = Fable.validate commentSchemaURL commentInstance
        ISADotNet.Fable.print(actual.instance)
        Expect.equal 0 0 "Test if js validator is correctly referenced with hello world example"
    }
]


//open TestingUtils
//open ISADotNet
//open ISADotNet.Validation
//open System.IO

//let testProcessValidation =     

//    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/ValidationTestFiles/"

//    testList "ProcessValidationTests" [

//        testCase "ProcessDateCorrect" (fun () -> 
            
//            let vr = JSchema.validateProcess TestFiles.Validation.processDate

//            Expect.isTrue vr.Success (sprintf "Process schema validation should have succeded but did not: %A" (vr.GetErrors()))

//        )

//        testCase "ProcessDateTimeCorrect" (fun () -> 
            
//            let vr = JSchema.validateProcess TestFiles.Validation.processDateTime

//            Expect.isTrue vr.Success (sprintf "Process schema validation should have succeded but did not: %A" (vr.GetErrors()))

//        )

//        testCase "ProcessDateWrong" (fun () -> 
            
//            let vr = JSchema.validateProcess TestFiles.Validation.processDateWrong

//            Expect.isFalse vr.Success "Process schema validation should have failed but did not"

//        )

//        testCase "ProcessAdditionalField" (fun () -> 
            
//            let vr = JSchema.validateProcess TestFiles.Validation.processAdditionalField

//            Expect.isFalse vr.Success "Process schema validation should have failed but did not"

//        )

//        testCase "ProcessBroken" (fun () -> 
            
//            let vr = JSchema.validateProcess TestFiles.Validation.processBroken

//            Expect.isFalse vr.Success "Process schema validation should have failed but did not"

//        )
//    ]

//let testProcessParameterValueValidation =     

//    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/ValidationTestFiles/"

//    testList "ProcessParameterValueValidationTests" [

//        testCase "PPVUnit" (fun () -> 
            
//            let vr = JSchema.validateProcessParameterValue TestFiles.Validation.ppvUnit

//            Expect.isTrue vr.Success (sprintf "Process parameter value schema validation should have succeded but did not: %A" (vr.GetErrors()))

//        )

//        testCase "PPVOntology" (fun () -> 
            
//            let vr = JSchema.validateProcessParameterValue TestFiles.Validation.ppvOntology

//            Expect.isTrue vr.Success (sprintf "Process parameter value schema validation should have succeded but did not: %A" (vr.GetErrors()))

//        )

//        testCase "PPVUriWrong" (fun () -> 
            
//            let vr = JSchema.validateProcessParameterValue TestFiles.Validation.ppvUriWrong

//            Expect.isFalse vr.Success "Process schema validation should have failed but did not"
            
//        )

//        testCase "PPVAdditionalField" (fun () -> 
            
//            let vr = JSchema.validateProcessParameterValue TestFiles.Validation.ppvAdditionalField

//            Expect.isFalse vr.Success "Process schema validation should have failed but did not"

//        )

//        testCase "PPVBroken" (fun () -> 
            
//            let vr = JSchema.validateProcessParameterValue TestFiles.Validation.ppvBroken

//            Expect.isFalse vr.Success "Process schema validation should have failed but did not"

//        )
//    ]

let main = 
    testList "APITests" [
        #if FABLE_COMPILER
        jsInteropTests
        #endif
        //testProcessValidation
        //testProcessParameterValueValidation
    ]