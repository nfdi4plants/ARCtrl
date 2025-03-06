module Tests.Assay

open ARCtrl
open ARCtrl.Json
open TestingUtils

module Helper =
    let create_empty() = ArcAssay.create("My Assay")
    let create_filled_datamap() = 
        ArcAssay.create(
            "My Cool Assay",
            OntologyAnnotation("MT", "MS", "MS:424242", ResizeArray [Comment.create("ByeBye","Space")]), 
            OntologyAnnotation("TT", "MS", "MS:696969"), 
            OntologyAnnotation("TP", "MS", "MS:123456", ResizeArray [Comment.create("Hello","Space")]), 
            ResizeArray([Tests.ArcTable.Helper.create_filled(); ArcTable.init("My Second Table")]),
            DataMap.Helper.create_filled(),
            performers = ResizeArray [|Person.create(firstName="Kevin", lastName="Frey")|],
            comments = ResizeArray [|Comment.create("Hello", "World")|]
        )
    let create_filled() = 
        ArcAssay.create(
            "My Cool Assay",
            OntologyAnnotation("MT", "MS", "MS:424242", ResizeArray [Comment.create("ByeBye","Space")]), 
            OntologyAnnotation("TT", "MS", "MS:696969"), 
            OntologyAnnotation("TP", "MS", "MS:123456", ResizeArray [Comment.create("Hello","Space")]), 
            ResizeArray([Tests.ArcTable.Helper.create_filled(); ArcTable.init("My Second Table")]),
            performers = ResizeArray [|Person.create(firstName="Kevin", lastName="Frey")|],
            comments = ResizeArray [|Comment.create("Hello", "World")|]
        )

    let compare =
        fun (a1: ArcAssay) (a2: ArcAssay) ->
            Expect.equal a1.Identifier a2.Identifier "Identifier"
            Expect.equal a1.MeasurementType a2.MeasurementType "MeasurementType"
            Expect.equal a1.TechnologyType a2.TechnologyType "TechnologyType"
            Expect.equal a1.TechnologyPlatform a2.TechnologyPlatform "TechnologyPlatform"
            Expect.equal a1.DataMap a2.DataMap "DataMap"
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

let private test_isa = testList "ISA" [
    ptestCase "IDReferencing_SameSamples" <| fun _ ->
        let a = ArcAssay.init("MyAssay")
        let oaHeader = OntologyAnnotation("organism", "OBI", "OBI:0100026")
        let oaValue = OntologyAnnotation("Chlamydomonas rheinhardtii", "NCBITaxon", "NCBITaxon:3055")
        let t1 = ArcTable.init("Table1")
        t1.AddColumn(CompositeHeader.Input IOType.Source, [|CompositeCell.FreeText "MySource"|])
        t1.AddColumn(CompositeHeader.Output IOType.Sample, [|CompositeCell.FreeText "MySample"|])
        let t2 = ArcTable.init("Table2")
        t2.AddColumn(CompositeHeader.Input IOType.Sample, [|CompositeCell.FreeText "MySample"|])
        t2.AddColumn(CompositeHeader.Characteristic oaHeader, [|CompositeCell.Term oaValue|])
        t2.AddColumn(CompositeHeader.Output IOType.Sample, [|CompositeCell.FreeText "MyOutputSample"|])
        a.Tables.Add(t1)
        a.Tables.Add(t2)
        let json = ArcAssay.toISAJsonString(0, useIDReferencing = true) a
        let a' = ArcAssay.fromISAJsonString json
        Expect.equal a' a "Assay"      
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
    ]
let private test_roCrate = testList "ROCrate" [
    // Wait for some issues to be resolved: https://github.com/nfdi4plants/isa-ro-crate-profile/issues
    // Mainly: #12, #9, #10, #13
    ptestCase "Write" <| fun _ ->
        let a = create_filled()
        let json = ArcAssay.toROCrateJsonString() a
        printfn "%s" json
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
    test_roCrate
]