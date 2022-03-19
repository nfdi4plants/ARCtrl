namespace ISADotNet.XLSX
open DocumentFormat.OpenXml.Spreadsheet
open FsSpreadsheet.ExcelIO
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
    
        static member FromSparseTable (matrix : SparseTable) =
        
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


        static member ToSparseTable (investigation: Investigation) =
            let i = 1
            let matrix = SparseTable.Create (keys = InvestigationInfo.Labels,length=2)
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

      
        static member fromRows lineNumber (rows : IEnumerator<SparseRow>) =
            SparseTable.FromRows(rows,InvestigationInfo.Labels,lineNumber)
            |> fun (s,ln,rs,sm) -> (s,ln,rs, InvestigationInfo.FromSparseTable sm)    
    
        static member toRows (investigation : Investigation) =  
            investigation
            |> InvestigationInfo.ToSparseTable
            |> SparseTable.ToRows
 
    let fromParts (investigationInfo:InvestigationInfo) (ontologySourceReference:OntologySourceReference list) publications contacts studies remarks =
        Investigation.make 
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


    let fromRows (rows:seq<SparseRow>) =
        let en = rows.GetEnumerator()              
        
        let emptyInvestigationInfo = InvestigationInfo.create "" "" "" "" "" []

        let rec loop lastLine ontologySourceReferences investigationInfo publications contacts studies remarks lineNumber =
            match lastLine with

            | Some k when k = ontologySourceReferenceLabel -> 
                let currentLine,lineNumber,newRemarks,ontologySourceReferences = OntologySourceReference.fromRows (lineNumber + 1) en         
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = investigationLabel -> 
                let currentLine,lineNumber,newRemarks,investigationInfo = InvestigationInfo.fromRows (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = publicationsLabel -> 
                let currentLine,lineNumber,newRemarks,publications = Publications.fromRows (Some publicationsLabelPrefix) (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = contactsLabel -> 
                let currentLine,lineNumber,newRemarks,contacts = Contacts.fromRows (Some contactsLabelPrefix) (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = studyLabel -> 
                let currentLine,lineNumber,newRemarks,study = Study.fromRows (lineNumber + 1) en  
                loop currentLine ontologySourceReferences investigationInfo publications contacts (study::studies) (List.append remarks newRemarks) lineNumber

            | k -> 
                fromParts investigationInfo ontologySourceReferences publications contacts (List.rev studies) remarks

        if en.MoveNext () then
            let currentLine = en.Current |> SparseRow.tryGetValueAt 0
            loop currentLine [] emptyInvestigationInfo [] [] [] [] 1
            
        else
            failwith "emptyInvestigationFile"
 
   
    let toRows (investigation:Investigation) : seq<SparseRow> =
        let insertRemarks (remarks:Remark list) (rows:seq<SparseRow>) = 
            let rm = remarks |> List.map Remark.toTuple |> Map.ofList 
            let rec loop i l nl =
                match Map.tryFind i rm with
                | Some remark ->
                     SparseRow.fromValues [wrapRemark remark] :: nl
                    |> loop (i+1) l
                | None -> 
                    match l with
                    | [] -> nl
                    | h :: t -> 
                        loop (i+1) t (h::nl)
            loop 1 (rows |> List.ofSeq) []
            |> List.rev
        seq {
            yield  SparseRow.fromValues[ontologySourceReferenceLabel]
            yield! OntologySourceReference.toRows (Option.defaultValue [] investigation.OntologySourceReferences)

            yield  SparseRow.fromValues[investigationLabel]
            yield! InvestigationInfo.toRows investigation

            yield  SparseRow.fromValues[publicationsLabel]
            yield! Publications.toRows (Some publicationsLabelPrefix) (Option.defaultValue [] investigation.Publications)

            yield  SparseRow.fromValues[contactsLabel]
            yield! Contacts.toRows (Some contactsLabelPrefix) (Option.defaultValue [] investigation.Contacts)

            for study in (Option.defaultValue [] investigation.Studies) do
                yield  SparseRow.fromValues[studyLabel]
                yield! Study.toRows study
        }
        |> insertRemarks investigation.Remarks        
        |> seq

    /// Diesen Block durch JS ersetzen ----> 

    /// Creates a new row from the given values.
    let ofSparseValues rowIndex (vals : 'T option seq) =
        let spans = Row.Spans.fromBoundaries 1u (Seq.length vals |> uint)
        vals
        |> Seq.mapi (fun i value -> 
            value
            |> Option.map (Cell.fromValue None (i + 1 |> uint) rowIndex)
        )
        |> Seq.choose id
        |> Row.create rowIndex spans 

    let fromSpreadsheet (doc:DocumentFormat.OpenXml.Packaging.SpreadsheetDocument) =           
        doc
        |> Spreadsheet.getRowsBySheetIndex 0u
        |> Seq.map (Row.getIndexedValues None >> Seq.map (fun (i,v) -> (int i) - 1, v))
        |> fromRows 
        

    let fromFile (path : string) =
        let doc = Spreadsheet.fromFile path false
        try
            fromSpreadsheet doc
        finally
            doc.Close()
        
    let fromStream (stream : System.IO.Stream) =
        let doc = Spreadsheet.fromStream stream false
        try
            fromSpreadsheet doc
        finally
            doc.Close()


    let toSpreadsheet (doc:DocumentFormat.OpenXml.Packaging.SpreadsheetDocument) (investigation:Investigation) =           
        
        let sheet = Spreadsheet.tryGetSheetBySheetIndex 0u doc |> Option.get

        investigation
        |> toRows
        |> Seq.mapi (fun i row -> 
            row
            |> SparseRow.getAllValues
            |> ofSparseValues (i+1 |> uint)
            )
        |> Seq.fold (fun s r -> 
            SheetData.appendRow r s
        ) sheet
        |> ignore
        
    let toFile (path : string) (investigation:Investigation) =
        let doc = Spreadsheet.initWithSst "isa_investigation" path
        try 
            toSpreadsheet doc investigation
        finally
            doc.Close()
        
    let toStream (stream : System.IO.Stream) (investigation:Investigation) =
        let doc = Spreadsheet.fromStream stream false
        try
            toSpreadsheet doc investigation
        finally
            doc.Close()

    /// ---->  Bis hier