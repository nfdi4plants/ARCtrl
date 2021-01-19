namespace ISADotNet.XLSX


open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module Factors = 
    
    let nameLabel = "Name"
    let factorTypeLabel = "Type"
    let typeTermAccessionNumberLabel = "Type Term Accession Number"
    let typeTermSourceREFLabel = "Type Term Source REF"

    let labels = [nameLabel;factorTypeLabel;typeTermAccessionNumberLabel;typeTermSourceREFLabel]

    let fromString name designType typeTermAccessionNumber typeTermSourceREF comments =
        let factorType = OntologyAnnotation.fromString designType typeTermAccessionNumber typeTermSourceREF
        Factor.create null name factorType comments

    let fromSparseMatrix (matrix : SparseMatrix) =
        
        List.init matrix.Length (fun i -> 

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            fromString
                (matrix.TryGetValueDefault("",(nameLabel,i)))
                (matrix.TryGetValueDefault("",(factorTypeLabel,i)))
                (matrix.TryGetValueDefault("",(typeTermAccessionNumberLabel,i)))
                (matrix.TryGetValueDefault("",(typeTermSourceREFLabel,i)))
                comments
        )

    let toSparseMatrix (factors: Factor list) =
        let matrix = SparseMatrix.Create (keys = labels,length=factors.Length)
        let mutable commentKeys = []
        factors
        |> List.iteri (fun i f ->
            let factorType,accession,source = OntologyAnnotation.toString f.FactorType
            do matrix.Matrix.Add ((nameLabel,i),                    f.Name)
            do matrix.Matrix.Add ((factorTypeLabel,i),              factorType)
            do matrix.Matrix.Add ((typeTermAccessionNumberLabel,i), accession)
            do matrix.Matrix.Add ((typeTermSourceREFLabel,i),       source)

            f.Comments
            |> List.iter (fun comment -> 
                commentKeys <- comment.Name :: commentKeys
                matrix.Matrix.Add((comment.Name,i),comment.Value)
            )      
        )
        {matrix with CommentKeys = commentKeys |> List.distinct}


    let readFactors (prefix : string) lineNumber (en:IEnumerator<Row>) =
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

       
    let writeFactors prefix (factors : Factor list) =
        factors
        |> toSparseMatrix
        |> fun m -> SparseMatrix.ToRows(m,prefix)
        