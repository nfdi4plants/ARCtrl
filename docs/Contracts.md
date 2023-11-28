# Contracts - from a code perspective

**Table of contents**
- [WRITE](#write-contracts)
- [READ](#read-contracts)

**Code can be found here**
- [F#](/docs/scripts_fsharp/Contracts.fsx)
- [JavaScript](/docs/scripts_js/Contracts.js)


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

Handling contracts can be generalized in a few functions.

## WRITE contracts

Here we will showcase minimal contract handling functions. These will not handle all edge cases but are sufficient to get started with!

Beginning with the WRITE contracts we may check the contract objects for the `Operation` field (This step is omitted in the .NET implementation). 
Next we need to know how to handle our DTO. In .NET this is implemented as Discriminate Union type, on which we can match. This is not possible in JS. Instead we just have any of the allowed DTO objects as value for the DTO field. Therefore, we must match on `DTOType` in js. 

In both languages we must specify how spreadsheet objects and plain text objects are correctly handled.

```fsharp
#r "nuget: FsSpreadsheet.ExcelIO, 5.0.2"
#r "nuget: ARCtrl, 1.0.0-beta.8"

open ARCtrl
open ARCtrl.Contract
open FsSpreadsheet
open FsSpreadsheet.ExcelIO

/// From ARCtrl.NET
let fulfillWriteContract basePath (c : Contract) =
    let ensureDirectory (filePath : string) =
        let file = new System.IO.FileInfo(filePath);
        file.Directory.Create()
    match c.DTO with
    | Some (DTO.Spreadsheet wb) ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        ensureDirectory path
        FsWorkbook.toFile path (wb :?> FsWorkbook)
    | Some (DTO.Text t) ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        ensureDirectory path
        System.IO.File.WriteAllText(path,t)
    | None ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        ensureDirectory path
        System.IO.File.Create(path).Close()
    | _ ->
        printfn "Contract %s is not an ISA contract" c.Path
```

```js
import {Xlsx} from "fsspreadsheet";
import fs from "fs";
import path from "path";

export async function fulfillWriteContract (basePath, contract) {
    function ensureDirectory (filePath) {
        let dirPath = path.dirname(filePath)
        if (!fs.existsSync(dirPath)){
            fs.mkdirSync(dirPath, { recursive: true });
        }
    }
    const p = path.join(basePath,contract.Path)
    if (contract.Operation = "CREATE") {
        if (contract.DTO == undefined) {
            ensureDirectory(p)
            fs.writeFileSync(p, "")
        } else if (contract.DTOType == "ISA_Assay" || contract.DTOType == "ISA_Assay" || contract.DTOType == "ISA_Investigation") {
            ensureDirectory(p)
            await Xlsx.toFile(p, contract.DTO)
            console.log("ISA", p)
        } else if (contract.DTOType == "PlainText") {
            ensureDirectory(p)
            fs.writeFileSync(p, contract.DTO)
        } else {
            console.log("Warning: The given contract is not a correct ARC write contract: ", contract)
        }
    }
}
```


## READ contracts

READ contracts follow the same logic, with one difference. ARCtrl will give you READ contracts with None/Null values for the `DTO` field. But with the given `Path` and `DTOType` we can correctly read in the required DTO and set it on the contract. Afterwards, we return it to the correct follow-up api call in ARCtrl (this step is shown in [ARC docs](./ARC.md)).

```fsharp
// from ARCtrl.NET
// https://github.com/nfdi4plants/ARCtrl.NET/blob/ba3d2fabe007d9ca2c8e07b62d02ddc5264306d0/src/ARCtrl.NET/Contract.fs#L7
let fulfillReadContract basePath (c : Contract) =
    match c.DTOType with
    | Some DTOType.ISA_Assay 
    | Some DTOType.ISA_Investigation 
    | Some DTOType.ISA_Study ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        let wb = FsWorkbook.fromXlsxFile path |> box |> DTO.Spreadsheet
        {c with DTO = Some wb}
    | Some DTOType.PlainText ->
        let path: string = System.IO.Path.Combine(basePath, c.Path)
        let text = System.IO.File.ReadAllText(path) |> DTO.Text
        {c with DTO = Some text}
    | _ -> 
        printfn "Contract %s is not an ISA contract" c.Path 
        c
```

```js
export async function fulfillReadContract (basePath, contract) {
  async function fulfill() {
      const normalizedPath = normalizePathSeparators(path.join(basePath, contract.Path))
      switch (contract.DTOType) {
          case "ISA_Assay":
          case "ISA_Study":
          case "ISA_Investigation":
              let fswb = await Xlsx.fromXlsxFile(normalizedPath)
              return fswb
              break;
          case "PlainText":
              let content = fs.readFile(normalizedPath)
              return content
              break;
          default:
              console.log(`Handling of ${contract.DTOType} in a READ contract is not yet implemented`)
      }
  }
  if (contract.Operation == "READ") {
      return await fulfill()
  } else {
      console.error(`Error (fulfillReadContract): "${contract}" is not a READ contract`)
  }
}
```