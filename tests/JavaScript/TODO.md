# Next native JavaScript / TypeScript test round

The next round should demonstrate that consumers can use the public API with
useful TypeScript hints and ordinary JavaScript values. Keep low-level generated
representation tests separate from tests of native usability.

## Public imports and Fable dependencies

- [ ] Exercise the published package entry point and documented subpaths, rather
  than relying only on relative imports into `src/ARCtrl/ts`.
- [x] Check access to the nested types used in the tests, including `Data`,
  `DataFile`, `Component`, `ScalarValue`, `Remark`, `Commit`, `License`, and CWL
  helper types. Export missing types through the intended public API.
- [ ] Read optional component values without importing Fable `Option.value` or
  handling `Some<T>` in consumer code (`Component.test.ts`).
- [ ] Construct CWL/YAML sequences and objects from native arrays without
  importing Fable `List.ofArray` (`CWLConstructors.test.ts`).
- [ ] Avoid imports from versioned `fable_modules` paths and generated `$cases`
  types in native usage examples. Record missing public alternatives as API gaps.

## Type hints and helpers

- [ ] Ensure consumer-facing constructors, factories, properties, and results
  retain concrete types; do not use `any`, `as any`, or suppression comments to
  make native usage compile.
- [ ] Check inferred types as well as explicit annotations. In particular, cover
  `IOType.Source`, `CompositeHeader.ProtocolREF`, and `DataFile.RawDataFile`, whose
  generated static values were typed `any` during the review.
- [ ] Prefer typed factories where available and verify their return types.
- [ ] Replace `expectProperties`' cast to `Record<string, unknown>` with direct
  property assertions or a helper that checks property names and value types
  against the actual model.
- [ ] Keep union case and payload types correlated; a case table must not accept
  a payload belonging to a different case.
- [ ] Add focused compile-time checks for invalid arguments and properties so
  accidental loss of typing is detected even when runtime tests still pass.

## Discriminated unions

- [ ] Construct core union values through named factories instead of numeric
  `new Union(tag, fields)` calls in native usage tests.
- [ ] Inspect core values through public predicates and accessors instead of
  `.tag`, `.fields`, or `.cases()`: for example, `isUnitized` / `AsUnitized`,
  `isData` / `AsData`, header predicates / `TryGetTerm()`, and DataFile predicates
  / `AsString`.
- [ ] Apply those checks to table operations and JSON/XLSX round trips as well as
  standalone constructors (`ArcTable.test.ts`, `JsonController.test.ts`, and
  `XlsxController.test.ts`).
- [ ] Check native construction and inspection of `ScalarValue` and CWL unions;
  list any cases that still require numeric tags or positional payload access.
- [ ] Preserve coverage of every union case and nested payload when replacing
  representation assertions with public API assertions.

## Public property access

- [ ] Verify all DataFile cases use the public `.AsString` getter and no longer
  depend on the emitted `DataFile__get_AsString` helper. The current working-tree
  tests already use this getter; verify it after regeneration.
- [ ] Read `InputBinding.Position` through its public property. Do not inspect
  `_position` to work around the missing getter in `CWLConstructors.test.ts`.
- [ ] Avoid generated free-function member helpers and backing fields in native
  usage tests; expose a public alternative wherever one is missing.

## Validation and completion

- [ ] Include `tests/JavaScript/**/*.test.ts` in a dedicated TypeScript check;
  the current root `tsconfig.json` includes only `src/ARCtrl/index.ts`.
- [ ] Run the native usage type checks with strict null checking and implicit
  `any` detection. Resolve diagnostics instead of weakening the checks.
- [ ] Regenerate TypeScript from the F# sources before the final run, so the
  checks exercise the current generated API.
- [ ] Run the native Vitest suite and report failures, skips, and todos separately.
- [ ] Verify a consumer example against the built package and its declarations,
  covering imports, optional values, collections, and union values.
- [ ] Finish with no required Fable imports, untyped workarounds, numeric union
  tags, or backing-field access in the native usage examples. Document any
  remaining API gaps explicitly rather than treating workarounds as a pass.
