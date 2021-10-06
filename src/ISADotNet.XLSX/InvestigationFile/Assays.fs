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

    
    let fromString measurementType measurementTypeTermSourceREF measurementTypeTermAccessionNumber technologyType technologyTypeTermSourceREF technologyTypeTermAccessionNumber technologyPlatform fileName comments =
        let measurementType = OntologyAnnotation.fromString measurementType measurementTypeTermSourceREF measurementTypeTermAccessionNumber
        let technologyType = OntologyAnnotation.fromString technologyType technologyTypeTermSourceREF technologyTypeTermAccessionNumber
        Assay.make 
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
        
    let fromSparseTable (matrix : SparseTable) =
        
        List.init matrix.Length (fun i -> 

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            fromString
                (matrix.TryGetValueDefault("",(measurementTypeLabel,i)))             
                (matrix.TryGetValueDefault("",(measurementTypeTermSourceREFLabel,i)))
                (matrix.TryGetValueDefault("",(measurementTypeTermAccessionNumberLabel,i)))
                (matrix.TryGetValueDefault("",(technologyTypeLabel,i)))               
                (matrix.TryGetValueDefault("",(technologyTypeTermSourceREFLabel,i)))   
                (matrix.TryGetValueDefault("",(technologyTypeTermAccessionNumberLabel,i))) 
                (matrix.TryGetValueDefault("",(technologyPlatformLabel,i)))     
                (matrix.TryGetValueDefault("",(fileNameLabel,i)))                    
                comments
        )

    let toSparseTable (assays: Assay list) =
        let matrix = SparseTable.Create (keys = labels,length=assays.Length)
        let mutable commentKeys = []
        assays
        |> List.iteri (fun i a ->
            let measurementType,measurementSource,measurementAccession = Option.defaultValue OntologyAnnotation.empty a.MeasurementType |> OntologyAnnotation.toString 
            let technologyType,technologySource,technologyAccession = Option.defaultValue OntologyAnnotation.empty  a.TechnologyType |> OntologyAnnotation.toString
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

    let fromRows (prefix : string option) lineNumber (rows) =
        match prefix with
        | Some p -> SparseTable.FromRows(rows,labels,lineNumber,p)
        | None -> SparseTable.FromRows(rows,labels,lineNumber)
        |> fun (s,ln,rs,sm) -> (s,ln,rs, fromSparseTable sm)
 
    let toRows prefix (assays : Assay list) =
        assays
        |> toSparseTable
        |> fun m -> 
            match prefix with 
            | Some prefix -> SparseTable.ToRows(m,prefix)
            | None -> SparseTable.ToRows(m)