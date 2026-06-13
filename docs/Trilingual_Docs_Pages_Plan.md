# ARCtrl Trilingual Documentation Page Content Plan

This plan defines the narrative documentation content to author for ARCtrl using
the trilingual docs pipeline described in
`docs/TrilingualDocs_Docs_Implementation_Plan.md`.

The goal is to document ARCtrl by capability and actual usage, not by the shape
of the currently sparse knowledgebase pages. Existing pages are useful migration
material, but they should not constrain the final information architecture.

## Guiding Principles

- Prefer real workflows over API inventories.
- Keep F# as the canonical snippet language.
- Render TypeScript and Python from the public package roots where possible:
  - F#: `open ARCtrl`
  - TypeScript: `import { ... } from "@nfdi4plants/arctrl";`
  - Python: `from arctrl import ...`
- Avoid obsolete imports in rendered examples:
  - `from arctrl.arc`
  - `from arctrl.arctrl`
  - `@nfdi4plants/arctrl/Core/...`
  - generated Fable internals
- Mark pages by language support:
  - **Trilingual stable**: F#, TypeScript, and Python root APIs are available.
  - **F#/TypeScript advanced**: public TypeScript surface exists, Python root
    surface is absent or unverified.
  - **F# advanced/internal**: useful capability exists but public polyglot
    surface needs confirmation before publishing as trilingual docs.
- Each visible code block should become an executable snippet where feasible.
- Pages may start with a small concept section, but the main body should be
  task-oriented.

## Evidence Sources

The page plan is based on:

- Current ARCtrl docs and scripts in this repository.
- Current knowledgebase ARCtrl pages.
- ARCtrl public TypeScript root exports in `src/ARCtrl/index.ts`.
- ARCtrl public Python root exports in `src/ARCtrl/__init__.py`.
- ARCtrl tests for core objects, tables, IO, filesystem, JSON, YAML,
  RO-Crate, CWL, workflow graphs, and validation packages.
- Usage in downstream repositories:
  - `nfdi4plants/Swate`
  - `nfdi4plants/ARCitect`
  - `nfdi4plants/arc-export`
  - `nfdi4plants/arcCommander`
  - `nfdi4plants/elab2arc`
  - `nfdi4plants/arc-validate-package-registry`
  - `IPK-BIT/brapi2arc`
  - `IPK-BIT/mira`

## Documentation Structure

Target generated knowledgebase structure:

```text
arctrl/
  index.mdx
  setup.mdx
  quickstart.mdx

  core/
    arc.mdx
    isa-objects.mdx
    people-publications-comments.mdx
    identifiers-and-registration.mdx

  tables/
    arc-table.mdx
    headers-and-cells.mdx
    rows-columns-and-updates.mdx
    arc-tables-collections.mdx
    table-json-and-templates.mdx

  io/
    local-arc-io.mdx
    contracts.mdx
    filesystem-tree.mdx
    spreadsheet-io.mdx
    custom-storage-and-datahub.mdx

  serialization/
    arctrl-json.mdx
    isa-json.mdx
    ro-crate.mdx
    json-ld-graph.mdx

  datamaps/
    datamap.mdx
    attach-datamaps.mdx

  templates/
    templates.mdx
    validation-packages.mdx

  workflows/
    runs-and-workflows.mdx
    cwl.mdx
    workflow-graph.mdx

  recipes/
    export-annotation-tables.mdx
    create-arc-from-sops.mdx
    transform-external-data.mdx
    update-existing-arc.mdx
    export-ro-crate-summary.mdx
```

This structure can be implemented gradually. Early milestones should publish a
smaller subset, but snippets and ids should already use the final page
categories.

## Priority 1: Foundation Pages

These pages should be implemented first because they establish modern imports,
basic mental models, and the snippet style used everywhere else.

### `index.mdx`: What ARCtrl Is

Language support: prose only, optional trilingual smoke snippet.

Include:

- ARCtrl as the in-memory model and IO toolkit for Annotated Research Contexts.
- Three package surfaces: NuGet, npm, PyPI.
- What ARCtrl can do:
  - construct ARC metadata objects
  - edit ISA annotation tables
  - read/write ARC scaffold metadata
  - serialize ARCtrl JSON, ISA-JSON, RO-Crate JSON-LD
  - work with datamaps, runs, workflows, templates, and validation packages
