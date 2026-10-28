module Tests.ValidationPackagesConfig

open TestingUtils

open ARCtrl
open ARCtrl.ValidationPackages
open ARCtrl.Yaml

let tests_extended = testList "extended" [
    let vp = ValidationPackages.ValidationPackage("name", "version")
    let vp_no_version = ValidationPackages.ValidationPackage("name")

    let vpc = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([vp; vp_no_version ]), "arc_specification")
    let vpc_no_specs = ValidationPackages.ValidationPackagesConfig(new ResizeArray<ValidationPackage>([vp; vp_no_version ]))

    let vpc_yaml_string = $"""{ValidationPackagesConfig.ARC_SPECIFICATION_KEY}: arc_specification
{ValidationPackagesConfig.VALIDATION_PACKAGES_KEY}:
  - {ValidationPackage.NAME_KEY}: name
    {ValidationPackage.VERSION_KEY}: version
  - {ValidationPackage.NAME_KEY}: name
"""

    let vpc_no_specs_yaml_string = $"""validation_packages:
  - {ValidationPackage.NAME_KEY}: name
    {ValidationPackage.VERSION_KEY}: version
  - {ValidationPackage.NAME_KEY}: name
"""

    testList "encoder (toYamlString)" [
        testCase "no specification validation" <| fun _ -> 
            let actual = ValidationPackagesConfig.encoder vpc |> Encode.toYamlString 2
            let expected = vpc_yaml_string
            Expect.trimEqual actual expected ""
        testCase "with specification validation" <| fun _ -> 
            let actual = ValidationPackagesConfig.encoder vpc_no_specs |> Encode.toYamlString 2
            let expected = vpc_no_specs_yaml_string
            Expect.trimEqual actual expected ""
    ]
    testList "decoder (fromYamlString)" [ 
        testCase "name and version" <| fun _ -> 
            let actual = Decode.fromYamlString ValidationPackagesConfig.decoder vpc_yaml_string
            let expected = vpc
            Expect.equal actual expected ""
        testCase "no version" <| fun _ -> 
            let actual = Decode.fromYamlString ValidationPackagesConfig.decoder vpc_no_specs_yaml_string
            let expected = vpc_no_specs
            Expect.equal actual expected ""
    ]
    testList "roundtrip (fromYamlString >> toYamlString)" [
        testCase "name and version" <| fun _ -> 
            let actual =
                vpc_yaml_string
                |> Decode.fromYamlString ValidationPackagesConfig.decoder
                |> ValidationPackagesConfig.encoder
                |> Encode.toYamlString 2
            let expected = vpc_yaml_string
            Expect.trimEqual actual expected ""
        testCase "no version" <| fun _ -> 
            let actual =
                vpc_no_specs_yaml_string
                |> Decode.fromYamlString ValidationPackagesConfig.decoder
                |> ValidationPackagesConfig.encoder
                |> Encode.toYamlString 2
            let expected = vpc_no_specs_yaml_string
            Expect.trimEqual actual expected ""
    ]
    testList "decoder (fromYamlString) needs a name for every package" [
        testCase "package without a name value" <| fun _ ->
            let text = """validation_packages:
  - name:
"""
            Expect.throws (fun () -> Decode.fromYamlString ValidationPackagesConfig.decoder text |> ignore) "A package name without a value must fail."
        testCase "package with a plain null name" <| fun _ ->
            let text = """validation_packages:
  - name: null
"""
            Expect.throws (fun () -> Decode.fromYamlString ValidationPackagesConfig.decoder text |> ignore) "A plain null is no name."
        testCase "a specification that reads as a number keeps its text" <| fun _ ->
            let text = """arc_specification: 2.0
validation_packages: []
"""
            let actual = Decode.fromYamlString ValidationPackagesConfig.decoder text
            Expect.equal actual.ARCSpecification (Some "2.0") "A plain float as the specification keeps its text."
    ]
    testList "roundtrip (toYamlString >> fromYamlString)" [
        testCase "name and version" <| fun _ -> 
            let actual =
                vpc
                |> ValidationPackagesConfig.encoder
                |> Encode.toYamlString 2
                |> Decode.fromYamlString ValidationPackagesConfig.decoder
            let expected = vpc
            Expect.equal actual expected ""
        testCase "no version" <| fun _ -> 
            let actual =
                vpc_no_specs
                |> ValidationPackagesConfig.encoder
                |> Encode.toYamlString 2
                |> Decode.fromYamlString ValidationPackagesConfig.decoder
            let expected = vpc_no_specs
            Expect.equal actual expected ""
    ]
]

let main = testList "ValidationPackageConfig" [
    tests_extended
]