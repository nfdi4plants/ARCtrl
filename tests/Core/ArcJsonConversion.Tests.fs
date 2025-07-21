module ArcJsonConversion.Tests

open ARCtrl
open ARCtrl.Helper
open ARCtrl.Process
open ARCtrl.Process.Conversion
open TestingUtils

module Helper =
    let tableName1 = "Test1"
    let tableName2 = "Test2"
    let oa_species = OntologyAnnotation("species", "GO", "GO:0123456")
    let oa_chlamy = OntologyAnnotation("Chlamy", "NCBI", "NCBI:0123456")
    let oa_instrumentModel = OntologyAnnotation("instrument model", "MS", "MS:0123456")
    let oa_SCIEXInstrumentModel = OntologyAnnotation("SCIEX instrument model", "MS", "MS:654321")
    let oa_time = OntologyAnnotation("time", "UO", "UO:0000010")
    let oa_hour = OntologyAnnotation("hour", "UO", "UO:0000032")
    let oa_temperature = OntologyAnnotation("temperature","NCIT","NCIT:0123210")
    let oa_degreeCel =  OntologyAnnotation("degree celsius","UO","UO:0000027")

    /// This function can be used to put ArcTable.Values into a nice format for printing/writing to IO
    let tableValues_printable (table:ArcTable) = 
        [
            for KeyValue((c,r),v) in table.Values do
                yield $"({c},{r}) {v}"
        ]

    let createCells_FreeText pretext (count) = ResizeArray.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
    let createCells_Sciex (count) = ResizeArray.init count (fun _ -> CompositeCell.createTerm oa_SCIEXInstrumentModel)
    let createCells_chlamy (count) = ResizeArray.init count (fun _ -> CompositeCell.createTerm oa_chlamy)
    let createCells_DegreeCelsius (count) = ResizeArray.init count (fun i -> CompositeCell.createUnitized (string i,oa_degreeCel))
    let createCells_Hour (count) = ResizeArray.init count (fun i -> CompositeCell.createUnitized (string i,oa_hour))

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

    let singleRowOutputSource = 
    /// Input [Source] --> Source_0 .. Source_4
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 1)
            CompositeColumn.create(CompositeHeader.Output IOType.Source, createCells_FreeText "Sample" 1)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    let singleRowMixedValues = 
    /// Input [Source] --> Source_0 .. Source_4
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 1)
            CompositeColumn.create(CompositeHeader.Parameter oa_time, createCells_Hour 1)
            CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_chlamy 1)
            CompositeColumn.create(CompositeHeader.Factor oa_temperature, createCells_DegreeCelsius 1)
            CompositeColumn.create(CompositeHeader.Component oa_instrumentModel, createCells_Sciex 1)
            CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 1)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    let singleRowDataInputWithCharacteristic = 
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.Data, createCells_FreeText "RData" 1)
            CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_chlamy 1)
            CompositeColumn.create(CompositeHeader.Output IOType.Data, createCells_FreeText "DData" 1)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    let singleRowDataOutputWithFactor = 
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.Data, createCells_FreeText "RData" 1)
            CompositeColumn.create(CompositeHeader.Factor oa_temperature, createCells_DegreeCelsius 1)
            CompositeColumn.create(CompositeHeader.Output IOType.Data, createCells_FreeText "DData" 1)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    let twoRowsSameParamValue = 
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 2)
            CompositeColumn.create(CompositeHeader.Parameter oa_species, createCells_chlamy 2)
            CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 2)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    let twoRowsDifferentParamValue = 
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 2)
            CompositeColumn.create(CompositeHeader.Parameter oa_time, createCells_Hour 2)
            CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 2)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    let singleRowWithProtocolRef = 
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 1)
            CompositeColumn.create(CompositeHeader.ProtocolREF, ResizeArray [|CompositeCell.createFreeText "MyProtocol"|])
            CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 1)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    /// Creates 5 empty tables
    ///
    /// Table Names: ["New Table 0"; "New Table 1" .. "New Table 4"]
    let create_exampleTables(appendStr:string) = ResizeArray.init 5 (fun i -> ArcTable.init($"{appendStr} Table {i}"))

    /// Valid TestAssay with empty tables: 
    ///
    /// Table Names: ["New Table 0"; "New Table 1" .. "New Table 4"]
    let create_exampleAssay() =
        let assay = ArcAssay("MyAssay")
        let sheets = create_exampleTables("My")
        sheets |> ResizeArray.iter (fun table -> assay.AddTable(table))
        assay

