module Program

open System
open System.Diagnostics
open System.IO
open System.Text
open System.Text.RegularExpressions
open System.Text.Json
open System.Text.Json.Nodes

type Language = FSharp | TypeScript | Python

type Snippet =
    {
        Id: string
        SnippetDirectory: string
        SourcePath: string
        SourceBaseName: string
        RenderRegion: string
        AssertionRegion: string
        GeneratedDirectory: string
        TypeScriptOverridePath: string option
        PythonOverridePath: string option
    }

type Generated =
    {
        Language: Language
        RenderedCode: string
        ExecutableCode: string
        OutputPath: string
    }

let fail message = raise (InvalidOperationException(message))

let repositoryRoot () =
    let rec loop dir =
        if File.Exists(Path.Combine(dir, "ARCtrl.sln")) then dir
        else
            let parent = Directory.GetParent dir
            if isNull parent then Directory.GetCurrentDirectory() else loop parent.FullName
    loop (Directory.GetCurrentDirectory())

let writeText (path: string) (text: string) =
    Directory.CreateDirectory(Path.GetDirectoryName(path)) |> ignore
    File.WriteAllText(path, text.Replace("\r\n", "\n").Replace("\n", Environment.NewLine))

let optionValue name (args: string array) =
    args
    |> Array.tryFindIndex ((=) name)
    |> Option.bind (fun i -> if i + 1 < args.Length then Some args.[i + 1] else None)

let snippetId args = optionValue "--snippet" args

let between startMarker endMarker (text: string) =
    let lines = text.Replace("\r\n", "\n").Split('\n')
    let startIndex = lines |> Array.tryFindIndex (fun line -> line.Trim() = startMarker)
    let endIndex = lines |> Array.tryFindIndex (fun line -> line.Trim() = endMarker)
    match startIndex, endIndex with
    | Some s, Some e when e > s -> lines.[s + 1 .. e - 1] |> String.concat "\n"
    | _ -> fail $"Could not find region {startMarker} ... {endMarker}"

let tryBetween startMarker endMarker (text: string) =
    let lines = text.Replace("\r\n", "\n").Split('\n')
    let startIndex = lines |> Array.tryFindIndex (fun line -> line.Trim() = startMarker)
    let endIndex = lines |> Array.tryFindIndex (fun line -> line.Trim() = endMarker)
    match startIndex, endIndex with
    | Some s, Some e when e > s -> Some(lines.[s + 1 .. e - 1] |> String.concat "\n")
    | _ -> None

let private metadataValue key (metadata: string) =
    metadata.Split('\n')
    |> Array.tryPick (fun line ->
        let trimmed = line.Trim()
        if trimmed.StartsWith(key + ":") then
            Some(trimmed.Substring((key + ":").Length).Trim().Trim('"'))
        else
            None)

let private loadSnippetFromMetadata repositoryRoot (metadataPath: string) =
    let snippetRoot = Path.Combine(repositoryRoot, "docs", "snippets")
    let metadata = File.ReadAllText metadataPath
    if metadata.Contains("compareSnapshot: true") then
        fail "Snapshot comparison is not implemented in the first trilingual docs milestone."

    let id = metadataValue "id" metadata |> Option.defaultWith (fun () -> fail $"Missing id in {metadataPath}")
    let source = metadataValue "source" metadata |> Option.defaultWith (fun () -> fail $"Missing source in {metadataPath}")
    let dir = Path.GetDirectoryName metadataPath
    let sourcePath = Path.Combine(dir, source)
    let fullText = File.ReadAllText sourcePath
    let relDir = Path.GetRelativePath(snippetRoot, dir)
    let baseName = Path.GetFileNameWithoutExtension(source)
    let existingOverride ext =
        let path = Path.Combine(dir, baseName + ext)
        if File.Exists path then Some path else None

    {
        Id = id
        SnippetDirectory = dir
        SourcePath = sourcePath
        SourceBaseName = baseName
        RenderRegion = between "// docs:begin" "// docs:end" fullText
        AssertionRegion = between "// docs:assert" "// docs:endassert" fullText
        GeneratedDirectory = Path.Combine(repositoryRoot, "docs", "generated", "snippets", relDir)
        TypeScriptOverridePath = existingOverride ".ts"
        PythonOverridePath = existingOverride ".py"
    }

