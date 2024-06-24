module Tests.Study

open ARCtrl
open ARCtrl.Json
open TestingUtils


module Helper =
    
    let create_empty() = ArcStudy.init("My Fancy Stundy")
    let create_filled() = ArcStudy.create(
        "My Study",
        "My Title",
        "My Description",
        #if !FABLE_COMPILER
        "2024-03-15T20:20:39",
        #else
        "2024-03-15",
        #endif        
        "2024-04-20",
        ResizeArray [|Publication.create(doi="any-nice-doi-42")|],
        ResizeArray [|Person.create(firstName="Kevin", lastName="Frey", phone="023382093810")|], 
        ResizeArray [|OntologyAnnotation(); OntologyAnnotation()|],
        ResizeArray [ArcTable.Helper.create_filled(); ArcTable.init("Table 2")],
        registeredAssayIdentifiers = ResizeArray ["Assay 1"; "Assay 2"],
        comments = ResizeArray [|Comment.create("Hello", "World")|]
    )

    let compareFields =
        fun (actual: ArcStudy) (expected: ArcStudy) ->
            Expect.equal actual.Identifier expected.Identifier "Identifier"
            Expect.equal actual.Title expected.Title "Title"
            Expect.equal actual.Description expected.Description "Description"
            Expect.equal actual.SubmissionDate expected.SubmissionDate "SubmissionDate"
            Expect.equal actual.PublicReleaseDate expected.PublicReleaseDate "PublicReleaseDate"
            Expect.sequenceEqual actual.Publications expected.Publications "Publications"
            Expect.sequenceEqual actual.Contacts expected.Contacts "Contacts"
            Expect.sequenceEqual actual.StudyDesignDescriptors expected.StudyDesignDescriptors "StudyDesignDescriptors"
            Expect.sequenceEqual actual.Tables expected.Tables "Tables"
            Expect.sequenceEqual actual.RegisteredAssayIdentifiers expected.RegisteredAssayIdentifiers "RegisteredAssayIdentifiers"
            Expect.sequenceEqual actual.Comments expected.Comments "RegisteredAssayIdentifiers"
        |> Some

open Helper


let private test_coreEmpty =
    createBaseJsonTests
        "core-empty"
        create_empty
        ArcStudy.toJsonString
        ArcStudy.fromJsonString
        None
        compareFields

let private test_compressedEmpty =
    createBaseJsonTests
        "compressed-empty"
        create_empty
        ArcStudy.toCompressedJsonString
        ArcStudy.fromCompressedJsonString
        None
        compareFields

let private test_isaEmpty =
    createBaseJsonTests
        "isa-empty"
        create_empty
        (fun () (s) -> ArcStudy.toISAJsonString [] s)
        (ArcStudy.fromISAJsonString >> fun (s,_) -> s)
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateStudy)
        #else
        None
        #endif
        compareFields

let private test_roCrateEmpty =

    createBaseJsonTests
        "ROCrate-empty"
        create_empty
        (fun () (s) -> ArcStudy.toROCrateJsonString [] s)
        (ArcStudy.fromROCrateJsonString >> fun (s,_) -> s)
        None
        compareFields

let private test_core =
    createBaseJsonTests
        "core"
        create_filled
        ArcStudy.toJsonString
        ArcStudy.fromJsonString
        None
        compareFields

let private test_compressed =
    createBaseJsonTests
        "compressed"
        create_filled
        ArcStudy.toCompressedJsonString
        ArcStudy.fromCompressedJsonString
        None
        compareFields

