module TestTasks

open BlackFox.Fake
open Fake.DotNet

open ProjectInfo
open BasicTasks
open Fake.Core

module RunTests = 

    let skipTestsFlag = "--skipTests"

    let runTestsUI = BuildTask.create "runTestsUI" [clean; build] {
        let path = "tests/UI"
        Trace.traceImportant "Start UI tests"
        // transpile library for native access
        run dotnet $"fable src/ARCtrl -o {path}/ARCtrl" ""
        GenerateIndexJs.ARCtrl_generate($"{path}/ARCtrl")
        run npx $"cypress run --component -P {path}" ""
    }

    let runTestsJsNative = BuildTask.createFn "runTestsJSNative" [clean; build] (fun tp ->
        if tp.Context.Arguments |> List.exists (fun a -> a.ToLower() = skipTestsFlag.ToLower()) |> not then
            Trace.traceImportant "Start native JavaScript tests"
            for path in ProjectInfo.jsTestProjects do
                // transpile library for native access
                run dotnet $"fable src/ARCtrl/ARCtrl.Javascript.fsproj -o {path}/ARCtrl" ""
                GenerateIndexJs.ARCtrl_generate($"{path}/ARCtrl")
                run npx $"mocha {path} --timeout 20000" ""
        else
            Trace.traceImportant "Skipping JavaScript tests"
    )

    let runTestsJs = BuildTask.createFn "runTestsJS" [clean; build] (fun tp ->
        if tp.Context.Arguments |> List.exists (fun a -> a.ToLower() = skipTestsFlag.ToLower()) |> not then
            Trace.traceImportant "Start Js tests"
            for path in ProjectInfo.testProjects do
                // transpile js files from fsharp code
                run dotnet $"fable {path} -o {path}/js" ""
                // run mocha in target path to execute tests
                // "--timeout 20000" is used, because json schema validation takes a bit of time.
                run node $"{path}/js/Main.js" ""
        else
            Trace.traceImportant "Skipping Js tests"
    )

    let runTestsPyNative = BuildTask.createFn "runTestsPyNative" [clean; build] (fun tp ->
        if tp.Context.Arguments |> List.exists (fun a -> a.ToLower() = skipTestsFlag.ToLower()) |> not then
            Trace.traceImportant "Start native Python tests"
            for path in ProjectInfo.pyTestProjects do
                // transpile library for native access
                run dotnet $"fable src/ARCtrl/ARCtrl.Python.fsproj -o {path}/ARCtrl --lang python" ""
                GenerateIndexPy.ARCtrl_generate($"{path}/ARCtrl")
                run python $"-m pytest {path}" ""
        else
            Trace.traceImportant "Skipping Python tests"
    )

    let runTestsPy = BuildTask.createFn "runTestsPy" [clean; build] (fun tp ->
        if tp.Context.Arguments |> List.exists (fun a -> a.ToLower() = skipTestsFlag.ToLower()) |> not then
            Trace.traceImportant "Start Python tests"
            for path in ProjectInfo.testProjects do
                //transpile py files from fsharp code
                run dotnet $"fable {path} -o {path}/py --lang python" ""
                // run pyxpecto in target path to execute tests in python
                run python $"{path}/py/main.py" ""
        else
            Trace.traceImportant "Skipping Python tests"

    )

    let runTestsDotnet = BuildTask.createFn "runTestsDotnet" [clean; build] (fun tp ->
        if tp.Context.Arguments |> List.exists (fun a -> a.ToLower() = skipTestsFlag.ToLower()) |> not then
            Trace.traceImportant "Start .NET tests"
            let dotnetRun = run dotnet "run"
            testProjects
            |> Seq.iter dotnetRun
        else
            Trace.traceImportant "Skipping .NET tests"
    )

    let runTestProject = BuildTask.createFn "runTestProject" [clean; build] (fun config ->
        let dotnetRun = run dotnet "run"
        match config.Context.Arguments with
        | projectName::[] ->
            let dotnetRun = run dotnet "run"
            match List.tryFind (fun (p:string) -> p.EndsWith(projectName)) testProjects with
            | Some p ->
                //
                printfn $"running tests for test project {p}"
                dotnetRun p
                //
                run dotnet $"fable {p} -o {p}/js" ""
                //transpile py files from fsharp code
                run dotnet $"fable {p} -o {p}/py --lang python" ""
                // run pyxpecto in target path to execute tests in python
                run python $"{p}/py/main.py" ""
                // transpile js files from fsharp code
                run dotnet $"fable {p} -o {p}/js" ""
                // run mocha in target path to execute tests
                // "--timeout 20000" is used, because json schema validation takes a bit of time.
                run node $"{p}/js/Main.js" ""
            | _ ->
                failwithf "Project %s not found" projectName
        | _ -> failwith "Please provide a project name to run tests for as the single argument"
    )
    

let runTests = BuildTask.create "RunTests" [clean; build; RunTests.runTestsJs; RunTests.runTestsJsNative; RunTests.runTestsPy; RunTests.runTestsPyNative; RunTests.runTestsDotnet] { 
    ()
}