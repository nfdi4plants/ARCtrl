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
        !! "src/**/*.*proj"
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

let packDotNet = BuildTask.create "PackDotNet" [clean; build; runTests] {
    BundleDotNet.bundle ProjectInfo.stableVersionTag None
}

let packDotNetPrerelease = BuildTask.create "PackDotNetPrerelease" [setPrereleaseTag; clean; build] {
    let prereleaseTag = PreReleaseFlag.toNugetTag release.SemVer prereleaseSuffix prereleaseSuffixNumber
    BundleDotNet.bundle prereleaseTag (Some prereleaseTag)
}

let packDotNetSwate = BuildTask.create "packDotNetSwate" [clean; build; RunTests.runTestsDotnet; RunTests.runTestsJs; RunTests.runTestsJsNative] {
    let semVer = release.SemVer
    let releaseTag = sprintf "%i.%i.%i-swate" semVer.Major semVer.Minor semVer.Patch
    BundleDotNet.bundle releaseTag (Some releaseTag)
}

module BundleJs =
    let bundle () =

        Fake.IO.File.readAsString "README.md"
        |> Fake.IO.File.writeString false $"{ProjectInfo.npmPkgDir}/README.md"

        "" // "fable-library.**/**"
        |> Fake.IO.File.writeString false $"{ProjectInfo.npmPkgDir}/fable_modules/.npmignore"

        Fake.JavaScript.Npm.exec "build" id 

let packJS = BuildTask.create "PackJS" [clean; build; transpileTS; runTests] {
    BundleJs.bundle ()
}

let packJSPrerelease = BuildTask.create "PackJSPrerelease" [setPrereleaseTag; clean; build; transpileTS; runTests] {
    let prereleaseTag = PreReleaseFlag.toNPMTag release.SemVer prereleaseSuffix prereleaseSuffixNumber
    Fake.JavaScript.Npm.exec $"version {prereleaseTag} --no-git-tag-version"  id 
    BundleJs.bundle ()
}

module BundlePy =
    let bundle (versionTag: string) =
        
        run dotnet $"fable src/ARCtrl/ARCtrl.Python.fsproj -o {ProjectInfo.pyPkgDir}/arctrl --lang python" ""
        run python "-m poetry install --no-root" ProjectInfo.pyPkgDir
        GenerateIndexPy.ARCtrl_generate (ProjectInfo.pyPkgDir + "/arctrl")

        Fake.IO.File.readAsString "pyproject.toml"
        |> fun t ->
            let t = t.Replace(ProjectInfo.stableVersionTag, versionTag)
            Fake.IO.File.writeString false $"{ProjectInfo.pyPkgDir}/pyproject.toml" t

        Fake.IO.File.readAsString "README.md"
        |> Fake.IO.File.writeString false $"{ProjectInfo.pyPkgDir}/README.md"

        //"" // "fable-library.**/**"
        //|> Fake.IO.File.writeString false $"{ProjectInfo.npmPkgDir}/fable_modules/.npmignore"

        run python "-m poetry build" ProjectInfo.pyPkgDir //Remove "-o ." because not compatible with publish 


let packPy = BuildTask.create "PackPy" [clean; build; runTests] {
    BundlePy.bundle ProjectInfo.stableVersionTag

}

let packPyPrerelease = BuildTask.create "PackPyPrerelease" [setPrereleaseTag; clean; build; runTests] {
    let prereleaseTag = PreReleaseFlag.toPyPITag release.SemVer prereleaseSuffix prereleaseSuffixNumber
    BundlePy.bundle prereleaseTag
    }


let pack = BuildTask.createEmpty "Pack" [packDotNet; packJS; packPy]

let packPrerelease = BuildTask.createEmpty "PackPrerelease" [packDotNetPrerelease;packJSPrerelease;packPyPrerelease]