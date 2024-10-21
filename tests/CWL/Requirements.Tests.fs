module Tests.Requirements

open ARCtrl.CWL
open YAMLicious
open TestingUtils

let decodeRequirement =
    TestObjects.CWL.Requirements.requirementsFileContent
    |> Decode.read
    |> Decode.requirementsDecoder
    |> fun r -> r.Value

let testRequirement =
    testList "Decode" [
        testCase "Length" <| fun _ -> Expect.equal 5  decodeRequirement.Count ""
        testList "DockerRequirement" [
            let dockerItem = decodeRequirement.[0]
            testCase "Class" <| fun _ ->
                let expected = DockerRequirement {DockerPull = None; DockerFile = Some (Map [("$include", "FSharpArcCapsule/Dockerfile")]); DockerImageId = Some "devcontainer"}
                let actual = dockerItem
                Expect.equal actual expected ""
        ]
        testList "InitialWorkDirRequirement" [
            let initialWorkDirItem = decodeRequirement.[1]
            testCase "Class" <| fun _ ->
                let expected = InitialWorkDirRequirement (ResizeArray [|Dirent {Entryname = Some "arc"; Entry = "$(inputs.arcDirectory)"; Writable = Some true}; Dirent {Entryname = None; Entry = "$(inputs.outputDirectory)"; Writable = Some true}|])
                let actual = initialWorkDirItem
                match actual, expected with
                | InitialWorkDirRequirement actualType, InitialWorkDirRequirement expectedType ->
                    Expect.sequenceEqual actualType expectedType ""
                | _ -> failwith "This test case can only be InitialWorkDirRequirement"
        ]
        testList "EnvVarRequirement" [
            let envVarItem = decodeRequirement.[2]
            testCase "Class" <| fun _ ->
                let expected = EnvVarRequirement (ResizeArray [|{EnvName = "DOTNET_NOLOGO"; EnvValue = "true"}; {EnvName = "TEST"; EnvValue = "false"}|])
                let actual = envVarItem
                match actual, expected with
                | EnvVarRequirement actualType, EnvVarRequirement expectedType ->
                    Expect.sequenceEqual actualType expectedType ""
                | _ -> failwith "This test case can only be EnvVarRequirement"
        ]
        testList "SoftwareRequirement" [
            let softwareItem = decodeRequirement.[3]
            testCase "Class" <| fun _ ->
                let expected = SoftwareRequirement (ResizeArray [|{Package = "interproscan"; Specs = Some (ResizeArray [| "https://identifiers.org/rrid/RRID:SCR_005829" |]); Version = Some (ResizeArray[| "5.21-60" |])}|])
                let actual = softwareItem
                match actual, expected with
                | SoftwareRequirement actualType, SoftwareRequirement expectedType ->
                    Expect.equal actualType.[0].Package expectedType.[0].Package ""
                    Expect.sequenceEqual actualType.[0].Specs.Value expectedType.[0].Specs.Value ""
                    Expect.sequenceEqual actualType.[0].Version.Value expectedType.[0].Version.Value ""
                | _ -> failwith "This test case can only be SoftwareRequirement"
        ]
        testList "NetworkAccess" [
            let networkAccessItem = decodeRequirement.[4]
            testCase "Class" <| fun _ ->
                let expected = NetworkAccessRequirement
                let actual = networkAccessItem
                Expect.equal actual expected ""
        ]
    ]

let main = 
    testList "Requirement" [
        testRequirement
    ]