let private test_isa = testList "ISA" [
    createBaseJsonTests
        "base"
        create_filled
        ArcStudy.toISAJsonString
        (ArcStudy.fromISAJsonString >> fun (s,_) -> s)
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateStudy)
        #else
        None
        #endif
        compareFields

    testCase "UseIDTable" <| fun _ ->
        let table = ArcTable.init ("Process1")

        let inputHeader = CompositeHeader.Input (IOType.Source)
        let inputCells = [|for i = 0 to 1 do CompositeCell.FreeText $"Source{i+1}"|]
        let inputColumn = CompositeColumn.create(inputHeader, inputCells)

        let organism = OntologyAnnotation.create("organism", "OBI", "http://purl.obolibrary.org/obo/OBI_0100026")
        let organismHeader = CompositeHeader.Characteristic organism
        let arabidopsis = OntologyAnnotation.create("Arabidopsis thaliana", "NCBITaxon", "http://purl.obolibrary.org/obo/NCBITaxon_3702")
        let organismCells = [|for i = 0 to 1 do CompositeCell.Term arabidopsis|]
        let organismColumn = CompositeColumn.create(organismHeader, organismCells)


        let protocolName = "peptide_digestion"
        let protocolREFHeader = CompositeHeader.ProtocolREF
        let protocolREFCells = [|for i = 0 to 1 do CompositeCell.FreeText protocolName|]
        let protocolREFColumn = CompositeColumn.create(protocolREFHeader, protocolREFCells)

        let protocolType = OntologyAnnotation.create("Protein Digestion", "NCIT", "http://purl.obolibrary.org/obo/NCIT_C70845")
        let protocolTypeHeader = CompositeHeader.ProtocolType
        let protocolTypeCells = [|for i = 0 to 1 do CompositeCell.Term protocolType|]
        let protocolTypeColumn = CompositeColumn.create(protocolTypeHeader, protocolTypeCells)

        let protocolDescription = "The isolated proteins get solubilized. Given protease is added and the solution is heated to a given temperature. After a given amount of time, the digestion is stopped by adding a denaturation agent."
        let protocolDescriptionHeader = CompositeHeader.ProtocolDescription
        let protocolDescriptionCells = [|for i = 0 to 1 do CompositeCell.FreeText protocolDescription|]
        let protocolDescriptionColumn = CompositeColumn.create(protocolDescriptionHeader, protocolDescriptionCells)

        let protocolURI = "http://madeUpProtocolWebsize.org/protein_digestion"
        let protocolURIHeader = CompositeHeader.ProtocolUri
        let protocolURICells = [|for i = 0 to 1 do CompositeCell.FreeText protocolURI|]
        let protocolURIColumn = CompositeColumn.create(protocolURIHeader, protocolURICells)

        let protocolVersion = "1.0.0"
        let protocolVersionHeader = CompositeHeader.ProtocolVersion
        let protocolVersionCells = [|for i = 0 to 1 do CompositeCell.FreeText protocolVersion|]
        let protocolVersionColumn = CompositeColumn.create(protocolVersionHeader, protocolVersionCells)

        let peptidase = OntologyAnnotation.create("Peptidase", "MS", "http://purl.obolibrary.org/obo/NCIT_C16965")
        let peptidaseHeader = CompositeHeader.Parameter peptidase
        let trypsin = OntologyAnnotation.create("Trypsin/P", "MS", "http://purl.obolibrary.org/obo/NCIT_C16965")
        let peptidaseCells = [|for i = 0 to 1 do CompositeCell.Term trypsin|]
        let peptidaseColumn = CompositeColumn.create(peptidaseHeader, peptidaseCells)

        let temperature = OntologyAnnotation.create("temperature", "Ontobee", "http://purl.obolibrary.org/obo/NCRO_0000029")
        let temperatureHeader = CompositeHeader.Parameter temperature
        let degrees = OntologyAnnotation.create("degrees Celsius", "OM2", "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius")
        let temperatureCells = [|for i = 0 to 1 do CompositeCell.Unitized ("23",degrees)|]
        let temperatureColumn = CompositeColumn.create(temperatureHeader, temperatureCells)

        let time = OntologyAnnotation.create("time", "EFO", "http://www.ebi.ac.uk/efo/EFO_0000721")
        let timeHeader = CompositeHeader.Parameter time
        let hours = OntologyAnnotation.create("hour", "UO", "http://purl.obolibrary.org/obo/UO_0000032")
        let timeCells = [|for i = 0 to 1 do CompositeCell.Unitized ("10",hours)|]
        let timeColumn = CompositeColumn.create(timeHeader, timeCells)

        let outputHeader = CompositeHeader.Output (IOType.Sample)
        let outputCells = [|for i = 0 to 1 do CompositeCell.FreeText $"Sample{i+1}"|]
        let outputColumn = CompositeColumn.create(outputHeader, outputCells)

        table.AddColumns [|
            inputColumn;
            organismColumn;
            protocolREFColumn;
            protocolTypeColumn;
            protocolDescriptionColumn;
            protocolURIColumn;
            protocolVersionColumn;
            peptidaseColumn;
            temperatureColumn;
            timeColumn;
            outputColumn        
        |]

        let study = ArcStudy.init("Study1")
        study.AddTable table

        let json = ArcStudy.toISAJsonString(useIDReferencing = true) study

        //let expectedString = TestObjects.Json.Study.studyWithIDTable.Replace("\s","")

        Expect.stringEqual json TestObjects.Json.Study.studyWithIDTable "json"
     ]

let private test_roCrate =
    createBaseJsonTests
        "ROCrate"
        create_filled
        ArcStudy.toROCrateJsonString
        (ArcStudy.fromROCrateJsonString >> fun (s,_) -> s)
        None
        compareFields

let main = testList "Study" [
    test_coreEmpty
    test_core
    test_compressedEmpty
    test_compressed
    test_isaEmpty
    test_isa
    test_roCrateEmpty
    test_roCrate
]