module ArcStudy.Tests

open ARCtrl

open TestingUtils

let createExampleAssays() = ResizeArray([|ArcAssay.init("Assay 1")|])

let getAssayIdentifiers(assays: ResizeArray<ArcAssay>) = assays |> Seq.map (fun a -> a.Identifier) |> ResizeArray

let private test_create =
    testList "create" [
        testCase "constructor" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Study Title"
            let description = "Study Description"
            let submissionDate = "2023-07-19"
            let publicReleaseDate = "2023-12-31"
            let publications = ResizeArray [|Publication.create("Publication 1")|]
            let contacts = ResizeArray [|Person.create(firstName = "John", lastName = "Doe")|]
            let studyDesignDescriptors = ResizeArray [|OntologyAnnotation("Design Descriptor")|]
            let tables = ResizeArray([|ArcTable.init("Table 1")|])
            let assays = createExampleAssays()
            let assay_identifiers = getAssayIdentifiers assays
            let comments = ResizeArray [|Comment.create("Comment 1")|]

            let actual =
                ArcStudy(
                    identifier = identifier,
                    title = title,
                    description = description,
                    submissionDate = submissionDate,
                    publicReleaseDate = publicReleaseDate,
                    publications = publications,
                    contacts = contacts,
                    studyDesignDescriptors = studyDesignDescriptors,
                    tables = tables,
                    registeredAssayIdentifiers = assay_identifiers,
                    comments = comments
                )

            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title (Some title) "Title"
            Expect.equal actual.Description (Some description) "Description"
            Expect.equal actual.SubmissionDate (Some submissionDate) "SubmissionDate"
            Expect.equal actual.PublicReleaseDate (Some publicReleaseDate) "PublicReleaseDate"
            Expect.equal actual.Publications publications "Publications"
            Expect.equal actual.Contacts contacts "Contacts"
            Expect.equal actual.StudyDesignDescriptors studyDesignDescriptors "StudyDesignDescriptors"
            Expect.equal actual.Tables tables "Tables"
            Expect.equal actual.RegisteredAssayIdentifiers assay_identifiers "Assays"
            Expect.equal actual.Comments comments "Comments"

        testCase "create" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Study Title"
            let description = "Study Description"
            let submissionDate = "2023-07-19"
            let publicReleaseDate = "2023-12-31"
            let publications = ResizeArray [|Publication.create("Publication 1")|]
            let contacts = ResizeArray [|Person.create(firstName = "John", lastName = "Doe")|]
            let studyDesignDescriptors = ResizeArray [|OntologyAnnotation("Design Descriptor")|]
            let tables = ResizeArray [|ArcTable.init("Table 1")|]
            let assays = createExampleAssays()
            let assay_identifiers = getAssayIdentifiers assays
            let comments = ResizeArray [|Comment.create("Comment 1")|]

            let actual = ArcStudy.create(
                identifier = identifier,
                title = title,
                description = description,
                submissionDate = submissionDate,
                publicReleaseDate = publicReleaseDate,
                publications = publications,
                contacts = contacts,
                studyDesignDescriptors = studyDesignDescriptors,
                tables = tables,
                registeredAssayIdentifiers = assay_identifiers,
                comments = comments
            )

            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title (Some title) "Title"
            Expect.equal actual.Description (Some description) "Description"
            Expect.equal actual.SubmissionDate (Some submissionDate) "SubmissionDate"
            Expect.equal actual.PublicReleaseDate (Some publicReleaseDate) "PublicReleaseDate"
            Expect.equal actual.Publications publications "Publications"
            Expect.equal actual.Contacts contacts "Contacts"
            Expect.equal actual.StudyDesignDescriptors studyDesignDescriptors "StudyDesignDescriptors"
            Expect.equal actual.Tables tables "Tables"
            Expect.equal actual.RegisteredAssayIdentifiers assay_identifiers "Assays"
            Expect.equal actual.Comments comments "Comments"

        testCase "init" <| fun _ ->
            let identifier = "MyIdentifier"
            let actual = ArcStudy.init(identifier)
            Expect.equal actual.Identifier identifier "identifier"
            Expect.equal actual.Title None "Title"
            Expect.equal actual.Description None "Description"
            Expect.equal actual.SubmissionDate None "SubmissionDate"
            Expect.equal actual.PublicReleaseDate None "PublicReleaseDate"
            Expect.isEmpty actual.Publications "Publications"
            Expect.isEmpty actual.Contacts "Contacts"
            Expect.isEmpty actual.StudyDesignDescriptors "StudyDesignDescriptors"
            Expect.isEmpty actual.Tables "Tables"
            Expect.isEmpty actual.RegisteredAssayIdentifiers "Assays"
            Expect.isEmpty actual.Comments "Comments"
        testCase "make" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = Some "Study Title"
            let description = Some "Study Description"
            let submissionDate = Some "2023-07-19"
            let publicReleaseDate = Some "2023-12-31"
            let publications = ResizeArray [|Publication.create("Publication 1")|]
            let contacts = ResizeArray [|Person.create(firstName = "John", lastName = "Doe")|]
            let studyDesignDescriptors = ResizeArray [|OntologyAnnotation("Design Descriptor")|]
            let tables = ResizeArray([|ArcTable.init("Table 1")|])
            let assays = createExampleAssays()
            let assay_identifiers = getAssayIdentifiers assays
            let comments = ResizeArray [|Comment.create("Comment 1")|]

            let actual = 
                ArcStudy.make
                    identifier
                    title
                    description
                    submissionDate
                    publicReleaseDate
                    publications
                    contacts
                    studyDesignDescriptors
                    tables
                    None
                    assay_identifiers
                    comments

            Expect.equal actual.Identifier identifier "Identifier"
            Expect.equal actual.Title title "Title"
            Expect.equal actual.Description description "Description"
            Expect.equal actual.SubmissionDate submissionDate "SubmissionDate"
            Expect.equal actual.PublicReleaseDate publicReleaseDate "PublicReleaseDate"
            Expect.equal actual.Publications publications "Publications"
            Expect.equal actual.Contacts contacts "Contacts"
            Expect.equal actual.StudyDesignDescriptors studyDesignDescriptors "StudyDesignDescriptors"
            Expect.equal actual.Tables tables "Tables"
            Expect.equal actual.RegisteredAssayIdentifiers assay_identifiers "Assays"
            Expect.equal actual.Comments comments "Comments"
        testCase "failsForInvalidCharacters" <| fun _ ->
            let createStudy =
                fun () -> ArcStudy("My%Study") |> ignore
            Expect.throws createStudy "throws, invalid characters"
        testCase "whiteSpaceTrimmed" <| fun _ ->
            let study = ArcStudy(" MyStudy ")
            Expect.equal study.Identifier "MyStudy" "Identifier"
    ]

