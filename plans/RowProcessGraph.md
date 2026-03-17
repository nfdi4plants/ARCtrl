# Row-Process Graph for ARC Table Editing

## 1) Goal

Create a Fable-compatible intermediate editing model for `ArcInvestigation` that:

- preserves `ArcTable` roundtrip fidelity,
- treats each physical table row as one editable process-row,
- derives connectivity from IO name equality,
- supports table-local grouping, selection-scoped grouping, and successive subgrouping,
- supports high-level edits on grouped row subsets,
- converts back into the default ARCtrl table model.

This type is additive. It replaces nothing in the existing `GetProcesses()` / `ProcessSequence` APIs and is not intended to change current ISA-JSON or RO-Crate serialization behavior.

## 2) Repository Fit and Constraints

- Source type: `ArcInvestigation`
- Wrapper target: `ARC`
- Compatibility target:
  - this model is defined against raw `ArcTable` / `ArcInvestigation` editing semantics
  - it is intentionally not defined as a compatibility layer over either current row-to-process conversion stack
- Canonical backing storage remains mutable `ArcTable` instances
- Public API must be Fable-safe:
  - use records, unions, arrays, `ResizeArray`, `Result`
  - avoid .NET-specific interfaces in the public surface
- Internal implementation may use mutable dictionaries and caches
- Row order must remain stable and reflect original physical table order
- The model is editor-oriented and should expose renderer-friendly row/table views

## 3) Scope

Included tables:

- all `ArcTable` instances found in `Study`, `Assay`, and `Run`
- tables may have:
  - one Input and one Output column,
  - only Input,
  - only Output,
  - neither Input nor Output

Out of scope for v2:

- replacing existing `ProcessSequence` conversion APIs
- persistent UI state or rendering logic
- automatic global entity rename propagation across all tables
- changing current ARCtrl table serialization semantics

## 4) Locked Decisions

- Canonical unit: `1 row = 1 process-row`
- Physical row order is semantically relevant for editor presentation and must stay stable
- Connectivity uses exact string equality of IO names
- Cross-container links are automatic:
  - if `outputName = inputName`, rows are linked regardless of Study/Assay/Run boundary
- Empty entity name `""` is unlinked
- No implicit trimming or case normalization
- Fan-in is allowed:
  - many rows may produce the same output name
- Fan-out is allowed:
  - many rows may consume the same input name
- Partial rows are allowed:
  - input-only rows may have predecessors but no successors
  - output-only rows may have successors but no predecessors
  - rows with neither IO are disconnected editor rows
- Grouping supports two scopes:
  - table scope over one table / one table schema
  - selection scope over multiple selected containers and/or tables
- Selection-scoped grouping always returns groups partitioned by table
- Selection-scoped groups may be edited in one operation across all returned table partitions
- Same-table logical descriptor duplicates are invalid for v2:
  - selector-addressable `Parameter` / `Factor` / `Characteristic` / `Component` / `Comment` / `FreeText` columns must be unique within one table by logical key
- Missing grouping values are not emitted as a normal group
- Rows excluded from groups because of missing values are returned as remainder rows
- Lineage ambiguity rule:
  - repeated reachable values that normalize to the same `ValueKey` are acceptable
  - distinct reachable values are an error for that row/key
- Lineage traversal is symmetric over the full reachable set:
  - no attempt is made to infer one intended branch or one preferred predecessor/successor path
- Reachability and lineage are cycle-safe:
  - traversal uses visited sets
  - 0-hop self is excluded
  - self is excluded from returned reachable sets and lineage evaluation even if a cycle reaches it again
- Grouped edits use overwrite semantics only
- Source-row materialization operations are first-class:
  - selected row outputs may be materialized as inputs of rows in a new or existing downstream table
  - selected row inputs may be reused to create additional rows with new outputs
- Adding a column for a grouped subset means:
  - add a normal table-wide column
  - write edited values to targeted rows
  - leave non-target rows empty
- Grouping is a derived cached view, not the canonical mutable state

## 5) Main Use Cases

### 5.1 Table-Less Table Editor

- show one logical row list per source table
- keep table-local schema
- display predecessor/successor and lineage-derived context
- edit one row, selected rows, or grouped subsets

### 5.2 Successive High-Level Editing

- group table rows by one key
- select one produced group
- group that subset again by another key
- edit only the resulting subgroup
- regroup later and retain edits because edits were written to rows

