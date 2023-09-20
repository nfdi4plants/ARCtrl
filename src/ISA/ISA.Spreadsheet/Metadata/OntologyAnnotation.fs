namespace ARCtrl.ISA.Spreadsheet

open ARCtrl.ISA
open Comment
open Remark
open System.Collections.Generic


module OntologyAnnotationSection = 

    let fromSparseTable label labelTAN labelTSR (matrix : SparseTable) =
        if matrix.ColumnCount = 0 && matrix.CommentKeys.Length <> 0 then
            let comments = SparseTable.GetEmptyComments matrix
            OntologyAnnotation.create(Comments = comments)
            |> List.singleton
        else
            List.init matrix.ColumnCount (fun i -> 

                let comments = 
                    matrix.CommentKeys 
                    |> List.map (fun k -> 
                        Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))
                    |> Array.ofList
                    |> Option.fromValueWithDefault [||]

                OntologyAnnotation.fromString(
                    ?termName = matrix.TryGetValue(label,i),
                    ?tsr = matrix.TryGetValue(labelTAN,i),
                    ?tan = matrix.TryGetValue(labelTSR,i),
                    ?comments = comments
                )
            )

    let toSparseTable label labelTAN labelTSR (designs: OntologyAnnotation list) =
        let matrix = SparseTable.Create (keys = [label;labelTAN;labelTSR],length=designs.Length + 1)
        let mutable commentKeys = []
        designs
        |> List.iteri (fun i d ->
            let i = i + 1
            let oa = OntologyAnnotation.toString(d,true)
            do matrix.Matrix.Add ((label,i),                      oa.TermName)
            do matrix.Matrix.Add ((labelTAN,i),   oa.TermAccessionNumber)
            do matrix.Matrix.Add ((labelTSR,i),         oa.TermSourceREF)

            match d.Comments with 
            | None -> ()
            | Some c ->
                c
                |> Array.iter (fun comment -> 
                    let n,v = comment |> Comment.toString
                    commentKeys <- n :: commentKeys
                    matrix.Matrix.Add((n,i),v)
                )
        )
        {matrix with CommentKeys = commentKeys |> List.distinct |> List.rev} 


    let fromRows (prefix : string option) label labelTAN labelTSR lineNumber (rows : IEnumerator<SparseRow>) =
        let labels = [label;labelTAN;labelTSR]
        match prefix with
        | Some p -> SparseTable.FromRows(rows,labels,lineNumber,p)
        | None -> SparseTable.FromRows(rows,labels,lineNumber)
        |> fun (s,ln,rs,sm) -> (s,ln,rs, fromSparseTable label labelTAN labelTSR  sm)  
    
    let toRows (prefix : string option) label labelTAN labelTSR (designs : OntologyAnnotation list) =
        designs
        |> toSparseTable label labelTAN labelTSR
        |> fun m -> 
            match prefix with 
            | Some prefix -> SparseTable.ToRows(m,prefix)
            | None -> SparseTable.ToRows(m)