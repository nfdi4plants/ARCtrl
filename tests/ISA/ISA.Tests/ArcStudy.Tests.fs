module ArcStudy.Tests

open ARCtrl.ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

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
            let publications = [|Publication.create("Publication 1")|]
            let contacts = [|Person.create(FirstName = "John", LastName = "Doe")|]
            let studyDesignDescriptors = [|OntologyAnnotation.fromString("Design Descriptor")|]
            let tables = ResizeArray([|ArcTable.init("Table 1")|])
            let assays = createExampleAssays()
            let assay_identifiers = getAssayIdentifiers assays
            let factors = [|Factor.create("Factor 1")|]
            let comments = [|Comment.create("Comment 1")|]

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
                    factors = factors,
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
            Expect.equal actual.Factors factors "Factors"
            Expect.equal actual.Comments comments "Comments"

        testCase "create" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = "Study Title"
            let description = "Study Description"
            let submissionDate = "2023-07-19"
            let publicReleaseDate = "2023-12-31"
            let publications = [|Publication.create("Publication 1")|]
            let contacts = [|Person.create(FirstName = "John", LastName = "Doe")|]
            let studyDesignDescriptors = [|OntologyAnnotation.fromString("Design Descriptor")|]
            let tables = ResizeArray([|ArcTable.init("Table 1")|])
            let assays = createExampleAssays()
            let assay_identifiers = getAssayIdentifiers assays
            let factors = [|Factor.create("Factor 1")|]
            let comments = [|Comment.create("Comment 1")|]

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
                factors = factors,
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
            Expect.equal actual.Factors factors "Factors"
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
            Expect.isEmpty actual.Factors "Factors"
            Expect.isEmpty actual.Comments "Comments"
        testCase "make" <| fun _ ->
            let identifier = "MyIdentifier"
            let title = Some "Study Title"
            let description = Some "Study Description"
            let submissionDate = Some "2023-07-19"
            let publicReleaseDate = Some "2023-12-31"
            let publications = [|Publication.create("Publication 1")|]
            let contacts = [|Person.create(FirstName = "John", LastName = "Doe")|]
            let studyDesignDescriptors = [|OntologyAnnotation.fromString("Design Descriptor")|]
            let tables = ResizeArray([|ArcTable.init("Table 1")|])
            let assays = createExampleAssays()
            let assay_identifiers = getAssayIdentifiers assays
            let factors = [|Factor.create("Factor 1")|]
            let comments = [|Comment.create("Comment 1")|]

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
                    assay_identifiers
                    factors
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
            Expect.equal actual.Factors factors "Factors"
            Expect.equal actual.Comments comments "Comments"
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
            Expect.equal study.RegisteredAssayCount 1 "registered assay count"
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
            Expect.equal study.RegisteredAssayCount 1 "registered assay count"
            study.DeregisterAssay(_assay_identifier)
            Expect.equal study.RegisteredAssayCount 0 "registered assay count 2"
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
            Expect.equal study.RegisteredAssayCount 1 "registered assay count"
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
            Expect.equal study.RegisteredAssayCount 0 "registered assay count 2"
            Expect.equal i.AssayCount 1 "AssayCount, only deregister from study, not remove"
            Expect.equal i.StudyCount 1 "StudyCount"
        testCase "InitAssay" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let study = createTestStudy()
            i.AddStudy(study)
            let assay = study.InitRegisteredAssay(_assay_identifier)
            Expect.equal i.AssayCount 1 "AssayCount"
            Expect.equal i.StudyCount 1 "StudyCount"
            Expect.equal study.RegisteredAssayCount 1 "registered assay count"
            Expect.equal i.Assays.[0] assay "equal"
        testCase "AddAssay" <| fun _ ->
            let i = ArcInvestigation.init("MyInvestigation")
            let study = createTestStudy()
            i.AddStudy(study)
            let assay = ArcAssay.init(_assay_identifier)
            study.AddRegisteredAssay(assay)
            Expect.equal i.AssayCount 1 "AssayCount"
            Expect.equal i.StudyCount 1 "StudyCount"
            Expect.equal study.RegisteredAssayCount 1 "registered assay count"
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
            Expect.equal study.RegisteredAssayCount 1 "AssayCount"
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
                Expect.equal study.RegisteredAssayCount 1 "AssayCount"
                let assayIdentifier = study.RegisteredAssayIdentifiers.[0]
                Expect.equal assayIdentifier _assay_identifier "_assay_identifier"
            let checkCopy =
                Expect.equal copy.Identifier _study_identifier "copy _study_identifier"
                Expect.equal copy.Description newDesciption "copy _study_description"
                Expect.equal copy.PublicReleaseDate newPublicReleaseDate "copy PublicReleaseDate"
                Expect.equal copy.RegisteredAssayCount 1 "copy AssayCount"
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
        let publications = [|Publication.create("Publication 1")|]
        let contacts = [|Person.create(FirstName = "John", LastName = "Doe")|]
        let studyDesignDescriptors = [|OntologyAnnotation.fromString("Design Descriptor")|]
        let tables = 
            let refTable = ArcTable.init(protocolREF)
            refTable.AddProtocolNameColumn [|protocolREF|]
            refTable.AddProtocolDescriptionColumn [|protocolDescription|]        
            ResizeArray([|refTable|])
        let assays = createExampleAssays()
        let assay_identifiers = getAssayIdentifiers assays
        let factors = [|Factor.create("Factor 1")|]
        let comments = [|Comment.create("Comment 1")|]
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
            factors = factors,
            comments = comments
        )
    testCase "full replace, no tables" <| fun _ ->
        let actual = createFullStudy()
        let next = 
            ArcStudy(
                identifier = "Next_identifier",
                title = "Next_Title",
                description = "Description",
                submissionDate = "Next_SubmissionDate",
                publicReleaseDate = "Next_PublicReleaseDate",
                publications = [||],
                contacts = [||],
                studyDesignDescriptors = [||],
                tables = ResizeArray(),
                registeredAssayIdentifiers = ResizeArray(),
                factors = [||],
                comments = [||]
            )
        actual.UpdateReferenceByStudyFile(next)
        Expect.notEqual actual next "not equal"
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        Expect.equal actual.Title next.Title "Title"
        Expect.equal actual.Description next.Description "Description"
        Expect.equal actual.SubmissionDate next.SubmissionDate "SubmissionDate"
        Expect.equal actual.PublicReleaseDate next.PublicReleaseDate "PublicReleaseDate"
        Expect.equal actual.Publications next.Publications "Publications"
        Expect.equal actual.Contacts next.Contacts "Contacts"
        Expect.equal actual.StudyDesignDescriptors next.StudyDesignDescriptors "StudyDesignDescriptors"
        Expect.equal actual.Tables.Count 1 "Tables.Count = 0" // Ok
        Expect.equal actual.RegisteredAssayIdentifiers next.RegisteredAssayIdentifiers "RegisteredAssayIdentifiers"
        Expect.equal actual.Factors next.Factors "Factors"
        Expect.equal actual.Comments next.Comments "Comments"
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
        Expect.equal actual.Publications expected.Publications "Publications"
        Expect.equal actual.Contacts expected.Contacts "Contacts"
        Expect.equal actual.StudyDesignDescriptors expected.StudyDesignDescriptors "StudyDesignDescriptors"
        TestingUtils.mySequenceEqual actual.Tables expected.Tables "Tables" 
        TestingUtils.mySequenceEqual actual.RegisteredAssayIdentifiers expected.RegisteredAssayIdentifiers "RegisteredAssayIdentifiers"
        Expect.equal actual.Factors expected.Factors "Factors"
        Expect.equal actual.Comments expected.Comments "Comments"
    testCase "replace existing, all replaced" <| fun _ ->
        let actual = createFullStudy()
        let next = 
            ArcStudy(
                identifier = "Next_identifier",
                title = "Next_Title",
                description = "Description",
                submissionDate = "Next_SubmissionDate",
                publicReleaseDate = "Next_PublicReleaseDate",
                publications = [|Publication.create(Title="My Next Title")|],
                contacts = [|Person.create(FirstName="NextKevin", LastName="NextFrey")|],
                studyDesignDescriptors = [|OntologyAnnotation.fromString "Next OA"|],
                tables = ResizeArray([ArcTable.init("NextTable")]),
                registeredAssayIdentifiers = ResizeArray(["NextIdentifier"]),
                factors = [|Factor.create(Name="NextFactor")|],
                comments = [|Comment.create(Name="NextCommentName", Value="NextCommentValue")|]
            )
        actual.UpdateReferenceByStudyFile(next, true)
        Expect.notEqual actual next "not equal"
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        Expect.equal actual.Title next.Title "Title"
        Expect.equal actual.Description next.Description "Description"
        Expect.equal actual.SubmissionDate next.SubmissionDate "SubmissionDate"
        Expect.equal actual.PublicReleaseDate next.PublicReleaseDate "PublicReleaseDate"
        Expect.equal actual.Publications next.Publications "Publications"
        Expect.equal actual.Contacts next.Contacts "Contacts"
        Expect.equal actual.StudyDesignDescriptors next.StudyDesignDescriptors "StudyDesignDescriptors"
        TestingUtils.mySequenceEqual actual.Tables next.Tables "Tables" 
        TestingUtils.mySequenceEqual actual.RegisteredAssayIdentifiers next.RegisteredAssayIdentifiers "RegisteredAssayIdentifiers"
        Expect.equal actual.Factors next.Factors "Factors"
        Expect.equal actual.Comments next.Comments "Comments"
    testCase "replace existing, append" <| fun _ ->
        let actual = createFullStudy()
        let next = 
            ArcStudy(
                identifier = "Next_identifier",
                title = "Next_Title",
                description = "Description",
                submissionDate = "Next_SubmissionDate",
                publicReleaseDate = "Next_PublicReleaseDate",
                publications = [|Publication.create(Title="My Next Title")|],
                contacts = [|Person.create(FirstName="NextKevin", LastName="NextFrey")|],
                studyDesignDescriptors = [|OntologyAnnotation.fromString "Next OA"|],
                tables = ResizeArray([ArcTable.init("NextTable")])
            )
        let original = createFullStudy()
        actual.UpdateReferenceByStudyFile(next, true)
        Expect.notEqual actual next "not equal"
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        Expect.equal actual.Title next.Title "Title"
        Expect.equal actual.Description next.Description "Description"
        Expect.equal actual.SubmissionDate next.SubmissionDate "SubmissionDate"
        Expect.equal actual.PublicReleaseDate next.PublicReleaseDate "PublicReleaseDate"
        Expect.equal actual.Publications [|Publication.create("Publication 1"); Publication.create(Title="My Next Title")|] "Publications"
        Expect.equal actual.Contacts [|Person.create(FirstName = "John", LastName = "Doe"); Person.create(FirstName="NextKevin", LastName="NextFrey")|] "Contacts"
        Expect.equal actual.StudyDesignDescriptors [|OntologyAnnotation.fromString("Design Descriptor"); OntologyAnnotation.fromString "Next OA"|] "StudyDesignDescriptors"
        TestingUtils.mySequenceEqual actual.Tables (ResizeArray([ArcTable.init("Table 1"); ArcTable.init("NextTable")])) "Tables" 
        TestingUtils.mySequenceEqual actual.RegisteredAssayIdentifiers original.RegisteredAssayIdentifiers "RegisteredAssayIdentifiers"
        Expect.equal actual.Factors original.Factors "Factors"
        Expect.equal actual.Comments original.Comments "Comments"
    testCase "full replace, append" <| fun _ ->
        let actual = createFullStudy()
        let next = 
            ArcStudy(
                identifier = "Next_identifier",
                title = "Next_Title",
                description = "Description",
                submissionDate = "Next_SubmissionDate",
                publicReleaseDate = "Next_PublicReleaseDate",
                publications = [|Publication.create(Title="My Next Title")|],
                contacts = [|Person.create(FirstName="NextKevin", LastName="NextFrey")|],
                studyDesignDescriptors = [|OntologyAnnotation.fromString "Next OA"|],
                tables = ResizeArray([ArcTable.init("NextTable")])
            )
        let original = createFullStudy()
        actual.UpdateReferenceByStudyFile(next, false)
        Expect.notEqual actual next "not equal"
        Expect.notEqual actual.Identifier next.Identifier "Identifier"
        Expect.equal actual.Title next.Title "Title"
        Expect.equal actual.Description next.Description "Description"
        Expect.equal actual.SubmissionDate next.SubmissionDate "SubmissionDate"
        Expect.equal actual.PublicReleaseDate next.PublicReleaseDate "PublicReleaseDate"
        Expect.equal actual.Publications [|Publication.create("Publication 1"); Publication.create(Title="My Next Title")|] "Publications"
        Expect.equal actual.Contacts [|Person.create(FirstName = "John", LastName = "Doe"); Person.create(FirstName="NextKevin", LastName="NextFrey")|] "Contacts"
        Expect.equal actual.StudyDesignDescriptors [|OntologyAnnotation.fromString("Design Descriptor"); OntologyAnnotation.fromString "Next OA"|] "StudyDesignDescriptors"
        TestingUtils.mySequenceEqual actual.Tables (ResizeArray([ArcTable.init("Table 1"); ArcTable.init("NextTable")])) "Tables" 
        TestingUtils.mySequenceEqual actual.RegisteredAssayIdentifiers original.RegisteredAssayIdentifiers "RegisteredAssayIdentifiers"
        Expect.equal actual.Factors original.Factors "Factors"
        Expect.equal actual.Comments original.Comments "Comments"
]

let main = 
    testList "ArcStudy" [
        tests_copy
        tests_RegisteredAssays
        test_create
        tests_UpdateBy
    ]



