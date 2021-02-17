namespace ISADotNet.XLSX
open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open ISADotNet.API
open Comment
open Remark
open System.Collections.Generic

module Investigation = 

    let identifierLabel = "Investigation Identifier"
    let titleLabel = "Investigation Title"
    let descriptionLabel = "Investigation Description"
    let submissionDateLabel = "Investigation Submission Date"
    let publicReleaseDateLabel = "Investigation Public Release Date"

    let investigationLabel = "INVESTIGATION"
    let ontologySourceReferenceLabel = "ONTOLOGY SOURCE REFERENCE"
    let publicationsLabel = "INVESTIGATION PUBLICATIONS"
    let contactsLabel = "INVESTIGATION CONTACTS"
    let studyLabel = "STUDY"

    let publicationsLabelPrefix = "Investigation Publication"
    let contactsLabelPrefix = "Investigation Person"

    
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
  
        static member Labels = [identifierLabel;titleLabel;descriptionLabel;submissionDateLabel;publicReleaseDateLabel]
    
        static member FromSparseMatrix (matrix : SparseMatrix) =
        
            let i = 0

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            InvestigationInfo.create
                (matrix.TryGetValueDefault("",(identifierLabel,i)))  
                (matrix.TryGetValueDefault("",(titleLabel,i)))  
                (matrix.TryGetValueDefault("",(descriptionLabel,i)))  
                (matrix.TryGetValueDefault("",(submissionDateLabel,i)))  
                (matrix.TryGetValueDefault("",(publicReleaseDateLabel,i)))  
                comments


        static member ToSparseMatrix (investigation: Investigation) =
            let i = 0
            let matrix = SparseMatrix.Create (keys = InvestigationInfo.Labels,length=1)
            let mutable commentKeys = []

            do matrix.Matrix.Add ((identifierLabel,i),          (Option.defaultValue "" investigation.Identifier))
            do matrix.Matrix.Add ((titleLabel,i),               (Option.defaultValue "" investigation.Title))
            do matrix.Matrix.Add ((descriptionLabel,i),         (Option.defaultValue "" investigation.Description))
            do matrix.Matrix.Add ((submissionDateLabel,i),      (Option.defaultValue "" investigation.SubmissionDate))
            do matrix.Matrix.Add ((publicReleaseDateLabel,i),   (Option.defaultValue "" investigation.PublicReleaseDate))

            match investigation.Comments with 
            | None -> ()
            | Some c ->
                c
                |> List.iter (fun comment -> 
                    let n,v = comment |> Comment.toString
                    commentKeys <- n :: commentKeys
                    matrix.Matrix.Add((n,i),v)
                )   

            {matrix with CommentKeys = commentKeys |> List.distinct |> List.rev}

      
        static member ReadInvestigationInfo lineNumber (en:IEnumerator<Row>) =
            let rec loop (matrix : SparseMatrix) remarks lineNumber = 

                if en.MoveNext() then  
                    let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v)
                    match Seq.tryItem 0 row |> Option.map snd, Seq.trySkip 1 row with

                    | Comment k, Some v -> 
                        loop (SparseMatrix.AddComment k v matrix) remarks (lineNumber + 1)

                    | Remark k, _  -> 
                        loop matrix (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                    | Some k, Some v when List.contains k InvestigationInfo.Labels -> 
                        loop (SparseMatrix.AddRow k v matrix) remarks (lineNumber + 1)

                    | Some k, _ -> Some k,lineNumber,remarks,InvestigationInfo.FromSparseMatrix matrix
                    | _ -> None, lineNumber,remarks,InvestigationInfo.FromSparseMatrix matrix
                else
                    None,lineNumber,remarks,InvestigationInfo.FromSparseMatrix matrix
            loop (SparseMatrix.Create()) [] lineNumber

    
        static member WriteInvestigationInfo (investigation : Investigation) =  
            investigation
            |> InvestigationInfo.ToSparseMatrix
            |> SparseMatrix.ToRows
 
    let fromParts (investigationInfo:InvestigationInfo) (ontologySourceReference:OntologySourceReference list) publications contacts studies remarks =
        Investigation.create 
            None 
            None 
            (Option.fromValueWithDefault "" investigationInfo.Identifier)
            (Option.fromValueWithDefault "" investigationInfo.Title)
            (Option.fromValueWithDefault "" investigationInfo.Description) 
            (Option.fromValueWithDefault "" investigationInfo.SubmissionDate) 
            (Option.fromValueWithDefault "" investigationInfo.PublicReleaseDate)
            (Option.fromValueWithDefault [] ontologySourceReference) 
            (Option.fromValueWithDefault [] publications)  
            (Option.fromValueWithDefault [] contacts)  
            (Option.fromValueWithDefault [] studies)  
            (Option.fromValueWithDefault [] investigationInfo.Comments)  
            remarks


    let fromRows (rows:seq<Row>) =
        let en = rows.GetEnumerator()              
        
        let emptyInvestigationInfo = InvestigationInfo.create "" "" "" "" "" []

        let rec loop lastLine ontologySourceReferences investigationInfo publications contacts studies remarks lineNumber =
            match lastLine with

            | Some k when k = ontologySourceReferenceLabel -> 
                let currentLine,lineNumber,newRemarks,ontologySourceReferences = OntologySourceReference.readTermSources (lineNumber + 1) en         
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = investigationLabel -> 
                let currentLine,lineNumber,newRemarks,investigationInfo = InvestigationInfo.ReadInvestigationInfo (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = publicationsLabel -> 
                let currentLine,lineNumber,newRemarks,publications = Publications.readPublications (Some publicationsLabelPrefix) (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = contactsLabel -> 
                let currentLine,lineNumber,newRemarks,contacts = Contacts.readPersons (Some contactsLabelPrefix) (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = studyLabel -> 
                let currentLine,lineNumber,newRemarks,study = Study.readStudy (lineNumber + 1) en  
                loop currentLine ontologySourceReferences investigationInfo publications contacts (study::studies) (List.append remarks newRemarks) lineNumber

            | k -> 
                fromParts investigationInfo ontologySourceReferences publications contacts (List.rev studies) remarks

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
            yield  Row.ofValues None 0u [ontologySourceReferenceLabel]
            yield! OntologySourceReference.writeTermSources (Option.defaultValue [] investigation.OntologySourceReferences)

            yield  Row.ofValues None 0u [investigationLabel]
            yield! InvestigationInfo.WriteInvestigationInfo investigation

            yield  Row.ofValues None 0u [publicationsLabel]
            yield! Publications.writePublications (Some publicationsLabelPrefix) (Option.defaultValue [] investigation.Publications)

            yield  Row.ofValues None 0u [contactsLabel]
            yield! Contacts.writePersons (Some contactsLabelPrefix) (Option.defaultValue [] investigation.Contacts)

            for study in (Option.defaultValue [] investigation.Studies) do
                yield  Row.ofValues None 0u [studyLabel]
                yield! Study.writeStudy study
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