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

let jsHelperFileName = "FileSystem.js"
let jsHelperFilePath = "src/ARCtrl/ContractIO/" + jsHelperFileName

let solutionFile  = $"{project}.sln"

let configuration = "Release"

let gitOwner = "nfdi4plants"

let gitHome = $"https://github.com/{gitOwner}"

let projectRepo = $"https://github.com/{gitOwner}/{project}"


let netPkgDir = "./dist/net"
let npmPkgDir = "./dist/js"
let pyPkgDir = "./dist/py"

// Create RELEASE_NOTES.md if not existing. Or "release" would throw an error.
Fake.Extensions.Release.ReleaseNotes.ensure()

let release = ReleaseNotes.load "RELEASE_NOTES.md"

let stableVersion = SemVer.parse release.NugetVersion

let stableVersionTag = (sprintf "%i.%i.%i" stableVersion.Major stableVersion.Minor stableVersion.Patch )

let mutable prereleaseSuffix = PreReleaseFlag.Alpha

let mutable prereleaseSuffixNumber = 0

let mutable isPrerelease = false