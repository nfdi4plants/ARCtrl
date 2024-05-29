module ArcTables.Tests

open ARCtrl

open TestingUtils
module TestObjects = 
    let TableName = "Test"
    let oa_species = OntologyAnnotation("species", "GO", "GO:0123456")
    let oa_chlamy = OntologyAnnotation("Chlamy", "NCBI", "NCBI:0123456")
    let oa_instrumentModel = OntologyAnnotation("instrument model", "MS", "MS:0123456")
    let oa_SCIEXInstrumentModel = OntologyAnnotation("SCIEX instrument model", "MS", "MS:654321")
    let oa_temperature = OntologyAnnotation("temperature","NCIT","NCIT:0123210")
  
    
    let protocolName1 = "Protocol 1"
    let protocolName2 = "Protocol 2"
    let descriptionValue1 = "Protocol is good"
    let descriptionValue2 = "Protocol is bad"
    let versionValue1 = "1.0.0"

    let sheetWithNoREF() =
        let t = ArcTable.init(protocolName1)
        let inputHeader = CompositeHeader.Input IOType.Sample
        let paramHeader = CompositeHeader.Parameter oa_species
        let paramValue = CompositeCell.createTerm oa_chlamy
        t.AddColumns
            [|
                CompositeColumn.create(inputHeader, Array.init 2 (fun i -> CompositeCell.createFreeText $"{i}")) 
                CompositeColumn.create(paramHeader, Array.create 2 paramValue)
            |]
        t

    let sheetWithREF() =
        let t = ArcTable.init("Simple")
        let inputHeader = CompositeHeader.Input IOType.Sample
        let paramHeader = CompositeHeader.Parameter oa_species
        let paramValue = CompositeCell.createTerm oa_chlamy
        t.AddColumns
            [|
                CompositeColumn.create(inputHeader, Array.init 2 (fun i -> CompositeCell.createFreeText $"{i}")) 
                CompositeColumn.create(CompositeHeader.ProtocolREF, Array.create 2 (CompositeCell.createFreeText protocolName1))        
                CompositeColumn.create(paramHeader, Array.create 2 paramValue)
            |]
        t

    let sheetWithREFAndFactor() =
        let t = ArcTable.init("SimpleWithFactor")
        let factorHeader = CompositeHeader.Factor oa_instrumentModel
        let factorValue = CompositeCell.createTerm oa_SCIEXInstrumentModel
        t.AddColumns
            [|
                CompositeColumn.create(CompositeHeader.ProtocolREF, Array.create 3 (CompositeCell.createFreeText protocolName1))        
                CompositeColumn.create(factorHeader, Array.create 3 factorValue)
            |]
        t

    let sheetWithTwoProtocolsOneRef() =
        let t = ArcTable.init("Simple")
        let inputHeader = CompositeHeader.Input IOType.Sample
        let paramHeader = CompositeHeader.Parameter oa_species
        let paramValue = CompositeCell.createTerm oa_chlamy
        t.AddColumns
            [|
                CompositeColumn.create(inputHeader, Array.init 4 (fun i -> CompositeCell.createFreeText $"{i}")) 
                CompositeColumn.create(CompositeHeader.ProtocolREF, Array.create 2 (CompositeCell.createFreeText protocolName1))        
                CompositeColumn.create(paramHeader, Array.create 4 paramValue)
            |]
        t

    let sheetWithTwoProtocolsTwoRefs() =
        let t = ArcTable.init("Simple")
        let inputHeader = CompositeHeader.Input IOType.Sample
        let paramHeader = CompositeHeader.Parameter oa_species
        let paramValue = CompositeCell.createTerm oa_chlamy
        let protocolREFColumn = 
            Array.create 2 (CompositeCell.createFreeText protocolName2)
            |> Array.append (Array.create 2 (CompositeCell.createFreeText protocolName1))
        t.AddColumns
            [|
                CompositeColumn.create(inputHeader, Array.init 4 (fun i -> CompositeCell.createFreeText $"{i}")) 
                CompositeColumn.create(CompositeHeader.ProtocolREF, protocolREFColumn)        
                CompositeColumn.create(paramHeader, Array.create 4 paramValue)
            |]
        t


    let descriptionRefTable() =
        let t = ArcTable.init("SimpleRef")
        
        t.AddColumns
            [|
                CompositeColumn.create(CompositeHeader.ProtocolDescription, Array.create 1 (CompositeCell.createFreeText descriptionValue1))       
                CompositeColumn.create(CompositeHeader.ProtocolREF, Array.create 1 (CompositeCell.createFreeText protocolName1))        
            |]
        t

    let descriptionRefTable2() =
        let t = ArcTable.init("Whatever")
        
        t.AddColumns
            [|
                CompositeColumn.create(CompositeHeader.ProtocolDescription, Array.create 1 (CompositeCell.createFreeText descriptionValue2))       
                CompositeColumn.create(CompositeHeader.ProtocolREF, Array.create 1 (CompositeCell.createFreeText protocolName2))        
            |]
        t

    let versionRefTable() =
        let t = ArcTable.init("SecondRef")
        
        t.AddColumns
            [|
                CompositeColumn.create(CompositeHeader.ProtocolVersion, Array.create 1 (CompositeCell.createFreeText versionValue1))       
                CompositeColumn.create(CompositeHeader.ProtocolREF, Array.create 1 (CompositeCell.createFreeText protocolName2))        
            |]
        t

