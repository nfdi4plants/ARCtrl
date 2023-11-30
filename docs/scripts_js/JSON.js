import {ArcInvestigation} from "@nfdi4plants/arctrl"
import {ArcInvestigation_toJsonString, ArcInvestigation_fromJsonString} from "@nfdi4plants/arctrl/ISA/ISA.Json/Investigation.js"

// Write

const investigation = ArcInvestigation.init("My Investigation")

const json = ArcInvestigation_toJsonString(investigation)

console.log(json)

// Read

const jsonString = json

const investigation_2 = ArcInvestigation_fromJsonString(jsonString)

console.log(investigation_2.Equals(investigation))