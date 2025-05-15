module PackageTasks

open MessagePrompts
open BasicTasks
open TestTasks
open ProjectInfo
open Helpers

open BlackFox.Fake
open Fake.Core
open Fake.IO.Globbing.Operators

open System.Text.RegularExpressions

/// https://github.com/Freymaurer/Fake.Extensions.Release#release-notes-in-nuget
let private replaceCommitLink input = 
    let commitLinkPattern = @"\[\[#[a-z0-9]*\]\(.*\)\] "
    Regex.Replace(input,commitLinkPattern,"")

module BundleDotNet =
    let bundle (versionTag : string) (versionSuffix : string option) =
        System.IO.Directory.CreateDirectory(ProjectInfo.netPkgDir) |> ignore
        !! "src/**/*.fsproj"
        -- "src/bin/*"
        |> Seq.iter (Fake.DotNet.DotNet.pack (fun p ->           
            let msBuildParams =
                {p.MSBuildParams with 
                    Properties = ([
                        "Version",versionTag
                        "PackageReleaseNotes",  (ProjectInfo.release.Notes |> List.map replaceCommitLink |> String.toLines )
                    ] @ p.MSBuildParams.Properties)
                    DisableInternalBinLog = true
                }
            {
                p with 
                    VersionSuffix = versionSuffix
                    MSBuildParams = msBuildParams
                    OutputPath = Some ProjectInfo.netPkgDir
            }
        ))

let packDotNet = BuildTask.create "PackDotNet" [clean; build; (*runTests*)] {
    BundleDotNet.bundle ProjectInfo.stableVersionTag None
}

let packDotNetPrerelease = BuildTask.create "PackDotNetPrerelease" [clean; build] {
    let prereleaseTag = PreReleaseFlag.toNugetTag release.SemVer prereleaseSuffix prereleaseSuffixNumber
    BundleDotNet.bundle prereleaseTag (Some prereleaseTag)
}

let packJS = BuildTask.create "PackJS" [clean; build; transpileTS] {
    Fake.JavaScript.Npm.exec "run build" id 
    Fake.JavaScript.Npm.exec $"pack --pack-destination {ProjectInfo.npmPkgDir}" id 
}

let packPy = BuildTask.create "PackPy" [clean; build; transpilePy] {
    run python "-m poetry install --no-root" ProjectInfo.pyPkgDir
    run python "-m poetry build" "."
}

let pack = BuildTask.createEmpty "Pack" [packDotNet; packJS; packPy]