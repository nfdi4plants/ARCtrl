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

    let tryGetValueAt i (vs : SparseRow) =
        vs 
        |> Seq.tryPick (fun (index,v) -> if index = i then Some v else None)

type SparseTable = 

    {
        Matrix : Dictionary<string*int,string>
        Keys : string list
        CommentKeys : string list
        ///Column Count
        Length : int
    }

    member this.TryGetValueDefault(defaultValue,key) =
        if this.Matrix.ContainsKey(key) then
            this.Matrix.Item(key)
        else
            defaultValue

    static member Create(?matrix,?keys,?commentKeys,?length) = 
        {
            Matrix= Option.defaultValue (Dictionary()) matrix
            Keys = Option.defaultValue [] keys
            CommentKeys = Option.defaultValue [] commentKeys
            Length = Option.defaultValue 0 length
        }

    static member AddRow key (values:seq<int*string>) (matrix : SparseTable) =

        values 
        |> Seq.iter (fun (i,v) -> 
            matrix.Matrix.Add((key,i),v)
        )

        let length = 
            if Seq.isEmpty values then 0
            else Seq.maxBy fst values |> fst |> (+) 1
        
        {matrix with 
            Keys = List.append matrix.Keys [key]
            Length = if length > matrix.Length then length else matrix.Length
        }

    static member AddComment key (values:seq<int*string>) (matrix : SparseTable) =
        
        values 
        |> Seq.iter (fun (i,v) -> 
            matrix.Matrix.Add((key,i),v)
        )

        let length = 
            if Seq.isEmpty values then 0
            else Seq.maxBy fst values |> fst |> (+) 1
        
        {matrix with 
            CommentKeys = List.append matrix.CommentKeys [key]
            Length = if length > matrix.Length then length else matrix.Length
        }

    static member FromRows(en:IEnumerator<SparseRow>,labels,lineNumber,?prefix) =
        let prefix = match prefix with | Some p -> p + " " | None -> ""
        let rec loop (matrix : SparseTable) remarks lineNumber = 

            if en.MoveNext() then  
                let row = en.Current |> Seq.map (fun (i,v) -> int i - 1,v)
                match Seq.tryItem 0 row |> Option.map snd, Seq.trySkip 1 row with

                | Comment.Comment k, Some v -> 
                    loop (SparseTable.AddComment k v matrix) remarks (lineNumber + 1)

                | Remark.Remark k, _  -> 
                    loop matrix (Remark.make lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some v when List.exists (fun label -> k = prefix + label) labels -> 
                    let label = List.find (fun label -> k = prefix + label) labels
                    loop (SparseTable.AddRow label v matrix) remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks, matrix
                | _ -> None, lineNumber,remarks, matrix
            else
                None,lineNumber,remarks, matrix
        loop (SparseTable.Create()) [] lineNumber

    static member ToRows(matrix,?prefix) : SparseRow seq =
        let prefix = match prefix with | Some p -> p + " " | None -> ""
        seq {
            for key in matrix.Keys do
                (SparseRow.fromValues (prefix + key :: List.init (matrix.Length - 1) (fun i -> matrix.TryGetValueDefault("",(key,i + 1)))))
            for key in matrix.CommentKeys do
                (SparseRow.fromValues (Comment.wrapCommentKey key :: List.init (matrix.Length - 1) (fun i -> matrix.TryGetValueDefault("",(key,i + 1)))))
        }

