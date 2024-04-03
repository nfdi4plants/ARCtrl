module ARCtrl.Spreadsheet.CompositeCell

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

let fromFsCells (cells : list<FsCell>) : CompositeCell =
    let cellValues = cells |> List.map (fun c -> c.ValueAsString())
    match cellValues with
    | [v] -> CompositeCell.createFreeText v
    | [v1;v2;v3] -> CompositeCell.createTermFromString(v1,v2,v3)
    | [v1;v2;v3;v4] -> CompositeCell.createUnitizedFromString(v1,v2,v3,v4)
    | _ -> 
        failwithf "Dafuq"

let toFsCells isTerm hasUnit (cell : CompositeCell) : list<FsCell> =
    match cell with
    | CompositeCell.FreeText v when hasUnit -> [FsCell(v); FsCell(""); FsCell(""); FsCell("")]
    | CompositeCell.FreeText v when isTerm -> [FsCell(v); FsCell(""); FsCell("")]
    | CompositeCell.FreeText v -> [FsCell(v)]

    | CompositeCell.Term v when hasUnit -> [FsCell(v.NameText); FsCell(""); FsCell(Option.defaultValue "" v.TermSourceREF); FsCell(v.TermAccessionOntobeeUrl)]
    | CompositeCell.Term v -> [FsCell(v.NameText); FsCell(Option.defaultValue "" v.TermSourceREF); FsCell(v.TermAccessionOntobeeUrl)]

    | CompositeCell.Unitized (v,unit) -> [FsCell(v); FsCell(unit.NameText); FsCell(Option.defaultValue "" unit.TermSourceREF); FsCell(unit.TermAccessionOntobeeUrl)]