let tests_RegisteredAssays = testList "RegisteredAssays" [
    let _study_identifier = "My Study"
    let _study_description = Some "Lorem Ipsum"
    let _assay_identifier = "My Assay"
    let createTestStudy() =
        let s = ArcStudy(_study_identifier)
        s.Description <- _study_description
        s
    let createTestAssay() =
        ArcAssay.init(_assay_identifier)
    testList "no parent" [
        testCase "RegisterAssay" <| fun _ ->
            let study = createTestStudy()
            study.RegisterAssay(_assay_identifier)
            Expect.equal study.RegisteredAssayIdentifierCount 1 "registered assay count"
        testCase "GetRegisteredAssay" <| fun _ ->
            let study = createTestStudy()
            study.RegisterAssay(_assay_identifier)
            let eval() = study.GetRegisteredAssay(_assay_identifier) |> ignore
            Expect.throws eval "throws as single study has no parent, therefore no access to full assays."
        testCase "GetRegisteredAssays" <| fun _ ->
            let study = createTestStudy()
            study.RegisterAssay(_assay_identifier)
            let eval() = study.RegisteredAssays |> ignore
            Expect.throws eval "throws as single study has no parent, therefore no access to full assays."
        testCase "DeregisterAssay" <| fun _ ->
            let study = createTestStudy()
            study.RegisterAssay(_assay_identifier)
            Expect.equal study.RegisteredAssayIdentifierCount 1 "registered assay count"
            study.DeregisterAssay(_assay_identifier)
            Expect.equal study.RegisteredAssayIdentifierCount 0 "registered assay count 2"
        testCase "InitAssay" <| fun _ ->
            let study = createTestStudy()
            let eval() = study.InitRegisteredAssay(_assay_identifier) |> ignore
            Expect.throws eval "throws as single study has no parent, therefore no access to full assays."
        testCase "AddAssay" <| fun _ ->
            let study = createTestStudy()
            let eval() = study.AddRegisteredAssay(ArcAssay(_assay_identifier)) |> ignore
            Expect.throws eval "throws as single study has no parent, therefore no access to full assays."
    ]
    testList "with parent" [
        testCase "RegisterAssay" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let assay = createTestAssay()
            let study = createTestStudy()
            i.AddStudy(study)
            i.AddAssay(assay)
            study.RegisterAssay(_assay_identifier)
            Expect.equal i.AssayCount 1 "AssayCount"
            Expect.equal i.StudyCount 1 "StudyCount"
            Expect.equal study.RegisteredAssayIdentifierCount 1 "registered assay count"
        testCase "GetRegisteredAssay" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let assay = createTestAssay()
            let study = createTestStudy()
            i.AddStudy(study)
            i.AddAssay(assay)
            study.RegisterAssay(_assay_identifier)
            let actual = study.GetRegisteredAssay(_assay_identifier)
            Expect.equal actual assay "equal"
        testCase "GetRegisteredAssays" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let assay = createTestAssay()
            let study = createTestStudy()
            i.AddStudy(study)
            i.AddAssay(assay)
            study.RegisterAssay(_assay_identifier)
            let actual = study.RegisteredAssays
            Expect.equal actual.[0] assay "equal"
        testCase "DeregisterAssay" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let assay = createTestAssay()
            let study = createTestStudy()
            i.AddStudy(study)
            i.AddAssay(assay)
            study.RegisterAssay(_assay_identifier)
            study.DeregisterAssay(_assay_identifier)
            Expect.equal study.RegisteredAssayIdentifierCount 0 "registered assay count 2"
            Expect.equal i.AssayCount 1 "AssayCount, only deregister from study, not remove"
            Expect.equal i.StudyCount 1 "StudyCount"
        testCase "InitAssay" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let study = createTestStudy()
            i.AddStudy(study)
            let assay = study.InitRegisteredAssay(_assay_identifier)
            Expect.equal i.AssayCount 1 "AssayCount"
            Expect.equal i.StudyCount 1 "StudyCount"
            Expect.equal study.RegisteredAssayIdentifierCount 1 "registered assay count"
            Expect.equal i.Assays.[0] assay "equal"
        testCase "AddAssay" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let study = createTestStudy()
            i.AddStudy(study)
            let assay = ArcAssay.init(_assay_identifier)
            study.AddRegisteredAssay(assay)
            Expect.equal i.AssayCount 1 "AssayCount"
            Expect.equal i.StudyCount 1 "StudyCount"
            Expect.equal study.RegisteredAssayIdentifierCount 1 "registered assay count"
            Expect.equal i.Assays.[0] assay "equal"
        testCase "Check mutability" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let study = createTestStudy()
            i.AddStudy(study)
            let assay = ArcAssay.init(_assay_identifier)
            study.AddRegisteredAssay(assay)
            let assayFromInv = i.Assays.[0]
            let table = assayFromInv.InitTable("MyNewTable")
            Expect.equal assayFromInv assay "equal"
            Expect.equal assayFromInv.TableCount 1 "assayFromInv.TableCount"
            Expect.equal assay.TableCount 1 "assay.TableCount"
    ]
]

