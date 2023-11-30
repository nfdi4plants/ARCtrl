#r "nuget: FsSpreadsheet.ExcelIO, 5.0.2"
#r "nuget: ARCtrl, 1.0.0-beta.8"

open ARCtrl.ISA

// Comments
let investigation_comments = ArcInvestigation.init("My Investigation")
let newComment = Comment.create("The Id", "The Name", "The Value")
let newComment2 = Comment.create("My other ID", "My other Name", "My other Value")

// This might be changed to a ResizeArray in the future
investigation_comments.Comments <- Array.append investigation_comments.Comments [|newComment; newComment2|]


// IO
open ARCtrl.ISA.Spreadsheet
open FsSpreadsheet.ExcelIO

let fswb = ArcInvestigation.toFsWorkbook investigation_comments

fswb.ToFile("test2.isa.investigation.xlsx")