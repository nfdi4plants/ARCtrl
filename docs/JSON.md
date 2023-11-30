# ISA-JSON

**Table of contents**
- [Write](#write)
- [Read](#read)

**Code can be found here**
- [F#](/docs/scripts_fsharp/JSON.fsx)
- [JavaScript](/docs/scripts_js/JSON.js)

ARCtrl ISA fully supports the [ISA-JSON](https://isa-specs.readthedocs.io/en/latest/isajson.html) schema! This means our ARCtrl.ISA model can be read from ISA-JSON as well as write to it.

# Write

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

# Read

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