- What ARCtrl is not:
  - not a full DataHUB client
  - not a spreadsheet UI
  - not a validator runner by itself, except for package configuration helpers

Snippet candidates:

- `overview.import-smoke-test`

### `setup.mdx`: Installation and Imports

Language support: trilingual stable.

Include:

- Install commands:
  - `dotnet add package ARCtrl`
  - `npm install @nfdi4plants/arctrl`
  - `pip install arctrl`
- F# scripting with `#r "nuget: ARCtrl"`.
- Modern imports from root packages.
- Short migration note for obsolete imports.
- TypeScript ESM expectation.
- Python package casing: package on PyPI is `arctrl`, import is `arctrl`.

Snippet candidates:

- `setup.fsharp-script-reference`
- `setup.typescript-root-import`
- `setup.python-root-import`

### `quickstart.mdx`: Create a Minimal ARC

Language support: trilingual stable.

Include:

- Create an empty `ARC`.
- Create an `ArcInvestigation`.
- Add one `ArcStudy` and one `ArcAssay`.
- Add one simple annotation table.
- Serialize to ARCtrl JSON or ISA-JSON.
- Keep filesystem writing for later IO pages.

Snippet candidates:

- `quickstart.create-arc`
- `quickstart.add-study-assay`
- `quickstart.serialize-investigation`

## Priority 2: Core ARC and ISA Model

### `core/arc.mdx`: The ARC Object

Language support: trilingual stable for basic object usage.

Include:

- `ARC` as the container for:
  - file system tree
  - optional ISA investigation
  - license/default files
  - runs/workflows where supported
- Empty ARC creation.
- ARC from existing file paths.
- ARC from investigation.
- How `ARC.load` differs from `ARC.fromFilePaths`.
- `TryGetStudy`, `TryGetAssay`, `TryGetRun`, `TryGetWorkflow` if root APIs are
  verified in each language.

Snippet candidates:

- `arc.empty`
- `arc.from-file-paths`
- `arc.from-investigation`
- `arc.find-contained-objects`

### `core/isa-objects.mdx`: Investigation, Study, and Assay

Language support: trilingual stable.

Include:

- `ArcInvestigation`, `ArcStudy`, `ArcAssay`.
- Identifiers versus titles/descriptions.
- Registering studies and assays.
- Creating objects directly versus initializing them through parent objects.
- Updating top-level metadata.
- Practical pattern from arcCommander/ARCitect: load ARC, get ISA or create
  missing ISA, modify, write/update.

Snippet candidates:

- `isa.create-investigation`
- `isa.create-study`
- `isa.create-assay`
- `isa.register-study-assay`
- `isa.update-metadata`

### `core/people-publications-comments.mdx`

Language support: trilingual stable after public shapes are verified.

Include:

- `Person`, `Publication`, `Comment`, `OntologyAnnotation`.
- Where people appear:
  - investigation contacts
  - study contacts
  - assay performers
- Publication status and DOI/PubMed metadata.
- Comments as generic key/value metadata.
- JSON roundtrip examples.

Snippet candidates:

- `core.person-with-role`
- `core.publication`
- `core.comments`
- `core.contacts-on-study`

### `core/identifiers-and-registration.mdx`

Language support: trilingual stable for public methods; F#/TypeScript advanced
for helper APIs until root exports are confirmed.

Include:

- Identifier rules and why they matter for folder paths.
- Registering assays under studies.
- Renaming objects safely.
- Missing identifiers and placeholder behavior.
- Publicly documented alternatives to raw identifier helper imports.
- Note: ARCitect currently uses raw identifier helper paths; docs should not
  recommend those until root exports exist.

Snippet candidates:

- `core.register-assay-under-study`
- `core.try-get-by-identifier`
- `core.rename-study-or-assay`

## Priority 3: Annotation Tables

### `tables/arc-table.mdx`: Build an Annotation Table

Language support: trilingual stable.

Include:

- What `ArcTable` represents.
- Column-major data model.
- Why one `CompositeColumn` may render as 1, 3, or 4 spreadsheet columns.
- Build a growth table with input, characteristic, parameter, protocol ref,
  and output.

