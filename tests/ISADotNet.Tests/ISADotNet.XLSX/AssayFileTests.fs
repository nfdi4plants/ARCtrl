module AssayFileTests


open FSharpSpreadsheetML
open ISADotNet

open Expecto
open TestingUtils

open ISADotNet.XLSX
open ISADotNet.XLSX.AssayFile

[<Tests>]
let testSwateHeaderFunctions = 

    testList "SwateHeaderFunctionTests" [
        testCase "RawKindTest" (fun () ->

            let headerString = "RawHeader"

            let header = AnnotationColumn.SwateHeader.fromStringHeader headerString

            let testHeader = AnnotationColumn.SwateHeader.create headerString "RawHeader" None None

            Expect.equal header testHeader "Should have used header string as kind"
        )
        testCase "NumberedRawKindTest" (fun () ->

            let headerString = "RawHeader (#5)"

            let header = AnnotationColumn.SwateHeader.fromStringHeader headerString

            let testHeader = AnnotationColumn.SwateHeader.create headerString "RawHeader" None (Some 5)

            Expect.equal header testHeader "Number was not parsed correctly"
        )
        testCase "NameWithNoOntology" (fun () ->

            let headerString = "NamedHeader [Name]"

            let header = AnnotationColumn.SwateHeader.fromStringHeader headerString

            let testHeader = AnnotationColumn.SwateHeader.create headerString "NamedHeader" (OntologyAnnotation.fromString "Name" "" "" |> Some) None

            Expect.equal header testHeader "Dit not parse Name correctly"
        )
        testCase "NumberedWithOntology" (fun () ->

            let headerString = "NamedHeader [Term] (#3; #tSource:Accession)"

            let header = AnnotationColumn.SwateHeader.fromStringHeader headerString

            let testHeader = AnnotationColumn.SwateHeader.create headerString "NamedHeader" (OntologyAnnotation.fromString "Term" "Accession" "Source" |> Some) (Some 3)

            Expect.equal header testHeader "Dit not parse Name correctly"
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

