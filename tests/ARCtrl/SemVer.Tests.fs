module ARCtrl.SemVer.Tests

open TestingUtils
open ARCtrl.Helper.SemVer
open ARCtrl


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
        // Equality ignores metadata, so it needs to be checked separately
        Expect.equal actual_semver.Metadata expected.Metadata "metadata"
    testCase "Prerelease+Metadata" <| fun _ ->
        let str = "1.7.9-alpha.01+fsx287j21"
        let output = SemVer.tryOfString str
        let actual_semver : SemVer = Expect.wantSome output ""
        let expected = SemVer.make 1 7 9 (Some "alpha.01") (Some "fsx287j21") // make is most robust with the least magic
        Expect.equal actual_semver expected "equal"
        Expect.equal actual_semver.Metadata expected.Metadata "metadata"
    testCase "MajorMinor" <| fun _ ->
        let str = "1.7"
        let output = SemVer.tryOfString str
        let actual_semver : SemVer = Expect.wantSome output ""
        let expected = SemVer.make 1 7 0 None None
        Expect.equal actual_semver expected "missing patch should default to 0"
    testCase "MajorOnly" <| fun _ ->
        let str = "1"
        let output = SemVer.tryOfString str
        let actual_semver : SemVer = Expect.wantSome output ""
        let expected = SemVer.make 1 0 0 None None
        Expect.equal actual_semver expected "missing minor and patch should default to 0"
    testCase "MajorMinor+Prerelease" <| fun _ ->
        let str = "1.7-alpha.1"
        let output = SemVer.tryOfString str
        let actual_semver : SemVer = Expect.wantSome output ""
        let expected = SemVer.make 1 7 0 (Some "alpha.1") None
        Expect.equal actual_semver expected "missing patch should default to 0"
    testCase "Invalid" <| fun _ ->
        let output = SemVer.tryOfString "1.7.x"
        Expect.isNone output "invalid version should not be parsed"
]

let private parse (str: string) =
    Expect.wantSome (SemVer.tryOfString str) $"\"{str}\" should be parseable"

// https://github.com/nfdi4plants/ARCtrl/issues/639
let private tests_compare = testList "Compare" [
    let testLower (lower: string) (higher: string) =
        testCase $"{lower} < {higher}" <| fun _ ->
            let l, h = parse lower, parse higher
            Expect.isTrue (l < h) $"{lower} should be lower than {higher}"
            Expect.isTrue (h > l) $"{higher} should be higher than {lower}"
            Expect.notEqual l h "should not be equal"

    // Examples from the issue
    testLower "1.0.0" "1.0.1"
    testLower "1.0.0-alpha" "1.0.0"
    testLower "1.0.0-alpha5" "1.0.0"
    // Precedence examples from https://semver.org/#spec-item-11
    testLower "1.0.0" "2.0.0"
    testLower "2.0.0" "2.1.0"
    testLower "2.1.0" "2.1.1"
    testLower "1.9.0" "1.10.0"
    testLower "1.0.0-alpha" "1.0.0-alpha.1"
    testLower "1.0.0-alpha.1" "1.0.0-alpha.beta"
    testLower "1.0.0-alpha.beta" "1.0.0-beta"
    testLower "1.0.0-beta" "1.0.0-beta.2"
    testLower "1.0.0-beta.2" "1.0.0-beta.11"
    testLower "1.0.0-beta.11" "1.0.0-rc.1"
    testLower "1.0.0-rc.1" "1.0.0"
    // Numeric identifiers are compared by value, also beyond the int range
    testLower "1.0.0-alpha.99999999999" "1.0.0-alpha.100000000000"
    testCase "Sort" <| fun _ ->
        let expected = [
            "1.0.0-alpha"; "1.0.0-alpha.1"; "1.0.0-alpha.beta"; "1.0.0-beta"; "1.0.0-beta.2"
            "1.0.0-beta.11"; "1.0.0-rc.1"; "1.0.0"; "1.0.1"; "1.10.0"; "2.0.0"
        ]
        let actual =
            [
                "1.0.0"; "1.10.0"; "1.0.0-beta.11"; "1.0.0-alpha.beta"; "2.0.0"; "1.0.0-alpha"
                "1.0.1"; "1.0.0-rc.1"; "1.0.0-beta"; "1.0.0-alpha.1"; "1.0.0-beta.2"
            ]
            |> List.map parse
            |> List.sort
            |> List.map (fun v -> v.AsString())
        Expect.sequenceEqual actual expected "versions should be sorted by precedence"
    testCase "Max" <| fun _ ->
        let actual = [ parse "1.0.0"; parse "1.0.0-alpha" ] |> List.max
        Expect.equal (actual.AsString()) "1.0.0" "release should be the latest version"
]

// https://github.com/nfdi4plants/ARCtrl/issues/639
let private tests_equality = testList "Equality" [
    testCase "Metadata ignored" <| fun _ ->
        let v1, v2 = parse "1.0.0", parse "1.0.0+build"
        Expect.equal v1 v2 "metadata should be ignored for equality"
        Expect.equal (compare v1 v2) 0 "metadata should be ignored for precedence"
        Expect.equal (v1.GetHashCode()) (v2.GetHashCode()) "metadata should be ignored for hash code"
    testCase "Metadata ignored, prerelease" <| fun _ ->
        let v1, v2 = parse "1.0.0-alpha.1+build.1", parse "1.0.0-alpha.1+build.2"
        Expect.equal v1 v2 "metadata should be ignored for equality"
        Expect.equal (compare v1 v2) 0 "metadata should be ignored for precedence"
        Expect.equal (v1.GetHashCode()) (v2.GetHashCode()) "metadata should be ignored for hash code"
    testCase "Prerelease not equal to release" <| fun _ ->
        let v1, v2 = parse "1.0.0", parse "1.0.0-alpha5"
        Expect.notEqual v1 v2 "prerelease should not equal release"
    // Comparison must only report equal precedence for equal versions, e.g. for use in sets and maps
    testCase "Compare consistent with equality" <| fun _ ->
        let versions =
            [
                "1.0.0"; "1.0.0+build"; "1.0.0-alpha"; "1.0.0-alpha.1"; "1.0.0-alpha.01"; "1.0.0-alpha.001+build"
                "1.0.0-alpha.a"; "1.0.0-alpha..1"; "1.0.1"
            ]
            |> List.map parse
        for v1 in versions do
            for v2 in versions do
                let name = $"{v1.AsString()} and {v2.AsString()}"
                Expect.equal (compare v1 v2 = 0) (v1 = v2) $"{name}: compare and equality should agree"
                Expect.equal (compare v1 v2 < 0) (compare v2 v1 > 0) $"{name}: compare should be antisymmetric"
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
    tests_compare
    tests_equality
]
