module ARCtrl.Spreadsheet.CompositeCell

open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet

let termFromStringCells (tsrCol : int option) (tanCol : int option ) (cellValues : array<string>) : CompositeCell= 
    let tan = Option.map (fun i -> cellValues.[i]) tanCol
    let tsr = Option.map (fun i -> cellValues.[i]) tsrCol
    CompositeCell.createTermFromString(cellValues.[0],?tsr = tsr, ?tan = tan)

let unitizedFromStringCells (unitCol : int) (tsrCol : int option ) (tanCol : int option) (cellValues : array<string>) : CompositeCell =
    let unit = cellValues.[unitCol]
    let tan = Option.map (fun i -> cellValues.[i]) tanCol
    let tsr = Option.map (fun i -> cellValues.[i]) tsrCol
    CompositeCell.createUnitizedFromString(cellValues.[0],unit,?tsr = tsr, ?tan = tan)

let freeTextFromStringCells (cellValues : array<string>) : CompositeCell =
    CompositeCell.createFreeText cellValues.[0]

let dataFromStringCells (format : int option) (selectorFormat : int option) (cellValues : array<string>) : CompositeCell =
    let format = Option.bind (fun i -> cellValues.[i] |> Option.fromValueWithDefault "") format
    let selectorFormat = Option.bind (fun i -> cellValues.[i] |> Option.fromValueWithDefault "") selectorFormat
    CompositeCell.createDataFromString(cellValues.[0],?format = format, ?selectorFormat = selectorFormat)


let toStringCells isTerm hasUnit (cell : CompositeCell) : array<string> =
    match cell with
    | CompositeCell.FreeText v when hasUnit -> [|v; ""; ""; ""|]
    | CompositeCell.FreeText v when isTerm -> [|v; ""; ""|]
    | CompositeCell.FreeText v -> [|v|]

    | CompositeCell.Term v when hasUnit -> [|v.NameText; ""; Option.defaultValue "" v.TermSourceREF; v.TermAccessionOntobeeUrl|]
    | CompositeCell.Term v -> [|v.NameText; Option.defaultValue "" v.TermSourceREF; v.TermAccessionOntobeeUrl|]

    | CompositeCell.Unitized (v,unit) -> [|v; unit.NameText; Option.defaultValue "" unit.TermSourceREF; unit.TermAccessionOntobeeUrl|]
    | CompositeCell.Data d -> 
        let format = d.Format |> Option.defaultValue ""
        let selectorFormat = d.SelectorFormat |> Option.defaultValue ""
        [|d.Name |> Option.defaultValue ""; format; selectorFormat|]