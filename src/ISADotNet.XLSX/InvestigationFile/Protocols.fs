namespace ISADotNet.XLSX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open ISADotNet.API
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
        let protocolType = OntologyAnnotation.fromString protocolType typeTermSourceREF typeTermAccessionNumber
        let parameters = ProtocolParameter.fromAggregatedStrings ';' parametersName parametersTermSourceREF parametersTermAccessionNumber
        let components = Component.fromAggregatedStrings ';' componentsName componentsType componentsTypeTermSourceREF componentsTypeTermAccessionNumber
        
        Protocol.make 
            None 
            (Option.fromValueWithDefault "" name |> Option.map URI.fromString) 
            (Option.fromValueWithDefault OntologyAnnotation.empty protocolType)
            (Option.fromValueWithDefault "" description)
            (Option.fromValueWithDefault "" uri |> Option.map URI.fromString) 
            (Option.fromValueWithDefault "" version)
            (Option.fromValueWithDefault [] parameters)
            (Option.fromValueWithDefault [] components) 
            (Option.fromValueWithDefault [] comments)


    let fromSparseTable (matrix : SparseTable) =
        
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
    
    let toSparseTable (protocols: Protocol list) =
        let matrix = SparseTable.Create (keys = labels,length=protocols.Length)
        let mutable commentKeys = []
        protocols
        |> List.iteri (fun i p ->
            let protocolType,protocolSource,protocolAccession = p.ProtocolType |> Option.defaultValue OntologyAnnotation.empty |> OntologyAnnotation.toString 
            let parameterType,parameterSource,parameterAccession = p.Parameters |> Option.defaultValue [] |> ProtocolParameter.toAggregatedStrings ';' 
            let componentName,componentType,componentSource,componentAccession = p.Components |> Option.defaultValue [] |> Component.toAggregatedStrings ';' 

            do matrix.Matrix.Add ((nameLabel,i),                                (Option.defaultValue "" p.Name))
            do matrix.Matrix.Add ((protocolTypeLabel,i),                        protocolType)
            do matrix.Matrix.Add ((typeTermAccessionNumberLabel,i),             protocolAccession)
            do matrix.Matrix.Add ((typeTermSourceREFLabel,i),                   protocolSource)
            do matrix.Matrix.Add ((descriptionLabel,i),                         (Option.defaultValue "" p.Description))
            do matrix.Matrix.Add ((uriLabel,i),                                 (Option.defaultValue "" p.Uri))
            do matrix.Matrix.Add ((versionLabel,i),                             (Option.defaultValue "" p.Version))
            do matrix.Matrix.Add ((parametersNameLabel,i),                      parameterType)
            do matrix.Matrix.Add ((parametersTermAccessionNumberLabel,i),       parameterAccession)
            do matrix.Matrix.Add ((parametersTermSourceREFLabel,i),             parameterSource)
            do matrix.Matrix.Add ((componentsNameLabel,i),                      componentName)
            do matrix.Matrix.Add ((componentsTypeLabel,i),                      componentType)
            do matrix.Matrix.Add ((componentsTypeTermAccessionNumberLabel,i),   componentAccession)
            do matrix.Matrix.Add ((componentsTypeTermSourceREFLabel,i),         componentSource)

            match p.Comments with 
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

    let readProtocols (prefix : string option) lineNumber (en:IEnumerator<Row>) =
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

    
    let writeProtocols prefix (protocols : Protocol list) =
        protocols
        |> toSparseTable
        |> fun m -> 
            match prefix with 
            | Some prefix -> SparseTable.ToRows(m,prefix)
            | None -> SparseTable.ToRows(m)