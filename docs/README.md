# ARCtrl trilingual docs tooling

This folder contains the source files and local tooling for generated trilingual
ARCtrl narrative documentation.

The docs tooling batch-processes source pages and executable snippets:

- Source pages: `docs/pages/**/*.mdx`
- Source snippets: `docs/snippets/**/*.fsx`
- Snippet metadata: `docs/snippets/**/*.snippet.yml`

Generated files are written to `docs/generated/` and are ignored by git.

## Source layout

Author prose in `docs/pages/**` as MDX-like files. Do not write manual language
tabs in these files. Insert examples with:

```mdx
<TriSnippet id="tables.arc-table.build-table" />
```

Use the `id` declared in the matching `*.snippet.yml` file.

Author executable F# snippets in `docs/snippets/**`. The rendered region is
between:

```fsharp
// docs:begin
// docs:end
```

Hidden assertions go between:

```fsharp
// docs:assert
// docs:endassert
```

The metadata file next to the snippet, `*.snippet.yml`, declares the snippet id,
source file, render tabs, public API imports, and checks.

The MDX renderer always writes one generated page for every source page under
`docs/pages/**`. Pages without snippet placeholders are copied through with
frontmatter and prose preserved. Pages with `<TriSnippet id="..." />`
placeholders are copied through with the matching placeholder replaced by
generated Starlight language tabs.

## Commands

Run commands from the repository root.

Build the docs CLI:

```powershell
dotnet build docs/tools/ARCtrl.Docs.Cli/ARCtrl.Docs.Cli.fsproj
```

Generate and validate the minimal API shape manifest:

```powershell
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- api-shape generate
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- api-shape validate
```

Generate F#, TypeScript, and Python snippets:

```powershell
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- snippets translate
```

Run generated snippets:

```powershell
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- snippets test
```

Render generated MDX:

```powershell
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- mdx render
```

This renders every page under `docs/pages/**`. Pages without snippet
placeholders are copied through. Pages with `<TriSnippet id="..." />`
placeholders receive generated Starlight language tabs for the referenced
snippets.

Run the full local pipeline. This is the main command to transpile, test, and
render the docs in batch:

```powershell
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- all
```

To focus on one snippet while developing it, add `--snippet <snippet-id>` to
`snippets translate`, `snippets test`, `mdx render`, or `all`.

## Generated output

The pipeline writes generated files under:

```text
docs/generated/snippets/
docs/generated/mdx/
docs/generated/tmp/
```

These files are build artifacts. Inspect them when reviewing output, but edit the
source page, F# snippet, or snippet metadata instead.

## Prerequisites

The full pipeline may build local Fable outputs and run snippets in all three
languages. It expects:

- .NET SDK 8
- restored .NET tools, including Fable
- Node.js with `npm` and `npx`
- installed npm dependencies for this repo
- Python 3.11 or newer

If `src/ARCtrl/ts`, `src/ARCtrl/py`, or `dist/ts` are missing, the CLI attempts
to create them with Fable and `npm run build`.

## Current limits

- Snippets can be generated from the supported deterministic F# subset or from
  explicit TypeScript and Python overrides.
- `mdx render` renders all source pages and expands all referenced snippets by
  default. A selected `--snippet` is useful for focused development.
- Translation is deterministic and intentionally narrow.
- Unsupported F# syntax fails instead of being approximated.
- Snapshot comparison is not implemented yet; use hidden assertions.
- Knowledgebase sync and publication PRs are out of scope for this local tooling.
