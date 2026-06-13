# ARCtrl Trilingual Narrative Documentation Implementation Plan

This plan is intended for an AI coding agent or human maintainer implementing tested, trilingual, story-style ARCtrl documentation in the `nfdi4plants/ARCtrl` repository.

The goal is **not** to generate API reference documentation. The goal is to author narrative documentation once, using canonical F# examples, then generate idiomatic TypeScript and Python examples, execute all three languages, and render final MDX for publication in `nfdi4plants/nfdi4plants.knowledgebase`.

---

## 1. Problem Statement

ARCtrl is a polyglot library with source in F#, transpiled to TypeScript and Python using Fable. The public user-facing APIs are not raw Fable output. They are curated through:

- F#: `ARCtrl` NuGet package
- TypeScript: `@nfdi4plants/arctrl` npm package, with the curated public surface exported from `src/ARCtrl/index.ts`
- Python: `arctrl` PyPI package, with the curated public surface exported from `src/ARCtrl/__init__.py`

The documentation should:

1. Be story-like and tutorial-oriented.
2. Mix prose and code snippets.
3. Use F# as the canonical authored snippet language.
4. Generate idiomatic TypeScript and Python snippets.
5. Avoid exposing raw or awkward Fable internals where a curated public API exists.
6. Execute every visible snippet in all three languages.
7. Fail CI if any visible snippet is stale, invalid, or semantically inconsistent.
8. Render final MDX with trilingual code tabs for the knowledgebase.

---

## 2. Repository Ownership Decision

Use two repositories, but only one canonical source of truth.

### Canonical source: `nfdi4plants/ARCtrl`

Place the authored documentation source, snippets, translation tooling, runners, and CI checks in ARCtrl.

Reason: ARCtrl owns the F# source, public package APIs, `index.ts`, `__init__.py`, and release cadence. Documentation examples should be tested against the same API changes as the library itself.

### Publication target: `nfdi4plants/nfdi4plants.knowledgebase`

Keep the final user-facing MDX in the knowledgebase repository.

Reason: the knowledgebase is already the public documentation site. The generated ARCtrl pages can be copied or submitted as a PR from ARCtrl CI.

### Do not create a third repository

Do not create a new repository unless documentation tooling becomes independently versioned and used by multiple libraries.

---

## 3. Target Architecture

Treat documentation snippets as executable cross-language integration tests.

```text
canonical narrative MDX template
+ executable F# snippets
+ snippet metadata
+ public API shape manifest
        │
        ▼
translate F# examples into TypeScript and Python
        │
        ▼
run F#, TypeScript, and Python snippets
        │
        ▼
compare assertions / normalized snapshots
        │
        ▼
render final trilingual MDX
        │
        ▼
copy or PR generated MDX into nfdi4plants.knowledgebase
```

Core invariant:

```text
No visible code block is published unless it has passed in F#, TypeScript, and Python.
```

---

## 4. Proposed ARCtrl Repository Layout

Add the following structure to `nfdi4plants/ARCtrl`:

```text
docs/
  README.md

  pages/
    arctrl/
      ISA/
        arc-table.mdx

  snippets/
    ISA/
      arc-table/
        build-table.fsx
        build-table.snippet.yml
        build-table.snapshot.json

  generated/
    snippets/
      ISA/
        arc-table/
          build-table.fsx
          build-table.ts
          build-table.py
    mdx/
      arctrl/
        ISA/
          arc-table.mdx

  api-shape/
    arctrl.public-api.generated.json
    arctrl.public-api.overrides.yml

  tools/
    ARCtrl.Docs.Cli/
      ARCtrl.Docs.Cli.fsproj
      Program.fs
    ARCtrl.Docs.Core/
      ARCtrl.Docs.Core.fsproj
      PublicApiShape.fs
      SnippetModel.fs
      SnippetParser.fs
      Translator.fs
      Renderer.fs
      Runner.fs
      Snapshot.fs

  test-projects/
    fsharp/
      ARCtrl.Docs.FSharpRunner.fsproj
    typescript/
      package.json
      tsconfig.json
      vitest.config.ts
    python/
      pyproject.toml
      pytest.ini
```

