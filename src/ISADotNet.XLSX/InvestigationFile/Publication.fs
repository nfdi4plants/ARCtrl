namespace ISADotNet.XLSX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module Publications = 

    let pubMedIDLabel =                     "PubMed ID"
    let doiLabel =                          "DOI"
    let authorListLabel =                   "Author List"
    let titleLabel =                        "Title"
    let statusLabel =                       "Status"
    let statusTermAccessionNumberLabel =    "Status Term Accession Number"
    let statusTermSourceREFLabel =          "Status Term Source REF"

    let labels = [pubMedIDLabel;doiLabel;authorListLabel;titleLabel;statusLabel;statusTermAccessionNumberLabel;statusTermSourceREFLabel]

    let fromString pubMedID doir author title status statursTermAccessionNumber statusTermSourceREF comments =
        let status = OntologyAnnotation.fromString status statursTermAccessionNumber statusTermSourceREF
        Publication.create null pubMedID doir author title status comments

    let fromSparseMatrix (matrix : SparseMatrix) =
        
        List.init matrix.Length (fun i -> 

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            fromString
                (matrix.TryGetValueDefault("",(pubMedIDLabel,i)))            
                (matrix.TryGetValueDefault("",(doiLabel,i)))             
                (matrix.TryGetValueDefault("",(authorListLabel,i)))         
                (matrix.TryGetValueDefault("",(titleLabel,i)))                 
                (matrix.TryGetValueDefault("",(statusLabel,i)))                
                (matrix.TryGetValueDefault("",(statusTermAccessionNumberLabel,i)))
                (matrix.TryGetValueDefault("",(statusTermSourceREFLabel,i)))    
                comments
        )

    let toSparseMatrix (publications: Publication list) =
        let matrix = SparseMatrix.Create (keys = labels,length=publications.Length)
        let mutable commentKeys = []
        publications
        |> List.iteri (fun i p ->
            let status,accession,source = OntologyAnnotation.toString p.Status
            do matrix.Matrix.Add ((pubMedIDLabel,i),                    p.PubMedID)
            do matrix.Matrix.Add ((doiLabel,i),                         p.DOI)
            do matrix.Matrix.Add ((authorListLabel,i),                  p.Authors)
            do matrix.Matrix.Add ((titleLabel,i),                       p.Title)
            do matrix.Matrix.Add ((statusLabel,i),                      status)
            do matrix.Matrix.Add ((statusTermAccessionNumberLabel,i),   accession)
            do matrix.Matrix.Add ((statusTermSourceREFLabel,i),         source)

            p.Comments
            |> List.iter (fun comment -> 
                commentKeys <- comment.Name :: commentKeys
                matrix.Matrix.Add((comment.Name,i),comment.Value)
            )      
        )
        {matrix with CommentKeys = commentKeys |> List.distinct}

    let readPublications (prefix : string) lineNumber (en:IEnumerator<Row>) =
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

    
    let writePublications prefix (publications : Publication list) =
        publications
        |> toSparseMatrix
        |> fun m -> SparseMatrix.ToRows(m,prefix)