module TestTasks

open BlackFox.Fake
open Fake.DotNet

open ProjectInfo
open BasicTasks

[<AutoOpen>]
module private Helper =

    open Fake
    open Fake.Core

    let createProcess exe arg dir =
        CreateProcess.fromRawCommandLine exe arg
        |> CreateProcess.withWorkingDirectory dir
        |> CreateProcess.ensureExitCode

    module Proc =

        module Parallel =

            open System

            let locker = obj()

            let colors = [|  
                ConsoleColor.DarkYellow
                ConsoleColor.DarkCyan 
                ConsoleColor.Magenta
                ConsoleColor.Blue
                ConsoleColor.Cyan
                ConsoleColor.DarkMagenta
                ConsoleColor.DarkBlue
                ConsoleColor.Yellow
            |]

            let print color (colored: string) (line: string) =
                lock locker
                    (fun () ->
                        let currentColor = Console.ForegroundColor
                        Console.ForegroundColor <- color
                        Console.Write colored
                        Console.ForegroundColor <- currentColor
                        Console.WriteLine line)

            let onStdout index name (line: string) =
                let color = colors.[index % colors.Length]
                if isNull line then
                    print color $"{name}: --- END ---" ""
                else if String.isNotNullOrEmpty line then
                    print color $"{name}: " line

            let onStderr name (line: string) =
                let color = ConsoleColor.Red
                if isNull line |> not then
                    print color $"{name}: " line

            let redirect (index, (name, createProcess)) =
                createProcess
                |> CreateProcess.redirectOutputIfNotRedirected
                |> CreateProcess.withOutputEvents (onStdout index name) (onStderr name)

            let printStarting indexed =
                for (index, (name, c: CreateProcess<_>)) in indexed do
                    let color = colors.[index % colors.Length]
                    let wd =
                        c.WorkingDirectory
                        |> Option.defaultValue ""
                    let exe = c.Command.Executable
                    let args = c.Command.Arguments.ToStartInfo
                    print color $"{name}: {wd}> {exe} {args}" ""

            let run cs =
                cs
                |> Seq.toArray
                |> Array.indexed
                |> fun x -> printStarting x; x
                |> Array.map redirect
                |> Array.Parallel.map Proc.run

    let dotnet = createProcess "dotnet"

    let npm =
        let npmPath =
            match ProcessUtils.tryFindFileOnPath "npm" with
            | Some path -> path
            | None ->
                "npm was not found in path. Please install it and make sure it's available from your path. " +
                "See https://safe-stack.github.io/docs/quickstart/#install-pre-requisites for more info"
                |> failwith

        createProcess npmPath

    let run proc arg dir =
        proc arg dir
        |> Proc.run
        |> ignore

    let runParallel processes =
        processes
        |> Proc.Parallel.run
        |> ignore
   
module RunTests = 

    /// runs `npm test` in root. 
    /// npm test consists of `test` and `pretest`
    /// check package.json in root for behavior
    let runTestsJs = BuildTask.create "runTestsJS" [clean; build] {
        //run npm "test --prefix ./src/ISA" ""
        run npm "test" ""
        run npm "run testJson" ""
        run npm "run testSpreadsheet" ""
        run npm "run testFS" ""
        run npm "run testjs" ""
        run npm "run testARC" ""
    }

    let runTestsDotnet = BuildTask.create "runTestsDotnet" [clean; build] {
        testProjects
        |> Seq.iter (fun testProject ->
            Fake.DotNet.DotNet.test(fun testParams ->
                {
                    testParams with
                        Logger = Some "console;verbosity=detailed"
                        Configuration = DotNet.BuildConfiguration.fromString configuration
                        NoBuild = true
                }
            ) testProject
        )
    }

let runTests = BuildTask.create "RunTests" [clean; build; RunTests.runTestsJs; RunTests.runTestsDotnet] { 
    ()
}

// // ! Decided to remove watch test logic for now, as formatting is really bad anyways and we support test explorer ! //
//module WatchTests =

    //let [<Literal>] JS_FolderName = "js" 

    //[<Literal>]
    //let FableTestPath_input = "tests/ISA/ISA.Tests"
    //let FableTestPath_output = System.IO.Path.Combine(FableTestPath_input, JS_FolderName)

    //[<Literal>]
    //let JsonFableTestPath_input = "tests/ISA/ISA.Json.Tests"
    //let JsonFableTestPath_output = System.IO.Path.Combine(JsonFableTestPath_input, JS_FolderName)

    //[<Literal>]
    //let SpreadsheetFableTestPath_input = "tests/ISA/ISA.Spreadsheet.Tests"
    //let SpreadsheetFableTestPath_output = System.IO.Path.Combine(SpreadsheetFableTestPath_input, JS_FolderName)

//    let private watchProjTests (projPath:string) =
//        let pName = 
//            let n = System.IO.Path.GetFileNameWithoutExtension(projPath)
//            $"[{n}]"
//        pName, dotnet "watch run" projPath

//    let private dotnetTestsProcesses =
//        [
//            for testProj in testProjects do
//                yield watchProjTests testProj
//        ]

//    let private fableTestsProcesses =
//        [
//            "[ISADotNet Fable]", dotnet $"fable watch {FableTestPath_input} -o {FableTestPath_output} --run npm run test:live" "."
//            "[ISADotNet Mocha]", npm $"run testnative:live" "."
//            "[ISADotNet.Json Fable]", dotnet $"fable watch {JsonFableTestPath_input} -o {JsonFableTestPath_output} --run npm run testJson:live" "."
//            "[ISADotNet.Json Mocha]", npm $"run testJsonnative:live" "."
//        ]

//    let allTest = dotnetTestsProcesses@fableTestsProcesses

//    let watchTestsDotnet = BuildTask.create "watchTestsDotnet" [clean; build] {
//        dotnetTestsProcesses
//        |> runParallel
//    }

//    let watchJS = BuildTask.create "watchTestsJS" [clean; build] {
//        fableTestsProcesses
//        |> runParallel
//    }

//let watchTests = BuildTask.create "watchTests" [clean; build] {
//    WatchTests.allTest
//    |> runParallel
//}

//// to do: use this once we have actual tests ~ Kevin Schneider
//// Not sure how this interacts with the changes for fable compatibility, i will..
//// ..leave it in for now ~ Kevin Frey
//let runTestsWithCodeCov = BuildTask.create "RunTestsWithCodeCov" [clean; build] {
//    let standardParams = Fake.DotNet.MSBuild.CliArguments.Create ()
//    testProjects
//    |> Seq.iter(fun testProject -> 
//        Fake.DotNet.DotNet.test(fun testParams ->
//            {
//                testParams with
//                    MSBuildParams = {
//                        standardParams with
//                            Properties = [
//                                "AltCover","true"
//                                "AltCoverCobertura","../../codeCov.xml"
//                                "AltCoverForce","true"
//                            ]
//                    };
//                    Logger = Some "console;verbosity=detailed"
//            }
//        ) testProject
//    )
//}