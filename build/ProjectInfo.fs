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

type VersionController() =

    let mutable release = ReleaseNotes.load "RELEASE_NOTES.md"

    let mutable isPrerelease = false
    let mutable prereleaseSuffix = PreReleaseFlag.Alpha
    let mutable prereleaseSuffixNumber = 0

    let refreshReleaseNotes() =
        match release.SemVer.PreRelease with
        | Some pr ->
            isPrerelease <- true
            prereleaseSuffix <- PreReleaseFlag.fromInput pr.Name
            prereleaseSuffixNumber <-
                pr.Values
                |> Seq.pick (fun seg ->
                    match seg with
                    | Numeric i -> Some (int i)
                    | _ -> None)
        | None ->
            isPrerelease <- false
            prereleaseSuffix <- PreReleaseFlag.Alpha
            prereleaseSuffixNumber <- 0

    do 
        refreshReleaseNotes()

    member this.StableVersion = SemVer.parse release.NugetVersion

    member this.StableVersionTag =
        let stableVersion = SemVer.parse release.NugetVersion
        (sprintf "%i.%i.%i" stableVersion.Major stableVersion.Minor stableVersion.Patch)

    member this.IsPrerelease = isPrerelease
    member this.PrereleaseSuffix = prereleaseSuffix
    member this.PrereleaseSuffixNumber = prereleaseSuffixNumber

    member this.NugetTag =
        if isPrerelease then
            PreReleaseFlag.toNugetTag release.SemVer prereleaseSuffix prereleaseSuffixNumber
        else
            this.StableVersionTag

    member this.NPMTag =
        if isPrerelease then
            PreReleaseFlag.toNPMTag release.SemVer prereleaseSuffix prereleaseSuffixNumber
        else
            this.StableVersionTag

    member this.PyTag =
        if isPrerelease then
            PreReleaseFlag.toPyPITag release.SemVer prereleaseSuffix prereleaseSuffixNumber
        else
            this.StableVersionTag

    member this.Notes = release.Notes

    member this.Refresh() =
        release <- ReleaseNotes.load "RELEASE_NOTES.md"
        refreshReleaseNotes()

let versionController = VersionController()