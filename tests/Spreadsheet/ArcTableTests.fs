module ArcTableTests

open ARCtrl
open ARCtrl.Spreadsheet

open TestingUtils
open TestObjects.Spreadsheet.ArcTable

let private dataColumnsTable =
    let mkInputStr (i:int) = sprintf "Input_%i" i
    let mkDataNameStr (i:int) = sprintf "MyData#row=%i" i
    testList "dataColumnsTable" [
        testCase "Only Freetext" <| fun _ ->
            let table = ArcTable.init("MyTable")
            table.AddColumn(CompositeHeader.Input(IOType.Data), [|for i in 1 .. 5 do mkInputStr i |> CompositeCell.FreeText|])
            let fsws = ArcTable.toFsWorksheet None table
            let actualColValues = (fsws.Column(1).Cells |> Seq.map (fun c -> c.ValueAsString())) 
            Expect.sequenceEqual actualColValues ["Input [Data]"; "Input_1"; "Input_2"; "Input_3"; "Input_4"; "Input_5"] ""
        testCase "Only Data" <| fun _ ->
            let table = ArcTable.init("MyTable")
            table.AddColumn(CompositeHeader.Input(IOType.Data), [|for i in 1 .. 5 do CompositeCell.createData (Data(name = mkDataNameStr i, format = "text/csv", selectorFormat = "MySelector"))|])
            let fsws = ArcTable.toFsWorksheet None table
            fsws.RescanRows()
            let rows = fsws.Rows |> Seq.map (fun x -> x.Cells |> Seq.map (fun c -> c.ValueAsString()) |> Array.ofSeq) |> Array.ofSeq
            Expect.equal rows.[0].Length 3 "col count"
            Expect.sequenceEqual rows.[0] ["Input [Data]"; "Data Format"; "Data Selector Format"] "header row"
            for i in 1 .. 5 do
                Expect.sequenceEqual rows.[i] [mkDataNameStr i; "text/csv"; "MySelector"] (sprintf "row %i" i)
        testCase "Mixed" <| fun _ ->
            let table = ArcTable.init("MyTable")
            table.AddColumn(
                CompositeHeader.Input(IOType.Data),
                [|
                    for i in 1 .. 5 do
                        CompositeCell.createData (Data(name = mkDataNameStr i, format = "text/csv", selectorFormat = "MySelector"))
                    for i in 6 .. 10 do
                        mkInputStr i |> CompositeCell.FreeText
                |]
            )
            let fsws = ArcTable.toFsWorksheet None table
            fsws.RescanRows()
            let rows = fsws.Rows |> Seq.map (fun x -> x.Cells |> Seq.map (fun c -> c.ValueAsString()) |> Array.ofSeq) |> Array.ofSeq
            Expect.equal rows.[0].Length 3 "col count"
            Expect.sequenceEqual rows.[0] ["Input [Data]"; "Data Format"; "Data Selector Format"] "header row"
            for i in 1 .. 5 do
                Expect.sequenceEqual rows.[i] [mkDataNameStr i; "text/csv"; "MySelector"] (sprintf "row %i" i)
            for i in 6 .. 10 do
                Expect.sequenceEqual rows.[i] [mkInputStr i; ""; ""] (sprintf "row %i" i)
    ]

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
            Expect.sequenceEqual (ws.Row(1).Cells |> Seq.map (fun c -> c.ValueAsString())) expectedHeaders "Headers did not match"

            let expectedValues = 
                [
                    Parameter.instrumentValueV1
                    Parameter.instrumentValueV2
                    Parameter.instrumentValueV3
                    Parameter.instrumentValueV1
                    Parameter.instrumentValueV2
                    Parameter.instrumentValueV3
                ]

            Expect.sequenceEqual (ws.Row(2).Cells |> Seq.map (fun c -> c.ValueAsString())) expectedValues "Values did not match"
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
                |> Array.map (fun c -> c |> Seq.toArray |> Array.map (fun c -> c.ValueAsString()))
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
                |> Array.map (fun cols -> cols |> Array.map (Array.head) |> Array.reduce (fun a b -> a + ";" + b))
            Expect.sequenceEqual actualHeaderGroups expectedHeaderGroups "Header groups did not match"
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
            Expect.sequenceEqual table.Headers expectedHeaders "Headers did not match"

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
            Expect.sequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
        )
        testCase "Write" (fun () -> 
            
            let table = ArcTable.tryFromFsWorksheet ws        
            Expect.isSome table "Table was not created"
            let out = ArcTable.toFsWorksheet None table.Value
            Expect.workSheetEqual out ws "Worksheet was not correctly written"
           
        )
    ]

