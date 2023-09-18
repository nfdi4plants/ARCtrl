namespace ARCtrl.ISA.Spreadsheet

open ARCtrl.ISA
open Comment
open Remark
open System.Collections.Generic

module DesignDescriptors = 

    let designTypeLabel = "Type"
    let designTypeTermAccessionNumberLabel = "Type Term Accession Number"
    let designTypeTermSourceREFLabel = "Type Term Source REF"

    let labels = [designTypeLabel;designTypeTermAccessionNumberLabel;designTypeTermSourceREFLabel]

    let fromSparseTable (matrix : SparseTable) =
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
                    ?termName = matrix.TryGetValue(designTypeLabel,i),
                    ?tsr = matrix.TryGetValue(designTypeTermSourceREFLabel,i),
                    ?tan = matrix.TryGetValue(designTypeTermAccessionNumberLabel,i),
                    ?comments = comments
                )
            )

    let toSparseTable (designs: OntologyAnnotation list) =
        let matrix = SparseTable.Create (keys = labels,length=designs.Length + 1)
        let mutable commentKeys = []
        designs
        |> List.iteri (fun i d ->
            let i = i + 1
            let oa = OntologyAnnotation.toString(d,true)
            do matrix.Matrix.Add ((designTypeLabel,i),                      oa.TermName)
            do matrix.Matrix.Add ((designTypeTermAccessionNumberLabel,i),   oa.TermAccessionNumber)
            do matrix.Matrix.Add ((designTypeTermSourceREFLabel,i),         oa.TermSourceREF)

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


    let fromRows (prefix : string option) lineNumber (rows : IEnumerator<SparseRow>) =
        match prefix with
        | Some p -> SparseTable.FromRows(rows,labels,lineNumber,p)
        | None -> SparseTable.FromRows(rows,labels,lineNumber)
        |> fun (s,ln,rs,sm) -> (s,ln,rs, fromSparseTable sm)  
    
    let toRows (prefix : string option) (designs : OntologyAnnotation list) =
        designs
        |> toSparseTable
        |> fun m -> 
            match prefix with 
            | Some prefix -> SparseTable.ToRows(m,prefix)
            | None -> SparseTable.ToRows(m)