open Helper


let private tests_ArcTableProcess = 
    testList "ARCTableProcess" [
        testCase "SingleRowSingleParam GetProcesses" (fun () ->
            let t = singleRowSingleParam.Copy()
            let processes = t.GetProcesses()
            let expectedParam = ProtocolParameter.create(ParameterName = oa_species)
            let expectedValue = Value.Ontology oa_chlamy
            let expectedPPV = ProcessParameterValue.create(Category = expectedParam, Value = expectedValue)
            let expectedInput = Source.create(Name = "Source_0") |> ProcessInput.Source 
            let expectedOutput = Sample.create(Name = "Sample_0") |> ProcessOutput.Sample
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            let paramValues = Expect.wantSome p.ParameterValues "Process should have parameter values"
            Expect.equal paramValues.Length 1 "Process should have 1 parameter values"
            Expect.equal paramValues.[0] expectedPPV "Param value does not match"
            let inputs = Expect.wantSome p.Inputs "Process should have inputs"
            Expect.equal inputs.Length 1 "Process should have 1 input"
            Expect.equal inputs.[0] expectedInput "Input value does not match"
            let outputs = Expect.wantSome p.Outputs "Process should have outputs"
            Expect.equal outputs.Length 1 "Process should have 1 output"
            Expect.equal outputs.[0] expectedOutput "Output value does not match"
            Expect.isSome p.Name "Process should have name"
            Expect.equal p.Name.Value tableName1 "Process name should match table name"
        )
        testCase "SingleRowSingleParam GetAndFromProcesses" (fun () ->
            let t = singleRowSingleParam.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses tableName1 processes
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )
        testCase "SingleRowOutputSource GetProcesses" (fun () ->
            let t = singleRowOutputSource.Copy()
            let processes = t.GetProcesses()           
            let expectedInput = Source.create(Name = "Source_0") |> ProcessInput.Source 
            let expectedOutput = Sample.create(Name = "Sample_0") |> ProcessOutput.Sample
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            let inputs = Expect.wantSome p.Inputs "Process should have inputs"
            Expect.equal inputs.Length 1 "Process should have 1 input"
            Expect.equal inputs.[0] expectedInput "Input value does not match"
            let outputs = Expect.wantSome p.Outputs "Process should have outputs"
            Expect.equal outputs.Length 1 "Process should have 1 output"
            Expect.equal outputs.[0] expectedOutput "Output value does not match"
            let name = Expect.wantSome p.Name "Process should have name"
            Expect.equal name tableName1 "Process name should match table name"
        )

        testCase "SingleRowMixedValues GetProcesses" (fun () ->          
            let t = singleRowMixedValues.Copy()          
            let processes = t.GetProcesses()           
            Expect.equal processes.Length 1 "Should have 1 process"
            Expect.equal processes.[0].ParameterValues.Value.Length 1 "Process should have 1 parameter values"
            Expect.equal processes.[0].Inputs.Value.Length 1 "Process should have 1 input"
            Expect.equal processes.[0].Outputs.Value.Length 1 "Process should have 1 output"
        )

        testCase "SingleRowMixedValues GetAndFromProcesses" (fun () ->
            let t = singleRowMixedValues.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses tableName1 processes
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )

        testCase "SingleRowDataInputWithCharacteristic GetProcesses" (fun () ->
            let t = singleRowDataInputWithCharacteristic.Copy()
            let processes = t.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            Expect.isNone p.ParameterValues "Process should have no parameter values"
            Expect.isSome p.Inputs "Process should have inputs"
            Expect.equal p.Inputs.Value.Length 2 "Process should have 2 inputs"
            Expect.isSome p.Outputs "Process should have outputs"
            Expect.equal p.Outputs.Value.Length 2 "Process should have 2 outputs"
            Expect.isTrue (ProcessInput.isData p.Inputs.Value.[0]) "First input should be data"
            Expect.isTrue (ProcessInput.isSample p.Inputs.Value.[1]) "Second input should be characteristic"
            let secondIn = p.Inputs.Value.[1] |> ProcessInput.trySample |> Option.get
            Expect.isSome secondIn.Characteristics "Second input should have characteristics"
            Expect.equal secondIn.Characteristics.Value.Length 1 "Second input should have 1 characteristic"
        )

        testCase "SingleRowDataInputWithCharacteristic GetAndFromProcesses" (fun () ->
            let t = singleRowDataInputWithCharacteristic.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses tableName1 processes
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )

        testCase "SingleRowDataOutputWithFactor GetProcesses" (fun () ->
            let t = singleRowDataOutputWithFactor.Copy()
            let processes = t.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            Expect.isNone p.ParameterValues "Process should have no parameter values"
            Expect.isSome p.Inputs "Process should have inputs"
            Expect.equal p.Inputs.Value.Length 2 "Process should have 2 inputs"
            Expect.isSome p.Outputs "Process should have outputs"
            Expect.equal p.Outputs.Value.Length 2 "Process should have 2 outputs"
            Expect.isTrue (ProcessOutput.isData p.Outputs.Value.[0]) "First output should be data"
            Expect.isTrue (ProcessOutput.isSample p.Outputs.Value.[1]) "Second output should be sample"
            let secondOut = p.Outputs.Value.[1] |> ProcessOutput.trySample |> Option.get
            Expect.isSome secondOut.FactorValues "Second output should have factor values"
            Expect.equal secondOut.FactorValues.Value.Length 1 "Second output should have 1 factor value"
        )

        testCase "SingleRowDataOutputWithFactor GetAndFromProcesses" (fun () ->
            let t = singleRowDataOutputWithFactor.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses tableName1 processes
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )

        testCase "TwoRowsSameParamValue GetAndFromProcesses" (fun () ->
            let t = twoRowsSameParamValue.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses tableName1 processes
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )

        testCase "TwoRowsDifferentParamValues GetProcesses" (fun () ->
            let t = twoRowsDifferentParamValue.Copy()
            let processes = t.GetProcesses()
            Expect.equal processes.Length 2 "Should have 2 processes"
        )

        testCase "TwoRowsDifferentParamValues GetAndFromProcesses" (fun () ->
            let t = twoRowsDifferentParamValue.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses tableName1 processes
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )

        testCase "SingleRowWithProtocolREF GetProcesses" (fun () ->
            let t = singleRowWithProtocolRef.Copy()
            let processes = t.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            Expect.isSome p.ExecutesProtocol "Process should have protocol"
            let prot = p.ExecutesProtocol.Value
            Expect.isSome prot.Name "Protocol should have name"
            Expect.equal prot.Name.Value "MyProtocol" "Protocol name should match"
            Expect.isSome p.Name "Process should have name"
            Expect.equal p.Name.Value tableName1 "Process name should match table name"
        )

        testCase "SingleRowWithProtocolREF GetAndFromProcesses" (fun () ->
            let t = singleRowWithProtocolRef.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses tableName1 processes
            Expect.arcTableEqual table t "Table should be equal"
        )

        testCase "EmptyTable GetProcesses" (fun () ->            
            let t = ArcTable.init tableName1
            let processes = t.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            Expect.wantSome p.Name "Process should have name"
            |> fun n -> Expect.equal n tableName1 "Process name should match table name"
            Expect.isNone p.Inputs "Process should have no inputs"
            Expect.isNone p.Outputs "Process should have no outputs"
            Expect.isNone p.ParameterValues "Process should have no parameter values"
            Expect.isNone p.ExecutesProtocol "Process should have no protocol"
        )

        // Currently only checks for function failing which it did in python
        testCase "MixedColumns GetProcesses" (fun () ->
            let table =
                TestObjects.Spreadsheet.ArcTable.initWorksheet "SheetName"
                    [
                        TestObjects.Spreadsheet.ArcTable.Protocol.REF.appendLolColumn 4          
                        TestObjects.Spreadsheet.ArcTable.Protocol.Type.appendCollectionColumn 2
                        TestObjects.Spreadsheet.ArcTable.Parameter.appendMixedTemperatureColumn 2 2
                        TestObjects.Spreadsheet.ArcTable.Parameter.appendInstrumentColumn 2 
                        TestObjects.Spreadsheet.ArcTable.Characteristic.appendOrganismColumn 3
                        TestObjects.Spreadsheet.ArcTable.Factor.appendTimeColumn 0
                    ]
                |> Spreadsheet.ArcTable.tryFromFsWorksheet
            table.Value.GetProcesses() |> ignore
            Expect.isTrue true ""
        )

        testCase "EmptyTable GetAndFromProcesses" (fun () ->
            let t = ArcTable.init tableName1
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses tableName1 processes
            Expect.arcTableEqual table t "Table should be equal"
        )
        ptestCase "ParamValueUnitizedNoValueNoInputOutput" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_temperature
            let unit = oa_degreeCel
            let cell = CompositeCell.createUnitized ("",unit)
            t.AddColumn(header,ResizeArray [|cell|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]
            Expect.equal 1 p.ParameterValues.Value.Length "Should have 1 parameter value"
            let pv = p.ParameterValues.Value.[0]
            Expect.isNone pv.Value "Should have no value"
            let resultCategory = Expect.wantSome pv.Category "Should have category"
            Expect.equal resultCategory (ProtocolParameter.create(ParameterName = oa_temperature)) "Category should match"
            let resultUnit = Expect.wantSome pv.Unit "Should have unit"
            Expect.equal resultUnit unit "Unit should match"

            let t' = ArcTable.fromProcesses tableName1 processes
            Expect.arcTableEqual t' t "Table should be equal"
        )
        testCase "ParamValueUnitizedNoValue" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_temperature
            let unit = oa_degreeCel
            let cell = CompositeCell.createUnitized ("",unit)
            t.AddColumn(CompositeHeader.Input(IOType.Source),ResizeArray [|CompositeCell.createFreeText "Source"|])
            t.AddColumn(header,ResizeArray [|cell|])
            t.AddColumn(CompositeHeader.Output(IOType.Sample),ResizeArray [|CompositeCell.createFreeText "Sample"|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]
            Expect.equal 1 p.ParameterValues.Value.Length "Should have 1 parameter value"
            let pv = p.ParameterValues.Value.[0]
            Expect.isNone pv.Value "Should have no value"
            let resultCategory = Expect.wantSome pv.Category "Should have category"
            Expect.equal resultCategory (ProtocolParameter.create(ParameterName = oa_temperature)) "Category should match"
            let resultUnit = Expect.wantSome pv.Unit "Should have unit"
            Expect.equal resultUnit unit "Unit should match"

            let t' = ArcTable.fromProcesses tableName1 processes
            Expect.arcTableEqual t' t "Table should be equal"
        )
        ptestCase "ParamValueUnitizedEmpty" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_temperature
            let cell = CompositeCell.createUnitized ("")
            t.AddColumn(CompositeHeader.Input(IOType.Source),ResizeArray [|CompositeCell.createFreeText "Source"|])
            t.AddColumn(header,ResizeArray [|cell|])
            t.AddColumn(CompositeHeader.Output(IOType.Sample),ResizeArray [|CompositeCell.createFreeText "Sample"|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]
            Expect.equal 1 p.ParameterValues.Value.Length "Should have 1 parameter value"
            let pv = p.ParameterValues.Value.[0]
            Expect.isNone pv.Value "Should have no value"
            let resultCategory = Expect.wantSome pv.Category "Should have category"
            Expect.equal resultCategory (ProtocolParameter.create(ParameterName = oa_temperature)) "Category should match"
            Expect.isNone pv.Unit "Should have no unit"

            let t' = ArcTable.fromProcesses tableName1 processes
            Expect.arcTableEqual t' t "Table should be equal"
        )
        testCase "ParamValueUnitizedNoUnit" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_temperature
            let cell = CompositeCell.createUnitized ("5")
            t.AddColumn(CompositeHeader.Input(IOType.Source),ResizeArray [|CompositeCell.createFreeText "Source"|])
            t.AddColumn(header,ResizeArray [|cell|])
            t.AddColumn(CompositeHeader.Output(IOType.Sample),ResizeArray [|CompositeCell.createFreeText "Sample"|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]
            Expect.equal 1 p.ParameterValues.Value.Length "Should have 1 parameter value"
            let pv = p.ParameterValues.Value.[0]
            let resultValue = Expect.wantSome pv.Value "Should have value"
            Expect.equal resultValue (Value.Int 5) "Value should match"
            let resultCategory = Expect.wantSome pv.Category "Should have category"
            Expect.equal resultCategory (ProtocolParameter.create(ParameterName = oa_temperature)) "Category should match"
            Expect.isNone pv.Unit "Should have no unit"

            let t' = ArcTable.fromProcesses tableName1 processes
            Expect.arcTableEqual t' t "Table should be equal"
        )
        testCase "ParamValueTermEmpty" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_species
            let cell = CompositeCell.createTerm (OntologyAnnotation.create())
            t.AddColumn(CompositeHeader.Input(IOType.Source),ResizeArray [|CompositeCell.createFreeText "Source"|])
            t.AddColumn(header,ResizeArray [|cell|])
            t.AddColumn(CompositeHeader.Output(IOType.Sample),ResizeArray [|CompositeCell.createFreeText "Sample"|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]
            Expect.equal 1 p.ParameterValues.Value.Length "Should have 1 parameter value"
            let pv = p.ParameterValues.Value.[0]
            Expect.isNone pv.Value "Should have no value"
            let resultCategory = Expect.wantSome pv.Category "Should have category"
            Expect.equal resultCategory (ProtocolParameter.create(ParameterName = oa_species)) "Category should match"
            Expect.isNone pv.Unit "Should have no unit"

            let t' = ArcTable.fromProcesses tableName1 processes
            Expect.arcTableEqual t' t "Table should be equal"
        )
        testCase "SingleRowIOAndComment GetAndFromProcesses" (fun () ->
            let t = ArcTable.init(tableName1)
            let commentKey = "MyCommentKey"
            let commentValue = "MyCommentValue"
            t.AddColumn(CompositeHeader.Input(IOType.Source),ResizeArray [|CompositeCell.createFreeText "Source"|])
            t.AddColumn(CompositeHeader.Comment(commentKey), ResizeArray [|CompositeCell.createFreeText commentValue|])
            t.AddColumn(CompositeHeader.Output(IOType.Sample),ResizeArray [|CompositeCell.createFreeText "Sample"|])
            let processes = t.GetProcesses()
            Expect.hasLength processes 1 ""
            let comments = Expect.wantSome processes.[0].Comments ""
            Expect.hasLength comments 1 ""
            let comment = comments.[0]
            Expect.equal comment (Comment(commentKey,commentValue)) ""
            let table = ArcTable.fromProcesses tableName1 processes
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )
    ]