Notes:

- `docs/pages/**` contains narrative source files with snippet placeholders.
- `docs/snippets/**` contains canonical executable snippets and metadata.
- `docs/generated/**` is build output and should be either ignored or committed depending on release workflow. Prefer ignoring generated snippets, but committing generated MDX only when syncing to the knowledgebase.
- `docs/api-shape/arctrl.public-api.generated.json` is generated from actual package surfaces.
- `docs/api-shape/arctrl.public-api.overrides.yml` contains deliberate idiom overrides that cannot be inferred automatically.

Suggested `.gitignore` additions:

```gitignore
# Trilingual docs generated test artifacts
docs/generated/snippets/
docs/generated/tmp/
docs/generated/reports/

# Optional: keep generated MDX uncommitted in ARCtrl if it is only used to open KB PRs
# docs/generated/mdx/
```

---

## 5. Knowledgebase Repository Layout

The generated target should be:

```text
nfdi4plants.knowledgebase/
  src/content/docs/arctrl/
    ISA/
      arc-table.mdx
```

Every generated file should start with a warning comment:

```mdx
{/*
  GENERATED FILE.
  Source: nfdi4plants/ARCtrl docs/pages/...
  Do not edit this file directly in nfdi4plants.knowledgebase.
  Edit the source page/snippets in nfdi4plants/ARCtrl instead.
*/}
```

---

## 6. Source Page Format

Use MDX-like source files with placeholders instead of manually duplicated language blocks.

Example: `docs/pages/arctrl/ISA/arc-table.mdx`

```mdx
---
title: "Annotation Tables"
lastUpdated: 2026-06-13
authors:
  - lukas-weil
sidebar:
  order: 4.1
---

import { Steps } from '@astrojs/starlight/components';

The tables shown in ISA xlsx files are represented in ARCtrl as `ArcTable` objects.

Each `CompositeColumn` consists of a `CompositeHeader` and a collection of `CompositeCell` values.

:::note
As the prefix *Composite* suggests, these types do not necessarily represent a single spreadsheet column. Depending on the header type, a single `CompositeColumn` can map to 1, 3, or 4 xlsx columns.
:::

## Building an ArcTable

<TriSnippet id="isa.arc-table.build-table" />
```

Rules:

1. Authors write prose in normal MDX.
2. Authors do not manually write language tabs in source pages.
3. Authors insert examples with `<TriSnippet id="..." />`.
4. The renderer replaces each placeholder with generated Starlight `Tabs` / `TabItem` markup.
5. The renderer must preserve frontmatter and imports.
6. The renderer must add the required `Tabs` / `TabItem` import if missing.

---

## 7. Snippet File Format

Each snippet consists of:

```text
<snippet-id>.fsx
<snippet-id>.snippet.yml
optional <snippet-id>.snapshot.json
optional manually overridden <snippet-id>.ts
optional manually overridden <snippet-id>.py
```

Example:

```text
docs/snippets/ISA/arc-table/
  build-table.fsx
  build-table.snippet.yml
  build-table.snapshot.json
```

### 7.1 Canonical F# snippet

Example: `build-table.fsx`

