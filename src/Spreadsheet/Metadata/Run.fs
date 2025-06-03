namespace ARCtrl.Spreadsheet

open ARCtrl
open Comment
open Remark
open System.Collections.Generic
open ARCtrl.Helper
open ARCtrl.Process.Conversion


module Run = 

    let [<Literal>] identifierLabel =                         "Identifier"
    let [<Literal>] titleLabel =                              "Title"
    let [<Literal>] descriptionLabel =                        "Description"
    let [<Literal>] workflowIdentifiersLabel =                "Workflow Identifiers"
    let [<Literal>] measurementTypeLabel =                    "Measurement Type"
    let [<Literal>] measurementTypeTermAccessionNumberLabel = "Measurement Type Term Accession Number"
    let [<Literal>] measurementTypeTermSourceREFLabel =       "Measurement Type Term Source REF"
    let [<Literal>] technologyTypeLabel =                     "Technology Type"
    let [<Literal>] technologyTypeTermAccessionNumberLabel =  "Technology Type Term Accession Number"
    let [<Literal>] technologyTypeTermSourceREFLabel =        "Technology Type Term Source REF"
    let [<Literal>] technologyPlatformLabel =                 "Technology Platform"
    let [<Literal>] fileNameLabel =                           "File Name"

    let [<Literal>] runLabel = "RUN"
    let [<Literal>] performersLabel = "RUN PERFORMERS"

    let [<Literal>] runLabelPrefix = "Run"
    let [<Literal>] performersLabelPrefix = "Run Person"

    let labels = 
        [
            identifierLabel; titleLabel; descriptionLabel; workflowIdentifiersLabel; measurementTypeLabel;measurementTypeTermAccessionNumberLabel;measurementTypeTermSourceREFLabel;
            technologyTypeLabel;technologyTypeTermAccessionNumberLabel;technologyTypeTermSourceREFLabel;technologyPlatformLabel;fileNameLabel
        ]

    
    let fromString identifier title description (workflowIdentifiers : string option) measurementType measurementTypeTermSourceREF measurementTypeTermAccessionNumber technologyType technologyTypeTermSourceREF technologyTypeTermAccessionNumber technologyPlatform fileName comments : ArcRun =
        let workflowIdentifiers =
            match workflowIdentifiers with
            | Some wi -> wi.Split(';') |> ResizeArray
            | None -> ResizeArray()
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
        ArcRun.make
            identifier
            title
            description
            (Option.fromValueWithDefault (OntologyAnnotation()) measurementType)
            (Option.fromValueWithDefault (OntologyAnnotation()) technologyType) 
            (technologyPlatform |> Option.map JsonTypes.decomposeTechnologyPlatform)
            workflowIdentifiers
            (ResizeArray())             
            None
            (ResizeArray())
            None
            (comments)
        
    let fromSparseTable (matrix : SparseTable) : ArcRun =
        let i = 0

        let comments = 
            matrix.CommentKeys 
            |> List.map (fun k -> 
                Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))
            |> ResizeArray

        fromString
            (matrix.TryGetValue(identifierLabel,i))
            (matrix.TryGetValue(titleLabel,i))
            (matrix.TryGetValue(descriptionLabel,i))
            (matrix.TryGetValue(workflowIdentifiersLabel,i))
            (matrix.TryGetValue(measurementTypeLabel,i))            
            (matrix.TryGetValue((measurementTypeTermSourceREFLabel,i)))
            (matrix.TryGetValue((measurementTypeTermAccessionNumberLabel,i)))
            (matrix.TryGetValue(technologyTypeLabel,i))             
            (matrix.TryGetValue((technologyTypeTermSourceREFLabel,i)))   
            (matrix.TryGetValue((technologyTypeTermAccessionNumberLabel,i))) 
            (matrix.TryGetValue(technologyPlatformLabel,i))     
            (matrix.TryGetValue(fileNameLabel,i))                    
            comments
            

    let toSparseTable (run: ArcRun) =
        let i = 1
        let matrix = SparseTable.Create (keys = labels,length = 2)
        let mutable commentKeys = []
        let processedIdentifier,processedFileName =
            if run.Identifier.StartsWith(Identifier.MISSING_IDENTIFIER) then "","" else 
                run.Identifier, Identifier.Run.fileNameFromIdentifier run.Identifier
        let workflowIdentifiers = String.concat ";" run.WorkflowIdentifiers
        let mt = Option.defaultValue (OntologyAnnotation()) run.MeasurementType |> fun mt -> OntologyAnnotation.toStringObject(mt,true)
        let tt = Option.defaultValue (OntologyAnnotation()) run.TechnologyType |> fun tt -> OntologyAnnotation.toStringObject(tt,true)
        do matrix.Matrix.Add ((identifierLabel,i),                            processedIdentifier)
        do matrix.Matrix.Add ((titleLabel,i),                                 (Option.defaultValue "" run.Title))
        do matrix.Matrix.Add ((descriptionLabel,i),                           (Option.defaultValue "" run.Description))
        do matrix.Matrix.Add ((workflowIdentifiersLabel,i),                   workflowIdentifiers)
        do matrix.Matrix.Add ((measurementTypeLabel,i),                       mt.TermName)
        do matrix.Matrix.Add ((measurementTypeTermAccessionNumberLabel,i),    mt.TermAccessionNumber)
        do matrix.Matrix.Add ((measurementTypeTermSourceREFLabel,i),          mt.TermSourceREF)
        do matrix.Matrix.Add ((technologyTypeLabel,i),                        tt.TermName)
        do matrix.Matrix.Add ((technologyTypeTermAccessionNumberLabel,i),     tt.TermAccessionNumber)
        do matrix.Matrix.Add ((technologyTypeTermSourceREFLabel,i),           tt.TermSourceREF)
        do matrix.Matrix.Add ((technologyPlatformLabel,i),                    (Option.defaultValue "" (run.TechnologyPlatform |> Option.map JsonTypes.composeTechnologyPlatform)))
        do matrix.Matrix.Add ((fileNameLabel,i),                              processedFileName)

        run.Comments
        |> ResizeArray.iter (fun comment -> 
            let n,v = comment |> Comment.toString
            commentKeys <- n :: commentKeys
            matrix.Matrix.Add((n,i),v)
        )   
        
        {matrix with CommentKeys = commentKeys |> List.distinct |> List.rev}

    let fromRows lineNumber (rows : IEnumerator<SparseRow>) =
        SparseTable.FromRows(rows,labels,lineNumber,prefix = runLabelPrefix)
        |> fun (s,ln,rs,sm) -> (s,ln,rs, fromSparseTable sm)
 
    let toRows (run : ArcRun) =
        run
        |> toSparseTable
        |> fun m -> SparseTable.ToRows(m,prefix = runLabelPrefix)
