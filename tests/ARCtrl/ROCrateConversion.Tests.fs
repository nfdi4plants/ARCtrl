module ARCtrl.ROCrateConversion.Tests

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.Conversion
open ARCtrl.Process
open TestingUtils
open ARCtrl.FileSystem
open CrossAsync
open DynamicObj

module Helper =

    let tryLDRef (o : obj) =
        match o with
        | :? LDRef as r -> Some r
        | _ -> None

    let tryLDNode (o : obj) =
        match o with
        | :? LDNode as n -> Some n
        | _ -> None

    /// Helper functions for constructing CWL test data with less boilerplate
    module CWL =
        
        /// Create an InputRecordField with minimal boilerplate
        let field name type_ : ARCtrl.CWL.InputRecordField = 
            ARCtrl.CWL.InputRecordField(name, type_)
        
        /// Create a CWL Record type with fields
        let record (fields: ARCtrl.CWL.InputRecordField list) : ARCtrl.CWL.CWLType = 
            ARCtrl.CWL.CWLType.Record (ARCtrl.CWL.InputRecordSchema(fields = ARCtrl.CWL.ListEntry.ofValues fields))
        
        /// Create a CWL Enum type with symbols
        let enum (symbols: string list) : ARCtrl.CWL.CWLType = 
            ARCtrl.CWL.CWLType.Enum (ARCtrl.CWL.InputEnumSchema(ResizeArray (List.map ARCtrl.CWL.SchemaSaladString.Literal symbols)))
        
        /// Create a CWL Array type
        let array (items: ARCtrl.CWL.CWLType) : ARCtrl.CWL.CWLType = 
            ARCtrl.CWL.CWLType.Array (ARCtrl.CWL.InputArraySchema(items))
        
        /// Create an optional type (union of null and another type)
        let optional (type_: ARCtrl.CWL.CWLType) : ARCtrl.CWL.CWLType =
            ARCtrl.CWL.CWLType.Union (ResizeArray [ARCtrl.CWL.CWLType.Null; type_])

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

    //type ArcWorkflow(identifier : string, ?title : string, ?description : string, ?workflowType : OntologyAnnotation, ?uri : string, ?version : string, ?subWorkflowIdentifiers : ResizeArray<string>, ?parameters : ResizeArray<Process.ProtocolParameter>, ?components : ResizeArray<Process.Component>, ?datamap : Datamap, ?contacts : ResizeArray<Person>, ?cwlDescription : CWL.CWLProcessingUnit, ?comments : ResizeArray<Comment>) =

    let create_workflow_full() =
        let identifier = "MyWorkflow"
        let title = "My Workflow Title"
        let description = "My Workflow Description"
        let workflowType = OntologyAnnotation(name = "data processing workflow", tsr = "EDAM", tan = "EDAM:3623")
        let uri = "http://example.com/myworkflow"
        let version = "1.0.0"
        //let subWorkflowIdentifiers = ResizeArray ["SubWorkflow1"; "SubWorkflow2"]
        //let parameters = ResizeArray [OntologyAnnotation(name = "statistical Method")]
        let components = ResizeArray [Process.Component.create(value = ScalarValue.Name "Proteomiqon", componentType = OntologyAnnotation(name = "Mass Spec Toolkit", tsr = "EDAM", tan = "EDAM:213123"))]
        let person =
            let role = OntologyAnnotation(name = "Resarcher", tsr = "PO", tan = "PO:123")
            ARCtrl.Person(orcid = "0000-0002-1825-0097", firstName = "John", lastName = "Doe", midInitials = "BD", email = "jd@email.com", phone = "123", fax = "456", address = "123 Main St", affiliation = "My University",roles = ResizeArray [role])
        let comment = Comment("MyCommentKey","MyCommentValue")
        ArcWorkflow(
            identifier = identifier,
            title = title,
            description = description,
            workflowType = workflowType,
            uri = uri,
            version = version,
            //subWorkflowIdentifiers = subWorkflowIdentifiers,
            //parameters = parameters,
            cwlDescription = TestObjects.CWL.CommandLineTool.Basic.processingUnit,
            components = components,
            contacts = ResizeArray [person],
            comments = ResizeArray [comment]
        )

    let create_run_full() =
        let identifier = "MyRun"
        let title = "My Run Title"
        let description = "My Run Description"
        let measurementType = OntologyAnnotation(name = "sugar measurement", tsr = "DPBO", tan = "DPBO:0000120")
        let technologyType = OntologyAnnotation(name = "Photometry", tsr = "NCIT", tan = "NCIT:C65109")
        let technologyPlatform = OntologyAnnotation(name = "Infinite M200 plate reader (Tecan)", tsr = "DPBO", tan = "DPBO:0000116")
        let person =
            let role = OntologyAnnotation(name = "Resarcher", tsr = "PO", tan = "PO:123")
            ARCtrl.Person(orcid = "0000-0002-1825-0097", firstName = "John", lastName = "Doe", midInitials = "BD", email = "jd@email.com", phone = "123", fax = "456", address = "123 Main St", affiliation = "My University",roles = ResizeArray [role])
        let table2 = ArcTable.fromArcTableValues("Table2", twoRowsDifferentParamValue.Headers, twoRowsDifferentParamValue.Values)
        // type ArcRun(identifier: string, ?title : string, ?description : string, ?measurementType : OntologyAnnotation, ?technologyType : OntologyAnnotation, ?technologyPlatform : OntologyAnnotation, ?workflowIdentifiers : ResizeArray<string>, ?tables: ResizeArray<ArcTable>, ?datamap : Datamap, ?performers : ResizeArray<Person>, ?cwlDescription : CWL.CWLProcessingUnit, ?cwlInput : ResizeArray<CWL.CWLParameterReference>, ?comments : ResizeArray<Comment>) = 
        ArcRun(
            identifier = identifier,
            title = title,
            description = description,
            measurementType = measurementType,
            technologyType = technologyType,
            technologyPlatform = technologyPlatform,
            //workflowIdentifiers = ResizeArray ["MyWorkflow"],
            tables = ResizeArray [singleRowMixedValues; table2],
            performers = ResizeArray [person],
            cwlDescription = TestObjects.CWL.CommandLineTool.Basic.processingUnit,
            cwlInput = ResizeArray [TestObjects.CWL.YAMLParameterFile.File.fileParameterReference; TestObjects.CWL.YAMLParameterFile.String.stringParameterReference]
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
            Expect.equal name $"{tableName1}_0" "Process name should be table name with row index"
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
            Expect.equal name $"{tableName1}_0" "Process name should be table name with row index"
        )
        // https://github.com/nfdi4plants/ARCtrl/issues/585
        testCase "EmptyInputOutputCells GetProcesses" (fun () ->
            let t = ArcTable.init(tableName1)
            t.AddColumns([|
                CompositeColumn.create(CompositeHeader.Input IOType.Source, ResizeArray [CompositeCell.createFreeText "EIO_Source_0"; CompositeCell.emptyFreeText])
                CompositeColumn.create(CompositeHeader.Parameter oa_species, createCells_chlamy 2)
                CompositeColumn.create(CompositeHeader.Output IOType.Data, ResizeArray [CompositeCell.createData (Data(name = "EIO_Data_0")); CompositeCell.emptyData])
            |])
            let processes = t.GetProcesses()
            Expect.equal processes.Length 2 "Should have 2 processes"
            Expect.equal (LDLabProcess.getObjects processes.[0]).Count 1 "First process should have 1 input"
            Expect.equal (LDLabProcess.getResults processes.[0]).Count 1 "First process should have 1 output"
            Expect.equal (LDLabProcess.getObjects processes.[1]).Count 0 "Empty input cell should not result in an input"
            Expect.equal (LDLabProcess.getResults processes.[1]).Count 0 "Empty output cell should not result in an output"
        )
        testCase "EmptyInputCellWithCharacteristic GetProcesses" (fun () ->
            let t = ArcTable.init(tableName1)
            t.AddColumns([|
                CompositeColumn.create(CompositeHeader.Input IOType.Source, ResizeArray [CompositeCell.emptyFreeText])
                CompositeColumn.create(CompositeHeader.Characteristic oa_species, createCells_chlamy 1)
                CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "EICC_Sample" 1)
            |])
            let processes = t.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let input = Expect.wantSome ((LDLabProcess.getObjects processes.[0]) |> Seq.tryExactlyOne) "Input with characteristic should be kept"
            Expect.hasLength (LDSample.getCharacteristics input) 1 "Input should have the characteristic"
        )
        testCase "EmptyOutputCellWithFactor GetProcesses" (fun () ->
            let t = ArcTable.init(tableName1)
            t.AddColumns([|
                CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "EOCF_Source" 1)
                CompositeColumn.create(CompositeHeader.Factor oa_temperature, createCells_DegreeCelsius 1)
                CompositeColumn.create(CompositeHeader.Output IOType.Sample, ResizeArray [CompositeCell.emptyFreeText])
            |])
            let processes = t.GetProcesses()
            Expect.equal processes.Length 1 "Should have 1 process"
            let output = Expect.wantSome ((LDLabProcess.getResults processes.[0]) |> Seq.tryExactlyOne) "Output with factor should be kept"
            Expect.hasLength (LDSample.getFactors output) 1 "Output should have the factor value"
        )
        testCase "EmptyInputOutputCells GetAndFromProcesses" (fun () ->
            let t = ArcTable.init(tableName1)
            t.AddColumns([|
                CompositeColumn.create(CompositeHeader.Input IOType.Source, ResizeArray [CompositeCell.createFreeText "EIO_Source_0"; CompositeCell.emptyFreeText])
                CompositeColumn.create(CompositeHeader.Parameter oa_species, createCells_chlamy 2)
                CompositeColumn.create(CompositeHeader.Output IOType.Sample, ResizeArray [CompositeCell.createFreeText "EIO_Sample_0"; CompositeCell.emptyFreeText])
            |])
            let processes = t.GetProcesses()
            let table = ArcTable.fromProcesses(tableName1,processes)
            // Empty input and output cells are not stored in the read table, so compare cell values instead of the table values
            Expect.sequenceEqual table.Headers t.Headers "Headers should be equal"
            Expect.equal table.RowCount t.RowCount "Row count should be equal"
            for c in 0 .. t.ColumnCount - 1 do
                for r in 0 .. t.RowCount - 1 do
                    Expect.equal (table.GetCellAt(c,r)) (t.GetCellAt(c,r)) $"Cell ({c},{r}) should be equal"
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
            Expect.equal pName $"{tableName1}_0" "Process name should be table name with row index"
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
            Expect.equal name $"{tableName1}_0" "Process name should be table name with row index"
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
        // https://github.com/nfdi4plants/ARCtrl/issues/651
        testCase "Full_Compacted" (fun () ->
            let dc = create_dataContextOfRow "MyFile" 1
            let fd = BaseTypes.composeFragmentDescriptor(dc)
            let graph = fd.Flatten()
            let context = LDContext(baseContexts = ResizeArray [Context.initV1_2(); Context.initBioschemasContext()])
            graph.SetContext(context)
            graph.Compact_InPlace()
            let fd' = graph.GetNode(fd.Id)
            let dc' = BaseTypes.decomposeFragmentDescriptor(fd', graph = graph, context = context)
            Expect.equal dc' dc "Data context should match"
        )
    ]

