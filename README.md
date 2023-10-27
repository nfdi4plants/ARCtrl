# ARCtrl

> **ARCtrl** the easy way to read, manipulate and write ARCs in __.NET__ 
and __JavaScript__! ❤️

| Version | Downloads |
| --------|-----------|
|![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/ARCtrl?logo=nuget&color=%234fb3d9)|![Nuget](https://img.shields.io/nuget/dt/ARCtrl?color=%234FB3D9)|
|![npm (scoped with tag)](https://img.shields.io/npm/v/%40nfdi4plants/arctrl/next?logo=npm&color=%234fb3d9)|![npm](https://img.shields.io/npm/dt/%40nfdi4plants%2Farctrl?color=%234fb3d9)|

## Install

(currently only prereleases available, check the [nuget page](https://www.nuget.org/packages/ARCtrl) or [npm page](https://www.npmjs.com/package/@nfdi4plants/arctrl) respectively)

### .NET

```fsharp
#r "nuget: ARCtrl"
``` 

```bash
<PackageReference Include="ARCtrl" Version="1.0.0-alpha9" />
```

### JavaScript

```bash
npm i @nfdi4plants/arctrl
```

## Docs

Currently we provide some documentation in form of markdown fiels in the `/docs` folder of this repository!

[Check it out!](/docs)

## Development

### Requirements

- [nodejs and npm](https://nodejs.org/en/download)
    - verify with `node --version` (Tested with v18.16.1)
    - verify with `npm --version` (Tested with v9.2.0)
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - verify with `dotnet --version` (Tested with 7.0.306)

### Local Setup

1. `dotnet tool restore`
2. `dotnet paket install`
3. `npm install`

Verify correct setup with `./build.cmd runtests` ✨