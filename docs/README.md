# ARCtrl trilingual docs tooling

This folder contains the source files and local tooling for generated trilingual
ARCtrl narrative documentation.

The current implementation is a first vertical slice. It supports all source
pages under `docs/pages/**` and one executable trilingual snippet shape:

- Source pages: `docs/pages/**/*.mdx`
- Source snippet: `docs/snippets/ISA/arc-table/build-table.fsx`
- Snippet id: `isa.arc-table.build-table`

Generated files are written to `docs/generated/` and are ignored by git.

## Source layout

Author prose in `docs/pages/**` as MDX-like files. Do not write manual language
tabs in these files. Insert examples with:

```mdx
<TriSnippet id="isa.arc-table.build-table" />
```

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
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- snippets translate --snippet isa.arc-table.build-table
```

Run generated snippets:

```powershell
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- snippets test --snippet isa.arc-table.build-table
```

Render generated MDX:

```powershell
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- mdx render --snippet isa.arc-table.build-table
```

This renders every page under `docs/pages/**`. The `--snippet` argument selects
which snippet is expanded in pages that reference it; pages without that snippet
are still rendered unchanged.

Run the full local pipeline:

```powershell
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- all --snippet isa.arc-table.build-table
```

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

- Only the `isa.arc-table.build-table` snippet shape is supported.
- `mdx render` renders all source pages, but expands only the selected snippet
  id for the current command.
- Translation is deterministic and intentionally narrow.
- Unsupported F# syntax fails instead of being approximated.
- Snapshot comparison is not implemented yet; use hidden assertions.
- Knowledgebase sync and publication PRs are out of scope for this local tooling.
