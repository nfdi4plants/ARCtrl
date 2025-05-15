module ProjectInfo

open Fake.Core
open Helpers

let project = "ARCtrl"

let allTestsProject = "tests/All"

/// Dotnet and JS test paths
let testProjects = 
    [
        "tests/All"
        "tests/ARCtrl"
        "tests/Contract"
        "tests/Core"
        "tests/CWL"
        "tests/FileSystem"
        "tests/Json"
        "tests/ROCrate"
        "tests/Spreadsheet"
        "tests/ValidationPackages"
        "tests/Yaml"
    ]

/// Native JS test paths
let jsTestProjects =
    [
        "tests/JavaScript"
    ]

/// Native JS test paths
let pyTestProjects =
    [
        "tests/Python"
    ]

let solutionFile  = $"{project}.sln"

let configuration = "Release"

let gitOwner = "nfdi4plants"

let gitHome = $"https://github.com/{gitOwner}"

let projectRepo = $"https://github.com/{gitOwner}/{project}"


let netPkgDir = "./dist/net"
let npmPkgDir = "./dist/ts"
let pyPkgDir = "./dist/py"

// Create RELEASE_NOTES.md if not existing. Or "release" would throw an error.
Fake.Extensions.Release.ReleaseNotes.ensure()

let mutable release = ReleaseNotes.load "RELEASE_NOTES.md"

let mutable stableVersion = SemVer.parse release.NugetVersion

let mutable stableVersionTag = (sprintf "%i.%i.%i" stableVersion.Major stableVersion.Minor stableVersion.Patch )

let loadReleaseNotes () =
    release <- ReleaseNotes.load "RELEASE_NOTES.md"
    stableVersion <- SemVer.parse release.NugetVersion
    stableVersionTag <- (sprintf "%i.%i.%i" stableVersion.Major stableVersion.Minor stableVersion.Patch )

let mutable prereleaseSuffix = PreReleaseFlag.Alpha
    //match release.SemVer.PreRelease with
    //| Some pr -> pr.PreReleaseFlag.fromInput

let mutable prereleaseSuffixNumber = 0

let mutable isPrerelease = release.SemVer.PreRelease.IsSome

do
    match release.SemVer.PreRelease with
    | Some pr ->
        let preReleaseFlag = PreReleaseFlag.fromInput pr.Name
        let preReleaseNumber =
            pr.Values
            |> Seq.pick (fun seg ->
                match seg with
                | Numeric i -> Some (int i)
                | _ -> None)
        isPrerelease <- true
        prereleaseSuffix <- preReleaseFlag
        prereleaseSuffixNumber <- preReleaseNumber
    | None ->
        isPrerelease <- false
        prereleaseSuffix <- PreReleaseFlag.Alpha
        prereleaseSuffixNumber <- 0