let private tests_Datamap =
    testList "Datamap" [
        testCase "Empty" (fun () ->
            let dm = Datamap.init()
            let fds = DatamapConversion.composeFragmentDescriptors dm
            let dm' = DatamapConversion.decomposeFragmentDescriptors fds
            Expect.equal dm dm' "Data map should match"
        )
        testCase "SingleEntry" (fun () ->
            let dc = create_dataContextOfRow "MyFile" 1
            let dm = Datamap(ResizeArray[dc])
            let fds = DatamapConversion.composeFragmentDescriptors dm
            let dm' = DatamapConversion.decomposeFragmentDescriptors fds
            Expect.equal dm dm' "Data map should match"
        )
        testCase "MultipleEntries" (fun () ->
            let dc1 = create_dataContextOfRow "MyFile" 1
            let dc2 = create_dataContextOfRow "MyFile" 2
            let dm = Datamap(ResizeArray[dc1;dc2])
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
            Expect.equal name $"{tableName1}_0" "Process name should be table name with row index"
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
        testCase "SingleProcessNameBeatsProtocolName" (fun () ->
            let protocol = LDLabProtocol.create(id = LDLabProtocol.genId(name = "My protocol"), name = "My protocol")
            let p =
                LDLabProcess.create(
                    name = "my-sheet",
                    objects = ResizeArray [LDSample.createSource(name = "MySource")],
                    results = ResizeArray [LDSample.createSample(name = "MySample")],
                    executesLabProtocol = protocol
                )
            let resultTables = ArcTables.fromProcesses [p]
            Expect.equal resultTables.TableCount 1 "Should have 1 table"
            Expect.equal resultTables.Tables.[0].Name "my-sheet" "Table should be named after the process"
            Expect.equal resultTables.Tables.[0].RowCount 1 "Table should have 1 row"
        )
        testCase "EmptyProcessNameFallsBackToProtocolName" (fun () ->
            let protocol = LDLabProtocol.create(id = LDLabProtocol.genId(name = "My protocol"), name = "My protocol")
            let createProcess (name : string) (id : string) (source : string) (sample : string) =
                LDLabProcess.create(
                    name = name,
                    id = id,
                    objects = ResizeArray [LDSample.createSource(name = source)],
                    results = ResizeArray [LDSample.createSample(name = sample)],
                    executesLabProtocol = protocol
                )
            let p1 = createProcess "" "#Process_Empty1" "Source1" "Sample1"
            let p2 = createProcess "  " "#Process_Empty2" "Source2" "Sample2"
            let resultTables = ArcTables.fromProcesses [p1; p2]
            Expect.equal resultTables.TableCount 1 "Should have 1 table"
            Expect.equal resultTables.Tables.[0].Name "My protocol" "Table should be named after the protocol"
            Expect.equal resultTables.Tables.[0].RowCount 2 "Table should have 2 rows"
        )
        testCase "UnnamedProcessFallsBackToProtocolName" (fun () ->
            let protocol = LDLabProtocol.create(id = LDLabProtocol.genId(name = "My protocol"), name = "My protocol")
            let p =
                LDLabProcess.create(
                    name = "temporary",
                    objects = ResizeArray [LDSample.createSource(name = "MySource")],
                    results = ResizeArray [LDSample.createSample(name = "MySample")],
                    executesLabProtocol = protocol
                )
            p.RemoveProperty(LDLabProcess.name) |> ignore
            Expect.isNone (LDLabProcess.tryGetNameAsString p) "Process should have no name"
            let resultTables = ArcTables.fromProcesses [p]
            Expect.equal resultTables.TableCount 1 "Should have 1 table"
            Expect.equal resultTables.Tables.[0].Name "My protocol" "Table should be named after the protocol"
            Expect.equal resultTables.Tables.[0].RowCount 1 "Table should have 1 row"
        )
        testCase "SingleRowTableNamesRoundTrip" (fun () ->
            let headers =
                ResizeArray [
                    CompositeHeader.Input IOType.Source
                    CompositeHeader.ProtocolREF
                    CompositeHeader.Output IOType.Sample
                ]
            let createRow (source : string) (protocol : string) (sample : string) =
                ResizeArray [
                    CompositeCell.createFreeText source
                    CompositeCell.createFreeText protocol
                    CompositeCell.createFreeText sample
                ]
            let mySheet = ArcTable.createFromRows("my-sheet", headers, ResizeArray [createRow "source1" "My protocol" "sample1"])
            let mySheetTwoRows =
                ArcTable.createFromRows(
                    "my-sheet-2rows",
                    headers,
                    ResizeArray [
                        createRow "source5" "My protocol" "sample5"
                        createRow "source6" "My protocol" "sample6"
                    ]
                )
            let mySheetUnderscore = ArcTable.createFromRows("my-sheet_1", headers, ResizeArray [createRow "source7" "My protocol" "sample7"])
            let twoRows =
                ArcTable.createFromRows(
                    "two-rows",
                    headers,
                    ResizeArray [
                        createRow "source2" "Other protocol" "sample2"
                        createRow "source3" "Other protocol" "sample3"
                    ]
                )
            let step2 =
                ArcTable.createFromRows(
                    "step_2",
                    ResizeArray [CompositeHeader.Input IOType.Source; CompositeHeader.Output IOType.Sample],
                    ResizeArray [ResizeArray [CompositeCell.createFreeText "source4"; CompositeCell.createFreeText "sample4"]]
                )
            let tables = ArcTables(ResizeArray [mySheet; mySheetTwoRows; mySheetUnderscore; twoRows; step2])
            let processes = tables.GetProcesses()
            let resultTables = ArcTables.fromProcesses processes
            let names = resultTables.Tables |> Seq.map (fun t -> t.Name) |> List.ofSeq
            let rowCounts = resultTables.Tables |> Seq.map (fun t -> t.RowCount) |> List.ofSeq
            Expect.equal names ["my-sheet"; "my-sheet-2rows"; "my-sheet_1"; "two-rows"; "step_2"] "Table names should survive the round trip"
            Expect.equal rowCounts [1; 2; 1; 2; 1] "Row counts should survive the round trip"
        )
        testCase "BlankProcessNamesWithoutProtocolStayTogether" (fun () ->
            let createProcess (id : string) (source : string) (sample : string) =
                LDLabProcess.create(
                    name = "",
                    id = id,
                    objects = ResizeArray [LDSample.createSource(name = source)],
                    results = ResizeArray [LDSample.createSample(name = sample)]
                )
            let p1 = createProcess "#Process_Blank1" "Source1" "Sample1"
            let p2 = createProcess "#Process_Blank2" "Source2" "Sample2"
            let resultTables = ArcTables.fromProcesses [p1; p2]
            Expect.equal resultTables.TableCount 1 "Should have 1 table"
            Expect.equal resultTables.Tables.[0].Name "" "Table should keep the blank name"
            Expect.equal resultTables.Tables.[0].RowCount 2 "Table should have 2 rows"
        )
        testCase "UnusualTableNamesRoundTrip" (fun () ->
            let createTable (name : string) (source : string) (sample : string) =
                let t = ArcTable.init name
                t.AddColumn(CompositeHeader.Input IOType.Source, ResizeArray [CompositeCell.createFreeText source])
                t.AddColumn(CompositeHeader.Output IOType.Sample, ResizeArray [CompositeCell.createFreeText sample])
                t
            let emptyName = createTable "" "source1" "sample1"
            let leadingSpace = createTable " x" "source2" "sample2"
            let plainName = createTable "x" "source3" "sample3"
            let zeroRows = ArcTable.init "step_2"
            let step = createTable "step" "source4" "sample4"
            let tables = ArcTables(ResizeArray [emptyName; leadingSpace; plainName; zeroRows; step])
            let processes = tables.GetProcesses()
            let resultTables = ArcTables.fromProcesses processes
            let names = resultTables.Tables |> Seq.map (fun t -> t.Name) |> List.ofSeq
            Expect.equal names [""; " x"; "x"; "step_2"; "step"] "Table names should survive the round trip"
        )
        testCase "OverflowingNumberSuffixKeepsFullName" (fun () ->
            let p =
                LDLabProcess.create(
                    name = "run_20240101120000",
                    objects = ResizeArray [LDSample.createSource(name = "MySource")],
                    results = ResizeArray [LDSample.createSample(name = "MySample")]
                )
            let resultTables = ArcTables.fromProcesses [p]
            Expect.equal resultTables.TableCount 1 "Should have 1 table"
            Expect.equal resultTables.Tables.[0].Name "run_20240101120000" "Table should keep the full process name"
            Expect.equal resultTables.Tables.[0].RowCount 1 "Table should have 1 row"
        )
        testCase "NumberSuffixInt32Boundary" (fun () ->
            Expect.equal (ProcessConversion.decomposeProcessName "x_2147483647") ("x", Some 2147483647) "Int32.MaxValue should be split off"
            Expect.equal (ProcessConversion.decomposeProcessName "x_2147483648") ("x_2147483648", None) "A number above Int32.MaxValue should keep the full name"
        )
        testCase "OverflowingNumberSuffixDoesNotMergeWithRoot" (fun () ->
            let createProcess (name : string) (id : string) (source : string) (sample : string) =
                LDLabProcess.create(
                    name = name,
                    id = id,
                    objects = ResizeArray [LDSample.createSource(name = source)],
                    results = ResizeArray [LDSample.createSample(name = sample)]
                )
            let p1 = createProcess "run_20240101120000" "#Process_Run1" "Source1" "Sample1"
            let p2 = createProcess "run_0" "#Process_Run2" "Source2" "Sample2"
            let resultTables = ArcTables.fromProcesses [p1; p2]
            let names = resultTables.Tables |> Seq.map (fun t -> t.Name) |> List.ofSeq
            Expect.equal names ["run_20240101120000"; "run"] "Overflowing suffix and row index should give two tables"
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
        // https://github.com/nfdi4plants/ARCtrl/issues/627
        testCase "DOI_FromScaffold_AbsoluteID" (fun () ->
            let p = ARCtrl.Publication.create(title = "My Paper", doi = "10.1234/5678", pubMedID = "12345678")
            let ro_Publication = ScholarlyArticleConversion.composeScholarlyArticle p
            Expect.equal ro_Publication.Id "https://doi.org/10.1234/5678" "Publication id should be the DOI url"
        )
        testCase "PubMedID_FromScaffold_AbsoluteID" (fun () ->
            let p = ARCtrl.Publication.create(title = "My Paper", pubMedID = "12345678")
            let ro_Publication = ScholarlyArticleConversion.composeScholarlyArticle p
            Expect.equal ro_Publication.Id "https://pubmed.ncbi.nlm.nih.gov/12345678/" "Publication id should be the PubMed url"
        )
        testCase "DOIUrl_FromScaffold_AbsoluteID" (fun () ->
            let p = ARCtrl.Publication.create(title = "My Paper", doi = "https://doi.org/10.1234/5678")
            let ro_Publication = ScholarlyArticleConversion.composeScholarlyArticle p
            Expect.equal ro_Publication.Id "https://doi.org/10.1234/5678" "Publication id should be the given DOI url"
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

open ARCtrl.CWL

let tests_FormalParameter =
    testList "FormalParameter" [
        testList "Input" [
            testCase "Simple" (fun () ->
                let name = "MyInput"
                let t = CWLType.String
                let input = CWL.CWLInput(name = name, type_ = t)
                let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                let name' = Expect.wantSome (LDFormalParameter.tryGetNameAsString formalParameter) "FormalParameter should have a name"
                Expect.equal name' name "FormalParameter name should match input name"
                let isRequired = Expect.wantSome (LDFormalParameter.tryGetValueRequiredAsBoolean formalParameter) "FormalParameter should have valueRequired"
                Expect.isTrue isRequired "FormalParameter should be required"
                Expect.hasLength (LDFormalParameter.getIdentifiers formalParameter) 0 "FormalParameter should have no identifiers"
                //let t' = Expect.wantExactlyOne (formalParameter.AdditionalType) "FormalParameter should have an additionalType"
                //Expect.equal t' t "FormalParameter type should match input type"
            )
            testCase "Simple_Roundabout" (fun () ->
                let name = "MyInput"
                let t = CWLType.String
                let input = CWL.CWLInput(name = name, type_ = t)
                let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                let input' = WorkflowConversion.decomposeInputFromFormalParameter formalParameter
                Expect.equal input' input "Input should match after roundabout"
            )
            testCase "Simple_Optional" (fun () ->
                let name = "MyInput"
                let t = CWLType.String
                let input = CWL.CWLInput(name = name, type_ = CWL.CWLType.makeOptional t)
                let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                let name' = Expect.wantSome (LDFormalParameter.tryGetNameAsString formalParameter) "FormalParameter should have a name"
                Expect.equal name' name "FormalParameter name should match input name"
                let isRequired = Expect.wantSome (LDFormalParameter.tryGetValueRequiredAsBoolean formalParameter) "FormalParameter should have valueRequired"
                Expect.isFalse isRequired "FormalParameter should not be required"          
            )
            testCase "Prefix" (fun () ->
                let name = "MyInput"
                let t = CWLType.String
                let binding = InputBinding.create(prefix = (SchemaSaladString.Literal "--eenput"))
                let input = CWL.CWLInput(name = name, type_ = t, inputBinding = CWL.ListEntry.Value binding)
                let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                let identifiers = LDFormalParameter.getIdentifiers formalParameter
                Expect.hasLength identifiers 1 "FormalParameter should have 1 identifier"
                let id = identifiers.[0]
                Expect.isTrue (LDPropertyValue.validatePrefix id) "Identifier should be a prefix"
                let prefix = Expect.wantSome (LDPropertyValue.tryGetValueAsString id) "Identifier should have a prefix"
                Expect.equal prefix "--eenput" "Prefix should match input binding"         
            )
            testCase "Position" (fun () ->
                let name = "MyInput"
                let t = CWLType.String
                let binding = InputBinding.create(position = BindingPosition.Number 1)
                let input = CWL.CWLInput(name = name, type_ = t, inputBinding = CWL.ListEntry.Value binding)
                let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                let identifiers = LDFormalParameter.getIdentifiers formalParameter
                Expect.hasLength identifiers 1 "FormalParameter should have 1 identifier"
                let id = identifiers.[0]
                Expect.isTrue (LDPropertyValue.validatePosition id) "Identifier should be a position"
                let position = Expect.wantSome (LDPropertyValue.tryGetValueAsString id) "Identifier should have a position"
                Expect.equal position "1" "Position should match input binding" 
            )
            testCase "A position that is an expression or a directive has no identifier" (fun () ->
                for position in [ BindingPosition.Expression "$(1 + 1)"; BindingPosition.Imported "p.yml"; BindingPosition.Included "p.txt"; BindingPosition.Mixed { Node = YAMLicious.Reader.readRoot "{$mixin: p.yml}" } ] do
                    let binding = InputBinding.create(position = position)
                    let input = CWL.CWLInput(name = "MyInput", type_ = CWLType.String, inputBinding = CWL.ListEntry.Value binding)
                    let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                    Expect.hasLength (LDFormalParameter.getIdentifiers formalParameter) 0 (sprintf "%A: the position has no number to write." position)
                let withPrefix = InputBinding.create(prefix = (SchemaSaladString.Literal "--x"), position = BindingPosition.Imported "p.yml")
                let formalParameter = WorkflowConversion.composeFormalParameterFromInput (CWL.CWLInput(name = "MyInput", type_ = CWLType.String, inputBinding = CWL.ListEntry.Value withPrefix))
                let identifiers = LDFormalParameter.getIdentifiers formalParameter
                Expect.hasLength identifiers 1 "The prefix stays and the directive position is left out."
                Expect.isTrue (LDPropertyValue.validatePrefix identifiers.[0]) "The identifier is the prefix."
            )
            testCase "A binding that a file holds has no identifiers" (fun () ->
                for binding in [ CWL.ListEntry.Imported "b.yml"; CWL.ListEntry.Mixed { Node = YAMLicious.Reader.readRoot "{$mixin: b.yml, position: 1}" } ] do
                    let input = CWL.CWLInput(name = "MyInput", type_ = CWLType.String, inputBinding = binding)
                    let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                    Expect.hasLength (LDFormalParameter.getIdentifiers formalParameter) 0 (sprintf "%A: the binding of a file has no prefix and no position to write." binding)
            )
            testCase "PrefixPosition" (fun () ->
                let name = "MyInput"
                let t = CWLType.String
                let binding = InputBinding.create(prefix = (SchemaSaladString.Literal "--eenput"), position = BindingPosition.Number 1)
                let input = CWL.CWLInput(name = name, type_ = t, inputBinding = CWL.ListEntry.Value binding)
                let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                let identifiers = LDFormalParameter.getIdentifiers formalParameter
                Expect.hasLength identifiers 2 "FormalParameter should have 2 identifiers"
                let prefixId = identifiers |> Seq.tryFind LDPropertyValue.validatePrefix |> fun p -> Expect.wantSome p "Should have a prefix identifier"
                let prefix = Expect.wantSome (LDPropertyValue.tryGetValueAsString prefixId) "Identifier should have a prefix"
                Expect.equal prefix "--eenput" "Prefix should match input binding"
                let positionId = identifiers |> Seq.tryFind LDPropertyValue.validatePosition |> fun p ->  Expect.wantSome p "Should have a position identifier"
                let position = Expect.wantSome (LDPropertyValue.tryGetValueAsString positionId) "Identifier should have a position"
                Expect.equal position "1" "Position should match input binding"
            )
            testCase "PrefixPosition_Roundabout" (fun () ->
                let name = "MyInput"
                let t = CWLType.String
                let binding = InputBinding.create(prefix = (SchemaSaladString.Literal "--eenput"), position = BindingPosition.Number 1)
                let input = CWL.CWLInput(name = name, type_ = t, inputBinding = CWL.ListEntry.Value binding)
                let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                let input' = WorkflowConversion.decomposeInputFromFormalParameter formalParameter
                Expect.equal input' input "Input should match after roundabout"
            )
            ptestCase "File" (fun () ->
                let name = "MyInput"
                let t = CWLType.File
                let input = CWL.CWLInput(name = name, type_ = t)
                let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                Expect.equal formalParameter.Id "#FormalParameter_MyInput" "FormalParameter ID should match"
                let name' = Expect.wantSome (LDFormalParameter.tryGetNameAsString formalParameter) "FormalParameter should have a name"
                Expect.equal name' name "FormalParameter name should match input name"
                let isRequired = Expect.wantSome (LDFormalParameter.tryGetValueRequiredAsBoolean formalParameter) "FormalParameter should have valueRequired"
                Expect.isTrue isRequired "FormalParameter should be required"
                Expect.hasLength (LDFormalParameter.getIdentifiers formalParameter) 0 "FormalParameter should have no identifiers"
            )
        ]
        testList "Output" [
            testCase "Simple" (fun () ->
                let name = "MyOutput"
                let t = CWLType.String
                let output = CWL.CWLOutput(name = name, type_ = t)
                let formalParameter = WorkflowConversion.composeFormalParameterFromOutput output
                let name' = Expect.wantSome (LDFormalParameter.tryGetNameAsString formalParameter) "FormalParameter should have a name"
                Expect.equal name' name "FormalParameter name should match output name"
                let isRequired = Expect.wantSome (LDFormalParameter.tryGetValueRequiredAsBoolean formalParameter) "FormalParameter should have valueRequired"
                Expect.isTrue isRequired "FormalParameter should be required"
            )
            testCase "FromOutput" (fun () ->
                let name = "MyOutput"
                let t = CWLType.String
                let output = CWL.CWLOutput(name = name, type_ = t)
                let formalParameter = WorkflowConversion.composeFormalParameterFromOutput output
                let name' = Expect.wantSome (LDFormalParameter.tryGetNameAsString formalParameter) "FormalParameter should have a name"
                Expect.equal name' name "FormalParameter name should match output name"
                let isRequired = Expect.wantSome (LDFormalParameter.tryGetValueRequiredAsBoolean formalParameter) "FormalParameter should have valueRequired"
                Expect.isTrue isRequired "FormalParameter should be required"
                Expect.hasLength (LDFormalParameter.getIdentifiers formalParameter) 0 "FormalParameter should have no identifiers"           
            )
            testCase "File" (fun () ->
                let name = "MyOutput"
                let t = CWLType.File
                let output = CWL.CWLOutput(name = name, type_ = t)
                let formalParameter = WorkflowConversion.composeFormalParameterFromOutput output
                let name' = Expect.wantSome (LDFormalParameter.tryGetNameAsString formalParameter) "FormalParameter should have a name"
                Expect.equal name' name "FormalParameter name should match output name"
                let isRequired = Expect.wantSome (LDFormalParameter.tryGetValueRequiredAsBoolean formalParameter) "FormalParameter should have valueRequired"
                Expect.isTrue isRequired "FormalParameter should be required"
                Expect.hasLength (LDFormalParameter.getIdentifiers formalParameter) 0 "FormalParameter should have no identifiers"           
            )
            testCase "File_GlobBinding" (fun () ->
                let name = "MyOutput"
                let t = CWLType.File
                let binding = OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "*.txt"))
                let output = CWL.CWLOutput(name = name, type_ = t, outputBinding = CWL.ListEntry.Value binding)
                let formalParameter = WorkflowConversion.composeFormalParameterFromOutput output
                let name' = Expect.wantSome (LDFormalParameter.tryGetNameAsString formalParameter) "FormalParameter should have a name"
                Expect.equal name' name "FormalParameter name should match output name"
                let isRequired = Expect.wantSome (LDFormalParameter.tryGetValueRequiredAsBoolean formalParameter) "FormalParameter should have valueRequired"
                Expect.isTrue isRequired "FormalParameter should be required"
                let identifiers = LDFormalParameter.getIdentifiers formalParameter
                Expect.hasLength identifiers 1 "FormalParameter should have one identifiers"
                let globId = identifiers |> Seq.tryFind LDPropertyValue.validateGlob |> fun p -> Expect.wantSome p "Should have a glob identifier"
                let glob = Expect.wantSome (LDPropertyValue.tryGetValueAsString globId) "Identifier should have a glob"
                Expect.equal glob "*.txt" "Glob should match output binding"
            )
            testCase "An output binding that a file holds has no identifiers" (fun () ->
                for binding in [ CWL.ListEntry.Imported "b.yml"; CWL.ListEntry.Mixed { Node = YAMLicious.Reader.readRoot "{$mixin: b.yml, glob: o.txt}" } ] do
                    let output = CWL.CWLOutput(name = "MyOutput", type_ = CWLType.File, outputBinding = binding)
                    let formalParameter = WorkflowConversion.composeFormalParameterFromOutput output
                    Expect.hasLength (LDFormalParameter.getIdentifiers formalParameter) 0 (sprintf "%A: the binding of a file has no glob to write." binding)
            )
            testCase "File_GlobBinding_Roundabout" (fun () ->
                let name = "MyOutput"
                let t = CWLType.File
                let binding = OutputBinding.create(glob = OutputGlob.Single (SchemaSaladString.Literal "*.txt"))
                let output = CWL.CWLOutput(name = name, type_ = t, outputBinding = CWL.ListEntry.Value binding)
                let formalParameter = WorkflowConversion.composeFormalParameterFromOutput output
                let output' = WorkflowConversion.decomposeOutputFromFormalParameter formalParameter
                Expect.equal output' output "Output should match after roundabout"
            )
        ]
    ]

let tests_AdditionalTypeDecoding =
    testList "AdditionalTypeDecoding" [
        testList "EveryTypeRoundTrip" [
            let cases : (string * CWLType) list =
                [ "File", CWLType.File
                  "Directory", CWLType.Directory
                  "String", CWLType.String
                  "Int", CWLType.Int
                  "Long", CWLType.Long
                  "Float", CWLType.Float
                  "Double", CWLType.Double
                  "Boolean", CWLType.Boolean
                  "Null", CWLType.Null
                  "Stdout", CWLType.Stdout
                  "Stderr", CWLType.Stderr
                  "Stdin", CWLType.Stdin
                  "Reference", CWLType.Reference "Point"
                  "ReferenceInFile", CWLType.Reference "shapes.yml#Circle"
                  "ReferenceArray", Helper.CWL.array (CWLType.Reference "Point")
                  "OptionalReference", Helper.CWL.optional (CWLType.Reference "Point")
                  "RecordWithReference", Helper.CWL.record [Helper.CWL.field "p" (CWLType.Reference "Point")]
                  "Any", CWLType.Any
                  "AnyArray", Helper.CWL.array CWLType.Any
                  "OptionalAny", Helper.CWL.optional CWLType.Any
                  "FileArray", Helper.CWL.array (CWLType.File)
                  "NestedStringArray", Helper.CWL.array (Helper.CWL.array CWLType.String)
                  "OptionalString", Helper.CWL.optional CWLType.String
                  "OptionalDirectoryArray", Helper.CWL.optional (Helper.CWL.array (CWLType.Directory))
                  "Union", CWLType.Union (ResizeArray [CWLType.String; CWLType.Int])
                  "UnionWithNull", CWLType.Union (ResizeArray [CWLType.Null; CWLType.String; CWLType.Int])
                  "Record", Helper.CWL.record [Helper.CWL.field "name" CWLType.String]
                  "RecordWithNullField", Helper.CWL.record [Helper.CWL.field "nothing" CWLType.Null]
                  "Enum", Helper.CWL.enum ["a"; "b"]
                  "RecordArray", Helper.CWL.array (Helper.CWL.record [Helper.CWL.field "value" CWLType.String]) ]
            for (name, original) in cases do
                testCase name (fun () ->
                    let encoded = WorkflowConversion.composeAdditionalType original
                    let decoded = WorkflowConversion.decomposeAdditionalType encoded
                    Expect.equal decoded original (sprintf "%s should survive compose and decompose, written as %s" name encoded)
                )
        ]
        testList "TypesWithoutSchema" [
            testCase "WellFormedUnknownNameIsAReference" (fun () ->
                Expect.equal (WorkflowConversion.decomposeAdditionalType "Strng") (CWLType.Reference "Strng") "No schema is in view to say that the name is wrong."
                Expect.equal
                    (WorkflowConversion.decomposeAdditionalType "Point[]?")
                    (Helper.CWL.optional (Helper.CWL.array (CWLType.Reference "Point")))
                    "The shorthand works with a reference."
            )
            testCase "MalformedNamesFail" (fun () ->
                for text in [ "File["; "a b"; "File]"; "File[]]"; "Po?int" ] do
                    match (try WorkflowConversion.decomposeAdditionalType text |> ignore; None with YAMLicious.YamlDecodeException error -> Some error) with
                    | Some error ->
                        Expect.stringContains error.Message "Invalid CWL type" $"{text}: the error should say that the type is invalid."
                        Expect.isSome error.Span $"{text}: the error should have the place in the text."
                    | None -> failwith $"{text} must fail"
            )
            testCase "StreamTypesAreTheWholeTypeOnly" (fun () ->
                for text in [ "[stdout, string]"; "stdout?"; "stdout[]"; "stderr[]"; "[stdin, \"null\"]" ] do
                    match (try WorkflowConversion.decomposeAdditionalType text |> ignore; None with YAMLicious.YamlDecodeException error -> Some error) with
                    | Some error ->
                        Expect.stringContains error.Message "valid only as the whole type" $"{text}: the error should say where a stream type is valid."
                        Expect.isSome error.Span $"{text}: the error should have the place in the text."
                    | None -> failwith $"{text} must fail"
            )
        ]
        testList "UnionOrder" [
            testCase "NullBehindTheTypeIsAList" (fun () ->
                let original = CWLType.Union (ResizeArray [CWLType.String; CWLType.Null])
                let encoded = WorkflowConversion.composeAdditionalType original
                Expect.equal encoded """[string, "null"]""" "A list with the null behind the type is no optional type."
                Expect.equal (WorkflowConversion.decomposeAdditionalType encoded) original "The list should keep its order."
            )
            testCase "NullInFrontOfTheTypeIsTheShorthand" (fun () ->
                let original = Helper.CWL.optional CWLType.String
                let encoded = WorkflowConversion.composeAdditionalType original
                Expect.equal encoded "string?" "A list with the null in front of the type is the optional type."
                Expect.equal (WorkflowConversion.decomposeAdditionalType encoded) original "The shorthand should give the list back."
            )
        ]
        testList "ScalarForms" [
            testCase "DoubleQuoted" (fun () ->
                Expect.equal (WorkflowConversion.decomposeAdditionalType "\"File\"") (CWLType.File) "A double-quoted type name is the type."
            )
            testCase "SingleQuotedOptional" (fun () ->
                Expect.equal (WorkflowConversion.decomposeAdditionalType "'string?'") (Helper.CWL.optional CWLType.String) "A single-quoted optional type is the type."
            )
            testCase "PlainNull" (fun () ->
                Expect.equal (WorkflowConversion.decomposeAdditionalType "null") CWLType.Null "The plain text null is the null type."
            )
            testCase "TrailingComment" (fun () ->
                Expect.equal (WorkflowConversion.decomposeAdditionalType "File[]? # an optional list") (Helper.CWL.optional (Helper.CWL.array (CWLType.File))) "A comment behind the type is no part of it."
            )
            testCase "MissingValueFails" (fun () ->
                Expect.throws (fun () -> WorkflowConversion.decomposeAdditionalType "" |> ignore) "No text is no type."
            )
        ]
        testList "SimpleTypes_FromYAML" [
            testCase "String" (fun () ->
                let encoded = "string"
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded CWLType.String "Should decode string from YAML"
            )
            testCase "Int" (fun () ->
                let encoded = "int"
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded CWLType.Int "Should decode int from YAML"
            )
            testCase "File" (fun () ->
                let expected = CWLType.File
                let encoded = "File"
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded expected "Should decode File from YAML"
            )
            testCase "FileArray" (fun () ->
                let expected = CWLType.Array (InputArraySchema(CWLType.File))
                let encoded = "File[]"
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded expected "Should decode File[] from YAML"
            )
            testCase "StringArray" (fun () ->
                let expected = CWLType.Array (InputArraySchema(CWLType.String))
                let encoded = "string[]"
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded expected "Should decode string[] from YAML"
            )
            testCase "NestedFileArray" (fun () ->
                let inner = InputArraySchema(CWLType.File)
                let expected = CWLType.Array (InputArraySchema(CWLType.Array inner))
                let encoded = "File[][]"
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded expected "Should decode File[][] from YAML"
            )
            testCase "DeeplyNestedStringArray" (fun () ->
                let innerArray = InputArraySchema(CWLType.String)
                let middleArray = InputArraySchema(CWLType.Array innerArray)
                let expected = CWLType.Array (InputArraySchema(CWLType.Array middleArray))
                let encoded = "string[][][]"
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded expected "Should decode string[][][] from YAML"
            )
        ]
        testList "ComplexTypes_FromYAML" [
            testCase "Record" (fun () ->
                let original = Helper.CWL.record [
                    Helper.CWL.field "name" CWLType.String
                    Helper.CWL.field "age" CWLType.Int
                ]
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "Should decode Record correctly"
            )
            testCase "Enum" (fun () ->
                let original = Helper.CWL.enum ["option1"; "option2"; "option3"]
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "Should decode Enum correctly"
            )
            testCase "RecordArray" (fun () ->
                let original = Helper.CWL.array (
                    Helper.CWL.record [Helper.CWL.field "value" CWLType.String]
                )
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "Should decode RecordArray correctly"
            )
            testCase "EnumArray" (fun () ->
                let original = Helper.CWL.array (Helper.CWL.enum ["A"; "B"; "C"])
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "Should decode EnumArray correctly"
            )
        ]
        testList "OptionalTypes" [
            testCase "OptionalString_YAML" (fun () ->
                let expected = Helper.CWL.optional CWLType.String
                let encoded = "string?"
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded expected "Should decode optional string correctly"
            )
            testCase "OptionalFileArray_YAML" (fun () ->
                let expected = Helper.CWL.optional (Helper.CWL.array (CWLType.File))
                let encoded = "File[]?"
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded expected "Should decode optional File[] correctly"
            )
            testCase "OptionalRecord_YAML" (fun () ->
                let original = Helper.CWL.optional (
                    Helper.CWL.record [Helper.CWL.field "id" CWLType.String]
                )
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "Should round-trip optional Record correctly"
            )
        ]
        testList "EdgeCases" [
            testCase "EmptyRecord" (fun () ->
                let original = CWLType.Record (InputRecordSchema(fields = ListEntry.ofValues []))
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "Should decode empty Record correctly"
            )
            testCase "SingleSymbolEnum" (fun () ->
                let original = CWLType.Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "only" ]))
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "Should decode single symbol Enum correctly"
            )
        ]
        testList "RoundTrip" [
            testCase "SimpleType_RoundTrip" (fun () ->
                let original = CWLType.String
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "String should round-trip correctly"
            )
            testCase "Array_RoundTrip" (fun () ->
                let original = CWLType.Array (InputArraySchema(CWLType.String))
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "Array should round-trip correctly"
            )
            testCase "Record_RoundTrip" (fun () ->
                let field1 = InputRecordField("name", CWLType.String)
                let field2 = InputRecordField("count", CWLType.Int)
                let original = CWLType.Record (InputRecordSchema(fields = ListEntry.ofValues [field1; field2]))
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "Record should round-trip correctly"
            )
            testCase "Enum_RoundTrip" (fun () ->
                let original = CWLType.Enum (InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "red"; SchemaSaladString.Literal "green"; SchemaSaladString.Literal "blue" ]))
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "Enum should round-trip correctly"
            )
            testCase "OptionalString_RoundTrip" (fun () ->
                let original = Helper.CWL.optional CWLType.String
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                Expect.equal decoded original "Optional string should round-trip correctly"
            )
        ]
    ]

