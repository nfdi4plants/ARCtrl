namespace ARCtrl.Spreadsheet

open ARCtrl
open Comment
open Remark
open System.Collections.Generic
open ARCtrl.Helper
open ARCtrl.Process.Conversion


module Assays = 

    let [<Literal>] identifierLabel =                         "Identifier"
    let [<Literal>] titleLabel =                              "Title"
    let [<Literal>] descriptionLabel =                        "Description"
    let [<Literal>] measurementTypeLabel =                    "Measurement Type"
    let [<Literal>] measurementTypeTermAccessionNumberLabel = "Measurement Type Term Accession Number"
    let [<Literal>] measurementTypeTermSourceREFLabel =       "Measurement Type Term Source REF"
    let [<Literal>] technologyTypeLabel =                     "Technology Type"
    let [<Literal>] technologyTypeTermAccessionNumberLabel =  "Technology Type Term Accession Number"
    let [<Literal>] technologyTypeTermSourceREFLabel =        "Technology Type Term Source REF"
    let [<Literal>] technologyPlatformLabel =                 "Technology Platform"
    let [<Literal>] fileNameLabel =                           "File Name"

    let labels = 
        [
            identifierLabel; titleLabel; descriptionLabel; measurementTypeLabel;measurementTypeTermAccessionNumberLabel;measurementTypeTermSourceREFLabel;
            technologyTypeLabel;technologyTypeTermAccessionNumberLabel;technologyTypeTermSourceREFLabel;technologyPlatformLabel;fileNameLabel
        ]

    
    let fromString identifier title description measurementType measurementTypeTermSourceREF measurementTypeTermAccessionNumber technologyType technologyTypeTermSourceREF technologyTypeTermAccessionNumber technologyPlatform fileName comments : ArcAssay = 
        let measurementType = OntologyAnnotation.create(?name = measurementType,?tan = measurementTypeTermAccessionNumber,?tsr = measurementTypeTermSourceREF)
        let technologyType = OntologyAnnotation.create(?name = technologyType,?tan = technologyTypeTermAccessionNumber,?tsr = technologyTypeTermSourceREF)
        let identifier =
            match identifier with
            | Some identifier -> identifier
            | None ->
                match fileName with
                | Some fileName ->
                    match Identifier.Assay.tryIdentifierFromFileName fileName with
                    | Some identifier -> identifier
                    | _ -> Identifier.createMissingIdentifier()
                | None -> Identifier.createMissingIdentifier()
        ArcAssay.make
            identifier
            title
            description
            (Option.fromValueWithDefault (OntologyAnnotation()) measurementType)
            (Option.fromValueWithDefault (OntologyAnnotation()) technologyType) 
            (technologyPlatform |> Option.map JsonTypes.decomposeTechnologyPlatform)
            (ResizeArray())             
            None
            (ResizeArray())      
            (comments)
        
    let fromSparseTable (matrix : SparseTable) : ArcAssay list=
        if matrix.ColumnCount = 0 && matrix.CommentKeys.Length <> 0 then
            let comments = SparseTable.GetEmptyComments matrix
            ArcAssay.create(Identifier.createMissingIdentifier(),comments = comments)
            |> List.singleton
        else
            List.init matrix.ColumnCount (fun i -> 

                let comments = 
                    matrix.CommentKeys 
                    |> List.map (fun k -> 
                        Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))
                    |> ResizeArray

                fromString
                    (matrix.TryGetValue(identifierLabel,i))
                    (matrix.TryGetValue(titleLabel,i))
                    (matrix.TryGetValue(descriptionLabel,i))
                    (matrix.TryGetValue(measurementTypeLabel,i))            
                    (matrix.TryGetValue((measurementTypeTermSourceREFLabel,i)))
                    (matrix.TryGetValue((measurementTypeTermAccessionNumberLabel,i)))
                    (matrix.TryGetValue(technologyTypeLabel,i))             
                    (matrix.TryGetValue((technologyTypeTermSourceREFLabel,i)))   
                    (matrix.TryGetValue((technologyTypeTermAccessionNumberLabel,i))) 
                    (matrix.TryGetValue(technologyPlatformLabel,i))     
                    (matrix.TryGetValue(fileNameLabel,i))                    
                    comments
            )

    let toSparseTable (assays: ArcAssay list) =
        let matrix = SparseTable.Create (keys = labels,length=assays.Length + 1)
        let mutable commentKeys = []
        assays
        |> List.iteri (fun i a ->
            let processedFileName =
                if a.Identifier.StartsWith(Identifier.MISSING_IDENTIFIER) then Identifier.removeMissingIdentifier(a.Identifier) else Identifier.Assay.fileNameFromIdentifier(a.Identifier)
            let i = i + 1
            let mt = Option.defaultValue (OntologyAnnotation()) a.MeasurementType |> fun mt -> OntologyAnnotation.toStringObject(mt,true)
            let tt = Option.defaultValue (OntologyAnnotation()) a.TechnologyType |> fun tt -> OntologyAnnotation.toStringObject(tt,true)
            do matrix.Matrix.Add ((identifierLabel,i),                            (Identifier.removeMissingIdentifier a.Identifier))
            do matrix.Matrix.Add ((titleLabel,i),                                 (Option.defaultValue "" a.Title))
            do matrix.Matrix.Add ((descriptionLabel,i),                           (Option.defaultValue "" a.Description))
            do matrix.Matrix.Add ((measurementTypeLabel,i),                       mt.TermName)
            do matrix.Matrix.Add ((measurementTypeTermAccessionNumberLabel,i),    mt.TermAccessionNumber)
            do matrix.Matrix.Add ((measurementTypeTermSourceREFLabel,i),          mt.TermSourceREF)
            do matrix.Matrix.Add ((technologyTypeLabel,i),                        tt.TermName)
            do matrix.Matrix.Add ((technologyTypeTermAccessionNumberLabel,i),     tt.TermAccessionNumber)
            do matrix.Matrix.Add ((technologyTypeTermSourceREFLabel,i),           tt.TermSourceREF)
            do matrix.Matrix.Add ((technologyPlatformLabel,i),                    (Option.defaultValue "" (a.TechnologyPlatform |> Option.map JsonTypes.composeTechnologyPlatform)))
            do matrix.Matrix.Add ((fileNameLabel,i),                              processedFileName)

            a.Comments
            |> ResizeArray.iter (fun comment -> 
                let n,v = comment |> Comment.toString
                commentKeys <- n :: commentKeys
                matrix.Matrix.Add((n,i),v)
            )   
        )
        {matrix with CommentKeys = commentKeys |> List.distinct |> List.rev}

    let fromRows (prefix : string option) lineNumber (rows : IEnumerator<SparseRow>) =
        SparseTable.FromRows(rows,labels,lineNumber,?prefix = prefix)
        |> fun (s,ln,rs,sm) -> (s,ln,rs, fromSparseTable sm)
 
    let toRows prefix (assays : ArcAssay list) =
        assays
        |> toSparseTable
        |> fun m -> SparseTable.ToRows(m,?prefix = prefix)