let tests_copy = 
    testList "copy" [
        let _study_identifier = "My Study"
        let _study_description = Some "Lorem Ipsum"
        let _assay_identifier = "My Assay"
        let createTestStudy() =
            let s = ArcStudy(_study_identifier)
            s.Description <- _study_description
            s.RegisterAssay(_assay_identifier)
            s
        testCase "ensure test study" <| fun _ -> 
            let study = createTestStudy()
            Expect.equal study.Identifier _study_identifier "_study_identifier"
            Expect.equal study.Description _study_description "_study_description"
            Expect.equal study.RegisteredAssayIdentifierCount 1 "AssayCount"
            let assayIdentifier = study.RegisteredAssayIdentifiers.[0]
            Expect.equal assayIdentifier _assay_identifier "_assay_identifier"
        testCase "test mutable fields" <| fun _ -> 
            let newDesciption = Some "New Description"
            let newPublicReleaseDate = Some "01.01.2000"
            let study = createTestStudy()
            let copy = study.Copy()
            copy.Description <- newDesciption
            copy.PublicReleaseDate <- newPublicReleaseDate
            let checkSourceStudy =
                Expect.equal study.Identifier _study_identifier "_study_identifier"
                Expect.equal study.Description _study_description "_study_description"
                Expect.equal study.PublicReleaseDate None "PublicReleaseDate"
                Expect.equal study.RegisteredAssayIdentifierCount 1 "AssayCount"
                let assayIdentifier = study.RegisteredAssayIdentifiers.[0]
                Expect.equal assayIdentifier _assay_identifier "_assay_identifier"
            let checkCopy =
                Expect.equal copy.Identifier _study_identifier "copy _study_identifier"
                Expect.equal copy.Description newDesciption "copy _study_description"
                Expect.equal copy.PublicReleaseDate newPublicReleaseDate "copy PublicReleaseDate"
                Expect.equal copy.RegisteredAssayIdentifierCount 1 "copy AssayCount"
                let assayIdentifier = study.RegisteredAssayIdentifiers.[0]
                Expect.equal assayIdentifier _assay_identifier "copy _assay_identifier"
            ()
    ]