let loadSnippets repositoryRoot =
    let snippetRoot = Path.Combine(repositoryRoot, "docs", "snippets")
    Directory.GetFiles(snippetRoot, "*.snippet.yml", SearchOption.AllDirectories)
    |> Array.map (loadSnippetFromMetadata repositoryRoot)
    |> Array.sortBy _.Id

let selectSnippets repositoryRoot id =
    let snippets = loadSnippets repositoryRoot
    match id with
    | None -> snippets
    | Some id ->
        let matches = snippets |> Array.filter (fun s -> s.Id = id)
        match matches with
        | [| s |] -> [| s |]
        | [||] -> fail $"Could not find snippet metadata with id '{id}'"
        | _ -> fail $"Snippet id '{id}' is not unique"

let loadSnippet repositoryRoot id =
    let matches = selectSnippets repositoryRoot (Some id)
    match matches with
    | [| s |] -> s
    | [||] -> fail $"Could not find snippet metadata with id '{id}'"
    | _ -> fail $"Snippet id '{id}' is not unique"

let label = function FSharp -> "F#" | TypeScript -> "TypeScript" | Python -> "Python"
let fence = function FSharp -> "fsharp" | TypeScript -> "ts" | Python -> "python"

let toCamel (name: string) =
    Regex.Replace(name, "_([a-zA-Z])", fun m -> m.Groups.[1].Value.ToUpperInvariant())

let normalizeExpr (expr: string) = Regex.Replace(expr.Trim(), @"\s+", " ")

let splitArguments (args: string) =
    let result = ResizeArray<string>()
    let mutable depth = 0
    let mutable inString = false
    let current = StringBuilder()
    for i = 0 to args.Length - 1 do
        let c = args.[i]
        let previous = if i = 0 then '\000' else args.[i - 1]
        if c = '"' && previous <> '\\' then inString <- not inString
        let isSeparator = c = ',' && depth = 0 && not inString
        if not inString then
            match c with
            | '(' | '[' -> depth <- depth + 1
            | ')' | ']' -> depth <- depth - 1
            | _ -> ()
        if isSeparator then
            result.Add(current.ToString().Trim())
            current.Clear() |> ignore
        else
            current.Append(c) |> ignore
    let tail = current.ToString().Trim()
    if tail <> "" then result.Add tail
    result |> Seq.toList

let rec convertExpr lang expr =
    let e = normalizeExpr expr
    let convert = convertExpr lang
    let variable name = if lang = TypeScript then toCamel name else name
    if e.StartsWith("ResizeArray [|") && e.EndsWith("|]") then
        let inner = e.Substring("ResizeArray [|".Length, e.Length - "ResizeArray [|".Length - 2).Trim()
        $"[{convert inner}]"
    elif e.StartsWith("[|") && e.EndsWith("|]") then
        let inner = e.Substring(2, e.Length - 4).Trim()
        $"[{convert inner}]"
    elif e.StartsWith("\"") && e.EndsWith("\"") then e
    elif Regex.IsMatch(e, @"^[a-z][a-zA-Z0-9_]*$") then variable e
    elif e = "IOType.Source" then "IOType.source()"
    elif e = "IOType.Sample" then "IOType.sample()"
    elif e.StartsWith("CompositeHeader.Input ") then
        let arg = e.Substring("CompositeHeader.Input ".Length)
        $"CompositeHeader.input({convert arg})"
    elif e.StartsWith("CompositeHeader.Output ") then
        let arg = e.Substring("CompositeHeader.Output ".Length)
        $"CompositeHeader.output({convert arg})"
    elif e.StartsWith("CompositeHeader.Characteristic ") then
        let arg = e.Substring("CompositeHeader.Characteristic ".Length)
        $"CompositeHeader.characteristic({convert arg})"
    elif e.StartsWith("CompositeHeader.Parameter ") then
        let arg = e.Substring("CompositeHeader.Parameter ".Length)
        $"CompositeHeader.parameter({convert arg})"
    elif e.StartsWith("CompositeCell.createFreeText ") then
        let arg = e.Substring("CompositeCell.createFreeText ".Length)
        if lang = Python then $"CompositeCell.create_free_text({convert arg})"
        else $"CompositeCell.createFreeText({convert arg})"
    elif e.StartsWith("CompositeCell.createTerm ") then
        let arg = e.Substring("CompositeCell.createTerm ".Length)
        if lang = Python then $"CompositeCell.create_term({convert arg})"
        else $"CompositeCell.createTerm({convert arg})"
    elif e.StartsWith("CompositeCell.createUnitized(") && e.EndsWith(")") then
        let inner = e.Substring("CompositeCell.createUnitized(".Length, e.Length - "CompositeCell.createUnitized(".Length - 1)
        let args = splitArguments inner |> List.map convert |> String.concat ", "
        if lang = Python then $"CompositeCell.create_unitized({args})" else $"CompositeCell.createUnitized({args})"
    elif e.StartsWith("ArcTable.init(") then e
    elif e.StartsWith("OntologyAnnotation(") && e.EndsWith(")") then
        if lang = TypeScript then $"new {e}" else e
    else fail $"Unsupported F# expression in snippet translation: {expr}"

