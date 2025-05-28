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


// https://github.com/Freymaurer/Fake.Extensions.Release#releaseupdate
let updateReleaseNotes = BuildTask.createFn "ReleaseNotes" [] (fun config ->
    printfn "Start releasenotes"
    if config.Context.Arguments |> List.exists (fun s -> s.StartsWith("pre:")) then
        failwith "Pre-release versioning using `pre` is not supported. Please use the 'ReleaseNotesPre' task instead."
    ReleaseNotes.update(ProjectInfo.gitOwner, ProjectInfo.project, config)
    versionController.Refresh()

    Trace.trace "Start updating package.json version"
    Fake.JavaScript.Npm.exec $"version {versionController.NPMTag} --no-git-tag-version" id
    Trace.trace "Finish updating package.json version"

    Trace.trace "Start updating pyproject.toml version"   
    run python $"-m uv version {versionController.PyTag}" "."
    Trace.trace "Finish updating pyproject.toml version"

    printfn "updated version to %s" versionController.NugetTag
)

// https://github.com/Freymaurer/Fake.Extensions.Release#releaseupdate
let updatePreReleaseNotes = BuildTask.createFn "PreReleaseNotes" [] (fun config ->
    printfn "Start pre releasenotes"
    if config.Context.Arguments |> List.exists (fun s -> s.StartsWith("pre:")) then
        failwith "Pre-release versioning using `pre` is not supported. Instead follow user input questions."
    let suffixTag, suffixNumber = setPrereleaseTag()
    let suffix = PreReleaseFlag.toNugetSuffix suffixTag suffixNumber
    let arguments = config.Context.Arguments @ ["pre:" + suffix]
    let config = {config with Context = {config.Context with Arguments = arguments}}
    ReleaseNotes.update(ProjectInfo.gitOwner, ProjectInfo.project, config)
    versionController.Refresh()

    Trace.trace "Start updating package.json version"
    Fake.JavaScript.Npm.exec $"version {versionController.NPMTag} --no-git-tag-version" id
    Trace.trace "Finish updating package.json version"

    Trace.trace "Start updating pyproject.toml version"   
    run python $"-m uv version {versionController.PyTag}" "."
    Trace.trace "Finish updating pyproject.toml version"

    printfn "updated version to %s" versionController.NugetTag
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