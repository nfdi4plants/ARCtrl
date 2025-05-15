module ReleaseNotesTasks

open Fake.Extensions.Release
open BlackFox.Fake
open Fake.Core
open BasicTasks.Helper
open BasicTasks
open Helpers
open ProjectInfo

/// This might not be necessary, mostly useful for apps which want to display current version as it creates an accessible F# version script from RELEASE_NOTES.md
let createAssemblyVersion = BuildTask.create "createvfs" [] {
    AssemblyVersion.create ProjectInfo.project
}


let bumpNPM (preRelease : bool) =
    
    Trace.trace "Start updating package.json version"
    if preRelease then
        let prereleaseTag = PreReleaseFlag.toNPMTag release.SemVer prereleaseSuffix prereleaseSuffixNumber
        Fake.JavaScript.Npm.exec $"version {prereleaseTag} --no-git-tag-version" id
    else
        Fake.JavaScript.Npm.exec $"version {stableVersionTag} --no-git-tag-version" id
    Trace.trace "Finish updating package.json version"

let bumpPy (preRelease : bool) =

    Trace.trace "Start updating pyproject.toml version"   
    if preRelease then
        let prereleaseTag = PreReleaseFlag.toPyPITag release.SemVer prereleaseSuffix prereleaseSuffixNumber
        run python $"-m poetry version {prereleaseTag}" "."
    else
        run python $"-m poetry version {stableVersionTag}" "."
    Trace.trace "Finish updating pyproject.toml version"


// https://github.com/Freymaurer/Fake.Extensions.Release#releaseupdate
let updateReleaseNotes = BuildTask.createFn "ReleaseNotes" [] (fun config ->
    printfn "Start releasenotes"
    if config.Context.Arguments |> List.exists (fun s -> s.StartsWith("pre:")) then
        failwith "Pre-release versioning using `pre` is not supported. Please use the 'ReleaseNotesPre' task instead."
    ReleaseNotes.update(ProjectInfo.gitOwner, ProjectInfo.project, config)
    loadReleaseNotes()
    bumpPy false
    bumpNPM false

    printfn "updated version to %s" stableVersionTag
)

// https://github.com/Freymaurer/Fake.Extensions.Release#releaseupdate
let updatePreReleaseNotes = BuildTask.createFn "PreReleaseNotes" [] (fun config ->
    printfn "Start pre releasenotes"
    if config.Context.Arguments |> List.exists (fun s -> s.StartsWith("pre:")) then
        failwith "Pre-release versioning using `pre` is not supported. Instead follow user input questions."
    setPrereleaseTag()
    let suffix = PreReleaseFlag.toNugetSuffix prereleaseSuffix prereleaseSuffixNumber
    let arguments = config.Context.Arguments @ ["pre:" + suffix]
    let config = {config with Context = {config.Context with Arguments = arguments}}
    ReleaseNotes.update(ProjectInfo.gitOwner, ProjectInfo.project, config)
    loadReleaseNotes()
    bumpPy true
    bumpNPM true
    let semVer = 
        Fake.Core.ReleaseNotes.load "RELEASE_NOTES.md"
        |> fun x -> x.SemVer.AsString

    printfn "updated version to %s" semVer
)

// https://github.com/Freymaurer/Fake.Extensions.Release#githubdraft
let githubDraft = BuildTask.createFn "GithubDraft" [] (fun config ->

    let body = "We are ready to go for the first release!"

    Github.draft(
        ProjectInfo.gitOwner,
        ProjectInfo.project,
        (Some body),
        None,
        config
    )
)