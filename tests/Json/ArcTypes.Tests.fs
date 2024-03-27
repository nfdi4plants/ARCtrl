module Tests.ArcTypes

open ARCtrl
open ARCtrl.Json
open TestingUtils

    //testCase "wöjsda" <| fun _ ->
    //    let obj = create_filled()
    //    let json = ArcTable.toCompressedJsonString () obj
    //    printfn "%s" json


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
    ResizeArray [OntologySourceReference.create("Description", "path/to/file", "OSR Name")],
    ResizeArray [Publication.create(doi="any-nice-doi-42")],
    ResizeArray [Person.create(firstName="Kevin", lastName="Frey")], 
    ResizeArray [ArcAssay.init("Assay 1"); ArcAssay.init("Assay 2")],
    ResizeArray [ArcStudy.init("Study")],
    ResizeArray ["Study"],
    ResizeArray [Comment.create("Hello", "World")]
  )
  let filled_jsonString = 
    """{"Identifier":"My Inv","Title":"My Title","Description":"My Description","SubmissionDate":"My Submission Date","PublicReleaseDate":"My Release Date","OntologySourceReferences":[{"description":"Description","file":"path/to/file","name":"OSR Name"}],"Publications":[{"doi":"any-nice-doi-42"}],"Contacts":[{"firstName":"Kevin","lastName":"Frey"}],"Assays":[{"Identifier":"Assay 1"},{"Identifier":"Assay 2"}],"Studies":[{"Identifier":"Study"}],"RegisteredStudyIdentifiers":["Study"],"Comments":[{"@id":"Hello","name":"World"}]}"""

  testList "encode" [
    testCase "Empty" <| fun _ ->
      let actual = Encode.toJsonString  0 <| Investigation.encoder init
      let expected = init_jsonString
      Expect.equal actual expected ""
    testCase "Filled" <| fun _ ->
      let actual = Encode.toJsonString  0 <| Investigation.encoder filled
      let expected = filled_jsonString
      Expect.equal actual expected ""
  ]
  testList "decode" [
    testCase "Empty" <| fun _ ->
      let actual = Decode.fromJsonString Investigation.decoder init_jsonString
      let expected = init
      Expect.equal actual expected ""
    testCase "Filled" <| fun _ ->
      let actual = Decode.fromJsonString Investigation.decoder filled_jsonString
      let expected = filled
      Expect.equal actual expected ""
  ]
]