### 5.3 Editing the Remainder

- group rows by a descriptor
- rows with missing key are excluded from normal groups
- edit grouped rows now
- later take the `RemainderRows` and group them by another descriptor

### 5.4 Lineage-Aware Editing

- group rows by a value found on reachable upstream or downstream rows
- if distinct lineage values are reachable for the same target row/key, return an error and exclude that row from groups

### 5.5 Draft / Incomplete Tables

- include tables without both IO columns
- allow normal descriptor grouping and editing even when connectivity is partial or absent

### 5.6 Higher-Level Selection Grouping

- select all tables, or a subset of studies / assays / runs / tables
- group rows across that selection by shared self or lineage keys
- receive each resulting group partitioned by table
- edit all returned table partitions of one logical group in one operation
- preserve table-local schemas and write edits back to each underlying table separately

### 5.7 Create Downstream Tables From Selected Outputs

- select rows from one table or from a selection-scoped grouping
- take each selected row's output as the input of a new downstream row
- create a new table or use an existing table as the target
- preserve source row order when materializing rows into the target
- optionally leave target outputs empty for later editing

### 5.8 Add New Outputs For Existing Inputs

- select rows whose existing inputs should branch into additional outputs
- create new rows that reuse the selected rows' input names
- assign new output names to those new rows
- allow branching inside an existing table or into another target table
- the source rows may already have outputs, or may currently have no output at all

## 6) Conceptual Model

### 6.1 Row Model

- Each included physical row becomes one row node
- A row node is anchored to:
  - container
  - table
  - stable `RowId`
  - current `RowIndex`

### 6.2 Entity Connectivity

For a row `r`:

- `InputName = row input cell -> ToFreeTextCell().AsFreeText`
- `OutputName = row output cell -> ToFreeTextCell().AsFreeText`

Connectivity:

- predecessor of `r`: any row whose `OutputName = r.InputName`
- successor of `r`: any row whose `InputName = r.OutputName`

This relation is global across the full investigation.

Lineage traversal uses the full reachable predecessor/successor set under this relation.
It does not attempt to collapse fan-in/fan-out into a single branch.
Traversal is cycle-safe and excludes self from reachable and lineage results.
As a consequence, a pure self-loop does not contribute lineage and evaluates as no reachable lineage.

### 6.3 Table-Local Grouping

Table-local grouping operates on a row subset belonging to one table.

Reason:

- grouped rows in the editor keep table-local schemas
- selector resolution is table-bound
- grouped edits add or update columns on one concrete table

### 6.4 Selection-Scoped Grouping

Selection-scoped grouping operates over multiple tables chosen by scope.

Rules:

- grouping evaluation may span multiple selected tables
- grouping keys are shared across the whole selection
- returned groups are partitioned by table
- no returned group may mix rows from different tables in the same partition
- simultaneous editing of one logical group is implemented by applying the same edit to all of its table partitions

### 6.5 Canonical State vs Derived Views

Canonical mutable state:

- copied or referenced `ArcTable` instances
- stable row identity and row metadata
- selector resolution caches
- lineage caches

Derived state:

- row subsets
- grouping results
- table view rows

Derived state may be recomputed and cached, but edits always apply to canonical rows.

### 6.6 Source-Row Materialization

High-level row creation helpers operate by projecting one chosen IO endpoint from source rows into the
Input column of newly created target rows.

Rules:

- source rows may come from one table or from any selection across the investigation
- target rows are always created in one concrete target table
- source row order determines created target row order
- `Output`-materialization:
  - source row `OutputName` becomes target row `InputName`
- `Input`-materialization:
  - source row `InputName` becomes target row `InputName`
- if the chosen source endpoint is missing or `""` for any selected source row, the operation fails
- optional target output names may be provided:
  - if provided, the count must equal the number of source rows
  - if omitted, created target rows receive empty outputs

## 7) Types

Add `src/Core/Process/ArcRowProcessGraph.fs` in `namespace ARCtrl.Process`.

### 7.1 References and IDs

- `type ContainerRef = Study of string | Assay of string | Run of string`
- `type TableRef = { Container: ContainerRef; TableName: string }`
- `type RowId = int`
- `type EntityName = string`
- `type SourceEndpoint = InputEndpoint | OutputEndpoint`
- `type SelectionScope =
    | All
    | Containers of ContainerRef array
    | Tables of TableRef array`

