# ARC

🔗 The script files for this documentation can be found here:
- [JavaScript](/scripts_js/ARC.js)
- [F#](/scripts_fsharp/ARC.fsx)

Table of Content
- [Create ARC](#create)
- [Write ARC](#write)
- [Read ARC](#read)

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

async function write(arcPath, arc)  {
    let contracts = arc.GetWriteContracts()
    contracts.forEach(async contract => {
        await fulfillWriteContract(arcPath,contract)
    });
}

await write(arcRootPath,arc)
```

## Read

Read may look intimidating at first, until you notice that most of this is just setup which can be reused for any read you do. 

Setup will be placed on top, with the actual read below.

```fsharp
// F#
open System.IO

// Setup

let normalizePathSeparators (str:string) = str.Replace("\\","/")

let getAllFilePaths (basePath: string) =
    let options = EnumerationOptions()
    options.RecurseSubdirectories <- true
    Directory.EnumerateFiles(basePath, "*", options)
    |> Seq.map (fun fp ->
        Path.GetRelativePath(basePath, fp)
        |> normalizePathSeparators
    )
    |> Array.ofSeq

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
        let path = System.IO.Path.Combine(basePath, c.Path)
        let text = System.IO.File.ReadAllText(path) |> DTO.Text
        {c with DTO = Some text}
    | _ -> 
        printfn "Contract %s is not an ISA contract" c.Path 
        c

// put it all together
let readARC(basePath: string) =
    let allFilePaths = getAllFilePaths basePath
    // Initiates an ARC from FileSystem but no ISA info.
    let arcRead = ARC.fromFilePaths allFilePaths
    // Read contracts will tell us what we need to read from disc.
    let readContracts = arcRead.GetReadContracts()
    let fulfilledContracts = 
        readContracts 
        |> Array.map (fulfillReadContract basePath)
    arcRead.SetISAFromContracts(fulfilledContracts)
    arcRead 

// execution

readARC arcRootPath
```

```js

```

[1]: <https://www.nuget.org/packages/ARCtrl.NET> "ARCtrl.NET Nuget"