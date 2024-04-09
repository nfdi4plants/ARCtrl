# ARC

🔗 The script files for this documentation can be found here:
- [JavaScript](./scripts_js/ARC.js)
- [F#](./scripts_fsharp/ARC.fsx)
- [Python](./scripts_python/ARC.py)

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

## Read

Read may look intimidating at first, until you notice that most of this is just setup which can be reused for any read you do. 

Setup will be placed on top, with the actual read below.

[1]: <https://www.nuget.org/packages/ARCtrl.NET> "ARCtrl.NET Nuget"