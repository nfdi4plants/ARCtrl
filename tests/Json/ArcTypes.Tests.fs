module Tests.ArcTypes

open TestingUtils

open ARCtrl.ISA
open ARCtrl.ISA.Json

open Thoth.Json.Core

let tests_CompositeCell =
  testList "CompositeCell" [
    let cell_freetext = CompositeCell.FreeText "Hello World"
    let cell_term = CompositeCell.Term (OntologyAnnotation.fromString("My Name", "MY", "MY:1"))
    let cell_term_empty = CompositeCell.emptyTerm
    let cell_unitized = CompositeCell.Unitized ("42", OntologyAnnotation.fromString("My Name", "MY", "MY:1"))
    let cell_unitized_empty = CompositeCell.emptyUnitized
    let cell_freetext_jsonString = sprintf """{"%s":"FreeText","values":["Hello World"]}""" CompositeCell.CellType
    let cell_term_jsonString = sprintf """{"%s":"Term","values":[{"annotationValue":"My Name","termSource":"MY","termAccession":"MY:1"}]}""" CompositeCell.CellType
    let cell_term_empty_jsonString = sprintf """{"%s":"Term","values":[{}]}""" CompositeCell.CellType
    let cell_unitized_jsonString = sprintf """{"%s":"Unitized","values":["42",{"annotationValue":"My Name","termSource":"MY","termAccession":"MY:1"}]}""" CompositeCell.CellType
    let cell_unitized_empty_jsonString = sprintf """{"%s":"Unitized","values":["",{}]}""" CompositeCell.CellType
    testList "encoder (toJsonString)" [
      testCase "FreeText" <| fun _ -> 
        let actual = CompositeCell.encoder cell_freetext |> GEncode.toJsonString  0
        let expected = cell_freetext_jsonString
        Expect.equal actual expected ""
      testCase "Term" <| fun _ -> 
        let actual = CompositeCell.encoder cell_term |> GEncode.toJsonString  0
        let expected = cell_term_jsonString
        Expect.equal actual expected ""
      testCase "Term empty" <| fun _ -> 
        let actual = CompositeCell.encoder cell_term_empty |> GEncode.toJsonString  0
        let expected = cell_term_empty_jsonString
        Expect.equal actual expected ""
      testCase "Unitized" <| fun _ -> 
        let actual = CompositeCell.encoder cell_unitized |> GEncode.toJsonString  0
        let expected = cell_unitized_jsonString
        Expect.equal actual expected ""
      testCase "Unitized empty" <| fun _ -> 
        let actual = CompositeCell.encoder cell_unitized_empty |> GEncode.toJsonString  0
        let expected = cell_unitized_empty_jsonString
        Expect.equal actual expected ""
    ]
    testList "decoder (fromJsonString)" [ 
      testCase "FreeText" <| fun _ -> 
        let actual = GDecode.fromJsonString CompositeCell.decoder cell_freetext_jsonString
        let expected = cell_freetext
        Expect.equal actual expected ""
      testCase "Term" <| fun _ -> 
        let actual = GDecode.fromJsonString CompositeCell.decoder cell_term_jsonString
        let expected = cell_term
        Expect.equal actual expected ""
      testCase "Term empty" <| fun _ -> 
        let actual = GDecode.fromJsonString CompositeCell.decoder cell_term_empty_jsonString
        let expected = cell_term_empty
        Expect.equal actual expected ""
      testCase "Unitized" <| fun _ -> 
        let actual = GDecode.fromJsonString CompositeCell.decoder cell_unitized_jsonString
        let expected = cell_unitized
        Expect.equal actual expected ""
      testCase "Unitized empty" <| fun _ -> 
        let actual = GDecode.fromJsonString CompositeCell.decoder cell_unitized_empty_jsonString
        let expected = cell_unitized_empty
        Expect.equal actual expected ""
    ]
  ]

let tests_IOType = testList "IOType" [
  let io_freetext = IOType.FreeText "Hello World"
  let io_freetext_jsonString =  "\"Hello World\"" 
  let io_sample = IOType.Sample
  let io_sample_jsonString =  "\"Sample Name\"" 
  testList "encoder" [
    testCase "FreeText" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| IOType.encoder io_freetext
      let expected = io_freetext_jsonString
      Expect.equal actual expected ""
    testCase "Sample" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| IOType.encoder io_sample
      let expected = io_sample_jsonString
      Expect.equal actual expected ""
  ]
  testList "decoder" [
    testCase "FreeText" <| fun _ ->
      let actual = GDecode.fromJsonString IOType.decoder io_freetext_jsonString 
      let expected = io_freetext
      Expect.equal actual expected ""
    testCase "Sample" <| fun _ ->
      let actual = GDecode.fromJsonString IOType.decoder io_sample_jsonString
      let expected = io_sample
      Expect.equal actual expected ""
  ]
]

