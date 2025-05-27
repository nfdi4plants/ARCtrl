module PerformanceTasks

open BlackFox.Fake
open Fake.DotNet

open ProjectInfo
open BasicTasks
open Fake.Core

module PerformanceReport = 

    let mutable cpu = ""

    let promptCpu() =
        if cpu = "" then
            printfn "Please enter your cpu name"
            cpu <- System.Console.ReadLine()

    let testPerformancePy = BuildTask.create "testPerformancePy" [clean] {
        promptCpu()
        let path = "tests/Speedtest"
     //transpile py files from fsharp code
        run dotnet $"fable {path} -o {path}/py --lang python --nocache" ""
        // run pyxpecto in target path to execute tests in python
        run python $"{path}/py/program.py \"{cpu}\"" ""
    }
    let testPerformanceJs = BuildTask.create "testPerformanceJS" [clean] {
        promptCpu()
        let path = "tests/Speedtest"
        // transpile js files from fsharp code
        run dotnet $"fable {path} -o {path}/js --nocache" ""
        // run mocha in target path to execute tests
        run node $"{path}/js/program.js \"{cpu}\"" ""
    }
    let testPerformanceDotnet = BuildTask.create "testPerformanceDotnet" [clean] {
        promptCpu()
        let path = "tests/Speedtest"
        run dotnet $"run --project {path} \"{cpu}\"" ""
    }

let perforanceReport = BuildTask.create "PerformanceReport" [PerformanceReport.testPerformancePy; PerformanceReport.testPerformanceJs; PerformanceReport.testPerformanceDotnet] { 
    ()
}