### 7.2 Descriptor Selection

- `type ColumnRef = { Table: TableRef; ColumnIndex: int; SchemaVersion: int }`
- `type DescriptorKey =
    | ProtocolREF | ProtocolType | ProtocolDescription | ProtocolUri | ProtocolVersion
    | Performer | Date
    | Parameter of string
    | Factor of string
    | Characteristic of string
    | Component of string
    | Comment of string
    | FreeText of string`
- `type DescriptorSelector =
    | Column of ColumnRef
    | Key of DescriptorKey`

Resolution rules:

- `Key` resolves to `0..1` physical columns within the row's table
- matching is case-sensitive and untrimmed
- term keys match `OntologyAnnotation.NameText`
- `Comment` matches the raw comment key
- `FreeText` matches the exact raw header string
- protocol fields, `Performer`, and `Date` are unique by header kind
- v2 invariant:
  - if multiple physical columns in one table would resolve to the same logical key, graph build fails
- selector edit semantics:
  - `Key`-based edits update all resolved columns in physical column order
  - `ColumnRef` edits update exactly one physical column
- `EnsureColumn` semantics:
  - if the target logical column already exists in the table, reuse it
  - otherwise append one new physical column
  - because same-table logical duplicates are invalid in v2, `EnsureColumn` never has to choose between multiple existing matches

### 7.3 Group Keys

- `type LineageDirection = Upstream | Downstream`
- `type RowGroupKey =
    | Self of DescriptorSelector
    | Lineage of LineageDirection * DescriptorSelector`

### 7.4 Value Keys

- `type ValueKey = Missing | Value of string`

Normalization:

- use `CompositeCell.GetContent()` without trimming or case normalization
- if all parts are exactly `""`, normalize to `Missing`
- otherwise encode deterministically using a collision-safe length-prefixed format
- multi-column selectors normalize to one combined `ValueKey`

### 7.5 Row and Group Views

- `type RowSet = { Table: TableRef; Rows: RowId array }`
- `type GroupView = { Key: ValueKey list; Rows: RowId array }`
- `type GroupingView =
    { Source: RowSet
      Keys: RowGroupKey list
      Groups: GroupView array
      RemainderRows: RowId array
      RowErrors: GraphError array }`
- `type PartitionedGroupView =
    { Key: ValueKey list
      Partitions: RowSet array }`
- `type SelectionGroupingView =
    { Scope: SelectionScope
      Keys: RowGroupKey list
      Groups: PartitionedGroupView array
      RemainderPartitions: RowSet array
      RowErrors: GraphError array }`
- `type TableViewRow =
    { RowId: RowId
      RowIndex: int
      InputName: string option
      OutputName: string option
      PredecessorCount: int
      SuccessorCount: int
      HasUpstream: bool
      HasDownstream: bool }`

### 7.6 Errors

- `type GraphError =
    | MissingContainer of ContainerRef
    | MissingTable of TableRef
    | UnknownRow of RowId
    | UnknownColumn of ColumnRef
    | StaleColumnRef of ColumnRef * expectedSchemaVersion:int * actualSchemaVersion:int
    | InvalidSelectorForTable of TableRef * DescriptorSelector
    | DuplicateLogicalColumns of TableRef * DescriptorKey * int array
    | MissingSourceEndpointValue of RowId * SourceEndpoint
    | InvalidOperation of string
    | AmbiguousLineageValue of RowId * RowGroupKey * ValueKey list`

## 8) Public API

`ArcRowProcessGraph` public members should stay Fable-safe and avoid exposing live mutable `ArcTable` references directly.

