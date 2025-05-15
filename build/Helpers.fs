module Helpers

open BlackFox.Fake
open Fake.Core
open Fake.DotNet

let initializeContext () =
    let execContext = Context.FakeExecutionContext.Create false "build.fsx" [ ]
    Context.setExecutionContext (Context.RuntimeContext.Fake execContext)

/// Executes a dotnet command in the given working directory
let runDotNet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

let runOrDefault defaultTarget args =
    Trace.trace (sprintf "%A" args)
    try
        match args with
        | [| target |] -> Target.runOrDefault target
        | arr when args.Length > 1 ->
            Target.run 0 (Array.head arr) ( Array.tail arr |> List.ofArray )
        | _ -> BuildTask.runOrDefault defaultTarget
        0
    with e ->
        printfn "%A" e
        1

type PreReleaseFlag = 
    | Alpha
    | Beta
    | ReleaseCandidate
    | Swate of PreReleaseFlag // this was added to create swate release hotfixes

    static member fromInput (input: string) =
        match input with
        | "a" | "alpha" -> Alpha
        | "b" | "beta" -> Beta
        | "rc" -> ReleaseCandidate
        | any when any.StartsWith "swate" ->
            let rmvdSwate = any.Replace("swate.","")
            Swate (PreReleaseFlag.fromInput rmvdSwate)
        | _ -> failwith "Invalid input"

    static member toNugetSuffix (flag: PreReleaseFlag) (number : int) =
        let rec mkSuffix (flag) (number) = 
            match flag with
            | Alpha -> $"alpha.{number}"
            | Beta -> $"beta.{number}"
            | ReleaseCandidate -> $"rc.{number}"
            | Swate any -> mkSuffix any number + ".swate"
        mkSuffix flag number

    static member toNugetTag (semVer : SemVerInfo) (flag: PreReleaseFlag) (number : int) =
        let suffix = PreReleaseFlag.toNugetSuffix flag number
        sprintf "%i.%i.%i-%s" semVer.Major semVer.Minor semVer.Patch suffix


    static member toNPMTag (semVer : SemVerInfo) (flag: PreReleaseFlag) (number : int) =
        PreReleaseFlag.toNugetTag semVer flag number

    static member toPyPITag (semVer : SemVerInfo) (tag: PreReleaseFlag) (number : int) =
        let suffix = 
            match tag with
            | Alpha -> $"a{number}"
            | Beta -> $"b{number}"
            | ReleaseCandidate -> $"rc{number}"
            | Swate any -> failwith "Cannot publish swate prerelease to pypi."
        sprintf "%i.%i.%i%s" semVer.Major semVer.Minor semVer.Patch suffix

