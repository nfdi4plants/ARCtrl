# Contracts - from a code perspective

**Table of contents**
- [WRITE](#write-contracts)
- [READ](#read-contracts)

**Code can be found here**
- [F#](./scripts_fsharp/Contracts.fsx)
- [JavaScript](./scripts_js/Contracts.js)
- [Python](./scripts_python/Contracts.js)


Contracts in ARCtrl are used to delegate *IO operations* to the api consumer to allow interoperability with any environment (.NET, Node, Browser..). Contracts use *Data Transfer Object"s* (DTOs) instead of the ARCtrl types to provide a higher interoperability.

A Contract object consists of the following fields:

```fsharp
// F#
// Using F# discriminate union types it is very easy to match for differen cases.
type Contract = 
    {
        /// Determines what io operation should be done: CREATE, READ, DELETE, ...
        Operation : Operation
        /// The path where the io operation should be executed. The path is relative to ARC root.
        Path: string
        /// The type of DTO that is expected: JSON, PlainText, ISA_Assay, ISA_Study, ISA_Investigation,..
        DTOType : DTOType option
        /// The actual DTO, as discriminate union.
        DTO: DTO option
    }
```

```js
// JavaScript
export class Contract extends Record {
    constructor(Operation, Path, DTOType, DTO) {
        super();
        // Can be any of: "CREATE", "UPDATE", "DELETE", "READ", "EXECUTE"
        this.Operation = Operation;
        // string, the path where the io operation should be executed. The path is relative to ARC root
        this.Path = Path;
        // Can be undefined or any of: "ISA_Assay", "ISA_Study", "ISA_Investigation", "JSON", "Markdown", "CWL", "PlainText", "Cli".
        this.DTOType = DTOType;
        // Can be undefined or any of: `FsWorkbook` (from fsspreadsheet), string (e.g. json,..), or a `CLITool`.
        this.DTO = DTO;
    }
```
```python
# Python
class Contract(Record):
    # Can be any of: "CREATE", "UPDATE", "DELETE", "READ", "EXECUTE"
    Operation: str
    # string, the path where the io operation should be executed. The path is relative to ARC root
    Path: str
    # Can be undefined or any of: "ISA_Assay", "ISA_Study", "ISA_Investigation", "JSON", "Markdown", "CWL", "PlainText", "Cli".
    DTOType: DTOType | None
    # Can be undefined or any of: `FsWorkbook` (from fsspreadsheet), string (e.g. json,..), or a `CLITool`.
    DTO: DTO | None
```

Handling contracts can be generalized in a few functions.

## WRITE contracts

Here we will showcase minimal contract handling functions. These will not handle all edge cases but are sufficient to get started with!

Beginning with the WRITE contracts we may check the contract objects for the `Operation` field (This step is omitted in the .NET implementation). 
Next we need to know how to handle our DTO. In .NET this is implemented as Discriminate Union type, on which we can match. This is not possible in JS. Instead we just have any of the allowed DTO objects as value for the DTO field. Therefore, we must match on `DTOType` in js. 

In both languages we must specify how spreadsheet objects and plain text objects are correctly handled.

## READ contracts

READ contracts follow the same logic, with one difference. ARCtrl will give you READ contracts with None/Null values for the `DTO` field. But with the given `Path` and `DTOType` we can correctly read in the required DTO and set it on the contract. Afterwards, we return it to the correct follow-up api call in ARCtrl (this step is shown in [ARC docs](./ARC.md)).