open TestObjects

let tests_Item = testList "Item" [
    // update test as soon as https://github.com/fable-compiler/Fable/issues/3571 is fixed
    testCase "simple" <| fun _ ->
        let table1 = ArcTable.init("Table 1")
        let table2 = ArcTable.init("Table 2")
        let tableSeq = [table1; table2]
        let arctables = ArcTables(ResizeArray tableSeq)
        Expect.equal arctables.[0] table1 "table1"
        Expect.equal arctables.[1] table2 "table2"
]

let tests_IEnumberable = testList "IEnumerable" [
    let createTestTables() = 
        let table1 = ArcTable.init("Table 1")
        let table2 = ArcTable.init("Table 2")
        ArcTables(ResizeArray [table1; table2])
    testCase "Seq.length" <| fun _ ->
        let tables = createTestTables()
        let actual = Seq.length tables
        Expect.equal actual 2 ""
    testCase "Seq.map" <| fun _ ->
        let tables = createTestTables()
        let actual = tables |> Seq.map (fun t -> t.Name)
        let expected = ["Table 1"; "Table 2"]
        Expect.sequenceEqual actual expected ""
]

let tests_member = testList "member" [
    testCase "TableCount init" <| fun _ ->
        let tableSeq = [ArcTable.init("Table 1"); ArcTable.init("Table 2")]
        let arctables = ArcTables(ResizeArray tableSeq)
        Expect.equal arctables.TableCount 2 "TableCount"
    testCase "TableCount addedTable" <| fun _ ->
        let tableSeq = [ArcTable.init("Table 1"); ArcTable.init("Table 2")]
        let arctables = ArcTables(ResizeArray tableSeq)
        let _ = arctables.InitTable("New Table!")
        Expect.equal arctables.TableCount 3 "TableCount should increase to 3"
    testCase "TableNames init" <| fun _ ->
        let tableSeq = [ArcTable.init("Table 1"); ArcTable.init("Table 2")]
        let arctables = ArcTables(ResizeArray tableSeq)
        Expect.sequenceEqual arctables.TableNames ["Table 1"; "Table 2"] "TableNames"
    testCase "TableNames addedTable" <| fun _ ->
        let tableSeq = [ArcTable.init("Table 1"); ArcTable.init("Table 2")]
        let arctables = ArcTables(ResizeArray tableSeq)
        let _ = arctables.InitTable("New Table!")
        Expect.sequenceEqual arctables.TableNames ["Table 1"; "Table 2"; "New Table!"] "TableNames"
    testList "MoveTable" [
        testCase "Move to end" <| fun _ ->
            let actual = ResizeArray [for i in 0 .. 5 do ArcTable.init(sprintf "Table %i" i)] |> ArcTables
            actual.MoveTable(1,5)
            let expected = List.map ArcTable.init ["Table 0"; "Table 2"; "Table 3"; "Table 4"; "Table 5"; "Table 1"] |> ResizeArray |> ArcTables
            Seq.iteri2 (fun i t1 t2 -> Expect.equal t1 t2 (sprintf "Test compare table at index %i" i)) actual expected
        testCase "Move to start" <| fun _ ->
            let actual = ResizeArray [for i in 0 .. 5 do ArcTable.init(sprintf "Table %i" i)] |> ArcTables
            actual.MoveTable(5,0)
            let expected = List.map ArcTable.init ["Table 5"; "Table 0"; "Table 1"; "Table 2"; "Table 3"; "Table 4";] |> ResizeArray |> ArcTables
            Seq.iteri2 (fun i t1 t2 -> Expect.equal t1 t2 (sprintf "Test compare table at index %i" i)) actual expected
    ]
]

