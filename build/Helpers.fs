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

    static member fromInput (input: string) =
        match input with
        | "a" -> Alpha
        | "b" -> Beta
        | "rc" -> ReleaseCandidate
        | _ -> failwith "Invalid input"

    static member toNugetTag (semVer : SemVerInfo) (flag: PreReleaseFlag) (number : int) =
        let suffix = 
            match flag with
            | Alpha -> $"alpha.{number}"
            | Beta -> $"beta.{number}"
            | ReleaseCandidate -> $"rc.{number}"
        sprintf "%i.%i.%i-%s" semVer.Major semVer.Minor semVer.Patch suffix


    static member toNPMTag (semVer : SemVerInfo) (flag: PreReleaseFlag) (number : int) =
        PreReleaseFlag.toNugetTag semVer flag number

    static member toPyPITag (semVer : SemVerInfo) (tag: PreReleaseFlag) (number : int) =
        let suffix = 
            match tag with
            | Alpha -> $"a{number}"
            | Beta -> $"b{number}"
            | ReleaseCandidate -> $"rc{number}"
        sprintf "%i.%i.%i%s" semVer.Major semVer.Minor semVer.Patch suffix