let private valuelessTable = 
    testList "valuelessTable" [
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
            Expect.sequenceEqual table.Headers expectedHeaders "Headers did not match"
        )
        testCase "Write TableElementHasTwoRows" (fun () -> 
            let table = ArcTable.init("MyTable")
            let header = CompositeHeader.Parameter (OntologyAnnotation("MyParameter"))
            table.AddColumn(header)
            let out = ArcTable.toFsWorksheet None table
            let fsTable = out.Tables.[0]
            let rowRangeLength = fsTable.RangeAddress.LastAddress.RowNumber - fsTable.RangeAddress.FirstAddress.RowNumber + 1
            Expect.equal rowRangeLength 2 "Row range length should be 2"
            // test against fail at read-in
            let inAgainOption = ArcTable.tryFromFsWorksheet out
            let inAgain = Expect.wantSome inAgainOption "Table was not created"
            Expect.equal inAgain.ColumnCount 1 "Column count should be 1"
            Expect.equal inAgain.Columns.[0].Header header "Header was not parsed correctly"
           
        )
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
            Expect.sequenceEqual table.Headers expectedHeaders "Headers did not match"
            
        )
        testCase "Write" (fun () -> 
            
            let table = ArcTable.tryFromFsWorksheet ws        
            Expect.isSome table "Table was not created"
            let out = ArcTable.toFsWorksheet None table.Value
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
                    Output.appendSimpleDataColumn           1
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
                    Output.simpleDataHeader 
                ]
            Expect.sequenceEqual table.Headers expectedHeaders "Headers did not match"
            let expectedCells = 
                [
                    Input.sampleValue 
                    Protocol.REF.lolValue        
                    Parameter.temperatureValue 
                    Characteristic.organismValue 
                    Factor.timeValue 
                    Output.simpleDataValue 
                ]
            Expect.sequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
        )
        testCase "Write" (fun () -> 
            
            let table = ArcTable.tryFromFsWorksheet ws        
            Expect.isSome table "Table was not created"
            let out = ArcTable.toFsWorksheet None table.Value
            Expect.workSheetEqual out ws "Worksheet was not correctly written"
           
        )
    ]

let private ioTable_obsoleteData = 
    testList "ioTable_obsoleteData" [
        let wsName = "MyWorksheet"
        let ws_obsoleteData = 
            initWorksheet wsName
                [
                    Input.appendSampleColumn                1
                    Protocol.REF.appendLolColumn            1          
                    Parameter.appendTemperatureColumn       1
                    Characteristic.appendOrganismColumn     1
                    Factor.appendTimeColumn                 1
                    Output.appendObsoleteDataColumn         1
                ]
        let ws = 
            initWorksheet wsName
                [
                    Input.appendSampleColumn                1
                    Protocol.REF.appendLolColumn            1          
                    Parameter.appendTemperatureColumn       1
                    Characteristic.appendOrganismColumn     1
                    Factor.appendTimeColumn                 1
                    Output.appendSimpleDataColumn           1
                ]
        testCase "Read" (fun () -> 
                    
            let table = ArcTable.tryFromFsWorksheet ws_obsoleteData        
        
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
                    Output.simpleDataHeader 
                ]
            Expect.sequenceEqual table.Headers expectedHeaders "Headers did not match"
            let expectedCells = 
                [
                    Input.sampleValue 
                    Protocol.REF.lolValue        
                    Parameter.temperatureValue 
                    Characteristic.organismValue 
                    Factor.timeValue 
                    Output.simpleDataValue 
                ]
            Expect.sequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
        )
        testCase "Write" (fun () -> 
            
            let table = ArcTable.tryFromFsWorksheet ws_obsoleteData        
            Expect.isSome table "Table was not created"
            let out = ArcTable.toFsWorksheet None table.Value
            Expect.workSheetEqual out ws "Worksheet was not correctly written"
           
        )
    ]

