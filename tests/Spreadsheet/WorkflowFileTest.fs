module ArcWorkflowTests


open ARCtrl
open TestingUtils
open ARCtrl.Spreadsheet

open TestObjects.Spreadsheet

let testMetaDataFunctions = 

    testList "WorkflowMetadataTests" [

        testCase "ReaderSuccess" (fun () -> 
            
            let readingSuccess = 
                try 
                    ArcWorkflow.fromMetadataSheet Workflow.Proteomics.workflowMetadata |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Reading the test file failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)

        )

        testCase "ReaderReadsORCID" (fun () -> 
            
            let workflow = ArcWorkflow.fromMetadataSheet Workflow.Proteomics.workflowMetadata
            Expect.equal workflow.Contacts.Count 3 "Workflow should have 3 contacts"
            Expect.isSome workflow.Contacts.[0].ORCID "ORCID should be set"
            Expect.equal workflow.Contacts.[0].ORCID.Value "0000-0002-1825-0097" "ORCID not read correctly"
            Expect.isNone workflow.Contacts.[1].ORCID "ORCID should not be set"
            Expect.isSome workflow.Contacts.[2].ORCID "ORCID should be set"
            Expect.equal workflow.Contacts.[2].ORCID.Value "0000-0002-1825-0098" "ORCID not read correctly"

        )

        testCase "WriterSuccess" (fun () ->

            let w = ArcWorkflow.fromMetadataSheet Workflow.Proteomics.workflowMetadata

            let writingSuccess = 
                try 
                    ArcWorkflow.toMetadataSheet w |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Error(sprintf "Writing the test file failed: %s" err.Message)

            Expect.isOk writingSuccess (Result.getMessage writingSuccess)
        )

        testCase "WriterCreatesNoEmptyCells" (fun () ->

            let o = 
                ArcWorkflow.fromMetadataSheet Workflow.Proteomics.workflowMetadata
                |> ArcWorkflow.toMetadataSheet
                
            o.CellCollection.GetCells()
            |> Seq.iter (fun c -> Expect.notEqual (c.ValueAsString().Trim()) "" $"Cell {c.Address.ToString()} should not contain empty string")  
        )

        testCase "OutputMatchesInput" (fun () ->
           
            let o =             
                Workflow.Proteomics.workflowMetadata
                |> ArcWorkflow.fromMetadataSheet 
                |> ArcWorkflow.toMetadataSheet
                
            Expect.workSheetEqual o Workflow.Proteomics.workflowMetadata "Written workflow metadata does not match read assay metadata"
        )                
    ]


let main = 
    testList "WorkflowFile" [
        testMetaDataFunctions      
    ]