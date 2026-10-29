# Next native Python test round

The native Python tests mirror the constructor, factory, table, JSON, and XLSX
coverage added in `tests/JavaScript`. Keep the same behavioral cases when
improving Python usability. This list corresponds to the follow-up work in
`tests/JavaScript/TODO.md`.

## Public imports and dependencies

- [x] Export the nested types used by the tests through `src.ARCtrl` and
  `src.ARCtrl.cwl`, including `Data`, `DataFile`, `Component`, `ScalarValue`,
  `Remark`, `Commit`, `License`, and CWL helpers.
- [ ] Exercise an installed package and its documented submodules, in addition
  to imports from the source checkout.
- [ ] Provide public constructors for every `ScalarValue` case so native tests
  do not need generated case-class imports.
- [ ] Construct CWL/YAML sequences and objects with ordinary Python lists,
  without importing `fable_library.list.of_array` or generated YAML types.
- [ ] Inspect union payloads with public predicates and accessors wherever
  available. Keep case-representation checks separate from native usage checks.
- [ ] Remove required Fable imports from native usage examples while preserving
  coverage of every union case and nested payload.

## Python usability and typing

- [ ] Check consumer-facing constructor, factory, property, and result type hints
  with a dedicated Python type check, including invalid arguments.
- [ ] Document Python record field names and factory names consistently with
  the generated API.
- [ ] Check optional values without requiring Fable-specific option helpers.
- [x] Access `DataFile.AsString` and `InputBinding.Position` through public
  properties.
- [ ] Preserve full value comparisons when replacing generated union payload
  assertions; Python equality for some model classes compares identity.

## Round trips and verification

- [ ] Resolve the two XLSX Datamap TODOs also present in the JavaScript suite:
  missing IDs and absent metadata becoming empty strings or annotations.
- [ ] Verify consumer examples against an installed package, covering imports,
  optional values, collections, and union values.
- [x] Regenerate Python from the current F# sources before verification.
- [x] Run the native pytest suite and report passing and pending cases
  separately.

Regenerate with
`dotnet fable ./src/ARCtrl/ARCtrl.Python.fsproj --lang python --noCache -o src/ARCtrl/py`,
then run `.venv/Scripts/python.exe -m pytest tests/Python` on Windows (or
`.venv/bin/python -m pytest tests/Python` on Unix). The repository's
`runTestsPyNative` build target performs both steps.