let collectStatements (region: string) =
    let lines = region.Replace("\r\n", "\n").Split('\n')
    let statements = ResizeArray<int * string>()
    let mutable startLine = 0
    let mutable current = ResizeArray<string>()
    let flush () =
        if current.Count > 0 then
            statements.Add(startLine, current |> String.concat "\n")
            current <- ResizeArray<string>()
    for i = 0 to lines.Length - 1 do
        let line = lines.[i]
        let trimmed = line.Trim()
        if trimmed = "" then flush ()
        elif not (line.StartsWith(" ") || line.StartsWith("\t")) && (trimmed.StartsWith("let ") || Regex.IsMatch(trimmed, @"^[a-z][a-zA-Z0-9_]*\.[A-Z][A-Za-z0-9_]*\(")) then
            flush ()
            startLine <- i + 1
            current.Add line
        else
            if current.Count = 0 then startLine <- i + 1
            current.Add line
    flush ()
    statements |> Seq.toList

let trimLetExpression (statement: string) =
    let m = Regex.Match(statement, @"^let\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*=\s*(.*)$", RegexOptions.Singleline)
    if m.Success then Some(m.Groups.[1].Value, m.Groups.[2].Value.Trim()) else None

let methodCall (statement: string) =
    let m = Regex.Match(normalizeExpr statement, @"^([a-z][a-zA-Z0-9_]*)\.([A-Z][A-Za-z0-9_]*)\((.*)\)$")
    if m.Success then Some(m.Groups.[1].Value, m.Groups.[2].Value, m.Groups.[3].Value) else None

let tsImports () =
    """import {
  ArcTable,
  OntologyAnnotation,
  CompositeHeader,
  CompositeCell,
  IOType,
} from "@nfdi4plants/arctrl";
"""

let pyImports () =
    """from arctrl import (
    ArcTable,
    OntologyAnnotation,
    CompositeHeader,
    CompositeCell,
    IOType,
)
"""

let renderStatement lang (line, statement) =
    match trimLetExpression statement with
    | Some(name, expr) ->
        if lang = TypeScript then $"const {toCamel name} = {convertExpr lang expr};"
        else $"{name} = {convertExpr lang expr}"
    | None ->
        match methodCall statement with
        | Some(target, methodName, args) ->
            let indent = if lang = TypeScript then "  " else "    "
            let converted = splitArguments args |> List.map (convertExpr lang) |> String.concat (",\n" + indent)
            if lang = TypeScript then $"{toCamel target}.{methodName}(\n  {converted},\n);"
            else $"{target}.{methodName}(\n    {converted},\n)"
        | None -> fail $"Unsupported F# syntax in snippet at render line {line}: {statement.Trim()}"

let overrideGenerated language path outputPath beginMarker endMarker assertMarker endAssertMarker =
    let text = File.ReadAllText path
    let rendered = between beginMarker endMarker text
    let assertions = tryBetween assertMarker endAssertMarker text |> Option.defaultValue ""
    let executable =
        if assertions.Trim() = "" then rendered.Trim() + "\n"
        else rendered.Trim() + "\n\n" + assertions.Trim() + "\n"
    { Language = language; RenderedCode = rendered.Trim() + "\n"; ExecutableCode = executable; OutputPath = outputPath }

