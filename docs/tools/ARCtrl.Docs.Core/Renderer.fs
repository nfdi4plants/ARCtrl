namespace ARCtrl.Docs

open System
open System.IO
open System.Text
open System.Text.RegularExpressions

module Renderer =

    let private tabsImport = "import { Tabs, TabItem } from '@astrojs/starlight/components';"

    let private ensureTabsImport (content: string) =
        if content.Contains("Tabs") && content.Contains("TabItem") then
            content
        else
            let normalized = content.Replace("\r\n", "\n")
            if normalized.StartsWith("---\n") then
                let endFrontmatter = normalized.IndexOf("\n---\n", 4, StringComparison.Ordinal)
                if endFrontmatter >= 0 then
                    let insertAt = endFrontmatter + "\n---\n".Length
                    normalized.Insert(insertAt, "\n" + tabsImport + "\n")
                else
                    tabsImport + "\n\n" + normalized
            else
                tabsImport + "\n\n" + normalized

    let private renderTabBlock snippetId (generated: GeneratedSnippet list) =
        let sb = StringBuilder()
        sb.AppendLine($"{{/* snippet: {snippetId} */}}") |> ignore
        sb.AppendLine("<Tabs syncKey=\"arctrl-language\">") |> ignore

        for snippet in generated do
            sb.AppendLine($"  <TabItem label=\"{snippet.Language.Label}\">") |> ignore
            sb.AppendLine() |> ignore
            sb.AppendLine($"```{snippet.Language.Fence}") |> ignore
            sb.Append(snippet.RenderedCode.TrimEnd()) |> ignore
            sb.AppendLine() |> ignore
            sb.AppendLine("```") |> ignore
            sb.AppendLine() |> ignore
            sb.AppendLine("  </TabItem>") |> ignore

        sb.AppendLine("</Tabs>") |> ignore
        sb.ToString().TrimEnd()

    let renderPage repositoryRoot snippet (generated: GeneratedSnippet list) =
        let sourcePage = Path.Combine(repositoryRoot, "docs", "pages", "arctrl", "ISA", "arc-table.mdx")
        let targetPage = Path.Combine(repositoryRoot, "docs", "generated", "mdx", "arctrl", "ISA", "arc-table.mdx")

        let content = File.ReadAllText sourcePage |> ensureTabsImport
        let pattern = $"""<TriSnippet\s+id="{Regex.Escape(snippet.Metadata.Id)}"\s*/>"""
        let replacement = renderTabBlock snippet.Metadata.Id generated
        let rendered = Regex.Replace(content, pattern, replacement)

        if rendered.Contains("<TriSnippet") then
            Errors.fail "MDX rendering left one or more <TriSnippet /> placeholders unresolved."

        if rendered.Contains("docs:assert") || rendered.Contains("docs:endassert") then
            Errors.fail "Generated MDX contains hidden assertion markers."

        Paths.writeAllText targetPage rendered
        targetPage