let tests_UpdateBy = testList "UpdateReferenceByStudyFile" [

    let protocolREF = "MyProtocol"
    let protocolDescription = "MyProtocolDescription"
    
    let createFullStudy() =
        let identifier = "MyIdentifier"
        let title = "Study Title"
        let description = "Study Description"
        let submissionDate = "2023-07-19"
        let publicReleaseDate = "2023-12-31"
        let publications = ResizeArray [|Publication.create("Publication 1")|]
        let contacts = ResizeArray [|Person.create(firstName = "John", lastName = "Doe")|]
        let studyDesignDescriptors = ResizeArray [|OntologyAnnotation("Design Descriptor")|]
        let tables = 
            let refTable = ArcTable.init(protocolREF)
            refTable.AddProtocolNameColumn (ResizeArray [|protocolREF|])
            refTable.AddProtocolDescriptionColumn (ResizeArray [|protocolDescription|])    
            ResizeArray([|refTable|])
        let assays = createExampleAssays()
        let assay_identifiers = getAssayIdentifiers assays
        let comments = ResizeArray [|Comment.create("Comment 1")|]
        ArcStudy(
            identifier = identifier,
            title = title,
            description = description,
            submissionDate = submissionDate,
            publicReleaseDate = publicReleaseDate,
            publications = publications,
            contacts = contacts,
            studyDesignDescriptors = studyDesignDescriptors,
            tables = tables,
            registeredAssayIdentifiers = assay_identifiers,
            comments = comments
        )
    testCase "tablesAreUpdated" <| fun _ ->
        let tableOfInterest = ArcTables.Tests.TestObjects.sheetWithTwoProtocolsTwoRefs()
        let tables = ArcTables.ofSeq [tableOfInterest]
        let refTables = ArcTables.ofSeq [ArcTables.Tests.TestObjects.descriptionRefTable();ArcTables.Tests.TestObjects.descriptionRefTable2()]
        let actual = 
            ArcStudy(
                "ReferenceStudy",
                tables = refTables.Tables
            )
        let next = 
            ArcStudy(
                identifier = "Next_identifier",
                tables = tables.Tables
            )
        actual.UpdateReferenceByStudyFile(next)
        Expect.notEqual actual next "not equal"
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        
        let result = actual.Tables
        Expect.equal result.Count tables.TableCount "Should be same number of tables"
        let resultTable = result.[0]
        Expect.equal resultTable.Name tableOfInterest.Name "Should be same table name"
        Expect.equal resultTable.ColumnCount (tableOfInterest.ColumnCount + 1) "Should be same number of columns"
        Expect.equal resultTable.RowCount tableOfInterest.RowCount "Should be same number of rows"

        let expectedDescription =              
            Array.create 2 (CompositeCell.createFreeText ArcTables.Tests.TestObjects.descriptionValue2)
            |> Array.append (Array.create 2 (CompositeCell.createFreeText ArcTables.Tests.TestObjects.descriptionValue1))
        TestingUtils.Expect.sequenceEqual
            (resultTable.GetColumnByHeader CompositeHeader.ProtocolDescription).Cells
            (expectedDescription)
            "Description value was not taken correctly"
        TestingUtils.Expect.sequenceEqual
            (resultTable.GetColumnByHeader (CompositeHeader.Parameter ArcTables.Tests.TestObjects.oa_species)).Cells
            (Array.create 4 (CompositeCell.createTerm ArcTables.Tests.TestObjects.oa_chlamy))
            "Check for previous param correctness"

    testCase "replace existing, none replaced" <| fun _ ->
        let actual = createFullStudy()
        let next = ArcStudy.init("NextIdentifier")
        let expected = createFullStudy()
        actual.UpdateReferenceByStudyFile(next, true)
        Expect.notEqual actual next "not equal"
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        Expect.equal actual.Title expected.Title "Title"
        Expect.equal actual.Description expected.Description "Description"
        Expect.equal actual.SubmissionDate expected.SubmissionDate "SubmissionDate"
        Expect.equal actual.PublicReleaseDate expected.PublicReleaseDate "PublicReleaseDate"
        Expect.sequenceEqual actual.Publications expected.Publications "Publications"
        Expect.sequenceEqual actual.Contacts expected.Contacts "Contacts"
        Expect.sequenceEqual actual.StudyDesignDescriptors expected.StudyDesignDescriptors "StudyDesignDescriptors"
        Expect.sequenceEqual actual.Tables expected.Tables "Tables" 
        Expect.sequenceEqual actual.RegisteredAssayIdentifiers expected.RegisteredAssayIdentifiers "RegisteredAssayIdentifiers"
        Expect.sequenceEqual actual.Comments expected.Comments "Comments"
    testCase "replace existing, all replaced" <| fun _ ->
        let actual = createFullStudy()
        let next = 
            ArcStudy(
                identifier = "Next_identifier",
                title = "Next_Title",
                description = "Description",
                submissionDate = "Next_SubmissionDate",
                publicReleaseDate = "Next_PublicReleaseDate",
                publications = ResizeArray [|Publication(title="My Next Title")|],
                contacts = ResizeArray [|Person(firstName="NextKevin", lastName="NextFrey")|],
                studyDesignDescriptors = ResizeArray [|OntologyAnnotation "Next OA"|],
                tables = ResizeArray([ArcTable.init("NextTable")]),
                registeredAssayIdentifiers = ResizeArray(["NextIdentifier"]),
                comments = ResizeArray [|Comment(name="NextCommentName", value="NextCommentValue")|]
            )
        actual.UpdateReferenceByStudyFile(next, true)
        Expect.notEqual actual next "not equal"
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        Expect.equal actual.Title next.Title "Title"
        Expect.equal actual.Description next.Description "Description"
        Expect.equal actual.SubmissionDate next.SubmissionDate "SubmissionDate"
        Expect.equal actual.PublicReleaseDate next.PublicReleaseDate "PublicReleaseDate"
        Expect.sequenceEqual actual.Publications next.Publications "Publications"
        Expect.sequenceEqual actual.Contacts next.Contacts "Contacts"
        Expect.sequenceEqual actual.StudyDesignDescriptors next.StudyDesignDescriptors "StudyDesignDescriptors"
        Expect.sequenceEqual actual.Tables next.Tables "Tables" 
        Expect.sequenceEqual actual.RegisteredAssayIdentifiers next.RegisteredAssayIdentifiers "RegisteredAssayIdentifiers"
        Expect.sequenceEqual actual.Comments next.Comments "Comments"
    testCase "full replace, empty" <| fun _ ->
        let actual = createFullStudy()
        let next = 
            ArcStudy(
                identifier = "Next_identifier"               
            )
        let original = createFullStudy()
        actual.UpdateReferenceByStudyFile(next, false)
        Expect.notEqual actual next "not equal"
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        Expect.isNone actual.Title "Title"
        Expect.isNone actual.Description  "Description"
        Expect.isNone actual.SubmissionDate "SubmissionDate"
        Expect.isNone actual.PublicReleaseDate "PublicReleaseDate"
        Expect.isEmpty actual.Publications "Publications"
        Expect.isEmpty actual.Contacts "Contacts"
        Expect.isEmpty actual.StudyDesignDescriptors "StudyDesignDescriptors"
        Expect.isEmpty actual.Tables "Tables" 
        Expect.isEmpty actual.RegisteredAssayIdentifiers "RegisteredAssayIdentifiers"
        Expect.isEmpty actual.Comments "Comments"
]

