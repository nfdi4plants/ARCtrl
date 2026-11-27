module Tests.Assay

open ARCtrl
open ARCtrl.Json
open TestingUtils

module Helper =
    let create_empty() = ArcAssay.create("My Assay")
    let create_filled_datamap() = 
        ArcAssay.create(
            "My Cool Assay",
            "Best Assay",
            "This is a test assay",
            OntologyAnnotation("MT", "MS", "MS:424242", comments = ResizeArray [Comment.create("ByeBye","Space")]), 
            OntologyAnnotation("TT", "MS", "MS:696969"), 
            OntologyAnnotation("TP", "MS", "MS:123456", comments = ResizeArray [Comment.create("Hello","Space")]), 
            ResizeArray([Tests.ArcTable.Helper.create_filled(); ArcTable.init("My Second Table")]),
            Datamap.Helper.create_filled(),
            performers = ResizeArray [|Person.create(firstName="Kevin", lastName="Frey")|],
            comments = ResizeArray [|Comment.create("Hello", "World")|]
        )
    let create_filled() = 
        ArcAssay.create(
            "My Cool Assay",
            "Best Assay",
            "This is a test assay",
            OntologyAnnotation("MT", "MS", "MS:424242", comments = ResizeArray [Comment.create("ByeBye","Space")]), 
            OntologyAnnotation("TT", "MS", "MS:696969"), 
            OntologyAnnotation("TP", "MS", "MS:123456", comments = ResizeArray [Comment.create("Hello","Space")]), 
            ResizeArray([Tests.ArcTable.Helper.create_filled(); ArcTable.init("My Second Table")]),
            performers = ResizeArray [|Person.create(firstName="Kevin", lastName="Frey")|],
            comments = ResizeArray [|Comment.create("Hello", "World")|]
        )

    /// Table with the given sample as output
    let create_sampleOutputTable (tableName : string) (sampleName : string) =
        let table = ArcTable.init(tableName)
        table.AddColumn(CompositeHeader.Input IOType.Source, ResizeArray [CompositeCell.FreeText $"{sampleName}_Source"])
        table.AddColumn(CompositeHeader.Output IOType.Sample, ResizeArray [CompositeCell.FreeText sampleName])
        table

    /// Table with the given sample as input, which is annotated with the given characteristic
    let create_sampleInputTable (tableName : string) (sampleName : string) (characteristic : string) (value : string) =
        let table = ArcTable.init(tableName)
        table.AddColumn(CompositeHeader.Input IOType.Sample, ResizeArray [CompositeCell.FreeText sampleName])
        table.AddColumn(CompositeHeader.Characteristic (OntologyAnnotation(characteristic)), ResizeArray [CompositeCell.Term (OntologyAnnotation(value))])
        table.AddColumn(CompositeHeader.Output IOType.Sample, ResizeArray [CompositeCell.FreeText $"{sampleName}_{tableName}"])
        table

    /// Decodes the characteristic values the given sample is written with in the materials of an ISA-Json study or assay
    let isaSampleCharacteristicsDecoder (sampleName : string) =
        Thoth.Json.Core.Decode.at ["materials"; "samples"] (Thoth.Json.Core.Decode.list Sample.ISAJson.decoder)
        |> Thoth.Json.Core.Decode.map (fun samples ->
            samples
            |> List.filter (fun s -> s.Name = Some sampleName)
            |> List.collect (fun s -> s.Characteristics |> Option.defaultValue [])
            |> List.map (fun c -> c.ValueText)
        )

    let compare =
        fun (a1: ArcAssay) (a2: ArcAssay) ->
            Expect.equal a1.Identifier a2.Identifier "Identifier"
            Expect.equal a1.Title a2.Title "Title"
            Expect.equal a1.Description a2.Description "Description"
            Expect.equal a1.MeasurementType a2.MeasurementType "MeasurementType"
            Expect.equal a1.TechnologyType a2.TechnologyType "TechnologyType"
            Expect.equal a1.TechnologyPlatform a2.TechnologyPlatform "TechnologyPlatform"
            Expect.equal a1.Datamap a2.Datamap "Datamap"
            Expect.sequenceEqual a1.Tables a2.Tables "Tables"
            Expect.sequenceEqual a1.Performers a2.Performers "Performers"
            Expect.sequenceEqual a1.Comments a2.Comments "Comments"
        |> Some

open Helper

let private test_coreEmpty =
    createBaseJsonTests
        "core-empty"
        create_empty
        ArcAssay.toJsonString
        ArcAssay.fromJsonString
        None
        compare

let private test_compressedEmpty =
    createBaseJsonTests
        "compressed-empty"
        create_empty
        ArcAssay.toCompressedJsonString
        ArcAssay.fromCompressedJsonString
        None
        compare

let private test_isaEmpty =
    createBaseJsonTests
        "isa-empty"
        create_empty
        ArcAssay.toISAJsonString
        ArcAssay.fromISAJsonString
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateAssay)
        #else
        None
        #endif
        compare

let private test_roCrateEmpty =
    createBaseJsonTests
        "ROCrate-empty"
        create_empty
        (fun () -> ArcAssay.toROCrateJsonString())
        ArcAssay.fromROCrateJsonString
        None
        compare

let private test_core =
    createBaseJsonTests
        "core"
        create_filled_datamap
        ArcAssay.toJsonString
        ArcAssay.fromJsonString
        None
        compare

let private test_compressed =
    createBaseJsonTests
        "compressed"
        create_filled_datamap
        ArcAssay.toCompressedJsonString
        ArcAssay.fromCompressedJsonString
        None
        compare

open TestObjects.Json

let private test_isa =
    createBaseJsonTests
        "isa"
        (fun () -> 
            let a = create_filled() 
            a.Performers <- ResizeArray() // ISA-JSON Assay does not contain persons
            a)
        ArcAssay.toISAJsonString
        ArcAssay.fromISAJsonString
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateAssay)
        #else
        None
        #endif
        compare

// https://github.com/nfdi4plants/ARCtrl/issues/492
let private test_isaIDTable =
    testCase "isa-UseIDTable_SampleAnnotatedInSecondTable" <| fun _ ->
        let assay = ArcAssay.init("Assay1")
        assay.AddTable (create_sampleOutputTable "Extraction" "Sample1")
        assay.AddTable (create_sampleInputTable "Measurement" "Sample1" "organism" "Arabidopsis thaliana")

        let json = ArcAssay.toISAJsonString(useIDReferencing = true) assay

        let characteristics = Decode.fromJsonString (isaSampleCharacteristicsDecoder "Sample1") json
        Expect.sequenceEqual characteristics ["Arabidopsis thaliana"] "Sample should be written with the characteristic it is annotated with in the second table"

let private test_roCrate = testList "ROCrate" [
    // Wait for some issues to be resolved: https://github.com/nfdi4plants/isa-ro-crate-profile/issues
    // Mainly: #12, #9, #10, #13
    ptestCase "Write" <| fun _ ->
        let a = create_filled()
        let json = ArcAssay.toROCrateJsonString() a
        ()
    createBaseJsonTests
        ""
        create_filled
        (fun () -> ArcAssay.toROCrateJsonString())
        ArcAssay.fromROCrateJsonString
        None
        compare
]

let main = testList "Assay" [
    test_coreEmpty
    test_compressedEmpty
    test_isaEmpty
    test_roCrateEmpty
    test_core
    test_compressed
    test_isa
    test_isaIDTable
    test_roCrate
]