Snippet candidates:

- `tables.arc-table.build-basic`
- Existing `isa.arc-table.build-table` can become the first snippet here.

### `tables/headers-and-cells.mdx`

Language support: trilingual stable.

Include:

- `CompositeHeader` types:
  - input/output with `IOType`
  - characteristic
  - factor
  - parameter
  - component
  - protocol ref/type/description/uri/version
  - date, performer, freetext/comment if public shape is stable
- `CompositeCell` types:
  - free text
  - term
  - unitized
  - data
  - empty term/free text
- Conversion from strings where public APIs exist.
- Choosing cell type based on header type.

Snippet candidates:

- `tables.headers.common`
- `tables.cells.common`
- `tables.cells.unitized`
- `tables.cells.data-file`
- `tables.headers.protocol`

### `tables/rows-columns-and-updates.mdx`

Language support: trilingual stable.

Include:

- Add one column with all cells.
- Add multiple columns.
- Add empty rows.
- Add populated rows.
- Get row and column values.
- Update a single cell by column/row index.
- Replace or copy rows between tables.
- Patterns from Swate and brapi2arc.

Snippet candidates:

- `tables.add-columns`
- `tables.add-rows`
- `tables.get-row-column`
- `tables.update-cell`
- `tables.copy-row-between-tables`

### `tables/arc-tables-collections.mdx`

Language support: trilingual stable after `ArcTables` root behavior is verified;
Python root exports include `ArcTables`.

Include:

- `ArcTables` as the study/assay table collection.
- Add, get, update, remove tables.
- Table order where public methods exist.
- Differences between a single `ArcTable` and the collection interface.
- How studies and assays expose tables.

Snippet candidates:

- `tables.collection-add-get`
- `tables.collection-update-table`
- `tables.collection-remove-table`

### `tables/table-json-and-templates.mdx`

Language support: trilingual stable for JSON; template support requires API
shape verification.

Include:

- Serialize a table to ARCtrl JSON.
- Deserialize a table.
- Compressed table JSON if it is part of public API.
- Swate-style template import/export concepts.
- Template JSON versus ARC metadata JSON.

Snippet candidates:

- `tables.json-roundtrip`
- `tables.template-json`

## Priority 4: IO, Contracts, and Filesystem

### `io/local-arc-io.mdx`

Language support: trilingual stable for `ARC.load`; write/update APIs must be
verified per language.

Include:

- Loading a local ARC scaffold.
- Writing a new ARC scaffold.
- Updating metadata files in an existing scaffold.
- What metadata files are touched.
- Handling missing ISA files.
- Temporary-directory examples for tested snippets.

Snippet candidates:

- `io.arc-load-local`
- `io.arc-write-new`
- `io.arc-update-existing`

### `io/contracts.mdx`

Language support: trilingual stable after `Contract` and DTO shapes are verified.

Include:

- Why contracts exist.
- `GetReadContracts`.
- `GetWriteContracts`.
- Contract operation kinds.
- DTO content for xlsx/json/binary payloads.
- Batch fulfillment.
- Error handling when files are absent.
- When to use contracts instead of `ARC.load`.

Snippet candidates:

- `io.contracts-list-read`
- `io.contracts-fulfill-memory`
- `io.contracts-write`

### `io/filesystem-tree.mdx`

Language support: trilingual stable.

Include:

- `FileSystem` and `FileSystemTree`.
- Build tree from file paths.
- Convert tree back to file paths.
- Add files.
- Filter hidden files or metadata files.
- Find a path with `TryGetPath`.
- Use cases:
  - ignore hidden files for RO-Crate export
  - detect registered/unregistered payload
  - scaffold an ARC before metadata exists

Snippet candidates:

- `filesystem.from-file-paths`
- `filesystem.to-file-paths`
- `filesystem.add-file`
- `filesystem.filter-hidden`
- `filesystem.try-get-path`

### `io/spreadsheet-io.mdx`

Language support: trilingual stable after `XlsxController` public shapes are
verified.

Include:

- Convert `ArcInvestigation`, `ArcStudy`, `ArcAssay`, and `Datamap` to workbook.
- Convert workbook back to objects.
- Difference between object-to-workbook and file-to-object helpers.
- Where `FsSpreadsheet` appears in F# examples.
- Browser/Node/Python differences for actual xlsx file IO.