let private tests_GetHashCode = testList "GetHashCode" [
    let createFullStudy(name) =
        ArcStudy.make 
            name
            (Some "My Study Title")
            (Some "My Study Description")
            (Some "My Study SubmissionDate")
            (Some "My Study PRD")
            (ResizeArray [|Publication(); Publication.create(title="Some nice title")|])
            (ResizeArray [|Person(firstName="John",lastName="Doe"); Person(firstName="Jane",lastName="Doe")|])
            (ResizeArray [|OntologyAnnotation(); OntologyAnnotation(); OntologyAnnotation("Name", "tsr", "Tan")|])
            (ResizeArray([ArcTable.init("My Table"); ArcTable.Tests.create_testTable()]))
            None
            (ResizeArray(["Registered Assay1"; "Registered Assay2"]))
            (ResizeArray [|Comment("Hello", "World"); Comment("ByeBye", "World") |])
    testCase "passing" <| fun _ ->
        let actual = ArcAssay.create("MyAssay", tables= ResizeArray([ArcTable.init("My Table")]))
        Expect.isSome (actual.GetHashCode() |> Some) ""
    testCase "equal minimal" <| fun _ -> 
        let actual = ArcAssay.init("MyAssay")
        let copy = actual.Copy()
        let actual2 = ArcAssay.init("MyAssay")
        Expect.equal actual copy "equal"
        Expect.equal (actual.GetHashCode()) (copy.GetHashCode()) "copy hash equal"
        Expect.equal (actual.GetHashCode()) (actual2.GetHashCode()) "assay2 hash equal"
    testCase "equal" <| fun _ ->
        let actual = createFullStudy "MyStudy"
        let copy = actual.Copy()
        Expect.equal (actual.GetHashCode()) (copy.GetHashCode()) ""
    testCase "not equal" <| fun _ ->
        let x1 = ArcStudy.init "Test"
        let x2 = ArcStudy.init "Other Test"
        Expect.notEqual x1 x2 "not equal"
        Expect.notEqual (x1.GetHashCode()) (x2.GetHashCode()) "not equal hash"
]

let main = 
    testList "ArcStudy" [
        tests_copy
        tests_RegisteredAssays
        test_create
        tests_UpdateBy
        tests_GetHashCode
    ]



