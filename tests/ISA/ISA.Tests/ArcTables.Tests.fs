module ArcTables.Tests

open ARCtrl.ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open TestingUtils
module TestObjects = 
    let TableName = "Test"
    let oa_species = OntologyAnnotation.fromString("species", "GO", "GO:0123456")
    let oa_chlamy = OntologyAnnotation.fromString("Chlamy", "NCBI", "NCBI:0123456")
    let oa_instrumentModel = OntologyAnnotation.fromString("instrument model", "MS", "MS:0123456")
    let oa_SCIEXInstrumentModel = OntologyAnnotation.fromString("SCIEX instrument model", "MS", "MS:654321")
    let oa_temperature = OntologyAnnotation.fromString("temperature","NCIT","NCIT:0123210")
  
    
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

let updateReferenceWithSheet = 

    testList "UpdateReferenceWithSheet" [
        
        testCase "NoReference" (fun () ->
            let tableOfInterest = sheetWithNoREF()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq []
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.Count tables.Count "Should be same number of tables"
            let resultTable = result.[0]
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
            
            Expect.equal result.Count tables.Count "Should be same number of tables"
            let resultTable = result.[0]
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
            
            Expect.equal result.Count (tables.Count + 1) "Should be same number of tables"
            let resultRefTable = result.[0]
            Expect.equal resultRefTable.Name refTable.Name "Should be same table name"
            Expect.equal resultRefTable.ColumnCount refTable.ColumnCount "Should be same number of columns"
            Expect.equal resultRefTable.RowCount refTable.RowCount "Should be same number of rows"

            let resultTable = result.[1]
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
            
            Expect.equal result.Count tables.Count "Should be same number of tables"
            let resultTable = result.[0]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount (tableOfInterest.ColumnCount + 1) "Should be same number of columns"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"
            mySequenceEqual
                (resultTable.GetColumnByHeader CompositeHeader.ProtocolDescription).Cells
                (Array.create 2 (CompositeCell.createFreeText descriptionValue1))
                "Description value was not taken correctly"
        )
        testCase "SimpleWithNoProtocolREF" (fun () ->
            let tableOfInterest = sheetWithNoREF()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq [descriptionRefTable()]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.Count tables.Count "Should be same number of tables"
            let resultTable = result.[0]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount (tableOfInterest.ColumnCount + 2) "Should be same number of columns"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"
            mySequenceEqual
                (resultTable.GetColumnByHeader CompositeHeader.ProtocolDescription).Cells
                (Array.create 2 (CompositeCell.createFreeText descriptionValue1))
                "Description value was not taken correctly"
        )
        testCase "TwoTablesWithSameProtocol" (fun () ->
            let tableOfInterest1 = sheetWithREF()
            let tableOfInterest2 = sheetWithREFAndFactor()
            let tables = ArcTables.ofSeq [tableOfInterest1;tableOfInterest2]
            let refTables = ArcTables.ofSeq [descriptionRefTable()]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.Count tables.Count "Should be same number of tables"

            let resultTable1 = result.[0]
            Expect.equal resultTable1.Name tableOfInterest1.Name "Should be same table name"
            Expect.equal resultTable1.ColumnCount (tableOfInterest1.ColumnCount + 1) "Should be same number of columns"
            Expect.equal tableOfInterest1.RowCount tableOfInterest1.RowCount "Should be same number of rows"
            mySequenceEqual
                (resultTable1.GetColumnByHeader CompositeHeader.ProtocolDescription).Cells
                (Array.create 2 (CompositeCell.createFreeText descriptionValue1))
                "Description value was not taken correctly"
            mySequenceEqual
                (resultTable1.GetColumnByHeader (CompositeHeader.Parameter oa_species)).Cells
                (Array.create 2 (CompositeCell.createTerm oa_chlamy))
                "Check for previous param correctness"
                
            let resultTable2 = result.[1]
            Expect.equal resultTable2.Name tableOfInterest2.Name "Should be same table name"
            Expect.equal resultTable2.ColumnCount (tableOfInterest2.ColumnCount + 1) "Should be same number of columns"
            Expect.equal tableOfInterest2.RowCount tableOfInterest2.RowCount "Should be same number of rows"
            mySequenceEqual
                (resultTable2.GetProtocolDescriptionColumn()).Cells
                (Array.create 3 (CompositeCell.createFreeText descriptionValue1))
                "Description value was not taken correctly"
            mySequenceEqual
                (resultTable2.GetColumnByHeader (CompositeHeader.Factor oa_instrumentModel)).Cells
                (Array.create 3 (CompositeCell.createTerm oa_SCIEXInstrumentModel))
                "Check for previous param correctness" 

        )
        testCase "TableWithTwoProtocolsOneRef" (fun () ->
            let tableOfInterest = sheetWithTwoProtocolsOneRef()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq [descriptionRefTable();descriptionRefTable2()]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.Count tables.Count "Should be same number of tables"
            let resultTable = result.[0]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount (tableOfInterest.ColumnCount + 1) "Should be same number of columns"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"

            let expectedDescription =              
                Array.create 2 (CompositeCell.emptyFreeText)
                |> Array.append (Array.create 2 (CompositeCell.createFreeText descriptionValue1))
            mySequenceEqual
                (resultTable.GetColumnByHeader CompositeHeader.ProtocolDescription).Cells
                (expectedDescription)
                "Description value was not taken correctly"
            mySequenceEqual
                (resultTable.GetColumnByHeader (CompositeHeader.Parameter oa_species)).Cells
                (Array.create 4 (CompositeCell.createTerm oa_chlamy))
                "Check for previous param correctness"

        )
        testCase "TableWithTwoProtocolsTwoRefs" (fun () ->
            let tableOfInterest = sheetWithTwoProtocolsTwoRefs()
            let tables = ArcTables.ofSeq [tableOfInterest]
            let refTables = ArcTables.ofSeq [descriptionRefTable();descriptionRefTable2()]
            let result = ArcTables.updateReferenceTablesBySheets(refTables,tables)
            
            Expect.equal result.Count tables.Count "Should be same number of tables"
            let resultTable = result.[0]
            Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
            Expect.equal resultTable.ColumnCount (tableOfInterest.ColumnCount + 1) "Should be same number of columns"
            Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"

            let expectedDescription =              
                Array.create 2 (CompositeCell.createFreeText descriptionValue2)
                |> Array.append (Array.create 2 (CompositeCell.createFreeText descriptionValue1))
            mySequenceEqual
                (resultTable.GetColumnByHeader CompositeHeader.ProtocolDescription).Cells
                (expectedDescription)
                "Description value was not taken correctly"
            mySequenceEqual
                (resultTable.GetColumnByHeader (CompositeHeader.Parameter oa_species)).Cells
                (Array.create 4 (CompositeCell.createTerm oa_chlamy))
                "Check for previous param correctness"
        )
    ]

let main = 
    testList "ArcTablesTests" [
        updateReferenceWithSheet        
    ]