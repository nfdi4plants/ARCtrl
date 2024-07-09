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

    let vpc_yaml_string = """arc_specification: arc_specification
validation_packages:
  -
    name: name
    version: version
  -
    name: name
"""                                 .ReplaceLineEndings(System.Environment.NewLine)

    let vpc_no_specs_yaml_string = """validation_packages:
  -
    name: name
    version: version
  -
    name: name
"""                                 .ReplaceLineEndings(System.Environment.NewLine)

    testList "encoder (toYamlString)" [
        testCase "no specification validation" <| fun _ -> 
            let actual = ValidationPackagesConfig.encoder vpc |> Encode.toYamlString 2
            let expected = vpc_yaml_string
            Expect.equal actual expected ""
        testCase "with specification validation" <| fun _ -> 
            let actual = ValidationPackagesConfig.encoder vpc_no_specs |> Encode.toYamlString 2
            let expected = vpc_no_specs_yaml_string
            Expect.equal actual expected ""
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
            Expect.equal actual expected ""
        testCase "no version" <| fun _ -> 
            let actual =
                vpc_no_specs_yaml_string
                |> Decode.fromYamlString ValidationPackagesConfig.decoder
                |> ValidationPackagesConfig.encoder
                |> Encode.toYamlString 2
            let expected = vpc_no_specs_yaml_string
            Expect.equal actual expected ""
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