let tests_CompositeHeader = testList "CompositeHeader" [
  let header_parameter = CompositeHeader.Parameter (OntologyAnnotation.fromString("My Name", "MY", "MY:2"))
  let header_parameter_jsonString =  """{"headertype":"Parameter","values":[{"annotationValue":"My Name","termSource":"MY","termAccession":"MY:2"}]}""" 
  let header_input_source = CompositeHeader.Input IOType.Source
  let header_input_source_jsonString =  """{"headertype":"Input","values":["Source Name"]}""" 
  let header_protocolref = CompositeHeader.ProtocolREF
  let header_protocolref_jsonString =  """{"headertype":"ProtocolREF","values":[]}""" 
  testList "encoder" [
    testCase "Parameter" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| CompositeHeader.encoder header_parameter
      let expected = header_parameter_jsonString
      Expect.equal actual expected ""
    testCase "Input Source" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| CompositeHeader.encoder header_input_source
      let expected = header_input_source_jsonString
      Expect.equal actual expected ""
    testCase "ProtocolREF" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| CompositeHeader.encoder header_protocolref
      let expected = header_protocolref_jsonString
      Expect.equal actual expected ""
  ]
  testList "decoder" [
    testCase "Parameter" <| fun _ ->
      let actual = GDecode.fromJsonString CompositeHeader.decoder header_parameter_jsonString
      let expected = header_parameter
      Expect.equal actual expected ""
    testCase "Input Source" <| fun _ ->
      let actual = GDecode.fromJsonString CompositeHeader.decoder header_input_source_jsonString
      let expected = header_input_source
      Expect.equal actual expected ""
    testCase "ProtocolREF" <| fun _ ->
      let actual = GDecode.fromJsonString CompositeHeader.decoder header_protocolref_jsonString
      let expected = header_protocolref
      Expect.equal actual expected ""
  ]
]

let tests_ArcTable = testList "ArcTable" [
  let init = ArcTable.init("New Table")
  let init_jsonString =  """{"name":"New Table"}""" 
  let filled = ArcTable.create(
    "New Table", 
    ResizeArray([
      CompositeHeader.Input IOType.Source; 
      CompositeHeader.Component (OntologyAnnotation.fromString("instrument model", "MS", "MS:424242")); 
      CompositeHeader.Output IOType.Sample
    ]),
    System.Collections.Generic.Dictionary(Map [
      (0,0), CompositeCell.FreeText "Input 1"
      (0,1), CompositeCell.FreeText "Input 2"
      (1,0), CompositeCell.Term (OntologyAnnotation.fromString("SCIES instrument model"))
      (1,1), CompositeCell.Term (OntologyAnnotation.fromString("SCIES instrument model"))
      (2,0), CompositeCell.FreeText "Output 2"
      (2,1), CompositeCell.FreeText "Output 2"
    ])
  )
  let filled_jsonString =  """{"name":"New Table","header":[{"headertype":"Input","values":["Source Name"]},{"headertype":"Component","values":[{"annotationValue":"instrument model","termSource":"MS","termAccession":"MS:424242"}]},{"headertype":"Output","values":["Sample Name"]}],"values":[[[0,0],{"celltype":"FreeText","values":["Input 1"]}],[[0,1],{"celltype":"FreeText","values":["Input 2"]}],[[1,0],{"celltype":"Term","values":[{"annotationValue":"SCIES instrument model"}]}],[[1,1],{"celltype":"Term","values":[{"annotationValue":"SCIES instrument model"}]}],[[2,0],{"celltype":"FreeText","values":["Output 2"]}],[[2,1],{"celltype":"FreeText","values":["Output 2"]}]]}""" 
  testList "encode" [
    testCase "Empty" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| ArcTable.encoder init
      let expected = init_jsonString
      Expect.equal actual expected ""
    testCase "Filled" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| ArcTable.encoder filled
      let expected = filled_jsonString
      Expect.equal actual expected ""
  ]
  testList "decode" [
    testCase "Empty" <| fun _ ->
      let actual = GDecode.fromJsonString ArcTable.decoder init_jsonString
      let expected = init
      Expect.equal actual expected ""
    testCase "Filled" <| fun _ ->
      let actual = GDecode.fromJsonString ArcTable.decoder filled_jsonString
      let expected = filled
      Expect.equal actual expected ""
  ]
  testList "compressedIO" [
    testCase "Empty" <| fun _ ->
      let encoded = ArcTable.toCompressedJsonString init
      let decoded = ArcTable.fromCompressedJsonString encoded
      Expect.equal decoded init "empty table is wrong after compressed encoding and decoding"
    testCase "Filled" <| fun _ ->
      let encoded = ArcTable.toCompressedJsonString filled
      let decoded = ArcTable.fromCompressedJsonString encoded
      Expect.equal decoded filled "empty table is wrong after compressed encoding and decoding"
    // Set to pTest in Fable, as compressed parsing is around 10times slower than uncompressed parsing. This is weird, since in dotnet it is around 10times faster
    testCase "rangeColumnSize" <| fun _ ->
        // testTable column should be saved as range column, this should make it smaller than the IO column even though it has more cells
        let testTable = ArcTable.init("Test")
        testTable.AddColumn (CompositeHeader.FreeText "ABC", [|for i = 0 to 200 do yield CompositeCell.FreeText "TestValue"|])
        let referenceTable = ArcTable.init("Test")
        referenceTable.AddColumn (CompositeHeader.Input IOType.Source, [|for i = 0 to 100 do yield CompositeCell.FreeText "TestValue"|])
        let compressedTest = testTable.ToCompressedJsonString()
        let compressedReference = referenceTable.ToCompressedJsonString()
        Expect.isTrue (compressedTest.Length < compressedReference.Length) "range column should be smaller than IO column"
        let decompressedTest = ArcTable.fromCompressedJsonString compressedTest
        Expect.arcTableEqual decompressedTest testTable "decompressed table should be equal to original table"
    testCase "rangeColumnCorrectness" <| fun _ ->
        let testTable = ArcTable.init("Test")
        testTable.AddColumn (CompositeHeader.FreeText "ABC")
        for i = 0 to 100 do testTable.AddRow ([|CompositeCell.FreeText "TestValue1"|])
        for i = 101 to 200 do testTable.AddRow ([|CompositeCell.FreeText "TestValue2"|])
        testTable.AddRow ([|CompositeCell.FreeText "TestValue3"|])
        let encoded = testTable.ToCompressedJsonString()
        let decoded = ArcTable.fromCompressedJsonString encoded
        Expect.arcTableEqual decoded testTable "decompressed table should be equal to original table"
  ]
]