let translate repositoryRoot snippet =
    let fsharpRender = "open ARCtrl\n\n" + snippet.RenderRegion.Trim() + "\n"
    let packageDll packageName version dllName =
        let userProfile =
            let value = Environment.GetEnvironmentVariable("USERPROFILE")
            if String.IsNullOrWhiteSpace value then Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            else value
        Path.Combine(
            userProfile,
            ".nuget",
            "packages",
            packageName,
            version,
            "lib",
            "netstandard2.0",
            dllName
        )
    let dependencyRefs =
        [|
            packageDll "fable.core" "5.0.0" "Fable.Core.dll"
            packageDll "thoth.json.core" "0.7.0" "Thoth.Json.Core.dll"
            packageDll "thoth.json.newtonsoft" "0.3.2" "Thoth.Json.Newtonsoft.dll"
            packageDll "newtonsoft.json" "13.0.3" "Newtonsoft.Json.dll"
            packageDll "fsspreadsheet" "7.0.0-alpha.1" "FsSpreadsheet.dll"
            packageDll "fsspreadsheet.net" "7.0.0-alpha.1" "FsSpreadsheet.Net.dll"
            packageDll "documentformat.openxml" "2.16.0" "DocumentFormat.OpenXml.dll"
            packageDll "closedxml" "0.102.2" "ClosedXML.dll"
            packageDll "system.io.packaging" "4.7.0" "System.IO.Packaging.dll"
        |]
        |> Array.filter File.Exists
    let arctrlRefs =
        let outputRoot = Path.Combine(repositoryRoot, "src", "ARCtrl", "bin", "Debug")
        if Directory.Exists outputRoot then
            Directory.GetFiles(outputRoot, "*.dll", SearchOption.AllDirectories)
            |> Array.sort
        else
            [||]
    let fsharpRefs =
        Array.append dependencyRefs arctrlRefs
        |> Array.distinct
        |> Array.map (fun path -> "#r @\"" + path + "\"")
        |> String.concat "\n"
    let fsharpExecutable =
        fsharpRefs + "\nopen ARCtrl\n\n"
        + snippet.RenderRegion.Trim()
        + "\n\n"
        + snippet.AssertionRegion.Trim()
        + "\n"
    let translatedExecutableAssertions lang variableName =
        if snippet.Id <> "tables.arc-table.build-table" then
            fail $"Snippet '{snippet.Id}' needs explicit TypeScript/Python overrides. The first translator only supports automatic translation for tables.arc-table.build-table."
        match lang with
        | TypeScript ->
            $"""if ({variableName}.Name !== "Growth") {{
  throw new Error("Expected table name to be Growth");
}}

if ({variableName}.ColumnCount !== 4) {{
  throw new Error(`Expected 4 columns, got ${{{variableName}.ColumnCount}}`);
}}
"""
        | Python ->
            $"""if {variableName}.Name != "Growth":
    raise Exception("Expected table name to be Growth")

if {variableName}.ColumnCount != 4:
    raise Exception(f"Expected 4 columns, got {{{variableName}.ColumnCount}}")
"""
        | FSharp -> ""
    let tsOutput = Path.Combine(snippet.GeneratedDirectory, snippet.SourceBaseName + ".ts")
    let pyOutput = Path.Combine(snippet.GeneratedDirectory, snippet.SourceBaseName + ".py")
    let tsGenerated =
        match snippet.TypeScriptOverridePath with
        | Some path -> overrideGenerated TypeScript path tsOutput "// docs:begin" "// docs:end" "// docs:assert" "// docs:endassert"
        | None ->
            let statements = collectStatements snippet.RenderRegion
            let tsBody = statements |> List.map (renderStatement TypeScript) |> String.concat "\n\n"
            let rendered = tsImports () + "\n" + tsBody + "\n"
            let executable = rendered + "\n" + translatedExecutableAssertions TypeScript "growth"
            { Language = TypeScript; RenderedCode = rendered; ExecutableCode = executable; OutputPath = tsOutput }
    let pyGenerated =
        match snippet.PythonOverridePath with
        | Some path -> overrideGenerated Python path pyOutput "# docs:begin" "# docs:end" "# docs:assert" "# docs:endassert"
        | None ->
            let statements = collectStatements snippet.RenderRegion
            let pyBody = statements |> List.map (renderStatement Python) |> String.concat "\n\n"
            let rendered = pyImports () + "\n" + pyBody + "\n"
            let executable = rendered + "\n" + translatedExecutableAssertions Python "growth"
            { Language = Python; RenderedCode = rendered; ExecutableCode = executable; OutputPath = pyOutput }
    [
        { Language = FSharp; RenderedCode = fsharpRender; ExecutableCode = fsharpExecutable; OutputPath = Path.Combine(snippet.GeneratedDirectory, snippet.SourceBaseName + ".fsx") }
        tsGenerated
        pyGenerated
    ]

