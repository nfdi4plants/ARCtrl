namespace ARCtrl.Spreadsheet

open ARCtrl
open Comment
open Remark
open System.Collections.Generic
open ARCtrl.Helper
open ARCtrl.Process
open ARCtrl.Process.Conversion

module Workflow = 

    let identifierLabel = "Identifier"
    let titleLabel = "Title"
    let descriptionLabel = "Description"
    let workflowTypeLabel = "Type"
    let typeTermAccessionNumberLabel = "Type Term Accession Number"
    let typeTermSourceREFLabel = "Type Term Source REF"
    let subWorkflowIdentifiersLabel = "Sub Workflow Identifiers"
    let uriLabel = "URI"
    let versionLabel = "Version"
    let parametersNameLabel = "Parameters Name"
    let parametersTermAccessionNumberLabel = "Parameters Term Accession Number"
    let parametersTermSourceREFLabel = "Parameters Term Source REF"
    let componentsNameLabel = "Components Name"
    let componentsTypeLabel = "Components Type"
    let componentsTypeTermAccessionNumberLabel = "Components Type Term Accession Number"
    let componentsTypeTermSourceREFLabel = "Components Type Term Source REF"
    let fileNameLabel = "File Name"

    let [<Literal>] workflowLabel = "WORKFLOW"
    let [<Literal>] contactsLabel = "WORKFLOW CONTACTS"

    let [<Literal>] workflowLabelPrefix = "Workflow"
    let [<Literal>] contactsLabelPrefix = "Workflow Person"

    let labels = [
        identifierLabel;
        titleLabel;
        descriptionLabel;
        workflowTypeLabel;
        typeTermAccessionNumberLabel;
        typeTermSourceREFLabel;
        subWorkflowIdentifiersLabel;
        uriLabel;
        versionLabel;
        parametersNameLabel;
        parametersTermAccessionNumberLabel;
        parametersTermSourceREFLabel;
        componentsNameLabel;
        componentsTypeLabel;
        componentsTypeTermAccessionNumberLabel;
        componentsTypeTermSourceREFLabel;
        fileNameLabel
    ]

    let fromString identifier title description workflowType workflowTypeTermAccessionNumber workflowTypeTermSourceREF (subworkflowIdentifiers : string option) uri version parametersName parametersTermAccessionNumber parametersTermSourceREF componentsName componentsType componentsTypeTermAccessionNumber componentsTypeTermSourceREF fileName comments : ArcWorkflow =
        let subworkflowIdentifiers = 
            match subworkflowIdentifiers with
            | Some subworkflowIdentifiers -> 
                subworkflowIdentifiers.Split(';') |> Seq.map (fun s -> s.Trim()) |> ResizeArray
            | None -> ResizeArray()
        let workflowType = OntologyAnnotation.create(?name = workflowType,?tan = workflowTypeTermAccessionNumber,?tsr = workflowTypeTermSourceREF) |> Option.fromValueWithDefault (OntologyAnnotation())
        let parameters = ProtocolParameter.fromAggregatedStrings ';' parametersName parametersTermSourceREF parametersTermAccessionNumber |> ResizeArray
        let components = Component.fromAggregatedStrings ';' componentsName componentsType componentsTypeTermSourceREF componentsTypeTermAccessionNumber |> ResizeArray
        let identifier =
            match identifier with
            | Some identifier -> identifier
            | None ->
                match fileName with
                | Some fileName ->
                    match Identifier.Workflow.tryIdentifierFromFileName fileName with
                    | Some identifier -> identifier
                    | _ -> Identifier.createMissingIdentifier()
                | None -> Identifier.createMissingIdentifier()
        ArcWorkflow.make
            identifier
            title
            description
            workflowType
            uri
            version
            subworkflowIdentifiers
            parameters
            components
            None
            (ResizeArray())
            None
            comments

    let fromSparseTable (matrix : SparseTable) =
        
        let i = 0

        let comments = 
            matrix.CommentKeys 
            |> List.map (fun k -> 
                Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

        fromString
            (matrix.TryGetValue(identifierLabel,i))  
            (matrix.TryGetValue(titleLabel,i))
            (matrix.TryGetValue(descriptionLabel,i))
            (matrix.TryGetValue(workflowTypeLabel,i))
            (matrix.TryGetValue(typeTermAccessionNumberLabel,i))
            (matrix.TryGetValue(typeTermSourceREFLabel,i))
            (matrix.TryGetValue(subWorkflowIdentifiersLabel,i))
            (matrix.TryGetValue(uriLabel,i))
            (matrix.TryGetValue(versionLabel,i))
            (matrix.TryGetValueDefault("",(parametersNameLabel,i)))
            (matrix.TryGetValueDefault("",(parametersTermAccessionNumberLabel,i)))
            (matrix.TryGetValueDefault("",(parametersTermSourceREFLabel,i)))
            (matrix.TryGetValueDefault("",(componentsNameLabel,i)))
            (matrix.TryGetValueDefault("",(componentsTypeLabel,i)))
            (matrix.TryGetValueDefault("",(componentsTypeTermAccessionNumberLabel,i)))
            (matrix.TryGetValueDefault("",(componentsTypeTermSourceREFLabel,i)))
            (matrix.TryGetValue(fileNameLabel,i))
            (ResizeArray comments)


    let toSparseTable (workflow: ArcWorkflow) =
        let i = 1
        let matrix = SparseTable.Create (keys = labels,length = 2)
        let mutable commentKeys = []
        let processedIdentifier,processedFileName =
            if workflow.Identifier.StartsWith(Identifier.MISSING_IDENTIFIER) then "","" else 
                workflow.Identifier, Identifier.Workflow.fileNameFromIdentifier workflow.Identifier

        let wt = Option.defaultValue (OntologyAnnotation()) workflow.WorkflowType |> fun tt -> OntologyAnnotation.toStringObject(tt,true)
        let pAgg = workflow.Parameters |> List.ofSeq |> ProtocolParameter.toAggregatedStrings ';' 
        let cAgg = workflow.Components |> List.ofSeq |> Component.toAggregatedStrings ';'
        let subWorkflowsAgg = String.concat ";" workflow.SubWorkflowIdentifiers

        do matrix.Matrix.Add ((identifierLabel,i),          processedIdentifier)
        do matrix.Matrix.Add ((titleLabel,i),               (Option.defaultValue "" workflow.Title))
        do matrix.Matrix.Add ((descriptionLabel,i),         (Option.defaultValue "" workflow.Description))
        do matrix.Matrix.Add ((workflowTypeLabel,i),                        wt.TermName)
        do matrix.Matrix.Add ((typeTermAccessionNumberLabel,i),             wt.TermAccessionNumber)
        do matrix.Matrix.Add ((typeTermSourceREFLabel,i),                   wt.TermSourceREF)
        do matrix.Matrix.Add ((subWorkflowIdentifiersLabel,i),              subWorkflowsAgg)
        do matrix.Matrix.Add ((uriLabel,i),                                 (Option.defaultValue "" workflow.URI))
        do matrix.Matrix.Add ((versionLabel,i),                             (Option.defaultValue "" workflow.Version))
        do matrix.Matrix.Add ((parametersNameLabel,i),                      pAgg.TermNameAgg)
        do matrix.Matrix.Add ((parametersTermAccessionNumberLabel,i),       pAgg.TermAccessionNumberAgg)
        do matrix.Matrix.Add ((parametersTermSourceREFLabel,i),             pAgg.TermSourceREFAgg)
        do matrix.Matrix.Add ((componentsNameLabel,i),                      cAgg.NameAgg)
        do matrix.Matrix.Add ((componentsTypeLabel,i),                      cAgg.TermNameAgg)
        do matrix.Matrix.Add ((componentsTypeTermAccessionNumberLabel,i),   cAgg.TermAccessionNumberAgg)
        do matrix.Matrix.Add ((componentsTypeTermSourceREFLabel,i),         cAgg.TermSourceREFAgg)
        do matrix.Matrix.Add ((fileNameLabel,i),                            processedFileName)

        workflow.Comments
        |> ResizeArray.iter (fun comment -> 
            let n,v = comment |> Comment.toString
            commentKeys <- n :: commentKeys
            matrix.Matrix.Add((n,i),v)
        )    

        {matrix with CommentKeys = commentKeys |> List.distinct |> List.rev}

    let fromRows lineNumber (rows : IEnumerator<SparseRow>) =
        SparseTable.FromRows(rows,labels,lineNumber, prefix = workflowLabelPrefix)
        |> fun (s,ln,rs,sm) -> (s,ln,rs, fromSparseTable sm)
    
    let toRows (workflow: ArcWorkflow) =  
        workflow
        |> toSparseTable
        |> fun st -> SparseTable.ToRows(st, prefix = workflowLabelPrefix)
