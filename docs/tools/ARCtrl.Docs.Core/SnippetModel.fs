namespace ARCtrl.Docs

open System
open System.IO

type Language =
    | FSharp
    | TypeScript
    | Python

    member this.FileExtension =
        match this with
        | FSharp -> "fsx"
        | TypeScript -> "ts"
        | Python -> "py"

    member this.Fence =
        match this with
        | FSharp -> "fsharp"
        | TypeScript -> "ts"
        | Python -> "python"

    member this.Label =
        match this with
        | FSharp -> "F#"
        | TypeScript -> "TypeScript"
        | Python -> "Python"

type SnippetMetadata =
    {
        Id: string
        Title: string
        Source: string
        Tabs: Language list
        CompareSnapshot: bool
    }

type SnippetPaths =
    {
        Root: string
        SnippetDirectory: string
        SourcePath: string
        MetadataPath: string
        GeneratedSnippetDirectory: string
        GeneratedMdxDirectory: string
    }

type SnippetSource =
    {
        Metadata: SnippetMetadata
        Paths: SnippetPaths
        FullText: string
        RenderRegion: string
        AssertionRegion: string
    }

type GeneratedSnippet =
    {
        Language: Language
        RenderedCode: string
        ExecutableCode: string
        OutputPath: string
    }

type PipelineOptions =
    {
        RepositoryRoot: string
        OutputRoot: string
        SnippetId: string option
    }

module Paths =

    let normalize (path: string) =
        path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar)

    let fullPath (root: string) (path: string) =
        if Path.IsPathFullyQualified path then
            path
        else
            Path.GetFullPath(Path.Combine(root, normalize path))

    let ensureDirectory (path: string) =
        Directory.CreateDirectory(path) |> ignore

    let writeAllText (path: string) (text: string) =
        Path.GetDirectoryName(path) |> ensureDirectory
        File.WriteAllText(path, text.Replace("\r\n", "\n").Replace("\n", Environment.NewLine))

module Errors =

    let fail message = raise (InvalidOperationException(message))
