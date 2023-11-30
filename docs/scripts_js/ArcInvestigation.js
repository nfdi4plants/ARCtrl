import { ArcInvestigation, Comment$ as Comment} from "@nfdi4plants/arctrl"
// Import Spreadsheet to XLSX reader/writer
import {Xlsx} from "@fslab/fsspreadsheet";
// Import ARCtrl Investigation to Spreadsheet transformation
import {toFsWorkbook, fromFsWorkbook} from "@nfdi4plants/arctrl/ISA/ISA.Spreadsheet/ArcInvestigation.js"

// Comments

const investigation_comments = ArcInvestigation.init("My Investigation")

const newComment = Comment.create("The Id", "The Name", "The Value")
const newComment2 = Comment.create("My other ID", "My other Name", "My other Value")

investigation_comments.Comments.push(newComment)
investigation_comments.Comments.push(newComment2)

console.log(investigation_comments)

// IO

let fswb = toFsWorkbook(investigation_comments)

Xlsx.toFile("test.isa.investigation.xlsx", fswb)