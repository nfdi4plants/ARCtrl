namespace ARCtrl.Docs

open System
open System.IO

module SnippetParser =

    let private between startMarker endMarker (text: string) =
        let lines = text.Replace("\r\n", "\n").Split('\n')
        let startIndex = lines |> Array.tryFindIndex (fun line -> line.Trim() = startMarker)
        let endIndex = lines |> Array.tryFindIndex (fun line -> line.Trim() = endMarker)

        match startIndex, endIndex with
        | Some s, Some e when e > s ->
            lines.[s + 1 .. e - 1] |> String.concat "\n"
        | _ ->
            Errors.fail $"Could not find region {startMarker} ... {endMarker}"

    let private readMetadata (path: string) =
        let lines = File.ReadAllLines path

        let valueFor key =
            lines
            |> Array.tryPick (fun line ->
                let trimmed = line.Trim()
                if trimmed.StartsWith(key + ":", StringComparison.Ordinal) then
                    Some(trimmed.Substring(key.Length + 1).Trim().Trim('"'))
                else
                    None
            )

        let compareSnapshot =
            lines
            |> Array.exists (fun line -> line.Trim().Equals("compareSnapshot: true", StringComparison.OrdinalIgnoreCase))

        let tabs =
            let text = String.concat "\n" lines
            [
                if text.Contains("- fsharp") then FSharp
                if text.Contains("- typescript") then TypeScript
                if text.Contains("- python") then Python
            ]

        {
            Id = valueFor "id" |> Option.defaultWith (fun () -> Errors.fail $"Missing id in {path}")
            Title = valueFor "title" |> Option.defaultValue ""
            Source = valueFor "source" |> Option.defaultWith (fun () -> Errors.fail $"Missing source in {path}")
            Tabs = if tabs.IsEmpty then [ FSharp; TypeScript; Python ] else tabs
            CompareSnapshot = compareSnapshot
        }

    let load repositoryRoot snippetId =
        let docsRoot = Path.Combine(repositoryRoot, "docs")
        let metadataFiles = Directory.GetFiles(Path.Combine(docsRoot, "snippets"), "*.snippet.yml", SearchOption.AllDirectories)

        let matches =
            metadataFiles
            |> Array.choose (fun metadataPath ->
                let metadata = readMetadata metadataPath
                if metadata.Id = snippetId then
                    let snippetDirectory = Path.GetDirectoryName metadataPath
                    let sourcePath = Path.Combine(snippetDirectory, metadata.Source)
                    let outputRoot = Path.Combine(docsRoot, "generated")
                    let relativeSnippetDir = Path.GetRelativePath(Path.Combine(docsRoot, "snippets"), snippetDirectory)

                    let paths =
                        {
                            Root = repositoryRoot
                            SnippetDirectory = snippetDirectory
                            SourcePath = sourcePath
                            MetadataPath = metadataPath
                            GeneratedSnippetDirectory = Path.Combine(outputRoot, "snippets", relativeSnippetDir)
                            GeneratedMdxDirectory = Path.Combine(outputRoot, "mdx")
                        }

                    let fullText = File.ReadAllText sourcePath
                    Some
                        {
                            Metadata = metadata
                            Paths = paths
                            FullText = fullText
                            RenderRegion = between "// docs:begin" "// docs:end" fullText
                            AssertionRegion = between "// docs:assert" "// docs:endassert" fullText
                        }
                else
                    None
            )

        match matches with
        | [| snippet |] -> snippet
        | [||] -> Errors.fail $"Could not find snippet metadata with id '{snippetId}' under docs/snippets"
        | _ -> Errors.fail $"Snippet id '{snippetId}' is not unique"

    let allSnippetIds repositoryRoot =
        let docsRoot = Path.Combine(repositoryRoot, "docs")
        Directory.GetFiles(Path.Combine(docsRoot, "snippets"), "*.snippet.yml", SearchOption.AllDirectories)
        |> Array.map (readMetadata >> _.Id)
        |> Array.toList