let private tests_ArcTablesProcessSeq = 
    testList "ARCTablesProcessSeq" [
        testCase "NoTables GetProcesses" (fun () ->
            let t = ArcTables(ResizeArray())
            let processes = t.GetProcesses()
            Expect.equal processes.Length 0 "Should have 0 processes"        
        )
        testCase "NoTables GetAndFromProcesses" (fun () ->
            let t = ArcTables(ResizeArray())
            let processes = t.GetProcesses()
            let resultTables = ArcTables.fromProcesses processes
            Expect.equal resultTables.TableCount 0 "Should have 0 tables"
        )

        testCase "EmptyTable GetProcesses" (fun () ->            
            let t = ArcTable.init tableName1
            let tables = ArcTables(ResizeArray([t]))
            let processes = tables.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            Expect.wantSome p.Name "Process should have name"
            |> fun n -> Expect.equal n tableName1 "Process name should match table name"
            Expect.isNone p.Inputs "Process should have no inputs"
            Expect.isNone p.Outputs "Process should have no outputs"
            Expect.isNone p.ParameterValues "Process should have no parameter values"
            Expect.isNone p.ExecutesProtocol "Process should have no protocol"
        )

        testCase "EmptyTable GetAndFromProcesses" (fun () ->
            let t = ArcTable.init tableName1
            let tables = ArcTables(ResizeArray([t]))
            let processes = tables.GetProcesses()
            let table = ArcTables.fromProcesses processes
            Expect.equal table.TableCount 1 "Should have 1 table"
            Expect.arcTableEqual table.Tables.[0] t "Table should be equal"
        )

        testCase "SimpleTables GetAndFromProcesses" (fun () ->
            let t1 = singleRowSingleParam.Copy()
            let t2 = 
                singleRowSingleParam.Copy() |> fun t -> ArcTable.fromArcTableValues(tableName2, t.Headers, t.Values)
            let tables = ResizeArray[t1;t2] |> ArcTables
            let processes = tables.GetProcesses() 
            Expect.equal processes.Length 2 "Should have 2 processes"
            let resultTables = ArcTables.fromProcesses processes
                
            let expectedTables = [ t1;t2 ]
                
            Expect.equal resultTables.TableCount 2 "2 Tables should have been created"
            Expect.arcTableEqual resultTables.Tables.[0] expectedTables.[0] "Table 1 should be equal"
            Expect.arcTableEqual resultTables.Tables.[1] expectedTables.[1] "Table 2 should be equal"

        )
        testCase "OneWithDifferentParamVals GetAndFromProcesses" (fun () ->
            let t1 = singleRowSingleParam.Copy()
            let t2 = 
                twoRowsDifferentParamValue.Copy() |> fun t -> ArcTable.fromArcTableValues(tableName2, t.Headers, t.Values)
            let tables = ResizeArray[t1;t2] |> ArcTables
            let processes = tables.GetProcesses()           
            Expect.equal processes.Length 3 "Should have 3 processes"
            let resultTables = ArcTables.fromProcesses processes
                
            let expectedTables = 
                [
                t1
                t2
                ]
                
            Expect.equal resultTables.TableCount 2 "2 Tables should have been created"
            Expect.arcTableEqual resultTables.Tables.[0] expectedTables.[0] "Table 1 should be equal"
            Expect.arcTableEqual resultTables.Tables.[1] expectedTables.[1] "Table 2 should be equal"
        )
        testCase "TableNameUndescore" (fun () ->
            let t1 = ArcTable.init "Test_Table"
            t1.AddColumn(
                CompositeHeader.Input(IOType.Sample),
                ResizeArray [|CompositeCell.createFreeText "MySample"|]
            )
            let t2 = ArcTable.init "Test_Tisch"
            t2.AddColumn(
                CompositeHeader.Input(IOType.Sample),
                ResizeArray [|CompositeCell.createFreeText "MeinSample"|]
            )
            let ts = ResizeArray [|t1;t2|] |> ArcTables
            let processes = ts.GetProcesses()
            Expect.equal processes.Length 2 "Should have 2 processes"
            let resultTables = ArcTables.fromProcesses processes
            Expect.equal resultTables.TableCount 2 "Should have 2 tables"
            Expect.equal resultTables.Tables.[0].Name "Test_Table" "Table 1 name should match"
            Expect.equal resultTables.Tables.[1].Name "Test_Tisch" "Table 2 name should match"
            Expect.arcTableEqual resultTables.Tables.[0] t1 "Table 1 should be equal"
            Expect.arcTableEqual resultTables.Tables.[1] t2 "Table 2 should be equal"
        )
    ]


