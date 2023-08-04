﻿module BasicTasks

open BlackFox.Fake
open Fake.IO
open Fake.DotNet
open Fake.IO.Globbing.Operators

open ProjectInfo

let setPrereleaseTag = BuildTask.create "SetPrereleaseTag" [] {
    printfn "Please enter pre-release package suffix"
    let suffix = System.Console.ReadLine()
    prereleaseSuffix <- suffix
    prereleaseTag <- (sprintf "%i.%i.%i-%s" release.SemVer.Major release.SemVer.Minor release.SemVer.Patch suffix)
    isPrerelease <- true
}

let clean = BuildTask.create "Clean" [] {
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "tests/**/bin"
    ++ "tests/**/obj"
    ++ ProjectInfo.pkgDir
    |> Shell.cleanDirs 
}

let build = BuildTask.create "Build" [clean] {
    solutionFile
    |> DotNet.build id
}