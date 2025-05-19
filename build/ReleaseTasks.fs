module ReleaseTasks

open MessagePrompts
open ProjectInfo
open BasicTasks
open TestTasks
open PackageTasks
open Helpers 

open BlackFox.Fake
open Fake.Core
open Fake.DotNet
open Fake.Api
open Fake.Tools
open Fake.IO
open Fake.IO.Globbing.Operators

let createTag = BuildTask.create "CreateTag" [clean; build; runTests] {
    let tag = versionController.NugetTag
    if promptYesNo (sprintf "tagging branch with %s OK?" tag ) then
        Git.Branches.tag "" tag
        Git.Branches.pushTag "" projectRepo tag
    else
        failwith "aborted"
}

let publishNuget = BuildTask.create "PublishNuget" [clean; build; runTests; packDotNet] {
    let targets = (!! (sprintf "%s/*.*pkg" netPkgDir ))
    for target in targets do printfn "%A" target
    
    let tag = versionController.NugetTag
    let msg = sprintf "[NUGET] release package with version %s?" tag 
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
    let tag = versionController.NPMTag
    let msg = sprintf "[NPM] release package with version %s?" tag 
    if promptYesNo msg then
        let apikey =  Environment.environVarOrNone "NPM_KEY"    
        let otp = if apikey.IsSome then $" --otp {apikey.Value}" else ""
        let tag = if versionController.IsPrerelease then $" --tag next" else ""
        Fake.JavaScript.Npm.exec $"publish {target} --access public {tag}{otp}" (fun o ->
            { o with
                WorkingDirectory = "./dist/ts/"
            })      
    else failwith "aborted"
}

let publishPyPi = BuildTask.create "PublishPyPi" [clean; build; runTests; packPy] {
    let tag = versionController.PyTag
    let msg = sprintf "[PyPi] release package with version %s?" tag
    if promptYesNo msg then
        let apikey = Environment.environVarOrNone "PYPI_KEY"
        let token = 
            match apikey with
            | Some key -> 
                 $"--username __token__ --password {key}"
            | None -> ""
        run python $"-m twine upload {token} {pyPkgDir}/*" "."
    else failwith "aborted"
}