let tests_ArcAssay = testList "ArcAssay" [
  let init = ArcAssay.create("My Assay")
  let init_jsonString = 
    """{"Identifier":"My Assay"}"""
  let filled = ArcAssay.create(
    "My Assay",
    OntologyAnnotation.empty, 
    OntologyAnnotation.empty, 
    OntologyAnnotation.empty, 
    ResizeArray([ArcTable.init("Table 1"); ArcTable.init("Table 2")]),
    [|Person.create(FirstName="Kevin", LastName="Frey")|],
    [|Comment.create("Hello", "World")|]
  )
  let filled_jsonString = 
    """{"Identifier":"My Assay","MeasurementType":{},"TechnologyType":{},"TechnologyPlatform":{},"Tables":[{"name":"Table 1"},{"name":"Table 2"}],"Performers":[{"firstName":"Kevin","lastName":"Frey"}],"Comments":[{"@id":"Hello","name":"World"}]}"""

  testList "encode" [
    testCase "Empty" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| ArcAssay.encoder init
      let expected = init_jsonString
      Expect.equal actual expected ""
    testCase "Filled" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| ArcAssay.encoder filled
      let expected = filled_jsonString
      Expect.equal actual expected ""
  ]
  testList "decode" [
    testCase "Empty" <| fun _ ->
      let actual = GDecode.fromJsonString ArcAssay.decoder init_jsonString
      let expected = init
      Expect.equal actual expected ""
    testCase "Filled" <| fun _ ->
      let actual = GDecode.fromJsonString ArcAssay.decoder filled_jsonString
      let expected = filled
      Expect.equal actual expected ""
  ]
  testList "compressedIO" [
    testCase "Empty" <| fun _ ->
      let encoded = ArcAssay.toCompressedJsonString init
      let decoded = ArcAssay.fromCompressedJsonString encoded
      Expect.equal decoded init "empty table is wrong after compressed encoding and decoding"
    testCase "Filled" <| fun _ ->
      let encoded = ArcAssay.toCompressedJsonString filled
      let decoded = ArcAssay.fromCompressedJsonString encoded
      Expect.equal decoded filled "empty table is wrong after compressed encoding and decoding"
  ]
]

