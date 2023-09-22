module ReleaseTasks

open MessagePrompts
open ProjectInfo
open BasicTasks
open TestTasks
open PackageTasks

open BlackFox.Fake
open Fake.Core
open Fake.DotNet
open Fake.Api
open Fake.Tools
open Fake.IO
open Fake.IO.Globbing.Operators

let createTag = BuildTask.create "CreateTag" [clean; build; runTests; pack] {
    if promptYesNo (sprintf "tagging branch with %s OK?" stableVersionTag ) then
        Git.Branches.tag "" stableVersionTag
        Git.Branches.pushTag "" projectRepo stableVersionTag
    else
        failwith "aborted"
}

let createPrereleaseTag = BuildTask.create "CreatePrereleaseTag" [setPrereleaseTag; clean; build; runTests; packPrerelease] {
    if promptYesNo (sprintf "tagging branch with %s OK?" prereleaseTag ) then 
        Git.Branches.tag "" prereleaseTag
        Git.Branches.pushTag "" projectRepo prereleaseTag
    else
        failwith "aborted"
}


let publishNuget = BuildTask.create "PublishNuget" [clean; build; runTests; pack] {
    let targets = (!! (sprintf "%s/*.*pkg" pkgDir ))
    for target in targets do printfn "%A" target
    let msg = sprintf "release package with version %s?" stableVersionTag
    if promptYesNo msg then
        let source = "https://api.nuget.org/v3/index.json"
        let apikey =  Environment.environVar "NUGET_KEY"
        for artifact in targets do
            let result = DotNet.exec id "nuget" (sprintf "push -s %s -k %s %s --skip-duplicate" source apikey artifact)
            if not result.OK then failwith "failed to push packages"
    else failwith "aborted"
}

let publishNugetPrerelease = BuildTask.create "PublishNugetPrerelease" [clean; build; runTests; packPrerelease] {
    let targets = (!! (sprintf "%s/*.*pkg" pkgDir ))
    for target in targets do printfn "%A" target
    let msg = sprintf "release package with version %s?" prereleaseTag 
    if promptYesNo msg then
        let source = "https://api.nuget.org/v3/index.json"
        let apikey =  Environment.environVar "NUGET_KEY"
        for artifact in targets do
            let result = DotNet.exec id "nuget" (sprintf "push -s %s -k %s %s --skip-duplicate" source apikey artifact)
            if not result.OK then failwith "failed to push packages"
    else failwith "aborted"
}

let publishNPM = BuildTask.create "PublishNPM" [clean; build; runTests; packJS] {
    let target = 
        (!! (sprintf "%s/*.tgz" npmPkgDir ))
        |> Seq.head
    printfn "%A" target
    let msg = sprintf "release package with version %s?" stableVersionTag
    if promptYesNo msg then
        let apikey = Environment.environVarOrNone "NPM_KEY" 
        let otp = if apikey.IsSome then $" --otp + {apikey.Value}" else ""
        Fake.JavaScript.Npm.exec $"publish {target} --access public{otp}" (fun o ->
            { o with
                WorkingDirectory = "./dist/js/"
            })        
    else failwith "aborted"
}

let publishNPMPrerelease = BuildTask.create "PublishNPMPrerelease" [clean; build; (*runTests*) packJSPrerelease] {
    let target = 
        (!! (sprintf "%s/*.tgz" npmPkgDir ))
        |> Seq.head
    printfn "%A" target
    let msg = sprintf "release package with version %s?" prereleaseTag 
    if promptYesNo msg then
        let apikey =  Environment.environVarOrNone "NPM_KEY"    
        let otp = if apikey.IsSome then $" --otp {apikey.Value}" else ""
        Fake.JavaScript.Npm.exec $"publish {target} --access public --tag next{otp}" (fun o ->
            { o with
                WorkingDirectory = "./dist/js/"
            })      
    else failwith "aborted"
}
