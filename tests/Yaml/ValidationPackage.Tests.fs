module Tests.ValidationPackage

open TestingUtils
open YAMLicious

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
    testList "decoder (fromYamlString) needs a name" [
        let decodeError (text: string) =
            try
                Decode.fromYamlString ValidationPackage.decoder text |> ignore
                None
            with YamlDecodeException error -> Some (DecodeError.pathText error, DecodeError.text error)
        testCase "name without a value" <| fun _ ->
            let yaml = """name:
version: version
"""
            let error = Expect.wantSome (decodeError yaml) "A name without a value must fail."
            Expect.equal (fst error) "name" "The error should name the field."
            Expect.stringContains (snd error) "Expected a value, found a missing value" "The error should say what it found."
        testCase "name null" <| fun _ ->
            let yaml = """name: null
"""
            let error = Expect.wantSome (decodeError yaml) "A plain null is no name."
            Expect.equal (fst error) "name" "The error should name the field."
            Expect.stringContains (snd error) "Expected a value, found the null \"null\"" "The error should say what it found."
        testCase "name tilde" <| fun _ ->
            let yaml = """name: ~
"""
            let error = Expect.wantSome (decodeError yaml) "A plain tilde is no name."
            Expect.equal (fst error) "name" "The error should name the field."
            Expect.stringContains (snd error) "Expected a value, found the null \"~\"" "The error should say what it found."
        testCase "name field absent" <| fun _ ->
            let yaml = """version: version
"""
            Expect.isSome (decodeError yaml) "A package without the field name must fail."
        testCase "quoted empty name stays valid" <| fun _ ->
            let yaml = """name: ""
"""
            let actual = Decode.fromYamlString ValidationPackage.decoder yaml
            Expect.equal actual.Name "" "A quoted empty string is a string, so it is a name."
        testCase "quoted null name stays valid" <| fun _ ->
            let yaml = """name: "null"
"""
            let actual = Decode.fromYamlString ValidationPackage.decoder yaml
            Expect.equal actual.Name "null" "The quoted text null is a string."
        testCase "version without a value is no version" <| fun _ ->
            let yaml = """name: name
version:
"""
            let actual = Decode.fromYamlString ValidationPackage.decoder yaml
            Expect.equal actual.Version None "An optional field without a value is missing."
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