```fsharp
#r "nuget: ARCtrl"
open ARCtrl

// docs:begin
let growth = ArcTable.init("Growth")

let oa_species =
    OntologyAnnotation(
        "species",
        "NCIT",
        "NCIT:C45293"
    )

let oa_chlamy =
    OntologyAnnotation(
        "Chlamydomonas reinhardtii",
        "NCBITaxon",
        "NCBITaxon:3055"
    )

let oa_time =
    OntologyAnnotation(
        "time",
        "EFO",
        "EFO:0000721"
    )

let oa_day =
    OntologyAnnotation(
        "day",
        "UO",
        "UO:0000033"
    )

growth.AddColumn(
    CompositeHeader.Input IOType.Source,
    [| CompositeCell.createFreeText "Input1" |]
)

growth.AddColumn(
    CompositeHeader.Characteristic oa_species,
    [| CompositeCell.createTerm oa_chlamy |]
)

growth.AddColumn(
    CompositeHeader.Parameter oa_time,
    [| CompositeCell.createUnitized("5", oa_day) |]
)

growth.AddColumn(
    CompositeHeader.Output IOType.Sample,
    [| CompositeCell.createFreeText "Output1" |]
)
// docs:end

// docs:assert
if growth.Name <> "Growth" then
    failwith "Expected table name to be Growth"

if growth.ColumnCount <> 4 then
    failwithf "Expected 4 columns, got %i" growth.ColumnCount
// docs:endassert
```

Rules:

1. The full `.fsx` file must be executable.
2. Only the region between `// docs:begin` and `// docs:end` is rendered.
3. Hidden assertions go between `// docs:assert` and `// docs:endassert`.
4. Hidden setup may be supported later with `// docs:setup` and `// docs:endsetup`.
5. Do not render `#r` package lines unless the snippet metadata explicitly requests standalone rendering.
6. Prefer examples that exercise public user APIs only.

### 7.2 Snippet metadata

Example: `build-table.snippet.yml`

```yaml
id: isa.arc-table.build-table
title: Build an annotation table
source: build-table.fsx

render:
  showImports: true
  showPackageInstall: false
  tabs:
    - fsharp
    - typescript
    - python

packages:
  fsharp:
    registry: nuget
    package: ARCtrl
    mode: local-or-registry
  typescript:
    registry: npm
    package: "@nfdi4plants/arctrl"
    mode: local-or-registry
  python:
    registry: pypi
    package: arctrl
    mode: local-or-registry

publicApi:
  fsharp:
    - ArcTable
    - OntologyAnnotation
    - CompositeHeader
    - CompositeCell
    - IOType
  typescript:
    - ArcTable
    - OntologyAnnotation
    - CompositeHeader
    - CompositeCell
    - IOType
  python:
    - ArcTable
    - OntologyAnnotation
    - CompositeHeader
    - CompositeCell
    - IOType

translation:
  profile: arctrl-public-api
  allowOverrides: false
  casing:
    fsharp: pascal-or-camel-as-authored
    typescriptVariables: camelCase
    pythonVariables: snake_case

checks:
  run: true
  compareSnapshot: true
  snapshot: build-table.snapshot.json
  minimum:
    fsharp:
      - compile
      - run
    typescript:
      - typecheck
      - run
    python:
      - compile
      - run
```

Metadata rules:

1. `id` must be globally unique.
2. `source` points to the canonical F# file.
3. `publicApi` lists names that must be imported or opened from curated API surfaces.
4. `translation.allowOverrides` defaults to `false`.
5. If overrides are enabled, explicit `.ts` and/or `.py` files may be supplied and must pass the same checks.
6. `checks.compareSnapshot` should be used when the snippet creates a serializable object.
7. If no snapshot is possible, require explicit assertions in all languages.

---

## 8. Public API Shape Manifest

Create a generated manifest at:

```text
docs/api-shape/arctrl.public-api.generated.json
```

This manifest describes how public ARCtrl API shapes are accessed in each target language.

Example shape:

