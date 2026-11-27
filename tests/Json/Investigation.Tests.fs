module Tests.Investigation

open ARCtrl
open ARCtrl.Json
open TestingUtils


module Helper =
    let create_empty() = ArcInvestigation.create("My Astonishing Investigation")
    let create_filled() = 
        let assay1 = Assay.Helper.create_filled();
        let assay2 = Assay.Helper.create_empty();
        let study1 = Study.Helper.create_filled();
        let study2 = Study.Helper.create_empty()
        study1.RegisteredAssayIdentifiers <- ResizeArray()
        study2.RegisteredAssayIdentifiers <- ResizeArray()
        let inv =
            ArcInvestigation.make
                "My Inv"
                (Some "My Title")
                (Some "My Description")
                (Some "2024-03-15")
                #if !FABLE_COMPILER
                (Some "2024-04-20T20:20:39")
                #else
                (Some "2024-04-20")
                #endif
                (ResizeArray [OntologySourceReference.create("Description", "path/to/file", "OSR Name")])
                (ResizeArray [Publication.create(doi="any-nice-doi-42")])
                (ResizeArray [Person.create(firstName="Kevin", lastName="Frey")])
                (ResizeArray [])
                (ResizeArray [study1; study2])
                (ResizeArray())
                (ResizeArray())
                (ResizeArray [study1.Identifier; study2.Identifier])
                (ResizeArray [Comment.create("Hello", "World")])
                (ResizeArray())
        inv.AddAssay(assay1, inv.Studies)
        inv.AddAssay(assay2, inv.Studies)
        inv

    let compareFields =
        fun (actual: ArcInvestigation) (expected: ArcInvestigation) ->  
            Expect.equal actual.Identifier expected.Identifier "Identifier"
            Expect.equal actual.Title expected.Title "Title"
            Expect.equal actual.Description expected.Description "Description"
            Expect.equal actual.SubmissionDate expected.SubmissionDate "SubmissionDate"
            Expect.equal actual.PublicReleaseDate expected.PublicReleaseDate "PublicReleaseDate"
            Expect.sequenceEqual actual.OntologySourceReferences expected.OntologySourceReferences "OntologySourceReferences"
            Expect.sequenceEqual actual.Publications expected.Publications "Publications"
            Expect.sequenceEqual actual.Contacts expected.Contacts "Contacts"
            Expect.sequenceEqual actual.Assays expected.Assays "Assays"
            Expect.sequenceEqual actual.Studies expected.Studies "Studies"
            Expect.sequenceEqual actual.RegisteredStudyIdentifiers expected.RegisteredStudyIdentifiers "RegisteredStudyIdentifiers"
            Expect.sequenceEqual actual.Comments expected.Comments "Comments"
            Expect.sequenceEqual actual.Remarks expected.Remarks "Remarks"
        |> Some
    
open Helper

let private test_coreEmpty =
    createBaseJsonTests
        "core-empty"
        create_empty
        ArcInvestigation.toJsonString
        ArcInvestigation.fromJsonString
        None
        compareFields

let private test_compressedEmpty =
    createBaseJsonTests
        "compressed-empty"
        create_empty
        ArcInvestigation.toCompressedJsonString
        ArcInvestigation.fromCompressedJsonString
        None
        compareFields

let private test_isaEmpty =
    createBaseJsonTests
        "isa-empty"
        create_empty
        ArcInvestigation.toISAJsonString
        ArcInvestigation.fromISAJsonString
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateInvestigation)
        #else
        None
        #endif
        compareFields

let private test_core =
    createBaseJsonTests
        "core"
        create_filled
        ArcInvestigation.toJsonString
        ArcInvestigation.fromJsonString
        None
        compareFields

let private test_compressed =

    createBaseJsonTests
        "compressed"
        create_filled
        ArcInvestigation.toCompressedJsonString
        ArcInvestigation.fromCompressedJsonString
        None
        compareFields

let private test_isa =

    createBaseJsonTests
        "isa"
        create_filled
        ArcInvestigation.toISAJsonString
        ArcInvestigation.fromISAJsonString
        #if !FABLE_COMPILER_PYTHON
        (Some Validation.validateInvestigation)
        #else
        None
        #endif
        compareFields

