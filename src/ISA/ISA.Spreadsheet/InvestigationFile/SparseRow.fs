namespace ISADotNet.XLSX

open ISADotNet
open System.Collections.Generic
open FSharpSpreadsheetML
open DocumentFormat.OpenXml.Spreadsheet


type SparseRow = (int * string) seq

module SparseRow =

    let fromValues (v : string seq) : SparseRow = Seq.indexed v 

    let getValues (i : SparseRow) = i |> Seq.map snd

    let fromAllValues (v : string option seq) : SparseRow = 
        Seq.indexed v 
        |> Seq.choose (fun (i,o) -> Option.map (fun v -> i,v) o)
    
    let getAllValues (i : SparseRow) = 
        let m = i |> Map.ofSeq
        let max = i |> Seq.maxBy fst |> fst
        Seq.init (max + 1) (fun i -> Map.tryFind i m)
