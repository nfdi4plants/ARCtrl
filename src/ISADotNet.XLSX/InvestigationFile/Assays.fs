namespace ISADotNet.XLSX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open ISADotNet.API
open Comment
open Remark
open System.Collections.Generic

module Assays = 

    let measurementTypeLabel =                    "Measurement Type"
    let measurementTypeTermAccessionNumberLabel = "Measurement Type Term Accession Number"
    let measurementTypeTermSourceREFLabel =       "Measurement Type Term Source REF"
    let technologyTypeLabel =                     "Technology Type"
    let technologyTypeTermAccessionNumberLabel =  "Technology Type Term Accession Number"
    let technologyTypeTermSourceREFLabel =        "Technology Type Term Source REF"
    let technologyPlatformLabel =                 "Technology Platform"
    let fileNameLabel =                           "File Name"

    let labels = 
        [
        measurementTypeLabel;measurementTypeTermAccessionNumberLabel;measurementTypeTermSourceREFLabel;
        technologyTypeLabel;technologyTypeTermAccessionNumberLabel;technologyTypeTermSourceREFLabel;technologyPlatformLabel;fileNameLabel
        ]

    
    let fromString measurementType measurementTypeTermAccessionNumber measurementTypeTermSourceREF technologyType technologyTypeTermAccessionNumber technologyTypeTermSourceREF technologyPlatform fileName comments =
        let measurementType = OntologyAnnotation.fromString measurementType measurementTypeTermAccessionNumber measurementTypeTermSourceREF
        let technologyType = OntologyAnnotation.fromString technologyType technologyTypeTermAccessionNumber technologyTypeTermSourceREF
        Assay.create 
            None 
            (Option.fromValueWithDefault "" fileName)
            (Option.fromValueWithDefault OntologyAnnotation.empty measurementType)
            (Option.fromValueWithDefault OntologyAnnotation.empty technologyType) 
            (Option.fromValueWithDefault "" technologyPlatform)
            None
            None
            None 
            None 
            None 
            (Option.fromValueWithDefault [] comments)
        
    let fromSparseMatrix (matrix : SparseMatrix) =
        
        List.init matrix.Length (fun i -> 

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            fromString
                (matrix.TryGetValueDefault("",(measurementTypeLabel,i)))             
                (matrix.TryGetValueDefault("",(measurementTypeTermAccessionNumberLabel,i)))
                (matrix.TryGetValueDefault("",(measurementTypeTermSourceREFLabel,i)))
                (matrix.TryGetValueDefault("",(technologyTypeLabel,i)))               
                (matrix.TryGetValueDefault("",(technologyTypeTermAccessionNumberLabel,i))) 
                (matrix.TryGetValueDefault("",(technologyTypeTermSourceREFLabel,i)))   
                (matrix.TryGetValueDefault("",(technologyPlatformLabel,i)))     
                (matrix.TryGetValueDefault("",(fileNameLabel,i)))                    
                comments
        )

    let toSparseMatrix (assays: Assay list) =
        let matrix = SparseMatrix.Create (keys = labels,length=assays.Length)
        let mutable commentKeys = []
        assays
        |> List.iteri (fun i a ->
            let measurementType,measurementAccession,measurementSource = Option.defaultValue OntologyAnnotation.empty a.MeasurementType |> OntologyAnnotation.toString 
            let technologyType,technologyAccession,technologySource = Option.defaultValue OntologyAnnotation.empty  a.TechnologyType |> OntologyAnnotation.toString
            do matrix.Matrix.Add ((measurementTypeLabel,i),                       measurementType)
            do matrix.Matrix.Add ((measurementTypeTermAccessionNumberLabel,i),    measurementAccession)
            do matrix.Matrix.Add ((measurementTypeTermSourceREFLabel,i),          measurementSource)
            do matrix.Matrix.Add ((technologyTypeLabel,i),                        technologyType)
            do matrix.Matrix.Add ((technologyTypeTermAccessionNumberLabel,i),     technologyAccession)
            do matrix.Matrix.Add ((technologyTypeTermSourceREFLabel,i),           technologySource)
            do matrix.Matrix.Add ((technologyPlatformLabel,i),                    (Option.defaultValue "" a.TechnologyPlatform))
            do matrix.Matrix.Add ((fileNameLabel,i),                              (Option.defaultValue "" a.FileName))

            match a.Comments with 
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

    let readAssays (prefix : string option) lineNumber (en:IEnumerator<Row>) =
        let prefix = match prefix with | Some p ->  p + " " | None -> ""
        let rec loop (matrix : SparseMatrix) remarks lineNumber = 

            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v)
                match Seq.tryItem 0 row |> Option.map snd, Seq.trySkip 1 row with

                | Comment k, Some v -> 
                    loop (SparseMatrix.AddComment k v matrix) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop matrix (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some v when List.exists (fun label -> k = prefix + label) labels -> 
                    let label = List.find (fun label -> k = prefix + label) labels
                    loop (SparseMatrix.AddRow label v matrix) remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,fromSparseMatrix matrix
                | _ -> None, lineNumber,remarks,fromSparseMatrix matrix
            else
                None,lineNumber,remarks,fromSparseMatrix matrix
        loop (SparseMatrix.Create()) [] lineNumber

    
    let writeAssays prefix (assays : Assay list) =
        assays
        |> toSparseMatrix
        |> fun m -> 
            match prefix with 
            | Some prefix -> SparseMatrix.ToRows(m,prefix)
            | None -> SparseMatrix.ToRows(m)