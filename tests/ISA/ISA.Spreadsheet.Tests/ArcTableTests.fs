module ArcTableTests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif


open ARCtrl.ISA
open ARCtrl.ISA.Spreadsheet

open TestingUtils
open TestObjects.Spreadsheet.ArcTable

let private ensureCorrectTestHeaders = 
    testList "ensureCorrectTestHeaders" [
        testCase "duplicateParam" (fun () -> 
            let ws = 
                initWorksheet "TestWorksheet"
                    [
                        Parameter.appendInstrumentColumn     1
                        Parameter.appendInstrumentColumn     1
                    ]
            Expect.equal ws.Name "TestWorksheet" "Worksheet name did not match"

            let expectedHeaders = 
                [
                    Parameter.instrumentHeaderV1
                    Parameter.instrumentHeaderV2
                    Parameter.instrumentHeaderV3
                    Parameter.instrumentHeaderV1 + " "
                    Parameter.instrumentHeaderV2 + " "
                    Parameter.instrumentHeaderV3 + " "
                
                ]
            Expect.mySequenceEqual (ws.Row(1).Cells |> Seq.map (fun c -> c.Value)) expectedHeaders "Headers did not match"

            let expectedValues = 
                [
                    Parameter.instrumentValueV1
                    Parameter.instrumentValueV2
                    Parameter.instrumentValueV3
                    Parameter.instrumentValueV1
                    Parameter.instrumentValueV2
                    Parameter.instrumentValueV3
                ]

            Expect.mySequenceEqual (ws.Row(2).Cells |> Seq.map (fun c -> c.Value)) expectedValues "Values did not match"
        )
    ]

let private groupCols = 
    testList "groupCols" [
        testCase "simpleTable" (fun () ->
            let ws = 
                initTableCols
                    [
                        Protocol.REF.appendLolColumn         1
                        Protocol.Type.appendCollectionColumn 1
                        Parameter.appendTemperatureColumn    1
                        Parameter.appendInstrumentColumn     1
                        Characteristic.appendOrganismColumn  1
                        Factor.appendTimeColumn              1
                    ]
            let grouped = ArcTable.groupColumnsByHeader ws
        
            let expectedHeaderGroups = 
                [   
                    Protocol.REF.lolHeaderV1
                    Protocol.Type.collectionHeaderV1+ ";" + Protocol.Type.collectionHeaderV2+ ";" +  Protocol.Type.collectionHeaderV3
                    Parameter.temperatureHeaderV1+ ";" +  Parameter.temperatureHeaderV2+ ";" +  Parameter.temperatureHeaderV3+ ";" +  Parameter.temperatureHeaderV4
                    Parameter.instrumentHeaderV1+ ";" +  Parameter.instrumentHeaderV2+ ";" +  Parameter.instrumentHeaderV3
                    Characteristic.organismHeaderV1+ ";" +  Characteristic.organismHeaderV2+ ";" +  Characteristic.organismHeaderV3
                    Factor.timeHeaderV1+ ";" +  Factor.timeHeaderV2+ ";" +  Factor.timeHeaderV3+ ";" + Factor.timeHeaderV4          
                ]
            let actualHeaderGroups =
                grouped
                |> List.map (fun cols -> cols |> List.map (fun c -> c.[1].Value) |> List.reduce (fun a b -> a + ";" + b))
            Expect.mySequenceEqual actualHeaderGroups expectedHeaderGroups "Header groups did not match"
        )
    ]