let tests_AdditionalTypeEncoding =
    testList "AdditionalTypeEncoding" [
        testList "SimpleTypes_UseYAML" [
            testCase "String" (fun () ->
                let t = CWLType.String
                let encoded = WorkflowConversion.composeAdditionalType t
                Expect.equal encoded "string" "String should encode to YAML shorthand"
            )
            testCase "Int" (fun () ->
                let t = CWLType.Int
                let encoded = WorkflowConversion.composeAdditionalType t
                Expect.equal encoded "int" "Int should encode to YAML shorthand"
            )
            testCase "File" (fun () ->
                let t = CWLType.File
                let encoded = WorkflowConversion.composeAdditionalType t
                Expect.equal encoded "File" "File should encode to YAML shorthand"
            )
            testCase "FileArray" (fun () ->
                let t = Helper.CWL.array (CWLType.File)
                let encoded = WorkflowConversion.composeAdditionalType t
                Expect.equal encoded "File[]" "File[] should encode to YAML shorthand"
            )
            testCase "StringArray" (fun () ->
                let t = Helper.CWL.array CWLType.String
                let encoded = WorkflowConversion.composeAdditionalType t
                Expect.equal encoded "string[]" "string[] should encode to YAML shorthand"
            )
            testCase "NestedFileArray" (fun () ->
                let t = Helper.CWL.array (Helper.CWL.array (CWLType.File))
                let encoded = WorkflowConversion.composeAdditionalType t
                Expect.equal encoded "File[][]" "File[][] should encode to YAML shorthand"
            )
            testCase "DeeplyNestedStringArray" (fun () ->
                let t = Helper.CWL.array (Helper.CWL.array (Helper.CWL.array CWLType.String))
                let encoded = WorkflowConversion.composeAdditionalType t
                Expect.equal encoded "string[][][]" "string[][][] should encode to YAML shorthand"
            )
        ]
        testList "ComplexTypes_UseYAML" [
            testCase "Record" (fun () ->
                let t = Helper.CWL.record [
                    Helper.CWL.field "name" CWLType.String
                    Helper.CWL.field "age" CWLType.Int
                ]
                let encoded = WorkflowConversion.composeAdditionalType t
                let expected = "{type: record, fields: [{name: name, type: string}, {name: age, type: int}]}"
                Expect.equal encoded expected "Record should encode to single-line YAML"
            )
            testCase "Enum" (fun () ->
                let schema = InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "option1"; SchemaSaladString.Literal "option2"; SchemaSaladString.Literal "option3" ])
                let t = CWLType.Enum schema
                let encoded = WorkflowConversion.composeAdditionalType t
                let expected = "{type: enum, symbols: [option1, option2, option3]}"
                Expect.equal encoded expected "Enum should encode to single-line YAML"
            )
            testCase "RecordArray" (fun () ->
                let field = InputRecordField("value", CWLType.String)
                let recordSchema = InputRecordSchema(fields = ListEntry.ofValues [field])
                let arraySchema = InputArraySchema(CWLType.Record recordSchema)
                let t = CWLType.Array arraySchema
                let encoded = WorkflowConversion.composeAdditionalType t
                let expected = "{type: array, items: {type: record, fields: [{name: value, type: string}]}}"
                Expect.equal encoded expected "Array of records should encode to single-line YAML"
            )
            testCase "EnumArray" (fun () ->
                let enumSchema = InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "A"; SchemaSaladString.Literal "B"; SchemaSaladString.Literal "C" ])
                let arraySchema = InputArraySchema(CWLType.Enum enumSchema)
                let t = CWLType.Array arraySchema
                let encoded = WorkflowConversion.composeAdditionalType t
                let expected = "{type: array, items: {type: enum, symbols: [A, B, C]}}"
                Expect.equal encoded expected "Array of enums should encode to single-line YAML"
            )
        ]
        testList "OptionalTypes" [
            testCase "OptionalString_YAML" (fun () ->
                let t = CWLType.Union (ResizeArray [CWLType.Null; CWLType.String])
                let encoded = WorkflowConversion.composeAdditionalType t
                Expect.equal encoded "string?" "Optional string should use YAML shorthand"
            )
            testCase "OptionalFileArray_YAML" (fun () ->
                let fileArray = InputArraySchema(CWLType.File)
                let t = CWLType.Union (ResizeArray [CWLType.Null; CWLType.Array fileArray])
                let encoded = WorkflowConversion.composeAdditionalType t
                Expect.equal encoded "File[]?" "Optional File[] should use YAML shorthand"
            )
            testCase "OptionalRecord_YAML" (fun () ->
                let field = InputRecordField("name", CWLType.String)
                let recordSchema = InputRecordSchema(fields = ListEntry.ofValues [field])
                let t = CWLType.Union (ResizeArray [CWLType.Null; CWLType.Record recordSchema])
                let encoded = WorkflowConversion.composeAdditionalType t
                let expected = "[\"null\", {type: record, fields: [{name: name, type: string}]}]"
                Expect.equal encoded expected "Optional record should use single-line YAML encoding"
            )
        ]
        testList "EdgeCases" [
            testCase "EmptyRecord" (fun () ->
                let schema = InputRecordSchema()
                let t = CWLType.Record schema
                let encoded = WorkflowConversion.composeAdditionalType t
                let expected = "{type: record, fields: []}"
                Expect.equal encoded expected "Empty record should encode to single-line YAML"
            )
            testCase "SingleSymbolEnum" (fun () ->
                let schema = InputEnumSchema(ResizeArray [ SchemaSaladString.Literal "only" ])
                let t = CWLType.Enum schema
                let encoded = WorkflowConversion.composeAdditionalType t
                let expected = "{type: enum, symbols: [only]}"
                Expect.equal encoded expected "Single symbol enum should encode to single-line YAML"
            )
        ]
        testList "Integration_WithFormalParameter" [
            testCase "RecordInput_CreatesValidFormalParameter" (fun () ->
                let field = InputRecordField("name", CWLType.String)
                let recordSchema = InputRecordSchema(fields = ListEntry.ofValues [field])
                let t = CWLType.Record recordSchema
                let input = CWL.CWLInput(name = "MyRecordInput", type_ = t)
                let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                let name' = Expect.wantSome (LDFormalParameter.tryGetNameAsString formalParameter) "FormalParameter should have a name"
                Expect.equal name' "MyRecordInput" "FormalParameter name should match input name"
                Expect.isTrue (formalParameter.AdditionalType.Count > 0) "FormalParameter should have additionalType"
                let additionalType = formalParameter.AdditionalType.[0]
                let expected = "{type: record, fields: [{name: name, type: string}]}"
                Expect.equal additionalType expected "AdditionalType should be single-line YAML-encoded record"
            )
            testCase "FileArrayInput_CreatesValidFormalParameter" (fun () ->
                let arraySchema = InputArraySchema(CWLType.File)
                let t = CWLType.Array arraySchema
                let input = CWL.CWLInput(name = "MyFileArrayInput", type_ = t)
                let formalParameter = WorkflowConversion.composeFormalParameterFromInput input
                let additionalType = formalParameter.AdditionalType.[0]
                Expect.equal additionalType "File[]" "AdditionalType should be YAML-encoded File[]"
            )
        ]
    ]

let tests_AdditionalTypeErrorHandling =
    testList "AdditionalType_ErrorHandling" [
        testList "MalformedInput" [
            testCase "InvalidYAML_ShouldThrow" (fun () ->
                // Unclosed flow sequence bracket is invalid YAML syntax
                let invalidYaml = "{type: record, fields: [unclosed"
                Expect.throws 
                    (fun () -> WorkflowConversion.decomposeAdditionalType invalidYaml |> ignore)
                    "Should throw on invalid YAML syntax"
            )
            testCase "UnknownName_IsReference" (fun () ->
                // The text has no schema around it, so a name that is no builtin type names a type that a SchemaDefRequirement defines.
                let unknownType = "unknownPrimitiveType"
                Expect.equal
                    (WorkflowConversion.decomposeAdditionalType unknownType)
                    (CWLType.Reference unknownType)
                    "Should read an unknown type string as a reference"
            )
            testCase "RecordWithoutFieldsProperty_CreatesEmptyRecord" (fun () ->
                // CWL specification allows records without explicit 'fields' property
                let recordWithoutFields = "{type: record}"
                let result = WorkflowConversion.decomposeAdditionalType recordWithoutFields
                match result with
                | CWLType.Record schema -> 
                    Expect.isTrue (schema.Fields.IsNone || (InputRecordSchema.getFieldValuesOrEmpty schema).Count = 0) "Should create empty record"
                | _ -> failwith "Should decode as record type"
            )
            testCase "MalformedEnum_MissingSymbols" (fun () ->
                let malformedEnum = "{type: enum}"  // Missing required 'symbols' property
                Expect.throws 
                    (fun () -> WorkflowConversion.decomposeAdditionalType malformedEnum |> ignore)
                    "Should throw on enum without symbols"
            )
        ]
        testList "EdgeCases" [
            testCase "DeeplyNestedRecord" (fun () ->
                // Test record containing a field that is itself a record
                let innerRecord = Helper.CWL.record [Helper.CWL.field "innerField" CWLType.String]
                let original = Helper.CWL.record [Helper.CWL.field "outerField" innerRecord]
                
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                
                Expect.equal decoded original "Deeply nested record should round-trip correctly"
            )
            testCase "UnicodeInFieldNames" (fun () ->
                let original = Helper.CWL.record [
                    Helper.CWL.field "名前" CWLType.String
                    Helper.CWL.field "年齢" CWLType.Int
                ]
                
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                
                Expect.equal decoded original "Unicode field names should round-trip correctly"
            )
            testCase "UnicodeInEnumSymbols" (fun () ->
                let original = Helper.CWL.enum ["オプション1"; "オプション2"; "オプション3"]
                
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                
                Expect.equal decoded original "Unicode enum symbols should round-trip correctly"
            )
            testCase "EmptyEnumSymbols_RoundTrip" (fun () ->
                let schema = InputEnumSchema(ResizeArray [])
                let original = CWLType.Enum schema
                
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded

                Expect.equal decoded original "Empty enum should round-trip correctly"
            )
            testCase "NullInNonOptionalUnion" (fun () ->
                // Test union with null not in optional position: [String, Null, Int]
                let union = ResizeArray [CWLType.String; CWLType.Null; CWLType.Int]
                let original = CWLType.Union union
                
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                
                Expect.equal decoded original "Multi-type union with null should round-trip correctly"
            )
            testCase "ComplexOptionalType_RecordWithNull" (fun () ->
                // Test optional complex type: [Null, Record]
                let original = Helper.CWL.optional (
                    Helper.CWL.record [Helper.CWL.field "field" CWLType.String]
                )
                
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                
                Expect.equal decoded original "Optional record type should round-trip correctly"
            )
            testCase "ArrayOfUnions_RoundTrip" (fun () ->
                // Arrays containing union item types are supported and should round-trip.
                let unionType = CWLType.Union (ResizeArray [CWLType.String; CWLType.Int])
                let arraySchema = InputArraySchema(unionType)
                let original = CWLType.Array arraySchema
                
                let encoded = WorkflowConversion.composeAdditionalType original
                let decoded = WorkflowConversion.decomposeAdditionalType encoded
                
                Expect.equal decoded original "Array of unions should round-trip correctly"
            )
        ]
    ]

let tests_YAMLInputValue =
    
    testList "YAMLInputValue" [
        testCase "SimpleFileInRun" (fun () ->
            let name = "MyInput"
            let runName = "MyRun"
            let filePath = "myfile.txt"
            let paramValue = CWLParameterReference(name, value = CWLParameterValue.File (FileInstance(path = CWL.SchemaSaladString.Literal filePath)))
            let cwlParam = CWL.CWLInput(name = name, type_ = CWLType.File)
            let formalParam = WorkflowConversion.composeFormalParameterFromInput cwlParam
            let file = RunConversion.composeCWLInputValue(paramValue, formalParam, cwlParam, runName)
            Expect.sequenceEqual file.SchemaType [LDFile.schemaType] "File should have correct schema type"
            let expectedID = $"runs/{runName}/{filePath}"
            Expect.equal file.Id expectedID "File ID should match run and file path"
            let name' = Expect.wantSome (LDFile.tryGetNameAsString file) "File should have a name"
            Expect.equal name' expectedID "File name should match run and file path"
            let exampleOfWork = Expect.wantSome (LDFile.tryGetExampleOfWork file) "File should have an exampleOfWork"
            //let exampleOfWorkAsRef : LDRef = Expect.wantSome (tryLDRef exampleOfWork) "ExampleOfWork should be a reference"
            let exampleOfWorkAsNode : LDNode = Expect.wantSome (tryLDNode exampleOfWork) "ExampleOfWork should be a node"
            Expect.equal exampleOfWorkAsNode.Id formalParam.Id "ExampleOfWork ID should match name"
        )
        testCase "SimpleFileInRun_Roundabout" (fun () ->
            let name = "MyInput"
            let runName = "MyRun"
            let filePath = "myfile.txt"
            let paramValue = CWLParameterReference(name, value = CWLParameterValue.File (FileInstance(path = CWL.SchemaSaladString.Literal filePath)))
            let cwlParam = CWL.CWLInput(name = name, type_ = CWLType.File)
            let formalParam = WorkflowConversion.composeFormalParameterFromInput cwlParam
            let file = RunConversion.composeCWLInputValue(paramValue, formalParam, cwlParam, runName)
            let paramValue' = RunConversion.decomposeCWLInputValue(file,runName)
            Expect.equal paramValue' paramValue "Parameter value should match after roundabout"
        )
        testCase "SimpleFileInRun_IsAFileObjectInTheJobFile" (fun () ->
            let name = "MyInput"
            let runName = "MyRun"
            let cwlParam = CWL.CWLInput(name = name, type_ = CWLType.File)
            let formalParam = WorkflowConversion.composeFormalParameterFromInput cwlParam
            let paramValue = CWLParameterReference(name, value = CWLParameterValue.File (FileInstance(path = CWL.SchemaSaladString.Literal "myfile.txt")))
            let file = RunConversion.composeCWLInputValue(paramValue, formalParam, cwlParam, runName)
            let decomposed = RunConversion.decomposeCWLInputValue(file, runName)
            let jobFile = CWL.Encode.encodeYAMLParameterFile (ResizeArray [ decomposed ])
            Expect.equal jobFile "MyInput:\n  class: File\n  path: myfile.txt\n" "The job file gives the file as a File object, which is what cwltool accepts for a File input."
        )
        testCase "SimpleFileInAssay" (fun () ->
            let name = "MyInput"
            let runName = "MyRun"
            let fileName = "myfile.txt"
            let filePath = $"../../assays/MyAssay/dataset/{fileName}"
            let paramValue = CWLParameterReference(name, value = CWLParameterValue.File (FileInstance(path = CWL.SchemaSaladString.Literal filePath)))
            let cwlParam = CWL.CWLInput(name = name, type_ = CWLType.File)
            let formalParam = WorkflowConversion.composeFormalParameterFromInput cwlParam
            let file = RunConversion.composeCWLInputValue(paramValue, formalParam, cwlParam, runName)
            Expect.sequenceEqual file.SchemaType [LDFile.schemaType] "File should have correct schema type"
            let expectedID = $"assays/MyAssay/dataset/{fileName}"
            Expect.equal file.Id expectedID "File ID should match run and file path"
            let name' = Expect.wantSome (LDFile.tryGetNameAsString file) "File should have a name"
            Expect.equal name' expectedID "File name should match run and file path"
            let exampleOfWork = Expect.wantSome (LDFile.tryGetExampleOfWork file) "File should have an exampleOfWork"
            //let exampleOfWorkAsRef : LDRef = Expect.wantSome (tryLDRef exampleOfWork) "ExampleOfWork should be a reference"
            let exampleOfWorkAsNode : LDNode = Expect.wantSome (tryLDNode exampleOfWork) "ExampleOfWork should be a node"
            Expect.equal exampleOfWorkAsNode.Id formalParam.Id "ExampleOfWork ID should match name"
        )
        testCase "SimpleFileInAssay_Roundabout" (fun () ->
            let name = "MyInput"
            let runName = "MyRun"
            let fileName = "myfile.txt"
            let filePath = $"../../assays/MyAssay/dataset/{fileName}"
            let paramValue = CWLParameterReference(name, value = CWLParameterValue.File (FileInstance(path = CWL.SchemaSaladString.Literal filePath)))
            let cwlParam = CWL.CWLInput(name = name, type_ = CWLType.File)
            let formalParam = WorkflowConversion.composeFormalParameterFromInput cwlParam
            let file = RunConversion.composeCWLInputValue(paramValue, formalParam, cwlParam, runName)
            let paramValue' = RunConversion.decomposeCWLInputValue(file,runName)
            Expect.equal paramValue' paramValue "Parameter value should match after roundabout"
        )
        testCase "SimpleString" (fun () ->
            let name = "MyInput"
            let runName = "MyRun"
            let value = ResizeArray.singleton "verbose"
            let paramValue = CWLParameterReference(name, values = value)
            let cwlParam = CWL.CWLInput(name = name, type_ = CWLType.String)
            let formalParam = WorkflowConversion.composeFormalParameterFromInput cwlParam
            let propValue = RunConversion.composeCWLInputValue(paramValue, formalParam, cwlParam, runName)
            Expect.sequenceEqual propValue.SchemaType [LDPropertyValue.schemaType] "PropertyValue should have correct schema type"
            Expect.isTrue (propValue.Id.StartsWith("#")) "PropertyValue ID should start with #"
            let name' = Expect.wantSome (LDPropertyValue.tryGetNameAsString propValue) "PropertyValue should have a name"
            Expect.equal name' name "PropertyValue name should match input name"
            let value' = Expect.wantSome (LDPropertyValue.tryGetValueAsString propValue) "PropertyValue should have a value"
            Expect.equal value' "verbose" "PropertyValue value should match input value"
            let exampleOfWork = Expect.wantSome (LDPropertyValue.tryGetExampleOfWork propValue) "PropertyValue should have an exampleOfWork"
            // Previously was LDRef, but now a node
            //let exampleOfWorkAsRef : LDRef = Expect.wantSome (tryLDRef exampleOfWork) "ExampleOfWork should be a reference"
            //Expect.equal exampleOfWorkAsRef.Id formalParam.Id "ExampleOfWork ID should match name"
            let exampleOfWorkAsNode : LDNode = Expect.wantSome (tryLDNode exampleOfWork) "ExampleOfWork should be a node"
            Expect.equal exampleOfWorkAsNode.Id formalParam.Id "ExampleOfWork ID should match name"
        )
        testCase "SimpleArray" (fun () ->
            let name = "MyInput"
            let runName = "MyRun"
            let value = ["a"; "b"]
            let paramValue = CWLParameterReference(name, values = ResizeArray value)
            let cwlParam = CWL.CWLInput(name = name, type_ = CWLType.Array(InputArraySchema(CWLType.String)))
            let formalParam = WorkflowConversion.composeFormalParameterFromInput cwlParam
            let propValue = RunConversion.composeCWLInputValue(paramValue, formalParam, cwlParam, runName)
            Expect.sequenceEqual propValue.SchemaType [LDPropertyValue.schemaType] "PropertyValue should have correct schema type"
            Expect.isTrue (propValue.Id.StartsWith("#")) "PropertyValue ID should start with #"
            let name' = Expect.wantSome (LDPropertyValue.tryGetNameAsString propValue) "PropertyValue should have a name"
            Expect.equal name' name "PropertyValue name should match input name"
            let values = LDPropertyValue.getValueObjects propValue |> Seq.cast<string> |> ResizeArray
            Expect.sequenceEqual values (ResizeArray value) "PropertyValue value should preserve the input array"
        )
        testCase "SimpleArray_DifferentSeparator" (fun () ->
            let name = "MyInput"
            let runName = "MyRun"
            let value = ["a"; "b"]
            let inputBinding = CWL.InputBinding.create(itemSeparator = (SchemaSaladString.Literal ";"))
            let paramValue = CWLParameterReference(name, values = ResizeArray value)
            let cwlParam = CWL.CWLInput(name = name, type_ = CWLType.Array(InputArraySchema(CWLType.String)), inputBinding = CWL.ListEntry.Value inputBinding)
            let formalParam = WorkflowConversion.composeFormalParameterFromInput cwlParam
            let propValue = RunConversion.composeCWLInputValue(paramValue, formalParam, cwlParam, runName)
            Expect.sequenceEqual propValue.SchemaType [LDPropertyValue.schemaType] "PropertyValue should have correct schema type"
            Expect.isTrue (propValue.Id.StartsWith("#")) "PropertyValue ID should start with #"
            let name' = Expect.wantSome (LDPropertyValue.tryGetNameAsString propValue) "PropertyValue should have a name"
            Expect.equal name' name "PropertyValue name should match input name"
            let values = LDPropertyValue.getValueObjects propValue |> Seq.cast<string> |> ResizeArray
            Expect.sequenceEqual values (ResizeArray value) "PropertyValue value should preserve the input array independently of itemSeparator"
        )
        testCase "NestedFileArray_Roundtrip" (fun () ->
            let name = "sampleRecordFiles"
            let runName = "kallisto"
            let createFile path =
                CWL.FileInstance(path = CWL.SchemaSaladString.Literal path, format = CWL.SchemaSaladString.Literal "edam:format_1930")
            let value =
                CWL.CWLParameterValue.Array (ResizeArray [
                    CWL.CWLParameterValue.Array (ResizeArray [
                        CWL.CWLParameterValue.File (createFile "../../assays/RNASeq/dataset/DB_097.fastq.gz")
                    ])
                    CWL.CWLParameterValue.Array (ResizeArray [
                        CWL.CWLParameterValue.File (createFile "../../assays/RNASeq/dataset/DB_163.fastq.gz")
                    ])
                ])
            let paramValue = CWLParameterReference(name, value = value)
            let cwlParam =
                CWL.CWLInput(
                    name = name,
                    type_ = CWLType.Array (InputArraySchema(CWLType.Array (InputArraySchema(CWLType.File))))
                )
            let formalParam = WorkflowConversion.composeFormalParameterFromInput(cwlParam, runName = runName)
            let node = RunConversion.composeCWLInputValue(paramValue, formalParam, cwlParam, runName)
            let graph = node.Flatten()
            let roundTripped = RunConversion.decomposeCWLInputValue(node, runName, graph = graph)
            Expect.equal roundTripped paramValue "Nested file arrays should roundtrip through RO-Crate."
        )
        testCase "FileWithDirectivePathOrLocation_CountsAsAbsent" (fun () ->
            let runName = "MyRun"
            let literal (text: string) = CWL.SchemaSaladString.Literal text
            let fileId (file: CWL.FileInstance) = (RunConversion.composeCWLParameterValue(CWLParameterValue.File file, runName) :?> LDNode).Id
            let directoryId (directory: CWL.DirectoryInstance) = (RunConversion.composeCWLParameterValue(CWLParameterValue.Directory directory, runName) :?> LDNode).Id
            Expect.equal (fileId (CWL.FileInstance(path = CWL.SchemaSaladString.Import "p.yml", location = literal "data/a.txt"))) (fileId (CWL.FileInstance(location = literal "data/a.txt"))) "A path that is a directive has no text, so the location names the file."
            Expect.equal (fileId (CWL.FileInstance(path = CWL.SchemaSaladString.Include "p.txt", location = CWL.SchemaSaladString.Import "l.yml"))) (fileId (CWL.FileInstance())) "Two directives count as a File without a path and a location."
            Expect.equal (directoryId (CWL.DirectoryInstance(path = CWL.SchemaSaladString.Import "p.yml", location = literal "data"))) (directoryId (CWL.DirectoryInstance(location = literal "data"))) "A Directory reads the same way."
            Expect.equal (directoryId (CWL.DirectoryInstance(location = CWL.SchemaSaladString.Include "l.txt"))) (directoryId (CWL.DirectoryInstance())) "A directive as the only name counts as absent."
        )
        testCase "FileWithDirectiveFormat_HasNoEncodingFormat" (fun () ->
            let name = "MyInput"
            let runName = "MyRun"
            let fileOf (format: CWL.SchemaSaladString) = CWL.FileInstance(path = CWL.SchemaSaladString.Literal "myfile.txt", format = format)
            let compose (format: CWL.SchemaSaladString) =
                let paramValue = CWLParameterReference(name, value = CWLParameterValue.File (fileOf format))
                let cwlParam = CWL.CWLInput(name = name, type_ = CWLType.File)
                let formalParam = WorkflowConversion.composeFormalParameterFromInput cwlParam
                RunConversion.composeCWLInputValue(paramValue, formalParam, cwlParam, runName)
            Expect.equal (LDFile.tryGetEncodingFormatAsString (compose (CWL.SchemaSaladString.Literal "text/csv"))) (Some "text/csv") "A literal format is the encoding format."
            Expect.isNone (LDFile.tryGetEncodingFormatAsString (compose (CWL.SchemaSaladString.Import "format.yml"))) "A format given by an import has no text, so the file has no encoding format."
            Expect.isNone (LDFile.tryGetEncodingFormatAsString (compose (CWL.SchemaSaladString.Include "format.txt"))) "A format given by an include has no text, so the file has no encoding format."
            let value = CWLParameterValue.Array (ResizeArray [ CWLParameterValue.File (fileOf (CWL.SchemaSaladString.Import "format.yml")) ])
            let composed = RunConversion.composeCWLParameterValue(value, runName)
            let files = composed :?> ResizeArray<obj>
            Expect.isNone (LDFile.tryGetEncodingFormatAsString (files.[0] :?> LDNode)) "A file in an array has no encoding format either."
        )
        testCase "ReferenceTypedRecord_Roundtrip" (fun () ->
            let name = "start"
            let runName = "plot"
            let value =
                CWL.CWLParameterValue.Record (ResizeArray [
                    CWL.CWLParameterRecordField("x", CWL.CWLParameterValue.Int 1L)
                    CWL.CWLParameterRecordField("y", CWL.CWLParameterValue.Int 2L)
                ])
            let paramValue = CWLParameterReference(name, value = value)
            let cwlParam = CWL.CWLInput(name = name, type_ = CWLType.Reference "Point")
            let formalParam = WorkflowConversion.composeFormalParameterFromInput(cwlParam, runName = runName)
            let node = RunConversion.composeCWLInputValue(paramValue, formalParam, cwlParam, runName)
            let graph = node.Flatten()
            let roundTripped = RunConversion.decomposeCWLInputValue(node, runName, graph = graph)
            Expect.equal roundTripped.Value paramValue.Value "A value of a referenced type should roundtrip through RO-Crate."
        )
    ]

