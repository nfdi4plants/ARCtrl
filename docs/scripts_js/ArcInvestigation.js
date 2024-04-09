import { ArcInvestigation, Comment$ as Comment, XlsxController, JsonController} from "@nfdi4plants/arctrl"
// Import Spreadsheet to XLSX reader/writer
import {Xlsx} from "@fslab/fsspreadsheet";

// # Comments

const investigation_comments = ArcInvestigation.init("My Investigation")

const newComment = new Comment("The Name", "The Value")
const newComment2 = new Comment("My other Name", "My other Value")

investigation_comments.Comments.push(newComment)
investigation_comments.Comments.push(newComment2)

console.log(investigation_comments)

// # IO

// ## XLSX - Write

let fswb = XlsxController.Investigation.toFsWorkbook(investigation_comments)

console.log(fswb)

// Xlsx.toFile("test.isa.investigation.xlsx", fswb)

// Json - Write

const investigation = ArcInvestigation.init("My Investigation")

const json = JsonController.Investigation.toJsonString(investigation)

console.log(json)

// Json - Read

const jsonString = json

const investigation_2 = JsonController.Investigation.fromJsonString(jsonString)

console.log(investigation_2.Equals(investigation))