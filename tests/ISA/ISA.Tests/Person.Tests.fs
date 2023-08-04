module Person.Tests

open ARCtrl.ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let private createTestPerson() = Person.create(FirstName="Kevin", LastName="Frey",Email="MyAwesomeMail@Awesome.mail")

let private tests_Copy = testList "Copy" [
    testCase "test copy" <| fun _ ->
        // this test is silly in dotnet as record types are immutable. Test this in js native too!
        let person = createTestPerson()
        Expect.equal person.FirstName (Some "Kevin") "firstname"
        let copy = person.Copy()
        let changedPerson = {person with FirstName = Some "DefNotKevin"}
        Expect.equal copy.FirstName (Some "Kevin") "copy firstname"
        Expect.equal changedPerson.FirstName (Some "DefNotKevin") "changedPerson firstname"
]

let main = 
    testList "Person" [
        tests_Copy
    ]