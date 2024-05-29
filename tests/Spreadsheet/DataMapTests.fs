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

                    ]
        testCase "Read" (fun () -> 
                    
            let table = DataMapTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.Table.ColumnCount 6 "Wrong number of columns"
            Expect.equal table.Table.RowCount 1 "Wrong number of rows"

            let expectedHeaders = 
                [
                        DataMapAux.dataHeader
                        DataMapAux.explicationHeader
                        DataMapAux.unitHeader
                        DataMapAux.objectTypeHeader
                        DataMapAux.descriptionHeader
                        DataMapAux.generatedByHeader
                ]
            Expect.sequenceEqual table.Headers expectedHeaders "Headers did not match"

            let expectedCells = 
                [
                        Data.dataValue
                        Explication.meanValue
                        Unit.ppmValue
                        ObjectType.floatValue
                        Description.descriptionValue
                        GeneratedBy.generatedByValue
                ]
            Expect.sequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
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
                    ]
        testCase "Read" (fun () -> 
                    
            let table = DataMapTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.Table.ColumnCount 6 "Wrong number of columns"
            Expect.equal table.Table.RowCount 0 "Wrong number of rows"

            let expectedHeaders = 
                [
                        DataMapAux.dataHeader
                        DataMapAux.explicationHeader
                        DataMapAux.unitHeader
                        DataMapAux.objectTypeHeader
                        DataMapAux.descriptionHeader
                        DataMapAux.generatedByHeader
                ]
            Expect.sequenceEqual table.Headers expectedHeaders "Headers did not match"
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

                    ]
        wb.AddWorksheet ws
        testCase "Read" (fun () -> 
                    
            let table = DataMap.fromFsWorkbook wb             

            Expect.equal table.Table.ColumnCount 6 "Wrong number of columns"
            Expect.equal table.Table.RowCount 1 "Wrong number of rows"

            let expectedHeaders = 
                [
                        DataMapAux.dataHeader
                        DataMapAux.explicationHeader
                        DataMapAux.unitHeader
                        DataMapAux.objectTypeHeader
                        DataMapAux.descriptionHeader
                        DataMapAux.generatedByHeader
                ]
            Expect.sequenceEqual table.Headers expectedHeaders "Headers did not match"

            let expectedCells = 
                [
                        Data.dataValue
                        Explication.meanValue
                        Unit.ppmValue
                        ObjectType.floatValue
                        Description.descriptionValue
                        GeneratedBy.generatedByValue
                ]
            Expect.sequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
        )
        testCase "Write" (fun () -> 
            
            let table = DataMap.fromFsWorkbook wb     
            
            let out = DataMap.toFsWorkbook table

            Expect.equal (out.GetWorksheets().Count) 1 "Wrong number of worksheets" 

            let wsOut = out.GetWorksheets().[0]

            Expect.workSheetEqual wsOut ws "Worksheet was not correctly written"
           
        )
    ]

let main = 
    testList "DataMapTableTests" [
        simpleTable
        valuelessTable
        emptyTable
        simpleFile
    ]