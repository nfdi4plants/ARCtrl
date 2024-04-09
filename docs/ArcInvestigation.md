# ArcInvestigation

**Table of contents**
- [ArcInvestigation](#arcinvestigation)
- [Fields](#fields)
- [Comments](#comments)

**Code can be found here**
- [F#](./scripts_fsharp/ArcInvestigation.fsx)
- [JavaScript](./scripts_js/ArcInvestigation.js)
- [Python](./scripts_python/ArcInvestigation.py)

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

The example code example will produce the following output after writing to `.xlsx`.

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
