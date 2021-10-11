namespace ISADotNet.XLSX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open ISADotNet.API
open Comment
open Remark
open System.Collections.Generic

module DesignDescriptors = 

    let designTypeLabel = "Type"
    let designTypeTermAccessionNumberLabel = "Type Term Accession Number"
    let designTypeTermSourceREFLabel = "Type Term Source REF"

    let labels = [designTypeLabel;designTypeTermAccessionNumberLabel;designTypeTermSourceREFLabel]

    let fromString designType typeTermSourceREF typeTermAccessionNumber comments =
        OntologyAnnotation.make 
            None 
            (Option.fromValueWithDefault "" designType |> Option.map AnnotationValue.fromString)
            (Option.fromValueWithDefault "" typeTermSourceREF)
            (Option.fromValueWithDefault "" typeTermAccessionNumber |> Option.map URI.fromString)
            (Option.fromValueWithDefault [] comments)

    let fromSparseTable (matrix : SparseTable) =
        
        List.init matrix.Length (fun i -> 

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            fromString
                (matrix.TryGetValueDefault("",(designTypeLabel,i)))
                (matrix.TryGetValueDefault("",(designTypeTermSourceREFLabel,i)))
                (matrix.TryGetValueDefault("",(designTypeTermAccessionNumberLabel,i)))
                comments
        )

    let toSparseTable (designs: OntologyAnnotation list) =
        let matrix = SparseTable.Create (keys = labels,length=designs.Length + 1)
        let mutable commentKeys = []
        designs
        |> List.iteri (fun i d ->
            let i = i + 1
            let name,source,accession = OntologyAnnotation.toString d
            do matrix.Matrix.Add ((designTypeLabel,i),                      name)
            do matrix.Matrix.Add ((designTypeTermAccessionNumberLabel,i),   accession)
            do matrix.Matrix.Add ((designTypeTermSourceREFLabel,i),         source)

            match d.Comments with 
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
    
    let toRows (prefix : string option) (designs : OntologyAnnotation list) =
        designs
        |> toSparseTable
        |> fun m -> 
            match prefix with 
            | Some prefix -> SparseTable.ToRows(m,prefix)
            | None -> SparseTable.ToRows(m)