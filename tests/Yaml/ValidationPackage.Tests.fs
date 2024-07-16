module Tests.ValidationPackage

open TestingUtils

open ARCtrl
open ARCtrl.ValidationPackages
open ARCtrl.Yaml

let tests_extended = testList "extended" [
    let vp = ValidationPackages.ValidationPackage("name", "version")
    let vp_no_version = ValidationPackages.ValidationPackage("name")

    let vp_yaml_string = $"""{ValidationPackage.NAME_KEY}: name
{ValidationPackage.VERSION_KEY}: version
"""

    let vp_no_version_yaml_string = $"""{ValidationPackage.NAME_KEY}: name
"""

    testList "encoder (toYamlString)" [
        testCase "name and version" <| fun _ -> 
            let actual = ValidationPackage.encoder vp |> Encode.toYamlString 2
            let expected = vp_yaml_string
            Expect.trimEqual actual expected ""
        testCase "no version" <| fun _ -> 
            let actual = ValidationPackage.encoder vp_no_version |> Encode.toYamlString 2
            let expected = vp_no_version_yaml_string
            Expect.trimEqual actual expected ""
    ]
    testList "decoder (fromYamlString)" [ 
        testCase "name and version" <| fun _ -> 
            let actual = Decode.fromYamlString ValidationPackage.decoder vp_yaml_string
            let expected = vp
            Expect.equal actual expected ""
        testCase "no version" <| fun _ -> 
            let actual = Decode.fromYamlString ValidationPackage.decoder vp_no_version_yaml_string
            let expected = vp_no_version
            Expect.equal actual expected ""
    ]
    testList "roundtrip (fromYamlString >> toYamlString)" [ 
        testCase "name and version" <| fun _ -> 
            let actual =
                vp_yaml_string
                |> Decode.fromYamlString ValidationPackage.decoder
                |> ValidationPackage.encoder
                |> Encode.toYamlString 2
            let expected = vp_yaml_string
            Expect.trimEqual actual expected ""
        testCase "no version" <| fun _ -> 
            let actual =
                vp_no_version_yaml_string
                |> Decode.fromYamlString ValidationPackage.decoder
                |> ValidationPackage.encoder
                |> Encode.toYamlString 2
            let expected = vp_no_version_yaml_string
            Expect.trimEqual actual expected ""
    ]
    testList "roundtrip (toYamlString >> fromYamlString)" [ 
        testCase "name and version" <| fun _ -> 
            let actual =
                vp
                |> ValidationPackage.encoder
                |> Encode.toYamlString 2
                |> Decode.fromYamlString ValidationPackage.decoder
            let expected = vp
            Expect.equal actual expected ""
        testCase "no version" <| fun _ -> 
            let actual =
                vp_no_version
                |> ValidationPackage.encoder
                |> Encode.toYamlString 2
                |> Decode.fromYamlString ValidationPackage.decoder
            let expected = vp_no_version
            Expect.equal actual expected ""
    ]
]

let main = testList "ValidationPackage" [
    tests_extended
]