```json
{
  "version": 1,
  "generatedFrom": {
    "fsharp": "ARCtrl NuGet/local build",
    "typescript": "src/ARCtrl/index.ts or dist/ts/index.d.ts",
    "python": "src/ARCtrl/__init__.py or installed arctrl package"
  },
  "types": {
    "CompositeHeader": {
      "fsharp": {
        "name": "CompositeHeader",
        "constructorsOrCases": {
          "Input": "CompositeHeader.Input {0}",
          "Output": "CompositeHeader.Output {0}",
          "Characteristic": "CompositeHeader.Characteristic {0}",
          "Parameter": "CompositeHeader.Parameter {0}"
        }
      },
      "typescript": {
        "name": "CompositeHeader",
        "exportsFrom": "@nfdi4plants/arctrl",
        "constructorsOrCases": {
          "Input": "CompositeHeader.input({0})",
          "Output": "CompositeHeader.output({0})",
          "Characteristic": "CompositeHeader.characteristic({0})",
          "Parameter": "CompositeHeader.parameter({0})"
        }
      },
      "python": {
        "name": "CompositeHeader",
        "exportsFrom": "arctrl",
        "constructorsOrCases": {
          "Input": "CompositeHeader.input({0})",
          "Output": "CompositeHeader.output({0})",
          "Characteristic": "CompositeHeader.characteristic({0})",
          "Parameter": "CompositeHeader.parameter({0})"
        }
      }
    },
    "IOType": {
      "fsharp": {
        "cases": {
          "Source": "IOType.Source",
          "Sample": "IOType.Sample"
        }
      },
      "typescript": {
        "cases": {
          "Source": "IOType.source()",
          "Sample": "IOType.sample()"
        }
      },
      "python": {
        "cases": {
          "Source": "IOType.source",
          "Sample": "IOType.sample"
        }
      }
    }
  }
}
```

Important:

- The exact Python form `IOType.source` vs `IOType.source()` must be verified by running probes against the current package. Do not guess.
- The exact TypeScript form must be verified with `tsc` and runtime execution.
- The manifest may combine generated discovery and curated overrides.
- Generated discovery should be conservative. If a shape cannot be proven, require an override.

### 8.1 API shape discovery commands

Implement CLI commands:

```bash
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- api-shape generate

dotnet run --project docs/tools/ARCtrl.Docs.Cli -- api-shape validate
```

The generator should inspect:

1. F# package/local assembly, where feasible.
2. TypeScript public exports from `src/ARCtrl/index.ts` and/or built `dist/ts/index.d.ts`.
3. Python public exports from `src/ARCtrl/__init__.py` and/or the installed `arctrl` package.
4. Manual overrides from `docs/api-shape/arctrl.public-api.overrides.yml`.

Minimum first implementation:

- Hard-code a small manifest for the first supported snippets.
- Validate it by compiling/running generated probes.
- Expand discovery later.

---

## 9. Translation Rules

Implement a deterministic translator for a restricted, documentation-friendly subset of F#.

Do not use an LLM in CI.

### 9.1 Supported first-pass F# subset

Support the following constructs first:

```text
open ARCtrl
let name = expression
constructor calls: TypeName(arg1, arg2, ...)
static method calls: TypeName.methodName(args)
instance method calls: object.MethodName(args)
union/case-style API expressions listed in the public API manifest
string literals
integer/float/bool literals
arrays: [| ... |]
lists: [ ... ]
simple comments
simple multiline calls
```

For the initial `arc-table` page, the translator must support at least:

```text
ArcTable.init("Growth")
OntologyAnnotation(...)
CompositeHeader.Input IOType.Source
CompositeHeader.Output IOType.Sample
CompositeHeader.Characteristic oa_species
CompositeHeader.Parameter oa_time
CompositeCell.createFreeText "Input1"
CompositeCell.createTerm oa_chlamy
CompositeCell.createUnitized("5", oa_day)
growth.AddColumn(...)
```

### 9.2 Unsupported syntax policy

If the F# snippet contains unsupported syntax, fail with a clear diagnostic.

Example:

```text
Unsupported F# syntax in snippet isa.arc-table.build-table:
  line 18: pattern matching is not supported by the docs translator.

Simplify the snippet or add explicit TypeScript/Python overrides.
```

Do not silently approximate unsupported syntax.

### 9.3 TypeScript output style

Generated TypeScript should be idiomatic user-facing TypeScript.

Example:

