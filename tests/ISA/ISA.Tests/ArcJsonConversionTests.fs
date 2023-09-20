module ArcJsonConversion.Tests

open ARCtrl.ISA

open TestingUtils

module Helper =
    let tableName1 = "Test1"
    let tableName2 = "Test2"
    let oa_species = OntologyAnnotation.fromString("species", "GO", "GO:0123456")
    let oa_chlamy = OntologyAnnotation.fromString("Chlamy", "NCBI", "NCBI:0123456")
    let oa_instrumentModel = OntologyAnnotation.fromString("instrument model", "MS", "MS:0123456")
    let oa_SCIEXInstrumentModel = OntologyAnnotation.fromString("SCIEX instrument model", "MS", "MS:654321")
    let oa_time = OntologyAnnotation.fromString("time", "UO", "UO:0000010")
    let oa_hour = OntologyAnnotation.fromString("hour", "UO", "UO:0000032")
    let oa_temperature = OntologyAnnotation.fromString("temperature","NCIT","NCIT:0123210")
    let oa_degreeCel =  OntologyAnnotation.fromString("degree celsius","UO","UO:0000027")

    /// This function can be used to put ArcTable.Values into a nice format for printing/writing to IO
    let tableValues_printable (table:ArcTable) = 
        [
            for KeyValue((c,r),v) in table.Values do
                yield $"({c},{r}) {v}"
        ]

    let createCells_FreeText pretext (count) = Array.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
    let createCells_Sciex (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_SCIEXInstrumentModel)
    let createCells_chlamy (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_chlamy)
    let createCells_DegreeCelsius (count) = Array.init count (fun i -> CompositeCell.createUnitized (string i,oa_degreeCel))
    let createCells_Hour (count) = Array.init count (fun i -> CompositeCell.createUnitized (string i,oa_hour))

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
            CompositeColumn.create(CompositeHeader.Input IOType.RawDataFile, createCells_FreeText "RData" 1)
            CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_chlamy 1)
            CompositeColumn.create(CompositeHeader.Output IOType.DerivedDataFile, createCells_FreeText "DData" 1)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    let singleRowDataOutputWithFactor = 
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.RawDataFile, createCells_FreeText "RData" 1)
            CompositeColumn.create(CompositeHeader.Factor oa_temperature, createCells_DegreeCelsius 1)
            CompositeColumn.create(CompositeHeader.Output IOType.DerivedDataFile, createCells_FreeText "DData" 1)
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
            CompositeColumn.create(CompositeHeader.ProtocolREF, [|CompositeCell.createFreeText "MyProtocol"|])
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
            let t = singleRowSingleParam.Copy()
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
            let t = singleRowSingleParam.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses tableName1 processes
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
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
    ]

let private tests_arcTablesProcessSeq = 
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
            Expect.equal resultTables.Count 0 "Should have 0 tables"
        )

        testCase "SimpleTables GetAndFromProcesses" (fun () ->
            let t1 = singleRowSingleParam.Copy()
            let t2 = 
                singleRowSingleParam.Copy() |> fun t -> ArcTable.create(tableName2, t.Headers, t.Values)
            let tables = ResizeArray[t1;t2] |> ArcTables
            let processes = tables.GetProcesses() 
            Expect.equal processes.Length 2 "Should have 2 processes"
            let resultTables = ArcTables.fromProcesses processes
                
            let expectedTables = [ t1;t2 ]
                
            Expect.equal resultTables.Count 2 "2 Tables should have been created"
            Expect.arcTableEqual resultTables.[0] expectedTables.[0] "Table 1 should be equal"
            Expect.arcTableEqual resultTables.[1] expectedTables.[1] "Table 2 should be equal"

        )
        testCase "OneWithDifferentParamVals GetAndFromProcesses" (fun () ->
            let t1 = singleRowSingleParam.Copy()
            let t2 = 
                twoRowsDifferentParamValue.Copy() |> fun t -> ArcTable.create(tableName2, t.Headers, t.Values)
            let tables = ResizeArray[t1;t2] |> ArcTables
            let processes = tables.GetProcesses()           
            Expect.equal processes.Length 3 "Should have 3 processes"
            let resultTables = ArcTables.fromProcesses processes
                
            let expectedTables = 
                [
                t1
                t2
                ]
                
            Expect.equal resultTables.Count 2 "2 Tables should have been created"
            Expect.arcTableEqual resultTables.[0] expectedTables.[0] "Table 1 should be equal"
            Expect.arcTableEqual resultTables.[1] expectedTables.[1] "Table 2 should be equal"
        )
    ]

