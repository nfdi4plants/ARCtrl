namespace ARCtrl

open Fable.Core

[<AttachMembers>]
type CompositeColumn = {
    Header: CompositeHeader
    Cells: ResizeArray<CompositeCell>
} with
    static member create (header: CompositeHeader, ?cells: ResizeArray<CompositeCell>) = 
        let cells = Option.defaultValue (ResizeArray()) cells
        {
            Header = header
            Cells = cells
        }

    /// Returns true if header and cells are a valid combination. E.g. Term header with term or unitized cells. IO header with freetext cells.
    ///
    /// ?raiseExeption: Default false. Set true if this function should raise an exception instead of return false.
    // TODO! Do not only check cells.Head
    member this.Validate(?raiseException: bool) =
        this.Cells
        |> Seq.exists (fun c -> c.ValidateAgainstHeader(this.Header, ?raiseException = raiseException) |> not)
        |> not

    /// <summary>
    /// Returns an array of all units found in the cells of this column. Returns None if no units are found.
    /// </summary>
    member this.TryGetColumnUnits() =
        let arr =
            [|
            for cell in this.Cells do
                if cell.isUnitized then
                    let _, unit = cell.AsUnitized
                    unit
            |]
            |> ResizeArray
        if Seq.isEmpty arr then None else Some arr

    /// <summary>
    /// Simple predictor for empty default cells.
    ///
    /// Currently uses majority vote for the column to decide cell type.
    /// </summary>
    member this.GetDefaultEmptyCell() =
            if not this.Header.IsTermColumn then
                CompositeCell.emptyFreeText
            else
                let unitCellCount, termCellCount =
                    this.Cells
                    |> Seq.fold (fun (units,terms) cell ->
                        if cell.isUnitized then (units+1,terms) else (units,terms+1)
                    ) (0,0)
                if termCellCount >= unitCellCount then
                    CompositeCell.emptyTerm
                else
                    CompositeCell.emptyUnitized

    member this.IsUnique =
        this.Header.IsUnique