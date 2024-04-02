module TestTasks

open BlackFox.Fake
open Fake.DotNet

open ProjectInfo
open BasicTasks
open Fake.Core

module RunTests = 

    let runTestsUI = BuildTask.create "runTestsUI" [clean; build] {
        let path = "tests/UI"
        Trace.traceImportant "Start UI tests"
        // transpile library for native access
        run dotnet $"fable src/ARCtrl -o {path}/ARCtrl" ""
        GenerateIndexJs.ARCtrl_generate($"{path}/ARCtrl")
        run npx $"cypress run --component -P {path}" ""
    }

    let runTestsJsNative = BuildTask.create "runTestsJSNative" [clean; build] {
        Trace.traceImportant "Start native JavaScript tests"
        for path in ProjectInfo.jsTestProjects do
            // transpile library for native access
            run dotnet $"fable src/ARCtrl -o {path}/ARCtrl" ""
            GenerateIndexJs.ARCtrl_generate($"{path}/ARCtrl")
            run npx $"mocha {path} --timeout 20000" "" 
    }

    let runTestsJs = BuildTask.create "runTestsJS" [clean; build] {
        for path in ProjectInfo.testProjects do
            // transpile js files from fsharp code
            run dotnet $"fable {path} -o {path}/js" ""
            // run mocha in target path to execute tests
            // "--timeout 20000" is used, because json schema validation takes a bit of time.
            run node $"{path}/js/Main.js" ""
    }

    let runTestsPyNative = BuildTask.create "runTestsPyNative" [clean; build] {
        Trace.traceImportant "Start native Python tests"
        for path in ProjectInfo.pyTestProjects do
            // transpile library for native access
            run dotnet $"fable src/ARCtrl -o {path}/ARCtrl --lang python" ""
            GenerateIndexPy.ARCtrl_generate($"{path}/ARCtrl")
            run python $"-m pytest {path}" "" 
    }

    let runTestsPy = BuildTask.create "runTestsPy" [clean; build] {
        for path in ProjectInfo.testProjects do
            //transpile py files from fsharp code
            run dotnet $"fable {path} -o {path}/py --lang python" ""
            // run pyxpecto in target path to execute tests in python
            run python $"{path}/py/main.py" ""
    }

    let runTestsDotnet = BuildTask.create "runTestsDotnet" [clean; build] {
        let dotnetRun = run dotnet "run"
        testProjects
        |> Seq.iter dotnetRun
    }

let runTests = BuildTask.create "RunTests" [clean; build; RunTests.runTestsJs; RunTests.runTestsJsNative; RunTests.runTestsPy; RunTests.runTestsPyNative; RunTests.runTestsDotnet] { 
    ()
}