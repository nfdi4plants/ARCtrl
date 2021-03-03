module AssayFileTests


open FSharpSpreadsheetML
open ISADotNet

open Expecto
open TestingUtils

open ISADotNet.XLSX
open ISADotNet.XLSX.AssayFile

[<Tests>]
let testColumnHeaderFunctions = 

    testList "ColumnHeaderFunctionTests" [
        testCase "RawKindTest" (fun () ->

            let headerString = "RawHeader"

            let header = AnnotationColumn.ColumnHeader.fromStringHeader headerString

            let testHeader = AnnotationColumn.ColumnHeader.create headerString "RawHeader" None None

            Expect.equal header testHeader "Should have used header string as kind"
        )
        testCase "NumberedRawKindTest" (fun () ->

            let headerString = "RawHeader (#5)"

            let header = AnnotationColumn.ColumnHeader.fromStringHeader headerString

            let testHeader = AnnotationColumn.ColumnHeader.create headerString "RawHeader" None (Some 5)

            Expect.equal header testHeader "Number was not parsed correctly"
        )
        testCase "NameWithNoOntology" (fun () ->

            let headerString = "NamedHeader [Name]"

            let header = AnnotationColumn.ColumnHeader.fromStringHeader headerString

            let testHeader = AnnotationColumn.ColumnHeader.create headerString "NamedHeader" (OntologyAnnotation.fromString "Name" "" "" |> Some) None

            Expect.equal header testHeader "Dit not parse Name correctly"
        )
        testCase "NumberedWithOntology" (fun () ->

            let headerString = "NamedHeader [Term] (#3; #tSource:Accession)"

            let header = AnnotationColumn.ColumnHeader.fromStringHeader headerString

            let testHeader = AnnotationColumn.ColumnHeader.create headerString "NamedHeader" (OntologyAnnotation.fromString "Term" "Accession" "Source" |> Some) (Some 3)

            Expect.equal header testHeader "Dit not parse Name correctly"
        )
    ]

[<Tests>]
let testHeaderSplittingFunctions = 
    testList "HeaderSplittingFunctionTests" [
        testCase "SplitBySamplesOnlySource" (fun () ->

            let headers = ["Source Name";"Paramer [MyDude]";"Characteristic [MyGuy]"]

            let resultHeaders = AnnotationTable.splitBySamples headers

            Expect.hasLength resultHeaders 1 "Shouldn't have split headers, as only one source column was set"
            Expect.sequenceEqual headers (resultHeaders |> Seq.head) "Shouldn't have split headers, as only one source column was set"
        )
        testCase "SplitBySamplesOnlySample" (fun () ->

            let headers = ["Sample Name";"Paramer [MyDude]";"Characteristic [MyGuy]"]

            let resultHeaders = AnnotationTable.splitBySamples headers

            Expect.hasLength resultHeaders 1 "Shouldn't have split headers, as only one sample column was set"
            Expect.sequenceEqual headers (resultHeaders |> Seq.head) "Shouldn't have split headers, as only one sample column was set"
        )
        testCase "SplitBySamplesOnlySourceAndSample" (fun () ->

            let headers = ["Source Name";"Paramer [MyDude]";"Sample Name";"Characteristic [MyGuy]"]

            let resultHeaders = AnnotationTable.splitBySamples headers

            Expect.hasLength resultHeaders 1 "Shouldn't have split headers, as only one source column and sample column was set"
            Expect.sequenceEqual headers (resultHeaders |> Seq.head) "Shouldn't have split headers, as only one source column and sample column was set"
        )
        testCase "SplitBySamplesManySamples" (fun () ->

            let headers = ["Source Name";"Paramer [MyDude]";"Sample Name";"Characteristic [MyGuy]";"Sample Name (#2)"]

            let resultHeaders = AnnotationTable.splitBySamples headers

            let testResults = seq [["Source Name";"Paramer [MyDude]";"Sample Name"];["Sample Name";"Characteristic [MyGuy]";"Sample Name (#2)"]] |> Seq.map (List.toSeq)

            Expect.hasLength resultHeaders 2 "Should have split headers, as several sample columns were set"
            Expect.sequenceEqual resultHeaders testResults "Headers were not split correctly"
        )
        testCase "splitByNamedProtocolsNoProtocols" (fun () ->

            let headers = ["Source Name";"Paramer [MyDude]";"Parameter [MyBro]";"Parameter [MyGuy]";"Sample Name"]

            let namedProtocols = []

            let resultHeaders = AnnotationTable.splitByNamedProtocols namedProtocols headers

            Expect.hasLength resultHeaders 1 "Shouldn't have split headers, as no named protocol was given"
            Expect.sequenceEqual headers (resultHeaders |> Seq.head |> snd) "Shouldn't have split headers, as no named protocol was given"
        )
        testCase "splitByNamedProtocols" (fun () ->

            let headers = ["Source Name";"Paramer [MyDude]";"Parameter [MyBro]";"Parameter [MyGuy]";"Sample Name"]

            let protocolWithName = Protocol.create None (Some "NamedProtocol") None None None None None None None
            let namedProtocols = [protocolWithName,seq ["Parameter [MyBro]";"Parameter [MyGuy]"]]

            let resultHeaders = AnnotationTable.splitByNamedProtocols namedProtocols headers |> Seq.map (fun (p,s) -> p, List.ofSeq s)

            let testResults = 
                [
                    Protocol.empty,["Source Name";"Paramer [MyDude]";"Sample Name"]
                    protocolWithName,["Source Name";"Parameter [MyBro]";"Parameter [MyGuy]";"Sample Name"]
                ]

            Expect.hasLength resultHeaders 2 "Should have split headers with the given protocol"
            Expect.isTrue (testResults.Head |> snd |> Seq.contains "Source Name" && testResults.Head |> snd |> Seq.contains "Sample Name") "Source and Sample column were not added to named Protocol"
            Expect.isTrue (testResults |> Seq.item 1|> snd |> Seq.contains "Source Name" && testResults |> Seq.item 1 |> snd |> Seq.contains "Sample Name") "Source and Sample column were not added to unnamed Protocol"
            Expect.sequenceEqual resultHeaders testResults "Headers were not split correctly"
        )
        testCase "splitByNamedProtocolsWrongHeaders" (fun () ->

            let headers = ["Source Name";"Paramer [MyDude]";"Parameter [MyBro]";"Parameter [MyGuy]";"Sample Name"]

            let protocolWithName = Protocol.create None (Some "NamedProtocol") None None None None None None None
            let namedProtocols = [protocolWithName,seq ["Parameter [Wreeeng]";"Parameter [Wraaang]"]]

            let resultHeaders = AnnotationTable.splitByNamedProtocols namedProtocols headers

            Expect.hasLength resultHeaders 1 "Shouldn't have split headers, as headers did not match"
            Expect.sequenceEqual headers (resultHeaders |> Seq.head |> snd) "Shouldn't have split headers, as headers did not match"
        )
    ]

