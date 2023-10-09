# ARC

🔗 The script files for this documentation can be found here:
- [JavaScript](/scripts_js/ARC.js)
- [F#](/scripts_fsharp/ARC.fsx)

Table of Content
- [Create ARC](#create)
- [Write ARC](#write)

## Create

ARCtrl aims to provide an easy solution to create and manipulate ARCs in memory. 

```fsharp
// F#
#r "nuget: FsSpreadsheet.ExcelIO, 4.1.0"
#r "nuget: ARCtrl, 1.0.0-alpha9"

open ARCtrl

/// Init a new empty ARC
let arc = ARC()
```

```js
// JavaScript
import {ARC} from "@nfdi4plants/arctrl";

let arc = new ARC()
```

This will initialize an ARC without metadata but with the basic ARC folder structure in `arc.FileSystem`

- 📁 ARC root
  - 📄 isa.investigation.xlsx
  - 📁 workflows
  - 📁 runs
  - 📁 assays
  - 📁 studies

## Write

In .NET you can use [ARCtrl.NET][1] to handle any contract based read/write operations. For this documentation we will extract the relevant ARCtrl.NET functions.

```fsharp
// F#
open ARCtrl.Contract
open FsSpreadsheet
open FsSpreadsheet.ExcelIO

let arcRootPath = @"path/where/you/want/the/NewTestARC"

// From ARCtrl.NET
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

// From ARCtrl.NET
let write (arcPath: string) (arc:ARC) =
    arc.GetWriteContracts()
    |> Array.iter (fulfillWriteContract arcPath)

write arcRootPath arc
```

```js
// JavaScript
import {ARC} from "@nfdi4plants/arctrl";
import {Xlsx} from "fsspreadsheet";
import fs from "fs";
import path from "path";

// Write
const arcRootPath = "C:/Users/Kevin/Desktop/NewTestARCJS"

async function fulfillWriteContract (basePath, contract) {
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
        } else if (contract.DTOType == "ISA_Assay" || contract.DTOType == "ISA_Assay" || "ISA_Investigation") {
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

async function write(arcPath, arc)  {
    let contracts = arc.GetWriteContracts()
    contracts.forEach(async contract => {
        await fulfillWriteContract(arcPath,contract)
    });
}

await write(arcRootPath,arc)
```

[1]: <https://www.nuget.org/packages/ARCtrl.NET> "ARCtrl.NET Nuget"