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
        let matrix = SparseTable.Create (keys = labels,length=designs.Length)
        let mutable commentKeys = []
        designs
        |> List.iteri (fun i d ->
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


    let readDesigns (prefix : string option) lineNumber (en:IEnumerator<Row>) =
        let prefix = match prefix with | Some p ->  p + " " | None -> ""
        let rec loop (matrix : SparseTable) remarks lineNumber = 

            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v)
                match Seq.tryItem 0 row |> Option.map snd, Seq.trySkip 1 row with

                | Comment k, Some v -> 
                    loop (SparseTable.AddComment k v matrix) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop matrix (Remark.make lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some v when List.exists (fun label -> k = prefix + label) labels -> 
                    let label = List.find (fun label -> k = prefix + label) labels
                    loop (SparseTable.AddRow label v matrix) remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,fromSparseTable matrix
                | _ -> None, lineNumber,remarks,fromSparseTable matrix
            else
                None,lineNumber,remarks,fromSparseTable matrix
        loop (SparseTable.Create()) [] lineNumber

    
    
    let writeDesigns (prefix : string option) (designs : OntologyAnnotation list) =
        designs
        |> toSparseTable
        |> fun m -> 
            match prefix with 
            | Some prefix -> SparseTable.ToRows(m,prefix)
            | None -> SparseTable.ToRows(m)