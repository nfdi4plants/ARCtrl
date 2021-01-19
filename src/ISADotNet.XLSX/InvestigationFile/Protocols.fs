namespace ISADotNet.XLSX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module Protocols = 

    let nameLabel = "Name"
    let protocolTypeLabel = "Type"
    let typeTermAccessionNumberLabel = "Type Term Accession Number"
    let typeTermSourceREFLabel = "Type Term Source REF"
    let descriptionLabel = "Description"
    let uriLabel = "URI"
    let versionLabel = "Version"
    let parametersNameLabel = "Parameters Name"
    let parametersTermAccessionNumberLabel = "Parameters Term Accession Number"
    let parametersTermSourceREFLabel = "Parameters Term Source REF"
    let componentsNameLabel = "Components Name"
    let componentsTypeLabel = "Components Type"
    let componentsTypeTermAccessionNumberLabel = "Components Type Term Accession Number"
    let componentsTypeTermSourceREFLabel = "Components Type Term Source REF"

    let labels = 
        [
            nameLabel;protocolTypeLabel;typeTermAccessionNumberLabel;typeTermSourceREFLabel;descriptionLabel;uriLabel;versionLabel;
            parametersNameLabel;parametersTermAccessionNumberLabel;parametersTermSourceREFLabel;
            componentsNameLabel;componentsTypeLabel;componentsTypeTermAccessionNumberLabel;componentsTypeTermSourceREFLabel
        ]

    let fromString name protocolType typeTermAccessionNumber typeTermSourceREF description uri version parametersName parametersTermAccessionNumber parametersTermSourceREF componentsName componentsType componentsTypeTermAccessionNumber componentsTypeTermSourceREF comments =
        let protocolType = OntologyAnnotation.fromString protocolType typeTermAccessionNumber typeTermSourceREF
        let parameters = ProtocolParameter.fromAggregatedStrings ';' parametersName parametersTermAccessionNumber parametersTermSourceREF
        let components = Component.fromAggregatedStrings ';' componentsName componentsType componentsTypeTermAccessionNumber componentsTypeTermSourceREF
        
        Protocol.create null name protocolType description uri version parameters components comments

    let fromSparseMatrix (matrix : SparseMatrix) =
        
        List.init matrix.Length (fun i -> 

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            fromString
                (matrix.TryGetValueDefault("",(nameLabel,i)))
                (matrix.TryGetValueDefault("",(protocolTypeLabel,i)))
                (matrix.TryGetValueDefault("",(typeTermAccessionNumberLabel,i)))
                (matrix.TryGetValueDefault("",(typeTermSourceREFLabel,i)))
                (matrix.TryGetValueDefault("",(descriptionLabel,i)))
                (matrix.TryGetValueDefault("",(uriLabel,i)))
                (matrix.TryGetValueDefault("",(versionLabel,i)))
                (matrix.TryGetValueDefault("",(parametersNameLabel,i)))
                (matrix.TryGetValueDefault("",(parametersTermAccessionNumberLabel,i)))
                (matrix.TryGetValueDefault("",(parametersTermSourceREFLabel,i)))
                (matrix.TryGetValueDefault("",(componentsNameLabel,i)))
                (matrix.TryGetValueDefault("",(componentsTypeLabel,i)))
                (matrix.TryGetValueDefault("",(componentsTypeTermAccessionNumberLabel,i)))
                (matrix.TryGetValueDefault("",(componentsTypeTermSourceREFLabel,i)))
                comments
        )
    
    let toSparseMatrix (protocols: Protocol list) =
        let matrix = SparseMatrix.Create (keys = labels,length=protocols.Length)
        let mutable commentKeys = []
        protocols
        |> List.iteri (fun i p ->
            let protocolType,protocolAccession,protocolSource = OntologyAnnotation.toString p.ProtocolType
            let parameterType,parameterAccession,parameterSource = ProtocolParameter.toAggregatedStrings ';' p.Parameters
            let componentName,componentType,componentAccession,componentSource = Component.toAggregatedStrings ';' p.Components

            do matrix.Matrix.Add ((nameLabel,i),                                p.Name)
            do matrix.Matrix.Add ((protocolTypeLabel,i),                        protocolType)
            do matrix.Matrix.Add ((typeTermAccessionNumberLabel,i),             protocolAccession)
            do matrix.Matrix.Add ((typeTermSourceREFLabel,i),                   protocolSource)
            do matrix.Matrix.Add ((descriptionLabel,i),                         p.Description)
            do matrix.Matrix.Add ((uriLabel,i),                                 p.Uri)
            do matrix.Matrix.Add ((versionLabel,i),                             p.Version)
            do matrix.Matrix.Add ((parametersNameLabel,i),                      parameterType)
            do matrix.Matrix.Add ((parametersTermAccessionNumberLabel,i),       parameterAccession)
            do matrix.Matrix.Add ((parametersTermSourceREFLabel,i),             parameterSource)
            do matrix.Matrix.Add ((componentsNameLabel,i),                      componentName)
            do matrix.Matrix.Add ((componentsTypeLabel,i),                      componentType)
            do matrix.Matrix.Add ((componentsTypeTermAccessionNumberLabel,i),   componentAccession)
            do matrix.Matrix.Add ((componentsTypeTermSourceREFLabel,i),         componentSource)

            p.Comments
            |> List.iter (fun comment -> 
                commentKeys <- comment.Name :: commentKeys
                matrix.Matrix.Add((comment.Name,i),comment.Value)
            )      
        )
        {matrix with CommentKeys = commentKeys |> List.distinct |> List.rev} 

    let readProtocols (prefix : string) lineNumber (en:IEnumerator<Row>) =
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

    
    let writeProtocols prefix (protocols : Protocol list) =
        protocols
        |> toSparseMatrix
        |> fun m -> SparseMatrix.ToRows(m,prefix)