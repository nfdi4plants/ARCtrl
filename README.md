# ARCtrl

> **ARCtrl** the easy way to read, manipulate and write ARCs in __.NET__, __JavaScript__ and __Python__! ❤️

| Version | Downloads |
| :--------|-----------:|
|<a href="https://www.nuget.org/packages/ARCtrl/"><img alt="Nuget" src="https://img.shields.io/nuget/vpre/ARCtrl?logo=nuget&color=%234fb3d9"></a>|<a href="https://www.nuget.org/packages/ARCtrl/"><img alt="Nuget" src="https://img.shields.io/nuget/dt/ARCtrl?color=%234FB3D9"></a>|
|<a href="https://www.npmjs.com/package/@nfdi4plants/arctrl"><img alt="NPM" src="https://img.shields.io/npm/v/%40nfdi4plants/arctrl?logo=npm&color=%234fb3d9"></a>|<a href="https://www.npmjs.com/package/@nfdi4plants/arctrl"><img alt="NPM" src="https://img.shields.io/npm/dt/%40nfdi4plants%2Farctrl?color=%234fb3d9"></a>|
|<a href="https://pypi.org/project/ARCtrl/"><img alt="PyPI" src="https://img.shields.io/pypi/v/arctrl?logo=pypi&color=%234fb3d9"></a>|<a href="https://pypi.org/project/ARCtrl/"><img alt="PyPI" src="https://img.shields.io/pepy/dt/arctrl?color=%234fb3d9"></a>|

## Install

#### .NET

```fsharp
#r "nuget: ARCtrl"
``` 

```bash
<PackageReference Include="ARCtrl" Version="1.1.0" />
```

#### JavaScript

```bash
npm i @nfdi4plants/arctrl
```

#### Python

```bash
pip install arctrl
```

## Docs

Currently we provide some documentation in form of markdown files in the `/docs` folder of this repository!

[Check it out!](/docs)

## Development

#### Requirements

- [nodejs and npm](https://nodejs.org/en/download)
    - verify with `node --version` (Tested with v18.16.1)
    - verify with `npm --version` (Tested with v9.2.0)
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - verify with `dotnet --version` (Tested with 7.0.306)
- [Python](https://www.python.org/downloads/)
    - verify with `py --version` (Tested with 3.12.2, known to work only for >=3.11)

#### Local Setup

On windows you can use the `setup.cmd` to run the following steps automatically!

1. Setup dotnet tools

   `dotnet tool restore`


2. Install NPM dependencies
   
    `npm install`

3. Setup python environment
    
    `py -m venv .venv`

4. Install [Poetry](https://python-poetry.org/) and dependencies

   1. `.\.venv\Scripts\python.exe -m pip install -U pip setuptools`
   2. `.\.venv\Scripts\python.exe -m pip install poetry`
   3. `.\.venv\Scripts\python.exe -m poetry install --no-root`

Verify correct setup with `./build.cmd runtests` ✨

## Performance

Measured on 13th Gen Intel(R) Core(TM) i7-13800H

| Name | Description | FSharp Time (ms) | JavaScript Time (ms) | Python Time (ms) |
| --- | --- | --- | --- | --- |
| Table_GetHashCode | From a table with 1 column and 10000 rows, retrieve the Hash Code |  5 | 21 | 226 |
| Table_AddRows | Add 10000 rows to a table with 4 columns. |  15 | 22 | 289 |
| Table_fillMissingCells | For a table 6 columns and 20000 rows, where each row has one missing value, fill those values with default values. | 49 | 108 | 4813 |
| Table_ToJson | Serialize a table with 5 columns and 10000 rows to json. |  1099 | 481 | 6833 |
| Table_ToCompressedJson | Serialize a table with 5 columns and 10000 rows to compressed json. |  261 |  2266 | 717334 |
| Assay_toJson | Parse an assay with one table with 10000 rows and 6 columns to json |  915 | 2459 | 28799 |
| Study_FromWorkbook | Parse a workbook with one study with 10000 rows and 6 columns to an ArcStudy |  97 | 87 | 1249 |
| Investigation_ToWorkbook_ManyStudies | Parse an investigation with 1500 studies to a workbook |  621 | 379 | 9974 |