let updateReferenceWithSheet = 

    testList "UpdateReferenceWithSheet" [
        
        testCase "NoReference" (fun () ->
            let tableOfInterest = sheetWithNoREF()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq []
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.TableCount tables.TableCount "Should be same number of tables"
            let resultTable = result.Tables.[0]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount tableOfInterest.ColumnCount "Should be same number of columns"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"
            Expect.isFalse (obj.ReferenceEquals(resultTable,tableOfInterest)) "Should not be same object"
        )
        testCase "NoMatchingReference" (fun () ->
            let tableOfInterest = sheetWithREF()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq [descriptionRefTable2()]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.TableCount tables.TableCount "Should be same number of tables"
            let resultTable = result.Tables.[0]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount tableOfInterest.ColumnCount "Should be same number of columns"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"
            Expect.isFalse (obj.ReferenceEquals(resultTable,tableOfInterest)) "Should not be same object"
        )
        testCase "NoMatchingReference_keepReference" (fun () ->
            let tableOfInterest = sheetWithREF()
            let refTable = descriptionRefTable2()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq [refTable]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables,true)
            
            Expect.equal result.TableCount (tables.TableCount + 1) "Should be same number of tables"
            let resultRefTable = result.Tables.[0]
            Expect.equal resultRefTable.Name refTable.Name "Should be same table name"
            Expect.equal resultRefTable.ColumnCount refTable.ColumnCount "Should be same number of columns"
            Expect.equal resultRefTable.RowCount refTable.RowCount "Should be same number of rows"

            let resultTable = result.Tables.[1]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount tableOfInterest.ColumnCount "Should be same number of columns"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"
            Expect.isFalse (obj.ReferenceEquals(resultTable,tableOfInterest)) "Should not be same object"
        )
        testCase "SimpleWithProtocolREF" (fun () ->
            let tableOfInterest = sheetWithREF()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq [descriptionRefTable()]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.TableCount tables.TableCount "Should be same number of tables"
            let resultTable = result.Tables.[0]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount (tableOfInterest.ColumnCount + 1) "Should be same number of columns"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"
            Expect.sequenceEqual
                (resultTable.GetColumnByHeader CompositeHeader.ProtocolDescription).Cells
                (Array.create 2 (CompositeCell.createFreeText descriptionValue1))
                "Description value was not taken correctly"
        )
        testCase "SimpleWithNoProtocolREF" (fun () ->
            let tableOfInterest = sheetWithNoREF()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq [descriptionRefTable()]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.TableCount tables.TableCount "Should be same number of tables"
            let resultTable = result.Tables.[0]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount (tableOfInterest.ColumnCount + 2) "Should be same number of columns"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"
            Expect.sequenceEqual
                (resultTable.GetColumnByHeader CompositeHeader.ProtocolDescription).Cells
                (Array.create 2 (CompositeCell.createFreeText descriptionValue1))
                "Description value was not taken correctly"
        )
        testCase "RefTableHasNoProtocolREF" (fun () ->
            let tableOfInterest = sheetWithREF()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq [sheetWithNoREF()]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.TableCount tables.TableCount "Should be same number of tables"
            let resultTable = result.Tables.[0]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount (tableOfInterest.ColumnCount) "Should have same number of columns as before"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of columns as before"
            
        )
        testCase "TwoTablesWithSameProtocol" (fun () ->
            let tableOfInterest1 = sheetWithREF()
            let tableOfInterest2 = sheetWithREFAndFactor()
            let tables = ArcTables.ofSeq [tableOfInterest1;tableOfInterest2]
            let refTables = ArcTables.ofSeq [descriptionRefTable()]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.TableCount tables.TableCount "Should be same number of tables"

            let resultTable1 = result.Tables.[0]
            Expect.equal resultTable1.Name tableOfInterest1.Name "Should be same table name"
            Expect.equal resultTable1.ColumnCount (tableOfInterest1.ColumnCount + 1) "Should be same number of columns"
            Expect.equal tableOfInterest1.RowCount tableOfInterest1.RowCount "Should be same number of rows"
            Expect.sequenceEqual
                (resultTable1.GetColumnByHeader CompositeHeader.ProtocolDescription).Cells
                (Array.create 2 (CompositeCell.createFreeText descriptionValue1))
                "Description value was not taken correctly"
            Expect.sequenceEqual
                (resultTable1.GetColumnByHeader (CompositeHeader.Parameter oa_species)).Cells
                (Array.create 2 (CompositeCell.createTerm oa_chlamy))
                "Check for previous param correctness"
                
            let resultTable2 = result.Tables.[1]
            Expect.equal resultTable2.Name tableOfInterest2.Name "Should be same table name"
            Expect.equal resultTable2.ColumnCount (tableOfInterest2.ColumnCount + 1) "Should be same number of columns"
            Expect.equal tableOfInterest2.RowCount tableOfInterest2.RowCount "Should be same number of rows"
            Expect.sequenceEqual
                (resultTable2.GetProtocolDescriptionColumn()).Cells
                (Array.create 3 (CompositeCell.createFreeText descriptionValue1))
                "Description value was not taken correctly"
            Expect.sequenceEqual
                (resultTable2.GetColumnByHeader (CompositeHeader.Factor oa_instrumentModel)).Cells
                (Array.create 3 (CompositeCell.createTerm oa_SCIEXInstrumentModel))
                "Check for previous param correctness" 

        )
        testCase "TableWithTwoProtocolsOneRef" (fun () ->
            let tableOfInterest = sheetWithTwoProtocolsOneRef()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq [descriptionRefTable();descriptionRefTable2()]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.TableCount tables.TableCount "Should be same number of tables"
            let resultTable = result.Tables.[0]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount (tableOfInterest.ColumnCount + 1) "Should be same number of columns"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"

            let expectedDescription =              
                Array.create 2 (CompositeCell.emptyFreeText)
                |> Array.append (Array.create 2 (CompositeCell.createFreeText descriptionValue1))
            Expect.sequenceEqual
                (resultTable.GetColumnByHeader CompositeHeader.ProtocolDescription).Cells
                (expectedDescription)
                "Description value was not taken correctly"
            Expect.sequenceEqual
                (resultTable.GetColumnByHeader (CompositeHeader.Parameter oa_species)).Cells
                (Array.create 4 (CompositeCell.createTerm oa_chlamy))
                "Check for previous param correctness"

        )
        testCase "TableWithTwoProtocolsTwoRefs" (fun () ->
            let tableOfInterest = sheetWithTwoProtocolsTwoRefs()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq [descriptionRefTable();descriptionRefTable2()]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.TableCount tables.TableCount "Should be same number of tables"
            let resultTable = result.Tables.[0]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount (tableOfInterest.ColumnCount + 1) "Should be same number of columns"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"

            let expectedDescription =              
                Array.create 2 (CompositeCell.createFreeText descriptionValue2)
                |> Array.append (Array.create 2 (CompositeCell.createFreeText descriptionValue1))
            Expect.sequenceEqual
                (resultTable.GetColumnByHeader CompositeHeader.ProtocolDescription).Cells
                (expectedDescription)
                "Description value was not taken correctly"
            Expect.sequenceEqual
                (resultTable.GetColumnByHeader (CompositeHeader.Parameter oa_species)).Cells
                (Array.create 4 (CompositeCell.createTerm oa_chlamy))
                "Check for previous param correctness"
        )
    ]

let tests_constructor = 

    testList "Constructor" [
        
        testCase "DuplicateNames" (fun () ->
            let table1 = ArcTable.init("Table 1")
            let table2 = ArcTable.init("Table 1")
            let createTables = 
                fun () -> ArcTables(ResizeArray [table1;table2]) |> ignore
            Expect.throws createTables "Should throw an exception"  
        )
    ]

let main = 
    testList "ArcTablesTests" [
        tests_Item
        tests_IEnumberable
        tests_member
        updateReferenceWithSheet
        tests_constructor
    ]