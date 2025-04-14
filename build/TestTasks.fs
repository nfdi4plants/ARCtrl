module TestTasks

open BlackFox.Fake
open Fake.DotNet

open ProjectInfo
open BasicTasks
open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators

module RunTests = 

    let skipTestsFlag = "--skipTests"

    [<Literal>]
    let jsIOResultFolder = "./tests/TestingUtils/TestResults/js"

    [<Literal>]
    let pyIOResultFolder = "./tests/TestingUtils/TestResults/py"


    let runTestsUI = BuildTask.create "runTestsUI" [clean; build] {
        let path = "tests/UI"
        Trace.traceImportant "Start UI tests"
        // transpile library for native access
        run dotnet $"fable src/ARCtrl -o {path}/ARCtrl" ""
        GenerateIndexJs.ARCtrl_generate($"{path}/ARCtrl")
        run npx $"cypress run --component -P {path}" ""
    }

    let runTestsJsNative = BuildTask.createFn "runTestsJSNative" [clean] (fun tp ->
        if tp.Context.Arguments |> List.exists (fun a -> a.ToLower() = skipTestsFlag.ToLower()) |> not then
            Trace.traceImportant "Start native JavaScript tests"
            for path in ProjectInfo.jsTestProjects do
                // transpile library for native access
                run dotnet $"fable src/ARCtrl/ARCtrl.Javascript.fsproj -o {path}/ts --lang ts -e fs.ts --nocache" ""
                System.IO.File.Copy("src/ARCtrl/index.js", $"{path}/index.js", overwrite = true) |> ignore
                run npx $"vitest run" ""
        else
            Trace.traceImportant "Skipping JavaScript tests"
    )

    let prePareJsTests = BuildTask.create "PrepareJsTests" [] {
        !! "tests/TestingUtils/TestResults"
        |> Shell.cleanDirs
        System.IO.Directory.CreateDirectory(jsIOResultFolder) |> ignore
        //System.IO.File.Copy(jsHelperFilePath, $"{allTestsProject}/js/{jsHelperFileName}") |> ignore

    }


    let runTestsJs = BuildTask.createFn "runTestsJS" [clean] (fun tp ->
        if tp.Context.Arguments |> List.exists (fun a -> a.ToLower() = skipTestsFlag.ToLower()) |> not then
            Trace.traceImportant "Start Js tests"
            // Setup test results directory after clean
            System.IO.Directory.CreateDirectory(jsIOResultFolder) |> ignore
            // transpile js files from fsharp code
            run dotnet $"fable {allTestsProject} -o {allTestsProject}/js --nocache" ""
            // run mocha in target path to execute tests
            // "--timeout 20000" is used, because json schema validation takes a bit of time.
            run node $"{allTestsProject}/js/Main.js" ""
        else
            Trace.traceImportant "Skipping Js tests"
    )

    let runTestsPyNative = BuildTask.createFn "runTestsPyNative" [clean] (fun tp ->
        if tp.Context.Arguments |> List.exists (fun a -> a.ToLower() = skipTestsFlag.ToLower()) |> not then
            Trace.traceImportant "Start native Python tests"
            for path in ProjectInfo.pyTestProjects do
                // transpile library for native access
                run dotnet $"fable src/ARCtrl/ARCtrl.Python.fsproj -o {path}/ARCtrl --lang python --nocache" ""
                GenerateIndexPy.ARCtrl_generate($"{path}/ARCtrl")
                run python $"-m pytest {path}" ""
        else
            Trace.traceImportant "Skipping Python tests"
    )

    let runTestsPy = BuildTask.createFn "runTestsPy" [clean] (fun tp ->
        if tp.Context.Arguments |> List.exists (fun a -> a.ToLower() = skipTestsFlag.ToLower()) |> not then
            Trace.traceImportant "Start Python tests"
            // Setup test results directory after clean
            System.IO.Directory.CreateDirectory(pyIOResultFolder) |> ignore
            //transpile py files from fsharp code
            run dotnet $"fable {allTestsProject} -o {allTestsProject}/py --lang python --nocache" ""
            // run pyxpecto in target path to execute tests in python
            run python $"{allTestsProject}/py/main.py" ""
        else
            Trace.traceImportant "Skipping Python tests"

    )

    let runTestsDotnet = BuildTask.createFn "runTestsDotnet" [clean; build] (fun tp ->
        if tp.Context.Arguments |> List.exists (fun a -> a.ToLower() = skipTestsFlag.ToLower()) |> not then
            Trace.traceImportant "Start .NET tests"
            let dotnetRun = run dotnet "run"
            dotnetRun allTestsProject
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
                System.IO.Directory.CreateDirectory(jsIOResultFolder) |> ignore
                
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