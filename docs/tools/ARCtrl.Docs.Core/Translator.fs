namespace ARCtrl.Docs

open System
open System.Text
open System.Text.RegularExpressions

module Translator =

    let private importsTs =
        """import {
  ArcTable,
  OntologyAnnotation,
  CompositeHeader,
  CompositeCell,
  IOType,
} from "@nfdi4plants/arctrl";
"""

    let private importsPy =
        """from arctrl import (
    ArcTable,
    OntologyAnnotation,
    CompositeHeader,
    CompositeCell,
    IOType,
)
"""

    let private toCamel (name: string) =
        Regex.Replace(name, "_([a-zA-Z])", fun m -> m.Groups.[1].Value.ToUpperInvariant())

    let private normalizeExpr (expr: string) =
        Regex.Replace(expr.Trim(), @"\s+", " ")

    let private splitArguments (args: string) =
        let result = ResizeArray<string>()
        let mutable depth = 0
        let mutable inString = false
        let mutable current = StringBuilder()

        for i = 0 to args.Length - 1 do
            let c = args.[i]
            let previous = if i = 0 then '\000' else args.[i - 1]

            if c = '"' && previous <> '\\' then
                inString <- not inString

            if not inString then
                match c with
                | '(' | '[' -> depth <- depth + 1
                | ')' | ']' -> depth <- depth - 1
                | ',' when depth = 0 ->
                    result.Add(current.ToString().Trim())
                    current.Clear() |> ignore
                | _ -> ()

            if not (c = ',' && depth = 0 && not inString) then
                current.Append(c) |> ignore

        let tail = current.ToString().Trim()
        if tail <> "" then result.Add tail
        result |> Seq.toList

    let private convertVar lang name =
        match lang with
        | TypeScript -> toCamel name
        | Python -> name
        | FSharp -> name

    let rec private convertExpr lang (expr: string) =
        let e = normalizeExpr expr

        let convert = convertExpr lang
        let var = convertVar lang

        if e.StartsWith("[|") && e.EndsWith("|]") then
            let inner = e.Substring(2, e.Length - 4).Trim()
            $"[{convert inner}]"
        elif e.StartsWith("[") && e.EndsWith("]") then
            let inner = e.Substring(1, e.Length - 2).Trim()
            $"[{convert inner}]"
        elif e.StartsWith("\"") && e.EndsWith("\"") then
            e
        elif Regex.IsMatch(e, @"^[a-z][a-zA-Z0-9_]*$") then
            var e
        elif e = "IOType.Source" then
            match lang with
            | TypeScript -> "IOType.source()"
            | Python -> "IOType.source"
            | FSharp -> e
        elif e = "IOType.Sample" then
            match lang with
            | TypeScript -> "IOType.sample()"
            | Python -> "IOType.sample"
            | FSharp -> e
        elif e.StartsWith("CompositeHeader.Input ") then
            let arg = e.Substring("CompositeHeader.Input ".Length)
            match lang with
            | TypeScript | Python -> $"CompositeHeader.input({convert arg})"
            | FSharp -> e
        elif e.StartsWith("CompositeHeader.Output ") then
            let arg = e.Substring("CompositeHeader.Output ".Length)
            match lang with
            | TypeScript | Python -> $"CompositeHeader.output({convert arg})"
            | FSharp -> e
        elif e.StartsWith("CompositeHeader.Characteristic ") then
            let arg = e.Substring("CompositeHeader.Characteristic ".Length)
            match lang with
            | TypeScript | Python -> $"CompositeHeader.characteristic({convert arg})"
            | FSharp -> e
        elif e.StartsWith("CompositeHeader.Parameter ") then
            let arg = e.Substring("CompositeHeader.Parameter ".Length)
            match lang with
            | TypeScript | Python -> $"CompositeHeader.parameter({convert arg})"
            | FSharp -> e
        elif e.StartsWith("CompositeCell.createFreeText ") then
            let arg = e.Substring("CompositeCell.createFreeText ".Length)
            match lang with
            | TypeScript -> $"CompositeCell.createFreeText({convert arg})"
            | Python -> $"CompositeCell.create_free_text({convert arg})"
            | FSharp -> e
        elif e.StartsWith("CompositeCell.createTerm ") then
            let arg = e.Substring("CompositeCell.createTerm ".Length)
            match lang with
            | TypeScript -> $"CompositeCell.createTerm({convert arg})"
            | Python -> $"CompositeCell.create_term({convert arg})"
            | FSharp -> e
        elif e.StartsWith("CompositeCell.createUnitized(") && e.EndsWith(")") then
            let inner = e.Substring("CompositeCell.createUnitized(".Length, e.Length - "CompositeCell.createUnitized(".Length - 1)
            let args = splitArguments inner |> List.map convert |> String.concat ", "
            match lang with
            | TypeScript -> $"CompositeCell.createUnitized({args})"
            | Python -> $"CompositeCell.create_unitized({args})"
            | FSharp -> e
        elif e.StartsWith("ArcTable.init(") then
            e
        elif e.StartsWith("OntologyAnnotation(") && e.EndsWith(")") then
            match lang with
            | TypeScript -> $"new {e}"
            | Python -> e
            | FSharp -> e
        else
            Errors.fail $"Unsupported F# expression in snippet translation: {expr}"

    let private collectStatements (region: string) =
        let lines = region.Replace("\r\n", "\n").Split('\n')
        let statements = ResizeArray<int * string>()
        let mutable currentLine = 0
        let mutable current = ResizeArray<string>()

        let flush () =
            if current.Count > 0 then
                statements.Add(currentLine, current |> String.concat "\n")
                current <- ResizeArray<string>()

        for i = 0 to lines.Length - 1 do
            let line = lines.[i]
            let trimmed = line.Trim()
            if trimmed = "" then
                flush ()
            elif not (line.StartsWith(" ") || line.StartsWith("\t")) && (trimmed.StartsWith("let ") || Regex.IsMatch(trimmed, @"^[a-z][a-zA-Z0-9_]*\.[A-Z][A-Za-z0-9_]*\(")) then
                flush ()
                currentLine <- i + 1
                current.Add line
            else
                if current.Count = 0 then currentLine <- i + 1
                current.Add line

        flush ()
        statements |> Seq.toList

    let private trimLetExpression (statement: string) =
        let m = Regex.Match(statement, @"^let\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*=\s*(.*)$", RegexOptions.Singleline)
        if not m.Success then None
        else
            let name = m.Groups.[1].Value
            let expr = m.Groups.[2].Value.Trim()
            Some(name, expr)

    let private methodCall (statement: string) =
        let normalized = normalizeExpr statement
        let m = Regex.Match(normalized, @"^([a-z][a-zA-Z0-9_]*)\.([A-Z][A-Za-z0-9_]*)\((.*)\)$")
        if not m.Success then None
        else Some(m.Groups.[1].Value, m.Groups.[2].Value, m.Groups.[3].Value)

    let private renderTsStatement (lineNumber: int, statement: string) =
        match trimLetExpression statement with
        | Some(name, expr) -> $"const {toCamel name} = {convertExpr TypeScript expr};"
        | None ->
            match methodCall statement with
            | Some(target, methodName, args) ->
                let convertedArgs =
                    splitArguments args
                    |> List.map (convertExpr TypeScript)
                    |> String.concat ",\n  "

                $"{toCamel target}.{methodName}(\n  {convertedArgs},\n);"
            | None -> Errors.fail $"Unsupported F# syntax in snippet at render line {lineNumber}: {statement.Trim()}"

    let private renderPyStatement (lineNumber: int, statement: string) =
        match trimLetExpression statement with
        | Some(name, expr) -> $"{name} = {convertExpr Python expr}"
        | None ->
            match methodCall statement with
            | Some(target, methodName, args) ->
                let convertedArgs =
                    splitArguments args
                    |> List.map (convertExpr Python)
                    |> String.concat ",\n    "

                $"{target}.{methodName}(\n    {convertedArgs},\n)"
            | None -> Errors.fail $"Unsupported F# syntax in snippet at render line {lineNumber}: {statement.Trim()}"

    let private tsAssertions =
        """
if (growth.Name !== "Growth") {
  throw new Error("Expected table name to be Growth");
}

if (growth.ColumnCount !== 4) {
  throw new Error(`Expected 4 columns, got ${growth.ColumnCount}`);
}
"""

    let private pyAssertions =
        """
if growth.Name != "Growth":
    raise Exception("Expected table name to be Growth")

if growth.ColumnCount != 4:
    raise Exception(f"Expected 4 columns, got {growth.ColumnCount}")
"""

    let translate (snippet: SnippetSource) =
        if snippet.Metadata.CompareSnapshot then
            Errors.fail "Snapshot comparison is not implemented in the first trilingual docs milestone."

        let statements = collectStatements snippet.RenderRegion

        let fsharpRender = "open ARCtrl\n\n" + snippet.RenderRegion.Trim() + "\n"
        let fsharpExecutable =
            "#r @\"../../../src/ARCtrl/bin/Debug/net8.0/ARCtrl.dll\"\n"
            + "open ARCtrl\n\n"
            + snippet.RenderRegion.Trim()
            + "\n\n"
            + snippet.AssertionRegion.Trim()
            + "\n"

        let tsBody =
            statements
            |> List.map renderTsStatement
            |> String.concat "\n\n"

        let pyBody =
            statements
            |> List.map renderPyStatement
            |> String.concat "\n\n"

        let basePath = snippet.Paths.GeneratedSnippetDirectory
        [
            {
                Language = FSharp
                RenderedCode = fsharpRender
                ExecutableCode = fsharpExecutable
                OutputPath = System.IO.Path.Combine(basePath, "build-table.fsx")
            }
            {
                Language = TypeScript
                RenderedCode = importsTs + "\n" + tsBody + "\n"
                ExecutableCode = importsTs + "\n" + tsBody + "\n" + tsAssertions
                OutputPath = System.IO.Path.Combine(basePath, "build-table.ts")
            }
            {
                Language = Python
                RenderedCode = importsPy + "\n" + pyBody + "\n"
                ExecutableCode = importsPy + "\n" + pyBody + "\n" + pyAssertions
                OutputPath = System.IO.Path.Combine(basePath, "build-table.py")
            }
        ]

    let writeGenerated snippets =
        for snippet in snippets do
            Paths.writeAllText snippet.OutputPath snippet.ExecutableCode