Snippet candidates:

- `xlsx.study-to-workbook`
- `xlsx.assay-roundtrip`
- `xlsx.datamap-roundtrip`

### `io/custom-storage-and-datahub.mdx`

Language support: Python and TypeScript examples should be allowed as explicit
overrides; F# canonical snippet can show the contract pattern.

Include:

- Loading without cloning a repository.
- DataHUB/GitLab file listing.
- Build `ARC.fromFilePaths`.
- Fulfill read contracts from HTTP responses.
- Inject fulfilled contracts with `SetISAFromContracts`.
- Patterns observed in brapi2arc, mira, and current knowledgebase
  consume-datahub page.

Snippet candidates:

- `io.custom-loader-contracts`
- `io.datahub-file-listing`
- `io.datahub-load-arc`

## Priority 5: Serialization Formats

### `serialization/arctrl-json.mdx`

Language support: trilingual stable.

Include:

- ARCtrl JSON as native object serialization.
- When to use ARCtrl JSON versus ISA-JSON.
- Roundtrip examples for:
  - table
  - study
  - assay
  - investigation
  - datamap
  - run/workflow where public APIs are verified

Snippet candidates:

- `json.arctrl.table-roundtrip`
- `json.arctrl.study-roundtrip`
- `json.arctrl.investigation-roundtrip`

### `serialization/isa-json.mdx`

Language support: trilingual stable after controller method shape verification.

Include:

- ISA-JSON as interoperability/export format.
- Investigation-level ISA-JSON.
- Study/assay export context.
- ID referencing options where public APIs expose them.
- Schema validation if currently used in tests.

Snippet candidates:

- `json.isa.investigation`
- `json.isa.study`
- `json.isa.assay`

### `serialization/ro-crate.mdx`

Language support: trilingual stable for high-level ARC/ISA conversion if public
methods exist; F#/TypeScript advanced for lower-level conversion helpers.

Include:

- RO-Crate JSON-LD as metadata export.
- ARC to RO-Crate metadata.
- RO-Crate back to ARC or investigation where supported.
- Handling missing/unregistered assays as seen in arc-export.
- LFS/hidden-file filtering concept, without coupling to Git.

Snippet candidates:

- `rocrate.arc-to-json`
- `rocrate.json-to-arc`
- `rocrate.export-with-filesystem`

### `serialization/json-ld-graph.mdx`

Language support: F#/TypeScript advanced. Python root exports do not currently
include `ROCrate`.

Include:

- `LDGraph`, `LDNode`, `LDRef`, `LDValue`, `LDContext`.
- Create a minimal graph.
- Add nodes.
- Set and get properties.
- Resolve compact names through context.
- Check types with `HasType`.
- Serialize graph to RO-Crate JSON.
- Use this when users need JSON-LD object manipulation beyond high-level ARC
  conversion.

Snippet candidates:

- `jsonld.create-node`
- `jsonld.set-get-property`
- `jsonld.create-graph`
- `jsonld.serialize-graph`

## Priority 6: Datamaps and Data Annotation

### `datamaps/datamap.mdx`

Language support: trilingual stable.

Include:

- `Datamap` and `DataContext`.
- What a datamap row represents.
- Annotating a CSV column or file fragment.
- Ontology annotations for measurement, unit, object type, labels, comments.
- Roundtrip to JSON and xlsx.

Snippet candidates:

- `datamap.create-context`
- `datamap.create-datamap`
- `datamap.json-roundtrip`
- `datamap.xlsx-roundtrip`

### `datamaps/attach-datamaps.mdx`

Language support: trilingual stable.

Include:

- Datamaps on studies, assays, runs, and workflows.
- Determine parent from path.
- Attach or replace datamap on parent object.
- ARCitect-style pattern for updating parent datamap while preserving stable
  hash if relevant and public.

Snippet candidates:

- `datamap.attach-to-assay`
- `datamap.attach-to-study`
- `datamap.attach-to-run`
- `datamap.attach-to-workflow`

## Priority 7: Templates, Validation Packages, and YAML

### `templates/templates.mdx`

Language support: trilingual stable after `Template`, `Templates`, and
`WebController` method shapes are verified.

