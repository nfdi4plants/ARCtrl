import { ArcInvestigation, Comment$ as Comment} from "@nfdi4plants/arctrl"
// Import Spreadsheet to XLSX reader/writer
import {Xlsx} from "@fslab/fsspreadsheet";
// Import ARCtrl Investigation to Spreadsheet transformation
import {toFsWorkbook, fromFsWorkbook} from "@nfdi4plants/arctrl/ISA/ISA.Spreadsheet/ArcInvestigation.js"
import {ArcInvestigation_toJsonString, ArcInvestigation_fromJsonString} from "@nfdi4plants/arctrl/ISA/ISA.Json/Investigation.js"

// # Comments

const investigation_comments = ArcInvestigation.init("My Investigation")

const newComment = Comment.create("The Id", "The Name", "The Value")
const newComment2 = Comment.create("My other ID", "My other Name", "My other Value")

investigation_comments.Comments.push(newComment)
investigation_comments.Comments.push(newComment2)

console.log(investigation_comments)

// # IO

// ## XLSX - Write

let fswb = toFsWorkbook(investigation_comments)

Xlsx.toFile("test.isa.investigation.xlsx", fswb)

// Json - Write

const investigation = ArcInvestigation.init("My Investigation")

const json = ArcInvestigation_toJsonString(investigation)

console.log(json)

// Json - Read

const jsonString = json

const investigation_2 = ArcInvestigation_fromJsonString(jsonString)

console.log(investigation_2.Equals(investigation))