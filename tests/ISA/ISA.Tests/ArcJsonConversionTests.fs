module ArcJsonConversion.Tests

open ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif
open TestingUtils

module Helper =
    let tableName1 = "Test1"
    let tableName2 = "Test2"
    let oa_species = OntologyAnnotation.fromString("species", "GO", "GO:0123456")
    let oa_chlamy = OntologyAnnotation.fromString("Chlamy", "NCBI", "NCBI:0123456")
    let oa_instrumentModel = OntologyAnnotation.fromString("instrument model", "MS", "MS:0123456")
    let oa_SCIEXInstrumentModel = OntologyAnnotation.fromString("SCIEX instrument model", "MS", "MS:654321")
    let oa_temperature = OntologyAnnotation.fromString("temperature","NCIT","NCIT:0123210")

    /// This function can be used to put ArcTable.Values into a nice format for printing/writing to IO
    let tableValues_printable (table:ArcTable) = 
        [
            for KeyValue((c,r),v) in table.Values do
                yield $"({c},{r}) {v}"
        ]

    let createCells_FreeText pretext (count) = Array.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
    let createCells_Sciex (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_SCIEXInstrumentModel)
    let createCells_chlamy (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_chlamy)
    let createCells_Unitized (count) = Array.init count (fun i -> CompositeCell.createUnitized (string i,OntologyAnnotation.empty))


    let singleRowSingleParam = 
    /// Input [Source] --> Source_0 .. Source_4
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 1)
            CompositeColumn.create(CompositeHeader.Parameter oa_species, createCells_chlamy 1)
            CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 1)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    /// Creates 5 empty tables
    ///
    /// Table Names: ["New Table 0"; "New Table 1" .. "New Table 4"]
    let create_exampleTables(appendStr:string) = Array.init 5 (fun i -> ArcTable.init($"{appendStr} Table {i}"))

    /// Valid TestAssay with empty tables: 
    ///
    /// Table Names: ["New Table 0"; "New Table 1" .. "New Table 4"]
    let create_exampleAssay() =
        let assay = ArcAssay("MyAssay")
        let sheets = create_exampleTables("My")
        sheets |> Array.iter (fun table -> assay.AddTable(table))
        assay

open Helper

let private tests_arcTableProcess = 
    testList "ARCTableProcess" [
        testCase "SingleRowSingleParam GetProcesses" (fun () ->
            let t = singleRowSingleParam
            let processes = t.GetProcesses()
            let expectedParam = ProtocolParameter.create(ParameterName = oa_species)
            let expectedValue = Value.Ontology oa_chlamy
            let expectedPPV = ProcessParameterValue.create(Category = expectedParam, Value = expectedValue)
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            Expect.isSome p.ParameterValues "Process should have parameter values"
            Expect.equal p.ParameterValues.Value.Length 1 "Process should have 1 parameter values"
            Expect.equal p.ParameterValues.Value.[0] expectedPPV "Param value does not match"
            Expect.isSome p.Inputs "Process should have inputs"
            Expect.equal p.Inputs.Value.Length 1 "Process should have 1 input"
            Expect.isSome p.Outputs "Process should have outputs"
            Expect.equal p.Outputs.Value.Length 1 "Process should have 1 output"
            Expect.isSome p.Name "Process should have name"
            Expect.equal p.Name.Value tableName1 "Process name should match table name"
        )
        testCase "SingleRowSingleParam GetAndFromProcesses" (fun () ->
            let t = singleRowSingleParam
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses tableName1 processes
            let expectedTable = 
                ArcTable.addColumn(
                    CompositeHeader.ProtocolREF,
                    [|CompositeCell.createFreeText tableName1|],
                    1                             
                ) t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )
    ]

