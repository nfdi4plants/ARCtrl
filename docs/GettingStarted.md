# Getting Started

- [Setup - .NET](#setup---net)
- [Setup - JavaScript](#setup---javascript)

## Setup - .NET

1. Install [.NET SDK](https://dotnet.microsoft.com/en-us/download).
2. Get the [ARCtrl nuget package](www.nuget.org/packages/ARCtrl).
3. (*OPTIONAL*) Use [FsSpreadsheet.ExcelIO](https://www.nuget.org/packages/FsSpreadsheet.ExcelIO) to get any xlsx files into the correct format with our supported readers.
4. (*OPTIONAL*) You can use contract handling from [ARCtrl.NET](https://www.nuget.org/packages/ARCtrl.NET). This documentation will avoid using ARCtrl.NET and extract the relevant functions.

Thats it! 🎉 

For any documentation we assume using ARCtrl from a .fsx file. Verify correct setup by creating a `ARC.fsx` file with the following content

```fsharp
#r "nuget: FsSpreadsheet.ExcelIO, 5.0.2"
#r "nuget: ARCtrl, 1.0.0-beta.8"

open ARCtrl

printfn "%A" <| ARC()
// > printfn "%A" <| ARC();;
// ARCtrl.ARC
// val it: unit = ()
```

You can open and run this code from [VisualStudio Code](https://code.visualstudio.com) with the [Ionide](https://ionide.io) extension. Running the code should return the result written as comment at the end of the file.

## Setup - JavaScript

1. Install [nodejs](https://nodejs.dev/en/download/)
2. Create folder and inside run `npm init`.
    - This will trigger a chain of questions creating a `package.json` file. Answer them as you see fit.
3. Edit the `package.json` file to include ``"type": "module"``. This allows us to use `import` syntax ([read more](https://nodejs.org/docs/latest-v13.x/api/esm.html#esm_enabling)).
4. Install the npm package [@nfdi4plants/arctrl](https://www.npmjs.com/package/@nfdi4plants/arctrl) with `npm i @nfdi4plants/arctrl`.
5. (*OPTIONAL*) Install the npm package [fsspreadsheet](https://www.npmjs.com/package/fsspreadsheet) with `npm i fsspreadsheet`.
    - This will be moved to the `@fslab` organisation soon!

Thats it! 🎉

Your `package.json` might look similiar to this:

```json
{
  "name": "docs",
  "version": "1.0.0",
  "description": "This subproject is only used to create test script used for documentation",
  "type": "module",
  "scripts": {
    "test": "echo \"Error: no test specified\" && exit 1"
  },
  "author": "Kevin Frey",
  "license": "MIT",
  "dependencies": {
    "@nfdi4plants/arctrl": "^1.0.0-alpha9",
    "fsspreadsheet": "^4.0.0-alpha2"
  }
}
```

You can now reference ARCtrl in any `.js` file and run it with `node path/to/Any.js`.

Verify correct setup by creating `ARC.js` file with the content from below in the same folder, which contains your `package.json`. Then run `node ./Arc.js`. This will print `[class ARC]` into the console.

```js
// ARC.js
import {ARC} from "@nfdi4plants/arctrl";

console.log(ARC) // [class ARC]
```