```ts
import {
  ArcTable,
  OntologyAnnotation,
  CompositeHeader,
  CompositeCell,
  IOType,
} from "@nfdi4plants/arctrl";

const growth = ArcTable.init("Growth");

const oaSpecies = new OntologyAnnotation(
  "species",
  "NCIT",
  "NCIT:C45293",
);

growth.AddColumn(
  CompositeHeader.input(IOType.source()),
  [CompositeCell.createFreeText("Input1")],
);
```

Style rules:

1. Use ESM imports from `@nfdi4plants/arctrl`.
2. Import from the curated root package only.
3. Use `const` unless mutation is required.
4. Use `camelCase` variables.
5. Add semicolons.
6. Format with Prettier if available.
7. Do not import from `dist`, `ts/Core/...`, or raw Fable paths in rendered examples.
8. Constructor calls must use `new` only where the TypeScript API requires it.

### 9.4 Python output style

Generated Python should be idiomatic user-facing Python.

Example:

```python
from arctrl import (
    ArcTable,
    OntologyAnnotation,
    CompositeHeader,
    CompositeCell,
    IOType,
)

growth = ArcTable.init("Growth")

oa_species = OntologyAnnotation(
    "species",
    "NCIT",
    "NCIT:C45293",
)

growth.AddColumn(
    CompositeHeader.input(IOType.source),
    [CompositeCell.create_free_text("Input1")],
)
```

Style rules:

1. Import from `arctrl`, not from raw generated internals.
2. Use `snake_case` variables.
3. Use `snake_case` members when that is the public Python API shape.
4. Format with Black if available.
5. Do not import from `arctrl.py...` or other raw generated internals.
6. The exact union/member shape must come from the public API manifest and must be runtime-tested.

---

## 10. Overrides

Allow explicit TypeScript/Python overrides only as an escape hatch.

Example layout:

```text
docs/snippets/ISA/arc-table/
  complex-example.fsx
  complex-example.ts
  complex-example.py
  complex-example.snippet.yml
```

Metadata:

```yaml
translation:
  allowOverrides: true
  overrides:
    typescript: complex-example.ts
    python: complex-example.py
```

Rules:

1. Overrides must use the same public API import policy.
2. Overrides must be executed.
3. Overrides must satisfy the same snapshot/assertion checks.
4. The renderer must not care whether a target snippet was generated or overridden.
5. Prefer fixing/expanding the translator over adding many overrides.

---

## 11. Execution and Validation

Every snippet must pass these stages.

### 11.1 F#

Minimum checks:

```bash
dotnet fsi docs/generated/snippets/ISA/arc-table/build-table.fsx
```

Preferred project-based runner:

```bash
dotnet run --project docs/test-projects/fsharp/ARCtrl.Docs.FSharpRunner.fsproj -- \
  docs/generated/snippets/ISA/arc-table/build-table.fsx
```

Checks:

1. The snippet compiles.
2. The snippet runs.
3. Hidden assertions pass.
4. If configured, the snippet writes normalized JSON output.

### 11.2 TypeScript

Generated test project should be ESM.

Minimum files:

```json
{
  "type": "module",
  "scripts": {
    "typecheck": "tsc --noEmit",
    "test:docs": "vitest run"
  },
  "dependencies": {
    "@nfdi4plants/arctrl": "file:../../../path-to-local-packed-arctrl.tgz"
  },
  "devDependencies": {
    "typescript": "~5.8.3",
    "vitest": "^3.1.1",
    "tsx": "latest"
  }
}
```

Minimum commands:

```bash
npm ci
npm run typecheck
npx tsx docs/generated/snippets/ISA/arc-table/build-table.ts
```

Checks:

1. The snippet typechecks with `tsc --noEmit`.
2. The snippet runs under Node.
3. Assertions pass.
4. If configured, the snippet writes normalized JSON output.

### 11.3 Python

Minimum checks:

```bash
python -m py_compile docs/generated/snippets/ISA/arc-table/build-table.py
python docs/generated/snippets/ISA/arc-table/build-table.py
```

Preferred:

```bash
python -m pytest docs/generated/snippets/python-tests
```

Checks:

