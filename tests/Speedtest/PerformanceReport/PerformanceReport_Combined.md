## 2024_15_03

Measured on 13th Gen Intel(R) Core(TM) i7-13800H

| Name | Description | FSharp Time (ms) | JavaScript Time (ms) | Python Time (ms) |
| --- | --- | --- | --- | --- |
| Table_GetHashCode | From a table with 1 column and 10000 rows, retrieve the Hash Code |  5 | 21 | 226 |
| Table_AddRows | Add 10000 rows to a table with 4 columns. |  15 | 22 | 289 |
| Table_fillMissingCells | For a table 6 columns and 20000 rows, where each row has one missing value, fill those values with default values. | 49 | 108 | 4813 |
| Table_ToJson | Serialize a table with 5 columns and 10000 rows to json. |  1099 | 481 | 6833 |
| Table_ToCompressedJson | Serialize a table with 5 columns and 10000 rows to compressed json. |  261 |  2266 | 717334 |
| Assay_toJson | Parse an assay with one table with 10000 rows and 6 columns to json |  915 | 2459 | 28799 |
| Study_FromWorkbook | Parse a workbook with one study with 10000 rows and 6 columns to an ArcStudy |  97 | 87 | 1249 |
| Investigation_ToWorkbook_ManyStudies | Parse an investigation with 1500 studies to a workbook |  621 | 379 | 9974 |