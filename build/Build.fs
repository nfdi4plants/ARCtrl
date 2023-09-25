module Build
open BlackFox.Fake
open System.IO
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Tools

open Helpers

initializeContext()

open BasicTasks
open TestTasks
open PackageTasks
open ReleaseTasks

/// Full release of nuget package for the prerelease version.
let _release = 
    BuildTask.createEmpty 
        "Release" 
        [clean; build; runTests; pack; createTag; publishNuget; publishNPM]

/// Full release of nuget package for the prerelease version.
let _preRelease = 
    BuildTask.createEmpty 
        "PreRelease" 
        [setPrereleaseTag; clean; build; runTests; packPrerelease; createPrereleaseTag; publishNugetPrerelease; publishNPMPrerelease]

ReleaseNotesTasks.updateReleaseNotes |> ignore

[<EntryPoint>]
let main args = 
    runOrDefault build args