let tests_ToolDescription =
    testList "ToolDescription" [
        testCase "BasicTool" (fun () ->
            let filePath = "tools/echo.cwl"
            let toolDescription = TestObjects.CWL.CommandLineTool.Basic.cwlTool
            let ro_Tool = WorkflowConversion.composeWorkflowProtocolFromToolDescription(filePath,toolDescription)
            Expect.equal ro_Tool.Id filePath "Tool ID should match file path"

            let inputs = LDComputationalWorkflow.getInputs ro_Tool
            Expect.hasLength inputs 2 "Tool should have one input"
            let input1 = inputs.[0]
            let input1Name = Expect.wantSome (LDFormalParameter.tryGetNameAsString input1) "Input should have a name"
            Expect.equal input1Name TestObjects.CWL.Inputs.File.inputFileName "Input name should match"
            Expect.sequenceEqual input1.SchemaType [LDFormalParameter.schemaType] "Input should have correct schema type"
            let input1Ids = LDFormalParameter.getIdentifiers input1
            Expect.hasLength input1Ids 2 "Input should have two identifiers"
            let input1Position = input1Ids |> Seq.tryPick LDPropertyValue.tryGetAsPosition |> fun p -> Expect.wantSome p "Should have a position identifier"
            Expect.equal input1Position TestObjects.CWL.Inputs.File.inputFilePosition "Position should match input binding"
            let input1Prefix = input1Ids |> Seq.tryPick LDPropertyValue.tryGetAsPrefix |> fun p -> Expect.wantSome p "Should have a prefix identifier"
            Expect.equal input1Prefix TestObjects.CWL.Inputs.File.inputFilePrefix "Prefix should match input binding"
            let input2 = inputs.[1]
            let input2Name = Expect.wantSome (LDFormalParameter.tryGetNameAsString input2) "Input should have a name"
            Expect.equal input2Name TestObjects.CWL.Inputs.String.inputStringName "Input name should match"
            Expect.sequenceEqual input2.SchemaType [LDFormalParameter.schemaType] "Input should have correct schema type"
            let input2Ids = LDFormalParameter.getIdentifiers input2
            Expect.hasLength input2Ids 1 "Input should have one identifier"
            let input2Position = input2Ids |> Seq.tryPick LDPropertyValue.tryGetAsPosition |> fun p -> Expect.wantSome p "Should have a position identifier"
            Expect.equal input2Position TestObjects.CWL.Inputs.String.inputStringPosition "Position should match input binding"

            let outputs = LDComputationalWorkflow.getOutputs ro_Tool
            Expect.hasLength outputs 1 "Tool should have one output"
            let output1 = outputs.[0]
            let output1Name = Expect.wantSome (LDFormalParameter.tryGetNameAsString output1) "Output should have a name"
            Expect.equal output1Name TestObjects.CWL.Outputs.CSV.outputCSVName "Output name should match"
            let output1Ids = LDFormalParameter.getIdentifiers output1
            Expect.hasLength output1Ids 1 "Output should have one identifier"
            let output1Glob = output1Ids |> Seq.tryPick LDPropertyValue.tryGetAsGlob |> fun p -> Expect.wantSome p "Should have a glob identifier"
            Expect.equal output1Glob TestObjects.CWL.Outputs.CSV.outputCSVGlobStr "Glob should match output binding"
        )
        testCase "BasicTool_Roundabout" (fun () ->
            let filePath = "tools/echo.cwl"
            let toolDescription = TestObjects.CWL.CommandLineTool.Basic.cwlTool
            let ro_Tool = WorkflowConversion.composeWorkflowProtocolFromToolDescription(filePath,toolDescription)
            let toolDescription' = WorkflowConversion.decomposeWorkflowProtocolToToolDescription ro_Tool
            Expect.equal toolDescription' toolDescription "Tool description should match after roundabout"
        )
        testCase "The layout of the doc and the label of a tool stays out of the RO-Crate" (fun () ->
            let tool (texts: string) =
                CWL.Decode.decodeCommandLineTool ("cwlVersion: v1.2\nclass: CommandLineTool\n" + texts + "baseCommand: [echo]\ninputs:\n  x:\n    type: string\noutputs: {}\n")
            let folded = tool """label: first part of the name
  second part of the name
doc: >-
  first line of the text
  second line of the text
"""
            let oneLine = tool """label: first part of the name second part of the name
doc: first line of the text second line of the text
"""
            Expect.equal folded oneLine "The tools should be equal whatever the layout of their texts."
            let json (tool: CWL.CWLToolDescription) =
                WorkflowConversion.composeWorkflowProtocolFromToolDescription("tools/echo.cwl", tool, workflowName = "workflow", runName = "run")
                |> Json.LDNode.encoder
                |> Json.Encode.toJsonString 0
            Expect.equal (json folded) (json oneLine) "The RO-Crate should not depend on the layout of the texts."
            Expect.isFalse ((json folded).ToLowerInvariant().Contains "sourcenodes") "The nodes of the texts should not reach the RO-Crate."
        )
    ]

let tests_ProcessOfUnknownClass =
    let unknown () =
        CWL.Decode.decodeCWLProcessingUnit "cwlVersion: v1.2\nclass: {$import: s.yml}\nbaseCommand: echo\ninputs: {a: string}\noutputs: {o: string}\n"
    testList "ProcessOfUnknownClass" [
        testCase "ComposeProtocol_HoldsAToolWithoutPorts" (fun () ->
            let protocol = WorkflowConversion.composeWorkflowProtocolFromProcessingUnit("tools/unknown.cwl", unknown ())
            Expect.equal (LDComputationalWorkflow.getAdditionalTypeAsString protocol) WorkflowConversion.toolDescriptionTypeName "The crate holds the process as a tool, like a protocol of unknown kind."
            Expect.equal (LDComputationalWorkflow.getInputsAsFormalParameters protocol).Count 0 "The crate holds no inputs, because the model does not show them."
            Expect.equal (LDComputationalWorkflow.getOutputsAsFormalParameter protocol).Count 0 "The crate holds no outputs, because the model does not show them."
            match WorkflowConversion.decomposeWorkflowProtocolToProcessingUnit(protocol) with
            | CWL.CommandLineTool tool -> Expect.equal (CWL.ListEntry.values tool.Outputs).Count 0 "The protocol reads back as a tool without ports."
            | other -> failwith $"Expected a tool but got %A{other}"
        )
        testCase "ArcRun_ComposesAndDecomposesWithoutThePorts" (fun () ->
            let run = ArcRun(identifier = "UnknownRun", cwlDescription = unknown ())
            let roRun = RunConversion.composeRun run
            match (RunConversion.decomposeRun roRun).CWLDescription with
            | Some (CWL.CommandLineTool tool) -> Expect.equal (CWL.CWLToolDescription.getOutputValues tool).Count 0 "The run reads back with a tool without ports."
            | other -> failwith $"Expected a tool but got %A{other}"
        )
        testCase "ArcWorkflow_ComposesAndDecomposesWithoutThePorts" (fun () ->
            let workflow = ArcWorkflow(identifier = "UnknownWorkflow", cwlDescription = unknown ())
            match (WorkflowConversion.decomposeWorkflow (WorkflowConversion.composeWorkflow workflow)).CWLDescription with
            | Some (CWL.CommandLineTool tool) -> Expect.equal (CWL.CWLToolDescription.getOutputValues tool).Count 0 "The workflow reads back with a tool without ports."
            | other -> failwith $"Expected a tool but got %A{other}"
        )
    ]

let tests_OperationDescription =
    testList "OperationDescription" [
        testCase "ComposeDecompose_Roundtrip_ProcessingUnit" (fun () ->
            let operation = CWL.Decode.decodeOperation TestObjects.CWL.Operation.minimalOperationFile
            let processingUnit = CWL.Operation operation
            let protocol =
                WorkflowConversion.composeWorkflowProtocolFromProcessingUnit(
                    "workflows/OperationWorkflow/operation.cwl",
                    processingUnit
                )
            let additionalType = LDComputationalWorkflow.getAdditionalTypeAsString protocol
            Expect.equal additionalType WorkflowConversion.operationDescriptionTypeName "Operation additionalType should be set"

            let roundTripped = WorkflowConversion.decomposeWorkflowProtocolToProcessingUnit(protocol)
            match roundTripped with
            | CWL.Operation rt ->
                Expect.equal (CWL.ListEntry.values rt.Inputs).Count (CWL.ListEntry.values operation.Inputs).Count "Input count should survive roundtrip"
                Expect.equal (CWL.ListEntry.values rt.Outputs).Count (CWL.ListEntry.values operation.Outputs).Count "Output count should survive roundtrip"
                Expect.equal (CWL.ListEntry.values rt.Inputs).[0].Name (CWL.ListEntry.values operation.Inputs).[0].Name "Input name should survive roundtrip"
                Expect.equal (CWL.ListEntry.values rt.Outputs).[0].Name (CWL.ListEntry.values operation.Outputs).[0].Name "Output name should survive roundtrip"
            | other ->
                Expect.isTrue false $"Expected Operation but got %A{other}"
        )
        testCase "ArcWorkflow_Roundtrip_PreservesOperationVariant" (fun () ->
            let operation = CWL.Decode.decodeOperation TestObjects.CWL.Operation.minimalOperationFile
            let workflow =
                ArcWorkflow(
                    identifier = "OperationWorkflow",
                    cwlDescription = CWL.Operation operation
                )
            let roWorkflow = WorkflowConversion.composeWorkflow workflow
            let roundTripped = WorkflowConversion.decomposeWorkflow roWorkflow
            match roundTripped.CWLDescription with
            | Some (CWL.Operation rt) ->
                Expect.equal rt.Inputs.Count operation.Inputs.Count "Operation inputs should survive ArcWorkflow roundtrip"
                Expect.equal rt.Outputs.Count operation.Outputs.Count "Operation outputs should survive ArcWorkflow roundtrip"
            | other ->
                Expect.isTrue false $"Expected Operation CWL description but got %A{other}"
        )
        testCase "ArcRun_Roundtrip_PreservesOperationVariant" (fun () ->
            let operation = CWL.Decode.decodeOperation TestObjects.CWL.Operation.minimalOperationFile
            let runInput =
                CWL.CWLParameterReference(
                    key = "input",
                    values = ResizeArray [ "value" ]
                )
            let run =
                ArcRun(
                    identifier = "OperationRun",
                    cwlDescription = CWL.Operation operation,
                    cwlInput = ResizeArray [ runInput ]
                )
            let roRun = RunConversion.composeRun run
            let roundTripped = RunConversion.decomposeRun roRun
            match roundTripped.CWLDescription with
            | Some (CWL.Operation rt) ->
                Expect.equal rt.Inputs.Count operation.Inputs.Count "Operation inputs should survive ArcRun roundtrip"
                Expect.equal rt.Outputs.Count operation.Outputs.Count "Operation outputs should survive ArcRun roundtrip"
            | other ->
                Expect.isTrue false $"Expected Operation CWL description but got %A{other}"
        )
    ]

let tests_WorkflowInvocation =
    testList "WorkflowInvocation" [
        testCase "OnlyCWL_BasicToolDescription" (fun () ->
            let inputValues = ResizeArray [
                TestObjects.CWL.YAMLParameterFile.File.fileParameterReference
                TestObjects.CWL.YAMLParameterFile.String.stringParameterReference
            ]
            let run = ArcRun(
                identifier = "MyRun",
                cwlDescription = TestObjects.CWL.CommandLineTool.Basic.processingUnit,
                cwlInput = inputValues
            )
            let workflowInvocation = Expect.wantExactlyOne (RunConversion.composeWorkflowInvocationFromArcRun run) "Should have one WorkflowInvocation"
            Expect.equal workflowInvocation.Id "#WorkflowInvocation_MyRun" "WorkflowInvocation ID should match"
            Expect.sequenceEqual workflowInvocation.SchemaType [LDCreateAction.schemaType;LDLabProcess.schemaType] "WorkflowInvocation should have correct schema type"

            let objects = LDLabProcess.getObjects workflowInvocation
            Expect.hasLength objects 2 "WorkflowInvocation should have two objects"

            let inputFile = Expect.wantSome (Seq.tryFind LDFile.validateCWLParameter objects) "Should have one input file"
            let expectedFilePath = ArcPathHelper.combineMany [|"runs"; run.Identifier; TestObjects.CWL.YAMLParameterFile.File.filePath|]
            Expect.equal inputFile.Id expectedFilePath "Input file path should match"
            let exampleOfWork = Expect.wantSome (LDFile.tryGetExampleOfWork inputFile) "Input file should have an exampleOfWork"
            //let exampleOfWorkAsRef : LDRef = Expect.wantSome (tryLDRef exampleOfWork) "File ExampleOfWork should be a reference"
            let exampleOfWorkAsNode : LDNode = Expect.wantSome (tryLDNode exampleOfWork) "File ExampleOfWork should be a node"
            Expect.equal exampleOfWorkAsNode.Id $"#FormalParameter_R_{run.Identifier}_{TestObjects.CWL.Inputs.File.inputFileName}" "File ExampleOfWork ID should match input formal parameter"

            let inputString = Expect.wantSome (Seq.tryFind LDPropertyValue.validateCWLParameter objects) "Should have one input string"
            let inputStringValue = Expect.wantSome (LDPropertyValue.tryGetValueAsString inputString) "Input string should have a value"
            Expect.equal inputStringValue TestObjects.CWL.YAMLParameterFile.String.stringValue "Input string value should match"
            let exampleOfWork2 = Expect.wantSome (LDPropertyValue.tryGetExampleOfWork inputString) "Input string should have an exampleOfWork"
            //let exampleOfWorkAsRef2 : LDRef = Expect.wantSome (tryLDRef exampleOfWork) "String ExampleOfWork should be a reference"
            let exampleOfWorkAsNode2 : LDNode = Expect.wantSome (tryLDNode exampleOfWork2) "StringExampleOfWork should be a node"
            Expect.equal exampleOfWorkAsNode2.Id $"#FormalParameter_R_{run.Identifier}_{TestObjects.CWL.Inputs.String.inputStringName}" "String ExampleOfWork ID should match input formal parameter"

            let results = LDLabProcess.getResults workflowInvocation
            Expect.hasLength results 0 "WorkflowInvocation should have no results"
            //let outputFile = Expect.wantSome (Seq.tryFind LDFile.validateCWLParameter results) "Should have one output file"
            //let outputFileName = Expect.wantSome (LDFile.tryGetNameAsString outputFile) "Output file should have a name"
            //Expect.equal outputFileName "runs/MyRun/output.csv" "Output file name should match"
        )
    ]

