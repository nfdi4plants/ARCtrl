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
#r "nuget: FsSpreadsheet.Net"
#r "nuget: ARCtrl"

open ARCtrl

/// Init a new empty ARC
let arc = ARC()
```

```js
// JavaScript
import {ARC} from "@nfdi4plants/arctrl";

let arc = new ARC()
```

```python
# Python
from arctrl.arc import ARC

myArc = ARC()
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
open FsSpreadsheet.Net

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

```python
# Python
from arctrl.arc import ARC
from Contract import fulfill_write_contract, fulfill_read_contract

async def write(arc_path, arc):
    contracts = arc.GetWriteContracts()
    for contract in contracts:
        # from Contracts.js docs
        await fulfill_write_contract(arc_path, contract)
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
```python
import os
from arctrl.arc import ARC
from Contract import fulfill_write_contract, fulfill_read_contract

#Python
def normalize_path_separators(path_str):
    normalized_path = os.path.normpath(path_str)
    return normalized_path.replace('\\', '/')

def get_all_file_paths(base_path):
    files_list = []
    def loop(dir_path):
        files = os.listdir(dir_path)
        for file_name in files:
            file_path = os.path.join(dir_path, file_name)
            if os.path.isdir(file_path):
                loop(file_path)
            else:
                relative_path = os.path.relpath(file_path, base_path)
                normalize_path = normalize_path_separators(relative_path)
                files_list.append(normalize_path)
    loop(base_path)
    return files_list

# put it all together
def read(base_path):
    all_file_paths = get_all_file_paths(base_path)
    arc = ARC.from_file_paths(all_file_paths)
    read_contracts = arc.GetReadContracts()
    print(read_contracts)
    fcontracts = (fulfill_read_contract(base_path, contract) for contract in read_contracts)
    for contract, content in zip(read_contracts, fcontracts):
        contract.DTO = content
    arc.SetISAFromContracts(fcontracts)
    return arc

```
[1]: <https://www.nuget.org/packages/ARCtrl.NET> "ARCtrl.NET Nuget"