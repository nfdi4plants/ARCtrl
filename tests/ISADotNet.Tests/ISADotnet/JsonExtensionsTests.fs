module JsonExtensionsTests

open Expecto
open TestingUtils
open ISADotNet
open System.Text.Json

module TestTypes = 

    type ExampleRecord = 
        {
            A : string
        }

    [<AnyOf>]
    type AnyOfType = 
        | [<SerializationOrder(0)>] IntField of int
        | [<SerializationOrder(1)>] ObjectField of ExampleRecord
        | [<SerializationOrder(2)>] StringField of string

    [<AnyOf>]
    type AnyOfTypeReverse = 
        | [<SerializationOrder(1)>] SecondField of int
        | [<SerializationOrder(0)>] FirstField of string
      
    type NotAnyOfType = 
        | [<SerializationOrder(0)>] NotIntField of int
        | [<SerializationOrder(1)>] NotObjectField of ExampleRecord
        | [<SerializationOrder(2)>] NotStringField of string

    [<StringEnum>]
    type StringEnumType =        
        | [<StringEnumValue("enumeroNumeroUno")>] EnumOne
        | EnumTwo

    type NotStringEnumType =        
        | [<StringEnumValue("enumeroNumeroUno")>] NotEnumOne
        | NotEnumTwo

open TestTypes

[<Tests>]
let testAnyOf =     

    let intExample      = IntField 5
    let objectExample   = ObjectField {A = "I am an object"}
    let stringExample   = StringField "I am a string"   

    testList "AnyOfTests" [

        testCase "WritesCorrect" (fun () -> 
            
            let s = JsonSerializer.Serialize(stringExample,JsonExtensions.options)

            let containsFlagString = s.Contains("Flag")
            let containsCaseString = s.Contains("Case")
            let containsKeyString = s.Contains("Key")
            let containsTagString =  s.Contains("Tag")

            Expect.isFalse containsFlagString "Should write anyof style but contained \"Flag\"" 
            Expect.isFalse containsCaseString "Should write anyof style but contained \"Case\"" 
            Expect.isFalse containsKeyString  "Should write anyof style but contained \"Key\"" 
            Expect.isFalse containsTagString  "Should write anyof style but contained \"Tag\"" 

            Expect.equal s "\"I am a string\"" "String was written incorrectly"

        )

        testCase "WriteAndReadInt" (fun () ->

            JsonSerializer.Serialize(intExample,JsonExtensions.options)
            |> fun s -> JsonSerializer.Deserialize<AnyOfType>(s,JsonExtensions.options)
            |> fun d -> Expect.equal d intExample "AnyOf Integer could not be parsed correctly"

        )

        testCase "WriteAndReadString" (fun () ->

            JsonSerializer.Serialize(stringExample,JsonExtensions.options)
            |> fun s -> JsonSerializer.Deserialize<AnyOfType>(s,JsonExtensions.options)
            |> fun d -> Expect.equal d stringExample "AnyOf String could not be parsed correctly"

        )

        testCase "WriteAndReadRecordType" (fun () ->

            JsonSerializer.Serialize(objectExample,JsonExtensions.options)
            |> fun s -> JsonSerializer.Deserialize<AnyOfType>(s,JsonExtensions.options)
            |> fun d -> Expect.equal d objectExample "AnyOf Integer could not be parsed correctly"
        )
        |> testSequenced

        testCase "WriteAndReadAnyOfList" (fun () ->

            let list = [intExample;stringExample;objectExample]

            JsonSerializer.Serialize(list,JsonExtensions.options)
            |> fun s -> JsonSerializer.Deserialize<List<AnyOfType>>(s,JsonExtensions.options)
            |> fun d -> Expect.equal d list "List of AnyOfs could not be parsed correctly"
        )
        |> testSequenced

        testCase "DoesFollowSerializationOrder" (fun () ->

            let v = SecondField 10

            JsonSerializer.Serialize(v,JsonExtensions.options)
            |> fun s -> JsonSerializer.Deserialize<AnyOfTypeReverse>(s,JsonExtensions.options)
            |> fun d -> Expect.notEqual d v "Did parse as int, even though string should have been tried first"
                  
        )

        testCase "DoesConsiderAnyOfAttribute" (fun () ->

            let notAnyOfEmample = NotStringField "I should not be parsed as anyof"

            let s = JsonSerializer.Serialize(notAnyOfEmample,JsonExtensions.options)

            let containsFlagString = s.Contains("Flag")
            let containsCaseString = s.Contains("Case")
            let containsKeyString = s.Contains("Key")
            let containsTagString =  s.Contains("Tag")

            Expect.isTrue (containsFlagString || containsCaseString || containsKeyString || containsTagString) "Should not write anyof style but did not contain any union field identifier" 
        )

        |> testSequenced

    ]


[<Tests>]
let testStringEnum =     

    let unnamedExample = EnumTwo
    let namedExample = EnumOne


    testList "StringEnumTests" [

        testCase "WritesCorrect" (fun () -> 
            
            let s = JsonSerializer.Serialize(unnamedExample,JsonExtensions.options)

            let containsFlagString = s.Contains("Flag")
            let containsCaseString = s.Contains("Case")
            let containsKeyString = s.Contains("Key")
            let containsTagString =  s.Contains("Tag")

            Expect.isFalse containsFlagString "Should write anyof style but contained \"Flag\"" 
            Expect.isFalse containsCaseString "Should write anyof style but contained \"Case\"" 
            Expect.isFalse containsKeyString  "Should write anyof style but contained \"Key\"" 
            Expect.isFalse containsTagString  "Should write anyof style but contained \"Tag\"" 

            Expect.equal s "\"EnumTwo\"" "String Enum was written incorrectly"

        )

        testCase "WriteAndReadUnnamed" (fun () ->

            JsonSerializer.Serialize(unnamedExample,JsonExtensions.options)
            |> fun s -> JsonSerializer.Deserialize<StringEnumType>(s,JsonExtensions.options)
            |> fun d -> Expect.equal d unnamedExample "Unnamed String Enum could not be parsed correctly"

        )

        testCase "WriteAndReadNamed" (fun () ->

            JsonSerializer.Serialize(namedExample,JsonExtensions.options)
            |> fun s -> JsonSerializer.Deserialize<StringEnumType>(s,JsonExtensions.options)
            |> fun d -> Expect.equal d namedExample "Named String Enum could not be parsed correctly"

        )


        testCase "WriteAndReadStringEnumList" (fun () ->

            let list = [namedExample;unnamedExample]

            JsonSerializer.Serialize(list,JsonExtensions.options)
            |> fun s -> JsonSerializer.Deserialize<List<StringEnumType>>(s,JsonExtensions.options)
            |> fun d -> Expect.equal d list "List of String Enums could not be parsed correctly"
        )
        |> testSequenced

        testCase "DoesConsiderStringEnumAttribute" (fun () ->

            let notStringEnumEmample = NotEnumOne

            let s = JsonSerializer.Serialize(notStringEnumEmample,JsonExtensions.options)

            let containsFlagString = s.Contains("Flag")
            let containsCaseString = s.Contains("Case")
            let containsKeyString = s.Contains("Key")
            let containsTagString =  s.Contains("Tag")

            Expect.isTrue (containsFlagString || containsCaseString || containsKeyString || containsTagString) "Should not write string enum style but did not contain any union field identifier" 
        )

        |> testSequenced

    ]