let private tests_arcAssay = 
    testList "ARCAssay" [

        let identifier = "MyIdentifier"
        let measurementType = OntologyAnnotation.fromString("Measurement Type")
        let technologyType = OntologyAnnotation.fromString("Technology Type")
        let technologyPlatformName = "Technology Platform"
        let technologyPlatformTSR = "ABC"
        let technologyPlatformTAN = "ABC:123"
        let technologyPlatform = OntologyAnnotation.fromString(technologyPlatformName,technologyPlatformTSR,technologyPlatformTAN)
        let technologyPlatformExpectedString = $"{technologyPlatformName} ({technologyPlatformTAN})"
        let t1 = singleRowSingleParam.Copy()
        let t2 = 
            singleRowDataInputWithCharacteristic.Copy() |> fun t -> ArcTable.create(tableName2, t.Headers, t.Values)
        let tables = ResizeArray[t1;t2] |> ArcTables
        let comments = [|Comment.create("Comment 1")|]
        let fullArcAssay =  ArcAssay.create(identifier, measurementType, technologyType, technologyPlatform, tables.Tables, Comments = comments)

        testCase "Identifier Set" (fun () ->
            let identifier = "MyIdentifier"
            let arcAssay = ArcAssay.create(identifier)
            let assay = arcAssay.ToAssay()
            Expect.isSome assay.FileName "Assay should have fileName" 
            let expectedFileName = Identifier.Assay.fileNameFromIdentifier identifier
            Expect.equal assay.FileName.Value expectedFileName "Assay fileName should match"
            let resultArcAssay = ArcAssay.fromAssay assay
            Expect.equal resultArcAssay.Identifier identifier "ArcAssay identifier should match"            
        )
        testCase "No Identifier Set" (fun () ->
            let identifier = Identifier.createMissingIdentifier()
            let arcAssay = ArcAssay.create(identifier)
            let assay = arcAssay.ToAssay()
            Expect.isNone assay.FileName "Assay should not have fileName" 
            let resultArcAssay = ArcAssay.fromAssay assay
            Expect.isTrue (Identifier.isMissingIdentifier resultArcAssay.Identifier) "ArcAssay identifier should be missing"
        )
        testCase "FullAssay ToAssay" (fun () ->
            let arcAssay = fullArcAssay.Copy()
            let assay = arcAssay.ToAssay()

            Expect.isSome assay.FileName "Assay should have fileName"
            let expectedFileName = Identifier.Assay.fileNameFromIdentifier identifier
            Expect.equal assay.FileName.Value expectedFileName "Assay fileName should match"
        
            Expect.isSome assay.MeasurementType "Assay should have measurementType"
            Expect.equal assay.MeasurementType.Value measurementType "Assay measurementType should match"
            
            Expect.isSome assay.TechnologyType "Assay should have technologyType"
            Expect.equal assay.TechnologyType.Value technologyType "Assay technologyType should match"

            Expect.isSome assay.TechnologyPlatform "Assay should have technologyPlatform"
            Expect.equal assay.TechnologyPlatform.Value technologyPlatformExpectedString "Assay technologyPlatform should match"

            Expect.isSome assay.ProcessSequence "Assay should have processes"
            Expect.equal assay.ProcessSequence.Value.Length 2 "Should have 2 processes"

            Expect.isSome assay.CharacteristicCategories "Assay should have characteristicCategories"
            Expect.equal assay.CharacteristicCategories.Value.Length 1 "Should have 1 characteristicCategory"

            Expect.isSome assay.DataFiles "Assay should have dataFiles"
            Expect.equal assay.DataFiles.Value.Length 1 "Should have 1 dataFile"

            Expect.isSome assay.Comments "Assay should have comments"
            Expect.equal assay.Comments.Value.Length 1 "Should have 1 comment"                      
        )
        testCase "FullAssay ToAndFromAssay" (fun () ->
            let arcAssay = fullArcAssay.Copy()
            let assay = arcAssay.ToAssay()

            let resultingArcAssay = ArcAssay.fromAssay assay
            Expect.equal resultingArcAssay.Identifier arcAssay.Identifier "ArcAssay identifier should match"
            Expect.equal resultingArcAssay.MeasurementType arcAssay.MeasurementType "ArcAssay measurementType should match"
            Expect.equal resultingArcAssay.TechnologyType arcAssay.TechnologyType "ArcAssay technologyType should match"
            Expect.equal resultingArcAssay.TechnologyPlatform arcAssay.TechnologyPlatform "ArcAssay technologyPlatform should match"
            let expectedTables = [t1;t2] 
                
            Expect.sequenceEqual resultingArcAssay.Tables expectedTables "ArcAssay tables should match"
            Expect.sequenceEqual resultingArcAssay.Comments arcAssay.Comments "ArcAssay comments should match"
        )
    ]