Include:

- Template object model.
- Template JSON.
- Template collections.
- Using templates to create reusable annotation tables.
- Web/template retrieval if `WebController` is stable.
- Relationship to Swate templates.

Snippet candidates:

- `templates.create`
- `templates.table-from-template`
- `templates.json-roundtrip`

### `templates/validation-packages.mdx`

Language support: F# stable; TypeScript/Python support depends on root exports.
`YamlController` is root-exported in both TypeScript and Python, but validation
package object exports need verification.

Include:

- What validation package configuration is.
- `ValidationPackage`.
- `ValidationPackagesConfig`.
- Serialize to YAML.
- Parse from YAML.
- Structural equality/copy behavior if useful.
- How this relates to ARC validation package registries and staging scripts.

Snippet candidates:

- `validation-package.create`
- `validation-package-config.create`
- `validation-package-config.yaml-roundtrip`

## Priority 8: Runs, Workflows, CWL, and Graphs

### `workflows/runs-and-workflows.mdx`

Language support: trilingual stable for `ArcRun` and `ArcWorkflow` core objects.

Include:

- What runs and workflows represent in ARCtrl.
- How they appear alongside studies and assays.
- Attach tables and datamaps.
- Serialize through `JsonController`.
- ARCitect usage pattern: add run/workflow to ARC, find by identifier, update.

Snippet candidates:

- `workflow.create-run`
- `workflow.create-workflow`
- `workflow.attach-datamap`
- `workflow.json-roundtrip`

### `workflows/cwl.mdx`

Language support: F#/TypeScript advanced. TypeScript root exports expose `CWL`;
Python root exports do not currently expose CWL.

Include:

- CWL support overview.
- CommandLineTool and Workflow descriptions.
- Inputs, outputs, steps, parameter references.
- Requirements:
  - Docker
  - environment variables
  - software
  - resources
  - initial workdir
- YAML/JSON decode and encode.
- How CWL descriptions connect to `ArcWorkflow`.

Snippet candidates:

- `cwl.command-line-tool`
- `cwl.workflow-step`
- `cwl.docker-requirement`
- `cwl.yaml-roundtrip`

### `workflows/workflow-graph.mdx`

Language support: F# advanced first; TypeScript only after public root API is
confirmed. This subsystem is not currently root-exported through
`@nfdi4plants/arctrl`.

Include:

- Build a workflow graph from CWL/ARC workflow descriptions.
- Node and edge kinds.
- Resolving run references.
- Diagnostics for malformed references or missing steps.
- Query helpers.
- Mermaid/Siren visualization output.
- Advanced warning: publish only after API stability is confirmed.

Snippet candidates:

- `workflow-graph.build-basic`
- `workflow-graph.missing-reference-diagnostic`
- `workflow-graph.visualize`

## Priority 9: Recipes

Recipes should combine the concept pages into end-to-end tasks. They may use
longer snippets and explicit TypeScript/Python overrides when necessary.

### `recipes/export-annotation-tables.mdx`

Language support: trilingual stable.

Include:

- Load an ARC.
- Select study/assay tables.
- Export each table to ARCtrl JSON.
- Mention Swate import/export compatibility.

Snippet candidates:

- `recipe.export-study-tables`
- `recipe.export-assay-tables`

### `recipes/create-arc-from-sops.mdx`

Language support: trilingual stable where table/template APIs permit.

Include:

- Load reusable SOP annotation tables.
- Create a new investigation/study/assay.
- Insert SOP tables.
- Write or serialize the ARC.

Snippet candidates:

- `recipe.sop-load-table`
- `recipe.sop-build-arc`

### `recipes/transform-external-data.mdx`

Language support: trilingual stable.

Include:

- Convert external domain data to ontology annotations.
- Create rows for sources/samples/data files.
- Add characteristics, factors, parameters, and outputs.
- Pattern based on brapi2arc and elab2arc.

Snippet candidates:

- `recipe.external-data-to-table`
- `recipe.external-data-to-assay`

### `recipes/update-existing-arc.mdx`

Language support: trilingual stable.

Include:

- Load ARC.
- Find study/assay.
- Get existing table.
- Add missing columns/rows.
- Update cells.
- Replace table on parent object.
- Write/update metadata.

