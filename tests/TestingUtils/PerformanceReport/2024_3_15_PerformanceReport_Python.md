| Name | Description | CPU | Python Time (ms) |
| --- | --- | --- | --- |
| Table_GetHashCode | From a table with 1 column and 10000 rows, retrieve the Hash Code | 13th Gen Intel(R) Core(TM) i7-13800H | 226 |
| Table_AddRows | Add 10000 rows to a table with 4 columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 289 |
| Table_fillMissingCells | For a table 6 columns and 20000 rows, where each row has one missing value, fill those values with default values. | 13th Gen Intel(R) Core(TM) i7-13800H | 4813 |
| Table_ToJson | Serialize a table with 5 columns and 10000 rows to json. | 13th Gen Intel(R) Core(TM) i7-13800H | 6833 |
| Table_ToCompressedJson | Serialize a table with 5 columns and 10000 rows to compressed json. | 13th Gen Intel(R) Core(TM) i7-13800H | 717334 |
| Assay_toJson | Parse an assay with one table with 10000 rows and 6 columns to json | 13th Gen Intel(R) Core(TM) i7-13800H | 28799 |
| Study_FromWorkbook | Parse a workbook with one study with 10000 rows and 6 columns to an ArcStudy | 13th Gen Intel(R) Core(TM) i7-13800H | 1249 |
| Investigation_ToWorkbook_ManyStudies | Parse an investigation with 1500 studies to a workbook | 13th Gen Intel(R) Core(TM) i7-13800H | 9974 |