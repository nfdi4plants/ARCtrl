module ARCtrl.ROCrateConversion.Tests

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.Conversion
open ARCtrl.Process
open TestingUtils
open ARCtrl.FileSystem

module Helper =
    let tableName1 = "Test1"
    let tableName2 = "Test2"
    let oa_species = OntologyAnnotation("species", "GO", "GO:0123456")
    let dt_species = LDDefinedTerm.create(name = "species", termCode = oa_species.TermAccessionOntobeeUrl)
    let oa_chlamy = OntologyAnnotation("Chlamy", "NCBI", "NCBI:0123456")
    let dt_chlamy = LDDefinedTerm.create(name = "Chlamy", termCode = oa_chlamy.TermAccessionOntobeeUrl)
    let oa_instrumentModel = OntologyAnnotation("instrument model", "MS", "MS:0123456")
    let dt_instrumentModel = LDDefinedTerm.create(name = "instrument model", termCode = oa_instrumentModel.TermAccessionOntobeeUrl)
    let oa_SCIEXInstrumentModel = OntologyAnnotation("SCIEX instrument model", "MS", "MS:654321")
    let dt_SCIEXInstrumentModel = LDDefinedTerm.create(name = "SCIEX instrument model", termCode = oa_SCIEXInstrumentModel.TermAccessionOntobeeUrl)
    let oa_time = OntologyAnnotation("time", "UO", "UO:0000010")
    let dt_time = LDDefinedTerm.create(name = "time", termCode = oa_time.TermAccessionOntobeeUrl)
    let oa_hour = OntologyAnnotation("hour", "UO", "UO:0000032")
    let dt_hour = LDDefinedTerm.create(name = "hour", termCode = oa_hour.TermAccessionOntobeeUrl)
    let oa_temperature = OntologyAnnotation("temperature","NCIT","NCIT:0123210")
    let dt_temperature = LDDefinedTerm.create(name = "temperature", termCode = oa_temperature.TermAccessionOntobeeUrl)
    let oa_degreeCel =  OntologyAnnotation("degree celsius","UO","UO:0000027")
    let dt_degreeCel = LDDefinedTerm.create(name = "degree celsius", termCode = oa_degreeCel.TermAccessionOntobeeUrl)

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
            CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "SRSP_Source" 1)
            CompositeColumn.create(CompositeHeader.Parameter oa_species, createCells_chlamy 1)
            CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "SRSP_Sample" 1)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    let singleRowOutputSource = 
    /// Input [Source] --> Source_0 .. Source_4
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "SROS_Source" 1)
            CompositeColumn.create(CompositeHeader.Output IOType.Source, createCells_FreeText "SROS_Sample" 1)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    let singleRowMixedValues = 
    /// Input [Source] --> Source_0 .. Source_4
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "SMR_Source" 1)
            CompositeColumn.create(CompositeHeader.Parameter oa_time, createCells_Hour 1)
            CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_chlamy 1)
            CompositeColumn.create(CompositeHeader.Factor oa_temperature, createCells_DegreeCelsius 1)
            CompositeColumn.create(CompositeHeader.Component oa_instrumentModel, createCells_Sciex 1)
            CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "SMR_Sample" 1)
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
            CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "TRSPV_Source" 2)
            CompositeColumn.create(CompositeHeader.Parameter oa_species, createCells_chlamy 2)
            CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "TRSPV_Sample" 2)
            |]
        let t = ArcTable.init(tableName1)
        t.AddColumns(columns)
        t

    let twoRowsDifferentParamValue = 
        let columns = 
            [|
            CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "TRDPV_Source" 2)
            CompositeColumn.create(CompositeHeader.Parameter oa_time, createCells_Hour 2)
            CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "TRDPV_Sample" 2)
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


    let create_dataRow (fileName : string) (row : int) =
        let name = $"{fileName}#row={row}"
        let format = "text/csv"
        let selectorFormat = "MySelector"
        Data(
            name = name,
            format = format,
            selectorFormat = selectorFormat
        )

    let create_dataContextOfRow (fileName : string) (row : int) =
        let explication = OntologyAnnotation.create(name = "MyExplication", tsr = "FO", tan = "FO:123")
        let unit = OntologyAnnotation.create(name = "MyUnit", tsr = "UO", tan = "UO:456")
        let objectType = OntologyAnnotation.create(name = "float", tsr = "TO", tan = "TO:987")
        let generatedBy = "MyGeneratedBy"
        let description = "MyDescription"
        let label = "MyLabel"
        let name : URI = $"{fileName}#row={row}"
        let format = "text/csv"
        let selectorFormat = "MySelector"
        DataContext(
            //id = name,
            name = name,
            format = format,
            selectorFormat = selectorFormat,
            explication = explication,
            objectType = objectType,
            unit = unit,
            generatedBy = generatedBy,
            description = description,
            label = label
        )

    let create_assay_full () =
        let title = "My Assay Title"
        let description = "My Assay Description"
        let measurementType = OntologyAnnotation(name = "sugar measurement", tsr = "DPBO", tan = "DPBO:0000120")
        let technologyType = OntologyAnnotation(name = "Photometry", tsr = "NCIT", tan = "NCIT:C65109")
        let technologyPlatform = OntologyAnnotation(name = "Infinite M200 plate reader (Tecan)", tsr = "DPBO", tan = "DPBO:0000116")
        let person =
            let role = OntologyAnnotation(name = "Resarcher", tsr = "PO", tan = "PO:123")
            ARCtrl.Person(orcid = "0000-0002-1825-0097", firstName = "John", lastName = "Doe", midInitials = "BD", email = "jd@email.com", phone = "123", fax = "456", address = "123 Main St", affiliation = "My University",roles = ResizeArray [role])
        let table2 = ArcTable.fromArcTableValues("Table2", twoRowsDifferentParamValue.Headers, twoRowsDifferentParamValue.Values)
        let p =
            ArcAssay(
                identifier = "My Assay",
                title = title,
                description = description,
                measurementType = measurementType,
                technologyType = technologyType,
                technologyPlatform = technologyPlatform,
                performers = ResizeArray [person],
                tables = ResizeArray [singleRowMixedValues; table2]
            )
        p

    let create_study_full ()  =
        let publication =
            let authors = "Lukas Weil, John Doe"
            let comment = Comment("MyCommentKey","MyCommentValue")
            let commentOnlyKey = Comment("MyEmptyKey")
            let status = OntologyAnnotation(name = "Published", tsr = "SO", tan = "SO:123")
            ARCtrl.Publication.create(title = "My Paper", doi = "10.1234/5678", authors = authors, status = status, comments = ResizeArray [comment; commentOnlyKey])
        let person =
            let role = OntologyAnnotation(name = "Resarcher", tsr = "PO", tan = "PO:123")
            ARCtrl.Person(orcid = "0000-0002-1825-0097", firstName = "John", lastName = "Doe", midInitials = "BD", email = "jd@email.com", phone = "123", fax = "456", address = "123 Main St", affiliation = "My University",roles = ResizeArray [role])
        let comment = Comment("MyCommentKey","MyCommentValue")
        let table2 = ArcTable.fromArcTableValues("Table2", twoRowsDifferentParamValue.Headers, twoRowsDifferentParamValue.Values)
        let p = ArcStudy(
            identifier = "My Study",
            title = "My Best Study",
            description = "My Description is very good and such",
            publicReleaseDate = DateTime.toString System.DateTime.Now,
            submissionDate = DateTime.toString System.DateTime.Now,
            publications = ResizeArray [publication],
            contacts = ResizeArray [person],
            tables = ResizeArray [singleRowMixedValues; table2],
            comments = ResizeArray [comment]
            )
        p

    let create_investigation_toplevel() =
        let publication =
            let authors = "Lukas Weil, John Doe"
            let comment = Comment("MyCommentKey","MyCommentValue")
            let commentOnlyKey = Comment("MyEmptyKey")
            let status = OntologyAnnotation(name = "Published", tsr = "SO", tan = "SO:123")
            ARCtrl.Publication.create(title = "My Paper", doi = "10.1234/5678", authors = authors, status = status, comments = ResizeArray [comment; commentOnlyKey])
        let person =
            let role = OntologyAnnotation(name = "Resarcher", tsr = "PO", tan = "PO:123")
            ARCtrl.Person(orcid = "0000-0002-1825-0097", firstName = "John", lastName = "Doe", midInitials = "BD", email = "jd@email.com", phone = "123", fax = "456", address = "123 Main St", affiliation = "My University",roles = ResizeArray [role])
        let comment = Comment("MyCommentKey","MyCommentValue")
        let p = ArcInvestigation(
            identifier = "My Investigation",
            title = "My Best Investigation",
            description = "My Description is very good and such",
            publicReleaseDate = DateTime.toString System.DateTime.Now,
            submissionDate = DateTime.toString System.DateTime.Now,
            publications = ResizeArray [publication],
            contacts = ResizeArray [person],
            comments = ResizeArray [comment]
            )
        p

open Helper


let tests_DefinedTerm =
    testList "DefinedTerm_Full" [
        let oa = OntologyAnnotation("species", "GO", "GO:0123456")
        let dt = BaseTypes.composeDefinedTerm oa
        let oa' = BaseTypes.decomposeDefinedTerm dt
        Expect.equal oa' oa "OntologyAnnotation should match"
    ]