Snippet candidates:

- `recipe.update-existing-table`
- `recipe.update-study-metadata`

### `recipes/export-ro-crate-summary.mdx`

Language support: F# first; TypeScript/Python support depends on stable high-level
RO-Crate APIs.

Include:

- Load ARC.
- Filter file system tree.
- Convert to RO-Crate metadata.
- Optionally create summary markdown or list registered payload files.
- Pattern based on arc-export.

Snippet candidates:

- `recipe.rocrate-export`
- `recipe.rocrate-filter-hidden`

## Public API Shape Work Required

Before implementing each page, extend or verify
`docs/api-shape/arctrl.public-api.generated.json` for the required public
members.

High-priority shapes:

- `ARC`
- `ArcInvestigation`
- `ArcStudy`
- `ArcAssay`
- `ArcRun`
- `ArcWorkflow`
- `ArcTable`
- `ArcTables`
- `CompositeHeader`
- `CompositeCell`
- `CompositeColumn`
- `IOType`
- `OntologyAnnotation`
- `Person`
- `Publication`
- `Comment`
- `Datamap`
- `DataContext`
- `JsonController`
- `XlsxController`
- `YamlController`
- `Contract`
- `FileSystem`
- `FileSystemTree`
- `Template`
- `Templates`
- `WebController`

Advanced shapes:

- `ROCrate.LDNode`
- `ROCrate.LDGraph`
- `ROCrate.LDRef`
- `ROCrate.LDValue`
- `ROCrate.LDContext`
- `ROCrate.Conversion`
- `CWL.CWLProcessingUnit`
- `CWL.CWLType`
- `CWL.CWLInput`
- `CWL.CWLOutput`
- `CWL.CWLToolDescription`
- `CWL.CWLWorkflowDescription`
- `CWL.WorkflowStep`
- `CWL.StepInput`
- `CWL.StepOutput`
- `CWL.Requirement`
- `CWL.DockerRequirement`
- Validation package types
- Workflow graph types and builders

## Translation Tooling Implications

The content plan requires expanding the snippet translator in stages.

Milestone A:

- Object construction.
- Static create/init methods.
- Basic property assignment.
- Arrays/lists.
- Instance method calls.
- Assertions.

Milestone B:

- Optional/named arguments where public APIs rely on them.
- Collection mutation patterns.
- JSON roundtrip helpers.
- Temporary file/directory setup for IO snippets.

Milestone C:

- Contract DTO construction.
- FileSystemTree callbacks.
- Table update/query examples.
- Datamap examples.

Milestone D:

- Advanced namespaces:
  - `ROCrate`
  - `CWL`
  - validation/YAML
  - workflow graph
- Allow explicit TypeScript/Python overrides for pages where deterministic
  translation is not yet practical.

## Initial Implementation Order

1. `setup.mdx`
2. `quickstart.mdx`
3. `tables/arc-table.mdx`
4. `tables/headers-and-cells.mdx`
5. `tables/rows-columns-and-updates.mdx`
6. `core/isa-objects.mdx`
7. `io/filesystem-tree.mdx`
8. `io/local-arc-io.mdx`
9. `serialization/arctrl-json.mdx`
10. `datamaps/datamap.mdx`

After those are stable, add:

1. `io/contracts.mdx`
2. `serialization/isa-json.mdx`
3. `serialization/ro-crate.mdx`
4. `templates/validation-packages.mdx`
5. `workflows/runs-and-workflows.mdx`

Advanced publication can follow later:

1. `serialization/json-ld-graph.mdx`
2. `workflows/cwl.mdx`
3. `workflows/workflow-graph.mdx`
4. all recipes

## Quality Gates

- Every visible code block is generated from, or linked to, an executable
  snippet.
- No rendered examples use obsolete Python imports.
- No rendered TypeScript examples import raw generated paths.
- Each page includes at least one assertion-backed snippet unless it is prose
  only.
- IO examples use temp paths or in-memory contracts, never user-specific paths.
- Serialization examples assert roundtrip identity for identifiers and key
  collection counts.
- Advanced pages clearly state language availability when not trilingual.
- Existing knowledgebase pages should be migrated only after their examples pass
  through the new snippet pipeline.
