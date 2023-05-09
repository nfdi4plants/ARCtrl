module JSchemaValidationTests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

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
        //testProcessValidation
        //testProcessParameterValueValidation
    ]