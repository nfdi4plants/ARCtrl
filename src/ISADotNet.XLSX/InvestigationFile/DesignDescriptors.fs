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

    let fromString designType typeTermAccessionNumber typeTermSourceREF comments =
        OntologyAnnotation.create 
            None 
            (Option.fromValueWithDefault "" designType |> Option.map AnnotationValue.fromString)
            (Option.fromValueWithDefault "" typeTermAccessionNumber |> Option.map URI.fromString)
            (Option.fromValueWithDefault "" typeTermSourceREF)
            (Option.fromValueWithDefault [] comments)

    let fromSparseMatrix (matrix : SparseMatrix) =
        
        List.init matrix.Length (fun i -> 

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            fromString
                (matrix.TryGetValueDefault("",(designTypeLabel,i)))
                (matrix.TryGetValueDefault("",(designTypeTermAccessionNumberLabel,i)))
                (matrix.TryGetValueDefault("",(designTypeTermSourceREFLabel,i)))
                comments
        )

    let toSparseMatrix (designs: OntologyAnnotation list) =
        let matrix = SparseMatrix.Create (keys = labels,length=designs.Length)
        let mutable commentKeys = []
        designs
        |> List.iteri (fun i d ->
            let name,accession,source = OntologyAnnotation.toString d
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


    let readDesigns (prefix : string) lineNumber (en:IEnumerator<Row>) =
        let rec loop (matrix : SparseMatrix) remarks lineNumber = 

            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v)
                match Seq.tryItem 0 row |> Option.map snd, Seq.trySkip 1 row with

                | Comment k, Some v -> 
                    loop (SparseMatrix.AddComment k v matrix) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop matrix (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some v when List.exists (fun label -> k = prefix + " " + label) labels -> 
                    let label = List.find (fun label -> k = prefix + " " + label) labels
                    loop (SparseMatrix.AddRow label v matrix) remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,fromSparseMatrix matrix
                | _ -> None, lineNumber,remarks,fromSparseMatrix matrix
            else
                None,lineNumber,remarks,fromSparseMatrix matrix
        loop (SparseMatrix.Create()) [] lineNumber

    
    
    let writeDesigns prefix (designs : OntologyAnnotation list) =
        designs
        |> toSparseMatrix
        |> fun m -> SparseMatrix.ToRows(m,prefix)