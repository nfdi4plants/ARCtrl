# Contracts - from a code perspective

**Table of contents**
- [Fields](#fields)
- [Comments](#comments)
- [IO](#io)
  - [Write](#write)

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
#r "nuget: FsSpreadsheet.ExcelIO, 5.0.2"
#r "nuget: ARCtrl, 1.0.0-beta.8"

open ARCtrl.ISA

// Comments
let investigation_comments = ArcInvestigation.init("My Investigation")
let newComment = Comment.create("The Id", "The Name", "The Value")
let newComment2 = Comment.create("My other ID", "My other Name", "My other Value")

// This might be changed to a ResizeArray in the future
investigation_comments.Comments <- Array.append investigation_comments.Comments [|newComment; newComment2|]
```

```js
import { ArcInvestigation, Comment$ as Comment} from "@nfdi4plants/arctrl"

const investigation_comments = ArcInvestigation.init("My Investigation")

const newComment = Comment.create("The Id", "The Name", "The Value")
const newComment2 = Comment.create("My other ID", "My other Name", "My other Value")

investigation_comments.Comments.push(newComment)
investigation_comments.Comments.push(newComment2)

console.log(investigation_comments)
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

## Write

```fsharp
open ARCtrl.ISA.Spreadsheet
open FsSpreadsheet.ExcelIO

let fswb = ArcInvestigation.toFsWorkbook investigation_comments

fswb.ToFile("test2.isa.investigation.xlsx")
```

```js
// Import Spreadsheet to XLSX reader/writer
import {Xlsx} from "@fslab/fsspreadsheet";
// Import ARCtrl Investigation to Spreadsheet transformation
import {toFsWorkbook, fromFsWorkbook} from "@nfdi4plants/arctrl/ISA/ISA.Spreadsheet/ArcInvestigation.js"

let fswb = toFsWorkbook(investigation_comments)

Xlsx.toFile("test.isa.investigation.xlsx", fswb)
```