- `type ArcRowProcessGraph =
    static member OfInvestigation : ArcInvestigation * ?copyTables:bool * ?maxCacheEntries:int -> ArcRowProcessGraph
    member TableRefs : TableRef array
    member GetTableSnapshot : TableRef -> Result<ArcTable, GraphError>
    member CreateTable : ContainerRef * tableName:string * ?input:IOType * ?output:IOType -> Result<TableRef, GraphError>
    member GetTableRows : TableRef -> Result<RowId array, GraphError>
    member GetRowSet : TableRef -> Result<RowSet, GraphError>
    member GetSelectionRowSets : SelectionScope -> Result<RowSet array, GraphError>
    member GetTableView : TableRef -> Result<TableViewRow array, GraphError>
    member ResolveSelectorColumns : TableRef * DescriptorSelector -> Result<ColumnRef array, GraphError>
    member GetRowLocation : RowId -> Result<TableRef * int, GraphError>
    member GetRowIO : RowId -> Result<string option * string option, GraphError>
    member GetPredecessors : RowId -> Result<RowId array, GraphError>
    member GetSuccessors : RowId -> Result<RowId array, GraphError>
    member GetReachableUpstream : RowId -> Result<RowId array, GraphError>
    member GetReachableDownstream : RowId -> Result<RowId array, GraphError>
    member TryGetCells : RowId * DescriptorSelector -> Result<CompositeCell array, GraphError>
    member GroupTableRows : TableRef * RowGroupKey list -> Result<GroupingView, GraphError>
    member GroupRowSet : RowSet * RowGroupKey list -> Result<GroupingView, GraphError>
    member GroupSelectionRows : SelectionScope * RowGroupKey list -> Result<SelectionGroupingView, GraphError>
    member SetCell : RowId * ColumnRef * CompositeCell -> Result<unit, GraphError>
    member SetCellsForRows : RowId array * DescriptorSelector * CompositeCell -> Result<int, GraphError>
    member EnsureColumn : TableRef * CompositeHeader * ?index:int -> Result<ColumnRef, GraphError>
    member EnsureColumnForRowSets : RowSet array * CompositeHeader * ?index:int -> Result<ColumnRef array, GraphError>
    member AddRowsFromSourceRows :
      targetTable:TableRef * sourceRows:RowId array * sourceEndpoint:SourceEndpoint * ?outputNames:string array
      -> Result<RowId array, GraphError>
    member CreateTableFromSourceRows :
      container:ContainerRef * tableName:string * sourceRows:RowId array * sourceEndpoint:SourceEndpoint * ?input:IOType * ?output:IOType * ?outputNames:string array
      -> Result<TableRef * RowId array, GraphError>
    member BranchOutputsFromRows :
      targetTable:TableRef * sourceRows:RowId array * outputNames:string array
      -> Result<RowId array, GraphError>
    member SetInputName : RowId * string -> Result<unit, GraphError>
    member SetOutputName : RowId * string -> Result<unit, GraphError>
    member AddRow : TableRef * ?inputName:string * ?outputName:string -> Result<RowId, GraphError>
    member InsertRow : TableRef * rowIndex:int * ?inputName:string * ?outputName:string -> Result<RowId, GraphError>
    member DeleteRow : RowId -> Result<unit, GraphError>
    member BeginBatch : unit -> Result<unit, GraphError>
    member EndBatch : unit -> Result<unit, GraphError>
    member ClearCaches : unit -> unit
    member ApplyToInvestigation : ArcInvestigation -> Result<unit, GraphError>`

Snapshot rule:

- `GetTableSnapshot` returns a deep copy of the current table state
- no public member may expose a live mutable `ArcTable` owned by the graph
- `TryGetCells` returns deep-copied `CompositeCell` values
- returned cell values must never alias live mutable graph state

IO safety rule:

- `SetCell` on Input/Output columns is rejected
- IO edits must go through `SetInputName` / `SetOutputName`
- for `IOType.Data` rows:
  - preserve the existing `Data` object
  - mutate only its `Name` / path-facing identity
  - preserve `Format` and `SelectorFormat`

Wrapper in `src/ARCtrl`:

- `type ARC with
    member this.ToRowProcessGraph : ?copyTables:bool * ?maxCacheEntries:int -> ArcRowProcessGraph
    member this.ApplyRowProcessGraph : ArcRowProcessGraph -> Result<unit, GraphError>`

## 9) Grouping and Editing Semantics

### 9.1 Grouping

Grouping evaluation for a row:

- compute one normalized `ValueKey` per requested `RowGroupKey`
- if any requested key is `Missing`, the row is excluded from normal groups and added to `RemainderRows`
- if a lineage key has no reachable lineage rows, that lineage key evaluates to `Missing`
- if any requested lineage key resolves to distinct reachable values, add `AmbiguousLineageValue` to `RowErrors`
- rows in `RowErrors` are excluded from both normal groups and remainder rows

Group ordering:

- groups are returned ordered by the physical row order of their first member
- rows inside each group are returned in physical row order

### 9.2 Selection-Scoped Grouping

Selection-scoped grouping:

