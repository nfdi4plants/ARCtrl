module Person.Tests

open ARCtrl.ISA

open TestingUtils

let private createTestPerson() = Person.create(FirstName="Kevin", LastName="Frey",Email="MyAwesomeMail@Awesome.mail")

let private tests_ORCID =
    testList "ORCID" [
        testCase "fromComment correctORCID" (fun () ->
            let comment = Comment.create(Name = Person.orcidKey, Value = "0000-0002-1825-0097")
            let p = Person.create(FirstName="My", LastName="Dude",Comments =[|comment|])
            let newP = Person.setOrcidFromComments p
            Expect.isSome newP.ORCID "ORCID should now be set" 
            Expect.equal newP.ORCID.Value "0000-0002-1825-0097" "ORCID not taken correctly"
            Expect.isNone newP.Comments "Comments not set to None"
        )
        testCase "fromComment wrongORCID" (fun () ->
            let comment = Comment.create(Name = "WrongKey", Value = "0000-0002-1825-0097")
            let p = Person.create(FirstName="My", LastName="Dude",Comments =[|comment|])
            let newP = Person.setOrcidFromComments p
            Expect.isNone newP.ORCID "ORCID should not have been taken"
            Expect.isSome newP.Comments "Comments should still be there"
            Expect.equal newP.Comments.Value.Length 1 "Comments should still be there"
        )
        testCase "fromComment deprecatedORCID" (fun () ->
            let comment = Comment.create(Name = "Investigation Person Orcid", Value = "0000-0002-1825-0097")
            let p = Person.create(FirstName="My", LastName="Dude",Comments =[|comment|])
            let newP = Person.setOrcidFromComments p
            Expect.isSome newP.ORCID "ORCID should now be set" 
            Expect.equal newP.ORCID.Value "0000-0002-1825-0097" "ORCID not taken correctly"
            Expect.isNone newP.Comments "Comments not set to None"
        )
        testCase "toComment SomeORCID" (fun () ->
            let p = Person.create(FirstName="My", LastName="Dude",ORCID = "0000-0002-1825-0097")
            let newP = Person.setCommentFromORCID p
            Expect.isSome newP.Comments "Comments should now be set" 
            Expect.equal newP.Comments.Value.Length 1 "Comments should now be set" 
            Expect.equal newP.Comments.Value.[0].Name.Value Person.orcidKey "Comments should now be set" 
            Expect.equal newP.Comments.Value.[0].Value.Value "0000-0002-1825-0097" "Comments should now be set"               
        ) 
        testCase "toComment NoneORCID" (fun () ->
            let p = Person.create(FirstName="My", LastName="Dude")
            let newP = Person.setCommentFromORCID p
            Expect.isNone newP.Comments "Comments should not be set" 
        )
    
    ]

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
        tests_ORCID
    ]