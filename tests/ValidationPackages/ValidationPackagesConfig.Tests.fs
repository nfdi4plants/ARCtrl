module Tests.ValidationPackagesConfig

open TestingUtils

open ARCtrl
open ARCtrl.ValidationPackages

let vp = ValidationPackages.ValidationPackage("name", "version")
let vp_no_version = ValidationPackages.ValidationPackage("name")

let vpc = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([vp; vp_no_version ]), "arc_specification")
let vpc_no_specs = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([vp; vp_no_version ]))

let tests_instance_methods = testList "Instance methods" [
    testCase "Copy - specs and packages" <| fun _ ->
        let actual = vpc.Copy()
        let expected = vpc
        Expect.equal actual expected ""
    testCase "Copy - no specs" <| fun _ ->
        let actual = vpc_no_specs.Copy()
        let expected = vpc_no_specs
        Expect.equal actual expected ""
    testCase "ToString - specs and packages" <| fun _ ->
        let actual = vpc.ToString()
        let expected = "{\n ARCSpecification = arc_specification\n ValidationPackages = [\n{\n Name = name\n Version = version\n};\n{\n Name = name\n}\n]\n}"
        Expect.trimEqual actual expected ""
    testCase "ToString - no specs" <| fun _ ->
        let actual = vpc_no_specs.ToString()
        let expected = "{\n ValidationPackages = [\n{\n Name = name\n Version = version\n};\n{\n Name = name\n}\n]\n}"
        Expect.trimEqual actual expected ""
    testCase "StructurallyEquals - specs and packages" <| fun _ ->
        let actual = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([vp_no_version; vp]), "arc_specification").StructurallyEquals(vpc)
        let expected = true
        Expect.equal actual expected ""
    testCase "StructurallyEquals - no specs" <| fun _ ->
        let actual = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([vp_no_version; vp])).StructurallyEquals(vpc_no_specs)
        let expected = true
        Expect.equal actual expected ""
    testCase "ReferenceEquals - specs and packages" <| fun _ ->
        let actual = vpc.ReferenceEquals(vpc)
        let expected = true
        Expect.equal actual expected ""
    testCase "ReferenceEquals - no specs" <| fun _ ->
        let actual = vpc_no_specs.ReferenceEquals(vpc_no_specs)
        let expected = true
        Expect.equal actual expected ""
    testCase "ReferenceEquals with other instance - specs and packages" <| fun _ ->
        let actual = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([vp; vp_no_version ]), "arc_specification").ReferenceEquals(vpc)
        let expected = false
        Expect.equal actual expected ""
    testCase "ReferenceEquals with other instance - no specs" <| fun _ ->
        let actual = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([vp; vp_no_version ])).ReferenceEquals(vpc_no_specs)
        let expected = false
        Expect.equal actual expected ""
    testCase "Equals - specs and packages" <| fun _ ->
        let actual = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([vp_no_version; vp]), "arc_specification").Equals(vpc)
        let expected = true
        Expect.equal actual expected ""
    testCase "Equals - no specs" <| fun _ ->
        let actual = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([vp_no_version; vp])).Equals(vpc_no_specs)
        let expected = true
        Expect.equal actual expected ""
    testCase "GetHashCode - specs and packages" <| fun _ ->
        let actual = vpc.GetHashCode() // not sure here if different object should have same or different hash (equivalkent to StructurallyEquals)
        let expected = vpc.GetHashCode()
        Expect.equal actual expected ""
    testCase "GetHashCode - no specs" <| fun _ ->
        let actual = vpc_no_specs.GetHashCode() // not sure here if different object should have same or different hash (equivalkent to StructurallyEquals)
        let expected = vpc_no_specs.GetHashCode()
        Expect.equal actual expected ""

]

let tests_static_methods = testList "Static methods" [
    testCase "make - specs and packages" <| fun _ ->
        let actual = ValidationPackagesConfig.make (new ResizeArray<ValidationPackage>([vp; vp_no_version ])) (Some "arc_specification")
        let expected = vpc
        Expect.equal actual expected ""
    testCase "make - no specs" <| fun _ ->
        let actual = ValidationPackagesConfig.make (new ResizeArray<ValidationPackage>([vp; vp_no_version ])) None
        let expected = vpc_no_specs
        Expect.equal actual expected ""
]


let main = testList "ValidationPackageConfig" [
    tests_instance_methods
    tests_static_methods
]