- starts from `GetSelectionRowSets`
- evaluates grouping keys over all rows in the selected scope
- merges rows with the same logical group key across the whole selection
- returns each logical group as `Partitions: RowSet array`
- partitions are ordered by deterministic container/table order
- rows inside each partition remain in physical table order

Selection remainder handling:

- rows with missing grouping values are returned as `RemainderPartitions`
- remainder is also partitioned by table

Simultaneous editing across partitions:

- create missing columns across all affected table partitions with `EnsureColumnForRowSets`
- flatten all target row ids from the partitions
- apply one `SetCellsForRows` call with a `Key` selector
- `ColumnRef` is table-specific and is not valid for simultaneous multi-table partition edits
- edits are written separately to each underlying table

### 9.3 Successive Grouping

Successive grouping works by grouping a derived `RowSet`:

- group full table
- take one `GroupView.Rows` or `RemainderRows`
- build a new `RowSet`
- group that subset again

No special persistent grouping tree is required in canonical storage.

Selection-scoped successive grouping works by:

- selecting one `PartitionedGroupView`
- regrouping each returned partition or the union of its rows by the next key
- returning the next result again partitioned by table

### 9.4 Source-Row Materialization

Downstream materialization:

- `AddRowsFromSourceRows` with `sourceEndpoint = OutputEndpoint` creates new target rows whose
  inputs are taken from the selected source rows' outputs
- this supports:
  - adding downstream rows into an existing table
  - building a new table from selected outputs through `CreateTableFromSourceRows`

Input reuse / branching:

- `AddRowsFromSourceRows` with `sourceEndpoint = InputEndpoint` creates new target rows whose
  inputs are taken from the selected source rows' inputs
- `BranchOutputsFromRows` is the convenience operation for this workflow
- this supports adding new outputs for existing inputs while keeping `1 row = 1 process-row`
- the source rows do not need to have an existing output; input-only rows are valid sources for this operation

Materialization ordering:

- created rows preserve source row order
- if `outputNames` are supplied, they are assigned positionally to created rows
- if `outputNames` are omitted, created rows receive empty outputs

### 9.5 Grouped Edits

Grouped edits are row-subset edits:

- target rows are supplied directly as `RowId array`
- edit mode is overwrite only
- when a required column does not exist:
  - call `EnsureColumn` to create it table-wide
  - write edited values into target rows
  - leave all other rows empty

This guarantees that later subgroup edits on previously empty rows remain possible.

## 10) Ordering Rules

Ordering must preserve editor expectations.

- `GetTableRows`, `GetTableView`, `GroupView.Rows`, and `RemainderRows` return rows in physical table order
- predecessor/successor/reachability results use deterministic global order:
  - Study containers in investigation order
  - then Assay containers in investigation order
  - then Run containers in investigation order
  - inside each container: table order
  - inside each table: row order
- this ordering is derived from current container/table/row positions, not from a separately stored ordinal field

## 11) Algorithms and Performance

Target scale: tens of thousands of rows.

### 11.1 Internal Indices

- `RowIdsByTable : Dictionary<TableRef, ResizeArray<RowId>>`
- `RowMetaById : ResizeArray<RowMeta option>`
- `ProducerRowsByName : Dictionary<EntityName, ResizeArray<RowId>>`
- `ConsumerRowsByName : Dictionary<EntityName, ResizeArray<RowId>>`
- `TableStateByRef : Dictionary<TableRef, TableState>`
- `TableSchemaVersion : Dictionary<TableRef, int>`
- `ConnectivityVersion : int`

`RowMeta`:

- `{ Table: TableRef
     mutable RowIndex: int
     mutable InputName: string option
     mutable OutputName: string option }`

`TableState`:

- current IO column indices
- selector resolver cache

### 11.2 Stable Row Mutation

Because row order must remain stable:

- insert row:
  - insert into `ArcTable`
  - insert `RowId` at same index in `RowIdsByTable`
  - update `RowIndex` for affected suffix rows
- delete row:
  - remove from `ArcTable` with order-preserving delete
  - remove `RowId` from `RowIdsByTable`
  - mark row metadata as deleted
  - update `RowIndex` for affected suffix rows

These operations are O(number of rows after the edited position) by design.

### 11.3 Caching Strategy

Cache selector projections and lineage projections, not group trees.

Recommended caches:

- resolved selector columns per table/schema version
- self value projection per `(TableRef, DescriptorSelector, SchemaVersion)`
- lineage value projection per `(TableRef, LineageDirection, DescriptorSelector, ConnectivityVersion, SchemaFingerprint)`

