| Name | Description | CPU | JavaScript Time (ms) |
| --- | --- | --- | --- |
| Table_GetHashCode | From a table with 1 column and 10000 rows, retrieve the Hash Code | 13th Gen Intel(R) Core(TM) i7-13800H | 9 ± 7 |
| Table_AddRows | Add 10000 rows to a table with 4 columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 20 ± 8 |
| Table_fillMissingCells | For a table 6 columns and 20000 rows, where each row has one missing value, fill those values with default values. | 13th Gen Intel(R) Core(TM) i7-13800H | 101 ± 11 |
| Table_ToJson | Serialize a table with 5 columns and 10000 rows to json. | 13th Gen Intel(R) Core(TM) i7-13800H | 293 ± 56 |
| Table_ToCompressedJson | Serialize a table with 5 columns and 10000 rows to compressed json. | 13th Gen Intel(R) Core(TM) i7-13800H | 4552 ± 326 |
| Assay_toJson | Parse an assay with one table with 10000 rows and 6 columns to json | 13th Gen Intel(R) Core(TM) i7-13800H | 271 ± 55 |
| Study_FromWorkbook | Parse a workbook with one study with 10000 rows and 6 columns to an ArcStudy | 13th Gen Intel(R) Core(TM) i7-13800H | 96 ± 12 |
| Investigation_ToWorkbook_ManyStudies | Parse an investigation with 1500 studies to a workbook | 13th Gen Intel(R) Core(TM) i7-13800H | 442 ± 61 |