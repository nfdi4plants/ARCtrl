# ARCtrl

> **ARCtrl** the easy way to read, manipulate and write ARCs in __.NET__ 
and __JavaScript__! ❤️

| Version | Downloads |
| :--------|-----------:|
|<a href="https://www.nuget.org/packages/ARCtrl/"><img alt="Nuget" src="https://img.shields.io/nuget/vpre/ARCtrl?logo=nuget&color=%234fb3d9"></a>|<a href="https://www.nuget.org/packages/ARCtrl/"><img alt="Nuget" src="https://img.shields.io/nuget/dt/ARCtrl?color=%234FB3D9"></a>|
|<a href="https://www.npmjs.com/package/@nfdi4plants/arctrl"><img alt="NPM" src="https://img.shields.io/npm/v/%40nfdi4plants/arctrl?logo=npm&color=%234fb3d9"></a>|<a href="https://www.npmjs.com/package/@nfdi4plants/arctrl"><img alt="NPM" src="https://img.shields.io/npm/dt/%40nfdi4plants%2Farctrl?color=%234fb3d9"></a>|


## Install

(currently only prereleases available, check the [nuget page](https://www.nuget.org/packages/ARCtrl) or [npm page](https://www.npmjs.com/package/@nfdi4plants/arctrl) respectively)

### .NET

```fsharp
#r "nuget: ARCtrl"
``` 

```bash
<PackageReference Include="ARCtrl" Version="1.1.0" />
```

### JavaScript

```bash
npm i @nfdi4plants/arctrl
```

## Docs

Currently we provide some documentation in form of markdown files in the `/docs` folder of this repository!

[Check it out!](/docs)

## Development

### Requirements

- [nodejs and npm](https://nodejs.org/en/download)
    - verify with `node --version` (Tested with v18.16.1)
    - verify with `npm --version` (Tested with v9.2.0)
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - verify with `dotnet --version` (Tested with 7.0.306)
- [Python](https://www.python.org/downloads/)
    - verify with `py --version` (Tested with 3.12.2, known to work only for >=3.11)

### Local Setup

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