let private tests_ProcessInput =
    testList "ProcessInput" [
        testCase "Source" (fun () ->
            let header = CompositeHeader.Input(IOType.Source)
            let cell = CompositeCell.createFreeText "MySource"
            let input = BaseTypes.composeProcessInput header cell None

            Expect.isTrue (LDSample.validateSource input) "Should be a valid source"
            let name = LDSample.getNameAsString input
            Expect.equal name "MySource" "Name should match"

            let header',cell' = BaseTypes.decomposeProcessInput input
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Sample" (fun () ->
            let header = CompositeHeader.Input(IOType.Sample)
            let cell = CompositeCell.createFreeText "MySample"
            let input = BaseTypes.composeProcessInput header cell None

            Expect.isTrue (LDSample.validateSample input) "Should be a valid sample"
            let name = LDSample.getNameAsString input
            Expect.equal name "MySample" "Name should match"

            let header',cell' = BaseTypes.decomposeProcessInput input
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Data_Data" (fun () ->
            let header = CompositeHeader.Input(IOType.Data)
            let data = Data(name = "MyData", format = "text/csv", selectorFormat = "MySelector")
            let cell = CompositeCell.createData data
            let input = BaseTypes.composeProcessInput header cell None
    
            Expect.isTrue (LDFile.validate input) "Should be a valid data"
            let name = LDFile.getNameAsString input
            Expect.equal name "MyData" "Name should match"
    
            let header',cell' = BaseTypes.decomposeProcessInput input
            Expect.equal header header' "Header should match"
            //data.ID <- Some input.Id
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Data_Freetext" (fun () ->
            let header = CompositeHeader.Input(IOType.Data)
            let cell = CompositeCell.createFreeText "MyData"
            let input = BaseTypes.composeProcessInput header cell None
    
            Expect.isTrue (LDFile.validate input) "Should be a valid data"
            let name = LDFile.getNameAsString input
            Expect.equal name "MyData" "Name should match"
    
            let header',cell' = BaseTypes.decomposeProcessInput input
            let expectedCell = CompositeCell.createData (Data ((*id = input.Id, *)name = "MyData"))
            Expect.equal header header' "Header should match"
            Expect.equal expectedCell cell' "Cell should match"
        )
        testCase "Material" (fun () ->
            let header = CompositeHeader.Input(IOType.Material)
            let cell = CompositeCell.createFreeText "MyMaterial"
            let input = BaseTypes.composeProcessInput header cell None
    
            Expect.isTrue (LDSample.validateMaterial input) "Should be a valid material"
            let name = LDSample.getNameAsString input
            Expect.equal name "MyMaterial" "Name should match"
    
            let header',cell' = BaseTypes.decomposeProcessInput input
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "FreeType" (fun () ->
            let header = CompositeHeader.Input (IOType.FreeText "MyInputType")
            let cell = CompositeCell.createFreeText "MyFreeText"
            let input = BaseTypes.composeProcessInput header cell None

            let name = LDSample.getNameAsString input
            Expect.equal name "MyFreeText" "Name should match"

            let schemaType = input.SchemaType
            Expect.hasLength schemaType 1 "Should have 1 schema type"
            Expect.equal schemaType.[0] "MyInputType" "Schema type should be FreeText"

            let header',cell' = BaseTypes.decomposeProcessInput input
            Expect.equal header header' "Header should match"
        )
    ]

let private tests_ProcessOutput =
    testList "ProcessOutput" [
        testCase "Sample" (fun () ->
            let header = CompositeHeader.Output(IOType.Sample)
            let cell = CompositeCell.createFreeText "MySample"
            let output = BaseTypes.composeProcessOutput header cell None

            Expect.isTrue (LDSample.validateSample output) "Should be a valid sample"
            let name = LDSample.getNameAsString output
            Expect.equal name "MySample" "Name should match"

            let header',cell' = BaseTypes.decomposeProcessOutput output
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Data_Data" (fun () ->
            let header = CompositeHeader.Output(IOType.Data)
            let data = Data(name = "MyData", format = "text/csv", selectorFormat = "MySelector")
            let cell = CompositeCell.createData data
            let output = BaseTypes.composeProcessOutput header cell None
    
            Expect.isTrue (LDFile.validate output) "Should be a valid data"
            let name = LDFile.getNameAsString output
            Expect.equal name "MyData" "Name should match"
    
            let header',cell' = BaseTypes.decomposeProcessOutput output
            Expect.equal header header' "Header should match"
            //data.ID <- Some output.Id
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Data_Freetext" (fun () ->
            let header = CompositeHeader.Output(IOType.Data)
            let cell = CompositeCell.createFreeText "MyData"
            let output = BaseTypes.composeProcessOutput header cell None
    
            Expect.isTrue (LDFile.validate output) "Should be a valid data"
            let name = LDFile.getNameAsString output
            Expect.equal name "MyData" "Name should match"
    
            let header',cell' = BaseTypes.decomposeProcessOutput output
            let expectedCell = CompositeCell.createData (Data ((*id = output.Id, *)name = "MyData"))
            Expect.equal header header' "Header should match"
            Expect.equal expectedCell cell' "Cell should match"
        )
        testCase "Material" (fun () ->
            let header = CompositeHeader.Output(IOType.Material)
            let cell = CompositeCell.createFreeText "MyMaterial"
            let output = BaseTypes.composeProcessOutput header cell None
    
            Expect.isTrue (LDSample.validateMaterial output) "Should be a valid material"
            let name = LDSample.getNameAsString output
            Expect.equal name "MyMaterial" "Name should match"
    
            let header',cell' = BaseTypes.decomposeProcessOutput output
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "FreeType" (fun () ->
            let header = CompositeHeader.Output (IOType.FreeText "MyOutputType")
            let cell = CompositeCell.createFreeText "MyFreeText"
            let output = BaseTypes.composeProcessOutput header cell None

            let name = LDSample.getNameAsString output
            Expect.equal name "MyFreeText" "Name should match"

            let schemaType = output.SchemaType
            Expect.hasLength schemaType 1 "Should have 1 schema type"
            Expect.equal schemaType.[0] "MyOutputType" "Schema type should be FreeText"

            let header',cell' = BaseTypes.decomposeProcessOutput output
            Expect.equal header header' "Header should match"
        )
    ]

let private tests_PropertyValue =
    testList "PropertyValue" [
        testCase "Characteristic_Ontology" (fun () ->
            let header = CompositeHeader.Characteristic oa_species
            let cell = CompositeCell.createTerm oa_chlamy
            let pv = BaseTypes.composeCharacteristicValue header cell

            let name = LDPropertyValue.getNameAsString pv
            Expect.equal name oa_species.NameText "Name should match"

            let value = LDPropertyValue.getValueAsString pv
            Expect.equal value oa_chlamy.NameText "Value should match"

            let propertyID = Expect.wantSome (LDPropertyValue.tryGetPropertyIDAsString pv) "Should have property ID"
            Expect.equal propertyID oa_species.TermAccessionOntobeeUrl "Property ID should match"

            let valueReference = Expect.wantSome (LDPropertyValue.tryGetValueReferenceAsString pv) "Should have value reference"
            Expect.equal valueReference oa_chlamy.TermAccessionOntobeeUrl "Value reference should match"

            let header',cell' = BaseTypes.decomposeCharacteristicValue pv
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Parameter_Unitized" (fun () ->
            let header = CompositeHeader.Parameter oa_temperature
            let cell = CompositeCell.createUnitized ("5",oa_degreeCel)
            let pv = BaseTypes.composeParameterValue header cell

            let name = LDPropertyValue.getNameAsString pv
            Expect.equal name oa_temperature.NameText "Name should match"

            let value = LDPropertyValue.getValueAsString pv
            Expect.equal value "5" "Value should match"

            let propertyID = Expect.wantSome (LDPropertyValue.tryGetPropertyIDAsString pv) "Should have property ID"
            Expect.equal propertyID oa_temperature.TermAccessionOntobeeUrl "Property ID should match"

            let unit = Expect.wantSome (LDPropertyValue.tryGetUnitTextAsString pv) "Should have unit"
            Expect.equal unit oa_degreeCel.NameText "Unit should match"

            let unitCode = Expect.wantSome (LDPropertyValue.tryGetUnitCodeAsString pv) "Should have unit code"
            Expect.equal unitCode oa_degreeCel.TermAccessionOntobeeUrl "Unit code should match"

            let header',cell' = BaseTypes.decomposeParameterValue pv
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Parameter_Unitized_Valueless" (fun () ->
            let header = CompositeHeader.Parameter oa_temperature
            let cell = CompositeCell.createUnitized ("",oa_degreeCel)
            let pv = BaseTypes.composeParameterValue header cell

            let name = LDPropertyValue.getNameAsString pv
            Expect.equal name oa_temperature.NameText "Name should match"

            Expect.isNone (LDPropertyValue.tryGetValueAsString pv) "Should not have value"

            let propertyID = Expect.wantSome (LDPropertyValue.tryGetPropertyIDAsString pv) "Should have property ID"
            Expect.equal propertyID oa_temperature.TermAccessionOntobeeUrl "Property ID should match"

            let unit = Expect.wantSome (LDPropertyValue.tryGetUnitTextAsString pv) "Should have unit"
            Expect.equal unit oa_degreeCel.NameText "Unit should match"

            let unitCode = Expect.wantSome (LDPropertyValue.tryGetUnitCodeAsString pv) "Should have unit code"
            Expect.equal unitCode oa_degreeCel.TermAccessionOntobeeUrl "Unit code should match"

            let header',cell' = BaseTypes.decomposeParameterValue pv
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "FactorValue_Valueless" (fun () ->
            let header = CompositeHeader.Factor oa_temperature
            let cell = CompositeCell.createTerm (OntologyAnnotation.create())
            let pv = BaseTypes.composeFactorValue header cell

            let name = LDPropertyValue.getNameAsString pv
            Expect.equal name oa_temperature.NameText "Name should match"

            let propertyID = Expect.wantSome (LDPropertyValue.tryGetPropertyIDAsString pv) "Should have property ID"
            Expect.equal propertyID oa_temperature.TermAccessionOntobeeUrl "Property ID should match"

            Expect.isNone (LDPropertyValue.tryGetValueAsString pv) "Should not have value"

            let header',cell' = BaseTypes.decomposeFactorValue pv
            Expect.equal header header' "Header should match"
            Expect.equal cell cell' "Cell should match"
        )
        testCase "Component_Freetexts" (fun () ->
            let header = CompositeHeader.Component (OntologyAnnotation.create(name = "MyComponentHeader"))
            let cell = CompositeCell.createTerm (OntologyAnnotation.create(name = "MyComponentValue"))
            let pv = BaseTypes.composeComponent header cell

            let name = LDPropertyValue.getNameAsString pv
            Expect.equal name "MyComponentHeader" "Name should match"

            let value = LDPropertyValue.getValueAsString pv
            Expect.equal value "MyComponentValue" "Value should match"

            Expect.isNone (LDPropertyValue.tryGetPropertyIDAsString pv) "Should not have property ID"
            Expect.isNone (LDPropertyValue.tryGetValueReferenceAsString pv) "Should not have value reference"
            Expect.isNone (LDPropertyValue.tryGetUnitTextAsString pv) "Should not have unit"
            Expect.isNone (LDPropertyValue.tryGetUnitCodeAsString pv) "Should not have unit code"

            let header',cell' = BaseTypes.decomposeComponent pv
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
                LDPropertyValue.createParameterValue(
                    name = oa_species.NameText,
                    value = oa_chlamy.NameText,
                    propertyID = oa_species.TermAccessionOntobeeUrl,
                    valueReference = oa_chlamy.TermAccessionOntobeeUrl
                )
            ColumnIndex.setIndex expectedPPV 0
            let expectedInput = LDSample.createSource(name = "SRSP_Source_0")
            let expectedOutput = LDSample.createSample(name = "SRSP_Sample_0")
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            let paramValues = LDLabProcess.getParameterValues(p)
            Expect.equal paramValues.Count 1 "Process should have 1 parameter values"
            Expect.equal paramValues.[0] expectedPPV "Param value does not match"
            let inputs = LDLabProcess.getObjects(p)
            Expect.equal inputs.Count 1 "Process should have 1 input"
            Expect.equal inputs.[0] expectedInput "Input value does not match"
            let outputs = LDLabProcess.getResults(p)
            Expect.equal outputs.Count 1 "Process should have 1 output"
            Expect.equal outputs.[0] expectedOutput "Output value does not match"
            let name = LDLabProcess.getNameAsString p
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
            let expectedInput = LDSample.createSource(name = "SROS_Source_0")
            let expectedOutput = LDSample.createSample(name = "SROS_Sample_0")
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            let inputs = LDLabProcess.getObjects(p)
            Expect.equal inputs.Count 1 "Process should have 1 input"
            Expect.equal inputs.[0] expectedInput "Input value does not match"
            let outputs = LDLabProcess.getResults(p)
            Expect.equal outputs.Count 1 "Process should have 1 output"
            Expect.equal outputs.[0] expectedOutput "Output value does not match"
            let name = LDLabProcess.getNameAsString p
            Expect.equal name tableName1 "Process name should match table name"
        )

        testCase "SingleRowMixedValues GetProcesses" (fun () ->          
            let t = singleRowMixedValues.Copy()          
            let processes = t.GetProcesses()           
            Expect.equal processes.Length 1 "Should have 1 process"
            Expect.equal (LDLabProcess.getParameterValues processes.[0]).Count 1 "Process should have 1 parameter values"
            let input = Expect.wantSome ((LDLabProcess.getObjects processes.[0]) |> Seq.tryExactlyOne) "Process should have 1 input"
            let output = Expect.wantSome ((LDLabProcess.getResults processes.[0]) |> Seq.tryExactlyOne) "Process should have 1 output"
            let charas = LDSample.getCharacteristics input
            Expect.hasLength charas 1 "Input should have the charactersitic"
            let factors = LDSample.getFactors output
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
            let inputs = LDLabProcess.getObjects(p)
            Expect.equal inputs.Count 1 "Process should have 1 input"
            let outputs = LDLabProcess.getResults(p)
            Expect.equal outputs.Count 1 "Process should have 1 output"
            Expect.isTrue (LDFile.validate(inputs.[0])) "First input should be data"
            let charas = LDSample.getCharacteristics(inputs.[0])
            Expect.hasLength charas 1 "Input should have the charactersitic"
        )

        testCase "SingleRowDataInputWithCharacteristic GetAndFromProcesses" (fun () ->
            let t = singleRowDataInputWithCharacteristic.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses(tableName1,processes)
            let expectedTable = t
            let expectedInputData = CompositeCell.Data(Data((*id = "RData_0", *)name = "RData_0"))
            let expectedOutputData = CompositeCell.Data(Data((*id = "DData_0", *)name = "DData_0"))
            expectedTable.SetCellAt(0,0,expectedInputData)
            expectedTable.SetCellAt(t.ColumnCount - 1,0,expectedOutputData)
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )

        testCase "SingleRowDataOutputWithFactor GetProcesses" (fun () ->
            let t = singleRowDataOutputWithFactor.Copy()
            let processes = t.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let p = processes.[0]
            let inputs = LDLabProcess.getObjects(p)
            Expect.equal inputs.Count 1 "Process should have 1 input"
            Expect.isTrue (LDFile.validate inputs.[0]) "input should be data"
            let outputs = LDLabProcess.getResults(p)
            Expect.equal outputs.Count 1 "Process should have 1 output"
            Expect.isTrue (LDFile.validate outputs.[0]) "output should be data"
            let factors = LDSample.getFactors(outputs.[0])
            Expect.hasLength factors 1 "output should have 1 factor value"
        )

        testCase "SingleRowDataOutputWithFactor GetAndFromProcesses" (fun () ->
            let t = singleRowDataOutputWithFactor.Copy()
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses(tableName1,processes)
            let expectedTable = t
            let expectedInputData = CompositeCell.Data(Data((*id = "RData_0", *)name = "RData_0"))
            let expectedOutputData = CompositeCell.Data(Data((*id = "DData_0", *)name = "DData_0"))
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
            let prot = Expect.wantSome (LDLabProcess.tryGetExecutesLabProtocol(p)) "Process should have protocol"
            let protName = Expect.wantSome (LDLabProtocol.tryGetNameAsString(prot)) "Protocol should have name"
            Expect.equal protName "MyProtocol" "Protocol name should match"
            let pName = Expect.wantSome (LDLabProcess.tryGetNameAsString(p)) "Process should have name"
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
            let name = LDLabProcess.getNameAsString p
            Expect.equal name tableName1 "Process name should match table name"
            Expect.isEmpty (LDLabProcess.getObjects(p)) "Process should have no inputs"
            Expect.isEmpty (LDLabProcess.getResults(p)) "Process should have no outputs"
            Expect.isEmpty (LDLabProcess.getParameterValues(p)) "Process should have no parameter values"
            Expect.isNone (LDLabProcess.tryGetExecutesLabProtocol(p)) "Process should have no protocol"
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
            t.AddColumn(CompositeHeader.Input(IOType.Source),ResizeArray [|CompositeCell.createFreeText "Source"|])
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses(tableName1,processes)
            Expect.arcTableEqual table t "Table should be equal"
        )
        testCase "ParamValueUnitizedNoValueNoInputOutput" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_temperature
            let unit = oa_degreeCel
            let cell = CompositeCell.createUnitized ("",unit)
            t.AddColumn(header,ResizeArray [|cell|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]
            let pvs = LDLabProcess.getParameterValues p
            Expect.hasLength pvs 1 "Should have 1 parameter value"
            let pv = pvs.[0]
            Expect.isNone (LDPropertyValue.tryGetValueAsString(pv)) "Should have no value"
            let categoryName = Expect.wantSome (LDPropertyValue.tryGetNameAsString(pv)) "Should have category name"
            Expect.equal categoryName oa_temperature.NameText "Category name should match"
            let categoryID = Expect.wantSome (LDPropertyValue.tryGetPropertyIDAsString(pv)) "Should have category ID"
            Expect.equal categoryID oa_temperature.TermAccessionOntobeeUrl "Category ID should match"
            let unitName = Expect.wantSome (LDPropertyValue.tryGetUnitTextAsString(pv)) "Should have unit name"
            Expect.equal unitName oa_degreeCel.NameText "Unit name should match"
            let unitCode = Expect.wantSome (LDPropertyValue.tryGetUnitCodeAsString(pv)) "Should have unit code"
            Expect.equal unitCode oa_degreeCel.TermAccessionOntobeeUrl "Unit code should match"

            let t' = ArcTable.fromProcesses(tableName1,processes)
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

            let pvs = LDLabProcess.getParameterValues p
            Expect.hasLength pvs 1 "Should have 1 parameter value"
            let pv = pvs.[0]
            Expect.isNone (LDPropertyValue.tryGetValueAsString(pv)) "Should have no value"
            let categoryName = Expect.wantSome (LDPropertyValue.tryGetNameAsString(pv)) "Should have category name"
            Expect.equal categoryName oa_temperature.NameText "Category name should match"
            let categoryID = Expect.wantSome (LDPropertyValue.tryGetPropertyIDAsString(pv)) "Should have category ID"
            Expect.equal categoryID oa_temperature.TermAccessionOntobeeUrl "Category ID should match"
            let unitName = Expect.wantSome (LDPropertyValue.tryGetUnitTextAsString(pv)) "Should have unit name"
            Expect.equal unitName oa_degreeCel.NameText "Unit name should match"
            let unitCode = Expect.wantSome (LDPropertyValue.tryGetUnitCodeAsString(pv)) "Should have unit code"
            Expect.equal unitCode oa_degreeCel.TermAccessionOntobeeUrl "Unit code should match"

            let t' = ArcTable.fromProcesses(tableName1,processes)
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
            let pvs = LDLabProcess.getParameterValues p
            Expect.hasLength pvs 1 "Should have 1 parameter value"
            let pv = pvs.[0]
            Expect.isNone (LDPropertyValue.tryGetValueAsString(pv)) "Should have no value"
            let categoryName = Expect.wantSome (LDPropertyValue.tryGetNameAsString(pv)) "Should have category name"
            Expect.equal categoryName oa_temperature.NameText "Category name should match"
            let categoryID = Expect.wantSome (LDPropertyValue.tryGetPropertyIDAsString(pv)) "Should have category ID"
            Expect.equal categoryID oa_temperature.TermAccessionOntobeeUrl "Category ID should match"
            Expect.isNone (LDPropertyValue.tryGetUnitTextAsString(pv)) "Should have no unit text"
            Expect.isNone (LDPropertyValue.tryGetUnitCodeAsString(pv)) "Should have no unit code"

            let t' = ArcTable.fromProcesses(tableName1,processes)
            Expect.arcTableEqual t' t "Table should be equal"
        )
        ptestCase "ParamValueUnitizedNoUnit" (fun () ->
            let t = ArcTable.init tableName1
            let header = CompositeHeader.Parameter oa_temperature
            let cell = CompositeCell.createUnitized ("5")
            t.AddColumn(CompositeHeader.Input(IOType.Source),ResizeArray [|CompositeCell.createFreeText "Source"|])
            t.AddColumn(header,ResizeArray [|cell|])
            t.AddColumn(CompositeHeader.Output(IOType.Sample),ResizeArray [|CompositeCell.createFreeText "Sample"|])

            let processes = t.GetProcesses()
            Expect.equal 1 processes.Length "Should have 1 process"
            let p = processes.[0]
            let pvs = LDLabProcess.getParameterValues p
            Expect.hasLength pvs 1 "Should have 1 parameter value"
            let pv = pvs.[0]
            let categoryName = Expect.wantSome (LDPropertyValue.tryGetNameAsString(pv)) "Should have category name"
            Expect.equal categoryName oa_temperature.NameText "Category name should match"
            let categoryID = Expect.wantSome (LDPropertyValue.tryGetPropertyIDAsString(pv)) "Should have category ID"
            Expect.equal categoryID oa_temperature.TermAccessionOntobeeUrl "Category ID should match"
            let value = Expect.wantSome (LDPropertyValue.tryGetValueAsString(pv)) "Should have value"
            Expect.equal value "5" "Value should match"
            Expect.isNone (LDPropertyValue.tryGetUnitTextAsString(pv)) "Should have no unit text"
            Expect.isNone (LDPropertyValue.tryGetUnitCodeAsString(pv)) "Should have no unit code"

            let t' = ArcTable.fromProcesses(tableName1,processes)
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
            let pvs = LDLabProcess.getParameterValues p
            Expect.hasLength pvs 1 "Should have 1 parameter value"
            let pv = pvs.[0]
            let categoryName = Expect.wantSome (LDPropertyValue.tryGetNameAsString(pv)) "Should have category name"
            Expect.equal categoryName oa_species.NameText "Category name should match"
            let categoryID = Expect.wantSome (LDPropertyValue.tryGetPropertyIDAsString(pv)) "Should have category ID"
            Expect.equal categoryID oa_species.TermAccessionOntobeeUrl "Category ID should match"
            Expect.isNone (LDPropertyValue.tryGetValueAsString(pv)) "Should have no value"
            Expect.isNone (LDPropertyValue.tryGetUnitTextAsString(pv)) "Should have no unit text"
            Expect.isNone (LDPropertyValue.tryGetUnitCodeAsString(pv)) "Should have no unit code"

            let t' = ArcTable.fromProcesses(tableName1,processes)
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
            Expect.hasLength processes 1 "Should have 1 process"
            let comments = LDLabProcess.getDisambiguatingDescriptionsAsString(processes.[0])
            Expect.hasLength comments 1 "Should have 1 comment"
            let comment = Comment.fromString comments.[0]
            Expect.equal comment (Comment(commentKey,commentValue)) ""
            let table = ArcTable.fromProcesses(tableName1,processes)
            let expectedTable = t
            Expect.arcTableEqual table expectedTable "Table should be equal"
        )
    ]

let private tests_Data =
    let files = [|
        "assays/MyAssay/dataset/ABC.D/SubFile.txt"
        "assays/MyAssay/dataset/ABC.D/SubFolder/SubSubFile.txt"
    |]
    let fs = FileSystem.fromFilePaths files
    testList "Data" [
        testCase "Basic_NotInFs" (fun () ->
            let path = "assays/MyAssay/dataset/MyData.csv"
            let data = Data(name = path)
            let f = BaseTypes.composeFile(data, fs = fs)
            Expect.sequenceEqual f.SchemaType [LDFile.schemaType] "Type should match"
            Expect.equal f.Id path "ID should match"
            let name = Expect.wantSome (LDFile.tryGetNameAsString f) "Should have name"
            Expect.equal name path "Name should match"
        )
        testCase "Basic_InFs" (fun () ->
            let path = "assays/MyAssay/dataset/ABC.D/SubFile.txt"
            let data = Data(name = path)
            let f = BaseTypes.composeFile(data, fs = fs)
            Expect.sequenceEqual f.SchemaType [LDFile.schemaType] "Type should match"
            Expect.equal f.Id path "ID should match"
            let name = Expect.wantSome (LDFile.tryGetNameAsString f) "Should have name"
            Expect.equal name path "Name should match"
        )
        testCase "WithFormatAndSelector" (fun () ->
            let path = "assays/MyAssay/dataset/MyData.csv"
            let data = Data(name = path, format = "text/csv", selectorFormat = "MySelector")
            let f = BaseTypes.composeFile(data, fs = fs)
            Expect.sequenceEqual f.SchemaType [LDFile.schemaType] "Type should match"
            Expect.equal f.Id path "ID should match"
            let name = Expect.wantSome (LDFile.tryGetNameAsString f) "Should have name"
            Expect.equal name path "Name should match"
            let format = Expect.wantSome (LDFile.tryGetEncodingFormatAsString f) "Should have format"
            Expect.equal format "text/csv" "Format should match"
            let selectorFormat = Expect.wantSome (LDFile.tryGetUsageInfoAsString f) "Should have selector format"
            Expect.equal selectorFormat "MySelector" "Selector format should match"
        )
        testCase "IsFolder" (fun () ->
            let path = "assays/MyAssay/dataset/ABC.D"
            let data = Data(name = path)
            let f = BaseTypes.composeFile(data, fs = fs)
            Expect.sequenceEqual f.SchemaType [LDFile.schemaType; LDDataset.schemaType] "Type should match"
            Expect.equal f.Id path "ID should match"
            let name = Expect.wantSome (LDFile.tryGetNameAsString f) "Should have name"
            Expect.equal name path "Name should match"
            let hasParts = LDDataset.getHasParts f
            Expect.hasLength hasParts 2 "Should have 2 parts"
            Expect.equal hasParts.[0].Id "assays/MyAssay/dataset/ABC.D/SubFile.txt" "First part Id should match"
            Expect.sequenceEqual hasParts.[0].SchemaType [LDFile.schemaType] "First part type should match"
            Expect.equal hasParts.[1].Id "assays/MyAssay/dataset/ABC.D/SubFolder/SubSubFile.txt" "Second part Id should match"
            Expect.sequenceEqual hasParts.[1].SchemaType [LDFile.schemaType] "Second part type should match"
        )



    ]

let private tests_DataContext =
    testList "DataContext" [
        testCase "Empty" (fun () ->
            let dc = DataContext()
            let f () = 
                let fd = BaseTypes.composeFragmentDescriptor(dc)
                let dc' = BaseTypes.decomposeFragmentDescriptor(fd)
                ()
            Expect.throws f "Should throw"
            //Expect.equal dc dc' "Data context should match"
        )
        testCase "OnlyName" (fun () ->
            let dc = DataContext(name = "MyFile")
            let fd = BaseTypes.composeFragmentDescriptor(dc)
            Expect.sequenceEqual (fd.GetPropertyNames()) [|LDPropertyValue.name; LDPropertyValue.propertyID; LDPropertyValue.subjectOf|] "Should have only name property"
            let dc' = BaseTypes.decomposeFragmentDescriptor(fd)
            Expect.equal dc dc' "Data context should match"
        )
        testCase "UnitOnlyName" (fun () ->
            let myUnit = OntologyAnnotation(name = "MyUnit")
            let dc = DataContext(name = "MyFile", unit = myUnit)
            let fd = BaseTypes.composeFragmentDescriptor(dc)
            Expect.sequenceEqual (fd.GetPropertyNames()) [|LDPropertyValue.name; LDPropertyValue.propertyID; LDPropertyValue.unitText; LDPropertyValue.subjectOf|] "Should have only name property"
            let dc' = BaseTypes.decomposeFragmentDescriptor(fd)
            Expect.equal dc dc' "Data context should match"
        )
        testCase "Full" (fun () ->
            let dc = create_dataContextOfRow "MyFile" 1
            let fd = BaseTypes.composeFragmentDescriptor(dc)
            let dc' = BaseTypes.decomposeFragmentDescriptor(fd)
            Expect.equal dc dc' "Data context should match"


        )
    ]

let private tests_DataMap =
    testList "DataMap" [
        testCase "Empty" (fun () ->
            let dm = DataMap.init()
            let fds = DatamapConversion.composeFragmentDescriptors dm
            let dm' = DatamapConversion.decomposeFragmentDescriptors fds
            Expect.equal dm dm' "Data map should match"
        )
        testCase "SingleEntry" (fun () ->
            let dc = create_dataContextOfRow "MyFile" 1
            let dm = DataMap(ResizeArray[dc])
            let fds = DatamapConversion.composeFragmentDescriptors dm
            let dm' = DatamapConversion.decomposeFragmentDescriptors fds
            Expect.equal dm dm' "Data map should match"
        )
        testCase "MultipleEntries" (fun () ->
            let dc1 = create_dataContextOfRow "MyFile" 1
            let dc2 = create_dataContextOfRow "MyFile" 2
            let dm = DataMap(ResizeArray[dc1;dc2])
            let fds = DatamapConversion.composeFragmentDescriptors dm
            let dm' = DatamapConversion.decomposeFragmentDescriptors fds
            Expect.equal dm dm' "Data map should match"
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
            let name = LDLabProcess.getNameAsString p
            Expect.equal name tableName1 "Process name should match table name"
            Expect.isEmpty (LDLabProcess.getObjects(p)) "Process should have no inputs"
            Expect.isEmpty (LDLabProcess.getResults(p)) "Process should have no outputs"
            Expect.isEmpty (LDLabProcess.getParameterValues(p)) "Process should have no parameter values"
            Expect.isNone (LDLabProcess.tryGetExecutesLabProtocol(p)) "Process should have no protocol"
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
            t1.AddColumn(
                CompositeHeader.Output IOType.Sample,
                ResizeArray [|CompositeCell.createFreeText "MyOutSample"|]
            )
            let t2 = ArcTable.init "Test_Tisch"
            t2.AddColumn(
                CompositeHeader.Input(IOType.Sample),
                ResizeArray [|CompositeCell.createFreeText "MeinSample"|]
            )
            t2.AddColumn(
                CompositeHeader.Output IOType.Sample,
                ResizeArray [|CompositeCell.createFreeText "MeinRausSample"|]
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


let tests_Person =
    testList "Person" [
        testCase "Full_FromScaffold" (fun () ->
            let role = OntologyAnnotation(name = "Resarcher", tsr = "oo", tan = "oo:123")
            let p = ARCtrl.Person(orcid = "0000-0002-1825-0097", firstName = "John", lastName = "Doe", midInitials = "BD", email = "jd@email.com", phone = "123", fax = "456", address = "123 Main St", affiliation = "My University",roles = ResizeArray [role])
            let ro_Person = PersonConversion.composePerson p
            let p' = PersonConversion.decomposePerson ro_Person
            Expect.equal p' p "Person should match"
        )
        testCase "Full_FromScaffold_Flattened" (fun () ->
            let role = OntologyAnnotation(name = "Resarcher", tsr = "oo", tan = "oo:123")
            let p = ARCtrl.Person(orcid = "0000-0002-1825-0097", firstName = "John", lastName = "Doe", midInitials = "BD", email = "jd@email.com", phone = "123", fax = "456", address = "123 Main St", affiliation = "My University",roles = ResizeArray [role])
            let ro_Person = PersonConversion.composePerson p
            let graph = ro_Person.Flatten()
            // Test that flattened worked
            Expect.isTrue (graph.Nodes.Count > 0) "Graph should have properties"
            let roleRef = Expect.wantSome (ro_Person.TryGetPropertyAsSingleton(LDPerson.jobTitle)) "Person should still have jobTitle"
            Expect.isTrue (roleRef :? LDRef) "Person should be flattened correctly"
            //
            let p' = PersonConversion.decomposePerson(ro_Person, graph = graph)
            Expect.equal p' p "Person should match"
        )
        testCase "ORCIDHandling_Number" (fun () ->
            let p = ARCtrl.Person(firstName = "MyDude", orcid = "0000-0002-1825-0097")
            let ro_Person = PersonConversion.composePerson p
            Expect.equal ro_Person.Id "http://orcid.org/0000-0002-1825-0097" "ORCID should be correct"
            let p' = PersonConversion.decomposePerson ro_Person
            Expect.equal p' p "Person should match"
        )
        testCase "ORCIDHandling_URL" (fun () ->
            let p = ARCtrl.Person(firstName = "MyDude", orcid = "http://orcid.org/0000-0002-1825-0097")
            let ro_Person = PersonConversion.composePerson p
            Expect.equal ro_Person.Id "http://orcid.org/0000-0002-1825-0097" "ORCID should be correct"
            let p' = PersonConversion.decomposePerson ro_Person
            p.ORCID <- Some "0000-0002-1825-0097"
            Expect.equal p' p "Person should match"
        )
        testCase "AddressAsObject_FromROCrate" (fun () ->
            let address = LDPostalAddress.create(addressCountry = "Germoney", postalCode = "6969", streetAddress = "I think I'm funny street 69")
            let p = LDPerson.create(givenName = "Loooookas",address = address)
            let scaffold_Person = PersonConversion.decomposePerson p
            let p' = PersonConversion.composePerson scaffold_Person
            Expect.equal p' p "Person should match"
        )
        testCase "AddressAsString_FromROCrate" (fun () ->
            let address = "Germoney, 6969, I think I'm funny street 69"
            let p = LDPerson.create(givenName = "Loooookas",address = address)
            let scaffold_Person = PersonConversion.decomposePerson p
            let p' = PersonConversion.composePerson scaffold_Person
            Expect.equal p' p "Person should match"
        )
        testCase "AffiliationOnlyName_FromROCrate" (fun () ->
            let affiliation = LDOrganization.create(name = "My University")
            let p = LDPerson.create(givenName = "Loooookas",affiliation = affiliation)
            let scaffold_Person = PersonConversion.decomposePerson p
            let p' = PersonConversion.composePerson scaffold_Person
            Expect.equal p' p "Person should match"
        )
        testCase "AffiliationMoreFields_FromROCrate" (fun () ->
            let affiliation = LDOrganization.create(name = "My University")
            affiliation.SetProperty("address","123 Main St")
            let p = LDPerson.create(givenName = "Loooookas",affiliation = affiliation)
            let scaffold_Person = PersonConversion.decomposePerson p
            let p' = PersonConversion.composePerson scaffold_Person
            Expect.equal p' p "Person should match"
        )
    ]

let tests_Publication =
    testList "Publication" [
        testCase "Full_FromScaffold" (fun () ->
            let authors = "Lukas Weil, John Doe"
            let comment = Comment("MyCommentKey","MyCommentValue")
            let commentOnlyKey = Comment("MyEmptyKey")
            let status = OntologyAnnotation(name = "Published", tsr = "oo", tan = "oo:123")
            let p = ARCtrl.Publication.create(title = "My Paper", doi = "10.1234/5678", authors = authors, status = status, comments = ResizeArray [comment; commentOnlyKey])
            let ro_Publication = ScholarlyArticleConversion.composeScholarlyArticle p
            let p' = ScholarlyArticleConversion.decomposeScholarlyArticle ro_Publication
            Expect.equal p' p "Publication should match"
        )
        testCase "DOI_PubMedID_FromScaffold" (fun () ->
            let doi = "10.1234/5678"
            let pubMedID = "12345678"
            let p = ARCtrl.Publication.create(title = "My Paper", doi = doi, pubMedID = pubMedID)
            let ro_Publication = ScholarlyArticleConversion.composeScholarlyArticle p
            let p' = ScholarlyArticleConversion.decomposeScholarlyArticle ro_Publication
            Expect.equal p' p "Publication should match"
        )
        testCase "DOI_PubMedID_FromScaffold_ToDeprecatedROCrate" (fun () ->
            let doi = "10.1234/5678"
            let pubMedID = "12345678"
            let p = ARCtrl.Publication.create(title = "My Paper", doi = doi, pubMedID = pubMedID)
            let json =
                Json.Publication.ROCrate.encoder p
                |> Json.Encode.toJsonString 0
            let ldNode = Json.Decode.fromJsonString Json.LDNode.decoder json
            let p' = ScholarlyArticleConversion.decomposeScholarlyArticle ldNode
            Expect.equal p' p "Publication should match"
        )
        testCase "Full_FromScaffold_Flattened" (fun () ->
            let authors = "Lukas Weil, John Doe"
            let comment = Comment("MyCommentKey","MyCommentValue")
            let commentOnlyKey = Comment("MyEmptyKey")
            let status = OntologyAnnotation(name = "Published", tsr = "oo", tan = "oo:123")
            let p = ARCtrl.Publication.create(title = "My Paper", doi = "10.1234/5678", authors = authors, status = status, comments = ResizeArray [comment; commentOnlyKey])
            let ro_Publication = ScholarlyArticleConversion.composeScholarlyArticle p
            let graph = ro_Publication.Flatten()
            // Test that flattened worked
            Expect.isTrue (graph.Nodes.Count > 0) "Graph should have properties"
            let statusRef = Expect.wantSome (ro_Publication.TryGetPropertyAsSingleton(LDScholarlyArticle.creativeWorkStatus)) "Publication should still have status"
            Expect.isTrue (statusRef :? LDRef) "Publication should be flattened correctly"
            //
            let p' = ScholarlyArticleConversion.decomposeScholarlyArticle(ro_Publication,graph = graph)
            Expect.equal p' p "Publication should match"
        )
        testCase "FullAuthors_FromROCrate" (fun () ->
            let author1 = LDPerson.create(givenName = "Lukas",familyName = "Weil", orcid = "0000-0002-1825-0097")
            let author2 = LDPerson.create(givenName = "John",familyName = "Doe", orcid = "0000-0002-1325-0077")
            let scholarlyArticle = LDScholarlyArticle.create(headline = "My Paper", identifiers = ResizeArray [], authors = ResizeArray [author1;author2])
            let scaffold_Publication = ScholarlyArticleConversion.decomposeScholarlyArticle scholarlyArticle
            let p = ScholarlyArticleConversion.composeScholarlyArticle scaffold_Publication
            Expect.equal p scholarlyArticle "Publication should match"
        )
    ]

let tests_GetDataFilesFromProcesses =
    testList "GetDataFilesFromProcesses" [
        testCase "EmptyArray" (fun () ->
            let processes = ResizeArray []
            let dataFiles = AssayConversion.getDataFilesFromProcesses processes
            Expect.isEmpty dataFiles "Should have no data files"
        )
        testCase "SingleProcessNoDataFiles" (fun () ->
            let p = LDLabProcess.create(name = Identifier.createMissingIdentifier())
            let processes = ResizeArray [p]
            let dataFiles = AssayConversion.getDataFilesFromProcesses processes
            Expect.isEmpty dataFiles "Should have no data files"
        )
        testCase "InputAndOutputFile" (fun () -> 
            let p = LDLabProcess.create(name = Identifier.createMissingIdentifier())
            let input = LDFile.create(name = "InputFile")
            let output = LDFile.create(name = "OutputFile")
            LDLabProcess.setObjects(p, ResizeArray [input])
            LDLabProcess.setResults(p, ResizeArray [output])
            let processes = ResizeArray [p]
            let dataFiles = AssayConversion.getDataFilesFromProcesses processes
            Expect.hasLength dataFiles 2 "Should have 2 data files"
            Expect.equal dataFiles.[0] input "First data file should be input"
            Expect.equal dataFiles.[1] output "Second data file should be output"
        )
        testCase "FragmentCorrectFieldCopies" (fun () ->
            let p = LDLabProcess.create(name = Identifier.createMissingIdentifier())
            let encodingFormat = "text/csv"
            let comment = LDComment.create(name = "MyCommentKey", text = "MyCommentValue")
            let input = LDFile.create(name = "InputFile#Fragment1", encodingFormat = encodingFormat, comments = ResizeArray [comment])
            LDLabProcess.setObjects(p, ResizeArray [input])
            let processes = ResizeArray [p]
            let dataFiles = AssayConversion.getDataFilesFromProcesses processes
            Expect.hasLength dataFiles 1 "Should have 1 data file"
            let first = dataFiles.[0]
            Expect.equal (LDFile.getNameAsString first) "InputFile" "First data file should be input"
            let encodFormat' = Expect.wantSome (LDFile.tryGetEncodingFormatAsString first) "Encoding format was not copied"
            Expect.equal encodFormat' encodingFormat "Encoding format should be copied"
            let comments = LDFile.getComments first
            Expect.hasLength comments 1 "Should have 1 comment"
            Expect.equal comments.[0] comment "Comment should be copied"
            let fragments = LDDataset.getHasPartsAsFile first
            Expect.hasLength fragments 1 "Should have 1 fragment"
            Expect.equal fragments.[0] input "Fragment should be input"
        )
        testCase "FragmentsOfDifferentFiles" (fun () ->
            let p = LDLabProcess.create(name = Identifier.createMissingIdentifier())
            let input1 = LDFile.create(name = "InputFile1#Fragment1")
            let output2 = LDFile.create(name = "InputFile2#Fragment2")
            LDLabProcess.setObjects(p, ResizeArray [input1])
            LDLabProcess.setResults(p, ResizeArray [output2])
            let processes = ResizeArray [p]
            let dataFiles = AssayConversion.getDataFilesFromProcesses processes
            Expect.hasLength dataFiles 2 "Should have 2 data files"
            let first = dataFiles.[0]
            Expect.equal (LDFile.getNameAsString first) "InputFile1" "First data file should be input1"
            let second = dataFiles.[1]
            Expect.equal (LDFile.getNameAsString second) "InputFile2" "Second data file should be input2"
            let firstFragments = LDDataset.getHasPartsAsFile(first)
            Expect.hasLength firstFragments 1 "First data file should have 1 fragment"
            Expect.equal firstFragments.[0] input1 "First fragment should be Fragment1"
            let secondFragments = LDDataset.getHasPartsAsFile(second)
            Expect.hasLength secondFragments 1 "Second data file should have 1 fragment"
            Expect.equal secondFragments.[0] output2 "Second fragment should be Fragment2"
        )
        testCase "FragmentsOfSameFile" (fun () ->
            let p = LDLabProcess.create(name = Identifier.createMissingIdentifier())
            let input1 = LDFile.create(name = "InputFile#Fragment1")
            let output2 = LDFile.create(name = "InputFile#Fragment2")
            LDLabProcess.setObjects(p, ResizeArray [input1])
            LDLabProcess.setResults(p, ResizeArray [output2])
            let processes = ResizeArray [p]
            let dataFiles = AssayConversion.getDataFilesFromProcesses processes
            Expect.hasLength dataFiles 1 "Should have 1 data file"
            let first = dataFiles.[0]
            Expect.equal (LDFile.getNameAsString first) "InputFile" "First data file should be input1"
            let firstFragments = LDDataset.getHasPartsAsFile first
            Expect.hasLength firstFragments 2 "First data file should have 2 fragments"
            Expect.equal firstFragments.[0] input1 "First fragment should be Fragment1"
            Expect.equal firstFragments.[1] output2 "Second fragment should be Fragment2"
        )
        testCase "ComplexMix" (fun () ->
            let p1 = LDLabProcess.create(name = Identifier.createMissingIdentifier())
            let p1_input = LDFile.create(name = "InputFile")
            let p1_output1 = LDFile.create(name = "MiddleFile#Fragment1")
            let p1_output2 = LDFile.create(name = "MiddleFile#Fragment2")
            LDLabProcess.setObjects(p1, ResizeArray [p1_input])
            LDLabProcess.setResults(p1, ResizeArray [p1_output1;p1_output2])
            let p2_output1 = LDFile.create(name = "OutFile1#Fragment1")
            let p2_output2 = LDFile.create(name = "OutFile2#Fragment2")
            let p2 = LDLabProcess.create(name = Identifier.createMissingIdentifier())
            LDLabProcess.setObjects(p2, ResizeArray [p1_output1;p1_output2])
            LDLabProcess.setResults(p2, ResizeArray [p2_output1;p2_output2])
            let processes = ResizeArray [p1;p2]
            let dataFiles = AssayConversion.getDataFilesFromProcesses processes
            Expect.hasLength dataFiles 4 "Should have 4 data files"
            Expect.equal dataFiles.[0] p1_input "First data file should be input"
            let middleFile = dataFiles.[1]
            Expect.equal (LDFile.getNameAsString middleFile) "MiddleFile" "Second data file should be middle file"
            let middleFragments = LDDataset.getHasPartsAsFile middleFile
            Expect.hasLength middleFragments 2 "Middle file should have 2 fragments"
            Expect.equal middleFragments.[0] p1_output1 "First fragment should be Fragment1"
            Expect.equal middleFragments.[1] p1_output2 "Second fragment should be Fragment2"
            let out1 = dataFiles.[2]
            Expect.equal (LDFile.getNameAsString out1) "OutFile1" "Third data file should be output1"
            let out1_Fragments = LDDataset.getHasPartsAsFile out1
            Expect.hasLength out1_Fragments 1 "Output1 should have 1 fragment"
            Expect.equal out1_Fragments.[0] p2_output1 "First fragment should be Fragment1"
            let out2 = dataFiles.[3]
            Expect.equal (LDFile.getNameAsString out2) "OutFile2" "Fourth data file should be output2"
            let out2_Fragments = LDDataset.getHasPartsAsFile out2
            Expect.hasLength out2_Fragments 1 "Output2 should have 1 fragment"
            Expect.equal out2_Fragments.[0] p2_output2 "First fragment should be Fragment2"
        )
    ]

let tests_Assay =
    testList "Assay" [
        testCase "Empty_FromScaffold" (fun () ->
            let p = ArcAssay.init("My Assay")
            let ro_Assay = AssayConversion.composeAssay p
            let p' = AssayConversion.decomposeAssay ro_Assay
            Expect.equal p' p "Assay should match"
        )
        testCase "Full_FromScaffold" (fun () ->
            let assay = create_assay_full()
            let ro_Assay = AssayConversion.composeAssay assay
            let assay' = AssayConversion.decomposeAssay ro_Assay
            Expect.equal assay' assay "Assay should match"
        )
        testCase "Full_FromScaffold_Flattened" (fun () ->
            let assay = create_assay_full()
            let ro_Assay = AssayConversion.composeAssay assay
            let graph = ro_Assay.Flatten()
            // Test that flattened worked
            Expect.isTrue (graph.Nodes.Count > 0) "Graph should have properties"
            let measurementTypeRef = Expect.wantSome (ro_Assay.TryGetPropertyAsSingleton(LDDataset.variableMeasured)) "Assay should still have measurementType"
            Expect.isTrue (measurementTypeRef :? LDRef) "Assay should be flattened correctly"
            //
            let assay' = AssayConversion.decomposeAssay(ro_Assay, graph = graph)
            Expect.equal assay' assay "Assay should match"
        )
        testList "WithDatamap" [
            testCase "MeasurementType" (fun () ->
                let measurementType = OntologyAnnotation(name = "sugar measurement", tsr = "DPBO", tan = "DPBO:0000120")
                let dc1 = create_dataContextOfRow "MyFile" 1
                let dc2 = create_dataContextOfRow "MyFile" 2
                let dm = DataMap(ResizeArray[dc1;dc2])
                let assay = ArcAssay("My Assay", measurementType = measurementType, datamap = dm)
                let ro_Assay = AssayConversion.composeAssay assay
                let assay' = AssayConversion.decomposeAssay ro_Assay
                let dm' = Expect.wantSome assay'.DataMap "Assay should have a data map"
                Expect.equal dm dm' "Data map should match"
                let measurementType' = Expect.wantSome assay'.MeasurementType "Assay should have a measurement type"
                Expect.equal measurementType measurementType' "Measurement type should match"
                Expect.equal assay' assay "Assay should match"
            )
            testCase "MeasurementType_Flattened" (fun () ->
                let measurementType = OntologyAnnotation(name = "sugar measurement", tsr = "DPBO", tan = "DPBO:0000120")
                let dc1 = create_dataContextOfRow "MyFile" 1
                let dc2 = create_dataContextOfRow "MyFile" 2
                let dm = DataMap(ResizeArray[dc1;dc2])
                let assay = ArcAssay("My Assay", measurementType = measurementType, datamap = dm)
                let ro_Assay = AssayConversion.composeAssay assay
                let graph = ro_Assay.Flatten()
                // Test that flattened worked
                let variableMeasureds = ro_Assay.GetPropertyValues(LDDataset.variableMeasured)
                Expect.hasLength variableMeasureds 3 "Assay should have three variableMeasureds"
                let measurementTypeRef = variableMeasureds.[2]
                Expect.isTrue (measurementTypeRef :? LDRef) "Assay should be flattened correctly"
                //
                let assay' = AssayConversion.decomposeAssay(ro_Assay, graph = graph)
                Expect.equal assay' assay "Assay should match"
            )
            testCase "ContextForFiles" (fun () ->
                let dc1 = create_dataContextOfRow "MyFile" 1
                let dc2 = create_dataContextOfRow "MyFile" 2
                let dm = DataMap(ResizeArray[dc1;dc2])
                let input1 = CompositeCell.createData(create_dataRow "MyFile" 1)
                let input2 = CompositeCell.createData(create_dataRow "MyFile" 2)
                let assay = ArcAssay("My Assay", datamap = dm)
                let table = assay.InitTable("Table")
                table.AddColumn(CompositeHeader.Input IOType.Data, ResizeArray [|input1; input2|])
                let ro_Assay = AssayConversion.composeAssay assay
                let assay' = AssayConversion.decomposeAssay ro_Assay
                let dm' = Expect.wantSome assay'.DataMap "Assay should have a data map"
                Expect.equal dm dm' "Data map should match"
                let table' = Expect.wantSome (Seq.tryExactlyOne assay'.Tables) "Assay should have a table"
                Expect.arcTableEqual table table' "Table should match"
                Expect.equal assay' assay "Assay should match"
            )
            testCase "ContextForFiles_Flattened" (fun () ->
                let dc1 = create_dataContextOfRow "MyFile" 1
                let dc2 = create_dataContextOfRow "MyFile" 2
                let dm = DataMap(ResizeArray[dc1;dc2])
                let input1 = CompositeCell.createData(create_dataRow "MyFile" 1)
                let input2 = CompositeCell.createData(create_dataRow "MyFile" 2)
                let assay = ArcAssay("My Assay", datamap = dm)
                let table = assay.InitTable("Table")
                table.AddColumn(CompositeHeader.Input IOType.Data, ResizeArray [|input1; input2|])
                let ro_Assay = AssayConversion.composeAssay assay
                let graph = ro_Assay.Flatten()
                // Test that flattened worked
                let dataFiles = ro_Assay.GetPropertyValues(LDDataset.hasPart)
                let dataFile = Expect.wantSome (Seq.tryExactlyOne dataFiles) "Assay should have one data file"
                Expect.isTrue (dataFile :? LDRef) "Data file should be flattened correctly"
                let dataContext = ro_Assay.GetPropertyValues(LDDataset.variableMeasured)
                Expect.hasLength dataContext 2 "Assay should have two data contexts"
                let dataContextRef = dataContext.[1]
                Expect.isTrue (dataContextRef :? LDRef) "Data context should be flattened correctly"
                //
                let assay' = AssayConversion.decomposeAssay(ro_Assay, graph = graph)
                let dm' = Expect.wantSome assay'.DataMap "Assay should have a data map"
                Expect.equal dm dm' "Data map should match"
                let table' = Expect.wantSome (Seq.tryExactlyOne assay'.Tables) "Assay should have a table"
                Expect.arcTableEqual table table' "Table should match"
                Expect.equal assay' assay "Assay should match"
            )
        ]
    ]

let tests_Study =
    testList "Study" [
        testCase "Empty_FromScaffold" (fun () ->
            let p = ArcStudy.init("My Study")
            let ro_Study = StudyConversion.composeStudy p
            let p' = StudyConversion.decomposeStudy ro_Study
            Expect.equal p' p "Study should match"
        )
        testCase "Full_FromScaffold" (fun () ->
            let study = create_study_full ()
            let ro_Study = StudyConversion.composeStudy study
            let study' = StudyConversion.decomposeStudy ro_Study
            Expect.equal study' study "Study should match"
        )
        testCase "Full_FromScaffold_Flattened" (fun () ->
            let study = create_study_full ()
            let ro_Study = StudyConversion.composeStudy study
            let graph = ro_Study.Flatten()
            // Test that flattened worked
            Expect.isTrue (graph.Nodes.Count > 0) "Graph should have properties"
            let publication = Expect.wantSome (ro_Study.TryGetPropertyAsSingleton(LDDataset.citation)) "Study should still have Publication"
            Expect.isTrue (publication :? LDRef) "Study should be flattened correctly"
            //
            let study' = StudyConversion.decomposeStudy(ro_Study, graph = graph)
            Expect.equal study' study "Study should match"
        )
        testList "WithDatamap" [            
            testCase "ContextForFiles" (fun () ->
                let dc1 = create_dataContextOfRow "MyFile" 1
                let dc2 = create_dataContextOfRow "MyFile" 2
                let dm = DataMap(ResizeArray[dc1;dc2])
                let input1 = CompositeCell.createData(create_dataRow "MyFile" 1)
                let input2 = CompositeCell.createData(create_dataRow "MyFile" 2)
                let study = ArcStudy("My Study", datamap = dm)
                let table = study.InitTable("Table")
                table.AddColumn(CompositeHeader.Input IOType.Data, ResizeArray [|input1; input2|])
                let ro_Study = StudyConversion.composeStudy study
                let study' = StudyConversion.decomposeStudy ro_Study
                let dm' = Expect.wantSome study'.DataMap "Study should have a data map"
                Expect.equal dm dm' "Data map should match"
                let table' = Expect.wantSome (Seq.tryExactlyOne study'.Tables) "Study should have a table"
                Expect.arcTableEqual table table' "Table should match"
                Expect.equal study' study "Study should match"
            )
            testCase "ContextForFiles_Flattened" (fun () ->
                let dc1 = create_dataContextOfRow "MyFile" 1
                let dc2 = create_dataContextOfRow "MyFile" 2
                let dm = DataMap(ResizeArray[dc1;dc2])
                let input1 = CompositeCell.createData(create_dataRow "MyFile" 1)
                let input2 = CompositeCell.createData(create_dataRow "MyFile" 2)
                let study = ArcStudy("My Study", datamap = dm)
                let table = study.InitTable("Table")
                table.AddColumn(CompositeHeader.Input IOType.Data, ResizeArray [|input1; input2|])
                let ro_Study = StudyConversion.composeStudy study
                let graph = ro_Study.Flatten()
                // Test that flattened worked
                let dataFiles = ro_Study.GetPropertyValues(LDDataset.hasPart)
                let dataFile = Expect.wantSome (Seq.tryExactlyOne dataFiles) "Study should have one data file"
                Expect.isTrue (dataFile :? LDRef) "Data file should be flattened correctly"
                let dataContext = ro_Study.GetPropertyValues(LDDataset.variableMeasured)
                Expect.hasLength dataContext 2 "Study should have two data contexts"
                let dataContextRef = dataContext.[1]
                Expect.isTrue (dataContextRef :? LDRef) "Data context should be flattened correctly"
                //
                let study' = StudyConversion.decomposeStudy(ro_Study, graph = graph)
                let dm' = Expect.wantSome study'.DataMap "Study should have a data map"
                Expect.equal dm dm' "Data map should match"
                let table' = Expect.wantSome (Seq.tryExactlyOne study'.Tables) "Study should have a table"
                Expect.arcTableEqual table table' "Table should match"
                Expect.equal study' study "Study should match"
            )
        ]
    ]


let tests_Investigation = 
    testList "Investigation" [
        testCase "Empty_FromScaffold" (fun () ->
            let p = ArcInvestigation(
                identifier = "My Investigation",
                title = "My Best Investigation"
                )
            let ro_Investigation = InvestigationConversion.composeInvestigation p
            let p' = InvestigationConversion.decomposeInvestigation ro_Investigation
            Expect.isSome p'.PublicReleaseDate "PublicReleaseDate default value should be set"
            p'.PublicReleaseDate <- None // As a default value is used otherwise
            Expect.equal p' p "Investigation should match"
        )
        testCase "TopLevel_FromScaffold" (fun () ->
            let investigation = create_investigation_toplevel()
            let ro_Investigation = InvestigationConversion.composeInvestigation investigation
            let investigation' = InvestigationConversion.decomposeInvestigation ro_Investigation
            Expect.equal investigation' investigation "Investigation should match"
        )
        testCase "TopLevel_FromScaffold_Flattened" (fun () ->
            let investigation = create_investigation_toplevel()
            let ro_Investigation = InvestigationConversion.composeInvestigation investigation
            let graph = ro_Investigation.Flatten()
            // Test that flatten worked
            Expect.isTrue (graph.Nodes.Count > 0) "Graph should have properties"
            let personRef = Expect.wantSome (ro_Investigation.TryGetPropertyAsSingleton(LDDataset.creator)) "Investigation should still have creator"
            Expect.isTrue (personRef :? LDRef) "Investigation should be flattened correctly"
            //
            let investigation' = InvestigationConversion.decomposeInvestigation(ro_Investigation, graph = graph)
            Expect.equal investigation' investigation "Investigation should match"
        )
        testCase "AssayAndStudy_FromScaffold" (fun () ->
            let assay = ArcAssay.init("My Assay")
            let study = ArcStudy.init("My Study")
            let p = ArcInvestigation(
                identifier = "My Investigation",
                title = "My Best Investigation",
                assays = ResizeArray [assay],
                studies = ResizeArray [study]
                )
            let ro_Investigation = InvestigationConversion.composeInvestigation p
            let p' = InvestigationConversion.decomposeInvestigation ro_Investigation
            Expect.isSome p'.PublicReleaseDate "PublicReleaseDate default value should be set"
            p'.PublicReleaseDate <- None // As a default value is used otherwise
            Expect.equal p' p "Investigation should match"
        )
        // This test is meant to check, that two processes created from two different tables with the same name are distinctly named
        // This is important to ensure they are not merged
        // https://github.com/nfdi4plants/ARCtrl/issues/514
        testCase "DistinctProcessIDGeneration" (fun () ->
            // Setup
            let arc = ArcInvestigation("MyArc", title = "MyTitle")
            let study1 = arc.InitStudy "Study1"
            let study2 = arc.InitStudy "Study2"
            let table1 = study1.InitTable("Growth")
            let table2 = study2.InitTable("Growth")
            table1.AddColumn(CompositeHeader.Input(IOType.Source),ResizeArray [|CompositeCell.createFreeText "Source1"; CompositeCell.createFreeText "Source2"|])
            table2.AddColumn(CompositeHeader.Input(IOType.Sample),ResizeArray [|CompositeCell.createFreeText "Sample"|])
            // Check Json LD
            let ro_Investigation = InvestigationConversion.composeInvestigation arc
            let subDatasets = ro_Investigation |> LDDataset.getHasPartsAsDataset
            let s1 = Expect.wantSome (subDatasets |> Seq.tryFind (fun s -> s.Id = "studies/Study1/")) "Study1 should exist"
            let s2 = Expect.wantSome (subDatasets |> Seq.tryFind (fun s -> s.Id = "studies/Study2/")) "Study2 should exist"
            let s1ps = s1 |> LDDataset.getAboutsAsLabProcess
            Expect.hasLength s1ps 2 "Study1 should have two processes"
            let s1p1 = s1ps.[0]
            let s1p2 = s1ps.[1]
            let p2 = Expect.wantSome (s2 |> LDDataset.getAboutsAsLabProcess |> Seq.tryExactlyOne) "Study2 should have a process"
            Expect.notEqual s1p1.Id s1p2.Id "Processes should have different Ids (1)"
            Expect.notEqual s1p1.Id p2.Id "Processes should have different Ids (2)"
            Expect.notEqual s1p2.Id p2.Id "Processes should have different Ids (3)"
            // Check reparsed scaffold
            let arc' = InvestigationConversion.decomposeInvestigation(ro_Investigation)
            let study1' = Expect.wantSome (arc'.TryGetStudy("Study1")) "Study1 should exist"
            let study2' = Expect.wantSome (arc'.TryGetStudy("Study2")) "Study2 should exist"
            Expect.equal study1'.TableCount 1 "Study1 should have one table"
            Expect.equal study2'.TableCount 1 "Study2 should have one table"
            let table1' = study1'.GetTable("Growth")
            let table2' = study2'.GetTable("Growth")
            Expect.arcTableEqual table1' table1 "Table1 should match"
            Expect.arcTableEqual table2' table2 "Table2 should match"
        )
                // This test is meant to check, that two processes created from two different tables with the same name are distinctly named
        // This is important to ensure they are not merged
        // https://github.com/nfdi4plants/ARCtrl/issues/514
        testCase "DistinctProcessID_AssayAndStudySameName" (fun () ->
            // Setup
            let arc = ArcInvestigation("MyArc", title = "MyTitle")
            let study = arc.InitStudy "Dataset"
            let assay = arc.InitAssay "Dataset"
            let table1 = study.InitTable("Growth")
            let table2 = assay.InitTable("Growth")
            table1.AddColumn(CompositeHeader.Input(IOType.Source),ResizeArray [|CompositeCell.createFreeText "Source1"; CompositeCell.createFreeText "Source2"|])
            table2.AddColumn(CompositeHeader.Input(IOType.Sample),ResizeArray [|CompositeCell.createFreeText "Sample"|])
            // Check Json LD
            let ro_Investigation = InvestigationConversion.composeInvestigation arc
            let subDatasets = ro_Investigation |> LDDataset.getHasPartsAsDataset
            let s = Expect.wantSome (subDatasets |> Seq.tryFind (fun s -> s.Id = "studies/Dataset/")) "Study should exist"
            let a = Expect.wantSome (subDatasets |> Seq.tryFind (fun s -> s.Id = "assays/Dataset/")) "Assay should exist"
            let sps = s |> LDDataset.getAboutsAsLabProcess
            Expect.hasLength sps 2 "Study should have two processes"
            let sp1 = sps.[0]
            let sp2 = sps.[1]
            let ap = Expect.wantSome (a |> LDDataset.getAboutsAsLabProcess |> Seq.tryExactlyOne) "Assay should have a process"
            Expect.notEqual sp1.Id sp2.Id "Processes should have different Ids (1)"
            Expect.notEqual sp1.Id ap.Id "Processes should have different Ids (2)"
            Expect.notEqual sp2.Id ap.Id "Processes should have different Ids (3)"
            // Check reparsed scaffold
            let arc' = InvestigationConversion.decomposeInvestigation(ro_Investigation)
            let study' = Expect.wantSome (arc'.TryGetStudy("Dataset")) "Study should exist"
            let assay' = Expect.wantSome (arc'.TryGetAssay("Dataset")) "Assay should exist"
            Expect.equal study'.TableCount 1 "Study should have one table"
            Expect.equal assay'.TableCount 1 "Assay should have one table"
            let table1' = study'.GetTable("Growth")
            let table2' = assay'.GetTable("Growth")
            Expect.arcTableEqual table1' table1 "Table1 should match"
            Expect.arcTableEqual table2' table2 "Table2 should match"
        )
    ]



let main = 
    testList "ArcROCrateConversion" [
        tests_DefinedTerm
        tests_PropertyValue
        tests_ProcessInput
        tests_ProcessOutput
        //tests_ProtocolTransformation
        tests_ArcTableProcess
        tests_ArcTablesProcessSeq
        tests_Person
        tests_Publication
        tests_GetDataFilesFromProcesses
        tests_Data
        tests_DataContext
        tests_DataMap
        tests_Assay
        tests_Study
        tests_Investigation
    ]