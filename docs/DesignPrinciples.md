# Design Principles

- [Jargon/Nomenclature](#jargonnomenclature)
- [API Design](#api-design)
- [Top level overview](#top-level-overview)
- [Stack](#stack)
- [Libraries](#libraries)
- [Design choices](#design-choices)
  - [Parsing priorities in unitized cvParam columns from table files](#parsing-priorities-in-unitized-cvparam-columns-from-table-files)
  - [Fable compatibility as top priority](#fable-compatibility-as-top-priority)

# Sub-Libraries

## ISA

### ISA datamodel

#### Requirements
1. MUST be parseable to a full representation of valid ISA-json
2. MUST contain structural information of isa tables (ISA-Tab and ISA-XLSX)
3. MUST allow for low level api calls (e.g. addParameterValue should be intuitive)
4. SHOULD be performant enough for use in GUI applications


#### Mögliche Lösungen:**

1. **Bestehendes ISA Schema nutzen**
    - Komplette strukturinformation via ISA comments (ISA assay spreadsheet json als comment auf Assay typ) 
    - Potentielle erweiterung für ISA schema -> ISA 2.0, schon direkt mit 1.0 compatibility lösung über comments
    - aus ISA json vs. XLSX wird ein uniformes format
    - Machen wir eh schon so -> kein extra arbeitsaufwand
    - import/export von standard ISA Json möglich
    
2. 'Structural' Metadata field on `ArcDataModel`
    - superset von ISA wird verhindert, wir können das einfach in unserem schema machen
    - problem: ISA json bleibt fallback für kommunikation mit externen tools
   


```mermaid
flowchart TD


subgraph ISAModel

    person[Person]
    publication[Publication]

    ch[CompositeHeader]
    cc[CompositeCells]
    oa[OntologyAnnotation]

    table[ArcTable<br><i>= Process + Protocol</i>]

    assay[ArcAssay]
    study[ArcStudy]
    investigation[ArcInvestigation]

end 

table --> ch
table --> cc
ch --> oa
cc --> oa

assay --> table
study --> table

study --> assay
investigation --> study

study --> person
investigation --> person
study --> publication
investigation --> publication

subgraph Legend
    json
    tab
end

%% Colorscheme
%% https://paletton.com/#uid=33m0E0kqOtKgGDvlJvDu0oyxCjs

style json fill:#5AA8B7,stroke:#0A6778,stroke-width:2px,color: black
style tab fill:#F04D63,stroke:#BA0C24,stroke-width:2px,color: black

style ch fill:#F04D63,stroke:#BA0C24,stroke-width:2px,color: black
style cc fill:#F04D63,stroke:#BA0C24,stroke-width:2px,color: black
style oa fill:#5AA8B7,stroke:#0A6778,stroke-width:2px,color: black
style person fill:#5AA8B7,stroke:#0A6778,stroke-width:2px,color: black
style publication fill:#5AA8B7,stroke:#0A6778,stroke-width:2px,color: black

style table fill:#F04D63,stroke:#BA0C24,stroke-width:4px,color: black
style assay stroke:#BA0C24,stroke-width:2px
style study stroke:#BA0C24,stroke-width:2px
style investigation stroke:#BA0C24,stroke-width:2px

```

### Annotation Term Handling

When creating an `Ontology Annotation` object, we should save the input string of the `Term Accession Number`. Like this, we can still get Short Accession and PURL when possible, but keep the possibility of returning the original input.

```mermaid
flowchart LR
obo[http://obo.obo/obo/EFO_0000721]
short[EFO:0000721]
other[http://www.ebi.ac.uk/efo/EFO_0000721]

subgraph OntologyAnnotation
    oboO[http://obo.obo/obo/EFO_0000721]
    shortO[EFO:0000721]
    otherO[http://www.ebi.ac.uk/efo/EFO_0000721]       
end

shortOut[EFO:0000721]
oboOut[http://obo.obo/obo/EFO_0000721]

otherAny[http://www.ebi.ac.uk/efo/EFO_0000721]

obo --> oboO
short --> shortO
other --> otherO

oboO --GetShort--> shortOut
oboO --GetPURL--> oboOut
oboO --GetOriginal--> oboOut

otherO --GetShort--> shortOut
otherO --GetPURL--> oboOut
otherO --GetOriginal--> otherAny

shortO --GetShort--> shortOut
shortO --GetPURL--> oboOut
shortO --GetOriginal--> shortOut
```


## Contracts

The ARC stack exclusively works with the in-memory representations of the ARC. 
In order to keep the in-memory ARC datamodel and the filesystem synchronized, the ARC.Core stack uses the concept of `Contracts`.

Each contract is a single IO operation. The operation contained in a contract does not use the datamodel objects, but rather uses `data transfer objects`,  which include for example the `json` representation as string and the `FsSpreadsheet` representation of the ISA.xlsx files. 

This intermediate conversion from the datamodel object to the data transfer object is important, as this step is the most complex and variable.

```mermaid
flowchart TD

arc[ARC Datamodel]

subgraph Contract
    obj[Data Transfer Object]
    type[DTO Type]
    path[Path]
    crud[CRUDE]
end

js[/Javascript IO\]
dotnet[/Dotnet IO\]
any[/Any IO\]

arc  <--> Contract
Contract <-- IO --> any
Contract <-- IO --> dotnet
Contract <-- IO --> js


style dotnet fill:#8C71E3,stroke:#3A0EC6,stroke-width:2px,color: black
style Contract stroke:#007B00,stroke-width:2px

style js fill:#C1AD09,stroke:#EFD81D,stroke-width:2px,color: black

style arc fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style any fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style obj fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style type fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style crud fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style path fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
```

Besides the data transfer object, the contract also contains the path where the file system manipulation should be executed and the kind of the operation (`C`reate, `R`ead, `U`pdate, `D`elete, `E`xecute).

- **CREATE**: represents file creation on a certain location relative to the ARC root. If the file exists, it is overwritten.
   **Examples:** 
    - Create `README.md` with default content
    - Create new assay file (with associated content via `DataTransferObject.FsSpreadsheet`)
    - Create empty `.gitkeep` file
  
- **READ**: represents instructions are used to transfer information from the filesystem into the ARC datamodel. For this, the ARC API returns a `READ` contract to the user or tool, which then fulfills the contract by reading the file as a DTO from the specified path and returning the filled out contract to the API. The API then handles the inclusion of the information into the datamodel. 


  Example: initate ARC Datamodel from filesystem:
  ```mermaid
  sequenceDiagram

  participant Filesystem
  participant Client
  participant ARCAPI
  
  Note over Client,ARCAPI: API.getReadContracts
  Client->>ARCAPI: filepaths
  ARCAPI->>Client: contracts
  
  Note over Client,Filesystem: File.read
  Client->>Filesystem: filepath
  Filesystem->>Client: DTO
  
  Note over Client,ARCAPI: API.createARCfromContracts
  Client->>ARCAPI: fullfilled Contracts
  ARCAPI->>Client: ARC datamodel
  ```
  
  Fullfilling a READ contract means to read the file in the specified path   and filling the contract with the resulting DTO.
  
  ```mermaid
  flowchart TD
  
  obj[Data Transfer Object]
  
  subgraph Contract
      type[DTO Type]
      path[Path]
      crud[READ]
  end
  
  subgraph A[Fullfilled Contract]
      fobj[Data Transfer Object]
      ftype[DTO Type]
      fpath[Path]
      fcrud[READ]
  end
  
  obj --> A
  Contract --> A
  ```

- **UPDATE**: represents manipulation of **existing** files in the ARC. It is important to distinct this operation from **CREATE**, as the DataTransferObjects can not handle all possible file content. 
For example: styling, plots, etc. in spreadsheet files that are not modelled in our metadata models.
    **Examples:**
    - Add a new `Person` to an existing Study
    - Add a new row to the `AnnotationTable` in an existing assay file, while keeping user content such as plots intact.
    
- **DELETE** does exactly what you think it does.
- **EXECUTE**: represents instructions for running a cli-tool with a given name and arguments. As the ARC API does not have information about absolute paths, the tool name should be executable as is. E.g. in Windows by adding it to the PATH.


## Examples


### Git-pull

```fsharp
let gitPull = 
    Contract.create(
        op = EXECUTE,
        path = "", //relative to arc root
        dto = DTO.CLITool (
            CLITool.create(
                name = "git",
                arguments = [|"pull"|]
            )
        )
    )
```


### Update study person name

```fsharp
// update a study person name
let updateStudyContracts = [
    // UPDATE INVESTIGATION METADATA
    Contract.create(
        op = UPDATE,
        path = "path/to/investigation.xlsx",          
        dtoType = DTOType.Spreadsheet,
        dto = DTO.Spreadsheet (new FsWorkbook())
            
    ) 
    Contract.create(
        op = UPDATE,
        path = "path/to/study.xlsx",
        dtoType = DTOType.Spreadsheet,
        dto = DTO.Spreadsheet (new FsWorkbook())         
    ) 
]
```


### Delete a study

```fsharp
// delete a study
let deleteStudyContracts = [
    // UPDATE INVESTIGATION METADATA
    Contract.create(
        op = DELETE,
        path = "path/to/studyFOLDER(!)"
    ) 
    Contract.create(
        op = UPDATE,
        path = "path/to/investigation.xlsx",
            
        dtoType = DTOType.Spreadsheet,
        dto = DTO.Spreadsheet (new FsWorkbook())
            
    )
]
```

### Add assay when no study exists

```fsharp
// Assay add, when no study exists
let addAssayContracts = [
    // create spreadsheet assays/AssayName/isa.assay.xlsx  
    Contract.create(
        op = CREATE,
        path = "path/to/isa.assay.xlsx",
        dtoType = DTOType.Spreadsheet,
        dto = DTO.Spreadsheet (new FsWorkbook())
    ) 
    // create empty file assays/AssayName/dataset/.gitkeep 
    Contract.create(
        op = CREATE,
        path = "path/to/dataset/.gitkeep"
    )        
    // create text assays/AssayName/README.md
    Contract.create(
        op = CREATE,
        path = "path/to/README.md",
        dtoType = DTOType.Markdown,
        dto = DTO.Text "# Markdown"
            
    )
    // create empty file assays/AssayName/protocols/.gitkeep
    Contract.create(
        op = CREATE,
        path = "path/to/protocols/.gitkeep"
    )
    // create spreadsheet studies/StudyName/isa.study.xlsx  
    Contract.create(
        op = CREATE,
        path = "path/to/study.xlsx",
        dtoType = DTOType.Spreadsheet,
        dto = DTO.Spreadsheet (new FsWorkbook())
    )
    // create empty file studies/StudyName/resources/.gitkeep 
    Contract.create(
        op = CREATE,
        path = "path/to/resources/.gitkeep"
    )        
    // create text studies/StudyName/README.md
    Contract.create(
        op = CREATE,
        path = "path/to/README.md",
        dtoType = DTOType.Markdown,
        dto = DTO.Text "# Markdown"
    )
    // create empty file studies/StudyName/protocols/.gitkeep
    Contract.create(
        op = CREATE,
        path = "path/to/protocols/.gitkeep"
    )
    // update spreadsheet isa.investigation.xlsx
    Contract.create(
        op = UPDATE,
        path = "path/to/investigation.xlsx",
        dtoType = DTOType.Spreadsheet,
        dto = DTO.Spreadsheet (new FsWorkbook())
        ) 
]
```

# Jargon/Nomenclature

In general, a distinction is made between `DataModel`s,  `API`s, and `Tools`:
- `DataModel`s are the data structures which represent the ARC or it's respective parts in memory. They are serializable and can be used as data exchange format between tool implementations:
  - `FileSystem`: Represents the file system structure of an ARC. All files and their path relative to the arc root folder are contained here.
  - `ISA`: Represents the experimental metadata of the ARC that is stored in the ISA-XLSX format (investigation, studies and assays).
  - `CWL`: Represents the workflow definitions of the ARC in the CWL format.
- `API`s are static methods on the `DataModel` types that perfrom operations on the `DataModel`s. Often, these are CRUD-like operations, and are aimed to be be composable. 
  
  **Example**: A `ARC.addAssay` function has to do several things:
  - Add a new assay to the `ISA`
  - Add a new assay to the `FileSystem`
  it should therefore combine the respective functions of `ISA API` and `FileSystem API` to achieve this.

- `Tools` or `Clients` are user-facing software such as Swate, ARCitect, or the ARCCommander. They should ideally compose their functionality from the `API`s and work with an in-memory representation of the ARC via `DataModel`s. There are operations such as IO for reading/writing actual files to the file system, which are not part of the `API`s, but rather part of the `Tools`.

## API Design

Command syntax should be inspired by ArcCommander commands, as they are already well established and known to the power user base.
See syntax : https://nfdi4plants.github.io/arcCommander-docs/docs/01GeneralCLIStructure.html
See selection: https://nfdi4plants.github.io/arcCommander-docs/docs/02SubcommandVerbs.html


## Top level overview

In the following, the dependency graph of the proposed arcAPI toolstack can be seen:

```mermaid
flowchart TD

%% ----- Nodes ------


subgraph ARCtrl
    arc[ARC]
    
    subgraph ISA
        isaj[ISA.Json]
        isax[ISA.Spreadsheet]      
        isaa[ISA]
    end
    subgraph FileSystem
        fs[FileSystem]
    end
    subgraph CWL
        cwl[CWL]
    end
end

subgraph DataTransferObject
    fsspread[FsSpreadsheet]
    thoth["Thoth.Json (string)"]
end

subgraph IO
    node[node.fs]   
    fsspreadx[FsSpreadsheet.Net]   
    excelJS[exceljs]
    systemio[System.IO]
end


%% ----- Edges ------


%% Arc
arc --> ISA
arc --> FileSystem
arc --> CWL

%% ISA
isaj --> isaa
isax --> isaa
isaj --> thoth

isax --> fsspread

%% ------ Tools ---------

subgraph Tools
    arcdotnet[/arcIO.NET\]
    arcitect[[ARCitect]]
    arccom[[arcCommander]]
    swate[[Swate]]
end 

%% ----- Edges ------

Tools --> ARCtrl

arcdotnet --> fsspreadx
arcdotnet --> systemio
arcitect --> excelJS
arcitect --> node
arccom --> arcdotnet


fsspreadx -.-> fsspread
systemio -.-> thoth
excelJS -.-> fsspread
node -.-> thoth

%% ----- Styling ------

%% dotnet
style Tools stroke:#3A0EC6,stroke-width:2px
style fsspreadx fill:#8C71E3,stroke:#3A0EC6,stroke-width:2px,color: black
style arcdotnet fill:#8C71E3,stroke:#3A0EC6,stroke-width:2px,color: black
style arccom fill:#8C71E3,stroke:#3A0EC6,stroke-width:2px,color: black
style systemio fill:#8C71E3,stroke:#3A0EC6,stroke-width:2px,color: black

%% javascript
style excelJS fill:#C1AD09,stroke:#EFD81D,stroke-width:2px,color: black
style arcitect fill:#C1AD09,stroke:#EFD81D,stroke-width:2px,color: black
style swate fill:#C1AD09,stroke:#EFD81D,stroke-width:2px,color: black
style node fill:#C1AD09,stroke:#EFD81D,stroke-width:2px,color: black


%% fable
style ARCtrl stroke:#007B00,stroke-width:2px
style ISA stroke:#007B00,stroke-width:2px
style FileSystem stroke:#007B00,stroke-width:2px
style CWL stroke:#007B00,stroke-width:2px
style DataTransferObject stroke:#007B00,stroke-width:2px

style fsspread fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style isax fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style isaa fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style isaj fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style arc fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style fs fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style cwl fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
style thoth fill:#39B539,stroke:#007B00,stroke-width:2px,color: black
```


## Stack

```mermaid
classDiagram 

class arc["ARCtrl"] {
    ISA
    CWL
    FileSystem
}
class isa["ISA"] {
    Investigation
    Studies
    Assays
    ArcTable
    CompositeColumn
    CompositeHeader
    CompositeCell
}
class file ["FileSystem"] {
    - FileSystemTree
    - History
}
class cwl["CWL"] {
    CommandlineTool?
    Workflows
}
class io["IO"] {
    FsSpreadsheet.Net
    System.IO
    exceljs
    node fs
}
io <|--|> arc : Contracts
arc <|--|> isa
arc <|--|> cwl
arc <|--|> file

```

## Design choices

Also taken from [here](https://github.com/nfdi4plants/ARCtrl/issues/114).

### ISA/ARC Specification

- All non term columns MUST be unique. (Input, Output, ProtocolREF, ProtocolType) 
- All term columns MAY be used any number of times.
- All table names inside one assay must be unique.

### Function

- ArcTable functions MUST always result in a valid table!
- All table body fields MUST be added explicitly to `ArcTable.Values`.
  - Therefore, adding a column with 2 rows to a table of 5 rows MUST also fill the missing 3 rows with "empty cells".
  - Therefore, adding a column with 7 rows to a table of 5 rows MUST also fill the missing 2 rows for all other columns in the table with "empty cells".
- Any function getting a value from ArcTable (row, column, cell) outside of table range MUST raise an exception or return `None`.
- Any function adding a value to ArcTable MUST only allow indices in table range + 1 (which MUST result in add new and append logic).
- Any "SingleColumn", such as Input, ProtocolREF... MUST have all `CompositeCell.Freetext` cells.
- Any "TermColumn", such as Factor, Parameter, plus "FeaturedColumn"s (ProtocolType) MUST contain `CompositeCell.Term` or `CompositeCell.Unitized`. These columns MAY be uniform or mixed, but MUST NOT contain any `CompositeCell.Freetext`.
- Any member function starting with the set keyword (`SetCellAt` , `SetColumn`,..) MUST NOT add new elements but only replace existing.

### Syntax

- All member functions (table.AddColumn,..) MUST return unit and work on mutable elements.
- All member functions (table.AddColumn,..) MUST have a equivalent static member function of the same name with lower-case first letter (ArcTable.addColumn).
- All static members on ArcTable (ArcTable.addColumn) MUST implement its equivalent member function on a copy of the given table. Therefore, not changing the existing table, but returning a new one!
```fsharp
// example
static member addColumn (header: CompositeHeader,cells: CompositeCell [],?Index: int ,?ForceReplace : bool) : (ArcTable -> ArcTable) =
   fun (table: ArcTable) ->
        let newTable = table.Copy()
        newTable.AddColumn(header, cells, ?index = Index)
        newTable
```
- All CRUD functions between assay and study are implemented on Investigation level. Propagated functions on assay/study must use investigation functions to avoid code duplication.

### Parsing priorities in unitized cvParam columns from table files

The handling of unitized columns might be complex, as discussed in
https://github.com/nfdi4plants/user-stories/issues/9

Proposed parsing:

| Parameter [MyCategory] | Unit   | Term Source REF (ABC:00000) | Term Accession Number (ABC:00000) | > | Results In   |
|-----------------|--------|-----------------------------|-----------------------------------|----|--------------|
| 5               | MyUnit | DEF                         | DEF:12345                         | > | UnitizedCell |
| 5               |        | DEF                         | DEF:12345                         | > | UnitizedCell |
| 5               |        |                             |                                   | > | UnitizedCell |
|                 | MyUnit | DEF                         | DEF:12345                         | > | UnitizedCell |
|                 |        |                             |                                   | > | UnitizedCell |
| MyValue         | MyUnit | DEF                         | DEF:12345                         | > | UnitizedCell |
| MyValue         |        | DEF                         | DEF:12345                         | > | TermCell     |
| MyValue         |        |                             |                                   | > | TermCell     |

Additionally, to keep the `hasUnit` information of valueless columns. We might add the hasUnit to the Header.

### Fable compatibility as top priority

All libraries should be Fable compatible, and produce javascript/typescript code that is ergonomic to use in a js/ts environment, therefore:
- we use classes with static members over nested modules
- we use the `[<AttachMembers>]` fable attribute for each class
  - Using overloads with the `[<AttachMembers>]` attribute will make js functions shadow itself. Never use this!
- we use the `[<NamedParams(n)>]` fable attribute for all optional parameters in static methods that use tupled, named params.
- we use `Array<'T>` for all collections in F#, since they get transpiled to native js arrays.
- we use the `[<Erase>]` fable attribute for union cases that contain data 
    ```fsharp
     [<Erase>] type X = | Y of string | Z of int
    ```
- we use the `[<StringEnum>]` for unions that contain no data (e.g.)
    ```fsharp
     [<StringEnum>] type YesOrNo = | Yes | No
    ```
    
**Example:**

```fsharp
[<AttachMembers>]
type Study = 
    {
        Identifier : string option
        Assays : Assay array option
    }

    [<NamedParams>]
    static member create (?Identifier, ?Assays : Assay array) = 
        {
            Identifier = Identifier
            Assays = Assays
        }
```

will become the following javascript code:

```javascript
export class Study extends Record {
    constructor(Identifier, Assays) {
        super();
        this.Identifier = Identifier;
        this.Assays = Assays;
    }
    static create({ Identifier, Assays }) {
        return new Study(Identifier, Assays);
    }
```

and the following typescript code:

```typescript
export class Study extends Record implements IEquatable<Study>, IComparable<Study> {
    readonly Identifier: Option<string>;
    readonly Assays: Option<Assay[]>;
    constructor(Identifier: Option<string>, Assays: Option<Assay[]>) {
        super();
        this.Identifier = Identifier;
        this.Assays = Assays;
    }
    static create({ Identifier, Assays }: {Identifier?: string, Assays?: Assay[] }): Study {
        return new Study(Identifier, Assays);
    }
```