let writeGenerated generated =
    for snippet in generated do writeText snippet.OutputPath snippet.ExecutableCode

let generateApiShape repositoryRoot =
    let path = Path.Combine(repositoryRoot, "docs", "api-shape", "arctrl.public-api.generated.json")
    let root = JsonObject()
    root["version"] <- JsonValue.Create(1)
    let generatedFrom = JsonObject()
    generatedFrom["fsharp"] <- JsonValue.Create("src/ARCtrl/ARCtrl.fsproj local build")
    generatedFrom["typescript"] <- JsonValue.Create("src/ARCtrl/index.ts and generated dist/ts/index.js")
    generatedFrom["python"] <- JsonValue.Create("src/ARCtrl/__init__.py and generated src/ARCtrl/py")
    root["generatedFrom"] <- generatedFrom
    writeText path (root.ToJsonString(JsonSerializerOptions(WriteIndented = true)))
    path

let validateApiShape repositoryRoot =
    let indexText = File.ReadAllText(Path.Combine(repositoryRoot, "src", "ARCtrl", "index.ts"))
    let initText = File.ReadAllText(Path.Combine(repositoryRoot, "src", "ARCtrl", "__init__.py"))
    for name in [ "ArcTable"; "OntologyAnnotation"; "CompositeHeader"; "CompositeCell"; "IOType" ] do
        if not (indexText.Contains name) then fail $"TypeScript public API shape is missing export for {name}"
        if not (initText.Contains name) then fail $"Python public API shape is missing export for {name}"

let quote (value: string) = "\"" + value.Replace("\"", "\\\"") + "\""

let commandFor (file: string) (args: string) =
    if OperatingSystem.IsWindows() && (file = "npm" || file = "npx") then
        let script = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "nodejs", file + ".ps1")
        if File.Exists script then
            "powershell", $"-NoProfile -ExecutionPolicy Bypass -File {quote script} {args}"
        else
            file + ".cmd", args
    else
        file, args

let runProcess (repositoryRoot: string) (file: string) (args: string) (env: (string * string) seq) =
    let file, args = commandFor file args
    let psi = ProcessStartInfo(file, args)
    psi.WorkingDirectory <- repositoryRoot
    psi.RedirectStandardOutput <- true
    psi.RedirectStandardError <- true
    psi.UseShellExecute <- false
    for key, value in env do psi.Environment.[key] <- value
    use p = Process.Start psi
    let stdout = p.StandardOutput.ReadToEnd()
    let stderr = p.StandardError.ReadToEnd()
    p.WaitForExit()
    if p.ExitCode <> 0 then fail $"Command failed with exit code {p.ExitCode}: {file} {args}\nSTDOUT:\n{stdout}\nSTDERR:\n{stderr}"

let ensureFableOutputs repositoryRoot =
    if not (File.Exists(Path.Combine(repositoryRoot, "src", "ARCtrl", "ts", "Core", "Table", "ArcTable.ts"))) then
        runProcess repositoryRoot "dotnet" "tool restore" []
        runProcess repositoryRoot "dotnet" "fable ./src/ARCtrl/ARCtrl.Javascript.fsproj --lang ts --fableLib @fable-org/fable-library-js --noCache -o src/ARCtrl/ts" []
    if not (File.Exists(Path.Combine(repositoryRoot, "src", "ARCtrl", "py", "Core", "Table", "arc_table.py"))) then
        runProcess repositoryRoot "dotnet" "tool restore" []
        runProcess repositoryRoot "dotnet" "fable ./src/ARCtrl/ARCtrl.Python.fsproj --lang python --noCache -o src/ARCtrl/py" []
    if not (Directory.Exists(Path.Combine(repositoryRoot, "dist", "ts"))) then
        runProcess repositoryRoot "npm" "run build" []

