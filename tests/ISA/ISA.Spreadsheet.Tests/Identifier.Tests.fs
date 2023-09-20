module IdentifierTests

open TestingUtils

open ARCtrl.ISA.Identifier

// Function to test identifierFromFileName
let private testIdentifierFromFileName () =
    let validTestFileNames = [
        @"assays/MyAssay/isa.assay.xlsx", "MyAssay"
        @"MyAssay/isa.assay.xlsx", "MyAssay"
        @"assays/My_Assay/isa.assay.xlsx", "My_Assay"
        @"assays/Awesome Assay/isa.assay.xlsx", "Awesome Assay"
        @"TestAssay1\isa.assay.xlsx", "TestAssay1" // Windows path separator
        @"assays\TestAssay1\isa.assay.xlsx", "TestAssay1" // Windows path separator
        @"assays\TestAssay1/isa.assay.xlsx", "TestAssay1" // Mixed path separator
        @"assays\my-awesome-assay/isa.assay.xlsx", "my-awesome-assay" // new allowed "-" character
    ]
    let invalidTestFileName = [
        "isa.assay.xlsx"
        "invalid_file_name.xlsx"
        ""
    ]

    let validTestCases = 
        validTestFileNames
        |> List.map (fun (fileName, expectedIdentifier) ->
            testCase ("identifierFromFileName:" + fileName) <| fun _ ->
                let actualIdentifier = Assay.identifierFromFileName fileName
                Expect.equal actualIdentifier expectedIdentifier "Extracted identifier matches the expected value"
        )
    let invalidTestCases = 
        invalidTestFileName
        |> List.map (fun (fileName) ->
            testCase ("identifierFromFileName:" + fileName) <| fun _ ->
                let eval = fun () -> Assay.identifierFromFileName fileName |> ignore
                Expect.throws eval ""
        )
    validTestCases@invalidTestCases

// Function to test fileNameFromIdentifier
let private testFileNameFromIdentifier () =
    let validTestIdentifier = [
        "MyAssay", "assays/MyAssay/isa.assay.xlsx"
        "My_Assay", "assays/My_Assay/isa.assay.xlsx"
        "Assay123", "assays/Assay123/isa.assay.xlsx"
    ]
    let invalidTestIdentifier = [
        ""
        "Invalid/path"
        "notassays/identifier/isa.assay.xlsx"
    ]

    let validTestCases = 
        validTestIdentifier
        |> List.map (fun (fileName, expectedIdentifier) ->
            testCase ("fileNameFromIdentifier:" + fileName) <| fun _ ->
                let actualIdentifier = Assay.fileNameFromIdentifier fileName
                Expect.equal actualIdentifier expectedIdentifier "Extracted identifier matches the expected value"
        )
    let invalidTestCases = 
        invalidTestIdentifier
        |> List.map (fun (fileName) ->
            testCase ("fileNameFromIdentifier:" + fileName) <| fun _ ->
                let eval = fun () -> Assay.fileNameFromIdentifier fileName |> ignore
                Expect.throws eval ""
        )
    validTestCases@invalidTestCases

let test_identifierFromFileName = testList "identifierFromFileName" [
    yield! testIdentifierFromFileName ()
    testCase "createMissingIdentifier" <| fun _ ->
        let pseudoFileName = createMissingIdentifier()
        let identifierFromFileName = Assay.identifierFromFileName pseudoFileName
        Expect.equal identifierFromFileName pseudoFileName ""

]
let test_fileNameFromIdentifier = testList "fileNameFromIdentifier" (testFileNameFromIdentifier ())

let tests_Assay = testList "Assay" [
    test_identifierFromFileName
    test_fileNameFromIdentifier
]   

let main = testList "Identifier" [
    tests_Assay
]