Grouping then becomes:

- scan selected `RowId`s
- read cached `ValueKey`s
- assemble `Groups`, `RemainderRows`, `RowErrors`

This supports successive grouping efficiently without storing persistent nested group state in the model.

Connectivity invalidation rule:

- any IO rename or row mutation that changes global connectivity increments `ConnectivityVersion`
- lineage caches for all tables become stale when `ConnectivityVersion` changes

### 11.4 Batch Mode

- `BeginBatch` defers cache publication/invalidation
- `EndBatch` publishes one invalidation pass

### 11.5 Cache Capacity

- `maxCacheEntries` default: 256
- evict least-recently-used entries above capacity

## 12) Roundtrip and Ownership

Ownership modes:

- `copyTables=true`:
  - graph owns copied tables
  - original investigation is unchanged until apply/writeback
- `copyTables=false`:
  - graph mutates referenced tables directly
  - still keep graph-managed invariants and cache invalidation

`ApplyToInvestigation`:

- locate container by identifier
- replace table with same `TableName` if present
- otherwise insert at end
- untouched tables remain unchanged

The graph is intended to be built from the base type, edited through the graph API, and then written back. External concurrent mutation of the same live tables is not a supported scenario.

## 13) Implementation Sequence

Implement in this order to keep each phase independently testable and to minimize churn.

### 13.1 Phase 0: Project Wiring

1. Add `src/Core/Process/ArcRowProcessGraph.fs`.
2. Wire compile order in `src/Core/ARCtrl.Core.fsproj` after `Conversion.fs`.
3. Add `src/ARCtrl/RowProcessGraph.fs` with `ARC` extension methods.
4. Wire `src/ARCtrl/RowProcessGraph.fs` after `ARC.fs` in:
   - `src/ARCtrl/ARCtrl.fsproj`
   - `src/ARCtrl/ARCtrl.Javascript.fsproj`
   - `src/ARCtrl/ARCtrl.Python.fsproj`
5. Add `tests/Core/ArcRowProcessGraph.Tests.fs` and register it in `tests/Core/Main.fs`.

Deliverable:

- projects compile with stub graph types and empty test module registration

### 13.2 Phase 1: Core Types and Build

1. Add public types:
   - `ContainerRef`
   - `TableRef`
   - `RowId`
   - `SourceEndpoint`
   - `SelectionScope`
   - `ColumnRef`
   - `DescriptorKey`
   - `DescriptorSelector`
   - `LineageDirection`
   - `RowGroupKey`
   - `ValueKey`
   - `RowSet`
   - `GroupView`
   - `GroupingView`
   - `PartitionedGroupView`
   - `SelectionGroupingView`
   - `TableViewRow`
   - `GraphError`
2. Add internal state records:
   - `RowMeta`
   - `TableState`
   - cache entry records as needed
3. Implement `OfInvestigation`:
   - enumerate Study / Assay / Run tables in deterministic order
   - optionally copy tables
   - assign `TableRef`
   - assign stable `RowId`
   - populate `RowIdsByTable`
   - populate `RowMetaById`
   - initialize schema versions
   - validate same-table logical selector uniqueness
4. Implement `TableRefs`, `GetTableSnapshot`, `CreateTable`, `GetTableRows`, `GetRowSet`, `GetSelectionRowSets`, `GetRowLocation`.

Deliverable:

- graph can be built from an investigation
- every included table and row can be enumerated deterministically

### 13.3 Phase 2: IO Extraction and Connectivity

1. Implement table IO column discovery:
   - `0..1` input column
   - `0..1` output column
2. Implement row IO extraction via `ToFreeTextCell().AsFreeText`.
3. Build `ProducerRowsByName` and `ConsumerRowsByName`.
4. Implement:
   - `GetRowIO`
   - `GetPredecessors`
   - `GetSuccessors`
   - `GetReachableUpstream`
   - `GetReachableDownstream`
5. Add tests for:
   - no-IO rows
   - input-only rows
   - output-only rows
   - fan-in
   - fan-out
   - cross-container linkage
   - cycle-safe traversal
   - self exclusion

Deliverable:

- graph adjacency works for all row shapes and container boundaries

### 13.4 Phase 3: Selector Resolution and Value Normalization

