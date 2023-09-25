module TestTasks

open BlackFox.Fake
open Fake.DotNet

open ProjectInfo
open BasicTasks
open Fake.Core

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

    let npx =
        let npmPath =
            match ProcessUtils.tryFindFileOnPath "npx" with
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
   
module GenerateIndexJs =

    open System
    open System.IO
    open System.Text.RegularExpressions

    let private getAllJsFiles(path: string) = 
        let options = EnumerationOptions()
        options.RecurseSubdirectories <- true
        IO.Directory.EnumerateFiles(path,"*.js",options)

    let private pattern (className: string) = sprintf @"^export class (?<ClassName>%s)+.*{" className

    type private FileInformation = {
        FilePath : string
        Lines : string []
    } with
        static member create(filePath: string, lines: string []) = {
            FilePath = filePath
            Lines = lines
        }

    let private findClasses (rootPath: string) (cois: string []) (filePaths: seq<string> ) = 
        let files = [|
            for fp in filePaths do
                yield FileInformation.create(fp, System.IO.File.ReadAllLines (fp))
        |]
        let importStatements = ResizeArray()
        let findClass (className: string) = 
            /// maybe set this as default if you do not want any whitelist
            let classNameDefault = @"[a-zA-Z_0-9]"
            let regex = Regex(Regex.Escape(className) |> pattern)
            let mutable found = false
            let mutable result = None
            let enum = files.GetEnumerator()
            while not found && enum.MoveNext() do
                let fileInfo = enum.Current :?> FileInformation
                for line in fileInfo.Lines do
                    let m = regex.Match(line)
                    match m.Success with
                    | true -> 
                        found <- true
                        result <- Some <| (className, IO.Path.GetRelativePath(rootPath,fileInfo.FilePath))
                    | false ->
                        ()
            match result with
            | None ->
                failwithf "Unable to find %s" className
            | Some r ->
                importStatements.Add r
        for coi in cois do findClass coi
        importStatements
        |> Array.ofSeq

    open System.Text

    let private createImportStatements (info: (string*string) []) =
        let sb = StringBuilder()
        let importCollection = info |> Array.groupBy snd |> Array.map (fun (p,a) -> p, a |> Array.map fst )
        for filePath, imports in importCollection do
            let p = filePath.Replace("\\","/")
            sb.Append "export { " |> ignore
            sb.AppendJoin(", ", imports) |> ignore
            sb.Append " } from " |> ignore
            sb.Append (sprintf "\"./%s\"" p) |> ignore
            sb.Append ";" |> ignore
            sb.AppendLine() |> ignore
        sb.ToString()

    let private writeJsIndexfile (path: string) (fileName: string) (content: string) =
        let filePath = Path.Combine(path, fileName)
        File.WriteAllText(filePath, content)

    let private generateIndexfile (rootPath: string, fileName: string, whiteList: string []) =
        getAllJsFiles(rootPath)
        |> findClasses rootPath whiteList
        |> createImportStatements
        |> writeJsIndexfile rootPath fileName

    let ARCtrl_generate(rootPath: string) = 
        let whiteList = [|
            "Comment$"
            "Person"
            "OntologyAnnotation"; 
            "IOType"
            "CompositeHeader"; 
            "CompositeCell"
            "CompositeColumn"
            "ArcTable"
            "ArcAssay"; 
            "ArcStudy"; 
            "ArcInvestigation"; 
            "Template"
            "Organisation"
            "JsWeb"
            "ARC"
        |]
        generateIndexfile(rootPath, "index.js", whiteList)

module RunTests = 

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
            run npx $"mocha {path}/js --timeout 20000" ""
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

let runTests = BuildTask.create "RunTests" [clean; build; RunTests.runTestsJs; RunTests.runTestsJsNative; RunTests.runTestsDotnet] { 
    ()
}