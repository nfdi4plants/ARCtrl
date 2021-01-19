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
            Keys = key :: matrix.Keys
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
            CommentKeys = key :: matrix.CommentKeys
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

        
module Seq = 
    
    /// If at least i values exist in seq a, builds a new array that contains the elements of the given seq, exluding the first i elements
    let trySkip i s =
        try
            Seq.skip i s
            |> Some
        with
        | _ -> None

module Array = 
    
    let ofIndexedSeq (s : seq<int*string>) = 
        Array.init 
            (Seq.maxBy fst s |> fst |> (+) 1)
            (fun i -> 
                match Seq.tryFind (fst >> (=) i) s with
                | Some (i,x) -> x
                | None -> ""               
            )

    /// If at least i values exist in array a, builds a new array that contains the elements of the given array, exluding the first i elements
    let trySkip i a =
        try
            Array.skip i a
            |> Some
        with
        | _ -> None

    /// Returns Item of array at index i if existing, else returns default value
    let tryItemDefault i d a = 
        match Array.tryItem i a with
        | Some v -> v
        | None -> d

    let map4 (f: 'A -> 'B -> 'C -> 'D -> 'T) (aa:'A []) (ba:'B []) (ca:'C []) (da:'D []) =
        if not (aa.Length = ba.Length && ba.Length = ca.Length && ca.Length = da.Length) then
            failwith ""
        Array.init aa.Length (fun i -> f aa.[i] ba.[i] ca.[i] da.[i])


module List =

    let tryPickDefault (chooser : 'T -> 'U Option) (d : 'U) (list : 'T list) =
        match List.tryPick chooser list with
        | Some v -> v
        | None -> d

    let unzip4 (l : ('A*'B*'C*'D) list ) =
        let rec loop la lb lc ld l =
            match l with
            | (a,b,c,d) :: l -> loop (a::la) (b::lb) (c::lc) (d::ld) l
            | [] -> List.rev la, List.rev lb, List.rev lc, List.rev ld
        loop [] [] [] [] l