1. Implement `ResolveSelectorColumns`.
2. Implement `TryGetCells`.
3. Implement `ValueKey` normalization for:
   - `FreeText`
   - `Term`
   - `Unitized`
   - `Data`
   - multi-column selector sequences
4. Implement stale `ColumnRef` validation as part of schema-aware read/edit paths.
5. Cache selector resolution by table/schema version.
6. Add tests for:
   - build failure on same-table duplicate logical descriptor columns
   - comment key matching
   - free-text header matching
   - protocol/date/performer matching
   - `Key` vs `ColumnRef` resolution behavior
   - `TryGetCells` copy isolation

Deliverable:

- selectors resolve deterministically and produce stable normalized keys

### 13.5 Phase 4: Table Views and Basic Row-Subset Operations

1. Implement `GetTableView`.
2. Populate `TableViewRow` with:
   - `RowId`
   - `RowIndex`
   - `InputName`
   - `OutputName`
   - predecessor/successor counts
   - upstream/downstream flags
3. Add helpers for building row subsets from returned row arrays.
4. Add tests for stable table view order and row metadata correctness.

Deliverable:

- renderer-friendly table views are available before advanced grouping/editing

### 13.6 Phase 5: Grouping Engine

1. Implement self-key grouping on a `RowSet`.
2. Implement lineage-key grouping:
   - compute lineage projection
   - collapse repeated equal values
   - error on distinct values
   - treat no reachable lineage as `Missing`
   - treat all reachable branches symmetrically, with no preferred path
3. Implement `GroupRowSet`.
4. Implement `GroupTableRows` as `GetRowSet >> GroupRowSet`.
5. Implement `GroupSelectionRows`:
   - resolve selected row sets
   - evaluate keys across the whole selection
   - return each logical group partitioned by table
6. Enforce grouping result semantics:
   - no `Missing` group
   - `Missing` rows go to `RemainderRows`
   - ambiguous lineage rows go to `RowErrors`
   - groups ordered by first member row
7. Add tests for:
   - self grouping
   - lineage grouping
   - remainder behavior
   - successive subgrouping
   - selection-scoped grouping across multiple tables
   - selection-scoped grouping over selected studies / assays / runs only
   - deterministic group ordering

Deliverable:

- table-local grouping, selection-scoped grouping, and successive grouping are usable for editor workflows

### 13.7 Phase 6: Editing API

1. Implement `SetCell` with IO-column rejection.
2. Implement `SetCellsForRows`:
   - resolve selector
   - overwrite all resolved columns for each targeted row
3. Implement `EnsureColumn`.
4. Implement `EnsureColumnForRowSets` for simultaneous multi-table edits.
5. Implement `AddRowsFromSourceRows`.
6. Implement `CreateTableFromSourceRows`.
7. Implement `BranchOutputsFromRows`.
8. Define grouped column creation as:
   - `EnsureColumn`
   - then `SetCellsForRows`
9. Define selection-group edit creation as:
   - `EnsureColumnForRowSets`
   - flatten partition rows
   - then `SetCellsForRows`
10. Implement `SetInputName` and `SetOutputName`:
   - write table cell
   - update row metadata
   - update producer/consumer indices
   - preserve existing `Data` objects for `IOType.Data`
11. Add tests for:
   - overwrite behavior
   - key-based edits over allowed logical keys
   - column-ref single-column edits
   - grouped column creation via `EnsureColumn` + `SetCellsForRows`
   - selection-group edit creation via `EnsureColumnForRowSets` + `SetCellsForRows`
   - downstream row materialization from selected outputs into an existing table
   - downstream table creation from selected outputs
   - branching new outputs from existing inputs
   - branching new outputs from input-only source rows
   - failure on missing selected source endpoint values
   - grouped subset edits leaving non-target rows empty
   - IO rename adjacency updates
   - `IOType.Data` rename preserving `Format` and `SelectorFormat`
   - `copyTables=false` edits mutating backing tables immediately while graph indices stay valid

Deliverable:

- grouped edits, row-subset edits, and source-row materialization helpers work directly against canonical tables

### 13.8 Phase 7: Row Mutation

1. Implement `AddRow`.
2. Implement `InsertRow`.
3. Implement `DeleteRow` using order-preserving table deletion.
4. Update:
   - `RowIdsByTable`
   - suffix `RowIndex` values
   - IO adjacency indices
   - `ConnectivityVersion`
   - affected caches
