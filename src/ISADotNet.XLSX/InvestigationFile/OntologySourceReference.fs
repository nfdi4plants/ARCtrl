namespace ISADotNet.XLSX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module OntologySourceReference = 

    let nameLabel = "Term Source Name"
    let fileLabel = "Term Source File"
    let versionLabel = "Term Source Version"
    let descriptionLabel = "Term Source Description"

    
    let labels = [nameLabel;fileLabel;versionLabel;descriptionLabel]

    let fromSparseMatrix (matrix : SparseMatrix) =
        
        List.init matrix.Length (fun i -> 

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            OntologySourceReference.create
                (matrix.TryGetValueDefault("",(descriptionLabel,i)))
                (matrix.TryGetValueDefault("",(fileLabel,i)))
                (matrix.TryGetValueDefault("",(nameLabel,i)))
                (matrix.TryGetValueDefault("",(versionLabel,i)))
                comments
        )

    let toSparseMatrix (ontologySources: OntologySourceReference list) =
        let matrix = SparseMatrix.Create (keys = labels,length=ontologySources.Length)
        let mutable commentKeys = []
        ontologySources
        |> List.iteri (fun i o ->
            do matrix.Matrix.Add ((nameLabel,i),        o.Name)
            do matrix.Matrix.Add ((fileLabel,i),        o.File)
            do matrix.Matrix.Add ((versionLabel,i),     o.Version)
            do matrix.Matrix.Add ((descriptionLabel,i), o.Description)

            o.Comments
            |> List.iter (fun comment -> 
                commentKeys <- comment.Name :: commentKeys
                matrix.Matrix.Add((comment.Name,i),comment.Value)
            )      
        )
        {matrix with CommentKeys = commentKeys |> List.distinct}

    let readTermSources lineNumber (en:IEnumerator<Row>) =
        let rec loop (matrix : SparseMatrix) remarks lineNumber = 

            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v)
                match Seq.tryItem 0 row |> Option.map snd, Seq.trySkip 1 row with

                | Comment k, Some v -> 
                    loop (SparseMatrix.AddComment k v matrix) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop matrix (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some v when List.contains k labels -> 
                    loop (SparseMatrix.AddRow k v matrix) remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,fromSparseMatrix matrix
                | _ -> None, lineNumber,remarks,fromSparseMatrix matrix
            else
                None,lineNumber,remarks,fromSparseMatrix matrix
        loop (SparseMatrix.Create()) [] lineNumber

    
    let writeTermSources (termSources : OntologySourceReference list) =
        termSources
        |> toSparseMatrix
        |> fun m -> SparseMatrix.ToRows(m)