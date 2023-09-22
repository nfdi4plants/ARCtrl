module PackageTasks

open ProjectInfo

open MessagePrompts
open BasicTasks
open TestTasks

open BlackFox.Fake
open Fake.Core
open Fake.IO.Globbing.Operators

open System.Text.RegularExpressions

/// https://github.com/Freymaurer/Fake.Extensions.Release#release-notes-in-nuget
let private replaceCommitLink input = 
    let commitLinkPattern = @"\[\[#[a-z0-9]*\]\(.*\)\] "
    Regex.Replace(input,commitLinkPattern,"")

let packDotNet = BuildTask.create "PackDotNet" [clean; build; runTests] {
    if promptYesNo (sprintf "creating stable package with version %s OK?" stableVersionTag ) 
        then
            !! "src/**/*.*proj"
            -- "src/bin/*"
            |> Seq.iter (Fake.DotNet.DotNet.pack (fun p ->
                let msBuildParams =
                    {p.MSBuildParams with 
                        Properties = ([
                            "Version",stableVersionTag
                            "PackageReleaseNotes",  (release.Notes |> List.map replaceCommitLink |> String.concat "\r\n" )
                        ] @ p.MSBuildParams.Properties)
                    }
                {
                    p with 
                        MSBuildParams = msBuildParams
                        OutputPath = Some pkgDir
                }
            ))
    else failwith "aborted"
}

let packDotNetPrerelease = BuildTask.create "PackDotNetPrerelease" [setPrereleaseTag; clean; build; (*runTests*)] {
    if promptYesNo (sprintf "package tag will be %s OK?" prereleaseTag )
        then 
            !! "src/**/*.*proj"
            -- "src/bin/*"
            |> Seq.iter (Fake.DotNet.DotNet.pack (fun p ->
                        let msBuildParams =
                            {p.MSBuildParams with 
                                Properties = ([
                                    "Version", prereleaseTag
                                    "PackageReleaseNotes",  (release.Notes |> List.map replaceCommitLink  |> String.toLines )
                                ] @ p.MSBuildParams.Properties)
                            }
                        {
                            p with 
                                VersionSuffix = Some prereleaseSuffix
                                OutputPath = Some pkgDir
                                MSBuildParams = msBuildParams
                        }
            ))
    else
        failwith "aborted"
}

let packJS = BuildTask.create "PackJS" [clean; build; runTests] {
    if promptYesNo (sprintf "creating stable package with version %s OK?" stableVersionTag ) 
        then
            Fake.JavaScript.Npm.run "bundlejs" (fun o -> o)
            Fake.IO.File.readAsString "build/release_package.json"
            |> Fake.IO.File.writeString false "dist/js/package.json"

            Fake.IO.File.readAsString "README.md"
            |> Fake.IO.File.writeString false "dist/js/README.md"

            "" // "fable-library.**/**"
            |> Fake.IO.File.writeString false "dist/js/fable_modules/.npmignore"

            Fake.JavaScript.Npm.exec "pack" (fun o ->
                { o with
                    WorkingDirectory = "./dist/js/"
                })
    else failwith "aborted"
}


let packJSPrerelease = BuildTask.create "PackJSPrerelease" [setPrereleaseTag; clean; build; runTests] {
    if promptYesNo (sprintf "package tag will be %s OK?" prereleaseTag ) then
        Fake.JavaScript.Npm.run "bundlejs" (fun o -> o)

        Fake.IO.File.readAsString "build/release_package.json"
        |> fun t ->
            let t = t.Replace(stableVersionTag, prereleaseTag)
            Fake.IO.File.writeString false "dist/js/package.json" t

        Fake.IO.File.readAsString "README.md"
        |> Fake.IO.File.writeString false "dist/js/README.md"

        "" // "fable-library.**/**"
        |> Fake.IO.File.writeString false "dist/js/fable_modules/.npmignore"

        Fake.JavaScript.Npm.exec "pack" (fun o ->
            { o with
                WorkingDirectory = "./dist/js/"
            })
    else failwith "aborted"
    }

let pack = BuildTask.createEmpty "Pack" [packDotNet; packJS]

let packPrerelease = BuildTask.createEmpty "PackPrerelease" [packDotNetPrerelease;packJSPrerelease]