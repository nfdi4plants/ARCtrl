module Identifier.Tests

open ARCtrl.ISA

open TestingUtils

open ARCtrl.ISA.Identifier

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


let main = 
    testList "Identifier" [
        tests_checkValidCharacters
    ]