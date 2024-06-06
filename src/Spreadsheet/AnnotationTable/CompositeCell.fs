module ARCtrl.Spreadsheet.CompositeCell

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

let termFromFsCells (tsrCol : int option) (tanCol : int option ) (cells : list<FsCell>) : CompositeCell= 
    let cellValues = cells |> List.map (fun c -> c.ValueAsString())
    let tan = Option.map (fun i -> cellValues.[i]) tanCol
    let tsr = Option.map (fun i -> cellValues.[i]) tsrCol
    CompositeCell.createTermFromString(cellValues.[0],?tsr = tsr, ?tan = tan)

let unitizedFromFsCells (unitCol : int) (tsrCol : int option ) (tanCol : int option) (cells : list<FsCell>) : CompositeCell =
    let cellValues = cells |> List.map (fun c -> c.ValueAsString())
    let unit = cellValues.[unitCol]
    let tan = Option.map (fun i -> cellValues.[i]) tanCol
    let tsr = Option.map (fun i -> cellValues.[i]) tsrCol
    CompositeCell.createUnitizedFromString(cellValues.[0],unit,?tsr = tsr, ?tan = tan)

let freeTextFromFsCells (cells : list<FsCell>) : CompositeCell =
    let cellValues = cells |> List.map (fun c -> c.ValueAsString())
    CompositeCell.createFreeText cellValues.[0]

let dataFromFsCells (format : int option) (selectorFormat : int option) (cells : list<FsCell>) : CompositeCell =
    let cellValues = cells |> List.map (fun c -> c.ValueAsString())
    let format = Option.map (fun i -> cellValues.[i]) format
    let selectorFormat = Option.map (fun i -> cellValues.[i]) selectorFormat
    CompositeCell.createDataFromString(cellValues.[0],?format = format, ?selectorFormat = selectorFormat)


let toFsCells isTerm hasUnit (cell : CompositeCell) : list<FsCell> =
    match cell with
    | CompositeCell.FreeText v when hasUnit -> [FsCell(v); FsCell(""); FsCell(""); FsCell("")]
    | CompositeCell.FreeText v when isTerm -> [FsCell(v); FsCell(""); FsCell("")]
    | CompositeCell.FreeText v -> [FsCell(v)]

    | CompositeCell.Term v when hasUnit -> [FsCell(v.NameText); FsCell(""); FsCell(Option.defaultValue "" v.TermSourceREF); FsCell(v.TermAccessionOntobeeUrl)]
    | CompositeCell.Term v -> [FsCell(v.NameText); FsCell(Option.defaultValue "" v.TermSourceREF); FsCell(v.TermAccessionOntobeeUrl)]

    | CompositeCell.Unitized (v,unit) -> [FsCell(v); FsCell(unit.NameText); FsCell(Option.defaultValue "" unit.TermSourceREF); FsCell(unit.TermAccessionOntobeeUrl)]
    | CompositeCell.Data d -> 
        let format = d.Format |> Option.defaultValue "" |> FsCell
        let selectorFormat = d.SelectorFormat |> Option.defaultValue "" |> FsCell
        [FsCell(d.Name |> Option.defaultValue ""); format; selectorFormat]