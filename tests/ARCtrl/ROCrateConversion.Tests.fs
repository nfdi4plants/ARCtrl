module ARCtrl.ROCrateConversion.Tests

open ARCtrl
open ARCtrl.Process
open ARCtrl.Conversion
open ARCtrl.ROCrate
open TestingUtils

module Helper =
    let tableName1 = "Test1"
    let tableName2 = "Test2"
    let oa_species = OntologyAnnotation("species", "GO", "GO:0123456")
    let dt_species = DefinedTerm.create(name = "species", termCode = oa_species.TermAccessionOntobeeUrl)
    let oa_chlamy = OntologyAnnotation("Chlamy", "NCBI", "NCBI:0123456")
    let dt_chlamy = DefinedTerm.create(name = "Chlamy", termCode = oa_chlamy.TermAccessionOntobeeUrl)
    let oa_instrumentModel = OntologyAnnotation("instrument model", "MS", "MS:0123456")
    let dt_instrumentModel = DefinedTerm.create(name = "instrument model", termCode = oa_instrumentModel.TermAccessionOntobeeUrl)
    let oa_SCIEXInstrumentModel = OntologyAnnotation("SCIEX instrument model", "MS", "MS:654321")
    let dt_SCIEXInstrumentModel = DefinedTerm.create(name = "SCIEX instrument model", termCode = oa_SCIEXInstrumentModel.TermAccessionOntobeeUrl)
    let oa_time = OntologyAnnotation("time", "UO", "UO:0000010")
    let dt_time = DefinedTerm.create(name = "time", termCode = oa_time.TermAccessionOntobeeUrl)
    let oa_hour = OntologyAnnotation("hour", "UO", "UO:0000032")
    let dt_hour = DefinedTerm.create(name = "hour", termCode = oa_hour.TermAccessionOntobeeUrl)
    let oa_temperature = OntologyAnnotation("temperature","NCIT","NCIT:0123210")
    let dt_temperature = DefinedTerm.create(name = "temperature", termCode = oa_temperature.TermAccessionOntobeeUrl)
    let oa_degreeCel =  OntologyAnnotation("degree celsius","UO","UO:0000027")
    let dt_degreeCel = DefinedTerm.create(name = "degree celsius", termCode = oa_degreeCel.TermAccessionOntobeeUrl)

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


