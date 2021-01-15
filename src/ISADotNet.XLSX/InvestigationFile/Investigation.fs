namespace ISADotNet.XSLX
open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module Investigation = 

    type InvestigationInfo =
        {
        Identifier : string
        Title : string
        Description : string
        SubmissionDate : string
        PublicReleaseDate : string
        Comments : Comment list
        }


        static member create identifier title description submissionDate publicReleaseDate comments =
            {
            Identifier = identifier
            Title = title
            Description = description
            SubmissionDate = submissionDate
            PublicReleaseDate = publicReleaseDate
            Comments = comments
            }

        static member IdentifierTab = "Investigation Identifier"
        static member TitleTab = "Investigation Title"
        static member DescriptionTab = "Investigation Description"
        static member SubmissionDateTab = "Investigation Submission Date"
        static member PublicReleaseDateTab = "Investigation Public Release Date"


    
    let readInvestigationInfo lineNumber (en:IEnumerator<Row>) =
        let rec loop identifier title description submissionDate publicReleaseDate comments remarks lineNumber = 

            if en.MoveNext() then    

                let row = en.Current

                match Row.tryGetValueAt None 1u row, Row.tryGetValueAt None 2u row with

                | Comment k, Some v -> 
                    loop identifier title description submissionDate publicReleaseDate ((Comment.create "" k v) :: comments) remarks (lineNumber + 1)         
                | Comment k, None -> 
                    loop identifier title description submissionDate publicReleaseDate ((Comment.create "" k "") :: comments) remarks (lineNumber + 1)         

                | Remark k, _  -> 
                    loop identifier title description submissionDate publicReleaseDate comments (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some identifier when k = InvestigationInfo.IdentifierTab->              
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                | Some k, Some title when k = InvestigationInfo.TitleTab ->
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                |Some k, Some description when k = InvestigationInfo.DescriptionTab ->
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                | Some k, Some submissionDate when k = InvestigationInfo.SubmissionDateTab ->
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                | Some k, Some publicReleaseDate when k = InvestigationInfo.PublicReleaseDateTab ->
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber, remarks, InvestigationInfo.create identifier title description submissionDate publicReleaseDate (comments |> List.rev)
                | _ -> 
                    None,lineNumber, remarks, InvestigationInfo.create identifier title description submissionDate publicReleaseDate (comments |> List.rev)
            else
                None,lineNumber, remarks, InvestigationInfo.create identifier title description submissionDate publicReleaseDate (comments |> List.rev)
        loop "" "" "" "" "" [] [] lineNumber

    
    let writeInvestigationInfo (investigation : Investigation) =  
        seq {       
            yield   ( Row.ofValues None 0u [Investigation.IdentifierTab;         investigation.Identifier])
            yield   ( Row.ofValues None 0u [Investigation.TitleTab;              investigation.Title])
            yield   ( Row.ofValues None 0u [Investigation.DescriptionTab;        investigation.Description])
            yield   ( Row.ofValues None 0u [Investigation.SubmissionDateTab;     investigation.SubmissionDate])
            yield   ( Row.ofValues None 0u [Investigation.PublicReleaseDateTab;  investigation.PublicReleaseDate])
            yield!  (investigation.Comments |> List.map (fun c ->  Row.ofValues None 0u [wrapCommentKey c.Name;c.Value]))         
        }
    
    let fromRows (rows:seq<Row>) =
        let en = rows.GetEnumerator()              
        
        let emptyInvestigationInfo = InvestigationInfo.create "" "" "" "" "" []

        let rec loop lastLine ontologySourceReferences investigationInfo publications contacts studies remarks lineNumber =
            match lastLine with

            | Some k when k = Investigation.OntologySourceReferenceTab -> 
                let currentLine,lineNumber,newRemarks,ontologySourceReferences = readTermSources (lineNumber + 1) en         
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.InvestigationTab -> 
                let currentLine,lineNumber,newRemarks,investigationInfo = readInvestigationInfo (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.PublicationsTab -> 
                let currentLine,lineNumber,newRemarks,publications = readPublications Investigation.PublicationsTabPrefix (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.ContactsTab -> 
                let currentLine,lineNumber,newRemarks,contacts = readPersons Investigation.ContactsTabPrefix (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.StudyTab -> 
                let currentLine,lineNumber,newRemarks,study = readStudy (lineNumber + 1) en  
                loop currentLine ontologySourceReferences investigationInfo publications contacts (study::studies) (List.append remarks newRemarks) lineNumber

            | k -> 
                Investigation.create 
                    "" "" investigationInfo.Identifier investigationInfo.Title investigationInfo.Description investigationInfo.SubmissionDate investigationInfo.PublicReleaseDate
                    ontologySourceReferences publications contacts (List.rev studies) investigationInfo.Comments remarks

        if en.MoveNext () then
            let currentLine = en.Current |> Row.tryGetValueAt None 1u
            loop currentLine [] emptyInvestigationInfo [] [] [] [] 1
            
        else
            failwith "emptyInvestigationFile"
   
    let fromFile (path : string) =
        
        let doc = 
            Spreadsheet.fromFile path false

        try 
            doc
            |> Spreadsheet.getRowsBySheetIndex 0u
            |> fromRows 
        finally
        doc.Close()

    
    let toRows (investigation:Investigation) : seq<Row> =
        let insertRemarks (remarks:Remark list) (rows:seq<Row>) = 
            let rm = remarks |> List.map Remark.toTuple |> Map.ofList 
            let rec loop i l nl =
                match Map.tryFind i rm with
                | Some remark ->
                     Row.ofValues None 1u [wrapRemark remark] :: nl
                    |> loop (i+1) l
                | None -> 
                    match l with
                    | [] -> nl
                    | h :: t -> 
                        loop (i+1) t (h::nl)
            loop 1 (rows |> List.ofSeq) []
            |> List.rev
        seq {
            yield  Row.ofValues None 0u [Investigation.OntologySourceReferenceTab]
            yield! writeTermSources (investigation.OntologySourceReferences |> List.map REF.toItem)

            yield  Row.ofValues None 0u [Investigation.InvestigationTab]
            yield! writeInvestigationInfo investigation

            yield  Row.ofValues None 0u [Investigation.PublicationsTab]
            yield! writePublications Investigation.PublicationsTabPrefix (investigation.Publications |> List.map REF.toItem)

            yield  Row.ofValues None 0u [Investigation.ContactsTab]
            yield! writePersons Investigation.ContactsTabPrefix (investigation.Contacts |> List.map REF.toItem)

            for study in investigation.Studies do
                yield  Row.ofValues None 0u [Investigation.StudyTab]
                yield! writeStudy (study |> REF.toItem)
        }
        |> insertRemarks investigation.Remarks
        |> Seq.mapi (fun i row -> Row.updateRowIndex (i+1 |> uint) row)


    let toFile (path:string) (investigation:Investigation) =

        let doc = Spreadsheet.initWithSST "isa_investigation" path
        let sheet = Spreadsheet.tryGetSheetBySheetIndex 0u doc |> Option.get

        investigation
        |> toRows
        |> Seq.fold (fun s r -> 
            SheetData.appendRow r s
        ) sheet
        |> ignore

        doc.Close()