let rec copyDirectory source target =
    Directory.CreateDirectory(target) |> ignore
    for file in Directory.GetFiles(source) do
        File.Copy(file, Path.Combine(target, Path.GetFileName(file)), true)
    for directory in Directory.GetDirectories(source) do
        copyDirectory directory (Path.Combine(target, Path.GetFileName(directory)))

let stagePythonPackage repositoryRoot =
    let source = Path.Combine(repositoryRoot, "src", "ARCtrl")
    let targetRoot = Path.Combine(repositoryRoot, "docs", "generated", "tmp", "python")
    let target = Path.Combine(targetRoot, "arctrl")
    copyDirectory source target
    targetRoot

let stageNodePackage repositoryRoot (snippetPath: string) =
    let snippetDir = Path.GetDirectoryName(snippetPath)
    let packageRoot = Path.Combine(snippetDir, "node_modules", "@nfdi4plants", "arctrl")
    Directory.CreateDirectory(packageRoot) |> ignore
    File.Copy(Path.Combine(repositoryRoot, "package.json"), Path.Combine(packageRoot, "package.json"), true)
    copyDirectory (Path.Combine(repositoryRoot, "dist", "ts")) (Path.Combine(packageRoot, "dist", "ts"))

let testGenerated repositoryRoot generated =
    for snippet in generated do
        match snippet.Language with
        | FSharp ->
            runProcess repositoryRoot "dotnet" "build src/ARCtrl/ARCtrl.fsproj" []
            runProcess repositoryRoot "dotnet" $"fsi {quote (Path.GetFullPath snippet.OutputPath)}" []
        | TypeScript ->
            ensureFableOutputs repositoryRoot
            stageNodePackage repositoryRoot snippet.OutputPath
            runProcess repositoryRoot "npx" $"tsc --noEmit --module es2022 --target es2022 --moduleResolution node --skipLibCheck {quote (Path.GetFullPath snippet.OutputPath)}" []
            runProcess repositoryRoot "node" (quote (Path.GetFullPath snippet.OutputPath)) []
        | Python ->
            ensureFableOutputs repositoryRoot
            let pythonPath = stagePythonPackage repositoryRoot
            runProcess repositoryRoot "python" $"-m py_compile {quote (Path.GetFullPath snippet.OutputPath)}" []
            runProcess repositoryRoot "python" (quote (Path.GetFullPath snippet.OutputPath)) [ "PYTHONPATH", pythonPath ]

let sourcePages repositoryRoot =
    let pagesRoot = Path.Combine(repositoryRoot, "docs", "pages")
    Directory.GetFiles(pagesRoot, "*.mdx", SearchOption.AllDirectories)

let renderTabBlock snippet generated =
    let tabs = StringBuilder()
    tabs.AppendLine($"{{/* snippet: {snippet.Id} */}}") |> ignore
    tabs.AppendLine("<Tabs syncKey=\"arctrl-language\">") |> ignore
    for item in generated do
        tabs.AppendLine($"  <TabItem label=\"{label item.Language}\">") |> ignore
        tabs.AppendLine() |> ignore
        tabs.AppendLine($"```{fence item.Language}") |> ignore
        tabs.AppendLine(item.RenderedCode.TrimEnd()) |> ignore
        tabs.AppendLine("```") |> ignore
        tabs.AppendLine() |> ignore
        tabs.AppendLine("  </TabItem>") |> ignore
    tabs.AppendLine("</Tabs>") |> ignore
    tabs.ToString().TrimEnd()

let hasTabsImport (content: string) =
    content.Contains("from '@astrojs/starlight/components'")
    && content.Contains("Tabs")
    && content.Contains("TabItem")

