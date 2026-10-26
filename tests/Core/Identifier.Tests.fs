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

// https://github.com/nfdi4plants/ARCtrl/issues/303
let private tests_tryCheckValidCharacters = testList "tryCheckValidCharacters" [
    let testValid name identifier =
        testCase ("Valid:" + name) <| fun _ ->
            Expect.isTrue (tryCheckValidCharacters identifier) $"\"{identifier}\" should be valid"

    let testInvalid name identifier =
        testCase ("Invalid:" + name) <| fun _ ->
            Expect.isFalse (tryCheckValidCharacters identifier) $"\"{identifier}\" should be invalid"

    testValid "boundary characters" "azAZ09_-"
    testValid "inner whitespace" "My Assay 1"
    testValid "surrounding whitespace" "  MyAssay  "
    testValid "surrounding tab and newline" "\tMyAssay\n"
    testInvalid "empty" ""
    testInvalid "whitespace only" "   "
    testInvalid "inner tab" "My\tAssay"
    testInvalid "inner newline" "My\nAssay"
    testInvalid "non-ascii letter" "Mäßig"
    testInvalid "dot" "My.Assay"
    testInvalid "last character" "MyAssay!"
    // The validation must not keep state between calls, e.g. lastIndex of a reused global regex in JavaScript
    testCase "repeated calls" <| fun _ ->
        for _ in 1 .. 3 do
            Expect.isTrue (tryCheckValidCharacters "MyAssay") "Valid identifier should stay valid"
            Expect.isTrue (tryCheckValidCharacters "MyAssay") "Same identifier should be valid twice in a row"
            Expect.isFalse (tryCheckValidCharacters "My/Assay") "Invalid identifier should stay invalid"
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
        tests_tryCheckValidCharacters
        tests_cwlFileNameFromIdentifier
    ]