let private tests_ProtocolTransformation =
    
    let pName = "ProtocolName"
    let pType = OntologyAnnotation("Growth","ABC","ABC:123")
    let pVersion = "1.0"
    let pDescription = "Great Protocol"

    let pParam1OA = OntologyAnnotation("Temperature","NCIT","NCIT:123")
    let pParam1 = ProtocolParameter.create(ParameterName = pParam1OA)

    testList "Protocol Transformation" [
        testCase "FromProtocol Empty" (fun () ->
            let p = Protocol.create()
            let t = p |> ArcTable.fromProtocol

            Expect.equal t.ColumnCount 0 "ColumnCount should be 0"
            Expect.isTrue (Identifier.isMissingIdentifier t.Name) $"Name should be missing identifier, not \"{t.Name}\""
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
        testCase "GetProtocols NoName" (fun () ->           
            let t = ArcTable.init "TestTable"
            let expected = [Protocol.create(Name = "TestTable")]

            TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocol Name should be ArcTable name."
        )
        testCase "GetProtocols SingleName" (fun () ->           
            let t = ArcTable.init "TestTable"
            let name = "Name"
            t.AddProtocolNameColumn(ResizeArray [|name|])
            let expected = [Protocol.create(Name = name)]

            TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocols do not match"
        )
        testCase "GetProtocols SameNames" (fun () ->           
            let t = ArcTable.init "TestTable"
            let name = "Name"
            t.AddProtocolNameColumn(ResizeArray [|name;name|])
            let expected = [Protocol.create(Name = name)]

            TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocols do not match"
        )
        testCase "GetProtocols DifferentNames" (fun () ->           
            let t = ArcTable.init "TestTable"
            let name1 = "Name"
            let name2 = "Name2"
            t.AddProtocolNameColumn(ResizeArray [|name1;name2|])
            let expected = [Protocol.create(Name = name1);Protocol.create(Name = name2)]

            TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocols do not match"
        )
        //testCase "GetProtocols Parameters" (fun () ->
        //    let t = ArcTable.init "TestTable"
        //    let name1 = "Name"
        //    let name2 = "Name2"
        //    t.AddProtocolNameColumn([|name1;name2|])
        //    t.addpara([|pParam1;pParam1|])
        //    let expected = [Protocol.create(Name = name1;Parameters = [pParam1]);Protocol.create(Name = name2;Parameters = [pParam1])]
        //    TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocols do not match"
        
        //)


    ]


let main = 
    testList "ArcJsonConversion" [
        tests_ProtocolTransformation
        tests_ArcTableProcess
        tests_ArcTablesProcessSeq
    ]