let private tests_protocolTransformation =
    
    let pName = "ProtocolName"
    let pType = OntologyAnnotation.fromString("Growth","ABC","ABC:123")
    let pVersion = "1.0"
    let pDescription = "Great Protocol"

    let pParam1OA = OntologyAnnotation.fromString("Temperature","NCIT","NCIT:123")
    let pParam1 = ProtocolParameter.create(ParameterName = pParam1OA)

    testList "Protocol Transformation" [
        testCase "FromProtocol Empty" (fun () ->
            let p = Protocol.create()
            let t = p |> ArcTable.fromProtocol

            Expect.equal t.ColumnCount 0 "ColumnCount should be 0"
        )
        testCase "FromProtocol SingleParameter" (fun () ->
            let p = Protocol.create(Parameters = [pParam1])
            let t = p |> ArcTable.fromProtocol

            Expect.equal t.ColumnCount 1 "ColumnCount should be 1"
            let c = t.TryGetCellAt(0,0)
            Expect.isNone c "Cell should not exist"
        )
        testCase "FromProtocol SingleProtocolType" (fun () ->
            let p = Protocol.create(ProtocolType = pType)
            let t = p |> ArcTable.fromProtocol
            let expected = CompositeCell.Term pType

            Expect.equal t.ColumnCount 1 "ColumnCount should be 1"
            let c = t.TryGetCellAt(0,0)
            Expect.isSome c "Cell should exist"
            let c = c.Value
            Expect.equal c expected "Cell value does not match"
        )
        testCase "FromProtocol SingleName" (fun () ->
            let p = Protocol.create(Name = pName)
            let t = p |> ArcTable.fromProtocol
            let expected = CompositeCell.FreeText pName

            Expect.equal t.ColumnCount 1 "ColumnCount should be 1"
            let c = t.TryGetCellAt(0,0) 
            Expect.isSome c "Cell should exist"
            let c = c.Value
            Expect.equal c expected "Cell value does not match"
            )
        testCase "FromProtocol SingleVersion" (fun () ->
            let p = Protocol.create(Version = pVersion)
            let t = p |> ArcTable.fromProtocol
            let expected = CompositeCell.FreeText pVersion

            Expect.equal t.ColumnCount 1 "ColumnCount should be 1"
            let c = t.TryGetCellAt(0,0)
            Expect.isSome c "Cell should exist"
            let c = c.Value
            Expect.equal c expected "Cell value does not match"
        )
        testCase "FromProtocol SingleDescription" (fun () ->
            let p = Protocol.create(Description = pDescription)
            let t = p |> ArcTable.fromProtocol
            let expected = CompositeCell.FreeText pDescription

            Expect.equal t.ColumnCount 1 "ColumnCount should be 1"
            let c = t.TryGetCellAt(0,0)
            Expect.isSome c "Cell should exist"
            let c = c.Value
            Expect.equal c expected "Cell value does not match"
        )

        testCase "GetProtocols SingleName" (fun () ->           
            let t = ArcTable.init "TestTable"
            let name = "Name"
            t.AddProtocolNameColumn([|name|])
            let expected = [Protocol.create(Name = name)]

            TestingUtils.mySequenceEqual (t.GetProtocols()) expected "Protocols do not match"
        )
        testCase "GetProtocols SameNames" (fun () ->           
            let t = ArcTable.init "TestTable"
            let name = "Name"
            t.AddProtocolNameColumn([|name;name|])
            let expected = [Protocol.create(Name = name)]

            TestingUtils.mySequenceEqual (t.GetProtocols()) expected "Protocols do not match"
        )
        testCase "GetProtocols DifferentNames" (fun () ->           
            let t = ArcTable.init "TestTable"
            let name1 = "Name"
            let name2 = "Name2"
            t.AddProtocolNameColumn([|name1;name2|])
            let expected = [Protocol.create(Name = name1);Protocol.create(Name = name2)]

            TestingUtils.mySequenceEqual (t.GetProtocols()) expected "Protocols do not match"
        )
        //testCase "GetProtocols Parameters" (fun () ->
        //    let t = ArcTable.init "TestTable"
        //    let name1 = "Name"
        //    let name2 = "Name2"
        //    t.AddProtocolNameColumn([|name1;name2|])
        //    t.addpara([|pParam1;pParam1|])
        //    let expected = [Protocol.create(Name = name1;Parameters = [pParam1]);Protocol.create(Name = name2;Parameters = [pParam1])]
        //    TestingUtils.mySequenceEqual (t.GetProtocols()) expected "Protocols do not match"
        
        //)


    ]


let main = 
    testList "ArcJsonConversion" [
        tests_protocolTransformation
        tests_arcTableProcess
    ]