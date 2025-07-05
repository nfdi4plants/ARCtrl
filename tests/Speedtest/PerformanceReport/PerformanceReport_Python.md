| Name | Description | CPU | Python Time (ms) |
| --- | --- | --- | --- |
| Table_GetHashCode | From a table with 1 column and 10000 rows, retrieve the Hash Code | mycpu | 97 ± 4 |
| Table_AddDistinctRows | Add 10000 distinct rows to a table with 4 columns. | mycpu | 158 ± 5 |
| Table_AddIdenticalRows | Add 10000 identical rows to a table with 4 columns. | mycpu | 148 ± 6 |
| Table_AddColumnsWithDistinctValues | Add 4 columns with 10000 distinct values each. | mycpu | 81 ± 5 |
| Table_AddColumnsWithIdenticalValues | Add 4 columns with 10000 identical values each. | mycpu | 73 ± 4 |
| Table_fillMissingCells | For a table 6 columns and 20000 rows, where each row has one missing value, fill those values with default values. | mycpu | 11 ± 1 |
| Table_ToJson | Serialize a table with 5 columns and 10000 rows to json. | mycpu | 4533 ± 605 |
| Table_ToCompressedJson | Serialize a table with 5 columns and 10000 rows to compressed json. | mycpu | 7323 ± 408 |
| Assay_toJson | Parse an assay with one table with 10000 rows and 6 columns to json | mycpu | 7070 ± 401 |
| Study_FromWorkbook | Parse a workbook with one study with 10000 rows and 6 columns to an ArcStudy | mycpu | 1640 ± 83 |
| Investigation_ToWorkbook_ManyStudies | Parse an investigation with 1500 studies to a workbook | mycpu | 9009 ± 297 |