module TestObjects.Study

open FsSpreadsheet

let studyMetadataEmpty =
    let ws = FsWorksheet("isa_study")
    let row1 = ws.Row(1)
    row1.[1].Value <- "STUDY"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Study Identifier"
    let row3 = ws.Row(3)
    row3.[1].Value <- "Study Title"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Study Description"
    let row5 = ws.Row(5)
    row5.[1].Value <- "Study Submission Date"
    let row6 = ws.Row(6)
    row6.[1].Value <- "Study Public Release Date"
    let row7 = ws.Row(7)
    row7.[1].Value <- "Study File Name"
    ws

let studyMetadataEmptyObsoleteSheetName =
    let cp = studyMetadataEmpty.Copy()
    cp.Name <- "Study"
    cp