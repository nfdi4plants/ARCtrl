module Tests.Requirements

open ARCtrl.CWL
open ARCtrl.CWL.CWLTypes
open ARCtrl.CWL.Requirements
open YAMLicious
open TestingUtils

let decodeRequirement =
    TestObjects.CWL.Requirements.requirements
    |> Decode.read
    |> Decode.requirementsDecoder
    |> fun r -> r.Value

let testRequirement =
    testList "requirements with DockerRequirement, InitialWorkDirRequirement, EnvVarRequirement and NetworkAccess" [
        testCase "Length" <| fun _ -> Expect.isTrue (5 = decodeRequirement.Length) "Length of requirements is not 5"
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
                let expected = InitialWorkDirRequirement [|Dirent {Entryname = Some "arc"; Entry = "$(inputs.arcDirectory)"; Writable = Some true}; Dirent {Entryname = None; Entry = "$(inputs.outputDirectory)"; Writable = Some true}|]
                let actual = initialWorkDirItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "EnvVarRequirement" [
            let envVarItem = decodeRequirement.[2]
            testCase "Class" <| fun _ ->
                let expected = EnvVarRequirement [|{EnvName = "DOTNET_NOLOGO"; EnvValue = "true"}; {EnvName = "TEST"; EnvValue = "false"}|]
                let actual = envVarItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "SoftwareRequirement" [
            let softwareItem = decodeRequirement.[3]
            testCase "Class" <| fun _ ->
                let expected = SoftwareRequirement [|{Package = "interproscan"; Specs = Some [| "https://identifiers.org/rrid/RRID:SCR_005829" |]; Version = Some [| "5.21-60" |]}|]
                let actual = softwareItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
        testList "NetworkAccess" [
            let networkAccessItem = decodeRequirement.[4]
            testCase "Class" <| fun _ ->
                let expected = NetworkAccessRequirement
                let actual = networkAccessItem
                Expect.isTrue
                    (expected = actual)
                    $"Expected: {expected}\nActual: {actual}"
        ]
    ]