// https://github.com/nfdi4plants/ARCtrl/issues/492
let private test_isaIDTable = testList "isa-UseIDTable" [

    let studyDecoder i decoder = Thoth.Json.Core.Decode.field "studies" (Thoth.Json.Core.Decode.index i decoder)
    let assayDecoder i decoder = Thoth.Json.Core.Decode.field "assays" (Thoth.Json.Core.Decode.index i decoder)

    testCase "SampleAnnotatedInAssay" <| fun _ ->
        let inv = ArcInvestigation.init("Inv")
        let study = ArcStudy.init("Study1")
        study.AddTable (Assay.Helper.create_sampleOutputTable "Growth" "Sample1")
        let assay = ArcAssay.init("Assay1")
        assay.AddTable (Assay.Helper.create_sampleInputTable "Extraction" "Sample1" "organism" "Arabidopsis thaliana")
        inv.AddAssay assay
        inv.AddRegisteredStudy study
        study.RegisterAssay "Assay1"

        let json = ArcInvestigation.toISAJsonString(useIDReferencing = true) inv

        let characteristics = Decode.fromJsonString (studyDecoder 0 (Assay.Helper.isaSampleCharacteristicsDecoder "Sample1")) json
        Expect.sequenceEqual characteristics ["Arabidopsis thaliana"] "Sample should be written with the characteristic it is annotated with in the assay"
        let categories =
            Decode.fromJsonString (studyDecoder 0 (Thoth.Json.Core.Decode.field "characteristicCategories" (Thoth.Json.Core.Decode.list MaterialAttribute.ISAJson.decoder))) json
            |> List.choose (fun c -> c.CharacteristicType |> Option.map (fun t -> t.NameText))
        Expect.sequenceEqual categories ["organism"] "Category of the characteristic should be listed in the study the sample is written in"

    testCase "SampleAnnotatedInTwoAssays" <| fun _ ->
        let inv = ArcInvestigation.init("Inv")
        let study = ArcStudy.init("Study1")
        let assay1 = ArcAssay.init("Assay1")
        assay1.AddTable (Assay.Helper.create_sampleInputTable "Extraction" "Sample1" "organism" "Arabidopsis thaliana")
        let assay2 = ArcAssay.init("Assay2")
        assay2.AddTable (Assay.Helper.create_sampleInputTable "Measurement" "Sample1" "genotype" "Col-0")
        inv.AddAssay assay1
        inv.AddAssay assay2
        inv.AddRegisteredStudy study
        study.RegisterAssay "Assay1"
        study.RegisterAssay "Assay2"

        let json = ArcInvestigation.toISAJsonString(useIDReferencing = true) inv

        let characteristics = Decode.fromJsonString (studyDecoder 0 (assayDecoder 0 (Assay.Helper.isaSampleCharacteristicsDecoder "Sample1"))) json
        Expect.sequenceEqual characteristics ["Arabidopsis thaliana"; "Col-0"] "Sample should be written with the characteristics it is annotated with in both assays"

    testCase "SampleAnnotatedInSecondStudy" <| fun _ ->
        let inv = ArcInvestigation.init("Inv")
        let study1 = ArcStudy.init("Study1")
        study1.AddTable (Assay.Helper.create_sampleOutputTable "Growth" "Sample1")
        let study2 = ArcStudy.init("Study2")
        study2.AddTable (Assay.Helper.create_sampleInputTable "Extraction" "Sample1" "organism" "Arabidopsis thaliana")
        inv.AddRegisteredStudy study1
        inv.AddRegisteredStudy study2

        let json = ArcInvestigation.toISAJsonString(useIDReferencing = true) inv

        let characteristics = Decode.fromJsonString (studyDecoder 0 (Assay.Helper.isaSampleCharacteristicsDecoder "Sample1")) json
        Expect.sequenceEqual characteristics ["Arabidopsis thaliana"] "Sample should be written with the characteristic it is annotated with in the second study"
]

let main = testList "Investigation" [
    test_coreEmpty
    test_core
    test_compressedEmpty
    test_compressed
    test_isaEmpty
    test_isa
    test_isaIDTable
]