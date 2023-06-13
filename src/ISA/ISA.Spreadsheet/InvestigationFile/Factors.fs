namespace ISA.Spreadsheet

open ISA
open Comment
open Remark
open System.Collections.Generic

module Factors = 
    
    let nameLabel = "Name"
    let factorTypeLabel = "Type"
    let typeTermAccessionNumberLabel = "Type Term Accession Number"
    let typeTermSourceREFLabel = "Type Term Source REF"

    let labels = [nameLabel;factorTypeLabel;typeTermAccessionNumberLabel;typeTermSourceREFLabel]
    
    let fromString name designType typeTermSourceREF typeTermAccessionNumber comments =
        let factorType = OntologyAnnotation.fromString designType typeTermSourceREF typeTermAccessionNumber
        Factor.make 
            None 
            (Option.fromValueWithDefault "" name) 
            (Option.fromValueWithDefault OntologyAnnotation.empty factorType) 
            (Option.fromValueWithDefault [] comments)

    let fromSparseTable (matrix : SparseTable) =
        
        List.init matrix.Length (fun i -> 

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            fromString
                (matrix.TryGetValueDefault("",(nameLabel,i)))
                (matrix.TryGetValueDefault("",(factorTypeLabel,i)))
                (matrix.TryGetValueDefault("",(typeTermSourceREFLabel,i)))
                (matrix.TryGetValueDefault("",(typeTermAccessionNumberLabel,i)))
                comments
        )

    let toSparseTable (factors: Factor list) =
        let matrix = SparseTable.Create (keys = labels,length=factors.Length + 1)
        let mutable commentKeys = []
        factors
        |> List.iteri (fun i f ->
            let i = i + 1
            let factorType,source,accession = f.FactorType |> Option.defaultValue OntologyAnnotation.empty |> OntologyAnnotation.toString 
            do matrix.Matrix.Add ((nameLabel,i),                    (Option.defaultValue "" f.Name))
            do matrix.Matrix.Add ((factorTypeLabel,i),              factorType)
            do matrix.Matrix.Add ((typeTermAccessionNumberLabel,i), accession)
            do matrix.Matrix.Add ((typeTermSourceREFLabel,i),       source)

            match f.Comments with 
            | None -> ()
            | Some c ->
                c
                |> List.iter (fun comment -> 
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

       
    let toRows prefix (factors : Factor list) =
        factors
        |> toSparseTable
        |> fun m -> 
            match prefix with 
            | Some prefix -> SparseTable.ToRows(m,prefix)
            | None -> SparseTable.ToRows(m)
        