let tests_ArcWorkflow =
    testList "ArcWorkflow" [
        ptestCase "Empty_FromScaffold" (fun () ->
            let p = ArcWorkflow.init("MyWorkflow")
            let ro_Workflow = WorkflowConversion.composeWorkflow p
            let p' = WorkflowConversion.decomposeWorkflow ro_Workflow
            Expect.equal p' p "Assay should match"
        )
        testCase "Full_FromScaffold" (fun () ->
            let workflow = create_workflow_full()
            let ro_Workflow = WorkflowConversion.composeWorkflow workflow
            let workflow' = WorkflowConversion.decomposeWorkflow ro_Workflow
            Expect.equal workflow' workflow "Workflow should match"
        )
        // https://github.com/nfdi4plants/ARCtrl/issues/651
        testCase "WithDatamap_FromScaffold" (fun () ->
            let workflow = create_workflow_full()
            let dc1 = create_dataContextOfRow "MyFile" 1
            let dc2 = create_dataContextOfRow "MyFile" 2
            workflow.Datamap <- Some (Datamap(ResizeArray[dc1;dc2]))
            let ro_Workflow = WorkflowConversion.composeWorkflow workflow
            let workflow' = WorkflowConversion.decomposeWorkflow ro_Workflow
            let dm' = Expect.wantSome workflow'.Datamap "Workflow should have a data map"
            Expect.equal dm' workflow.Datamap.Value "Data map should match"
            Expect.equal workflow' workflow "Workflow should match"
        )
        testCase  "Full_FromScaffold_Flattened" (fun () ->
            let workflow = create_workflow_full()
            let ro_Workflow = WorkflowConversion.composeWorkflow workflow
            let graph = ro_Workflow.Flatten()
            // Test that flattened worked
            Expect.isTrue (graph.Nodes.Count > 0) "Graph should have properties"
            let wf = Expect.wantSome (ro_Workflow.TryGetPropertyAsSingleton(LDDataset.mainEntity)) "ArcWorkflow should still have main workflow"
            Expect.isTrue (wf :? LDRef) "Workflow should be flattened correctly"
            //
            let workflow' = WorkflowConversion.decomposeWorkflow(ro_Workflow, graph = graph)
            Expect.equal workflow' workflow "Workflow should match"
        )
        testCaseCrossAsync "FullWorkflow_File_StepsParsedIntoDatamodel" (crossAsync {
            let fullWorkflowDirectory =
                ArcPathHelper.combineMany [|
                    TestObjects.IO.testSimpleARCWithCWL
                    "workflows"
                    "ProteomIQon"
                |]
            let fullWorkflowPath = ArcPathHelper.combine fullWorkflowDirectory "workflow.cwl"
            let! workflowExists = FileSystemHelper.fileExistsAsync fullWorkflowPath
            Expect.isTrue workflowExists $"Expected workflow fixture at {fullWorkflowPath}"
            let! workflowText = FileSystemHelper.readFileTextAsync fullWorkflowPath
            let processingUnit = CWL.Decode.decodeCWLProcessingUnit (workflowText.TrimEnd())
            let sourceWorkflow =
                let sourceWorkflowOpt =
                    match processingUnit with
                    | CWL.Workflow wf -> Some wf
                    | _ -> None
                Expect.wantSome sourceWorkflowOpt $"Expected full workflow fixture to decode as Workflow but got %A{processingUnit}"

            let workflow =
                ArcWorkflow(
                    identifier = "FullWorkflow",
                    title = "Full Workflow",
                    description = "Full workflow conversion test",
                    cwlDescription = processingUnit
                )

            let ro_Workflow = WorkflowConversion.composeWorkflow workflow
            let workflow' = WorkflowConversion.decomposeWorkflow ro_Workflow

            let parsedWorkflow =
                let parsedWorkflowOpt =
                    match workflow'.CWLDescription with
                    | Some (CWL.Workflow wf) -> Some wf
                    | _ -> None
                Expect.wantSome parsedWorkflowOpt $"Parsed workflow should contain a CWL workflow description but got %A{workflow'.CWLDescription}"

            let getRunId (step: CWL.WorkflowStep) =
                match step.Run with
                | CWL.RunString runPath
                | CWL.RunImport runPath -> runPath
                | CWL.RunMixed raw -> raw.MixinPath
                | CWL.RunRaw _ -> failwith "A process whose class is a directive has no run identifier."
                | CWL.RunCommandLineTool _ -> "inline:CommandLineTool"
                | CWL.RunWorkflow _ -> "inline:Workflow"
                | CWL.RunExpressionTool _ -> "inline:ExpressionTool"
                | CWL.RunOperation _ -> "inline:Operation"

            let sourceStepIds = (CWL.ListEntry.values sourceWorkflow.Steps) |> Seq.map (fun step -> step.Id) |> Seq.toList
            let parsedStepIds = (CWL.ListEntry.values parsedWorkflow.Steps) |> Seq.map (fun step -> step.Id) |> Seq.toList
            Expect.equal parsedStepIds sourceStepIds "Step identifiers should be preserved through workflow conversion"

            // The crate holds the path of each run, so a run written as ./tool.cwl comes back as tool.cwl.
            let sourceRuns = (CWL.ListEntry.values sourceWorkflow.Steps) |> Seq.map (getRunId >> ArcPathHelper.normalize) |> Seq.toList
            let parsedRuns = (CWL.ListEntry.values parsedWorkflow.Steps) |> Seq.map getRunId |> Seq.toList
            Expect.equal parsedRuns sourceRuns "Step run references should name the same files after workflow conversion"

            Expect.equal parsedWorkflow.Steps.Count 12 "Full workflow should contain all expected steps"
        })
        testCaseCrossAsync "FullWorkflow_File_StepsResolveToCommandLineTools_WithContent" (crossAsync {
            let fullWorkflowDirectory =
                ArcPathHelper.combineMany [|
                    TestObjects.IO.testSimpleARCWithCWL
                    "workflows"
                    "ProteomIQon"
                |]
            let fullWorkflowPath = ArcPathHelper.combine fullWorkflowDirectory "workflow.cwl"
            let! workflowExists = FileSystemHelper.fileExistsAsync fullWorkflowPath
            Expect.isTrue workflowExists $"Expected workflow fixture at {fullWorkflowPath}"

            let! workflowText = FileSystemHelper.readFileTextAsync fullWorkflowPath
            let processingUnit = CWL.Decode.decodeCWLProcessingUnit (workflowText.TrimEnd())
            let sourceWorkflow =
                let sourceWorkflowOpt =
                    match processingUnit with
                    | CWL.Workflow wf -> Some wf
                    | _ -> None
                Expect.wantSome sourceWorkflowOpt $"Expected full workflow fixture to decode as Workflow but got %A{processingUnit}"

            let expectedRunPathByStepId =
                (CWL.ListEntry.values sourceWorkflow.Steps)
                |> Seq.map (fun step ->
                    let runPath =
                        let runPathOpt =
                            match step.Run with
                            | CWL.RunString runPath -> Some runPath
                            | _ -> None
                        Expect.wantSome runPathOpt $"Expected source run of step {step.Id} to be a string path but got %A{step.Run}"
                    step.Id, runPath
                )
                |> Map.ofSeq

            let normalizeLocalRunPath (runPath: string) =
                let normalizedPath = runPath |> FileSystemHelper.standardizeSlashes
                if normalizedPath.StartsWith("./") then normalizedPath.Substring(2)
                elif normalizedPath.StartsWith("/") then normalizedPath.Substring(1)
                else normalizedPath

            let loadCommandLineToolFromRunPath (runPath: string) =
                crossAsync {
                    let relativeRunPath = normalizeLocalRunPath runPath
                    let fixtureRunPath = ArcPathHelper.combine fullWorkflowDirectory relativeRunPath
                    let! fixtureExists = FileSystemHelper.fileExistsAsync fixtureRunPath
                    Expect.isTrue fixtureExists $"Expected step run fixture at {fixtureRunPath}"
                    let! runText = FileSystemHelper.readFileTextAsync fixtureRunPath
                    let decodedRun =
                        try
                            CWL.Decode.decodeCWLProcessingUnit (runText.TrimEnd())
                        with ex ->
                            Expect.isTrue false $"Failed to decode run fixture {runPath} ({fixtureRunPath}): {ex.Message}"
                            CWL.CommandLineTool (CWL.CWLToolDescription(outputs = ResizeArray()))
                    let commandLineTool =
                        let commandLineToolOpt =
                            match decodedRun with
                            | CWL.CommandLineTool tool -> Some tool
                            | _ -> None
                        let mismatchMessage =
                            match decodedRun with
                            | CWL.Workflow wf -> $"Expected run fixture {runPath} to decode as CommandLineTool but got Workflow with {wf.Steps.Count} steps"
                            | other -> $"Expected run fixture {runPath} to decode as CommandLineTool but got %A{other}"
                        Expect.wantSome commandLineToolOpt mismatchMessage
                    return commandLineTool
                }

            let workflow =
                ArcWorkflow(
                    identifier = "FullWorkflow",
                    title = "Full Workflow",
                    description = "Full workflow conversion test",
                    cwlDescription = processingUnit
                )

            let ro_Workflow = WorkflowConversion.composeWorkflow workflow
            let graph = ro_Workflow.Flatten()

            let uniqueRunPaths =
                expectedRunPathByStepId
                |> Map.toSeq
                |> Seq.map snd
                |> Seq.distinct
                |> Seq.toList

            let mutable expectedToolByRunPath : Map<string, CWL.CWLToolDescription> = Map.empty

            for runPath in uniqueRunPaths do
                let! expectedTool = loadCommandLineToolFromRunPath runPath
                let relativeRunPath = normalizeLocalRunPath runPath
                let runNodePathInGraph =
                    ArcPathHelper.combineMany [|
                        ArcPathHelper.WorkflowsFolderName
                        workflow.Identifier
                        relativeRunPath
                    |]
                let runNode =
                    WorkflowConversion.composeWorkflowProtocolFromProcessingUnit(
                        runNodePathInGraph,
                        CWL.CommandLineTool expectedTool,
                        workflowName = workflow.Identifier
                    )
                graph.AddNode runNode
                expectedToolByRunPath <- expectedToolByRunPath.Add(runPath, expectedTool)

            let workflow' = WorkflowConversion.decomposeWorkflow(ro_Workflow, graph = graph)

            let parsedWorkflow =
                let parsedWorkflowOpt =
                    match workflow'.CWLDescription with
                    | Some (CWL.Workflow wf) -> Some wf
                    | _ -> None
                Expect.wantSome parsedWorkflowOpt $"Parsed workflow should contain a CWL workflow description but got %A{workflow'.CWLDescription}"

            let inputSignature (input: CWL.CWLInput) =
                input.Name,
                (input.Type_ |> Option.map WorkflowConversion.composeAdditionalType),
                (input.InputBinding |> Option.bind CWL.ListEntry.tryValue |> Option.bind (fun binding -> binding.Prefix)),
                (input.InputBinding |> Option.bind CWL.ListEntry.tryValue |> Option.bind (fun binding -> binding.Position))

            let outputSignature (output: CWL.CWLOutput) =
                output.Name,
                (output.Type_ |> Option.map WorkflowConversion.composeAdditionalType),
                (output.OutputBinding |> Option.bind CWL.ListEntry.tryValue |> Option.bind (fun binding -> binding.Glob))

            let commandLineToolSignatures (tool: CWL.CWLToolDescription) =
                let inputSignatures =
                    CWL.CWLToolDescription.getInputValuesOrEmpty tool
                    |> Seq.map inputSignature
                    |> Seq.sortBy (fun (name, _, _, _) -> name)
                    |> Seq.toList
                let outputSignatures =
                    (CWL.ListEntry.values tool.Outputs)
                    |> Seq.map outputSignature
                    |> Seq.sortBy (fun (name, _, _) -> name)
                    |> Seq.toList
                inputSignatures, outputSignatures

            Expect.equal (CWL.ListEntry.values parsedWorkflow.Steps).Count (CWL.ListEntry.values sourceWorkflow.Steps).Count "All workflow steps should be preserved"

            for parsedStep in (CWL.ListEntry.values parsedWorkflow.Steps) do
                let runPath =
                    let runPathOpt = expectedRunPathByStepId.TryFind parsedStep.Id
                    Expect.wantSome runPathOpt $"Expected source run path for step {parsedStep.Id}"
                let expectedTool =
                    let expectedToolOpt = expectedToolByRunPath.TryFind runPath
                    Expect.wantSome expectedToolOpt $"Expected decoded command line tool for run path {runPath}"

                let parsedTool =
                    let parsedToolOpt =
                        match CWL.WorkflowStep.getResolvedRun parsedStep with
                        | CWL.RunCommandLineTool toolObj ->
                            match toolObj with
                            | :? CWL.CWLToolDescription as parsedTool -> Some parsedTool
                            | _ -> None
                        | _ -> None
                    let mismatchMessage =
                        match CWL.WorkflowStep.getResolvedRun parsedStep with
                        | CWL.RunCommandLineTool toolObj ->
                            $"Expected resolved run of step {parsedStep.Id} to be CWLToolDescription object but got %A{toolObj}"
                        | other ->
                            $"Expected resolved run of step {parsedStep.Id} to be RunCommandLineTool but got %A{other}"
                    Expect.wantSome parsedToolOpt mismatchMessage

                let expectedInputs, expectedOutputs = commandLineToolSignatures expectedTool
                let parsedInputs, parsedOutputs = commandLineToolSignatures parsedTool
                Expect.equal parsedTool.CWLVersion expectedTool.CWLVersion $"CWLVersion should match for step {parsedStep.Id}"
                Expect.equal parsedInputs expectedInputs $"Input signatures should match for step {parsedStep.Id}"
                Expect.equal parsedOutputs expectedOutputs $"Output signatures should match for step {parsedStep.Id}"
        })
        testCase "ExpressionTool_ROCrateComposeDecompose_Roundtrip" (fun () ->
            let expressionTool =
                CWL.Decode.decodeExpressionTool TestObjects.CWL.ExpressionTool.expressionToolWithRequirementsFile
            let processingUnit = CWL.ExpressionTool expressionTool
            let protocol =
                WorkflowConversion.composeWorkflowProtocolFromProcessingUnit(
                    "workflows/ExpressionToolWorkflow/expression.cwl",
                    processingUnit
                )
            let roundTripped =
                WorkflowConversion.decomposeWorkflowProtocolToProcessingUnit(protocol)
            match roundTripped with
            | CWL.ExpressionTool rt ->
                Expect.equal rt.Expression expressionTool.Expression "Expression should survive RO-Crate roundtrip"
                let expectedInputs = expressionTool.Inputs |> Option.defaultValue (ResizeArray())
                let actualInputs = rt.Inputs |> Option.defaultValue (ResizeArray())
                Expect.equal actualInputs.Count expectedInputs.Count "Input count should match after roundtrip"
                Expect.equal rt.Outputs.Count expressionTool.Outputs.Count "Output count should match after roundtrip"
            | other ->
                Expect.isTrue false $"Expected ExpressionTool but got %A{other}"
        )
        testCase "Workflow_WithInlineExpressionToolStep_ROCrateRoundtrip_PreservesStep" (fun () ->
            let workflow =
                CWL.Decode.decodeWorkflow TestObjects.CWL.Workflow.workflowWithUnsupportedInlineRunClassFile
            let protocol =
                WorkflowConversion.composeWorkflowProtocolFromProcessingUnit(
                    "workflows/InlineExpressionWorkflow/workflow.cwl",
                    CWL.Workflow workflow
                )
            let roundTripped =
                WorkflowConversion.decomposeWorkflowProtocolToProcessingUnit(protocol)
            match roundTripped with
            | CWL.Workflow rtWorkflow ->
                Expect.equal (CWL.ListEntry.values rtWorkflow.Steps).Count (CWL.ListEntry.values workflow.Steps).Count "Step count should be preserved through RO-Crate roundtrip"
                let runValue = (CWL.ListEntry.values rtWorkflow.Steps).[0].Run
                match runValue with
                | CWL.RunString runIdentifier ->
                    Expect.equal runIdentifier "inline:ExpressionTool" "Inline ExpressionTool marker should be preserved as run string"
                | other ->
                    Expect.isTrue false $"Expected RunString marker but got %A{other}"
            | other ->
                Expect.isTrue false $"Expected Workflow but got %A{other}"
        )
        testCase "Tool_WithMixedPorts_ConvertsTheTypedPortsOnly" (fun () ->
            let tool =
                CWL.Decode.decodeCommandLineTool "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\ninputs:\n  x: {$mixin: mix.yml}\n  y: string\n  z: {type: int, $mixin: typed.yml}\noutputs:\n  - {id: o, $mixin: out.yml}\n  - {id: p, type: stdout}\n"
            Expect.equal tool.Inputs.Value.Count 3 "The three inputs stay in the tool."
            let protocol = WorkflowConversion.composeWorkflowProtocolFromToolDescription("tools/mixed.cwl", tool)
            let roundTripped = WorkflowConversion.decomposeWorkflowProtocolToToolDescription protocol
            Expect.equal (CWL.ListEntry.values roundTripped.Inputs.Value |> Seq.map (fun input -> input.Name) |> List.ofSeq) [ "y" ] "Only the typed input is converted, because the file of a mixin holds the type of the others."
            Expect.equal (CWL.ListEntry.values roundTripped.Outputs |> Seq.map (fun output -> output.Name) |> List.ofSeq) [ "p" ] "Only the typed output is converted."
        )
        testCase "Workflow_WithRawRun_HasNoIdentifierAndTheCrateGivesBackTheOtherSteps" (fun () ->
            let workflow =
                CWL.Decode.decodeWorkflow "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  a:\n    run: {class: {$import: c.yml}, baseCommand: echo, inputs: [], outputs: []}\n    in: {}\n    out: []\n  b:\n    run: tools/echo.cwl\n    in: {}\n    out: []\n"
            let steps = CWL.ListEntry.values workflow.Steps
            Expect.isNone (WorkflowConversion.runToIdentifier("workflows/w/workflow.cwl", steps.[0].Run)) "A process whose class is a directive has no path and no marker."
            Expect.equal (WorkflowConversion.runToIdentifier("workflows/w/workflow.cwl", steps.[1].Run)) (Some "./workflows/w/tools/echo.cwl") "A run string becomes its path from the root of the crate."
            let protocol = WorkflowConversion.composeWorkflowProtocolFromWorkflow("workflows/w/workflow.cwl", workflow)
            let parts = LDComputationalWorkflow.getHasPart protocol
            Expect.equal parts.Count 2 "Both steps are in the crate."
            Expect.isNone (LDDataset.tryGetIdentifierAsString parts.[0]) "The step with the raw run has no identifier, and no empty one."
            Expect.equal (LDDataset.tryGetIdentifierAsString parts.[1]) (Some "./workflows/w/tools/echo.cwl") "The other step has its path."
            let roundTripped = WorkflowConversion.decomposeWorkflowProtocolToWorkflow(protocol, resolveRunReferences = false)
            Expect.equal (CWL.ListEntry.values roundTripped.Steps |> Seq.map (fun step -> step.Id) |> List.ofSeq) [ Some "b" ] "The crate gives back the step that it can describe."
        )
        testCase "Workflow_WithStepsWithoutId_TheCrateKeepsThemWithoutAnIdAndGivesThemBack" (fun () ->
            let workflow =
                CWL.Decode.decodeWorkflow "cwlVersion: v1.2\nclass: Workflow\nid: main\ninputs: {}\noutputs: {}\nsteps:\n  - {run: tools/a.cwl, in: [], out: []}\n  - {id: named, run: tools/b.cwl, in: [], out: []}\n  - {run: tools/a.cwl, in: [], out: []}\n  - {id: \"\", run: tools/c.cwl, in: [], out: []}\n"
            let protocol = WorkflowConversion.composeWorkflowProtocolFromWorkflow("workflows/w/workflow.cwl", workflow)
            let parts = LDComputationalWorkflow.getHasPart protocol
            Expect.equal parts.Count 4 "All four steps are in the crate."
            Expect.equal (parts |> Seq.map (fun part -> LDComputationalWorkflow.tryGetNameAsString part) |> List.ofSeq) [ None; Some "named"; None; Some "" ] "A step without an id has no name, and the empty id is a name."
            Expect.equal (parts |> Seq.map (fun part -> part.Id) |> Seq.distinct |> Seq.length) 4 "The crate gives each step an identifier of its own."
            Expect.isFalse (parts.[0].Id.Contains "named") "The identifier of a step without an id is no id of the step."
            let roundTripped = WorkflowConversion.decomposeWorkflowProtocolToWorkflow(protocol, resolveRunReferences = false)
            let steps = CWL.ListEntry.values roundTripped.Steps
            Expect.equal (steps |> Seq.map (fun step -> step.Id) |> List.ofSeq) [ None; Some "named"; None; Some "" ] "The ids come back, and a missing id stays missing."
            Expect.equal (steps |> Seq.map (fun step -> step.Run) |> List.ofSeq) [ CWL.RunString "tools/a.cwl"; CWL.RunString "tools/b.cwl"; CWL.RunString "tools/a.cwl"; CWL.RunString "tools/c.cwl" ] "The runs come back."
        )
        testCase "StepIdentifier_IsThePathFromTheCrateRoot" (fun () ->
            let workflow =
                CWL.Decode.decodeWorkflow "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  a: {run: tool.cwl, in: {}, out: []}\n  b: {run: ./sub/../tools/t.cwl, in: {}, out: []}\n  c: {run: https://example.org/t.cwl, in: {}, out: []}\n  d: {run: ../../common.cwl, in: {}, out: []}\n  e: {run: {$import: tool.cwl}, in: {}, out: []}\n  f: {run: {class: CommandLineTool, baseCommand: echo, inputs: [], outputs: []}, in: {}, out: []}\n"
            let identifiers =
                CWL.ListEntry.values workflow.Steps
                |> Seq.map (fun step -> WorkflowConversion.runToIdentifier("workflows/W/workflow.cwl", step.Run))
                |> List.ofSeq
            Expect.equal identifiers [ Some "./workflows/W/tool.cwl"; Some "./workflows/W/tools/t.cwl"; Some "https://example.org/t.cwl"; Some "../../common.cwl"; Some "./workflows/W/tool.cwl"; Some "inline:CommandLineTool" ] "A run in a folder of the ARC becomes its path from the crate root, a remote run and a run outside the folders stay as written, and an inline run has its marker."
            Expect.equal (WorkflowConversion.runToIdentifier("runs/R/run.cwl", CWL.RunString "../../workflows/S/workflow.cwl")) (Some "./workflows/S/workflow.cwl") "The workflow that a run runs is a path from the crate root."
        )
        testCase "StepIdentifier_ReadsBackAsTheRunAsWritten" (fun () ->
            Expect.equal (WorkflowConversion.identifierToRun("./workflows/W/workflow.cwl", "./workflows/W/tool.cwl")) "tool.cwl" "A path in the folder of the workflow is relative to it."
            Expect.equal (WorkflowConversion.identifierToRun("workflows/W/workflow.cwl", "./workflows/W/tools/t.cwl")) "tools/t.cwl" "A path below the folder of the workflow is relative to it."
            Expect.equal (WorkflowConversion.identifierToRun("./runs/R/run.cwl", "./workflows/S/workflow.cwl")) "../../workflows/S/workflow.cwl" "A path in another folder leaves the folder of the run."
            for identifier in [ "tool.cwl"; "./tool.cwl"; "tools/echo.cwl"; "../../workflows/S/workflow.cwl"; "../../common.cwl"; "https://example.org/t.cwl"; "inline:Workflow" ] do
                Expect.equal (WorkflowConversion.identifierToRun("./runs/R/run.cwl", identifier)) identifier $"The identifier {identifier} is a run as written, as older crates hold it, or a marker."
        )
        testCase "StepIdentifier_RoundTripThroughTheCrateKeepsTheRuns" (fun () ->
            let workflow = create_workflow_full()
            workflow.CWLDescription <- Some (CWL.Decode.decodeCWLProcessingUnit "cwlVersion: v1.2\nclass: Workflow\ninputs:\n  x: string\noutputs: {}\nsteps:\n  s: {run: tool.cwl, in: {}, out: []}\n")
            let json = (WorkflowConversion.composeWorkflow workflow).Flatten() |> JsonController.LDGraph.toROCrateJsonString()
            let graph = JsonController.LDGraph.fromROCrateJsonString json
            let step = Expect.wantSome (graph.TryGetNode "WorkflowStep_workflows/MyWorkflow/workflow.cwl_s") "The step is in the crate."
            Expect.equal (LDDataset.tryGetIdentifierAsString(step, ?context = graph.TryGetContext())) (Some "./workflows/MyWorkflow/tool.cwl") "The crate holds the path of the tool from its root."
            let workflow' = WorkflowConversion.decomposeWorkflow(Expect.wantSome (graph.TryGetNode "./workflows/MyWorkflow/") "The workflow is in the crate.", graph = graph)
            match workflow'.CWLDescription with
            | Some (CWL.Workflow workflow') ->
                Expect.equal ((CWL.ListEntry.values workflow'.Steps).[0].Run) (CWL.RunString "tool.cwl") "The workflow runs tool.cwl again."
            | other -> failwithf "Expected a workflow but got %A" other
            let run = create_run_full()
            run.CWLDescription <- Some (CWL.Decode.decodeCWLProcessingUnit "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  main: {run: ../../workflows/W/workflow.cwl, in: {}, out: []}\n")
            let json = (RunConversion.composeRun run).Flatten() |> JsonController.LDGraph.toROCrateJsonString()
            let graph = JsonController.LDGraph.fromROCrateJsonString json
            let run' = RunConversion.decomposeRun(Expect.wantSome (graph.TryGetNode "./runs/MyRun/") "The run is in the crate.", graph = graph)
            match run'.CWLDescription with
            | Some (CWL.Workflow runWorkflow) ->
                Expect.equal ((CWL.ListEntry.values runWorkflow.Steps).[0].Run) (CWL.RunString "../../workflows/W/workflow.cwl") "The run runs the workflow by the same relative path again."
            | other -> failwithf "Expected a workflow but got %A" other
        )
        testCase "StepIdentifier_OlderCratesHoldTheRunAsWritten" (fun () ->
            let workflow = CWL.Decode.decodeWorkflow "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  a: {run: tool.cwl, in: {}, out: []}\n  b: {run: tool.cwl, in: {}, out: []}\n"
            let protocol = WorkflowConversion.composeWorkflowProtocolFromWorkflow("workflows/W/workflow.cwl", workflow)
            let parts = LDComputationalWorkflow.getHasPart protocol
            LDDataset.setIdentifierAsString(parts.[0], "./tool.cwl")
            LDDataset.setIdentifierAsString(parts.[1], "tools/echo.cwl")
            let roundTripped = WorkflowConversion.decomposeWorkflowProtocolToWorkflow(protocol, resolveRunReferences = false)
            Expect.equal (CWL.ListEntry.values roundTripped.Steps |> Seq.map (fun step -> step.Run) |> List.ofSeq) [ CWL.RunString "./tool.cwl"; CWL.RunString "tools/echo.cwl" ] "The runs of an older crate come back as they were written."
        )
        testCase "Workflow_WithMixedRun_UsesThePathOfTheMixinAsIdentifier" (fun () ->
            let workflow =
                CWL.Decode.decodeWorkflow "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  s:\n    run: {$mixin: tools/echo.cwl}\n    in: {}\n    out: []\n"
            let step = (CWL.ListEntry.values workflow.Steps).[0]
            Expect.equal (WorkflowConversion.runToIdentifier("workflows/w/workflow.cwl", step.Run)) (Some "./workflows/w/tools/echo.cwl") "The identifier is the path of the mixin, as it is the path of an import."
        )
    ]

let tests_ArcRun =
    testList "Run" [
        testCase "Full_FromScaffold" (fun () ->
            let run = create_run_full()
            let ro_Run = RunConversion.composeRun run
            let run' = RunConversion.decomposeRun ro_Run
            Expect.equal run' run "Run should match"
        )
        testCase "Full_FromScaffold_Flattened" (fun () ->
            let run = create_run_full()
            let ro_Run = RunConversion.composeRun run
            let graph = ro_Run.Flatten()
            // Test that flattened worked
            Expect.isTrue (graph.Nodes.Count > 0) "Graph should have properties"
            let r = Expect.wantSome (ro_Run.TryGetPropertyAsSingleton(LDDataset.mentions)) "ArcRun should still have associated workflow"
            Expect.isTrue (r :? LDRef) "Run should be flattened correctly"
            //
            let run' = RunConversion.decomposeRun(ro_Run, graph = graph)
            Expect.equal run' run "Run should match"
        )
        // File ids are normalized to start with ./, see https://github.com/nfdi4plants/ARCtrl/issues/615
        testCase "Full_FromScaffold_NormalizedIds" (fun () ->
            let run = create_run_full()
            let ro_Run = RunConversion.composeRun run
            let json = ro_Run.Flatten() |> JsonController.LDGraph.toROCrateJsonString()
            let graph = JsonController.LDGraph.fromROCrateJsonString json
            let ro_Run' = Expect.wantSome (graph.TryGetNode "./runs/MyRun/") "Run should have normalized id"
            let run' = RunConversion.decomposeRun(ro_Run', graph = graph)
            Expect.sequenceEqual run'.CWLInput run.CWLInput "CWL inputs should match"
            Expect.equal run' run "Run should match"
        )
        testCase "CWLAndISAInputMatch" (fun () ->
            let inputFileName = "input.txt"
            let tool = 
                CWL.CWLToolDescription(
                    outputs = ListEntry.ofValues [TestObjects.CWL.Outputs.CSV.outputCSV],
                    inputs = ListEntry.ofValues [CWL.CWLInput(name = TestObjects.CWL.Inputs.File.inputFileName, type_ = CWLType.File)]
                )
                |> CWL.CommandLineTool
            let table = ArcTable.init("MyTable")
            table.AddColumn(
                CompositeHeader.Input IOType.Data,
                ResizeArray [|CompositeCell.createDataFromString ("runs/MyRun/" + inputFileName) |]
            )
            table.AddColumn(
                CompositeHeader.Output IOType.Data,
                ResizeArray [|CompositeCell.createDataFromString "MyOutputFile.csv" |]
            )
            let ymlParam = CWLParameterReference(
                key = TestObjects.CWL.Inputs.File.inputFileName,
                value = CWLParameterValue.File (FileInstance(path = CWL.SchemaSaladString.Literal inputFileName))
            )
            let run = ArcRun(
                "MyRun",
                cwlDescription = tool,
                cwlInput = ResizeArray [ymlParam],
                tables = ResizeArray [table]
                )
            let ro_Run = RunConversion.composeRun run
            let graph = ro_Run.Flatten()
            let run' = RunConversion.decomposeRun(ro_Run,graph)
            Expect.equal run' run "Run should match"
        )
        testCase "WorkflowProtocolAsMainEntity" (fun () ->
            let run = create_run_full()
            let ro_Run = RunConversion.composeRun run
            
            // Test that workflow protocol is in mainEntity
            let mainEntities = LDDataset.getMainEntities(ro_Run)
            Expect.equal mainEntities.Count 1 "Should have one main entity"
            let workflowProtocol = mainEntities.[0]
            Expect.isTrue (LDWorkflowProtocol.validate(workflowProtocol)) "Main entity should be a workflow protocol"
            
            // Test that workflow protocol is also in hasParts
            let hasParts = LDDataset.getHasParts(ro_Run)
            let workflowInHasParts = hasParts |> Seq.exists (fun hp -> hp.Id = workflowProtocol.Id)
            Expect.isTrue workflowInHasParts "Workflow protocol should be in hasParts"
            
            // Test that workflow protocol is a File
            Expect.isTrue (LDFile.validate(workflowProtocol)) "Workflow protocol should be a File"
            
            // Test workflow protocol has required properties
            let inputs = LDComputationalWorkflow.getInputs workflowProtocol
            Expect.isTrue (inputs.Count > 0) "Workflow protocol should have inputs"
            
            let outputs = LDComputationalWorkflow.getOutputs workflowProtocol
            Expect.isTrue (outputs.Count > 0) "Workflow protocol should have outputs"
            
            let programmingLanguages = LDComputationalWorkflow.getProgrammingLanguages workflowProtocol
            Expect.hasLength programmingLanguages 1 "Workflow protocol should have one programming language"
            Expect.isTrue (LDComputerLanguage.validateCWL(programmingLanguages.[0])) "Programming language should be CWL"
            
            // Test workflow protocol has creator, publisher, and dateCreated
            let creators = LDComputationalWorkflow.getCreators workflowProtocol
            Expect.equal creators.Count 1 "Workflow protocol should have one creator"
            Expect.isTrue (LDPerson.validate(creators.[0])) "Creator should be a Person"
            
            let publisher = Expect.wantSome (LDComputationalWorkflow.tryGetSdPublisher workflowProtocol) "Workflow protocol should have a publisher"
            Expect.isTrue (LDOrganization.validate(publisher)) "Publisher should be an Organization"
            let publisherName = Expect.wantSome (LDOrganization.tryGetNameAsString publisher) "Publisher should have a name"
            Expect.equal publisherName "DataPLANT" "Publisher name should be DataPLANT"
            
            let dateCreated = Expect.wantSome (LDComputationalWorkflow.tryGetDateCreatedAsDateTime workflowProtocol) "Workflow protocol should have dateCreated"
            Expect.isSome (Some dateCreated) "DateCreated should exist"
            
            // Test decomposition works with mainEntity
            let run' = RunConversion.decomposeRun ro_Run
            Expect.equal run' run "Run should match after decomposition"
        )
        testCase "WorkflowProtocolAsMainEntity_Flattened" (fun () ->
            let run = create_run_full()
            let ro_Run = RunConversion.composeRun run
            let graph = ro_Run.Flatten()
            
            // Test that mainEntity is still accessible after flattening
            let mainEntityRef = Expect.wantSome (ro_Run.TryGetPropertyAsSingleton(LDDataset.mainEntity)) "Run should have mainEntity"
            Expect.isTrue (mainEntityRef :? LDRef) "MainEntity should be flattened to LDRef"
            
            // Retrieve workflow protocol from graph
            let mainEntities = LDDataset.getMainEntities(ro_Run, graph = graph)
            Expect.equal mainEntities.Count 1 "Should have one main entity"
            let workflowProtocol = mainEntities.[0]
            Expect.isTrue (LDWorkflowProtocol.validate(workflowProtocol)) "Main entity should be a workflow protocol"
            
            // Test workflow protocol has required properties after flattening
            let inputs = LDComputationalWorkflow.getInputs(workflowProtocol, graph = graph)
            Expect.isTrue (inputs.Count > 0) "Workflow protocol should have inputs"
            
            let outputs = LDComputationalWorkflow.getOutputs(workflowProtocol, graph = graph)
            Expect.isTrue (outputs.Count > 0) "Workflow protocol should have outputs"
            
            let programmingLanguages = LDComputationalWorkflow.getProgrammingLanguages(workflowProtocol, graph = graph)
            Expect.hasLength programmingLanguages 1 "Workflow protocol should have one programming language"
            Expect.isTrue (LDComputerLanguage.validateCWL(programmingLanguages.[0])) "Programming language should be CWL"
            
            // Test workflow protocol has creator, publisher, and dateCreated after flattening
            let creators = LDComputationalWorkflow.getCreators(workflowProtocol, graph = graph)
            Expect.equal creators.Count 1 "Workflow protocol should have one creator"
            Expect.isTrue (LDPerson.validate(creators.[0])) "Creator should be a Person"
            
            let publisher = Expect.wantSome (LDComputationalWorkflow.tryGetSdPublisher(workflowProtocol, graph = graph)) "Workflow protocol should have a publisher"
            Expect.isTrue (LDOrganization.validate(publisher)) "Publisher should be an Organization"
            let publisherName = Expect.wantSome (LDOrganization.tryGetNameAsString publisher) "Publisher should have a name"
            Expect.equal publisherName "DataPLANT" "Publisher name should be DataPLANT"
            
            let dateCreated = Expect.wantSome (LDComputationalWorkflow.tryGetDateCreatedAsDateTime workflowProtocol) "Workflow protocol should have dateCreated"
            Expect.isSome (Some dateCreated) "DateCreated should exist"
            
            // Test decomposition works with mainEntity and graph
            let run' = RunConversion.decomposeRun(ro_Run, graph = graph)
            Expect.equal run' run "Run should match after decomposition with graph"
        )
        testCase "NestedFileArrayInput_Roundtrip" (fun () ->
            let inputName = "sampleRecordFiles"
            let inputType =
                CWLType.Array (InputArraySchema(CWLType.Array (InputArraySchema(CWLType.File))))
            let workflow =
                CWL.CWLWorkflowDescription(
                    steps = ResizeArray(),
                    inputs = ListEntry.ofValues [CWL.CWLInput(name = inputName, type_ = inputType)],
                    outputs = ResizeArray()
                )
            let runInputValues =
                """sampleRecordFiles:
  - - class: File
      path: ../../assays/RNASeq/dataset/DB_097.fastq.gz
      format: edam:format_1930
  - - class: File
      path: ../../assays/RNASeq/dataset/DB_163.fastq.gz
      format: edam:format_1930"""
                |> CWL.DecodeParameters.decodeYAMLParameterFile
            let run =
                ArcRun.create(
                    identifier = "kallisto",
                    cwlDescription = CWL.Workflow workflow,
                    cwlInput = runInputValues
                )
            let ro_Run = RunConversion.composeRun run
            let graph = ro_Run.Flatten()
            let run' = RunConversion.decomposeRun(ro_Run, graph = graph)
            Expect.equal run'.Identifier run.Identifier "Run identifier should roundtrip."
            let expectedInput = Expect.wantExactlyOne run.CWLInput "Expected one source input."
            let actualInput = Expect.wantExactlyOne run'.CWLInput "Expected one roundtripped input."
            Expect.equal actualInput.Key expectedInput.Key "Input key should roundtrip."
            Expect.equal actualInput.Value expectedInput.Value "Run with nested File[][] input should roundtrip through RO-Crate."
            Expect.sequenceEqual actualInput.Values expectedInput.Values "Legacy flattened Values projection should roundtrip."
        )
        testCase "JobFilePaths_DotSegmentsAreResolved" (fun () ->
            Expect.equal (RunConversion.composeCWLInputFilePath("./x", "R")) "runs/R/x" "A leading . segment is dropped."
            Expect.equal (RunConversion.composeCWLInputFilePath("a/./b", "R")) "runs/R/a/b" "A . segment inside the path is dropped."
            Expect.equal (RunConversion.composeCWLInputFilePath("../../assays/A/dataset/d.csv", "R")) "assays/A/dataset/d.csv" "The .. segments leave the run folder."
            Expect.equal (RunConversion.composeCWLInputFilePath("../S/./out.csv", "R")) "runs/S/out.csv" "A path into another run resolves both kinds of segment."
        )
        testCase "JobFilePaths_CrateIdsHaveNoDotSegmentsAndReadBack" (fun () ->
            let workflow =
                CWL.CWLWorkflowDescription(
                    steps = ResizeArray(),
                    inputs = ListEntry.ofValues [CWL.CWLInput(name = "params", type_ = CWLType.File); CWL.CWLInput(name = "table", type_ = CWLType.File)],
                    outputs = ResizeArray()
                )
            let runInputValues =
                "params:\n  class: File\n  path: ./quantalignresult/AlignmentBasedQuantificationParams.json\ntable:\n  class: File\n  path: ../../assays/A/./dataset/table.csv\n"
                |> CWL.DecodeParameters.decodeYAMLParameterFile
            let run =
                ArcRun.create(
                    identifier = "DilutionSeries",
                    cwlDescription = CWL.Workflow workflow,
                    cwlInput = runInputValues
                )
            let json = (RunConversion.composeRun run).Flatten() |> JsonController.LDGraph.toROCrateJsonString()
            let graph = JsonController.LDGraph.fromROCrateJsonString json
            Expect.isSome (graph.TryGetNode "./runs/DilutionSeries/quantalignresult/AlignmentBasedQuantificationParams.json") "The id of a file in the run folder has no . segment."
            Expect.isSome (graph.TryGetNode "./assays/A/dataset/table.csv") "The id of a file outside the run folder has no . or .. segment."
            let ro_Run = Expect.wantSome (graph.TryGetNode "./runs/DilutionSeries/") "The run is in the crate."
            let run' = RunConversion.decomposeRun(ro_Run, graph = graph)
            let pathOf (key : string) =
                run'.CWLInput
                |> Seq.find (fun input -> input.Key = key)
                |> fun input ->
                    match input.Value with
                    | Some (CWL.CWLParameterValue.File file) -> CWL.FileInstance.pathOrLocation file
                    | other -> failwithf "Expected a File value for %s but got %A" key other
            Expect.equal (pathOf "params") "quantalignresult/AlignmentBasedQuantificationParams.json" "The value reads back relative to the run folder."
            Expect.equal (pathOf "table") "../../assays/A/dataset/table.csv" "The value outside the run folder reads back relative to the run folder."
        )
        testCase "JobFilePaths_URLsStayAsTheyAre" (fun () ->
            Expect.equal (RunConversion.composeCWLInputFilePath("https://example.org/data/p.json", "R")) "https://example.org/data/p.json" "A URL is not joined to the run folder."
            Expect.equal (RunConversion.composeCWLInputFilePath("file:///data/p.json", "R")) "file:///data/p.json" "A file URI is not joined to the run folder."
            Expect.equal (RunConversion.decomposeCWLInputFilePath("https://example.org/data/p.json", "R")) "https://example.org/data/p.json" "A URL reads back as it is."
            let workflow =
                CWL.CWLWorkflowDescription(
                    steps = ResizeArray(),
                    inputs = ListEntry.ofValues [CWL.CWLInput(name = "params", type_ = CWLType.File)],
                    outputs = ResizeArray()
                )
            let run =
                ArcRun.create(
                    identifier = "R",
                    cwlDescription = CWL.Workflow workflow,
                    cwlInput = CWL.DecodeParameters.decodeYAMLParameterFile "params:\n  class: File\n  location: https://example.org/data/p.json\n"
                )
            let graph = (RunConversion.composeRun run).Flatten() |> JsonController.LDGraph.toROCrateJsonString() |> JsonController.LDGraph.fromROCrateJsonString
            Expect.isSome (graph.TryGetNode "https://example.org/data/p.json") "The id of the file is the URL."
            let run' = RunConversion.decomposeRun(Expect.wantSome (graph.TryGetNode "./runs/R/") "The run is in the crate.", graph = graph)
            match (Seq.exactlyOne run'.CWLInput).Value with
            | Some (CWL.CWLParameterValue.File file) -> Expect.equal (CWL.FileInstance.pathOrLocation file) "https://example.org/data/p.json" "The value reads back as the URL."
            | other -> failwithf "Expected a File value but got %A" other
        )
        testCase "JobFilePaths_URLsAreNoFilesOfTheArc" (fun () ->
            let tool = CWL.Decode.decodeCWLProcessingUnit "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: cat\ninputs:\n  a: File\n  b: File\noutputs: []\n"
            let input = CWL.DecodeParameters.decodeYAMLParameterFile "a:\n  class: File\n  location: file:///data/p.json\nb:\n  class: File\n  location: s3://bucket/x.csv\n"
            let run = ArcRun.create(identifier = "R", title = "t", description = "d", cwlDescription = tool, cwlInput = input)
            let arc = ARC("I", title = "T", description = "D", runs = ResizeArray [run])
            let arc' = ARC.fromROCrateJsonString (arc.ToROCrateJsonString())
            let paths = arc'.FileSystem.Tree.ToFilePaths()
            Expect.isFalse (paths |> Array.exists (fun path -> path.Contains "p.json" || path.Contains "x.csv")) $"No URL becomes a file of the ARC, but the paths are %A{paths}."
            let pathOf (key : string) =
                match (arc'.Runs.[0].CWLInput |> Seq.find (fun input -> input.Key = key)).Value with
                | Some (CWL.CWLParameterValue.File file) -> CWL.FileInstance.pathOrLocation file
                | other -> failwithf "Expected a File value for %s but got %A" key other
            Expect.equal (pathOf "a") "file:///data/p.json" "The file URI reads back as it is."
            Expect.equal (pathOf "b") "s3://bucket/x.csv" "The S3 URL reads back as it is."
        )
        testCase "JobFilePaths_TheRunFolderIsComparedBySegments" (fun () ->
            Expect.equal (RunConversion.decomposeCWLInputFilePath("./runs/R/out.csv", "R")) "out.csv" "A file in the run folder is relative to it."
            Expect.equal (RunConversion.decomposeCWLInputFilePath("./runs/R2/out.csv", "R")) "../../runs/R2/out.csv" "A file of the run R2 is no file of the run R."
            Expect.equal (RunConversion.decomposeCWLInputFilePath("runs/R/sub/out.csv", "R")) "sub/out.csv" "An id without the leading ./ is read the same way."
            Expect.equal (RunConversion.decomposeCWLInputFilePath("./assays/A/dataset/d.csv", "R")) "../../assays/A/dataset/d.csv" "A file outside the run folder leaves it."
        )
        testCase "JobFilePaths_TheRunFolderItselfReadsBackAsDot" (fun () ->
            Expect.equal (RunConversion.composeCWLInputFilePath(".", "R")) "runs/R" "The run folder is the path of the run from the crate root."
            for id in [ "runs/R"; "./runs/R"; "./runs/R/" ] do
                Expect.equal (RunConversion.decomposeCWLInputFilePath(id, "R")) "." $"The id {id} is the run folder, which is . from the run."
            Expect.equal (RunConversion.decomposeCWLInputFilePath("./runs", "R")) "../../runs" "The folder of all runs is no run folder."
            let workflow =
                CWL.CWLWorkflowDescription(
                    steps = ResizeArray(),
                    inputs = ListEntry.ofValues [CWL.CWLInput(name = "here", type_ = CWLType.Directory)],
                    outputs = ResizeArray()
                )
            let run =
                ArcRun.create(
                    identifier = "R",
                    cwlDescription = CWL.Workflow workflow,
                    cwlInput = CWL.DecodeParameters.decodeYAMLParameterFile "here:\n  class: Directory\n  location: .\n"
                )
            let run' = RunConversion.decomposeRun(RunConversion.composeRun run)
            match (Seq.exactlyOne run'.CWLInput).Value with
            | Some (CWL.CWLParameterValue.Directory directory) -> Expect.equal (CWL.DirectoryInstance.pathOrLocation directory) "." "The directory reads back as the run folder."
            | other -> failwithf "Expected a Directory value but got %A" other
        )
        testCaseCrossAsync "RelativeWorkflowRunReference_ResolvesToWorkflow" (crossAsync {
            let sampleArcDirectory = TestObjects.IO.testSimpleARCWithCWL
            let runFixturePath =
                ArcPathHelper.combineMany [|
                    sampleArcDirectory
                    "runs"
                    "tests"
                    "run.cwl"
                |]
            let runParameterFixturePath =
                ArcPathHelper.combineMany [|
                    sampleArcDirectory
                    "runs"
                    "tests"
                    "run.yml"
                |]
            let referencedWorkflowFixturePath =
                ArcPathHelper.combineMany [|
                    sampleArcDirectory
                    "workflows"
                    "ProteomIQon"
                    "workflow.cwl"
                |]

            let normalizePath (path: string) =
                path
                |> FileSystemHelper.standardizeSlashes
                |> ArcPathHelper.normalize

            let fixtureRootPath = normalizePath sampleArcDirectory
            let runFixturePathNormalized = normalizePath runFixturePath
            let referencedWorkflowFixturePathNormalized = normalizePath referencedWorkflowFixturePath

            let fixturePathToGraphPath (fixturePath: string) =
                fixturePath.Substring(fixtureRootPath.Length).TrimStart('/')

            let graphPathToFixturePath (graphPath: string) =
                ArcPathHelper.combine sampleArcDirectory graphPath
                |> normalizePath

            let! runFixtureExists = FileSystemHelper.fileExistsAsync runFixturePath
            Expect.isTrue runFixtureExists $"Expected run fixture at {runFixturePath}"
            let! runParameterFixtureExists = FileSystemHelper.fileExistsAsync runParameterFixturePath
            Expect.isTrue runParameterFixtureExists $"Expected run parameter fixture at {runParameterFixturePath}"
            let! referencedWorkflowFixtureExists = FileSystemHelper.fileExistsAsync referencedWorkflowFixturePath
            Expect.isTrue referencedWorkflowFixtureExists $"Expected referenced workflow fixture at {referencedWorkflowFixturePath}"

            let! runText = FileSystemHelper.readFileTextAsync runFixturePath
            let runProcessingUnit = CWL.Decode.decodeCWLProcessingUnit (runText.TrimEnd())
            let sourceRunWorkflow =
                let sourceRunWorkflowOpt =
                    match runProcessingUnit with
                    | CWL.Workflow workflow -> Some workflow
                    | _ -> None
                Expect.wantSome sourceRunWorkflowOpt $"Expected run fixture to decode as Workflow but got %A{runProcessingUnit}"
            let! runParameterText = FileSystemHelper.readFileTextAsync runParameterFixturePath
            let runInputValues = CWL.DecodeParameters.decodeYAMLParameterFile (runParameterText.TrimEnd())
            Expect.equal runInputValues.Count (CWL.ListEntry.values sourceRunWorkflow.Inputs).Count "run.yml should define all required run inputs"
            let expectedInputKeys =
                (CWL.ListEntry.values sourceRunWorkflow.Inputs)
                |> Seq.map (fun input -> input.Name)
                |> Seq.sort
                |> Seq.toList
            let providedInputKeys =
                runInputValues
                |> Seq.map (fun input -> input.Key)
                |> Seq.sort
                |> Seq.toList
            Expect.equal providedInputKeys expectedInputKeys "run.yml input keys should match workflow input keys"

            let run =
                ArcRun.create(
                    identifier = "tests",
                    cwlDescription = runProcessingUnit,
                    cwlInput = runInputValues
                )
            let ro_Run = RunConversion.composeRun run
            let graph = ro_Run.Flatten()

            let! fixtureRelativePaths = FileSystemHelper.getAllFilePathsAsync sampleArcDirectory
            let fixtureCwlPaths =
                fixtureRelativePaths
                |> Array.filter (fun relativePath -> relativePath.EndsWith(".cwl"))
                |> Array.map (fun relativePath -> ArcPathHelper.combine sampleArcDirectory relativePath)
                |> Array.map normalizePath

            for fixtureCwlPath in fixtureCwlPaths do
                if fixtureCwlPath <> runFixturePathNormalized then
                    let! fixtureText = FileSystemHelper.readFileTextAsync fixtureCwlPath
                    let processingUnit = CWL.Decode.decodeCWLProcessingUnit (fixtureText.TrimEnd())
                    let graphPath = fixturePathToGraphPath fixtureCwlPath
                    let node = WorkflowConversion.composeWorkflowProtocolFromProcessingUnit(graphPath, processingUnit)
                    graph.AddNode node

            let! referencedWorkflowText = FileSystemHelper.readFileTextAsync referencedWorkflowFixturePath
            let referencedWorkflowProcessingUnit = CWL.Decode.decodeCWLProcessingUnit (referencedWorkflowText.TrimEnd())
            let sourceReferencedWorkflow =
                let sourceReferencedWorkflowOpt =
                    match referencedWorkflowProcessingUnit with
                    | CWL.Workflow workflow -> Some workflow
                    | _ -> None
                Expect.wantSome sourceReferencedWorkflowOpt $"Expected referenced workflow fixture to decode as Workflow but got %A{referencedWorkflowProcessingUnit}"

            let workflowInputSignature (input: CWL.CWLInput) =
                input.Name,
                (input.Type_ |> Option.map WorkflowConversion.composeAdditionalType),
                (input.InputBinding |> Option.bind CWL.ListEntry.tryValue |> Option.bind (fun binding -> binding.Prefix)),
                (input.InputBinding |> Option.bind CWL.ListEntry.tryValue |> Option.bind (fun binding -> binding.Position))

            let workflowOutputSignature (output: CWL.CWLOutput) =
                output.Name,
                (output.Type_ |> Option.map WorkflowConversion.composeAdditionalType),
                (output.OutputBinding |> Option.bind CWL.ListEntry.tryValue |> Option.bind (fun binding -> binding.Glob))

            let workflowSignatures (workflow: CWL.CWLWorkflowDescription) =
                let inputSignatures =
                    (CWL.ListEntry.values workflow.Inputs)
                    |> Seq.map workflowInputSignature
                    |> Seq.sortBy (fun (name, _, _, _) -> name)
                    |> Seq.toList
                let outputSignatures =
                    (CWL.ListEntry.values workflow.Outputs)
                    |> Seq.map workflowOutputSignature
                    |> Seq.sortBy (fun (name, _, _) -> name)
                    |> Seq.toList
                inputSignatures, outputSignatures

            let toolSignatures (tool: CWL.CWLToolDescription) =
                let inputSignatures =
                    CWL.CWLToolDescription.getInputValuesOrEmpty tool
                    |> Seq.map workflowInputSignature
                    |> Seq.sortBy (fun (name, _, _, _) -> name)
                    |> Seq.toList
                let outputSignatures =
                    (CWL.ListEntry.values tool.Outputs)
                    |> Seq.map workflowOutputSignature
                    |> Seq.sortBy (fun (name, _, _) -> name)
                    |> Seq.toList
                inputSignatures, outputSignatures

            let run' = RunConversion.decomposeRun(ro_Run, graph = graph)
            let parsedRunWorkflow =
                let parsedRunWorkflowOpt =
                    match run'.CWLDescription with
                    | Some (CWL.Workflow workflow) -> Some workflow
                    | _ -> None
                Expect.wantSome parsedRunWorkflowOpt $"Expected decomposed run to contain workflow CWL but got %A{run'.CWLDescription}"

            let sourceRunStepIds = (CWL.ListEntry.values sourceRunWorkflow.Steps) |> Seq.map (fun step -> step.Id) |> Seq.toList
            let parsedRunStepIds = (CWL.ListEntry.values parsedRunWorkflow.Steps) |> Seq.map (fun step -> step.Id) |> Seq.toList
            Expect.equal parsedRunStepIds sourceRunStepIds "Run workflow step identifiers should be preserved"
            let sourceRunInputs, sourceRunOutputs = workflowSignatures sourceRunWorkflow
            let parsedRunInputs, parsedRunOutputs = workflowSignatures parsedRunWorkflow
            Expect.equal parsedRunInputs sourceRunInputs "Run workflow input signatures should be preserved"
            Expect.equal parsedRunOutputs sourceRunOutputs "Run workflow output signatures should be preserved"

            let parsedRunStep = (CWL.ListEntry.values parsedRunWorkflow.Steps).[0]
            Expect.isTrue
                (match CWL.WorkflowStep.getResolvedRun parsedRunStep with | CWL.RunWorkflow _ -> true | _ -> false)
                (sprintf "Expected run step to resolve to RunWorkflow but got %A" (CWL.WorkflowStep.getResolvedRun parsedRunStep))

            let referencedWorkflowObj: obj =
                match CWL.WorkflowStep.getResolvedRun parsedRunStep with
                | CWL.RunWorkflow workflowObj -> workflowObj
                | _ -> Unchecked.defaultof<obj>
            Expect.isTrue
                (referencedWorkflowObj :? CWL.CWLWorkflowDescription)
                (sprintf "Expected RunWorkflow payload to be CWLWorkflowDescription but got %A" referencedWorkflowObj)
            let parsedReferencedWorkflow = referencedWorkflowObj :?> CWL.CWLWorkflowDescription

            let sourceReferencedStepIds = (CWL.ListEntry.values sourceReferencedWorkflow.Steps) |> Seq.map (fun step -> step.Id) |> Seq.toList
            let parsedReferencedStepIds = (CWL.ListEntry.values parsedReferencedWorkflow.Steps) |> Seq.map (fun step -> step.Id) |> Seq.toList
            Expect.equal parsedReferencedStepIds sourceReferencedStepIds "Referenced workflow step identifiers should be preserved"
            let sourceReferencedInputs, sourceReferencedOutputs = workflowSignatures sourceReferencedWorkflow
            let parsedReferencedInputs, parsedReferencedOutputs = workflowSignatures parsedReferencedWorkflow
            Expect.equal parsedReferencedInputs sourceReferencedInputs "Referenced workflow input signatures should be preserved"
            Expect.equal parsedReferencedOutputs sourceReferencedOutputs "Referenced workflow output signatures should be preserved"

            let unresolvedNestedRuns =
                (CWL.ListEntry.values parsedReferencedWorkflow.Steps)
                |> Seq.choose (fun step ->
                    match CWL.WorkflowStep.getResolvedRun step with
                    | CWL.RunString unresolvedPath -> Some (step.Id, unresolvedPath)
                    | _ -> None
                )
                |> Seq.toList
            Expect.isEmpty unresolvedNestedRuns "All referenced workflow steps should resolve to concrete CWL objects"

            let referencedWorkflowGraphPath = fixturePathToGraphPath referencedWorkflowFixturePathNormalized
            let expectedRunPathByStepId =
                (CWL.ListEntry.values sourceReferencedWorkflow.Steps)
                |> Seq.map (fun step ->
                    let runPath =
                        let runPathOpt =
                            match step.Run with
                            | CWL.RunString runPath -> Some runPath
                            | _ -> None
                        Expect.wantSome runPathOpt $"Expected source referenced workflow run of step {step.Id} to be a string path but got %A{step.Run}"
                    step.Id, runPath
                )
                |> Map.ofSeq

            let mutable expectedToolByStepId : Map<string option, CWL.CWLToolDescription> = Map.empty
            for KeyValue(stepId, runPath) in expectedRunPathByStepId do
                let expectedToolGraphPath =
                    ArcPathHelper.resolvePathFromFile referencedWorkflowGraphPath runPath
                    |> ArcPathHelper.normalize
                let expectedToolFixturePath = graphPathToFixturePath expectedToolGraphPath
                let! expectedToolExists = FileSystemHelper.fileExistsAsync expectedToolFixturePath
                Expect.isTrue expectedToolExists $"Expected referenced tool fixture for step {stepId} at {expectedToolFixturePath}"
                let! expectedToolText = FileSystemHelper.readFileTextAsync expectedToolFixturePath
                let expectedToolProcessingUnit = CWL.Decode.decodeCWLProcessingUnit (expectedToolText.TrimEnd())
                let expectedTool =
                    let expectedToolOpt =
                        match expectedToolProcessingUnit with
                        | CWL.CommandLineTool tool -> Some tool
                        | _ -> None
                    Expect.wantSome expectedToolOpt $"Expected referenced tool fixture for step {stepId} to decode as CommandLineTool but got %A{expectedToolProcessingUnit}"
                expectedToolByStepId <- expectedToolByStepId.Add(stepId, expectedTool)

            for parsedStep in (CWL.ListEntry.values parsedReferencedWorkflow.Steps) do
                let expectedTool =
                    let expectedToolOpt = expectedToolByStepId.TryFind parsedStep.Id
                    Expect.wantSome expectedToolOpt $"Expected source tool for step {parsedStep.Id}"

                Expect.isTrue
                    (match CWL.WorkflowStep.getResolvedRun parsedStep with | CWL.RunCommandLineTool _ -> true | _ -> false)
                    (sprintf "Expected step %A to resolve to RunCommandLineTool but got %A" parsedStep.Id (CWL.WorkflowStep.getResolvedRun parsedStep))

                let parsedToolObj: obj =
                    match CWL.WorkflowStep.getResolvedRun parsedStep with
                    | CWL.RunCommandLineTool toolObj -> toolObj
                    | _ -> Unchecked.defaultof<obj>
                Expect.isTrue
                    (parsedToolObj :? CWL.CWLToolDescription)
                    (sprintf "Expected resolved tool payload of step %A to be CWLToolDescription but got %A" parsedStep.Id parsedToolObj)
                let parsedTool = parsedToolObj :?> CWL.CWLToolDescription

                let expectedInputs, expectedOutputs = toolSignatures expectedTool
                let parsedInputs, parsedOutputs = toolSignatures parsedTool
                Expect.equal parsedTool.CWLVersion expectedTool.CWLVersion $"CWLVersion should match for resolved tool of step {parsedStep.Id}"
                Expect.equal parsedInputs expectedInputs $"Input signatures should match for resolved tool of step {parsedStep.Id}"
                Expect.equal parsedOutputs expectedOutputs $"Output signatures should match for resolved tool of step {parsedStep.Id}"
        })


    ]

module WorkflowCrate =

    let workflowText =
        "cwlVersion: v1.2\nclass: Workflow\nrequirements:\n  DockerRequirement:\n    dockerFile: {$include: Dockerfile}\ninputs:\n  x: string\noutputs:\n  y:\n    type: string\n    outputSource: s/o\nsteps:\n  s:\n    run: tool.cwl\n    in: {}\n    out: [o]\n"

    let toolText =
        "cwlVersion: v1.2\nclass: CommandLineTool\nrequirements:\n  SchemaDefRequirement:\n    types:\n      - $import: types.yml\nbaseCommand: echo\ninputs: []\noutputs:\n  o: string\n"

    let runText =
        "cwlVersion: v1.2\nclass: Workflow\ninputs: {}\noutputs: {}\nsteps:\n  main:\n    run: ../../workflows/W/workflow.cwl\n    in: {}\n    out: []\n"

    /// Decodes each file once and resolves the runs of the CWL file at the path, as the reader of an ARC does.
    let resolve (cwlFilePath : string) (files : (string * string) list) =
        let byPath =
            files
            |> List.map (fun (path, text) -> ArcPathHelper.normalizePathKey path, CWL.Decode.decodeCWLProcessingUnit text)
            |> Map.ofList
        CWLRunResolver.resolveRunReferencesFromLookup cwlFilePath byPath.[ArcPathHelper.normalizePathKey cwlFilePath] (fun path -> byPath.TryFind (ArcPathHelper.normalizePathKey path))

    let fileSystemOf (paths : string list) = FileSystem.fromFilePaths (Array.ofList paths)

    let license = License(contentType = LicenseContentType.Fulltext, content = "MIT", path = "LICENSE")

    let hasPartIds (json : string) =
        let graph = JsonController.LDGraph.fromROCrateJsonString json
        let root = Expect.wantSome (graph.TryGetNode "./") "The crate has a root."
        let parts = LDDataset.getHasParts(root, graph = graph, ?context = graph.TryGetContext())
        parts |> Seq.iter (fun part -> Expect.isTrue (LDFile.validate(part, ?context = graph.TryGetContext())) $"The part {part.Id} is a File.")
        parts |> Seq.map (fun part -> part.Id) |> List.ofSeq

let tests_WorkflowCrate =
    testList "WorkflowCrate" [
        testCase "ReferencedFiles_StepRunsAndDirectivesOfEveryFileReached" (fun () ->
            let workflow =
                WorkflowCrate.resolve "workflows/W/workflow.cwl" [
                    "workflows/W/workflow.cwl", WorkflowCrate.workflowText
                    "workflows/W/tool.cwl", WorkflowCrate.toolText
                ]
            let fs = WorkflowCrate.fileSystemOf ["workflows/W/workflow.cwl"; "workflows/W/tool.cwl"; "workflows/W/Dockerfile"; "workflows/W/types.yml"]
            let paths = WorkflowConversion.getReferencedFilePaths("workflows/W/workflow.cwl", workflow, fs)
            Expect.sequenceEqual paths ["workflows/W/Dockerfile"; "workflows/W/tool.cwl"; "workflows/W/types.yml"] "The include and the run of the workflow come first, then the import of the tool."
        )
        testCase "ReferencedFiles_TheWorkflowOfARunAndItsTools" (fun () ->
            let run =
                WorkflowCrate.resolve "runs/R/run.cwl" [
                    "runs/R/run.cwl", WorkflowCrate.runText
                    "workflows/W/workflow.cwl", WorkflowCrate.workflowText
                    "workflows/W/tool.cwl", WorkflowCrate.toolText
                ]
            let fs = WorkflowCrate.fileSystemOf ["runs/R/run.cwl"; "workflows/W/workflow.cwl"; "workflows/W/tool.cwl"; "workflows/W/Dockerfile"; "workflows/W/types.yml"]
            let paths = WorkflowConversion.getReferencedFilePaths("runs/R/run.cwl", run, fs)
            Expect.sequenceEqual paths ["workflows/W/workflow.cwl"; "workflows/W/Dockerfile"; "workflows/W/tool.cwl"; "workflows/W/types.yml"] "The run needs the workflow that it runs and every file of the workflow."
        )
        testCase "ReferencedFiles_MissingRemoteAndOutsideFilesAreLeftOut" (fun () ->
            let text =
                "cwlVersion: v1.2\nclass: Workflow\nrequirements:\n  DockerRequirement:\n    dockerFile: {$include: Dockerfile}\ninputs: {}\noutputs: {}\nsteps:\n  a:\n    run: missing.cwl\n    in: {}\n    out: []\n  b:\n    run: https://example.org/tool.cwl\n    in: {}\n    out: []\n  c:\n    run: ../../../outside.cwl\n    in: {}\n    out: []\n  d:\n    run: /workflows/W/tool.cwl\n    in: {}\n    out: []\n"
            let workflow = WorkflowCrate.resolve "workflows/W/workflow.cwl" ["workflows/W/workflow.cwl", text]
            let fs = WorkflowCrate.fileSystemOf ["workflows/W/workflow.cwl"; "workflows/W/tool.cwl"; "outside.cwl"]
            let paths = WorkflowConversion.getReferencedFilePaths("workflows/W/workflow.cwl", workflow, fs)
            Expect.isEmpty paths "No reference names a file of the ARC."
        )
        testCase "ReferencedFiles_ACycleEnds" (fun () ->
            let text (other : string) =
                $"cwlVersion: v1.2\nclass: Workflow\ninputs: {{}}\noutputs: {{}}\nsteps:\n  s:\n    run: {other}\n    in: {{}}\n    out: []\n"
            let a = CWL.Decode.decodeCWLProcessingUnit (text "b.cwl")
            let b = CWL.Decode.decodeCWLProcessingUnit (text "a.cwl")
            let stepOf (processingUnit : CWL.CWLProcessingUnit) =
                match processingUnit with
                | CWL.Workflow workflow -> (CWL.ListEntry.values workflow.Steps).[0]
                | other -> failwithf "Expected a workflow but got %A" other
            // Each step holds the other workflow, which no reader of an ARC does, so only the visited files end the walk.
            (stepOf a).ResolvedRun <- { Run = CWLRunResolver.tryRunFromProcessingUnit b }
            (stepOf b).ResolvedRun <- { Run = CWLRunResolver.tryRunFromProcessingUnit a }
            let fs = WorkflowCrate.fileSystemOf ["workflows/W/a.cwl"; "workflows/W/b.cwl"]
            let paths = WorkflowConversion.getReferencedFilePaths("workflows/W/a.cwl", a, fs)
            Expect.sequenceEqual paths ["workflows/W/b.cwl"] "The walk visits each file once and leaves out the main file."
        )
        testCase "ReferencedFiles_FileAndDirectoryLiterals" (fun () ->
            let text =
                "cwlVersion: v1.2\nclass: CommandLineTool\nrequirements:\n  InitialWorkDirRequirement:\n    listing:\n      - class: File\n        location: x.R\n      - class: Directory\n        location: data\n      - class: File\n        location: missing.R\n      - class: File\n        location: https://example.org/y.R\nbaseCommand: [Rscript, x.R]\ninputs:\n  table:\n    type: File\n    default:\n      class: File\n      path: tables/t.csv\n      secondaryFiles:\n        - class: File\n          location: tables/t.csv.idx\noutputs: []\n"
            let tool = CWL.Decode.decodeCWLProcessingUnit text
            let fs = WorkflowCrate.fileSystemOf ["workflows/W/workflow.cwl"; "workflows/W/x.R"; "workflows/W/data/a.txt"; "workflows/W/data/sub/b.txt"; "workflows/W/tables/t.csv"; "workflows/W/tables/t.csv.idx"; "workflows/W/other.txt"]
            let paths = WorkflowConversion.getReferencedFilePaths("workflows/W/workflow.cwl", tool, fs)
            Expect.equal (paths |> Seq.sort |> List.ofSeq) ["workflows/W/data/a.txt"; "workflows/W/data/sub/b.txt"; "workflows/W/tables/t.csv"; "workflows/W/tables/t.csv.idx"; "workflows/W/x.R"] "A File literal names its file, a Directory literal every file under its directory, and a missing or remote literal nothing."
        )
        testCase "WorkflowCrate_ListsTheFilesTheWorkflowNeeds" (fun () ->
            let workflow = create_workflow_full()
            workflow.CWLDescription <-
                WorkflowCrate.resolve "workflows/MyWorkflow/workflow.cwl" [
                    "workflows/MyWorkflow/workflow.cwl", WorkflowCrate.workflowText
                    "workflows/MyWorkflow/tool.cwl", WorkflowCrate.toolText
                ]
                |> Some
            let fs = WorkflowCrate.fileSystemOf ["workflows/MyWorkflow/workflow.cwl"; "workflows/MyWorkflow/tool.cwl"; "workflows/MyWorkflow/Dockerfile"]
            let json = ARCtrl.Json.ARC.ROCrate.writeWorkflowAsCrate(workflow, fs, WorkflowCrate.license)
            Expect.sequenceEqual (WorkflowCrate.hasPartIds json) ["./workflows/MyWorkflow/workflow.cwl"; "./workflows/MyWorkflow/Dockerfile"; "./workflows/MyWorkflow/tool.cwl"] "The crate holds the workflow, its include and its tool, and no file that is missing."
        )
        testCase "RunCrate_ListsTheWorkflowItRunsAndItsTools" (fun () ->
            let run = create_run_full()
            run.CWLDescription <-
                WorkflowCrate.resolve "runs/MyRun/run.cwl" [
                    "runs/MyRun/run.cwl", WorkflowCrate.runText
                    "workflows/W/workflow.cwl", WorkflowCrate.workflowText
                    "workflows/W/tool.cwl", WorkflowCrate.toolText
                ]
                |> Some
            let fs = WorkflowCrate.fileSystemOf ["runs/MyRun/run.cwl"; "workflows/W/workflow.cwl"; "workflows/W/tool.cwl"; "workflows/W/Dockerfile"; "workflows/W/types.yml"]
            let ids = WorkflowCrate.hasPartIds (ARCtrl.Json.ARC.ROCrate.writeRunAsCrate(run, fs, WorkflowCrate.license))
            for id in ["./runs/MyRun/run.cwl"; "./workflows/W/workflow.cwl"; "./workflows/W/Dockerfile"; "./workflows/W/tool.cwl"; "./workflows/W/types.yml"] do
                Expect.isTrue (List.contains id ids) $"The run crate holds {id}."
            Expect.equal (ids |> List.distinct |> List.length) ids.Length "Each part is listed once."
        )
        testCase "Crates_RootIsPublishedAtTheGivenDate" (fun () ->
            let workflow = create_workflow_full()
            let date = System.DateTime(2026, 1, 2, 3, 4, 5, System.DateTimeKind.Utc)
            let json = ARCtrl.Json.ARC.ROCrate.writeWorkflowAsCrate(workflow, WorkflowCrate.fileSystemOf [], WorkflowCrate.license, datePublished = date)
            Expect.stringContains json "\"datePublished\": \"2026-01-02T03:04:05" "The root of the workflow crate is published at the given date."
            let run = create_run_full()
            let json = ARCtrl.Json.ARC.ROCrate.writeRunAsCrate(run, WorkflowCrate.fileSystemOf [], WorkflowCrate.license, datePublished = date)
            Expect.stringContains json "\"datePublished\": \"2026-01-02T03:04:05" "The root of the run crate is published at the given date."
        )
        testCase "Crates_RootIsPublishedNowWithoutADate" (fun () ->
            let graph = JsonController.LDGraph.fromROCrateJsonString (ARCtrl.Json.ARC.ROCrate.writeWorkflowAsCrate(create_workflow_full(), WorkflowCrate.fileSystemOf [], WorkflowCrate.license))
            let root = Expect.wantSome (graph.TryGetNode "./") "The crate has a root."
            Expect.isSome (LDDataset.tryGetDatePublished(root, ?context = graph.TryGetContext())) "The root has a date of publication."
        )
        testCase "RunCrate_NamesItsProfilesAsEntities" (fun () ->
            let graph = JsonController.LDGraph.fromROCrateJsonString (ARCtrl.Json.ARC.ROCrate.writeRunAsCrate(create_run_full(), WorkflowCrate.fileSystemOf [], WorkflowCrate.license))
            let context = graph.TryGetContext()
            let root = Expect.wantSome (graph.TryGetNode "./") "The crate has a root."
            let profiles = root.GetPropertyNodes(LDDataset.conformsTo, graph = graph, ?context = context)
            let describe (node : LDNode) =
                let text (property : string) = node.TryGetPropertyAsSingleton(property, ?context = context) |> Option.map string
                node.Id, LDCreativeWork.validate(node, ?context = context), text LDCreativeWork.name, text LDCreativeWork.version
            Expect.equal (profiles |> Seq.map describe |> List.ofSeq) [
                "https://w3id.org/ro/wfrun/process/0.5", true, Some "Process Run Crate", Some "0.5"
                "https://w3id.org/ro/wfrun/workflow/0.5", true, Some "Workflow Run Crate", Some "0.5"
                "https://w3id.org/workflowhub/workflow-ro-crate/1.0", true, Some "Workflow RO-Crate", Some "1.0"
            ] "Each profile that the root conforms to is a CreativeWork with its name and version."
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
                let dm = Datamap(ResizeArray[dc1;dc2])
                let assay = ArcAssay("My Assay", measurementType = measurementType, datamap = dm)
                let ro_Assay = AssayConversion.composeAssay assay
                let assay' = AssayConversion.decomposeAssay ro_Assay
                let dm' = Expect.wantSome assay'.Datamap "Assay should have a data map"
                Expect.equal dm dm' "Data map should match"
                let measurementType' = Expect.wantSome assay'.MeasurementType "Assay should have a measurement type"
                Expect.equal measurementType measurementType' "Measurement type should match"
                Expect.equal assay' assay "Assay should match"
            )
            testCase "MeasurementType_Flattened" (fun () ->
                let measurementType = OntologyAnnotation(name = "sugar measurement", tsr = "DPBO", tan = "DPBO:0000120")
                let dc1 = create_dataContextOfRow "MyFile" 1
                let dc2 = create_dataContextOfRow "MyFile" 2
                let dm = Datamap(ResizeArray[dc1;dc2])
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
                let dm = Datamap(ResizeArray[dc1;dc2])
                let input1 = CompositeCell.createData(create_dataRow "MyFile" 1)
                let input2 = CompositeCell.createData(create_dataRow "MyFile" 2)
                let assay = ArcAssay("My Assay", datamap = dm)
                let table = assay.InitTable("Table")
                table.AddColumn(CompositeHeader.Input IOType.Data, ResizeArray [|input1; input2|])
                let ro_Assay = AssayConversion.composeAssay assay
                let assay' = AssayConversion.decomposeAssay ro_Assay
                let dm' = Expect.wantSome assay'.Datamap "Assay should have a data map"
                Expect.equal dm dm' "Data map should match"
                let table' = Expect.wantSome (Seq.tryExactlyOne assay'.Tables) "Assay should have a table"
                Expect.arcTableEqual table table' "Table should match"
                Expect.equal assay' assay "Assay should match"
            )
            testCase "ContextForFiles_Flattened" (fun () ->
                let dc1 = create_dataContextOfRow "MyFile" 1
                let dc2 = create_dataContextOfRow "MyFile" 2
                let dm = Datamap(ResizeArray[dc1;dc2])
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
                let dm' = Expect.wantSome assay'.Datamap "Assay should have a data map"
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
                let dm = Datamap(ResizeArray[dc1;dc2])
                let input1 = CompositeCell.createData(create_dataRow "MyFile" 1)
                let input2 = CompositeCell.createData(create_dataRow "MyFile" 2)
                let study = ArcStudy("My Study", datamap = dm)
                let table = study.InitTable("Table")
                table.AddColumn(CompositeHeader.Input IOType.Data, ResizeArray [|input1; input2|])
                let ro_Study = StudyConversion.composeStudy study
                let study' = StudyConversion.decomposeStudy ro_Study
                let dm' = Expect.wantSome study'.Datamap "Study should have a data map"
                Expect.equal dm dm' "Data map should match"
                let table' = Expect.wantSome (Seq.tryExactlyOne study'.Tables) "Study should have a table"
                Expect.arcTableEqual table table' "Table should match"
                Expect.equal study' study "Study should match"
            )
            testCase "ContextForFiles_Flattened" (fun () ->
                let dc1 = create_dataContextOfRow "MyFile" 1
                let dc2 = create_dataContextOfRow "MyFile" 2
                let dm = Datamap(ResizeArray[dc1;dc2])
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
                let dm' = Expect.wantSome study'.Datamap "Study should have a data map"
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
        testCase "AssayAndStudy_FromScaffold_Flattened" (fun () ->
            let assay = ArcAssay.init("My Assay")
            let study = ArcStudy.init("My Study")
            let p = ArcInvestigation(
                identifier = "My Investigation",
                title = "My Best Investigation",
                assays = ResizeArray [assay],
                studies = ResizeArray [study]
                )
            let ro_Investigation = InvestigationConversion.composeInvestigation p
            let graph = ro_Investigation.Flatten()
            // Test that flatten worked
            Expect.isTrue (graph.Nodes.Count > 0) "Graph should have properties"
            let datasetRef = Expect.wantSome (ro_Investigation.TryGetPropertyAsSingleton(LDDataset.hasPart)) "Investigation should still have sub datasets"
            Expect.isTrue (datasetRef :? LDRef) "Investigation should be flattened correctly"
            //
            let p' = InvestigationConversion.decomposeInvestigation(ro_Investigation, graph = graph)
            Expect.isSome p'.PublicReleaseDate "PublicReleaseDate default value should be set"
            p'.PublicReleaseDate <- None // As a default value is used otherwise
            Expect.equal p' p "Investigation should match"
        )
        testCase "AssayStudyWorkflowRun_FromScaffold" (fun () ->
            let assay = create_assay_full()
            let study = create_study_full ()
            let workflow = create_workflow_full()
            let run = create_run_full()
            let p = ArcInvestigation(
                identifier = "My Investigation",
                title = "My Best Investigation",
                assays = ResizeArray [assay],
                studies = ResizeArray [study],
                workflows = ResizeArray [workflow],
                runs = ResizeArray [run]
                )
            let ro_Investigation = InvestigationConversion.composeInvestigation p
            let p' = InvestigationConversion.decomposeInvestigation ro_Investigation
            Expect.isSome p'.PublicReleaseDate "PublicReleaseDate default value should be set"
            p'.PublicReleaseDate <- None // As a default value is used otherwise
            Expect.equal p' p "Investigation should match"
        )
        testCase "AssayStudyWorkflowRun_FromScaffold_Flattened" (fun () ->
            let assay = create_assay_full()
            let study = create_study_full ()
            let workflow = create_workflow_full()
            let run = create_run_full()
            let p = ArcInvestigation(
                identifier = "My Investigation",
                title = "My Best Investigation",
                assays = ResizeArray [assay],
                studies = ResizeArray [study],
                workflows = ResizeArray [workflow],
                runs = ResizeArray [run]
                )
            let ro_Investigation = InvestigationConversion.composeInvestigation p
            let graph = ro_Investigation.Flatten()
            // Test that flatten worked
            Expect.isTrue (graph.Nodes.Count > 0) "Graph should have properties"
            let datasetRef = Expect.wantSome (ro_Investigation.TryGetPropertyAsSingleton(LDDataset.hasPart)) "Investigation should still have sub datasets"
            Expect.isTrue (datasetRef :? LDRef) "Investigation should be flattened correctly"
            //
            let p' = InvestigationConversion.decomposeInvestigation(ro_Investigation, graph = graph)
            Expect.isSome p'.PublicReleaseDate "PublicReleaseDate default value should be set"
            p'.PublicReleaseDate <- None // As a default value is used otherwise
            Expect.equal p' p "Investigation should match"
        )
        // https://github.com/nfdi4plants/ARCtrl/issues/651
        testCase "RegisteredAssays_FromScaffold_Flattened" (fun () ->
            let investigation = ArcInvestigation("My Investigation", title = "My Best Investigation")
            let study = investigation.InitStudy("My Study")
            investigation.InitAssay("My Assay") |> ignore
            study.RegisterAssay("My Assay")
            study.RegisterAssay("Missing Assay")
            let ro_Investigation = InvestigationConversion.composeInvestigation investigation
            let graph = ro_Investigation.Flatten()
            Expect.isNone (graph.TryGetNode(LDDataset.genIDAssay "Missing Assay")) "Missing assay should not be referenced"
            let investigation' = InvestigationConversion.decomposeInvestigation(ro_Investigation, graph = graph)
            let study' = Expect.wantSome (investigation'.TryGetStudy "My Study") "Study should exist"
            Expect.sequenceEqual study'.RegisteredAssayIdentifiers ["My Assay"] "Existing registered assay should be read"
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
        testCase "RunNoCWL" (fun () ->
            let run = ArcRun("MyRun")
            let p = ArcInvestigation(
                identifier = "My Investigation",
                title = "My Best Investigation",
                runs = ResizeArray [run]
                )
            Expect.throws (fun () -> InvestigationConversion.composeInvestigation p |> ignore ) "Investigation with Run without CWL should throw error"
        )
        testCase "RunNoCWL_Ignore" (fun () ->
            let run = ArcRun("MyRun")
            let p = ArcInvestigation(
                identifier = "My Investigation",
                title = "My Best Investigation",
                runs = ResizeArray [run]
                )
            let ro_Investigation = InvestigationConversion.composeInvestigation(p,ignoreBrokenWR = true)
            let datasets = LDDataset.getHasPartsAsDataset ro_Investigation
            Expect.hasLength datasets 0 "Investigation should have no sub datasets (runs ignored)"
        )
        testCase "WorkflowNoCWL" (fun () ->
            let workflow = ArcWorkflow("MyWorkflow")
            let p = ArcInvestigation(
                identifier = "My Investigation",
                title = "My Best Investigation",
                workflows = ResizeArray [workflow]
                )
            Expect.throws (fun () -> InvestigationConversion.composeInvestigation p |> ignore ) "Investigation with Workflow without CWL should throw error"
        )
        testCase "WorkflowNoCWL_Ignore" (fun () ->
            let workflow = ArcWorkflow("MyWorkflow")
            let p = ArcInvestigation(
                identifier = "My Investigation",
                title = "My Best Investigation",
                workflows = ResizeArray [workflow]
                )
            let ro_Investigation = InvestigationConversion.composeInvestigation(p,ignoreBrokenWR = true)
            let datasets = LDDataset.getHasPartsAsDataset ro_Investigation
            Expect.hasLength datasets 0 "Investigation should have no sub datasets (workflows ignored)"
        )
        testCase "ArbitraryDateTimeSurvivesReadWrite" (fun () ->
            let submissionDate = "Hello"
            let publicaReleaseDate = "World"
            let dateModified = "!"
            let mdComment = Comment("dateModified", dateModified)
            let inv = ArcInvestigation("MyInv", title = "MyTitle", submissionDate = submissionDate, publicReleaseDate = publicaReleaseDate, comments = ResizeArray [mdComment])
            let ro_Investigation = InvestigationConversion.composeInvestigation inv
            let inv' = InvestigationConversion.decomposeInvestigation ro_Investigation
            Expect.equal inv'.SubmissionDate (Some submissionDate) "SubmissionDate should match"
            Expect.equal inv'.PublicReleaseDate (Some publicaReleaseDate) "PublicReleaseDate should match"
            let mdComment' = Expect.wantSome (inv'.Comments |> Seq.tryFind (fun c -> c.Name = Some "dateModified")) "dateModified comment should exist"
            Expect.equal mdComment'.Value (Some dateModified) "dateModified comment value should match"
        )
        // https://github.com/nfdi4plants/ARCtrl/issues/642
        testCase "DateModifiedCommentNotDuplicated" (fun () ->
            let dateModified = "2020-05-06"
            let mdComment = Comment("dateModified", dateModified)
            let inv = ArcInvestigation("MyInv", title = "MyTitle", comments = ResizeArray [mdComment])
            let ro_Investigation = InvestigationConversion.composeInvestigation inv
            let inv' = InvestigationConversion.decomposeInvestigation ro_Investigation
            let mdComments = inv'.Comments |> Seq.filter (fun c -> c.Name = Some "dateModified") |> Seq.toList
            Expect.hasLength mdComments 1 "There should be exactly one dateModified comment"
            Expect.equal mdComments.[0].Value (Some dateModified) "dateModified comment value should match"
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
        tests_Datamap
        tests_FormalParameter
        tests_AdditionalTypeDecoding
        tests_AdditionalTypeEncoding
        tests_AdditionalTypeErrorHandling
        tests_YAMLInputValue
        tests_ToolDescription
        tests_OperationDescription
        tests_ProcessOfUnknownClass
        tests_WorkflowInvocation
        tests_ArcWorkflow
        tests_ArcRun
        tests_WorkflowCrate
        tests_Assay
        tests_Study
        tests_Investigation
    ]
