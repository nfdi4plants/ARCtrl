| Name | Description | CPU | Python Time (ms) |
| --- | --- | --- | --- |
| Table_GetHashCode | From a table with 1 column and 10000 rows, retrieve the Hash Code | 13th Gen Intel(R) Core(TM) i7-13800H | 91 ± 12 |
| Table_AddDistinctRows | Add 10000 distinct rows to a table with 4 columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 119 ± 12 |
| Table_AddIdenticalRows | Add 10000 identical rows to a table with 4 columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 104 ± 6 |
| Table_AddColumnsWithDistinctValues | Add 4 columns with 10000 distinct values each. | 13th Gen Intel(R) Core(TM) i7-13800H | 53 ± 1 |
| Table_AddColumnsWithIdenticalValues | Add 4 columns with 10000 identical values each. | 13th Gen Intel(R) Core(TM) i7-13800H | 47 ± 1 |
| Table_fillMissingCells | For a table 6 columns and 20000 rows, where each row has one missing value, fill those values with default values. | 13th Gen Intel(R) Core(TM) i7-13800H | 6 ± 4 |
| Table_ToJson | Serialize a table with 5 columns and 10000 rows to json, with 3 fixed and 2 variable columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 7851 ± 1411 |
| Table_ToCompressedJson | Serialize a table with 5 columns and 10000 rows to compressed json, with 3 fixed and 2 variable columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 6303 ± 1798 |
| Assay_toJson | Parse an assay with one table with 10000 rows and 6 columns to json, with 3 fixed and 3 variable columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 12644 ± 550 |
| Assay_fromJson | Parse an assay with one table with 10000 rows and 6 columns from json, with 3 fixed and 3 variable columns. | 13th Gen Intel(R) Core(TM) i7-13800H | 6499 ± 1068 |
| Assay_toISAJson | Parse an assay with one table with 10000 rows and 6 columns to json, with 3 fixed and 3 variable columns | 13th Gen Intel(R) Core(TM) i7-13800H | 15618 ± 482 |
| Assay_fromISAJson | Parse an assay with one table with 10000 rows and 6 columns from json, with 3 fixed and 3 variable columns | 13th Gen Intel(R) Core(TM) i7-13800H | 9587 ± 621 |
| Study_FromWorkbook | Parse a workbook with one study with 10000 rows and 6 columns to an ArcStudy | 13th Gen Intel(R) Core(TM) i7-13800H | 818 ± 36 |
| Investigation_ToWorkbook_ManyStudies | Parse an investigation with 1500 studies to a workbook | 13th Gen Intel(R) Core(TM) i7-13800H | 3657 ± 199 |
| Investigation_FromWorkbook_ManyStudies | Parse a workbook with 1500 studies to an ArcInvestigation | 13th Gen Intel(R) Core(TM) i7-13800H | 9469 ± 412 |
| ARC_ToROCrate | Parse an ARC with one assay with 10000 rows and 6 columns to a RO-Crate metadata file. | 13th Gen Intel(R) Core(TM) i7-13800H | 61224 ± 728 |