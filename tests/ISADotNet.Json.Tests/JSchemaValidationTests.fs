module JSchemaValidationTests

open Expecto
open TestingUtils
open ISADotNet
open System.Text.Json
open ISADotNet.Validation
open System.IO

[<Tests>]
let testProcessValidation =     

    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/ValidationTestFiles/"

    testList "ProcessValidationTests" [

        testCase "ProcessDateCorrect" (fun () -> 
            
            let p = File.ReadAllText(Path.Combine(sourceDirectory,"ProcessDate.json"))

            let vr = JSchema.validateProcess p

            Expect.isTrue vr.Success (sprintf "Process schema validation should have succeded but did not: %A" (vr.GetErrors()))

        )

        testCase "ProcessDateTimeCorrect" (fun () -> 
            
            let p = File.ReadAllText(Path.Combine(sourceDirectory,"ProcessDateTime.json"))

            let vr = JSchema.validateProcess p

            Expect.isTrue vr.Success (sprintf "Process schema validation should have succeded but did not: %A" (vr.GetErrors()))

        )

        testCase "ProcessDateWrong" (fun () -> 
            
            let p = File.ReadAllText(Path.Combine(sourceDirectory,"ProcessDateWrong.json"))

            let vr = JSchema.validateProcess p

            Expect.isFalse vr.Success "Process schema validation should have failed but did not"

        )

        testCase "ProcessAdditionalField" (fun () -> 
            
            let p = File.ReadAllText(Path.Combine(sourceDirectory,"ProcessAdditionalField.json"))

            let vr = JSchema.validateProcess p

            Expect.isFalse vr.Success "Process schema validation should have failed but did not"

        )

        testCase "ProcessBroken" (fun () -> 
            
            let p = File.ReadAllText(Path.Combine(sourceDirectory,"ProcessBroken.json"))

            let vr = JSchema.validateProcess p

            Expect.isFalse vr.Success "Process schema validation should have failed but did not"

        )
    ]

[<Tests>]
let testProcessParameterValueValidation =     

    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/ValidationTestFiles/"

    testList "ProcessParameterValueValidationTests" [

        testCase "PPVUnit" (fun () -> 
            
            let p = File.ReadAllText(Path.Combine(sourceDirectory,"PPVUnit.json"))

            let vr = JSchema.validateProcessParameterValue p

            Expect.isTrue vr.Success (sprintf "Process parameter value schema validation should have succeded but did not: %A" (vr.GetErrors()))

        )

        testCase "PPVOntology" (fun () -> 
            
            let p = File.ReadAllText(Path.Combine(sourceDirectory,"PPVOntology.json"))

            let vr = JSchema.validateProcessParameterValue p

            Expect.isTrue vr.Success (sprintf "Process parameter value schema validation should have succeded but did not: %A" (vr.GetErrors()))

        )

        testCase "PPVUriWrong" (fun () -> 
            
            let p = File.ReadAllText(Path.Combine(sourceDirectory,"PPVUriWrong.json"))

            let vr = JSchema.validateProcessParameterValue p

            Expect.isFalse vr.Success "Process schema validation should have failed but did not"
            
        )

        testCase "PPVAdditionalField" (fun () -> 
            
            let p = File.ReadAllText(Path.Combine(sourceDirectory,"PPVAdditionalField.json"))

            let vr = JSchema.validateProcessParameterValue p

            Expect.isFalse vr.Success "Process schema validation should have failed but did not"

        )

        testCase "PPVBroken" (fun () -> 
            
            let p = File.ReadAllText(Path.Combine(sourceDirectory,"PPVBroken.json"))

            let vr = JSchema.validateProcessParameterValue p

            Expect.isFalse vr.Success "Process schema validation should have failed but did not"

        )
    ]