5. Add tests for:
   - stable row order after insert/delete
   - stable `RowId` for unaffected rows
   - correct table snapshot after mutation

Deliverable:

- row mutation is stable and editor-safe

### 13.9 Phase 8: Cache Invalidation and Batching

1. Add selector caches.
2. Add lineage caches.
3. Add LRU eviction.
4. Implement `BeginBatch`, `EndBatch`, `ClearCaches`.
5. Define invalidation triggers for:
   - descriptor edits
   - schema changes
   - IO edits
   - row insert/delete
6. Add tests for:
   - stale `ColumnRef`
   - cache reuse
   - cache invalidation correctness
   - batch recompute behavior
   - investigation-global lineage invalidation after IO changes in another table

Deliverable:

- repeated grouping/editing workloads are efficient enough for large tables

### 13.10 Phase 9: Roundtrip and Wrapper

1. Implement `ApplyToInvestigation`.
2. Implement `ARC` wrapper methods.
3. Add tests for:
   - `copyTables=true`
   - `copyTables=false`
   - replace existing table
   - insert missing table
   - untouched tables remain unchanged

Deliverable:

- graph can be used as an edit session and written back cleanly

### 13.11 Phase 10: Fable Validation and Documentation

1. Add basic Fable-facing coverage for:
   - create graph
   - enumerate table rows
   - group rows
   - apply grouped edit
   - create table
   - materialize selected source rows into downstream rows
   - branch new outputs from existing inputs
   - write back through wrapper
2. Ensure the above flow is compiled and exercised through the JS and Python targets, not only .NET tests over Fable-safe code.
3. Add concise docs for:
   - row identity
   - grouping vs remainder rows
   - overwrite semantics
   - additive relationship to existing process APIs

Deliverable:

- implementation is usable from dotnet and through Fable targets

## 14) Acceptance Criteria

Must pass:

- Row cardinality:
  - every included physical row becomes one graph row
- Included tables:
  - tables with both IO, input-only, output-only, and no-IO tables are all present
- Connectivity:
  - name-equality links work automatically across Study / Assay / Run
  - fan-in works
  - fan-out works
  - empty IO names never connect
- Stable order:
  - row order is preserved after insert/delete
  - grouped row output keeps physical row order
- Grouping:
  - self grouping by descriptor keys
  - lineage grouping across multiple hops
  - rows with missing grouping key go to `RemainderRows`, not to a `Missing` group
  - rows with no reachable lineage for a lineage key go to `RemainderRows`
  - distinct lineage values produce row errors
  - identical repeated lineage values do not error
  - groups are returned in deterministic first-member row order
  - cycles do not loop indefinitely
  - self is excluded from reachable and lineage results
- Successive grouping:
  - group a full table
  - regroup one returned subgroup
  - regroup remainder rows
- Selection-scoped grouping:
  - group across all selected tables
  - return each logical group partitioned by table
  - allow selection by all tables, selected studies, selected assays, selected runs, or selected tables
  - support simultaneous edit across all partitions of one logical group
- Source-row materialization:
  - create a new table in a selected container
  - materialize selected row outputs as inputs of new downstream rows
  - materialize selected row outputs as inputs of a new downstream table
  - branch new outputs from existing inputs by creating additional rows
  - support branching from input-only source rows that currently have no output
- Grouped editing:
  - overwrite existing values on targeted rows
  - add missing column table-wide
  - non-target rows remain empty
  - regroup later and observe persisted edits
- Partial tables:
  - input-only and output-only rows behave as expected
  - disconnected rows remain editable
- Column references:
  - non-terminal insert invalidates stale refs
- Duplicate logical columns:
  - graph build fails if one table contains duplicate logical selector keys
- Selector edits:
  - `Key` edits update all resolved columns in physical order
  - `ColumnRef` edits update only the referenced column
  - simultaneous multi-table group edits use `Key`, not `ColumnRef`
- `TryGetCells`:
  - returned cells are deep copies and safe to mutate outside the graph
- `IOType.Data` edits:
  - rename preserves `Format` and `SelectorFormat`
- Fable compatibility:
  - public API compiles and remains usable through Fable
  - `ARC` wrapper is available on dotnet, JavaScript, and Python targets
  - basic create / group / edit / apply flows are covered by Fable-facing tests
- Performance:
  - grouping on large row sets uses cached projections
  - repeated successive groupings avoid full recomputation where possible
