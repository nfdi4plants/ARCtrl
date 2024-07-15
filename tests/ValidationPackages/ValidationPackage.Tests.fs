module Tests.ValidationPackage

open TestingUtils

open ARCtrl
open ARCtrl.ValidationPackages

let tests_instance_methods = testList "Instance methods" [
    let vp = ValidationPackage("name", "version")
    let vp_no_version = ValidationPackage("name")
    testCase "make - name and version" <| fun _ ->
        let actual = ValidationPackage.make "name" (Some "version")
        let expected = vp
        Expect.equal actual expected ""
    testCase "make - no version" <| fun _ ->
        let actual = ValidationPackage.make "name" None
        let expected = vp_no_version
        Expect.equal actual expected ""
    testCase "Copy - name and version" <| fun _ ->
        let actual = vp.Copy()
        let expected = vp
        Expect.equal actual expected ""
    testCase "Copy - no version" <| fun _ ->
        let actual = vp_no_version.Copy()
        let expected = vp_no_version
        Expect.equal actual expected ""
    testCase "ToString - name and version" <| fun _ ->
        let actual = vp.ToString()
        let expected = "-\n  name: name\n  version: version"
        Expect.trimEqual actual expected ""
    testCase "ToString - no version" <| fun _ ->
        let actual = vp_no_version.ToString()
        let expected = "-\n  name: name"
        Expect.trimEqual actual expected ""
    testCase "Equals - name and version" <| fun _ ->
        let actual = ValidationPackage("name", "version").Equals(vp)
        let expected = true
        Expect.equal actual expected ""
    testCase "Equals - no version" <| fun _ ->
        let actual = ValidationPackage("name").Equals(vp_no_version)
        let expected = true
        Expect.equal actual expected ""
    testCase "GetHashCode - name and version" <| fun _ ->
        let actual = ValidationPackage("name", "version").GetHashCode()
        let expected = vp.GetHashCode()
        Expect.equal actual expected ""
    testCase "GetHashCode - no version" <| fun _ ->
        let actual = ValidationPackage("name").GetHashCode()
        let expected = vp_no_version.GetHashCode()
        Expect.equal actual expected ""

]

let main = testList "ValidationPackage" [
    tests_instance_methods
]