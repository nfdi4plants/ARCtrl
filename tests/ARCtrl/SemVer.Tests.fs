module ARCtrl.SemVer.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open ARCtrl
open ARCtrl.Contract
open FsSpreadsheet


let private tests_tryOfString = testList "tryOfString" [
    testCase "Basic" <| fun _ ->
        let str = "1.7.9"
        let output = SemVer.tryOfString str
        let actual_semver : SemVer = Expect.wantSome output ""
        let expected = SemVer.make 1 7 9 None None // make is most robust with the least magic
        Expect.equal actual_semver expected "equal"
    testCase "Prerelease" <| fun _ ->
        let str = "1.7.9-alpha.01"
        let output = SemVer.tryOfString str
        let actual_semver : SemVer = Expect.wantSome output ""
        let expected = SemVer.make 1 7 9 (Some "alpha.01") None // make is most robust with the least magic
        Expect.equal actual_semver expected "equal"
    testCase "Metadata" <| fun _ ->
        let str = "1.7.9+fsx287j21"
        let output = SemVer.tryOfString str
        let actual_semver : SemVer = Expect.wantSome output ""
        let expected = SemVer.make 1 7 9 (None) (Some "fsx287j21") // make is most robust with the least magic
        Expect.equal actual_semver expected "equal"
    testCase "Prerelease+Metadata" <| fun _ ->
        let str = "1.7.9-alpha.01+fsx287j21"
        let output = SemVer.tryOfString str
        let actual_semver : SemVer = Expect.wantSome output ""
        let expected = SemVer.make 1 7 9 (Some "alpha.01") (Some "fsx287j21") // make is most robust with the least magic
        Expect.equal actual_semver expected "equal"
]

let private tests_AsString = testList "AsString" [
    testCase "Basic" <| fun _ ->
        let semver = SemVer.make 1 7 9 None None // make is most robust with the least magic
        let actual = semver.AsString()
        let expected = "1.7.9"
        Expect.equal actual expected "equal"
    testCase "Prerelease" <| fun _ ->
        let semver = SemVer.make 1 7 9 (Some "alpha.01") None // make is most robust with the least magic
        let actual = semver.AsString()
        let expected = "1.7.9-alpha.01"
        Expect.equal actual expected "equal"
    testCase "Metadata" <| fun _ ->
        let semver = SemVer.make 1 7 9 (None) (Some "fsx287j21") // make is most robust with the least magic
        let actual = semver.AsString()
        let expected = "1.7.9+fsx287j21"
        Expect.equal actual expected "equal"
    testCase "Prerelease+Metadata" <| fun _ ->
        let semver = SemVer.make 1 7 9 (Some "alpha.01") (Some "fsx287j21") // make is most robust with the least magic
        let actual = semver.AsString()
        let expected = "1.7.9-alpha.01+fsx287j21"
        Expect.equal actual expected "equal"
]



let main = testList "SemVer" [
    tests_tryOfString
    tests_AsString
]