let addTabsImport (sourceContent: string) (renderedContent: string) =
    if hasTabsImport sourceContent then renderedContent
    else
        let importLine = "import { Tabs, TabItem } from '@astrojs/starlight/components';"
        if renderedContent.StartsWith("---\n") then
            let i = renderedContent.IndexOf("\n---\n", 4, StringComparison.Ordinal)
            if i >= 0 then renderedContent.Insert(i + 5, "\n" + importLine + "\n") else importLine + "\n\n" + renderedContent
        else importLine + "\n\n" + renderedContent

let renderOneMdx repositoryRoot generatedBySnippet sourcePage =
    let pagesRoot = Path.Combine(repositoryRoot, "docs", "pages")
    let rel = Path.GetRelativePath(pagesRoot, sourcePage)
    let targetPage = Path.Combine(repositoryRoot, "docs", "generated", "mdx", rel)
    let content = File.ReadAllText(sourcePage).Replace("\r\n", "\n")
    let mutable rendered = content
    let mutable replaced = false
    for snippet, generated in generatedBySnippet do
        let placeholder = $"<TriSnippet id=\"{snippet.Id}\" />"
        if rendered.Contains placeholder then
            rendered <- rendered.Replace(placeholder, renderTabBlock snippet generated)
            replaced <- true
    if replaced then rendered <- addTabsImport content rendered
    if rendered.Contains("<TriSnippet") then fail $"MDX rendering left a <TriSnippet /> placeholder unresolved in {sourcePage}."
    if rendered.Contains("docs:assert") then fail "Generated MDX contains hidden assertion markers."
    writeText targetPage rendered
    targetPage

let renderMdx repositoryRoot generatedBySnippet =
    let pages = sourcePages repositoryRoot
    if pages.Length = 0 then
        fail "No source pages found under docs/pages."

    pages
    |> Array.map (renderOneMdx repositoryRoot generatedBySnippet)

let command args =
    match args |> Array.toList with
    | "api-shape" :: "generate" :: _ -> "api-shape-generate"
    | "api-shape" :: "validate" :: _ -> "api-shape-validate"
    | "snippets" :: "translate" :: _ -> "snippets-translate"
    | "snippets" :: "test" :: _ -> "snippets-test"
    | "mdx" :: "render" :: _ -> "mdx-render"
    | "all" :: _ -> "all"
    | _ -> fail "Unknown docs command."

[<EntryPoint>]
let main args =
    try
        let repositoryRoot = repositoryRoot()
        let id = snippetId args
        let snippets () = selectSnippets repositoryRoot id
        let generatedBySnippet () =
            snippets ()
            |> Array.map (fun snippet ->
                let generated = translate repositoryRoot snippet
                writeGenerated generated
                snippet, generated)
        match command args with
        | "api-shape-generate" -> printfn "Generated API shape manifest: %s" (generateApiShape repositoryRoot)
        | "api-shape-validate" -> validateApiShape repositoryRoot; printfn "Validated API shape manifest inputs."
        | "snippets-translate" ->
            for _, generated in generatedBySnippet () do
                for item in generated do printfn "Generated %s snippet: %s" (label item.Language) item.OutputPath
        | "snippets-test" ->
            for snippet, generated in generatedBySnippet () do
                testGenerated repositoryRoot generated
                printfn "All generated snippets passed for %s" snippet.Id
        | "mdx-render" ->
            renderMdx repositoryRoot (generatedBySnippet ())
            |> Array.iter (printfn "Rendered MDX: %s")
        | "all" ->
            generateApiShape repositoryRoot |> ignore
            validateApiShape repositoryRoot
            let generatedBySnippet = generatedBySnippet ()
            for snippet, generated in generatedBySnippet do
                testGenerated repositoryRoot generated
                printfn "All generated snippets passed for %s" snippet.Id
            renderMdx repositoryRoot generatedBySnippet
            |> Array.iter (printfn "Rendered MDX: %s")
            match id with
            | Some id -> printfn "Trilingual docs pipeline passed for %s" id
            | None -> printfn "Trilingual docs pipeline passed for all snippets"
        | _ -> fail "Unknown docs command."
        0
    with ex ->
        eprintfn "%s" ex.Message
        1
