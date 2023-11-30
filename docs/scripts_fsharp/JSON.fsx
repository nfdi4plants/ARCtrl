#r "nuget: ARCtrl, 1.0.0-beta.9"

open ARCtrl.ISA
open ARCtrl.ISA.Json

// Write

let investigation = ArcInvestigation.init("My Investigation")

let json = ArcInvestigation.toJsonString investigation

// Read

let jsonString = json

let investigation' = ArcInvestigation.fromJsonString jsonString

investigation = investigation' //true