let tests_ArcStudy = testList "ArcStudy" [
  let init = ArcStudy.create(
    "My Study"
  )
  let init_jsonString = 
    """{"Identifier":"My Study"}"""
  let filled = ArcStudy.create(
    "My Study",
    "My Title",
    "My Description",
    "My Submission Date",
    "My Release Date",
    [|Publication.create(Doi="any-nice-doi-42")|],
    [|Person.create(FirstName="Kevin", LastName="Frey")|], 
    [|OntologyAnnotation.empty; OntologyAnnotation.empty|],
    ResizeArray([ArcTable.init("Table 1"); ArcTable.init("Table 2")]),
    ResizeArray(["Assay 1"; "Assay 2"]),
    [|Factor.empty; Factor.create(Name="My Factor")|],
    [|Comment.create("Hello", "World")|]
  )
  let filled_jsonString = 
    """{"Identifier":"My Study","Title":"My Title","Description":"My Description","SubmissionDate":"My Submission Date","PublicReleaseDate":"My Release Date","Publications":[{"doi":"any-nice-doi-42"}],"Contacts":[{"firstName":"Kevin","lastName":"Frey"}],"StudyDesignDescriptors":[{},{}],"Tables":[{"name":"Table 1"},{"name":"Table 2"}],"RegisteredAssayIdentifiers":["Assay 1","Assay 2"],"Factors":[{},{"factorName":"My Factor"}],"Comments":[{"@id":"Hello","name":"World"}]}"""
  testList "encode" [
    testCase "Empty" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| ArcStudy.encoder init
      let expected = init_jsonString
      Expect.equal actual expected ""
    testCase "Filled" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| ArcStudy.encoder filled
      let expected = filled_jsonString
      Expect.equal actual expected ""
  ]
  testList "decode" [
    testCase "Empty" <| fun _ ->
      let actual = GDecode.fromJsonString ArcStudy.decoder init_jsonString
      let expected = init
      Expect.equal actual expected ""
    testCase "Filled" <| fun _ ->
      let actual = GDecode.fromJsonString ArcStudy.decoder filled_jsonString
      let expected = filled
      Expect.equal actual expected ""
  ]
]


let tests_ArcInvestigation = testList "ArcInvestigation" [
  let init = ArcInvestigation.create(
    "My Inv"
  )
  let init_jsonString = 
    """{"Identifier":"My Inv"}"""
  let filled = ArcInvestigation.create(
    "My Inv",
    "My Title",
    "My Description",
    "My Submission Date",
    "My Release Date",
    [|OntologySourceReference.create("Description", "path/to/file", "OSR Name")|],
    [|Publication.create(Doi="any-nice-doi-42")|],
    [|Person.create(FirstName="Kevin", LastName="Frey")|], 
    ResizeArray([ArcAssay.init("Assay 1"); ArcAssay.init("Assay 2")]),
    ResizeArray([ArcStudy.init("Study")]),
    ResizeArray(["Study"]),
    [|Comment.create("Hello", "World")|]
  )
  let filled_jsonString = 
    """{"Identifier":"My Inv","Title":"My Title","Description":"My Description","SubmissionDate":"My Submission Date","PublicReleaseDate":"My Release Date","OntologySourceReferences":[{"description":"Description","file":"path/to/file","name":"OSR Name"}],"Publications":[{"doi":"any-nice-doi-42"}],"Contacts":[{"firstName":"Kevin","lastName":"Frey"}],"Assays":[{"Identifier":"Assay 1"},{"Identifier":"Assay 2"}],"Studies":[{"Identifier":"Study"}],"RegisteredStudyIdentifiers":["Study"],"Comments":[{"@id":"Hello","name":"World"}]}"""

  testList "encode" [
    testCase "Empty" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| ArcInvestigation.encoder init
      let expected = init_jsonString
      Expect.equal actual expected ""
    testCase "Filled" <| fun _ ->
      let actual = GEncode.toJsonString  0 <| ArcInvestigation.encoder filled
      let expected = filled_jsonString
      Expect.equal actual expected ""
  ]
  testList "decode" [
    testCase "Empty" <| fun _ ->
      let actual = GDecode.fromJsonString ArcInvestigation.decoder init_jsonString
      let expected = init
      Expect.equal actual expected ""
    testCase "Filled" <| fun _ ->
      let actual = GDecode.fromJsonString ArcInvestigation.decoder filled_jsonString
      let expected = filled
      Expect.equal actual expected ""
  ]
]

let Main = testList "ArcTypes" [
    tests_CompositeCell
    tests_IOType
    tests_CompositeHeader
    tests_ArcTable
    tests_ArcAssay
    tests_ArcStudy
    tests_ArcInvestigation
]