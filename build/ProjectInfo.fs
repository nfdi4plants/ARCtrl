module ProjectInfo

open Fake.Core

let project = "ARCtrl"

/// Dotnet and JS test paths
let testProjects = 
    [
        "tests/ISA/ISA.Tests"
        "tests/ISA/ISA.Json.Tests"
        "tests/ISA/ISA.Spreadsheet.Tests"
        "tests/FileSystem"
        "tests/ARCtrl"
    ]

/// Native JS test paths
let jsTestProjects =
    [
        "tests/JavaScript"
    ]

let solutionFile  = $"{project}.sln"

let configuration = "Release"

let gitOwner = "nfdi4plants"

let gitHome = $"https://github.com/{gitOwner}"

let projectRepo = $"https://github.com/{gitOwner}/{project}"

let pkgDir = "dist/pkg"

// Create RELEASE_NOTES.md if not existing. Or "release" would throw an error.
Fake.Extensions.Release.ReleaseNotes.ensure()

let release = ReleaseNotes.load "RELEASE_NOTES.md"

let stableVersion = SemVer.parse release.NugetVersion

let stableVersionTag = (sprintf "%i.%i.%i" stableVersion.Major stableVersion.Minor stableVersion.Patch )

let mutable prereleaseSuffix = ""

let mutable prereleaseTag = ""

let mutable isPrerelease = false