1. The snippet imports from `arctrl`.
2. The snippet compiles.
3. The snippet runs.
4. Assertions pass.
5. If configured, the snippet writes normalized JSON output.

---

## 12. Semantic Snapshot Checks

Where possible, compare behavior across languages using normalized JSON snapshots.

Preferred flow:

```text
F# snippet creates object
F# serializes object to JSON through ARCtrl public serializer
TypeScript snippet creates equivalent object
TypeScript serializes object to JSON through ARCtrl public serializer
Python snippet creates equivalent object
Python serializes object to JSON through ARCtrl public serializer
runner normalizes all JSON
runner compares all outputs to snapshot
```

Snapshot rules:

1. Normalize object property order.
2. Normalize insignificant whitespace.
3. Normalize line endings.
4. Avoid snapshots for values that contain timestamps, random IDs, machine-specific paths, or dependency-specific formatting.
5. If a stable serializer is not available, use explicit assertions instead.

Example generated hidden assertion concept:

```text
assert normalized(fsharpOutput) == normalized(snapshot)
assert normalized(typescriptOutput) == normalized(snapshot)
assert normalized(pythonOutput) == normalized(snapshot)
```

---

## 13. MDX Rendering

Render `<TriSnippet id="..." />` placeholders into Starlight tabs.

Target output:

````mdx
import { Tabs, TabItem } from '@astrojs/starlight/components';

<Tabs syncKey="arctrl-language">
  <TabItem label="F#">

```fsharp
open ARCtrl

let growth = ArcTable.init("Growth")
```

  </TabItem>
  <TabItem label="TypeScript">

```ts
import { ArcTable } from "@nfdi4plants/arctrl";

const growth = ArcTable.init("Growth");
```

  </TabItem>
  <TabItem label="Python">

```python
from arctrl import ArcTable

growth = ArcTable.init("Growth")
```

  </TabItem>
</Tabs>
````

Renderer rules:

1. Preserve frontmatter.
2. Preserve narrative MDX.
3. Add `Tabs` and `TabItem` import if not already present.
4. Use `syncKey="arctrl-language"` consistently.
5. Use language labels exactly: `F#`, `TypeScript`, `Python`.
6. Use fenced code languages: `fsharp`, `ts`, `python`.
7. Do not render hidden assertions.
8. Optionally render a small comment with snippet id for debugging:

```mdx
{/* snippet: isa.arc-table.build-table */}
```

9. Do not publish generated MDX if snippet checks fail.

---

## 14. CLI Design

Implement a single CLI entry point.

Suggested commands:

```bash
# Generate or validate public API shape manifest
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- api-shape generate
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- api-shape validate

# Translate snippets only
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- snippets translate

# Run all snippets in all languages
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- snippets test

# Render final MDX
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- mdx render

# Full pipeline
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- all
```

Useful options:

```bash
--mode local
--mode registry
--snippet isa.arc-table.build-table
--page arctrl/ISA/arc-table.mdx
--update-snapshots
--fail-on-overrides
--output docs/generated
--knowledgebase-output ../nfdi4plants.knowledgebase/src/content/docs/arctrl
```

Exit codes:

```text
0 success
1 validation failed
2 translation failed
3 snippet runtime failed
4 snapshot mismatch
5 renderer failed
```

---

## 15. CI Design

Use two modes: PR mode and release/publication mode.

### 15.1 PR mode: local packages

Purpose: catch doc and public API breakage before release.

Steps:

```text
1. Restore/build ARCtrl.
2. Build local NuGet package.
3. Build local npm package tarball.
4. Build local Python wheel.
5. Generate/validate public API shape manifest.
6. Translate snippets.
7. Run F# snippets against local NuGet/package output.
8. Run TypeScript snippets against local npm tarball.
9. Run Python snippets against local wheel.
10. Compare snapshots/assertions.
11. Render MDX.
12. Optionally run a knowledgebase build against generated pages.
```

Suggested GitHub Actions job name:

```yaml
name: docs-trilingual-pr
```

### 15.2 Release mode: registry packages

