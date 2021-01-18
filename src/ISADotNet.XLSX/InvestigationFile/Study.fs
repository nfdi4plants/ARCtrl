namespace ISADotNet.XSLX

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

    
    let studyInfoLabels = [identifierLabel;titleLabel;descriptionLabel;submissionDateLabel;publicReleaseDateLabel;fileNameLabel]
    
    let fromSparseMatrix (matrix : SparseMatrix) =
        
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


    let toSparseMatrix (studyInfo: StudyInfo) =
        let i = 0
        let matrix = SparseMatrix.Create (keys = studyInfoLabels)
        let mutable commentKeys = []

        do matrix.Matrix.Add ((identifierLabel,i),          studyInfo.Identifier)
        do matrix.Matrix.Add ((titleLabel,i),               studyInfo.Title)
        do matrix.Matrix.Add ((descriptionLabel,i),         studyInfo.Description)
        do matrix.Matrix.Add ((submissionDateLabel,i),      studyInfo.SubmissionDate)
        do matrix.Matrix.Add ((publicReleaseDateLabel,i),   studyInfo.PublicReleaseDate)
        do matrix.Matrix.Add ((fileNameLabel,i),            studyInfo.FileName)


        studyInfo.Comments
        |> List.iter (fun comment -> 
            commentKeys <- comment.Name :: commentKeys
            matrix.Matrix.Add((comment.Name,i),comment.Value)
            )      

        {matrix with CommentKeys = commentKeys |> List.distinct}


    
    let readStudyInfo lineNumber (en:IEnumerator<Row>) =
        let rec loop (matrix : SparseMatrix) remarks lineNumber = 

            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v)
                match Seq.tryItem 0 row |> Option.map snd, Seq.trySkip 1 row with

                | Comment k, Some v -> 
                    loop (SparseMatrix.AddComment k v matrix) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop matrix (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some v when List.contains k studyInfoLabels -> 
                    loop (SparseMatrix.AddRow k v matrix) remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,fromSparseMatrix matrix
                | _ -> None, lineNumber,remarks,fromSparseMatrix matrix
            else
                None,lineNumber,remarks,fromSparseMatrix matrix
        loop (SparseMatrix.Create()) [] lineNumber

    
    let writeStudyInfo (study : Study) =  
        study.
        |> toSparseMatrix
        |> SparseMatrix.ToRows prefix
    
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
                k,lineNumber,remarks, 
                    Study.create 
                        "" studyInfo.FileName studyInfo.Identifier studyInfo.Title studyInfo.Description studyInfo.SubmissionDate studyInfo.PublicReleaseDate 
                        publications contacts designDescriptors protocols ([],[],[]) [] assays factors [] [] studyInfo.Comments
    
        let currentLine,lineNumber,remarks,item = readStudyInfo lineNumber en  
        loop currentLine item [] [] [] [] [] [] remarks lineNumber

    
    let writeStudy (study : Study) =
        seq {          
            yield! writeStudyInfo study

            yield  Row.ofValues None 0u [designDescriptorsLabel]
            yield! DesignDescriptors.writeDesigns designDescriptorsLabelPrefix (study.StudyDesignDescriptors |> List.map REF.toItem)

            yield  Row.ofValues None 0u [publicationsLabel]
            yield! writePublications publicationsLabelPrefix (study.Publications |> List.map REF.toItem)

            yield  Row.ofValues None 0u [factorsLabel]
            yield! writeFactors factorsLabelPrefix (study.Factors |> List.map REF.toItem)

            yield  Row.ofValues None 0u [assaysLabel]
            yield! writeAssays assaysLabelPrefix (study.Assays |> List.map REF.toItem)

            yield  Row.ofValues None 0u [protocolsLabel]
            yield! writeProtocols protocolsLabelPrefix (study.Protocols |> List.map REF.toItem)

            yield  Row.ofValues None 0u [contactsLabel]
            yield! writePersons contactsLabelPrefix (study.Contacts |> List.map REF.toItem)
        }