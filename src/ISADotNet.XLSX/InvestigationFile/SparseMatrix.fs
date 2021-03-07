namespace ISADotNet.XLSX

open ISADotNet
open System.Collections.Generic
open FSharpSpreadsheetML

type SparseMatrix = 

    {
        Matrix : Dictionary<string*int,string>
        Keys : string list
        CommentKeys : string list
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

    static member AddRow key (values:seq<int*string>) (matrix : SparseMatrix) =
        let values = 
            values 
            |> Seq.map (fun (i,v) -> 
                let i = i-1
                matrix.Matrix.Add((key,i),v)
                i,v
            )
            |> Seq.toArray

        let length = 
            if Seq.isEmpty values then 0
            else Seq.maxBy fst values |> fst |> (+) 1
        
        {matrix with 
            Keys = List.append matrix.Keys [key]
            Length = if length > matrix.Length then length else matrix.Length
        }

    static member AddComment key (values:seq<int*string>) (matrix : SparseMatrix) =
        let values = 
            values 
            |> Seq.map (fun (i,v) -> 
                let i = i-1
                matrix.Matrix.Add((key,i),v)
                i,v
            )
            |> Seq.toArray
        let length = 
            if Seq.isEmpty values then 0
            else Seq.maxBy fst values |> fst |> (+) 1
        
        {matrix with 
            CommentKeys = List.append matrix.CommentKeys [key]
            Length = if length > matrix.Length then length else matrix.Length
        }

    static member ToRows(matrix,?prefix) =
        let prefix = match prefix with | Some p -> p + " " | None -> ""
        seq {
            for key in matrix.Keys do
                (Row.ofValues None 0u (prefix + key :: List.init matrix.Length (fun i -> matrix.TryGetValueDefault("",(key,i)))))
            for key in matrix.CommentKeys do
                (Row.ofValues None 0u (Comment.wrapCommentKey key :: List.init matrix.Length (fun i -> matrix.TryGetValueDefault("",(key,i)))))
        }