let private tests_ProcessInput =
    testList "ProcessInput" [
        testCase "Source" (fun () ->
            let header = CompositeHeader.Input(IOType.Source)
            let cell = CompositeCell.createFreeText "MySource"
            let input = JsonTypes.composeProcessInput header cell

            Expect.isTrue (Sample.validateSource input) "Should be a valid source"
            let name = Sample.getNameAsString input
            Expect.equal name "MySource" "Name should match"

            let header',cell' = JsonTypes.decomposeProcessInput input
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Sample" (fun () ->
            let header = CompositeHeader.Input(IOType.Sample)
            let cell = CompositeCell.createFreeText "MySample"
            let input = JsonTypes.composeProcessInput header cell

            Expect.isTrue (Sample.validateSample input) "Should be a valid sample"
            let name = Sample.getNameAsString input
            Expect.equal name "MySample" "Name should match"

            let header',cell' = JsonTypes.decomposeProcessInput input
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Data_Data" (fun () ->
            let header = CompositeHeader.Input(IOType.Data)
            let data = Data(name = "MyData", format = "text/csv", selectorFormat = "MySelector")
            let cell = CompositeCell.createData data
            let input = JsonTypes.composeProcessInput header cell
    
            Expect.isTrue (File.validate input) "Should be a valid data"
            let name = File.getNameAsString input
            Expect.equal name "MyData" "Name should match"
    
            let header',cell' = JsonTypes.decomposeProcessInput input
            Expect.equal header header' "Header should match"
            data.ID <- Some input.Id
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Data_Freetext" (fun () ->
            let header = CompositeHeader.Input(IOType.Data)
            let cell = CompositeCell.createFreeText "MyData"
            let input = JsonTypes.composeProcessInput header cell
    
            Expect.isTrue (File.validate input) "Should be a valid data"
            let name = File.getNameAsString input
            Expect.equal name "MyData" "Name should match"
    
            let header',cell' = JsonTypes.decomposeProcessInput input
            let expectedCell = CompositeCell.createData (Data (id = input.Id, name = "MyData"))
            Expect.equal header header' "Header should match"
            Expect.equal expectedCell cell' "Cell should match"
        )
        testCase "Material" (fun () ->
            let header = CompositeHeader.Input(IOType.Material)
            let cell = CompositeCell.createFreeText "MyMaterial"
            let input = JsonTypes.composeProcessInput header cell
    
            Expect.isTrue (Sample.validateMaterial input) "Should be a valid material"
            let name = Sample.getNameAsString input
            Expect.equal name "MyMaterial" "Name should match"
    
            let header',cell' = JsonTypes.decomposeProcessInput input
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "FreeType" (fun () ->
            let header = CompositeHeader.Input (IOType.FreeText "MyInputType")
            let cell = CompositeCell.createFreeText "MyFreeText"
            let input = JsonTypes.composeProcessInput header cell

            let name = Sample.getNameAsString input
            Expect.equal name "MyFreeText" "Name should match"

            let schemaType = input.SchemaType
            Expect.hasLength schemaType 1 "Should have 1 schema type"
            Expect.equal schemaType.[0] "MyInputType" "Schema type should be FreeText"

            let header',cell' = JsonTypes.decomposeProcessInput input
            Expect.equal header header' "Header should match"
        )
    ]

let private tests_ProcessOutput =
    testList "ProcessOutput" [
        testCase "Sample" (fun () ->
            let header = CompositeHeader.Output(IOType.Sample)
            let cell = CompositeCell.createFreeText "MySample"
            let output = JsonTypes.composeProcessOutput header cell

            Expect.isTrue (Sample.validateSample output) "Should be a valid sample"
            let name = Sample.getNameAsString output
            Expect.equal name "MySample" "Name should match"

            let header',cell' = JsonTypes.decomposeProcessOutput output
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Data_Data" (fun () ->
            let header = CompositeHeader.Output(IOType.Data)
            let data = Data(name = "MyData", format = "text/csv", selectorFormat = "MySelector")
            let cell = CompositeCell.createData data
            let output = JsonTypes.composeProcessOutput header cell
    
            Expect.isTrue (File.validate output) "Should be a valid data"
            let name = File.getNameAsString output
            Expect.equal name "MyData" "Name should match"
    
            let header',cell' = JsonTypes.decomposeProcessOutput output
            Expect.equal header header' "Header should match"
            data.ID <- Some output.Id
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Data_Freetext" (fun () ->
            let header = CompositeHeader.Output(IOType.Data)
            let cell = CompositeCell.createFreeText "MyData"
            let output = JsonTypes.composeProcessOutput header cell
    
            Expect.isTrue (File.validate output) "Should be a valid data"
            let name = File.getNameAsString output
            Expect.equal name "MyData" "Name should match"
    
            let header',cell' = JsonTypes.decomposeProcessOutput output
            let expectedCell = CompositeCell.createData (Data (id = output.Id, name = "MyData"))
            Expect.equal header header' "Header should match"
            Expect.equal expectedCell cell' "Cell should match"
        )
        testCase "Material" (fun () ->
            let header = CompositeHeader.Output(IOType.Material)
            let cell = CompositeCell.createFreeText "MyMaterial"
            let output = JsonTypes.composeProcessOutput header cell
    
            Expect.isTrue (Sample.validateMaterial output) "Should be a valid material"
            let name = Sample.getNameAsString output
            Expect.equal name "MyMaterial" "Name should match"
    
            let header',cell' = JsonTypes.decomposeProcessOutput output
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "FreeType" (fun () ->
            let header = CompositeHeader.Output (IOType.FreeText "MyOutputType")
            let cell = CompositeCell.createFreeText "MyFreeText"
            let output = JsonTypes.composeProcessOutput header cell

            let name = Sample.getNameAsString output
            Expect.equal name "MyFreeText" "Name should match"

            let schemaType = output.SchemaType
            Expect.hasLength schemaType 1 "Should have 1 schema type"
            Expect.equal schemaType.[0] "MyOutputType" "Schema type should be FreeText"

            let header',cell' = JsonTypes.decomposeProcessOutput output
            Expect.equal header header' "Header should match"
        )
    ]

let private tests_PropertyValue =
    testList "PropertyValue" [
        testCase "Characteristic_Ontology" (fun () ->
            let header = CompositeHeader.Characteristic oa_species
            let cell = CompositeCell.createTerm oa_chlamy
            let pv = JsonTypes.composeCharacteristicValue header cell

            let name = PropertyValue.getNameAsString pv
            Expect.equal name oa_species.NameText "Name should match"

            let value = PropertyValue.getValueAsString pv
            Expect.equal value oa_chlamy.NameText "Value should match"

            let propertyID = Expect.wantSome (PropertyValue.tryGetPropertyIDAsString pv) "Should have property ID"
            Expect.equal propertyID oa_species.TermAccessionOntobeeUrl "Property ID should match"

            let valueReference = Expect.wantSome (PropertyValue.tryGetValueReference pv) "Should have value reference"
            Expect.equal valueReference oa_chlamy.TermAccessionOntobeeUrl "Value reference should match"

            let header',cell' = JsonTypes.decomposeCharacteristicValue pv
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Parameter_Unitized" (fun () ->
            let header = CompositeHeader.Parameter oa_temperature
            let cell = CompositeCell.createUnitized ("5",oa_degreeCel)
            let pv = JsonTypes.composeParameterValue header cell

            let name = PropertyValue.getNameAsString pv
            Expect.equal name oa_temperature.NameText "Name should match"

            let value = PropertyValue.getValueAsString pv
            Expect.equal value "5" "Value should match"

            let propertyID = Expect.wantSome (PropertyValue.tryGetPropertyIDAsString pv) "Should have property ID"
            Expect.equal propertyID oa_temperature.TermAccessionOntobeeUrl "Property ID should match"

            let unit = Expect.wantSome (PropertyValue.tryGetUnitTextAsString pv) "Should have unit"
            Expect.equal unit oa_degreeCel.NameText "Unit should match"

            let unitCode = Expect.wantSome (PropertyValue.tryGetUnitCodeAsString pv) "Should have unit code"
            Expect.equal unitCode oa_degreeCel.TermAccessionOntobeeUrl "Unit code should match"

            let header',cell' = JsonTypes.decomposeParameterValue pv
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Parameter_Unitized_Valueless" (fun () ->
            let header = CompositeHeader.Parameter oa_temperature
            let cell = CompositeCell.createUnitized ("",oa_degreeCel)
            let pv = JsonTypes.composeParameterValue header cell

            let name = PropertyValue.getNameAsString pv
            Expect.equal name oa_temperature.NameText "Name should match"

            Expect.isNone (PropertyValue.tryGetValueAsString pv) "Should not have value"

            let propertyID = Expect.wantSome (PropertyValue.tryGetPropertyIDAsString pv) "Should have property ID"
            Expect.equal propertyID oa_temperature.TermAccessionOntobeeUrl "Property ID should match"

            let unit = Expect.wantSome (PropertyValue.tryGetUnitTextAsString pv) "Should have unit"
            Expect.equal unit oa_degreeCel.NameText "Unit should match"

            let unitCode = Expect.wantSome (PropertyValue.tryGetUnitCodeAsString pv) "Should have unit code"
            Expect.equal unitCode oa_degreeCel.TermAccessionOntobeeUrl "Unit code should match"

            let header',cell' = JsonTypes.decomposeParameterValue pv
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "FactorValue_Valueless" (fun () ->
            let header = CompositeHeader.Factor oa_temperature
            let cell = CompositeCell.createTerm (OntologyAnnotation.create())
            let pv = JsonTypes.composeFactorValue header cell

            let name = PropertyValue.getNameAsString pv
            Expect.equal name oa_temperature.NameText "Name should match"

            let propertyID = Expect.wantSome (PropertyValue.tryGetPropertyIDAsString pv) "Should have property ID"
            Expect.equal propertyID oa_temperature.TermAccessionOntobeeUrl "Property ID should match"

            Expect.isNone (PropertyValue.tryGetValueAsString pv) "Should not have value"

            let header',cell' = JsonTypes.decomposeFactorValue pv
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Component_Freetexts" (fun () ->
            let header = CompositeHeader.Component (OntologyAnnotation.create(name = "MyComponentHeader"))
            let cell = CompositeCell.createTerm (OntologyAnnotation.create(name = "MyComponentValue"))
            let pv = JsonTypes.composeComponent header cell

            let name = PropertyValue.getNameAsString pv
            Expect.equal name "MyComponentHeader" "Name should match"

            let value = PropertyValue.getValueAsString pv
            Expect.equal value "MyComponentValue" "Value should match"

            Expect.isNone (PropertyValue.tryGetPropertyIDAsString pv) "Should not have property ID"
            Expect.isNone (PropertyValue.tryGetValueReference pv) "Should not have value reference"
            Expect.isNone (PropertyValue.tryGetUnitTextAsString pv) "Should not have unit"
            Expect.isNone (PropertyValue.tryGetUnitCodeAsString pv) "Should not have unit code"

            let header',cell' = JsonTypes.decomposeComponent pv
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
    ]


let private tests_ArcTableProcess = 
    testList "ARCTableProcess" [
        testCase "SingleRowSingleParam GetProcesses" (fun () ->
            let t = singleRowSingleParam.Copy()
            let processes = t.GetProcesses()
            let expectedPPV =
                PropertyValue.createParameterValue(
                    name = oa_species.NameText,
                    value = oa_chlamy.NameText,
                    propertyID = oa_species.TermAccessionOntobeeUrl,
                    valueReference = oa_chlamy.TermAccessionOntobeeUrl
                )
            ColumnIndex.setIndex expectedPPV 0
            let expectedInput = Sample.createSource(name = "Source_0")
            let expectedOutput = Sample.createSample(name = "Sample_0")
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            let paramValues = LabProcess.getParameterValues(p)
            Expect.equal paramValues.Count 1 "Process should have 1 parameter values"
            Expect.equal paramValues.[0] expectedPPV "Param value does not match"
            let inputs = LabProcess.getObjects(p)
            Expect.equal inputs.Count 1 "Process should have 1 input"
            Expect.equal inputs.[0] expectedInput "Input value does not match"
            let outputs = LabProcess.getResults(p)
            Expect.equal outputs.Count 1 "Process should have 1 output"
            Expect.equal outputs.[0] expectedOutput "Output value does not match"
            let name = LabProcess.getNameAsString p
            Expect.equal name tableName1 "Process name should match table name"
        )
        testCase "SingleRowSingleParam GetAndFromProcesses" (fun () ->
            let t = singleRowSingleParam.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses(tableName1,processes)
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )
        testCase "SingleRowOutputSource GetProcesses" (fun () ->
            let t = singleRowOutputSource.Copy()
            let processes = t.GetProcesses()           
            let expectedInput = Sample.createSource(name = "Source_0")
            let expectedOutput = Sample.createSample(name = "Sample_0")
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            let inputs = LabProcess.getObjects(p)
            Expect.equal inputs.Count 1 "Process should have 1 input"
            Expect.equal inputs.[0] expectedInput "Input value does not match"
            let outputs = LabProcess.getResults(p)
            Expect.equal outputs.Count 1 "Process should have 1 output"
            Expect.equal outputs.[0] expectedOutput "Output value does not match"
            let name = LabProcess.getNameAsString p
            Expect.equal name tableName1 "Process name should match table name"
        )

        testCase "SingleRowMixedValues GetProcesses" (fun () ->          
            let t = singleRowMixedValues.Copy()          
            let processes = t.GetProcesses()           
            Expect.equal processes.Length 1 "Should have 1 process"
            Expect.equal (LabProcess.getParameterValues processes.[0]).Count 1 "Process should have 1 parameter values"
            let input = Expect.wantSome ((LabProcess.getObjects processes.[0]) |> Seq.tryExactlyOne) "Process should have 1 input"
            let output = Expect.wantSome ((LabProcess.getResults processes.[0]) |> Seq.tryExactlyOne) "Process should have 1 output"
            let charas = Sample.getCharacteristics input
            Expect.hasLength charas 1 "Input should have the charactersitic"
            let factors = Sample.getFactors output
            Expect.hasLength factors 1 "Output should have the factor value"
        )

        testCase "SingleRowMixedValues GetAndFromProcesses" (fun () ->
            let t = singleRowMixedValues.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses(tableName1,processes)
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )

        testCase "SingleRowDataInputWithCharacteristic GetProcesses" (fun () ->
            let t = singleRowDataInputWithCharacteristic.Copy()
            let processes = t.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            let inputs = LabProcess.getObjects(p)
            Expect.equal inputs.Count 1 "Process should have 1 input"
            let outputs = LabProcess.getResults(p)
            Expect.equal outputs.Count 1 "Process should have 1 output"
            Expect.isTrue (File.validate(inputs.[0])) "First input should be data"
            let charas = Sample.getCharacteristics(inputs.[0])
            Expect.hasLength charas 1 "Input should have the charactersitic"
        )

        testCase "SingleRowDataInputWithCharacteristic GetAndFromProcesses" (fun () ->
            let t = singleRowDataInputWithCharacteristic.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses(tableName1,processes)
            let expectedTable = t
            let expectedInputData = CompositeCell.Data(Data(id = "RData_0", name = "RData_0"))
            let expectedOutputData = CompositeCell.Data(Data(id = "DData_0", name = "DData_0"))
            expectedTable.SetCellAt(0,0,expectedInputData)
            expectedTable.SetCellAt(t.ColumnCount - 1,0,expectedOutputData)
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )

        testCase "SingleRowDataOutputWithFactor GetProcesses" (fun () ->
            let t = singleRowDataOutputWithFactor.Copy()
            let processes = t.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            let inputs = LabProcess.getObjects(p)
            Expect.equal inputs.Count 1 "Process should have 1 input"
            Expect.isTrue (File.validate inputs.[0]) "input should be data"
            let outputs = LabProcess.getResults(p)
            Expect.equal outputs.Count 1 "Process should have 1 output"
            Expect.isTrue (File.validate outputs.[0]) "output should be data"
            let factors = Sample.getFactors(outputs.[0])
            Expect.hasLength factors 1 "output should have 1 factor value"
        )

        testCase "SingleRowDataOutputWithFactor GetAndFromProcesses" (fun () ->
            let t = singleRowDataOutputWithFactor.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses(tableName1,processes)
            let expectedTable = t
            let expectedInputData = CompositeCell.Data(Data(id = "RData_0", name = "RData_0"))
            let expectedOutputData = CompositeCell.Data(Data(id = "DData_0", name = "DData_0"))
            expectedTable.SetCellAt(0,0,expectedInputData)
            expectedTable.SetCellAt(t.ColumnCount - 1,0,expectedOutputData)
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )

        testCase "TwoRowsSameParamValue GetAndFromProcesses" (fun () ->
            let t = twoRowsSameParamValue.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses(tableName1,processes)
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
            let table = ArcTable.fromProcesses(tableName1,processes)
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )

        testCase "SingleRowWithProtocolREF GetProcesses" (fun () ->
            let t = singleRowWithProtocolRef.Copy()
            let processes = t.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            let prot = Expect.wantSome (LabProcess.tryGetExecutesLabProtocol(p)) "Process should have protocol"
            let protName = Expect.wantSome (LabProtocol.tryGetNameAsString(prot)) "Protocol should have name"
            Expect.equal protName "MyProtocol" "Protocol name should match"
            let pName = Expect.wantSome (LabProcess.tryGetNameAsString(p)) "Process should have name"
            Expect.equal pName tableName1 "Process name should match table name"
        )

        testCase "SingleRowWithProtocolREF GetAndFromProcesses" (fun () ->
            let t = singleRowWithProtocolRef.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses(tableName1,processes)
            Expect.arcTableEqual table t "Table should be equal"
        )

        testCase "EmptyTable GetProcesses" (fun () ->            
            let t = ArcTable.init tableName1
            let processes = t.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            let name = LabProcess.getNameAsString p
            Expect.equal name tableName1 "Process name should match table name"
            Expect.isEmpty (LabProcess.getObjects(p)) "Process should have no inputs"
            Expect.isEmpty (LabProcess.getResults(p)) "Process should have no outputs"
            Expect.isEmpty (LabProcess.getParameterValues(p)) "Process should have no parameter values"
            Expect.isNone (LabProcess.tryGetExecutesLabProtocol(p)) "Process should have no protocol"
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
            let table = ArcTable.fromProcesses(tableName1,processes)
            Expect.arcTableEqual table t "Table should be equal"
        )
        testCase "OnlyInput GetAndFromProcessess" (fun () ->
            let t = ArcTable.init tableName1
            t.AddColumn(CompositeHeader.Input(IOType.Source),[|CompositeCell.createFreeText "Source"|])
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses(tableName1,processes)
            Expect.arcTableEqual table t "Table should be equal"
        )
        testCase "ParamValueUnitizedNoValueNoInputOutput" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_temperature
            let unit = oa_degreeCel
            let cell = CompositeCell.createUnitized ("",unit)
            t.AddColumn(header,[|cell|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]
            let pvs = LabProcess.getParameterValues p
            Expect.hasLength pvs 1 "Should have 1 parameter value"
            let pv = pvs.[0]
            Expect.isNone (PropertyValue.tryGetValueAsString(pv)) "Should have no value"
            let categoryName = Expect.wantSome (PropertyValue.tryGetNameAsString(pv)) "Should have category name"
            Expect.equal categoryName oa_temperature.NameText "Category name should match"
            let categoryID = Expect.wantSome (PropertyValue.tryGetPropertyIDAsString(pv)) "Should have category ID"
            Expect.equal categoryID oa_temperature.TermAccessionOntobeeUrl "Category ID should match"
            let unitName = Expect.wantSome (PropertyValue.tryGetUnitTextAsString(pv)) "Should have unit name"
            Expect.equal unitName oa_degreeCel.NameText "Unit name should match"
            let unitCode = Expect.wantSome (PropertyValue.tryGetUnitCodeAsString(pv)) "Should have unit code"
            Expect.equal unitCode oa_degreeCel.TermAccessionOntobeeUrl "Unit code should match"

            let t' = ArcTable.fromProcesses(tableName1,processes)
            Expect.arcTableEqual t' t "Table should be equal"
        )
        testCase "ParamValueUnitizedNoValue" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_temperature
            let unit = oa_degreeCel
            let cell = CompositeCell.createUnitized ("",unit)
            t.AddColumn(CompositeHeader.Input(IOType.Source),[|CompositeCell.createFreeText "Source"|])
            t.AddColumn(header,[|cell|])
            t.AddColumn(CompositeHeader.Output(IOType.Sample),[|CompositeCell.createFreeText "Sample"|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]

            let pvs = LabProcess.getParameterValues p
            Expect.hasLength pvs 1 "Should have 1 parameter value"
            let pv = pvs.[0]
            Expect.isNone (PropertyValue.tryGetValueAsString(pv)) "Should have no value"
            let categoryName = Expect.wantSome (PropertyValue.tryGetNameAsString(pv)) "Should have category name"
            Expect.equal categoryName oa_temperature.NameText "Category name should match"
            let categoryID = Expect.wantSome (PropertyValue.tryGetPropertyIDAsString(pv)) "Should have category ID"
            Expect.equal categoryID oa_temperature.TermAccessionOntobeeUrl "Category ID should match"
            let unitName = Expect.wantSome (PropertyValue.tryGetUnitTextAsString(pv)) "Should have unit name"
            Expect.equal unitName oa_degreeCel.NameText "Unit name should match"
            let unitCode = Expect.wantSome (PropertyValue.tryGetUnitCodeAsString(pv)) "Should have unit code"
            Expect.equal unitCode oa_degreeCel.TermAccessionOntobeeUrl "Unit code should match"

            let t' = ArcTable.fromProcesses(tableName1,processes)
            Expect.arcTableEqual t' t "Table should be equal"
        )
        ptestCase "ParamValueUnitizedEmpty" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_temperature
            let cell = CompositeCell.createUnitized ("")
            t.AddColumn(CompositeHeader.Input(IOType.Source),[|CompositeCell.createFreeText "Source"|])
            t.AddColumn(header,[|cell|])
            t.AddColumn(CompositeHeader.Output(IOType.Sample),[|CompositeCell.createFreeText "Sample"|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]
            let pvs = LabProcess.getParameterValues p
            Expect.hasLength pvs 1 "Should have 1 parameter value"
            let pv = pvs.[0]
            Expect.isNone (PropertyValue.tryGetValueAsString(pv)) "Should have no value"
            let categoryName = Expect.wantSome (PropertyValue.tryGetNameAsString(pv)) "Should have category name"
            Expect.equal categoryName oa_temperature.NameText "Category name should match"
            let categoryID = Expect.wantSome (PropertyValue.tryGetPropertyIDAsString(pv)) "Should have category ID"
            Expect.equal categoryID oa_temperature.TermAccessionOntobeeUrl "Category ID should match"
            Expect.isNone (PropertyValue.tryGetUnitTextAsString(pv)) "Should have no unit text"
            Expect.isNone (PropertyValue.tryGetUnitCodeAsString(pv)) "Should have no unit code"

            let t' = ArcTable.fromProcesses(tableName1,processes)
            Expect.arcTableEqual t' t "Table should be equal"
        )
        ptestCase "ParamValueUnitizedNoUnit" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_temperature
            let cell = CompositeCell.createUnitized ("5")
            t.AddColumn(CompositeHeader.Input(IOType.Source),[|CompositeCell.createFreeText "Source"|])
            t.AddColumn(header,[|cell|])
            t.AddColumn(CompositeHeader.Output(IOType.Sample),[|CompositeCell.createFreeText "Sample"|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]
            let pvs = LabProcess.getParameterValues p
            Expect.hasLength pvs 1 "Should have 1 parameter value"
            let pv = pvs.[0]
            let categoryName = Expect.wantSome (PropertyValue.tryGetNameAsString(pv)) "Should have category name"
            Expect.equal categoryName oa_temperature.NameText "Category name should match"
            let categoryID = Expect.wantSome (PropertyValue.tryGetPropertyIDAsString(pv)) "Should have category ID"
            Expect.equal categoryID oa_temperature.TermAccessionOntobeeUrl "Category ID should match"
            let value = Expect.wantSome (PropertyValue.tryGetValueAsString(pv)) "Should have value"
            Expect.equal value "5" "Value should match"
            Expect.isNone (PropertyValue.tryGetUnitTextAsString(pv)) "Should have no unit text"
            Expect.isNone (PropertyValue.tryGetUnitCodeAsString(pv)) "Should have no unit code"

            let t' = ArcTable.fromProcesses(tableName1,processes)
            Expect.arcTableEqual t' t "Table should be equal"
        )
        testCase "ParamValueTermEmpty" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_species
            let cell = CompositeCell.createTerm (OntologyAnnotation.create())
            t.AddColumn(CompositeHeader.Input(IOType.Source),[|CompositeCell.createFreeText "Source"|])
            t.AddColumn(header,[|cell|])
            t.AddColumn(CompositeHeader.Output(IOType.Sample),[|CompositeCell.createFreeText "Sample"|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]
            let pvs = LabProcess.getParameterValues p
            Expect.hasLength pvs 1 "Should have 1 parameter value"
            let pv = pvs.[0]
            let categoryName = Expect.wantSome (PropertyValue.tryGetNameAsString(pv)) "Should have category name"
            Expect.equal categoryName oa_species.NameText "Category name should match"
            let categoryID = Expect.wantSome (PropertyValue.tryGetPropertyIDAsString(pv)) "Should have category ID"
            Expect.equal categoryID oa_species.TermAccessionOntobeeUrl "Category ID should match"
            Expect.isNone (PropertyValue.tryGetValueAsString(pv)) "Should have no value"
            Expect.isNone (PropertyValue.tryGetUnitTextAsString(pv)) "Should have no unit text"
            Expect.isNone (PropertyValue.tryGetUnitCodeAsString(pv)) "Should have no unit code"

            let t' = ArcTable.fromProcesses(tableName1,processes)
            Expect.arcTableEqual t' t "Table should be equal"
        )
        testCase "SingleRowIOAndComment GetAndFromProcesses" (fun () ->
            let t = ArcTable.init(tableName1)
            let commentKey = "MyCommentKey"
            let commentValue = "MyCommentValue"
            t.AddColumn(CompositeHeader.Input(IOType.Source),[|CompositeCell.createFreeText "Source"|])
            t.AddColumn(CompositeHeader.Comment(commentKey), [|CompositeCell.createFreeText commentValue|])
            t.AddColumn(CompositeHeader.Output(IOType.Sample),[|CompositeCell.createFreeText "Sample"|])
            let processes = t.GetProcesses()
            Expect.hasLength processes 1 "Should have 1 process"
            let comments = LabProcess.getDisambiguatingDescriptionsAsString(processes.[0])
            Expect.hasLength comments 1 "Should have 1 comment"
            let comment = ARCtrl.Comment.fromString comments.[0]
            Expect.equal comment (ARCtrl.Comment(commentKey,commentValue)) ""
            let table = ArcTable.fromProcesses(tableName1,processes)
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
            let name = LabProcess.getNameAsString p
            Expect.equal name tableName1 "Process name should match table name"
            Expect.isEmpty (LabProcess.getObjects(p)) "Process should have no inputs"
            Expect.isEmpty (LabProcess.getResults(p)) "Process should have no outputs"
            Expect.isEmpty (LabProcess.getParameterValues(p)) "Process should have no parameter values"
            Expect.isNone (LabProcess.tryGetExecutesLabProtocol(p)) "Process should have no protocol"
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
                singleRowSingleParam.Copy() |> fun t -> ArcTable.create(tableName2, t.Headers, t.Values)
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
                
            Expect.equal resultTables.TableCount 2 "2 Tables should have been created"
            Expect.arcTableEqual resultTables.Tables.[0] expectedTables.[0] "Table 1 should be equal"
            Expect.arcTableEqual resultTables.Tables.[1] expectedTables.[1] "Table 2 should be equal"
        )
    ]


//let private tests_ProtocolTransformation =
    
//    let pName = "ProtocolName"
//    let pType = OntologyAnnotation("Growth","ABC","ABC:123")
//    let pVersion = "1.0"
//    let pDescription = "Great Protocol"

//    let pParam1OA = OntologyAnnotation("Temperature","NCIT","NCIT:123")
//    let pParam1 = PropertyValue.createParameterValue(name = "Growth")

//    //LabProtocol.crea

//    testList "Protocol Transformation" [
//        testCase "FromProtocol Empty" (fun () ->
//            let p = LabProtocol.create(id = Identifier.createMissingIdentifier())
//            let t = p |> ArcTable.fromProtocol

//            Expect.equal t.ColumnCount 0 "ColumnCount should be 0"
//            Expect.isTrue (Identifier.isMissingIdentifier t.Name) $"Name should be missing identifier, not \"{t.Name}\""
//        )
//        testCase "FromProtocol SingleParameter" (fun () ->
//            let p = LabProtocol.create(id = Identifier.createMissingIdentifier())(Parameters = [pParam1])
//            let t = p |> ArcTable.fromProtocol

//            Expect.equal t.ColumnCount 1 "ColumnCount should be 1"
//            let c = t.TryGetCellAt(0,0)
//            Expect.isNone c "Cell should not exist"
//        )
//        testCase "FromProtocol SingleProtocolType" (fun () ->
//            let p = Protocol.create(ProtocolType = pType)
//            let t = p |> ArcTable.fromProtocol
//            let expected = CompositeCell.Term pType

//            Expect.equal t.ColumnCount 1 "ColumnCount should be 1"
//            let c = t.TryGetCellAt(0,0)
//            Expect.isSome c "Cell should exist"
//            let c = c.Value
//            Expect.equal c expected "Cell value does not match"
//        )
//        testCase "FromProtocol SingleName" (fun () ->
//            let p = Protocol.create(Name = pName)
//            let t = p |> ArcTable.fromProtocol
//            let expected = CompositeCell.FreeText pName

//            Expect.equal t.ColumnCount 1 "ColumnCount should be 1"
//            let c = t.TryGetCellAt(0,0) 
//            Expect.isSome c "Cell should exist"
//            let c = c.Value
//            Expect.equal c expected "Cell value does not match"
//            )
//        testCase "FromProtocol SingleVersion" (fun () ->
//            let p = Protocol.create(Version = pVersion)
//            let t = p |> ArcTable.fromProtocol
//            let expected = CompositeCell.FreeText pVersion

//            Expect.equal t.ColumnCount 1 "ColumnCount should be 1"
//            let c = t.TryGetCellAt(0,0)
//            Expect.isSome c "Cell should exist"
//            let c = c.Value
//            Expect.equal c expected "Cell value does not match"
//        )
//        testCase "FromProtocol SingleDescription" (fun () ->
//            let p = Protocol.create(Description = pDescription)
//            let t = p |> ArcTable.fromProtocol
//            let expected = CompositeCell.FreeText pDescription

//            Expect.equal t.ColumnCount 1 "ColumnCount should be 1"
//            let c = t.TryGetCellAt(0,0)
//            Expect.isSome c "Cell should exist"
//            let c = c.Value
//            Expect.equal c expected "Cell value does not match"
//        )
//        testCase "GetProtocols NoName" (fun () ->           
//            let t = ArcTable.init "TestTable"
//            let expected = [Protocol.create(Name = "TestTable")]

//            TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocol Name should be ArcTable name."
//        )
//        testCase "GetProtocols SingleName" (fun () ->           
//            let t = ArcTable.init "TestTable"
//            let name = "Name"
//            t.AddProtocolNameColumn([|name|])
//            let expected = [Protocol.create(Name = name)]

//            TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocols do not match"
//        )
//        testCase "GetProtocols SameNames" (fun () ->           
//            let t = ArcTable.init "TestTable"
//            let name = "Name"
//            t.AddProtocolNameColumn([|name;name|])
//            let expected = [Protocol.create(Name = name)]

//            TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocols do not match"
//        )
//        testCase "GetProtocols DifferentNames" (fun () ->           
//            let t = ArcTable.init "TestTable"
//            let name1 = "Name"
//            let name2 = "Name2"
//            t.AddProtocolNameColumn([|name1;name2|])
//            let expected = [Protocol.create(Name = name1);Protocol.create(Name = name2)]

//            TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocols do not match"
//        )
        //testCase "GetProtocols Parameters" (fun () ->
        //    let t = ArcTable.init "TestTable"
        //    let name1 = "Name"
        //    let name2 = "Name2"
        //    t.AddProtocolNameColumn([|name1;name2|])
        //    t.addpara([|pParam1;pParam1|])
        //    let expected = [Protocol.create(Name = name1;Parameters = [pParam1]);Protocol.create(Name = name2;Parameters = [pParam1])]
        //    TestingUtils.Expect.sequenceEqual (t.GetProtocols()) expected "Protocols do not match"
        
        //)


    //]


let main = 
    testList "ArcROCrateConversion" [
        tests_PropertyValue
        tests_ProcessInput
        tests_ProcessOutput
        //tests_ProtocolTransformation
        tests_ArcTableProcess
        tests_ArcTablesProcessSeq
    ]