module Identifier.Tests

open ARCtrl

open TestingUtils

open ARCtrl.Helper.Identifier

let private tests_checkValidCharacters = testList "checkValidCharacters" [
    // Function to test the checkValidCharacters function with a given identifier
    let testCheckValidCharacter identifier =
        testCase ("Valid:" + identifier) <| fun _ ->
            checkValidCharacters identifier

    // Function to test the checkValidCharacters function with an invalid identifier
    let testCheckInvalidCharacter identifier =
        testCase ("Invalid:" + identifier) <| fun _ ->
           let eval() = checkValidCharacters identifier
           Expect.throws eval ""

    testCheckValidCharacter "HelloWorld123"
    testCheckValidCharacter "Valid_Identifier_1"
    testCheckValidCharacter "My Awesome new identifier 12"
    testCheckValidCharacter "Whitespace Test"
    testCheckInvalidCharacter "Invalid*Identifier"
    testCheckInvalidCharacter "Contains!Special#Characters"
    testCheckInvalidCharacter "Identifier with @ Symbol"
    testCheckInvalidCharacter "oh/no/a/path"
    testCheckInvalidCharacter "oh\no\a\windows\path"
]

let private tests_cwlFileNameFromIdentifier = testList "cwlFileNameFromIdentifier" [
    testCase "Workflow_Valid" <| fun _ ->
        let actual = Workflow.cwlFileNameFromIdentifier "MyWorkflow"
        let expected = "workflows/MyWorkflow/workflow.cwl"
        Expect.equal actual expected "Workflow cwl file path should be normalized"

    testCase "Run_Valid" <| fun _ ->
        let actual = Run.cwlFileNameFromIdentifier "MyRun"
        let expected = "runs/MyRun/run.cwl"
        Expect.equal actual expected "Run cwl file path should be normalized"

    testCase "Workflow_InvalidIdentifier_Throws" <| fun _ ->
        let eval () = Workflow.cwlFileNameFromIdentifier "workflow/invalid" |> ignore
        Expect.throws eval "Workflow cwl path generation should reject invalid identifiers"

    testCase "Run_InvalidIdentifier_Throws" <| fun _ ->
        let eval () = Run.cwlFileNameFromIdentifier "run\\invalid" |> ignore
        Expect.throws eval "Run cwl path generation should reject invalid identifiers"
]


let main = 
    testList "Identifier" [
        tests_checkValidCharacters
        tests_cwlFileNameFromIdentifier
    ]
