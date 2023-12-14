# Tables

🔗 The script files for this documentation can be found here:
- [JavaScript](./scripts_js/Tables.js)
- [F#](./scripts_fsharp/Tables.fsx)

The Tables shown in xlsx files in ISA files are modelled in ARCtrl as `ArcTable` object. These objects are assigned as `ArcTables` interface in both `ArcAssay` and `ArcStudy`.

`ArcTable` has the following fields:
- `Name`: Unique (per `ArcTables`) identifier.
- `Headers`: Array of `CompositeHeader`.
- `Values` (might be changed in the future.): A Dictionary of key value pairs. Keys are of type `int * int`, representing `columnIndex * rowIndex` of the cell. The value is of type `CompositeCell`.

`ArcTable`s are built on the system, that each "column" (built from a `CompositeHeader` and all `CompositeCell`s of the same column index) create a `CompositeColumn`, which can be translated to all types of columns in xlsx format. Each `CompositeColumn` may translate to 1, 3 or 4 xlsx columns, depending on the column type. 

🔗 Check out the code examples for a minimal table manipulation example!