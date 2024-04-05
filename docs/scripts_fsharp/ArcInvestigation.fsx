#r "nuget: FsSpreadsheet.Net"
#r "nuget: ARCtrl, 2.0.0-alpha.2"

open ARCtrl

// # Comments
let investigation_comments = ArcInvestigation.init("My Investigation")
let newComment = Comment("The Name", "The Value")
let newComment2 = Comment("My other Name", "My other Value")

// This might be changed to a ResizeArray in the future
investigation_comments.Comments.AddRange([|newComment; newComment2|])


// # IO

// ## Xlsx - Write
open ARCtrl.Spreadsheet
open FsSpreadsheet.Net

let fswb = ArcInvestigation.toFsWorkbook investigation_comments

fswb.ToXlsxFile("test2.isa.investigation.xlsx")

// ## Json - Write

open ARCtrl.Json

let investigation = ArcInvestigation.init("My Investigation")
let json = ArcInvestigation.toJsonString () investigation

// ## Json - Read

let jsonString = json

let investigation' = ArcInvestigation.fromJsonString jsonString

investigation = investigation' //true