let private simpleTable = 
    testList "simpleTable" [
        let wsName = "MyWorksheet"
        let ws = 
                initWorksheet wsName
                    [
                        Protocol.REF.appendLolColumn            1
                        Protocol.Type.appendCollectionColumn    1
                        Protocol.Component.appendInstrumentColumn 1
                        Parameter.appendTemperatureColumn       1
                        Parameter.appendInstrumentColumn        1
                        Characteristic.appendOrganismColumn     1
                        Factor.appendTimeColumn                 1
                    ]
        testCase "Read" (fun () -> 
                    
            let table = ArcTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.Name wsName "Name did not match"
            Expect.equal table.ColumnCount 7 "Wrong number of columns"
            Expect.equal table.RowCount 1 "Wrong number of rows"

            let expectedHeaders = 
                [
                        Protocol.REF.lolHeader
                        Protocol.Type.collectionHeader
                        Protocol.Component.instrumentHeader
                        Parameter.temperatureHeader
                        Parameter.instrumentHeader
                        Characteristic.organismHeader
                        Factor.timeHeader
                ]
            Expect.mySequenceEqual table.Headers expectedHeaders "Headers did not match"

            let expectedCells = 
                [
                        Protocol.REF.lolValue
                        Protocol.Type.collectionValue
                        Protocol.Component.instrumentValue
                        Parameter.temperatureValue
                        Parameter.instrumentValue
                        Characteristic.organismValue
                        Factor.timeValue
                ]
            Expect.mySequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
        )
        testCase "Write" (fun () -> 
            
            let table = ArcTable.tryFromFsWorksheet ws        
            Expect.isSome table "Table was not created"
            let out = ArcTable.toFsWorksheet table.Value
            Expect.workSheetEqual out ws "Worksheet was not correctly written"
           
        )
    ]

let private emptyTable = 
    testList "emptyTable" [
        let wsName = "MyWorksheet"
        let ws = 
                initWorksheet wsName
                    [
                        Protocol.REF.appendLolColumn            0
                        Protocol.Type.appendCollectionColumn    0
                        Parameter.appendTemperatureColumn       0
                        Parameter.appendInstrumentColumn        0
                        Characteristic.appendOrganismColumn     0
                        Factor.appendTimeColumn                 0
                    ]
        testCase "Read" (fun () -> 
                    
            let table = ArcTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.Name wsName "Name did not match"
            Expect.equal table.ColumnCount 6 "Wrong number of columns"
            Expect.equal table.RowCount 0 "Wrong number of rows"

            let expectedHeaders = 
                [
                        Protocol.REF.lolHeader
                        Protocol.Type.collectionHeader
                        Parameter.temperatureHeader
                        Parameter.instrumentHeader
                        Characteristic.organismHeader
                        Factor.timeHeader
                ]
            Expect.mySequenceEqual table.Headers expectedHeaders "Headers did not match"
        )
        // TODO: What should we do with units of empty columns?
        //testCase "Write" (fun () -> 
            
        //    let table = ArcTable.tryFromFsWorksheet ws        
        //    Expect.isSome table "Table was not created"
        //    let out = ArcTable.toFsWorksheet table.Value
        //    Expect.workSheetEqual out ws "Worksheet was not correctly written"
           
        //)
    ]

let private mixedTable = 
    testList "mixedTable" [
        let wsName = "MyWorksheet"
        let ws = 
                initWorksheet wsName
                    [
                        Protocol.REF.appendLolColumn 4          
                        Protocol.Type.appendCollectionColumn 2
                        Parameter.appendMixedTemperatureColumn 2 2
                        Parameter.appendInstrumentColumn 2 
                        Characteristic.appendOrganismColumn 3
                        Factor.appendTimeColumn 0
                    ]
        testCase "Read" (fun () -> 
                    
            let table = ArcTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.Name wsName "Name did not match"
            Expect.equal table.ColumnCount 6 "Wrong number of columns"
            Expect.equal table.RowCount 4 "Wrong number of rows"

            let expectedHeaders = 
                [
                        Protocol.REF.lolHeader
                        Protocol.Type.collectionHeader
                        Parameter.temperatureHeader
                        Parameter.instrumentHeader
                        Characteristic.organismHeader
                        Factor.timeHeader
                ]
            Expect.mySequenceEqual table.Headers expectedHeaders "Headers did not match"
            
        )
        testCase "Write" (fun () -> 
            
            let table = ArcTable.tryFromFsWorksheet ws        
            Expect.isSome table "Table was not created"
            let out = ArcTable.toFsWorksheet table.Value
            Expect.workSheetEqual out ws "Worksheet was not correctly written"
           
        )
    ]