let private tests_arcStudy = 
    testList "ARCStudy" [
        testCase "Identifier Set" (fun () ->
            let identifier = "MyIdentifier"       
            let arcStudy = ArcStudy.create(identifier)
            let study = arcStudy.ToStudy(ResizeArray())
            Expect.isSome study.Identifier "Study should have identifier" 
            Expect.equal study.Identifier.Value identifier "Study identifier should match"
            let resultArcStudy, resultArcAssays = ArcStudy.fromStudy study
            Expect.equal resultArcStudy.Identifier identifier "ArcStudy identifier should match"
            Expect.isEmpty resultArcAssays "ArcAssays should match"
        )
        testCase "No Identifier Set" (fun () ->
            let identifier = Identifier.createMissingIdentifier()
            let arcStudy = ArcStudy.create(identifier)
            let study = arcStudy.ToStudy(ResizeArray())
            Expect.isNone study.Identifier "Study should not have identifier" 
            let resultArcStudy, resultArcAssays = ArcStudy.fromStudy study
            Expect.isTrue (Identifier.isMissingIdentifier resultArcStudy.Identifier) "ArcStudy identifier should be missing"
            Expect.isEmpty resultArcAssays "ArcAssays should match"
        )
    ]



let private tests_arcInvestigation = 
    testList "ARCInvestigation" [
        testCase "Identifier Set" (fun () ->
            let identifier = "MyIdentifier"
            let arcInvestigation = ArcInvestigation.create(identifier)
            let investigation = arcInvestigation.ToInvestigation()
            Expect.isSome investigation.Identifier "Investigation should have identifier" 
            Expect.equal investigation.Identifier.Value identifier "Investigation identifier should match"
            let resultArcInvestigation = ArcInvestigation.fromInvestigation investigation
            Expect.equal resultArcInvestigation.Identifier identifier "ArcInvestigation identifier should match"
        )
        testCase "No Identifier Set" (fun () ->
            let identifier = Identifier.createMissingIdentifier()
            let arcInvestigation = ArcInvestigation.create(identifier)
            let investigation = arcInvestigation.ToInvestigation()
            Expect.isNone investigation.Identifier "Investigation should not have identifier" 
            let resultArcInvestigation = ArcInvestigation.fromInvestigation investigation
            Expect.isTrue (Identifier.isMissingIdentifier resultArcInvestigation.Identifier) "ArcInvestigation identifier should be missing"
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
            t.AddProtocolNameColumn([|name|])
            let expected = [Protocol.create(Name = name)]

            TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocols do not match"
        )
        testCase "GetProtocols SameNames" (fun () ->           
            let t = ArcTable.init "TestTable"
            let name = "Name"
            t.AddProtocolNameColumn([|name;name|])
            let expected = [Protocol.create(Name = name)]

            TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocols do not match"
        )
        testCase "GetProtocols DifferentNames" (fun () ->           
            let t = ArcTable.init "TestTable"
            let name1 = "Name"
            let name2 = "Name2"
            t.AddProtocolNameColumn([|name1;name2|])
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
        tests_protocolTransformation
        tests_arcTableProcess
        tests_arcTablesProcessSeq
        tests_arcAssay
        tests_arcStudy
        tests_arcInvestigation
    ]