Purpose: confirm published docs match published packages.

Steps:

```text
1. Install ARCtrl from NuGet.
2. Install @nfdi4plants/arctrl from npm.
3. Install arctrl from PyPI.
4. Generate/validate public API shape from installed packages.
5. Translate snippets.
6. Run all snippets.
7. Render final MDX.
8. Open PR against nfdi4plants/nfdi4plants.knowledgebase.
```

Suggested GitHub Actions job name:

```yaml
name: docs-trilingual-publish
```

---

## 16. Knowledgebase Sync

Implement one of these strategies.

### Option A: manual copy first

For the first milestone, generate MDX locally and manually copy it into the knowledgebase repository.

Pros:

- Simple.
- Easy to inspect.
- No token/permission complexity.

Cons:

- Manual step.

### Option B: CI opens a PR

In release mode, ARCtrl CI opens a PR against `nfdi4plants/nfdi4plants.knowledgebase`.

PR title:

```text
Update generated ARCtrl trilingual docs for ARCtrl <version>
```

PR body should include:

```text
Generated from nfdi4plants/ARCtrl commit <sha>.
All F#, TypeScript, and Python documentation snippets passed.
Package mode: registry.
NuGet: ARCtrl <version>
npm: @nfdi4plants/arctrl <version>
PyPI: arctrl <version>
```

Generated files should include source comments so maintainers know where to edit.

---

## 17. Migration of Existing `arc-table.mdx`

The existing knowledgebase page at:

```text
src/content/docs/arctrl/ISA/arc-table.mdx
```

currently contains manually maintained F#, JavaScript, and Python snippets.

Migration steps:

1. Copy the prose into `ARCtrl/docs/pages/arctrl/ISA/arc-table.mdx`.
2. Replace the manually written language blocks with `<TriSnippet id="isa.arc-table.build-table" />`.
3. Create `docs/snippets/ISA/arc-table/build-table.fsx` from the existing F# snippet.
4. Add hidden assertions.
5. Generate TypeScript and Python snippets.
6. Update Python import style to use the curated root package:

```python
from arctrl import ArcTable, OntologyAnnotation, CompositeHeader, CompositeCell, IOType
```

7. Validate the actual public Python API shape for `IOType.source` / `IOType.source()` and similar union/member cases.
8. Render final MDX.
9. Replace the knowledgebase page with generated MDX.

---

## 18. First Milestone Scope

Implement the smallest useful vertical slice.

### Must support

One page:

```text
docs/pages/arctrl/ISA/arc-table.mdx
```

One snippet:

```text
docs/snippets/ISA/arc-table/build-table.fsx
```

Required F# constructs:

```text
open ARCtrl
let bindings
OntologyAnnotation construction
ArcTable.init
CompositeHeader.Input
CompositeHeader.Output
CompositeHeader.Characteristic
CompositeHeader.Parameter
IOType.Source
IOType.Sample
CompositeCell.createFreeText
CompositeCell.createTerm
CompositeCell.createUnitized
array literals
growth.AddColumn
```

Required generated TypeScript style:

```ts
import {
  ArcTable,
  OntologyAnnotation,
  CompositeHeader,
  CompositeCell,
  IOType,
} from "@nfdi4plants/arctrl";
```

Required generated Python style:

```python
from arctrl import (
    ArcTable,
    OntologyAnnotation,
    CompositeHeader,
    CompositeCell,
    IOType,
)
```

### Acceptance criteria for milestone 1

1. `dotnet run --project docs/tools/ARCtrl.Docs.Cli -- all --snippet isa.arc-table.build-table` succeeds.
2. F# snippet runs.
3. TypeScript snippet typechecks and runs.
4. Python snippet compiles and runs.
5. Generated MDX contains a single trilingual tab block.
6. Generated MDX has no hidden assertions.
7. Generated TypeScript imports only from `@nfdi4plants/arctrl`.
8. Generated Python imports only from `arctrl`.
9. The generated page can replace the existing knowledgebase page.

