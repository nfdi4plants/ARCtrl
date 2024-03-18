# ArcInvestigation

**Table of contents**
- [Fields](#fields)
- [Comments](#comments)
- [IO](#io)
  - [Xlsx](#xlsx)
    - [Write](#write-xlsx)
  - [Json](#json)
    - [Write](#write-json)
    - [Read](#read-json)

**Code can be found here**
- [F#](/docs/scripts_fsharp/ArcInvestigation.fsx)
- [JavaScript](/docs/scripts_js/ArcInvestigation.js)

The ArcInvestigation is the container object, which contains all ISA related information inside of ARCtrl.

# Fields

The following shows a simple representation of the metadatainformation on ArcInvestigation, using a json format to get at least some color differences in markdown.

Here `option` means the value is nullable.

```json
{
  "ArcInvestigation": {
    "Identifier": "string",
    "Title" : "string option",
    "Description" : "string option",
    "SubmissionDate" : "string option",
    "PublicReleaseDate" : "string option",
    "OntologySourceReferences" : "OntologySourceReference []",
    "Publications" : "Publication []",
    "Contacts" : "Person []",
    "Assays" : "ArcAssay []",
    "Studies" : "ArcStudy []",
    "RegisteredStudyIdentifiers" : "string []",
    "Comments" : "Comment []",
    "Remarks" : "Remark []",
  }  
}
```

# Comments

Comments can be used to add freetext information to the Investigation metadata sheet. 

```fsharp
// F#
#r "nuget: FsSpreadsheet.Net"
#r "nuget: ARCtrl"

open ARCtrl.ISA

// Comments
let investigation_comments = ArcInvestigation.init("My Investigation")
let newComment = Comment.create("The Id", "The Name", "The Value")
let newComment2 = Comment.create("My other ID", "My other Name", "My other Value")

// This might be changed to a ResizeArray in the future
investigation_comments.Comments <- Array.append investigation_comments.Comments [|newComment; newComment2|]
```

```js
// JavaScript
import { ArcInvestigation, Comment$ as Comment} from "@nfdi4plants/arctrl"

const investigation_comments = ArcInvestigation.init("My Investigation")

const newComment = Comment.create("The Id", "The Name", "The Value")
const newComment2 = Comment.create("My other ID", "My other Name", "My other Value")

investigation_comments.Comments.push(newComment)
investigation_comments.Comments.push(newComment2)

console.log(investigation_comments)
```
```python
#Python
from arctrl.arctrl import ArcInvestigation, Comment

# Comments
investigation_comments = ArcInvestigation.init("My Investigation")

new_comment = Comment.create("The Id", "The Name", "The Value")
new_comment2 = Comment.create("My other ID", "My other Name", "My other Value")

investigation_comments.Comments.append(new_comment)
investigation_comments.Comments.append(new_comment2)

print(investigation_comments)
```
This code example will produce the following output after writing to `.xlsx`.

| INVESTIGATION                     |                  |
|-----------------------------------|------------------|
| ...                               | ...              |
| Investigation Identifier          | My Investigation |
| Investigation Title               |                  |
| Investigation Description         |                  |
| Investigation Submission Date     |                  |
| Investigation Public Release Date |                  |
| Comment[The Name]                 | The Value        |
| Comment[My other Name]            | My other Value   |
| INVESTIGATION PUBLICATIONS        |                  |
| ...                               | ...              |

# IO

## Xlsx

### Write Xlsx

```fsharp
// F#
open ARCtrl.ISA.Spreadsheet
open FsSpreadsheet.Net

let fswb = ArcInvestigation.toFsWorkbook investigation_comments

fswb.ToFile("test2.isa.investigation.xlsx")
```

```js
// JavaScript
import {Xlsx} from "@fslab/fsspreadsheet";
import {toFsWorkbook, fromFsWorkbook} from "@nfdi4plants/arctrl/ISA/ISA.Spreadsheet/ArcInvestigation.js"

let fswb = toFsWorkbook(investigation_comments)

Xlsx.toFile("test.isa.investigation.xlsx", fswb)
```
```python
# Python
from fsspreadsheet.xlsx import Xlsx
from arctrl.ISA.ISA_Spreadsheet.arc_investigation import to_fs_workbook, from_fs_workbook

fswb = to_fs_workbook(investigation_comments)

Xlsx.to_file("test.isa.investigation.xlsx", fswb)
```


## Json

ARCtrl ISA fully supports the [ISA-JSON](https://isa-specs.readthedocs.io/en/latest/isajson.html) schema! This means our ARCtrl.ISA model can be read from ISA-JSON as well as write to it.

### Write Json

```fsharp
// F#
#r "nuget: ARCtrl, 1.0.0-beta.9"

open ARCtrl.ISA
open ARCtrl.ISA.Json

let investigation = ArcInvestigation.init("My Investigation")

let json = ArcInvestigation.toJsonString investigation
```

```js
// JavaScript
import {ArcInvestigation} from "@nfdi4plants/arctrl"
import {ArcInvestigation_toJsonString, ArcInvestigation_fromJsonString} from "@nfdi4plants/arctrl/ISA/ISA.Json/Investigation.js"

const investigation = ArcInvestigation.init("My Investigation")

const json = ArcInvestigation_toJsonString(investigation)

console.log(json)
```
```python
# Python
from arctrl.ISA.ISA_Json.ArcTypes.arc_investigation import ArcInvestigation_toJsonString, ArcInvestigation_fromJsonString

investigation = ArcInvestigation.init("My Investigation")

json_str = ArcInvestigation_toJsonString(investigation)

print(json_str)
```

### Read Json

```fsharp
// F#
#r "nuget: ARCtrl, 1.0.0-beta.9"

open ARCtrl.ISA
open ARCtrl.ISA.Json

let jsonString = json

let investigation' = ArcInvestigation.fromJsonString jsonString

investigation = investigation' //true
```

```js
// JavaScript
import {ArcInvestigation} from "@nfdi4plants/arctrl"
import {ArcInvestigation_toJsonString, ArcInvestigation_fromJsonString} from "@nfdi4plants/arctrl/ISA/ISA.Json/Investigation.js"

const jsonString = json

const investigation_2 = ArcInvestigation_fromJsonString(jsonString)

console.log(investigation_2)
```
```python
# Python
from arctrl.ISA.ISA_Json.ArcTypes.arc_investigation import ArcInvestigation_toJsonString, ArcInvestigation_fromJsonString

json_string = json_str

investigation_2 = ArcInvestigation_fromJsonString(json_string)

print(investigation_2.Equals(investigation))
```