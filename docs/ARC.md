# ARC

🔗 The script files for this documentation can be found here:
- [JavaScript](./scripts_js/ARC.js)
- [F#](./scripts_fsharp/ARC.fsx)

Table of Content
- [Create](#create)
- [Write](#write)
- [Read](#read)

## Create

ARCtrl aims to provide an easy solution to create and manipulate ARCs in memory. 

```fsharp
// F#
#r "nuget: FsSpreadsheet.ExcelIO, 5.0.2"
#r "nuget: ARCtrl, 1.0.0-beta.8"

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
let write (arcPath: string) (arc:ARC) =
    arc.GetWriteContracts()
    // `Contracts.fulfillWriteContract` from Contracts.fsx docs
    |> Array.iter (Contracts.fulfillWriteContract arcPath)

write arcRootPath arc
```

```js
// JavaScript
import {fulfillWriteContract} from "./Contracts.js";

async function write(arcPath, arc)  {
    let contracts = arc.GetWriteContracts()
    for (const contract of contracts) {
        // from Contracts.js docs
        await fulfillWriteContract(arcPath,contract)
    };
}

await write(arcRootPath, arc)
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

// put it all together
let readARC(basePath: string) =
    // Get all file paths for a given ARC folder
    let allFilePaths = getAllFilePaths basePath
    // Initiates an ARC from FileSystem but no ISA info.
    let arcRead = ARC.fromFilePaths allFilePaths
    // Read contracts will tell us what we need to read from disc.
    let readContracts = arcRead.GetReadContracts()
    let fulfilledContracts = 
        readContracts 
        // `Contracts.fulfillReadContract` from Contracts.fsx docs
        |> Array.map (Contracts.fulfillReadContract basePath) 
    arcRead.SetISAFromContracts(fulfilledContracts)
    arcRead 

// execution
readARC arcRootPath
```

```js
// JavaScript
import {ARC} from "@nfdi4plants/arctrl";
import {fulfillWriteContract, fulfillReadContract} from "./Contracts.js";

// Setup
function normalizePathSeparators (str) {
  const normalizedPath = path.normalize(str)
  return normalizedPath.replace(/\\/g, '/');
}

export function getAllFilePaths(basePath) {
  const filesList = []
  function loop (dir) {
      const files = fs.readdirSync(dir);
      for (const file of files) {
          const filePath = path.join(dir, file);
  
          if (fs.statSync(filePath).isDirectory()) {
              // If it's a directory, recursively call the function on that directory
              loop(filePath);
          } else {
              // If it's a file, calculate the relative path and add it to the list
              const relativePath = path.relative(basePath, filePath);
              const normalizePath = normalizePathSeparators(relativePath)
              filesList.push(normalizePath);
          }
      }
  }
  loop(basePath)
  return filesList;
}

// put it all together
async function read(basePath) {
    let allFilePaths = getAllFilePaths(basePath)
    // Initiates an ARC from FileSystem but no ISA info.
    let arc = ARC.fromFilePaths(allFilePaths)
    // Read contracts will tell us what we need to read from disc.
    let readContracts = arc.GetReadContracts()
    console.log(readContracts)
    let fcontracts = await Promise.all(
        readContracts.map(async (contract) => {
            let content = await fulfillReadContract(basePath, contract)
            contract.DTO = content
            return (contract) 
        })
    )
    arc.SetISAFromContracts(fcontracts)
    return arc
}

// execution

await read(arcRootPath).then(arc => console.log(arc))
```

[1]: <https://www.nuget.org/packages/ARCtrl.NET> "ARCtrl.NET Nuget"