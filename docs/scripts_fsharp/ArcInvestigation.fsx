#r "nuget: FsSpreadsheet.Net"
#r "nuget: ARCtrl"

open ARCtrl.ISA

// # Comments
let investigation_comments = ArcInvestigation.init("My Investigation")
let newComment = Comment.create("The Id", "The Name", "The Value")
let newComment2 = Comment.create("My other ID", "My other Name", "My other Value")

// This might be changed to a ResizeArray in the future
investigation_comments.Comments <- Array.append investigation_comments.Comments [|newComment; newComment2|]


// # IO

// ## Xlsx - Write
open ARCtrl.ISA.Spreadsheet
open FsSpreadsheet.Net

let fswb = ArcInvestigation.toFsWorkbook investigation_comments

fswb.ToXlsxFile("test2.isa.investigation.xlsx")

// ## Json - Write

open ARCtrl.ISA
open ARCtrl.ISA.Json

let investigation = ArcInvestigation.init("My Investigation")
let json = ArcInvestigation.toJsonString investigation

// ## Json - Read

let jsonString = json

let investigation' = ArcInvestigation.fromJsonString jsonString

investigation = investigation' //true