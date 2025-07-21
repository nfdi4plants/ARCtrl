| Name | Description | CPU | FSharp Time (ms) |
| --- | --- | --- | --- |
| Table_GetHashCode | From a table with 1 column and 10000 rows, retrieve the Hash Code | 13th Gen Intel(R) Core(TM) i7-13800H | 0 ± 0 |
| Table_AddDistinctRows | Add 10000 distinct rows to a table with 4 columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 13 ± 2 |
| Table_AddIdenticalRows | Add 10000 identical rows to a table with 4 columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 6 ± 2 |
| Table_AddColumnsWithDistinctValues | Add 4 columns with 10000 distinct values each. | 13th Gen Intel(R) Core(TM) i7-13800H | 8 ± 3 |
| Table_AddColumnsWithIdenticalValues | Add 4 columns with 10000 identical values each. | 13th Gen Intel(R) Core(TM) i7-13800H | 5 ± 1 |
| Table_fillMissingCells | For a table 6 columns and 20000 rows, where each row has one missing value, fill those values with default values. | 13th Gen Intel(R) Core(TM) i7-13800H | 0 ± 0 |
| Table_ToJson | Serialize a table with 5 columns and 10000 rows to json, with 3 fixed and 2 variable columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 227 ± 64 |
| Table_ToCompressedJson | Serialize a table with 5 columns and 10000 rows to compressed json, with 3 fixed and 2 variable columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 147 ± 15 |
| Assay_toJson | Parse an assay with one table with 10000 rows and 6 columns to json, with 3 fixed and 3 variable columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 330 ± 36 |
| Assay_fromJson | Parse an assay with one table with 10000 rows and 6 columns from json, with 3 fixed and 3 variable columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 355 ± 66 |
| Assay_toISAJson | Parse an assay with one table with 10000 rows and 6 columns to json, with 3 fixed and 3 variable columns | 13th Gen Intel(R) Core(TM) i7-13800H | 487 ± 36 |
| Assay_fromISAJson | Parse an assay with one table with 10000 rows and 6 columns from json, with 3 fixed and 3 variable columns | 13th Gen Intel(R) Core(TM) i7-13800H | 359 ± 29 |
| Study_FromWorkbook | Parse a workbook with one study with 10000 rows and 6 columns to an ArcStudy | 13th Gen Intel(R) Core(TM) i7-13800H | 29 ± 14 |
| Investigation_ToWorkbook_ManyStudies | Parse an investigation with 1500 studies to a workbook | 13th Gen Intel(R) Core(TM) i7-13800H | 240 ± 31 |
| Investigation_FromWorkbook_ManyStudies | Parse a workbook with 1500 studies to an ArcInvestigation | 13th Gen Intel(R) Core(TM) i7-13800H | 127 ± 20 |
| ARC_ToROCrate | Parse an ARC with one assay with 10000 rows and 6 columns to a RO-Crate metadata file. | 13th Gen Intel(R) Core(TM) i7-13800H | 1431 ± 99 |