module Tests.Requirements

open ARCtrl.CWL
open ARCtrl.CWL.CWLTypes
open ARCtrl.CWL.Requirements
open ARCtrl.CWL.Inputs
open ARCtrl.CWL.Outputs
open YAMLicious
open TestingUtils

let decodeRequirement =
    TestObjects.CWL.Requirements.requirements
    |> Decode.read
    |> Decode.requirementsDecoder
    |> fun r -> r.Value

let testRequirement =
    testList "requirements with DockerRequirement, InitialWorkDirRequirement, EnvVarRequirement and NetworkAccess" [
        testCase "Length" <| fun _ -> Expect.isTrue (4 = decodeRequirement.Length) "Length of requirements is not 4"
        testList "DockerRequirement" [
            let dockerItem = decodeRequirement.[0]
            testCase "Class" <| fun _ ->
                let expected = DockerRequirement {DockerPull = None; DockerFile = Some (Map [("$include", "FSharpArcCapsule/Dockerfile")]); DockerImageId = Some "devcontainer"}
                let actual = dockerItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "InitialWorkDirRequirement" [
            let initialWorkDirItem = decodeRequirement.[1]
            testCase "Class" <| fun _ ->
                let expected = InitialWorkDirRequirement [||]
                let actual = initialWorkDirItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "EnvVarRequirement" [
            let envVarItem = decodeRequirement.[2]
            testCase "Class" <| fun _ ->
                let expected = EnvVarRequirement {EnvName = ""; EnvValue = ""}
                let actual = envVarItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "NetworkAccess" [
            let networkAccessItem = decodeRequirement.[3]
            testCase "Class" <| fun _ ->
                let expected = NetworkAccessRequirement
                let actual = networkAccessItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
    ]