let private fullDataTable = 
    testList "fullDataTable" [
        let wsName = "MyWorksheet"
        let ws = 
            initWorksheet wsName
                [
                    Input.appenddataColumn                1
                    Protocol.REF.appendLolColumn          1          
                    Parameter.appendTemperatureColumn     1
                    Output.appendFullDataColumn           1
                ]
        testCase "Read" (fun () -> 
                    
            let table = ArcTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.Name wsName "Name did not match"
            Expect.equal table.ColumnCount 4 "Wrong number of columns"
            Expect.equal table.RowCount 1 "Wrong number of rows"

            let expectedHeaders = 
                [
                    Input.dataHeader 
                    Protocol.REF.lolHeader        
                    Parameter.temperatureHeader 
                    Output.fullDataHeader 
                ]
            Expect.sequenceEqual table.Headers expectedHeaders "Headers did not match"
            let expectedCells = 
                [
                    Input.dataValue 
                    Protocol.REF.lolValue        
                    Parameter.temperatureValue 
                    Output.fullDataValue 
                ]
            Expect.sequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
        )
        testCase "Write" (fun () -> 
            
            let table = ArcTable.tryFromFsWorksheet ws        
            Expect.isSome table "Table was not created"
            let out = ArcTable.toFsWorksheet None table.Value
            Expect.workSheetEqual out ws "Worksheet was not correctly written"
           
        )
    ]

let private commentTable = 
    testList "commentTable" [
        let wsName = "MyWorksheet"
        let ws = 
                initWorksheet wsName
                    [
                        Factor.appendTimeColumn           1
                        Comment.appendSimpleCommentColumn 1
                        Comment.appendNiceCommentColumn   1
                        
                    ]
        testCase "Read" (fun () -> 
                    
            let table = ArcTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.Name wsName "Name did not match"
            Expect.equal table.ColumnCount 3 "Wrong number of columns"
            Expect.equal table.RowCount 1 "Wrong number of rows"

            let expectedHeaders = 
                [                      
                        Factor.timeHeader
                        Comment.simpleCommentHeader
                        Comment.niceCommentHeader
                ]
            Expect.sequenceEqual table.Headers expectedHeaders "Headers did not match"

            let expectedCells = 
                [                        
                        Factor.timeValue
                        Comment.simpleCommentValue
                        Comment.niceCommentValue
                ]
            Expect.sequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
        )
        testCase "Write" (fun () -> 
            
            let table = ArcTable.tryFromFsWorksheet ws        
            Expect.isSome table "Table was not created"
            let out = ArcTable.toFsWorksheet None table.Value
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
            Expect.sequenceEqual table.Headers expectedHeaders "Headers did not match"

            let expectedCells = 
                [
                        Input.sourceValue
                        Protocol.REF.lolValue
                        Protocol.Type.collectionValue
                        Parameter.temperatureValue
                        Output.sampleValue
                ]
            Expect.sequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
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
                        Output.appendSimpleDataColumn           1
                        Protocol.REF.appendLolColumn            1          
                        Input.appendSampleColumn                1
                        Factor.appendTimeColumn                 1
                    ]

            let mixedTable = ArcTable.tryFromFsWorksheet mixedWs |> Option.get
            let mixedOut = ArcTable.toFsWorksheet None mixedTable

            let orderedWs =  
                initWorksheet wsName
                    [
                        Input.appendSampleColumn                1
                        Protocol.REF.appendLolColumn            1          
                        Parameter.appendTemperatureColumn       1
                        Characteristic.appendOrganismColumn     1
                        Factor.appendTimeColumn                 1
                        Output.appendSimpleDataColumn           1
                    ]
                
            Expect.workSheetEqual mixedOut orderedWs "Columns were not ordered correctly"

        )
    ]

let private emptyTable = 
    testList "emptyTable" [
        let name = "EmptyTable"
        let t = ArcTable.init(name)
        testCase "Write" (fun () -> 
            let sheet = ArcTable.toFsWorksheet None t
            Expect.equal name sheet.Name "Worksheet name did not match"
            Expect.equal 0 sheet.Rows.Count "Row count should be 0"
        )
        testCase "Read" (fun () ->
            let sheet = ArcTable.toFsWorksheet None t
            Expect.isNone (ArcTable.tryFromFsWorksheet sheet) "Table was not created"
        )
    ]

let main = 
    testList "ArcTableTests" [
        dataColumnsTable
        ensureCorrectTestHeaders
        groupCols
        simpleTable
        mixedTable
        ioTable
        ioTable_obsoleteData
        valuelessTable
        fullDataTable
        commentTable
        deprecatedColumnTable
        writeOrder
        emptyTable
    ]