namespace ISADotNet.XLSX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module Study = 

    let identifierLabel = "Study Identifier"
    let titleLabel = "Study Title"
    let descriptionLabel = "Study Description"
    let submissionDateLabel = "Study Submission Date"
    let publicReleaseDateLabel = "Study Public Release Date"
    let fileNameLabel = "Study File Name"

    let designDescriptorsLabelPrefix = "Study Design"
    let publicationsLabelPrefix = "Study Publication"
    let factorsLabelPrefix = "Study Factor"
    let assaysLabelPrefix = "Study Assay"
    let protocolsLabelPrefix = "Study Protocol"
    let contactsLabelPrefix = "Study Person"

    let designDescriptorsLabel = "STUDY DESIGN DESCRIPTORS"
    let publicationsLabel = "STUDY PUBLICATIONS"
    let factorsLabel = "STUDY FACTORS"
    let assaysLabel = "STUDY ASSAYS"
    let protocolsLabel = "STUDY PROTOCOLS"
    let contactsLabel = "STUDY CONTACTS"

    type StudyInfo =
        {
        Identifier : string
        Title : string
        Description : string
        SubmissionDate : string
        PublicReleaseDate : string
        FileName : string
        Comments : Comment list
        }

        static member create identifier title description submissionDate publicReleaseDate fileName comments =
            {
            Identifier = identifier
            Title = title
            Description = description
            SubmissionDate = submissionDate
            PublicReleaseDate = publicReleaseDate
            FileName = fileName
            Comments = comments
            }
  
        static member Labels = [identifierLabel;titleLabel;descriptionLabel;submissionDateLabel;publicReleaseDateLabel;fileNameLabel]
    
        static member FromSparseMatrix (matrix : SparseMatrix) =
        
            let i = 0

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            StudyInfo.create
                (matrix.TryGetValueDefault("",(identifierLabel,i)))  
                (matrix.TryGetValueDefault("",(titleLabel,i)))  
                (matrix.TryGetValueDefault("",(descriptionLabel,i)))  
                (matrix.TryGetValueDefault("",(submissionDateLabel,i)))  
                (matrix.TryGetValueDefault("",(publicReleaseDateLabel,i)))  
                (matrix.TryGetValueDefault("",(fileNameLabel,i)))                    
                comments


        static member ToSparseMatrix (study: Study) =
            let i = 0
            let matrix = SparseMatrix.Create (keys = StudyInfo.Labels,length = 1)
            let mutable commentKeys = []

            do matrix.Matrix.Add ((identifierLabel,i),          study.Identifier)
            do matrix.Matrix.Add ((titleLabel,i),               study.Title)
            do matrix.Matrix.Add ((descriptionLabel,i),         study.Description)
            do matrix.Matrix.Add ((submissionDateLabel,i),      study.SubmissionDate)
            do matrix.Matrix.Add ((publicReleaseDateLabel,i),   study.PublicReleaseDate)
            do matrix.Matrix.Add ((fileNameLabel,i),            study.FileName)

            study.Comments
            |> List.iter (fun comment -> 
                commentKeys <- comment.Name :: commentKeys
                matrix.Matrix.Add((comment.Name,i),comment.Value)
                )      

            {matrix with CommentKeys = commentKeys |> List.distinct |> List.rev}

      
        static member ReadStudyInfo lineNumber (en:IEnumerator<Row>) =
            let rec loop (matrix : SparseMatrix) remarks lineNumber = 

                if en.MoveNext() then  
                    let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v)
                    match Seq.tryItem 0 row |> Option.map snd, Seq.trySkip 1 row with

                    | Comment k, Some v -> 
                        loop (SparseMatrix.AddComment k v matrix) remarks (lineNumber + 1)

                    | Remark k, _  -> 
                        loop matrix (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                    | Some k, Some v when List.contains k StudyInfo.Labels -> 
                        loop (SparseMatrix.AddRow k v matrix) remarks (lineNumber + 1)

                    | Some k, _ -> Some k,lineNumber,remarks,StudyInfo.FromSparseMatrix matrix
                    | _ -> None, lineNumber,remarks,StudyInfo.FromSparseMatrix matrix
                else
                    None,lineNumber,remarks,StudyInfo.FromSparseMatrix matrix
            loop (SparseMatrix.Create()) [] lineNumber

    
        static member WriteStudyInfo (study : Study) =  
            study
            |> StudyInfo.ToSparseMatrix
            |> SparseMatrix.ToRows
    
    let fromParts (studyInfo:StudyInfo) (designDescriptors:OntologyAnnotation list) publications factors assays protocols contacts =
        Study.create 
            null studyInfo.FileName studyInfo.Identifier studyInfo.Title studyInfo.Description studyInfo.SubmissionDate studyInfo.PublicReleaseDate
            publications contacts designDescriptors protocols StudyMaterials.Empty [] assays factors [] [] studyInfo.Comments

    let readStudy lineNumber (en:IEnumerator<Row>) = 

        let rec loop lastLine (studyInfo : StudyInfo) designDescriptors publications factors assays protocols contacts remarks lineNumber =
           
            match lastLine with

            | Some k when k = designDescriptorsLabel -> 
                let currentLine,lineNumber,newRemarks,designDescriptors = DesignDescriptors.readDesigns designDescriptorsLabelPrefix (lineNumber + 1) en         
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = publicationsLabel -> 
                let currentLine,lineNumber,newRemarks,publications = Publications.readPublications publicationsLabelPrefix (lineNumber + 1) en       
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = factorsLabel -> 
                let currentLine,lineNumber,newRemarks,factors = Factors.readFactors factorsLabelPrefix (lineNumber + 1) en       
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = assaysLabel -> 
                let currentLine,lineNumber,newRemarks,assays = Assays.readAssays assaysLabelPrefix (lineNumber + 1) en       
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = protocolsLabel -> 
                let currentLine,lineNumber,newRemarks,protocols = Protocols.readProtocols protocolsLabelPrefix (lineNumber + 1) en  
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = contactsLabel -> 
                let currentLine,lineNumber,newRemarks,contacts = Contacts.readPersons contactsLabelPrefix (lineNumber + 1) en  
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | k -> 
                k,lineNumber,remarks, fromParts studyInfo designDescriptors publications factors assays protocols contacts
    
        let currentLine,lineNumber,remarks,item = StudyInfo.ReadStudyInfo lineNumber en  
        loop currentLine item [] [] [] [] [] [] remarks lineNumber

    
    let writeStudy (study : Study) =
        seq {          
            yield! StudyInfo.WriteStudyInfo study

            yield  Row.ofValues None 0u [designDescriptorsLabel]
            yield! DesignDescriptors.writeDesigns designDescriptorsLabelPrefix study.StudyDesignDescriptors

            yield  Row.ofValues None 0u [publicationsLabel]
            yield! Publications.writePublications publicationsLabelPrefix study.Publications

            yield  Row.ofValues None 0u [factorsLabel]
            yield! Factors.writeFactors factorsLabelPrefix study.Factors

            yield  Row.ofValues None 0u [assaysLabel]
            yield! Assays.writeAssays assaysLabelPrefix study.Assays

            yield  Row.ofValues None 0u [protocolsLabel]
            yield! Protocols.writeProtocols protocolsLabelPrefix study.Protocols

            yield  Row.ofValues None 0u [contactsLabel]
            yield! Contacts.writePersons contactsLabelPrefix study.Contacts
        }