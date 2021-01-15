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




    
    let readStudyInfo lineNumber (en:IEnumerator<Row>) =
        let rec loop identifier title description submissionDate publicReleaseDate fileName comments remarks lineNumber = 

            if en.MoveNext() then        
                let row = en.Current
                match Row.tryGetValueAt None 1u row, Row.tryGetValueAt None 2u row with

                | Comment k, Some v -> 
                    loop identifier title description submissionDate publicReleaseDate fileName ((Comment.create "" k v) :: comments) remarks (lineNumber + 1)         
                | Comment k, None -> 
                    loop identifier title description submissionDate publicReleaseDate fileName ((Comment.create "" k "") :: comments) remarks (lineNumber + 1)         

                | Remark k, _  -> 
                    loop identifier title description submissionDate publicReleaseDate fileName comments (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some identifier when k = identifierLabel->              
                    loop identifier  title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some title when k = titleLabel ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                |Some k, Some description when k = descriptionLabel ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some submissionDate when k = submissionDateLabel ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some publicReleaseDate when k = publicReleaseDateLabel ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some fileName when k = fileNameLabel ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber, remarks, StudyInfo.create identifier title description submissionDate publicReleaseDate fileName (comments |> List.rev)
                | _ -> 
                    None,lineNumber, remarks, StudyInfo.create identifier title description submissionDate publicReleaseDate fileName (comments |> List.rev)
            else
                None,lineNumber, remarks, StudyInfo.create identifier title description submissionDate publicReleaseDate fileName (comments |> List.rev)
        loop "" "" "" "" "" "" [] [] lineNumber

    
    let writeStudyInfo (study : Study) =  

        seq {   
            yield   ( Row.ofValues None 0u [identifierLabel;         study.Identifier]          )
            yield   ( Row.ofValues None 0u [titleLabel;              study.Title]               )
            yield   ( Row.ofValues None 0u [descriptionLabel;        study.Description]         )
            yield   ( Row.ofValues None 0u [submissionDateLabel;     study.SubmissionDate]      )
            yield   ( Row.ofValues None 0u [publicReleaseDateLabel;  study.PublicReleaseDate]   )
            yield   ( Row.ofValues None 0u [fileNameLabel;           study.FileName]            )
            yield!  (study.Comments |> List.map (fun c ->  Row.ofValues None 0u [wrapCommentKey c.Name;c.Value]))          
        }
    
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