let private ioTable = 
    testList "ioTable" [
        let wsName = "MyWorksheet"
        let ws = 
            initWorksheet wsName
                [
                    Input.appendSampleColumn                1
                    Protocol.REF.appendLolColumn            1          
                    Parameter.appendTemperatureColumn       1
                    Characteristic.appendOrganismColumn     1
                    Factor.appendTimeColumn                 1
                    Output.appendRawDataColumn              1
                ]
        testCase "Read" (fun () -> 
                    
            let table = ArcTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.Name wsName "Name did not match"
            Expect.equal table.ColumnCount 6 "Wrong number of columns"
            Expect.equal table.RowCount 1 "Wrong number of rows"

            let expectedHeaders = 
                [
                    Input.sampleHeader 
                    Protocol.REF.lolHeader        
                    Parameter.temperatureHeader 
                    Characteristic.organismHeader 
                    Factor.timeHeader 
                    Output.rawDataHeader 
                ]
            Expect.mySequenceEqual table.Headers expectedHeaders "Headers did not match"
            let expectedCells = 
                [
                    Input.sampleValue 
                    Protocol.REF.lolValue        
                    Parameter.temperatureValue 
                    Characteristic.organismValue 
                    Factor.timeValue 
                    Output.rawDataValue 
                ]
            Expect.mySequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
        )
        testCase "Write" (fun () -> 
            
            let table = ArcTable.tryFromFsWorksheet ws        
            Expect.isSome table "Table was not created"
            let out = ArcTable.toFsWorksheet table.Value
            Expect.workSheetEqual out ws "Worksheet was not correctly written"
           
        )
    ]



let private deprecatedColumnTable = 
    testList "deprecatedIOColumnTable" [
        let wsName = "MyWorksheet"
        let ws = 
                initWorksheet wsName
                    [
                        Input.appendDeprecatedSourceColumn      1
                        Protocol.REF.appendLolColumn            1
                        Protocol.Type.appendCollectionColumn    1
                        Parameter.appendTemperatureColumn       1
                        Output.appendDeprecatedSampleColumn     1
                    ]
        testCase "Read" (fun () -> 
                    
            let table = ArcTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.Name wsName "Name did not match"
            Expect.equal table.ColumnCount 5 "Wrong number of columns"
            Expect.equal table.RowCount 1 "Wrong number of rows"

            let expectedHeaders = 
                [
                        Input.deprecatedSourceHeader
                        Protocol.REF.lolHeader
                        Protocol.Type.collectionHeader
                        Parameter.temperatureHeader
                        Output.deprecatedSampleHeader
                ]
            Expect.mySequenceEqual table.Headers expectedHeaders "Headers did not match"

            let expectedCells = 
                [
                        Input.sampleValue
                        Protocol.REF.lolValue
                        Protocol.Type.collectionValue
                        Parameter.temperatureValue
                        Output.rawDataValue
                ]
            Expect.mySequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
        )
    ]

let private writeOrder = 
    testList "writeOrder" [
        
        testCase "MixedHeaders" (fun () -> 
                  
            let wsName = "MyWorksheet"
            let mixedWs = 
                initWorksheet wsName
                    [
                        Parameter.appendTemperatureColumn       1
                        Characteristic.appendOrganismColumn     1
                        Output.appendRawDataColumn              1
                        Protocol.REF.appendLolColumn            1          
                        Input.appendSampleColumn                1
                        Factor.appendTimeColumn                 1
                    ]

            let mixedTable = ArcTable.tryFromFsWorksheet mixedWs |> Option.get
            let mixedOut = ArcTable.toFsWorksheet mixedTable

            let orderedWs =  
                initWorksheet wsName
                    [
                        Input.appendSampleColumn                1
                        Protocol.REF.appendLolColumn            1          
                        Parameter.appendTemperatureColumn       1
                        Characteristic.appendOrganismColumn     1
                        Factor.appendTimeColumn                 1
                        Output.appendRawDataColumn              1
                    ]
                
            Expect.workSheetEqual mixedOut orderedWs "Columns were not ordered correctly"

        )
    ]

let main = 
    testList "ArcTableTests" [
        ensureCorrectTestHeaders
        groupCols
        simpleTable
        mixedTable
        ioTable
        emptyTable
        deprecatedColumnTable
        writeOrder
    ]