module DataMapTests


open ARCtrl
open ARCtrl.Spreadsheet
open FsSpreadsheet

open TestingUtils
open TestObjects.Spreadsheet.DataMap

let private simpleTable = 
    testList "simpleTable" [
        let wsName = "isa_datamap"
        let ws = 
                initWorksheet wsName
                    [
                        Data.appendDataColumn               1  
                        Explication.appendMeanColumn        1
                        Unit.appendPPMColumn                1
                        ObjectType.appendFloatColumn        1
                        Description.appendDescriptionColumn 1
                        GeneratedBy.appendGeneratedByColumn 1
                        Label.appendLabelColumn             1

                    ]
        testCase "Read" (fun () -> 
                    
            let table = DataMapTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.DataContexts.Count 1 "Wrong number of rows"

            let dc = table.GetDataContext(0)

            Expect.equal (dc.AsData()) Data.dataValue "Data did not match"

            let explication = Expect.wantSome dc.Explication "Explication was not set"
            Expect.equal explication Explication.meanValue "Explication did not match"

            let unit = Expect.wantSome dc.Unit "Unit was not set"
            Expect.equal unit Unit.ppmValue "Unit did not match"

            let objectType = Expect.wantSome dc.ObjectType "ObjectType was not set"
            Expect.equal objectType ObjectType.floatValue "ObjectType did not match"

            let description = Expect.wantSome dc.Description "Description was not set"
            Expect.equal description Description.descriptionValue "Description did not match"

            let generatedBy = Expect.wantSome dc.GeneratedBy "GeneratedBy was not set"
            Expect.equal generatedBy GeneratedBy.generatedByValue "GeneratedBy did not match"

            let label = Expect.wantSome dc.Label "Label was not set"
            Expect.equal label Label.labelValue "Label did not match"

            Expect.isEmpty dc.Comments "Comments should be empty"
        )
        testCase "Write" (fun () ->            
            let table = DataMapTable.tryFromFsWorksheet ws        
            Expect.isSome table "Table was not created"
            let out = DataMapTable.toFsWorksheet table.Value
            Expect.workSheetEqual out ws "Worksheet was not correctly written"
           
        )
    ]

let private valuelessTable = 
    testList "valuelessTable" [
        let wsName = "isa_datamap"
        let ws = 
                initWorksheet wsName
                    [
                        Data.appendDataColumn               0  
                        Explication.appendMeanColumn        0
                        Unit.appendPPMColumn                0
                        ObjectType.appendFloatColumn        0
                        Description.appendDescriptionColumn 0
                        GeneratedBy.appendGeneratedByColumn 0
                        Label.appendLabelColumn             0
                    ]
        testCase "Read" (fun () -> 
                    
            let table = DataMapTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.DataContexts.Count 0 "Wrong number of rows"

        )
        // TODO: What should we do with units of empty columns?
        //testCase "Write" (fun () -> 
            
        //    let table = ArcTable.tryFromFsWorksheet ws        
        //    Expect.isSome table "Table was not created"
        //    let out = ArcTable.toFsWorksheet table.Value
        //    Expect.workSheetEqual out ws "Worksheet was not correctly written"
           
        //)
    ]

let private emptyTable = 
    testList "emptyTable" [
        let t = DataMap.init()
        testCase "Write" (fun () -> 
            let sheet = DataMapTable.toFsWorksheet t
            Expect.equal "isa_datamap" sheet.Name "Worksheet name did not match"
            Expect.equal 0 sheet.Rows.Count "Row count should be 0"           
        )
        testCase "Read" (fun () ->
            let sheet = DataMapTable.toFsWorksheet t
            Expect.isNone (DataMapTable.tryFromFsWorksheet sheet) "Table was not created"
        )
    ]

let private simpleFile = 
    testList "simpleFile" [
        let wb = new FsWorkbook()
        let wsName = "isa_datamap"
        let ws = 
                initWorksheet wsName
                    [
                        Data.appendDataColumn               1  
                        Explication.appendMeanColumn        1
                        Unit.appendPPMColumn                1
                        ObjectType.appendFloatColumn        1
                        Description.appendDescriptionColumn 1
                        GeneratedBy.appendGeneratedByColumn 1
                        Label.appendLabelColumn             1

                    ]
        wb.AddWorksheet ws
        testCase "Read" (fun () -> 
                    
            let table = DataMap.fromFsWorkbook wb             

            Expect.equal table.DataContexts.Count 1 "Wrong number of rows"

            let dc = table.GetDataContext(0)

            Expect.equal (dc.AsData()) Data.dataValue "Data did not match"

            let explication = Expect.wantSome dc.Explication "Explication was not set"
            Expect.equal explication Explication.meanValue "Explication did not match"

            let unit = Expect.wantSome dc.Unit "Unit was not set"
            Expect.equal unit Unit.ppmValue "Unit did not match"

            let objectType = Expect.wantSome dc.ObjectType "ObjectType was not set"
            Expect.equal objectType ObjectType.floatValue "ObjectType did not match"

            let description = Expect.wantSome dc.Description "Description was not set"
            Expect.equal description Description.descriptionValue "Description did not match"

            let generatedBy = Expect.wantSome dc.GeneratedBy "GeneratedBy was not set"
            Expect.equal generatedBy GeneratedBy.generatedByValue "GeneratedBy did not match"

            let label = Expect.wantSome dc.Label "Label was not set"
            Expect.equal label Label.labelValue "Label did not match"

            Expect.isEmpty dc.Comments "Comments should be empty"
        )
        testCase "Write" (fun () -> 
            
            let table = DataMap.fromFsWorkbook wb     
            
            let out = DataMap.toFsWorkbook table

            Expect.equal (out.GetWorksheets().Count) 1 "Wrong number of worksheets" 

            let wsOut = out.GetWorksheets().[0]

            Expect.workSheetEqual wsOut ws "Worksheet was not correctly written"
           
        )
    ]

let private emptyDatamap = 
    testList "emptyDatamap" [       
        testCase "WriteAndRead" (fun () ->
            let t = DataMap.init()
            let wb = DataMap.toFsWorkbook t
            let datamap = DataMap.fromFsWorkbook wb
            Expect.equal datamap t "DataMap was not correctly written and read"
        )
    ]
let main = 
    testList "DataMapTableTests" [
        simpleTable
        valuelessTable
        emptyTable
        simpleFile
        emptyDatamap
    ]