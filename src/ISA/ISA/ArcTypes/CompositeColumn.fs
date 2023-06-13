namespace ISA

open Fable.Core

[<AttachMembers>]
type CompositeColumn = {
    Header: CompositeHeader
    Cells: CompositeCell []
} with
    static member create (header: CompositeHeader, ?cells: CompositeCell []) = 
        let cells = Option.defaultValue [||] cells
        {
            Header = header
            Cells = cells
        }

    /// Returns true if header and cells are a valid combination. E.g. Term header with term or unitized cells. IO header with freetext cells.
    ///
    /// ?raiseExeption: Default false. Set true if this function should raise an exception instead of return false.
    member this.validate(?raiseExeption: bool) =
        let raiseExeption = Option.defaultValue false raiseExeption
        let header = this.Header
        let cells = this.Cells
        match header, cells with
        // no cell values will be handled later and is no error case
        | _, emptyCell when cells.Length = 0 -> 
            true
        | isTerm when header.IsTermColumn && (cells.[0].isTerm || cells.[0].isUnitized) -> 
            true
        | isNotTerm when not header.IsTermColumn && cells.[0].isFreeText -> 
            true
        | h, c -> 
            if raiseExeption then 
                let n = System.Math.Min(c.Length,3)
                let exampleCells = c.[n]
                let msg = $"Invalid combination of header `{h}` and cells `{exampleCells}`"
                failwith msg
            // Maybe still return `msg` somehow if `raiseExeption` is false?
            false

