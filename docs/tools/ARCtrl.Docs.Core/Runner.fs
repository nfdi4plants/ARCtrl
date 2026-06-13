namespace ARCtrl.Docs

open System
open System.Diagnostics
open System.IO

module Runner =

    let private runProcess repositoryRoot fileName args (env: (string * string) seq) =
        let psi = ProcessStartInfo()
        psi.FileName <- fileName
        psi.Arguments <- args
        psi.WorkingDirectory <- repositoryRoot
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardError <- true
        psi.UseShellExecute <- false

        for key, value in env do
            psi.Environment.[key] <- value

        use p = new Process()
        p.StartInfo <- psi

        if not (p.Start()) then
            Errors.fail $"Failed to start command: {fileName} {args}"

        let stdout = p.StandardOutput.ReadToEnd()
        let stderr = p.StandardError.ReadToEnd()
        p.WaitForExit()

        if p.ExitCode <> 0 then
            Errors.fail $"Command failed with exit code {p.ExitCode}: {fileName} {args}\nSTDOUT:\n{stdout}\nSTDERR:\n{stderr}"

        stdout

    let private quote (value: string) = "\"" + value.Replace("\"", "\\\"") + "\""

    let buildFSharpProject repositoryRoot =
        runProcess repositoryRoot "dotnet" "build src/ARCtrl/ARCtrl.fsproj" Seq.empty |> ignore

    let ensureFableOutputs repositoryRoot =
        let tsIndex = Path.Combine(repositoryRoot, "src", "ARCtrl", "ts", "Core", "Table", "ArcTable.js")
        let pyArcTable = Path.Combine(repositoryRoot, "src", "ARCtrl", "py", "Core", "Table", "arc_table.py")

        if not (File.Exists tsIndex) then
            runProcess repositoryRoot "dotnet" "tool restore" Seq.empty |> ignore
            runProcess repositoryRoot "dotnet" "fable ./src/ARCtrl/ARCtrl.Javascript.fsproj --lang ts --fableLib @fable-org/fable-library-js --noCache -o src/ARCtrl/ts" Seq.empty |> ignore

        if not (File.Exists pyArcTable) then
            runProcess repositoryRoot "dotnet" "tool restore" Seq.empty |> ignore
            runProcess repositoryRoot "dotnet" "fable ./src/ARCtrl/ARCtrl.Python.fsproj --lang python --noCache -o src/ARCtrl/py" Seq.empty |> ignore

        if not (Directory.Exists(Path.Combine(repositoryRoot, "dist", "ts"))) then
            runProcess repositoryRoot "npm" "run build" Seq.empty |> ignore

    let testSnippet repositoryRoot (snippet: GeneratedSnippet) =
        match snippet.Language with
        | FSharp ->
            buildFSharpProject repositoryRoot
            let full = Path.GetFullPath snippet.OutputPath
            runProcess repositoryRoot "dotnet" $"fsi {quote full}" Seq.empty |> ignore
        | TypeScript ->
            ensureFableOutputs repositoryRoot
            let full = Path.GetFullPath snippet.OutputPath
            runProcess repositoryRoot "npx" $"tsc --noEmit --module es2020 --target es2020 --moduleResolution node --skipLibCheck {quote full}" Seq.empty |> ignore
            runProcess repositoryRoot "node" (quote full) Seq.empty |> ignore
        | Python ->
            ensureFableOutputs repositoryRoot
            let full = Path.GetFullPath snippet.OutputPath
            let pythonPath = Path.Combine(repositoryRoot, "src", "ARCtrl")
            runProcess repositoryRoot "python" $"-m py_compile {quote full}" Seq.empty |> ignore
            runProcess repositoryRoot "python" (quote full) [ "PYTHONPATH", pythonPath ] |> ignore

    let testAll repositoryRoot snippets =
        for snippet in snippets do
            testSnippet repositoryRoot snippet
