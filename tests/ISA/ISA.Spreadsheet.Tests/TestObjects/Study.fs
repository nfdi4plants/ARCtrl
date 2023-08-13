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

[<Literal>]
let studyIdentifier = "BII-S-1"

let studyMetadata =
    let ws = FsWorksheet("isa_study")
    let row1 = ws.Row(1)
    row1.[1].Value <- "STUDY"
    let row2 = ws.Row(2)
    row2.[1].Value <- "Study Identifier"
    row2.[2].Value <- studyIdentifier
    let row3 = ws.Row(3)
    row3.[1].Value <- "Study Title"
    row3.[2].Value <- "Study of the impact of changes in flux on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae under different nutrient limitations"
    let row4 = ws.Row(4)
    row4.[1].Value <- "Study Description"
    row4.[2].Value <- "We wished to study the impact of growth rate on the total complement of mRNA molecules, proteins, and metabolites in S. cerevisiae, independent of any nutritional or other physiological effects. To achieve this, we carried out our analyses on yeast grown in steady-state chemostat culture under four different nutrient limitations (glucose, ammonium, phosphate, and sulfate) at three different dilution (that is, growth) rates (D = u = 0.07, 0.1, and 0.2/hour, equivalent to population doubling times (Td) of 10 hours, 7 hours, and 3.5 hours, respectively; u = specific growth rate defined as grams of biomass generated per gram of biomass present per unit time)."
    let row5 = ws.Row(5)
    row5.[1].Value <- "Study Submission Date"
    row5.[2].Value <- "2007-04-30"
    let row6 = ws.Row(6)
    row6.[1].Value <- "Study Public Release Date"
    row6.[2].Value <- "2009-03-10"
    let row7 = ws.Row(7)
    row7.[1].Value <- "Study File Name"
    row7.[2].Value <- $"studies/{studyIdentifier}/isa.study.xlsx"
    ws

let studyMetadataEmptyObsoleteSheetName =
    let cp = studyMetadataEmpty.Copy()
    cp.Name <- "Study"
    cp