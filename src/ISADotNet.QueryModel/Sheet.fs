namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections

/// Queryable type representing a collection of processes implementing the same protocol. Or in ISAtab / ISAXLSX logic a sheet in an assay or study file.
///
/// Values are represented rowwise with input and output entities.
type QSheet = 
    {
        [<JsonPropertyName(@"sheetName")>]
        SheetName : string
        [<JsonPropertyName(@"rows")>]
        Rows : QRow list
    }

    static member create sheetName rows : QSheet =
        {
            SheetName = sheetName
            Rows = rows
        }

    static member fromProcesses name (processes : Process list) =        
        processes
        |> List.collect (QRow.fromProcess)
        |> QSheet.create name 

    member this.Values = 
        this.Rows
        |> List.collect (fun r -> 
            r.Vals
            |> List.map (fun v -> 
                KeyValuePair ((r.Input,r.Output),v)
            )
        )
        |> IOValueCollection

    member this.Item (i : int) =
        this.Rows.[i]

    member this.Item (input : string) =
        let row = 
            this.Rows 
            |> List.tryFind (fun r -> r.Input = input)
        match row with
        | Some r -> r
        | None -> failwith $"Sheet \"{this.SheetName}\" does not contain row with input \"{input}\""

    member this.RowCount =
        this.Rows 
        |> List.length

    member this.InputNames =
        this.Rows 
        |> List.map (fun row -> row.Input)

    member this.OutputNames =
        this.Rows 
        |> List.map (fun row -> row.Output)
    
    member this.Inputs =
        this.Rows 
        |> List.map (fun row -> row.Input, row.InputType)

    member this.Outputs =
        this.Rows 
        |> List.map (fun row -> row.Output,row.OutputType)

    interface IEnumerable<QRow> with
        member this.GetEnumerator() = (seq this.Rows).GetEnumerator()

    interface IEnumerable with
        member this.GetEnumerator() = (this :> IEnumerable<QRow>).GetEnumerator() :> IEnumerator