---

## 19. Later Milestones

### Milestone 2: More arc-table examples

Add snippets for:

- Reading table metadata.
- Adding rows.
- Updating cells.
- Serializing table data.
- Converting between ISA xlsx representation and ARCtrl objects, if stable public APIs exist.

### Milestone 3: Snapshot comparison

Add stable JSON serialization and normalized snapshot comparison.

### Milestone 4: API manifest generation

Replace hard-coded manifest entries with generated probes and validation.

### Milestone 5: More pages

Migrate additional ARCtrl knowledgebase pages.

### Milestone 6: Documentation publication workflow

Add GitHub Actions workflow to open PRs against `nfdi4plants.knowledgebase`.

---

## 20. Quality Gates

The agent must enforce the following gates.

### Translation quality

- No raw Fable import paths in rendered docs.
- No undocumented generated member names unless they are part of the curated public API.
- No silently skipped code.
- Unsupported F# syntax fails explicitly.

### Runtime quality

- Every snippet compiles or typechecks.
- Every snippet runs.
- Hidden assertions pass.
- Snapshots match where configured.

### Documentation quality

- Narrative prose remains in MDX.
- Code examples are minimal and readable.
- Generated TypeScript and Python examples are idiomatic, not mechanical transliterations.
- The final MDX builds in the knowledgebase site.

---

## 21. Non-Goals

Do not implement these in the first version:

1. Full F# to TypeScript/Python transpilation.
2. General-purpose Markdown code block translation.
3. API reference generation.
4. LLM-based CI translation.
5. Automatic semantic equivalence for arbitrary objects.
6. Support for every F# language feature.
7. Editing generated MDX directly in the knowledgebase.

---

## 22. Implementation Notes for the Agent

Start with a pragmatic, narrow implementation.

Suggested order:

1. Add `docs/` layout.
2. Add one source page with `<TriSnippet />`.
3. Add one executable F# snippet.
4. Add snippet metadata schema.
5. Add a minimal hard-coded API shape manifest for the first example.
6. Implement region extraction from F#.
7. Implement translation for the first supported syntax subset.
8. Generate TypeScript and Python files.
9. Add basic runners for F#, TypeScript, and Python.
10. Add MDX renderer.
11. Add `--all` CLI command.
12. Add CI job.
13. Migrate existing `arc-table.mdx`.
14. Expand only after the first page works end-to-end.

Avoid premature generalization. The first success criterion is one complete, tested, generated trilingual page.

---

## 23. Example End-to-End Command Sequence

Local developer flow:

```bash
# From ARCtrl repository root

dotnet restore
npm install
python -m pip install build pytest

# Generate/validate API shape
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- api-shape generate --mode local

# Translate snippets
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- snippets translate --mode local

# Run snippets
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- snippets test --mode local

# Render MDX
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- mdx render

# Full pipeline
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- all --mode local
```

Registry verification flow:

```bash
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- all --mode registry
```

Knowledgebase output:

```bash
dotnet run --project docs/tools/ARCtrl.Docs.Cli -- mdx render \
  --knowledgebase-output ../nfdi4plants.knowledgebase/src/content/docs/arctrl
```

---

## 24. Definition of Done

This implementation is complete when:

1. ARCtrl contains source narrative docs with snippet placeholders.
2. ARCtrl contains executable F# snippets with hidden assertions.
3. TypeScript and Python snippets are generated or explicitly overridden.
4. All three languages are run in CI.
5. Public API imports come only from curated package roots.
6. A generated MDX page exists for `arc-table.mdx`.
7. The generated MDX can be used in `nfdi4plants.knowledgebase`.
8. The old manually synchronized trilingual snippets are replaced.
9. Documentation CI fails when any snippet breaks.
10. The workflow is documented for contributors.

---

## 25. Key Principle

The generated documentation is not the source of truth.

The source of truth is:

```text
prose template + executable snippets + public API manifest + passing checks
```

The published MDX is only the rendered result of a passing cross-language documentation test suite.