[<Tests>]
let testMetaDataFunctions = 

    let sourceDirectory = __SOURCE_DIRECTORY__ + @"/TestFiles/"
    let sinkDirectory = System.IO.Directory.CreateDirectory(__SOURCE_DIRECTORY__ + @"/TestResult/").FullName
    let referenceAssayFilePath = System.IO.Path.Combine(sourceDirectory,"AssayTestFile.xlsx")

    testList "MetaDataTests" [
        testCase "CanReadMetaData" (fun () ->

            let doc = Spreadsheet.fromFile referenceAssayFilePath false

            let sst = Spreadsheet.getSharedStringTable doc

            let rows = 
                Spreadsheet.tryGetSheetBySheetName "Investigation" doc
                |> Option.get
                |> SheetData.getRows
                |> Seq.map (Row.mapCells (Cell.includeSharedStringValue sst))

            let readingSuccess = 
                try 
                    AssayFile.MetaData.fromRows rows |> ignore
                    Result.Ok "DidRun"
                with
                | err -> Result.Ok(sprintf "Reading the test metadata failed: %s" err.Message)

            Expect.isOk readingSuccess (Result.getMessage readingSuccess)
        )
        testCase "ReadsMetaDataCorrectly" (fun () ->

            let doc = Spreadsheet.fromFile referenceAssayFilePath false

            let sst = Spreadsheet.getSharedStringTable doc

            let rows = 
                Spreadsheet.tryGetSheetBySheetName "Investigation" doc
                |> Option.get
                |> SheetData.getRows
                |> Seq.map (Row.mapCells (Cell.includeSharedStringValue sst))

            let assay,contacts = AssayFile.MetaData.fromRows rows 

            let testAssay = Assays.fromString "protein expression profiling" "http://purl.obolibrary.org/obo/OBI_0000615" "OBI" "mass spectrometry" "" "OBI" "iTRAQ" "" []

            let testContact = Contacts.fromString "Leo" "Zeef" "A" "" "" "" "Oxford Road, Manchester M13 9PT, UK" "Faculty of Life Sciences, Michael Smith Building, University of Manchester" "author" "" "" [Comment.fromString "Worksheet" "Sheet3"]
                
            Expect.isSome assay "Assay metadata information could not be read from metadata sheet"

            Expect.equal assay.Value testAssay "Assay metadata information could not be correctly read from metadata sheet"

            Expect.hasLength contacts 3 "Wrong count of parsed contacts"

            Expect.equal contacts.[2] testContact "Test Person could not be correctly read from metadata sheet"
        )
    ]
    |> testSequenced

